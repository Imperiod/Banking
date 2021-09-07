using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Interfaces
{
    public interface IBankCurrency
    {
        public DateTime Date { get; init; }

        public ICurrency Currency { get; init; }

        public ICurrencyRate CurrencyRate { get; init; }

        public string ShortName { get; init; }

        public bool Equals(IBankCurrency currency);

        public bool Equals(object obj);
    }
}
