<%@ Page Title="Encontro de Contas a Pagar/Receber" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadEncontroContas.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadEncontroContas" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNomeCliente", "input").value = "";
                return false;
            }

            FindControl("txtNomeCliente", "input").value = retorno[1];
        }

        function getFornec(idFornec) {
            if (idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNomeFornec", "input").value = "";
                return false;
            }

            FindControl("txtNomeFornec", "input").value = retorno[1];
        }

        // Validações realizadas ao gerar o encontro
        function onInsertUpdate() {
            if (!validate())
                return false;

            var idCliente = FindControl("txtNumCli", "input").value;
            var idFornec = FindControl("txtNumFornec", "input").value;
            var obs = FindControl("txtObs", "textarea").value;

            if (idCliente == "") {
                alert("Informe o Cliente.");
                return false;
            }

            if (idFornec == "") {
                alert("Informe o Fornecedor.");
                return false;
            }

            var valida = CadEncontroContas.ValidaClienteFornecedor(idCliente, idFornec).value.split(";");

            if (valida[0] == "Erro") {
                alert(valida[1]);
                return false;
            }

            return true;
        }

        function openBuscarContasPg() {

            var idFornecedor = FindControl("hdfIdFornecedor", "input").value;

            if (idFornecedor == "") {
                alert("Nenhum fornecedor selecionado.");
                return false;
            }

            openWindow(600, 800, '../Utils/SelContaPagar.aspx?encontroContas=1&Num=' + idFornecedor);
        }

        function openBuscarContasR() {

            var idCliente = FindControl("hdfIdCliente", "input").value;

            if (idCliente == "") {
                alert("Nenhum cliente selecionado.");
                return false;
            }

            openWindow(600, 800, '../Utils/SelContaReceber.aspx?encontroContas=1&IdCli=' + idCliente);
        }

        function setContaPagar(idContaPg, idCompra, idCustoFixo, idImpostoServ, idFornec, nomeFornec, valorVenc, dataVenc, descrPlanoConta, selContasPgWin) {
            var idEncontro = GetQueryString("idEncontroContas");
            var retorno = CadEncontroContas.AddContaPg(idEncontro, idContaPg).value.split(";");

            if (retorno[0] == "Erro") {
                selContasPgWin.alert(retorno[1]);
                return false;
            }
        }

        function setContaReceber(IdContaR, IdPedido, PedidosLiberacao, NomeCli, ValorVec, DataVec, Juros, Multa, obsScript, descricaoContaContabil, selContasRWin) {
            var idEncontro = GetQueryString("idEncontroContas");
            var retorno = CadEncontroContas.AddContaR(idEncontro, IdContaR).value.split(";");

            if (retorno[0] == "Erro") {
                selContasRWin.alert(retorno[1]);
                return false;
            }
        }

        function finalizar() {
        
            if(!confirm("Deseja finalizar o encontro de contas a pagar/receber?")) return false
        
            bloquearPagina();
            desbloquearPagina(false);

            return true;
        }

    
    </script>

    <table style="width: 100%" align="center">
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvEncontroContas" runat="server" AutoGenerateRows="False" DataKeyNames="IdEncontroContas"
                    DataSourceID="odsEncontroContas" DefaultMode="Insert" GridLines="None">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <table cellspacing="0">
                                    <tr class="dtvAlternatingRow">
                                        <td class="dtvHeader">
                                            Cliente
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                                onblur="getCli(this);" Text='<%# Bind("idCliente") %>'></asp:TextBox>
                                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                                Text='<%# Bind("NomeCliente") %>'></asp:TextBox>
                                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx'); return false;"
                                                ToolTip="Pesquisar" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Fornecedor
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtNumFornec" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                                onblur="getFornec(this);" Text='<%# Bind("idFornecedor") %>'></asp:TextBox>
                                            <asp:TextBox ID="txtNomeFornec" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                                Text='<%# Bind("NomeFornecedor") %>'></asp:TextBox>
                                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                                OnClientClick="openWindow(590, 760, '../Utils/SelFornec.aspx?tipo=antecipFornec'); return false;" />
                                        </td>
                                    </tr>
                                    <tr class="dtvAlternatingRow">
                                        <td class="dtvHeader">
                                            Obs.
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtObs" runat="server" TextMode="MultiLine" Height="50px" Width="400px"
                                                Text='<%# Bind("Obs") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <table>
                                    <tr class="dtvAlternatingRow">
                                        <td class="dtvHeader">
                                            Cliente
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("IdNomeCliente") %>'></asp:Label>
                                            <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Eval("idCliente") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Fornecedor
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label2" runat="server" Text='<%# Eval("IdNomeFornecedor") %>'></asp:Label>
                                            <asp:HiddenField ID="hdfIdFornecedor" runat="server" Value='<%# Eval("idFornecedor") %>' />
                                        </td>
                                    </tr>
                                    <tr class="dtvAlternatingRow">
                                        <td class="dtvHeader">
                                            Obs.
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label3" runat="server" Text='<%# Eval("Obs") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr runat="server" id="trTotalPagar" visible="false">
                                        <td class="dtvHeader">
                                            Total a pagar:
                                        </td>
                                        <td>
                                            <asp:Label ID="lblTotalPagar" runat="server" Text="Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr runat="server" id="trTotalReceber" class="dtvAlternatingRow" visible="false">
                                        <td class="dtvHeader">
                                            Total a receber:
                                        </td>
                                        <td>
                                            <asp:Label ID="lblTotalReceber" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr runat="server" id="trSaldo" visible="false">
                                        <td class="dtvHeader">
                                            Valor Excedente:
                                        </td>
                                        <td>
                                            <asp:Label ID="lblSaldo" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <table>
                                    <tr>
                                        <td>
                                            Data de Vencimento:
                                        </td>
                                        <td>
                                            <uc1:ctrlData ID="ctrlDataVenc" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Button ID="btnOpenContasPG" runat="server" Text="Buscar Contas a Pagar" OnClientClick="openBuscarContasPg();return false;" />
                                        </td>
                                        <td>
                                            &nbsp
                                        </td>
                                        <td>
                                            <asp:Button ID="btnOpenContasR" runat="server" Text="Buscar Contas a Receber" OnClientClick="openBuscarContasR();return false;" />
                                        </td>
                                    </tr>
                                </table>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:GridView ID="grdContaPgEncontroContas" runat="server" AllowPaging="True" AllowSorting="True"
                                                AutoGenerateColumns="False" CssClass="gridStyle" DataKeyNames="IdContaPG, IdEncontroContas"
                                                DataSourceID="odsContaPGEncontroContas" GridLines="None" OnDataBound="grdContaPgEncontroContas_DataBound">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Conta a Pagar" DataField="IdContaPg" />
                                                    <asp:TemplateField HeaderText="Referência">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblReferencia" runat="server" Text='<%# Eval("ContasPg.Referencia") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Parc.">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblParc" runat="server" Text='<%# Eval("ContasPg.DescrNumParc") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Vencimento">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblDataVenc" runat="server" Text='<%# Eval("ContasPg.DataVenc", "{0:d}") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Valor">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblValorVenc" runat="server" Text='<%# Eval("ContasPg.ValorVenc", "{0:C}") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Obs.">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblObs" runat="server" Text='<%# Eval("ContasPg.Obs") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <PagerStyle CssClass="pgr" />
                                                <EditRowStyle CssClass="edit" />
                                                <AlternatingRowStyle CssClass="alt" />
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaPgEncontroContas" runat="server" DataObjectTypeName="Glass.Data.Model.ContasPagarEncontroContas"
                                                DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                                SelectCountMethod="GetListCount" SelectMethod="GetList" SortParameterName="sortExpression"
                                                StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ContasPagarEncontroContasDAO"
                                                UpdateMethod="Update" OnDeleted="odsContaPgEncontroContas_Deleted">
                                                <SelectParameters>
                                                    <asp:QueryStringParameter Name="idEncontroContas" QueryStringField="idEncontroContas"
                                                        Type="UInt32" />
                                                </SelectParameters>
                                            </colo:VirtualObjectDataSource>
                                        </td>
                                    </tr>
                                </table>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:GridView ID="grdContaREncontroContas" runat="server" AllowPaging="True" AllowSorting="True"
                                                AutoGenerateColumns="False" CssClass="gridStyle" DataKeyNames="IdContaR, IdEncontroContas"
                                                DataSourceID="odsContaREncontroContas" GridLines="None" OnDataBound="grdContaREncontroContas_DataBound">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Conta a Receber" DataField="IdContaR" />
                                                    <asp:TemplateField HeaderText="Referência">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblReferencia" runat="server" Text='<%# Eval("ContasR.Referencia") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Parc.">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblParc" runat="server" Text='<%# Eval("ContasR.NumParcString") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Vencimento">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblDataVenc" runat="server" Text='<%# Eval("ContasR.DataVec", "{0:d}") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Valor">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblValorVenc" runat="server" Text='<%# Eval("ContasR.ValorVec", "{0:C}") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Obs.">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblObs" runat="server" Text='<%# Eval("ContasR.Obs") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <PagerStyle CssClass="pgr" />
                                                <EditRowStyle CssClass="edit" />
                                                <AlternatingRowStyle CssClass="alt" />
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaREncontroContas" runat="server" DataObjectTypeName="Glass.Data.Model.ContasReceberEncontroContas"
                                                DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                                SelectCountMethod="GetListCount" SelectMethod="GetList" SortParameterName="sortExpression"
                                                StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ContasReceberEncontroContasDAO"
                                                UpdateMethod="Update" OnDeleted="odsContaREncontroContas_Deleted">
                                                <SelectParameters>
                                                    <asp:QueryStringParameter Name="idEncontroContas" QueryStringField="idEncontroContas"
                                                        Type="UInt32" />
                                                </SelectParameters>
                                            </colo:VirtualObjectDataSource>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                    OnClientClick="if (!onInsertUpdate()) return false;" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" Text="Cancelar"
                                    OnClientClick="redirectUrl(window.location.href); return false" />
                                <asp:HiddenField ID="hdfIdFuncCad" runat="server" Value='<%# Bind("IdFuncCad") %>' />
                                <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Bind("Situacao") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="if (!onInsertUpdate()) return false;" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    Text="Cancelar" OnClick="btnCancelar_Click" />
                                <asp:HiddenField ID="hdfIdFuncCad" runat="server" Value='<%# Bind("IdFuncCad") %>' />
                                <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Bind("Situacao") %>' />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Button ID="btnEditar" runat="server" CommandName="Edit" Text="Editar" />
                                <asp:Button ID="btnFinalizar" runat="server" Text="Finalizar" OnClick="btnFinalizar_Click"
                                    OnClientClick="return finalizar();" />
                                <asp:Button ID="btnVoltar" runat="server" Text="Voltar" OnClick="btnCancelar_Click" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <asp:HiddenField runat="server" ID="hdfContasR" />
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEncontroContas" runat="server" DataObjectTypeName="Glass.Data.Model.EncontroContas"
                    InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.EncontroContasDAO"
                    UpdateMethod="Update" OnInserted="odsEncontroContas_Inserted" OnUpdated="odsEncontroContas_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="IdEncontroContas" QueryStringField="IdEncontroContas"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
