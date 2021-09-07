using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Implementations;

namespace Banking.Interfaces
{
    public interface IBankAccount<TBankCode, TSeriesCode, TSeriesNumber>
    {
        public IBankSerialNumber<TBankCode, TSeriesCode, TSeriesNumber> SerialNumber { get; init; }

        public IBankUser User { get; init; }

        public IBankCurrency Currency { get; init; }

        public PositiveDouble Amount { get; init; }

        public string ToString();

        public bool Equals(IBankAccount<TBankCode, TSeriesCode, TSeriesNumber> account);

        public bool Equals(object obj);
    }
}
