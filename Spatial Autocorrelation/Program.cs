using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Resources;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace SAC
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine("This program is to calculate the Moran`s I and Geary C, indicators of spatial autocorrelation, using spatial adjacent relationship of eight neighborhoods, original images were classified to five classes from deep red to deep green.\n");
            var sheet1=DataImport.ImportFromExcel("sheet1");
            var sheet2 = DataImport.ImportFromExcel("sheet2");
            var map1=DataImport.DT2List(sheet1);
            var map2=DataImport.DT2List(sheet2);

            var (moran1,z1,p1) = Index.MoranIndex(map1);
            var (moran2,z2,p2)=Index.MoranIndex(map2);
            Console.WriteLine("The First Image`s Moran Index is " + Math.Round(moran1,3) + ", with Norm CDF value "+Math.Round(p1,3)+ $". Meaning z={Math.Round(z1, 3)}, p={Math.Round(p1>0.5?2*(1-p1):2*p1,3)}");
            Console.WriteLine("The Second Image`s Moran Index is " + Math.Round(moran2,3) + ", with Norm CDF value " + Math.Round(p2,3) + $". Meaning z={Math.Round(z2, 3)}, p={Math.Round(p2 > 0.5 ? 2 * (1 - p2) : 2 * p2, 3)}");

            var m1=MonteCarlo.MentoCarloSimulation(Index.MoranIndex, 999, 30, 30, moran1);
            var m2=MonteCarlo.MentoCarloSimulation(Index.MoranIndex, 999, 30, 30, moran2);
            Console.WriteLine($"For MentoCarlo Testing, the P value is Image One: {Math.Round(p1 > 0.5 ? 2 * (1 - p1) : 2 * p1, 3)}, Image Two:{Math.Round(p2 > 0.5 ? 2 * (1 - p2) : 2 * p2, 3)}");
            Console.WriteLine();

            double geary1, geary2;
            (geary1,z1,p1) = Index.GearyCorrelation(map1);
            (geary2,z2,p2) = Index.GearyCorrelation(map2);
            Console.WriteLine("The First Image`s Geary Correlation is " + Math.Round(geary1,3) + ", with Norm CDF value "+Math.Round(p1,3)+$". Meaning z={Math.Round(z1, 3)}, p={Math.Round(p1 > 0.5 ? 2 * (1 - p1) : 2 * p1, 3)}");
            Console.WriteLine("The Second Image`s Geary Correlation is " + Math.Round(geary2,3) + ", with Norm CDF value " + Math.Round(p2,3) + $". Meaning z={Math.Round(z2, 3)}, p={Math.Round(p2 > 0.5 ? 2 * (1 - p2) : 2 * p2, 3)}");
            m1 = MonteCarlo.MentoCarloSimulation(Index.GearyCorrelation, 999, 30, 30, geary1);
            m2 = MonteCarlo.MentoCarloSimulation(Index.GearyCorrelation, 999, 30, 30, geary2);
            Console.WriteLine($"For MentoCarlo Testing, the P value is Image One: {Math.Round(p1 > 0.5 ? 2 * (1 - p1) : 2 * p1, 3)}, Image Two:{Math.Round(p2 > 0.5 ? 2 * (1 - p2) : 2 * p2, 3)}");
            
            Console.WriteLine();
            Console.WriteLine(@"Thus, the hypothesis of random distribution and normal distribution is rejected on the result of first image in which positive spatial autocorrelation may exists, while the second image doesn`t demonstrate plausible significance to reject the normal distribution and purely random distribution (simulated by mento carlo test), meaning its distribution to be random.");
            Console.ReadKey();
        }
       
    }
}
