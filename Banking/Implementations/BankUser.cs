using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Interfaces;

namespace Banking.Implementations
{
    public sealed class BankUser : IBankUser
    {
        private string _password;
        public string Password 
        { 
            init
            {
                if (value.Length < 4)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Password length must be more then 4 chars");
                }
                _password = value;
            } 
        }
        public string Login { get; init; }
    
        public BankUser(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentException("String was null or white space");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("String was null or white space");
            }

            Login = login;
            Password = password;
        }

        public override string ToString()
        {
            return Login;
        }

        public bool EqualsByPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("String was null or white space", nameof(password));
            }
            return _password == password;
        }

        public bool Equals(IBankUser user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (GetHashCode() == user.GetHashCode())
            {
                return true;
            }
            else
            {
                return Login == user.Login && user.EqualsByPassword(_password);
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
                if (obj is IBankUser user)
                {
                    return Equals(user);
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
