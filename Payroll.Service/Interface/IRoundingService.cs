using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service.Interface
{
	public interface IRoundingService
	{
		decimal Round(decimal value, int decimalPlaces);
	}
}
