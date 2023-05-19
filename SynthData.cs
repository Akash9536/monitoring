using System.Linq;

namespace flx_web
{
    public class SynthData
    {
        public List<SynthCustomer> Customers;

        public SynthData()
        {
            Customers = new List<SynthCustomer>();
        }

        public void AddOrUpdateCustomer(SynthCustomer customer)
        {
            var existingCustomer = Customers.
                FirstOrDefault(c => c.Id == customer.Id);

            if (existingCustomer == null)
            {
                Customers.Add(customer);
            }
            else
            {
                existingCustomer.Results.
                    AddRange(customer.Results);
            }
        }

        public void CleanUpResults(int limit = 1000, int purge = 100)
        {
            if (purge > limit)
            {
                throw new ArgumentException("Cannot have purgeCount > limit.");
            }

            foreach (var customer in Customers)
            {
                if (customer.Results.Count > limit)
                {
                    customer.Results = customer.
                        Results.Skip(customer.Results.Count - (limit + purge)).
                        ToList();
                }
            }
        }

        public void TransformByCustomer(DateTime fromDate, List<TimeBlock> timeBlocks, List<string> customers)
        {
            if (customers != null && customers.Count > 0)
            {
                Customers.RemoveAll(c => 
                    !customers.Contains(c.Id) && 
                    !customers.Contains(c.Name));
            }

            var now = new DateTime(fromDate.Year,
                fromDate.Month, fromDate.Day,
                fromDate.Hour, fromDate.Minute, 0);

            foreach (var customer in Customers)
            {
                var results = new List<SynthResult>();

                foreach(var timeBlock in timeBlocks)
                {
                    var from = timeBlock.From;
                    var to = timeBlock.To;

                    var existingResults = customer.
                        Results.Where(r => 
                            r.Timestamp >= from && 
                            r.Timestamp < to).ToList();

                    if (existingResults.Count > 0)
                    {
                        var errorResult = existingResults.FirstOrDefault(r => !r.Success);

                        results.Add(new SynthResult()
                        {
                            Test = $"All Tests",
                            Timestamp = from,
                            Success = errorResult == null,
                            Action = errorResult?.Action,
                            ErrorMessage = errorResult?.ErrorMessage,
                            ConsolidatedCount = existingResults.Count,
                            ConsolidatedSuccessCount = existingResults.Where(r => r.Success).Count(),
                            ConsolidatedFailureCount = existingResults.Where(r => !r.Success).Count(),
                            ConsolidatedLegitErrorCount = existingResults.Where(r => r.OutcomeType == "Legitimate Error").Count(),
                        });                            
                    }
                    else
                    {
                        results.Add(new SynthResult()
                        {
                            Test = $"All Tests",
                            Timestamp = from,
                            Success = false,
                            Action =  "No-Tests-Performed",
                            ErrorMessage = "There were no tests Performed during this time interval.",
                            ConsolidatedCount = 0,
                            ConsolidatedLegitErrorCount = 0,
                        });
                    }
                }

                customer.Results = results;
            }
        }

        public void TransformByCustomerRequests(DateTime fromDate, List<TimeBlock> timeBlocks, List<string> customers)
        {
            if (customers != null && customers.Count > 0)
            {
                Customers.RemoveAll(c =>
                    !customers.Contains(c.Id) &&
                    !customers.Contains(c.Name));
            }

            var now = new DateTime(fromDate.Year,
                fromDate.Month, fromDate.Day,
                fromDate.Hour, fromDate.Minute, 0);

            AddNames();

            var newCustomers = new List<SynthCustomer>();

            foreach (var synthRequestType in Global.GetSynthRequestTypes())
            {
                foreach (var customer in Customers)
                {
                    var results = new List<SynthResult>();

                    foreach (var timeBlock in timeBlocks)
                    {
                            var from = timeBlock.From;
                            var to = timeBlock.To;

                            var existingResults = customer.
                                Results.Where(r =>
                                    r.Timestamp >= from &&
                                    r.Timestamp < to &&
                                    r.Test.Contains(synthRequestType)).ToList();

                            if (existingResults.Count > 0)
                            {
                                var errorResult = existingResults.FirstOrDefault(r => !r.Success);

                                results.Add(new SynthResult()
                                {
                                    Test = $"All Tests",
                                    Timestamp = from,
                                    Success = errorResult == null,
                                    Action = errorResult?.Action,
                                    ErrorMessage = errorResult?.ErrorMessage,
                                    ConsolidatedCount = existingResults.Count,
                                    ConsolidatedSuccessCount = existingResults.Where(r => r.Success).Count(),
                                    ConsolidatedFailureCount = existingResults.Where(r => !r.Success).Count(),
                                    ConsolidatedLegitErrorCount = existingResults.Where(r => r.OutcomeType == "Legitimate Error").Count(),
                                });
                            }
                            else
                            {
                                results.Add(new SynthResult()
                                {
                                    Test = $"All Tests",
                                    Timestamp = from,
                                    Success = false,
                                    Action = "No-Tests-Performed",
                                    ErrorMessage = "There were no tests Performed during this time interval.",
                                    ConsolidatedCount = 0,
                                    ConsolidatedLegitErrorCount = 0,
                                });
                        }
                    }

                    if (results.Any(r => r.ConsolidatedCount > 0))
                    {
                        newCustomers.Add(new SynthCustomer()
                        {
                            Id = $"{customer.Id}",
                            Name = $"{customer.Name} - {synthRequestType}",
                            Results = results
                        });
                    }
                }
            }

            this.Customers = newCustomers;
        }

        public void AddNames()
        {
            foreach (var customer in Customers)
            {
                var idName = Global.GetCustomerIdName(customer.Id);
                customer.Id = idName.Item1;
                customer.Name = idName.Item2;
            }
        }

        public static List<TimeBlock> GenerateTimeBlocks(DateTime dateTime, int interval, int count)
        {
            dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 
                dateTime.Hour, dateTime.Minute, 0);

            var mod = dateTime.Minute % interval;
            var startTime = dateTime.AddMinutes(-mod);//.AddMinutes(-interval);

            var timeBlocks = new List<TimeBlock>();

            for (var i = 0; i < count; i++)
            {
                timeBlocks.Add(new TimeBlock() 
                { 
                    From = startTime, To = startTime.AddMinutes(interval) 
                });

                startTime = startTime.AddMinutes(-interval);
            }

            return timeBlocks;
        }

    }

    public class SynthCustomer
    {
        public string Id;
        public string Name;
        public string Release;
        public DateTime ReleaseTimestamp;

        public List<SynthResult> Results;

        public SynthCustomer()
        {
            Results = new List<SynthResult>();
        }
    }

    public class SynthResult
    {
        public string Test;
        public DateTime Timestamp;
        public string EndPointType;
        public bool Success;
        public string OutcomeType;
        public string Action;
        public string ErrorMessage;
        public int ConsolidatedCount;
        public int ConsolidatedSuccessCount;
        public int ConsolidatedFailureCount;
        public int ConsolidatedLegitErrorCount;
        public double ResponseTime;
        public double MaxResponseTime;
    }

    public class TimeBlock
    {
        public DateTime From;
        public DateTime To;
    }
}
