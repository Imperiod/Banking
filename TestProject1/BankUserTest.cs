using System;
using Xunit;
using Banking.Interfaces;
using Banking.Implementations;

namespace TestBanking
{
    public class BankUserTest
    {
        [Fact]
        public void Constructor_NullArgument_ArgumentException()
        {            
            Assert.Throws<ArgumentException>(() => new BankUser(null, "Test"));
            Assert.Throws<ArgumentException>(() => new BankUser("Test", null));
            Assert.Throws<ArgumentException>(() => new BankUser(null, null));
        }

        [Fact]
        public void Constructor_EmptyString_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new BankUser("", "Test"));
            Assert.Throws<ArgumentException>(() => new BankUser("Test", ""));
            Assert.Throws<ArgumentException>(() => new BankUser("", ""));
            Assert.Throws<ArgumentException>(() => new BankUser(" ", "Test"));
            Assert.Throws<ArgumentException>(() => new BankUser("Test", " "));
            Assert.Throws<ArgumentException>(() => new BankUser(" ", " "));
        }

        [Fact]
        public void Constructor_UncorrectPassword_ArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new BankUser("Test", "T"));
        }
    }
}
