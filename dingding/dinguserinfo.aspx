<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dinguserinfo.aspx.cs" Inherits="Recruit_dinguserinfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <script type="text/javascript" src="../Script/jquery.min.js"></script>
    <script>
        $(function () {
            var userinfo = '<%=userinfo%>';
            if (userinfo == 'manager6376' | userinfo == '0233294438691039') {
                $('#reports').append("<a href='http://www.datahunt.cn/dingding/dingline.aspx?userid=" + userinfo + "'>报表1</a><br /><a href='http://www.datahunt.cn/dingding/dingpie.aspx?userid=" + userinfo + "'>报表2</a><br />");
            } else {
                $('#reports').append("<a href='http://www.datahunt.cn/dingding/dingline.aspx?userid=" + userinfo + "'>报表1</a><br />");
            }
	    var message = '<%=message%>';
            alert(message);
        });
    </script>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <label>部门信息</label>
        <table>
        <%for (int i = 0; i < deparmentname.Count; i++)
          {
              %><tr><td>部门名称：<%=deparmentname[i]%></td><td></td></tr><%
              for (int j = listLength[i]; j < listLength[i + 1]; j++)
              {
                  %><tr><td></td><td><lable><%=deparmentinfolist[j] %></lable></td></tr><%
              }
          } %>
        </table>
    </div>
    <br />
    <div id="admin"></div>
    <label>可查看报表</label>
    <div id="reports"></div>
    </form>
</body>
</html>
