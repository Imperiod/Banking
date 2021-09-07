using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Interfaces;

namespace Banking.Implementations
{
    public class Money<TBankCode, TSeriesCode, TSeriesNumber> : IMoney<TBankCode, TSeriesCode, TSeriesNumber>
    {
        public IBankSerialNumber<TBankCode, TSeriesCode, TSeriesNumber> SerialNumber { get; init; }
        public ICurrency Currency { get; init; }
        public PositiveDouble Nominal { get; init; }

        public Money(IBankSerialNumber<TBankCode, TSeriesCode, TSeriesNumber> serialNumber, ICurrency currency, PositiveDouble nominal)
        {
            if (serialNumber is null)
            {
                throw new ArgumentNullException(nameof(serialNumber));
            }
            if (currency is null)
            {
                throw new ArgumentNullException(nameof(currency));
            }
            if (nominal.Value == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nominal), "Nominal must be greater 0");
            }

            SerialNumber = serialNumber;
            Currency = currency;
            Nominal = nominal;
        }


        public override string ToString()
        {
            return $"{Nominal} {Currency} {SerialNumber}";
        }

        public bool EqualsByCurrency(IMoney<TBankCode, TSeriesCode, TSeriesNumber> money)
        {
            if (money is null)
            {
                throw new ArgumentNullException(nameof(money));
            }

            return Nominal == money.Nominal && Currency.Name == money.Currency.Name;
        }

        public bool EqualsBySerialNumbers(IMoney<TBankCode, TSeriesCode, TSeriesNumber> money)
        {
            if (money is null)
            {
                throw new ArgumentNullException(nameof(money));
            }

            return SerialNumber.Equals(money.SerialNumber);
        }
    }
}
