using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Interfaces
{
    public interface ITransaction
    {
        public DateTime Date { get; init; }

        public string Description { get; init; }

        public string ToString();

        public bool Equals(ITransaction transaction);

        public bool Equals(object obj);
    }
}
