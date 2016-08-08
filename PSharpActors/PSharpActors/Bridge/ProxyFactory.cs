//-----------------------------------------------------------------------
// <copyright file="ProxyFactory.cs">
//      Copyright (c) Microsoft Corporation. All rights reserved.
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//      IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
//      CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//      TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
//      SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Microsoft.PSharp.LanguageServices.Compilation;

namespace Microsoft.PSharp.Actors.Bridge
{
    /// <summary>
    /// Factory of proxies.
    /// </summary>
    public class ProxyFactory<TActor>
    {
        /// <summary>
        /// The proxy types.
        /// </summary>
        private readonly Dictionary<Type, Type> ProxyTypes;

        /// <summary>
        /// Set of ignored interface types.
        /// </summary>
        private readonly ISet<Type> IgnoredInterfaceTypes;

        /// <summary>
        /// The factory lock.
        /// </summary>
        private readonly object Lock;

        private readonly ISet<string> RequiredNamespaces;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ProxyFactory(ISet<string> requiredNamespaces)
        {
            this.ProxyTypes = new Dictionary<Type, Type>();
            this.IgnoredInterfaceTypes = new HashSet<Type>();
            this.Lock = new object();
            this.RequiredNamespaces = requiredNamespaces;
        }

        /// <summary>
        /// Registers the ignored interface types.
        /// </summary>
        /// <param name="interfaceTypes">Types</param>
        public void RegisterIgnoredInterfaceTypes(ISet<Type> interfaceTypes)
        {
            this.IgnoredInterfaceTypes.UnionWith(interfaceTypes);
        }

        /// <summary>
        /// Get the proxy type.
        /// </summary>
        /// <param name="interfaceType">Type</param>
        /// <param name="actorMachineType">Actor machine type</param>
        /// <param name="assemblyPath">Assembly path</param>
        /// <returns>Type</returns>
        public Type GetProxyType(Type interfaceType, Type actorMachineType, string assemblyPath)
        {
            lock (this.Lock)
            {
                Type res;
                this.ProxyTypes.TryGetValue(interfaceType, out res);

                if (res == null)
                {
                    res = CreateProxyType(interfaceType, actorMachineType, assemblyPath);
                    this.ProxyTypes.Add(interfaceType, res);
                }
                return res;
            }
        }

        /// <summary>
        /// Create a new proxy type.
        /// </summary>
        /// <param name="interfaceType">Type</param>
        /// <param name="actorMachineType">Actor machine type</param>
        /// <param name="assemblyPath">Assembly path</param>
        /// <returns>Type</returns>
        private Type CreateProxyType(Type interfaceType, Type actorMachineType, string assemblyPath)
        {
            if (!interfaceType.IsInterface)
            {
                throw new InvalidOperationException();
            }
            
            var actorType = this.GetActorType(interfaceType, assemblyPath);
            var references = new HashSet<MetadataReference>();
            references.Add(MetadataReference.CreateFromFile(actorType.Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(ActorMachine).Assembly.Location));
            foreach (var referencedAssembly in actorType.Assembly.GetReferencedAssemblies())
            {
                references.Add(MetadataReference.CreateFromFile(Assembly.Load(referencedAssembly).Location));
            }

            SyntaxTree syntaxTree = this.CreateProxySyntaxTree(interfaceType, actorType, actorMachineType);
            //Console.WriteLine(syntaxTree);

            var context = CompilationContext.Create().LoadSolution(syntaxTree.ToString(), references, "cs");
            var compilation = context.GetSolution().Projects.First().GetCompilationAsync().Result;
            syntaxTree = context.GetSolution().Projects.First().Documents.First().GetSyntaxTreeAsync().Result;

            SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

            var typeSymbol = semanticModel.GetDeclaredSymbol(syntaxTree.GetRoot().DescendantNodes()
                .OfType<ClassDeclarationSyntax>().First());
            
            compilation = compilation.WithAssemblyName(interfaceType.Name + "_PSharpProxy");
            compilation = compilation.WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            string proxyAssemblyPath = CompilationEngine.Create(context).ToFile(compilation,
                OutputKind.DynamicallyLinkedLibrary, Assembly.GetExecutingAssembly().Location,
                false, false);

            Assembly proxyAssembly = Assembly.LoadFrom(proxyAssemblyPath);

            Type proxyType = proxyAssembly.GetType(typeSymbol.ContainingNamespace.ToString() +
                    "." + typeSymbol.MetadataName);
            
            return proxyType;
        }

