<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dd_treeview.aspx.cs" Inherits="dd_treeview" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>选择部门</title>
        <link rel="stylesheet" type="text/css" href="../CSS/cqkozo_obg.css">
</head>
<body>
    <form id="form1" runat="server">    
     <section class="kz-section">
         <div>
         <div class="kz-treeview">
              <asp:TreeView ID="TreeView1" runat="server" ShowLines="True" onselectednodechanged="TreeView1_SelectedNodeChanged" >
              </asp:TreeView>    
        </div>
        </div>
    </section>
    </form>
</body>
</html>
