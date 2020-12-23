using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MuuseumiKylastus
{
    class Program
    {
        static List<VisitPeriod> ListOfPeriods = new List<VisitPeriod>() { };
        static List<OpenTime> ListOfMinutes = new List<OpenTime>() { };

        public class OpenTime
        {
            public DateTime OpenMinute { get; set; }
            public UInt32 Visitors { get; set; }
        }

        public class VisitPeriod
        {
            public DateTime VisitStartTime { get; set; }
            public DateTime VisitEndtime { get; set; }
        }

        static int Main(string[] args)
        {
            System.Console.WriteLine("Muuseumi külastus v1.00\n\r");
            if (args.Length >= 1)
            {
                System.Console.WriteLine("Külastusaegade fail: " + args[0]);

                // Loen failist külastusaegade andmed
                int FailiRida = 1;
                try
                {
                    var VisitPeriodLines = File.ReadLines(args[0]);
                    foreach (var VisitPeriodLine in VisitPeriodLines)
                    {
                        System.Console.WriteLine(FailiRida + ". " + VisitPeriodLine);
                        AddPeriodToList(VisitPeriodLine);
                        FailiRida++;
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                    return 1;
                }

                // Loon listi muuseumi lahtiolekuaja minutitest

                DateTime OM = Convert.ToDateTime("10:00");  // Muuseum on avatud 10:00 kuni...

                while (OM.Hour < 20)    // ... 20:00
                {
                    UInt32 V = 0;
                    foreach (var P in ListOfPeriods)
                    {
                        if (OM >= P.VisitStartTime && OM <= P.VisitEndtime)
                            V++;
                    }
                    ListOfMinutes.Add(new OpenTime { OpenMinute = OM, Visitors=V });
                    OM = OM.AddMinutes(1);
                }

                var lm = from element in ListOfMinutes orderby element.Visitors descending select element;
                var PeakVisitors = lm.ElementAt(0).Visitors;  // Sorteeritud listis on minutid külastajate arvu järgi kahanevas järjekorras

                int i=0;
                DateTime PeakVisitorsStartTime = lm.ElementAt(0).OpenMinute;
                DateTime PeakVistorsEndTime = PeakVisitorsStartTime;

                if (PeakVisitors > 0) {
                    while (lm.ElementAt(i).Visitors == PeakVisitors)
                    {
                        PeakVistorsEndTime = lm.ElementAt(i).OpenMinute;
                        i++;
                    }
                    System.Console.WriteLine("\n\rEnim külastajaid, " + PeakVisitors + ", oli muuseumis ajavahemikul " + PeakVisitorsStartTime.ToString("HH:mm") + " - " + PeakVistorsEndTime.ToString("HH:mm"));
                    System.Console.ReadKey();
                    return 0;
                }
                else
                {
                    System.Console.WriteLine("  Failis pole arusaadavaid ajavahemikke !");
                    return 1;
                }
            }
            else
            {
                System.Console.WriteLine("Puudub külastusaegade failinimi !");
                return 1;
            }
        }

        static void AddPeriodToList(string LineWithPeriod)
        {

            try
            {
                var PeriodStartTime = Convert.ToDateTime(LineWithPeriod.Substring(0, 5));
                var PeriodEndTime = Convert.ToDateTime(LineWithPeriod.Substring(6, 5));
                if (PeriodEndTime < PeriodStartTime)
                {
                    throw new ArgumentException();
                }
                ListOfPeriods.Add(new VisitPeriod { VisitStartTime = PeriodStartTime, VisitEndtime = PeriodEndTime });
            }
            catch
            {
                System.Console.WriteLine("Vigased lähteandmed kellaaegade failis! Peab olema: <SaabumisAeg>,<LahkumisAeg>. Näiteks 10:15,11:30");
            }
        }
    }
}
