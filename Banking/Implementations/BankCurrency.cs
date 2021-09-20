using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Interfaces;

namespace Banking.Implementations
{
    public sealed class BankCurrency : IBankCurrency
    {
        public DateTime Date { get; init; }

        public ICurrency Currency { get; init; }

        public ICurrencyRate CurrencyRate { get; init; }

        public string ShortName { get; init; }

        public BankCurrency(ICurrency currency, string shortName, ICurrencyRate rate, DateTime date)
        {
            if (currency is null)
            {
                throw new ArgumentNullException(nameof(currency));
            }
            if (string.IsNullOrWhiteSpace(shortName))
            {
                throw new ArgumentException("String was null or white space", nameof(shortName));
            }
            if (rate is null)
            {
                throw new ArgumentNullException(nameof(rate));
            }

            Currency = currency;
            ShortName = shortName;
            CurrencyRate = rate;
            Date = date;
        }

        public bool Equals(IBankCurrency currency)
        {
            if (currency is null)
            {
                throw new ArgumentNullException(nameof(currency));
            }
            else
            {
                if (GetHashCode().Equals(currency.GetHashCode()))
                {
                    return true;
                }
                else
                {
                    return Date == currency.Date &&
                        Currency.Equals(currency.Currency) &&
                        ShortName == currency.ShortName &&
                        CurrencyRate.Equals(currency.CurrencyRate);
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            if (obj is IBankCurrency currency)
            {
                return Equals(currency);
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
