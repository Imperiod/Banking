using System;
using Banking.Interfaces;

namespace Banking.Implementations
{
    public sealed class BankAccount<TBankCode, TSeriesCode, TSeriesNumber> : IBankAccount<TBankCode, TSeriesCode, TSeriesNumber>
    {
        public IBankSerialNumber<TBankCode, TSeriesCode, TSeriesNumber> SerialNumber { get; init; }

        public IBankUser User { get; init; }

        public IBankCurrency Currency { get; init; }

        public PositiveDouble Amount { get; init; }

        public BankAccount(IBankSerialNumber<TBankCode, TSeriesCode, TSeriesNumber> serialNumber, IBankUser user, IBankCurrency currency, PositiveDouble amount)
        {
            if (serialNumber is null)
            {
                throw new ArgumentNullException(nameof(serialNumber));
            }
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (currency is null)
            {
                throw new ArgumentNullException(nameof(currency));
            }
            if (amount is null)
            {
                throw new ArgumentNullException(nameof(amount));
            }

            SerialNumber = serialNumber;
            User = user;
            Currency = currency;
            Amount = new PositiveDouble(Math.Round(amount.Value, 2, MidpointRounding.ToZero));
        }

        public bool Equals(IBankAccount<TBankCode, TSeriesCode, TSeriesNumber> account)
        {
            if (account is null)
            {
                throw new ArgumentNullException(nameof(account));
            }
            else
            {
                if (GetHashCode().Equals(account.GetHashCode()))
                {
                    return true;
                }
                else
                {
                    return SerialNumber.Equals(account.SerialNumber) &&
                        User.Equals(account.User) &&
                        Currency.Equals(account.Currency) &&
                        Amount.Equals(account.Amount);
                }
            }
        }

        public override string ToString()
        {
            return $"Account of {User} {SerialNumber}, which has {Amount} {Currency.ShortName}";
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            else
            {
                if (obj is IBankAccount<TBankCode, TSeriesCode, TSeriesNumber> account)
                {
                    return Equals(account);
                }
                else
                {
                    return false;
                }
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
