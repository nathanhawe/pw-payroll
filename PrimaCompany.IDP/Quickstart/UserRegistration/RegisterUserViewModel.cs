using System.ComponentModel.DataAnnotations;

namespace PrimaCompany.IDP.UserRegistration
{
	public class RegisterUserViewModel
	{
		[Required]
		[MaxLength(200)]
		[Display(Name = "Username")]
		public string Username { get; set; }

		[Required]
		[MinLength(8)]
		[MaxLength(200)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare(nameof(Password))]
		public string ConfirmPassword { get; set; }

		[Required]
		[MaxLength(250)]
		[Display(Name = "Given name")]
		public string GivenName { get; set; }

		[Required]
		[MaxLength(250)]
		[Display(Name = "Family name")]
		public string FamilyName { get; set; }

		[Required]
		[MaxLength(200)]
		[Display(Name = "Email")]
		[EmailAddress]
		public string Email { get; set; }

		public string ReturnUrl { get; set; }
	}
}
