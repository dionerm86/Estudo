<%@ Page Title="Obrigações do ICMS Recolhido ou a Recolher - Operações Próprias / Substituição Tributária" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstObrigacaoRecolhidoRecolher.aspx.cs" Inherits="Glass.UI.Web.Listas.LstObrigacaoRecolhidoRecolher" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Período: " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="txtDataInicio" runat="server" ValidateEmptyText="False" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="txtDataFim" runat="server" ValidateEmptyText="False" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Tipo de Imposto: " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoImposto0" runat="server" AppendDataBoundItems="True"
                                AutoPostBack="True" DataSourceID="odsTipoImposto" DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem></asp:ListItem>
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
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir Novo Registro</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdObrigacao" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsObrigacao"
                    GridLines="None" EmptyDataText="Nenhum registro encontrado"
                    OnDataBound="grdObrigacao_DataBound" 
                    OnRowCommand="grdObrigacao_RowCommand" DataKeyNames="Id">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# Eval("Id", "~/Cadastros/CadObrigacaoRecolhidoRecolher.aspx?id={0}") %>'
                                    Text=""><img border="0" src="../Images/edit.gif" /></asp:HyperLink>
                                <asp:ImageButton ID="imgExcluir" runat="server" CausesValidation="False" CommandName="Delete"
                                    ImageUrl="~/Images/ExcluirGrid.gif" OnClientClick="if (!confirm(&quot;Deseja excluir esse ajuste?&quot;)) return false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescricaoTipoImposto" HeaderText="Imposto" 
                            SortExpression="DescricaoTipoImposto" />
                        <asp:BoundField DataField="DescricaoCodigoObrigacao" HeaderText="Cód. Obrigação"
                            SortExpression="DescricaoCodigoObrigacao" />
                        <asp:BoundField DataField="Valor" DataFormatString="{0:c}" HeaderText="Valor" SortExpression="Valor" />
                        <asp:BoundField DataField="DataVencimento" DataFormatString="{0:d}" HeaderText="Data Venc."
                            SortExpression="DataVencimento" />
                        <asp:BoundField DataField="Uf" HeaderText="UF" SortExpression="Uf" />
                        <asp:BoundField DataField="DescricaoCodigoReceita" HeaderText="Cód. Receita" SortExpression="DescricaoCodigoReceita" />
                        <asp:BoundField DataField="NumeroProcesso" HeaderText="Nº Processo" SortExpression="NumeroProcesso" />
                        <asp:BoundField DataField="DescricaoIndProc" HeaderText="Ind. Origem" ReadOnly="True"
                            SortExpression="DescricaoIndProc" />
                        <asp:BoundField DataField="DescricaoProcesso" HeaderText="Desc. Processo" SortExpression="DescricaoProcesso" />
                        <asp:BoundField DataField="DescricaoComplementar" HeaderText="Desc. Compl." SortExpression="DescricaoComplementar" />
                        <asp:BoundField DataField="MesReferencia" HeaderText="Mês Ref." SortExpression="MesReferencia" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsObrigacao" runat="server" DataObjectTypeName="Glass.Data.Model.ObrigacaoRecolhidoRecolher"
                    DeleteMethod="Delete" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount"
                    SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.ObrigacaoRecolhidoRecolherDAO" EnablePaging="True"
                    UpdateMethod="Update" >
                    <SelectParameters>
                     <asp:ControlParameter ControlID="drpTipoImposto0" Name="tipoImposto" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtDataInicio" Name="dataInicio" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsIndicadorOrigem" runat="server" SelectMethod="GetIndOrigemCred"
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodigo" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.AjusteBeneficioIncentivoDAO"
                    >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodObrigacao" runat="server" SelectMethod="GetTabelaCodigoObrigacaoICMS"
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodigoReceita" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.TabelaCodigoReceitaDAO" >
                </colo:VirtualObjectDataSource>
                 <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoImposto" runat="server" SelectMethod="GetTipoImpostoSPED"
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
