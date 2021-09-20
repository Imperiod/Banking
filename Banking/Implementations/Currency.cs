using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Interfaces;

namespace Banking.Implementations
{
    public class Currency: ICurrency
    {
        public string Name { get; init; }

        public Currency(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("String was null or white space", nameof(name));
            }

            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(ICurrency currency)
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
                    return Name.Equals(currency.Name);
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
                if (obj is ICurrency currency)
                {
                    return Equals(currency);
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
