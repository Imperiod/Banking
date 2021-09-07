using System;
using Xunit;
using Banking.Interfaces;
using Banking.Implementations;

namespace TestBanking
{
    public class TransactionTest
    {
        [Fact]
        public void Constructor_NullArgument_ArgumentException()
        {            
            Assert.Throws<ArgumentException>(() => 
            new Transaction(DateTime.Now, null));
        }

        [Fact]
        public void Constructor_EmptyString_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            new Transaction(DateTime.Now, ""));
        }

        [Fact]
        public void ToString_EqualStrings()
        {
            ITransaction transaction =
            new Transaction(DateTime.Now, "TEST");

            Assert.Equal(transaction.ToString(), transaction.ToString());
        }
    }
}
