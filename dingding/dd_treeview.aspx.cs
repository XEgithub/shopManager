using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public partial class dd_treeview : System.Web.UI.Page
{
    private DataTable dts = new DataTable();
    public string userid = "";
    public string times = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        //04512354156505 02332944386604
        if (!string.IsNullOrEmpty(Session["userid"].ToString()))
        {
            userid = Session["userid"].ToString();
            times = Session["nowtimes"].ToString();
        }
        if (string.IsNullOrEmpty(times))
        {
            times = DateTime.Now.ToString("yyyy-MM-dd");
        }
        if (!IsPostBack)
        {
            CreateTable(userid);
            //CreateNode();
        }
    }
    /// <summary>
    /// 获得树节点数据
    /// </summary>
    /// <param name="userid">用户id</param>
    /// <returns></returns>
    public void CreateTable(string userid)
    {
        DataSet ds = new DataSet();

        SqlConnection conn = new SqlConnection(ConfigurationSettings.AppSettings["dingding"]);
        SqlCommand cmd = new SqlCommand("dd_departmentlist", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        SqlParameter sqlParam = cmd.Parameters.Add("@userid", SqlDbType.VarChar, 100);
        sqlParam.Value = userid;

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
        dts = ds.Tables[0];
        
        //添加根节点
        if (ds.Tables[1].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
            {
                //声明节点
                TreeNode mynode = new TreeNode();
                mynode.Text = ds.Tables[1].Rows[i]["departmentname"].ToString();
                mynode.Value = ds.Tables[1].Rows[i]["departmentid"].ToString();
                TreeView1.Nodes.Add(mynode);

                AddTree(ds.Tables[1].Rows[i]["departmentid"].ToString(), (TreeNode)null);
            }
        }
    }
    /// <summary>
    /// 递归添加树的节点 
    /// </summary>
    /// <param name="parentid">参数ParentID为树的最大父结点</param>
    /// <param name="pNode"></param>
    public void AddTree(string parentid,TreeNode pNode)
    {
        DataView DataView_Tree = new DataView(dts);

      //过滤ParentID,得到当前的所有子节点 ParentID为父节点ID
      DataView_Tree.RowFilter =  "[parentid] = " + parentid;
      //循环递归
      foreach(DataRowView Row in DataView_Tree)
      {
        //声明节点
        TreeNode Node=new TreeNode() ;
        //绑定超级链接
        //Node.NavigateUrl = "List.aspx?Item_ID="+Row["Item_ID"].ToString();

        //开始递归
        if(pNode == null)
        {

          //添加根节点
          Node.Text = Row["departmentname"].ToString();
          Node.Value = Row["departmentid"].ToString();
          TreeView1.Nodes.Add(Node);
          Node.Expanded=true; //节点状态展开
          AddTree(Row["departmentid"].ToString(), Node);    //再次递归
        }

        else
        {  
          //添加当前节点的子节点
          Node.Text = Row["departmentname"].ToString();
          Node.Value = Row["departmentid"].ToString();
          pNode.ChildNodes.Add(Node);
          Node.Expanded = true; //节点状态展开
          AddTree(Row["departmentid"].ToString(),Node);     //再次递归
        }
      }
      }

    protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
    {
        Session["departmentid"] = TreeView1.SelectedNode.Value;
        if (Request["kind"] == "day")
        {
            Response.Redirect("dd_dailyreport.aspx");
        }
        else
        {
            Response.Redirect("dd_hourreport.aspx");
        }
        
    }
}
