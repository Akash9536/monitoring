namespace flx_web
{
    public static class Global
    {
        public static List<string> GetSynthCustomers(Settings settings)
        {
            return settings.SynthCustomers;
        }

        public static List<string> GetKibanaCustomers(Settings settings)
        {
            return settings.KibanaCustomers;
        }

        public static List<string> GetKibanaRequest(Settings settings)
        {
            return settings.KibanaRequests;
        }

        public static List<string> GetCustomersThatCanRelease()
        {
            return new List<string>()
            {
                "EKD", "QF"
            };
        }

        public static Tuple<string, string> GetCustomerIdName(string customerId)
        {
            switch (customerId.ToLower())
            {
                case "a3oa":
                    return new Tuple<string, string>("A3OA", "Aegean Olympic");
                case "aa":
                    return new Tuple<string, string>("AA", "American");
                case "cm":
                case "cmi":
                    return new Tuple<string, string>("CM", "COPA");
                case "cmd":
                    return new Tuple<string, string>("CMD", "COPA Direct");
                case "ek":
                    return new Tuple<string, string>("EK", "Emirates");
                case "ekd":
                    return new Tuple<string, string>("EKD", "Emirates Direct");
                case "ha":
                    return new Tuple<string, string>("HA", "Hawaiian");
                case "had":
                    return new Tuple<string, string>("HAD", "Hawaiian Direct");
                case "lh":
                case "lhg":
                    return new Tuple<string, string>("LHG", "Lufthansa");
                case "qf":
                    return new Tuple<string, string>("QF", "Qantas");
                case "uad":
                    return new Tuple<string, string>("UAD", "United");
                case "ws":
                    return new Tuple<string, string>("WS", "West Jet");
                default:
                    return new Tuple<string, string>(customerId.ToUpper(), "");
            }
        }

        public static bool CustomerWantsRequest(Settings settings, string customerId, string request)
        {
            switch (customerId.ToLower())
            {
                case "a3oa":
                    return settings.A3OA_Requests.ToUpper().Contains(request.ToUpper());
                case "aa":
                    return settings.AA_Requests.ToUpper().Contains(request.ToUpper());
                case "cm":
                case "cmi":
                    return settings.CM_Requests.ToUpper().Contains(request.ToUpper());
                case "cmd":
                    return settings.CMD_Requests.ToUpper().Contains(request.ToUpper());
                case "ek":
                    return settings.EK_Requests.ToUpper().Contains(request.ToUpper());
                case "ekd":
                    return settings.EK_Requests.ToUpper().Contains(request.ToUpper());
                case "ha":
                    return settings.HA_Requests.ToUpper().Contains(request.ToUpper());
                case "had":
                    return settings.HA_Requests.ToUpper().Contains(request.ToUpper());
                case "lhg":
                    return settings.LHG_Requests.ToUpper().Contains(request.ToUpper());
                case "qf":
                    return settings.QF_Requests.ToUpper().Contains(request.ToUpper());
                case "uad":
                    return settings.UAD_Requests.ToUpper().Contains(request.ToUpper());
                case "ws":
                    return settings.WS_Requests.ToUpper().Contains(request.ToUpper());
                default:
                    return false;
            }
        }

        public static List<string> GetSynthRequestTypes()
        {
            return new List<string>()
            {
                "(AirShopping-ServiceList)",
                "(AirShopping-OfferPrice)",
                "(AirShopping-FlightPrice)",
                "(AirShopping)",
                "(FareSearch)",
                "(FareQuote-FareRule)",
                "(FareQuote)",
                "(Native/Other)"
            };
        }

        public static List<DateTime> GetHistoryDates()
        {
            var now = DateTime.UtcNow;
            var from = DateTime.UtcNow.AddHours(1);
            var start = new DateTime(from.Year, from.Month, from.Day, from.Hour, 0, 0);

            var current = start;

            var dates = new List<DateTime>();

            while (current > now.AddDays(-14))
            {
                if (current < now.AddMinutes(-30))
                {
                    dates.Add(current);
                }

                current = current.AddMinutes(-30);
            }

            return dates;
        }
    }
}