        /// <summary>
        /// Gets the actor type from the specified
        /// interface and assembly path.
        /// </summary>
        /// <param name="interfaceType">Type</param>
        /// <param name="assemblyPath">Assembly path</param>
        /// <returns>Type</returns>
        private Type GetActorType(Type interfaceType, string assemblyPath)
        {
            List<Assembly> allAssemblies = new List<Assembly>();
            allAssemblies.AddRange(this.GetAllAssemblies(assemblyPath, "exe"));
            allAssemblies.AddRange(this.GetAllAssemblies(assemblyPath, "dll"));

            var types = new List<Type>();
            foreach (var asm in allAssemblies)
            {
                try
                {
                    types.AddRange(asm.GetTypes());
                }
                catch
                {
                    continue;
                }
            }

            IEnumerable<Type> actorTypes = null;

            if (interfaceType.IsGenericType)
            {
                var potentialTypes = types.Where(p => !p.IsInterface);
                foreach(var pType in potentialTypes)
                {

                    foreach (var potentialInterfaceType in pType.GetInterfaces())
                    {
                        if(potentialInterfaceType.IsGenericType && potentialInterfaceType.GetGenericTypeDefinition() == interfaceType.GetGenericTypeDefinition())
                        {
                            return pType;
                        }
                    }
                }
            }
            else
            {
                actorTypes = types.Where(p => interfaceType.IsAssignableFrom(p) && !p.IsInterface);
            }
            ActorModel.Assert(actorTypes.Any(), "No implementation found for actor " +
                $"type '{interfaceType}'.");

            return actorTypes.First();
        }

        /// <summary>
        /// Creates the proxy syntax tree
        /// </summary>
        /// <param name="interfaceType">Actor interface type</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorMachineType">Actor machine type</param>
        /// <returns>SyntaxTree</returns>
        private SyntaxTree CreateProxySyntaxTree(Type interfaceType, Type actorType, Type actorMachineType)
        {
            ClassDeclarationSyntax proxyDecl = this.CreateProxyClassDeclaration(
                interfaceType, actorType, actorMachineType);

            NamespaceDeclarationSyntax namespaceDecl = SyntaxFactory.NamespaceDeclaration(
                SyntaxFactory.IdentifierName(actorType.Namespace + "_PSharpProxy"));
            
            var usingDirectives = new List<UsingDirectiveSyntax>
            {
                SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("Microsoft.PSharp.Actors")),
                SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("Microsoft.PSharp.Actors.Utilities"))
            };

