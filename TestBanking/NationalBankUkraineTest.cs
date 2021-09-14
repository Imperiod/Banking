using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Banking.Interfaces;
using Banking.Implementations;

namespace TestBanking
{
    public class NationalBankUkraineTest : IClassFixture<NationalBankUkraineFixture>
    {
        private NationalBankUkraineFixture fixture;

        public NationalBankUkraineTest(NationalBankUkraineFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void LoadCurrenciesAsync_DateTimeArray_NoExeptions()
        {
            var taskOne = new NationalBankUkraine().LoadCurrenciesAsync();
            var taskTwo = new NationalBankUkraine().LoadCurrenciesAsync(new DateTime(2020, 6, 30));
            var taskThree = new NationalBankUkraine().LoadCurrenciesAsync(new DateTime(1000, 6, 30));
            var task = Task.WhenAll(taskOne, taskTwo, taskThree);

            Assert.True(task.ContinueWith(t =>
            {
                return t.IsCompletedSuccessfully;
            }).Result);
        }

        [Fact]
        public void AuthorizeAsync_Null_ArgumentNullException()
        {
            Assert.ThrowsAsync<AggregateException>(() => fixture.Bank.AuthorizeAsync(null));
        }

        [Fact]
        public void AuthorizeAsync_User_Successed()
        {
            fixture.Bank.AuthorizeAsync(new BankUser("TEST1", "TEST1")).Wait();
            Assert.Equal(new BankUser("TEST1", "TEST1"), fixture.Bank.GetAuthorizedUser().Result);
            var task = fixture.Bank.AuthorizeAsync(new BankUser("TEST", "TEST"));
            task.Wait();
            Assert.True(task.IsCompletedSuccessfully);
        }

        [Fact]
        public void RegisterAccountAsync_Null_ArgumentNullException()
        {
            Assert.ThrowsAsync<AggregateException>(() => fixture.Bank.RegisterAccountAsync(null));
        }

        #region GetBanknotesForCurrencyAsync
        [Fact]
        public void GetBanknotesForCurrencyAsync_Null_ArgumentNullException()
        {
            Assert.ThrowsAsync<AggregateException>(() => fixture.Bank.GetBanknotesForCurrencyAsync(null));
        }

        [Fact]
        public void GetBanknotesForCurrencyAsync_BadValue_InvalidProgramException()
        {
            var dummyCurrency = new Currency("TEST");

            Assert.ThrowsAsync<AggregateException>(() => 
            fixture.Bank.GetBanknotesForCurrencyAsync(dummyCurrency));
        }

        [Fact]
        public void GetBanknotesForCurrencyAsync_Currency_CurrencyList()
        {
            var currency = fixture.Bank.GetBaseNationalCurrencyAsync().Result;

            Assert.NotEmpty(fixture.Bank.GetBanknotesForCurrencyAsync(currency).Result);
        }


        #endregion

        #region AccountExistAsync
        [Fact]
        public void AccountExistAsync_Null_ArgumentNullException()
        {
            Assert.ThrowsAsync<AggregateException>(() => fixture.Bank.AccountExistAsync(null));
        }

        [Fact]
        public void AccountExistAsync_BadValue_False()
        {
            var serialNumber = new BankSerialNumber<string, ulong, ulong>("TEST", 1, 1);
            var user = new BankUser("TEST20", "TEST20");
            var currency = fixture.Bank.GetFullNationalCurrencyAsync().Result;
            var value = new PositiveDouble(10);

            var dummyAccount = new BankAccount<string, ulong, ulong>(serialNumber, user, currency, value);

            Assert.False(fixture.Bank.AccountExistAsync(dummyAccount).Result);
        }

        [Fact]
        public void AccountExistAsync_CorrectValue_True()
        {
            Assert.True(fixture.Bank.AccountExistAsync(fixture.Accounts.First()).Result);
        }
        #endregion

        #region ExchangeDigitalCurrencyAsync
        [Theory]
        [InlineData(0.01)]
        public void ExchangeDigitalCurrencyAsync_Nums_SuccessedConvert(double howMuchConvert)
        {
            var taskConvert = fixture.Bank.ExchangeDigitalCurrencyAsync(
                fixture.Accounts.First(),
                fixture.Accounts.Last(),
                new PositiveDouble(howMuchConvert));

            taskConvert.Wait();

            Assert.True(taskConvert.IsCompletedSuccessfully);
        }

        [Theory]
        [InlineData(0)]
        public void ExchangeDigitalCurrencyAsync_BadValue_ArgumentOutOfRangeException(double value)
        {
            var task = fixture.Bank.ExchangeDigitalCurrencyAsync(
                fixture.Accounts.First(),
                fixture.Accounts.Last(),
                new PositiveDouble(value));

            Assert.ThrowsAsync<AggregateException>(() => task);
        }

        [Fact]
        public void ExchangeDigitalCurrencyAsync_Null_ArgumentNullException()
        {
            var taskOne = fixture.Bank.ExchangeDigitalCurrencyAsync(
                    fixture.Accounts.First(),
                    null,
                    new PositiveDouble(10));

            var taskTwo = fixture.Bank.ExchangeDigitalCurrencyAsync(
                    null,
                    fixture.Accounts.Last(),
                    new PositiveDouble(10));

            Assert.ThrowsAsync<AggregateException>(() => taskOne);
            Assert.ThrowsAsync<AggregateException>(() => taskTwo);
        }
        #endregion

        #region ExchangeCashCurrencyAsync
        [Fact]
        public void ExchangeCashCurrencyAsync_Money_SuccessedConvert()
        {
            ICurrency currency = fixture.Bank.GetBaseNationalCurrencyAsync().Result;
            var list = fixture.AvailableMoney.First(f => f.Key.Equals(currency)).Value;

            Task<List<IMoney<string, ulong, ulong>>>[] tasks = new Task<List<IMoney<string, ulong, ulong>>>[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                tasks[i] = fixture.Bank.ExchangeCashCurrencyAsync(list[i], currency);
            }

            var finalTask = Task.WhenAll(tasks);
            finalTask.Wait();

            Assert.True(finalTask.IsCompletedSuccessfully);
        }

        [Fact]
        public void ExchangeCashCurrencyAsync_Null_ArgumentNullException()
        {
            ICurrency currency = fixture.Bank.GetBaseNationalCurrencyAsync().Result;
            var list = fixture.AvailableMoney.First(f => f.Key.Equals(currency)).Value;

            var taskOne = fixture.Bank.ExchangeCashCurrencyAsync(
                    null,
                    currency);

            var taskTwo = fixture.Bank.ExchangeCashCurrencyAsync(
                    list.First(),
                    null);

            Assert.ThrowsAsync<AggregateException>(() => taskOne);
            Assert.ThrowsAsync<AggregateException>(() => taskTwo);
        }
        #endregion

        #region SendAsync
        [Fact]
        public void SendAsync_Null_ArgumentNullException()
        {
            var from = fixture.Accounts.First();
            var to = fixture.Accounts.First();
            var value = new PositiveDouble(10);

            Assert.ThrowsAsync<AggregateException>(() => 
                fixture.Bank.SendAsync(null, fixture.Bank, to, value));
            Assert.ThrowsAsync<AggregateException>(() =>
                fixture.Bank.SendAsync(from, null, to, value));
            Assert.ThrowsAsync<AggregateException>(() =>
                fixture.Bank.SendAsync(from, fixture.Bank, null, value));
            Assert.ThrowsAsync<AggregateException>(() =>
                fixture.Bank.SendAsync(from, fixture.Bank, to, null));
        }

        [Fact]
        public void SendAsync_BadValue_ArgumentOutOfRangeException()
        {
            var from = fixture.Accounts.First();
            var to = fixture.Accounts.First();
            var value = new PositiveDouble(0);

            Assert.ThrowsAsync<AggregateException>(() =>
                fixture.Bank.SendAsync(from, fixture.Bank, to, value));
        }

        [Fact]
        public void SendAsync_EqualsAccount_ArgumentException()
        {
            var from = fixture.Accounts.First();
            var to = fixture.Accounts.First();
            var value = new PositiveDouble(10);

            Assert.ThrowsAsync<AggregateException>(() =>
                fixture.Bank.SendAsync(from, fixture.Bank, to, value));
        }

        [Fact]
        public void SendAsync_DifferentCurrency_ArgumentException()
        {
            var from = fixture.Accounts.First();
            var to = fixture.Accounts.Last();
            var value = new PositiveDouble(10);

            Assert.ThrowsAsync<AggregateException>(() =>
                fixture.Bank.SendAsync(from, fixture.Bank, to, value));
        }

        [Fact]
        public void SendAsync_Success()
        {
            var from = fixture.Accounts.First();
            var to = fixture.AnotherBank.GetAccountsAsync().Result.First(f => f.Currency.Equals(from.Currency));
            var value = new PositiveDouble(1000);
            var preValue = to.Amount.Value;

            var task = fixture.Bank.SendAsync(from, fixture.AnotherBank, to, value);
            task.Wait();

            var newValue = fixture.AnotherBank.GetAccountsAsync().Result.First(f => f.Currency.Equals(to.Currency));
            Assert.Equal(preValue + value.Value, newValue.Amount.Value);
        }
        #endregion

        #region PutCashAsync
        [Fact]
        public void PutCashAsync_Null_ArgumentNullException()
        {
            var account = fixture.Accounts.First();
            var money = fixture.AvailableMoney.First(f => f.Key.Equals(account.Currency.Currency)).Value;


            Assert.ThrowsAsync<AggregateException>(() => 
            fixture.Bank.PutCashAsync<string, ulong, ulong>(null, money));

            Assert.ThrowsAsync<AggregateException>(() => 
            fixture.Bank.PutCashAsync<string, ulong, ulong>(account, null));

            Assert.ThrowsAsync<AggregateException>(() => 
            fixture.Bank.PutCashAsync<string, ulong, ulong>(null, null));
        }

        [Fact]
        public void PutCashAsync_BadValue_ArgumentException()
        {
            var account = fixture.Accounts.First();
            var money = fixture.AvailableMoney.First(f => f.Key.Equals(account.Currency.Currency)).Value;

            IBankSerialNumber<string, ulong, ulong> serialNumber = 
                new BankSerialNumber<string, ulong, ulong>("NBU", 1, 1);
            IBankUser user = new BankUser("TEST2", "TEST2");
            IBankCurrency currency = fixture.Bank.GetFullNationalCurrencyAsync().Result;
            PositiveDouble value = new PositiveDouble(1);

            var dummyAccount = new BankAccount<string, ulong, ulong>(serialNumber, user, currency, value);
            var dummyMoney = new List<IMoney<string, ulong, ulong>>();

            Assert.ThrowsAsync<AggregateException>(() => fixture.Bank.PutCashAsync(dummyAccount, money));
            Assert.ThrowsAsync<AggregateException>(() => fixture.Bank.PutCashAsync(account, dummyMoney));
        }

        [Fact]
        public void PutCashAsync_DifferentCurrency_ArgumentException()
        {
            var account = fixture.Accounts.First();
            var money = fixture.AvailableMoney.First(f => f.Key.Equals(account.Currency.Currency) == false).Value;

            Assert.ThrowsAsync<AggregateException>(() => fixture.Bank.PutCashAsync(account, money));
        }

        [Fact]
        public void PutCashAsync_Success()
        {
            var account = fixture.Accounts.First();
            var money = fixture.AvailableMoney.First(f => f.Key.Equals(account.Currency.Currency)).Value;
            var preAmount = account.Amount.Value;
            var postAmount = preAmount + money.Sum(s => s.Nominal.Value);

            fixture.Bank.PutCashAsync<string, ulong, ulong>(account, money).Wait();
            fixture.Accounts = fixture.Bank.GetAccountsAsync().Result;

            Assert.Equal(postAmount, fixture.Accounts.First(f => f.SerialNumber.Equals(account.SerialNumber)).Amount.Value);
        }
        #endregion

        #region PutAsync
        [Fact]
        public void PutAsync_Null_ArgumentNullException()
        {
            var from = fixture.Accounts.First();
            var to = fixture.AnotherBank.GetAccountsAsync().Result.First(f => f.Currency.Currency.Equals(from.Currency.Currency));
            var value = new PositiveDouble(10);

            Assert.ThrowsAsync<AggregateException>(() => 
            fixture.Bank.PutAsync<string, ulong, ulong>(null, fixture.Bank, from, value));

            Assert.ThrowsAsync<AggregateException>(() => 
            fixture.Bank.PutAsync<string, ulong, ulong>(to, null, from, value));

            Assert.ThrowsAsync<AggregateException>(() => 
            fixture.Bank.PutAsync<string, ulong, ulong>(to, fixture.Bank, null, value));

            Assert.ThrowsAsync<AggregateException>(() =>
            fixture.Bank.PutAsync<string, ulong, ulong>(to, fixture.Bank, from, null));
        }

        [Fact]
        public void PutAsync_BadValue_ArgumentOutOfRangeException()
        {
            var from = fixture.Accounts.First();
            var to = fixture.AnotherBank.GetAccountsAsync().Result.First(f => f.Currency.Currency.Equals(from.Currency.Currency));
            var value = new PositiveDouble(0);

            Assert.ThrowsAsync<AggregateException>(() =>
            fixture.Bank.PutAsync<string, ulong, ulong>(to, fixture.Bank, from, value));
        }

        [Fact]
        public void PutAsync_EnemyAccount_ArgumentOutOfRangeException()
        {
            var from = fixture.Accounts.First();
            var to = fixture.AnotherBank.GetAccountsAsync().Result.First(f => f.Currency.Currency.Equals(from.Currency.Currency));
            var value = new PositiveDouble(10);

            var serialNumber = new BankSerialNumber<string, ulong, ulong>("NBU", 999, 999);
            var user = new BankUser("TEST10", "TEST10");
            var currency = fixture.Bank.GetFullNationalCurrencyAsync().Result;

            var dummyFrom = new BankAccount<string, ulong, ulong>(serialNumber, user, currency, value);
            var dummyTo = dummyFrom;

            Assert.ThrowsAsync<AggregateException>(() =>
            fixture.Bank.PutAsync<string, ulong, ulong>(to, fixture.Bank, dummyFrom, value));

            Assert.ThrowsAsync<AggregateException>(() =>
            fixture.Bank.PutAsync<string, ulong, ulong>(dummyTo, fixture.Bank, from, value));
        }

        [Fact]
        public void PutAsync_DifferentCurrency_InvalidCastException()
        {
            var from = fixture.Accounts.First();
            var to = fixture.AnotherBank.GetAccountsAsync().Result.First(f => f.Currency.Currency.Equals(from.Currency.Currency) == false);
            var value = new PositiveDouble(10);

            Assert.ThrowsAsync<AggregateException>(() =>
                fixture.Bank.PutAsync(to, fixture.Bank, from, value));
        }

        [Fact]
        public void PutAsync_Success()
        {
            var from = fixture.Accounts.First();
            var to = fixture.AnotherBank.GetAccountsAsync().Result.First(f => f.Currency.Equals(from.Currency));
            var value = new PositiveDouble(1000);
            var preValue = to.Amount.Value;
            var postValue = preValue + value.Value;

            fixture.AnotherBank.PutAsync(to, fixture.Bank, from, value).Wait();

            var newValue = fixture.AnotherBank.GetAccountsAsync().Result.First(f => f.Currency.Equals(to.Currency));

            Assert.Equal(postValue, newValue.Amount.Value);
        }
        #endregion


    }

