using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Interfaces;

namespace Banking.Implementations
{
    public class Transactions : ITransactionsCollection
    {
        private object _transactionsLocker;

        private List<ITransaction> _transactions;

        public Transactions(ref List<ITransaction> transactions, ref object locker)
        {
            if (transactions is null)
            {
                throw new ArgumentNullException(nameof(transactions));
            }
            if (locker is null)
            {
                throw new ArgumentNullException(nameof(locker));
            }

            _transactions = transactions;
            _transactionsLocker = locker;
        }

        public async Task<List<ITransaction>> GetAsync()
        {
            return await Task.Run(() =>
            {
                lock (_transactionsLocker)
                {
                    return _transactions;
                }
            });
        }
    }
}
