using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Interfaces;

namespace Banking.Implementations
{
    public class PositiveDouble
    {

        public double Value { get; init; }

        public PositiveDouble(double value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Negative value");
            }
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public bool Equals(PositiveDouble value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (GetHashCode() == value.GetHashCode())
            {
                return true;
            }
            else
            {
                return Value == value.Value;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            else
            {
                if (obj is PositiveDouble value)
                {
                    return Equals(value);
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
