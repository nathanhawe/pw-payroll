using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.UnitTest.Helpers
{
	public class GrossFromPiecesTestCase
	{
		public int Id { get; set; }
		public decimal Pieces { get; set; }
		public decimal PieceRate { get; set; }
		public decimal ExpectedGrossFromPieces { get; set; }
	}
}
