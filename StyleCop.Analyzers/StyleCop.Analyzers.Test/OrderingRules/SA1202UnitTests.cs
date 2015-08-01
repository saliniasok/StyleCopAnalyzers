﻿namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1202ElementsMustBeOrderedByAccess"/>.
    /// </summary>
    public class SA1202UnitTests : DiagnosticVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle valid access level ordering.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidOrderAsync()
        {
            var testCode = @"public class TestClass
{
    private const string TestConst = ""Test"";

    protected readonly string TestReadonlyString;

    public string TestString;

    public TestClass()
    {
    }

    private TestClass(string a)
    {
    }

    ~TestClass() { }
    
    public string TestProperty1 { get; set; }
    
    internal string TestProperty2 { get; set; }
    
    protected internal string TestProperty3 { get; set; }
    
    protected string TestProperty4 { get; set; }
    
    private string TestProperty5 { get; set; }
    
    public void TestMethod1()
    {
    }
    
    private void TestMethod2()
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle class access levels.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestClassOrderingAsync()
        {
            var testCode = @"internal class TestClass1 { }
public class TestClass2 { }
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(2, 14).WithArguments("Public", "classes", "internal")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle interfaces before classes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInternalInterfaceBeforePublicClassAsync()
        {
            var testCode = @"internal interface ITestInterface { }
public class TestClass2 { }
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle property access levels.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPropertiesAsync()
        {
            var testCode = @"public class TestClass
{
    private string TestProperty1 { get; set; }
    protected string TestProperty2 { get; set; }
    protected internal string TestProperty3 { get; set; }
    internal string TestProperty4 { get; set; }
    public string TestProperty5 { get; set; }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 22).WithArguments("Protected", "properties", "private"),
                this.CSharpDiagnostic().WithLocation(5, 31).WithArguments("Protected internal", "properties", "protected"),
                this.CSharpDiagnostic().WithLocation(6, 21).WithArguments("Internal", "properties", "protected internal"),
                this.CSharpDiagnostic().WithLocation(7, 19).WithArguments("Public", "properties", "internal")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle method access levels.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMethodsAsync()
        {
            var testCode = @"public class TestClass
{
    private void TestMethod1() { }
    protected void TestMethod2() { }
    protected internal void TestMethod3() { }
    internal void TestMethod4() { }
    public void TestMethod5() { }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 20).WithArguments("Protected", "methods", "private"),
                this.CSharpDiagnostic().WithLocation(5, 29).WithArguments("Protected internal", "methods", "protected"),
                this.CSharpDiagnostic().WithLocation(6, 19).WithArguments("Internal", "methods", "protected internal"),
                this.CSharpDiagnostic().WithLocation(7, 17).WithArguments("Public", "methods", "internal")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle protected internal before public.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestProtectedInternalBeforePublicAsync()
        {
            var testCode = @"public class TestClass
{
    protected internal event System.Action TestEvent1;
    public event System.Action TestEvent2;
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 5).WithArguments("Public", "events", "protected internal")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle protected before public.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestProtectedBeforePublicAsync()
        {
            var testCode = @"public class TestClass
{
    protected string TestField1;
    public string TestField2;
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 19).WithArguments("Public", "fields", "protected")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle private before public.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPrivateBeforePublicAsync()
        {
            var testCode = @"public class TestClass
{
    private event System.Action TestEvent1 { add { } remove { } }
    public event System.Action TestEvent2 { add { } remove { } }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 32).WithArguments("Public", "events", "private")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle protected before internal.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestProtectedBeforeInternalAsync()
        {
            var testCode = @"public class TestClass
{
    protected event System.Action TestEvent1 { add { } remove { } }
    internal event System.Action TestEvent2 { add { } remove { } }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 34).WithArguments("Internal", "events", "protected")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle private before internal.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPrivateBeforeInternalAsync()
        {
            var testCode = @"public class TestClass
{
    private delegate void TestDelegate1();
    internal delegate void TestDelegate2();
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 28).WithArguments("Internal", "delegates", "private")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle private before internal.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPrivateBeforeProtectedInternalAsync()
        {
            var testCode = @"public class TestClass
{
    private void TestMethod1() { }
    protected internal void TestMethod2() { }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 29).WithArguments("Protected internal", "methods", "private")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle internal keyword followed by protected.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInternalProtectedCountsAsProtectedInternalAsync()
        {
            var testCode = @"public class TestClass
{
    private void TestMethod1() { }
    internal protected void TestMethod2() { }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 29).WithArguments("Protected internal", "methods", "private")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle unqualified members.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMembersWithoutAccessModifiersSkippedAsync()
        {
            var testCode = @"public class TestClass
{
    string TestField1;
    public string TestField2;
    string TestField3;
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle multiple violations.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestOnlyFirstViolationReportedAsync()
        {
            var testCode = @"public class TestClass
{
    private string TestField1;
    public string TestField2;
    public string TestField3;
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 19).WithArguments("Public", "fields", "private")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle incomplete members.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIncompleteMemberAsync()
        {
            string testCode = @"public class OuterType
{
    private string Test;
    public string
    public string
}
";

            // We don't care about the syntax errors.
            var expected = new[]
            {
                 new DiagnosticResult
                 {
                     Id = "CS1585",
                     Message = "Member modifier 'public' must precede the member type and name",
                     Severity = DiagnosticSeverity.Error,
                     Locations = new[] { new DiagnosticResultLocation("Test0.cs", 5, 5) }
                 },
                 new DiagnosticResult
                 {
                     Id = "CS1519",
                     Message = "Invalid token '}' in class, struct, or interface member declaration",
                     Severity = DiagnosticSeverity.Error,
                     Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 1) }
                 }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1202ElementsMustBeOrderedByAccess();
        }
    }
}
