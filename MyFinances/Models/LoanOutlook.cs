using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Models
{
    public class LoanOutlook
    {
        public LoanOutlook(DateTime date, decimal interestAmount, decimal baseAmount, decimal addAmount, decimal escrowAmount, decimal principal)
        {
            Date = date;
            InterestAmount = interestAmount;
            BaseAmount = baseAmount;
            AddAmount = addAmount;
            EscrowAmount = escrowAmount;
            Principal = principal;
        }

        [Display(Name = "Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }

        [Display(Name = "Interest"), DisplayFormat(DataFormatString = "{0:c}")]
        public decimal InterestAmount { get; set; }

        [Display(Name = "Base"), DisplayFormat(DataFormatString = "{0:c}")]
        public decimal BaseAmount { get; set; }

        [Display(Name = "Additional"), DisplayFormat(DataFormatString = "{0:c}")]
        public decimal AddAmount { get; set; }

        [Display(Name = "Escrow"), DisplayFormat(DataFormatString = "{0:c}")]
        public decimal EscrowAmount { get; set; }

        [Display(Name = "Payment"), DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Payment { get { return InterestAmount + BaseAmount + AddAmount + EscrowAmount; } }

        [Display(Name = "Principal"), DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Principal { get; set; }
    }
}