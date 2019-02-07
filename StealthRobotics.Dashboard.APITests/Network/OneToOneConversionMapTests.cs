using NUnit.Framework;
using StealthRobotics.Dashboard.API.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.API.Network.Tests
{
    [TestFixture()]
    public class OneToOneConversionMapTests
    {
        private OneToOneConversionMap<string, string> instance;

        [SetUp]
        public void PerTestSetup()
        {
            //because we have knowledge of implementation, we can assume that add will maintain a valid state
            //since not much is going on. Likewise, there's not too much going on in the Get methods
            //For milestone 1, I really only want to test the "Try____" methods because they have meaningful choices being made
            //with a few inputs and outputs
            instance = new OneToOneConversionMap<string, string>();
            //add some data. doesn't matter too much as long as we know what's in the map
            instance.Add("a", "b");
            instance.Add("", "c");
            instance.Add("d", "text");
        }

        [TearDown]
        public void PerTestTeardown()
        {
            instance.Clear();
            instance = null;
        }

        [Test()]
        public void AddTest()
        {
            Assert.Fail("Not implemented");
        }

        [Test()]
        public void GetByFirstTest()
        {
            Assert.Fail("Not implemented");
        }

        [Test()]
        public void GetBySecondTest()
        {
            Assert.Fail("Not implemented");
        }

        [Test()]
        public void RemoveByFirstTest()
        {
            Assert.Fail("Not implemented");
        }

        [Test()]
        public void RemoveBySecondTest()
        {
            Assert.Fail("Not implemented");
        }

        [Test()]
        public void MapConversionByFirstTest()
        {
            Assert.Fail("Not implemented");
        }

        [Test()]
        public void MapConversionBySecondTest()
        {
            Assert.Fail("Not implemented");
        }

        [Test()]
        public void GetConverterByFirstTest()
        {
            Assert.Fail("Not implemented");
        }

        [Test()]
        public void GetConverterBySecondTest()
        {
            Assert.Fail("Not implemented");
        }

        [Test()]
        //both duplicate values
        [TestCase("a", "b", false)]
        //duplicate first only
        [TestCase("", "x", false)]
        //duplicate second only
        [TestCase("asdf", "text", false)]
        //inverted value (ie first is a value for second but not first, vice versa)
        [TestCase("b", "a", true)]
        //totally new values
        [TestCase("f", "g", true)]
        public void TryAddTest(string first, string second, bool expected)
        {
            Assert.AreEqual(expected, instance.TryAdd(first, second));
        }

        [Test()]
        //first is in table
        [TestCase("", "c", true)]
        //first is not in table at all
        [TestCase("x", "don't care", false)]
        //first is a value for second, but not for first
        [TestCase("text", "don't care", false)]
        public void TryGetByFirstTest(string first, string expectedOut, bool expectedResult)
        {
            //2 ways to pass/fail this test. we should always compare expectedResult
            //if expectedResult is true we also need to check the output value
            bool val = instance.TryGetByFirst(first, out string @out);
            //fine to terminate if this fails; in fact it may be unhelpful to check the output value
            Assert.AreEqual(expectedResult, val, "Did not return correct truth value");
            if(expectedResult)
            {
                Assert.AreEqual(expectedOut, @out, "Did not return correct data value");
            }
        }

        [Test()]
        //second is in table
        [TestCase("text", "d", true)]
        //second is not in the table at all
        [TestCase("x", "don't care", false)]
        //second is a value for first, but not for second
        [TestCase("a", "don't care", false)]
        public void TryGetBySecondTest(string second, string expectedOut, bool expectedResult)
        {
            //2 ways to pass/fail this test. we should always compare expectedResult
            //if expectedResult is true we also need to check the output value
            bool val = instance.TryGetBySecond(second, out string @out);
            //fine to terminate if this fails; in fact it may be unhelpful to check the output value
            Assert.AreEqual(expectedResult, val, "Did not return correct truth value");
            if (expectedResult)
            {
                Assert.AreEqual(expectedOut, @out, "Did not return correct data value");
            }
        }

        [Test()]
        //first is in table
        [TestCase("", true)]
        //first is not in table at all
        [TestCase("x", false)]
        //first is a value for second, but not for first
        [TestCase("b", false)]
        public void TryRemoveByFirstTest(string first, bool expectedResult)
        {
            Assert.AreEqual(expectedResult, instance.TryRemoveByFirst(first));
        }

        [Test()]
        //second is in table
        [TestCase("b", true)]
        //second is not in table at all
        [TestCase("x", false)]
        //second is a value for first, but not for second
        [TestCase("", false)]
        public void TryRemoveBySecondTest(string second, bool expectedResult)
        {
            Assert.AreEqual(expectedResult, instance.TryRemoveBySecond(second));
        }
    }
}