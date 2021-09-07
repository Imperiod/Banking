using System;
using Xunit;
using Banking.Interfaces;
using Banking.Implementations;

namespace TestBanking
{
    public class BankCurrencyTest
    {
        ICurrency currency;
        PositiveDouble rate;
        ICurrencyRate currencyRate;
        string shortName;
        DateTime dateTime;

        public BankCurrencyTest()
        {
            currency = new Currency("TEST");
            rate = new PositiveDouble(1);
            currencyRate = new CurrencyRate(rate);
            shortName = "TEST";
            dateTime = DateTime.Now;
        }

        [Fact]
        public void Constructor_EmptyString_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new BankCurrency(currency, "", currencyRate, dateTime));

            Assert.Throws<ArgumentException>(() => new BankCurrency(currency, null, currencyRate, dateTime));
            Assert.Throws<ArgumentException>(() => new BankCurrency(currency, null, null, dateTime));
        }

        [Fact]
        public void Constructor_Null_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new BankCurrency(null, shortName, currencyRate, dateTime));
            Assert.Throws<ArgumentNullException>(() => new BankCurrency(currency, shortName, null, dateTime));
            Assert.Throws<ArgumentNullException>(() => new BankCurrency(null, null, currencyRate, dateTime));
            Assert.Throws<ArgumentNullException>(() => new BankCurrency(null, shortName, null, dateTime));
            Assert.Throws<ArgumentNullException>(() => new BankCurrency(null, null, null, dateTime));
        }

        [Fact]
        public void Equals_Object_False()
        {
            IBankCurrency bankCurrency = new BankCurrency(currency, shortName, currencyRate, dateTime);
            Assert.False(bankCurrency.Equals(new object()));
        }

        [Fact]
        public void Equals_Currency_True()
        {
            IBankCurrency bankCurrency = new BankCurrency(currency, shortName, currencyRate, dateTime);
            IBankCurrency equalCurrency = bankCurrency;

            Assert.True(bankCurrency.Equals(equalCurrency));
        }

        [Fact]
        public void Equals_Currencies_False()
        {
            IBankCurrency bankCurrency = new BankCurrency(currency, shortName, currencyRate, dateTime);

            ICurrency otherCurrency = new Currency("OTHER");
            PositiveDouble otherRate = new PositiveDouble(2);
            ICurrencyRate otherCurrencyRate = new CurrencyRate(otherRate);
            string otherShortName = "OTHER";
            DateTime otherDateTime = dateTime.AddMilliseconds(1);

            IBankCurrency[] currencies = new BankCurrency[4];
            currencies[0] = new BankCurrency(otherCurrency, shortName, currencyRate, dateTime);
            currencies[1] = new BankCurrency(currency, otherShortName, currencyRate, dateTime);
            currencies[2] = new BankCurrency(currency, shortName, otherCurrencyRate, dateTime);
            currencies[3] = new BankCurrency(currency, shortName, currencyRate, otherDateTime);

            for (int i = 0; i < currencies.Length; i++)
            {
                Assert.False(bankCurrency.Equals(currencies[i]));
            }
        }
    }
}
