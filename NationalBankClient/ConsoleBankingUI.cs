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
            IBankUser user;

            Console.Write("Enter login: ");
            string login = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

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

                    PrintInfo(new List<string>() { "User was registered" });
                }
                catch (Exception ex)
                {
                    PrintInfo(new List<string>() { ex.Message }, ConsoleColor.DarkRed);
                }
            }
        }

        async Task RegisterAccountAsync()
        {
            List<IBankCurrency> currencies;

            try
            {
                currencies = await _bank.Currencies.GetAsync();
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

                    await _bank.RegisterAccountAsync(currency);

                    PrintInfo(new List<string>() { "Account was register" });
                }
            }
            catch (Exception ex)
            {
                PrintInfo(new List<string>() { ex.Message }, ConsoleColor.DarkRed);
            }
        }

        async Task PutCashMoneyAsync()
        {
            List<IBankAccount<string, ulong, ulong>> accounts;

            try
            {
                accounts = await _bank.GetAccountsAsync();
                if (accounts.Count == 0)
                {
                    PrintInfo(new List<string>() { "No any accounts" }, ConsoleColor.Gray);
                    return;
                }

                Console.WriteLine("Which it will be account?\n");
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
                            var moneyList = new List<IMoney<dynamic, dynamic, dynamic>>();
                            var currency = account.Currency.Currency;

                            for (uint m = 0; m < result; m++)
                            {
                                var serialNumber = new BankSerialNumber<dynamic, dynamic, dynamic>(
                                        "NBU", 1, (ulong)(m + 1));

                                var money = new Money<dynamic, dynamic, dynamic>(
                                        serialNumber, currency, new PositiveDouble(100));

                                moneyList.Add(money);
                            }

                            await _bank.PutCashAsync(account, moneyList);
                            PrintInfo(new List<string>() { "Put was succeed" });
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Wrong value, try again");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PrintInfo(new List<string>() { ex.Message }, ConsoleColor.DarkRed);
            }
        }

        async Task ConvertDigitalMoneyAsync()
        {
            List<IBankAccount<string, ulong, ulong>> accounts;

            try
            {
                accounts = await _bank.GetAccountsAsync();
                if (accounts.Count == 0)
                {
                    PrintInfo(new List<string>() { "No any accounts" }, ConsoleColor.Gray);
                    return;
                }
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
                        PrintInfo(new List<string>() { "Exchange was succeed" });
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
                if (accounts.Count == 0)
                {
                    PrintInfo(new List<string>() { "No any accounts" }, ConsoleColor.Gray);
                }
                else
                {
                    PrintInfo(accounts.Select(s => s.ToString()).ToList(), ConsoleColor.Cyan);
                }
            }
            catch (Exception ex)
            {
                PrintInfo(new List<string>() { ex.Message }, ConsoleColor.DarkRed);
            }
        }

        async Task PrintTransactions()
        {
            List<ITransaction> transactions;
            try
            {
                transactions = await _bank.Transactions.GetAsync();
                if (transactions.Count == 0)
                {
                    PrintInfo(new List<string>() { "No any transactions" }, ConsoleColor.Gray);
                }
                else
                {
                    PrintInfo(transactions.Select(s => s.ToString()).ToList(), ConsoleColor.Magenta);
                }
            }
            catch (Exception ex)
            {
                PrintInfo(new List<string>() { ex.Message }, ConsoleColor.DarkRed);
            }
        }

        void PrintInfo(List<string> msg, ConsoleColor color = default)
        {
            Console.Clear();
            Console.ForegroundColor = color == default ? ConsoleColor.Green : color;
            foreach (var item in msg)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
