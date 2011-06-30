﻿using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SnittListan.Models
{
	public class RegisterModel
	{
		[Required]
		[Display(Name = "Förnamn")]
		public string FirstName { get; set; }

		[Required]
		[Display(Name = "Efternamn")]
		public string LastName { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		[Display(Name = "E-postadress")]
		public string Email { get; set; }

		[DataType(DataType.EmailAddress)]
		[Display(Name = "Upprepa e-postadressen")]
		[Compare("Email", ErrorMessage = "Adresserna är olika.")]
		public string ConfirmEmail { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "Ditt lösenord måste vara minst {2} tecken långt.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Lösenord")]
		public string Password { get; set; }
	}
}