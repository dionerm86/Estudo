<%@ Page Title="Instalações" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstInstalacao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstInstalacao" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register src="../Controls/ctrlLogCancPopup.ascx" tagname="ctrlLogCancPopup" tagprefix="uc2" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>

    <script type="text/javascript">

        function openRptPedido(idPedido, tipo) {
            openWindow(600, 800, "../Relatorios/RelPedido.aspx?idPedido=" + idPedido + "&tipo=" + tipo);
            return false;
        }

        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = LstInstalacao.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtCliente", "input").value = "";
                return false;
            }

            FindControl("txtCliente", "input").value = retorno[1];
        }

        // Abre relatório das ordens de instalação
        function openRpt(exportarExcel) {
            var idOrdemInst = FindControl("txtOrdemInst", "input").value;
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idOrcamento = FindControl("txtIdOrcamento", "input").value;
            var idEquipe = FindControl("drpEquipe", "select").value;
            var tiposInstalacao = FindControl("cbdTipoColocacao", "select").itens();
            var situacoes = FindControl("cbdSituacao", "select").itens();
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var dataIniEnt = FindControl("ctrlDataIniEnt_txtData", "input").value;
            var dataFimEnt = FindControl("ctrlDataFimEnt_txtData", "input").value;
            var dataIniOrdemInst = FindControl("ctrlDataIniOrdemInst_txtData", "input").value;
            var dataFimOrdemInst = FindControl("ctrlDataFimOrdemInst_txtData", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var nomeCliente = FindControl("txtCliente", "input").value;
            var observacao = FindControl("txtObservacao", "input").value;
            var telefone = FindControl("txtTelefone", "input").value;
            var agrupar = FindControl("drpAgruparImpressao", "select").value;

            if (idOrdemInst == "")
                idOrdemInst = 0;

            if (idPedido == "")
                idPedido = 0;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=ListaInstalacao&IdOrdemInst=" + idOrdemInst + "&idPedido=" + idPedido +
                "&idOrcamento=" + idOrcamento + "&idEquipe=" + idEquipe + "&tiposInstalacao=" + tiposInstalacao + "&situacoes=" + situacoes +
                "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&dataIniEnt=" + dataIniEnt + "&dataFimEnt=" + dataFimEnt + "&idLoja=" + idLoja +
                "&exportarExcel=" + exportarExcel + "&dataIniOrdemInst=" + dataIniOrdemInst + "&dataFimOrdemInst=" + dataFimOrdemInst +
                "&nomeCliente=" + nomeCliente + "&telefone=" + telefone + "&observacao=" + observacao +
                "&agrupar=" + agrupar);
                
            return false;
        }
        
        function cancelar(idInstalacao) {
        if (!confirm('Tem certeza que deseja cancelar esta Instalação?'))
            return false;

        var obs = "";

        while (obs == "")
            obs = prompt("Motivo:");

        var retorno = LstInstalacao.Cancelar(idInstalacao, obs).value.split('|');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            return false;
        }
        else
            alert(retorno[0]);
        
        //refresh na pagina
        redirectUrl(window.location.href);
    }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Ordem Inst." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtOrdemInst" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" Width="16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Num. Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="lblIdOrcamento" runat="server" Text="Orçamento" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdOrcamento" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Equipe" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpEquipe" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsEquipe" DataTextField="NomeEstendido" DataValueField="IdEquipe">
                                <asp:ListItem Value="0">TODAS</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Tipo Instalação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdTipoColocacao" runat="server" Title="Selecione o tipo"
                                DataSourceID="odsTipoInstalacao" DataTextField="Descr" DataValueField="Id">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdSituacao" runat="server" Title="Selecione a situação"
                                DataSourceID="odsSituacaoInstalacao" DataTextField="Descr" DataValueField="Id">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" Width="16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Período Ordem Inst." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIniOrdemInst" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFimOrdemInst" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="True" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Período Entrega" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIniEnt" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFimEnt" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtCliente" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('pesqCli', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="pesqCli" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label17" runat="server" ForeColor="#0066FF" Text="Telefone"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtTelefone" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                Width="80px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label14" runat="server" Text="Observação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtObservacao" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('pesqCli', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label15" runat="server" Text="Agrupar impressão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpAgruparImpressao" runat="server">
                                <asp:ListItem Value="0">Nenhum</asp:ListItem>
                                <asp:ListItem Value="1">Tipo de instalação</asp:ListItem>
                                <asp:ListItem Value="2">Orçamento</asp:ListItem>
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
                <asp:GridView GridLines="None" ID="grdColocacoes" runat="server" AllowPaging="True"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" AutoGenerateColumns="False" DataSourceID="odsInstalacao"
                    DataKeyNames="IdInstalacao" EmptyDataText="Nenhuma instalação encontrada." AllowSorting="True">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>

                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                <asp:ImageButton ID="imbCancelarFinalizacao" runat="server" 
                                    ImageUrl="../Images/ExcluirGrid.gif" 
                                    OnClientClick='<%# "cancelar(" + Eval("IdInstalacao") +"); return false;" %>' 
                                    ToolTip="Cancelar Finalização" 
                                    Visible='<%# Bind("CancelarFinalizadaVisible") %>' />
                                <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Eval("ExibirImpressaoPcp") %>'>
                                    <a href="#" onclick='openRptPedido(&#039;<%# Eval("IdPedido") %>&#039;, 2);'>
                                        <img border="0" src="../Images/page_gear.png" title="Pedido PCP" /></a>
                                </asp:PlaceHolder>
                                <a href="#" onclick="openRptPedido('<%# Eval("IdPedido") %>', 0);">
                                    <img border="0" src="../Images/Relatorio.gif" title="Visualizar Pedido" /></a>
                                <asp:ImageButton ID="imgPedidoInstalacao" runat="server" ImageUrl="~/Images/Nota.gif"
                                    Visible='<%# Eval("ImprimirOrdemInstVisible") %>' OnClientClick='<%# "openWindow(600, 800, \"../Relatorios/RelBase.aspx?rel=PedidoInstalacao&idInstalacao=" + Eval("IdInstalacao") + "\"); return false" %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="false" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pedido" SortExpression="IdPedido">
                            <EditItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("IdPedido") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("IdPedido") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Orçamento" SortExpression="IdOrcamento">
                            <EditItemTemplate>
                                <asp:Label ID="lblIdOrcamento" runat="server" Text='<%# Bind("IdOrcamento") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblIdOrcamento" runat="server" Text='<%# Bind("IdOrcamento") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor do pedido" SortExpression="ValorPedido">
                            <EditItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Eval("ValorPedido", "{0:c}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("ValorPedido", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ordem Inst." SortExpression="IdOrdemInstalacao">
                            <EditItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("IdOrdemInstalacao") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("IdOrdemInstalacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Ordem Inst." SortExpression="DataOrdemInstalacao">
                            <EditItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("DataOrdemInstalacao") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("DataOrdemInstalacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Cli." SortExpression="IdCliente">
                            <ItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("IdCliente") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("IdCliente") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cliente" SortExpression="NomeCliente">
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("NomeCliente") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("NomeCliente") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Local" SortExpression="LocalObra">
                            <EditItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("LocalObra") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("LocalObra") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Equipes" SortExpression="NomesEquipes">
                            <EditItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("NomesEquipes") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("NomesEquipes") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Inst." SortExpression="DataInstalacao">
                            <EditItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("DataInstalacao", "{0:d}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("DataInstalacao", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Final." SortExpression="DataFinal">
                            <EditItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("DataFinal", "{0:d}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("DataFinal", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Entrega" SortExpression="DataEntrega">
                            <EditItemTemplate>
                                <uc1:ctrlData ID="ctrlDataEntrega" runat="server" ReadOnly="ReadWrite" DataString='<%# Bind("DataEntrega") %>'
                                    ValidateEmptyText="true" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DataEntrega", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="DescrTipoInstalacao">
                            <EditItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("DescrTipoInstalacao") %>'></asp:Label>
                                <asp:DropDownList ID="drpTipoInstalacao" runat="server" DataSourceID="odsTipoInstalacao"
                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoInstalacao") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("DescrTipoInstalacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="DescrSituacao">
                            <EditItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Eval("DescrSituacao") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("DescrSituacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:PlaceHolder ID="pchObs" runat="server" Visible='<%# Eval("Obs") != null %>'><a
                                    id="lnkObs" href="#" onmouseover='Tip(&quot;<%# Eval("Obs") %>&quot;);' onmouseout="UnTip();">
                                    <img border="0" src="../Images/blocodenotas.png" /></a> </asp:PlaceHolder>
                               
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField>
                            <ItemTemplate>
                                <uc2:ctrlLogCancPopup ID="ctrlLogPopup1" runat="server" Tabela="FinalizacaoInstalacao" IdRegistro='<%# Eval("IdInstalacao") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <a id="lnkImprimir" href="#" onclick="return openRpt(false);">
                    <img alt="" border="0" src="../Images/printer.png" />
                    Imprimir</a>&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;">
                    <img border="0" src="../Images/Excel.gif" />Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsInstalacao" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.InstalacaoDAO" SortParameterName="sortExpression" DataObjectTypeName="Glass.Data.Model.Instalacao"
                    UpdateMethod="AtualizaDataEntregaSituacao" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtOrdemInst" Name="idOrdemInstalacao" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtIdOrcamento" Name="idOrcamento" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpEquipe" Name="idEquipe" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="cbdTipoColocacao" Name="tiposInstalacao" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="cbdSituacao" Name="situacoes" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniEnt" Name="dataIniEnt" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimEnt" Name="dataFimEnt" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniOrdemInst" Name="dataIniOrdemInst" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimOrdemInst" Name="dataFimOrdemInst" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCliente" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtTelefone" Name="telefone" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtObservacao" Name="observacao" PropertyName="Text"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoInstalacao" runat="server" SelectMethod="GetTipoInstalacao"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSituacaoInstalacao" runat="server" SelectMethod="GetSituacaoInstalacao"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEquipe" runat="server" SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.EquipeDAO"
                    MaximumRowsParameterName="" StartRowIndexParameterName="">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
