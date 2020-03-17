using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Provide consistent rounding behavior to the application
	/// </summary>
	public class RoundingService : IRoundingService
	{
		/// <summary>
		/// Rounds the provided value to the decimal places specified.  This method ensures
		/// that rounding is performed accurately for six decimal places.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="decimalPlaces"></param>
		/// <returns></returns>
		public decimal Round(decimal value, int decimalPlaces)
		{
			for (int i = 6; i > decimalPlaces; i--)
			{
				value = Math.Round(value, i, MidpointRounding.AwayFromZero);
			}
			return Math.Round(value, decimalPlaces, MidpointRounding.AwayFromZero);
		}
	}
}
