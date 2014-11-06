using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AskMeAQuestion.ViewModels
{
    public class IndexViewModel
    {
        [Display(Name="User Name")]
        [Required(ErrorMessage="*")]
        public String UserName { get; set; }
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "*")]
        public String Password { get; set; }
    }
}