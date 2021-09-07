using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Interfaces
{
    public interface IUser
    {
        public string Login { get; init; }

        public string ToString();

        public bool Equals(object obj);
    }
}
