using CodeDojo.TestFantastico.Attributes;
using NUnit.Framework;
using System.Collections.Generic;

namespace CodeDojo.TestFantastico
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCaseRandomStrings(5, 10, 6)]
        public void Test1(string randomString)
        {
            Assert.AreEqual("a", randomString);
        }
    }
}