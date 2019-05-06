using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;

/// <summary>
/// logEvent 的摘要描述
/// </summary>
public class logEvent : myDB
{
    private void writeEvent(string evenType, string eventDesc)
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
        try
        {

            Page page = (Page)HttpContext.Current.Handler;

            ListItemCollection pp = new ListItemCollection();
            pp.Add(new ListItem("ip",page.Form ==null?"": page.Request.UserHostAddress)); //0
            pp.Add(new ListItem("url", page.Form == null ? "" : page.Request.Url.AbsolutePath)); //1
            pp.Add(new ListItem("user", page.Form == null ? "" : page.Session["user"] == null ? "" : page.Session["user"].ToString())); //2
            pp.Add(new ListItem("grp", "")); //3
            pp.Add(new ListItem("prg", "")); //4
            pp.Add(new ListItem("evenType", evenType)); // 5
            pp.Add(new ListItem("eventDesc", eventDesc =="" ? evenType : eventDesc)); // 6
            string brow = string.Format("{0} {1}", page.Form == null ? "" : page.Request.Browser.Browser, page.Form == null ? "" : page.Request.Browser.Version);
            pp.Add(new ListItem("browser", brow)); //3


            DataView dv = this.selectSQL(@"select Roadmap,PrgNo from Program  where url=@url", pp);

            if (dv != null && dv.Count > 0)
            {
                pp[3].Value = dv[0]["Roadmap"].ToString();
                pp[4].Value = dv[0]["PrgNo"].ToString();
            }

            this.insertText = string.Format(@"INSERT INTO {0}(FullPath,PrgNo,EventKind,EventDesc,EventTime,LoginUser,IP,URL,Browser)
                VALUES(@grp,@prg,@evenType,@eventDesc,now(),@user,@ip,@url,@browser)", this.TableName);

            int r = this.instSQL(pp);
            if (r == -1) { new logError(this.errorMessage); }
        }
        catch (Exception ex)
        {
            new logError(new object[]{900,ex});
        }
    }

    public logEvent()
    {
        this.TableName = "LogEvent";
    }

    public logEvent(string evenType, string eventDesc)
    {
        this.TableName = "LogEvent";
        writeEvent(evenType, eventDesc);
    }

    public void delLog(string d1, string d2)
    {
        this.deleteText = string.Format("DELETE FROM {0} WHERE 1=1", this.TableName);
        if (d1 != "") { this.deleteText += " AND convert(char(10),111,evenTime) >= @d1"; }
        if (d2 != "") { this.deleteText += " AND convert(char(10),111,evenTime) <= @d2"; }

        ListItemCollection pp = new ListItemCollection();
        pp.Add(new ListItem("d1", d1)); //0
        pp.Add(new ListItem("d2", d2)); //1
        this.delSQL(pp);

        if (d1 != "" && d2 == "") { d1 = string.Format("delete from {0} to {1:yyyy/MM/dd}", d1, DateTime.Now); }
        else if (d1 == "" && d2 != "") { d1 = string.Format("delete smaller equal than {0}", d2); }
        else if (d1 == "" && d2 == "") { d1 = string.Format("delete smaller equal than {0:yyyy/MM/dd}", DateTime.Now); }
        else { d1 = string.Format("delete from {0} to {1}", d1, d2); }

        writeEvent("刪除", d1);
    }
}