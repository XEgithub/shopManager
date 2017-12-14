<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dd_organization.aspx.cs" Inherits="Dingding_dd_organization" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="../CSS/cqkozo_obg.css">
    <title>选择部门</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width,initial-scale=1.0,user-scalable=no,minimum-scale=1.0,maximum-scale=1.0">
    <meta name="format-detection" content="telephone=no">
    <meta name="format-detection" content="email=no">
    <meta name="mobile-web-app-capable" content="yes">
    <meta name="apple-mobile-web-app-capable" content="yes">    
</head>
<body>
    <form id="form1" runat="server">
      <header class="kz-header">
        <div class="kz-title">
              <div class="title">选择部门</div>
              <div class="date"><asp:Button ID="btn_submit" runat="server" Text="返回" Width="100px" onclick="btn_submit_Click"></asp:Button>
              </div>                          
        </div>
     </header>

        <section class="kz-section">
         <div>              
              <ul class="org-list">                   
                <li class="item1">
                            <asp:DataList ID="dl_organization" runat="server" RepeatDirection="Horizontal" >                    
                              <ItemTemplate>                                                             
                                   <a href ="dd_organization.aspx?departmentid=<%#Eval("departmentid") %>">
                                    <%#Eval("departmentname") %>
                                   </a>
                                   >
                           </ItemTemplate>                        
                       </asp:DataList>
                </li>                
              </ul>

           <asp:DataList ID="dl_department" runat="server" RepeatDirection="Vertical" Width="100%">
                    <HeaderTemplate>
                          <div class="kz-title-1">
                                <ul class="kz-list">   
                                   <li class="item1">部门列表</li>                                   
                                </ul>
                          </div>
                    </HeaderTemplate>
                    <ItemTemplate>                             
                                <ul class="kz-list">   
                                   <li class="item1">
                                        <a href ="dd_organization.aspx?departmentid=<%#Eval("departmentid") %>">
                                              <%#Eval("departmentname") %>
                                        </a>
                                   </li>                                   
                                </ul>                                
                    </ItemTemplate>                        
          </asp:DataList>                           
           
              </div>
    </section>
    </form>
</body>
</html>
