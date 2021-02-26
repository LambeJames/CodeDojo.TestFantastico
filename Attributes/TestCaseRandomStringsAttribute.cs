using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using System.Collections.Generic;

namespace CodeDojo.TestFantastico.Attributes
{
    public class TestCaseRandomStringsAttribute : NUnitAttribute, ITestBuilder, IImplyFixture
    {
        private const string Numeric = "0123456789";
        private const string AlphaUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string AlphaLower = "abcdefghijklmnopqrstuvwxyz";
        private const string Special = "!£$%&@()<>#{}[]\\?/";

        private readonly NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();
        private readonly Randomizer _random = TestContext.CurrentContext.Random;

        public int Min { get; }

        public int Max { get; }

        public bool ExcludeSpecialCharacters { get; }
        public bool ExcludeLowercase { get; }
        public bool ExcludeUppercase { get; }
        public bool ExcludeNumeric { get; }


        public TestCaseRandomStringsAttribute(
            int min = 0,
            int max = 0,
            bool excludeSpecialChars = false,
            bool excludeLowercase = false,
            bool excludeUppercase = false,
            bool excludeNumeric = false)
        {
            Min = min;
            Max = max;
            ExcludeSpecialCharacters = excludeSpecialChars;
            ExcludeLowercase = excludeLowercase;
            ExcludeUppercase = excludeUppercase;
            ExcludeNumeric = excludeNumeric;
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
            var baseString = !ExcludeLowercase
                ? AlphaLower
                : !ExcludeUppercase
                    ? AlphaUpper
                    : !ExcludeSpecialCharacters
                        ? Special
                        : Numeric;

            testCaseData.AddRange(StringTests(Min));

            if (Min > 0)
            {
                testCaseData.AddRange(BoundaryTestsMin(Min, baseString));
            }

            if (Max > 0 && Max > Min)
            {
                testCaseData.AddRange(BoundaryTestsMax(Max, baseString));
            }

            return testCaseData;
        }

        private IEnumerable<TestCaseData> BoundaryTestsMin(int charLength, string baseString)
        {
            return new List<TestCaseData>
            {
                new TestCaseData(RandomString(charLength - 1, baseString)).Returns(false),
                new TestCaseData(RandomString(charLength, baseString)).Returns(true),
                new TestCaseData(RandomString(charLength + 1, baseString)).Returns(true)
            };
        }

        private IEnumerable<TestCaseData> BoundaryTestsMax(int charLength, string baseString)
        {
            return new List<TestCaseData>
            {
                new TestCaseData(RandomString(charLength - 1, baseString)).Returns(true),
                new TestCaseData(RandomString(charLength, baseString)).Returns(true),
                new TestCaseData(RandomString(charLength + 1, baseString)).Returns(false)
            };
        }

        private IEnumerable<TestCaseData> StringTests(int charLength)
        {
            return new List<TestCaseData>
            {
                new TestCaseData(RandomString(charLength, AlphaUpper, Numeric)).Returns(!ExcludeNumeric && !ExcludeUppercase),
                new TestCaseData(RandomString(charLength, AlphaLower, Numeric)).Returns(!ExcludeNumeric && !ExcludeLowercase),
                new TestCaseData(RandomString(charLength, Numeric)).Returns(!ExcludeNumeric),
                new TestCaseData(RandomString(charLength, AlphaUpper)).Returns(!ExcludeUppercase),
                new TestCaseData(RandomString(charLength, AlphaLower)).Returns(!ExcludeLowercase),
                new TestCaseData(RandomString(charLength, Special)).Returns(!ExcludeSpecialCharacters)
            };
        }

        private string RandomString(int length, string source1, string source2)
        {
            var source1Length = (length - 1) / 2;
            var source2Length = (length - 1) / 2 + 1;
            return $"{RandomString(source1Length, source1)}{RandomString(source2Length, source2)}";
        }

        private string RandomString(int length, string source)
        {
            var stringChars = new char[length];
            for (var i = 0; i < length; i++)
            {
                stringChars[i] = source[_random.Next(source.Length)];
            }

            return new string(stringChars);
        }
    }
}
