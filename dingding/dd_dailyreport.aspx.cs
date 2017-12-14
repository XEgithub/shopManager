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

public partial class dd_dailyreport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {       
        if (!Page.IsPostBack)
        {
             //if (Session["userid"] == null) 
             //{
             //     string usercode = Request.QueryString["usercode"];
             //     string access_token = Request.QueryString["access_token"];
             //   if (!string.IsNullOrEmpty(usercode) && !string.IsNullOrEmpty(access_token))
             //  {
             //      Session["userid"] =getuserid(usercode, access_token);                
             //      Session["departmentid"]="";
             //      Session["nowtimes"] =DateTime.Now.ToString("yyyy-MM-dd");
             //  }
            //Session["userid"] = "024164092429274526";//04512113551383 李雪梅 056859204529950662 02332944386604 周勇 03075755501182 赵弘 024164092429274526 王智谍
            //Session["departmentid"] = "";
            //Session["nowtimes"] = DateTime.Now.ToString("yyyy-MM-dd");
             //}

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
    /// 钉钉的程序步骤日志记录
    /// </summary>
    /// <param name="stepid">当前步骤id</param>
    /// <param name="userid">当前用户id</param>
    public void dd_log(int stepid,string userid, string departmentid, string nowtime)
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
    /// 选择时间
    /// </summary>
    protected void btn_changetime_Click(object sender, EventArgs e)
    {
        string gettime = Request.Form["gettimes"].ToString();
        Session["nowtimes"] = gettime;
        populateData(Session["userid"].ToString(), Session["departmentid"].ToString(), Session["nowtimes"].ToString());
    }
    /// <summary>
    /// 删除竞争数据
    /// </summary>
    protected void dlUserlist_DeleteCommand(object source, DataListCommandEventArgs e)
    {
        string cid=dl_competition.DataKeys[e.Item.ItemIndex].ToString();
        Manualdata(int.Parse(cid), "", 0);
        getManualdata();
        tb_jinzhenpinlei.Focus();
    }
    /// <summary>
    /// 添加手工数据到数据库
    /// </summary>
    protected void tb_submit_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(tb_xiaoshoujine.Text))
        {
            int i = 0;
            if (int.TryParse(tb_xiaoshoujine.Text,out i))
            {
                Manualdata(0, tb_jinzhenpinlei.Text, int.Parse(tb_xiaoshoujine.Text));
            }
            else
            {
                Response.Write("<script>alert('竞争品牌金额请填写为有效整数')</script>");
            }
        }
        else
        {
            Manualdata(0, tb_jinzhenpinlei.Text,0); 
        }
        getManualdata();
        tb_jinzhenpinlei.Focus();
    }
    /// <summary>
    /// 去到选择部门的页面
    /// </summary>
    protected void btn_changedepartment_Click(object sender, EventArgs e)
    {
        Session["kind"] = "day";
       //Response.Redirect("dd_organization.aspx?userid=" + Session["userid"].ToString() + "&kind=day");       
        Response.Redirect("dd_orgnavigate.aspx");       
    }
    /// <summary>
    /// 手工部分数据显示
    /// </summary>
    public void getManualdata()
    {
        DataSet ds = new DataSet();
        SqlConnection conn = new SqlConnection(ConfigurationSettings.AppSettings["dingding"]);
        SqlCommand cmd = new SqlCommand("dd_Manualdata", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@departmentid", SqlDbType.VarChar, 100);
        cmd.Parameters.Add("@rq", SqlDbType.DateTime);
        cmd.Parameters["@departmentid"].Value = Session["departmentid"].ToString();
        if (!string.IsNullOrEmpty(Session["nowtimes"].ToString()))
        {
            cmd.Parameters["@rq"].Value = DateTime.Parse(Session["nowtimes"].ToString());
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
            throw new Exception("Execption getting dailyreport by userid. " + ex.Message);
        }
        finally
        {
            conn.Close();
        }
        /**************************************
         * Populate the data on the page
         * ************************************/
        //商场活动
        if (ds.Tables[0].Rows.Count > 0)
        {
            tb_chuxiao.Text = ds.Tables[0].Rows[0]["note"].ToString();
        }
        else
        {
            tb_chuxiao.Text="";
        }
         
        //连单奖 突破奖
        if (ds.Tables[1].Rows.Count > 0)
        {
            if (!string.IsNullOrEmpty(ds.Tables[1].Rows[0]["serial"].ToString()))
            {
                tb_liandanjiang.Text = ds.Tables[1].Rows[0]["serial"].ToString();
            }
            else
            {
                tb_liandanjiang.Text = "0";
            }
            if (!string.IsNullOrEmpty(ds.Tables[1].Rows[0]["breakthroughs"].ToString()))
            {
                tb_tupojiang.Text = ds.Tables[1].Rows[0]["breakthroughs"].ToString();
            }
            else
            {
                tb_tupojiang.Text = "0";
            }
        }
        else
        {
            tb_liandanjiang.Text = "0";
            tb_tupojiang.Text = "0";
        }
        
        //竞争品牌信息
        dl_competition.DataSource = ds.Tables[2];
        dl_competition.DataBind();
        //手工数据输入人
        if (ds.Tables[3].Rows.Count > 0)
        {
            lb_Manualdatauser.Text = ds.Tables[3].Rows[0]["username"].ToString();
        }
        else
        {
            lb_Manualdatauser.Text = "";
        }
       //手工数据输入部门制0
        tb_jinzhenpinlei.Text = "";
        tb_xiaoshoujine.Text = "0";
    }
    /// <summary>
    /// 修改手工数据
    /// </summary>
    /// <param name="cid">竞争品牌id</param>
    public void Manualdata(int cid,string name,int moeny)
    {
        SqlConnection conn = new SqlConnection(ConfigurationSettings.AppSettings["dingding"]);
        SqlCommand cmd = new SqlCommand("dd_updateManualdata", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@userid", SqlDbType.VarChar, 100);
        cmd.Parameters.Add("@departmentid", SqlDbType.VarChar, 100);
        cmd.Parameters.Add("@rq", SqlDbType.DateTime);
        cmd.Parameters.Add("@note", SqlDbType.VarChar, 500);
        cmd.Parameters.Add("@serial", SqlDbType.Int);
        cmd.Parameters.Add("@breakthroughs", SqlDbType.Int);
        cmd.Parameters.Add("@name", SqlDbType.VarChar, 100);
        cmd.Parameters.Add("@money", SqlDbType.Int);
        cmd.Parameters.Add("@cid", SqlDbType.Int);
        cmd.Parameters["@userid"].Value = Session["userid"].ToString();
        cmd.Parameters["@departmentid"].Value = Session["departmentid"].ToString();
        cmd.Parameters["@rq"].Value = DateTime.Parse(Session["nowtimes"].ToString());
        cmd.Parameters["@note"].Value = tb_chuxiao.Text;
        cmd.Parameters["@serial"].Value = int.Parse(tb_liandanjiang.Text);
        cmd.Parameters["@breakthroughs"].Value =int.Parse(tb_tupojiang.Text);
        cmd.Parameters["@name"].Value = name;
        cmd.Parameters["@money"].Value =moeny;
        cmd.Parameters["@cid"].Value = cid;
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
    public void populateData(string userid, string deparmentid, string nowtime)
    {
        //dd_log(2, userid,deparmentid,nowtime);
        /**************************************
         * Get all the data from the database 
         * ************************************/
        DataSet ds = new DataSet();
        SqlConnection conn = new SqlConnection(ConfigurationSettings.AppSettings["dingding"]);
        SqlCommand cmd = new SqlCommand("dd_dailyreport", conn);
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
        }
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

        /**************************************
         * Populate the data on the page
         * ************************************/
        //今日销售量 以及 今日销售金额
        dl_todaysales.DataSource = ds.Tables[0];
        dl_todaysales.DataBind();

        lb_salesprogress.Text = ds.Tables[1].Rows[0]["month_progress"].ToString();
        lb_timeprogress.Text = ds.Tables[1].Rows[0]["day_progress"].ToString();

        //本月销售量 以及 本月销售金额
        dl_sales.DataSource = ds.Tables[2];
        dl_sales.DataBind();

        //商品类别
        dl_shangpinsales.DataSource = ds.Tables[3];
        dl_shangpinsales.DataBind();

        //结算方式
        dl_js.DataSource = ds.Tables[4];
        dl_js.DataBind();

        //连带率
        lb_monthapr.Text = ds.Tables[5].Rows[0]["apr"].ToString();
        //个人连带率
        if (ds.Tables[9].Rows[0]["isstore"].ToString()!="0")
        {
            dl_liandailv.Visible = true;
            dl_liandailv.DataSource = ds.Tables[7];
            dl_liandailv.DataBind();
        }
        else
        {
            dl_liandailv.Visible = false;
        }

        //VIP数据        
        lb_addedvip.Text = ds.Tables[6].Rows[0]["today_addedvip"].ToString();
        //vip数据 占比部分
        today_addedvipmoneypercent.Text = ds.Tables[6].Rows[0]["today_addedvipmoneypercent_varchar"].ToString();
        today_vipmoneypercent.Text = ds.Tables[6].Rows[0]["today_vipmoenypercent_varchar"].ToString();
        month_vipmoneypercent.Text = ds.Tables[6].Rows[0]["month_vipmoenypercent_varchar"].ToString();
        Session["today_addedvipmoneypercent"] = ds.Tables[6].Rows[0]["today_addedvipmoneypercent"].ToString();
        Session["today_vipmoneypercent"] = ds.Tables[6].Rows[0]["today_vipmoenypercent"].ToString();
        Session["month_vipmoneypercent"] = ds.Tables[6].Rows[0]["month_vipmoenypercent"].ToString();
        //vip数据金额部分
        if (!string.IsNullOrEmpty(ds.Tables[6].Rows[0]["today_addedvipmoney"].ToString()))
        {
            Session["today_addedvipmoney"] = ds.Tables[6].Rows[0]["today_addedvipmoney"].ToString();
        }
        else
        {
            Session["today_addedvipmoney"] = "0";
        }
        if (!string.IsNullOrEmpty(ds.Tables[6].Rows[0]["today_vipmoeny"].ToString()))
        {
            Session["today_vipmoeny"] = ds.Tables[6].Rows[0]["today_vipmoeny"].ToString();
        }
        else
        {
            Session["today_vipmoeny"] = "0";
        }
        if (!string.IsNullOrEmpty(ds.Tables[6].Rows[0]["month_vipmoney"].ToString()))
        {
            Session["month_vipmoney"] = ds.Tables[6].Rows[0]["month_vipmoney"].ToString();
        }
        else
        {
            Session["month_vipmoney"] = "0";
        }

        //个人销售
        if (ds.Tables[9].Rows[0]["isstore"].ToString() != "0")
        {
            dl_dianyuan.Visible = true;
            dl_dianyuan.DataSource = ds.Tables[7];
            dl_dianyuan.DataBind();
        }
        else
        {
            dl_dianyuan.Visible = false;
        }

        //店铺名称
        deparmentname.Text = ds.Tables[8].Rows[0]["departmentname"].ToString();
        Session["departmentid"] = ds.Tables[8].Rows[0]["departmentid"].ToString();
        nowtimes.Text = Session["nowtimes"].ToString();

        //手工数据
        if (ds.Tables[9].Rows[0]["isstore"].ToString() != "0")
        {
            mywork.Visible = true;
            getManualdata();
        }
        else
        {
            mywork.Visible = false; 
        }
        //时间输入框值
        gettimes.Value = nowtime;
        //dd_log(3, userid,deparmentid,nowtime);
    }
}