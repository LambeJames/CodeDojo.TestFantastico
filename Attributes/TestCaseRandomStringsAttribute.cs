using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeDojo.TestFantastico.Attributes
{
    public class TestCaseRandomStringsAttribute : NUnitAttribute, ITestBuilder, IImplyFixture
    {
        private const string alphaNumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const string numberic = "0123456789";
        private const string alphaUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string alphaLower = "abcdefghijklmnopqrstuvwxyz";
        private const string special = "!£$%&@()<>#{}[]\\?/";

        private readonly NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();
        private readonly Random random = new Random();

        public int Min { get; }

        public int Max { get; }

        public int Length { get; }


        public TestCaseRandomStringsAttribute(int min = 0, int max = 0, int length = 6)
        {
            Min = min;
            Max = max;
            Length = length;
        }

        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite)
        {
            int count = 0;

            foreach (TestCaseParameters parms in BuildTestCaseData())
            {
                count++;
                yield return _builder.BuildTestMethod(method, suite, parms);
            }

            // If count > 0, error messages will be shown for each case
            // but if it's 0, we need to add an extra "test" to show the message.
            if (count == 0 && method.GetParameters().Length == 0)
            {
                var parms = new TestCaseParameters();
                parms.RunState = RunState.NotRunnable;
                parms.Properties.Set(PropertyNames.SkipReason, "TestCaseSourceAttribute may not be used on a method without parameters");

                yield return _builder.BuildTestMethod(method, suite, parms);
            }
        }

        public IEnumerable<TestCaseData> BuildTestCaseData()
        {
            var testCaseData = new List<TestCaseData>();

            testCaseData.AddRange(StringTests(Length));

            if (Min > 0)
            {
                testCaseData.AddRange(BoundaryTests(Min));
            }

            if (Max > 0 && Max > Min)
            {
                testCaseData.AddRange(BoundaryTests(Max));
            }

            return testCaseData;
        }

        private IEnumerable<TestCaseData> BoundaryTests(int charLength)
        {
            return Enumerable.Range(charLength - 1, 3)
                    .Select(x => new TestCaseData(RandomString(x, alphaNumeric)));
        }

        private IEnumerable<TestCaseData> StringTests(int charLength)
        {
            return new List<TestCaseData>
            {
                new TestCaseData(RandomString(charLength, alphaNumeric)),
                new TestCaseData(RandomString(charLength, numberic)),
                new TestCaseData(RandomString(charLength, alphaUpper)),
                new TestCaseData(RandomString(charLength, alphaLower)),
                new TestCaseData(RandomString(charLength, special))
            };
        }

        private string RandomString(int length, string source)
        {
            return new string(Enumerable.Repeat(source, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
