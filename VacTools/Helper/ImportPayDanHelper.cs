using aiet.Tools;
using Aiet_DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Web.UI.WebControls;

namespace VacWebSiteTools.Helper
{
    public class ImportPayDanHelper
    {
        protected aNpoi npoi = new aNpoi();
        protected myDB db = new myDB();
        protected tools tool = new tools();
        protected SqlTransHelper trans = new SqlTransHelper();
        protected clsLessonR lessonR = new clsLessonR();
        protected clsLessonRKind lessonRKind = new clsLessonRKind();
        protected List<NewLessonRKind> newLRKind;
        protected List<NewLessonR> newLR;

        public class ImportField
        {
            public string PayDanID { set; get; }
            public string Period { set; get; }
            public string PayDanName { set; get; }
            public string StartDate { set; get; }
            public string EndDate { set; get; }
        }

        public ImportPayDanHelper()
        {
        }

        public void ImportPayDan(FileUpload fileUpload, ImportField field)
        {
            ImportPayDanHandler(fileUpload, field, true);
        }

        public void ImportPayDanOnlyDetail(FileUpload fileUpload, ImportField field)
        {
            ImportPayDanHandler(fileUpload, field, false);
        }

        protected virtual void ImportPayDanHandler(FileUpload fileUpload, ImportField field, bool importMaster)
        {
            try
            {
                var isBreak = false;
                var dt = npoi.getNPOIToDataTable(fileUpload, "", 1);

                ReNew_NewLessonRKindClass();

                if (importMaster)
                    trans.AddTransCommand(CreatePayDanCommand(field));

                npoi.Import(dt, InitImportFormat(), (lst, dr, i) =>
                {
                    if (!VerifyLessonPro(dr))
                    {
                        isBreak = true;
                        return false;
                    }

                    var teachersid = new List<string>();
                    if (!VerifyTeacher(dr, teachersid))
                    {
                        isBreak = true;
                        return false;
                    }

                    if (!VerifyLessonRName(dr, i))
                    {
                        isBreak = true;
                        return false;
                    }

                    var lessonRKindID = CreatLessonRKindCommand(dr);
                    var lessonRID = CreateLessonRCommand(dr, lessonRKindID);
                    CreatePayDanDetailCommand(field.PayDanID, lessonRID, dr);
                    CreatPayDanLecturerCommand(field.PayDanID, teachersid);

                    return true;
                });

                if (!isBreak)
                {
                    db.ExecuteTranscation(trans);
                    tool.ShowMessage("上傳成功");
                }


            }
            catch (Exception ex)
            {
                tool.ShowExceptionMessage(ex.Message);
                new logError(new object[] { 900, ex, "[ImportPayDanHandler]" });
            }
        }

