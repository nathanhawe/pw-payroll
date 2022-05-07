using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data;
using Payroll.Domain;
using Payroll.Domain.Constants;
using Payroll.Service;
using Payroll.UnitTest.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Payroll.UnitTest
{
	[TestClass]
	public class RanchBonusPayServiceTests
	{
		private RoundingService _roundingService = new RoundingService();

		[TestMethod]
		public void CreatesCorrectIndividualBonusRecords()
		{
			var dbName = "CreatesCorrectIndividualBonusRecords";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			int pendingBlock = 1;
			int excludedBlock = 1075;
			int group1 = 1074;
			int group2 = 682;
			int group3 = 607;
			int group4 = 561;

			string employeeA = "employeeA";
			string employeeB = "employeeB";
			string employeeC = "employeeC";
			string employeeD = "employeeD";

			var weekEndDate = new DateTime(2022, 3, 27);
			var monday = new DateTime(2022, 3, 21);
			var tuesday = new DateTime(2022, 3, 22);
			var wednesday = new DateTime(2022, 3, 23);
			var thursday = new DateTime(2022, 3, 24);
			var friday = new DateTime(2022, 3, 25);
			var saturday = new DateTime(2022, 3, 26);

			// Mock a batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock Ranch Bonus Piece Rates
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 3.5M, perTreeBonus: 2M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: tuesday, perHourThreshold: 3.5M, perTreeBonus: 2.1M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: wednesday, perHourThreshold: 3.5M, perTreeBonus: 2.2M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: thursday, perHourThreshold: 3.75M, perTreeBonus: 2M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: friday, perHourThreshold: 3.75M, perTreeBonus: 2.1M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: saturday, perHourThreshold: 3.75M, perTreeBonus: 2.2M));

			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: excludedBlock, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 0M, perTreeBonus: 0M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group2, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 1.75M, perTreeBonus: 2M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group3, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 1.25M, perTreeBonus: 2M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group4, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 1.5M, perTreeBonus: 2M));

			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, effectiveDate: monday, perHourThreshold: 0, perTreeBonus: .25M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group2, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, effectiveDate: monday, perHourThreshold: 0, perTreeBonus: .25M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group3, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, effectiveDate: monday, perHourThreshold: 0, perTreeBonus: .25M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group4, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, effectiveDate: monday, perHourThreshold: 0, perTreeBonus: .25M));

			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, laborCode: (int)RanchLaborCode.PieceRatePruningSummer, effectiveDate: monday, perHourThreshold: 0, perTreeBonus: .15M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group2, laborCode: (int)RanchLaborCode.PieceRatePruningSummer, effectiveDate: monday, perHourThreshold: 0, perTreeBonus: .15M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group3, laborCode: (int)RanchLaborCode.PieceRatePruningSummer, effectiveDate: monday, perHourThreshold: 0, perTreeBonus: .15M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group4, laborCode: (int)RanchLaborCode.PieceRatePruningSummer, effectiveDate: monday, perHourThreshold: 0, perTreeBonus: .15M));

			// Mock Ranch Group Bonus Piece Rates
			context.Add(EntityMocker.MockRanchGroupBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, effectiveDate: tuesday, perVesselBonus: 0M));
			context.Add(EntityMocker.MockRanchGroupBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote, effectiveDate: tuesday, perVesselBonus: 0M));

			context.Add(EntityMocker.MockRanchGroupBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, effectiveDate: wednesday, perVesselBonus: 0.10M));
			context.Add(EntityMocker.MockRanchGroupBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote, effectiveDate: wednesday, perVesselBonus: 0.12M));

			context.Add(EntityMocker.MockRanchGroupBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, effectiveDate: friday, perVesselBonus: 0.11M));
			context.Add(EntityMocker.MockRanchGroupBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote, effectiveDate: friday, perVesselBonus: 0.13M));

			context.Add(EntityMocker.MockRanchGroupBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, effectiveDate: saturday, perVesselBonus: 0M));

			// Mock Ranch Pay Lines
			/* Employee A */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Thinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: excludedBlock, hoursWorked: 2, pieces: 28, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: pendingBlock, hoursWorked: 2, pieces: 28, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: friday));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));


			/* Employee B */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2.66M, pieces: 5, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2.67M, pieces: 5, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.67M, pieces: 5, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Thinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 4M, pieces: 5, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2M, pieces: 10, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2M, pieces: 10, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2M, pieces: 2.5M, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2M, pieces: 2.5M, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2M, pieces: 5M, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2M, pieces: 5M, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));

			/* Employee C */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2.5M, pieces: 8.75M, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: monday));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2.5M, pieces: 8.75M, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: tuesday));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2.5M, pieces: 8.75M, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: wednesday));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2.5M, pieces: 8.75M, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2.5M, pieces: 8.75M, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2.5M, pieces: 8.75M, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: pendingBlock, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: excludedBlock, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday));

			/* Employee D */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 8, pieces: 100, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Thinning, payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 8, pieces: 100, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning, payType: PayType.HourlyPlusPieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 8, pieces: 100, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning, payType: PayType.Holiday));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 8, pieces: 100, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning, payType: PayType.Bonus));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 8, pieces: 100, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning, payType: PayType.ProductionIncentiveBonus));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 8, pieces: 100, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning, payType: PayType.Vacation));

			context.SaveChanges();

			// Execute test
			var rbpService = new RanchBonusPayService(context, _roundingService);
			int originalCount = context.RanchPayLines.Count();
			List<RanchPayLine> results = rbpService.CalculateRanchBonusPayLines(batch.Id);

			// Compare results
			/* Original record count unchanged */
			Assert.AreEqual(originalCount, context.RanchPayLines.Count());

			/* Employee A */
			Assert.AreEqual(6, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 28 && x.PieceRate == 2 && x.TotalGross == 56).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 14 && x.PieceRate == 2.1M && x.TotalGross == 29.4M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 28 && x.PieceRate == 2.2M && x.TotalGross == 61.6M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 26 && x.PieceRate == 2M && x.TotalGross == 52M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 19.5M && x.PieceRate == 2.1M && x.TotalGross == 40.95M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 26 && x.PieceRate == 2.2M && x.TotalGross == 57.2M).Count());

			/* Employee B */
			Assert.AreEqual(16, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == .34M && x.PieceRate == 2M && x.TotalGross == .68M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group3 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 1.66M && x.PieceRate == 2M && x.TotalGross == 3.32M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == .99M && x.PieceRate == 2M && x.TotalGross == 1.98M).Count());


			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 1.5M && x.PieceRate == 2M && x.TotalGross == 3M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group3 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 2.5M && x.PieceRate == 2M && x.TotalGross == 5M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 3M && x.PieceRate == 2M && x.TotalGross == 6M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group3 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 2.5M && x.PieceRate == 2M && x.TotalGross == 5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 2M && x.PieceRate == 2M && x.TotalGross == 4M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 1.5M && x.PieceRate == 2M && x.TotalGross == 3M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group3 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 5M && x.PieceRate == 2M && x.TotalGross == 10M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 2M && x.PieceRate == 2M && x.TotalGross == 4M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 1.5M && x.PieceRate == 2M && x.TotalGross == 3M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 2M && x.PieceRate == 2M && x.TotalGross == 4M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 1.5M && x.PieceRate == 2M && x.TotalGross == 3M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group3 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 2.5M && x.PieceRate == 2M && x.TotalGross == 5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 4M && x.PieceRate == 2M && x.TotalGross == 8M).Count());


			/* Employee C */
			Assert.AreEqual(10, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 5M && x.PieceRate == .25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group3 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 5M && x.PieceRate == .15M && x.TotalGross == .75M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 5M && x.PieceRate == .25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group3 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 5M && x.PieceRate == .15M && x.TotalGross == .75M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 5M && x.PieceRate == .25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group3 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 5M && x.PieceRate == .15M && x.TotalGross == .75M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 5M && x.PieceRate == .25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group3 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 5M && x.PieceRate == .15M && x.TotalGross == .75M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 5M && x.PieceRate == .25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group3 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 5M && x.PieceRate == .15M && x.TotalGross == .75M).Count());


			/* Employee D */
			Assert.AreEqual(0, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD).Count());

			/* Overall */
			Assert.AreEqual(32, results.Count());
		}

		[TestMethod]
		public void CreatesCorrectGroupBonusRecords()
		{
			var dbName = "CreatesCorrectGroupBonusRecords";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			int excludedBlock = 1075;
			int group1 = 1074;
			int group2 = 682;
			int group3 = 607;
			int group4 = 561;

			string employeeA = "employeeA";
			string employeeB = "employeeB";
			string employeeC = "employeeC";
			string employeeD = "employeeD";

			var weekEndDate = new DateTime(2022, 3, 27);
			var monday = new DateTime(2022, 3, 21);
			var tuesday = new DateTime(2022, 3, 22);
			var wednesday = new DateTime(2022, 3, 23);
			var thursday = new DateTime(2022, 3, 24);
			var friday = new DateTime(2022, 3, 25);
			var saturday = new DateTime(2022, 3, 26);

			// Mock a batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock Ranch Bonus Piece Rates
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 3.5M, perTreeBonus: 2M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: tuesday, perHourThreshold: 3.5M, perTreeBonus: 2.1M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: wednesday, perHourThreshold: 3.5M, perTreeBonus: 2.2M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: thursday, perHourThreshold: 3.75M, perTreeBonus: 2M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: friday, perHourThreshold: 3.75M, perTreeBonus: 2.1M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: saturday, perHourThreshold: 3.75M, perTreeBonus: 2.2M));

			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: excludedBlock, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 0M, perTreeBonus: 0M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group2, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 1.75M, perTreeBonus: 2M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group3, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 1.25M, perTreeBonus: 2M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group4, laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 1.5M, perTreeBonus: 2M));

			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, effectiveDate: monday, perHourThreshold: 0, perTreeBonus: .25M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group2, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, effectiveDate: monday, perHourThreshold: 0, perTreeBonus: .25M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group3, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, effectiveDate: monday, perHourThreshold: 0, perTreeBonus: .25M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group4, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, effectiveDate: monday, perHourThreshold: 0, perTreeBonus: .25M));

			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, laborCode: (int)RanchLaborCode.PieceRatePruningSummer, effectiveDate: monday, perHourThreshold: 0, perTreeBonus: .15M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group2, laborCode: (int)RanchLaborCode.PieceRatePruningSummer, effectiveDate: monday, perHourThreshold: 0, perTreeBonus: .15M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group3, laborCode: (int)RanchLaborCode.PieceRatePruningSummer, effectiveDate: monday, perHourThreshold: 0, perTreeBonus: .15M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group4, laborCode: (int)RanchLaborCode.PieceRatePruningSummer, effectiveDate: monday, perHourThreshold: 0, perTreeBonus: .15M));

			// Mock Ranch Group Bonus Piece Rates
			//context.Add(EntityMocker.MockRanchGroupBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, effectiveDate: tuesday, perVesselBonus: 0M));
			//context.Add(EntityMocker.MockRanchGroupBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote, effectiveDate: tuesday, perVesselBonus: 0M));

			//context.Add(EntityMocker.MockRanchGroupBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, effectiveDate: wednesday, perVesselBonus: 0.10M));
			//context.Add(EntityMocker.MockRanchGroupBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote, effectiveDate: wednesday, perVesselBonus: 0.12M));
			
			//context.Add(EntityMocker.MockRanchGroupBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, effectiveDate: friday, perVesselBonus: 0.11M));
			//context.Add(EntityMocker.MockRanchGroupBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote, effectiveDate: friday, perVesselBonus: 0.13M));

			//context.Add(EntityMocker.MockRanchGroupBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, effectiveDate: saturday, perVesselBonus: 0M));

			// Mock Ranch Pay Lines
			/* Monday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			
			/* Tuesday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Wednesday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Thursday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 15, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.TractorPieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote));

			
			/* Friday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.TractorPieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 20, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 40, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.TractorPieceRateHarvest_Tote, payType: PayType.CBHourlyTrees));


			/* Saturday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Sunday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 8, pieces: 1000, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.TractorPieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2.02M, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateThinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 4.5M, pieces: 50, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 3.5M, pieces: 57, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateThinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 7, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: excludedBlock, hoursWorked: 6, pieces: 500, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.QualityControl));

			context.SaveChanges();

			// Execute test
			var rbpService = new RanchBonusPayService(context, _roundingService);
			int originalCount = context.RanchPayLines.Count();
			List<RanchPayLine> results = rbpService.CalculateRanchBonusPayLines(batch.Id);

			// Compare results
			/* Original record count unchanged */
			Assert.AreEqual(originalCount, context.RanchPayLines.Count());

			/* Monday */
			Assert.AreEqual(8, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == monday).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 2.5M && x.TotalGross == 2.5M).Count());

			/* Tuesday */
			Assert.AreEqual(8, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == tuesday).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 2.5M && x.TotalGross == 2.5M).Count());

			/* Wednesday */
			Assert.AreEqual(4, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == wednesday).Count());
			
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 304.1M && x.PieceRate == .10M && x.OtherGross == 30.41M && x.TotalGross == 30.41M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 303.1M && x.PieceRate == .10M && x.OtherGross == 30.31M && x.TotalGross == 30.31M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 228.1M && x.PieceRate == .10M && x.OtherGross == 22.81M && x.TotalGross == 22.81M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 164.7M && x.PieceRate == .10M && x.OtherGross == 16.47M && x.TotalGross == 16.47M).Count());

			/* Thursday */
			Assert.AreEqual(8, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == thursday).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 304.1M && x.PieceRate == .10M && x.OtherGross == 30.41M && x.TotalGross == 30.41M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 303.1M && x.PieceRate == .10M && x.OtherGross == 30.31M && x.TotalGross == 30.31M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 228.1M && x.PieceRate == .10M && x.OtherGross == 22.81M && x.TotalGross == 22.81M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 164.7M && x.PieceRate == .10M && x.OtherGross == 16.47M && x.TotalGross == 16.47M).Count());
			
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 12M && x.PieceRate == .12M && x.OtherGross == 1.44M && x.TotalGross == 1.44M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 12M && x.PieceRate == .12M && x.OtherGross == 1.44M && x.TotalGross == 1.44M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 12M && x.PieceRate == .12M && x.OtherGross == 1.44M && x.TotalGross == 1.44M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 12M && x.PieceRate == .12M && x.OtherGross == 1.44M && x.TotalGross == 1.44M).Count());

			/* Friday */
			Assert.AreEqual(7, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == friday).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 304.1M && x.PieceRate == .10M && x.OtherGross == 30.41M && x.TotalGross == 30.41M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 303.1M && x.PieceRate == .10M && x.OtherGross == 30.31M && x.TotalGross == 30.31M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 228.1M && x.PieceRate == .10M && x.OtherGross == 22.81M && x.TotalGross == 22.81M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 164.7M && x.PieceRate == .10M && x.OtherGross == 16.47M && x.TotalGross == 16.47M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 14.59M && x.PieceRate == .12M && x.OtherGross == 1.75M && x.TotalGross == 1.75M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 14.73M && x.PieceRate == .12M && x.OtherGross == 1.77M && x.TotalGross == 1.77M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 15.68M && x.PieceRate == .12M && x.OtherGross == 1.88M && x.TotalGross == 1.88M).Count());

			/* Saturday */
			Assert.AreEqual(7, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == saturday).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 304.1M && x.PieceRate == .10M && x.OtherGross == 30.41M && x.TotalGross == 30.41M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 303.1M && x.PieceRate == .10M && x.OtherGross == 30.31M && x.TotalGross == 30.31M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 228.1M && x.PieceRate == .10M && x.OtherGross == 22.81M && x.TotalGross == 22.81M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 164.7M && x.PieceRate == .10M && x.OtherGross == 16.47M && x.TotalGross == 16.47M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 14.59M && x.PieceRate == .12M && x.OtherGross == 1.75M && x.TotalGross == 1.75M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 14.73M && x.PieceRate == .12M && x.OtherGross == 1.77M && x.TotalGross == 1.77M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 15.68M && x.PieceRate == .12M && x.OtherGross == 1.88M && x.TotalGross == 1.88M).Count());

			/* Sunday */
			Assert.AreEqual(4, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 572.3M && x.PieceRate == .12M && x.OtherGross == 68.68M && x.TotalGross == 68.68M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 427.8M && x.PieceRate == .12M && x.OtherGross == 51.34M && x.TotalGross == 51.34M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 39.46M && x.PieceRate == .12M && x.OtherGross == 4.74M && x.TotalGross == 4.74M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 17.54M && x.PieceRate == .12M && x.OtherGross == 2.11M && x.TotalGross == 2.11M).Count());


			/* Overall */
			Assert.AreEqual(46, results.Count());
		}


	}
}
