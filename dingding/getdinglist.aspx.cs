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
using Newtonsoft.Json.Linq;

public partial class dingding_getdinglist : System.Web.UI.Page
{
    //本页面主要完成钉钉组织架构的同步功能
    public string CorpID = "ding9f8fe97fe75dfabd";
    public string CorpSecret = "YOccTOcc3A902CdKExmoCABvCYOTmKmU9HPDYVt0qju_d0cXenX_zkgexYQHXRZz";
    protected void Page_Load(object sender, EventArgs e)
    {
        string access_token_url = "https://oapi.dingtalk.com/gettoken?corpid="+CorpID+"&corpsecret=" + CorpSecret;
        string access_token = getaccess_token(access_token_url);
        //同步之前先备份
        dd_synchronize(1);
        //同步部门列表和人员列表
        getalldepartment(access_token);
        //同步之后的数据处理
        dd_synchronize(2);
        //同步之后的权限处理
        dd_synchronize(3);
        Response.Write("<script>alert('完成')</script>");
    }
    private SqlConnection getConnection()
    {
        string strconn = ConfigurationSettings.AppSettings["dingding"];
        SqlConnection conn = new SqlConnection(strconn);
        return conn;
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
            access_token = accessjson.Substring(accessjson.IndexOf("access_token") + 15, accessjson.IndexOf("errcode") - access_token.IndexOf("access_token") - 21);
            return access_token;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }
    /// <summary>
    /// 同步流程控制以及处理存储过程
    /// </summary>
    /// <param name="step">1.备份数据，2.处理同步之后的数据，3.同步权限表</param>
    public void dd_synchronize(int step)
    {
        SqlConnection conn = this.getConnection();
        SqlCommand cmd = new SqlCommand("dd_synchronize", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@step", SqlDbType.Int);
        cmd.Parameters["@step"].Value = step;
        try
        {
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception("Execption getting dailyreport by userid. " + ex.Message);
        }
        finally
        {
            conn.Close();
        }
    }
    /// <summary>
    /// 返回部门列表
    /// </summary>
    /// <param name="access_token">公司access_token</param>
    /// <returns></returns>
    public void getalldepartment(string access_token)
    {
        string url = "https://oapi.dingtalk.com/department/list?access_token=" + access_token;
        string departmentname = "";
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
            string departmentjson = readStream.ReadToEnd().ToString();
            readStream.Close();
            JObject obj = JObject.Parse(departmentjson);
            var record = (JArray)obj["department"];
            for (int i = 0; i < record.Count; i++)
            {
                var jp = (JObject)record[i + 1];
                //添加部门数据
                insertdepartmentlist(jp["id"].ToString(), jp["parentid"].ToString(), jp["name"].ToString());
                //获取人员列表并添加到数据库
                getalldepartmentlist(access_token, jp["id"].ToString(), jp["parentid"].ToString(), jp["name"].ToString());
            }
        }
        catch (Exception ex)
        {
            ex.ToString();
        }
    }
    /// <summary>
    /// 返回部门成员详细信息 并添加到数据库
    /// </summary>
    /// <param name="access_token">公司access_token</param>
    /// <param name="departmentid">部门id</param>
    /// <param name="parentid">父级部门id</param>
    /// <param name="departmentname">部门名称</param>
    /// <returns></returns>
    public void getalldepartmentlist(string access_token, string departmentid, string parentid, string departmentname)
    {
        string url = "https://oapi.dingtalk.com/user/list?access_token=" + access_token + "&department_id=" + departmentid;
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
            string departmentjson = readStream.ReadToEnd().ToString();
            readStream.Close();
            JObject obj = JObject.Parse(departmentjson);
            var record = (JArray)obj["userlist"];
            for (int i = 0; i < record.Count; i++)
            {
                var jp = (JObject)record[i];
                //添加人员信息到数据库
                insertuserlist(departmentid, parentid, departmentname, jp["userid"].ToString(), jp["name"].ToString(), jp["position"].ToString());
            }
        }
        catch (Exception ex)
        {
            ex.ToString();
        }
    }
    /// <summary>
    /// 添加所有公司
    /// </summary>
    /// <param name="departmentid">部门id</param>
    /// <param name="parentid">父级部门id</param>
    /// <param name="dearptmentname">部门名称</param>
    public void insertdepartmentlist(string departmentid, string parentid, string dearptmentname)
    {
        string sql = "insert into departmentlist values(@deparmentid,@parentid,@deparmentname)";
        SqlConnection conn = this.getConnection();
        SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@deparmentid", SqlDbType.VarChar, 50);
        cmd.Parameters.Add("@parentid", SqlDbType.VarChar, 50);
        cmd.Parameters.Add("@deparmentname", SqlDbType.VarChar, 50);
        cmd.Parameters["@deparmentid"].Value = departmentid;
        cmd.Parameters["@parentid"].Value = parentid;
        cmd.Parameters["@deparmentname"].Value = dearptmentname;
        try
        {
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            conn.Close();
        }
    }
    /// <summary>
    /// 添加组织数据到数据库
    /// </summary>
    /// <param name="departmentid">部门id</param>
    /// <param name="parentid">父级部门id</param>
    /// <param name="dearptmentname">部门名称</param>
    /// <param name="userid">用户id</param>
    /// <param name="username">用户名</param>
    /// <param name="position">用户职位</param>
    public void insertuserlist(string departmentid, string parentid, string dearptmentname, string userid, string username, string position)
    {
        string sql = "insert into userlist values(@userid,@username,@deparmentid,@parentid,@deparmentname,@position)";
        SqlConnection conn = this.getConnection();
        SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@userid", SqlDbType.VarChar, 50);
        cmd.Parameters.Add("@username", SqlDbType.VarChar, 50);
        cmd.Parameters.Add("@deparmentid", SqlDbType.VarChar, 50);
        cmd.Parameters.Add("@parentid", SqlDbType.VarChar, 50);
        cmd.Parameters.Add("@deparmentname", SqlDbType.VarChar, 50);
        cmd.Parameters.Add("@position", SqlDbType.VarChar, 50);
        cmd.Parameters["@userid"].Value = userid;
        cmd.Parameters["@username"].Value = username;
        cmd.Parameters["@deparmentid"].Value = departmentid;
        cmd.Parameters["@parentid"].Value = parentid;
        cmd.Parameters["@deparmentname"].Value = dearptmentname;
        cmd.Parameters["@position"].Value = position;
        try
        {
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            conn.Close();
        }
    }
}