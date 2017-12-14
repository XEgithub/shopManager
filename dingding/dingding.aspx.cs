using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using System.Web.Util;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

using System.Security.Cryptography;
using System.Text;  

public partial class Recruit_dingding : System.Web.UI.Page
{
    //public string CorpID = "dinge9419b6e6a09217235c2f4657eb6378f";
    //public string CorpSecret = "RoG-tmXGZ6X64qSV3jIbQ8NiSe-Y7vWSWo8hDJkro7ybm1nkgbB9lnWE4V0zqlB-";
    public string CorpID = "ding9f8fe97fe75dfabd";
    public string CorpSecret = "YOccTOcc3A902CdKExmoCABvCYOTmKmU9HPDYVt0qju_d0cXenX_zkgexYQHXRZz";
    public string timestamp;
    public string nonceStr = "";
    public string signature = "";
    //public string agentId = "38959995";
    public string agentId = "41866347";
    public string signatures = "";
    public string access_token = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        dd_log();
        string access_token_url = "https://oapi.dingtalk.com/gettoken?corpid="+CorpID+"&corpsecret=" + CorpSecret;
        access_token = getaccess_token(access_token_url);
        if (!string.IsNullOrEmpty(access_token))
        {
            string jsapi_ticket_url = "https://oapi.dingtalk.com/get_jsapi_ticket?access_token=" + access_token + "&type=jsapi";
            string ticket = getjsapi_ticket(jsapi_ticket_url);
            timestamp = gettimestamp();
            nonceStr = getnonceStr();
            // 这里参数的顺序要按照 key 值 ASCII 码升序排序 
            string rawstring = "jsapi_ticket=" + ticket + "&noncestr=" + nonceStr + "&timestamp=" + timestamp + "&url=http://www.datahunt.cn/dingding/dingding.aspx";
            signatures = Sha1Hex(rawstring).ToLower();  
        }
    }
    /// <summary>
    /// 通过CorpID和CorpSecret去获取access_token，每次获取之后有2小时的时限
    /// </summary>
    /// <param name="url">access_token的钉钉请求地址</param>
    /// <returns></returns>
    public string getaccess_token(string url)
    {
        string access_token = "";
        try
        {
            Uri uri = new Uri(url);
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(uri);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.AllowAutoRedirect = false;
            request.Timeout = 5000;
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(responseStream, System.Text.Encoding.UTF8);
            string accessjson = readStream.ReadToEnd().ToString();
            readStream.Close();
            access_token = accessjson.Substring(accessjson.IndexOf("access_token") +15, accessjson.IndexOf("errcode") - access_token.IndexOf("access_token")-21); 
            return access_token;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }
    /// <summary>
    /// 通过请求得到的access_token去获取jsapi_ticket
    /// </summary>
    /// <param name="url">jsapi_ticket的微信请求地址</param>
    /// <returns></returns>
    public string getjsapi_ticket(string url)
    {
        string jsapi_ticket = "";
        try
        {
            Uri uri = new Uri(url);
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(uri);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.AllowAutoRedirect = false;
            request.Timeout = 5000;
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(responseStream, System.Text.Encoding.UTF8);
            string jsapi_ticketjson = readStream.ReadToEnd().ToString();
            readStream.Close();
            jsapi_ticket = jsapi_ticketjson.Substring(jsapi_ticketjson.IndexOf("ticket") + 9, jsapi_ticketjson.IndexOf("}") - jsapi_ticketjson.IndexOf("ticket")-10);  
            return jsapi_ticket;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }
    /// <summary>
    /// 产生随机字符 用于生产签名
    /// </summary>
    /// <returns></returns>
    public string getnonceStr()
    {
        string[] strs = new string[]
                                 {
                                  "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
                                  "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"
                                 };
        Random r = new Random();
        var sb = new StringBuilder();
        var length = strs.Length;
        for (int i = 0; i < 15; i++)
        {
            sb.Append(strs[r.Next(length - 1)]);
        }
        return sb.ToString();
    }
    /// <summary>
    /// 产生随机时间戳 用于生产签名
    /// </summary>
    /// <returns></returns>
    public string gettimestamp()
    {
        long nowtime = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        return nowtime.ToString();
    }
    /// <summary>
    /// 生成签名包
    /// </summary>
    /// <param name="value">生成签名包需要的变量(ticket, nonceStr, timeStamp, url)</param>
    /// <returns></returns>
    public string Sha1Hex(string value)
    {
        SHA1 algorithm = SHA1.Create();
        byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
        string sh1 = "";
        for (int i = 0; i < data.Length; i++)
        {
            sh1 += data[i].ToString("x2").ToUpperInvariant();
        }
        return sh1;  
    }
    /// <summary>
    /// 钉钉的程序步骤日志记录
    /// </summary>
    public void dd_log()
    {
        DataSet ds = new DataSet();

        SqlConnection conn = new SqlConnection(ConfigurationSettings.AppSettings["dingding"]);
        SqlCommand cmd = new SqlCommand("proc_dd_log", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@userid", SqlDbType.VarChar, 100);
        cmd.Parameters.Add("@hostip", SqlDbType.VarChar, 100);
        cmd.Parameters.Add("@now", SqlDbType.DateTime);
        cmd.Parameters.Add("@step", SqlDbType.Int);
        cmd.Parameters.Add("@departmentid", SqlDbType.VarChar, 100);
        cmd.Parameters.Add("@rq", SqlDbType.VarChar, 20);
        cmd.Parameters.Add("@agentId", SqlDbType.VarChar, 50);
        cmd.Parameters["@userid"].Value = "";
        cmd.Parameters["@hostip"].Value = Request.UserHostAddress;
        cmd.Parameters["@now"].Value = DateTime.Now;
        cmd.Parameters["@step"].Value = 1;
        cmd.Parameters["@departmentid"].Value = "";
        cmd.Parameters["@rq"].Value = "";
        cmd.Parameters["@agentId"].Value = agentId;

        try
        {
            conn.Open();
            SqlDataAdapter a = new SqlDataAdapter(cmd);
            a.Fill(ds);
        }
        catch (Exception ex)
        {
            throw new Exception("Execption getting departmentlist by departmentid. " + ex.Message);
        }
        finally
        {
            conn.Close();
        }
        Session["timelog"] = ds.Tables[0].Rows[0]["nowtime"];
    }
}