using Banking.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Interfaces
{
    public interface IMoney<TBankCode, TSeriesCode, TSeriesNumber>
    {
        public IBankSerialNumber<TBankCode, TSeriesCode, TSeriesNumber> SerialNumber { get; init; }
        public ICurrency Currency { get; init; }
        public PositiveDouble Nominal { get; init; }

        public string ToString();

        public bool EqualsByCurrency(IMoney<TBankCode, TSeriesCode, TSeriesNumber> money);

        public bool EqualsBySerialNumbers(IMoney<TBankCode, TSeriesCode, TSeriesNumber> money);
    }
}
