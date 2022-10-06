using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;

namespace SAC
{
    internal class Index
    {
        /// <summary>
        /// 根据二维数据矩阵计算莫兰指数，公式为I=n/S_0*Sum(i)Sum(j)w_{ij}(y_i-y_m)(y_j-y_m)/Sum(n)(y_n-y_m)。
        /// </summary>
        /// <param name="dm">二维数据地图</param>
        /// <returns></returns>
        public static (double,double,double) MoranIndex(List<List<double>> dm)
        {
            double p, z;
            var Length = dm.Count;

            //点-序转换委托
            Func<int, int, int> node_cal = ((int x, int y) => x * (Length + 1) + y);
            Func<int, (int, int)> loca_cal = (int node) => (node / (Length + 1), node % (Length + 1));

            var matrix = CreateMatrix(dm);

            double num = matrix.Count;

            double S = 0;//S_0
            foreach (var row in matrix)
            {
                S += row.Count;
            }

            double total = 0;
            foreach (var row in dm)
            {
                foreach (var item in row)
                    total += item;
            }
            double ym = (double)total / (double)num;

            double fracdown = 0;
            for (var i = 0; i < dm.Count; i++)
            {
                for (var j = 0; j < dm[0].Count; j++)
                {
                    fracdown += Math.Pow(dm[i][j] - ym, 2);
                }
            }

            double fracup = 0;
            for (var i = 0; i < dm.Count; i++)
            {
                for (var j = 0; j < dm[0].Count; j++)
                {
                    var node = node_cal(i, j);
                    var nodes = matrix[node - i];

                    foreach (var ob in nodes)
                    {
                        (int x, int y) = loca_cal(ob);
                        fracup += (dm[i][j] - ym) * (dm[x][y] - ym);
                    }
                }
            }

            double moran = num / S * fracup / fracdown;

            double e = -1 / (num - 1);

            double s_1 = 0;
            for (var i = 0; i < dm.Count; i++)
            {
                for (var j = 0; j < dm[0].Count; j++)
                {
                    var node = node_cal(i, j);
                    var nodes = matrix[node - i];

                    foreach (var ob in nodes)
                    {
                        (int x, int y) = loca_cal(ob);
                        s_1 += 0.5 * Math.Pow(2, 2);
                    }
                }
            }

            double s_2 = 0;
            for (var i = 0; i < dm.Count; i++)
            {
                for (var j = 0; j < dm[0].Count; j++)
                {
                    var node = node_cal(i, j);
                    var nodes = matrix[node - i];

                    s_2 += Math.Pow(2 * nodes.Count, 2);
                }
            }
            double sd = (num * num * s_1 - num * s_2 + 3 * S * S)/(S*S*(num*num-1))-e*e;

            z = (moran - e) / Math.Sqrt(sd);

            p = Normal.CDF(e,Math.Sqrt(sd),moran);
            var p_0 = Normal.CDF(0, 1, z);

            return (moran,z,p);
        }
        /// <summary>
        /// 根据所给二维数据计算Geary Correlation。
        /// </summary>
        /// <param name="dm">二维数据地图</param>
        /// <returns></returns>
        public static (double,double,double) GearyCorrelation(List<List<double>> dm)
        {
            double p, z;
            var Length = dm.Count;

            //点-序转换委托
            Func<int, int, int> node_cal = ((int x, int y) => x * (Length + 1) + y);
            Func<int, (int, int)> loca_cal = (int node) => (node / (Length + 1), node % (Length + 1));

            var matrix = CreateMatrix(dm);

            double num = matrix.Count;

            double S = 0;//S_0
            foreach (var row in matrix)
            {
                S += row.Count;
            }

            double total = 0;
            foreach (var row in dm)
            {
                foreach (var item in row)
                    total += item;
            }
            double ym = (double)total / (double)num;

            double fracup = 0;
            for (var i = 0; i < dm.Count; i++)
            {
                for (var j = 0; j < dm[0].Count; j++)
                {
                    var node = node_cal(i, j);
                    var nodes = matrix[node - i];

                    foreach (var ob in nodes)
                    {
                        (int x, int y) = loca_cal(ob);
                        fracup += Math.Pow((dm[i][j] - dm[x][y]), 2);
                    }
                }
            }

            double fracdown = 0;
            for (var i = 0; i < dm.Count; i++)
            {
                for (var j = 0; j < dm[0].Count; j++)
                {
                    fracdown += Math.Pow(dm[i][j] - ym, 2);
                }
            }
            double geary = (num - 1) / (2 * S) * fracup / fracdown;
 
            double e = 1;

            double s_1 = 0;
            for (var i = 0; i < dm.Count; i++)
            {
                for (var j = 0; j < dm[0].Count; j++)
                {
                    var node = node_cal(i, j);
                    var nodes = matrix[node - i];

                    foreach (var ob in nodes)
                    {
                        (int x, int y) = loca_cal(ob);
                        s_1 += 0.5 * Math.Pow(2, 2);
                    }
                }
            }

            double s_2 = 0;
            for (var i = 0; i < dm.Count; i++)
            {
                for (var j = 0; j < dm[0].Count; j++)
                {
                    var node = node_cal(i, j);
                    var nodes = matrix[node - i];

                    s_2 += Math.Pow(2 * nodes.Count, 2);
                }
            }
            var sd = 1 / (2 * (num + 1) * S * S) * ((2 * s_1 + s_2) * (num - 1) - 4 * S * S);

            z =( (geary) - e) / Math.Sqrt(sd);

            p =Normal.CDF(e, Math.Sqrt(sd), geary);
            var p_0 = Normal.CDF(0, 1, z);

            return (geary,z,p);
        }

        /// <summary>
        /// 根据二维数据创建邻接表稀疏矩阵。
        /// </summary>
        /// <param name="DataMap">二维数据地图</param>
        /// <returns></returns>
        private static List<List<int>> CreateMatrix(List<List<double>> DataMap)
        {
            var Length = DataMap.Count;
            var Width = DataMap[0].Count;

            Func<int, int, int> node_cal = ((int x, int y) => x * (Length + 1) + y);

            List<List<int>> adjacency_matrix = new List<List<int>>();

            var k = 0;
            for (var i = 0; i < DataMap.Count; i++)
                for (var j = 0; j < DataMap[0].Count; j++)
                {
                    var node = node_cal(i, j);
                    adjacency_matrix.Add(new List<int>());
                    //八邻空间权重矩阵
                    int[] adjacency_point = { node - Length - 2, node - Length - 1, node - Length, node - 1, node + 1, node + Length, node + Length + 1, node + Length + 2 };
                    foreach (var point in adjacency_point)
                    {
                        if (point >= 0 && point <= DataMap.Count * DataMap[0].Count - 1 && (point + 1) % 31 != 0)
                        {
                            adjacency_matrix[k].Add(point);
                        }
                    }
                    k++;
                }
            return adjacency_matrix;
        }
    }
}
