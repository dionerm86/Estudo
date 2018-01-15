<%@ Page Title="Cadastro de Pagamento Antecipado" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadObraNovo.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadObraNovo" %>

<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlFormaPagto.ascx" TagName="ctrlFormaPagto" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Cheque.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

var totalASerPago = 0;

function getProduto()
{
    openWindow(600, 800, "../Utils/SelProd.aspx?obra=true");
}

function setProduto(codInterno)
{
    loadProduto(codInterno);
}

function loadProduto(codInterno)
{
    var idObra = <%= Request["idObra"] != null ? Request["idObra"] : "0" %>;
    var idCliente = FindControl("lblIdCliente", "span").innerHTML;
    if (codInterno == "")
    {
        FindControl("txtCodProd", "input").value = "";
        FindControl("lblDescrProd", "span").innerHTML = "";
        FindControl("hdfIdProd", "input").value = "";
        FindControl("txtValorUnit", "input").value = "";
        return;
    }
    else if (CadObraNovo.IsVidro(codInterno).value != "true" || CadObraNovo.ProdutoJaExiste(idObra, codInterno).value == "true")
    {
        FindControl("txtCodProd", "input").value = "";
        FindControl("lblDescrProd", "span").innerHTML = "";
        FindControl("hdfIdProd", "input").value = "";
        FindControl("txtValorUnit", "input").value = "";
        
        alert("Apenas produtos do grupo Vidro podem ser incluídos nesse pagamento antecipado.");
        return;
    }
    
    var resposta = CadObraNovo.GetProd(codInterno, idCliente).value.split(";");
    if (resposta[0] == "Erro")
    {
        FindControl("txtCodProd", "input").value = "";
        FindControl("lblDescrProd", "span").innerHTML = "";
        FindControl("hdfIdProd", "input").value = "";
        FindControl("txtValorUnit", "input").value = "";
    
        alert(resposta[1]);
        return;
    }
    
    FindControl("txtCodProd", "input").value = codInterno;
    FindControl("lblDescrProd", "span").innerHTML = resposta[2];
    FindControl("hdfIdProd", "input").value = resposta[1];
    FindControl("txtValorUnit", "input").value = resposta[3];
}

// Validações realizadas ao receber conta
function onInsertUpdate() {
    if (!validate())
        return false;
    
    var idCliente = FindControl("txtNumCli", "input").value;
    var descricao = FindControl("txtDescricao", "textarea").value;
    
    if (descricao == "")
    {
        alert("Informe a descrição.");
        return false;
    }
    
    if (idCliente == "")
    {
        alert("Informe o Cliente.");
        return false;
    }
    
    return true;
}

function limpar() {
    try
    {
        <%= dtvObra.ClientID %>_ctrlFormaPagto1.Limpar();
    }
    catch (err) { }
}

function getCli(idCli)
{
    if (idCli.value == "")
        return;

    var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');
    
    if (retorno[0] == "Erro")
    {
        alert(retorno[1]);
        idCli.value = "";
        FindControl("txtNome", "input").value = "";
        return false;
    }
    
    FindControl("txtNome", "input").value = retorno[1];
}

// Abre popup para cadastrar cheques
function queryStringCheques() {
    return "?origem=3";
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
    var nomeControle = "<%= dtvObra.ClientID %>_ctrlParcelas1";
    if (typeof <%= dtvObra.ClientID %>_ctrlParcelas1 != "undefined")
        Parc_visibilidadeParcelas(nomeControle);
}

