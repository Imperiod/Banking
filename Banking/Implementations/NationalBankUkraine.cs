using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Banking.Interfaces;

namespace Banking.Implementations
{
    public sealed partial class NationalBankUkraine : INationalBank<string, ulong, ulong>
    {
        #region Lockers
        private object _accountLocker;

        private object _serialNumberLocker;

        private object _moneyLocker;

        private object _currencyLocker;

        private object _transactionLocker;

        private object _balanceLocker;
        #endregion

        private const string NATIONAL_CURRENCY_SHORT = "UAH";

        private const string NATIONAL_CURRENCY_FULL = "Ukrainian hryvnia";

        private IBankSerialNumber<string, ulong, ulong> _lastSerialNumber;

        private List<IBankCurrency> _currencies;
        
        private List<IBankAccount<string, ulong, ulong>> _accounts;

        private List<IMoney<string, ulong, ulong>> _releasedMoney;

        private List<IMoney<dynamic, dynamic, dynamic>> _balance;

        private IBankUser _authorizedUser;

        private List<ITransaction> _transactions;
        
        /// <summary>
        /// Use <see cref="LoadCurrenciesAsync"/> before call any methods
        /// </summary>
        public NationalBankUkraine()
        {

            _accounts = new List<IBankAccount<string, ulong, ulong>>();
            _lastSerialNumber = new BankSerialNumber<string, ulong, ulong>("NBU", 1, 0);

            _transactionLocker = new object();
            _transactions = new List<ITransaction>();
            Transactions = new Transactions(ref _transactions, ref _transactionLocker);

            _currencyLocker = new object();
            _currencies = new List<IBankCurrency>();
            Currencies = new BankCurrencies(ref _currencies, ref _currencyLocker);


            _releasedMoney = new List<IMoney<string, ulong, ulong>>();
            _balance = new List<IMoney<dynamic, dynamic, dynamic>>();

            
            _accountLocker = new object();
            _serialNumberLocker = new object();
            _moneyLocker = new object();
            _balanceLocker = new object();
        }

        #region LoadingCurrencies
        private async Task<byte[]> GetResponseAsync(Uri uri)
        {
            System.Net.WebClient webClient = new System.Net.WebClient();
            return await webClient.DownloadDataTaskAsync(uri);
        }

        private async Task<JsonDocument> GetJsonDocumentAsync(Uri uri)
        {
            try
            {
                return await GetResponseAsync(uri).ContinueWith(t => { return JsonDocument.Parse(t.Result); });
            }
            catch
            {
                throw;
            }
        }

        private IBankCurrency ParseCurrency(JsonElement item)
        {
            string name = item.GetProperty("txt").GetString();
            string shortName = item.GetProperty("cc").GetString();
            PositiveDouble rate = new PositiveDouble(item.GetProperty("rate").GetDouble());
            ICurrencyRate currencyRate = new CurrencyRate(rate);

            string year = item.GetProperty("exchangedate").GetString()[^4..];
            string month = item.GetProperty("exchangedate").GetString()[3..5];
            string day = item.GetProperty("exchangedate").GetString()[0..2];

            DateTime exchangeDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
            ICurrency currency = new Currency(name);

            return new BankCurrency(currency, shortName, currencyRate, exchangeDate);
        }

        private async Task<List<IBankCurrency>> GetCurrencies(Uri uri)
        {
            List<IBankCurrency> list = new List<IBankCurrency>();
            using (JsonDocument document = await GetJsonDocumentAsync(uri))
            {
                foreach (var item in document.RootElement.EnumerateArray())
                {
                    IBankCurrency currency = ParseCurrency(item);

                    list.Add(currency);
                }
            }

            return list;
        }

        private void AddCurrency(IBankCurrency currency)
        {
            if (currency is null)
            {
                throw new ArgumentNullException(nameof(currency));
            }

            lock (_currencyLocker)
            {
                if (_currencies.Contains(currency) == false)
                {
                    _currencies.Add(currency);
                }
            }
        }
        #endregion

        private async Task<IBankSerialNumber<string, ulong, ulong>> GetNewSeriesNumber()
        {
            return await Task.Run(() =>
            {
                lock (_serialNumberLocker)
                {
                    if (_lastSerialNumber.SeriesNumber == ulong.MaxValue)
                    {
                        _lastSerialNumber = new BankSerialNumber<string, ulong, ulong>(
                            _lastSerialNumber.BankCode,
                            _lastSerialNumber.SeriesCode + 1,
                            1);
                    }
                    else
                    {
                        _lastSerialNumber = new BankSerialNumber<string, ulong, ulong>(
                            _lastSerialNumber.BankCode,
                            _lastSerialNumber.SeriesCode,
                            _lastSerialNumber.SeriesNumber + 1);
                    }

                    return _lastSerialNumber;
                }
            });
        }

        private async Task RefreshAccountRateAsync(IBankAccount<string, ulong, ulong> account)
        {
            if (account is null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            await LoadCurrenciesAsync();

            IBankCurrency filteredCurrency = default;

            var currencies = await Currencies.GetAsync();

            filteredCurrency = currencies.FirstOrDefault(w =>
            w.ShortName == account.Currency.ShortName && w.Date.Date == DateTime.Now.Date);

            if (filteredCurrency is null)
            {
                throw new ArgumentOutOfRangeException(nameof(account));
            }

            account = new BankAccount<string, ulong, ulong>(account.SerialNumber, account.User, filteredCurrency, account.Amount);
        }

        private void WriteTransaction(DateTime dateTime, string operation)
        {
            if (string.IsNullOrWhiteSpace(operation))
            {
                throw new ArgumentException("String can't be null or white space");
            }

            ITransaction transaction = new Transaction(dateTime, operation);

            lock (_transactionLocker)
            {
                _transactions.Add(transaction);
            }
        }
    }
}
