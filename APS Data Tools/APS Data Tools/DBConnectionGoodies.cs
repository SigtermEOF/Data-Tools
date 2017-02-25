using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace APS_Data_Tools
{
    class DBConnectionGoodies
    {
        protected string sStop = string.Empty;
        string sCDSConnString = APS_Data_Tools.Properties.Settings.Default.CDSConnString.ToString();
        string sDP2ConnString = APS_Data_Tools.Properties.Settings.Default.DP2ConnString.ToString();

        public bool SQLNonQuery(string sConnString, string sCommText, ref bool bSuccess)
        {
            try
            {
                SqlConnection sqlConn = new SqlConnection(sConnString);

                SqlCommand sqlComm = sqlConn.CreateCommand();

                sqlComm.CommandText = sCommText;

                sqlConn.Open();

                sqlComm.ExecuteNonQuery();

                sqlComm.Dispose();

                sqlConn.Close();
                sqlConn.Dispose();

                bSuccess = true;
            }
            catch (Exception ex)
            {
                bSuccess = false;
                MessageBox.Show(ex.ToString().Trim());
            }
            return bSuccess;
        }

        public void SQLQuery(string sConnString, string sCommText, DataTable dTbl)
        {
            try
            {
                SqlConnection sqlConn = new SqlConnection(sConnString);

                SqlCommand sqlComm = sqlConn.CreateCommand();

                sqlComm.CommandText = sCommText;

                sqlConn.Open();

                SqlDataReader sqlDReader = sqlComm.ExecuteReader();

                if (sqlDReader.HasRows)
                {
                    dTbl.Clear();
                    dTbl.Load(sqlDReader);
                }

                sqlDReader.Close();
                sqlDReader.Dispose();

                sqlComm.Dispose();

                sqlConn.Close();
                sqlConn.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
            }
        }

        public void CDSQuery(string sConnString, string sCommText, DataTable dTbl)
        {
            try
            {
                OleDbConnection olDBConn = new OleDbConnection(sConnString);

                OleDbCommand oleDBComm = olDBConn.CreateCommand();

                oleDBComm.CommandText = sCommText;

                olDBConn.Open();

                oleDBComm.CommandTimeout = 0;

                OleDbDataReader oleDBDReader = oleDBComm.ExecuteReader();

                if (oleDBDReader.HasRows)
                {
                    dTbl.Clear();
                    dTbl.Load(oleDBDReader);
                }

                oleDBComm.Dispose();

                oleDBDReader.Close();
                oleDBDReader.Dispose();

                olDBConn.Close();
                olDBConn.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
            }
        }

        public bool CDSNonQuery(string sConnString, string sCommText, ref bool bSuccess)
        {
            try
            {
                OleDbConnection oleDBConn = new OleDbConnection(sConnString);

                OleDbCommand oleDBComm = oleDBConn.CreateCommand();

                oleDBComm.CommandText = sCommText;

                oleDBConn.Open();

                oleDBComm.CommandTimeout = 0;

                oleDBComm.ExecuteNonQuery();

                oleDBComm.Dispose();

                oleDBConn.Close();
                oleDBConn.Dispose();

                bSuccess = true;
            }
            catch (Exception ex)
            {
                bSuccess = false;
                MessageBox.Show(ex.ToString().Trim());
            }
            return bSuccess;
        }

    }
}
