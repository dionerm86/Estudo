<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelPlanoConta.aspx.cs" Inherits="Glass.UI.Web.Utils.SelPlanoConta"
    Title="Selecione o Plano de Conta" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function setPlanoConta(idConta, grupo, descricao)
        {
            window.opener.setPlanoConta(idConta, grupo + " - " + descricao);
            closeWindow();
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Grupo Conta" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupoConta" runat="server" AutoPostBack="True" DataSourceID="odsGrupoConta"
                                DataTextField="Name" DataValueField="Id" AppendDataBoundItems="True"
                                OnSelectedIndexChanged="drp_SelectedIndexChanged">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" Style="width: 16px" />
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
                <asp:GridView GridLines="None" ID="grdCfop" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    DataKeyNames="IdConta" PageSize="15">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="setPlanoConta(<%# Eval("IdConta") %>, '<%# Eval("DescrGrupo") %>', '<%# Eval("Descricao") %>');">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescrGrupo" HeaderText="Grupo" SortExpression="DescrGrupo" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" runat="server" ID="odsPlanoConta" EnablePaging="True" 
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" MaximumRowsParameterName="pageSize"
                    TypeName="Glass.Data.DAL.PlanoContasDAO"
                    SelectCountMethod="GetCountSel" SelectMethod="GetListSel">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpGrupoConta" DefaultValue="0" Name="idGrupo" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="txtDescricao" DefaultValue="" Name="descricao" PropertyName="Text" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" runat="server" ID="odsPlanoContaPorTipo" EnablePaging="True" 
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" MaximumRowsParameterName="pageSize"
                    TypeName="Glass.Data.DAL.PlanoContasDAO"
                    SelectCountMethod="GetCountPlanoContasPeloTipo" SelectMethod="GetPlanoContasPeloTipo">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="2" Name="tipo" Type="Int32" />
                        <asp:ControlParameter ControlID="drpGrupoConta" DefaultValue="0" Name="idGrupo" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="txtDescricao" DefaultValue="" Name="descricao" PropertyName="Text" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoConta" runat="server" 
                    SelectMethod="ObtemGruposContaCadastro"
                    TypeName="Glass.Financeiro.Negocios.IPlanoContasFluxo">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
