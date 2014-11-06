using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AskMeAQuestion.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "User Name")]
        [Required(ErrorMessage = "*")]
        public String UserName { get; set; }
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "*")]
        public String Password { get; set; }
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage="*")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage="Password inputs do not match")]
        public String ConfirmPassword { get; set; }
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "*")]
        public String FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "*")]
        public String LastName { get; set; }
        [Display(Name = "Account Type")]
        [Required(ErrorMessage = "*")]
        public String AccountType { get; set; }

        public List<SelectListItem> AccountTypeList { get; set; }
    }
}