<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstOrcamento.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstOrcamento" Title="Orçamentos" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

    var lnkGerarPedido;
    var hdfIdCliente;

    function geraPedido(link, hiddenIdCliente, perguntar)
    {
        if (perguntar && !confirm("Tem certeza que deseja gerar um pedido para este orçamento?"))
            return false;
        
        lnkGerarPedido = link;
        hdfIdCliente = document.getElementById(hiddenIdCliente);
        if (hdfIdCliente.value == "")
        {
            alert("Você deve selecionar o cliente antes de continuar.");
            openWindow(500, 750, "../Utils/SelCliente.aspx");
            return false;
        }
        
        return true;
    }

    function openRptProj(idOrcamento) {
        openWindow(600, 800, "../Cadastros/Projeto/ImprimirProjeto.aspx?idOrcamento=" + idOrcamento);
        return false;
    }

    function getCli(abrirPopup) {
        var idCliente = FindControl("txtNumCli", "input");
        var nomeCliente = FindControl("txtNome", "input");

        if (idCliente.value == "" && nomeCliente.value == "" && abrirPopup) 
        {
            openWindow(600, 800, "../Utils/SelCliente.aspx?custom=1");                
            return false;
        }

        if (idCliente.value == "")
            return false;

        var retorno = MetodosAjax.GetCli(idCliente.value).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idCliente.value = "";
            nomeCliente.value = "";
            return false;
        }

        nomeCliente.value = retorno[1];

        return false;
    }

    function setClienteCustom(idCli, nome) {
        FindControl("txtNumCli", "input").value = idCli;
        FindControl("txtNome", "input").value = nome;
    }

    function setCliente(idCli, nome)
    {
        hdfIdCliente.value = idCli;
        eval(lnkGerarPedido.href);
    }

    function openRpt(idOrca)
    {
        openWindow(600, 800, "../Relatorios/RelOrcamento.aspx?idOrca=" + idOrca);
        return false;
    }

    function openRptMemoria(idOrca)
    {
        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=MemoriaCalculoOrcamento&idOrca=" + idOrca);
        return false;
    }

    function setCidade(idCidade, nomeCidade, nomeUf) {
        FindControl('hdfCidade', 'input').value = idCidade;
        FindControl('txtCidade', 'input').value = nomeCidade + " - " + nomeUf;
    }
    
    function limpaCampoCidade()
    {
        FindControl('hdfCidade', 'input').value = 0;
        FindControl('txtCidade', 'input').value = "";
    }

    function confirmarReenvioEmail() {
        return confirm("Deseja realmente enviar o e-mail do orçamento?");
    }

    </script>

    <table>
        <tr>
            <td align="center">
                <table class="style1">
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label3" runat="server" Text="Cód." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtCod" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" OnClick="imgPesq_Click" ImageUrl="~/Images/Pesquisar.gif" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(false);"></asp:TextBox>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtNome" runat="server" Width="180px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td align="left">
                            <asp:LinkButton ID="lnkPesquisar0" runat="server" OnClick="lnkPesquisar_Click"><img border="0" 
                                src="../Images/Pesquisar.gif" onclick="getCli(true);" /></asp:LinkButton>
                        </td>
                        <td align="left">
                            <asp:Label ID="Label6" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                             <sync:CheckBoxListDropDown runat="server" ID="drpSituacao" DataSourceID="odsSituacaoOrca"
                                DataTextField="Descr" DataValueField="Id" AutoPostBack="True">
                             </sync:CheckBoxListDropDown>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblPeriodoSituacao" runat="server" Text="Período " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td align="left">
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td align="left">
                            <asp:ImageButton ID="imgPesqPeriodoSituacao" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label16" runat="server" ForeColor="#0066FF" Text="Loja"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Vendedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpVendedor" runat="server" DataSourceID="odsVendedor" DataTextField="Nome"
                                DataValueField="IdFunc" AutoPostBack="True" AppendDataBoundItems="True" OnTextChanged="drpSituacao_SelectedIndexChanged">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label17" runat="server" Text="Cidade" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCidade" runat="server" MaxLength="50" Width="200px" ReadOnly="True"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx?retUf=1'); return false;" />
                            <asp:HiddenField ID="hdfCidade" runat="server"></asp:HiddenField>
                            <asp:ImageButton ID="imbLimpaCampoCidade" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                OnClientClick="limpaCampoCidade(); return false;"></asp:ImageButton>
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Endereço" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtEndereco" runat="server" Width="140px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:LinkButton ID="lnkPesquisar3" runat="server" OnClick="lnkPesquisar_Click"><img border="0" 
                                src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="lblComplemento" runat="server" Text="Complemento" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtComplemento" runat="server" Width="140px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:LinkButton ID="lnkPesquisar5" runat="server" OnClick="lnkPesquisar_Click"><img border="0" 
                                src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Bairro" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtBairro" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                Width="90px"></asp:TextBox>
                            <asp:LinkButton ID="lnkPesquisar4" runat="server" OnClick="lnkPesquisar_Click"><img border="0" 
                                src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label5" runat="server" Text="Telefone" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtTelefone" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                Width="80px"></asp:TextBox>
                            <asp:LinkButton ID="lnkPesquisar" runat="server" OnClick="lnkPesquisar_Click"><img border="0" 
                                src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <br />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkOrçamento" runat="server" OnClick="lnkOrçamento_Click">Inserir 
                Orçamento</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdOrcamento" runat="server" AllowPaging="True"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" AllowSorting="True" AutoGenerateColumns="False"
                    DataKeyNames="IdOrcamento" DataSourceID="odsOrcamento" OnRowCommand="grdOrcamento_RowCommand">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" Visible='<%# Eval("EditVisible") %>'
                                    NavigateUrl='<%# "../Cadastros/CadOrcamento.aspx?idorca=" + Eval("Idorcamento") %>'>
                                    <img border="0" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# Eval("ExibirImpressao") %>'>
                                    <a href="#" onclick="openRpt('<%# Eval("IdOrcamento") %>');"><img border="0"
                                        src="../Images/Relatorio.gif" /></a>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="pchFotos" runat="server" Visible='<%# Eval("IdsMedicao") != null && !string.IsNullOrWhiteSpace(Eval("IdsMedicao").ToString()) %>'>
                                    <a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?tipo=medicao&id=<%# Eval("IdsMedicao") %>&#039;); return false;'>
                                        <img border="0px" src="../Images/Fotos.gif"></img></a></asp:PlaceHolder>
                                <asp:ImageButton ID="imbMemoriaCalculo" runat="server" Visible='<%# Eval("ExibirRelatorioCalculo") %>'
                                    OnClientClick='<%# "openRptMemoria(" + Eval("IdOrcamento") + "); return false;" %>'
                                    ToolTip="Memória de cálculo" ImageUrl="~/Images/calculator.gif" />
                                <asp:PlaceHolder ID="pchImprProj" runat="server" Visible='<%# Eval("ExibirImpressaoProjeto") %>'>
                                    <a href="#" onclick="openRptProj('<%# Eval("IdOrcamento") %>');">
                                        <img border="0" src="../Images/clipboard.gif" title="Projeto" /></a> </asp:PlaceHolder>
                                <asp:PlaceHolder ID="pchAnexos" runat="server"><a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=<%# Eval("IdOrcamento") %>&tipo=orcamento&#039;); return false;'>
                                    <img border="0px" src="../Images/Clipe.gif"></img></a></asp:PlaceHolder>
                                <asp:HyperLink ID="lnkSugestao" runat="server" Visible='<%# SugestoesVisible() %>' ImageUrl="../Images/Nota.gif" ToolTip="Sugestões"
                                    NavigateUrl='<%# "../Listas/LstSugestaoCliente.aspx?idOrcamento=" + Eval("IdOrcamento") %>'></asp:HyperLink>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdOrcamento" HeaderText="Num." SortExpression="IdOrcamento" />
                        <asp:BoundField DataField="IdProjeto" HeaderText="Projeto" SortExpression="IdProjeto" />
                        <asp:BoundField DataField="IdPedidoEspelho" HeaderText="Pedido Conf." SortExpression="IdPedidoEspelho" />
                        <asp:BoundField DataField="NomeClienteLista" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="NomeFuncAbrv" HeaderText="Funcionário" SortExpression="NomeFuncionario" />
                        <asp:BoundField DataField="TelCliente" HeaderText="Tel. Res." SortExpression="TelCliente" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:C}">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataCad" HeaderText="Data" SortExpression="DataCad" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="DescrSituacao" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="Orcamento" IdRegistro='<%# Eval("IdOrcamento") %>' />
                                <a href="#" onclick='TagToTip("orcamento_<%# Eval("IdOrcamento") %>", FADEIN, 300, COPYCONTENT, false, TITLE, "Detalhes", CLOSEBTN, true, CLOSEBTNTEXT, "Fechar", CLOSEBTNCOLORS, ["#cc0000", "#ffffff", "#D3E3F6", "#0000cc"], STICKY, true, FIX, [this, 10, 0]); return false;'>
                                    <img border="0" src="../Images/user_comment.gif" /></a>
                                <div id='orcamento_<%# Eval("IdOrcamento") %>' style="display: none">
                                    <asp:Label ID="Label1" runat="server" Text='<%# "Data de cadastro: " + Eval("DataCad") %>'></asp:Label>
                                    <br />
                                    <asp:Label ID="Label2" runat="server" Text='<%# "Usuário que cadastrou: " + Eval("DescrUsuCad") %>'></asp:Label>
                                    <br />
                                    <asp:Label ID="Label6" runat="server" Text='<%# "Data de alteração: " + Eval("DataAlt") %>'></asp:Label>
                                    <br />
                                    <asp:Label ID="Label7" runat="server" Text='<%# "Usuário que alterou: " + Eval("DescrUsuAlt") %>'></asp:Label>
                                    <br />
                                    <asp:Label ID="Label10" runat="server" Text='<%# "Data da última alteração de preço: " + Eval("DataRecalcular") %>'></asp:Label>
                                </div>
                                <br />
                                <asp:LinkButton ID="lnkGerarPedido" runat="server" CommandArgument='<%# Eval("IdOrcamento") %>'
                                    CommandName="GerarPedido" OnClientClick="return geraPedido(this, '{0}', true)"
                                    Visible='<%# Eval("GerarPedidoVisible") %>' OnLoad="lnkGerarPedido_Load">
                                      <img src="../Images/cart_add.gif" border="0">&nbsp;Gerar Pedido</asp:LinkButton>
                                <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Eval("IdCliente") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbEnviarEmail" runat="server" CommandArgument='<%# Eval("IdOrcamento") %>'
                                    CommandName="EnviarEmail" ImageUrl="~/Images/email.png" ToolTip="Enviar e-mail do orçamento" 
                                    OnClientClick='<%# "return confirmarEnvioEmail();" %>' Visible='<%# Glass.Configuracoes.OrcamentoConfig.MostrarIconeEnvioEmailListagem %>'/>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsOrcamento" runat="server" DataObjectTypeName="Glass.Data.Model.Orcamento"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.OrcamentoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCod" Name="idOrca" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpVendedor" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtTelefone" Name="telefone" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="hdfCidade" Name="idCidade" PropertyName="Value"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtEndereco" Name="endereco" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtComplemento" Name="complemento" PropertyName="Text" />
                        <asp:ControlParameter ControlID="txtBairro" Name="bairro" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValues" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                         <asp:ControlParameter ControlID="ctrlDataIni" Name="dataInicio" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsVendedor" runat="server" SelectMethod="GetVendedoresOrca"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="0" Name="idOrcamento" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource  culture="pt-BR"  ID="odsSituacaoOrca" runat="server" SelectMethod="GetSituacaoOrcamento"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
