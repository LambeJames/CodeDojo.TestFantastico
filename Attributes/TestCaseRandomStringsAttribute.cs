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
        //private const string special = "!£$%&@()<>#{}[]\\?/";
        private const string special = "!";

        private readonly NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();
        private readonly Randomizer random = TestContext.CurrentContext.Random;

        public int Min { get; }

        public int Max { get; }

        public bool ExcludeSpecialCharacters { get; }


        public TestCaseRandomStringsAttribute(int min = 0, int max = 0, bool excludeSpecialChars = false)
        {
            Min = min;
            Max = max;
            ExcludeSpecialCharacters = excludeSpecialChars;
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

            testCaseData.AddRange(StringTests(Min));

            if (Min > 0)
            {
                testCaseData.AddRange(BoundaryTestsMin(Min));
            }

            if (Max > 0 && Max > Min)
            {
                testCaseData.AddRange(BoundaryTestsMax(Max));
            }

            return testCaseData;
        }

        private IEnumerable<TestCaseData> BoundaryTests(int charLength)
        {
            return Enumerable.Range(charLength - 1, 3)
                    .Select(x => new TestCaseData(RandomString(x, alphaNumeric)));
        }

        private IEnumerable<TestCaseData> BoundaryTestsMin(int charLength)
        {
            return new List<TestCaseData>
            {
                new TestCaseData(RandomString(charLength - 1, alphaNumeric)).Returns(false),
                new TestCaseData(RandomString(charLength, alphaNumeric)).Returns(true),
                new TestCaseData(RandomString(charLength + 1, alphaNumeric)).Returns(true)
            };
        }

        private IEnumerable<TestCaseData> BoundaryTestsMax(int charLength)
        {
            return new List<TestCaseData>
            {
                new TestCaseData(RandomString(charLength - 1, alphaNumeric)).Returns(true),
                new TestCaseData(RandomString(charLength, alphaNumeric)).Returns(true),
                new TestCaseData(RandomString(charLength + 1, alphaNumeric)).Returns(false)
            };
        }

        private IEnumerable<TestCaseData> StringTests(int charLength)
        {
            return new List<TestCaseData>
            {
                new TestCaseData(RandomString(charLength, alphaNumeric)).Returns(true),
                new TestCaseData(RandomString(charLength, numberic)).Returns(true),
                new TestCaseData(RandomString(charLength, alphaUpper)).Returns(true),
                new TestCaseData(RandomString(charLength, alphaLower)).Returns(true),
                new TestCaseData(RandomString(charLength, special)).Returns(!ExcludeSpecialCharacters)
            };
        }

        private string RandomString(int length, string source)
        {
            var stringChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = source[random.Next(source.Length)];
            }

            return new string(stringChars);
        }
    }
}
