<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dd_dailyreport.aspx.cs" Inherits="dd_dailyreport" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="System.Web.UI.WebControls" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>欧碧倩日报表</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width,initial-scale=1.0,user-scalable=no,minimum-scale=1.0,maximum-scale=1.0">
    <meta name="format-detection" content="telephone=no">
    <meta name="format-detection" content="email=no">
    <meta name="mobile-web-app-capable" content="yes">
    <meta name="apple-mobile-web-app-capable" content="yes">    
    <link rel="stylesheet" type="text/css" href="../CSS/cqkozo_obg.css">
    <script language="javascript" type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <script language="javascript" type="text/javascript" src="../javascript/progressbar.js-master/dist/progressbar.js"></script>
    <script language="javascript" type="text/javascript" src="../js/jquery-2.2.1.min.js"></script>
    <script language="javascript" type="text/javascript" src="../js/myjs.js"></script>
    <script type="text/javascript" language="javascript">
        function myfun() {
            var bar1 = getBar(today_addedvipmoneypercentbar,<%=Session["today_addedvipmoney"]%>);
            bar1.animate(<%=Session["today_addedvipmoneypercent"]%>);
            var bar2 = getBar(today_vipmoneypercentbar,<%=Session["today_vipmoeny"]%>);
            bar2.animate(<%=Session["today_vipmoneypercent"]%>);
            var bar3 = getBar(month_vipmoneypercentbar,<%=Session["month_vipmoney"]%>);
            bar3.animate(<%=Session["month_vipmoneypercent"]%>);
        }
        /*用window.onload调用myfun()*/
        window.onload = myfun;//不要括号
    </script>
