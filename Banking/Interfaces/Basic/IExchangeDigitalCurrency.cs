using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Implementations;

namespace Banking.Interfaces
{
    public interface IExchangeDigitalCurrency
    {
        public Task ExchangeDigitalCurrencyAsync(IBankAccount<string, ulong, ulong> from, IBankAccount<string, ulong, ulong> to, PositiveDouble amountFrom);
    }
}
