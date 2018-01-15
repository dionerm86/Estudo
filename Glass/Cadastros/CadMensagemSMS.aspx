<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadMensagemSMS.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadMensagemSMS" Title="Nova Mensagem SMS" %>

<%@ Register src="../Controls/ctrlLimiteTexto.ascx" tagname="ctrlLimiteTexto" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
<script type="text/javascript">

    function calculaCusto()
    {
        var lblCusto = FindControl("lblCustoSMS", "span");
        var totalDest = FindControl("txtDest", "textarea").value.split(',').length;
        
        if(totalDest > 0)
            totalDest--;
            
        if(FindControl("hdfTotalClientes", "input").value == "-1")
            totalDest = MetodosAjax.GetCountCliente().value;
        
        //lblCusto.innerHtml = "Custo Estimado: R$ " + (totalDest * 0.15).toFixed(2); 
        lblCusto.innerText = "Custo Estimado: R$ " + (totalDest * 0.15).toFixed(2);
    }

    function setDest(idFunc, nome, windowChild) {
        // Verifica se o destinatário já foi inserido
        var lstDest = FindControl("hdfDest", "input").value.split(',');        
        for (var i = 0; i < lstDest.length; i++)
            if (lstDest[i] == idFunc) {
                windowChild.alert("Este destinatário já foi inserido.");
                return;
            }

        FindControl("hdfDest", "input").value += idFunc + ",";

        if(idFunc == "-1")
            FindControl("hdfTotalClientes", "input").value = "-1";
        else
            FindControl("hdfTotalClientes", "input").value = "0";

        nome = nome.indexOf(' ') > 0 ? nome.substr(0, nome.indexOf(' ')) : nome;
        
        FindControl("txtDest", "textarea").value += nome + ", ";
        
        calculaCusto();
    }

    function enviarMsg() {
        if (!confirm('Enviar mensagem para os destinatários selecionados?'))
            return false;

        FindControl("btnEnviar", "input").disabled = true;
    
        var dest = FindControl("hdfDest", "input").value;
        var mensagem = FindControl("txtMensagem", "textarea").value;

        if (dest == "") {
            alert("Informe o(s) destinatário(s) da mensagem.");
            FindControl("btnEnviar", "input").disabled = false;
            return false;
        }

        if (mensagem == "") {
            alert("Informe a descrição da mensagem.");
            FindControl("btnEnviar", "input").disabled = false;
            return false;
        }

        var response = MetodosAjax.EnviarSMS("", dest, mensagem).value;

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
        FindControl("hdfTotalClientes", "input").value = "0";
    }

    function limpar() {
        limparDest();
        FindControl("txtMensagem", "textarea").value = "";
        FindControl("hdfTotalClientes", "input").value = "0";
        calculaCusto();
    }

</script>

    <div style="margin:0 auto; width:500px;">
        <div>
            <table>
                <tr>
                    <td align="center">
                        <table>
                            <tr>
                                <td align="left" class="dtvHeader" nowrap="nowrap">
                                    <asp:Label ID="Label1" runat="server" Text="Destinatários"></asp:Label>
                                </td>
                                <td align="left" nowrap="nowrap" colspan="2">
                                    <asp:TextBox ID="txtDest" runat="server" ReadOnly="true" Width="300px" Rows="2" 
                                        TextMode="MultiLine"></asp:TextBox>
                                    <a href="#" onclick="openWindow(500, 700, '../Utils/SelDestinatarioCliente.aspx');">
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
                                    <asp:TextBox ID="txtMensagem" runat="server" MaxLength="140" Width="300px" 
                                        Rows="5" TextMode="MultiLine"></asp:TextBox>
                                </td>
                                <td class="dtvAlternatingRow">
                                    <uc1:ctrlLimiteTexto ID="lmtTxtMensagem" runat="server" IdControlToValidate="txtMensagem" />
                                    <div style="margin-top:7px;position:relative;float:left;">
                                        <asp:Label ID="lblCustoSMS" runat="server" Text="Custo Estimado: R$ 0,00" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="3">
                                    <br />
                                    <asp:Button ID="btnEnviar" runat="server" Text="Enviar" OnClientClick="return enviarMsg();" />
                                </td>
                            </tr>
                            </table>
                    </td>
                </tr>
            </table>
        </div>
        <div id="divhiddens">
            <asp:HiddenField ID="hdfDest" runat="server" />         
            <asp:HiddenField ID="hdfTotalClientes" runat="server" />
        </div>
    </div>
</asp:Content>

