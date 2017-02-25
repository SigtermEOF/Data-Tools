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

namespace Studio5Groups
{
    class TaskMethods
    {
        DBConnectionGoodies DBConnGoodies04 = new DBConnectionGoodies();
        DataSetAndDatatableGoodies DSandDTGoodies04 = new DataSetAndDatatableGoodies();
        protected string sCDSConnString = Studio5Groups.Properties.Settings.Default.CDSConnString.ToString();
        protected string sDP2ConnString = Studio5Groups.Properties.Settings.Default.DP2ConnString.ToString();

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

        public void PullDP2ImagesEditInfoIntoCDSDP2Image(DataSet dSetAllTheGoodies)
        {
            try
            {
                bool bValueChanged = false;

                bool bImageDataPrepTableSetup = false;
                DataTable dTblImageData = new DataTable("dTblImageData");
                dSetAllTheGoodies.Tables.Add("dTblImageData");

                DSandDTGoodies04.SetUpImageDataPrepTable(dTblImageData, ref bImageDataPrepTableSetup);

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

                            DBConnGoodies04.SQLQuery(sDP2ConnString, sCommText, dTblDP2Images);

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

                                //add a bool to indicate matching frames to path
                                //verify every frame matched up

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
                                    // i need to delete CDS.DP2Image data then insert records from DP2.Images into CDS.DP2Image (or update??????)
                                    // dump CDS.DP2Image data
                                    // insert record into CDS.DP2Image

                                    bValueChanged = true;
                                }
                                else
                                {
                                    // do not need here because we are always going to insert data into dp2.images after dumping initially
                                    // keep records in DP2.Images as is (no color correction within DP2 has been done)
                                    // at some at some point will dump dp2.images

                                    bValueChanged = false;
                                }

                                string sInsertCommand = string.Empty;
                                string sUpdateCommand = string.Empty;

                                dTblImageData.Rows.Add(sRefNum, sFramesLookupnum, sDP2FrameNum, iDP2ImagesWidth, iDP2ImagesLength, iDP2ImagesPWidth, iDP2ImagesPLength, iDP2ImagesCropX,
                                +iDP2ImagesCropY, iDP2ImagesCropWidth, iDP2ImagesCropLength, iDP2ImagesBrt, iDP2ImagesRed, iDP2ImagesGrn, iDP2ImagesBlu, iDP2ImagesCon, iDP2ImagesSaturation,
                                +iDP2ImagesSharpen, iDP2ImagesGamma, iDP2ImagesRotation, sDP2Path, sFramesImage_id, sDP2PathFile, bValueChanged, bCDSImageMatchedDP2Image, sInsertCommand, sUpdateCommand);


                            }
                            else if (dTblDP2Images.Rows.Count == 0)
                            {
                                // If no records are returned from this query then simply push CDS.DP2Image record data into DP2.Images
                                // always going to insert data after dumping dp2.images
                            }
                        }

                        // End of foreach loop.
                        string sStop = string.Empty;

                    }
                    else if (dSetAllTheGoodies.Tables["dTblFrames"].Rows.Count == 0)
                    {
                        //Note: this should never happen
                    }
                }
                else if (dSetAllTheGoodies.Tables["dTblItems"].Rows.Count == 0 || bImageDataPrepTableSetup == false)
                {
                    string sStop = string.Empty;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString().Trim());
            }
        }
    }
}
