using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Interfaces;

namespace Banking.Implementations
{
    public class Transaction: ITransaction
    {
        public DateTime Date { get; init; }

        public string Description { get; init; }

        public Transaction(DateTime dateTime, string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("String was null or white space");
            }

            Date = dateTime;
            Description = description;
        }

        public override string ToString()
        {
            return $"{Date} {Description}";
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            else
            {
                if (obj is ITransaction transaction)
                {
                    return Equals(transaction);
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Equals(ITransaction transaction)
        {
            if (transaction is null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            else
            {
                if (GetHashCode().Equals(transaction.GetHashCode()))
                {
                    return true;
                }
                else
                {
                    return Date.Equals(transaction.Date) && Description.Equals(transaction.Description);
                }
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
