using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Payroll.Data;
using Payroll.Domain.Constants;
using Payroll.Service;
using Payroll.Service.Interface;
using Payroll.UnitTest.Mocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.UnitTest
{
	[TestClass]
	public class RanchHourlyRateSelectorTests
	{
		private readonly decimal _crewLaborRate = 15M;
		private ICrewLaborWageSelector _mockCrewLaborWageSelector;
		private RanchHourlyRateSelector _ranchHourlyRateSelector;

		[TestInitialize]
		public void Setup()
		{
			// Create a mock of the crew labor wage selector that always returns the crew labor rate
			_mockCrewLaborWageSelector = new MockCrewLaborWageSelector(_crewLaborRate);

			// Setup common instance of rate selector to test
			_ranchHourlyRateSelector = new RanchHourlyRateSelector(_mockCrewLaborWageSelector);
		}

		private decimal DefaultTest(
			string payType = Payroll.Domain.Constants.PayType.Regular,
			int crew = 100,
			int laborCode = -1,
			decimal employeeHourlyRate = 14,
			decimal hourlyRateOverride = 0,
			DateTime? shiftDate = null)
		{
			shiftDate ??= new DateTime(2020, 1, 1);
			return _ranchHourlyRateSelector.GetHourlyRate(payType, crew, laborCode, employeeHourlyRate, hourlyRateOverride, shiftDate.Value);
		}

		[TestMethod]
		public void HourlyRateOverride_ReturnsHourlyRateOverride()
		{
			var payType = Payroll.Domain.Constants.PayType.Regular;
			var crew = 100;
			var laborCode = -1;
			var employeeHourly = 15;
			var hourlyOverride = 42;
			var shiftDate = new DateTime(2020, 1, 1);

			var hourlyRate = _ranchHourlyRateSelector.GetHourlyRate(payType, crew, laborCode, employeeHourly, hourlyOverride, shiftDate);
			Assert.AreEqual(hourlyOverride, hourlyRate);
		}

		[TestMethod]
		public void HourlyRateOverride_SupercedesLaborCodeAndCrew()
		{

			var payType = Payroll.Domain.Constants.PayType.Regular;
			var crew = 100;
			var laborCode = (int)RanchLaborCode.GrapeHarvestCrewHelper;
			var employeeHourly = 15;
			var hourlyOverride = 42;
			var shiftDate = new DateTime(2020, 1, 1);

			// Labor code 116 rate is currently set to 12 dollars but hourly rate override will ensure it is 42
			var hourlyRate = _ranchHourlyRateSelector.GetHourlyRate(payType, crew, laborCode, employeeHourly, hourlyOverride, shiftDate);
			Assert.AreEqual(hourlyOverride, hourlyRate);

			// Crew 100 should receive the regular crew labor rate of 15 but hourly rate override will ensure it is 42
			laborCode = -1;
			hourlyRate = _ranchHourlyRateSelector.GetHourlyRate(payType, crew, laborCode, employeeHourly, hourlyOverride, shiftDate);
			Assert.AreEqual(hourlyOverride, hourlyRate);
		}

		#region Pay Type Tests

		[TestMethod]
		public void PayTypeRegular_ReturnsNonZero()
		{
			Assert.IsTrue(0 < DefaultTest(payType: Payroll.Domain.Constants.PayType.Regular));
		}

		[TestMethod]
		public void PayTypeOverTime_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.OverTime));
		}

		[TestMethod]
		public void PayTypeDoubleTime_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.DoubleTime));
		}

		[TestMethod]
		public void PayTypePieces_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.Pieces));
		}

		[TestMethod]
		public void PayTypeHourlyPlusPieces_ReturnsNonZero()
		{
			Assert.IsTrue(0 < DefaultTest(payType: Payroll.Domain.Constants.PayType.HourlyPlusPieces));
		}

		[TestMethod]
		public void PayTypeProductivityOnlyPieces_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.ProductivityOnlyPieces));
		}

		[TestMethod]
		public void PayTypeMinimumAssurance_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.MinimumAssurance));
		}

		[TestMethod]
		public void PayTypeMinimumAssuranceRegular_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.MinimumAssurance_Regular));
		}

		[TestMethod]
		public void PayTypeMinimumAssuranceOT_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.MinimumAssurance_OverTime));
		}

		[TestMethod]
		public void PayTypeMinimumAssuranceDT_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.MinimumAssurance_DoubleTime));
		}

		[TestMethod]
		public void PayTypeMinimumAssuranceWOT_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.MinimumAssurance_WeeklyOverTime));
		}

		[TestMethod]
		public void PayTypeVacation_ReturnsNonZero()
		{
			Assert.IsTrue(0 < DefaultTest(payType: Payroll.Domain.Constants.PayType.Vacation));
		}

		[TestMethod]
		public void PayTypeHoliday_ReturnsNonZero()
		{
			Assert.IsTrue(0 < DefaultTest(payType: Payroll.Domain.Constants.PayType.Holiday));
		}

		[TestMethod]
		public void PayTypeBereavement_ReturnsNonZero()
		{
			Assert.IsTrue(0 < DefaultTest(payType: Payroll.Domain.Constants.PayType.Bereavement));
		}

		[TestMethod]
		public void PayTypeSickLeave_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.SickLeave));
		}

		[TestMethod]
		public void PayTypeCrewBossDaily_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.CBDaily));
		}

		[TestMethod]
		public void PayTypeCrewBossPerWorker_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.CBPerWorker));
		}

		[TestMethod]
		public void PayTypeCrewBossHourlyTrees_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.CBHourlyTrees));
		}

		[TestMethod]
		public void PayTypeCrewBossHourlyVines_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.CBHourlyVines));
		}

		[TestMethod]
		public void PayTypeCrewBossSouthDaily_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.CBSouthDaily));
		}

		[TestMethod]
		public void PayTypeCrewBossSouthHourly_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.CBSouthHourly));
		}

		[TestMethod]
		public void PayTypeCommission_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.CBCommission));
		}

		[TestMethod]
		public void PayTypeWeeklyOverTime_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.WeeklyOverTime));
		}

		[TestMethod]
		public void PayTypeAdjustment_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.Adjustment));
		}

		[TestMethod]
		public void PayTypeSpecialAdjustment_ReturnsNonZero()
		{
			Assert.IsTrue(0 < DefaultTest(payType: Payroll.Domain.Constants.PayType.SpecialAdjustment));
		}

		[TestMethod]
		public void PayTypeCompTime_ReturnsNonZero()
		{
			Assert.IsTrue(0 < DefaultTest(payType: Payroll.Domain.Constants.PayType.CompTime));
		}

		[TestMethod]
		public void PayTypeReportingPay_ReturnsNonZero()
		{
			Assert.IsTrue(0 < DefaultTest(payType: Payroll.Domain.Constants.PayType.ReportingPay));
		}

		[TestMethod]
		public void PayTypeReportPayAdjustment_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.ReportingPayAdjustment));
		}

		[TestMethod]
		public void PayTypeBonus_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.Bonus));
		}
		#endregion

		#region Labor Code Tests

		[TestMethod]
		public void LaborCode_103_AlmostHarvestEquipmentOperatorDay()
		{
			var laborCode = (int)RanchLaborCode.AlmondHarvestEquipmentOperatorDay;
			// [Labor Code]=103 => If([Employee Hourly Rate]>[Crew Labor Rate],[Employee Hourly Rate],14.25)
			Assert.AreEqual(14.25M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(14.25M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(_crewLaborRate + 1, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1));
		}

		[TestMethod]
		public void LaborCode_104_AlmondHarvestEquipmentOperatorNight()
		{
			var laborCode = (int)RanchLaborCode.AlmondHarvestEquipmentOperatorNight;
			// [LC104Rate] = If([Employee Hourly Rate]>[Crew Labor Rate],[Employee Hourly Rate]+1,15.25)
			Assert.AreEqual(15.25M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(15.25M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(_crewLaborRate + 2, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1));
		}

		[TestMethod]
		public void LaborCode_105_AlmondHarvestGeneral()
		{
			var laborCode = (int)RanchLaborCode.AlmondHarvestGeneral;
			// [LC105Rate] = If([Crew]=65,If([Employee Hourly Rate]>[Crew Labor Rate],[Employee Hourly Rate],14),If([Employee Hourly Rate]>[Crew Labor Rate],[Employee Hourly Rate],[Crew Labor Rate]))
			Assert.AreEqual(14M, DefaultTest(laborCode: laborCode, crew: (int)Crew.AlmondHarvest_Nights, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(14M, DefaultTest(laborCode: laborCode, crew: (int)Crew.AlmondHarvest_Nights, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(_crewLaborRate + 1, DefaultTest(laborCode: laborCode, crew: (int)Crew.AlmondHarvest_Nights, employeeHourlyRate: _crewLaborRate + 1));

			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(_crewLaborRate + 1, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1));
		}

		[TestMethod]
		public void LaborCode_116_GrapeHarvestCrewHelper()
		{
			var laborCode = (int)RanchLaborCode.GrapeHarvestCrewHelper;
			// [LC116Rate] = 12
			Assert.AreEqual(12M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(12M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(12M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1));
		}

		[TestMethod]
		public void LaborCode_117_GrapeHarvestCrewHelper_BonusRate()
		{
			var laborCode = (int)RanchLaborCode.GrapeHarvestCrewHelper_BonusRate;
			// [LC117Rate] = NULL
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1));
		}

		[TestMethod]
		public void LaborCode_120_GrapeHarvestSupport()
		{
			var laborCode = (int)RanchLaborCode.GrapeHarvestSupport;
			// [Labor Code] = 120 => [CulturalRate]
			// [CulturalRate] = If([Employee Hourly Rate]<[Crew Labor Rate],[Crew Labor Rate],[Employee Hourly Rate])
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(_crewLaborRate + 1, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1));
		}

		[TestMethod]
		public void LaborCode_215_Girdling_Before20200321_IsIgnored()
		{
			var laborCode = (int)RanchLaborCode.Girdling;
			// Crew 27 returns _crewLaborRate + .50 (15.50) in the first two instances and _crewLaborRate + 2 (17) in the last
			// since labor code 215 does not take precedence before 3/21/2020
			Assert.AreEqual(_crewLaborRate + .5M, DefaultTest(laborCode: laborCode, crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(_crewLaborRate + .5M, DefaultTest(laborCode: laborCode, crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(_crewLaborRate + 1.5M, DefaultTest(laborCode: laborCode, crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate + 1));
		}

		[TestMethod]
		public void LaborCode_215_Girdling_OnOrAfter20200321_Returns15_50()
		{
			var laborCode = (int)RanchLaborCode.Girdling;
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: new DateTime(2020, 3, 21)));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: new DateTime(2030, 12, 31)));
		}

		[TestMethod]
		public void LaborCode_380_RecoveryTime_ReturnsZero()
		{
			var laborCode = (int)RanchLaborCode.RecoveryTime;
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1));
		}

		[TestMethod]
		public void LaborCode_381_NonProductiveTime_ReturnsZero()
		{
			var laborCode = (int)RanchLaborCode.NonProductiveTime;
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1));
		}

		[TestMethod]
		public void LaborCode_551_QualityControl_Before20200511_IsIgnored()
		{
			var laborCode = (int)RanchLaborCode.QualityControl;
			// Crew 27 returns _crewLaborRate + .50 (15.50) in the first two instances and _crewLaborRate + 2 (17) in the last
			// since labor code 551 does not take precedence before 5/11/2020
			Assert.AreEqual(_crewLaborRate + .5M, DefaultTest(laborCode: laborCode, crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate - 1, shiftDate: new DateTime(2020, 5, 10)));
			Assert.AreEqual(_crewLaborRate + .5M, DefaultTest(laborCode: laborCode, crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate, shiftDate: new DateTime(2020, 5, 10)));
			Assert.AreEqual(_crewLaborRate + 1.5M, DefaultTest(laborCode: laborCode, crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate + 1, shiftDate: new DateTime(2020, 5, 10)));
		}

		[TestMethod]
		public void LaborCode_551_QualityControl_OnOrAfter20200511_ReturnsGreaterOfEmployeeRateAndCrewLaborPlusQuarterDollar()
		{
			var laborCode = (int)RanchLaborCode.QualityControl;
			var effectiveDate = new DateTime(2020, 5, 11);

			// Return crew labor rate + .25 when it is greater than employee hourly rate
			Assert.AreEqual((_crewLaborRate + .25M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: effectiveDate));
			Assert.AreEqual((_crewLaborRate + .25M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: effectiveDate.AddYears(10)));

			// Return the employee hourly rate when it is greater than crew labor rate + .25
			Assert.AreEqual((_crewLaborRate + .5M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_crewLaborRate + .5M), shiftDate: effectiveDate));
			Assert.AreEqual((_crewLaborRate + .5M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_crewLaborRate + .5M), shiftDate: effectiveDate.AddYears(10)));
		}

		[TestMethod]
		public void LaborCode_SupercedesCrew()
		{
			var laborCode = (int)RanchLaborCode.AlmondHarvestEquipmentOperatorNight;
			// Crew 27 would return _crewLaborRate + .50 (15.50) in the first two instances and _crewLaborRate + 2 (17.50) in the last
			// but labor code 104 takes precedence
			Assert.AreEqual(15.25M, DefaultTest(laborCode: laborCode, crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(15.25M, DefaultTest(laborCode: laborCode, crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(_crewLaborRate + 2, DefaultTest(laborCode: laborCode, crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate + 1));
		}

		#endregion

		#region Crew Tests

		[TestMethod]
		public void CrewLabor()
		{
			// Crews over 100 always get the crew labor rate.
			for (int i = 101; i < 1000; i++)
			{
				Assert.AreEqual(_crewLaborRate, DefaultTest(crew: i, employeeHourlyRate: _crewLaborRate - 1));
				Assert.AreEqual(_crewLaborRate, DefaultTest(crew: i, employeeHourlyRate: _crewLaborRate));
				Assert.AreEqual(_crewLaborRate, DefaultTest(crew: i, employeeHourlyRate: _crewLaborRate + 1));
			}
		}

		[TestMethod]
		public void CulturalLabor()
		{
			// Crews that are less than 100 and don't otherwise have exceptions (27, 75, and 76) receive
			// the cultural rate.
			// [CulturalRate] = If([Employee Hourly Rate]<[Crew Labor Rate],[Crew Labor Rate],[Employee Hourly Rate])
			List<int> exceptionCrews = new List<int> { (int)Crew.WestTractor_Night, (int)Crew.LightDuty_East, (int)Crew.LightDuty_West };
			for (int i = 1; i < 100; i++)
			{
				if (exceptionCrews.Contains(i)) continue;
				Assert.AreEqual(_crewLaborRate, DefaultTest(crew: i, employeeHourlyRate: _crewLaborRate - 1));
				Assert.AreEqual(_crewLaborRate, DefaultTest(crew: i, employeeHourlyRate: _crewLaborRate));
				Assert.AreEqual(_crewLaborRate + 1, DefaultTest(crew: i, employeeHourlyRate: _crewLaborRate + 1));
			}
		}

		[TestMethod]
		public void Crew_27_WestTractor_Night()
		{
			// [CulturalRate] + .5
			Assert.AreEqual(_crewLaborRate + .5M, DefaultTest(crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(_crewLaborRate + .5M, DefaultTest(crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(_crewLaborRate + 1.5M, DefaultTest(crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate + 1));
		}

		[TestMethod]
		public void Crew_75_LightDuty_East()
		{
			// Crew 75 always get the crew labor rate.
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: (int)Crew.LightDuty_East, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: (int)Crew.LightDuty_East, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: (int)Crew.LightDuty_East, employeeHourlyRate: _crewLaborRate + 1));
		}

		[TestMethod]
		public void Crew_76_LightDutyWest()
		{
			// Crew 76 always get the crew labor rate.
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: (int)Crew.LightDuty_West, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: (int)Crew.LightDuty_West, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: (int)Crew.LightDuty_West, employeeHourlyRate: _crewLaborRate + 1));
		}

		[TestMethod]
		public void Crew_223_JoseLuisRodriguez_AndGrafting_BuddingExpertCrew()
		{
			var laborCode = (int)RanchLaborCode.Grafting_BuddingExpertCrew;
			var crew = (int)Crew.JoseLuisRodriguez;
			// Crew 223 gets the grafting rate of 15 when using labor code 217, otherwise they get the crew labor rate.
			Assert.AreEqual(15, DefaultTest(crew: crew, laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(15, DefaultTest(crew: crew, laborCode: laborCode, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(15, DefaultTest(crew: crew, laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1));

			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: crew, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: crew, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: crew, employeeHourlyRate: _crewLaborRate + 1));
		}

		#endregion
	}
}
