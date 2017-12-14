<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dingline.aspx.cs" Inherits="Recruit_dingline" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <script src="http://echarts.baidu.com/build/dist/echarts.js"></script>
    <title></title>
</head>
<body>
    <a href="http://www.datahunt.cn/dingding/dingding.aspx">返回</a><br />
    <div id="timecountdata"  style="height: 400px; border: 1px solid #ccc; padding: 10px;"></div>
    <script type="text/javascript" language="javascript">
        var industrytimeyears=[];  //时间
        var industry=[]; //客户
        var allindustrytimedata=[]; //所有年份数据
        var yearindustrytimedata=[]; //对应的年份数据
         <%for (int i = 0; i < times.Count; i++)
          {
              %>industrytimeyears[<%=i%>]='<%=times[i]%>';<%
          }
        %>
         <%for (int i = 0; i < kehu.Count; i++)
          {
              if (i % 2 == 0)
              {
                  %>industry[<%=i%>]='<%=kehu[i]%>';<%
              }
              else
              {
                  %>industry[<%=i%>]='\n<%=kehu[i]%>';<%
              }   
          }
        %>
         <%for (int i = 0; i < je.Count; i++)
          {
              %>allindustrytimedata[<%=i%>]='<%=je[i]%>';<%
          }
        %>
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
             var myChartTimeline = ec.init(document.getElementById('timecountdata'));

             //计算每个年份的每个行业的招聘量
             var industrydatas = [];
             for (var i = 0; i < industrytimeyears.length; i++) {
                 var k=0;
                 //一共15个行业 所以每次都有15条数据
                 for(var j=i*industry.length;j<(i+1)*industry.length;j++){
                     yearindustrytimedata[k]=allindustrytimedata[j];
                     k=k+1
                 }
                 industrydatas[i] = {
                     title: { text: industrytimeyears[i]+' 各店铺销售量' },
                     tooltip: { trigger: 'axis' },
                     xAxis: { data: industry },
                     yAxis: { type: 'value' },
                     series: [{ name: industrytimeyears[i], type: 'bar', data: yearindustrytimedata }]
                 };
                 yearindustrytimedata=[]; //进行下一次数据添加的时候 需要把上一次添加的数据清除掉
             }
             var option = {
                 timeline: {
                     data: industrytimeyears,
                     label: {
                         formatter: function (s) {
                             return s.slice(0, 7);
                         }
                     },
                     autoPlay: true,
                     playInterval: 1000
                 },
                 options: industrydatas
             };
             // 为echarts对象加载数据 
             myChartTimeline.setOption(option);
         }
         );
        </script>
</body>
</html>
