using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Models
{
    [MetadataType(typeof(UserMeta))]
    public partial class User
    {

    }

    public class UserMeta
    {
        /* In Database */
        [Display(Name = "First Name")]
        public object FirstName { get; set; }

        [Display(Name = "Last Name")]
        public object LastName { get; set; }

        [Display(Name = "Email")]
        public object Email { get; set; }

        [Display(Name = "Password")]
        public object Password { get; set; }

        [Display(Name = "Is Active")]
        public object IsActive { get; set; }

        /* Not Used In View */
        public object Id { get; set; }

        public object Version { get; set; }

        public object CreationDate { get; set; }

        public object ModifyDate { get; set; }
    }
}