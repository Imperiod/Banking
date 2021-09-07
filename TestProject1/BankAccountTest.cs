using System;
using Xunit;
using Banking.Interfaces;
using Banking.Implementations;

namespace TestBanking
{
    public class BankAccountTest
    {
        [Fact]
        public void Constructor_NullArgument_ArgumentNullException()
        {
            var serialNumber = new BankSerialNumber<string, ulong, ulong>("NBU", 1, 1);
            IBankUser user = new BankUser("Test", "Test");
            ICurrency currencyBase = new Currency("Test");
            PositiveDouble rate = new PositiveDouble(1);
            ICurrencyRate currencyRate = new CurrencyRate(rate);

            IBankCurrency currency = new BankCurrency(currencyBase, "Test", currencyRate, DateTime.Now);
            
            Assert.Throws<ArgumentNullException>(() => 
            new BankAccount<string, ulong, ulong>(null, user, currency, rate));

            Assert.Throws<ArgumentNullException>(() => 
            new BankAccount<string, ulong, ulong>(serialNumber, null, currency, rate));

            Assert.Throws<ArgumentNullException>(() => 
            new BankAccount<string, ulong, ulong>(serialNumber, user, null, rate));

            Assert.Throws<ArgumentNullException>(() =>
            new BankAccount<string, ulong, ulong>(serialNumber, user, currency, null));
        }
    }
}
