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

public partial class Recruit_dingadmin : System.Web.UI.Page
{
    public string userinfo = "";
    public List<string> departmentids = new List<string>();
    public List<string> departmentnames = new List<string>();
    public List<string> userids = new List<string>();
    public List<string> usernames = new List<string>();
    public List<int> listLength = new List<int>();
    protected void Page_Load(object sender, EventArgs e)
    {
        //string userid = Request["userid"];
        string userid = "暂无";
        string access_token = Request["access_token"];
        if (!string.IsNullOrEmpty(userid) && !string.IsNullOrEmpty(access_token))
        {
            getalldepartment(access_token);
            listLength.Add(0);  //表示第一个数组从0开始
            for (int i = 0; i < departmentids.Count; i++)
            {
                getdepartmentlist(access_token, departmentids[i]);
                listLength.Add(userids.Count);
            }
            getshoplist();
        }
    }
    private SqlConnection getConnection()
    {
        string strconn = ConfigurationSettings.AppSettings["ERPFX"];
        SqlConnection conn = new SqlConnection(strconn);
        return conn;
    }
    /// <summary>
    /// 获取所有部门的id和部门的名称
    /// </summary>
    /// <param name="access_token">公司的access_token</param>
    /// <param name="userid">用户id</param>
    /// <returns></returns>
    public void getalldepartment(string access_token)
    {
        string url = "https://oapi.dingtalk.com/department/list?access_token=" + access_token;
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
            departmentjson=departmentjson.Substring(departmentjson.IndexOf("department") + 13, departmentjson.IndexOf("]") - departmentjson.IndexOf("department") - 13);
	        //分割部门信息
            string[] alllist = departmentjson.Split(new char[] { '}' }, StringSplitOptions.RemoveEmptyEntries);
            string departmentid="";
            string departmentname="";
            for (int i = 0; i < alllist.Length; i++)
            {
                departmentid = alllist[i].Substring(alllist[i].IndexOf("id")+4,alllist[i].IndexOf("name")-alllist[i].IndexOf("id")-6);
                departmentname = alllist[i].Substring(alllist[i].IndexOf("name") + 6);
                departmentids.Add(departmentid);
                departmentnames.Add(departmentname);
            
            }
        }
        catch (Exception ex)
        {
            ex.ToString();
        }
    }
    /// <summary>
    /// 获得部门成员信息
    /// </summary>
    /// <param name="access_token">公司access_token</param>
    /// <param name="departmentid">部门id</param>
    /// <returns></returns>
    public void getdepartmentlist(string access_token, string departmentid)
    {
        string url = "https://oapi.dingtalk.com/user/simplelist?access_token=" + access_token + "&department_id=" + departmentid;
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
            departmentjson = departmentjson.Substring(departmentjson.IndexOf("userlist") + 11, departmentjson.IndexOf("]") - departmentjson.IndexOf("userlist") - 11);
            //分割部门人员信息
            string[] alllist = departmentjson.Split(new char[] { '}' }, StringSplitOptions.RemoveEmptyEntries);
            string userid = "";
            string username = "";
            for (int i = 0; i < alllist.Length; i++)
            {
                userid = alllist[i].Substring(alllist[i].IndexOf("userid") + 9);
                username = alllist[i].Substring(alllist[i].IndexOf("name") + 7, alllist[i].IndexOf("userid") - alllist[i].IndexOf("name") - 10);
                userids.Add(userid);
                usernames.Add(username);
            }
        }
        catch (Exception ex)
        {
            ex.ToString();
        }
    }
    /// <summary>
    /// 获取店铺列表
    /// </summary>
    public void getshoplist()
    {
        string sql = "select KHDM,KHMC from KEHU";
        SqlConnection conn = this.getConnection();
        try
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                shoplist.Items.Add(new ListItem(dr["KHMC"].ToString(), dr["KHDM"].ToString()));
            }
            dr.Close();
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