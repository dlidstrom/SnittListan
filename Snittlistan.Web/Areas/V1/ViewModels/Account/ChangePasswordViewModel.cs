﻿using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Snittlistan.Web.Areas.V1.ViewModels.Account
{
    public class ChangePasswordViewModel
    {
        [HiddenInput]
        public string Email { get; set; }

        [Required(ErrorMessage = "Ange nytt lösenord")]
        [StringLength(100, ErrorMessage = "Ditt lösenord måste vara minst {2} tecken långt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Upprepa lösenordet")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Lösenorden är olika")]
        public string ConfirmPassword { get; set; }
    }
}