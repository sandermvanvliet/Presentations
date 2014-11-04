using Microsoft.CodeAnalysis.Diagnostics;
using System;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Threading;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;

namespace RoslynDemos.Demo1
{

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SingleClassPerFileAnalyzer : ISemanticModelAnalyzer
    {
        public const string DiagnosticId = "SOCO0002";
        internal const string Description = "File should contain a single type";
        internal const string MessageFormat = "File '{0}' contains more than one type";
        internal const string Category = "Structure";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Description,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            true);

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public void AnalyzeSemanticModel(
            SemanticModel semanticModel,
            Action<Diagnostic> addDiagnostic,
            AnalyzerOptions options,
            CancellationToken cancellationToken)
        {
            var rootSyntaxNode = semanticModel.SyntaxTree.GetRoot();

            var classes = rootSyntaxNode
                .DescendantNodes()
                .Where(node => node.CSharpKind() == Microsoft.CodeAnalysis.CSharp.SyntaxKind.ClassDeclaration)
                .OfType<ClassDeclarationSyntax>()
                .ToArray();

            var moreThanOne = classes
                .Select(c => new
                {
                    Name = c.Identifier.Text,
                    FilePath = Path.GetFileName(c.GetLocation().SourceTree.FilePath),
                    Location = c.Identifier.GetLocation()
                })
                .GroupBy(res => res.FilePath,
                         res => res,
                         (key, items) => new
                         {
                             FilePath = key,
                             Classes = items
                         })
                .Where(res => res.Classes.Count() > 1)
                .ToArray();

            foreach (var match in moreThanOne)
            {
                foreach (var offendingClass in match.Classes.Skip(1))
                {
                    var diagnostic = Diagnostic.Create(Rule, offendingClass.Location, offendingClass.FilePath);

                    addDiagnostic(diagnostic);
                }
            }
        }
    }
}
