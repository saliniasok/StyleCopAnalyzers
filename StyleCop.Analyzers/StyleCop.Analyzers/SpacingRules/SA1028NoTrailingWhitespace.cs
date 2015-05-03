﻿namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Discovers any C# lines of code with trailing whitespace.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains whitespace at the end of the line.</para>
    ///
    /// <para>Trailing whitespace causes unnecessary diffs in source control,
    /// looks tacky in editors that show invisible whitespace as visible characters,
    /// and is highlighted as an error in some configurations of git.</para>
    ///
    /// <para>For these reasons, trailing whitespace should be avoided.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1028NoTrailingWhitespace : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1028NoTrailingWhitespace"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1028";
        internal const string Title = "Code must not contain trailing whitespace";
        internal const string MessageFormat = "Remove trailing whitespace";
        internal const string Category = "Style";

        internal static DiagnosticDescriptor Rule { get; } = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, customTags: new[] { WellKnownDiagnosticTags.Unnecessary });

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(Rule);
            }
        }

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(this.HandleSyntaxTree);
        }

        /// <summary>
        /// Scans an entire document for lines with trailing whitespace.
        /// </summary>
        /// <param name="context">The context that provides the document to scan.</param>
        private void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetRoot(context.CancellationToken);
            foreach (var trivia in root.DescendantTrivia(descendIntoTrivia: true))
            {
                if (!trivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    continue;
                }

                bool reportWarning = false;
                var token = trivia.Token;
                SyntaxTriviaList triviaList;
                if (token.LeadingTrivia.Contains(trivia))
                {
                    triviaList = token.LeadingTrivia;
                }
                else
                {
                    triviaList = token.TrailingTrivia;
                }

                bool foundWhitespace = false;
                foreach (var innerTrivia in triviaList)
                {
                    if (!foundWhitespace)
                    {
                        if (innerTrivia.Equals(trivia))
                            foundWhitespace = true;

                        continue;
                    }

                    if (innerTrivia.IsKind(SyntaxKind.EndOfLineTrivia))
                        reportWarning = true;

                    break;
                }

                if (reportWarning)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, trivia.GetLocation()));
                }
            }
        }
    }
}
