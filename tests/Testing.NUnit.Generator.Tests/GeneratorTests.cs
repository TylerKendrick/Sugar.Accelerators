using MyNamespace;

[assembly: GenerateTestDouble(typeof(Testing.NUnit.TestClassConcern))]

namespace Testing.NUnit.Tests
{
    public partial class TestClassConcernTestsWrapper
    {
        private Testing.NUnit.TestDoubles.TestClassConcernTestDouble _testDouble;
    }
}