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


public partial class dingding_dd_hourreport : System.Web.UI.Page
{
    //页面数据所需数组
    public List<string> hours = new List<string>();
    public List<string> dayamount = new List<string>();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (Session["userid"] == null)
            {
                string usercode = Request.QueryString["usercode"];
                string access_token = Request.QueryString["access_token"];
                if (!string.IsNullOrEmpty(usercode) && !string.IsNullOrEmpty(access_token))
                {
                    Session["userid"] = getuserid(usercode, access_token);
                    Session["departmentid"] = "";
                    Session["nowtimes"] = DateTime.Now.ToString("yyyy-MM-dd");
                }
                Session["userid"] = "056859204529950662";
                Session["departmentid"] = "";
                Session["nowtimes"] = DateTime.Now.ToString("yyyy-MM-dd");
            }
            populateData(Session["userid"].ToString(), Session["departmentid"].ToString(), Session["nowtimes"].ToString());
        }
    }
    /// <summary>
    /// 返回用户id
    /// </summary>
    /// <param name="usercode"></param>
    /// <param name="access_token"></param>
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
    /// 选择部门
    /// </summary>
    protected void btn_changedepartment_Click(object sender, EventArgs e)
    {
        if (Session["userid"] == null)
        {
            Response.Redirect("sessionexpire.html");
        }
        else
        {
            Session["kind"] = "hour";
            //Response.Redirect("dd_organization.aspx?userid=" + Session["userid"].ToString() + "&kind=day");  //在原来的部门选择界面多加一个判断 以用来返回日报或者小时报表界面      
            Response.Redirect("dd_orgnavigate.aspx");  
        }
    }
    /// <summary>
    /// 选择时间
    /// </summary>
    protected void btn_changetime_Click(object sender, EventArgs e)
    {
        if (Session["userid"] == null)
        {
            Response.Redirect("sessionexpire.html");
        }
        else
        {
            string gettime = Request.Form["gettimes"].ToString();
            Session["nowtimes"] = gettime;
            populateData(Session["userid"].ToString(), Session["departmentid"].ToString(), Session["nowtimes"].ToString());
        }
    }
    public void populateData(string userid, string deparmentid, string nowtime)
    {
        /**************************************
         * Get all the data from the database 
         * ************************************/
        DataSet ds = new DataSet();
        SqlConnection conn = new SqlConnection(ConfigurationSettings.AppSettings["dingding"]);
        SqlCommand cmd = new SqlCommand("dd_hourlyreport", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@userid", SqlDbType.VarChar, 100);
        cmd.Parameters.Add("@departmentid", SqlDbType.VarChar, 100);
        cmd.Parameters.Add("@rq", SqlDbType.DateTime);
        cmd.Parameters["@userid"].Value = userid;
        cmd.Parameters["@departmentid"].Value = deparmentid;
        if (!string.IsNullOrEmpty(nowtime))
        {
            cmd.Parameters["@rq"].Value = DateTime.Parse(nowtime);
        }
        else
        {
            cmd.Parameters["@rq"].Value = DateTime.Now;
            Session["nowtimes"] = DateTime.Now.ToString("yyyy-MM-dd");
            nowtimes.Text = Session["nowtimes"].ToString();
        }
        try
        {
            conn.Open();
            SqlDataAdapter a = new SqlDataAdapter(cmd);
            a.Fill(ds);
        }
        catch (Exception ex)
        {
            throw new Exception("Execption getting hourreport by userid. " + ex.Message);
        }
        finally
        {
            conn.Close();
        }

        /**************************************
         * Populate the data on the page
         * ************************************/
        //小时销售额
        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            hours.Add(ds.Tables[0].Rows[i]["hourid"].ToString());
            dayamount.Add(ds.Tables[0].Rows[i]["je"].ToString());
        }
        //店铺名称
        deparmentname.Text = ds.Tables[1].Rows[0]["departmentname"].ToString();
        Session["departmentid"] = ds.Tables[1].Rows[0]["departmentid"].ToString();
        nowtimes.Text = Session["nowtimes"].ToString();

        //目标
        if (ds.Tables[2].Rows.Count > 0)
        {
            lb_target.Text = "目标："+ds.Tables[2].Rows[0]["quota"].ToString();
            lb_sumje.Text = "实际销售："+ds.Tables[2].Rows[0]["sumje"].ToString();
        }
        //完成度
        if (ds.Tables[3].Rows.Count > 0)
        {
            Session["completion"] = ds.Tables[3].Rows[0]["completion"].ToString();
        }
        else
        {
            Session["completion"] = 0;
        }
        //时间输入框值
        gettimes.Value = nowtime;
    }
}