function finalizar(exibir)
{
    FindControl("btnEditar", "input").style.display = exibir ? "none" : "";
    FindControl("btnFinalizar", "input").style.display = exibir ? "none" : "";
    FindControl("btnVoltar", "input").style.display = exibir ? "none" : "";
    
    var tabProdutos = FindControl("grdProdutoObra", "table");
    var numLinha = tabProdutos.rows.length - 1;
    if (tabProdutos.rows[numLinha].cells.length == 1)
        numLinha--;
    
    tabProdutos.rows[numLinha].style.display = exibir ? "none" : "";
    
    document.getElementById("receber").style.display = exibir ? "" : "none";
    FindControl("btnReceber", "input").style.display = exibir ? "" : "none";
    FindControl("btnCancelar", "input").style.display = exibir ? "" : "none";
}

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvObra" runat="server" AutoGenerateRows="False" DataKeyNames="IdObra"
                    DataSourceID="odsObra" DefaultMode="Insert" GridLines="None">
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
                                            Cliente
                                        </td>
                                        <td nowrap="nowrap" align="left">
                                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                                onblur="getCli(this);" Text='<%# Bind("IdCliente") %>'></asp:TextBox>
                                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="200px" 
                                                Text='<%# Bind("NomeCliente") %>'></asp:TextBox>
                                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                                OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx?tipo=obra'); return false;" />
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
                                            Cliente
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblIdCliente" runat="server" Text='<%# Eval("IdCliente") %>'></asp:Label>
                                            &nbsp;-
                                            <asp:Label ID="Label2" runat="server" Text='<%# Eval("NomeCliente") %>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <asp:GridView ID="grdProdutoObra" runat="server" AllowPaging="True" 
                                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" 
                                    DataKeyNames="IdProdObra" DataSourceID="odsProdutoObra" GridLines="None" 
                                    ondatabound="grdProdutoObra_DataBound" ShowFooter="True">
                                    <Columns>
                                        <asp:TemplateField>
                                            <EditItemTemplate>
                                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" 
                                                    ImageUrl="~/Images/ok.gif" />
                                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" 
                                                    CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" />
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" 
                                                    ImageUrl="~/Images/EditarGrid.gif" />
                                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" 
                                                    ImageUrl="~/Images/ExcluirGrid.gif" 
                                                    onclientclick="if (!confirm(&quot;Deseja excluir esse produto da obra?&quot;)) return false" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtCodProd" runat="server" onblur="loadProduto(this.value);" 
                                                    onkeydown="if (isEnter(event)) loadProduto(this.value);" 
                                                    onkeypress="return !(isEnter(event));" Text='<%# Eval("CodInterno") %>' 
                                                    Width="50px"></asp:TextBox>
                                                <asp:Label ID="lblDescrProd" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>
                                                <a href="#" onclick="getProduto(); return false;">
                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                                                <asp:HiddenField ID="HidhdfIdObra" runat="server" Value='<%# Bind("IdObra") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtCodProd" runat="server" 
                                                    onblur="loadProduto(this.value);" 
                                                    onkeydown="if (isEnter(event)) loadProduto(this.value);" 
                                                    onkeypress="return !(isEnter(event));" Width="50px"></asp:TextBox>
                                                <asp:Label ID="lblDescrProd" runat="server"></asp:Label>
                                                <a href="#" onclick="getProduto(); return false;">
                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                <asp:HiddenField ID="hdfIdProd" runat="server" />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("CodInterno") %>'></asp:Label>
                                                -
                                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Valor Unit." SortExpression="ValorUnitario">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtValorUnit" runat="server" onkeypress="return soNumeros(event, false, true)"
                                                    Text='<%# Bind("ValorUnitario") %>' Width="70px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtValorUnit" runat="server" 
                                                    onkeypress="return soNumeros(event, false, true)" 
                                                    Text='<%# Bind("ValorUnitario") %>' Width="70px"></asp:TextBox>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" 
                                                    Text='<%# Bind("ValorUnitario", "{0:C}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="M² Máximo" SortExpression="TamanhoMaximo">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtTamanhoMax" runat="server" 
                                                    Text='<%# Bind("TamanhoMaximo") %>' 
                                                    onkeypress="return soNumeros(event, false, true)" Width="70px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtTamanhoMax" runat="server" 
                                                    onkeypress="return soNumeros(event, false, true)" 
                                                    Text='<%# Bind("TamanhoMaximo") %>' Width="70px"></asp:TextBox>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("TamanhoMaximo") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <FooterTemplate>
                                                <asp:ImageButton ID="imbAdd" runat="server" ImageUrl="~/Images/ok.gif" 
                                                    onclick="imbAdd_Click" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle CssClass="pgr" />
                                    <EditRowStyle CssClass="edit" />
                                    <AlternatingRowStyle CssClass="alt" />
                                </asp:GridView>
                                <asp:Panel ID="panReceber" runat="server" Visible="false">
                                    <div style="padding: 12px">
                                        Tipo de pagamento
                                        <asp:DropDownList ID="drpTipoPagto" runat="server" onchange="tipoPagtoChanged(true)">
                                            <asp:ListItem Value="1">À vista</asp:ListItem>
                                            <asp:ListItem Value="2">À prazo</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div id="a_vista">
                                        <uc2:ctrlFormaPagto ID="ctrlFormaPagto1" runat="server"
                                            CobrarJurosCartaoClientes="False" ExibirComissaoComissionado="false" ExibirCredito="True"
                                            ExibirDataRecebimento="false" ExibirGerarCredito="true" ExibirJuros="false"
                                            ExibirRecebParcial="false" ExibirUsarCredito="True" ExibirValorAPagar="True"
                                            FuncaoQueryStringCheques="queryStringCheques" UsarCreditoMarcado="false"
                                            OnLoad="ctrlFormaPagto1_Load" ParentID="a_vista" />
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
                                            <asp:ListItem>6</asp:ListItem>
                                            <asp:ListItem>7</asp:ListItem>
                                            <asp:ListItem>8</asp:ListItem>
                                            <asp:ListItem>9</asp:ListItem>
                                            <asp:ListItem>10</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:HiddenField ID="hdfCalcularParcelas" runat="server" Value="true" />
                                        <uc1:ctrlParcelas ID="ctrlParcelas1" runat="server" NumParcelas="10" NumParcelasLinha="3"
                                            OnLoad="ctrlParcelas1_Load" ParentID="a_prazo" />
                                    </div>
                                    <asp:HiddenField ID="hdfTotalObra" runat="server" />
                                </asp:Panel>
                                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutoObra" runat="server" 
                                    DataObjectTypeName="Glass.Data.Model.ProdutoObra" DeleteMethod="Delete" 
                                    EnablePaging="True" MaximumRowsParameterName="pageSize" 
                                    SelectCountMethod="GetCount" SelectMethod="GetList" 
                                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" 
                                    TypeName="Glass.Data.DAL.ProdutoObraDAO" UpdateMethod="Update" >
                                    <SelectParameters>
                                        <asp:QueryStringParameter Name="idObra" QueryStringField="idObra" 
                                            Type="UInt32" />
                                    </SelectParameters>                                    
                                </colo:VirtualObjectDataSource>
                                <asp:HiddenField ID="hdfCreditoCliente" runat="server" Value='<%# Eval("CreditoCliente") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <br />
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                    OnClientClick="if (!onInsertUpdate()) return false;" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False"
                                    Text="Cancelar" 
                                    onclientclick="redirectUrl(window.location.href); return false" />
                                <asp:HiddenField ID="hdfIdFunc" runat="server" Value='<%# Bind("IdFunc") %>' />
                                <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Bind("Situacao") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <br />
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="if (!onInsertUpdate()) return false;" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    Text="Cancelar" OnClick="btnCancelar_Click" />
                                <asp:HiddenField ID="hdfIdFunc" runat="server" Value='<%# Bind("IdFunc") %>' />
                                <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Bind("Situacao") %>' />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <br />
                                <asp:Button ID="btnEditar" runat="server" CommandName="Edit" Text="Editar" />
                                <asp:Button ID="btnFinalizar" runat="server" Text="Finalizar" 
                                    onclick="btnFinalizar_Click" 
                                    onclientclick="if (!confirm(&quot;Deseja finalizar o pagamento antecipado?&quot;)) return false" />
                                <asp:Button ID="btnVoltar" runat="server" Text="Voltar" 
                                    onclick="btnCancelar_Click" />
                                <asp:Button ID="btnReceber" runat="server" Text="Receber" Visible="False" 
                                    onclick="btnReceber_Click" />
                                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" 
                                    onclick="btnCancelarReceb_Click" Visible="False" 
                                    
                                    onclientclick="if (!confirm(&quot;Deseja cancelar o recebimento?&quot;)) return false" 
                                    CausesValidation="False" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsObra" runat="server" DataObjectTypeName="Glass.Data.Model.Obra"
                    InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.ObraDAO"
                    UpdateMethod="Update" OnInserted="odsObra_Inserted" 
                    onupdated="odsObra_Updated" >
                    <SelectParameters>
                        <asp:QueryStringParameter Name="IdObra" QueryStringField="IdObra" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        tipoPagtoChanged(false);
    </script>

</asp:Content>
