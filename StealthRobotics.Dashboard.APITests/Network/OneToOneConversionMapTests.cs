using NUnit.Framework;
using StealthRobotics.Dashboard.API.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace StealthRobotics.Dashboard.API.Network.Tests
{
    [TestFixture()]
    public class OneToOneConversionMapTests
    {
        private OneToOneConversionMap<string, string> instance;
        private readonly IValueConverter testConverter = new BooleanToVisibilityConverter();

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
        public void Add_WithDuplicateFirst_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => instance.Add("a", "Don't care"));
        }

        [Test()]
        public void Add_WithDuplicateSecond_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => instance.Add("Don't care", "b"));
        }

        [Test()]
        public void Add_WithNewValues_Passes()
        {
            Assert.DoesNotThrow(() => instance.Add("e", "f"));
        }

        [Test()]
        public void GetByFirst_WithExistingValue_ReturnsCorrespondingValue()
        {
            string expected = "b";
            Assert.AreEqual(expected, instance.GetByFirst("a"));
        }

        [Test()]
        public void GetByFirst_WithNewValue_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => instance.GetByFirst("b"));
        }

        [Test()]
        public void GetBySecond_WithExistingValue_ReturnsCorrespondingValue()
        {
            string expected = "";
            Assert.AreEqual(expected, instance.GetBySecond("c"));
        }

        [Test()]
        public void GetBySecond_WithNewValue_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => instance.GetBySecond("q"));
        }

        [Test()]
        public void RemoveByFirst_WithExistingValue_Passes()
        {
            Assert.DoesNotThrow(() => instance.RemoveByFirst("d"));
        }

        [Test()]
        public void RemoveByFirst_WithNewValue_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => instance.RemoveByFirst("x"));
        }

        [Test()]
        public void RemoveBySecond_WithExistingValue_Passes()
        {
            Assert.DoesNotThrow(() => instance.RemoveBySecond("c"));
        }

        [Test]
        public void RemoveBySecond_WithNewValue_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => instance.RemoveBySecond("nothing"));
        }

        [Test()]
        public void MapConversionByFirst_WithExistingValueRealConverter_Passes()
        {
            Assert.DoesNotThrow(() => instance.MapConversionByFirst("a", testConverter));
        }

        [Test()]
        public void MapConversionByFirst_WithExistingValueNullConverter_Passes()
        {
            Assert.DoesNotThrow(() => instance.MapConversionByFirst("", null));
        }

        [Test()]
        public void MapConversionByFirst_WithNewValueRealConverter_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => instance.MapConversionByFirst("x", testConverter));
        }

        [Test()]
        public void MapConversionByFirst_WithNewValueNullConverter_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => instance.MapConversionByFirst("q", null));
        }

        [Test()]
        public void MapConversionBySecond_WithExistingValueRealConverter_Passes()
        {
            Assert.DoesNotThrow(() => instance.MapConversionBySecond("b", testConverter));
        }

        [Test()]
        public void MapConversionBySecond_WithExistingValueNullConverter_Passes()
        {
            Assert.DoesNotThrow(() => instance.MapConversionBySecond("c", null));
        }

        [Test()]
        public void MapConversionBySecond_WithNewValueRealConverter_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => instance.MapConversionBySecond("x", testConverter));
        }

        [Test()]
        public void MapConversionBySecond_WithNewValueNullConverter_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => instance.MapConversionBySecond("q", null));
        }

        [Test()]
        public void GetConverterByFirst_WithExistingValueDefaultConverter_ReturnsNull()
        {
            Assert.AreEqual(null, instance.GetConverterByFirst("a"));
        }

        [Test()]
        public void GetConverterByFirst_WithExistingValuePassedConverter_ReturnsThatConverter()
        {
            //arrange
            instance.MapConversionByFirst("", testConverter);

            //act/assert
            Assert.AreEqual(testConverter, instance.GetConverterByFirst(""));
        }

        [Test()]
        public void GetConverterByFirst_WithNewValue_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => instance.GetConverterByFirst("x"));
        }

        [Test()]
        public void GetConverterBySecond_WithExistingValueDefaultConverter_ReturnsNull()
        {
            Assert.AreEqual(null, instance.GetConverterBySecond("b"));
        }

        [Test()]
        public void GetConverterBySecond_WithExistingValuePassedConverter_ReturnsThatConverter()
        {
            //arrange
            instance.MapConversionBySecond("text", testConverter);

            //act/assert
            Assert.AreEqual(testConverter, instance.GetConverterBySecond("text"));
        }

        [Test()]
        public void GetConverterBySecond_WithNewValue_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => instance.GetConverterBySecond("q"));
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

        [Test()]
        public void GetCount_Returns3()
        {
            Assert.AreEqual(3, instance.Count);
        }

        [Test()]
        public void IndexingByFirst_ReturnsCorrectSecond()
        {
            //indexing <string,string> is ambiguous
            //arrange
            OneToOneConversionMap<string, int> alphaIndices = new OneToOneConversionMap<string, int>();
            alphaIndices.Add("a", 1);
            alphaIndices.Add("b", 2);

            //act/assert
            Assert.AreEqual(2, alphaIndices["b"]);
        }

        [Test()]
        public void IndexingBySecond_ReturnsCorrectFirst()
        {
            //indexing <string,string> is ambiguous
            //arrange
            OneToOneConversionMap<string, int> alphaIndices = new OneToOneConversionMap<string, int>();
            alphaIndices.Add("a", 1);
            alphaIndices.Add("b", 2);

            //act/assert
            Assert.AreEqual("a", alphaIndices[1]);
        }
    }
}