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

        private List<IUserPasswordRule<string>> _rules;

        public string Password 
        { 
            init
            {
                foreach (var rule in _rules)
                {
                    if(rule.True(value) == false)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), rule.ErrorMessage());
                    }
                }
                _password = value;
            }
        }
        public string Login { get; init; }
    
        public BankUser(string login, string password, List<IUserPasswordRule<string>> rules)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentException("String was null or white space");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("String was null or white space");
            }
            if (rules is null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            Login = login;
            Password = password;
            _rules = rules;
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
            return _password.Equals(password);
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
                return Login.Equals(user.Login) && user.EqualsByPassword(_password);
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

        public bool Equals(IUser user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            else
            {
                return Login.Equals(user.Login);
            }
        }
    }
}
