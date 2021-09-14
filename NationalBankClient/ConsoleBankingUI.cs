using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Interfaces;
using Banking.Implementations;

namespace NationalBankClient
{
    public class ConsoleBankingUI : IConsoleBankingUI
    {
        INationalBank<string, ulong, ulong> _bank;
        bool _run = true;
        Dictionary<string, string> _availableCommands;

        public ConsoleBankingUI()
        {
            _bank = new NationalBankUkraine();
            _bank.LoadCurrenciesAsync().Wait();
            _availableCommands = new Dictionary<string, string>();
            _availableCommands.Add("0", "0. Exit");
            _availableCommands.Add("1", "1. Register user");
            _availableCommands.Add("2", "2. Register account");
            _availableCommands.Add("3", "3. Put cash money");
            _availableCommands.Add("4", "4. Convert digital money");
            _availableCommands.Add("5", "5. Get my balance");
            _availableCommands.Add("6", "6. Get transactions");
        }

        public async Task Run()
        {
            while (_run)
            {
                PrintCommandsList();
                string command = Console.ReadLine();
                if (_availableCommands.Keys.Contains(command) == false)
                {
                    Console.WriteLine("Wrong command, try again");
                    continue;
                }
                else
                {
                    Console.Clear();
                    switch (command)
                    {
                        case "0":
                            _run = false;
                            break;
                        case "1":
                            await RegisterUserAsync();
                            break;
                        case "2":
                            await RegisterAccountAsync();
                            break;
                        case "3":
                            await PutCashMoneyAsync();
                            break;
                        case "4":
                            await ConvertDigitalMoneyAsync();
                            break;
                        case "5":
                            await PrintMyBalance();
                            break;
                        case "6":
                            await PrintTransactions();
                            break;
                        default:
                            Console.WriteLine("Wrong value, try again");
                            break;
                    }
                }
            }
        }

        void PrintCommandsList()
        {
            Console.WriteLine("Available commands:");
            _availableCommands.Values.ToList().ForEach(f => Console.WriteLine(f));
        }

        async Task RegisterUserAsync()
        {
            Console.Write("Enter login: ");
            string login = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            IBankUser user;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("String was null or empty, try again");
                await RegisterUserAsync();
                return;
            }
            else
            {
                try
                {
                    user = new BankUser(login, password);
                    await _bank.RegisterUserAsync(user);

                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("User was registered\n");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
        }

        async Task RegisterAccountAsync()
        {

            List<IBankCurrency> currencies;
            try
            {
                currencies = await _bank.Currencies.GetAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            currencies.ForEach(f => Console.WriteLine($"{f.ShortName}"));
            Console.WriteLine("Print short name of currency which you want register");

            string inputCurrency = Console.ReadLine();
            if (currencies.FirstOrDefault(f => f.ShortName == inputCurrency) is null)
            {
                Console.WriteLine("Wrong value, try again");
                await RegisterAccountAsync();
                return;
            }
            else
            {
                ICurrency currency = currencies.First(f => f.ShortName == inputCurrency).Currency;
                try
                {
                    await _bank.RegisterAccountAsync(currency);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }

                PrintSucceedMsg("Account was register");
            }
        }

        async Task PutCashMoneyAsync()
        {
            List<IBankAccount<string, ulong, ulong>> accounts;

            try
            {
                accounts = await _bank.GetAccountsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine("Which it will be account?");
            accounts.ForEach(f => Console.WriteLine(f.Currency.ShortName));

            string inputAccount = Console.ReadLine();
            if (accounts.FirstOrDefault(f => f.Currency.ShortName == inputAccount) is null)
            {
                Console.WriteLine("Wrong value, try again");
                await PutCashMoneyAsync();
                return;
            }
            else
            {
                var account = accounts.First(f => f.Currency.ShortName == inputAccount);
                Console.WriteLine("How much money you want put, with banknotes 100? Limit is 1_000");

                while (true)
                {
                    if (uint.TryParse(Console.ReadLine(), out uint result) && result <= 1000)
                    {
                        List<IMoney<dynamic, dynamic, dynamic>> list = 
                            new List<IMoney<dynamic, dynamic, dynamic>>();

                        for (uint m = 0; m < result; m++)
                        {
                            ICurrency currency = account.Currency.Currency;
                            
                            IBankSerialNumber<dynamic, dynamic, dynamic> serialNumber = 
                                new BankSerialNumber<dynamic, dynamic, dynamic>(
                                    "NBU",
                                    1, 
                                    (ulong)(m + 1));
                            
                            IMoney<dynamic, dynamic, dynamic> money = 
                                new Money<dynamic, dynamic, dynamic>(
                                    serialNumber, 
                                    currency, 
                                    new PositiveDouble(100));
                            list.Add(money);
                        }

                        try
                        {
                            await _bank.PutCashAsync(account, list);
                            PrintSucceedMsg("Put was succeed");
                            return;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Wrong value, try again");
                    }
                }
            }
        }

        async Task ConvertDigitalMoneyAsync()
        {
            List<IBankAccount<string, ulong, ulong>> accounts;

            try
            {
                accounts = await _bank.GetAccountsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine("Choose account for send funds: ");
            accounts.ForEach(f => Console.WriteLine($"{f.Amount} {f.Currency.ShortName}"));
            string inputAccount;
            inputAccount = Console.ReadLine();

            if (accounts.FirstOrDefault(f => f.Currency.ShortName == inputAccount) is null)
            {
                Console.WriteLine("Wrong value, try again");
                await ConvertDigitalMoneyAsync();
                return;
            }
            else
            {
                var first = accounts.First(f => f.Currency.ShortName == inputAccount);

                Console.WriteLine("Choose account for recipient funds: ");
                inputAccount = Console.ReadLine();

                if (accounts.FirstOrDefault(f => f.Currency.ShortName == inputAccount) is null)
                {
                    Console.WriteLine("Wrong value, try again");
                    await ConvertDigitalMoneyAsync();
                    return;
                }
                else
                {
                    var second = accounts.First(f => f.Currency.ShortName == inputAccount);

                    Console.WriteLine($"How much you want exchange? From 0,01 up to {double.MaxValue}");
                    PositiveDouble value = null;

                    while (value is null)
                    {
                        try
                        {
                            string inputValue = Console.ReadLine();
                            if (double.TryParse(inputValue, out double result))
                            {
                                value = new PositiveDouble(result);
                            }
                            else
                            {
                                Console.WriteLine("Wrong value, try again");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            return;
                        }
                    }

                    try
                    {
                        await _bank.ExchangeDigitalCurrencyAsync(first, second, value);
                        PrintSucceedMsg("Exchange was succeed");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return;
                    }
                }
            }
        }

        async Task PrintMyBalance()
        {
            List<IBankAccount<string, ulong, ulong>> accounts;
            try
            {
                accounts = await _bank.GetAccountsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            accounts.ForEach(f => Console.WriteLine(f));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        async Task PrintTransactions()
        {
            List<ITransaction> transactions;
            try
            {
                transactions = await _bank.Transactions.GetAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            transactions.ForEach(f => Console.WriteLine(f));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        void PrintSucceedMsg(string msg)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
