using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CityNet.Controllers
{
    public class DBAccess
    {
      //  public static string strConn
       //          = "Data Source=10.20.76.187;Initial Catalog=DATA_Manager;Persist Security Info=True;User ID=sa;Password=123456;MultipleActiveResultSets=True";
        public static string strConn
                 = "Data Source=localhost;Initial Catalog=CityNet;Persist Security Info=True;User ID=sa;Password=Zjj20130711;MultipleActiveResultSets=True";

        public static bool Test(string connectStr)
        {
            SqlConnection OleConn = new SqlConnection(connectStr);
            bool res = false;
            try
            {
                OleConn.Open();
                OleConn.Close();
                res = true;
            }
            catch (Exception)
            {
                OleConn.Close();
                res = false;
            }
            return res;
        }

        public static void NoQuery(string sql, IList table)
        {
            SqlConnection OleConn = new SqlConnection(strConn);
            OleConn.Open();
            try
            {

                SqlCommand cmd = new SqlCommand(sql, OleConn);
                if (table != null)
                {
                    foreach (DictionaryEntry de in table)
                    {
                        cmd.Parameters.AddWithValue(de.Key.ToString(), de.Value);
                    }
                }
                cmd.ExecuteNonQuery();
                OleConn.Close();
            }
            catch (Exception ex)
            {
                OleConn.Close();
            }
        }

        public static string getConentFromtable(int id, string tableName, string columnName,string key)
        {
            string sql = "select * from " + tableName + " where [" + key + "] = " + id.ToString();
            DataSet ds = DBAccess.Query(sql, tableName);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][columnName].ToString();
            }
            else
            {
                return "";
            }
        }


        public static string getConentFromtable(int id, string tableName, string columnName)
        {
            string sql = "select * from " + tableName + " where [ID] = " + id.ToString();
            DataSet ds = DBAccess.Query(sql, tableName);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][columnName].ToString();
            }
            else
            {
                return "";
            }
        }

        public static int QueryStatistic(string sql, IList table)
        {
            int result = -1;
            SqlConnection OleConn = new SqlConnection(strConn);
            OleConn.Open();
            try
            {

                SqlCommand cmd = new SqlCommand(sql, OleConn);
                if (table != null)
                {
                    foreach (DictionaryEntry de in table)
                    {
                        cmd.Parameters.AddWithValue(de.Key.ToString(), de.Value);
                    }
                }
                object obj = cmd.ExecuteScalar();
                if (!int.TryParse(obj.ToString(), out result))
                {
                    result = -1;
                }
                OleConn.Close();
            }
            catch (Exception ex)
            {
                OleConn.Close();
            }

            return result;
        }

        public static DataSet Query(string sql, string tableName, IList table)
        {
            SqlConnection OleConn = new SqlConnection(strConn);

            try
            {
                OleConn.Open();

                SqlDataAdapter OleDaExcel = new SqlDataAdapter(sql, OleConn);
                SqlCommand cmd = OleDaExcel.SelectCommand;
                if (table != null)
                {
                    foreach (DictionaryEntry de in table)
                    {
                        cmd.Parameters.AddWithValue(de.Key.ToString(), de.Value);
                    }
                }
                DataSet OleDsExcle = new DataSet();
                OleDaExcel.Fill(OleDsExcle, tableName);
                OleConn.Close();
                return OleDsExcle;
            }
            catch (Exception ex)
            {
                OleConn.Close();
                return null;
            }
        }


        public static DataSet Query(string sql, string tableName)
        {
            SqlConnection OleConn = new SqlConnection(strConn);

            try
            {
                OleConn.Open();

                SqlDataAdapter OleDaExcel = new SqlDataAdapter(sql, OleConn);
                DataSet OleDsExcle = new DataSet();
                OleDaExcel.Fill(OleDsExcle, tableName);
                OleConn.Close();
                return OleDsExcle;
            }
            catch (Exception ex)
            {
                OleConn.Close();
                return null;
            }
        }
    }
}