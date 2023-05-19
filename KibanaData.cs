namespace flx_web
{
    public class KibanaData
    {
        public List<KibanaCustomer> Customers { get; set; }

        public KibanaData()
        {
            Customers = new List<KibanaCustomer>();
        }

        public void AddOrUpdateCustomer(KibanaCustomer customer)
        {
            var existingCustomer = Customers.
                FirstOrDefault(c => c.Id == customer.Id);

            if (existingCustomer == null)
            {
                Customers.Add(customer);
            }
            else
            {
                existingCustomer.Name = customer.Name;
                existingCustomer.LastLogTimestamp = customer.LastLogTimestamp;

                existingCustomer.Results.RemoveAll(r => r == null);

                foreach (var result in customer.Results)
                {
                    if (result == null)
                    {
                        continue;
                    }

                    existingCustomer.Results.
                        RemoveAll(r =>
                            r?.Request == result?.Request &&
                            r?.Timestamp == result?.Timestamp);

                    existingCustomer.Results.Add(result);
                }
            }
        }

        public void RestrictCustomerToSettings(List<string> customers)
        {
            if (customers != null && customers.Count > 0)
            {
                Customers = Customers.Where(c => customers.Select(cus => cus.ToUpper()).Contains(c.Id.ToUpper())).ToList();
            }
        }

        public void RestrictToInterval(int interval)
        {
            foreach(var customer in Customers)
            {
                var results = new List<KibanaResult>();
                var requests = customer.Results.Select(r => r.Request).Distinct();

                foreach(var request in requests)
                {
                    results.AddRange(customer.Results.Where(r => r.Request == request).OrderByDescending(r => r.Timestamp).Take(interval));
                }

                customer.Results = results;
            }
        }

        public void Transform(DateTime fromDate, List<TimeBlock> timeBlocks, List<string> customers, List<string> requests)
        {
            if (customers != null && customers.Count > 0)
            {
                Customers = Customers.Where(c => customers.Select(cus => cus.ToUpper()).Contains(c.Id.ToUpper())).ToList();
            }

            var now = new DateTime(fromDate.Year,
                fromDate.Month, fromDate.Day,
                fromDate.Hour, fromDate.Minute, 0);

            AddNames();

            foreach (var customer in Customers)
            {
                var results = new List<KibanaResult>();

                foreach (var request in requests)
                {
                    foreach (var timeBlock in timeBlocks)
                    {
                        var from = timeBlock.From;
                        var to = timeBlock.To;

                        var existingResult = customer.
                            Results.FirstOrDefault(r =>
                                r.Request == request &&
                                r.Timestamp >= from &&
                                r.Timestamp < to);

                        if (existingResult != null)
                        {
                            results.Add(new KibanaResult()
                            {
                                Request = request,
                                Timestamp = to,
                                Duration = existingResult.Duration,
                                TotalCount = existingResult.TotalCount,
                                ErrorCount = existingResult.ErrorCount,
                                TotalCountBaseline = existingResult.TotalCountBaseline,
                                ErrorCountBaseline = existingResult.ErrorCountBaseline,
                                ErrorPercentBaseline = existingResult.ErrorPercentBaseline
                            });
                        }
                        else
                        {
                            results.Add(new KibanaResult()
                            {
                                Request = request,
                                Timestamp = to,
                                Duration = 0,
                                TotalCount = 0,
                                ErrorCount = 0
                            });
                        }
                    }

                    var totalCountBaseline = results.Any(r =>
                        r.Request == request && r.TotalCountBaseline > 0) ?
                        results.Where(r =>
                            r.Request == request && r.TotalCountBaseline > 0).
                            Average(r => r.TotalCountBaseline) : 0;

                    var errorCountBaseline = results.Any(r =>
                        r.Request == request && r.ErrorCountBaseline > 0) ?
                        results.Where(r =>
                            r.Request == request && r.ErrorCountBaseline > 0).
                            Average(r => r.ErrorCountBaseline) : 0;

                    var errorPercentBaseline = results.Any(r =>
                        r.Request == request && r.ErrorPercentBaseline > 0) ?
                        results.Where(r =>
                            r.Request == request && r.ErrorPercentBaseline > 0).
                            Average(r => r.ErrorPercentBaseline) : 0;

                    var average = results.Any(r => r.Request == request) ?
                        results.Where(r => r.Request == request).
                            Average(r => r.ErrorPercent) : 0;

                    var max = results.Any(r => r.Request == request) ?
                        results.Where(r => r.Request == request).
                            Max(r => r.ErrorPercent) : 0;

                    var min = results.Any(r => r.Request == request) ?
                        results.Where(r => r.Request == request).
                            Min(r => r.ErrorPercent) : 0;

                    foreach (var result in results.Where(r => r.Request == request))
                    {
                        result.TotalCountBaseline = totalCountBaseline;
                        result.ErrorCountBaseline = errorCountBaseline;
                        result.ErrorPercentBaseline = errorPercentBaseline;
                        result.Average = average;
                        result.Max = max;
                        result.Min = min;
                    }
                }

                customer.Results = results;
            }
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
                    From = startTime,
                    To = startTime.AddMinutes(interval)
                });

                startTime = startTime.AddMinutes(-interval);
            }

            return timeBlocks;
        }
    }

    public class KibanaCustomer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime LastLogTimestamp { get; set; }
        public List<KibanaResult> Results { get; set; }

        public KibanaCustomer()
        {
            Results = new List<KibanaResult>();
        }
    }

    public class KibanaResult
    {
        public string Request { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public double Duration { get; set; }
        public int TotalCount { get; set; }
        public int ErrorCount { get; set; }
        public double DurationBaseline { get; set; }
        public double TotalCountBaseline { get; set; }
        public double ErrorCountBaseline { get; set; }
        public double ErrorPercentBaseline { get; set; }
        public double Average { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
        public double ErrorPercent 
        {
            get
            {
                if (TotalCount == 0)
                {
                    return 0;
                }

                return (double)ErrorCount / (double)TotalCount * 100;
            }
        }
        public double DurationAverage
        {
            get
            {
                if (TotalCount == 0)
                {
                    return 0;
                }

                return (double)Duration / (double)TotalCount;
            }
        }
        public double DurationBaselineAverage
        {
            get
            {
                if (TotalCount == 0)
                {
                    return 0;
                }

                return (double)DurationBaseline / (double)TotalCount;
            }
        }
    }
}
