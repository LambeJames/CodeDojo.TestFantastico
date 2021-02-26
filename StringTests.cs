using System.Linq;
using CodeDojo.TestFantastico.Attributes;
using NUnit.Framework;

namespace CodeDojo.TestFantastico
{
    public class StringTests
    {
        private const int MaxLength = 10;
        private const int MinLength = 5;

        [SetUp]
        public void Setup()
        {
        }

        [TestCaseRandomStrings(MinLength, MaxLength)]
        public bool LengthTest(string randomString)
        {
            return StringLengthCheck(randomString);
        }

        [TestCaseRandomStrings(MinLength, MaxLength, excludeUppercase: true, excludeSpecialChars: true, excludeNumeric: true)]
        public bool LowerTest(string randomString)
        {
            return StringLengthCheck(randomString) && StringOnlyLower(randomString);
        }

        [TestCaseRandomStrings(MinLength, MaxLength, excludeLowercase: true, excludeSpecialChars: true, excludeNumeric: true)]
        public bool UpperTest(string randomString)
        {
            return StringLengthCheck(randomString) && StringOnlyUpper(randomString);
        }

        [TestCaseRandomStrings(MinLength, MaxLength, excludeLowercase: true, excludeUppercase: true, excludeNumeric: true)]
        public bool SpecialCharsTest(string randomString)
        {
            return StringLengthCheck(randomString) && StringOnlySpecial(randomString);
        }

        [TestCaseRandomStrings(MinLength, MaxLength, true)]
        public bool NotSpecialCharsTest(string randomString)
        {
            return StringLengthCheck(randomString) && !StringOnlySpecial(randomString);
        }

        private static bool StringLengthCheck(string input) => input.Length <= MaxLength && input.Length >= MinLength;

        private static bool StringOnlyLower(string input) => input.All(char.IsLower);

        private static bool StringOnlyUpper(string input) => input.All(char.IsUpper);

        private static bool StringOnlySpecial(string input) => !input.Any(char.IsLetterOrDigit);
    }
}