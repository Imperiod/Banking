using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Implementations;

namespace Banking.Interfaces
{
    public interface ICurrencyRate
    {
        public PositiveDouble Rate { get; init; }

        public string ToString();

        public bool Equals(ICurrencyRate rate);

        public bool Equals(object obj);
    }
}
