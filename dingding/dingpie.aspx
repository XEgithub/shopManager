<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dingpie.aspx.cs" Inherits="Recruit_dingpie" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <script src="http://echarts.baidu.com/build/dist/echarts.js"></script>
    <title></title>
</head>
<body>
    <a href="http://www.datahunt.cn/dingding/dingding.aspx">返回</a><br />
    <div id="keyworddata"  style="height: 400px; border: 1px solid #ccc; padding: 10px;"></div>
    <script type="text/javascript" language="javascript">
        var keywordPie=[] //pie的键值数组
        var keyword=[] //店铺
        var keycount=[] //销售量
            <%for (int i = 0; i < kehu.Count; i++)
              {
                  %>keyword[<%=i%>]='<%=kehu[i]%>';<%
                  %>keycount[<%=i%>]='<%=sl[i]%>';<%
              }
            %>
            for(var i=0;i<keycount.length;i++){
                keywordPie[i]={name:keyword[i],value:keycount[i]};
            }
            // 路径配置
            require.config({
                paths: {
                    echarts: 'http://echarts.baidu.com/build/dist'
                }
            });
            // 使用
            require(
                [
                    'echarts',
                    'echarts/chart/pie', // 使用柱状图就加载bar模块，按需加载
                    'echarts/chart/funnel'
                ],
                function (ec) {
                    // 基于准备好的dom，初始化echarts图表
                    var myChartPie = ec.init(document.getElementById('keyworddata'));

                    var option = {
                        title: {
                            text: '2016各店铺销售占比情况',
                            x: 'center'
                        },
                        tooltip: {
                            trigger: 'item',
                            formatter: "{a} <br/>{b} : {c} ({d}%)"
                        },
                        legend: {
                            orient: 'vertical',
                            x: 'left',
                            data: keyword
                        },
                        toolbox: {
                            show: true,
                            feature: {
                                mark: { show: true },
                                dataView: { show: true, readOnly: false },
                                restore: { show: true },
                                saveAsImage: { show: true }
                            }
                        },
                        calculable: true,
                        series: [
                            {
                                name: '店铺',
                                type: 'pie',
                                radius: '55%',
                                center: ['50%', '60%'],
                                data: keywordPie
                            }
                        ]
                    };
                    // 为echarts对象加载数据 
                    myChartPie.setOption(option);
                }
            );
    </script>
</body>
</html>
