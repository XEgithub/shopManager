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

public partial class Recruit_dinguserinfo : System.Web.UI.Page
{
    public string userinfo = "";
    public string message = "";
    public List<string> departmentids = new List<string>();
    public List<string> departmentnames = new List<string>();
    public List<string> deparmentname=new List<string> ();
    public List<string> deparmentinfolist=new List<string> ();
    public List<int> listLength = new List<int>();
    //添加组织架构所用list
    public List<string> deparmentlists = new List<string>();
    public List<string> parentid = new List<string>();
    public List<string> deparmentnamexxx = new List<string>();
    public List<string> userid = new List<string>();
    public List<string> username = new List<string>();
    protected void Page_Load(object sender, EventArgs e)
    {
        string usercode = Request["usercode"];
        string access_token = Request["access_token"];
        if (!string.IsNullOrEmpty(usercode) && !string.IsNullOrEmpty(access_token))
        {
            string userid = getuserid(usercode, access_token);
            userinfo = userid;
            //message = access_token;
            string deparmentid = getdepartment(access_token, userid);
            getalldepartment(access_token);
            //string [] deparmentlist=deparmentid.Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries);
            listLength.Add(0);  //表示第一个数组从0开始
            for (int i = 0; i < deparmentlists.Count; i++)
            {
                deparmentname.Add(getdepartmentinfo(access_token, deparmentlists[i]));
                string[] alllist = getdepartmentlist(access_token, deparmentlists[i]).Split(new char[] { '}' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < alllist.Length; j++)
                {
                    if (alllist[j].IndexOf("name") > 0 && alllist[j].IndexOf("userid") > 0)
                    {
                        deparmentinfolist.Add(alllist[j].Substring(alllist[j].IndexOf("name") + 7, alllist[j].IndexOf("userid") - alllist[j].IndexOf("name") - 10));
                    }   
                }
                listLength.Add(deparmentinfolist.Count);
                getalldepartmentlist(access_token, deparmentlists[i], parentid[i], deparmentnamexxx[i]);
            }
        }
    }
    private SqlConnection getConnection()
    {
        string strconn = ConfigurationSettings.AppSettings["ERPFX"];
        SqlConnection conn = new SqlConnection(strconn);
        return conn;
    }
    /// <summary>
    /// 返回客户id
    /// </summary>
    /// <param name="usercode">免登陆code</param>
    /// <param name="access_token">公司的access_token</param>
    /// <returns></returns>
    public string getuserid(string usercode, string access_token)
    {
        string url = "https://oapi.dingtalk.com/user/getuserinfo?access_token=" + access_token + "&code=" + usercode;
        string userid = "";
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
            string userinfojson = readStream.ReadToEnd().ToString();
            readStream.Close();
            userid = userinfojson.Substring(userinfojson.IndexOf("userid") + 9, userinfojson.IndexOf("}") - userinfojson.IndexOf("userid") - 10);
            return userid;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }
    /// <summary>
    /// 返回部门id
    /// </summary>
    /// <param name="access_token">公司的access_token</param>
    /// <param name="userid">用户id</param>
    /// <returns></returns>
    public string getdepartment(string access_token, string userid)
    {
        string url = "https://oapi.dingtalk.com/user/get?access_token="+access_token+"&userid="+userid;
        string departmentid = "";
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
            departmentid = departmentjson.Substring(departmentjson.IndexOf("department") + 13, departmentjson.IndexOf("dingId") - departmentjson.IndexOf("department") - 16);
            return departmentid;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }
    /// <summary>
    /// 返回部门信息
    /// </summary>
    /// <param name="access_token">公司access_token</param>
    /// <param name="departmentid">部门id</param>
    /// <returns></returns>
    public string getdepartmentinfo(string access_token, string departmentid)
    {
        string url = "https://oapi.dingtalk.com/department/get?access_token=" + access_token + "&id=" + departmentid;
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
            departmentname = departmentjson.Substring(departmentjson.IndexOf("name") + 7, departmentjson.IndexOf("order") - departmentjson.IndexOf("name") - 10);
            return departmentname;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }
    /// <summary>
    /// 返回部门成员信息
    /// </summary>
    /// <param name="access_token">公司access_token</param>
    /// <param name="departmentid">部门id</param>
    /// <returns></returns>
    public string getdepartmentlist(string access_token, string departmentid)
    {
        string url = "https://oapi.dingtalk.com/user/simplelist?access_token="+access_token+"&department_id=" +departmentid;
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
            departmentname = departmentjson.Substring(departmentjson.IndexOf("userlist") + 11, departmentjson.IndexOf("]") - departmentjson.IndexOf("userlist") - 11);
            return departmentname;
        }
        catch (Exception ex)
        {
            return ex.ToString();
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
            for (int i = 0; i < 1000; i++)
            {
                deparmentlists.Add(obj["department"][i+1]["id"].ToString());
                deparmentnamexxx.Add(obj["department"][i+1]["name"].ToString());
                parentid.Add(obj["department"][i+1]["parentid"].ToString());
                insertdepartmentlist(obj["department"][i + 1]["id"].ToString(), obj["department"][i + 1]["parentid"].ToString(), obj["department"][i + 1]["name"].ToString());
            }
        }
        catch (Exception ex)
        {
            ex.ToString();
        }
    }
    /// <summary>
    /// 返回部门成员详细信息
    /// </summary>
    /// <param name="access_token">公司access_token</param>
    /// <param name="departmentid">部门id</param>
    /// <param name="parentid">父级部门id</param>
    /// <param name="departmentname">部门名称</param>
    /// <returns></returns>
    public void getalldepartmentlist(string access_token, string departmentid, string parentid, string departmentname)
    {
        string url = "https://oapi.dingtalk.com/user/list?access_token=" + access_token + "&department_id=" + departmentid;
        string userid = "";
        string username = "";
        string position = "";
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
            for (int i = 0; i <500; i++)
            {
                userid = obj["userlist"][i]["userid"].ToString();
                username=obj["userlist"][i]["name"].ToString();
                position=obj["userlist"][i]["position"].ToString();
                insertuserlist(departmentid, parentid, departmentname, userid, username, position);
            }
        }
        catch (Exception ex)
        {
            ex.ToString();
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
    public void insertuserlist(string departmentid, string parentid,string dearptmentname,string userid,string username,string position)
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
            message = "写入成功";
            //Response.Write("<script>alert('表导入成功!');</script>");
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
            //Response.Write("<script>alert('表导入成功!');</script>");
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