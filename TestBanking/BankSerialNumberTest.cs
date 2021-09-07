using System;
using Xunit;
using Banking.Interfaces;
using Banking.Implementations;

namespace TestBanking
{
    public class BankSerialNumberTest
    {
        IBankSerialNumber<string, ulong, ulong> serialNumber =
                new BankSerialNumber<string, ulong, ulong>("TEST", 1, 0);

        [Fact]
        public void Constructor_Null_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
            new BankSerialNumber<string, string, string>(null, "1", "1"));

            Assert.Throws<ArgumentNullException>(() => 
            new BankSerialNumber<string, string, string>("NBU", null, "1"));

            Assert.Throws<ArgumentNullException>(() => 
            new BankSerialNumber<string, string, string>("NBU", "1", null));

            Assert.Throws<ArgumentNullException>(() => 
            new BankSerialNumber<string, string, string>(null, null, null));
        }

        [Fact]
        public void Equals_Null_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => serialNumber.Equals(null));
        }

        [Fact]
        public void Equals_Object_False()
        {
            Assert.False(serialNumber.Equals(new object()));
        }

        [Fact]
        public void Equals_AnotherType_False()
        {
            IBankSerialNumber<string, string, string> otherTypeSerialNumber = 
                new BankSerialNumber<string, string, string>("TEST", "TEST", "TEST");

            Assert.False(serialNumber.Equals(otherTypeSerialNumber));
        }

        [Fact]
        public void Equals_BankSerialNumber_True()
        {
            IBankSerialNumber<string, ulong, ulong> anotherSerialNumber =
                new BankSerialNumber<string, ulong, ulong>("TEST", 1, 0);

            Assert.True(serialNumber.Equals(anotherSerialNumber));
        }
    }
}
