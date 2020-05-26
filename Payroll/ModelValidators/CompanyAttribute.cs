using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Payroll.ModelValidators
{
	public class CompanyAttribute : ValidationAttribute
	{
		public CompanyAttribute()
		{

		}

		public string GetErrorMessage() => $"String must be a valid company such as '{Payroll.Domain.Constants.Company.Plants}' or '{Payroll.Domain.Constants.Company.Ranches}'.";

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value is string)
			{
				var temp = (string)value;
				if(temp == Payroll.Domain.Constants.Company.Plants || temp == Payroll.Domain.Constants.Company.Ranches)
				{
					return ValidationResult.Success;
				}
			}

			return new ValidationResult(GetErrorMessage());
		}
	}
}
