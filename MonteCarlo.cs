using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SAC
{
    internal class MonteCarlo
    {

        /// <summary>
        /// Towards particular spatial autocorrelation index, run mentocarlo simulation and return a mentocarlo P value with given mapsize defined by length and width.
        /// </summary>
        /// <param name="ACindex">Index with z,p</param>
        /// <param name="times">Times of simulation</param>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static double MentoCarloSimulation(Func<List<List<double>>, (double, double,double)> ACindex, int times, int length, int width,double statistic)
        {
            var dic=FreqCal(ACindex, times, length, width);
            return MentoCarloP(dic, times,statistic);
        }
        private static List<List<double>> MapCreate(int length,int width)
        {
            var generator = new Random();
            var x = 0;
            var map = new List<List<double>>();
            for (var i = 0; i < length; i++)
            {
                map.Add(new List<double>());
                for(var j=0;j<width;j++)
                {
                    x = generator.Next(0, 5);
                    map[i].Add(x);
                }
            }
            return map;
        }

        private static Dictionary<double, int> FreqCal(Func<List<List<double>>, (double,double,double)> ACindex, int times,int length,int width)
        {
            Dictionary<double, int> result = new Dictionary<double, int>();
            for (var i = 0; i < times; i++)
            {
                double z = 0;
                double p=0;
                double index = 0;
                var map=MapCreate(length, width);
                (index,z,p)=ACindex(map);
                index = Math.Round(index,3);
                if(!result.ContainsKey(index))
                {
                    result.Add(index, 1);
                }
                else
                {
                    result[index]++;
                }
            }
            return result;
        }
        /// <summary>
        /// Calculate the two-tail P value with given mento carlo result.
        /// </summary>
        /// <param name="freq">mento carlo frequency table</param>
        /// <param name="times"></param>
        /// <param name="statistic"></param>
        /// <returns></returns>
        private static double MentoCarloP(Dictionary<double,int> freq,int times,double statistic)
        { 
            var min=freq.Keys.Min();
            var max=freq.Keys.Max();
            double p = 0;
            for (var i = min; i <=statistic; i+=0.001)
            {
                i= Math.Round(i,3);
                if (freq.ContainsKey(i))
                {
                    p += freq[i];
                }
            }
            var _statistic = (max+min-statistic);
            double _p = 0;
            for (var i = min; i <= _statistic; i += 0.001)
            {
                i = Math.Round(i, 3);
                if (freq.ContainsKey(i))
                {
                    _p += freq[i];
                }
            }
            var result = 0.0;
            return result = 1-Math.Abs(_p-p)/times;
                                     
        }
        /// <summary>
        /// 施工中，直方图绘图。
        /// </summary>
        /// <param name="freq"></param>
        public static void FreqPlot(Dictionary<double,double> freq)
        {

        }
    }
}
