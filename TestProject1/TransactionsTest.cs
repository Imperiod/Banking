using System;
using Xunit;
using Banking.Interfaces;
using Banking.Implementations;
using System.Collections.Generic;

namespace TestBanking
{
    public class TransactionsTest
    {
        [Fact]
        public void Constructor_NullArgument_ArgumentNullException()
        {
            List<ITransaction> list = null;
            object locker = null;

            Assert.Throws<ArgumentNullException>(() => new Transactions(ref list, ref locker));

            locker = new object();

            Assert.Throws<ArgumentNullException>(() => new Transactions(ref list, ref locker));

            list = new List<ITransaction>();
            locker = null;

            Assert.Throws<ArgumentNullException>(() => new Transactions(ref list, ref locker));
        }

        [Fact]
        public void Get_EmptyList()
        {
            List<ITransaction> list = new List<ITransaction>();
            object locker = new object();
            ITransactionsCollection currencies = new Transactions(ref list, ref locker);

            Assert.Empty(currencies.GetAsync().Result);
        }
    }
}
