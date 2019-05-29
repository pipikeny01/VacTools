using System.Data;

public interface IclsEducationLecturer
{
    /// <summary>
    /// 查詢教師日期區間的時數
    /// </summary>
    /// <param name="aid"></param>
    /// <param name="d1"></param>
    /// <param name="d2"></param>
    /// <returns></returns>
    DataView SelectTeacherHourCount(string aid , string d1 , string d2);
}