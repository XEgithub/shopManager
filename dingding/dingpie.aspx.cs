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

public partial class Recruit_dingpie : System.Web.UI.Page
{
    public List<string> kehu = new List<string>();
    public List<string> sl = new List<string>();
    protected void Page_Load(object sender, EventArgs e)
    {
        string userid = Request["userid"];
        if (!string.IsNullOrEmpty(userid))
        {
            string userjurisdiction = getuserjurisdiction(userid);
            string[] jurisdiction = userjurisdiction.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string where = "";
            for (int i = 0; i < jurisdiction.Length; i++)
            {
                if (i == jurisdiction.Length - 1)
                {
                    where += "'" + jurisdiction[i] + "'";
                }
                else
                {
                    where += "'" + jurisdiction[i] + "',";
                }
            }
            getkehuinfo(where);
        }
    }
    private string connection = "ERPFX";
    private SqlConnection getConnection()
    {
        string strconn = ConfigurationSettings.AppSettings["ERPFX"];
        SqlConnection conn = new SqlConnection(strconn);
        return conn;
    }
    /// <summary>
    /// 根据传入的userid返回对应的用户权限，目前一个用户只有一个权限信息返回
    /// </summary>
    /// <param name="userid">用户id</param>
    /// <returns></returns>
    public string getuserjurisdiction(string userid)
    {
        string userjurisdiction = "";
        string sql = "select jurisdiction from userjurisdiction where userid='" + userid+"'";
        SqlConnection conn = this.getConnection();
        try
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                userjurisdiction = dr["jurisdiction"].ToString();
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
        return userjurisdiction;
    }
    /// <summary>
    /// 填充每个店铺的销量占比
    /// </summary>
    /// <param name="where">店铺查询条件</param>
    public void getkehuinfo(string where)
    {
        string khdm = "";
        string sql = "select a.KHDM,sum(b.SL) as sl from LSXHD as a inner join LSXHDMX as b on a.DJBH=b.DJBH where a.RQ>='2016-01-01' and a.KHDM in (" + where + ") group by a.KHDM";
        SqlConnection conn = this.getConnection();
        try
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                kehu.Add(dr["KHDM"].ToString());
                sl.Add(dr["sl"].ToString());
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