        /// <summary>
        /// 驗證課程屬性及修課別限制值
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private bool VerifyLessonPro(DataRow dr)
        {
            var listLessonPro = new List<string> { "1", "2" };
            var listLessonPType = new List<string> { "1", "2", "3" };

            if (!listLessonPro.Contains(dr["課程屬性"].ToString().Trim()))
            {
                tool.ShowMessage(string.Format("課程屬性只允許{0}", string.Join(",", listLessonPro)));
                return false;
            }

            if (!listLessonPro.Contains(dr["修課別"].ToString().Trim()))
            {
                tool.ShowMessage(string.Format("修課別只允許{0}", string.Join(",", listLessonPType)));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 重建NewLessonRKind實體 , 預防使用其他的Import方法時資料重複
        /// </summary>
        protected virtual void ReNew_NewLessonRKindClass()
        {
            newLRKind = new List<NewLessonRKind>();
            newLR = new List<NewLessonR>();
        }

        protected virtual SqlTransHelper.SqlTransCommand CreatePayDanCommand(ImportField field)
        {
            //新增PayDan Sql
            return trans.CreateCommandItem(@"
Insert into PayDan (PayDanID,Period,PayDanName,CrtTime,CrtUser,Verify,StartDate,EndDate)
Values(@PayDanID,@Period,@PayDanName,now(),@CrtUser,1,@StartDate,@EndDate)"
                , new SqlTransParameter("PayDanID", field.PayDanID)
                , new SqlTransParameter("Period", field.Period)
                , new SqlTransParameter("PayDanName", field.PayDanName)
                , new SqlTransParameter("CrtUser", "admin")
                , new SqlTransParameter("StartDate", DateHelper.DateTimeCovert(field.StartDate, true))
                , new SqlTransParameter("EndDate", DateHelper.DateTimeCovert(field.EndDate, true)));
        }

        protected virtual List<ImportFormat> InitImportFormat()
        {
            return new List<ImportFormat>
            {
                new ImportFormat("", "課程屬性", ""),
                new ImportFormat("", "修課別", ""),
                new ImportFormat("", "課目分類", ""),
                new ImportFormat("", "課目名稱", ""),
                new ImportFormat("", "講授時數", ""),
                new ImportFormat("", "教師姓名/教師編號", ""),
                new ImportFormat("", "備註", "")
            };
        }

        /// <summary>
        /// 驗證老師欄位
        /// </summary>
        protected virtual bool VerifyTeacher(DataRow dr, List<string> teachersid)
        {
            var teacherData = dr["教師姓名/教師編號"].ToString().Trim();
            var teacher = new clsEducationLecturer();
            DataView dv;
            if (!string.IsNullOrEmpty(teacherData))
            {
                var teachers = VacTools.GetTeachers(teacherData);

                foreach (var teach in teachers)
                {
                    //驗證老師資料
                    dv = teacher.GetLecturerFromNameOrNo(teach.Trim());
                    if (dv.Count == 0)
                    {
                        tool.ShowMessage(string.Format("找不到教師姓名或編號({0}) ,教師必須從講師主檔新增 , 匯入已終止", teach.Trim()));
                        return false;
                    }

                    if (dv.Count > 1)
                    {
                        tool.ShowMessage(string.Format("教師姓名或編號({0})找到兩筆以上資料 , 同名教師必須指定教師編號 匯入已終止", teach.Trim()));
                        return false;
                    }

                    if (dv[0]["yearsold"] != DBNull.Value 
                        && Convert.ToInt16( dv[0]["yearsold"]) > clsParameters.GetParameterForAgeLimit())
                    {
                        tool.ShowMessage(string.Format("教師姓名或編號({0})超過年齡限制, 匯入已終止", teach.Trim()));
                        return false;
                    }


                    teachersid.Add(dv[0]["LecturerNO"].ToString());
                }
            }

            return true;
        }

        /// <summary>
        ///驗證課目名稱欄位
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected virtual bool VerifyLessonRName(DataRow dr, int row)
        {
            if (string.IsNullOrEmpty(dr["課目名稱"].ToString().Trim()) ||
                string.IsNullOrEmpty(dr["課目分類"].ToString().Trim()))
            {
                tool.ShowMessage(string.Format("第{0}筆課程名稱或分類有缺  , 匯入已終止", row));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 比對課目分類及是否要新增資料到DB
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected virtual string CreatLessonRKindCommand(DataRow dr)
        {
            var lessonRKindName = dr["課目分類"].ToString().Trim();
            var lessonRKindID = string.Empty;

            //比對名稱新增課程分類
            //先找集合
            var lstRKind = newLRKind.FirstOrDefault(p => p.LessonRKindName == lessonRKindName);
            if (lstRKind != null)
                lessonRKindID = lstRKind.LessonRKindID;
            else
            {
                //沒有再找資料庫
                var dvLessonRKind = lessonRKind.CheckLessonRKindIsExist(lessonRKindName);
                if (dvLessonRKind.Count == 0)
                {
                    lessonRKindID = lessonRKind.getSNo;
                    trans.CreateCommand(@"
Insert into LessonRKind (LanNo,LessonRKindID,LessonRKindName,CrtTime,CrtUser)
Values(@LanNo,@LessonRKindID,@LessonRKindName,now(),@CrtUser)"
                        , new SqlTransParameter("LanNo", tool.currentLanguage)
                        , new SqlTransParameter("LessonRKindID", lessonRKindID)
                        , new SqlTransParameter("LessonRKindName", lessonRKindName)
                        , new SqlTransParameter("CrtUser", "admin"));
                }
                else
                {
                    lessonRKindID = dvLessonRKind[0]["LessonRKindID"].ToString().Trim();
                }

                //資料庫找到的加入集合
                newLRKind.Add(new NewLessonRKind { LessonRKindID = lessonRKindID, LessonRKindName = lessonRKindName });
            }

            return lessonRKindID;
        }

        /// <summary>
        /// 比對名稱新增課程
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="lessonRKindID"></param>
        /// <returns></returns>
        protected virtual string CreateLessonRCommand(DataRow dr, string lessonRKindID)
        {
            var lessonRName = dr["課目名稱"].ToString().Trim();
            var lessonPro = dr["課程屬性"].ToString().Trim();
            var lessonRID = string.Empty;
            var lstR = newLR.FirstOrDefault(p => p.LessonRName == lessonRName);
            if (lstR != null)
                lessonRID = lstR.LessonRID;
            else
            {
                var dvLessonR = lessonR.CheckLessonRIsExist(lessonRName);
                if (dvLessonR.Count == 0)
                {
                    //一般課目有是不是支付鐘點費的問題 , 這邊不自動新增
                    if (lessonPro == "2")
                        throw new Exception(string.Format("找不到一般輔教課目名稱({0}),請確認課目是否已設定", lessonRName));

                    lessonRID = lessonR.getSNo;
                    trans.CreateCommand(@"
Insert into  LessonR (LanNo,LessonRID,LessonRKindID,LessonRName,LessonRType,LessonPro,CrtTime,CrtUser)
Values(@LanNo,@LessonRID,@LessonRKindID,@LessonRName,@LessonRType,@LessonPro,now(),@CrtUser)"
                        , new SqlTransParameter("LanNo", tool.currentLanguage)
                        , new SqlTransParameter("LessonRID", lessonRID)
                        , new SqlTransParameter("LessonRKindID", lessonRKindID)
                        , new SqlTransParameter("LessonRName", lessonRName)
                        , new SqlTransParameter("LessonRType", dr["修課別"].ToString().Trim())
                        , new SqlTransParameter("LessonPro", dr["課程屬性"].ToString().Trim())
                        , new SqlTransParameter("CrtUser", "admin"));
                }
                else
                {
                    lessonRID = dvLessonR[0]["lessonRID"].ToString();
                }

                //資料庫找到的加入集合
                newLR.Add(new NewLessonR { LessonRID = lessonRID, LessonRName = lessonRName });
            }

            return lessonRID;
        }

        /// <summary>
        /// 新增PayDanDetail
        /// </summary>
        /// <param name="field"></param>
        /// <param name="lessonRID"></param>
        /// <param name="dr"></param>
        protected virtual void CreatePayDanDetailCommand(string paydanID, string lessonRID, DataRow dr)
        {
            trans.CreateCommand(@"
Insert into PayDanDetail (PayDanID,LessonRID,TeachHour,PracticHour,BeforeHour,AfterHour,LecturerNO,Memo,CrtTime,CrtUser,LanNo,LessonRName)
Values(@PayDanID,@LessonRID,@TeachHour,@PracticHour,@BeforeHour,@AfterHour,@LecturerNO,@Memo,now(),@CrtUser,'zh-tw',@LessonRName)"
                , new SqlTransParameter("PayDanID", paydanID)
                , new SqlTransParameter("LessonRID", lessonRID)
                , new SqlTransParameter("TeachHour", dr["講授時數"].ToString().Trim())
                , new SqlTransParameter("PracticHour", "0")
                , new SqlTransParameter("BeforeHour", "0")
                , new SqlTransParameter("AfterHour", "0")
                , new SqlTransParameter("LecturerNO", "")
                , new SqlTransParameter("Memo", dr["備註"].ToString().Trim())
                , new SqlTransParameter("CrtUser", "admin")
                , new SqlTransParameter("LessonRName", dr["課目名稱"].ToString().Trim()));
        }

        /// <summary>
        /// 加入教師到 paydan_lecturer
        /// </summary>
        /// <param name="field"></param>
        /// <param name="teachersid"></param>
        protected virtual void CreatPayDanLecturerCommand(string paydanID, List<string> teachersid)
        {
            foreach (var teachID in teachersid)
            {
                trans.CreateCommand(@"
Insert into paydan_lecturer (paydandetail_aid,lecturerNo,PayDanID)
Values((SELECT max(aid) from paydandetail) ,@lecturerNo,@PayDanID)"
                    , new SqlTransParameter("lecturerNo", teachID)
                    , new SqlTransParameter("PayDanID", paydanID));
            }
        }
    }
}