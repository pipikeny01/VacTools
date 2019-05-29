using aiet.Base;
using aiet.Tools;
using Aiet_DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using VacWebSiteTools.Helper;

namespace VacWebSiteTools.Service
{
    public class LessonService : BaseService
    {
        private clsLesson _db;
        private clsLessonSetting _cLessonSetting;
        private clsLessonRnd _clsLessonRnd;

        public LessonService()
        {
            _db = new clsLesson();
            _cLessonSetting = new clsLessonSetting();
            _clsLessonRnd = new clsLessonRnd();
        }

        public LessonService(clsLesson db, clsLessonSetting cLessonSetting, clsLessonRnd clsLessonRnd)
        {
            _db = db;
            _cLessonSetting = cLessonSetting;
            _clsLessonRnd = clsLessonRnd;
        }

        public string LessonCopy(DataView dv, string copyLessonID, string txtCopyDate0)
        {
            var trans = new SqlTransHelper();
            var eMsg = "複製已完成";

            var newLessonId = AddCopyLessonCommand(dv, copyLessonID, txtCopyDate0, trans);

            //沒填日期無法排課
            if (txtCopyDate0 != string.Empty)
            {
                var dvLrnd_Count = _clsLessonRnd.SelectDistinctForLessonService(copyLessonID);
                var dvLSetting = _cLessonSetting.SelectLessonSetting(copyLessonID);

                //找出該班期間的全部資料,檢查衝堂用
                var copyDate = DateTime.Parse(tools.DateTimeCovert(txtCopyDate0, true));
                DataView dvAll = SelectAllRndFromDate(dv, copyDate);

                var newDateHandler = new LessonCopyNewDateHandler(copyDate);
                for (int k = 0; k < dvLrnd_Count.Count; k++)
                {
                    var dvLrnd = _clsLessonRnd.GetRndDate(copyLessonID, dvLrnd_Count[k]["RndID"].ToString());
                    for (int i = 0; i < dvLrnd.Count; i++)
                    {
                        var newDate0 = newDateHandler.GetNewCopyDate(Convert.ToDateTime(dvLrnd[i]["Date0"]));

                        var rndid = Guid.NewGuid().ToString();

                        trans.CreateCommand(@"
insert into LessonRnd
(RndID,LanNo,LessonID,LessonRID,Room, Date0,CrtTime,CrtUser,Obligatory,LessonRName,Agendar)
values
(@RndID,@LanNo,@LessonID,@LessonRID,@Room, @Date0,now(),@CrtUser,@Obligatory,@LessonRName,@Agendar)"
                         , new SqlTransParameter("RndID", rndid)
                         , new SqlTransParameter("LanNo", tool.currentLanguage)
                         , new SqlTransParameter("LessonID", newLessonId)
                         , new SqlTransParameter("LessonRID", tools.Cast<string>(dvLrnd[i]["LessonRID"]))
                         , new SqlTransParameter("Room", tools.Cast<string>(dvLrnd[i]["Room"]))
                         , new SqlTransParameter("Date0", newDate0.ToString("yyyy/MM/dd"))
                         , new SqlTransParameter("CrtUser", MyApp.SysUser.Accnt)
                         , new SqlTransParameter("Obligatory", tools.Cast<string>(dvLrnd[i]["Obligatory"]))
                         , new SqlTransParameter("LessonRName", dvLrnd[i]["LessonRName"])
                         , new SqlTransParameter("Agendar", dvLrnd[i]["Agendar"]));

                        trans.CreateCommand(@"
insert into LessonLecturerGroup (LanNo, RndID, LecturerId, LessonID)
select LanNo, @nRndID, LecturerId, @nLessonID from LessonLecturerGroup where LessonID=@LessonID and RndID=@RndID"
                         , new SqlTransParameter("RndID", dvLrnd[i]["RndID"].ToString())
                         , new SqlTransParameter("nRndID", rndid)
                         , new SqlTransParameter("LessonID", tools.Cast<string>(dvLrnd[i]["LessonID"]))
                         , new SqlTransParameter("nLessonID", newLessonId));

                        dvLSetting.RowFilter = "RndID='" + dvLrnd[i]["RndID"].ToString() + "'";

                        var dtAll = dvAll.ToTable();
                        for (var j = 0; j < dvLSetting.Count; j++)
                        {
                            //教室衝堂檢查
                            var r = _cLessonSetting.CheckRoom(dtAll, dvLrnd[i]["Room"].ToString(), newDate0.ToString("yyyy/MM/dd"), dvLSetting[j]["LSettingID"].ToString());
                            if (r.Count == 0)
                            {
                                var t = _cLessonSetting.CheckTeacher(dvAll, dvLrnd[i]["Teacher"].ToString(), newDate0.ToString("yyyy/MM/dd"), dvLSetting[j]["LSettingID"].ToString());
                                if (t.Count == 0)
                                {
                                    //開始建LessonSetting
                                    trans.CreateCommand(@"
                insert into LessonSetting
                (LSettingID, RndID, LanNo, LessonID, CrtTime, CrtUser)
                select LSettingID, @nRndID, LanNo, @nLessonID, now(), @CrtUser from LessonSetting
                where LessonID=@LessonID and RndID=@RndID and LSettingID=@LSettingID"
                                         , new SqlTransParameter("RndID", dvLrnd[i]["RndID"].ToString())
                                         , new SqlTransParameter("nLessonID", newLessonId)
                                         , new SqlTransParameter("nRndID", rndid)
                                         , new SqlTransParameter("CrtUser", MyApp.SysUser.Accnt)
                                         , new SqlTransParameter("LSettingID", dvLSetting[j]["LSettingID"].ToString())
                                         , new SqlTransParameter("LessonID", dvLrnd[i]["LessonID"].ToString()));
                                }
                                else
                                {
                                    eMsg = "複製已完成 , 但有偵測到衝堂";
                                }
                            }
                            else
                            {
                                eMsg = "複製已完成 , 但有偵測到衝堂";
                            }
                        }
                    }
                }
            }

            _db.ExecuteTranscation(trans);

            return eMsg;

            ////沒填日期無法排課
            //if (txtCopyDate0 != string.Empty)
            //{
            //    var dvLrnd_Count = _clsLessonRnd.SelectDistinctForLessonService(copyLessonID);
            //    var dvLSetting = _cLessonSetting.SelectLessonSetting(copyLessonID);

            //    //找出該班期間的全部資料,檢查衝堂用
            //    var dvAll = _cLessonSetting.SelectRndCheckSetting(Convert.ToDateTime(dv[0]["RDate0"]).ToString("yyyy/MM/dd"), Convert.ToDateTime(dv[0]["RDate1"]).ToString("yyyy/MM/dd"));

            //    DateTime? lastDate = null; //原本排課迴圈跑到的日期
            //    DateTime newDate0 = DateTime.Parse(tools.DateTimeCovert(txtCopyDate0, true)).AddDays(-1);//新排課要排的日期
            //    for (int k = 0; k < dvLrnd_Count.Count; k++)
            //    {
            //        var dvLrnd = _clsLessonRnd.GetRndDate(copyLessonID, dvLrnd_Count[k]["RndID"].ToString());
            //        for (int i = 0; i < dvLrnd.Count; i++)
            //        {
            //            var date0 = Convert.ToDateTime(dvLrnd[i]["Date0"]);
            //            if (lastDate != date0)
            //            {
            //                lastDate = date0;
            //                newDate0 = _dateHelper.GetNextDateSkipHoliday(newDate0);
            //            }
            //            lastDate = Convert.ToDateTime(dvLrnd[i]["Date0"]);

            //            var rndid = Guid.NewGuid().ToString();
            //            trans.CreateCommand(@"
        }
        protected virtual DataView SelectAllRndFromDate(DataView dv, DateTime copyDate)
        {
            var days = GetDaysCount(Convert.ToDateTime(dv[0]["RDate1"]), Convert.ToDateTime(dv[0]["RDate0"]));
            return _cLessonSetting.SelectRndCheckSetting(copyDate.ToString("yyyy/MM/dd"), copyDate.AddDays(days).ToString("yyyy/MM/dd"));
        }

