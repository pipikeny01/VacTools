using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;
using aiet.Base;
using System.Linq;


/// <summary>
/// clsAccount 的摘要描述
/// </summary>
public class clsSerialFmt : BaseTable
{

    public clsSerialFmt()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
        this.TableName = "SerialFormat";
        NameField = "CallerName";
        pk = "Caller";
        dfs = new DataFields("Caller", "CallerName");

    }

    public override bool isUsed(string id)
    {
        _params.Clear();
        _params.Add(new ListItem("id", id));

        DataView dv = this.selectSQL(@"SELECT  a.aID FROM [CurSerialNo]  a inner join  [SerialFormat] b on a.LanNo = b.LanNo and a.Caller = b.Caller WHERE b.aID =@id", _params);
        if (dv != null && dv.Count > 0) { return true; }

        return false;
    }

    public ListItem[] DtFomat
    {
        get
        {
            ListItem[] li = new ListItem[5];
            li[0] =new ListItem("","-");
            li[1] = new ListItem("yyyy","yyyy");
            li[2] = new ListItem("yyyyMM","yyyyMM");
            li[3] = new ListItem("yyyyMMdd","yyyyMMdd");
            li[4] =  new ListItem("YMD","YMD");
            return li;
        }
    }

    /// <summary>
    ///  取序號
    /// </summary>
    /// <param name="Caller">執行程式編號</param>
    /// <param name="Len">總長度 (Prefix  + Date Foramt + Serial Number chars + Suffix)</param>
    /// <returns></returns>
    public string getSNo (string Caller,int Len)
    {
        if (Caller == string.Empty) throw new Exception("Caller 參數未設定");

        string N = "";
        ListItemCollection pp = new ListItemCollection();
        pp.Add(new ListItem("LanNo", tool.currentLanguage));
        pp.Add(new ListItem("Caller", Caller));
        pp.Add(new ListItem("Len", Len.ToString()));
        pp.Add(new ListItem("User", tool.currentLanguage));


        selectText = @"select * from SerialFormat where Caller=@Caller and LanNo=@LanNo";
        DataView dv = selectSQL(pp);
        int r = 0;

        //-- 有無定義序號格式
        if (dv.Count == 0)
        {

            r = instSQL(@"insert into SerialFormat(LanNo,Caller,CallerName,Prefix,Suffix,dtFormat,Len,isOverwrite,CrtTime,CrtUser)
 Values(@LanNo,@Caller,@Caller,NULL,NULL,'-',@Len,1,now(),@User)",pp);

            r = instSQL(@"insert into CurSerialNo(LanNo,Caller,dtFormat,CurNo,ModTime) Values(@LanNo,@Caller,'-',1,now())", pp);
            N = N.PadLeft(Len - 1, '0') + "1";
        }
        else
        {
            string dtFormat = dv[0]["dtFormat"].ToString();
            Len = Convert.ToInt32(dv[0]["Len"]);
            if ( dtFormat !="-")
            {
                dtFormat = DateTime.Now.ToString(dv[0]["dtFormat"].ToString());
            }

            pp.Add(new ListItem("dtFormat", dtFormat));

            r = updSQL(@"update CurSerialNo set CurNo = CurNo+1,ModTime=now() where LanNo=@LanNo and Caller=@Caller and dtFormat=@dtFormat", pp);
            
            if (r == 0)
            {
                //-- 覆蓋
                if (Convert.ToBoolean(dv[0]["isOverwrite"]))
                {
                    r = updSQL(@"update CurSerialNo set CurNo = 1,dtFormat=@dtFormat,ModTime=now() where LanNo=@LanNo and Caller=@Caller", pp);
                }

                if (r == 0)
                {
                    r = instSQL(@"insert into CurSerialNo(LanNo,Caller,dtFormat,CurNo,ModTime) Values(@LanNo,@Caller,@dtFormat,1,now())", pp);
                }
            }

            DataView dv0 = selectSQL(@"select CurNo from CurSerialNo where LanNo=@LanNo and Caller=@Caller and dtFormat=@dtFormat", pp);
            N = dv0[0]["CurNo"].ToString();
            N = "".PadLeft(Len - N.Length, '0') + N;
            N = string.Format("{0}{1}{2}{3}", dv[0]["Prefix"], dtFormat == "-" ? "" : dtFormat, N,dv[0]["Suffix"] );
        }

        return N;
    }

}