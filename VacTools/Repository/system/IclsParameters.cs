public interface IclsParameters
{
    /// <summary>
    /// 
    /// </summary>
     clsParameters.TeacherQualityualifiODT TeacherQualityualifi { get; }


    /// <summary>
    /// 取得內聘時數限制
    /// </summary>
    /// <returns></returns>
    int GetParameterForInTeacherHourLimit();

    /// <summary>
    /// 取得外聘時數限制
    /// </summary>
    /// <returns></returns>
    int GetParameterForOutTeacherHourLimit();
}