        protected virtual int GetDaysCount(DateTime begin, DateTime end)
        {
            return begin.Subtract(end).Days;
        }

        private string AddCopyLessonCommand(DataView dv, string copyLessonID, string txtCopyDate0, SqlTransHelper trans)
        {
            //原班別起訖天數
            var days = GetDaysCount(Convert.ToDateTime(dv[0]["RDate1"]), Convert.ToDateTime(dv[0]["RDate0"]));
            var newId = this.LessonGetSNO(MyApp.SysUser.Dept, dv[0]["LKID"].ToString());

            var tableColService = new TableColmunsHelper(_db);
            var removeColumns = new List<string> { "aID", "LessonID", "LessonName", "RDate0", "RDate1", "CrtTime" };
            var resultColumns = tableColService.GetTableColumnsForCommand("lesson", removeColumns);
            var sql = string.Format(@"
INSERT Lesson
(
    {0},
LessonID,
LessonName,
RDate0,
RDate1,
CrtTime
)
SELECT

    {0},
@newLessoonid,
@LessonName,
@RDate0,
@RDate1,
now()

  FROM Lesson
  where LessonID =@lessonid;
", string.Join(",", resultColumns.Item1));

            trans.CreateCommand(sql
                , new SqlTransParameter("newLessoonid", newId)
                , new SqlTransParameter("LessonName", string.Format("複製-{0}", dv[0]["LessonName"]).ToString())
                //沒填日期就給複製班的日期
                , new SqlTransParameter("RDate0", txtCopyDate0 == string.Empty ? Convert.ToDateTime(dv[0]["RDate0"]).ToString("yyyy/MM/dd") : tools.DateTimeCovert(txtCopyDate0, true))
                , new SqlTransParameter("RDate1", txtCopyDate0 == string.Empty ? Convert.ToDateTime(dv[0]["RDate1"]).ToString("yyyy/MM/dd")
                    : DateTime.Parse(tools.DateTimeCovert(txtCopyDate0, true)).AddDays(days).ToString("yyyy/MM/dd")) //新班別起日加上天數
                , new SqlTransParameter("lessonid", copyLessonID)
                , new SqlTransParameter("CrtUSer", MyApp.SysUser.Accnt));

            return newId;
        }


        public virtual string LessonGetSNO(string ouid, string lkid)
        {
            //年序號+處別碼+類別代碼+班序號
            var sno = _db.getSNo;
            var dvSetting = _db.selectSQL("select SNo from LessonSerialSetting where ouid=@ouid"
                    , new ListItemCollection { new ListItem("ouid", ouid) });
            var ouNo = dvSetting.Count > 0 ? dvSetting[0]["SNo"].ToString() : "85"; //沒設定用員訓中心
            if (sno.Length >= 7)
            {
                var y = (int.Parse(sno.left(4)) - 1911).ToString();
                var no = sno.Right(3);
                lkid = lkid.Right(2);
                return string.Concat(y, ouNo, lkid, no);
            }
            else
                throw new Exception("開班編碼設定錯誤");
        }
    }

    public class LessonCopyNewDateHandler
    {
        protected DateTime? lastDate = null; //原本排課迴圈跑到的日期
        protected DateTime newDate0;
        protected DateHelper _dateHelper;

        public LessonCopyNewDateHandler(DateTime copyDate)
        {
            _dateHelper = Factory.HelperFactory.GetDateHelper();
            newDate0 = copyDate.AddDays(-1);//新排課要排的日期
        }

        /// <summary>
        /// 根據date0日期 取得複製排課的日期
        /// </summary>
        /// <param name="date0"></param>
        /// <returns></returns>
        public virtual DateTime GetNewCopyDate(DateTime date0)
        {
            if (lastDate != date0)
            {
                lastDate = date0;
                newDate0 = _dateHelper.GetNextDateSkipHoliday(newDate0);
            }
            lastDate = date0;

            return newDate0;
        }
    }
}