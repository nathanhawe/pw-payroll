using Payroll.Domain;
using System;
using System.Collections.Generic;

namespace Payroll.Service
{
    public class CrewBossPayService
    {
        public CrewBossPayService(/*PayrollContext context*/)
        {

        }
        public List<RanchPayLine> CalculateCrewBossPay(int batchId)
        {
            // For each crew boss in the batch
                // If daily south -> Apply Rate
                // If daily hourly -> Apply Rate
                // If hourly vines -> Lookup and apply rate based on worker count
                // If hourly trees -> Lookup and apply rate based on worker count
                // Update worker count and rate in CrewBossPay record
                // Create new RanchPayLine record

            // Add RanchPaylines to DB
            // Save Changes
            throw new NotImplementedException();
        }
    }
}
