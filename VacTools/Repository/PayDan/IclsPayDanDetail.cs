using System.Data;

public interface IclsPayDanDetail
{
    /// <summary>
    /// 用PayDanID當條間取得PayDanDetail資料
    /// </summary>
    /// <param name="PayDanID"></param>
    /// <returns></returns>
    DataTable GetPayDanDetail(string PayDanID);
}