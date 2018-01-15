<%@ Page Title="Disponibilizar Retalhos para Produção" Language="C#" MasterPageFile="~/Layout.master" AutoEventWireup="true" CodeBehind="DisponibilizarRetalhos.aspx.cs" Inherits="Glass.UI.Web.Utils.DisponibilizarRetalhos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Menu" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Pagina" Runat="Server">
    <script type="text/javascript">
        window.onunload = function()
        {
            window.opener.redirectUrl(window.opener.location.href);
        }
    </script>

    <table>
        <tr>
            <td>
                <asp:Label ID="Label5" runat="server" ForeColor="#0066FF" Text="Etiqueta"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtEtiqueta" runat="server" Width="90px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
            </td>
            <td>
                <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClick="imgPesq_Click" />
            </td>
        </tr>
    </table>

    <div style="display: inline-block">
        <asp:GridView ID="grdRetalhosDisponiveis" runat="server" AllowPaging="True" 
            AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" 
            DataKeyNames="CodigoRetalho" DataSourceID="odsRetalhosDisponiveis" 
            GridLines="None" EmptyDataText="Não há retalhos disponíveis.">
            <Columns>
                <asp:BoundField DataField="CodigoProduto" HeaderText="Cód." ReadOnly="True" 
                    SortExpression="CodigoProduto" />
                <asp:BoundField DataField="DescricaoProduto" HeaderText="Descrição" 
                    ReadOnly="True" SortExpression="DescricaoProduto" />
                <asp:BoundField DataField="Altura" HeaderText="Altura" ReadOnly="True" 
                    SortExpression="Altura" />
                <asp:BoundField DataField="Largura" HeaderText="Largura" ReadOnly="True" 
                    SortExpression="Largura" />
                <asp:BoundField DataField="TotalM2" HeaderText="Total m²" ReadOnly="True" 
                    SortExpression="TotalM2" />
                <asp:BoundField DataField="NumeroEtiqueta" HeaderText="Etiqueta" 
                    ReadOnly="True" SortExpression="NumeroEtiqueta" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkColocarEstoque" runat="server" CommandName="Update" 
                            onclientclick="if (!confirm(&quot;Voltar esse retalho para estoque?&quot;)) return false;">Voltar para Estoque</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerStyle CssClass="pgr" />
            <AlternatingRowStyle CssClass="alt" />
        </asp:GridView>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsRetalhosDisponiveis" runat="server" 
            SelectMethod="ObtemRetalhosDisponiveis" 
            TypeName="WebGlass.Business.RetalhoProducao.Fluxo.DisponibilizarRetalho" 
            UpdateMethod="MarcaRetalhoEmEstoque"  
            onupdated="odsRetalhosDisponiveis_Updated">
            <UpdateParameters>
                <asp:Parameter Name="codigoRetalho" Type="UInt32" />
            </UpdateParameters>
             <SelectParameters>
                <asp:ControlParameter ControlID="txtEtiqueta" Name="numEtiqueta" PropertyName="Text" Type="String" />
            </SelectParameters>
        </colo:VirtualObjectDataSource>
    </div>
    &nbsp;
    <div style="display: inline-block">
        <asp:GridView ID="grdRetalhosEstoque" runat="server" AllowPaging="True" 
            AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" 
            DataKeyNames="CodigoRetalho" DataSourceID="odsRetalhosEstoque" 
            GridLines="None" EmptyDataText="Não há retalhos em estoque.">
            <Columns>
                <asp:BoundField DataField="CodigoProduto" HeaderText="Cód." ReadOnly="True" 
                    SortExpression="CodigoProduto" />
                <asp:BoundField DataField="DescricaoProduto" HeaderText="Descrição" 
                    ReadOnly="True" SortExpression="DescricaoProduto" />
                <asp:BoundField DataField="Altura" HeaderText="Altura" ReadOnly="True" 
                    SortExpression="Altura" />
                <asp:BoundField DataField="Largura" HeaderText="Largura" ReadOnly="True" 
                    SortExpression="Largura" />
                <asp:BoundField DataField="TotalM2" HeaderText="Total m²" ReadOnly="True" 
                    SortExpression="TotalM2" />
                <asp:BoundField DataField="NumeroEtiqueta" HeaderText="Etiqueta" 
                    ReadOnly="True" SortExpression="NumeroEtiqueta" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkDisponibilizar" runat="server" CommandName="Update" 
                            onclientclick="if (!confirm(&quot;Disponibilizar retalho?&quot;)) return false;">Disponibilizar</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerStyle CssClass="pgr" />
            <AlternatingRowStyle CssClass="alt" />
        </asp:GridView>
      
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsRetalhosEstoque" runat="server" 
            SelectMethod="ObtemRetalhosEmEstoque" 
            TypeName="WebGlass.Business.RetalhoProducao.Fluxo.DisponibilizarRetalho" 
            UpdateMethod="MarcaRetalhoDisponivel"  
            onupdated="odsRetalhosEstoque_Updated">
            <UpdateParameters>
                <asp:Parameter Name="codigoRetalho" Type="UInt32" />
            </UpdateParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="txtEtiqueta" Name="numEtiqueta" PropertyName="Text" Type="String" />
            </SelectParameters>
        </colo:VirtualObjectDataSource>
    </div>
</asp:Content>
