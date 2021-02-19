using CodeDojo.TestFantastico.Attributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CodeDojo.TestFantastico
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCaseRandomStrings(5, 10)]
        public bool Test1(string randomString)
        {
            return StringTest(randomString);
        }

        [TestCaseRandomStrings(5, 10, true)]
        public bool Test2(string randomString)
        {
            return SpecialTest(randomString);
        }

        private bool StringTest(string input)
        {
            return input.Length <= 10 && input.Length >= 5;
        }

        private bool SpecialTest(string input)
        {
            return !input.Contains("!") && StringTest(input);
        }
    }
}