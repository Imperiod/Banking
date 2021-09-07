using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Banking.Interfaces;

namespace Banking.Implementations
{
    public sealed class BankCurrencies : IBankCurrencyCollection
    {
        private object _currenciesLocker;

        private List<IBankCurrency> _currencies;

        public BankCurrencies(ref List<IBankCurrency> currencies, ref object locker)
        {
            if (currencies is null)
            {
                throw new ArgumentNullException(nameof(currencies));
            }
            if (locker is null)
            {
                throw new ArgumentNullException(nameof(locker));
            }

            _currenciesLocker = locker;
            _currencies = currencies;
        }


        public async Task<List<IBankCurrency>> GetAsync()
        {
            return await Task.Run(() =>
            {
                lock (_currenciesLocker)
                {
                    return _currencies;
                }
            });
        }
    }
}