</head>
<body>
   <form id="form1" runat="server">
    <header class="kz-header">
        <div class="kz-title">
              <div class="title"><asp:Label ID="deparmentname" runat="server" Text="" ForeColor="White"></asp:Label> </div>
              <div class="date"><asp:Label ID="nowtimes" runat="server" Text="" ForeColor="White"></asp:Label></div>                          
        </div>
    </header>
    
    <section class="kz-section">
         <div>

             <ul class="kz-list">                   
                   <li class="item1"> <asp:Button ID="btn_changedepartment" runat="server" Text="选择店铺"  Width="100px" OnClick="btn_changedepartment_Click" /></li>
                   <li class="item2"> <input runat="server" id="gettimes" class="Wdate" style="width:150px" type="text" onClick="WdatePicker()"></li>
                   <li class="item3"> <asp:Button ID="btn_changetime" runat="server" Text="选择时间" Width="100px"  OnClick="btn_changetime_Click" /></li>                
             </ul>

          <asp:DataList ID="dl_todaysales" runat="server" RepeatDirection="Vertical" Width="100%">
                    <HeaderTemplate>
                        <div class="kz-title-1">
                          <div class="kz-title">
                               <div class="title">分类</div>
                               <div class="date">数字</div>                
                          </div>
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>                             
                                <div class="kz-title">
                                   <div class="title"><%#Eval("分类") %></div>
                                   <div class="date"><%#Eval("数字") %></div>                                   
                               </div>
                    </ItemTemplate>                        
          </asp:DataList>

            <ul class="kz-list">                   
                <li class="item1"> <asp:Label ID="lb_salesprogress" runat="server" Text=""></asp:Label></li>
                <li class="item2"> </li>
                <li class="item3"> <asp:Label ID="lb_timeprogress" runat="server" Text=""></asp:Label></li>                
            </ul>            
            <ul class="kz-list">                   
                <li class="item1"> 销售进度</li>
                <li class="item2"> </li>
                <li class="item3"> 时间进度</li>                
            </ul>

            <asp:DataList ID="dl_sales" runat="server" RepeatDirection="Vertical" Width="100%">
                    <HeaderTemplate>
                          <div class="kz-title-1">
                                <ul class="kz-list">   
                                   <li class="item1">分类</li>
                                   <li class="item2">数量</li>
                                   <li class="item3">金额</li>
                                </ul>
                          </div>
                    </HeaderTemplate>
                    <ItemTemplate>                             
                                <ul class="kz-list">   
                                   <li class="item1"><%#Eval("分类") %></li>
                                   <li class="item2"><%#Eval("数量") %></li>
                                   <li class="item3"><%#Eval("金额") %></li>
                                </ul>                                
                    </ItemTemplate>                        
          </asp:DataList>

           <asp:DataList ID="dl_shangpinsales" runat="server" RepeatDirection="Vertical" Width="100%">
                    <HeaderTemplate>
                          <div class="kz-title-1">
                                <ul class="kz-list">   
                                   <li class="item1">分类</li>
                                   <li class="item2">数量</li>
                                   <li class="item3">金额</li>
                                </ul>
                          </div>
                    </HeaderTemplate>
                    <ItemTemplate>                             
                                <ul class="kz-list">   
                                   <li class="item1"><%#Eval("分类") %></li>
                                   <li class="item2"><%#Eval("数量") %></li>
                                   <li class="item3"><%#Eval("金额") %></li>
                                </ul>                                
                    </ItemTemplate>                        
          </asp:DataList>

          <asp:DataList ID="dl_js" runat="server" RepeatDirection="Vertical" Width="100%">
                    <HeaderTemplate>
                        <div class="kz-title-1">
                          <div class="kz-title">
                               <div class="title">结算方式</div>
                               <div class="date">结算金额</div>                
                          </div>
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>                             
                                <div class="kz-title">
                                   <div class="title"><%#Eval("结算方式") %></div>
                                   <div class="date"><%#Eval("结算金额") %></div>                                   
                               </div>
                    </ItemTemplate>                        
          </asp:DataList>

          <div class="kz-title-1">连带率</div>
          <div class="kz-title">
                <div class="title">本月连带率</div>
                <div class="date"><asp:Label ID="lb_monthapr" runat="server" Text=""></asp:Label></div>                
          </div>

          <asp:DataList ID="dl_liandailv" runat="server" RepeatDirection="Vertical" Width="100%">
                    <HeaderTemplate>
                        <div class="kz-title-1">
                          <div class="kz-title">
                               <div class="title">店员</div>
                               <div class="date">连带率</div>                
                          </div>
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>                             
                                <div class="kz-title">
                                   <div class="title"><%#Eval("店员") %></div>
                                   <div class="date"><%#Eval("本月连带率") %></div>                                   
                               </div>
                    </ItemTemplate>                        
          </asp:DataList>


          <div class="kz-title-1">VIP</div>
          <div class="kz-title">
                <div class="title">今日新增VIP会员</div>
                <div class="date"><asp:Label ID="lb_addedvip" runat="server" Text=""></asp:Label></div>                
          </div>

          <ul class="kz-list">   
                                   <li class="item1">新增VIP消费金额占比</li>
                                   <li class="item2">今日vip消费金额占比</li>
                                   <li class="item3">本月VIP消费金额占比</li>
          </ul>         
          <ul class="kz-list">   
                                   <li class="item1"><asp:Label ID="today_addedvipmoneypercent" runat="server" Text=""></asp:Label></li>
                                   <li class="item2"><asp:Label ID="today_vipmoneypercent" runat="server" Text=""></asp:Label></li>
                                   <li class="item3"><asp:Label ID="month_vipmoneypercent" runat="server" Text=""></asp:Label></li>
          </ul>      
          <ul class="kz-list">   
              <div style="width:380px;height:105px">
                  <div id="today_addedvipmoneypercentbar" style="margin:5px;width: 108px;height: 108px;position:relative;float:left"></div>
                  <div id="today_vipmoneypercentbar" style="margin:5px;width: 108px;height: 108px;position:relative;float:left"></div>
                  <div id="month_vipmoneypercentbar" style="margin:5px;width: 108px;height: 108px;position:relative;float:left"></div>
              </div>
                                   
          </ul>      
                     
           
           <asp:DataList ID="dl_dianyuan" runat="server" RepeatDirection="Vertical" Width="100%">
                    <HeaderTemplate>
                          <div class="kz-title-1">
                                <ul class="kz-list-6">   
                                   <li class="item1">店员</li>
                                   <li class="item3">今日</li>
                                   <li class="item1">本月</li>
                                   <li class="item2">目标</li>
                                </ul>
                          </div>
                    </HeaderTemplate>
                    <ItemTemplate>                             
                                <ul class="kz-list-6">   
                                   <li class="item1"><%#Eval("店员") %></li>                                
                                   <li class="item3"><%#Eval("今日销售") %></li>
                                    <li class="item1"><%#Eval("本月金额") %></li>
                                   <li class="item2"><%#Eval("本月目标") %></li>                                
                                </ul>                                
                    </ItemTemplate>                        
          </asp:DataList>
          <div id="mywork" runat="server">
          <div class="kz-title-1">奖励</div>    
           <ul class="kz-list">                   
                <li class="item1"> 今日连单奖</li>
                <li class="item2"> </li>
                <li class="item1"><asp:TextBox ID="tb_liandanjiang" runat="server" Width="120px" class="Wdate"></asp:TextBox> </li>                
            </ul>    
            
            <ul class="kz-list">                   
                <li class="item1"> 今日突破奖</li>
                <li class="item2"> </li>
                <li class="item1"><asp:TextBox ID="tb_tupojiang" runat="server" Width="120px" class="Wdate"></asp:TextBox> </li>                
            </ul>
            
            <div class="kz-title-1">促销活动与竞品动态</div>    
             <ul class="kz-list">                   
                <li class="item1"> 商场店铺促销活动</li>
                <li class="item2"> </li>
                <li class="item3"> </li>                
            </ul>            

        <div class="kz-title">
            <div style="height:100px;padding-top:10px">
              <asp:TextBox ID="tb_chuxiao" runat="server" TextMode="MultiLine" class="Wdate" Width="200px" Height="80px"></asp:TextBox>              
            </div>
        </div>

           <div class="kz-title-1">竞争动态</div>
             <asp:DataList ID="dl_competition" runat="server" RepeatDirection="Vertical" Width="100%" DataKeyField="cid" OnDeleteCommand="dlUserlist_DeleteCommand" >
                    <HeaderTemplate>
                          <div class="kz-title-1">
                                <ul class="kz-list">                   
                                    <li class="item1"> 竞争品类</li>
                                    <li class="item2"> 销售金额</li>
                                    <li class="item3">删除</li>               
                                </ul>
                          </div>
                    </HeaderTemplate>
                    <ItemTemplate>                             
                                <ul class="kz-list">   
                                    <li class="item1"><%#Eval("竞争品牌") %></li>
                                    <li class="item2"><%#Eval("销售金额") %></li>
                                    <li class="item3"><asp:LinkButton ID="linkDelete" runat="server" CommandName="Delete"><span onclick="return confirm('确实删除该条记录吗？')";>删除</span></asp:LinkButton></li> 
                                </ul>                                
                    </ItemTemplate>                        
          </asp:DataList>
          <div class="kz-title-1">竞争数据填写</div>
             <ul class="kz-list">
                <li class="item1"><asp:TextBox id="tb_jinzhenpinlei" runat="server" class="Wdate" value=""></asp:TextBox></li>
                <li class="item2"><asp:TextBox id="tb_xiaoshoujine" runat="server" class="Wdate" value="0"></asp:TextBox></li>
                <li class="item3"></li>    
             </ul>   
            <div class="kz-title-1">填写人</div>    
             <ul class="kz-list">                   
                <li class="item1"><asp:Label ID="lb_Manualdatauser" runat="server" Text=""></asp:Label></li>
                <li class="item2"> </li>
                <li class="item3"> </li>                
            </ul> 
          <ul class="kz-list">                   
              <div class="title"><asp:Button id="tb_submit" runat="server" Width="100px" Text="提交" OnClick="tb_submit_Click"></asp:Button></div>
          </ul>
              </div>
          </div>
    </section>
    </form>
</body>
</html>
