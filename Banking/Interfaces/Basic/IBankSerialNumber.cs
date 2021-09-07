using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Interfaces
{
    public interface IBankSerialNumber<TBankCode, TSeriesCode, TSeriesNumber>
    {
        public TBankCode BankCode { get; init; }

        public TSeriesCode SeriesCode { get; init; }

        public TSeriesNumber SeriesNumber { get; init; }

        public string ToString();

        public bool Equals(IBankSerialNumber<TBankCode, TSeriesCode, TSeriesNumber> serialNumber);

        public bool Equals(object serialNumber);
    }
}
