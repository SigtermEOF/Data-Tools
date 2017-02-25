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
    class DataSetAndDatatableGoodies
    {
        protected string sStop = string.Empty;
        DBConnectionGoodies DBConnGoodies03 = new DBConnectionGoodies();
        protected string sCDSConnString = APS_Data_Tools.Properties.Settings.Default.CDSConnString.ToString();
        protected string sDP2ConnString = APS_Data_Tools.Properties.Settings.Default.DP2ConnString.ToString();

        public void SetUpSubjectInfoPrepTable(DataTable dTblSubjectInfoPrep, ref bool bSubjectInfoPrepTableBuilt)
        {
            try
            {
                dTblSubjectInfoPrep.Columns.Add("ProdNum", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("Sequence", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("Sitting", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("GroupID", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("ShotType", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("First_Name", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("Last_Name", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("SchoolName", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("Principal", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("Teacher", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("Grade", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("Year", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("NameOn", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("ImagePath", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("TeamPicPath", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("Line1", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("Line2", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("Line3", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("Line4", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("Line5", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("Line6", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("Line7", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("Line8", typeof(string));
                dTblSubjectInfoPrep.Columns.Add("MemoText", typeof(string));

                bSubjectInfoPrepTableBuilt = true;
            }
            catch (Exception ex)
            {
                bSubjectInfoPrepTableBuilt = false;

                MessageBox.Show(ex.ToString().Trim());
            }
        }

        public void SetUpFinishedSubjectInfoPrepTable(DataTable dTblFinishedSubjectInfo, ref bool bFinishedSubjectInfoPrepTableBuilt)
        {
            try
            {
                dTblFinishedSubjectInfo.Columns.Add("RefNum", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("ProdNum", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Sequence", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Sitting", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("GroupID", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("ShotType", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("First_Name", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Last_Name", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("SchoolName", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Principal", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Teacher", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Grade", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Year", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("NameOn", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("ImagePath", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("TeamPicPath", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Line1", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Line2", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Line3", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Line4", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Line5", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Line6", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Line7", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Line8", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("MemoText", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("P1", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Q1", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Label1", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Label2", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("SortNum", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("SubjectID", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("Tracking", typeof(string));
                dTblFinishedSubjectInfo.Columns.Add("CommText", typeof(string));

                bFinishedSubjectInfoPrepTableBuilt = true;
            }
            catch (Exception ex)
            {
                bFinishedSubjectInfoPrepTableBuilt = false;

                MessageBox.Show(ex.ToString().Trim());
            }
        }

        public void SetUpImagesPrepTable(DataTable dTblImages, ref bool bImagesTableBuilt)
        {
            try
            {
                dTblImages.Columns.Add("ProdNum", typeof(string));
                dTblImages.Columns.Add("RefNum", typeof(string));
                dTblImages.Columns.Add("CDSFrameNum", typeof(string));
                dTblImages.Columns.Add("DP2FrameNum", typeof(string));
                dTblImages.Columns.Add("Sitting", typeof(string));
                dTblImages.Columns.Add("Width", typeof(string));
                dTblImages.Columns.Add("Length", typeof(string));
                dTblImages.Columns.Add("PreviewWidth", typeof(string));
                dTblImages.Columns.Add("PreviewLength", typeof(string));
                dTblImages.Columns.Add("CropX", typeof(string));
                dTblImages.Columns.Add("CropY", typeof(string));
                dTblImages.Columns.Add("CropWidth", typeof(string));
                dTblImages.Columns.Add("CropLength", typeof(string));
                dTblImages.Columns.Add("Brt", typeof(string));
                dTblImages.Columns.Add("Red", typeof(string));
                dTblImages.Columns.Add("Grn", typeof(string));
                dTblImages.Columns.Add("Blu", typeof(string));
                dTblImages.Columns.Add("Con", typeof(string));
                dTblImages.Columns.Add("Saturation", typeof(string));
                dTblImages.Columns.Add("Sharpen", typeof(string));
                dTblImages.Columns.Add("Gamma", typeof(string));
                dTblImages.Columns.Add("Rotation", typeof(string));
                dTblImages.Columns.Add("Path", typeof(string));
                dTblImages.Columns.Add("FileExists", typeof(bool));
                dTblImages.Columns.Add("Rejected", typeof(string));
                dTblImages.Columns.Add("CurrentYear", typeof(string));
                dTblImages.Columns.Add("SubjectID", typeof(string));
                dTblImages.Columns.Add("FileType", typeof(string));
                dTblImages.Columns.Add("AlternateID", typeof(string));
                dTblImages.Columns.Add("CommText", typeof(string));

                bImagesTableBuilt = true;
            }
            catch (Exception ex)
            {
                bImagesTableBuilt = false;

                MessageBox.Show(ex.ToString().Trim());
            }
        }

        public void SetUpCodesPrepTable(DataTable dTblCodesPrep, ref bool bCodesPrepTableSetUp)
        {
            try
            {
                dTblCodesPrep.Columns.Add("ProdNum", typeof(string));
                dTblCodesPrep.Columns.Add("FrameNum", typeof(string));
                dTblCodesPrep.Columns.Add("Code", typeof(string));
                dTblCodesPrep.Columns.Add("Package", typeof(string));
                dTblCodesPrep.Columns.Add("Quantity", typeof(string));

                bCodesPrepTableSetUp = true;

            }
            catch (Exception ex)
            {
                bCodesPrepTableSetUp = false;

                MessageBox.Show(ex.ToString().Trim());
            }
        }

        public void SetUpOrdersPrepTable(DataTable dTblOrdersPrepTable, ref bool bOrdersPrepTableSetUp)
        {
            try
            {
                dTblOrdersPrepTable.Columns.Add("RefNum", typeof(string));
                dTblOrdersPrepTable.Columns.Add("CustNum", typeof(string));
                dTblOrdersPrepTable.Columns.Add("SubjectInfoType", typeof(string));
                dTblOrdersPrepTable.Columns.Add("Logo", typeof(string));
                dTblOrdersPrepTable.Columns.Add("SchoolName", typeof(string));
                dTblOrdersPrepTable.Columns.Add("Principal", typeof(string));
                dTblOrdersPrepTable.Columns.Add("CurrentYear", typeof(string));
                dTblOrdersPrepTable.Columns.Add("InsertCommText", typeof(string));
                dTblOrdersPrepTable.Columns.Add("UpdateCommText", typeof(string));

                bOrdersPrepTableSetUp = true;

            }
            catch (Exception ex)
            {
                bOrdersPrepTableSetUp = false;

                MessageBox.Show(ex.ToString().Trim());
            }
        }

        public void SetUpImageDataPrepTable(DataTable dTblImageData, ref bool bImageDataPrepTableSetup)
        {
            try
            {
                dTblImageData.Columns.Add("RefNum", typeof(string));
                dTblImageData.Columns.Add("ProdNum", typeof(string));
                dTblImageData.Columns.Add("FrameNum", typeof(string));
                dTblImageData.Columns.Add("Width", typeof(int));
                dTblImageData.Columns.Add("Length", typeof(int));
                dTblImageData.Columns.Add("PreviewWidth", typeof(int));
                dTblImageData.Columns.Add("PreviewLength", typeof(int));
                dTblImageData.Columns.Add("CropX", typeof(int));
                dTblImageData.Columns.Add("CropY", typeof(int));
                dTblImageData.Columns.Add("CropWidth", typeof(int));
                dTblImageData.Columns.Add("CropLength", typeof(int));
                dTblImageData.Columns.Add("Brt", typeof(int));
                dTblImageData.Columns.Add("Red", typeof(int));
                dTblImageData.Columns.Add("Grn", typeof(int));
                dTblImageData.Columns.Add("Blu", typeof(int));
                dTblImageData.Columns.Add("Con", typeof(int));
                dTblImageData.Columns.Add("Saturation", typeof(int));
                dTblImageData.Columns.Add("Sharpen", typeof(int));
                dTblImageData.Columns.Add("Gamma", typeof(int));
                dTblImageData.Columns.Add("RotateFromDisk", typeof(int));
                dTblImageData.Columns.Add("DP2Path", typeof(string));
                dTblImageData.Columns.Add("FileExists", typeof(bool));
                dTblImageData.Columns.Add("CDSImageName", typeof(string));
                dTblImageData.Columns.Add("DP2ImageName", typeof(string));
                dTblImageData.Columns.Add("DP2CropOrColorEdited", typeof(bool));
                dTblImageData.Columns.Add("MatchVerified", typeof(bool));
                dTblImageData.Columns.Add("InsertCommText", typeof(string));
                dTblImageData.Columns.Add("UpdateCommText", typeof(string));

                bImageDataPrepTableSetup = true;
            }
            catch(Exception ex)
            {
                bImageDataPrepTableSetup = false;

                MessageBox.Show(ex.ToString().Trim());
            }
        }

        public void InsertRecordsIntoTables(DataSet dSetAllTheGoodies, ref bool bClearForm)
        {
            try
            {
                //Check if order data exists. If so then update records to avoid losing keyed data.

                bool bOrderExists = false;

                if (dSetAllTheGoodies.Tables["dTblItems"].Rows.Count > 0)
                {
                    string sRefNum = Convert.ToString(dSetAllTheGoodies.Tables["dTblItems"].Rows[0]["Order"]).Trim();

                    DataTable dTblCheckIfOrderDataExists = new DataTable("dTblCheckIfOrderDataExists");
                    string sCommText = "SELECT * FROM [Orders] WHERE [ID] = '" + sRefNum + "'";

                    DBConnGoodies03.SQLQuery(sDP2ConnString, sCommText, dTblCheckIfOrderDataExists);

                    if (dTblCheckIfOrderDataExists.Rows.Count > 0)
                    {
                        bOrderExists = true;
                    }
                    else if (dTblCheckIfOrderDataExists.Rows.Count == 0)
                    {
                        bOrderExists = false;
                    }

                    foreach (DataRow dRowOrdersPrepTable in dSetAllTheGoodies.Tables["dTblOrdersPrepTable"].Rows)
                    {
                        if (bOrderExists == true)
                        {
                            sCommText = Convert.ToString(dRowOrdersPrepTable["UpdateCommText"]);
                        }
                        else if (bOrderExists != true)
                        {
                            sCommText = Convert.ToString(dRowOrdersPrepTable["InsertCommText"]);
                        }

                        bool bOrderRecordInserted = false;

                        DBConnGoodies03.SQLNonQuery(sDP2ConnString, sCommText, ref bOrderRecordInserted);

                        if (bOrderRecordInserted == true)
                        {

                        }
                        else if (bOrderRecordInserted != true)
                        {
                            MessageBox.Show("Failed to insert or update Orders data.");
                            bClearForm = true;
                        }
                    }
                }
                else if (dSetAllTheGoodies.Tables["dTblItems"].Rows.Count == 0)
                {
                    // This should never happen.
                }

                //Insert Image data into DP2.Images table. 
                //Verify image path exists here.

                bool bAllFilesExist = true;
                this.CheckImageFileExistence(dSetAllTheGoodies, ref bAllFilesExist);

                if (bAllFilesExist == true)
                {
                    foreach (DataRow dRowImagesRecordPrepTable in dSetAllTheGoodies.Tables["dTblImagesRecordPrep"].Rows)
                    {
                        string sCommText = Convert.ToString(dRowImagesRecordPrepTable["CommText"]);

                        bool bImageRecordInserted = false;

                        DBConnGoodies03.SQLNonQuery(sDP2ConnString, sCommText, ref bImageRecordInserted);

                        if (bImageRecordInserted == true)
                        {

                        }
                        else if (bImageRecordInserted != true)
                        {
                            MessageBox.Show("Failed to insert Images data.");
                            bClearForm = true;
                            break;
                        }
                    }
                }
                else if (bAllFilesExist != true)
                {
                    MessageBox.Show("File existence check failed.");
                    bClearForm = true;
                }

                // Insert SubjectInfo data into required SubjectInfo table.

                foreach (DataRow dRowFinishedSubjectInfoPrepTable in dSetAllTheGoodies.Tables["dTblFinishedSubjectInfo"].Rows)
                {
                    string sCommText = Convert.ToString(dRowFinishedSubjectInfoPrepTable["CommText"]);

                    bool bSubjectInfoInserted = false;

                    DBConnGoodies03.SQLNonQuery(sDP2ConnString, sCommText, ref bSubjectInfoInserted);

                    if (bSubjectInfoInserted == true)
                    {
                        sCommText = sCommText.Trim();
                    }
                    else if (bSubjectInfoInserted != true)
                    {
                        MessageBox.Show("Failed to insert SubjectInfo data.");
                        bClearForm = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
                bClearForm = true;
            }
        }

        public void ClearDP2Records(DataSet dSetAllTheGoodies, ref bool bClearForm)
        {
            try
            {
                string sRefNum = string.Empty;

                // Delete Images data.                

                if (dSetAllTheGoodies.Tables["dTblItems"].Rows.Count > 0)
                {
                    sRefNum = Convert.ToString(dSetAllTheGoodies.Tables["dTblItems"].Rows[0]["Order"]).Trim();

                    DataTable dTblCheckForImageData = new DataTable("dTblCheckForImageData");
                    string sCommText01 = "SELECT * FROM [Images] WHERE [OrderID] = '" + sRefNum + "'";

                    DBConnGoodies03.SQLQuery(sDP2ConnString, sCommText01, dTblCheckForImageData);

                    if (dTblCheckForImageData.Rows.Count > 0)
                    {
                        bool bImageDataDeletionSuccess = false;
                        sCommText01 = "DELETE FROM [Images] WHERE OrderID = '" + sRefNum + "'";

                        DBConnGoodies03.SQLNonQuery(sDP2ConnString, sCommText01, ref bImageDataDeletionSuccess);

                        if (bImageDataDeletionSuccess == true)
                        {

                        }
                        else if (bImageDataDeletionSuccess != true)
                        {
                            MessageBox.Show("Failed to delete Images data.");
                            bClearForm = true;
                        }
                    }
                    else if (dTblCheckForImageData.Rows.Count == 0)
                    {

                    }
                }
                else if (dSetAllTheGoodies.Tables["dTblItems"].Rows.Count == 0)
                {
                    //Note: this should never happen
                }

                // Delete SubjectInfo data.

                if (dSetAllTheGoodies.Tables["dTblOrdersPrepTable"].Rows.Count > 0)
                {
                    string sSubjectInfoType = Convert.ToString(dSetAllTheGoodies.Tables["dTblOrdersPrepTable"].Rows[0]["SubjectInfoType"]).Trim();

                    DataTable dTblCheckForSubjectInfoData = new DataTable("dTblCheckForSubjectInfoData");
                    string sCommText01 = "SELECT * FROM [SubjectInfo" + sSubjectInfoType + "] WHERE [OrderID] = '" + sRefNum + "'";

                    DBConnGoodies03.SQLQuery(sDP2ConnString, sCommText01, dTblCheckForSubjectInfoData);

                    if (dTblCheckForSubjectInfoData.Rows.Count > 0)
                    {
                        bool bSubjectInfoDateDeletionSuccess = false;
                        sCommText01 = "DELETE FROM [SubjectInfo" + sSubjectInfoType + "] WHERE [OrderID] = '" + sRefNum + "'";

                        DBConnGoodies03.SQLNonQuery(sDP2ConnString, sCommText01, ref bSubjectInfoDateDeletionSuccess);

                        if (bSubjectInfoDateDeletionSuccess == true)
                        {

                        }
                        else if (bSubjectInfoDateDeletionSuccess != true)
                        {
                            MessageBox.Show("Failed to delete SubjectInfo data.");
                            bClearForm = true;
                        }
                    }
                    else if (dTblCheckForSubjectInfoData.Rows.Count == 0)
                    {

                    }
                }
                else if (dSetAllTheGoodies.Tables["dTblOrdersPrepTable"].Rows.Count == 0)
                {
                    //Note: this should never happen
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
            }
        }

        public void CheckImageFileExistence(DataSet dSetAllTheGoodies, ref bool bAllFilesExist) // Set bool to true leading into method.
        {
            try
            {
                if (dSetAllTheGoodies.Tables["dTblImagesRecordPrep"].Rows.Count > 0)
                {
                    foreach (DataRow dRowImagesRecordPrep in dSetAllTheGoodies.Tables["dTblImagesRecordPrep"].Rows)
                    {
                        string sPath = Convert.ToString(dRowImagesRecordPrep["Path"]).Trim();

                        if (File.Exists(sPath))
                        {
                            dRowImagesRecordPrep["FileExists"] = true;
                        }
                        else if (!File.Exists(sPath))
                        {
                            bAllFilesExist = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
            }
        }

        public void UpdateCDSDp2imageWithDP2ImagesEditedInfo(DataSet dSetAllTheGoodies, ref bool bClearForm)
        {
            try
            {
                if (dSetAllTheGoodies.Tables["dTblImageData"].Rows.Count > 0)
                {
                    foreach(DataRow dRowImageData in dSetAllTheGoodies.Tables["dTblImageData"].Rows)
                    {
                        bool bValueChanged = Convert.ToBoolean(dRowImageData["DP2CropOrColorEdited"]);

                        if (bValueChanged == true)
                        {
                            bool bCDSDp2imageUpdated = false;
                            string sCommText = Convert.ToString(dRowImageData["UpdateCommText"]);

                            DBConnGoodies03.CDSNonQuery(sCDSConnString, sCommText, ref bCDSDp2imageUpdated);

                            if (bCDSDp2imageUpdated == true)
                            {

                            }
                            else if (bCDSDp2imageUpdated != true)
                            {
                                string sStop = string.Empty;
                            }
                        }
                        else if (bValueChanged != true)
                        {
                            // Do nothing.
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
                bClearForm = true;
            }
        }
    }
}
