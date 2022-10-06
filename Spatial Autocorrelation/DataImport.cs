using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAC
{
    internal class DataImport
    {
        /// <summary>
        /// 从Excel中读取指定工作表的全部数据（以最大非空角结束），返回数据表，仅支持Windows平台。
        /// </summary>
        /// <param name="sheet_name">工作表名称</param>
        /// <returns></returns>
        public static DataTable  ImportFromExcel(string sheet_name)
        {
            string path = $"Resource/Excel/Data.xls";//资源路径

            try
            {
                string connStr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + path + ";" + ";Extended Properties=\"Excel 12.0;HDR=NO;IMEX=1\"";
                using (OleDbConnection conn = new OleDbConnection(connStr))
                {
                    conn.Open();
                    using (OleDbCommand cmd=conn.CreateCommand())
                    {
                        cmd.CommandText = $"select * from [{sheet_name}$]";
                        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                        DataSet ds=new DataSet();
                        adapter.Fill(ds);
                        return ds.Tables[0];
                    }
                }
            }
            catch
            {
                return new DataTable();
            }
        }
        /// <summary>
        /// 将Excel工作表转换为二维列表。
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<List<double>> DT2List(DataTable dt)
        {
            var rows = dt.Rows.Count;
            var columns = dt.Rows.Count;
            List<List<double>> dm = new List<List<double>>();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                dm.Add(new List<double>());
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    dm[i].Add(double.Parse(dt.Rows[i][j].ToString()));
                }
            }
            return dm;
        }
    }
}