    public class NationalBankUkraineFixture
    {
        public INationalBank<string, ulong, ulong> Bank { get; private set; }

        /// <summary>
        /// Only for test send and put
        /// </summary>
        public INationalBank<string, ulong, ulong> AnotherBank { get; private set; }

        public List<IBankCurrency> Currencies { get; private set; }

        public List<IBankAccount<string, ulong, ulong>> Accounts { get; set; }

        public Dictionary<ICurrency, List<IMoney<string, ulong, ulong>>> AvailableMoney { get; private set; }

        public NationalBankUkraineFixture()
        {
            AvailableMoney = new Dictionary<ICurrency, List<IMoney<string, ulong, ulong>>>();
            Bank = new NationalBankUkraine();
            var loadTask = Bank.LoadCurrenciesAsync();
            var regUserTask = Bank.RegisterUserAsync(new BankUser("TEST", "TEST"));

            Task.WaitAll(loadTask, regUserTask);

            Bank.Currencies.GetAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    throw t.Exception;
                }
                Task.WaitAll(t.Result.Where(w => w.Date.Date == DateTime.Now.Date)
                .Select(s => Bank.RegisterAccountAsync(s.Currency)).ToArray());
            }).Wait();

            Bank.GetAccountsAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    throw t.Exception;
                }

                Task[] tasks = new Task[t.Result.Count];
                for (int i = 0; i < t.Result.Count; i++)
                {
                    List<IMoney<dynamic, dynamic, dynamic>> list = new List<IMoney<dynamic, dynamic, dynamic>>();
                    ICurrency currency = t.Result[i].Currency.Currency;

                    for (uint m = 0; m < 1000; m++)
                    {
                        IBankSerialNumber<dynamic, dynamic, dynamic> serialNumber = new BankSerialNumber<dynamic, dynamic, dynamic>("NBU", 1, (ulong)(m * (i + 1)) + 1);
                        IMoney<dynamic, dynamic, dynamic> money = new Money<dynamic, dynamic, dynamic>(serialNumber, currency, new PositiveDouble(100));
                        list.Add(money);
                    }

                    tasks[i] = Bank.PutCashAsync(t.Result[i], list);
                }
                Task.WaitAll(tasks);
            }).Wait();

            var acc = Bank.GetAccountsAsync();
            var curr = Bank.Currencies.GetAsync();

            Task.WaitAll(acc, curr);

            Accounts = acc.Result;
            Currencies = curr.Result;

            for (uint i = 1; i <= Currencies.Count; i++)
            {
                ICurrency currency = Currencies[(int)(i-1)].Currency;
                var banknotes = Bank.GetBanknotesForCurrencyAsync(currency).Result;

                PositiveDouble nominal = banknotes.First(f =>
                f.Value == banknotes.Max(m => m.Value));

                List<IMoney<string, ulong, ulong>> list = new List<IMoney<string, ulong, ulong>>();

                for (uint m = 1; m < 101; m++)
                {
                    IBankSerialNumber<string, ulong, ulong> serialNumber =
                        new BankSerialNumber<string, ulong, ulong>("NBU_M", i, m);

                    IMoney<string, ulong, ulong> money =
                        new Money<string, ulong, ulong>(serialNumber, currency, nominal);

                    list.Add(money);
                }

                AvailableMoney.Add(currency, list);
            }


            AnotherBank = new NationalBankUkraine();
            var loadTaskAnother = AnotherBank.LoadCurrenciesAsync();
            var regUserTaskAnother = AnotherBank.RegisterUserAsync(new BankUser("TESTELSE", "TESTELSE"));

            Task.WaitAll(loadTaskAnother, regUserTaskAnother);

            AnotherBank.Currencies.GetAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    throw t.Exception;
                }
                Task.WaitAll(t.Result.Where(w => w.Date.Date == DateTime.Now.Date)
                .Select(s => AnotherBank.RegisterAccountAsync(s.Currency)).ToArray());
            }).Wait();

            AnotherBank.GetAccountsAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    throw t.Exception;
                }

                Task[] tasks = new Task[t.Result.Count];
                for (int i = 0; i < t.Result.Count; i++)
                {
                    List<IMoney<dynamic, dynamic, dynamic>> list = new List<IMoney<dynamic, dynamic, dynamic>>();
                    ICurrency currency = t.Result[i].Currency.Currency;

                    for (uint m = 0; m < 1000; m++)
                    {
                        IBankSerialNumber<dynamic, dynamic, dynamic> serialNumber = new BankSerialNumber<dynamic, dynamic, dynamic>("NBU", 1, (ulong)(m * (i + 1)) + 1);
                        IMoney<dynamic, dynamic, dynamic> money = new Money<dynamic, dynamic, dynamic>(serialNumber, currency, new PositiveDouble(100));
                        list.Add(money);
                    }

                    tasks[i] = AnotherBank.PutCashAsync(t.Result[i], list);
                }
                Task.WaitAll(tasks);
            }).Wait();
        }
    }
}
