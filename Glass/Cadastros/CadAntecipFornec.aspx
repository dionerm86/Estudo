<%@ Page Title="Cadastro de Pagamento Antecipado" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadAntecipFornec.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadAntecipFornec" %>

<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlFormaPagto.ascx" TagName="ctrlFormaPagto" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

    function getFornec(idFornec) {
        if (idFornec.value == "")
            return;

        var retorno = MetodosAjax.GetFornec(idFornec.value).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idFornec.value = "";
            FindControl("txtNomeFornec", "input").value = "";
            return false;
        }

        FindControl("txtNomeFornec", "input").value = retorno[1];
    }

    var clicked = false;

    function onInsertUpdate() {
        if (!validate())
            return false;

        if (clicked)
            return false;

        clicked = true;

        var idFornec = FindControl("txtNumFornec", "input").value;
        var descricao = FindControl("txtDescricao", "textarea").value;

        if (descricao == "") {
            alert("Informe a descrição.");
            clicked = false;
            return false;
        }

        if (idFornec == "") {
            alert("Informe o Fornecedor.");
            clicked = false;
            return false;
        }

        return true;
    }
    
    function tipoPagtoChanged(calcParcelas)
    {
        var tipoPagto = FindControl("drpTipoPagto", "select");
        
        if (tipoPagto == null)
            return;
        else
            tipoPagto = tipoPagto.value;
        
        document.getElementById("a_vista").style.display = (tipoPagto == 1) ? "" : "none";
        document.getElementById("a_prazo").style.display = (tipoPagto == 2) ? "" : "none";
            
        FindControl("hdfCalcularParcelas", "input").value = calcParcelas;
        var nomeControle = "<%= dtvAntecipFornec.ClientID %>_ctrlParcelas1";
        if (typeof <%= dtvAntecipFornec.ClientID %>_ctrlParcelas1 != "undefined")
            Parc_visibilidadeParcelas(nomeControle);
    }
    
    function getUrlCheques(tipoPagto, urlPadrao)
    {
        return tipoPagto == 2 ? "CadChequePagto.aspx" : "CadChequePagtoTerc.aspx";
    }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvAntecipFornec" runat="server" AutoGenerateRows="False" DataKeyNames="IdAntecipFornec"
                    DataSourceID="odsAntecipFornec" DefaultMode="Insert" GridLines="None">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <table cellspacing="0">
                                    <tr class="dtvAlternatingRow">
                                        <td class="dtvHeader">
                                            Descrição
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtDescricao" runat="server" MaxLength="200" Rows="3" Width="400px"
                                                Text='<%# Bind("Descricao") %>' TextMode="MultiLine"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Fornecedor
                                        </td>
                                        <td nowrap="nowrap" align="left">
                                            <asp:TextBox ID="txtNumFornec" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                                onblur="getFornec(this);" Text='<%# Bind("IdFornec") %>'></asp:TextBox>
                                            <asp:TextBox ID="txtNomeFornec" runat="server" Width="200px" Text='<%# Bind("NomeFornec") %>'></asp:TextBox>
                                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                                OnClientClick="openWindow(590, 760, '../Utils/SelFornec.aspx?'); return false;" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Valor do pagamento antecipado
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtTotal" runat="server" Text='<%# Bind("ValorAntecip") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <table cellspacing="0" cellpadding="4">
                                    <tr>
                                        <td class="dtvHeader">
                                            Descrição
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("Descricao") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Fornecedor
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblIdFornec" runat="server" Text='<%# Eval("IdFornec") %>'></asp:Label>
                                            &nbsp;-
                                            <asp:Label ID="Label2" runat="server" Text='<%# Eval("NomeFornec") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Valor do pagamento antecipado
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label3" runat="server" Text='<%# Eval("ValorAntecip", "{0:c}") %>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <asp:Panel ID="panPagar" runat="server" Visible="false">
                                    <div style="padding: 12px">
                                        Tipo de pagamento
                                        <asp:DropDownList ID="drpTipoPagto" runat="server" onchange="tipoPagtoChanged(true)">
                                            <asp:ListItem Value="1">À vista</asp:ListItem>
                                            <asp:ListItem Value="2">À prazo</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>  
                                    <div id="a_vista">
                                        <uc2:ctrlFormaPagto ID="ctrlFormaPagto1" runat="server" OnLoad="ctrlFormaPagto1_Load"
                                            TextoValorReceb="Valor Pagto." ExibirJuros="false"
                                            ExibirRecebParcial="false" MetodoFormasPagto="GetForPagto" FuncaoUrlCheques="getUrlCheques"
                                            IsRecebimento="false" EfetuarBindContaBanco="false" />
                                    </div>
                                    <div id="a_prazo">
                                        <br />
                                        Número de parcelas:
                                        <asp:DropDownList ID="drpNumParcelas" runat="server">
                                            <asp:ListItem>1</asp:ListItem>
                                            <asp:ListItem>2</asp:ListItem>
                                            <asp:ListItem>3</asp:ListItem>
                                            <asp:ListItem>4</asp:ListItem>
                                            <asp:ListItem>5</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:HiddenField ID="hdfCalcularParcelas" runat="server" Value="true" />
                                        <uc1:ctrlParcelas ID="ctrlParcelas1" runat="server" NumParcelas="5" NumParcelasLinha="3"
                                            OnLoad="ctrlParcelas1_Load" ParentID="a_prazo" />
                                    </div>
                                    <asp:HiddenField ID="hdfTotalAntecipFornec" runat="server" />
                                </asp:Panel>
                                <asp:HiddenField ID="hdfCreditoFornec" runat="server" Value='<%# Eval("CreditoFornec") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <br />
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                    OnClientClick="if (!onInsertUpdate()) return false;" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" Text="Cancelar"
                                    OnClientClick="redirectUrl(window.location.href); return false" />
                                <asp:HiddenField ID="hdfIdFunc" runat="server" Value='<%# Bind("IdFunc") %>' />
                                <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Bind("Situacao") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <br />
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="return onInsertUpdate();" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    Text="Cancelar" OnClick="btnCancelar_Click" />
                                <asp:HiddenField ID="hdfIdFunc" runat="server" Value='<%# Bind("IdFunc") %>' />
                                <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Bind("Situacao") %>' />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <br />
                                <asp:Button ID="btnEditar" runat="server" CommandName="Edit" Text="Editar" />
                                <asp:Button ID="btnFinalizar" runat="server" Text="Finalizar" OnClick="btnFinalizar_Click"
                                    OnClientClick="if (!confirm('Deseja finalizar o pagamento antecipado?')) return false;" />
                                <asp:Button ID="btnVoltar" runat="server" Text="Voltar" OnClick="btnCancelar_Click" />
                                <asp:Button ID="btnPagar" runat="server" Text="Pagar" Visible="False" OnClick="btnPagar_Click" />
                                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelarPagamento_Click"
                                    Visible="False" OnClientClick="if (!confirm(&quot;Deseja cancelar o pagamento?&quot;)) return false"
                                    CausesValidation="False" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAntecipFornec" runat="server" DataObjectTypeName="Glass.Data.Model.AntecipacaoFornecedor"
                    InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.AntecipacaoFornecedorDAO"
                    UpdateMethod="Update" OnInserted="odsAntecipFornec_Inserted" OnUpdated="odsAntecipFornec_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="IdAntecipFornec" QueryStringField="IdAntecipFornec"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        tipoPagtoChanged(false);
    </script>

</asp:Content>
