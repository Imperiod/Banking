using System;
using Xunit;
using Banking.Interfaces;
using Banking.Implementations;

namespace TestBanking
{
    public class CurrencyTest
    {
        [Fact]
        public void Constructor_EmptyString_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Currency(""));
        }

        [Fact]
        public void Constructor_Null_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Currency(null));
        }

        [Fact]
        public void ToString_EqualStrings()
        {
            ICurrency currency = new Currency("TEST");
            ICurrency anotherCurrency = new Currency("TEST");

            Assert.Equal(currency.ToString(), anotherCurrency.ToString());
        }

        [Fact]
        public void Equals_Null_ArgumentNullException()
        {
            ICurrency currency = new Currency("TEST");
            Assert.Throws<ArgumentNullException>(() => currency.Equals(null));
        }

        [Fact]
        public void Equals_Object_False()
        {
            ICurrency currency = new Currency("TEST");
            Assert.False(currency.Equals(new object()));
        }

        [Fact]
        public void Equals_Currency_True()
        {
            ICurrency currency = new Currency("TEST");
            ICurrency equalCurrency = currency;

            Assert.True(currency.Equals(equalCurrency));
        }

        [Fact]
        public void Equals_Currencies_False()
        {
            ICurrency currency = new Currency("TEST");
            ICurrency otherCurrency = new Currency("OTHER");


            Assert.False(currency.Equals(otherCurrency));
            
        }
    }
}
