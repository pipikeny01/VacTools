using System;
using System.Collections.Generic;


using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace aiet.Base
{
    /// <summary>
    /// BaseTable 提供資料表物件底層功能 , 資料表物件請繼承此類別
    /// </summary>
    public abstract class BaseTable : myDB
    {
        protected DataView listName;
        protected ListItemCollection _params = new ListItemCollection();
        protected ListItem[] _items;
        protected ListItemCollection _itemsParams;

        /// <summary>
        /// PrimaryKey 欄位 (預設aid)
        /// </summary>
        public string pk = "aid";

        #region property

        /// <summary>
        /// 提供給listitem欄位名稱
        /// </summary>
        public string NameField { set; get; }

        /// <summary>
        /// 提供給listitem欄位名稱
        /// </summary>
        public DataFields dfs { set; get; }

        /// <summary>
        /// 提供public virtual string[] strings , 顯示文字
        /// </summary>
        public string OptionText { set; get; }

        /// <summary>
        /// Items 屬性 where 條件 (and 開頭)
        /// </summary>
        public string ItemsWhereString { set; get; }

        /// <summary>
        /// Items 屬性 自訂Sql
        /// </summary>
        public string ItemsSqlString { set; get; }

        /// <summary>
        /// 決定 ListItem.Text 顯示內容
        /// true : ListItem.Text = Value -Name
        /// false : ListItem.Text = Name
        /// </summary>
        public bool itemConcat = true;



        /// <summary>
        /// items 屬性 查詢參數設定
        /// </summary>
        public ListItemCollection ItemsParams
        {
            get
            {
                if (_itemsParams == null)
                    _itemsParams = new ListItemCollection();

                return _itemsParams;
            }
        }

        /// <summary>
        /// 產生key和name ListItem集合 , 可用於listItemCollection的屬性
        /// </summary>
        public virtual ListItem[] Items
        {
            get
            {
                if (_items == null)
                {
                    if (dfs != null)
                    {
                        DataView listName;
                        if (string.IsNullOrEmpty(ItemsSqlString))
                        {
                            listName = this.selectSQL(string.Format("SELECT {0} as NO,{1} as  NAME FROM {2} where 1=1  {3} ", dfs.DataValue, dfs.DataText, TableName, ItemsWhereString), ItemsParams);
                            listName.Sort = "NO";
                        }
                        else
                        {
                            listName = this.selectSQL(ItemsSqlString, ItemsParams);
                        }

                        _items = new ListItem[listName.Count + 1];
                        ListItem li = new ListItem(OptionText, "");
                        _items.SetValue(li, 0);
                        for (int i = 0; i < listName.Count; i++)
                        {
                            if (itemConcat)
                            {
                                li = new ListItem(string.Concat(listName[i]["NO"].ToString(), " - ", listName[i]["NAME"].ToString()), listName[i]["NO"].ToString());
                            }
                            else
                            {
                                li = new ListItem(listName[i]["NAME"].ToString(), listName[i]["NO"].ToString());
                            }
                            _items.SetValue(li, i + 1);
                        }
                    }
                }
                return _items;
            }
        }

        /// <summary>
        /// Items 轉string[] ,提供js.Select() 方法參數使用
        /// 以:分隔 , 第一欄是value , 第二欄是Text
        /// </summary>
        public virtual string[] strings
        {
            get
            {
                ListItem[] items = this.Items;
                List<string> lst = new List<string>();
                lst.Add(":" + OptionText);
                for (int i = 1; i < items.Length; i++)
                {
                    lst.Add(string.Format("{0}:{1}", items[i].Value, items[i].Text));
                }
                return lst.ToArray();
            }
        }

        /// <summary>
        /// 臨時唯一鍵 ，-1 開始往左算
        /// </summary>
        public virtual string getTempNo
        {
            get
            {
                this.selectText = "select tmpNo from TempNo where aTable=@table";
                this.updateText = "Update TempNo set tmpNo=@no  where aTable=@table";
                this.insertText = "INSERT INTO TempNo(aTable,tmpNo) Values(@table,@no)";

                _params.Clear();
                _params.Add(new ListItem("table", TableName));
                _params.Add(new ListItem("no", "-1"));

                DataView dv = this.selectSQL(_params);
                if (dv.Count == 0)
                {
                    this.instSQL(_params);
                    return "-1";
                }
                else
                {
                    int i = Convert.ToInt32(dv[0][0]) - 1;
                    _params[1].Value = i.ToString();
                    this.updSQL(_params);
                    return i.ToString();
                }
            }
        }

        public virtual string getSNo
        {
            get
            {
                throw new Exception("必須override");
            }
        }

        public string IDENT_CURRENT
        {
            get
            {
                return GetIDENT_CURRENT();
            }
        }

        public string NextID
        {
            get
            {
                selectText = string.Format("select max({0}) No from {1} ", pk, TableName);
                DataView dv = selectSQL();

                if (dv[0][0] is DBNull)
                {
                    return "1";
                }
                else
                {
                    return (Convert.ToInt32(dv[0][0]) + 1).ToString();
                }
            }
        }

        #endregion property

        public BaseTable()
        {
        }

        /// <summary>
        /// 取得NameField的值
        /// </summary>
        /// <param name="aID">pk</param>
        /// <returns></returns>
        public virtual string getName(string aID)
        {
            DataView listName;
            string fieldName = "";

            if (dfs.DataValue == "")
            {
                fieldName = pk;
                this.selectText = string.Format("SELECT [{0}] as NAME FROM {2} Where {1}=@{1}  ", NameField, pk, TableName);
            }
            else
            {
                fieldName = dfs.DataValue;
                this.selectText = string.Format("SELECT [{0}] as NAME FROM {2} Where {1}=@{1}  ", dfs.DataText, dfs.DataValue, TableName);
            }
            _params.Clear();
            _params.Add(new ListItem(fieldName, aID));
            listName = this.selectSQL(_params);

            return listName.Count > 0 ? listName[0][0].ToString().Trim() : "";
        }

        /// <summary>
        /// 條件查詢 by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual DataView getRowById(string id)
        {
            _params.Clear();
            _params.Add(new ListItem(pk, id));
            this.selectText = string.Format("SELECT * FROM {0} WHERE {1} = @{1}", TableName, pk);
            return this.selectSQL(_params);
        }

        public virtual DataView SelectAll()
        {
            this.selectText = string.Format("SELECT * FROM {0} ", TableName);
            return this.selectSQL();

        }
        /// <summary>
        /// 檢查是否存在
        /// </summary>
        /// <param name="name">名稱</param>
        public virtual bool isExisted(string key)
        {
            _params.Clear();
            _params.Add(new ListItem(pk, key));
            string sql = string.Format("SELECT * FROM {0} WHERE {1}=@{1}", TableName, pk);
            DataView dv = this.selectSQL(sql, _params);
            return (dv.Count > 0);
        }

        /// <summary>
        /// 不要用了
        /// </summary>
        /// <param name="name">名稱</param>
        /// <param name="id">aid</param>
        /// <returns></returns>
        public virtual bool isExisted(string name, string id)
        {
            var param = new ListItemCollection();
            param.Add(new ListItem(NameField, name));
            param.Add(new ListItem(pk, id));

            string sql = string.Format("SELECT * FROM {0} WHERE {1}=@{1} AND {2} <> @{2} ", TableName, NameField, pk);
            DataView dv = this.selectSQL(sql, param);
            return (dv.Count > 0);
        }

        /// <summary>
        /// 不要用了
        /// </summary>
        /// <param name="Lang"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool ixExisted(string _NameField, string name)
        {
            selectText = string.Format("SELECT * FROM {0} WHERE {1}=@{1} and LanNo=@LanNo", TableName, _NameField);
            _params.Clear();
            _params.Add(new ListItem("LanNo", "zh-tw"));
            _params.Add(new ListItem(_NameField, name));
            return selectSQL(_params).Count == 0 ? false : true;
        }

        /// <summary>
        /// 依照語系檢查PK是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public virtual bool isExistedLan(string key, string pkName)
        {
            selectText = string.Format("SELECT * FROM {0} WHERE {1}=@{1}", TableName, pkName);
            _params.Clear();
            //_params.Add(new ListItem("LanNo", tool.currentLanguage));
            _params.Add(new ListItem(pkName, key));
            return selectSQL(_params).Count == 0 ? false : true;
        }

        /// <summary>
        /// 依照語系檢查PK是否重複 , 當檢查重複的欄位非pk時才需檢查
        /// </summary>
        /// <param name="Lang"></param>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool ixExisted(string Lang, string name, string id)
        {
            _params.Clear();
            _params.Add(new ListItem("LanNo", "zh-tw"));
            _params.Add(new ListItem(NameField, name));
            _params.Add(new ListItem(pk, id));

            selectText = string.Format("SELECT * FROM {0} WHERE {1}=@{1} AND {2} <> @{2} and LanNo=@LanNo ", TableName, NameField, pk);
            DataView dv = this.selectSQL(_params);
            return (dv.Count > 0);
        }

        /// <summary>
        /// IDENT_CURRENT
        /// </summary>
        /// <returns></returns>
        public string GetIDENT_CURRENT()
        {
            return (TableName != string.Empty) ? currentTableIdentity(TableName) : "0";
        }

        /// <summary>
        /// 依條件取得紀錄總數
        /// </summary>
        /// <param name="sds"></param>
        /// <param name="ww"></param>
        /// <returns></returns>
        public int GetRowsCount(SqlDataSource sds, string ww)
        {
            sds.SelectCommand = string.Format("SELECT count({0}) FROM {1} where 1=1 {2}", pk, this.TableName, ww);
            DataView dv = sds.Select(DataSourceSelectArguments.Empty) as DataView;
            return dv != null && dv.Count > 0 ? Convert.ToInt32(dv[0][0]) : 0;
        }

        /// <summary>
        /// 取得指定欄位的值
        /// </summary>
        /// <param name="fieldsName">欄位名稱</param>
        /// <param name="pkValue">唯一鍵</param>
        /// <returns></returns>
        public DataView GetFields(string pkValue = "", params string[] fieldsName)
        {
            _params.Clear();
            string sql = string.Format("SELECT {0} FROM {1} where 1=1", string.Join(",", fieldsName), this.TableName);

            if (pkValue != "")
            {
                sql += string.Format(" and {0}=@{0} ", pk);
                _params.Add(new ListItem(pk, pkValue));
            }

            var dv = this.selectSQL(sql, _params);
            return dv;
        }

        public virtual bool isUsed(string id)
        {
            return false;
        }

        /// <summary>
        /// 判斷是否有其他地方使用，如果是設定僅讀
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sender"></param>
        /// <param name="id"></param>
        /// <param name="t"></param>
        public void setReadOnly<T>(object sender, string id) where T : BaseTable
        {
            T t = (T)Activator.CreateInstance(typeof(T));
            TextBox obj = (TextBox)sender;
            obj.ReadOnly = t.isUsed(id);
            if (obj.ReadOnly)
            {
                obj.BackColor = System.Drawing.Color.Silver;
            }
        }

        /// <summary>
        /// 判斷是否有其他地方使用，如果有取消刪除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sender"></param>
        /// <param name="id"></param>
        public void cancelDeleting<T>(object sender, string id, SqlDataSourceCommandEventArgs e) where T : BaseTable
        {
            T t = (T)Activator.CreateInstance(typeof(T));
            if (t.isUsed(id))
            {
                tool.ShowMessage("已被使用");
                new logEvent("刪除失敗", "已被使用");
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 傳入sql語法加上sql分頁語法 , row_number別名固定用RowIndex
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="param"></param>
        /// <param name="selectTotal">查詢筆數</param>
        /// <returns></returns>
        public DataModel AddPagerScript(string sql, int pageIndex, int pageSize, ListItemCollection param, bool selectTotal = false)
        {
            var db = new myDB();
            int start = pageIndex * pageSize;
            int total = 0;

            if (true/*selectTotal*/)
            {
                var dvTotal = db.selectSQL(string.Format(@"
select count(*) cnt from ({0})tt
", sql), param);
                total = Convert.ToInt32(dvTotal[0][0]);
            }

            sql = string.Format(@"
select * from (
{0})t
where RowIndex between cast( @starRowIndex as int) +1  and cast( @starRowIndex as int)  + cast(@take as int)"
       , sql);

            param.Add(new ListItem("starRowIndex", start.ToString()));
            param.Add(new ListItem("take", pageSize.ToString()));

            var dv = db.selectSQL(sql, param);

            return new DataModel
            {
                Data = dv.ToTable(),
                Total = total,
                End = dv.Count > 0 ? Int32.Parse(dv[dv.Count - 1]["RowIndex"].ToString()) >= total : true  //如果最後剛好取五筆 , 還是會為false , 比較好的作法是用總筆數去算
            };
        }

        /// <summary>
        /// 傳入sql語法加上sql分頁語法 , row_number別名固定用RowIndex
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="param"></param>
        /// <param name="selectTotal">查詢筆數</param>
        /// <returns></returns>
        public DataModel AddPagerScriptOther(string sql, int pageIndex, int pageSize, ListItemCollection param, bool selectTotal = true, string otherSql = "" )
        {
            var db = new myDB();
            int start = pageIndex * pageSize;
            int total = 0;

            if (true/*selectTotal*/)
            {
                var dvTotal = db.selectSQL(string.Format(@"
{1} select count(*) cnt from ({0})tt
", sql, otherSql), param);
                total = Convert.ToInt32(dvTotal[0][0]);
            }

            sql = string.Format(@"
{1} select * from (
{0})t
where RowIndex between cast( @starRowIndex as int) +1  and cast( @starRowIndex as int)  + cast(@take as int)"
       , sql, otherSql);

            param.Add(new ListItem("starRowIndex", start.ToString()));
            param.Add(new ListItem("take", pageSize.ToString()));

            var dv = db.selectSQL(sql, param);

            return new DataModel
            {
                Data = dv.ToTable(),
                Total = total,
                End = dv.Count > 0 ? Int32.Parse(dv[dv.Count - 1]["RowIndex"].ToString()) >= total : true  //如果最後剛好取五筆 , 還是會為false , 比較好的作法是用總筆數去算
            };
        }

    }
    /// <summary>
    /// 提供給listitem欄位名稱
    /// </summary>
    public class DataFields
    {
        private string _DataText = "";
        private string _DataValue = "";

        /// <summary>
        /// 提供給listitem欄位名稱
        /// </summary>
        /// <param name="dataValue">值的欄位</param>
        /// <param name="dataText">顯示的欄位</param>
        public DataFields(string dataValue, string dataText)
        {
            _DataText = dataText;
            _DataValue = dataValue;
        }

        /// <summary>
        /// 顯示的欄位
        /// </summary>
        public string DataText
        {
            set { _DataText = value; }
            get { return _DataText; }
        }

        /// <summary>
        /// 值的欄位
        /// </summary>
        public string DataValue
        {
            set { _DataValue = value; }
            get { return _DataValue; }
        }
    }

}