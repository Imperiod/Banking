using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Interfaces;

namespace Banking.Implementations
{
    public sealed partial class NationalBankUkraine : INationalBank<string, ulong, ulong>
    {
        /// <summary>
        /// Provides access to methods of currency list<br></br>
        /// </summary>
        public IBankCurrencyCollection Currencies { get; private set; }

        /// <summary>
        /// Provides access to methods of transactions list<br></br>
        /// </summary>
        public ITransactionsCollection Transactions { get; private set; }

        /// <summary>
        /// Provides a basic national currency that is necessary for currency exchange<br></br>
        /// </summary>
        /// <returns>The task of providing the basic national currency</returns>
        public async Task<ICurrency> GetBaseNationalCurrencyAsync()
        {
            return await Task.Run(() =>
            {
                return new Currency(NATIONAL_CURRENCY_FULL);
            });
        }

        /// <summary>
        /// Provides a full national currency that is necessary for currency exchange<br></br>
        /// <br></br>
        /// <b>Notes:</b><br></br>
        /// <i>
        /// Can have more deep exceptions from used methods<br />
        /// </i>
        /// <br></br>
        /// <b>Uses methods:</b><br></br>
        /// <i>
        /// <see cref="LoadCurrenciesAsync"/><br />
        /// <see cref="BankCurrencies.GetAsync"/><br />
        /// </i>
        /// </summary>
        /// <returns>The task of providing the full national currency</returns>
        public async Task<IBankCurrency> GetFullNationalCurrencyAsync()
        {
            await LoadCurrenciesAsync();
            var list = await Currencies.GetAsync();
            IBankCurrency currency;
            lock (_currencyLocker)
            {
                currency = list.First(f => f.Currency.Name == NATIONAL_CURRENCY_FULL);
            }

            return currency;
        }


        /// <summary>
        /// Updating list of currency, including necessary date<br></br>
        /// <br />
        /// <b>Notes:</b><br></br>
        /// <i>
        /// Will be update the list of currencies to date if the date will be null<br></br>
        /// List of currency will be empty if date will be isn't correct<br></br>
        /// Can have more deep exceptions from used methods<br />
        /// </i>
        /// <br></br>
        /// <b>Uses methods:</b><br></br>
        /// <i>
        /// <see cref="GetBaseNationalCurrencyAsync"/><br />
        /// <see cref="GetCurrencies"/><br />
        /// <see cref="AddCurrency"/><br />
        /// </i>
        /// </summary>
        /// <param name="dateTime">Necessary date</param>
        /// <returns>The task of update of list of currency</returns>
        public async Task LoadCurrenciesAsync(DateTime? dateTime = null)
        {
            string dateParam;
            if (dateTime is null)
            {
                dateTime = DateTime.Now;
            }

            dateParam = $"&date={dateTime.Value.Year}" +
                    (dateTime.Value.Month < 10 ? "0" + dateTime.Value.Month : dateTime.Value.Month) +
                    (dateTime.Value.Day < 10 ? "0" + dateTime.Value.Day : dateTime.Value.Day);

            var availableCurrencies = await Currencies.GetAsync();
            if (availableCurrencies.Count > 0)
            {
                if (availableCurrencies.Exists(e => e.Date.Date == dateTime.Value.Date))
                {
                    return;
                }
            }

            Uri uri = new Uri($@"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchangenew?json{dateParam}");

            var newCurrencies = await GetCurrencies(uri);
            newCurrencies.ForEach(f => AddCurrency(f));

            ICurrency currency = await GetBaseNationalCurrencyAsync();
            PositiveDouble rate = new PositiveDouble(1);
            ICurrencyRate currencyRate = new CurrencyRate(rate);

            var uniqCurrList = newCurrencies
                .Where(w => w.Currency.Equals(currency) == false)
                .GroupBy(g => g.Date.Date);

            foreach (var item in uniqCurrList)
            {
                IBankCurrency nationalCurrency = new BankCurrency(
                        currency, NATIONAL_CURRENCY_SHORT, currencyRate,
                        item.Key);

                AddCurrency(nationalCurrency);
            }
        }


