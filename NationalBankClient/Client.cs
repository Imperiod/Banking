using Banking.Implementations;
using Banking.Interfaces;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Threading;


namespace NationalBankClient
{
    public class Client
    {
        static ConsoleBankingUI ui;
        static async Task Main(string[] args)
        {
            ui = new ConsoleBankingUI();
            await ui.Run();
        }
    }
}
