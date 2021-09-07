using System;
using Xunit;
using Banking.Interfaces;
using Banking.Implementations;
using System.Collections.Generic;

namespace TestBanking
{
    public class BankCurrenciesTest
    {
        [Fact]
        public void Constructor_NullArgument_ArgumentNullException()
        {
            List<IBankCurrency> list = null;
            object locker = null;

            Assert.Throws<ArgumentNullException>(() => new BankCurrencies(ref list, ref locker));

            locker = new object();

            Assert.Throws<ArgumentNullException>(() => new BankCurrencies(ref list, ref locker));

            list = new List<IBankCurrency>();
            locker = null;

            Assert.Throws<ArgumentNullException>(() => new BankCurrencies(ref list, ref locker));
        }

        [Fact]
        public void Get_EmptyList()
        {
            List<IBankCurrency> list = new List<IBankCurrency>();
            object locker = new object();
            IBankCurrencyCollection currencies = new BankCurrencies(ref list, ref locker);

            Assert.Empty(currencies.GetAsync().Result);
        }
    }
}
