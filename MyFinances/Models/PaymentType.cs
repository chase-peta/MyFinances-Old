using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Models
{
    [MetadataType(typeof(PaymentTypeMeta))]
    public partial class PaymnetType
    {

    }

    public class PaymentTypeMeta
    {
        /* In Database */
        [Display(Name = "Name")]
        public object Name { get; set; }

        [Display(Name = "Payments Per Year")]
        public object PaymentsPerYear { get; set; }

        /* Not Used In View */
        public object Id { get; set; }

        public object Version { get; set; }

        public object CreationDate { get; set; }

        public object ModifyDate { get; set; }
    }
}