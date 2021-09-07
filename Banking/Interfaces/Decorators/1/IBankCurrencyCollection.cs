﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Interfaces
{
    public interface IBankCurrencyCollection
    {
        public Task<List<IBankCurrency>> GetAsync();
    }
}
