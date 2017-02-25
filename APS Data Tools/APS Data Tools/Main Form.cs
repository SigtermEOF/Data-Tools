//*****************************
//#define dev
//*****************************

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
    public partial class Form1 : Form
    {
        TaskMethods TM01 = null;
        DataSetAndDatatableGoodies DSandDTGoodies01 = null;
        DBConnectionGoodies DBConnGoodies01 = null;
        CommonGatheringGoodies CMG01 = null;
        protected string sCDSConnString = APS_Data_Tools.Properties.Settings.Default.CDSConnString.ToString();
        protected string sDP2ConnString = APS_Data_Tools.Properties.Settings.Default.DP2ConnString.ToString();
        protected string sStop = string.Empty;
        DataSet dSetAllTheGoodies = new DataSet("dSetAllTheGoodies");
        protected string sTaskText = string.Empty;

        #region Form events.

        public Form1()
        {
            InitializeComponent();

            TM01 = new TaskMethods();
            DSandDTGoodies01 = new DataSetAndDatatableGoodies();
            DBConnGoodies01 = new DBConnectionGoodies();
            CMG01 = new CommonGatheringGoodies();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ActiveControl = textBox1;
            Application.DoEvents();
            this.Refresh();
        }

        private void rTxtBoxMain_TextChanged(object sender, EventArgs e)
        {
            rTxtBoxMain.SelectionStart = rTxtBoxMain.Text.Length;
            rTxtBoxMain.ScrollToCaret();
            rTxtBoxMain.Refresh();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // On click event verify the user wishes to exit the program.
            DialogResult verifyExit;
            verifyExit = MessageBox.Show("Exit the program?", "Exit?", MessageBoxButtons.YesNo);
            // Exit the application if yes is chosen.
            if (verifyExit == DialogResult.Yes)
            {
                Application.Exit();
            }
            else if (verifyExit == DialogResult.No)
            {
                // Do nothing if the user answers no.
                return;
            }
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            try
            {
                rTxtBoxMain.Clear();
                rTxtBoxMain.Refresh();

                string sInput = textBox1.Text.Trim();

                if (sInput.Length > 0 && sInput != string.Empty)
                {
                    bool bDigitOnly = false;

                    TM01.IsDigitsOnly(sInput, ref bDigitOnly);

                    if (bDigitOnly == true)
                    {
                        bool bClearForm = false;

                        sTaskText = "Gathering order data from CDS.";
                        this.LogText(sTaskText);

                        CMG01.GatherOrderData(sInput, dSetAllTheGoodies, ref bClearForm); // Gather DP2.Orders data from CDS.

                        if (bClearForm == true)
                        {
                            sTaskText = "Clearing form due to an error.";
                            this.LogText(sTaskText);

                            this.Clear();
                        }
                        else if (bClearForm != true)
                        {
                            sTaskText = "Gathering initial SubjectInfo data from CDS.";
                            this.LogText(sTaskText);

                            CMG01.GatherSubjectInfoData(sInput, dSetAllTheGoodies, ref bClearForm); // Gather initial DP2.SubjectInfo<suffix> data from CDS.

                            if (bClearForm == true)
                            {
                                sTaskText = "Clearing form due to an error.";
                                this.LogText(sTaskText);

                                this.Clear();
                            }
                            else if (bClearForm != true)
                            {
                                sTaskText = "Gathering any color correction or cropping data from DP2.";
                                this.LogText(sTaskText);

                                CMG01.PullDP2ImagesEditInfoIntoCDSDP2Image(dSetAllTheGoodies, ref bClearForm); // Gather DP2.Images data for back writing of editing info to CDS.Dp2image.

                                if (bClearForm == true)
                                {
                                    sTaskText = "Clearing form due to an error.";
                                    this.LogText(sTaskText);

                                    this.Clear();
                                }
                                else if (bClearForm != true)
                                {
                                    sTaskText = "Updating CDS with cropping or color correction data from DP2.";
                                    this.LogText(sTaskText);

                                    DSandDTGoodies01.UpdateCDSDp2imageWithDP2ImagesEditedInfo(dSetAllTheGoodies, ref bClearForm); // Update CDS.Dp2image with DP2.Images data related to edited images.

                                    if (bClearForm == true)
                                    {
                                        sTaskText = "Clearing form due to an error.";
                                        this.LogText(sTaskText);

                                        this.Clear();
                                    }
                                    else if (bClearForm != true)
                                    {
                                        sTaskText = "Gathering image data from CDS.";
                                        this.LogText(sTaskText);

                                        CMG01.GatherImageDataFromCDS(sInput, dSetAllTheGoodies, ref bClearForm); // Gather CDS.Dp2image data.

                                        if (bClearForm == true)
                                        {
                                            sTaskText = "Clearing form due to an error.";
                                            this.LogText(sTaskText);

                                            this.Clear();
                                        }
                                        else if (bClearForm != true)
                                        {
                                            sTaskText = "Gathering Codes data from CDS.";
                                            this.LogText(sTaskText);

                                            CMG01.GatherCodes(sInput, dSetAllTheGoodies, ref bClearForm); // Gather codes associated with each frame of a job and populate final datatable for DP2.SubjectInfo<suffix> records.

                                            if (bClearForm == true)
                                            {
                                                sTaskText = "Clearing form due to an error.";
                                                this.LogText(sTaskText);

                                                this.Clear();
                                            }
                                            else if (bClearForm != true)
                                            {
                                                sTaskText = "Deleting DP2 Images data as well as DP2 SubjectInfo data.";
                                                this.LogText(sTaskText);

                                                DSandDTGoodies01.ClearDP2Records(dSetAllTheGoodies, ref bClearForm); // Delete DP2.SubjectInfo<suffix> and DP2.Images data.

                                                if (bClearForm == true)
                                                {
                                                    sTaskText = "Clearing form due to an error.";
                                                    this.LogText(sTaskText);

                                                    this.Clear();
                                                }
                                                else if (bClearForm != true)
                                                {
                                                    sTaskText = "Inserting/Updating records into DP2.";
                                                    this.LogText(sTaskText);

                                                    DSandDTGoodies01.InsertRecordsIntoTables(dSetAllTheGoodies, ref bClearForm); // Loop through gathered records and insert/update records in DP2.Orders, DP2.Images and DP2.SubjectInfo,suffix>.

                                                    if (bClearForm == true)
                                                    {
                                                        sTaskText = "Clearing form due to an error.";
                                                        this.LogText(sTaskText);

                                                        this.Clear();
                                                    }
                                                    else if (bClearForm != true)
                                                    {
                                                        sTaskText = "Complete.";
                                                        this.LogText(sTaskText);

                                                        this.Clear();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (bDigitOnly != true)
                    {
                        MessageBox.Show("Please enter a numeric production number.");
                        this.Clear();
                    }
                }
                else if (sInput.Length == 0 || sInput == string.Empty)
                {
                    MessageBox.Show("Please enter a production number.");
                    this.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
                this.Clear();
            }
        }

        #endregion

        #region Form specific methods.

        private void Clear()
        {
            try
            {
                dSetAllTheGoodies.Clear();
                dSetAllTheGoodies.Dispose();

                while (dSetAllTheGoodies.Tables.Count > 0)
                {
                    DataTable dTbl = dSetAllTheGoodies.Tables[0];
                    if (dSetAllTheGoodies.Tables.CanRemove(dTbl))
                    {
                        dSetAllTheGoodies.Tables.Remove(dTbl);
                    }
                }

                sTaskText = string.Empty;
                this.textBox1.Text = string.Empty;
                this.ActiveControl = textBox1;
                Application.DoEvents();
                this.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
            }
        }

        private void SetLogText(string sLogText, string sDTime3)
        {
            try
            {
                rTxtBoxMain.AppendText(sLogText + Environment.NewLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
            }
        }

        public void LogText(string sTaskText)
        {
            try
            {
                string sDTime1 = DateTime.Now.ToString("MM-dd-yy").Trim();
                string sDTime2 = DateTime.Now.ToString("HH:mm:ss").Trim();
                string sDTime3 = "[" + sDTime1 + "][" + sDTime2 + "]";
                string sLogText = sDTime3 + " " + sTaskText;
                this.SetLogText(sLogText, sDTime3);

                Application.DoEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
            }
        }

        #endregion



    }
}
