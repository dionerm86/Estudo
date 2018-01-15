<%@ Page Title="Retificar Encontro de Contas a Pagar/Receber" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstRetificarEncontroContas.aspx.cs" Inherits="Glass.UI.Web.Listas.LstRetificarEncontroContas" %>

<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function atualizaContaPg(idContaPg, adicionar) {
            var controle = FindControl("hdfIdsContasPg", "input");
            var idsContasPg = controle.value.split(',');
            var nova = new Array();

            for (j = 0; j < idsContasPg.length; j++) {
                if (idsContasPg[j] != idContaPg && idsContasPg[j] != "")
                    nova.push(idsContasPg[j]);
            }

            if (adicionar)
                nova.push(idContaPg);

            controle.value = nova.join(",");
        }

        function atualizaContaR(idContaR, adicionar) {
            var controle = FindControl("hdfIdsContasR", "input");
            var idsContaR = controle.value.split(',');
            var nova = new Array();

            for (j = 0; j < idsContaR.length; j++) {
                if (idsContaR[j] != idContaR && idsContaR[j] != "")
                    nova.push(idsContaR[j]);
            }

            if (adicionar)
                nova.push(idContaR);

            controle.value = nova.join(",");
        }

        function selecionaTodosContaPg() {
            var selecionar = FindControl("chkSelecionarTodosContaPg", "input");
            var inputs = FindControl("grdContaPgEncontroContas", "table").getElementsByTagName("input");

            for (i = 0; i < inputs.length; i++) {
                if (inputs[i].id != selecionar.id) {
                    inputs[i].checked = selecionar.checked;
                    inputs[i].onclick();
                }
            }
        }

        function selecionaTodosContaR() {
            var selecionar = FindControl("chkSelecionarTodosContaR", "input");
            var inputs = FindControl("grdContaREncontroContas", "table").getElementsByTagName("input");

            for (i = 0; i < inputs.length; i++) {
                if (inputs[i].id != selecionar.id) {
                    inputs[i].checked = selecionar.checked;
                    inputs[i].onclick();
                }
            }
        }

        function validar() {
            var numContasPg = FindControl("grdContaPgEncontroContas", "table").rows.length - 1;
            var numContasR = FindControl("grdContaREncontroContas", "table").rows.length - 1;
            var idsContasPg = FindControl("hdfIdsContasPg", "input").value;
            var idsContasR = FindControl("hdfIdsContasR", "input").value;

            var numContasRemPg = idsContasPg.split(','),
                numContasRemR = idsContasR.split(',');

            var j;
            for (var i = 0, j = 0; i < numContasRemPg.length; i++)
                j += numContasRemPg[i] != "" ? 1 : 0;

            numContasRemPg = j;

            for (var i = 0, j = 0; i < numContasRemR.length; i++)
                j += numContasRemR[i] != "" ? 1 : 0;

            numContasRemR = j;

            if (idsContasPg == "" && idsContasR == "") {
                alert("Selecione pelo menos 1 conta a pagar ou receber para remover do encontro de contas.");
                return false;
            }

            if (numContasRemPg == numContasPg)
            {
                alert("Mantenha ao menos 1 conta a pagar no encontro de contas.\nPara que todas as contas a pagar sejam removidas, cancele o encontro de contas.");
                return false;
            }

            if (numContasRemR == numContasR)
            {
                alert("Mantenha ao menos 1 conta a receber no encontro de contas.\nPara que todas as contas a receber sejam removidas, cancele o encontro de contas.");
                return false;
            }

            bloquearPagina();
            desbloquearPagina(false);
            return true;
        }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Encontro Contas" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <uc1:ctrlSelPopup ID="selEncontroContas" runat="server" ColunasExibirPopup="IdEncontroContas|IdNomeCliente|IdNomeFornecedor|ValorPagar|ValorReceber|valorExcedente"
                                DataSourceID="odsEncontroContas" DataTextField="IdEncontroContas" DataValueField="IdEncontroContas"
                                FazerPostBackBotaoPesquisar="True" PermitirVazio="False" TituloTela="Selecione o Encontro de Contas"
                                ValidationGroup="id" TitulosColunas="Cód|Cliente|Fornecedor|Contas a Pagar|Contas a Receber|Valor Excedente"
                                TextWidth="80px" ExibirIdPopup="true" />
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
                <table runat="server" id="tbDtVenc">
                    <tr>
                        <td>
                            Data de Vencimento:
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataVenc" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr> 
        <tr runat="server" id="captionContasPg">
            <td align="center">
                Selecione as contas a pagar que serão removidas do encontro de contas
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdContaPgEncontroContas" runat="server" AutoGenerateColumns="False"
                    CssClass="gridStyle" DataKeyNames="IdContaPG,IdEncontroContas" DataSourceID="odsContaPgEncontroContas"
                    GridLines="None" OnDataBound="grdContaPgEncontroContas_DataBound">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelecionarContaPg" runat="server" onclick='<%# Eval("IdContaPg", "atualizaContaPg({0}, this.checked)") %>' />
                            </ItemTemplate>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkSelecionarTodosContaPg" runat="server" onclick="selecionaTodosContaPg()" />
                            </HeaderTemplate>
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
        <tr runat="server" id="captionContasR">
            <td align="center">
                Selecione as contas a receber que serão removidas do encontro de contas
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdContaREncontroContas" runat="server" AutoGenerateColumns="False"
                    CssClass="gridStyle" DataKeyNames="IdContaR,IdEncontroContas" DataSourceID="odsContaREncontroContas"
                    GridLines="None">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelecionarContaR" runat="server" onclick='<%# Eval("IdContaR", "atualizaContaR({0}, this.checked)") %>' />
                            </ItemTemplate>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkSelecionarTodosContaR" runat="server" onclick="selecionaTodosContaR()" />
                            </HeaderTemplate>
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
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEncontroContas" runat="server" SelectMethod="GetForRetificar"
                    TypeName="Glass.Data.DAL.EncontroContasDAO" >
                    <SelectParameters>
                        <asp:Parameter DefaultValue="2" Name="situacao" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaPgEncontroContas" runat="server" DataObjectTypeName="Glass.Data.Model.ContasPagarEncontroContas"
                    SelectMethod="GetByIdEncontroContas" TypeName="Glass.Data.DAL.ContasPagarEncontroContasDAO">
                    <SelectParameters>
                        <asp:ControlParameter Name="idEncontroContas" ControlID="SelEncontroContas" PropertyName="Valor"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaREncontroContas" runat="server" DataObjectTypeName="Glass.Data.Model.ContasReceberEncontroContas"
                    SelectMethod="GetByIdEncontroContas" TypeName="Glass.Data.DAL.ContasReceberEncontroContasDAO">
                    <SelectParameters>
                        <asp:ControlParameter Name="idEncontroContas" ControlID="SelEncontroContas" PropertyName="Valor"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfIdEncontroContas" runat="server" />
                <asp:HiddenField ID="hdfIdsContasPg" runat="server" />
                <asp:HiddenField ID="hdfIdsContasR" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnRetificarEncontroContas" runat="server" OnClick="btnRetificarEncontroContas_Click"
                    Text="Retificar Encontro Contas" OnClientClick="if (!validar()) return false" />
            </td>
        </tr>
    </table>
</asp:Content>
