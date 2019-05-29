using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// clsEncrypt 的摘要描述
/// MS-SQL RSA非對稱式加解密 SQL Exrpess 字串回傳
/// </summary>
public static class clsRSA
{
    static string KeyID = "TRSEKey";
    static string KeyPSW = "tourism@1";

     //public clsRSA()
     //{
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    //}

    /// <summary>
    /// 傳回加密 SQL Express 字串
    /// </summary>
    /// <param name="Constant">常數</param>
    /// <returns></returns>
    public static string EncryptByConstant(string Constant)
    {
        return string.Format(@"ENCRYPTBYASYMKEY(ASYMKEY_ID('{0}'),CONVERT(nvarchar(150), '{1}' ))", KeyID, Constant);
    }

    /// <summary>
    /// 傳回加密 SQL Express 字串
    /// </summary>
    /// <param name="Variable">SQL變數 ex. @var </param>
    /// <returns></returns>
    public static string Encrypt(string Variable)
    {
        return string.Format(@"ENCRYPTBYASYMKEY(ASYMKEY_ID('{0}'),CONVERT(nvarchar(150), {1} ))", KeyID, Variable);
    }

    /// <summary>
    /// 傳回解密 SQL Express 字串
    /// </summary>
    /// <param name="fldName">欄位名稱</param>
    /// <returns></returns>
    public static string Decrypt(string fldName)
    {
        return string.Format(@"CONVERT(nvarchar(150),  DECRYPTBYASYMKEY(ASYMKEY_ID('{0}'),{1}, N'{2}'))",KeyID,fldName,KeyPSW);
    }

    public static string EncryptForMySql(string Variable)
    {
        return string.Format(@"AES_ENCRYPT({0}, '{1}')", Variable, KeyID);
    }

    public static string DecryptForMySql(string fldName)
    {
        return string.Format(@"CAST(AES_DECRYPT({0},'{1}') as char)", fldName, KeyID);
    }

}