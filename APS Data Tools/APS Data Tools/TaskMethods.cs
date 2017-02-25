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
    class TaskMethods
    {
        protected string sStop = string.Empty;
        DBConnectionGoodies DBConnGoodies04 = new DBConnectionGoodies();
        DataSetAndDatatableGoodies DSandDTGoodies04 = new DataSetAndDatatableGoodies();
        protected string sCDSConnString = APS_Data_Tools.Properties.Settings.Default.CDSConnString.ToString();
        protected string sDP2ConnString = APS_Data_Tools.Properties.Settings.Default.DP2ConnString.ToString();

        public bool IsDigitsOnly(string sString, ref bool bDigitOnly)
        {
            try
            {
                foreach (char c in sString)
                {
                    if (c < '0' || c > '9')
                    {
                        bDigitOnly = false;
                    }
                    else
                    {
                        bDigitOnly = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
            }

            return bDigitOnly;
        }

        public void TranslateCDSCodeToDP2Product(string sCDSCode, ref string sDP2ProductID)
        {
            try
            {

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
            }
        }

        public void GetPackagesForFrame(string sProdNum, string sFrameNum, ref string sLabel2Packages)
        {
            try
            {
                string sSequence = sFrameNum.TrimStart('0');

                DataTable dTblCodes = new DataTable("dTblCodes");
                string sCommText = "SELECT * FROM Codes WHERE Lookupnum = '" + sProdNum + "' AND Sequence = " + sSequence + " AND (Labeltyp = 'P' OR Labeltyp = 'S') ORDER BY Sequence ASC";

                DBConnGoodies04.CDSQuery(sCDSConnString, sCommText, dTblCodes);

                if (dTblCodes.Rows.Count > 0)
                {
                    foreach(DataRow dRowCodes in dTblCodes.Rows)
                    {
                        sLabel2Packages += @">" + Convert.ToString(dRowCodes["Code"]).Trim();
                    }
                }
                else if (dTblCodes.Rows.Count == 0)
                {

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
            }
        }
    }
}
