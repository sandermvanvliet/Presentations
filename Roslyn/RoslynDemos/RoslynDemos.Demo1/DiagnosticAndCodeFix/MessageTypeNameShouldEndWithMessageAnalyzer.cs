using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RoslynDemos.Demo1
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MessageTypeNameShouldEndWithMessageAnalyzer : ISymbolAnalyzer
    {
        public const string DiagnosticId = "SOCO0001";
        internal const string Description = "Name of a type that derives from Message should end with Message";
        internal const string MessageFormat = "Type name '{0}' does not end with Message";
        internal const string Category = "Naming";

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

        public ImmutableArray<SymbolKind> SymbolKindsOfInterest
        {
            get { return ImmutableArray.Create(SymbolKind.NamedType); }
        }

        public void AnalyzeSymbol(
            ISymbol symbol,
            Compilation compilation,
            Action<Diagnostic> addDiagnostic,
            AnalyzerOptions options,
            CancellationToken cancellationToken)
        {
            var namedTypeSymbol = (INamedTypeSymbol)symbol;

            if (TypeDerivesFromMessage(namedTypeSymbol) &&
                TypeNameDoesNotEndWithMessage(namedTypeSymbol))
            {
                var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

                addDiagnostic(diagnostic);
            }
        }

        private static bool TypeDerivesFromMessage(INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol.BaseType.Name.Equals("Message", StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool TypeNameDoesNotEndWithMessage(INamedTypeSymbol namedTypeSymbol)
        {
            return !namedTypeSymbol.Name.EndsWith("Message", StringComparison.InvariantCultureIgnoreCase);
        }

    }
}
