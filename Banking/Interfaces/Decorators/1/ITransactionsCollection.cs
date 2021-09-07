using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Interfaces
{
    public interface ITransactionsCollection
    {
        public Task<List<ITransaction>> GetAsync();
    }
}
