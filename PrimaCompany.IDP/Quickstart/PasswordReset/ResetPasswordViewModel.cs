using System.ComponentModel.DataAnnotations;

namespace PrimaCompany.IDP.PasswordReset
{
	public class ResetPasswordViewModel
	{
		[Required]
		[MinLength(8)]
		[MaxLength(200)]
		[DataType(DataType.Password)]
		[Display(Name = "Your new password")]
		public string Password { get; set; }

		[Required]
		[MinLength(8)]
		[MaxLength(200)]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare(nameof(Password))]
		public string ConfirmPassword { get; set; }

		public string SecurityCode { get; set; }
	}
}