            foreach (var requiredNamespace in this.RequiredNamespaces)
            {
                usingDirectives.Add(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(requiredNamespace)));
            }

            namespaceDecl = namespaceDecl.WithUsings(SyntaxFactory.List(usingDirectives))
                .WithMembers(SyntaxFactory.List(
                new List<MemberDeclarationSyntax>
                {
                    proxyDecl
                }));

            namespaceDecl = namespaceDecl.NormalizeWhitespace();

            return namespaceDecl.SyntaxTree;
        }

        /// <summary>
        /// Creates a proxy class declaration.
        /// </summary>
        /// <param name="interfaceType">Actor interface type</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorMachineType">Actor machine type</param>
        /// <returns>ClassDeclarationSyntax</returns>
        private ClassDeclarationSyntax CreateProxyClassDeclaration(Type interfaceType,
            Type actorType, Type actorMachineType)
        {
            string interfaceTypeName;
            if (interfaceType.IsGenericType)
                interfaceTypeName = interfaceType.Name.Substring(0, interfaceType.Name.IndexOf("`"));
            else
                interfaceTypeName = interfaceType.Name;
            ClassDeclarationSyntax classDecl = SyntaxFactory.ClassDeclaration(interfaceTypeName + "_PSharpProxy")
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));

            List<TypeParameterSyntax> paramList = new List<TypeParameterSyntax>();
            foreach(Type genericType in actorType.GetGenericArguments())
            {
                paramList.Add(SyntaxFactory.TypeParameter(SyntaxFactory.Identifier(genericType.ToString())));
            }

            if(paramList.Count > 0)
                classDecl = classDecl.WithTypeParameterList(SyntaxFactory.TypeParameterList(SyntaxFactory.SeparatedList(paramList)));

            FieldDeclarationSyntax target = this.CreateProxyField(
                interfaceType, "Target", SyntaxKind.PrivateKeyword);
            FieldDeclarationSyntax machineId = this.CreateProxyField(
                typeof(MachineId), "MachineId", SyntaxKind.PrivateKeyword);

            BlockSyntax idGetAccessorBody = SyntaxFactory.Block(
                SyntaxFactory.ReturnStatement(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.ThisExpression(),
                        SyntaxFactory.IdentifierName("MachineId"))));
            PropertyDeclarationSyntax id = this.CreateProxyProperty(typeof(MachineId), "Id",
                idGetAccessorBody, SyntaxKind.PublicKeyword, SyntaxKind.PrivateKeyword, false);

            ConstructorDeclarationSyntax constructor = this.CreateProxyConstructor(
                interfaceType, actorType, actorMachineType);

            var baseTypes = new HashSet<BaseTypeSyntax>
            {
                SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(typeof(IPSharpActor).FullName))
            };

            var methodDecls = new List<MethodDeclarationSyntax>();

            foreach (var type in actorType.GetInterfaces().Where(
                i => !this.IgnoredInterfaceTypes.Contains(i)))
            {
                if (type.IsGenericType)
                {
                    //baseTypes.Add(SyntaxFactory.SimpleBaseType(SyntaxFactory.GenericName(SyntaxFactory.Identifier(type.GetGenericTypeDefinition().FullName),
                    //    SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(genericTypes)))));
                    baseTypes.Add(SyntaxFactory.SimpleBaseType(GetGenericName(type)));
                }
                else
                    baseTypes.Add(SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(type.FullName)));
                foreach (var method in type.GetMethods())
                {
                    methodDecls.Add(this.CreateProxyMethod(method, actorType, actorMachineType));
                }
            }

            classDecl = classDecl.WithBaseList(SyntaxFactory.BaseList(
                SyntaxFactory.SeparatedList(baseTypes)));

            classDecl = classDecl.WithMembers(SyntaxFactory.List(
                new List<MemberDeclarationSyntax>
                {
                    target, machineId,
                    id,
                    constructor
                }).AddRange(methodDecls));

            return classDecl;
        }
        
        class temp<Tin, Tout>
        {

        }
        /// <summary>
        /// Creates the proxy constructor.
        /// </summary>
        /// <param name="interfaceType">Actor interface type</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorMachineType">Actor machine type</param>
        /// <returns>ConstructorDeclarationSyntax</returns>
        private ConstructorDeclarationSyntax CreateProxyConstructor(Type interfaceType,
            Type actorType, Type actorMachineType)
        {
            string interfaceTypeName;
            if (interfaceType.IsGenericType)
                interfaceTypeName = interfaceType.Name.Substring(0, interfaceType.Name.IndexOf("`"));
            else
                interfaceTypeName = interfaceType.Name;
            ConstructorDeclarationSyntax constructor = SyntaxFactory.ConstructorDeclaration(
                interfaceTypeName + "_PSharpProxy")
                .WithParameterList(SyntaxFactory.ParameterList(
                    SyntaxFactory.SeparatedList(
                        new List<ParameterSyntax>
                        {
                            SyntaxFactory.Parameter(
                                SyntaxFactory.List<AttributeListSyntax>(),
                                SyntaxFactory.TokenList(),
                                this.GetTypeSyntax(typeof(object)),
                                SyntaxFactory.Identifier("primaryKey"),
                                null)
                        })))
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));

            string actorTypeName;
            if (actorType.IsGenericType)
            {
                actorTypeName = GetGenericTypeString(actorType, interfaceType.GetGenericArguments());
            }
            else
            {
                actorTypeName = actorType.FullName;
            }

            ExpressionStatementSyntax targetConstruction = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.ThisExpression(), SyntaxFactory.IdentifierName("Target")),
                SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName(actorTypeName))
                .WithArgumentList(SyntaxFactory.ArgumentList())));
            
            LocalDeclarationStatementSyntax machineTypeDecl = SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.IdentifierName(typeof(Type).FullName),
                    SyntaxFactory.SeparatedList(
                        new List<VariableDeclaratorSyntax>
                        {
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("machineType"),
                            null,
                            SyntaxFactory.EqualsValueClause(SyntaxFactory.TypeOfExpression(
                                SyntaxFactory.IdentifierName(actorMachineType.FullName))))
                        })));

            string eventType = actorMachineType.FullName + "." + typeof(ActorMachine.InitEvent).Name;
            string eventName = "initEvent";

            ArgumentListSyntax arguments = SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList(
                    new List<ArgumentSyntax>
                    {
                        SyntaxFactory.Argument(SyntaxFactory.ThisExpression()),
                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("primaryKey")),
                        SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.ThisExpression(),
                            SyntaxFactory.IdentifierName("Target"))),
                        SyntaxFactory.Argument(SyntaxFactory.TypeOfExpression(
                            SyntaxFactory.IdentifierName(actorTypeName)))
                    }));

            LocalDeclarationStatementSyntax eventDecl = this.CreateEventDeclaration(
                eventType, eventName, arguments);

            ExpressionStatementSyntax createMachine = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.ThisExpression(), SyntaxFactory.IdentifierName("MachineId")),
                SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                     SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                      SyntaxFactory.IdentifierName("ActorModel"),
                     SyntaxFactory.IdentifierName("Runtime")),
                    SyntaxFactory.IdentifierName("CreateMachine")),
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SeparatedList(
                            new List<ArgumentSyntax>
                            {
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("machineType")),
                                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    SyntaxFactory.Literal(actorTypeName))),
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(eventName))
                            })))));

            BlockSyntax body = SyntaxFactory.Block(targetConstruction,
                machineTypeDecl, eventDecl, createMachine);
            constructor = constructor.WithBody(body);

            return constructor;
        }

        /// <summary>
        /// Creates a proxy field declaration.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="identifier">Identifier</param>
        /// <param name="accessModifierKeyword">SyntaxKind</param>
        /// <returns>FieldDeclarationSyntax</returns>
        private FieldDeclarationSyntax CreateProxyField(Type type, string identifier,
            SyntaxKind accessModifierKeyword)
        {
            FieldDeclarationSyntax fieldDecl = SyntaxFactory.FieldDeclaration(
                SyntaxFactory.List<AttributeListSyntax>(),
                SyntaxFactory.TokenList(
                    SyntaxFactory.Token(accessModifierKeyword),
                    SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)),
                SyntaxFactory.VariableDeclaration(
                    this.GetTypeSyntax(type),
                    SyntaxFactory.SeparatedList(
                        new List<VariableDeclaratorSyntax>
                        {
                            SyntaxFactory.VariableDeclarator(identifier)
                        })));

            return fieldDecl;
        }

        /// <summary>
        /// Creates a proxy property declaration.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="identifier">Identifier</param>
        /// <param name="getBody">BlockSyntax</param>
        /// <param name="getModifierKeyword">SyntaxKind</param>
        /// <param name="setModifierKeyword">SyntaxKind</param>
        /// <param name="hasSetAccessor">Boolean</param>
        /// <returns>PropertyDeclarationSyntax</returns>
        private PropertyDeclarationSyntax CreateProxyProperty(Type type, string identifier,
            BlockSyntax getBody, SyntaxKind getModifierKeyword, SyntaxKind setModifierKeyword,
            bool hasSetAccessor)
        {
            AccessorDeclarationSyntax getAccessor = SyntaxFactory.AccessorDeclaration(
                SyntaxKind.GetAccessorDeclaration);

            if (getBody != null)
            {
                getAccessor = getAccessor.WithBody(getBody);
            }
            else
            {
                getAccessor = getAccessor.WithSemicolonToken(
                    SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            }

            if (getModifierKeyword == SyntaxKind.PrivateKeyword ||
                getModifierKeyword == SyntaxKind.InternalKeyword)
            {   
                getAccessor = getAccessor.WithModifiers(SyntaxFactory.TokenList(
                    SyntaxFactory.Token(getModifierKeyword)));
            }

            AccessorDeclarationSyntax setAccessor = null;
            if (hasSetAccessor)
            {
                setAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).
                WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                if (setModifierKeyword == SyntaxKind.PrivateKeyword ||
                    setModifierKeyword == SyntaxKind.InternalKeyword)
                {
                    setAccessor = setAccessor.WithModifiers(SyntaxFactory.TokenList(
                        SyntaxFactory.Token(setModifierKeyword)));
                }
            }

            var accessors = new List<AccessorDeclarationSyntax>();
            accessors.Add(getAccessor);

            if (hasSetAccessor)
            {
                accessors.Add(setAccessor);
            }

            PropertyDeclarationSyntax propertyDecl = SyntaxFactory.PropertyDeclaration(
                SyntaxFactory.List<AttributeListSyntax>(),
                SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                this.GetTypeSyntax(type),
                null,
                SyntaxFactory.Identifier(identifier),
                SyntaxFactory.AccessorList(
                    SyntaxFactory.List(accessors)));
            return propertyDecl;
        }

        /// <summary>
        /// Creates a proxy method declaration.
        /// </summary>
        /// <param name="method">MethodInfo</param>
        /// <param name="interfaceType">Actor interface type</param>
        /// <param name="actorMachineType">Actor machine type</param>
        /// <returns>MethodDeclarationSyntax</returns>
        private MethodDeclarationSyntax CreateProxyMethod(MethodInfo method,
            Type actorType, Type actorMachineType)
        {
            List<ParameterSyntax> parameters = new List<ParameterSyntax>();
            List<ArgumentSyntax> payloadArguments = new List<ArgumentSyntax>();
            foreach (var parameter in method.GetParameters())
            {
                parameters.Add(SyntaxFactory.Parameter(
                    SyntaxFactory.List<AttributeListSyntax>(),
                    SyntaxFactory.TokenList(),
                    this.GetTypeSyntax(parameter.ParameterType),
                    SyntaxFactory.Identifier(parameter.Name),
                    null));
                payloadArguments.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parameter.Name)));
            }

            TypeSyntax returnTaskType = this.GetTypeSyntax(method.ReturnType);
            MethodDeclarationSyntax methodDecl = SyntaxFactory.MethodDeclaration(returnTaskType, method.Name)
                .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters)));

            LocalDeclarationStatementSyntax payloadDecl = SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.IdentifierName("object[]"),
                    SyntaxFactory.SeparatedList(
                        new List<VariableDeclaratorSyntax>
                        {
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("payload"),
                            null,
                            SyntaxFactory.EqualsValueClause(SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName("Serialization"),
                                    SyntaxFactory.IdentifierName("Serialize")),
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SeparatedList(payloadArguments)))))
                        })));

            LocalDeclarationStatementSyntax taskCompletionSource = null;
            if (method.ReturnType.GetGenericArguments().Count() > 0)
            {
                Type genericType = method.ReturnType.GetGenericArguments().First();
                taskCompletionSource = this.CreateActorCompletionTask(genericType);
            }
            else
            {
                taskCompletionSource = this.CreateActorCompletionTask(typeof(object));
            }
            
            GenericNameSyntax actionType = SyntaxFactory.GenericName(SyntaxFactory.Identifier(
                typeof(Action).Name),
                SyntaxFactory.TypeArgumentList(
                    SyntaxFactory.SeparatedList(
                        new List<TypeSyntax>
                        {
                            this.GetTypeSyntax(typeof(object))
                        })));

            string eventType = actorMachineType.FullName + "." + typeof(ActorMachine.ActorEvent).Name;
            string eventName = "actorEvent";

            string interfaceTypeString;
            if (!actorType.IsGenericType)
                interfaceTypeString = actorType.FullName; 
            else
            {
                interfaceTypeString = GetGenericTypeString(actorType);
            }
            ArgumentListSyntax arguments = SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList(
                    new List<ArgumentSyntax>
                    {
                        SyntaxFactory.Argument(SyntaxFactory.TypeOfExpression(
                            SyntaxFactory.IdentifierName(interfaceTypeString))),
                        SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            SyntaxFactory.Literal(method.Name))),
                        SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.ThisExpression(),
                            SyntaxFactory.IdentifierName("Target"))),
                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("payload")),
                        SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("actorCompletionTask"),
                            SyntaxFactory.IdentifierName("ActorCompletionMachine")))
                    }));

            LocalDeclarationStatementSyntax eventDecl = this.CreateEventDeclaration(
                eventType, eventName, arguments);

            ExpressionStatementSyntax sendExpr = this.CreateSendEventExpression("MachineId", eventName);
            ReturnStatementSyntax returnStmt = SyntaxFactory.ReturnStatement(
                SyntaxFactory.IdentifierName("actorCompletionTask"));

            BlockSyntax body = SyntaxFactory.Block(payloadDecl, taskCompletionSource,
                eventDecl, sendExpr, returnStmt);
            methodDecl = methodDecl.WithBody(body).WithModifiers(
                SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));

            return methodDecl;
        }

        /// <summary>
        /// Creates an event declaration.
        /// </summary>
        /// <param name="eventType">Type of the event</param>
        /// <param name="eventName">Name of the event</param>
        /// <param name="arguments">Arguments</param>
        /// <returns>LocalDeclarationStatementSyntax</returns>
        private LocalDeclarationStatementSyntax CreateEventDeclaration(
            string eventType, string eventName, ArgumentListSyntax arguments)
        {
            LocalDeclarationStatementSyntax eventDecl = SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.IdentifierName(eventType),
                    SyntaxFactory.SeparatedList(
                        new List<VariableDeclaratorSyntax>
                        {
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(eventName),
                            null,
                            SyntaxFactory.EqualsValueClause(SyntaxFactory.ObjectCreationExpression(
                                SyntaxFactory.IdentifierName(eventType))
                                .WithArgumentList(arguments)))
                        })));
            return eventDecl;
        }

        /// <summary>
        /// Creates a send event expression.
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="eventName">Event name</param>
        /// <returns>ExpressionStatementSyntax</returns>
        private ExpressionStatementSyntax CreateSendEventExpression(
            string target, string eventName)
        {
            ExpressionStatementSyntax sendExpr = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                     SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                     SyntaxFactory.IdentifierName("ActorModel"),
                     SyntaxFactory.IdentifierName("Runtime")),
                    SyntaxFactory.IdentifierName("SendEvent")),
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SeparatedList(
                            new List<ArgumentSyntax>
                            {
                                SyntaxFactory.Argument(
                                    SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.ThisExpression(), SyntaxFactory.IdentifierName(target))),
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(eventName))
                            }))));
            return sendExpr;
        }

        /// <summary>
        /// Creates an actor completion task.
        /// </summary>
        /// <param name="genericType">Type</param>
        /// <returns>LocalDeclarationStatementSyntax</returns>
        private LocalDeclarationStatementSyntax CreateActorCompletionTask(Type genericType)
        {
            GenericNameSyntax tcsType = SyntaxFactory.GenericName(SyntaxFactory.Identifier(
                        "Microsoft.PSharp.Actors.Bridge.ActorCompletionTask"),
                        SyntaxFactory.TypeArgumentList(
                            SyntaxFactory.SeparatedList(
                                new List<TypeSyntax>
                                {
                                        this.GetTypeSyntax(genericType)
                                })));

            LocalDeclarationStatementSyntax tcs = SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                    tcsType,
                    SyntaxFactory.SeparatedList(
                        new List<VariableDeclaratorSyntax>
                        {
                                SyntaxFactory.VariableDeclarator(
                                    SyntaxFactory.Identifier("actorCompletionTask"),
                                    null,
                                    SyntaxFactory.EqualsValueClause(
                                        SyntaxFactory.ObjectCreationExpression(
                                            tcsType,
                                            SyntaxFactory.ArgumentList(),
                                            null)))
                        })));

            return tcs;
        }

        /// <summary>
        /// Creates a set result lamda.
        /// </summary>
        /// <param name="argument">ArgumentSyntax</param>
        /// <returns>SimpleLambdaExpressionSyntax</returns>
        private SimpleLambdaExpressionSyntax CreateSetResultLambda(ArgumentSyntax argument)
        {
            SimpleLambdaExpressionSyntax lambda = SyntaxFactory.SimpleLambdaExpression(
                SyntaxFactory.Parameter(
                    SyntaxFactory.List<AttributeListSyntax>(),
                    SyntaxFactory.TokenList(),
                    null,
                    SyntaxFactory.Identifier("task"),
                    null),
                SyntaxFactory.Block(
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("tcs"),
                                SyntaxFactory.IdentifierName("SetResult")),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList(
                                    new List<ArgumentSyntax>
                                    {
                                        argument
                                    }))))));

            return lambda;
        }

        #region helper methods

        /// <summary>
        /// Returns all assemblies from the specified
        /// assembly path and extension.
        /// </summary>
        /// <param name="assemblyPath">Path</param>
        /// <param name="extension">Extension</param>
        /// <returns>Assemblies</returns>
        private List<Assembly> GetAllAssemblies(string assemblyPath, string extension)
        {
            List<Assembly> allAssemblies = new List<Assembly>();
            foreach (string dll in Directory.GetFiles(assemblyPath, "*." + extension, SearchOption.AllDirectories))
            {
                try
                {
                    if (dll.Contains("\\obj\\"))
                        continue;
                    Assembly asm = Assembly.LoadFrom(dll);
                    allAssemblies.Add(asm);
                }
                catch
                {
                    continue;
                }
            }

            return allAssemblies;
        }

        /// <summary>
        /// Returns the type syntax from the specified type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>TypeSyntax</returns>
        private TypeSyntax GetTypeSyntax(Type type)
        {
            TypeSyntax syntax = null;

            if (type.IsGenericType)
            {
                List<TypeSyntax> genericTypes = new List<TypeSyntax>();
                foreach (var genericType in type.GetGenericArguments())
                {
                    if (genericType.IsGenericParameter)
                        genericTypes.Add(SyntaxFactory.IdentifierName(genericType.ToString()));
                    else if (genericType.IsGenericType)
                        genericTypes.Add(GetTypeSyntax(genericType));
                    else
                        genericTypes.Add(SyntaxFactory.IdentifierName(genericType.FullName));
                }

                string genericTypeName = type.GetGenericTypeDefinition().FullName;
                genericTypeName = genericTypeName.Substring(0, genericTypeName.LastIndexOf('`'));

                syntax = SyntaxFactory.GenericName(
                    SyntaxFactory.Identifier(genericTypeName),
                    SyntaxFactory.TypeArgumentList(
                        SyntaxFactory.SeparatedList(genericTypes)));
            }
            else if(type.IsGenericParameter)
                syntax = SyntaxFactory.IdentifierName(type.ToString());
            else
            {
                syntax = SyntaxFactory.IdentifierName(type.FullName);
            }
            
            return syntax;
        }


        /// <summary>
        /// Returns the GenericName for generic types
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>GenericNameSyntax</returns>
        private GenericNameSyntax GetGenericName(Type type)
        {
            List<TypeSyntax> genericTypes = new List<TypeSyntax>();
            foreach(Type genericType in type.GetGenericArguments())
            {
                if (genericType.IsGenericParameter)
                    genericTypes.Add(SyntaxFactory.IdentifierName(genericType.ToString()));
                else if (genericType.IsGenericType)
                {
                    genericTypes.Add(GetGenericName(genericType));
                }
                else
                    genericTypes.Add(SyntaxFactory.IdentifierName(genericType.FullName));
            }
            return SyntaxFactory.GenericName(SyntaxFactory.Identifier(type.GetGenericTypeDefinition().FullName.Substring(0, type.GetGenericTypeDefinition().FullName.IndexOf("`"))),
                SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(genericTypes)));
        }

        /// <summary>
        /// Returns the name(string) for generic types
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="arguments">Type[]</param>
        /// <returns></returns>
        string GetGenericTypeString(Type type, Type[] arguments = null)
        {
            string typeString = type.GetGenericTypeDefinition().FullName.Substring(0, type.GetGenericTypeDefinition().FullName.IndexOf("`")) + "<";
            Type[] typeArguments;
            if (arguments == null)
                typeArguments = type.GetGenericArguments();
            else
                typeArguments = arguments;
            foreach (Type genericParameter in typeArguments)
            {
                typeString = typeString + genericParameter.ToString();
            }
            typeString = typeString + ">";
            return typeString;
        }
        #endregion
    }
}
