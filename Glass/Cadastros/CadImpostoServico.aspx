<%@ Page Title="Cadastro de Imposto/Serviços Avulsos" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadImpostoServico.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadImpostoServico" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrlTextBoxFloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcAluminio.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcProd.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CallbackItem_ctrlBenef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" >

// Pega o id do forma de pagto boleto
var codBoleto = MetodosAjax.GetIdFormaPagto("boleto").value;
var codCheque = MetodosAjax.GetIdFormaPagto("cheque").value;
var idFornec = <%= GetIdFornec() %>;
var numeroParcelasMax = <%= Glass.Configuracoes.FinanceiroConfig.Compra.NumeroParcelasCompra %>;
var loading = true;

// Evento acionado ao trocar o tipo de compra (à vista/à prazo)
function tipoCompraChange(control) {
    if (control == null)
        return;

    var cNumParc = FindControl("txtNumParc", "input");

    // Se for à vista, Desabilita o campo número de parcelas se for compra à vista
    FindControl("txtNumParc", "input").disabled = control.value == 1;

    // Desabilita valor da parcela e data base de vencimento se for compra a vista ou 
    // se o numero de parcelas for < 6 && != 0
    FindControl("ctrValorParc_txtNumber", "input").disabled = control.value == 1 || (cNumParc < numeroParcelasMax && cNumParc != 0);
    FindControl("ctrlDataVenc_txtData", "input").disabled = control.value == 1 || (cNumParc < numeroParcelasMax && cNumParc != 0);

    // Se for à prazo e o número de parcelas estiver 0, muda para 1
    if (control.value == 2 && cNumParc.value == 0)
        cNumParc.value = 1;
    
    exibeParcelas();
}

function exibeParcelas()
{
    var drpTipoCompra = FindControl("drpTipoCompra", "select");
    var txtNumParc = FindControl("txtNumParc", "input");
    var txtValorParc = FindControl("ctrValorParc_txtNumber", "input");
    var txtDataBaseVenc = FindControl("ctrlDataVenc_txtData", "input");
    var imgDataEntrega = FindControl("ctrlDataVenc_imgData", "input");
    
    var exibirParcelas = drpTipoCompra.value == 2 && parseInt(txtNumParc.value, 10) <= numeroParcelasMax;
    FindControl("hdfExibirParcelas", "input").value = exibirParcelas;
    
    txtValorParc.disabled = exibirParcelas;
    txtDataBaseVenc.disabled = exibirParcelas;
    imgDataEntrega.disabled = exibirParcelas;
    
    Parc_visibilidadeParcelas("<%= dtvImpostoServ.ClientID %>_ctrlParcelas1");
    if (!loading)
        Parc_atualizaFormasPagto("<%= dtvImpostoServ.ClientID %>_ctrlParcelas1");
}

var clicou = false;

function onInsert() {
    if (clicou)
        return false;
                
    clicou = true;

    var fornec = FindControl("hdfFornec", "input").value;
    var planoConta = FindControl("ddlPlanoConta", "select").value;

    // Verifica se o fornecedor foi selecionado
    if (fornec == "" || fornec == null) {
        alert("Informe o Fornecedor.");
        clicou = false;
        return false;
    }
    
    // Verifica se o plano de contas foi selecionado
    if (planoConta == "" || planoConta == null || planoConta == "0")
    {
        alert("Informe o Plano de Contas.");
        clicou = false;
        return false;
    }
    
    var prazo = parseInt(FindControl("hdfPrazoFornec", "input").value, 10);
    var numParcelas = parseInt(FindControl("txtNumParc", "input").value, 10);
    
    if (prazo != "" && numParcelas > prazo)
    {
        if (prazo > 0)
            alert("Esse fornecedor aceita apenas " + prazo + " parcela(s).");
        else
            alert("Esse fornecedor aceita apenas compras à vista.")
        
        clicou = false;
        return false;
    }

    // Se for compra a prazo
    if (FindControl("drpTipoCompra", "select").value == "2")
    {
        var numParc = FindControl("txtNumParc", "input").value;
        var valorParc = FindControl("ctrValorParc_txtNumber", "input").value;
        var dataBaseVenc = FindControl("ctrlDataVenc_txtData", "input").value;

        if (numParc == "0" || numParc == "") {
            alert("Informe o número de parcelas.");
            clicou = false;
            return false;
        }
        else if (parseInt(numParc) > numeroParcelasMax) {
            if (valorParc == "" || valorParc == "0" || valorParc == "0,00" || valorParc == 0) {
                alert("Informe o valor da parcela.");
                clicou = false;
                return false;
            }
            else if (dataBaseVenc == "") {
                alert("Informe a data de vencimento base das parcelas.");
                clicou = false;
                return false;
            }
        }
    }
}

