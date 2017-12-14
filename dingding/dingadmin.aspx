<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dingadmin.aspx.cs" Inherits="Recruit_dingadmin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <script type="text/javascript" src="../Script/jquery.min.js"></script>
    <script>
    $(function () {
                var userinfo = '<%=userinfo%>';
            
            });
    </script>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <label>部门信息</label>
        <table>
        <%for (int i = 0; i < departmentnames.Count; i++)
          {
              %><tr><td>部门名称：<%=departmentnames[i]%></td><td></td><td></td></tr><%
              for (int j = listLength[i]; j < listLength[i + 1]; j++)
              {
                  %><tr><td></td><td><lable><%=usernames[j] %></lable></td><td><input type="checkbox" name="usercheck"/></td></tr><%
              }
          } %>
        </table>
    </div>
    <label>店铺：</label><asp:DropDownList ID="shoplist" runat="server"></asp:DropDownList><input type="button" value="授权" onclick=""/>
    </form>
   </body>
</html>
