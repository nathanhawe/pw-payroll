using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data;
using Payroll.Domain;
using System;
using System.Linq;

namespace Payroll.UnitTest
{
    [TestClass]
    public class CrewBossPayServiceTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var options = new DbContextOptionsBuilder<PayrollContext>()
                .UseInMemoryDatabase(databaseName: "TestMethod1")
                .Options;

            using (var context = new PayrollContext(options))
            {
                context.Database.EnsureCreated();
                var batch = new Batch
                {
                    StartDate = DateTime.Now,
                    Owner = "Me",
                    State = "Pending"
                };

                context.Batches.Add(batch);
                context.SaveChanges();

                var temp = context.Batches.ToList();
                var test = context.CrewBossWages.ToList();
                Assert.AreEqual(1, temp.Count);
            }
        }
    }
}
