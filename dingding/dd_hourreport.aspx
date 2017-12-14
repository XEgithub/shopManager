<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dd_hourreport.aspx.cs" Inherits="dingding_dd_hourreport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width,initial-scale=1.0,user-scalable=no,minimum-scale=1.0,maximum-scale=1.0">
    <meta name="format-detection" content="telephone=no">
    <meta name="format-detection" content="email=no">
    <meta name="mobile-web-app-capable" content="yes">
    <meta name="apple-mobile-web-app-capable" content="yes">    
    <link rel="stylesheet" type="text/css" href="../CSS/cqkozo_obg.css">
    <script language="javascript" type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <script language="javascript" type="text/javascript" src="../javascript/progressbar.js-master/dist/progressbar.js"></script>
    <script language="javascript" type="text/javascript" src="../js/myjs.js"></script>
    <script src="http://echarts.baidu.com/build/dist/echarts.js"></script>
    <title>欧碧倩小时销售报表</title>
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
          </div>
        <div class="kz-title-1">今日销售情况</div>
          <div class="kz-title">
                <div class="title"><asp:Label ID="lb_sumje" runat="server" Text=""></asp:Label></div>
                <div class="date"><asp:Label ID="lb_target" runat="server" Text="" ForeColor="Red"></asp:Label></div> 
                <div id="lines" style="margin:35px;width: 200px;height: 10px;position:relative;"></div>                
          </div>
        <div class="kz-title-1">小时销售</div>
         <div id="main" style="width:560px;height: 400px; border: 1px solid #ccc; padding: 10px;"></div>
        <script type="text/javascript" language="javascript">
            function myfun() {
                var line = getline(lines);
                line.animate(<%=Session["completion"]%>);
            }
            window.onload = myfun;
        </script>
        <script type="text/javascript" language="javascript">
            // 路径配置
            require.config({
                paths: {
                    echarts: 'http://echarts.baidu.com/build/dist'
                }
            });
            require(
             [
                 'echarts',
                 'echarts/chart/line', //按需加载图表关于线性图、折线图的部分
                 'echarts/chart/bar'
             ], function (ec) {
                 // 基于准备好的dom，初始化echarts图表
                 var myChart = ec.init(document.getElementById('main'));

                 var times = [];//时间段
                 var amount = [];//销售额
                 <%for (int i = 0; i <hours.Count; i++)
                  {
                      %>times[<%=i%>]='<%=hours[i]%>';<%
                      %>amount[<%=i%>]=<%=dayamount[i]%>;<%
                  }%>
                 var option = {
                     title : {
                         text: '欧碧倩小时销售报表',
                     },
                     tooltip : {
                         trigger: 'axis'
                     },
                     legend: {
                         data:['销售额']
                     },
                     toolbox: {
                         show : true,
                         feature : {
                             magicType : {show: true, type: ['line', 'bar']},
                             restore : {show: true},
                             saveAsImage : {show: true}
                         }
                     },
                     calculable : true,
                     xAxis : [
                         {
                             type : 'value'
                         }
                     ],
                     yAxis : [
                         {
                             type : 'category',
                             data : times
                         }
                     ],
                     series : [
                         {
                             name:'销售额',
                             type:'bar',
                             data:amount,
                             markPoint : {
                                 data : [
                                     {type : 'max', name: '最大值'},
                                     {type : 'min', name: '最小值'}
                                 ]
                             },
                             markLine : {
                                 data : [
                                     {type : 'average', name: '平均值'}
                                 ]
                             }
                         }
                     ]
                 };
                 // 为echarts对象加载数据 
                 myChart.setOption(option);
             }
             );
        </script>
    </section>
    </form>
</body>
</html>
