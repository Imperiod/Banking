using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Interfaces;

namespace Banking.Implementations
{
    public sealed class User : IUser
    {
        public string Login { get; init; }

        public User(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentException("String was null or white space");
            }
            Login = login;
        }

        public override string ToString()
        {
            return Login;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            if (obj is IUser user)
            {
                return Login == user.Login;
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
