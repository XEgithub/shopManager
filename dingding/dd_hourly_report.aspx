<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dd_hourly_report.aspx.cs" Inherits="Recruit_dingding" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <script type="text/javascript" src="http://g.alicdn.com/dingding/open-develop/0.7.0/dingtalk.js"></script>
    <script type="text/javascript" src="../Script/jquery.min.js"></script>
    <script type="text/javascript" src="https://g.alicdn.com/ilw/ding/0.9.2/scripts/dingtalk.js"></script>
    <script>
        dd.config({
            url: 'http://www.datahunt.cn/dingding/dd_hourly_report.aspx',
            agentId: '<%=agentId%>',
            corpId: '<%=CorpID%>',
            timeStamp: '<%=timestamp%>',
            nonceStr: '<%=nonceStr%>',
            signature: '<%=signatures%>',
            jsApiList: ['runtime.info', 'biz.contact.choose',
        'device.notification.confirm', 'device.notification.alert',
        'device.notification.prompt', 'biz.ding.post',
        'biz.util.openLink' ,'runtime.permission.requestAuthCode']
        });
        dd.ready(function () {
            dd.error(function(error){
                alert('dd error: ' + JSON.stringify(err));
            });
            dd.runtime.permission.requestAuthCode({
                corpId: '<%=CorpID%>',
                onSuccess: function (result) {
                    var access_token = '<%=access_token%>';
                    location.href = "http://www.datahunt.cn/dingding/dd_hourreport.aspx?usercode=" + result.code + "&access_token=" + access_token;
                },
                onFail: function (err) { alert(JSON.stringify(err)) }

            })
        });
    </script>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    </div>
    </form>
</body>
</html>
