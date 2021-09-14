using System;
using Xunit;
using Banking.Interfaces;
using Banking.Implementations;

namespace TestBanking
{
    public class PositiveDoubleTest
    {
        PositiveDouble positiveDouble = new PositiveDouble(1);

        [Fact]
        public void Constructor_NegativeValue_ArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PositiveDouble(-0.00000000001));
        }

        [Theory]
        [InlineData(1000)]
        public void ToString_Double_EqualStrings(double value)
        {
            Assert.Equal(value.ToString(), new PositiveDouble(value).ToString());
        }

        [Fact]
        public void Equals_Null_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => positiveDouble.Equals(null));
        }

        [Fact]
        public void Equals_AnotherType_False()
        {
            double x = 1;

            Assert.False(positiveDouble.Equals(x));
        }
    }
}
