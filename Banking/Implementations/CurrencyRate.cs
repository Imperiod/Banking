using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Interfaces;

namespace Banking.Implementations
{
    public class CurrencyRate : ICurrencyRate
    {
        public PositiveDouble Rate { get; init; }

        public CurrencyRate(PositiveDouble rate)
        {
            if (rate is null)
            {
                throw new ArgumentNullException(nameof(rate));
            }

            Rate = new PositiveDouble(Math.Round(rate.Value, 4, MidpointRounding.ToZero));
        }

        public override string ToString()
        {
            return Rate.ToString();
        }

        public bool Equals(ICurrencyRate rate)
        {
            if (rate is null)
            {
                throw new ArgumentNullException(nameof(rate));
            }
            else
            {
                if (GetHashCode().Equals(rate.GetHashCode()))
                {
                    return true;
                }
                else
                {
                    return Rate.Equals(rate.Rate);
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            else
            {
                if (obj is ICurrencyRate rate)
                {
                    return Equals(rate);
                }
                else
                {
                    return false;
                }
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
