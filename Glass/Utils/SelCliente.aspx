<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelCliente.aspx.cs" Inherits="Glass.UI.Web.Utils.SelCliente"
    Title="Selecione o Cliente" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString"
    TagPrefix="uc1" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        var selecionado = false;

        function validateSetCliente(idCli, nome, cpfCnpj, suframa, situacao, credito, obsNfe)
        {
            if (selecionado)
                return;

            selecionado = true;

            var isPedido = FindControl('ctrlTipo_hdfLink', 'input').value == "pedido";
            var isOrcamento = FindControl('ctrlTipo_hdfLink', 'input').value == "orcamento";
            var isProjeto = FindControl('ctrlTipo_hdfLink', 'input').value == "projeto";
            var isObra = FindControl('ctrlTipo_hdfLink', 'input').value == "obra";
            var isGerarNfe = FindControl('ctrlTipo_hdfLink', 'input').value == "gerarNfe";
            var permitirClienteInativo = isOrcamento && <%=Glass.Configuracoes.OrcamentoConfig.TelaCadastro.PermitirInserirClienteInativoBloqueado.ToString().ToLower() %>;
            var permitirClienteBloqueado = isOrcamento && <%=Glass.Configuracoes.OrcamentoConfig.TelaCadastro.PermitirInserirClienteInativoBloqueado.ToString().ToLower() %>;

            // Se for buscar cliente para um pedido ou obra, verifica se o mesmo está inativo
            if ((isPedido || isProjeto || isOrcamento || isObra) &&
                (situacao != 1 && situacao != "1" && !permitirClienteBloqueado && !permitirClienteInativo))
            {
                alert("Cliente inativo. ");
                selecionado = false;
                return false;
            }

            if (GetQueryString("buscaComPopup") === "true") {
                var idControle = GetQueryString("id-controle");
                if (idControle) {
                    window.opener.Busca.Popup.atualizar(idControle, idCli, nome);
                    closeWindow();
                    return;
                }
            }

            if (FindControl("hdfCustom", "input").value == 1)
                window.opener.setClienteCustom(idCli, nome);
            else if (FindControl("hdfNfe", "input").value == 1) // Se for busca de cliente para NF-e
                window.opener.setClienteNfe(idCli, nome, cpfCnpj, suframa, obsNfe);
            else if (FindControl("hdfNfe", "input").value == 2) // Se for busca de cliente para NF-e
                window.opener.setClienteEmit(idCli, nome, cpfCnpj);
            else if (FindControl("hdfChequeDev", "input").value == 1)
                window.opener.setClienteChequeDev(idCli, nome);
            else if (FindControl("hdfControleFormaPagto", "input").value != "")
                window.opener.setCliFormaPagto(FindControl("hdfControleFormaPagto", "input").value, idCli, nome, credito);
            else if ('<%= Request["callback"] %>' == "setForPopup")
                eval("window.opener." + '<%= Request["controle"] %>').AlteraValor(idCli, idCli);
            else if ('<%= Request["callback"] %>' == "setForPart")
                window.opener.ctrlSelParticipante_setCliente(idCli, '<%= Request["controle"] %>');
            else if ('<%= Request["callback"] %>' == "participanteFiscal")
                window.opener.ControleSelecaoParticipanteFiscal.selecionar('<%= Request["controle"] %>', idCli, nome);
            else if (FindControl("hdfDadosCliente", "input").value == 1)
            {
                var dadosCli = "";

                if (FindControl("hdfMedicao", "input").value != 1) {
                    dadosCli = MetodosAjax.GetDadosCli(idCli).value;
                }
                else {
                    dadosCli = SelCliente.GetCliMedicao(idCli).value;
                }

                try {
                    dadosCli = dadosCli.split('|');
                    window.opener.setDadosCliente(dadosCli[0], dadosCli[1], dadosCli[2], dadosCli[3], dadosCli[4], dadosCli[5],
                        dadosCli[6], dadosCli[7], idCli, dadosCli[8], dadosCli[9]);
                }
                catch (err) {
                    closeWindow();
                }
            }
            else if ((isPedido || isProjeto || isGerarNfe) && window.opener.getCli != undefined)
                window.opener.getCli(idCli);
            else
                window.opener.setCliente(idCli, nome, window);

            closeWindow();
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:Panel ID="panPesquisar" runat="server" BorderColor="#EDE9DA" BorderWidth="2"
                    HorizontalAlign="Left" Style="white-space: nowrap" Width="377px">
                    <span style="display: block; text-align: center; font-weight: bold; background-color: #5D7B9D;
                        color: #FFFFFF;">Pesquisar</span>
                    <div style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px; padding-top: 4px">
                        <table border="0" cellspacing="1">
                            <tr>
                                <td align="left">
                                    Cód. Cliente
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtCodigo" runat="server" onkeypress="return soNumeros(event, true, true);"
                                        onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                </td>
                                <td align="left">
                                    <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                        ToolTip="Pesquisar" />
                                    <asp:ImageButton ID="imgExcluirFiltro" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                        OnClick="imgExcluirFiltro_Click" ToolTip="Remover filtro" />
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    Nome/Apelido
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtNome" runat="server" Width="250px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                </td>
                                <td align="left">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    Bairro
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtBairro" runat="server" Width="250px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                </td>
                                <td align="left">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    CPF/CNPJ
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtCpf" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                </td>
                                <td align="left">
                                    <asp:LinkButton ID="LnkCliente" runat="server" OnClick="LnkCliente_Click"><img
                            border="0" src="../Images/Insert.gif" /></asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:Panel>
                <br />
                <asp:GridView GridLines="None" ID="grdCliente" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsCliente" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="A pesquisa não retornou resultados."
                    DataKeyNames="IdCli" AllowPaging="True" AllowSorting="True">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="return validateSetCliente('<%# Eval("IdCli") %>', '<%# Eval("Nome") %>', '<%# Eval("CpfCnpj") %>', '<%# Eval("Suframa") %>', '<%# Eval("Situacao") %>', '<%# Eval("Credito") %>', '<%# (Eval("ObsNfe") ?? String.Empty).ToString().Replace("\r", "").Replace("\n", "\\n").Replace("'", " ").Replace("\"", " ") %>');">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdCli" HeaderText="Num" SortExpression="IdCli" />
                        <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                        <asp:BoundField DataField="CpfCnpj" HeaderText="CPF/CNPJ" SortExpression="CpfCnpj" />
                        <asp:BoundField DataField="EnderecoCompleto" HeaderText="Endereço" ReadOnly="True"
                            SortExpression="EnderecoCompleto" />
                        <asp:BoundField DataField="TelCont" HeaderText="Tel. Cont." SortExpression="TelCont" />
                        <asp:BoundField DataField="TelRes" HeaderText="Tel. Res" SortExpression="TelRes" />
                        <asp:BoundField DataField="TelCel" HeaderText="Tel. Cel" SortExpression="TelCel" />
                        <asp:BoundField DataField="LimiteDisp" HeaderText="Limite disp." SortExpression="LimiteDisp" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <asp:Label ID="lblAviso" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCliente" runat="server" DataObjectTypeName="Glass.Data.Model.Cliente"
                    DeleteMethod="Delete" SelectMethod="GetForSel" TypeName="Glass.Data.DAL.ClienteDAO"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCountSel"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodigo" Name="codCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtNome" Name="nome" PropertyName="Text" Type="String" />
                        <asp:Parameter Name="codRota" Type="String" />
                        <asp:Parameter Name="idFunc" Type="UInt32" />
                        <asp:Parameter Name="endereco" Type="String" />
                        <asp:ControlParameter ControlID="txtBairro" Name="bairro" PropertyName="Text" Type="String" />
                        <asp:Parameter Name="telefone" Type="String" />
                        <asp:ControlParameter ControlID="txtCpf" Name="cpfCnpj" PropertyName="Text" Type="String" />
                        <asp:ControlParameter Name="situacao" ControlID="hdfSituacaoBusca" Type="Int32" />
                        <asp:QueryStringParameter DefaultValue="false" Name="isRota" QueryStringField="rota"
                            Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <uc1:ctrlLinkQueryString ID="ctrlTipo" runat="server" NameQueryString="tipo" />
                <asp:HiddenField ID="hdfNfe" runat="server" />
                <asp:HiddenField ID="hdfSituacaoBusca" runat="server" />
                <asp:HiddenField ID="hdfChequeDev" runat="server" />
                <asp:HiddenField ID="hdfDadosCliente" runat="server" />
                <asp:HiddenField ID="hdfMedicao" runat="server" />
                <asp:HiddenField ID="hdfCustom" runat="server" />
                <asp:HiddenField ID="hdfControleFormaPagto" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
