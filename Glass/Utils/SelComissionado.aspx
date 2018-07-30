<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelComissionado.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SelComissionado" Title="Selecione o Comissionado" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function setComissionado(idComissionado, nome, percentual, situacao)
        {
            // Se for buscar cliente para um pedido, verifica se o mesmo está inativo
            if (situacao == 2 || situacao == "2")
            {
                alert("Comissionado Inativo. " + obs);
                return false;
            }

            if (GetQueryString("buscaComPopup") === "true") {
                var idControle = GetQueryString("id-controle");
                if (idControle) {
                    window.opener.Busca.Popup.atualizar(idControle, idComissionado, nome);
                    closeWindow();
                    return;
                }
            }

            window.opener.setComissionado(idComissionado, nome, percentual, "1");

            closeWindow();
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label4" runat="server" Text="Nome" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" Width="16px" />
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
                <asp:GridView GridLines="None" ID="grdComissionado" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsComissionado" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="A pesquisa não retornou resultados."
                    DataKeyNames="IdComissionado" AllowPaging="True" AllowSorting="True">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="return setComissionado('<%# Eval("IdComissionado") %>', '<%# Eval("Nome") %>', '<%# Eval("Percentual") %>', '<%# Eval("Situacao") %>');">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdComissionado" HeaderText="Num" SortExpression="IdComissionado" />
                        <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                        <asp:BoundField DataField="CpfCnpj" HeaderText="CPF/CNPJ" SortExpression="CpfCnpj" />
                        <asp:BoundField DataField="EnderecoCompleto" HeaderText="Endereço" ReadOnly="True"
                            SortExpression="EnderecoCompleto" />
                        <asp:BoundField DataField="TelRes" HeaderText="Tel. Res" SortExpression="TelRes" />
                        <asp:BoundField DataField="TelCel" HeaderText="Tel. Cel" SortExpression="TelCel" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsComissionado" runat="server" SelectMethod="GetList"
                    TypeName="Glass.Data.DAL.ComissionadoDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNome" Name="nome" PropertyName="Text" Type="String" />
                        <asp:Parameter DefaultValue="0" Name="situacao" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