// Acionado quando a Compra está para ser salva
function onSave() {
    if (!validate()){
        clicou = false;
        return false;
    }
    
    var fornec = FindControl("hdfFornec", "input").value;
    var planoConta = FindControl("ddlPlanoConta", "select").value;

    // Verifica se o Fornecedor foi selecionado
    if (fornec == "" || fornec == null)
    {
        alert("Informe o Fornecedor.");
        clicou = false;
        return false;
    }
    
    // Verifica se o plano de contas foi selecionado
    if (planoConta == "" || planoConta == null || planoConta == "0")
    {
        alert("Informe o Plano de Contas.");
        clicou = false;
        return false;
    }

    // Se for compra a prazo
    if (FindControl("drpTipoCompra", "select").value == "2")
    {
        var numParc = FindControl("txtNumParc", "input").value;
        var valorParc = FindControl("ctrValorParc_txtNumber", "input").value;
        var dataBaseVenc = FindControl("ctrlDataVenc_txtData", "input").value;

        if (numParc == "0" || numParc == "") {
            alert("Informe o número de parcelas.");
            clicou = false;
            return false;
        }
        else if (parseInt(numParc) > numeroParcelasMax) {
            if (valorParc == "" || valorParc == "0" || valorParc == "0,00" || valorParc == 0) {
                alert("Informe o valor da parcela.");
                clicou = false;
                return false;
            }
            else if (dataBaseVenc == "") {
                alert("Informe a data de vencimento base das parcelas.");
                clicou = false;
                return false;
            }
        }
    }
    
    var prazo = parseInt(FindControl("hdfPrazoFornec", "input").value, 10);
    var numParcelas = parseInt(FindControl("txtNumParc", "input").value, 10);
    
    if (prazo != "" && numParcelas > prazo)
    {
        if (prazo > 0)
            alert("Esse fornecedor aceita apenas " + prazo + " parcela(s).");
        else
            alert("Esse fornecedor aceita apenas compras à vista.")
            
        clicou = false;
        return false;
    }
}

