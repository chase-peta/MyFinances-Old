﻿using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Models
{
    [MetadataType(typeof(LoanHistoryMeta))]
    public partial class LoanHistory
    {
        public decimal Payment { get { return BasicPayment + AddPayment + Interest + Escrow; } }

        public decimal Principal { get; set; }
    }

    public static class LoanHistoryExentions
    {
        public static IEnumerable<LoanHistory> GetLoanHistory(this Loan loan)
        {
            IEnumerable<LoanHistory> loanHistory = loan.LoanHistories.OrderBy(x => x.DatePaid).ToList();
            foreach (LoanHistory history in loanHistory)
            {
                loan.Principal -= (history.AddPayment + history.BasicPayment);
                history.Principal = loan.Principal;
            }
            return loanHistory;
        }
    }

    public class LoanHistoryMeta
    {
        /* Not In Database */
        [Display(Name = "Payment"), DisplayFormat(DataFormatString = "{0:c}")]
        public object Payment { get; set; }

        [Display(Name = "Principal"), DisplayFormat(DataFormatString = "{0:c}")]
        public object Principal { get; set; }

        /* In Database */
        [Display(Name = "Additional"), DisplayFormat(DataFormatString = "{0:c}")]
        public object AddPayment { get; set; }

        [Display(Name = "Base"), DisplayFormat(DataFormatString = "{0:c}")]
        public object BasicPayment { get; set; }

        [Display(Name = "Date Paid"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public object DatePaid { get; set; }

        [Display(Name = "Escrow"), DisplayFormat(DataFormatString = "{0:c}")]
        public object Escrow { get; set; }

        [Display(Name = "Interest"), DisplayFormat(DataFormatString = "{0:c}")]
        public object Interest { get; set; }

        [Display(Name = "Payment Type")]
        public object PaymentTypeId { get; set; }

        /* Not Used In View */
        public object Id { get; set; }

        public object Version { get; set; }

        public object CreationDate { get; set; }

        public object ModifyDate { get; set; }

        public object LoanId { get; set; }
    }
}