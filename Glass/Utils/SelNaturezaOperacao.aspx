<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelNaturezaOperacao.aspx.cs" Inherits="Glass.UI.Web.Utils.SelNaturezaOperacao"
    Title="Selecione a Natureza da Operação" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        var selecionado = false;

        function setNaturezaOperacao(controle, id, descr)
        {
            if (GetQueryString("buscaComPopup") === "true") {
                var idControle = GetQueryString("id-controle");
                if (idControle) {
                    window.opener.Busca.Popup.atualizar(idControle, null, descr);
                    closeWindow();
                    return;
                }
            }

            if (selecionado)
                return;

            selecionado = true;

            controle = eval("window.opener." + controle);
            controle.AlteraValor(id, descr);
            closeWindow();
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Cód. Natureza Operação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodNaturezaOperacao" runat="server" Width="70px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cód. CFOP" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodCfop" runat="server" Width="70px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricaoCfop" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdNaturezaOperacao" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsNaturezaOperacao" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataKeyNames="IdCfop">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="return setNaturezaOperacao('<%= Request["controle"] %>', '<%# Eval("IdNaturezaOperacao") %>',
                                    '<%# Eval("CodInterno") != "" && Eval("CodInterno") != null ? Eval("CodInterno") : Eval("CodCfop") %>');">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CFOP" SortExpression="CodCfop">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("CodCfop") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="DescricaoCfop">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DescricaoCfop") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CST ICMS" SortExpression="CstIcms">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("CstIcms") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Calcula ICMS" SortExpression="CalcIcms">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkCalcIcms" runat="server" Checked='<%# Bind("CalcIcms") %>' Enabled="false"></asp:CheckBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Calcula ICMS-ST" SortExpression="CalcIcmsSt">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkCalcIcmsSt" runat="server" Checked='<%# Bind("CalcIcmsSt") %>' Enabled="false"></asp:CheckBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Calcula IPI" SortExpression="CalcIpi">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkCalcIpi" runat="server" Checked='<%# Bind("CalcIpi") %>' Enabled="false"></asp:CheckBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Calcula Cofins" SortExpression="CalcCofins">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkCalcCofins" runat="server" Checked='<%# Bind("CalcCofins") %>' Enabled="false"></asp:CheckBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Alterar Estoque Fiscal" SortExpression="AlterarEstoqueFiscal">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkAlterarEstoqueFiscal" runat="server" Checked='<%# Bind("AlterarEstoqueFiscal") %>' Enabled="false"></asp:CheckBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsNaturezaOperacao" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="ObtemNumeroItens"
                    SelectMethod="ObtemLista" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.NaturezaOperacaoDAO" DataObjectTypeName="Glass.Data.Model.NaturezaOperacao">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodNaturezaOperacao" Name="codNaturezaOperacao" PropertyName="Text"
                            Type="String" />
                        <asp:Parameter Name="idCfop" DefaultValue="0" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCodCfop" Name="codigoCfop" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescricaoCfop" Name="descricaoCfop" PropertyName="Text"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