function getFornec(idFornec)
{
    var retorno = MetodosAjax.GetFornec(idFornec.value).value.split(';');
    
    if (retorno[0] == "Erro")
    {
        alert(retorno[1]);
        idFornec.value = "";
        FindControl("txtNomeFornec", "input").value = "";
        FindControl("hdfFornec", "input").value = "";
        FindControl("hdfPrazoFornec", "input").value = "";
        return false;
    }
    
    FindControl("txtNomeFornec", "input").value = retorno[1];
    FindControl("hdfFornec", "input").value = idFornec.value;
    FindControl("hdfPrazoFornec", "input").value = retorno[2];
    if (retorno[3] != "")
        FindControl("ddlPlanoConta", "select").value = retorno[3];
}

        function setPlanoConta(idConta, descricao) {
            var planoConta = FindControl("ddlPlanoConta", "select");

            if (planoConta == null)
                return false;
            
            planoConta.value = idConta;
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td>
                <table style="width: 100%">
                    <tr>
                        <td align="center">
                            <asp:DetailsView ID="dtvImpostoServ" runat="server" AutoGenerateRows="False" DataSourceID="odsImpostoServ"
                                DefaultMode="Insert" GridLines="None" Height="50px" Width="125px" OnDataBound="dtvImpostoServ_DataBound"
                                DataKeyNames="IdImpostoServ">
                                <Fields>
                                    <asp:TemplateField>
                                        <EditItemTemplate>
                                            <table cellpadding="2" cellspacing="0">
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Fornecedor
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtNumFornec" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                                            onblur="getFornec(this);" Text='<%# Bind("IdFornec") %>'></asp:TextBox>
                                                        <asp:TextBox ID="txtNomeFornec" runat="server" ReadOnly="True" Text='<%# Eval("NomeFornec") %>'
                                                            Width="250px"></asp:TextBox>
                                                        <asp:LinkButton ID="lnkSelFornec" runat="server" OnClientClick="openWindow(570, 760, '../Utils/SelFornec.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Num Parc.
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtNumParc" runat="server" onblur="exibeParcelas()" onkeypress="return soNumeros(event, true, true);"
                                                            Text='<%# Bind("NumParc") %>' Width="50px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Plano de Conta
                                                    </td>
                                                    <td align="left" nowrap="nowrap" valign="middle">
                                                        <asp:DropDownList ID="ddlPlanoConta" runat="server" DataSourceID="odsPlanoConta" OnLoad="ddlPlanoContaEdit_Load"
                                                            DataTextField="DescrPlanoGrupo" DataValueField="IdConta" SelectedValue='<%# Bind("IdConta") %>'
                                                            AppendDataBoundItems="True">
                                                            <asp:ListItem Value="0" Text=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:LinkButton ID="lnkSelPlanoConta" runat="server" OnClientClick="openWindow(600, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" class="dtvHeader">
                                                        Valor Parc.
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <uc1:ctrlTextBoxFloat ID="ctrValorParc" runat="server" onblur="calcValParc(true);"
                                                            Value='<%# Bind("ValorParc") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Tipo Pagto
                                                    </td>
                                                    <td align="left" nowrap="nowrap" valign="middle">
                                                        <asp:DropDownList ID="drpTipoCompra" runat="server" onchange="tipoCompraChange(this);"
                                                            SelectedValue='<%# Bind("TipoPagto") %>'>
                                                            <asp:ListItem Value="1">À Vista</asp:ListItem>
                                                            <asp:ListItem Value="2">À Prazo</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Data Base Venc.
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <uc5:ctrlData ID="ctrlDataVenc" runat="server" ReadOnly="ReadWrite" DataNullable='<%# Bind("DataBaseVenc") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Forma Pagto.
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:DropDownList ID="drpFormaPagto" runat="server" DataSourceID="odsFormaPagto"
                                                            DataTextField="Descricao" DataValueField="IdFormaPagto" SelectedValue='<%# Bind("IdFormaPagto") %>'>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" class="dtvHeader">
                                                        Loja
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                                            DataValueField="IdLoja" SelectedValue='<%# Bind("IdLoja") %>'>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" class="dtvHeader">
                                                        NF/Pedido
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td align="left" nowrap="nowrap">
                                                                    <asp:TextBox ID="txtNf" runat="server" MaxLength="20" Text='<%# Bind("Nf") %>'></asp:TextBox>
                                                                    &nbsp;
                                                                </td>
                                                                <td align="left" nowrap="nowrap" class="dtvHeader" style="padding-left: 4px; padding-right: 4px">
                                                                    Contabil
                                                                </td>
                                                                <td align="left" nowrap="nowrap">
                                                                    <asp:CheckBox ID="chkContabil" runat="server" Checked='<%# Bind("Contabil") %>' />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" class="dtvHeader">
                                                        Total
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtTotal" runat="server" onkeypress="return soNumeros(event, false, true)"
                                                            Text='<%# Bind("Total") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" id="parcelas" colspan="4" nowrap="nowrap">
                                                        <uc3:ctrlParcelas ID="ctrlParcelas1" runat="server" ExibirCampoAdicional="False"
                                                            ParentID="parcelas" NumParcelasLinha="3" Datas='<%# Bind("DatasParcelas") %>'
                                                            Valores='<%# Bind("ValoresParcelas") %>' OnLoad="ctrlParcelas1_Load" IsCompra="False" />
                                                        <asp:HiddenField ID="hdfCalcularParcelas" runat="server" Value="false" />
                                                        <asp:HiddenField ID="hdfExibirParcelas" runat="server" Value="true" />
                                                        <asp:HiddenField ID="hdfDataCad" runat="server" Value='<%# Bind("DataCad") %>' />
                                                        <asp:HiddenField ID="hdfUsuCad" runat="server" Value='<%# Bind("UsuCad") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%">
                                                <tr>
                                                    <td class="dtvHeader">
                                                        Observação
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:TextBox ID="TextBox3" runat="server" MaxLength="300" Text='<%# Bind("Obs") %>'
                                                            TextMode="MultiLine" Width="650px"></asp:TextBox>
                                                        <asp:HiddenField ID="hdfFornec" runat="server" Value='<%# Bind("IdFornec") %>' />
                                                        <asp:HiddenField ID="hdfPrazoFornec" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <table cellpadding="2" cellspacing="2">
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Cód.
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblIdImpostoServ" runat="server" Text='<%# Eval("IdImpostoServ") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Fornecedor
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNomeFornecedor" runat="server" Text='<%# Eval("NomeFornec") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Loja
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNomeLoja" runat="server" Text='<%# Eval("NomeLoja") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Funcionário
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNomeFunc" runat="server" Text='<%# Eval("DescrUsuCad") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Tipo Pagto
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTipoPagto" runat="server" Text='<%# Eval("DescrTipoPagto") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Forma Pagto.
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblFormaPagto" runat="server" Text='<%# Eval("FormaPagto") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        NF/Pedido</td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="Label11" runat="server" Text='<%# Eval("Nf") %>'></asp:Label>
                                                    </td>
                                                    <td colspan="2">
                                                        <table width="100%">
                                                            <tr>
                                                                <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                                    Núm. Parcelas
                                                                </td>
                                                                <td align="left" nowrap="nowrap">
                                                                    <asp:Label ID="Label9" runat="server" Text='<%# Eval("NumParc") %>'></asp:Label>
                                                                </td>
                                                                <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                                    Valor Parcela
                                                                </td>
                                                                <td align="left" nowrap="nowrap">
                                                                    <asp:Label ID="Label10" runat="server" Text='<%# Eval("ValorParc", "{0:C}") %>'></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Total
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTotal" runat="server" Text='<%# Eval("Total", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Bind("Total") %>' />
                                            <asp:HiddenField ID="hdfIdLoja" runat="server" Value='<%# Eval("IdLoja") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                                OnClientClick="return onSave();" />
                                            <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" OnClick="btnCancelar_Click"
                                                Text="Cancelar" />
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <asp:Button ID="btnInserir" runat="server" CommandName="Insert" OnClientClick="return onInsert();"
                                                Text="Inserir" />
                                            <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" OnClick="btnCancelar_Click"
                                                Text="Cancelar" />
                                        </InsertItemTemplate>
                                        <ItemTemplate>
                                            <asp:Button ID="btnEditar" runat="server" CommandName="Edit" Text="Editar" />
                                            <asp:Button ID="btnVoltar" runat="server" OnClick="btnCancelar_Click" Text="Voltar"
                                                Visible='false' />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Fields>
                            </asp:DetailsView>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsImpostoServ" runat="server" DataObjectTypeName="Glass.Data.Model.ImpostoServ"
            InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.ImpostoServDAO"
            UpdateMethod="Update" OnInserted="odsImpostoServ_Inserted" OnUpdated="odsImpostoServ_Updated">
            <SelectParameters>
                <asp:QueryStringParameter Name="idImpostoServ" QueryStringField="idImpostoServ" Type="UInt32" />
            </SelectParameters>
        </colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetVendedores"
            TypeName="Glass.Data.DAL.FuncionarioDAO"></colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoConta" runat="server" SelectMethod="GetPlanoContasCompra"
            TypeName="Glass.Data.DAL.PlanoContasDAO"></colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetListSituacao" TypeName="Glass.Data.DAL.LojaDAO">
            <SelectParameters>
                <asp:Parameter DefaultValue="1" Name="situacao" Type="Int32" />
            </SelectParameters>
        </colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForCompra"
            TypeName="Glass.Data.DAL.FormaPagtoDAO"></colo:VirtualObjectDataSource>
    </table>

    <script type="text/javascript">
        tipoCompraChange(FindControl("drpTipoCompra", "select"));
        loading = false;
    </script>

</asp:Content>