        /// <summary>
        /// Exchanging funds between accounts of user, with currency rate to date<br />
        /// <br />
        /// <b>Notes:</b><br />
        /// <i>
        /// Funds which was converted, will be rounded up to 2 decimal signs,
        /// closer to zero, for avoid lose bank owner<br />
        /// Can have more deep exceptions from used methods<br />
        /// </i>
        /// <br />
        /// <b>Required conditions for success:</b><br />
        /// <i>
        /// User must be authorized before call this method<br />
        /// Both account must be in list of accounts of bank owner<br />
        /// Also, both accounts should belong to the authorized user<br/>
        /// Amount must be greater 0<br />
        /// Accounts can't be equal<br />
        /// </i>
        /// <br />
        /// <b>Possible exceptions:</b><br />
        /// <i>
        /// <see cref="ArgumentNullException"/><br />
        /// <see cref="UnauthorizedAccessException"/><br />
        /// <see cref="ArgumentOutOfRangeException"/><br />
        /// <see cref="ArgumentException"/><br />
        /// <see cref="InvalidOperationException"/><br />
        /// </i>
        /// <br />
        /// <b>Uses methods:</b><br />
        /// <i>
        /// <see cref="AccountExistAsync"/><br></br>
        /// <see cref="LoadCurrenciesAsync"/><br />
        /// <see cref="BankCurrencies.GetAsync"/><br />
        /// <see cref="RefreshAccountRateAsync"/><br />
        /// <see cref="GetFullNationalCurrencyAsync"/><br></br>
        /// <see cref="WriteTransaction"/><br />
        /// </i>
        /// </summary>
        /// <param name="from">The account from which funds will be withdraw</param>
        /// <param name="to">The recipient account</param>
        /// <param name="amountFrom">How much funds will be converted</param>
        /// <returns>The task of exchanging digital currency</returns>
        public async Task ExchangeDigitalCurrencyAsync(IBankAccount<string, ulong, ulong> from, IBankAccount<string, ulong, ulong> to, PositiveDouble amountFrom)
        {
            DateTime now = DateTime.Now.Date;

            if (from is null)
            {
                throw new ArgumentNullException(nameof(from));
            }
            if (to is null)
            {
                throw new ArgumentNullException(nameof(to));
            }
            if (_authorizedUser is null)
            {
                throw new UnauthorizedAccessException("You are not authorized");
            }
            if (amountFrom.Value == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amountFrom), "Value is zero");
            }

            if (_accounts.FirstOrDefault(f => f?.User.Equals(_authorizedUser) == true) is null)
            {
                throw new ArgumentException($"User {_authorizedUser} have not any account", nameof(_authorizedUser));
            }

            if (from.SerialNumber.Equals(to.SerialNumber))
            {
                throw new InvalidOperationException("Accounts is equals");
            }

            if (await AccountExistAsync(from) == false)
            {
                throw new ArgumentException("This account is not exist", nameof(from));
            }
            if (await AccountExistAsync(to) == false)
            {
                throw new ArgumentException("This account is not exist", nameof(to));
            }

            await LoadCurrenciesAsync(now);

            var currencies = await Currencies.GetAsync();

            lock (_currencyLocker)
            {
                if (currencies.FirstOrDefault(f => f?.ShortName == from.Currency.ShortName) is null)
                {
                    throw new ArgumentOutOfRangeException(nameof(from), "Currency is not exist");
                }
                if (currencies.FirstOrDefault(f => f?.ShortName == to.Currency.ShortName) is null)
                {
                    throw new ArgumentOutOfRangeException(nameof(to), "Currency is not exist");
                }
            }

            lock (_accountLocker)
            {
                var index = _accounts.IndexOf(_accounts.First(f => f.SerialNumber.Equals(from.SerialNumber)));

                _accounts[index] = new BankAccount<string, ulong, ulong>(
                    from.SerialNumber,
                    from.User,
                    from.Currency,
                    new PositiveDouble(from.Amount.Value - amountFrom.Value));
            }

            double endMoneyValue = default;

