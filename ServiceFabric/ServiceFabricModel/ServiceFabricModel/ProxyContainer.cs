using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Microsoft.PSharp;
using Microsoft.PSharp.LanguageServices;
using Microsoft.PSharp.LanguageServices.Compilation;
using Microsoft.ServiceFabric.Actors;

namespace ServiceFabricModel
{
    public class ProxyContainer
    {
        private readonly Dictionary<Type, Type> proxyTypes;
        private AssemblyName aName;
        private AssemblyBuilder ab;
        private ModuleBuilder mb;
        private readonly object mutex;

        public ProxyContainer()
        {
            proxyTypes = new Dictionary<Type, Type>();
            mutex = new object();
        }

        /// <summary>
        /// For debugging.
        /// </summary>
        public void SaveModule()
        {
            lock (mutex)
            {
                ab.Save(aName.Name + ".dll");
            }
        }

        public Type GetProxyType(Type interfaceType, ActorId actorId)
        {
            lock (mutex)
            {
                Type res;
                proxyTypes.TryGetValue(interfaceType, out res);
                if (res == null)
                {
                    res = CreateProxyType(interfaceType, actorId);
                    proxyTypes.Add(interfaceType, res);
                }
                return res;
            }
        }

        private static Type CreateProxyType(Type interfaceType, ActorId actorId)
        {
            if (!interfaceType.IsInterface)
            {
                throw new InvalidOperationException();
            }

            var references = new HashSet<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(ActorId).Assembly.Location)
            };

            string assemblyPath = Assembly.GetEntryAssembly().Location + "\\..\\..\\..\\..";
            List<Assembly> allAssemblies = new List<Assembly>();
            foreach (string dll in Directory.GetFiles(assemblyPath, "*.exe", SearchOption.AllDirectories))
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

            var actorType = types.Where(p => interfaceType.IsAssignableFrom(p) && !p.IsInterface).Single();
            references.Add(MetadataReference.CreateFromFile(actorType.Assembly.Location));

