//-----------------------------------------------------------------------
// <copyright file="AwaitRewriter.cs">
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

namespace Microsoft.PSharp.LanguageServices.Rewriting.CSharp
{
    /// <summary>
    /// The await statement rewriter.
    /// </summary>
    [CustomCSharpRewritingPass]
    public sealed class AwaitRewriter : CSharpRewriter
    {
        #region public API

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="program">IPSharpProgram</param>
        public AwaitRewriter(IPSharpProgram program)
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

            var statements = this.Program.GetSyntaxTree().GetRoot().DescendantNodes().OfType<AwaitExpressionSyntax>().
                ToList();
            
            if (statements.Count == 0)
            {
                return;
            }
            
            var root = this.Program.GetSyntaxTree().GetRoot().ReplaceNodes(
                nodes: statements,
                computeReplacementNode: (node, rewritten) => this.RewriteStatement(rewritten, model));

            base.UpdateSyntaxTree(root.ToString());
        }

        #endregion

        #region private methods

        /// <summary>
        /// Rewrites the await statement.
        /// </summary>
        /// <param name="node">ExpressionSyntax</param>
        /// <param name="model">SemanticModel</param>
        /// <returns>SyntaxNode</returns>
        private SyntaxNode RewriteStatement(ExpressionSyntax node, SemanticModel model)
        {
            AwaitExpressionSyntax awaitExpression = node as AwaitExpressionSyntax;
            ExpressionSyntax rewrittenNode = node;

            ITypeSymbol typeSymbol = model.GetTypeInfo(awaitExpression.Expression).Type;
            if (typeSymbol is INamedTypeSymbol)
            {
                string text = null;
                INamedTypeSymbol namedTypeSymbol = typeSymbol as INamedTypeSymbol;
                if (namedTypeSymbol.IsGenericType && namedTypeSymbol.TypeArguments.Count() == 1)
                {
                    text = $"ActorModel.GetResult<{namedTypeSymbol.TypeArguments[0]}>({awaitExpression.Expression})";
                }
                else
                {
                    text = $"ActorModel.Wait({awaitExpression.Expression})";
                }

                var waitExpression = SyntaxFactory.ParseExpression(text);
                waitExpression = waitExpression.WithTriviaFrom(node);
                rewrittenNode = waitExpression;
            }

            return rewrittenNode;
        }

        #endregion
    }
}
