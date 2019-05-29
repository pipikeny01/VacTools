using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Reflection.Emit;
using System.Data;
/// <summary>
/// GridViewTemplate 的摘要描述
/// </summary>
public class  GridViewTemplate<T> : ITemplate where T : Control
{

    private DataControlRowType templateType;
    private string columnName;
    private DataColumnCollection  columns;

    public delegate void _dataBinding(object sender, Dictionary<string, string> row);
    public _dataBinding dataBinding;

    public delegate void _mydataBinding(object sender, Dictionary<string, string> row, string columname);
    public _mydataBinding mydataBinding;


    public T TemplateControl { set; get; }

    public GridViewTemplate(DataControlRowType type, string colname,DataColumnCollection Columns=null)
    {
        templateType = type;
        columnName = colname;
        columns = Columns;

        TemplateControl = Activator.CreateInstance<T>();
    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        switch (templateType)
        {
            case DataControlRowType.Header:

                T head = Activator.CreateInstance<T>();
                //T head = TemplateControl;
                head.GetType().GetProperty("Text").SetValue(head, columnName, null);
                container.Controls.Add(head);

                break;

            case DataControlRowType.DataRow:

                T data = Activator.CreateInstance<T>();
                //T data = TemplateControl;
                data.DataBinding += data_DataBinding;
                container.Controls.Add(data);

                break;

            default:

                break;
        }
    }

    protected void data_DataBinding(object sender, EventArgs e)
    {
        T l = (T)sender;
        
        GridViewRow row = (GridViewRow)l.NamingContainer;
        Dictionary<string,string> rowData = new Dictionary<string,string>();
        if (columns  == null ) {
              rowData.Add(columnName,DataBinder.Eval(row.DataItem , columnName).ToString());
        }
        else
        {
           foreach (DataColumn col in columns) {
              rowData.Add(col.ColumnName,DataBinder.Eval(row.DataItem , col.ColumnName).ToString());
           }
        }

        if ( dataBinding != null)
        {
            dataBinding(l, rowData);
        }

        if(mydataBinding != null)
            mydataBinding(l, rowData, columnName);

        //l.GetType().GetProperty("Text").SetValue(l, DataBinder.Eval(row.DataItem, columnName).ToString(), null);
    }

}