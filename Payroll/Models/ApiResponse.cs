using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payroll.Models
{
	public class Pagination
	{
		public int Offset { get; set; }
		public int Limit { get; set; }
		public int Total { get; set; }
		public bool OrderByDescending { get; set; }
	}

	public class ApiResponse<T>
	{
		public Pagination Pagination { get; set; }
		public T Data { get; set; }
		public Dictionary<string, List<string>> Errors { get; set; }
	}
}
