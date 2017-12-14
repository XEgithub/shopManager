<%@ Page Language="C#" AutoEventWireup="true" CodeFile="test.aspx.cs" Inherits="dingding_test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <script language="javascript" type="text/javascript" src="../javascript/progressbar.js-master/dist/progressbar.js"></script>
    <script language="javascript" type="text/javascript" src="../js/myjs.js"></script>
    <title>测试界面</title>
</head>
<body>
    <div id="containers" style="margin:20px;width: 200px;height: 200px;position:relative;"></div>
    <div id="abc" style="margin:220px;width: 200px;height: 200px;position:relative;"></div>
    <div id="lines" style="margin:220px;width: 200px;height: 10px;position:relative;"></div>
    <script type="text/javascript" language="javascript">
        function myfun() {
            var bar = getBar(containers,'789000');
            bar.text.style.fontFamily = '"Raleway", Helvetica, sans-serif';
            bar.text.style.fontSize = '1rem';
            bar.text.style.color = 'red';

            bar.animate(0.35);  // Number from 0.0 to 1.0
            var bbc = getBar(abc,'124324');
            bbc.text.style.fontFamily = '"Raleway", Helvetica, sans-serif';
            bbc.text.style.fontSize = '2rem';
            bbc.animate(0.56);
            
            var line = getline(lines);
            line.animate(0.76);
        }
        window.onload = myfun;
   </script>
    <iframe src="../Recruit/echarts.aspx" width="700" height="500"></iframe>
</body>
</html>
