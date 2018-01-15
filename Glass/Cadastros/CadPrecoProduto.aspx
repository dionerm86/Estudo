<%@ Page Title="Inserção Rápida de Preço" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadPrecoProduto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadPrecoProduto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

    // Carrega dados do produto com base no código do produto passado
    function loadProduto(codInterno) {
        if (codInterno == "")
            return false;

        try
        {
            var tipoPreco = FindControl("drpTipoPreco", "select").value;
            var retorno = CadPrecoProduto.GetProduto(codInterno, tipoPreco).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                FindControl("txtCodProd", "input").value = "";
                return false;
            }

            FindControl("hdfIdProd", "input").value = retorno[1];
            FindControl("lblDescrProd", "span").innerHTML = retorno[2];
            FindControl("txtPreco", "input").value = retorno[3];
        }
        catch (err) {
            alert(err);
        }
    }

    // Atualiza preço de custo do produto
    function atualizaPreco() {
        var retorno = CadPrecoProduto.AtualizaPreco(FindControl("hdfIdProd", "input").value, FindControl("drpTipoPreco", "select").value, FindControl("txtPreco", "input").value).value.split('\t');

        if (retorno[0] == "Ok") {
            alert("Preço atualizado.");
            limpar();
            FindControl("txtCodProd", "input").focus();
        }
        else
            alert(retorno[1]);

        return false;
    }

    function limpar() {
        FindControl("txtCodProd", "input").value = "";
        FindControl("hdfIdProd", "input").value = "";
        FindControl("lblDescrProd", "span").innerHTML = "";
        FindControl("txtPreco", "input").value = "";
    }

    </script>
    
    <section>
        <div class="boxLinha">
             <asp:Label ID="Label4" runat="server" Text="Produto:" ForeColor="#0066FF"></asp:Label>
             <asp:TextBox ID="txtCodProd" runat="server" onblur="loadProduto(this.value);" onkeydown="if (isEnter(event)) loadProduto(this.value);"
                                onkeypress="return !(isEnter(event));" Width="70px"></asp:TextBox>
                                
             <asp:Label ID="lblDescrProd" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>
             
             <asp:Label ID="Label5" runat="server" Text="Preço:" ForeColor="#0066FF"></asp:Label>
             <asp:TextBox ID="txtPreco" runat="server" onkeypress="return soNumeros(event, false, true);"
                                Width="70px"></asp:TextBox>
                                
             <asp:Button ID="btnConfirmar" OnClientClick="return atualizaPreco();" runat="server"
                                Text="Confirmar" />
        </div>
        <div class="boxLinha">
            <asp:Label ID="Label1" runat="server" Text="Tipo de Preço:" ForeColor="#0066FF"></asp:Label>
             <asp:DropDownList ID="drpTipoPreco" runat="server" onchange="loadProduto(FindControl('txtCodProd', 'input').value)">
                                <asp:ListItem Value="0">Custo Forn.</asp:ListItem>
                                <asp:ListItem Value="1">Custo Imp.</asp:ListItem>
                                <asp:ListItem Value="2">Atacado</asp:ListItem>
                                <asp:ListItem Value="3">Balcão</asp:ListItem>
                                <asp:ListItem Value="4">Obra</asp:ListItem>
                            </asp:DropDownList>
                              <asp:HiddenField ID="hdfIdProd" runat="server" />
        </div>
    </section>


   
   <%--  <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Produto:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" onblur="loadProduto(this.value);" onkeydown="if (isEnter(event)) loadProduto(this.value);"
                                onkeypress="return !(isEnter(event));" Width="70px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lblDescrProd" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Preço:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPreco" runat="server" onkeypress="return soNumeros(event, false, true);"
                                Width="70px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button ID="btnConfirmar" OnClientClick="return atualizaPreco();" runat="server"
                                Text="Confirmar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Tipo de Preço:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoPreco" runat="server" onchange="loadProduto(FindControl('txtCodProd', 'input').value)">
                                <asp:ListItem Value="0">Custo Forn.</asp:ListItem>
                                <asp:ListItem Value="1">Custo Imp.</asp:ListItem>
                                <asp:ListItem Value="2">Atacado</asp:ListItem>
                                <asp:ListItem Value="3">Balcão</asp:ListItem>
                                <asp:ListItem Value="4">Obra</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HiddenField ID="hdfIdProd" runat="server" />
            </td>
        </tr>
    </table>--%>

    <script type="text/javascript">
        FindControl("txtCodProd", "input").focus();  
    </script>

</asp:Content>
