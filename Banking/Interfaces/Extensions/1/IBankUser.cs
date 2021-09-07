using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Interfaces
{
    public interface IBankUser : IUser
    {
        public string Password { init; }

        public bool Equals(IBankUser user);

        public bool EqualsByPassword(string password);

        public new bool Equals(object obj);
    }
}
