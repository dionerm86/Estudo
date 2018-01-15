<%@ Page Title="Nova Mensagem (para Parceiro)" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadMensagemParceiro.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadMensagemParceiro" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
<script type="text/javascript">

    function setDest(idCli, nome, windowChild) {
        // Verifica se o destinatário já foi inserido
        var lstDest = FindControl("hdfDest", "input").value.split(',');        
        for (var i = 0; i < lstDest.length; i++)
            if (lstDest[i] == idCli) {
                windowChild.alert("Este destinatário já foi inserido.");
                return;
            }

            FindControl("hdfDest", "input").value += idCli + ",";

        nome = nome.indexOf(' ') > 0 ? nome.substr(0, nome.indexOf(' ')) : nome;
        
        FindControl("txtDest", "textarea").value += nome + ", ";
    }

    function enviarMsg() {
        if (!confirm('Enviar mensagem para os destinatários selecionados?'))
            return false;

        FindControl("btnEnviar", "input").disabled = true;
    
        var assunto = FindControl("txtAssunto", "input").value;
        var dest = FindControl("hdfDest", "input").value;
        var mensagem = FindControl("txtMensagem", "textarea").value;

        if (assunto == "") {
            alert("Informe o assunto da mensagem.");
            FindControl("btnEnviar", "input").disabled = false;
            return false;
        }

        if (dest == "") {
            alert("Informe os destinatários da mensagem.");
            FindControl("btnEnviar", "input").disabled = false;
            return false;
        }

        if (mensagem == "") {
            alert("Informe a descrição da mensagem.");
            FindControl("btnEnviar", "input").disabled = false;
            return false;
        }

        var response = CadMensagemParceiro.Enviar(assunto, dest, mensagem).value;

        if (response == null) {
            alert("Falha ao enviar mensagem. Ajax Erro.");
            FindControl("btnEnviar", "input").disabled = false;
            return false;
        }

        response = response.split('\t');

        if (response[0] == "Erro") {
            alert(response[1]);
            FindControl("btnEnviar", "input").disabled = false;
            return false;
        }
        else {
            limpar();
            alert(response[1]);
            FindControl("btnEnviar", "input").disabled = false;
            return false;
        }
    }

    function limparDest() {
        FindControl("hdfDest", "input").value = "";
        FindControl("txtDest", "textarea").value = "";
    }

    function limpar() {
        limparDest();
        FindControl("txtAssunto", "input").value = "";
        FindControl("txtMensagem", "textarea").value = "";
    }

</script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="left" class="dtvHeader">
                            <asp:Label ID="Label33" runat="server" Text="Assunto"></asp:Label>
                        </td>
                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                            <asp:TextBox ID="txtAssunto" runat="server" MaxLength="50" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" class="dtvHeader" nowrap="nowrap">
                            <asp:Label ID="Label1" runat="server" Text="Destinatários"></asp:Label>
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:TextBox ID="txtDest" runat="server" ReadOnly="true" Width="300px" Rows="2" 
                                TextMode="MultiLine"></asp:TextBox>
                            <a href="#" onclick="openWindow(500, 700, '../Utils/SelDestinatarioParceiro.aspx');">
                                <img src="../Images/pesquisar.gif" border="0px"></a>
                            <a href="#" onclick="limpar(); return false;">
                                <img src="../Images/ExcluirGrid.gif" border="0px"></a>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" class="dtvHeader" nowrap="nowrap">
                            <asp:Label ID="Label32" runat="server" Text="Mensagem"></asp:Label>
                        </td>
                        <td align="left" class="dtvAlternatingRow">
                            <asp:TextBox ID="txtMensagem" runat="server" MaxLength="500" Width="300px" 
                                Rows="5" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2">
                            <br />
                            <asp:Button ID="btnEnviar" runat="server" Text="Enviar" OnClientClick="return enviarMsg();" />
                        </td>
                    </tr>
                    </table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="hdfDest" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>

