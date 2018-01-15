<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlPesquisaCliente.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlPesquisaCliente" %>

<style type="text/css">
    .auto-style1 {
        width: 16px;
    }
</style>

<script type="text/javascript">

    function obterNomeCliente(idCli) {
        if (idCli.value == "")
            return;

        var retorno = ctrlPesquisaCliente.ObterNomeCliente(idCli.value);

        if (retorno.error != null) {
            alert(retorno.error.description);
            FindControl("txtNumCliente", "input").value = "";
            FindControl("txtNomeCliente", "input").value = "";
            return;
        }

        FindControl("txtNomeCliente", "input").value = retorno.value;
    }

</script>

<table>
    <tr>
        <td>
            <asp:TextBox runat="server" ID="txtNumCliente" Width="50px" onkeypress="return soNumeros(event, true, true);"
                onblur="obterNomeCliente(this); return false;"></asp:TextBox>
        </td>
        <td>
            <asp:TextBox runat="server" ID="txtNomeCliente" Width="170px"></asp:TextBox>
        </td>
    </tr>
</table>



