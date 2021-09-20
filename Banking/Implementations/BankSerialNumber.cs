using System;
using Banking.Interfaces;

namespace Banking.Implementations
{
    public sealed class BankSerialNumber<TBankCode, TSeriesCode, TSeriesNumber> :
        IBankSerialNumber<TBankCode, TSeriesCode, TSeriesNumber>
    {
        private readonly TBankCode _bankCode = default;

        private readonly TSeriesCode _seriesCode = default;

        private readonly TSeriesNumber _seriesNumber = default;


        public TBankCode BankCode
        { 
            get 
            {
                return _bankCode;
            }
            init
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _bankCode = value;
            }
        }
        
        public TSeriesCode SeriesCode 
        { 
            get
            {
                return _seriesCode;
            }
            init
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _seriesCode = value;
            }
        }

        public TSeriesNumber SeriesNumber 
        { 
            get
            {
                return _seriesNumber;
            }
            init
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _seriesNumber = value;
            }
        }

        public BankSerialNumber(TBankCode bankCode, TSeriesCode seriesCode, TSeriesNumber seriesNumber)
        {
            BankCode = bankCode;
            SeriesCode = seriesCode;
            SeriesNumber = seriesNumber;
        }

        public override string ToString()
        {
            return $"[{BankCode} S:{SeriesCode} N:{SeriesNumber}]";
        }

        public bool Equals(IBankSerialNumber<TBankCode, TSeriesCode, TSeriesNumber> serialNumber)
        {
            if (serialNumber is null)
            {
                throw new ArgumentNullException(nameof(serialNumber));
            }
            if (GetHashCode().Equals(serialNumber.GetHashCode()))
            {
                return true;
            }
            else
            {
                return ToString().Equals(serialNumber.ToString());
            }
        }

        public override bool Equals(object serialNumber)
        {
            if (serialNumber is null)
            {
                throw new ArgumentNullException(nameof(serialNumber));
            }
            if (serialNumber is IBankSerialNumber<TBankCode, TSeriesCode, TSeriesNumber> serial)
            {
                return Equals(serial);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
