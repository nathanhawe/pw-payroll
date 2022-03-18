namespace Payroll.Domain.Constants.QuickBase
{
	/// <summary>
	/// Fields used in the Bonus Piece Rates table in Quick Base
	/// </summary>
	public enum RanchBonusPieceRateField
	{
		Unknown = 0,
		RecordId = 3,

		/// <summary>
		/// [Per Hour Threshold] - Numeric
		/// </summary>
		PerHourThreshold = 6,

		/// <summary>
		/// [Per Tree Bonus] - Currency
		/// </summary>
		PerTreeBonus = 7,
		
		/// <summary>
		/// [Effective Date] - Date
		/// </summary>
		EffectiveDate = 8,

		/// <summary>
		/// [Related Block] - Numeric Reference
		/// </summary>
		RelatedBlock = 9,

		/// <summary>
		/// [Related Labor Code] - Numeric Reference
		/// </summary>
		RelatedLaborCode = 15,
	}
}
