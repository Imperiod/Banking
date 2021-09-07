using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Implementations;

namespace Banking.Interfaces
{
    public interface IExchangeCashCurrency
    {
        public Task<List<IMoney<string, ulong, ulong>>> ExchangeCashCurrencyAsync(IMoney<string, ulong, ulong> from, ICurrency to);
    }
}
