using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Interfaces;
using Banking.Implementations;


namespace NationalBankClient
{
    public interface IConsoleBankingUI
    {
        public Task Run();
    }
}
