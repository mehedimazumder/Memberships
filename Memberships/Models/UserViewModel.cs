using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Memberships.Models
{
    public class UserViewModel
    {
        [DisplayName("User Id")]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public int Email { get; set; }

        [Display(Name = "First Name")]
        [StringLength(30, ErrorMessage = "The {0} must be at least {1} character long")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}