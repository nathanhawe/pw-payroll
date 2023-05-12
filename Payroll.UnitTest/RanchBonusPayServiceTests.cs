using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data;
using Payroll.Data.QuickBase;
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
		public void CreatesCorrectIndividualBonusRecordsByBlock()
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
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, designation: nameof(group1), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 3.5M, perTreeBonus: 2M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, designation: nameof(group1), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: tuesday, perHourThreshold: 3.5M, perTreeBonus: 2.1M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, designation: nameof(group1), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: wednesday, perHourThreshold: 3.5M, perTreeBonus: 2.2M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, designation: nameof(group1), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: thursday, perHourThreshold: 3.75M, perTreeBonus: 2M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, designation: nameof(group1), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: friday, perHourThreshold: 3.75M, perTreeBonus: 2.1M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group1, designation: nameof(group1), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: saturday, perHourThreshold: 3.75M, perTreeBonus: 2.2M));

			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: excludedBlock, designation: nameof(excludedBlock), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 0M, perTreeBonus: 0M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group2, designation: nameof(group2), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 1.75M, perTreeBonus: 2M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group3, designation: nameof(group3), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 1.25M, perTreeBonus: 2M));
			context.Add(EntityMocker.MockRanchBonusPieceRate(batchId: batch.Id, blockId: group4, designation: nameof(group4), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 1.5M, perTreeBonus: 2M));

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
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: monday));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2.5M, pieces: 8.75M, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: tuesday));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2.5M, pieces: 8.75M, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: wednesday));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2.5M, pieces: 8.75M, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2.5M, pieces: 8.75M, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2.5M, pieces: 8.75M, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: pendingBlock, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: excludedBlock, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Summer));
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
			List<RanchPayLine> results = rbpService.CalculateRanchBonusPayLines(batch.Id, weekEndDate);

			// Compare results
			/* Original record count unchanged */
			Assert.AreEqual(originalCount, context.RanchPayLines.Count());

			/* Employee A */
			Assert.AreEqual(6, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.Designation == nameof(group1) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 28 && x.PieceRate == 2 && x.OtherGross == 0 && x.GrossFromPieces == 56 && x.TotalGross == 56).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.Designation == nameof(group1) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 14 && x.PieceRate == 2.1M && x.OtherGross == 0 && x.GrossFromPieces == 29.4M && x.TotalGross == 29.4M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group1 && x.Designation == nameof(group1) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 28 && x.PieceRate == 2.2M && x.OtherGross == 0 && x.GrossFromPieces == 61.6M && x.TotalGross == 61.6M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.Designation == nameof(group1) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 26 && x.PieceRate == 2M && x.OtherGross == 0 && x.GrossFromPieces == 52 && x.TotalGross == 52M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group1 && x.Designation == nameof(group1) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 19.5M && x.PieceRate == 2.1M && x.OtherGross == 0 && x.GrossFromPieces == 40.95M && x.TotalGross == 40.95M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.Designation == nameof(group1) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 26 && x.PieceRate == 2.2M && x.OtherGross == 0 && x.GrossFromPieces == 57.2M && x.TotalGross == 57.2M).Count());

			/* Employee B */
			Assert.AreEqual(16, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.Designation == nameof(group2) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == .34M && x.PieceRate == 2M && x.OtherGross == 0 && x.GrossFromPieces == .68M && x.TotalGross == .68M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group3 && x.Designation == nameof(group3) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 1.66M && x.PieceRate == 2M && x.OtherGross == 0 && x.GrossFromPieces == 3.32M && x.TotalGross == 3.32M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group4 && x.Designation == nameof(group4) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == .99M && x.PieceRate == 2M && x.OtherGross == 0 && x.GrossFromPieces == 1.98M && x.TotalGross == 1.98M).Count());


			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.Designation == nameof(group2) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 1.5M && x.PieceRate == 2M && x.OtherGross == 0 && x.GrossFromPieces == 3 && x.TotalGross == 3M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group3 && x.Designation == nameof(group3) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 2.5M && x.PieceRate == 2M && x.OtherGross == 0 && x.GrossFromPieces == 5 && x.TotalGross == 5M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group2 && x.Designation == nameof(group2) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 3M && x.PieceRate == 2M && x.OtherGross == 0 && x.GrossFromPieces == 6 && x.TotalGross == 6M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group3 && x.Designation == nameof(group3) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 2.5M && x.PieceRate == 2M && x.OtherGross == 0 && x.GrossFromPieces == 5 && x.TotalGross == 5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group4 && x.Designation == nameof(group4) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 2M && x.PieceRate == 2M && x.OtherGross == 0 && x.GrossFromPieces == 4 && x.TotalGross == 4M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group2 && x.Designation == nameof(group2) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 1.5M && x.PieceRate == 2M && x.OtherGross == 0 && x.GrossFromPieces == 3 && x.TotalGross == 3M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group3 && x.Designation == nameof(group3) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 5M && x.PieceRate == 2M && x.OtherGross == 0 && x.GrossFromPieces == 10 && x.TotalGross == 10M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.Designation == nameof(group4) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 2M && x.PieceRate == 2M && x.OtherGross == 0 && x.GrossFromPieces == 4 && x.TotalGross == 4M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group2 && x.Designation == nameof(group2) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 1.5M && x.PieceRate == 2M && x.OtherGross == 0 && x.GrossFromPieces == 3 && x.TotalGross == 3M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.Designation == nameof(group4) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 2M && x.PieceRate == 2M && x.OtherGross == 0 && x.GrossFromPieces == 4 && x.TotalGross == 4M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group2 && x.Designation == nameof(group2) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 1.5M && x.PieceRate == 2M && x.OtherGross == 0 && x.GrossFromPieces == 3 && x.TotalGross == 3M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group3 && x.Designation == nameof(group3) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 2.5M && x.PieceRate == 2M && x.OtherGross == 0 && x.GrossFromPieces == 5 && x.TotalGross == 5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.Designation == nameof(group4) && x.LaborCode == (int)RanchLaborCode.Thinning && x.Pieces == 4M && x.PieceRate == 2M && x.OtherGross == 0 && x.GrossFromPieces == 8 && x.TotalGross == 8M).Count());


			/* Employee C */
			Assert.AreEqual(0, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.WeekEndDate == weekEndDate).Count());

			/* Employee D */
			Assert.AreEqual(0, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.WeekEndDate == weekEndDate).Count());

			/* Overall */
			Assert.AreEqual(22, results.Count());
		}

		[TestMethod]
		public void CreatesCorrectGroupHarvestBonusRecords()
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
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

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
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 15, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.TractorPieceRateHarvest_Tote, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote, crewLocation: (int)Location.South));

			
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
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRateHarvest_Tote, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

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
			List<RanchPayLine> results = rbpService.CalculateRanchBonusPayLines(batch.Id, weekEndDate);

			// Compare results
			/* Original record count unchanged */
			Assert.AreEqual(originalCount, context.RanchPayLines.Count());

			/* Monday */
			Assert.AreEqual(8, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == monday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());

			/* Tuesday */
			Assert.AreEqual(8, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == tuesday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());

			/* Wednesday */
			Assert.AreEqual(4, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == wednesday && x.WeekEndDate == weekEndDate).Count());
			
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 304.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 30.41M && x.TotalGross == 30.41M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 303.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 30.31M && x.TotalGross == 30.31M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 228.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 22.81M && x.TotalGross == 22.81M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 164.7M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 16.47M && x.TotalGross == 16.47M).Count());

			/* Thursday */
			Assert.AreEqual(8, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == thursday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 304.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 30.41M && x.TotalGross == 30.41M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 303.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 30.31M && x.TotalGross == 30.31M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 228.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 22.81M && x.TotalGross == 22.81M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 164.7M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 16.47M && x.TotalGross == 16.47M).Count());
			
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 12M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 1.44M && x.TotalGross == 1.44M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 12M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 1.44M && x.TotalGross == 1.44M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 12M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 1.44M && x.TotalGross == 1.44M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 12M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 1.44M && x.TotalGross == 1.44M).Count());

			/* Friday */
			Assert.AreEqual(7, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == friday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 304.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 30.41M && x.TotalGross == 30.41M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 303.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 30.31M && x.TotalGross == 30.31M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 228.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 22.81M && x.TotalGross == 22.81M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 164.7M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 16.47M && x.TotalGross == 16.47M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 14.59M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 1.75M && x.TotalGross == 1.75M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 14.73M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 1.77M && x.TotalGross == 1.77M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 15.68M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 1.88M && x.TotalGross == 1.88M).Count());

			/* Saturday */
			Assert.AreEqual(7, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == saturday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 304.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 30.41M && x.TotalGross == 30.41M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 303.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 30.31M && x.TotalGross == 30.31M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 228.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 22.81M && x.TotalGross == 22.81M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 164.7M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 16.47M && x.TotalGross == 16.47M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 14.59M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 1.75M && x.TotalGross == 1.75M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 14.73M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 1.77M && x.TotalGross == 1.77M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 15.68M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 1.88M && x.TotalGross == 1.88M).Count());

			/* Sunday */
			Assert.AreEqual(4, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == weekEndDate && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 572.3M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 68.68M && x.TotalGross == 68.68M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 427.8M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 51.34M && x.TotalGross == 51.34M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 39.46M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 4.74M && x.TotalGross == 4.74M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote && x.Pieces == 17.54M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 2.11M && x.TotalGross == 2.11M).Count());


			/* Overall */
			Assert.AreEqual(46, results.Count());
		}

		[TestMethod]
		public void CreatesCorrectIndividualAndTractorDriverBonusRecords()
		{
			var dbName = "CreatesCorrectIndividualAndTractorDriverBonusRecords";
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
			string employeeLightDuty = "employeeLightDuty";

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


			// Mock Ranch Pay Lines
			/* Monday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 1, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 1, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 1, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 1, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_LightDuty_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_LightDuty_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Tuesday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_LightDuty_PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_LightDuty_PieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			/* Wednesday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Thursday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 15, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_LightDuty_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_LightDuty_PieceRateHarvest_Tote));

			/* Friday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 20, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket, payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 40, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote, payType: PayType.CBHourlyTrees));


			/* Saturday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Individual_LightDuty_PieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Individual_LightDuty_PieceRateHarvest_Tote));

			/* Sunday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 8, pieces: 1000, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Tote));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2.02M, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateThinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 4.5M, pieces: 50, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Tote));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 3.5M, pieces: 57, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateThinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 7, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Individual_PieceRateHarvest_Tote));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: excludedBlock, hoursWorked: 6, pieces: 500, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.QualityControl));

			context.SaveChanges();

			// Execute test
			var rbpService = new RanchBonusPayService(context, _roundingService);
			int originalCount = context.RanchPayLines.Count();
			List<RanchPayLine> results = rbpService.CalculateRanchBonusPayLines(batch.Id, weekEndDate);

			// Compare results
			/* Original record count unchanged */
			Assert.AreEqual(originalCount, context.RanchPayLines.Count());

			/* Monday */
			Assert.AreEqual(10, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == monday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket && x.Pieces == 150M && x.PieceRate == .005M && x.OtherGross == 0 && x.GrossFromPieces == 0.75M && x.TotalGross == 0.75M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_LightDuty_PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket && x.Pieces == 150M && x.PieceRate == .005M && x.OtherGross == 0 && x.GrossFromPieces == 0.75M && x.TotalGross == 0.75M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Individual_LightDuty_PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());

			/* Tuesday - All crew South */
			Assert.AreEqual(10, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == tuesday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket && x.Pieces == 125M && x.PieceRate == .01M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_LightDuty_PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket && x.Pieces == 125M && x.PieceRate == .01M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Individual_LightDuty_PieceRateHarvest_Bucket && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());

			/* Wednesday */
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == wednesday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket && x.Pieces == 1000M && x.PieceRate == .005M && x.OtherGross == 0 && x.GrossFromPieces == 5M && x.TotalGross == 5M).Count());

			/* Thursday - TD South */
			Assert.AreEqual(8, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == thursday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket && x.Pieces == 1000M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 100M && x.TotalGross == 100M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket && x.Pieces == 2000M && x.PieceRate == .01M && x.OtherGross == 0 && x.GrossFromPieces == 20M && x.TotalGross == 20M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_LightDuty_PieceRateHarvest_Bucket && x.Pieces == 1000M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 100M && x.TotalGross == 100M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Tote && x.Pieces == 25M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 3M && x.TotalGross == 3M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote && x.Pieces == 73M && x.PieceRate == .01M && x.OtherGross == 0 && x.GrossFromPieces == 0.73M && x.TotalGross == 0.73M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Tote && x.Pieces == 5M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 0.6M && x.TotalGross == 0.6M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Tote && x.Pieces == 3M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 0.36M && x.TotalGross == 0.36M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Individual_LightDuty_PieceRateHarvest_Tote && x.Pieces == 25M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 3M && x.TotalGross == 3M).Count());

			/* Friday */
			Assert.AreEqual(4, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == friday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket && x.Pieces == 1000M && x.PieceRate == .005M && x.OtherGross == 0 && x.GrossFromPieces == 5M && x.TotalGross == 5M).Count());
			
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote && x.Pieces == 45M && x.PieceRate == .005M && x.OtherGross == 0 && x.GrossFromPieces == 0.23M && x.TotalGross == 0.23M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Tote && x.Pieces == 15M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 1.8M && x.TotalGross == 1.8M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Tote && x.Pieces == 5M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 0.6M && x.TotalGross == 0.6M).Count());

			/* Saturday - TD South */
			Assert.AreEqual(7, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == saturday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket && x.Pieces == 1000M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 100M && x.TotalGross == 100M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket && x.Pieces == 2000M && x.PieceRate == .01M && x.OtherGross == 0 && x.GrossFromPieces == 20M && x.TotalGross == 20M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_LightDuty_PieceRateHarvest_Bucket && x.Pieces == 1000M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 100M && x.TotalGross == 100M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Tote && x.Pieces == 25M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 3M && x.TotalGross == 3M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Tote && x.Pieces == 15M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 1.8M && x.TotalGross == 1.8M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Tote && x.Pieces == 5M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 0.6M && x.TotalGross == 0.6M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Individual_LightDuty_PieceRateHarvest_Tote && x.Pieces == 25M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 3M && x.TotalGross == 3M).Count());

			/* Sunday */
			Assert.AreEqual(3, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == weekEndDate && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote && x.Pieces == 1000M && x.PieceRate == .005M && x.OtherGross == 0 && x.GrossFromPieces == 5M && x.TotalGross == 5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Tote && x.Pieces == 50M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 6M && x.TotalGross == 6M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Tote && x.Pieces == 7M && x.PieceRate == .12M && x.OtherGross == 0 && x.GrossFromPieces == 0.84M && x.TotalGross == 0.84M).Count());


			/* Overall */
			Assert.AreEqual(43, results.Count());
		}


		[TestMethod]
		public void CreatesCorrectIndividualWinterPruningBonusRecordsForWinterPruningPlusBonus_Before_20221212()
		{
			var dbName = "CreatesCorrectIndividualWinterPruningBonusRecordsForWinterPruningPlusBonus_Before_20221212";
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
			string employeeLightDuty = "employeeLightDuty";

			var weekEndDate = new DateTime(2022, 12, 11);
			var monday = new DateTime(2022, 12, 5);
			var tuesday = new DateTime(2022, 12, 6);
			var wednesday = new DateTime(2022, 12, 7);
			var thursday = new DateTime(2022, 12, 8);
			var friday = new DateTime(2022, 12, 9);
			var saturday = new DateTime(2022, 12, 10);

			// Mock a batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock Ranch Pay Lines
			/* Monday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Tuesday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			/* Wednesday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Thursday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			/* Friday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 20, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote, payType: PayType.CBHourlyTrees));


			/* Saturday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			/* Sunday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 8, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2.02M, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateThinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 4.5M, pieces: 50, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 3.5M, pieces: 57, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateThinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 7, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: excludedBlock, hoursWorked: 6, pieces: 500, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.QualityControl));

			context.SaveChanges();

			// Execute test
			var rbpService = new RanchBonusPayService(context, _roundingService);
			int originalCount = context.RanchPayLines.Count();
			List<RanchPayLine> results = rbpService.CalculateRanchBonusPayLines(batch.Id, weekEndDate);

			// Compare results
			/* Original record count unchanged */
			Assert.AreEqual(originalCount, context.RanchPayLines.Count());

			/* Monday */
			Assert.AreEqual(8, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == monday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());

			/* Tuesday - All crew South */
			Assert.AreEqual(8, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == tuesday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());

			/* Wednesday */
			Assert.AreEqual(0, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == wednesday && x.WeekEndDate == weekEndDate).Count());


			/* Thursday - TD South */
			Assert.AreEqual(6, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == thursday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 1000M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 250M && x.TotalGross == 250M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 1000M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 250M && x.TotalGross == 250M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 5M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 3M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 0.75M && x.TotalGross == 0.75M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());

			/* Friday */
			Assert.AreEqual(2, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == friday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 15M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 3.75M && x.TotalGross == 3.75M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 5M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());

			/* Saturday - TD South */
			Assert.AreEqual(6, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == saturday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 1000M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 250M && x.TotalGross == 250M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 1000M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 250M && x.TotalGross == 250M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 15M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 3.75M && x.TotalGross == 3.75M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 5M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 25M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 6.25M && x.TotalGross == 6.25M).Count());

			/* Sunday */
			Assert.AreEqual(2, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == weekEndDate && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 50M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 12.5M && x.TotalGross == 12.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 7M && x.PieceRate == .25M && x.OtherGross == 0 && x.GrossFromPieces == 1.75M && x.TotalGross == 1.75M).Count());


			/* Overall */
			Assert.AreEqual(32, results.Count());
		}

		[TestMethod]
		public void CreatesNoNonVariableWinterPruningBonusRecordsForWinterPruningPlusBonus_OnOrAfter_20221212()
		{
			var dbName = "CreatesNoNonVariableWinterPruningBonusRecordsForWinterPruningPlusBonus_OnOrAfter_20221212";
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
			string employeeLightDuty = "employeeLightDuty";

			var weekEndDate = new DateTime(2022, 12, 18);
			var monday = new DateTime(2022, 12, 12);
			var tuesday = new DateTime(2022, 12, 13);
			var wednesday = new DateTime(2022, 12, 14);
			var thursday = new DateTime(2022, 12, 15);
			var friday = new DateTime(2022, 12, 16);
			var saturday = new DateTime(2022, 12, 17);

			// Mock a batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock Ranch Pay Lines
			/* Monday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Tuesday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			/* Wednesday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Thursday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			/* Friday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 20, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter, payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote, payType: PayType.CBHourlyTrees));


			/* Saturday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));

			/* Sunday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 8, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2.02M, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateThinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 4.5M, pieces: 50, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 3.5M, pieces: 57, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateThinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 7, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRatePruningWinter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: excludedBlock, hoursWorked: 6, pieces: 500, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.QualityControl));

			context.SaveChanges();

			// Execute test
			var rbpService = new RanchBonusPayService(context, _roundingService);
			int originalCount = context.RanchPayLines.Count();
			List<RanchPayLine> results = rbpService.CalculateRanchBonusPayLines(batch.Id, weekEndDate);

			// Compare results
			/* Original record count unchanged */
			Assert.AreEqual(originalCount, context.RanchPayLines.Count());

			
			/* Overall */
			Assert.AreEqual(0, results.Count());
		}

		[TestMethod]
		public void CreatesNoIndividualWinterPruningBonusRecordsForHourlyPruning()
		{
			var dbName = "CreatesNoIndividualWinterPruningBonusRecordsForHourlyPruning";
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
			string employeeLightDuty = "employeeLightDuty";

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

			// Mock Ranch Pay Lines
			/* Monday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Tuesday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Winter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Winter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Winter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Winter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Winter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Winter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Winter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Winter, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			/* Wednesday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Thursday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Winter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Winter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Winter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Winter));

			/* Friday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Pruning_Winter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Pruning_Winter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 20, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Pruning_Winter, payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote, payType: PayType.CBHourlyTrees));


			/* Saturday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Winter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Winter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Winter));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Winter));

			/* Sunday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 8, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2.02M, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateThinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 4.5M, pieces: 50, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 3.5M, pieces: 57, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateThinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 7, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Pruning_Winter));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: excludedBlock, hoursWorked: 6, pieces: 500, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.QualityControl));

			context.SaveChanges();

			// Execute test
			var rbpService = new RanchBonusPayService(context, _roundingService);
			int originalCount = context.RanchPayLines.Count();
			List<RanchPayLine> results = rbpService.CalculateRanchBonusPayLines(batch.Id, weekEndDate);

			// Compare results
			/* Original record count unchanged */
			Assert.AreEqual(originalCount, context.RanchPayLines.Count());
						
			/* Overall */
			Assert.AreEqual(0, results.Count());
		}


		[TestMethod]
		public void CreatesCorrectIndividualSummerPruningBonusRecordsForSummerPruningPlusBonus()
		{
			var dbName = "CreatesCorrectIndividualSummerPruningBonusRecordsForSummerPruningPlusBonus";
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
			string employeeLightDuty = "employeeLightDuty";

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

			// Mock Ranch Pay Lines
			/* Monday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Tuesday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			/* Wednesday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Thursday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));

			/* Friday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 20, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer, payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote, payType: PayType.CBHourlyTrees));


			/* Saturday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));

			/* Sunday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 8, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2.02M, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateThinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 4.5M, pieces: 50, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 3.5M, pieces: 57, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateThinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 7, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRatePruningSummer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: excludedBlock, hoursWorked: 6, pieces: 500, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.QualityControl));

			context.SaveChanges();

			// Execute test
			var rbpService = new RanchBonusPayService(context, _roundingService);
			int originalCount = context.RanchPayLines.Count();
			List<RanchPayLine> results = rbpService.CalculateRanchBonusPayLines(batch.Id, weekEndDate);

			// Compare results
			/* Original record count unchanged */
			Assert.AreEqual(originalCount, context.RanchPayLines.Count());

			/* Monday */
			Assert.AreEqual(8, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == monday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());

			/* Tuesday - All crew South */
			Assert.AreEqual(8, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == tuesday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());

			/* Wednesday */
			Assert.AreEqual(0, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == wednesday && x.WeekEndDate == weekEndDate).Count());


			/* Thursday - TD South */
			Assert.AreEqual(6, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == thursday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 1000M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 100M && x.TotalGross == 100M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 1000M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 100M && x.TotalGross == 100M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 5M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 0.5M && x.TotalGross == 0.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 3M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 0.3M && x.TotalGross == 0.3M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());

			/* Friday */
			Assert.AreEqual(2, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == friday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 15M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 1.5M && x.TotalGross == 1.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 5M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 0.5M && x.TotalGross == 0.5M).Count());

			/* Saturday - TD South */
			Assert.AreEqual(6, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == saturday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 1000M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 100M && x.TotalGross == 100M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 1000M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 100M && x.TotalGross == 100M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 15M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 1.5M && x.TotalGross == 1.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 5M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 0.5M && x.TotalGross == 0.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());

			/* Sunday */
			Assert.AreEqual(2, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == weekEndDate && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 50M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 5M && x.TotalGross == 5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer && x.Pieces == 7M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 0.7M && x.TotalGross == 0.7M).Count());


			/* Overall */
			Assert.AreEqual(32, results.Count());
		}

		[TestMethod]
		public void CreatesNoIndividualSummerPruningBonusRecordsForHourlyPruning()
		{
			var dbName = "CreatesNoIndividualSummerPruningBonusRecordsForHourlyPruning";
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
			string employeeLightDuty = "employeeLightDuty";

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

			// Mock Ranch Pay Lines
			/* Monday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Tuesday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Summer, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Summer, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Summer, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Summer, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Summer, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Summer, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Summer, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Summer, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			/* Wednesday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Thursday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Summer));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Summer));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Summer));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Summer));

			/* Friday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Pruning_Summer));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Pruning_Summer));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 20, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Pruning_Summer, payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote, payType: PayType.CBHourlyTrees));


			/* Saturday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Summer));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Summer));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Summer));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Summer));

			/* Sunday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 8, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2.02M, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateThinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 4.5M, pieces: 50, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 3.5M, pieces: 57, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateThinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 7, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Pruning_Summer));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: excludedBlock, hoursWorked: 6, pieces: 500, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.QualityControl));

			context.SaveChanges();

			// Execute test
			var rbpService = new RanchBonusPayService(context, _roundingService);
			int originalCount = context.RanchPayLines.Count();
			List<RanchPayLine> results = rbpService.CalculateRanchBonusPayLines(batch.Id, weekEndDate);

			// Compare results
			/* Original record count unchanged */
			Assert.AreEqual(originalCount, context.RanchPayLines.Count());

			/* Overall */
			Assert.AreEqual(0, results.Count());
		}

		[TestMethod]
		public void CreatesCorrectGraftingBonusRecords()
		{
			var dbName = "CreatesCorrectGraftingBonusRecords";
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
			string employeeLightDuty = "employeeLightDuty";

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

			// Mock Ranch Pay Lines
			/* Monday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 1, pieces: 0, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Tuesday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			/* Wednesday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Thursday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));

			/* Friday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 20, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew, payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote, payType: PayType.CBHourlyTrees));


			/* Saturday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeLightDuty, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));

			/* Sunday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 8, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2.02M, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateThinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 4.5M, pieces: 50, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 3.5M, pieces: 57, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateThinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 7, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Grafting_BuddingExpertCrew));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: excludedBlock, hoursWorked: 6, pieces: 500, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.QualityControl));

			context.SaveChanges();

			// Execute test
			var rbpService = new RanchBonusPayService(context, _roundingService);
			int originalCount = context.RanchPayLines.Count();
			List<RanchPayLine> results = rbpService.CalculateRanchBonusPayLines(batch.Id, weekEndDate);

			// Compare results
			/* Original record count unchanged */
			Assert.AreEqual(originalCount, context.RanchPayLines.Count());

			/* Monday */
			Assert.AreEqual(8, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == monday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());

			/* Tuesday - All crew South */
			Assert.AreEqual(8, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == tuesday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());

			/* Wednesday */
			Assert.AreEqual(0, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == wednesday && x.WeekEndDate == weekEndDate).Count());


			/* Thursday - TD South */
			Assert.AreEqual(6, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == thursday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 1000M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 50M && x.TotalGross == 50M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 1000M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 50M && x.TotalGross == 50M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 5M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 0.25M && x.TotalGross == 0.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 3M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 0.15M && x.TotalGross == 0.15M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());

			/* Friday */
			Assert.AreEqual(2, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == friday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 15M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 0.75M && x.TotalGross == 0.75M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 5M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 0.25M && x.TotalGross == 0.25M).Count());

			/* Saturday - TD South */
			Assert.AreEqual(6, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == saturday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 1000M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 50M && x.TotalGross == 50M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 1000M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 50M && x.TotalGross == 50M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 15M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 0.75M && x.TotalGross == 0.75M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 5M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 0.25M && x.TotalGross == 0.25M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeLightDuty && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 25M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 1.25M && x.TotalGross == 1.25M).Count());

			/* Sunday */
			Assert.AreEqual(2, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == weekEndDate && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 50M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew && x.Pieces == 7M && x.PieceRate == .05M && x.OtherGross == 0 && x.GrossFromPieces == 0.35M && x.TotalGross == 0.35M).Count());


			/* Overall */
			Assert.AreEqual(32, results.Count());
		}

		[TestMethod]
		public void CreatesCorrectReplantingBonusRecords()
		{
			var dbName = "CreatesCorrectReplantingBonusRecords";
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


			// Mock Ranch Pay Lines
			/* Monday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Replanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Replanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Replanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Replanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Replanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Replanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.TractorReplanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.TractorReplanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Tuesday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Replanting, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Replanting, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.TractorReplanting, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.TractorReplanting, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Replanting, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Replanting, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Replanting, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group2, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Replanting, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group3, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			/* Wednesday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.TractorReplanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Replanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Replanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Replanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.QualityControl));

			/* Thursday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Replanting, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Replanting, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.TractorReplanting, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2, pieces: 15, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.TractorReplanting, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Replanting, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Replanting, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Replanting, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Replanting, crewLocation: (int)Location.South));


			/* Friday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.TractorReplanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.TractorReplanting));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Replanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Replanting));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Replanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Replanting));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Replanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 20, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Replanting, payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 40, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.TractorReplanting, payType: PayType.CBHourlyTrees));


			/* Saturday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 6, pieces: 1000, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Replanting, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group4, hoursWorked: 2, pieces: 25, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Replanting, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Replanting, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group4, hoursWorked: 2.02M, pieces: 15, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Replanting, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group1, hoursWorked: 4.5M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Replanting, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 2.15M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Replanting, crewLocation: (int)Location.South));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group1, hoursWorked: 3.25M, pieces: 0, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.TractorReplanting, crewLocation: (int)Location.South));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 3, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.QualityControl, crewLocation: (int)Location.South));

			/* Sunday */
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeA, blockId: group1, hoursWorked: 8, pieces: 1000, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.TractorReplanting));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 5.98M, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Replanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeB, blockId: group1, hoursWorked: 2.02M, pieces: 0, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateThinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group4, hoursWorked: 4.5M, pieces: 50, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Replanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeC, blockId: group3, hoursWorked: 3.5M, pieces: 57, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.PieceRateThinning));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: group4, hoursWorked: 2, pieces: 7, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.Replanting));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: employeeD, blockId: excludedBlock, hoursWorked: 6, pieces: 500, weekEndDate: weekEndDate, shiftDate: weekEndDate, laborCode: (int)RanchLaborCode.QualityControl));

			context.SaveChanges();

			// Execute test
			var rbpService = new RanchBonusPayService(context, _roundingService);
			int originalCount = context.RanchPayLines.Count();
			List<RanchPayLine> results = rbpService.CalculateRanchBonusPayLines(batch.Id, weekEndDate);

			// Compare results
			/* Original record count unchanged */
			Assert.AreEqual(originalCount, context.RanchPayLines.Count());

			/* Monday */
			Assert.AreEqual(8, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == monday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == monday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());

			/* Tuesday */
			Assert.AreEqual(8, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == tuesday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == tuesday && x.BlockId == group2 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 25M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 2.5M && x.TotalGross == 2.5M).Count());

			/* Wednesday */
			Assert.AreEqual(4, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == wednesday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 304.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 30.41M && x.TotalGross == 30.41M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 303.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 30.31M && x.TotalGross == 30.31M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 228.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 22.81M && x.TotalGross == 22.81M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == wednesday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 164.7M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 16.47M && x.TotalGross == 16.47M).Count());

			/* Thursday */
			Assert.AreEqual(8, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == thursday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 304.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 30.41M && x.TotalGross == 30.41M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 303.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 30.31M && x.TotalGross == 30.31M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 228.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 22.81M && x.TotalGross == 22.81M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 164.7M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 16.47M && x.TotalGross == 16.47M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 12M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 1.2M && x.TotalGross == 1.2M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 12M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 1.2M && x.TotalGross == 1.2M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 12M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 1.2M && x.TotalGross == 1.2M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == thursday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 12M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 1.2M && x.TotalGross == 1.2M).Count());

			/* Friday */
			Assert.AreEqual(7, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == friday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 304.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 30.41M && x.TotalGross == 30.41M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 303.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 30.31M && x.TotalGross == 30.31M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 228.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 22.81M && x.TotalGross == 22.81M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 164.7M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 16.47M && x.TotalGross == 16.47M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 14.59M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 1.46M && x.TotalGross == 1.46M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 14.73M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 1.47M && x.TotalGross == 1.47M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == friday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 15.68M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 1.57M && x.TotalGross == 1.57M).Count());

			/* Saturday */
			Assert.AreEqual(7, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == saturday && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 304.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 30.41M && x.TotalGross == 30.41M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 303.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 30.31M && x.TotalGross == 30.31M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 228.1M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 22.81M && x.TotalGross == 22.81M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 164.7M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 16.47M && x.TotalGross == 16.47M).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 14.59M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 1.46M && x.TotalGross == 1.46M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 14.73M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 1.47M && x.TotalGross == 1.47M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == saturday && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 15.68M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 1.57M && x.TotalGross == 1.57M).Count());

			/* Sunday */
			Assert.AreEqual(4, results.Where(x => x.BatchId == batch.Id && x.ShiftDate == weekEndDate && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeA && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 572.3M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 57.23M && x.TotalGross == 57.23M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeB && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group1 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 427.8M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 42.78M && x.TotalGross == 42.78M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeC && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 39.46M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 3.95M && x.TotalGross == 3.95M).Count());
			Assert.AreEqual(1, results.Where(x => x.BatchId == batch.Id && x.EmployeeId == employeeD && x.PayType == PayType.ProductionIncentiveBonus && x.ShiftDate == weekEndDate && x.BlockId == group4 && x.LaborCode == (int)RanchLaborCode.Replanting && x.Pieces == 17.54M && x.PieceRate == .10M && x.OtherGross == 0 && x.GrossFromPieces == 1.75M && x.TotalGross == 1.75M).Count());


			/* Overall */
			Assert.AreEqual(46, results.Count());
		}

		[TestMethod]
		public void SetsCorrectGroupDesignationForRegularRecords()
		{
			var ranchPayLines = new List<RanchPayLine>();
			var bonusRates = new List<RanchBonusPieceRate>();

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

			// Mock Ranch Bonus Piece Rates
			bonusRates.Add(EntityMocker.MockRanchBonusPieceRate(blockId: group1, designation: nameof(group1), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 3.5M, perTreeBonus: 2M));
			bonusRates.Add(EntityMocker.MockRanchBonusPieceRate(blockId: group1, designation: nameof(group1), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: tuesday, perHourThreshold: 3.5M, perTreeBonus: 2.1M));
			bonusRates.Add(EntityMocker.MockRanchBonusPieceRate(blockId: group1, designation: nameof(group1), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: wednesday, perHourThreshold: 3.5M, perTreeBonus: 2.2M));
			bonusRates.Add(EntityMocker.MockRanchBonusPieceRate(blockId: group1, designation: nameof(group1), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: thursday, perHourThreshold: 3.75M, perTreeBonus: 2M));
			bonusRates.Add(EntityMocker.MockRanchBonusPieceRate(blockId: group1, designation: nameof(group1), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: friday, perHourThreshold: 3.75M, perTreeBonus: 2.1M));
			bonusRates.Add(EntityMocker.MockRanchBonusPieceRate(blockId: group1, designation: nameof(group1), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: saturday, perHourThreshold: 3.75M, perTreeBonus: 2.2M));

			bonusRates.Add(EntityMocker.MockRanchBonusPieceRate(blockId: excludedBlock, designation: nameof(excludedBlock), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 0M, perTreeBonus: 0M));
			bonusRates.Add(EntityMocker.MockRanchBonusPieceRate(blockId: group2, designation: nameof(group2), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 1.75M, perTreeBonus: 2M));
			bonusRates.Add(EntityMocker.MockRanchBonusPieceRate(blockId: group3, designation: nameof(group3), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 1.25M, perTreeBonus: 2M));
			bonusRates.Add(EntityMocker.MockRanchBonusPieceRate(blockId: group4, designation: nameof(group4), laborCode: (int)RanchLaborCode.Thinning, effectiveDate: monday, perHourThreshold: 1.5M, perTreeBonus: 2M));
						

			// Mock Ranch Pay Lines
			/* Employee A */
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Thinning));

			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: excludedBlock, hoursWorked: 2, pieces: 28, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: pendingBlock, hoursWorked: 2, pieces: 28, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning));

			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));

			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));

			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: friday));

			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeA, blockId: group1, hoursWorked: 2, pieces: 14, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));


			/* Employee B */
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group2, hoursWorked: 2.66M, pieces: 5, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group3, hoursWorked: 2.67M, pieces: 5, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group4, hoursWorked: 2.67M, pieces: 5, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Thinning));

			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group2, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group3, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group4, hoursWorked: 4M, pieces: 5, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning));

			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group2, hoursWorked: 2M, pieces: 0, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group3, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group4, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group2, hoursWorked: 2M, pieces: 10, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));

			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group2, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group3, hoursWorked: 2M, pieces: 0, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group4, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group3, hoursWorked: 2M, pieces: 10, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));

			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group2, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group3, hoursWorked: 2M, pieces: 2.5M, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group4, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group3, hoursWorked: 2M, pieces: 2.5M, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning));

			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group2, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group3, hoursWorked: 2M, pieces: 5M, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group4, hoursWorked: 2M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeB, blockId: group4, hoursWorked: 2M, pieces: 5M, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));

			/* Employee C */
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group1, hoursWorked: 2.5M, pieces: 8.75M, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group2, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group3, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group4, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: monday));

			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group1, hoursWorked: 2.5M, pieces: 8.75M, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group2, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group3, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group4, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: tuesday));

			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group1, hoursWorked: 2.5M, pieces: 8.75M, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group2, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group3, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group4, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: wednesday));

			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group1, hoursWorked: 2.5M, pieces: 8.75M, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group2, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group3, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group4, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: thursday));

			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group1, hoursWorked: 2.5M, pieces: 8.75M, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group2, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group3, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group4, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday));

			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group1, hoursWorked: 2.5M, pieces: 8.75M, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: pendingBlock, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Winter));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: excludedBlock, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Pruning_Summer));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeC, blockId: group4, hoursWorked: 2.5M, pieces: 5, weekEndDate: weekEndDate, shiftDate: friday));

			/* Employee D */
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeD, blockId: group1, hoursWorked: 8, pieces: 100, weekEndDate: weekEndDate, shiftDate: monday, laborCode: (int)RanchLaborCode.Thinning, payType: PayType.Pieces));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeD, blockId: group1, hoursWorked: 8, pieces: 100, weekEndDate: weekEndDate, shiftDate: tuesday, laborCode: (int)RanchLaborCode.Thinning, payType: PayType.HourlyPlusPieces));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeD, blockId: group1, hoursWorked: 8, pieces: 100, weekEndDate: weekEndDate, shiftDate: wednesday, laborCode: (int)RanchLaborCode.Thinning, payType: PayType.Holiday));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeD, blockId: group1, hoursWorked: 8, pieces: 100, weekEndDate: weekEndDate, shiftDate: thursday, laborCode: (int)RanchLaborCode.Thinning, payType: PayType.Bonus));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeD, blockId: group1, hoursWorked: 8, pieces: 100, weekEndDate: weekEndDate, shiftDate: friday, laborCode: (int)RanchLaborCode.Thinning, payType: PayType.ProductionIncentiveBonus));
			ranchPayLines.Add(EntityMocker.MockRanchPayLine(employeeId: employeeD, blockId: group1, hoursWorked: 8, pieces: 100, weekEndDate: weekEndDate, shiftDate: saturday, laborCode: (int)RanchLaborCode.Thinning, payType: PayType.Vacation));



			// Execute test
			using var context = new PayrollContext();
			var rbpService = new RanchBonusPayService(context, _roundingService);
			int originalCount = ranchPayLines.Count();
			rbpService.SetRegularLineGroupDesignation(bonusRates, ranchPayLines);

			// Compare results
			/* Original record count unchanged */
			Assert.AreEqual(originalCount, ranchPayLines.Count());

			/* Employee A */
			Assert.AreEqual(24, ranchPayLines.Where(x => x.EmployeeId == employeeA && x.PayType == PayType.Regular && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(4, ranchPayLines.Where(x => x.EmployeeId == employeeA && x.PayType == PayType.Regular && x.ShiftDate == monday && x.BlockId == group1 && x.Designation == nameof(group1) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2 && x.Pieces == 14).Count());
			Assert.AreEqual(2, ranchPayLines.Where(x => x.EmployeeId == employeeA && x.PayType == PayType.Regular && x.ShiftDate == tuesday && x.BlockId == group1 && x.Designation == nameof(group1) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2 && x.Pieces == 14).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeA && x.PayType == PayType.Regular && x.ShiftDate == tuesday && x.BlockId == excludedBlock && x.Designation == nameof(excludedBlock) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2 && x.Pieces == 28).Count());
			//Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeA && x.PayType == PayType.Regular && x.ShiftDate == tuesday && x.BlockId == pendingBlock && x.Designation == nameof(pendingBlock) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2 && x.Pieces == 28).Count());
			Assert.AreEqual(4, ranchPayLines.Where(x => x.EmployeeId == employeeA && x.PayType == PayType.Regular && x.ShiftDate == wednesday && x.BlockId == group1 && x.Designation == nameof(group1) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2 && x.Pieces == 14).Count());
			Assert.AreEqual(4, ranchPayLines.Where(x => x.EmployeeId == employeeA && x.PayType == PayType.Regular && x.ShiftDate == thursday && x.BlockId == group1 && x.Designation == nameof(group1) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2 && x.Pieces == 14).Count());
			Assert.AreEqual(3, ranchPayLines.Where(x => x.EmployeeId == employeeA && x.PayType == PayType.Regular && x.ShiftDate == friday && x.BlockId == group1 && x.Designation == nameof(group1) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2 && x.Pieces == 14).Count());
			Assert.AreEqual(4, ranchPayLines.Where(x => x.EmployeeId == employeeA && x.PayType == PayType.Regular && x.ShiftDate == saturday && x.BlockId == group1 && x.Designation == nameof(group1) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2 && x.Pieces == 14).Count());
			Assert.AreEqual(2, ranchPayLines.Where(x => x.EmployeeId == employeeA && x.PayType == PayType.Regular && (x.Designation == null || x.Designation == "")).Count());
			
			/* Employee B */
			Assert.AreEqual(22, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == monday && x.BlockId == group2 && x.Designation == nameof(group2) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2.66M && x.Pieces == 5).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == monday && x.BlockId == group3 && x.Designation == nameof(group3) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2.67M && x.Pieces == 5).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == monday && x.BlockId == group4 && x.Designation == nameof(group4) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2.67M && x.Pieces == 5).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == tuesday && x.BlockId == group2 && x.Designation == nameof(group2) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2M && x.Pieces == 5).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == tuesday && x.BlockId == group3 && x.Designation == nameof(group3) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2M && x.Pieces == 5).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == tuesday && x.BlockId == group4 && x.Designation == nameof(group4) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 4M && x.Pieces == 5).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == wednesday && x.BlockId == group2 && x.Designation == nameof(group2) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2M && x.Pieces == 0).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == wednesday && x.BlockId == group3 && x.Designation == nameof(group3) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2M && x.Pieces == 5).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == wednesday && x.BlockId == group4 && x.Designation == nameof(group4) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2M && x.Pieces == 5).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == wednesday && x.BlockId == group2 && x.Designation == nameof(group2) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2M && x.Pieces == 10).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == thursday && x.BlockId == group2 && x.Designation == nameof(group2) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2M && x.Pieces == 5).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == thursday && x.BlockId == group3 && x.Designation == nameof(group3) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2M && x.Pieces == 0).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == thursday && x.BlockId == group4 && x.Designation == nameof(group4) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2M && x.Pieces == 5).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == thursday && x.BlockId == group3 && x.Designation == nameof(group3) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2M && x.Pieces == 10).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == friday && x.BlockId == group2 && x.Designation == nameof(group2) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2M && x.Pieces == 5).Count());
			Assert.AreEqual(2, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == friday && x.BlockId == group3 && x.Designation == nameof(group3) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2M && x.Pieces == 2.5M).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == friday && x.BlockId == group4 && x.Designation == nameof(group4) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2M && x.Pieces == 5).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == saturday && x.BlockId == group2 && x.Designation == nameof(group2) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2M && x.Pieces == 5).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == saturday && x.BlockId == group3 && x.Designation == nameof(group3) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2M && x.Pieces == 5).Count());
			Assert.AreEqual(2, ranchPayLines.Where(x => x.EmployeeId == employeeB && x.PayType == PayType.Regular && x.ShiftDate == saturday && x.BlockId == group4 && x.Designation == nameof(group4) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2M && x.Pieces == 5).Count());

			/* Employee C */
			Assert.AreEqual(24, ranchPayLines.Where(x => x.EmployeeId == employeeC && x.PayType == PayType.Regular && x.WeekEndDate == weekEndDate).Count());

			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeC && x.PayType == PayType.Regular && x.ShiftDate == monday && x.BlockId == group1 && x.Designation == nameof(group1) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2.5M && x.Pieces == 8.75M).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeC && x.PayType == PayType.Regular && x.ShiftDate == tuesday && x.BlockId == group1 && x.Designation == nameof(group1) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2.5M && x.Pieces == 8.75M).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeC && x.PayType == PayType.Regular && x.ShiftDate == wednesday && x.BlockId == group1 && x.Designation == nameof(group1) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2.5M && x.Pieces == 8.75M).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeC && x.PayType == PayType.Regular && x.ShiftDate == thursday && x.BlockId == group1 && x.Designation == nameof(group1) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2.5M && x.Pieces == 8.75M).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeC && x.PayType == PayType.Regular && x.ShiftDate == friday && x.BlockId == group1 && x.Designation == nameof(group1) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2.5M && x.Pieces == 8.75M).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.EmployeeId == employeeC && x.PayType == PayType.Regular && x.ShiftDate == saturday && x.BlockId == group1 && x.Designation == nameof(group1) && x.LaborCode == (int)RanchLaborCode.Thinning && x.HoursWorked == 2.5M && x.Pieces == 8.75M).Count());
			Assert.AreEqual(18, ranchPayLines.Where(x => x.EmployeeId == employeeC && x.PayType == PayType.Regular && (x.Designation == null || x.Designation == "")).Count());

			/* Employee D */
			Assert.AreEqual(0, ranchPayLines.Where(x => x.EmployeeId == employeeD && x.WeekEndDate == weekEndDate && x.PayType == PayType.Regular).Count());

		}

	}
}
