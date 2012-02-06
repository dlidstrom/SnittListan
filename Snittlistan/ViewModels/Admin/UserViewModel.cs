﻿namespace Snittlistan.ViewModels.Admin
{
    using System.ComponentModel.DataAnnotations;

    public class UserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "E-postadress")]
        public string Email { get; set; }

        [Display(Name = "Aktiv")]
        public bool IsActive { get; set; }
    }
}