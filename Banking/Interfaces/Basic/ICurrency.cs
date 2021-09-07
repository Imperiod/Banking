using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Interfaces
{
    public interface ICurrency
    {
        public string Name { get; init; }

        public bool Equals(ICurrency currency);

        public bool Equals(object obj);

        public string ToString();
    }
}