            SyntaxTree syntaxTree = ProxyContainer.CreateProxySyntaxTree(
                interfaceType, actorType);
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
                OutputKind.DynamicallyLinkedLibrary, Assembly.GetExecutingAssembly().Location, false);

            Assembly proxyAssembly = Assembly.LoadFrom(proxyAssemblyPath);
            
            Type proxyType = null;

            try
            {
                proxyType = proxyAssembly.GetType(typeSymbol.ContainingNamespace.ToString() + "." + typeSymbol.Name);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Environment.Exit(Environment.ExitCode);
            }
            // Done
            return proxyType;
        }

        private static void LoadArguments(
            Type[] paramTypes,
            ILGenerator ilGen)
        {
            for (int i = 0; i < paramTypes.Length; ++i)
            {
                ilGen.Ldarg(i + 1);
            }
            // [ params ... ]

        }

        /// <summary>
        /// Creates the proxy syntax tree
        /// </summary>
        /// <param name="interfaceType">Actor interface type</param>
        /// <param name="actorType">Actor type</param>
        /// <returns>SyntaxTree</returns>
        private static SyntaxTree CreateProxySyntaxTree(Type interfaceType, Type actorType)
        {
            ClassDeclarationSyntax proxyDecl = ProxyContainer.CreateProxyClassDeclaration(
                interfaceType, actorType);

            NamespaceDeclarationSyntax namespaceDecl = SyntaxFactory.NamespaceDeclaration(
                SyntaxFactory.IdentifierName(interfaceType.Namespace));

            namespaceDecl = namespaceDecl.WithUsings(SyntaxFactory.List(
                new List<UsingDirectiveSyntax>
                {
                    SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System.Threading.Tasks")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("Microsoft.PSharp")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("Microsoft.ServiceFabric.Actors"))
                }));

            namespaceDecl = namespaceDecl.WithMembers(SyntaxFactory.List(
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
        /// <returns>ClassDeclarationSyntax</returns>
        private static ClassDeclarationSyntax CreateProxyClassDeclaration(Type interfaceType, Type actorType)
        {
            ClassDeclarationSyntax classDecl = SyntaxFactory.ClassDeclaration(interfaceType.Name + "_PSharpProxy")
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));

            FieldDeclarationSyntax target = ProxyContainer.CreateProxyField(
                interfaceType, "Target");
            FieldDeclarationSyntax id = ProxyContainer.CreateProxyField(
                typeof(MachineId), "Id");
            FieldDeclarationSyntax runtime = ProxyContainer.CreateProxyField(
                typeof(PSharpRuntime), "Runtime");

            ConstructorDeclarationSyntax constructor = ProxyContainer.CreateProxyConstructor(
                interfaceType, actorType);

            var baseTypes = new HashSet<BaseTypeSyntax>();
            var methodDecls = new List<MethodDeclarationSyntax>();
            foreach (var type in actorType.GetInterfaces())
            {
                baseTypes.Add(SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(type.FullName)));
                foreach (var method in type.GetMethods())
                {
                    methodDecls.Add(ProxyContainer.CreateProxyMethod(method, interfaceType));
                }
            }

            classDecl = classDecl.WithBaseList(SyntaxFactory.BaseList(
                SyntaxFactory.SeparatedList(baseTypes)));

            classDecl = classDecl.WithMembers(SyntaxFactory.List(
                new List<MemberDeclarationSyntax>
                {
                    target, id, runtime,
                    constructor
                }).AddRange(methodDecls));

            return classDecl;
        }

        /// <summary>
        /// Creates the proxy constructor.
        /// </summary>
        /// <param name="interfaceType">Actor interface type</param>
        /// <param name="actorType">Actor type</param>
        /// <returns>ConstructorDeclarationSyntax</returns>
        private static ConstructorDeclarationSyntax CreateProxyConstructor(Type interfaceType, Type actorType)
        {
            ConstructorDeclarationSyntax constructor = SyntaxFactory.ConstructorDeclaration(
                interfaceType.Name + "_PSharpProxy")
                .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(
                    new List<ParameterSyntax>
                    {
                        SyntaxFactory.Parameter(
                            SyntaxFactory.List<AttributeListSyntax>(),
                            SyntaxFactory.TokenList(),
                            SyntaxFactory.IdentifierName(typeof(PSharpRuntime).FullName),
                            SyntaxFactory.Identifier("runtime"),
                            null)
                    })))
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));

            ExpressionStatementSyntax targetConstruction = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.ThisExpression(), SyntaxFactory.IdentifierName("Target")),
                SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName(actorType.FullName))
                .WithArgumentList(SyntaxFactory.ArgumentList())));

            ExpressionStatementSyntax runtimeAssignment = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.ThisExpression(), SyntaxFactory.IdentifierName("Runtime")),
                SyntaxFactory.IdentifierName("runtime")));

            LocalDeclarationStatementSyntax machineTypeDecl = SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.IdentifierName(typeof(Type).FullName),
                    SyntaxFactory.SeparatedList(
                        new List<VariableDeclaratorSyntax>
                        {
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("machineType"),
                            null,
                            SyntaxFactory.EqualsValueClause(SyntaxFactory.TypeOfExpression(
                                SyntaxFactory.IdentifierName(typeof(ActorMachine).FullName))))
                        })));

            ExpressionStatementSyntax createMachine = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.ThisExpression(), SyntaxFactory.IdentifierName("Id")),
                SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                     SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                     SyntaxFactory.ThisExpression(), SyntaxFactory.IdentifierName("Runtime")),
                    SyntaxFactory.IdentifierName("CreateMachine")),
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SeparatedList(
                            new List<ArgumentSyntax>
                            {
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("machineType"))
                            })))));

            string eventType = typeof(ActorMachine).FullName + "." + typeof(ActorMachine.InitEvent).Name;
            string eventName = "initEvent";

            ArgumentListSyntax arguments = SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList(
                    new List<ArgumentSyntax>
                    {
                        SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.ThisExpression(),
                            SyntaxFactory.IdentifierName("Target")))
                    }));

            LocalDeclarationStatementSyntax eventDecl = ProxyContainer
                .CreateEventDeclaration(eventType, eventName, arguments);

            ExpressionStatementSyntax sendExpr = ProxyContainer.CreateSendEventExpression("Id", eventName);

            BlockSyntax body = SyntaxFactory.Block(targetConstruction, runtimeAssignment,
                machineTypeDecl, createMachine, eventDecl, sendExpr);
            constructor = constructor.WithBody(body);

            return constructor;
        }

        /// <summary>
        /// Creates a proxy field declaration.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="identifier">Identifier</param>
        /// <returns></returns>
        private static FieldDeclarationSyntax CreateProxyField(Type type, string identifier)
        {
            FieldDeclarationSyntax fieldDecl = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(
                ProxyContainer.GetTypeSyntax(type),
                SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(identifier))))
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)));
            return fieldDecl;
        }

        /// <summary>
        /// Creates a proxy method declaration.
        /// </summary>
        /// <param name="method">MethodInfo</param>
        /// <param name="interfaceType">Actor interface type</param>
        /// <returns>MethodDeclarationSyntax</returns>
        private static MethodDeclarationSyntax CreateProxyMethod(MethodInfo method, Type interfaceType)
        {
            List<ParameterSyntax> parameters = new List<ParameterSyntax>();
            List<ExpressionSyntax> payloadArguments = new List<ExpressionSyntax>();
            foreach (var parameter in method.GetParameters())
            {
                parameters.Add(SyntaxFactory.Parameter(
                    SyntaxFactory.List<AttributeListSyntax>(),
                    SyntaxFactory.TokenList(),
                    ProxyContainer.GetTypeSyntax(parameter.ParameterType),
                    SyntaxFactory.Identifier(parameter.Name),
                    null));
                payloadArguments.Add(SyntaxFactory.IdentifierName(parameter.Name));
            }

            MethodDeclarationSyntax methodDecl = SyntaxFactory.MethodDeclaration(
                ProxyContainer.GetTypeSyntax(method.ReturnType),
                method.Name)
                .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters)));

            LocalDeclarationStatementSyntax payloadDecl = SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.IdentifierName("object[]"),
                    SyntaxFactory.SeparatedList(
                        new List<VariableDeclaratorSyntax>
                        {
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("payload"),
                            null,
                            SyntaxFactory.EqualsValueClause(SyntaxFactory.ObjectCreationExpression(
                                SyntaxFactory.IdentifierName("object[]"))
                                .WithInitializer(SyntaxFactory.InitializerExpression(
                                    SyntaxKind.ArrayInitializerExpression,
                                    SyntaxFactory.SeparatedList(payloadArguments)))))
                        })));

            string eventType = typeof(ActorMachine).FullName + "." + typeof(ActorMachine.ActorEvent).Name;
            string eventName = "actorEvent";

            ArgumentListSyntax arguments = SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList(
                    new List<ArgumentSyntax>
                    {
                        SyntaxFactory.Argument(SyntaxFactory.TypeOfExpression(
                            SyntaxFactory.IdentifierName(interfaceType.FullName))),
                        SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            SyntaxFactory.Literal(method.Name))),
                        SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.ThisExpression(),
                            SyntaxFactory.IdentifierName("Target"))),
                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("payload"))
                    }));

            LocalDeclarationStatementSyntax eventDecl = ProxyContainer
                .CreateEventDeclaration(eventType, eventName, arguments);

            ExpressionStatementSyntax sendExpr = ProxyContainer.CreateSendEventExpression("Id", eventName);

            ReturnStatementSyntax returnStmt = null;
            if (method.ReturnType.GetGenericArguments().Count() > 0)
            {
                Type genericType = method.ReturnType.GetGenericArguments().First();
                returnStmt = ProxyContainer.CreateReturnExpression(method.ReturnType, genericType,
                    SyntaxFactory.Block(SyntaxFactory.ReturnStatement(
                        SyntaxFactory.DefaultExpression(
                            ProxyContainer.GetTypeSyntax(genericType)))));
            }
            else
            {
                var returnType = method.ReturnType.GetGenericArguments().First();
                returnStmt = ProxyContainer.CreateReturnExpression(method.ReturnType, null,
                    SyntaxFactory.Block());
            }

            BlockSyntax body = SyntaxFactory.Block(payloadDecl, eventDecl,
                sendExpr, returnStmt);
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
        private static LocalDeclarationStatementSyntax CreateEventDeclaration(
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
        private static ExpressionStatementSyntax CreateSendEventExpression(
            string target, string eventName)
        {
            ExpressionStatementSyntax sendExpr = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                     SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                     SyntaxFactory.ThisExpression(), SyntaxFactory.IdentifierName("Runtime")),
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
        /// Creates a return statement.
        /// </summary>
        /// <param name="returnType">Return type</param>
        /// <param name="genericType">Generic type</param>
        /// <param name="eventName">Body</param>
        /// <returns>ReturnStatementSyntax</returns>
        private static ReturnStatementSyntax CreateReturnExpression(Type returnType, Type genericType, BlockSyntax body)
        {
            List<TypeSyntax> genericTypes = new List<TypeSyntax>();
            if (genericType != null)
            {
                genericTypes.Add(SyntaxFactory.IdentifierName(genericType.FullName));
            }

            ReturnStatementSyntax returnStmt = SyntaxFactory.ReturnStatement(
                SyntaxFactory.ObjectCreationExpression(
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(typeof(DummyTask).FullName),
                    SyntaxFactory.TypeArgumentList(
                        SyntaxFactory.SeparatedList(genericTypes))))
                .WithArgumentList(SyntaxFactory.ArgumentList(
                    SyntaxFactory.SeparatedList(
                        new List<ArgumentSyntax>
                        {
                            SyntaxFactory.Argument(SyntaxFactory.ParenthesizedLambdaExpression(body))
                        }))));
            return returnStmt;
        }

        /// <summary>
        /// Returns the type syntax from the specified type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>TypeSyntax</returns>
        private static TypeSyntax GetTypeSyntax(Type type)
        {
            TypeSyntax syntax = null;

            if (type.IsGenericType)
            {
                List<TypeSyntax> genericTypes = new List<TypeSyntax>();
                foreach (var genericType in type.GetGenericArguments())
                {
                    genericTypes.Add(SyntaxFactory.IdentifierName(genericType.FullName));
                }

                string genericTypeName = type.GetGenericTypeDefinition().FullName;
                genericTypeName = genericTypeName.Substring(0, genericTypeName.LastIndexOf('`'));

                syntax = SyntaxFactory.GenericName(
                    SyntaxFactory.Identifier(genericTypeName),
                    SyntaxFactory.TypeArgumentList(
                        SyntaxFactory.SeparatedList(genericTypes)));
            }
            else
            {
                syntax = SyntaxFactory.IdentifierName(type.FullName);
            }
            
            return syntax;
        }
    }
}
