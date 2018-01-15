<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelCidade.aspx.cs" Inherits="Glass.UI.Web.Utils.SelCidade"
    Title="Selecione a Cidade" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function setCidade(idCidade, nomeCidade, nomeUf)
        {
            if ('<%= Request["controleTxt"] %>' != null && '<%= Request["controleTxt"] %>' != "")
            {
                window.opener.setCidade(idCidade, nomeCidade, '<%= Request["controleTxt"] %>', '<%= Request["controleHdf"] %>');
            }
            else if ('<%= Request["retUf"] %>' != "1")
                window.opener.setCidade(idCidade, nomeCidade);
            else
                window.opener.setCidade(idCidade, nomeCidade, nomeUf);

            closeWindow();
        }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Cidade" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCidade" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" OnClick="lnkPesq_Click" ImageUrl="~/Images/Pesquisar.gif" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="UF" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpUf" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem>MG</asp:ListItem>
                                <asp:ListItem>AC</asp:ListItem>
                                <asp:ListItem>AL</asp:ListItem>
                                <asp:ListItem>AM</asp:ListItem>
                                <asp:ListItem>AP</asp:ListItem>
                                <asp:ListItem>BA</asp:ListItem>
                                <asp:ListItem>CE</asp:ListItem>
                                <asp:ListItem>DF</asp:ListItem>
                                <asp:ListItem>ES</asp:ListItem>
                                <asp:ListItem>GO</asp:ListItem>
                                <asp:ListItem>MA</asp:ListItem>
                                <asp:ListItem>MG</asp:ListItem>
                                <asp:ListItem>MS</asp:ListItem>
                                <asp:ListItem>MT</asp:ListItem>
                                <asp:ListItem>PB</asp:ListItem>
                                <asp:ListItem>PA</asp:ListItem>
                                <asp:ListItem>PE</asp:ListItem>
                                <asp:ListItem>PI</asp:ListItem>
                                <asp:ListItem>PR</asp:ListItem>
                                <asp:ListItem>RJ</asp:ListItem>
                                <asp:ListItem>RN</asp:ListItem>
                                <asp:ListItem>RO</asp:ListItem>
                                <asp:ListItem>RR</asp:ListItem>
                                <asp:ListItem>RS</asp:ListItem>
                                <asp:ListItem>SC</asp:ListItem>
                                <asp:ListItem>SP</asp:ListItem>
                                <asp:ListItem>SE</asp:ListItem>
                                <asp:ListItem>TO</asp:ListItem>
                            </asp:DropDownList>
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
                <asp:Button ID="btnFechar" runat="server" Text="Fechar" OnClientClick="closeWindow();" />
                <br />
                <br />
                <asp:GridView GridLines="None" ID="grdCidade" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsCidade" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataKeyNames="IdCidade"
                    EmptyDataText="Nenhuma cidade encontrada.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="setCidade('<%# Eval("IdCidade") %>', '<%# Eval("NomeCidadeString") %>', '<%# Eval("NomeUf") %>');">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="NomeCidade" HeaderText="Cidade" SortExpression="NomeCidade" />
                        <asp:BoundField DataField="CodIbgeCidade" HeaderText="Cód. IBGE" SortExpression="CodIbgeCidade" />
                        <asp:BoundField DataField="NomeUf" HeaderText="UF" SortExpression="NomeUf" />
                        <asp:BoundField DataField="CodIbgeUf" HeaderText="Cód. IBGE UF" SortExpression="CodIbgeUf" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCidade" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.CidadeDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpUf" Name="uf" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="txtCidade" Name="cidade" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
