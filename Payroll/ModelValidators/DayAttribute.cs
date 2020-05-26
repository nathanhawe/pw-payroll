using System;
using System.ComponentModel.DataAnnotations;

namespace Payroll.ModelValidators
{
	public class DayAttribute : ValidationAttribute
	{
		public DayOfWeek Day { get; }

		public DayAttribute(DayOfWeek dayOfWeek)
		{
			Day = dayOfWeek;
		}

		public string GetErrorMessage() => $"Date must be a {Day}.";

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if(value is DateTime)
			{
				if (((DateTime)value).DayOfWeek == Day) return ValidationResult.Success;
			}
			return new ValidationResult(GetErrorMessage());
		}
	}
}
