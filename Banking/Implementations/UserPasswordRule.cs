using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Interfaces;

namespace Banking.Implementations
{
    public class BankUserPasswordLengthRule : IUserPasswordRule<string>
    {
        public string ErrorMessage()
        {
            return "Password length must be more then 4 chars";
        }

        public bool True(string password)
        {
            return password.Length > 4;
        }
    }
}
