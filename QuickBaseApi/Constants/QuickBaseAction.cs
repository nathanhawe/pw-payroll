using System;
using System.Collections.Generic;
using System.Text;

namespace QuickBase.Api.Constants
{
	/// <summary>
	/// Supported Quick Base API actions.
	/// </summary>
	internal enum QuickBaseAction
	{
		Unknown = 0,
		API_DoQuery,
		API_DoQueryCount,
		API_ImportFromCSV
	}
}
