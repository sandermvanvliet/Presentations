using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace RoslynDemos.Demo1
{
    [ExportCodeFixProvider(MessageTypeNameShouldEndWithMessageAnalyzer.DiagnosticId, LanguageNames.CSharp)]
    public class CodeFixProvider : ICodeFixProvider
    {
        public IEnumerable<string> GetFixableDiagnosticIds()
        {
            return new[] { MessageTypeNameShouldEndWithMessageAnalyzer.DiagnosticId };
        }

        public async Task<IEnumerable<CodeAction>> GetFixesAsync(
            Document document,
            TextSpan span,
            IEnumerable<Diagnostic> diagnostics,
            CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var diagnosticSpan = diagnostics.First().Location.SourceSpan;

            var declaration = GetTypeDeclarationFor(diagnosticSpan, root);

            return new[] {
                CodeAction.Create("Append 'Message'", c => AppendMessageToTypeName(document, declaration, c))
            };
        }

        private async Task<Solution> AppendMessageToTypeName(
            Document document,
            TypeDeclarationSyntax typeDecl,
            CancellationToken cancellationToken)
        {
            var identifierToken = typeDecl.Identifier;
            var newName = identifierToken.Text + "Message";

            // Get the symbol representing the type to be renamed.
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);

            // Produce a new solution that has all references to that type renamed, including the declaration.
            var originalSolution = document.Project.Solution;

            var optionSet = originalSolution.Workspace.Options;

            var newSolution = await Renamer.RenameSymbolAsync(
                    document.Project.Solution,
                    typeSymbol,
                    newName,
                    optionSet,
                    cancellationToken)
                .ConfigureAwait(false);

            return newSolution;
        }
        private static TypeDeclarationSyntax GetTypeDeclarationFor(TextSpan diagnosticSpan, SyntaxNode root)
        {
            return root
                    .FindToken(diagnosticSpan.Start)
                    .Parent
                    .AncestorsAndSelf()
                    .OfType<TypeDeclarationSyntax>()
                    .First();
        }
    }
}