<%@ Page Language="C#" CodeBehind="Default.aspx.cs" Inherits="DevelopmentWithADot.AspNetSpeechRecognition.Test.Default" %>
<%@ Register TagPrefix="web" Namespace="DevelopmentWithADot.AspNetSpeechRecognition" Assembly="DevelopmentWithADot.AspNetSpeechRecognition" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<script type="text/javascript">
	
		function onSpeechRecognized(result)
		{
			window.alert(result.Text);
		}

		function start()
		{
			document.getElementById('startBtn').setAttribute('disabled', 'disabled');
			document.getElementById('stopBtn').removeAttribute('disabled');
			document.getElementById('processor').startRecording();
		}

		function stop()
		{
			document.getElementById('startBtn').removeAttribute('disabled');
			document.getElementById('stopBtn').setAttribute('disabled', 'disabled');
			document.getElementById('processor').stopRecording();
		}

	</script>
</head>
<body>
	<form runat="server">
	<div>
		<web:SpeechRecognition runat="server" ID="processor" ClientIDMode="Static" Mode="Desktop" Culture="en-US" OnSpeechRecognized="OnSpeechRecognized" OnClientSpeechRecognized="onSpeechRecognized" _Choices="One,Two,Three" />
		<input type="button" id="startBtn" value="Start" onclick="start()" />
		<input type="button" id="stopBtn" value="Stop" disabled="disabled" onclick="stop()" />
	</div>
	</form>
</body>
</html>
