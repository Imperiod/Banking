using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Implementations;

namespace Banking.Interfaces
{
    public interface INationalBank<TBankCode, TSeriesCode, TSeriesNumber> : IExchangeDigitalCurrency, IExchangeCashCurrency
    {
        public ITransactionsCollection Transactions { get; }

        public IBankCurrencyCollection Currencies { get; }

        public Task<ICurrency> GetBaseNationalCurrencyAsync();

        public Task<IBankCurrency> GetFullNationalCurrencyAsync();

        public Task<IBankUser> GetAuthorizedUser();

        public Task LoadCurrenciesAsync(DateTime? dateTime = null);

        public Task AuthorizeAsync(IBankUser user);

        public Task RegisterUserAsync(IBankUser user);

        public Task RegisterAccountAsync(ICurrency currency);

        public Task<List<IBankAccount<TBankCode, TSeriesCode, TSeriesNumber>>> GetAccountsAsync();

        public Task SendAsync<TBankCodeTo, TSeriesCodeTo, TSeriesNumberTo>(IBankAccount<TBankCode, TSeriesCode, TSeriesNumber> from, INationalBank<TBankCodeTo, TSeriesCodeTo, TSeriesNumberTo> bank, IBankAccount<TBankCodeTo, TSeriesCodeTo, TSeriesNumberTo> to, PositiveDouble amount);

        public Task PutCashAsync<TBankCodeMoney, TSeriesCodeMoney, TSeriesNumberMoney>(IBankAccount<TBankCode, TSeriesCode, TSeriesNumber> account, List<IMoney<TBankCodeMoney, TSeriesCodeMoney, TSeriesNumberMoney>> money);

        public Task PutAsync<TBankCodeFrom, TSeriesCodeFrom, TSeriesNumberFrom>(IBankAccount<TBankCode, TSeriesCode, TSeriesNumber> to, INationalBank<TBankCodeFrom, TSeriesCodeFrom, TSeriesNumberFrom> bank, IBankAccount<TBankCodeFrom, TSeriesCodeFrom, TSeriesNumberFrom> from, PositiveDouble amount);

        public Task<bool> AccountExistAsync(IBankAccount<TBankCode, TSeriesCode, TSeriesNumber> account);

        public Task<bool> MoneyExistAsync(IMoney<TBankCode, TSeriesCode, TSeriesNumber> money);

        public Task<List<PositiveDouble>> GetBanknotesForCurrencyAsync(ICurrency currency);
    }
}
