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
    }
}
