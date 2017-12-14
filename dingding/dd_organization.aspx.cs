using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public partial class Dingding_dd_organization : System.Web.UI.Page
{
    public DataSet ds_all;    

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["departmentid"] = Request.QueryString["departmentid"];
            if(Session["departmentid"] == null) { Session["departmentid"] = ""; }
            populateOrganization(Session["userid"].ToString(), Session["departmentid"].ToString());

            dl_organization.DataSource = ds_all.Tables[0];
            dl_organization.DataBind();

            dl_department.DataSource = ds_all.Tables[1];
            dl_department.DataBind();
        }
    }

    public void populateOrganization(string userid, string departmentid)
    {
        DataSet ds = new DataSet();

        SqlConnection conn = new SqlConnection(ConfigurationSettings.AppSettings["dingding"]);
        SqlCommand cmd = new SqlCommand("dd_orglist", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        SqlParameter sqlParam = cmd.Parameters.Add("@userid", SqlDbType.VarChar, 100);
        sqlParam.Value = @userid; // "01182620601252645"; 高峡

        sqlParam = cmd.Parameters.Add("@departmentid", SqlDbType.VarChar, 100);
        sqlParam.Value = @departmentid;

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