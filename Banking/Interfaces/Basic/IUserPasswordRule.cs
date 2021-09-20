using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Interfaces
{
    public interface IUserPasswordRule<TPassword>
    {
        public bool True(TPassword password);

        public string ErrorMessage();
    }
}
