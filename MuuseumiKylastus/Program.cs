using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MuuseumiKylastus
{
    class Program
    {
        static List<visittime> listoftimes = new List<visittime>() { };
        static List<openminute> listofminutes = new List<openminute>() { };

        public class openminute
        {
            public DateTime minute { get; set; }
            public UInt32 visitors { get; set; }
        }

        public class visittime
        {
            public DateTime start { get; set; }
            public DateTime end { get; set; }
        }

        static int Main(string[] args)
        {
            System.Console.WriteLine("Muuseumi külastus v1.01\n\r");
            if (args.Length >= 1)
            {
                System.Console.WriteLine("Külastusaegade fail: " + args[0]);

                // Loen failist külastusaegade andmed
                int failirida = 1;
                try
                {
                    var visittimes = File.ReadLines(args[0]);
                    foreach (var visittime in visittimes)
                    {
                        System.Console.WriteLine(failirida + ". " + visittime);

                        try
                        {
                            var visitstarttime = Convert.ToDateTime(visittime.Substring(0, 5));
                            var visitendtime = Convert.ToDateTime(visittime.Substring(6, 5));
                            if (visitendtime < visitstarttime)
                            {
                                throw new ArgumentException();
                            }
                            listoftimes.Add(new visittime { start = visitstarttime, end = visitendtime });
                        }
                        catch
                        {
                            System.Console.WriteLine("Vigane külastusaaegade fail! Peab olema: <SaabumisAeg>,<LahkumisAeg>. Näiteks 10:15,11:30");
                        }
                        failirida++;
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
                    foreach (var P in listoftimes)
                    {
                        if (OM >= P.start && OM <= P.end)
                            V++;
                    }
                    listofminutes.Add(new openminute { minute = OM, visitors = V });
                    OM = OM.AddMinutes(1);
                }

                var lm = from element in listofminutes orderby element.visitors descending select element;
                var peakvisitors = lm.ElementAt(0).visitors;  // Sorteeritud listis on minutid külastajate arvu järgi kahanevas järjekorras

                int i=0;
                DateTime peakvisitorsstarttime = lm.ElementAt(0).minute;
                DateTime peakvisitorsendtime = peakvisitorsstarttime;

                if (peakvisitors > 0) {
                    while (lm.ElementAt(i).visitors == peakvisitors)
                    {
                        peakvisitorsendtime = lm.ElementAt(i).minute;
                        i++;
                    }
                    System.Console.WriteLine("\n\rEnim külastajaid, " + peakvisitors + ", oli muuseumis ajavahemikul " + peakvisitorsstarttime.ToString("HH:mm") + " - " + peakvisitorsendtime.ToString("HH:mm"));
                    System.Console.ReadKey();
                    return 0;
                }
                else
                {
                    System.Console.WriteLine("  Failis pole nõuetekohaseid ajavahemike andmeid!");
                    return 1;
                }
            }
            else
            {
                System.Console.WriteLine("Puudub külastusaegade failinimi !");
                return 1;
            }
        }
    }
}