            if (from.Currency.Equals(await GetFullNationalCurrencyAsync()))
            {
                await RefreshAccountRateAsync(to);
                endMoneyValue = amountFrom.Value / to.Currency.CurrencyRate.Rate.Value;
            }
            else
            {
                if (to.Currency.Equals(await GetFullNationalCurrencyAsync()))
                {
                    await RefreshAccountRateAsync(from);
                    endMoneyValue = amountFrom.Value * from.Currency.CurrencyRate.Rate.Value;
                }
                else
                {
                    await RefreshAccountRateAsync(to);
                    await RefreshAccountRateAsync(from);
                    endMoneyValue = amountFrom.Value * from.Currency.CurrencyRate.Rate.Value / to.Currency.CurrencyRate.Rate.Value;
                }
            }

            lock (_accountLocker)
            {
                var index = _accounts.IndexOf(_accounts.First(f => f.SerialNumber.Equals(to.SerialNumber)));
                _accounts[index] = new BankAccount<string, ulong, ulong>(
                    to.SerialNumber,
                    to.User,
                    to.Currency,
                    new PositiveDouble(Math.Round(to.Amount.Value + endMoneyValue, 2)));
            }

            WriteTransaction(DateTime.Now,
                $"{_authorizedUser} converted {amountFrom} {from.Currency.ShortName} from {from.SerialNumber} " +
                $"in {Math.Round(endMoneyValue, 2)} {to.Currency.ShortName} to {to.SerialNumber}");
        }


        /// <summary>
        /// Exchanging cash money with currency rate to date<br />
        /// <br />
        /// <b>Notes:</b><br />
        /// <i>
        /// Money which was converted, will be rounded up to 2 decimal signs,
        /// closer to zero, for avoid lose<br />
        /// Can have more deep exceptions from used methods<br />
        /// </i>
        /// <br />
        /// <b>Required conditions for success:</b><br />
        /// <i>
        /// Amount must be greater 0<br />
        /// Name of currency for 'from' and 'to', must be in bank list of currency<br></br>
        /// </i>
        /// <br />
        /// <b>Possible exceptions:</b><br />
        /// <i>
        /// <see cref="ArgumentNullException"/><br />
        /// <see cref="ArgumentOutOfRangeException"/><br />
        /// </i>
        /// <br />
        /// <b>Uses methods:</b><br />
        /// <i>
        /// <see cref="LoadCurrenciesAsync"/><br />
        /// <see cref="BankCurrencies.GetAsync"/><br />
        /// <see cref="GetBanknotesForCurrencyAsync"/><br />
        /// <see cref="GetFullNationalCurrencyAsync"/><br></br>
        /// <see cref="GetNewSeriesNumber"/><br />
        /// <see cref="WriteTransaction"/><br />
        /// </i>
        /// </summary>
        /// <param name="from">Money that will exchange</param>
        /// <param name="to">The currency that will be obtained at the end</param>
        /// <returns>The task of exchanging cash currency</returns>
        public async Task<List<IMoney<string, ulong, ulong>>> ExchangeCashCurrencyAsync(IMoney<string, ulong, ulong> from, ICurrency to)
        {
            DateTime now = DateTime.Now.Date;

            if (from is null)
            {
                throw new ArgumentNullException(nameof(from));
            }
            if (to is null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            await LoadCurrenciesAsync(now);

            var currencies = await Currencies.GetAsync();

            IBankCurrency current = currencies.FirstOrDefault(f => f.Currency.Equals(from.Currency));
            IBankCurrency next = currencies.FirstOrDefault(f => f.Currency.Equals(to));

            if (current is null)
            {
                throw new ArgumentOutOfRangeException(nameof(from), "Currency is not exist");
            }
            if (next is null)
            {
                throw new ArgumentOutOfRangeException(nameof(to), "Currency is not exist");
            }

            var nominals = await GetBanknotesForCurrencyAsync(to);
            double endValue = default;
            List<IMoney<string, ulong, ulong>> money = new List<IMoney<string, ulong, ulong>>();


            if (current.Equals(await GetFullNationalCurrencyAsync()))
            {
                if (next.Equals(await GetFullNationalCurrencyAsync()))
                {
                    return new List<IMoney<string, ulong, ulong>>() { from };
                }
                else
                {
                    endValue = Math.Round(from.Nominal.Value / next.CurrencyRate.Rate.Value, 2);
                }
            }
            else
            {
                if (next.Equals(await GetFullNationalCurrencyAsync()))
                {
                    endValue = Math.Round(from.Nominal.Value / current.CurrencyRate.Rate.Value, 2);
                }
                else
                {
                    endValue = Math.Round(from.Nominal.Value * current.CurrencyRate.Rate.Value / next.CurrencyRate.Rate.Value, 2);
                }
            }

            while (endValue > 0)
            {
                foreach (var item in nominals)
                {
                    if (endValue - item.Value > 0)
                    {

                        endValue -= item.Value;
                        var serialNum = await GetNewSeriesNumber();
                        lock (_moneyLocker)
                        {
                            money.Add(new Money<string, ulong, ulong>(serialNum, to, item));
                        }

                    }
                }
            }

            WriteTransaction(DateTime.Now,
                $"{from.Nominal.Value} {from.Currency.Name} was exchange" +
                $" in {to.Name} {Math.Round(money.Sum(s => s.Nominal.Value), 2)}");

            return money;
        }


        /// <summary>
        /// Conducts user authorization<br></br>
        /// <br></br>
        /// <b>Possible exceptions:</b><br></br>
        /// <i>
        /// <see cref="ArgumentNullException"/><br />
        /// </i>
        /// </summary>
        /// <param name="user">Bank user</param>
        /// <returns>The task of holding user authorization</returns>
        public async Task AuthorizeAsync(IBankUser user)
        {
            await Task.Run(() =>
            {
                if (user is null)
                {
                    throw new ArgumentNullException(nameof(user));
                }
                _authorizedUser = user;
            });
        }

        public async Task<IBankUser> GetAuthorizedUser()
        {
            return await Task.Run(() =>
            {
                return _authorizedUser;
            });
        }

        /// <summary>
        /// Just conducts authorization of this user<br></br>
        /// <br></br>
        /// <b>Notes:</b><br />
        /// <i>
        /// Can have more deep exceptions from used methods<br />
        /// </i>
        /// <br />
        /// <b>Required conditions for success:</b><br></br>
        /// <i>
        /// Login must be correct, without white space<br></br>
        /// Password min length 4 chars without white space<br />
        /// </i>
        /// <br />
        /// <b>Uses methods:</b><br />
        /// <i>
        /// <see cref="AuthorizeAsync"/><br />
        /// </i>
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>The task of registration of the user</returns>
        public async Task RegisterUserAsync(IBankUser user)
        {
            await AuthorizeAsync(user);
        }

        /// <summary>
        /// Just register new account for authorized user<br />
        /// <br />
        /// <b>Notes:</b><br />
        /// <i>
        /// Can have more deep exceptions from used methods<br />
        /// </i>
        /// <br />
        /// <b>Required conditions for success:</b><br />
        /// <i>
        /// User must be authorized before call this method<br />
        /// Currency must exist in currency list of bank<br />
        /// </i>
        /// <br />
        /// <b>Possible exceptions:</b><br />
        /// <i>
        /// <see cref="ArgumentNullException"/><br />
        /// <see cref="UnauthorizedAccessException"/><br />
        /// <see cref="ArgumentOutOfRangeException"/><br />
        /// </i>
        /// <br />
        /// <b>Uses methods:</b><br />
        /// <i>
        /// <see cref="LoadCurrenciesAsync"/><br />
        /// <see cref="BankCurrencies.GetAsync"/><br />
        /// <see cref="GetNewSeriesNumber"/><br />
        /// </i>
        /// </summary>
        /// <param name="currency">Currency</param>
        /// <returns>The task of registration of the account</returns>
        public async Task RegisterAccountAsync(ICurrency currency)
        {
            if (currency is null)
            {
                throw new ArgumentNullException(nameof(currency));
            }
            if (_authorizedUser is null)
            {
                throw new UnauthorizedAccessException("You are not authorized");
            }

            DateTime now = DateTime.Now;

            await LoadCurrenciesAsync(now);

            var currencies = await Currencies.GetAsync();

            IBankCurrency newCurrency;
            lock (_currencyLocker)
            {
                newCurrency = currencies
                .FirstOrDefault(f => f.Currency.Equals(currency) &&
                                     f.Date.Date == now.Date);
            }

            if (newCurrency is null)
            {
                throw new ArgumentOutOfRangeException(nameof(currency));
            }

            var serialNumber = await GetNewSeriesNumber();

            lock (_accountLocker)
            {
                if (_accounts.Any(a => a.Currency.Equals(currency)))
                {
                    throw new ArgumentOutOfRangeException(nameof(currency), $"Account with {currency.Name} currency already exist");
                }

                _accounts.Add(new BankAccount<string, ulong, ulong>(serialNumber, _authorizedUser, newCurrency, new PositiveDouble(0)));
            }

        }

        /// <summary>
        /// Just return list of bank accounts for authorized user<br></br>
        /// <br></br>
        /// <b>Possible exceptions:</b><br></br>
        /// <i>
        /// <see cref="UnauthorizedAccessException"/>
        /// </i>
        /// </summary>
        /// <returns>The task of getting list of bank accounts</returns>
        public async Task<List<IBankAccount<string, ulong, ulong>>> GetAccountsAsync()
        {
            return await Task.Run(new Func<List<IBankAccount<string, ulong, ulong>>>(() =>
            {
                if (_authorizedUser is null)
                {
                    throw new UnauthorizedAccessException("You are not authorized");
                }

                List<IBankAccount<string, ulong, ulong>> list;
                lock (_accountLocker)
                {
                    list = _accounts.Where(w => w?.User.Equals(_authorizedUser) == true).ToList();
                }

                return list;
            }));
        }

        /// <summary>
        /// Do transfer money from account of authorized user to another account<br></br>
        /// <br />
        /// <b>Notes:</b><br />
        /// <i>
        /// 
        /// Can have more deep exceptions from used methods<br />
        /// </i>
        /// <br />
        /// <b>Required conditions for success:</b><br />
        /// <i>
        /// User must be authorized before call this method<br />
        /// Both account must be in list of accounts of their bank owner<br />
        /// From account should belong to the authorized user<br/>
        /// Amount must be greater 0<br />
        /// Both currency of accounts must be equal<br />
        /// </i>
        /// <br />
        /// <b>Possible exceptions:</b><br />
        /// <i>
        /// <see cref="ArgumentNullException"/><br />
        /// <see cref="UnauthorizedAccessException"/><br />
        /// <see cref="ArgumentOutOfRangeException"/><br />
        /// <see cref="ArgumentException"/><br />
        /// </i>
        /// <br />
        /// <b>Uses methods:</b><br />
        /// <i>
        /// <see cref="AccountExistAsync"/><br />
        /// <see cref="PutAsync"/><br />
        /// <see cref="WriteTransaction"/><br />
        /// </i>
        /// </summary>
        /// <typeparam name="TBankCodeTo">Type of bank code for to argument</typeparam>
        /// <typeparam name="TSeriesCodeTo">Type of series code for to argument</typeparam>
        /// <typeparam name="TSeriesNumberTo">Type of series number  for to argument</typeparam>
        /// <param name="from">The account from which funds will be withdraw</param>
        /// <param name="bank">The bank of recipient account</param>
        /// <param name="to">The recipient account</param>
        /// <param name="amount">How much funds will be converted</param>
        /// <returns>The task of send funds from one account to another account</returns>
        public async Task SendAsync<TBankCodeTo, TSeriesCodeTo, TSeriesNumberTo>(IBankAccount<string, ulong, ulong> from, INationalBank<TBankCodeTo, TSeriesCodeTo, TSeriesNumberTo> bank, IBankAccount<TBankCodeTo, TSeriesCodeTo, TSeriesNumberTo> to, PositiveDouble amount)
        {
            if (from is null)
            {
                throw new ArgumentNullException(nameof(from));
            }
            if (to is null)
            {
                throw new ArgumentNullException(nameof(to));
            }
            if (bank is null)
            {
                throw new ArgumentNullException(nameof(bank));
            }
            if (amount is null)
            {
                throw new ArgumentNullException(nameof(amount));
            }
            if (amount.Value == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Value is zero");
            }
            if (_authorizedUser is null || from.User.Equals(_authorizedUser) == false)
            {
                throw new UnauthorizedAccessException("You are not authorized");
            }
            if (await AccountExistAsync(from) == false)
            {
                throw new ArgumentOutOfRangeException(nameof(from), "Account is not exist");
            }
            if (await bank.AccountExistAsync(to) == false)
            {
                throw new ArgumentOutOfRangeException(nameof(to), "Account is not exist");
            }
            if (from.Currency.Equals(to.Currency) == false)
            {
                throw new ArgumentException("Different currencies");
            }
            if (from.Equals(to))
            {
                throw new ArgumentException("Accounts is equals");
            }

            await bank.PutAsync<string, ulong, ulong>(to, this, from, amount);

            lock (_accountLocker)
            {
                var index = _accounts.IndexOf(_accounts.First(f => f.SerialNumber.Equals(from.SerialNumber)));
                _accounts[index] = new BankAccount<string, ulong, ulong>(
                    from.SerialNumber,
                    from.User,
                    from.Currency,
                    new PositiveDouble(from.Amount.Value - amount.Value));
            }

            WriteTransaction(DateTime.Now,
                $"{_authorizedUser} send {amount} {from.Currency.ShortName}" +
                $" from {from.SerialNumber} to account of {to} of {bank.GetType().Name}");
        }

        /// <summary>
        /// Put cash money to account<br></br>
        /// <br />
        /// <b>Notes:</b><br />
        /// <i>
        /// Can have more deep exceptions from used methods<br />
        /// </i>
        /// <br />
        /// <b>Required conditions for success:</b><br />
        /// <i>
        /// Account must be in list of accounts of bank owner<br />
        /// Currency of money and account must be equals<br></br>
        /// Amount must be greater 0<br />
        /// </i>
        /// <br />
        /// <b>Possible exceptions:</b><br />
        /// <i>
        /// <see cref="ArgumentNullException"/><br />
        /// <see cref="ArgumentException"/><br />
        /// </i>
        /// <br />
        /// <b>Uses methods:</b><br />
        /// <i>
        /// <see cref="AccountExistAsync"/><br></br>
        /// <see cref="WriteTransaction"/><br />
        /// </i>
        /// </summary>
        /// <typeparam name="TBankCodeMoney">Type of bank code money</typeparam>
        /// <typeparam name="TSeriesCodeMoney">Type of series code money</typeparam>
        /// <typeparam name="TSeriesNumberMoney">Type of series number money</typeparam>
        /// <param name="account">Recipient account</param>
        /// <param name="money">Cash money</param>
        /// <returns>The task of put cash money to account</returns>
        public async Task PutCashAsync<TBankCodeMoney, TSeriesCodeMoney, TSeriesNumberMoney>(IBankAccount<string, ulong, ulong> account, List<IMoney<TBankCodeMoney, TSeriesCodeMoney, TSeriesNumberMoney>> money)
        {
            if (account is null)
            {
                throw new ArgumentNullException(nameof(account));
            }
            if (money is null)
            {
                throw new ArgumentNullException(nameof(money));
            }
            if (money.Count == 0)
            {
                throw new ArgumentException("List is empty", nameof(money));
            }
            if (await AccountExistAsync(account) == false)
            {
                throw new ArgumentException("Account isn't exist", nameof(account));
            }

            foreach (var item in money)
            {
                if (account.Currency.Currency.Equals(item.Currency) == false)
                {
                    throw new ArgumentException("Different currency", nameof(money));
                }
            }

            var sum = money.Sum(s => s.Nominal.Value);

            lock (_accountLocker)
            {
                var index = _accounts.IndexOf(_accounts.First(f => f.SerialNumber.Equals(account.SerialNumber)));
                var preAccount = _accounts[index];
                preAccount.Amount.Value += sum;

                _accounts[index] = new BankAccount<string, ulong, ulong>(
                    preAccount.SerialNumber,
                    preAccount.User,
                    preAccount.Currency,
                    preAccount.Amount);
            }

            var castMoney = money.Select(s => 
            new Money<dynamic, dynamic, dynamic>(
                new BankSerialNumber<dynamic, dynamic, dynamic>(
                    s.SerialNumber.BankCode, s.SerialNumber.SeriesCode, s.SerialNumber.SeriesNumber),
                s.Currency, s.Nominal)).ToList();

            lock (_balanceLocker)
            {
                _balance.AddRange(castMoney);
            }

            WriteTransaction(DateTime.Now, $"Someone put {sum} {account.Currency.ShortName} to {account}");
        }

        /// <summary>
        /// Put funds from some account to account<br></br>
        /// <br />
        /// <b>Notes:</b><br />
        /// <i>
        /// Be careful with call this method, look which object call him and which bank get funds<br></br>
        /// Can have more deep exceptions from used methods<br />
        /// </i>
        /// <br />
        /// <b>Required conditions for success:</b><br />
        /// <i>
        /// Both account must be in list of accounts of their bank owner<br />
        /// Amount must be greater 0<br />
        /// Accounts can't be equal<br />
        /// </i>
        /// <br />
        /// <b>Possible exceptions:</b><br />
        /// <i>
        /// <see cref="ArgumentNullException"/><br />
        /// <see cref="ArgumentOutOfRangeException"/><br />
        /// <see cref="InvalidCastException"/><br />
        /// </i>
        /// <br />
        /// <b>Uses methods:</b><br />
        /// <i>
        /// <see cref="AccountExistAsync"/><br></br>
        /// <see cref="WriteTransaction"/><br />
        /// </i>
        /// </summary>
        /// <typeparam name="TBankCodeFrom">Type of bank code from</typeparam>
        /// <typeparam name="TSeriesCodeFrom">Type of series code from</typeparam>
        /// <typeparam name="TSeriesNumberFrom">Type of series number from</typeparam>
        /// <param name="to">Recipient account</param>
        /// <param name="bank">Sender bank</param>
        /// <param name="from">Sender account</param>
        /// <param name="amount">How much will be send</param>
        /// <returns>The task of put funds</returns>
        public async Task PutAsync<TBankCodeFrom, TSeriesCodeFrom, TSeriesNumberFrom>(IBankAccount<string, ulong, ulong> to, INationalBank<TBankCodeFrom, TSeriesCodeFrom, TSeriesNumberFrom> bank, IBankAccount<TBankCodeFrom, TSeriesCodeFrom, TSeriesNumberFrom> from, PositiveDouble amount)
        {
            if (to is null)
            {
                throw new ArgumentNullException(nameof(to));
            }
            if (bank is null)
            {
                throw new ArgumentNullException(nameof(bank));
            }
            if (from is null)
            {
                throw new ArgumentNullException(nameof(from));
            }
            if (amount is null)
            {
                throw new ArgumentNullException(nameof(amount));
            }

            if (amount.Value == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Value is zero");
            }

            if (await AccountExistAsync(to) == false)
            {
                throw new ArgumentOutOfRangeException(nameof(to), "Can't find this account");
            }
            if (await bank.AccountExistAsync(from) == false)
            {
                throw new ArgumentOutOfRangeException(nameof(to), "Can't find this account");
            }
            if (from.Currency.Equals(to.Currency) == false)
            {
                throw new InvalidCastException("Different currency");
            }

            lock (_accountLocker)
            {
                var index = _accounts.IndexOf(_accounts.First(f => f.SerialNumber.Equals(to.SerialNumber)));
                _accounts[index] = new BankAccount<string, ulong, ulong>(
                to.SerialNumber,
                to.User,
                to.Currency,
                new PositiveDouble(to.Amount.Value + amount.Value));
            }

            WriteTransaction(
                DateTime.Now,
                $"{_authorizedUser} got {amount} {to.Currency.ShortName} to {to.SerialNumber} " +
                $"from {bank.GetType().Name} from account of {from}");
        }

        /// <summary>
        /// Checking account exist<br></br>
        /// <br />
        /// <b>Notes:</b><br />
        /// <i>
        /// Checking will be by serial number of account<br></br>
        /// Can have more deep exceptions from used methods<br />
        /// </i>
        /// <br />
        /// <b>Required conditions for success:</b><br />
        /// <i>
        /// Account must be in list of accounts of bank owner<br />
        /// </i>
        /// <br />
        /// <b>Possible exceptions:</b><br />
        /// <i>
        /// <see cref="ArgumentNullException"/><br />
        /// </i>
        /// <br />
        /// <b>Uses methods:</b><br />
        /// <i>
        /// <see cref="GetAccountsAsync"/><br></br>
        /// </i>
        /// </summary>
        /// <param name="account">Account that will be checked</param>
        /// <returns>The task of checking account exist</returns>
        public async Task<bool> AccountExistAsync(IBankAccount<string, ulong, ulong> account)
        {
            if (account is null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            return await GetAccountsAsync()
                .ContinueWith(t =>
                {
                    return t.Result.FirstOrDefault(f =>
                                f.SerialNumber.Equals(account.SerialNumber)) is not null;
                });
        }

        /// <summary>
        /// Checking money exist<br></br>
        /// <br />
        /// <b>Notes:</b><br />
        /// <i>
        /// Checking will be by serial number of money<br></br>
        /// </i>
        /// <br />
        /// <b>Required conditions for success:</b><br />
        /// <i>
        /// Money must be in list of released money of bank owner<br />
        /// </i>
        /// <br />
        /// <b>Possible exceptions:</b><br />
        /// <i>
        /// <see cref="ArgumentNullException"/><br />
        /// </i>
        /// </summary>
        /// <param name="money">Cash money</param>
        /// <returns>The task of checking money exist</returns>
        public async Task<bool> MoneyExistAsync(IMoney<string, ulong, ulong> money)
        {
            if (money is null)
            {
                throw new ArgumentNullException(nameof(money));
            }

            return await Task.Run(new Func<bool>(() =>
            {
                return _releasedMoney.Any(a => a.SerialNumber.Equals(money.SerialNumber));
            }));
        }

        /// <summary>
        /// Get available banknotes for currency<br></br>
        /// <br />
        /// <b>Notes:</b><br />
        /// <i>
        /// Can have more deep exceptions from used methods<br />
        /// </i>
        /// <br />
        /// <b>Required conditions for success:</b><br />
        /// <i>
        /// Currency must be exist in list of currency<br />
        /// </i>
        /// <br />
        /// <b>Possible exceptions:</b><br />
        /// <i>
        /// <see cref="ArgumentNullException"/><br />
        /// <see cref="InvalidProgramException"/><br />
        /// </i>
        /// <br />
        /// <b>Uses methods:</b><br />
        /// <i>
        /// <see cref="LoadCurrenciesAsync"/><br />
        /// <see cref="BankCurrencies.GetAsync"/><br />
        /// </i>
        /// </summary>
        /// <param name="currency">Currency</param>
        /// <returns>The task of get list of available banknotes for currency</returns>
        public async Task<List<PositiveDouble>> GetBanknotesForCurrencyAsync(ICurrency currency)
        {
            //Here must be implementation nominals load for currency from real database
            if (currency is null)
            {
                throw new ArgumentNullException(nameof(currency));
            }

            await LoadCurrenciesAsync();

            var currencies = await Currencies.GetAsync();

            DateTime? maxDate;
            IBankCurrency realCurrency;

            lock (_currencyLocker)
            {
                maxDate = currencies.Where(w => w.Currency.Equals(currency))?.Max(m => m?.Date);
                if (maxDate is null)
                {
                    throw new InvalidProgramException("Can't find necessary date for currency");
                }
                realCurrency = currencies.FirstOrDefault(f => f.Currency.Equals(currency) && f?.Date == maxDate);
                if (realCurrency is null)
                {
                    throw new InvalidProgramException("Can't find currency");
                }
            }

            List<PositiveDouble> list = new List<PositiveDouble>();
            list.Add(new PositiveDouble(100));
            list.Add(new PositiveDouble(50));
            list.Add(new PositiveDouble(20));
            list.Add(new PositiveDouble(10));
            list.Add(new PositiveDouble(5));
            list.Add(new PositiveDouble(2));
            list.Add(new PositiveDouble(1));
            list.Add(new PositiveDouble(0.5));
            list.Add(new PositiveDouble(0.25));
            list.Add(new PositiveDouble(0.10));
            list.Add(new PositiveDouble(0.05));
            list.Add(new PositiveDouble(0.01));

            return list;
        }
    }
}
