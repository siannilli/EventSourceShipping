using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.ValueObjects;

namespace SharedShippingDomainsObjects.ValueObjects
{
    public class CostAmount: ValueObject<CostAmount>
    {

        public Currency Currency { get; private set; }
        public decimal Amount { get; private set; }
        
        public CostAmount(Currency currency, decimal amount)
        {
            this.Currency = currency;
            this.Amount = amount;            
        }

        public override string ToString()
        {
            return $"{this.Currency.Symbol ?? this.Currency.Code} {this.Amount:#,##0.00;(#,##0.00);-}";
        }
    }
}
