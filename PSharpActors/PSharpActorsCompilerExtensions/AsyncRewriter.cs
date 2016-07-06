//-----------------------------------------------------------------------
// <copyright file="AsyncRewriter.cs">
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
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks;

namespace Microsoft.PSharp.LanguageServices.Rewriting.CSharp
{
    /// <summary>
    /// The async method rewriter.
    /// </summary>
    [CustomCSharpRewritingPass]
    [RewritingPassDependency(typeof(AwaitRewriter))]
    public sealed class AsyncRewriter : CSharpRewriter
    {
        #region public API

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="program">IPSharpProgram</param>
        public AsyncRewriter(IPSharpProgram program)
            : base(program)
        {

        }

        /// <summary>
        /// Rewrites the program.
        /// </summary>
        public override void Rewrite()
        {
            var compilation = base.Program.GetProject().GetCompilation();
            var model = compilation.GetSemanticModel(base.Program.GetSyntaxTree());

            var methods = this.Program.GetSyntaxTree().GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().
                Where(method => method.Modifiers.Any(val => val.IsKind(SyntaxKind.AsyncKeyword))).
                ToList();
            
            if (methods.Count == 0)
            {
                return;
            }
            
            var root = this.Program.GetSyntaxTree().GetRoot().ReplaceNodes(
                nodes: methods,
                computeReplacementNode: (node, rewritten) => this.InstrumentReturnStatementsInMethod(rewritten, model));

            base.UpdateSyntaxTree(root.ToString());

            methods = this.Program.GetSyntaxTree().GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().
                Where(method => method.Modifiers.Any(val => val.IsKind(SyntaxKind.AsyncKeyword))).
                ToList();

            root = this.Program.GetSyntaxTree().GetRoot().ReplaceNodes(
                nodes: methods,
                computeReplacementNode: (node, rewritten) => this.RemoveAsyncFromMethod(rewritten, model));

            base.UpdateSyntaxTree(root.ToString());
        }

        #endregion

        #region private methods

        /// <summary>
        /// Removes the async modifier in the specified method.
        /// </summary>
        /// <param name="node">MethodDeclarationSyntax</param>
        /// <param name="model">SemanticModel</param>
        /// <returns>SyntaxNode</returns>
        private SyntaxNode RemoveAsyncFromMethod(MethodDeclarationSyntax node, SemanticModel model)
        {
            MethodDeclarationSyntax rewrittenNode = node;
            SyntaxTokenList modifiers = rewrittenNode.Modifiers;
            SyntaxTokenList newModifiers = new SyntaxTokenList();

            foreach (var modifier in modifiers)
            {
                if (modifier.IsKind(SyntaxKind.AsyncKeyword))
                {
                    newModifiers = modifiers.Remove(modifier);
                }
            }

            rewrittenNode = node.WithModifiers(newModifiers);

            return rewrittenNode;
        }

        /// <summary>
        /// Instruments return statements in the specified method.
        /// </summary>
        /// <param name="node">MethodDeclarationSyntax</param>
        /// <param name="model">SemanticModel</param>
        /// <returns>SyntaxNode</returns>
        private SyntaxNode InstrumentReturnStatementsInMethod(MethodDeclarationSyntax node, SemanticModel model)
        {
            MethodDeclarationSyntax rewrittenNode = node;

            ITypeSymbol typeSymbol = model.GetTypeInfo(rewrittenNode.ReturnType).Type;
            if (typeSymbol is INamedTypeSymbol)
            {
                var returnStmts = rewrittenNode.DescendantNodes(_ => true).OfType<ReturnStatementSyntax>();
                INamedTypeSymbol namedTypeSymbol = typeSymbol as INamedTypeSymbol;
                if (namedTypeSymbol.IsGenericType && namedTypeSymbol.TypeArguments.Count() == 1)
                {
                    rewrittenNode = rewrittenNode.ReplaceNodes(
                        nodes: returnStmts,
                        computeReplacementNode: (stmt, rewritten) =>
                        {
                            var expr = SyntaxFactory.ParseExpression("System.Threading.Tasks." +
                                $"Task.FromResult({rewritten.Expression})");
                            expr = expr.WithTriviaFrom(rewritten.Expression);
                            return rewritten.WithExpression(expr);
                        });
                }
                else
                {
                    var expr = SyntaxFactory.ParseExpression("System.Threading.Tasks.Task.FromResult(true)");
                    expr = expr.WithLeadingTrivia(SyntaxFactory.Whitespace(" "));

                    rewrittenNode = rewrittenNode.ReplaceNodes(
                        nodes: returnStmts,
                        computeReplacementNode: (stmt, rewritten) =>
                        {
                            return rewritten.WithExpression(expr);
                        });

                    rewrittenNode = rewrittenNode.WithBody(
                        rewrittenNode.Body.AddStatements(
                            SyntaxFactory.ReturnStatement(expr)));
                }
            }

            return rewrittenNode;
        }

        #endregion
    }
}
