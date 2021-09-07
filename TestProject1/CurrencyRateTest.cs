using System;
using Xunit;
using Banking.Interfaces;
using Banking.Implementations;

namespace TestBanking
{
    public class CurrencyRateTest
    {
        PositiveDouble value = new PositiveDouble(1);

        [Fact]
        public void Constructor_Null_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CurrencyRate(null));
        }

        [Fact]
        public void ToString_EqualStrings()
        {
            ICurrencyRate rate = new CurrencyRate(value);
            ICurrencyRate anotherRate = new CurrencyRate(value);

            Assert.Equal(rate.ToString(), anotherRate.ToString());
        }

        [Fact]
        public void Equals_Object_False()
        {
            ICurrencyRate rate = new CurrencyRate(value);

            Assert.False(rate.Equals(new object()));
        }

        [Fact]
        public void Equals_CurrencyRate_True()
        {
            ICurrencyRate rate = new CurrencyRate(value);
            ICurrencyRate anotherRate = new CurrencyRate(value);

            Assert.Equal(rate, anotherRate);
        }

        [Fact]
        public void Equals_CurrencyRate_False()
        {
            ICurrencyRate rate = new CurrencyRate(value);
            ICurrencyRate anotherRate = new CurrencyRate(new PositiveDouble(value.Value + 1));

            Assert.NotEqual(rate, anotherRate);
        }
    }
}
