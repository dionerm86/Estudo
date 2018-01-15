<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelOrcamento.aspx.cs" Inherits="Glass.UI.Web.Utils.SelOrcamento"
    Title="Selecione o Orçamento" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function setOrcamento(idOrcamento)
        {
            window.opener.setOrcamento(idOrcamento);
            closeWindow();
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
                            <asp:ImageButton ID="imgPesq" runat="server" OnClick="imgPesq_Click" ImageUrl="~/Images/Pesquisar.gif"
                                Style="width: 16px" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtCliente" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq3" runat="server" OnClick="imgPesq_Click" ImageUrl="~/Images/Pesquisar.gif"
                                Style="width: 16px" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label5" runat="server" Text="Telefone" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtTelefone" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" OnClick="imgPesq_Click" ImageUrl="~/Images/Pesquisar.gif"
                                Style="width: 16px" Width="16px" />
                        </td>
                        <td align="left">
                            <asp:Label ID="Label6" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpSituacao_SelectedIndexChanged">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Em Aberto</asp:ListItem>
                                <asp:ListItem Value="2">Negociado</asp:ListItem>
                                <asp:ListItem Value="3">Não Negociado</asp:ListItem>
                                <asp:ListItem Value="4">Em Negociação</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Vendedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpVendedor" runat="server" DataSourceID="odsVendedor" DataTextField="Nome"
                                DataValueField="IdFunc" AutoPostBack="True" AppendDataBoundItems="True" OnTextChanged="drpSituacao_SelectedIndexChanged">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Endereço" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtEndereco" runat="server" Width="140px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq2" runat="server" OnClick="imgPesq_Click" ImageUrl="~/Images/Pesquisar.gif"
                                Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Bairro" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtBairro" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                Width="90px"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq1" runat="server" OnClick="imgPesq_Click" ImageUrl="~/Images/Pesquisar.gif"
                                Style="width: 16px" />
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
                <asp:GridView GridLines="None" ID="grdOrcamento" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdOrcamento" DataSourceID="odsOrcamento"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="setOrcamento('<%# Eval("IdOrcamento") %>');">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                            <ItemStyle Wrap="false" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdOrcamento" HeaderText="Num." SortExpression="IdOrcamento" />
                        <asp:BoundField DataField="IdProjeto" HeaderText="Projeto" SortExpression="IdProjeto" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="NomeFuncAbrv" HeaderText="Funcionário" SortExpression="NomeFuncionario" />
                        <asp:BoundField DataField="TelCliente" HeaderText="Tel. Res." SortExpression="TelCliente" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:C}">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataCad" HeaderText="Data" SortExpression="DataCad" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="DescrSituacao" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsOrcamento" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountSel" SelectMethod="GetListSel" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.OrcamentoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCod" Name="idOrca" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCliente" Name="cliente" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpVendedor" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtTelefone" Name="telefone" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtEndereco" Name="endereco" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtBairro" Name="bairro" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsVendedor" runat="server" SelectMethod="GetVendedoresOrca"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="0" Name="idOrcamento" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
