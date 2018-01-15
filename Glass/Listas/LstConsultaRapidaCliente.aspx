<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstConsultaRapidaCliente.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstConsultaRapidaCliente" Title="Consulta Rápida de Clientes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function getCli(txtIdCliente) {
            var idCliente = txtIdCliente.value != undefined ? txtIdCliente.value : txtIdCliente;

            if (txtIdCliente.value == undefined)
                FindControl("txtNumCli", "input").value = idCliente;

            var retorno = LstConsultaRapidaCliente.GetCli(idCliente).value.split(';');
            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNomeCliente", "input").value = "";

                return false;
            }

            FindControl("txtNomeCliente", "input").value = retorno[1];
        }

        function exibirProdutos(botao, idGrupo, idSubgrupo) {
            var linha = document.getElementById("produtos_" + idGrupo + "_" + idSubgrupo);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " produtos";
        }

        function openRpt() {
            var idCli = FindControl("txtNumCli", "input").value;
            var situacao = FindControl("drpSituacao", "select").itens();

            if (idCli == 0 || idCli === null)
                return;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ConsultaRapidaCliente&idCli=" + idCli + "&situacao=" + situacao);
        }

    </script>

    <table id="tblPagina">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label3" runat="server" Text="Cliente: " ForeColor="#0066FF"></asp:Label>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeydown="if (isEnter(event)) getCli(this);"
                                onkeypress="return soNumeros(event, true, true);" onblur="getCli(this);">
                            </asp:TextBox>
                            &nbsp;
                            <asp:TextBox ID="txtNomeCliente" runat="server" ReadOnly="True" Width="250px">
                            </asp:TextBox>
                            <asp:LinkButton ID="lnkSelCliente" runat="server" Visible='<%# Eval("ClienteEnabled") %>'
                                OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx?tipo=pedido'); return false;">
                                            <img border="0" src="../Images/Pesquisar.gif" />
                            </asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="11">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label18" runat="server" Text="Situação Sugestão" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <sync:CheckBoxListDropDown ID="drpSituacao" runat="server" CheckAll="True">
                                            <asp:ListItem Value="0">Ativas</asp:ListItem>
                                            <asp:ListItem Value="1">Canceladas</asp:ListItem>
                                        </sync:CheckBoxListDropDown>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            ToolTip="Pesquisar" Style="width: 16px" OnClick="imgPesq_click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td></td>
        </tr>
        <tr>
            <td align="center" style="height: 30px">
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
            </td>
        </tr>
        <tr>
            <td></td>
        </tr>
        <tr>
            <td>
                <table id="tblInfo" runat="server" width="100%">
                    <tr>
                        <td colspan="8" align="center" bgcolor="#D2D2D2">
                            <asp:Label ID="Label1" runat="server" Text="Dados Cadastrais" Font-Bold="True"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label4" runat="server" Text="Código: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblInfoCodCli" runat="server" Text=""></asp:Label>
                        </td>

                        <td align="right">
                            <asp:Label ID="Label6" runat="server" Text="Vendedor: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblInfoVendedor" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="lblContato" runat="server" Text="Contato: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblInfoContato" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label25" runat="server" Text="Telefone de Contato: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblInfoTelefoneContato" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label2" runat="server" Text="Nome: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblInfoNomeCli" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="lblTipoFiscal" runat="server" Text="Tipo Fiscal: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblInfoTipoFiscal" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="lblTipoContribuinte" runat="server" Text="Tipo contribuinte: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblInfoTipoContribuinte" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label5" runat="server" Text="Situação: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblInfoSituacao" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label13" runat="server" Text="Observação: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblObs" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label23" runat="server" Text="Observação Liberação: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblObsLib" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label24" runat="server" Text="Observação NFe: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblObsNFe" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="lblCPF" runat="server" Text="CPF/CNPJ: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblInfoCPF" runat="server" Text=""></asp:Label>
                        </td>

                    </tr>
                    <tr>
                        <td align="right">
                            <asp:Label ID="lblInscEst" runat="server" Text="Insc. Est.: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblInfoInscEst" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="lblCrt" runat="server" Text="CRT: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblInfoCrt" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="lblCnae" runat="server" Text="CNAE: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblInfoCnae" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8" height="20px">&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8" align="center" bgcolor="#D2D2D2">
                            <asp:Label ID="Label16" runat="server" Text="Dados Financeiros" Font-Bold="True"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label8" runat="server" Text="Perc. Sinal Mínimo: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblFinancPercSinalMin" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label10" runat="server" Text="Crédito: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblFinancCredito" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label15" runat="server" Text="Pagar antes da produção: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblPagarAntesProducao" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label17" runat="server" Text="Limite Padrão: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblFinancLimPadrao" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label19" runat="server" Text="Limite Disponível: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblFinancLimDisp" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label7" runat="server" Text="Forma de Pagto Padrão:" Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblFinancPagtoPadrao" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label21" runat="server" Text="Parcela Padrão: " Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblFinancParcPadrao" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8" height="20px"></td>
                    </tr>
                    <tr>
                        <td colspan="4" bgcolor="#E0E0E0" align="center">
                            <asp:Label ID="Label9" runat="server" Text="Formas de Pagamento Disponíveis" Font-Bold="True"></asp:Label>
                            <br />
                        </td>
                        <td colspan="4" bgcolor="#E0E0E0" align="center">
                            <asp:Label ID="Label11" runat="server" Text="Parcelas Disponíveis" Font-Bold="True"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" valign="top">
                            <table>
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="lblFinancPagtoDisp" runat="server" Text=""></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td colspan="4" valign="top">
                            <table>
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="lblFinancParcDisp" runat="server" Text=""></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table align="center" id="tblTabelas" runat="server">
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td bgcolor="#D2D2D2" align="center">
                            <asp:Label ID="lblPedidosBloqueio" runat="server" Text="Pedidos prontos porém não liberados"
                                Font-Bold="True"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 31px">
                            <asp:GridView GridLines="None" ID="grdPedidosBloqueio" runat="server" AutoGenerateColumns="False"
                                DataSourceID="odsPedidosBloqueio" CssClass="gridStyleDestacada" PagerStyle-CssClass="pgr"
                                AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" Width="600px">
                                <Columns>
                                    <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                                    <asp:BoundField DataField="NomeFunc" HeaderText="Vendedor" SortExpression="NomeFunc" />
                                    <asp:BoundField DataField="DataEntrega" HeaderText="Data de Entrega" SortExpression="DataEntrega"
                                        DataFormatString="{0:d}" />
                                    <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:C}" />
                                </Columns>
                                <PagerStyle />
                                <EditRowStyle />
                                <AlternatingRowStyle />
                            </asp:GridView>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td bgcolor="#D2D2D2" align="center">
                            <asp:Label ID="Label12" runat="server" Text="Descontos / Acréscimos do Cliente" Font-Bold="True"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:GridView GridLines="None" ID="grdDescontoAcrescimo" runat="server" AutoGenerateColumns="False"
                                DataSourceID="odsDescontos" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EditRowStyle-CssClass="edit" Width="600px" OnRowDataBound="grdDescontoAcrescimo_RowDataBound">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirProdutos(this, " + Eval("IdGrupoProd") + "," + Eval("IdSubGrupoProd") + "); return false" %>'
                                                ToolTip="Exibir produtos" Visible='<%# Eval("TemOcorrenciasProdutos") %>' Width="10px" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="DescrGrupoSubgrupo" HeaderText="Grupo / Subgrupo" SortExpression="DescrGrupoSubgrupo" />
                                    <asp:BoundField DataField="Desconto" HeaderText="Desconto (%)" SortExpression="Desconto"
                                        DataFormatString="{0:F}" />
                                    <asp:BoundField DataField="Acrescimo" HeaderText="Acrescimo (%)" SortExpression="Acrescimo"
                                        DataFormatString="{0:F}" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            </td> </tr>
                                            <tr id="produtos_<%# Eval("IdGrupoProd") + "_" + Eval("IdSubGrupoProd") %>" style="display: none">
                                                <td></td>
                                                <td colspan="3" style="padding: 0px">
                                                    <asp:GridView ID="grdLimiteCliente" runat="server" AlternatingRowStyle-CssClass="alt"
                                                        AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsDescontosProdutos"
                                                        EditRowStyle-CssClass="edit" GridLines="None" PagerStyle-CssClass="pgr" OnDataBound="grdLimiteCliente_DataBound"
                                                        Width="100%">
                                                        <Columns>
                                                            <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto">
                                                                <ControlStyle Width="60%" />
                                                                <FooterStyle Width="60%" />
                                                                <HeaderStyle Width="60%" />
                                                                <ItemStyle Width="60%" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Desconto" HeaderText="Desconto (%)" SortExpression="Desconto"
                                                                DataFormatString="{0:F}" />
                                                            <asp:BoundField DataField="Acrescimo" HeaderText="Acréscimo (%)" SortExpression="Acrescimo"
                                                                DataFormatString="{0:F}" />
                                                        </Columns>
                                                        <PagerStyle />
                                                        <EditRowStyle />
                                                        <AlternatingRowStyle />
                                                    </asp:GridView>
                                                    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsDescontosProdutos" runat="server" SelectMethod="GetOcorrenciasByClienteGrupoSubgrupo"
                                                        TypeName="Glass.Data.DAL.DescontoAcrescimoClienteDAO" MaximumRowsParameterName=""
                                                        StartRowIndexParameterName="" EnableViewState="False">
                                                        <SelectParameters>
                                                            <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                                                                Type="UInt32" />
                                                            <asp:ControlParameter ControlID="hdfIdGrupo" DefaultValue="" Name="idGrupo" PropertyName="Value"
                                                                Type="UInt32" />
                                                            <asp:ControlParameter ControlID="hdfIdSubGrupo" DefaultValue="" Name="idSubgrupo"
                                                                PropertyName="Value" Type="UInt32" />
                                                        </SelectParameters>
                                                    </colo:VirtualObjectDataSource>
                                                    <asp:HiddenField ID="hdfIdGrupo" runat="server" Value='<%# Eval("IdGrupoProd") %>' />
                                                    <asp:HiddenField ID="hdfIdSubGrupo" runat="server" Value='<%# Eval("IdSubGrupoProd") %>' />
                                                    <br />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle />
                                <EditRowStyle />
                                <AlternatingRowStyle />
                            </asp:GridView>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td bgcolor="#D2D2D2" align="center">
                            <asp:Label ID="Label22" runat="server" Text="Débitos do Cliente" Font-Bold="True"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:GridView GridLines="None" ID="grdLimiteCliente" runat="server" AutoGenerateColumns="False"
                                DataSourceID="odsLimiteCliente" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                                AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" Width="600px">
                                <Columns>
                                    <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                                    <asp:BoundField DataField="IdLiberarPedido" HeaderText="Liberação" SortExpression="IdLiberarPedido" />
                                    <asp:BoundField DataField="IdNomeCli" HeaderText="Cliente" SortExpression="IdNomeCli" />
                                    <asp:BoundField DataField="ValorVec" HeaderText="Débito" SortExpression="ValorVec"
                                        DataFormatString="{0:c}" />
                                    <asp:BoundField DataField="DataVecString" HeaderText="Data Venc."
                                        SortExpression="DataVec" />
                                    <asp:BoundField DataField="DescrPlanoConta" HeaderText="Referente a" SortExpression="DescrPlanoConta" />
                                </Columns>
                                <PagerStyle />
                                <EditRowStyle />
                                <AlternatingRowStyle />
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" nowrap="nowrap">
                            <asp:Label ID="Label20" runat="server" Text="Total débitos" Font-Bold="True"></asp:Label>
                            <asp:Label ID="lblTotalDebitos" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLimiteCliente" runat="server" SelectMethod="GetDebitosList"
                                TypeName="Glass.Data.DAL.ContasReceberDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                SelectCountMethod="GetDebitosCount" SortParameterName="sortExpression"
                                StartRowIndexParameterName="startRow">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                                        Type="UInt32" />
                                    <asp:Parameter DefaultValue="0" Name="idPedido" Type="UInt32" />
                                    <asp:Parameter DefaultValue="0" Name="idLiberarPedido" Type="UInt32" />
                                    <asp:Parameter Name="buscarItens" Type="String" />
                                    <asp:Parameter Name="ordenar" Type="Int32" />
                                    <asp:Parameter Name="tipoBuscaData" Type="Int32" />
                                    <asp:Parameter Name="dataIni" Type="String" />
                                    <asp:Parameter Name="dataFim" Type="String" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsDescontos" runat="server" SelectMethod="GetOcorrenciasByCliente"
                                TypeName="Glass.Data.DAL.DescontoAcrescimoClienteDAO">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                                        Type="UInt32" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedidosBloqueio" runat="server" SelectMethod="GetPedidosBloqueioEmissaoByCliente"
                                TypeName="Glass.Data.DAL.PedidoDAO">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                                        Type="UInt32" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td bgcolor="#D2D2D2" align="center">
                            <asp:Label ID="Label14" runat="server" Text="Sugestões/Reclamações do Cliente" Font-Bold="True"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:GridView GridLines="None" ID="grdSugestao" runat="server" AllowPaging="False"
                                AllowSorting="False" AutoGenerateColumns="False"
                                DataSourceID="odsSugestao" CssClass="gridStyle"
                                PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                                DataKeyNames="IdSugestao" EmptyDataText="Nenhuma sugestão encontrada" Style="margin-right: 0px"
                                OnRowCommand="grdSugestao_RowCommand"
                                OnRowDataBound="grdSugestao_RowDataBound" Width="100%">
                                <Columns>
                                    <asp:TemplateField ShowHeader="False">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkExcluir" runat="server" CausesValidation="False" CommandName="Cancelar"
                                                CommandArgument='<%# Eval("IdSugestao") %>' OnClientClick="return confirm(&quot;Tem certeza que deseja cancelar esta sugestão/reclamação?&quot;);"
                                                Text=""><img src="../Images/ExcluirGrid.gif" style="border:none" title="Cancelar" /></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="IdSugestao" HeaderText="Cód." SortExpression="IdSugestao" />
                                    <asp:TemplateField HeaderText="Cliente" SortExpression="IdCliente">
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("IdCliente") + " - " + Eval("NomeCliente") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="DataCad" HeaderText="Data" SortExpression="DataCad" />
                                    <asp:BoundField DataField="DescrUsuCad" HeaderText="Funcionário" SortExpression="DescrUsuCad" />
                                    <asp:BoundField DataField="DescrTipoSugestao" HeaderText="Tipo" ReadOnly="True" SortExpression="DescrTipoSugestao" />
                                    <asp:BoundField DataField="Descricao" HeaderText="Descricao" SortExpression="Descricao" />
                                </Columns>
                                <PagerStyle />
                                <EditRowStyle />
                                <AlternatingRowStyle />
                            </asp:GridView>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSugestao" runat="server" MaximumRowsParameterName="pageSize"
                                SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                                StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.SugestaoClienteDAO"
                                EnablePaging="True">
                                <SelectParameters>
                                    <asp:Parameter Name="idSugestao" Type="UInt32" />
                                    <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                                        Type="UInt32" />
                                    <asp:Parameter Name="idFunc" Type="UInt32" />
                                    <asp:Parameter Name="nomeFunc" Type="String" />
                                    <asp:Parameter Name="nomeCli" Type="String" />
                                    <asp:Parameter Name="dataIni" Type="String" />
                                    <asp:Parameter Name="dataFim" Type="String" />
                                    <asp:Parameter Name="tipo" Type="Int32" />
                                    <asp:Parameter Name="descr" Type="String" />
                                    <asp:ControlParameter ControlID="drpSituacao" DefaultValue="" Name="situacao" PropertyName="SelectedValue" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt();"><img alt="" border="0" src="../Images/printer.png" />Imprimir</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
