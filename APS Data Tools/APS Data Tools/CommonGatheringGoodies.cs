﻿using System;
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
    class CommonGatheringGoodies
    {
        protected string sStop = string.Empty;
        protected string sCDSConnString = APS_Data_Tools.Properties.Settings.Default.CDSConnString.ToString();
        protected string sDP2ConnString = APS_Data_Tools.Properties.Settings.Default.DP2ConnString.ToString();

        DataSetAndDatatableGoodies DSandDTGoodies02 = new DataSetAndDatatableGoodies();
        DBConnectionGoodies DBConnGoodies02 = new DBConnectionGoodies();
        TaskMethods TM02 = new TaskMethods();

        public void GatherOrderData(string sInput, DataSet dSetAllTheGoodies, ref bool bClearForm)
        {
            try
            {
                bool bOrdersPrepTableSetUp = false;
                DataTable dTblOrdersPrepTable = new DataTable("dTblOrdersPrepTable");

                DSandDTGoodies02.SetUpOrdersPrepTable(dTblOrdersPrepTable, ref bOrdersPrepTableSetUp);

                if (bOrdersPrepTableSetUp == true)
                {
                    dSetAllTheGoodies.Tables.Add(dTblOrdersPrepTable);
                    DataRow dRowOrdersPrepTable = dSetAllTheGoodies.Tables["dTblOrdersPrepTable"].NewRow();

                    string sLogo = string.Empty;
                    string sSubjectInfoType = "SportsAIO";

                    DataTable dTblItems = new DataTable("dTblItems");
                    string sCommText = "SELECT * FROM Items WHERE Lookupnum = '" + sInput + "'";

                    DBConnGoodies02.CDSQuery(sCDSConnString, sCommText, dTblItems);

                    if (dTblItems.Rows.Count > 0) // Validate the entered number as a current production number.
                    {
                        dSetAllTheGoodies.Tables.Add(dTblItems);

                        string sCustNum = Convert.ToString(dTblItems.Rows[0]["customer"]).Trim();
                        string sRefNum = Convert.ToString(dTblItems.Rows[0]["order"]).Trim();
                        string sPathToImage = Convert.ToString(dTblItems.Rows[0]["Imgloc"]).Trim();

                        DataTable dTblFrames = new DataTable("dTblFrames");
                        sCommText = "SELECT Lookupnum, Sequence, Sitting, Image_id, Camid FROM Frames WHERE Lookupnum = '" + sInput + "' ORDER BY Sequence ASC";

                        DBConnGoodies02.CDSQuery(sCDSConnString, sCommText, dTblFrames);

                        if (dTblFrames.Rows.Count > 0)
                        {
                            dSetAllTheGoodies.Tables.Add(dTblFrames);

                            string sSearchString = "sitting = 'LOGO'";
                            DataRow[] dRowSearchedRows = dTblFrames.Select(sSearchString);

                            foreach (DataRow dRow in dRowSearchedRows)
                            {
                                sLogo = Convert.ToString(dRow["Image_id"]).Trim();
                            }

                            if (sLogo != string.Empty || sLogo != "" || sLogo.Length > 0)
                            {
                                sLogo = sPathToImage + sLogo;
                            }
                            else if(sLogo == string.Empty || sLogo == "" || sLogo.Length == 0)
                            {
                                sLogo = "";
                            }                            

                            DataTable dTblGroup = new DataTable("dTblGroup");
                            sCommText = "SELECT * FROM Group WHERE Lookupnum = '" + sInput + "' ORDER BY Sequence ASC";

                            DBConnGoodies02.CDSQuery(sCDSConnString, sCommText, dTblGroup);

                            if (dTblGroup.Rows.Count > 0)
                            {
                                dSetAllTheGoodies.Tables.Add(dTblGroup);

                                string sSchoolName = Convert.ToString(dTblGroup.Rows[0]["School"]).Trim();
                                string sPrincipal = Convert.ToString(dTblGroup.Rows[0]["Principal"]).Trim();
                                string sCurrentYear = Convert.ToString(dTblGroup.Rows[0]["Year"]);

                                string sInsertCommText = "INSERT INTO [Orders] (ID,CustomerID,InfoType,Logo,Description,OrderName,Owner) VALUES" +
                                    " ('" + sRefNum + "','" + sCustNum + "','" + sSubjectInfoType + "','" + sLogo + "','" + sSchoolName + "','" + sPrincipal + "','" + sCurrentYear + "')";
                                string sUpdateCommText = "UPDATE [Orders] SET [InfoType] = '" + sSubjectInfoType + "', [Logo] = '" + sLogo + "', [Description] = '" + sSchoolName + "', [OrderName] = '" +
                                    sPrincipal + "', [Owner] = '" + sCurrentYear + "' WHERE [ID] = '" + sRefNum + "'";

                                dRowOrdersPrepTable["RefNum"] = sRefNum;
                                dRowOrdersPrepTable["CustNum"] = sCustNum;
                                dRowOrdersPrepTable["SubjectInfoType"] = sSubjectInfoType;
                                dRowOrdersPrepTable["Logo"] = sLogo;
                                dRowOrdersPrepTable["SchoolName"] = sSchoolName;
                                dRowOrdersPrepTable["Principal"] = sPrincipal;
                                dRowOrdersPrepTable["CurrentYear"] = sCurrentYear;
                                dRowOrdersPrepTable["InsertCommText"] = sInsertCommText;
                                dRowOrdersPrepTable["UpdateCommText"] = sUpdateCommText;

                                dSetAllTheGoodies.Tables["dTblOrdersPrepTable"].Rows.Add(dRowOrdersPrepTable.ItemArray);
                            }
                            else if (dTblGroup.Rows.Count == 0)
                            {
                                sStop = string.Empty;
                            }
                        }
                        else if (dTblFrames.Rows.Count == 0)
                        {
                            MessageBox.Show("No Frames records for entered production number.");
                            bClearForm = true;
                        }
                    }
                    else if (dTblItems.Rows.Count == 0)
                    {
                        MessageBox.Show("Invalid production number.");
                        bClearForm = true;
                    }
                }
                else if (bOrdersPrepTableSetUp != true)
                {
                    MessageBox.Show("Failed to setup OrdersPrepTable.");
                    bClearForm = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
                bClearForm = true;
            }
        }

        public void GatherSubjectInfoData(string sInput, DataSet dSetAllTheGoodies, ref bool bClearForm)
        {
            try
            {
                string sTeamPicPath = string.Empty;
                string sLineName1 = string.Empty;
                string sLineName2 = string.Empty;
                string sLineName3 = string.Empty;
                string sLineName4 = string.Empty;
                string sLineName5 = string.Empty;
                string sLineName6 = string.Empty;
                string sLineName7 = string.Empty;
                string sLineName8 = string.Empty;
                string sLineData1 = string.Empty;
                string sLineData2 = string.Empty;
                string sLineData3 = string.Empty;
                string sLineData4 = string.Empty;
                string sLineData5 = string.Empty;
                string sLineData6 = string.Empty;
                string sLineData7 = string.Empty;
                string sLineData8 = string.Empty;
                string sFirstName = string.Empty;
                string sLastName = string.Empty;
                string sSchoolName = string.Empty;

                DataTable dTblGroup = new DataTable("dTblGroup");
                string sCommText = "SELECT * FROM Group WHERE Lookupnum = '" + sInput + "' ORDER BY Sequence ASC";

                DBConnGoodies02.CDSQuery(sCDSConnString, sCommText, dTblGroup);

                if (dTblGroup.Rows.Count > 0)
                {
                    bool bSubjectInfoPrepTableBuilt = false;
                    DataTable dTblSubjectInfoPrep = new DataTable("dTblSubjectInfoPrep");

                    DSandDTGoodies02.SetUpSubjectInfoPrepTable(dTblSubjectInfoPrep, ref bSubjectInfoPrepTableBuilt);

                    if (bSubjectInfoPrepTableBuilt == true)
                    {
                        foreach (DataRow dRowdTblGroup in dTblGroup.Rows)
                        {
                            string sProdNum = Convert.ToString(dRowdTblGroup["Lookupnum"]).Trim();
                            string sSequence = Convert.ToString(dRowdTblGroup["Sequence"]).Trim();
                            string sSitting = Convert.ToString(dRowdTblGroup["Sitting"]);
                            string sGroupID = Convert.ToString(dRowdTblGroup["Groupid"]).Trim();
                            string sShotType = Convert.ToString(dRowdTblGroup["Shottype"]).Trim();
                            string sClassNames = Convert.ToString(dRowdTblGroup["Classnames"]);
                            string sYear = Convert.ToString(dRowdTblGroup["Year"]).Trim();
                            string sGradeTitle = Convert.ToString(dRowdTblGroup["Grdtitle"]).Trim();
                            string sPrincipal = Convert.ToString(dRowdTblGroup["Principal"]).Trim();
                            string sTeacher = Convert.ToString(dRowdTblGroup["Teachers"]).Trim();
                            sFirstName = Convert.ToString(dRowdTblGroup["First_name"]).Trim();
                            sLastName = Convert.ToString(dRowdTblGroup["Last_name"]).Trim();
                            sSchoolName = Convert.ToString(dRowdTblGroup["School"]).Trim();

                            if (sShotType == "I" || sShotType == "G")
                            {
                                DataTable dTblGetTeamPicPath = new DataTable("dTblGetTeamPicPath");
                                sCommText = "SELECT * FROM Group WHERE Lookupnum = '" + sProdNum + "' AND Shottype = 'G' AND Sitting = '" + sGroupID + "'";

                                DBConnGoodies02.CDSQuery(sCDSConnString, sCommText, dTblGetTeamPicPath);

                                if (dTblGetTeamPicPath.Rows.Count > 0)
                                {
                                    string sTeamPicPathSequence = Convert.ToString(dTblGetTeamPicPath.Rows[0]["sequence"]).Trim();
                                    string sTeamPicPathSitting = Convert.ToString(dTblGetTeamPicPath.Rows[0]["sitting"]).Trim();

                                    DataTable dTblTeamPicPathFromDP2Image = new DataTable("dTblTeamPicPathFromDP2Image");
                                    sCommText = "SELECT Path FROM Dp2image WHERE Lookupnum = '" + sProdNum + "' AND Frame = " + sTeamPicPathSequence + " AND Sitting = '" + sTeamPicPathSitting + "'";

                                    DBConnGoodies02.CDSQuery(sCDSConnString, sCommText, dTblTeamPicPathFromDP2Image);

                                    if (dTblTeamPicPathFromDP2Image.Rows.Count > 0)
                                    {
                                        sTeamPicPath = Convert.ToString(dTblTeamPicPathFromDP2Image.Rows[0]["Path"]).Trim();
                                    }
                                    else if (dTblTeamPicPathFromDP2Image.Rows.Count == 0)
                                    {
                                        sStop = string.Empty;
                                    }
                                }
                                else if (dTblGetTeamPicPath.Rows.Count == 0)
                                {
                                    sStop = string.Empty;
                                }
                            }
                            else if (sShotType != "I" || sShotType != "G")
                            {

                            }

                            DataTable dTblEndCust = new DataTable("dTblEndCust");
                            sCommText = "SELECT * FROM Endcust WHERE Lookupnum = '" + sProdNum + "' AND Sequence = " + sSequence + " AND Sitting = '" + sSitting + "'";

                            DBConnGoodies02.CDSQuery(sCDSConnString, sCommText, dTblEndCust);

                            if (dTblEndCust.Rows.Count > 0)
                            {
                                string sWName = Convert.ToString(dTblEndCust.Rows[0]["Wname"]).Trim();

                                if (sFirstName.Length == 0 || sFirstName == string.Empty)
                                {
                                    sFirstName = Convert.ToString(dTblEndCust.Rows[0]["First_name"]).Trim();
                                }
                                if (sLastName.Length == 0 || sLastName == string.Empty)
                                {
                                    sLastName = Convert.ToString(dTblEndCust.Rows[0]["Last_name"]).Trim();
                                }
                                if (sSchoolName.Length == 0 || sSchoolName == string.Empty)
                                {
                                    sSchoolName = Convert.ToString(dTblEndCust.Rows[0]["Schoolname"]).Trim();
                                }
                                if (sTeacher.Length == 0 || sTeacher == string.Empty)
                                {
                                    sTeacher = Convert.ToString(dTblEndCust.Rows[0]["Teacher"]).Trim();
                                }

                                DataTable dTblDP2Image = new DataTable("dTblDP2Image");
                                sCommText = "SELECT Path FROM Dp2image WHERE Lookupnum = '" + sProdNum + "' AND Frame = " + sSequence + " AND Sitting = '" + sSitting + "'";

                                DBConnGoodies02.CDSQuery(sCDSConnString, sCommText, dTblDP2Image);

                                if (dTblDP2Image.Rows.Count > 0)
                                {
                                    string sImagePath = Convert.ToString(dTblDP2Image.Rows[0]["Path"]).Trim();

                                    DataTable dTblGroupLine = new DataTable("dTblGroupLine");
                                    sCommText = "SELECT * FROM Group_line WHERE Lookupnum = '" + sInput + "' AND Sequence = " + sSequence;

                                    DBConnGoodies02.CDSQuery(sCDSConnString, sCommText, dTblGroupLine);

                                    if (dTblGroupLine.Rows.Count > 0)
                                    {
                                        int idTblGroupLineRowCount = dTblGroupLine.Rows.Count;

                                        if (idTblGroupLineRowCount == 1)
                                        {
                                            sLineName1 = Convert.ToString(dTblGroupLine.Rows[0]["Linename"]).Trim();
                                            sLineData1 = Convert.ToString(dTblGroupLine.Rows[0]["Linedata"]);
                                            sLineData2 = string.Empty;
                                            sLineData3 = string.Empty;
                                            sLineData4 = string.Empty;
                                            sLineData5 = string.Empty;
                                            sLineData6 = string.Empty;
                                            sLineData7 = string.Empty;
                                            sLineData8 = string.Empty;

                                        }
                                        else if (idTblGroupLineRowCount == 2)
                                        {
                                            sLineName1 = Convert.ToString(dTblGroupLine.Rows[0]["Linename"]).Trim();
                                            sLineData1 = Convert.ToString(dTblGroupLine.Rows[0]["Linedata"]);
                                            sLineName2 = Convert.ToString(dTblGroupLine.Rows[1]["Linename"]).Trim();
                                            sLineData2 = Convert.ToString(dTblGroupLine.Rows[1]["Linedata"]);
                                            sLineData3 = string.Empty;
                                            sLineData4 = string.Empty;
                                            sLineData5 = string.Empty;
                                            sLineData6 = string.Empty;
                                            sLineData7 = string.Empty;
                                            sLineData8 = string.Empty;
                                        }
                                        else if (idTblGroupLineRowCount == 3)
                                        {
                                            sLineName1 = Convert.ToString(dTblGroupLine.Rows[0]["Linename"]).Trim();
                                            sLineData1 = Convert.ToString(dTblGroupLine.Rows[0]["Linedata"]);
                                            sLineName2 = Convert.ToString(dTblGroupLine.Rows[1]["Linename"]).Trim();
                                            sLineData2 = Convert.ToString(dTblGroupLine.Rows[1]["Linedata"]);
                                            sLineName3 = Convert.ToString(dTblGroupLine.Rows[2]["Linename"]).Trim();
                                            sLineData3 = Convert.ToString(dTblGroupLine.Rows[2]["Linedata"]);
                                            sLineData4 = string.Empty;
                                            sLineData5 = string.Empty;
                                            sLineData6 = string.Empty;
                                            sLineData7 = string.Empty;
                                            sLineData8 = string.Empty;
                                        }
                                        else if (idTblGroupLineRowCount == 4)
                                        {
                                            sLineName1 = Convert.ToString(dTblGroupLine.Rows[0]["Linename"]).Trim();
                                            sLineData1 = Convert.ToString(dTblGroupLine.Rows[0]["Linedata"]);
                                            sLineName2 = Convert.ToString(dTblGroupLine.Rows[1]["Linename"]).Trim();
                                            sLineData2 = Convert.ToString(dTblGroupLine.Rows[1]["Linedata"]);
                                            sLineName3 = Convert.ToString(dTblGroupLine.Rows[2]["Linename"]).Trim();
                                            sLineData3 = Convert.ToString(dTblGroupLine.Rows[2]["Linedata"]);
                                            sLineName4 = Convert.ToString(dTblGroupLine.Rows[3]["Linename"]).Trim();
                                            sLineData4 = Convert.ToString(dTblGroupLine.Rows[3]["Linedata"]);
                                            sLineData5 = string.Empty;
                                            sLineData6 = string.Empty;
                                            sLineData7 = string.Empty;
                                            sLineData8 = string.Empty;
                                        }
                                        else if (idTblGroupLineRowCount == 5)
                                        {
                                            sLineName1 = Convert.ToString(dTblGroupLine.Rows[0]["Linename"]).Trim();
                                            sLineData1 = Convert.ToString(dTblGroupLine.Rows[0]["Linedata"]);
                                            sLineName2 = Convert.ToString(dTblGroupLine.Rows[1]["Linename"]).Trim();
                                            sLineData2 = Convert.ToString(dTblGroupLine.Rows[1]["Linedata"]);
                                            sLineName3 = Convert.ToString(dTblGroupLine.Rows[2]["Linename"]).Trim();
                                            sLineData3 = Convert.ToString(dTblGroupLine.Rows[2]["Linedata"]);
                                            sLineName4 = Convert.ToString(dTblGroupLine.Rows[3]["Linename"]).Trim();
                                            sLineData4 = Convert.ToString(dTblGroupLine.Rows[3]["Linedata"]);
                                            sLineName5 = Convert.ToString(dTblGroupLine.Rows[4]["Linename"]).Trim();
                                            sLineData5 = Convert.ToString(dTblGroupLine.Rows[4]["Linedata"]);
                                            sLineData6 = string.Empty;
                                            sLineData7 = string.Empty;
                                            sLineData8 = string.Empty;
                                        }
                                        else if (idTblGroupLineRowCount == 6)
                                        {
                                            sLineName1 = Convert.ToString(dTblGroupLine.Rows[0]["Linename"]).Trim();
                                            sLineData1 = Convert.ToString(dTblGroupLine.Rows[0]["Linedata"]);
                                            sLineName2 = Convert.ToString(dTblGroupLine.Rows[1]["Linename"]).Trim();
                                            sLineData2 = Convert.ToString(dTblGroupLine.Rows[1]["Linedata"]);
                                            sLineName3 = Convert.ToString(dTblGroupLine.Rows[2]["Linename"]).Trim();
                                            sLineData3 = Convert.ToString(dTblGroupLine.Rows[2]["Linedata"]);
                                            sLineName4 = Convert.ToString(dTblGroupLine.Rows[3]["Linename"]).Trim();
                                            sLineData4 = Convert.ToString(dTblGroupLine.Rows[3]["Linedata"]);
                                            sLineName5 = Convert.ToString(dTblGroupLine.Rows[4]["Linename"]).Trim();
                                            sLineData5 = Convert.ToString(dTblGroupLine.Rows[4]["Linedata"]);
                                            sLineName6 = Convert.ToString(dTblGroupLine.Rows[5]["Linename"]).Trim();
                                            sLineData6 = Convert.ToString(dTblGroupLine.Rows[5]["Linedata"]);
                                            sLineData7 = string.Empty;
                                            sLineData8 = string.Empty;
                                        }
                                        else if (idTblGroupLineRowCount == 7)
                                        {
                                            sLineName1 = Convert.ToString(dTblGroupLine.Rows[0]["Linename"]).Trim();
                                            sLineData1 = Convert.ToString(dTblGroupLine.Rows[0]["Linedata"]);
                                            sLineName2 = Convert.ToString(dTblGroupLine.Rows[1]["Linename"]).Trim();
                                            sLineData2 = Convert.ToString(dTblGroupLine.Rows[1]["Linedata"]);
                                            sLineName3 = Convert.ToString(dTblGroupLine.Rows[2]["Linename"]).Trim();
                                            sLineData3 = Convert.ToString(dTblGroupLine.Rows[2]["Linedata"]);
                                            sLineName4 = Convert.ToString(dTblGroupLine.Rows[3]["Linename"]).Trim();
                                            sLineData4 = Convert.ToString(dTblGroupLine.Rows[3]["Linedata"]);
                                            sLineName5 = Convert.ToString(dTblGroupLine.Rows[4]["Linename"]).Trim();
                                            sLineData5 = Convert.ToString(dTblGroupLine.Rows[4]["Linedata"]);
                                            sLineName6 = Convert.ToString(dTblGroupLine.Rows[5]["Linename"]).Trim();
                                            sLineData6 = Convert.ToString(dTblGroupLine.Rows[5]["Linedata"]);
                                            sLineName7 = Convert.ToString(dTblGroupLine.Rows[6]["Linename"]).Trim();
                                            sLineData7 = Convert.ToString(dTblGroupLine.Rows[6]["Linedata"]);
                                            sLineData8 = string.Empty;
                                        }
                                        else if (idTblGroupLineRowCount == 8)
                                        {
                                            sLineName1 = Convert.ToString(dTblGroupLine.Rows[0]["Linename"]).Trim();
                                            sLineData1 = Convert.ToString(dTblGroupLine.Rows[0]["Linedata"]);
                                            sLineName2 = Convert.ToString(dTblGroupLine.Rows[1]["Linename"]).Trim();
                                            sLineData2 = Convert.ToString(dTblGroupLine.Rows[1]["Linedata"]);
                                            sLineName3 = Convert.ToString(dTblGroupLine.Rows[2]["Linename"]).Trim();
                                            sLineData3 = Convert.ToString(dTblGroupLine.Rows[2]["Linedata"]);
                                            sLineName4 = Convert.ToString(dTblGroupLine.Rows[3]["Linename"]).Trim();
                                            sLineData4 = Convert.ToString(dTblGroupLine.Rows[3]["Linedata"]);
                                            sLineName5 = Convert.ToString(dTblGroupLine.Rows[4]["Linename"]).Trim();
                                            sLineData5 = Convert.ToString(dTblGroupLine.Rows[4]["Linedata"]);
                                            sLineName6 = Convert.ToString(dTblGroupLine.Rows[5]["Linename"]).Trim();
                                            sLineData6 = Convert.ToString(dTblGroupLine.Rows[5]["Linedata"]);
                                            sLineName7 = Convert.ToString(dTblGroupLine.Rows[6]["Linename"]).Trim();
                                            sLineData7 = Convert.ToString(dTblGroupLine.Rows[6]["Linedata"]);
                                            sLineName8 = Convert.ToString(dTblGroupLine.Rows[7]["Linename"]).Trim();
                                            sLineData8 = Convert.ToString(dTblGroupLine.Rows[7]["Linedata"]);
                                        }
                                    }
                                    else if (dTblGroupLine.Rows.Count == 0)
                                    {

                                    }

                                    dTblSubjectInfoPrep.Rows.Add(sProdNum, sSequence, sSitting, sGroupID, sShotType, sFirstName, sLastName, sSchoolName, sPrincipal, sTeacher, sGradeTitle, sYear, sWName, sImagePath, sTeamPicPath, sLineData1, sLineData2, sLineData3, sLineData4, sLineData5, sLineData6, sLineData7, sLineData8, sClassNames);
                                }
                                else if (dTblDP2Image.Rows.Count == 0)
                                {
                                    MessageBox.Show("Failed to gather path to image.");
                                    bClearForm = true;
                                }
                            }
                            else if (dTblEndCust.Rows.Count == 0)
                            {
                                MessageBox.Show("Failed to gather Endcust data.");
                                bClearForm = true;
                            }

                        }

                        // End of foreach.
                        sStop = string.Empty;

                        dSetAllTheGoodies.Tables.Add(dTblSubjectInfoPrep);

                    }
                    else if (bSubjectInfoPrepTableBuilt != true)
                    {
                        MessageBox.Show("Failed to build SubjectInfoPrepTable.");
                        bClearForm = true;
                    }
                }
                else if (dTblGroup.Rows.Count == 0)
                {
                    MessageBox.Show("Failed to gather data from Group table.");
                    bClearForm = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
                bClearForm = true;
            }
        }

        public void PullDP2ImagesEditInfoIntoCDSDP2Image(DataSet dSetAllTheGoodies, ref bool bClearForm)
        {
            try
            {
                bool bValueChanged = false;
                string sInsertCommand = string.Empty;
                string sUpdateCommand = string.Empty;
                bool bFileExists = false;

                bool bImageDataPrepTableSetup = false;
                DataTable dTblImageData = new DataTable("dTblImageData");

                DSandDTGoodies02.SetUpImageDataPrepTable(dTblImageData, ref bImageDataPrepTableSetup);

                dSetAllTheGoodies.Tables.Add(dTblImageData);

                if (dSetAllTheGoodies.Tables["dTblItems"].Rows.Count > 0 && bImageDataPrepTableSetup == true)
                {
                    string sRefNum = Convert.ToString(dSetAllTheGoodies.Tables["dTblItems"].Rows[0]["Order"]).Trim();

                    if (dSetAllTheGoodies.Tables["dTblFrames"].Rows.Count > 0)
                    {
                        foreach (DataRow dRowFrames in dSetAllTheGoodies.Tables["dTblFrames"].Rows)
                        {
                            string sFramesLookupnum = Convert.ToString(dRowFrames["Lookupnum"]).Trim();
                            string sFramesSequence = Convert.ToString(dRowFrames["Sequence"]).Trim();
                            string sDP2FrameNum = sFramesSequence.PadLeft(4, '0');
                            string sFramesSitting = Convert.ToString(dRowFrames["Sitting"]).Trim();
                            string sFramesImage_id = Convert.ToString(dRowFrames["Image_id"]).ToUpper().Trim();

                            DataTable dTblDP2Images = new DataTable("dTblDP2Images");
                            string sCommText = "SELECT * FROM [Images] WHERE [OrderID] = '" + sRefNum + "' AND [Roll] = '" + sFramesLookupnum + "' AND [Frame] = '" + sDP2FrameNum + "'";

                            DBConnGoodies02.SQLQuery(sDP2ConnString, sCommText, dTblDP2Images);

                            if (dTblDP2Images.Rows.Count > 0)
                            {
                                int iDP2ImagesWidth = Convert.ToInt32(dTblDP2Images.Rows[0]["Width"]);
                                int iDP2ImagesLength = Convert.ToInt32(dTblDP2Images.Rows[0]["Length"]);
                                int iDP2ImagesPWidth = Convert.ToInt32(dTblDP2Images.Rows[0]["PreviewWidth"]); //always write back to cds
                                int iDP2ImagesPLength = Convert.ToInt32(dTblDP2Images.Rows[0]["PreviewLength"]); //always write back to cds
                                int iDP2ImagesCropX = Convert.ToInt32(dTblDP2Images.Rows[0]["CropX"]);
                                int iDP2ImagesCropY = Convert.ToInt32(dTblDP2Images.Rows[0]["CropY"]);
                                int iDP2ImagesCropWidth = Convert.ToInt32(dTblDP2Images.Rows[0]["CropWidth"]);
                                int iDP2ImagesCropLength = Convert.ToInt32(dTblDP2Images.Rows[0]["CropLength"]);
                                int iDP2ImagesBrt = Convert.ToInt32(dTblDP2Images.Rows[0]["Brt"]);
                                int iDP2ImagesRed = Convert.ToInt32(dTblDP2Images.Rows[0]["Red"]);
                                int iDP2ImagesGrn = Convert.ToInt32(dTblDP2Images.Rows[0]["Grn"]);
                                int iDP2ImagesBlu = Convert.ToInt32(dTblDP2Images.Rows[0]["Blu"]);
                                int iDP2ImagesCon = Convert.ToInt32(dTblDP2Images.Rows[0]["Con"]); //always write back to cds
                                int iDP2ImagesSaturation = Convert.ToInt32(dTblDP2Images.Rows[0]["Saturation"]); //always write back to cds
                                int iDP2ImagesSharpen = Convert.ToInt32(dTblDP2Images.Rows[0]["Sharpen"]); //always write back to cds
                                int iDP2ImagesGamma = Convert.ToInt32(dTblDP2Images.Rows[0]["Gamma"]); //always write back to cds
                                int iDP2ImagesRotation = Convert.ToInt32(dTblDP2Images.Rows[0]["RotateFromDisk"]); //always write back to cds
                                string sDP2Path = Convert.ToString(dTblDP2Images.Rows[0]["Path"]).Trim();
                                string sDP2PathFile = Path.GetFileName(sDP2Path).Trim();

                                bool bCDSImageMatchedDP2Image = false;

                                if (sFramesImage_id == sDP2PathFile)
                                {
                                    bCDSImageMatchedDP2Image = true;
                                }
                                else if (sFramesImage_id != sDP2PathFile)
                                {
                                    bCDSImageMatchedDP2Image = false;
                                }

                                if (iDP2ImagesCropX != 50 || iDP2ImagesCropY != 50 || iDP2ImagesCropLength != 100 || iDP2ImagesCropWidth != 100 || iDP2ImagesBrt != 0 || iDP2ImagesRed != 0 || iDP2ImagesGrn != 0 || iDP2ImagesBlu != 0)
                                {
                                    sUpdateCommand = "UPDATE Dp2image SET Width = " + iDP2ImagesWidth + ", Length = " + iDP2ImagesLength + ", Previewwid = " + iDP2ImagesPWidth +
                                        ", Previewlen = " + iDP2ImagesPLength +
                                        ", Cropx = " + iDP2ImagesCropX + ", Cropy = " + iDP2ImagesCropY + ", Cropwidth = " + iDP2ImagesCropWidth + ", Croplength = " + iDP2ImagesCropLength +
                                        ", Brt = " + iDP2ImagesBrt + ", Red = " + iDP2ImagesRed + ", Grn = " + iDP2ImagesGrn + ", Blu = " + iDP2ImagesBlu + ", Con = " + iDP2ImagesCon +
                                        ", Saturation = " + iDP2ImagesSaturation + ", Sharpen = " + iDP2ImagesSharpen + ", Gamma = " + iDP2ImagesGamma + ", Rotate = " + iDP2ImagesRotation +
                                        " WHERE Lookupnum = '" + sFramesLookupnum + "' AND Frame = " + sFramesSequence + " AND Sitting = '" + sFramesSitting + "' AND Path = '" + sDP2Path + "'";

                                    bValueChanged = true;
                                }
                                else
                                {
                                    sUpdateCommand = string.Empty;

                                    bValueChanged = false;
                                }

                                dTblImageData.Rows.Add(sRefNum, sFramesLookupnum, sDP2FrameNum, iDP2ImagesWidth, iDP2ImagesLength, iDP2ImagesPWidth, iDP2ImagesPLength, iDP2ImagesCropX,
                                +iDP2ImagesCropY, iDP2ImagesCropWidth, iDP2ImagesCropLength, iDP2ImagesBrt, iDP2ImagesRed, iDP2ImagesGrn, iDP2ImagesBlu, iDP2ImagesCon, iDP2ImagesSaturation,
                                +iDP2ImagesSharpen, iDP2ImagesGamma, iDP2ImagesRotation, sDP2Path, bFileExists, sFramesImage_id, sDP2PathFile, bValueChanged, bCDSImageMatchedDP2Image, sInsertCommand, sUpdateCommand);


                            }
                            else if (dTblDP2Images.Rows.Count == 0)
                            {
                                // If no records are returned from this query then simply push CDS.DP2Image record data into DP2.Images
                            }
                        }

                        // End of foreach loop.
                        string sStop = string.Empty;

                    }
                    else if (dSetAllTheGoodies.Tables["dTblFrames"].Rows.Count == 0)
                    {
                        //Note: this should never happen
                        bClearForm = true;
                    }
                }
                else if (dSetAllTheGoodies.Tables["dTblItems"].Rows.Count == 0 || bImageDataPrepTableSetup == false)
                {
                    string sStop = string.Empty;
                    bClearForm = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
                bClearForm = true;
            }
        }

        public void GatherImageDataFromCDS(string sInput, DataSet dSetAllTheGoodies, ref bool bClearForm)
        {
            try
            {
                bool bImagesTableBuilt = false;
                DataTable dTblImagesRecordPrep = new DataTable("dTblImagesRecordPrep");

                DSandDTGoodies02.SetUpImagesPrepTable(dTblImagesRecordPrep, ref bImagesTableBuilt);

                if (bImagesTableBuilt == true)
                {
                    dSetAllTheGoodies.Tables.Add(dTblImagesRecordPrep);
                    DataRow dRowImages = dSetAllTheGoodies.Tables["dTblImagesRecordPrep"].NewRow();

                    DataTable dTblDP2Image = new DataTable("dTblDP2Image");
                    string sCommText = "SELECT * FROM Dp2image WHERE Lookupnum = '" + sInput + "' ORDER BY Frame ASC";

                    DBConnGoodies02.CDSQuery(sCDSConnString, sCommText, dTblDP2Image);

                    if (dTblDP2Image.Rows.Count > 0)
                    {
                        dSetAllTheGoodies.Tables.Add(dTblDP2Image);

                        foreach (DataRow dRowDP2Image in dTblDP2Image.Rows)
                        {
                            string sProdNum = sInput;
                            string sRefNum = Convert.ToString(dRowDP2Image["Cust_ref"]).Trim();
                            string sCDSFrameNum = Convert.ToString(dRowDP2Image["Frame"]).Trim();
                            string sDP2FrameNum = sCDSFrameNum.PadLeft(4, '0').Trim();
                            string sSitting = Convert.ToString(dRowDP2Image["Sitting"]).Trim();
                            string sWidth = Convert.ToString(dRowDP2Image["Width"]).Trim();
                            string sLength = Convert.ToString(dRowDP2Image["Length"]).Trim();
                            string sPreviewWidth = Convert.ToString(dRowDP2Image["Previewwid"]).Trim();
                            string sPreviewLength = Convert.ToString(dRowDP2Image["Previewlen"]).Trim();
                            string sCropX = Convert.ToString(dRowDP2Image["Cropx"]).Trim();
                            string sCropY = Convert.ToString(dRowDP2Image["Cropy"]).Trim();
                            string sCropWidth = Convert.ToString(dRowDP2Image["Cropwidth"]).Trim();
                            string sCropLength = Convert.ToString(dRowDP2Image["Croplength"]).Trim();
                            string sBrt = Convert.ToString(dRowDP2Image["Brt"]).Trim();
                            string sRed = Convert.ToString(dRowDP2Image["Red"]).Trim();
                            string sGrn = Convert.ToString(dRowDP2Image["Grn"]).Trim();
                            string sBlu = Convert.ToString(dRowDP2Image["Blu"]).Trim();
                            string sCon = Convert.ToString(dRowDP2Image["Con"]).Trim();
                            string sSaturation = Convert.ToString(dRowDP2Image["Saturation"]).Trim();
                            string sSharpen = Convert.ToString(dRowDP2Image["Sharpen"]).Trim();
                            string sGamma = Convert.ToString(dRowDP2Image["Gamma"]).Trim();
                            string sRotation = Convert.ToString(dRowDP2Image["Rotate"]).Trim();
                            string sPath = Convert.ToString(dRowDP2Image["Path"]).Trim();
                            string sRejected = Convert.ToString(dRowDP2Image["Rejected"]).Trim();
                            string sCurrentYear = DateTime.Now.Year.ToString().Trim();
                            string sSubjectID = sProdNum + sDP2FrameNum + sCurrentYear;
                            string sFileType = Path.GetExtension(sPath);
                            string sAlternateID = string.Empty;

                            if (sFileType == ".JPG" || sFileType == ".JPEG")
                            {
                                sFileType = "JPEG";
                            }
                            else if (sFileType == ".PNG")
                            {
                                sFileType = "PNG";
                            }

                            DataTable dTblGetAlternateID = new DataTable("dTblGetAlternateID");
                            sCommText = "SELECT Shottype FROM Group WHERE Lookupnum = '" + sProdNum + "' AND Sequence = " + sCDSFrameNum + " AND Sitting = '" + sSitting + "'";

                            DBConnGoodies02.CDSQuery(sCDSConnString, sCommText, dTblGetAlternateID);

                            if (dTblGetAlternateID.Rows.Count > 0)
                            {
                                sAlternateID = Convert.ToString(dTblGetAlternateID.Rows[0]["Shottype"]).Trim();
                            }
                            else if (dTblGetAlternateID.Rows.Count == 0)
                            {
                                MessageBox.Show("Unable to gather Shottype value from the Group table.");
                            }

                            sCommText = "INSERT INTO [Images] (OrderID,Roll,Frame,SubjectID,Rejected,AlternateID,FileType,Width,Length,PreviewWidth,PreviewLength,CropX,CropY," +
                                "CropWidth,CropLength,Brt,Red,Grn,Blu,Con,Saturation,Sharpen,Gamma,RotateFromDisk,Path)" +
                                " VALUES ('" + sRefNum + "','" + sProdNum + "','" + sDP2FrameNum + "','" + sSubjectID + "','" + sRejected + "','" + sAlternateID + "','" + sFileType + "','" +
                            sWidth + "','" + sLength + "','" + sPreviewWidth + "','" + sPreviewLength + "','" + sCropX + "','" + sCropY + "','" + sCropWidth + "','" + sCropLength + "','" +
                            sBrt + "','" + sRed + "','" + sGrn + "','" + sBlu + "','" + sCon + "','" + sSaturation + "','" + sSharpen + "','" + sGamma + "','" + sRotation + "','" + sPath + "')";

                            bool bFileExists = false;

                            dRowImages["ProdNum"] = sProdNum;
                            dRowImages["RefNum"] = sRefNum;
                            dRowImages["CDSFrameNum"] = sCDSFrameNum;
                            dRowImages["DP2FrameNum"] = sDP2FrameNum;
                            dRowImages["Sitting"] = sSitting;
                            dRowImages["Width"] = sWidth;
                            dRowImages["Length"] = sLength;
                            dRowImages["PreviewWidth"] = sPreviewWidth;
                            dRowImages["PreviewLength"] = sPreviewLength;
                            dRowImages["CropX"] = sCropX;
                            dRowImages["CropY"] = sCropY;
                            dRowImages["CropWidth"] = sCropWidth;
                            dRowImages["CropLength"] = sCropLength;
                            dRowImages["Brt"] = sBrt;
                            dRowImages["Red"] = sRed;
                            dRowImages["Grn"] = sGrn;
                            dRowImages["Blu"] = sBlu;
                            dRowImages["Con"] = sCon;
                            dRowImages["Saturation"] = sSaturation;
                            dRowImages["Sharpen"] = sSharpen;
                            dRowImages["Gamma"] = sGamma;
                            dRowImages["Rotation"] = sRotation;
                            dRowImages["Path"] = sPath;
                            dRowImages["FileExists"] = bFileExists;
                            dRowImages["Rejected"] = sRejected;
                            dRowImages["CurrentYear"] = sCurrentYear;
                            dRowImages["SubjectID"] = sSubjectID;
                            dRowImages["FileType"] = sFileType;
                            dRowImages["AlternateID"] = sAlternateID;
                            dRowImages["CommText"] = sCommText;

                            dSetAllTheGoodies.Tables["dTblImagesRecordPrep"].Rows.Add(dRowImages.ItemArray);
                        }
                    }
                    else if (dTblDP2Image.Rows.Count == 0)
                    {
                        MessageBox.Show("Failed to gather Dp2image records.");
                        bClearForm = true;
                    }
                }
                else if (bImagesTableBuilt != true)
                {
                    MessageBox.Show("Images table failed to build properly.");
                    bClearForm = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
                bClearForm = true;
            }
        }

        public void GatherCodes(string sInput, DataSet dSetAllTheGoodies, ref bool bClearForm)
        {
            try
            {
                string sCodesProdNum = string.Empty;
                string sCodesFrameNum = string.Empty;
                string sCodesCode = string.Empty;
                string sCodesPackage = string.Empty;
                string sCodesQuantity = string.Empty;
                string sDP2ProductID = string.Empty;
                string sSequence = string.Empty;
                DataTable dTblCodesPerFrame = new DataTable("dTblCodesPerFrame");
                DataTable dTblFinishedSubjectInfo = new DataTable("dTblFinishedSubjectInfo");

                string sCustNum = Convert.ToString(dSetAllTheGoodies.Tables["dTblItems"].Rows[0]["Customer"]).Trim();

                DataTable dTblCodes = new DataTable("dTblCodes");
                //string sCommText = "SELECT * FROM Codes WHERE Lookupnum = '" + sInput + "' ORDER BY Sequence ASC";
                string sCommText = "SELECT * FROM Codes WHERE Lookupnum = '" + sInput + "' AND (Labeltyp = 'P' OR Labeltyp = 'S') ORDER BY Sequence ASC";
                //string sCommText = "SELECT * FROM Codes WHERE Lookupnum = '" + sInput + "' AND (Labeltyp = 'P') ORDER BY Sequence ASC";
                //string sCommText = "SELECT * FROM Codes WHERE Lookupnum = '" + sInput + "' AND (Labeltyp = 'S') ORDER BY Sequence ASC";

                DBConnGoodies02.CDSQuery(sCDSConnString, sCommText, dTblCodes);

                if (dTblCodes.Rows.Count > 0)
                {
                    dSetAllTheGoodies.Tables.Add(dTblCodes);

                    bool bCodesPrepTableSetup = false;
                    DataTable dTblCodesPrep = new DataTable("dTblCodesPrep");
                    dSetAllTheGoodies.Tables.Add(dTblCodesPrep);
                    DataRow dRowCodes = dSetAllTheGoodies.Tables["dTblCodesPrep"].NewRow();

                    DSandDTGoodies02.SetUpCodesPrepTable(dTblCodesPrep, ref bCodesPrepTableSetup);

                    if (bCodesPrepTableSetup == true)
                    {
                        foreach (DataRow dRowFrame in dSetAllTheGoodies.Tables["dTblFrames"].Rows)
                        {
                            dTblCodesPerFrame.Clear();

                            string sProdNum = Convert.ToString(dRowFrame["lookupnum"]).Trim();
                            sSequence = Convert.ToString(dRowFrame["sequence"]).Trim();
                            string sSitting = Convert.ToString(dRowFrame["sitting"]).Trim();

                            sCommText = "SELECT * FROM Codes WHERE Lookupnum = '" + sProdNum + "' AND Sequence = " + sSequence + " AND (Labeltyp = 'P' OR Labeltyp = 'S') ORDER BY Sequence ASC";

                            DBConnGoodies02.CDSQuery(sCDSConnString, sCommText, dTblCodesPerFrame);

                            if (dTblCodesPerFrame.Rows.Count > 0)
                            {
                                foreach (DataRow dRowCodesPerFrame in dTblCodesPerFrame.Rows)
                                {
                                    dRowCodes["ProdNum"] = sProdNum;
                                    dRowCodes["FrameNum"] = sSequence;
                                    dRowCodes["Code"] = Convert.ToString(dRowCodesPerFrame["Code"]).Trim();
                                    dRowCodes["Package"] = Convert.ToString(dRowCodesPerFrame["Package"]).Trim();
                                    dRowCodes["Quantity"] = Convert.ToString(dRowCodesPerFrame["Quantity"]).Trim();

                                    dSetAllTheGoodies.Tables["dTblCodesPrep"].Rows.Add(dRowCodes.ItemArray);
                                }
                            }
                            else if (dTblCodesPerFrame.Rows.Count == 0)
                            {
                                // Do nothing. It is possible for a frame not to have a code associated.
                            }
                        }

                        //End of foreach
                        sStop = string.Empty;

                        bool bFinishedSubjectInfoPrepTableBuilt = false;

                        DSandDTGoodies02.SetUpFinishedSubjectInfoPrepTable(dTblFinishedSubjectInfo, ref bFinishedSubjectInfoPrepTableBuilt);

                        if (bFinishedSubjectInfoPrepTableBuilt == true)
                        {
                            int iSeq = 0;

                            dSetAllTheGoodies.Tables.Add(dTblFinishedSubjectInfo);
                            DataRow dRowFinishedSubjectInfoNewRow = dSetAllTheGoodies.Tables["dTblFinishedSubjectInfo"].NewRow();

                            foreach (DataRow dRowSubjectInfoPrep in dSetAllTheGoodies.Tables["dTblSubjectInfoPrep"].Rows)
                            {
                                string sSIPProdNum = Convert.ToString(dRowSubjectInfoPrep["ProdNum"]).Trim();
                                string sSIPSequence = Convert.ToString(dRowSubjectInfoPrep["Sequence"]).Trim();
                                string sDP2FrameNum = sSIPSequence.PadLeft(4, '0');
                                string sSIPSitting = Convert.ToString(dRowSubjectInfoPrep["Sitting"]);
                                string sSIPGroupID = Convert.ToString(dRowSubjectInfoPrep["GroupID"]).Trim();
                                string sSIPShotType = Convert.ToString(dRowSubjectInfoPrep["ShotType"]).Trim();
                                string sSIPFirstName = Convert.ToString(dRowSubjectInfoPrep[@"First_Name"]).Trim();
                                string sSIPLastName = Convert.ToString(dRowSubjectInfoPrep[@"Last_Name"]).Trim();
                                string sSIPSchoolName = Convert.ToString(dRowSubjectInfoPrep[@"SchoolName"]).Trim();
                                string sSIPPrincipal = Convert.ToString(dRowSubjectInfoPrep[@"Principal"]).Trim();
                                string sSIPTeacher = Convert.ToString(dRowSubjectInfoPrep[@"Teacher"]).Trim();
                                string sSIPGrade = Convert.ToString(dRowSubjectInfoPrep["Grade"]).Trim();
                                string sSIPYear = Convert.ToString(dRowSubjectInfoPrep["Year"]).Trim();
                                string sSIPNameOn = Convert.ToString(dRowSubjectInfoPrep[@"NameOn"]).Trim();
                                string sSIPImagePath = Convert.ToString(dRowSubjectInfoPrep["ImagePath"]).Trim();
                                string sSIPTeamPicPath = Convert.ToString(dRowSubjectInfoPrep["TeamPicPath"]).Trim();
                                string sSIPLine1 = Convert.ToString(dRowSubjectInfoPrep[@"Line1"]);
                                string sSIPLine2 = Convert.ToString(dRowSubjectInfoPrep[@"Line2"]);
                                string sSIPLine3 = Convert.ToString(dRowSubjectInfoPrep[@"Line3"]);
                                string sSIPLine4 = Convert.ToString(dRowSubjectInfoPrep[@"Line4"]);
                                string sSIPLine5 = Convert.ToString(dRowSubjectInfoPrep[@"Line5"]);
                                string sSIPLine6 = Convert.ToString(dRowSubjectInfoPrep[@"Line6"]);
                                string sSIPLine7 = Convert.ToString(dRowSubjectInfoPrep[@"Line7"]);
                                string sSIPLine8 = Convert.ToString(dRowSubjectInfoPrep[@"Line8"]);
                                string sSIPMemoText = Convert.ToString(dRowSubjectInfoPrep[@"MemoText"]);

                                string sRefNum = Convert.ToString(dSetAllTheGoodies.Tables["dTblItems"].Rows[0]["Order"]).Trim();

                                string sSearchString = "ProdNum = '" + sSIPProdNum + "' AND FrameNum = '" + sSIPSequence + "'";
                                DataRow[] dRowCodesSearch = dSetAllTheGoodies.Tables["dTblCodesPrep"].Select(sSearchString);

                                if (dRowCodesSearch.Length > 0)
                                {
                                    foreach (DataRow dRowCodesSearchResults in dRowCodesSearch)
                                    {
                                        iSeq += 1;

                                        sCodesProdNum = Convert.ToString(dRowCodesSearchResults["ProdNum"]).Trim();
                                        sCodesFrameNum = Convert.ToString(dRowCodesSearchResults["FrameNum"]).Trim();
                                        sCodesCode = Convert.ToString(dRowCodesSearchResults["Code"]).Trim();
                                        sCodesPackage = Convert.ToString(dRowCodesSearchResults["Package"]).Trim();
                                        sCodesQuantity = Convert.ToString(dRowCodesSearchResults["Quantity"]).Trim();

                                        string sPackageTag = Convert.ToString(dSetAllTheGoodies.Tables["dTblItems"].Rows[0]["Packagetag"]).Trim();

                                        DataTable dTblLabels = new DataTable("dTblLabels");
                                        sCommText = "SELECT * FROM Labels WHERE Packagecod = '" + sCodesCode + "' AND Packagetag = '" + sPackageTag + "'";

                                        DBConnGoodies02.CDSQuery(sCDSConnString, sCommText, dTblLabels);

                                        if (dTblLabels.Rows.Count > 0)
                                        {
                                            string sCDSCode = Convert.ToString(dTblLabels.Rows[0]["Code"]).Trim();

                                            if (sCDSCode == "GP8")
                                            {
                                                if (sCustNum == "27312")
                                                {
                                                    sDP2ProductID = "8X10 NO OVERLAY Group";
                                                }
                                                else if (sCustNum == "71336")
                                                {
                                                    sDP2ProductID = "GP8 S51";
                                                }
                                            }
                                            else if (sCDSCode == "WCJ")
                                            {
                                                sDP2ProductID = "WCJ Group";
                                            }

                                            string sLabel1 = "PKG>" + sCodesCode + " QTY>" + sCodesQuantity;

                                            string sLabel2Packages = string.Empty;

                                            TM02.GetPackagesForFrame(sCodesProdNum, sDP2FrameNum, ref sLabel2Packages);

                                            string sLabel2 = "ALL PKGS" + sLabel2Packages;

                                            string sSubjectID = sSIPProdNum + sDP2FrameNum + DateTime.Now.Year.ToString().Trim();
                                            string sTracking = "PKG";

                                            string sCurrentYear = DateTime.Now.Year.ToString().Trim();

                                            sSIPFirstName = sSIPFirstName.Replace("'", "''");
                                            sSIPLastName = sSIPLastName.Replace("'", "''");
                                            sSIPSchoolName = sSIPSchoolName.Replace("'", "''");
                                            sSIPPrincipal = sSIPPrincipal.Replace("'", "''");
                                            sSIPTeacher = sSIPTeacher.Replace("'", "''");
                                            sSIPNameOn = sSIPNameOn.Replace("'", "''");
                                            sSIPLine1 = sSIPLine1.Replace("'", "''");
                                            sSIPLine2 = sSIPLine2.Replace("'", "''");
                                            sSIPLine3 = sSIPLine3.Replace("'", "''");
                                            sSIPLine4 = sSIPLine4.Replace("'", "''");
                                            sSIPLine5 = sSIPLine5.Replace("'", "''");
                                            sSIPLine6 = sSIPLine6.Replace("'", "''");
                                            sSIPLine7 = sSIPLine7.Replace("'", "''");
                                            sSIPLine8 = sSIPLine8.Replace("'", "''");
                                            sSIPMemoText = sSIPMemoText.Replace("'", "''");

                                            string sInsertCommText = "INSERT INTO [SubjectInfoSportsAIO] (OrderID,SubjectID,Seq,P1,Q1,TextBR,First_Name,Last_Name,School,Sitting,Year," +
                                                "TeamPicPath,Line_One,Line_Two,Line_Three,Line_Four,Line_Five,Line_Six,Line_Seven,Line_Eight,MemoText,Sortnum,Tracking,Label1,Label2,GroupID,Grade,Teacher)" +
                                                " VALUES ('" + sRefNum + "','" + sSubjectID + "','" + iSeq + "','" + sDP2ProductID + "','" + sCodesQuantity + "','" + sSIPNameOn + "','" + sSIPFirstName +
                                            "','" + sSIPLastName + "','" + sSIPSchoolName + "','" + sSIPSitting + "','" + sCurrentYear + "','" + sSIPTeamPicPath + "','" + sSIPLine1 +
                                            "','" + sSIPLine2 + "','" + sSIPLine3 + "','" + sSIPLine4 + "','" + sSIPLine5 + "','" + sSIPLine6 + "','" + sSIPLine7 + "','" + sSIPLine8 +
                                            "','" + sSIPMemoText + "','" + sSIPSequence + "','" + sTracking + "','" + sLabel1 + "','" + sLabel2 + "','" + sSIPGroupID + "','" + sSIPGrade + "','" + sSIPTeacher + "')";

                                            dTblFinishedSubjectInfo.Rows.Add(sRefNum, sSIPProdNum, sSIPSequence, sSIPSitting, sSIPGroupID, sSIPShotType, sSIPFirstName, sSIPLastName, sSIPSchoolName, sSIPPrincipal, sSIPTeacher, sSIPGrade, sCurrentYear, sSIPNameOn, sSIPImagePath, sSIPTeamPicPath, sSIPLine1, sSIPLine2, sSIPLine3, sSIPLine4, sSIPLine5, sSIPLine6, sSIPLine7, sSIPLine8, sSIPMemoText, sDP2ProductID, sCodesQuantity, sLabel1, sLabel2, sSIPSequence, sSubjectID, sTracking, sInsertCommText);

                                        }
                                        else if (dTblLabels.Rows.Count == 0)
                                        {

                                        }
                                    }

                                    // End of foreach.
                                    sStop = string.Empty;

                                }
                                else if (dRowCodesSearch.Length == 0)
                                {
                                    if (sSIPShotType != "G")
                                    {
                                        sSIPFirstName = sSIPFirstName.Replace("'", "''");
                                        sSIPLastName = sSIPLastName.Replace("'", "''");
                                        sSIPSchoolName = sSIPSchoolName.Replace("'", "''");
                                        sSIPPrincipal = sSIPPrincipal.Replace("'", "''");
                                        sSIPTeacher = sSIPTeacher.Replace("'", "''");
                                        sSIPNameOn = sSIPNameOn.Replace("'", "''");
                                        sSIPLine1 = sSIPLine1.Replace("'", "''");
                                        sSIPLine2 = sSIPLine2.Replace("'", "''");
                                        sSIPLine3 = sSIPLine3.Replace("'", "''");
                                        sSIPLine4 = sSIPLine4.Replace("'", "''");
                                        sSIPLine5 = sSIPLine5.Replace("'", "''");
                                        sSIPLine6 = sSIPLine6.Replace("'", "''");
                                        sSIPLine7 = sSIPLine7.Replace("'", "''");
                                        sSIPLine8 = sSIPLine8.Replace("'", "''");
                                        sSIPMemoText = sSIPMemoText.Replace("'", "''");


                                        sDP2ProductID = string.Empty;
                                        string sLabel1 = string.Empty;
                                        string sLabel2 = string.Empty;
                                        string sSubjectID = sSIPProdNum + sDP2FrameNum + DateTime.Now.Year.ToString().Trim();
                                        string sTracking = string.Empty;
                                        sCodesQuantity = string.Empty;
                                        iSeq += 1;
                                        string sCurrentYear = DateTime.Now.Year.ToString().Trim();

                                        string sInsertCommText = "INSERT INTO [SubjectInfoSportsAIO] (OrderID,SubjectID,Seq,P1,Q1,TextBR,First_Name,Last_Name,School,Sitting,Year," +
                                            "TeamPicPath,Line_One,Line_Two,Line_Three,Line_Four,Line_Five,Line_Six,Line_Seven,Line_Eight,MemoText,Sortnum,Tracking,Label1,Label2,GroupID,Grade,Teacher)" +
                                            " VALUES ('" + sRefNum + "','" + sSubjectID + "','" + iSeq + "','" + sDP2ProductID + "','" + sCodesQuantity + "','" + sSIPNameOn + "','" + sSIPFirstName +
                                        "','" + sSIPLastName + "','" + sSIPSchoolName + "','" + sSIPSitting + "','" + sCurrentYear + "','" + sSIPTeamPicPath + "','" + sSIPLine1 +
                                        "','" + sSIPLine2 + "','" + sSIPLine3 + "','" + sSIPLine4 + "','" + sSIPLine5 + "','" + sSIPLine6 + "','" + sSIPLine7 + "','" + sSIPLine8 +
                                        "','" + sSIPMemoText + "','" + sSIPSequence + "','" + sTracking + "','" + sLabel1 + "','" + sLabel2 + "','" + sSIPGroupID + "','" + sSIPGrade + "','" + sSIPTeacher + "')";

                                        dTblFinishedSubjectInfo.Rows.Add(sRefNum, sSIPProdNum, sSIPSequence, sSIPSitting, sSIPGroupID, sSIPShotType, sSIPFirstName, sSIPLastName, sSIPSchoolName, sSIPPrincipal, sSIPTeacher, sSIPGrade, sCurrentYear, sSIPNameOn, sSIPImagePath, sSIPTeamPicPath, sSIPLine1, sSIPLine2, sSIPLine3, sSIPLine4, sSIPLine5, sSIPLine6, sSIPLine7, sSIPLine8, sSIPMemoText, sDP2ProductID, sCodesQuantity, sLabel1, sLabel2, sSIPSequence, sSubjectID, sTracking, sInsertCommText);
                                    }
                                    else if (sSIPShotType == "G")
                                    {
                                        sSIPFirstName = sSIPFirstName.Replace("'", "''");
                                        sSIPLastName = sSIPLastName.Replace("'", "''");
                                        sSIPSchoolName = sSIPSchoolName.Replace("'", "''");
                                        sSIPPrincipal = sSIPPrincipal.Replace("'", "''");
                                        sSIPTeacher = sSIPTeacher.Replace("'", "''");
                                        sSIPNameOn = sSIPNameOn.Replace("'", "''");
                                        sSIPLine1 = sSIPLine1.Replace("'", "''");
                                        sSIPLine2 = sSIPLine2.Replace("'", "''");
                                        sSIPLine3 = sSIPLine3.Replace("'", "''");
                                        sSIPLine4 = sSIPLine4.Replace("'", "''");
                                        sSIPLine5 = sSIPLine5.Replace("'", "''");
                                        sSIPLine6 = sSIPLine6.Replace("'", "''");
                                        sSIPLine7 = sSIPLine7.Replace("'", "''");
                                        sSIPLine8 = sSIPLine8.Replace("'", "''");
                                        sSIPMemoText = sSIPMemoText.Replace("'", "''");

                                        sDP2ProductID = string.Empty;
                                        string sLabel1 = string.Empty;
                                        string sLabel2 = string.Empty;
                                        string sSubjectID = sSIPProdNum + sDP2FrameNum + DateTime.Now.Year.ToString().Trim();
                                        string sTracking = string.Empty;
                                        sCodesQuantity = string.Empty;
                                        iSeq += 1;
                                        string sCurrentYear = DateTime.Now.Year.ToString().Trim();

                                        string sInsertCommText = "INSERT INTO [SubjectInfoSportsAIO] (OrderID,SubjectID,Seq,P1,Q1,TextBR,First_Name,Last_Name,School,Sitting,Year," +
                                            "TeamPicPath,Line_One,Line_Two,Line_Three,Line_Four,Line_Five,Line_Six,Line_Seven,Line_Eight,MemoText,Sortnum,Tracking,Label1,Label2,GroupID,Grade,Teacher)" +
                                            " VALUES ('" + sRefNum + "','" + sSubjectID + "','" + iSeq + "','" + sDP2ProductID + "','" + sCodesQuantity + "','" + sSIPNameOn + "','" + sSIPFirstName +
                                        "','" + sSIPLastName + "','" + sSIPSchoolName + "','" + sSIPSitting + "','" + sCurrentYear + "','" + sSIPTeamPicPath + "','" + sSIPLine1 +
                                        "','" + sSIPLine2 + "','" + sSIPLine3 + "','" + sSIPLine4 + "','" + sSIPLine5 + "','" + sSIPLine6 + "','" + sSIPLine7 + "','" + sSIPLine8 +
                                        "','" + sSIPMemoText + "','" + sSIPSequence + "','" + sTracking + "','" + sLabel1 + "','" + sLabel2 + "','" + sSIPGroupID + "','" + sSIPGrade + "','" + sSIPTeacher + "')";

                                        dTblFinishedSubjectInfo.Rows.Add(sRefNum, sSIPProdNum, sSIPSequence, sSIPSitting, sSIPGroupID, sSIPShotType, sSIPFirstName, sSIPLastName, sSIPSchoolName, sSIPPrincipal, sSIPTeacher, sSIPGrade, sCurrentYear, sSIPNameOn, sSIPImagePath, sSIPTeamPicPath, sSIPLine1, sSIPLine2, sSIPLine3, sSIPLine4, sSIPLine5, sSIPLine6, sSIPLine7, sSIPLine8, sSIPMemoText, sDP2ProductID, sCodesQuantity, sLabel1, sLabel2, sSIPSequence, sSubjectID, sTracking, sInsertCommText);
                                    }
                                }
                            }

                            // End of foreach.
                            sStop = string.Empty;

                        }
                        else if (bFinishedSubjectInfoPrepTableBuilt != true)
                        {
                            MessageBox.Show("Failed to build SubjectInfoPrepTable.");
                            bClearForm = true;
                        }

                    }
                    else if (bCodesPrepTableSetup != true)
                    {
                        MessageBox.Show("Failed to build CodesPrepTable.");
                        bClearForm = true;
                    }

                }
                else if (dTblCodes.Rows.Count == 0)
                {
                    MessageBox.Show("Failed to gather records from the Codes table.");
                    bClearForm = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
                bClearForm = true;
            }
        }
    }
}
