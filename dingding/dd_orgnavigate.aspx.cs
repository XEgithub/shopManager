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


public partial class dd_orgnavigate : System.Web.UI.Page
{
    public DataSet ds_all;    

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
           string usercode = Request.QueryString["usercode"];
           string access_token = Request.QueryString["access_token"];
           if (!string.IsNullOrEmpty(usercode) && !string.IsNullOrEmpty(access_token))
           {
                Session["userid"] = getuserid(usercode, access_token);
                Session["departmentid"] = "";
                Session["nowtimes"] = DateTime.Now.ToString("yyyy-MM-dd");
                Session["kind"] = "day";
           }
           Session["userid"] = "051101395121419832";//04512113551383 李雪梅 056859204529950662 02332944386604 周勇 03075755501182 赵弘 024164092429274526 王智谍
           Session["departmentid"] = "";
           Session["nowtimes"] = DateTime.Now.ToString("yyyy-MM-dd");
            //dd_log(2, Session["userid"].ToString(), Session["departmentid"].ToString(), Session["nowtimes"].ToString());
            Session["kind"] = "day";
            //选择部门的时候 会修改数据表上面的层级关系所以需要重新对Session["departmentid"]赋值 例如：直营》重庆直营
            if (!string.IsNullOrEmpty(Request.QueryString["departmentid"]))
            {
                Session["departmentid"] = Request.QueryString["departmentid"];
            }

            populateOrganization(Session["userid"].ToString(), Session["departmentid"].ToString(), Session["nowtimes"].ToString());
            if (ds_all.Tables[0].Rows.Count == 1)
            {
                Session["departmentid"] = ds_all.Tables[0].Rows[0]["departmentid"].ToString();
            }
            dl_organization.DataSource = ds_all.Tables[0];
            dl_organization.DataBind();

            dl_department.DataSource = ds_all.Tables[1];
            dl_department.DataBind();

            //dd_log(3, Session["userid"].ToString(), Session["departmentid"].ToString(), Session["nowtimes"].ToString());
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
    /// 钉钉的程序步骤日志记录
    /// </summary>
    /// <param name="stepid">当前步骤id</param>
    /// <param name="userid">当前用户id</param>
    public void dd_log(int stepid, string userid, string departmentid, string nowtime)
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
        cmd.Parameters["@userid"].Value = userid;
        cmd.Parameters["@hostip"].Value = Request.UserHostAddress;
        cmd.Parameters["@now"].Value = Session["timelog"];
        cmd.Parameters["@step"].Value = stepid;
        cmd.Parameters["@departmentid"].Value = departmentid;
        cmd.Parameters["@rq"].Value = nowtime;
        cmd.Parameters["@agentId"].Value = "41866347";

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
    /// <summary>
    /// 获取部门数据以及简报数据
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="departmentid"></param>
    public void populateOrganization(string userid, string departmentid,string nowtime)
    {
        DataSet ds = new DataSet();

        SqlConnection conn = new SqlConnection(ConfigurationSettings.AppSettings["dingding"]);
        SqlCommand cmd = new SqlCommand("dd_briefshopsales_view", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        SqlParameter sqlParam = cmd.Parameters.Add("@userid", SqlDbType.VarChar, 100);
        sqlParam.Value = userid;         

        sqlParam = cmd.Parameters.Add("@departmentid", SqlDbType.VarChar, 100);
        sqlParam.Value = departmentid;

        sqlParam = cmd.Parameters.Add("@rq", SqlDbType.DateTime);
        sqlParam.Value = DateTime.Parse(nowtime);

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

        ds_all = ds;
        
    }
    /// <summary>
    /// 判断选择的departmentid是否有效 有效则返回大于0的整数
    /// </summary>
    protected int departmentisok()
    {
        int isok = 0;
        DataSet ds = new DataSet();
        SqlConnection conn = new SqlConnection(ConfigurationSettings.AppSettings["dingding"]);
        SqlCommand cmd = new SqlCommand("dd_checkpermission", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@userid", SqlDbType.VarChar, 100);
        cmd.Parameters.Add("@departmentid", SqlDbType.VarChar, 100);
        cmd.Parameters["@userid"].Value = Session["userid"].ToString();
        cmd.Parameters["@departmentid"].Value = Session["departmentid"].ToString();
        try
        {
            conn.Open();
            SqlDataAdapter a = new SqlDataAdapter(cmd);
            a.Fill(ds);
        }
        catch (Exception ex)
        {
            throw new Exception("Execption getting dailyreport by userid. " + ex.Message);
        }
        finally
        {
            conn.Close();
        }
        isok = int.Parse(ds.Tables[0].Rows[0]["isok"].ToString());
        return isok;
    }
    protected void btn_submit_Click(object sender, EventArgs e)
    {
        if (departmentisok() > 0)
        {
            if (Session["kind"].ToString() == "day")
            {
                Response.Redirect("dd_dailyreport.aspx");
            }
            else
            {
                Response.Redirect("dd_hourreport.aspx");
            }
        }
        else
        {
            Response.Write("<script>alert('请选择有效部门')</script>");
        }
    }
}