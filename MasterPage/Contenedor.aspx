<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Contenedor.aspx.cs" Inherits="MasterPage.Contenedor" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="https://ajax.googleapis.com/ajax/libs/dojo/1.13.0/dojo/dojo.js"></script>
    <script type="application/javascript">
        function fnLocalStorage(val) {
            
            $('iframe:first').contents().find('#txtId').val('Prueba');
            localStorage.setItem('login', val );
        };
       
        </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        
          <iframe id="ifAngular" runat="server" width="100%" height="768px" >

          </iframe>
    
    </div>
    </form>
</body>
</html>
