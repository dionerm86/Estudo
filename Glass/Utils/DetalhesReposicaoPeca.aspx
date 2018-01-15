<%@ Page Title="Detalhes de Reposição de Peça" Language="C#" MasterPageFile="~/Layout.master" AutoEventWireup="true" 
    CodeBehind="DetalhesReposicaoPeca.aspx.cs" Inherits="Glass.UI.Web.Utils.DetalhesReposicaoPeca" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Menu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Pagina" runat="server">
    <div class="subtitle1">
        Etiqueta:
        <asp:Label runat="server" ID="lblEtiqueta"></asp:Label>
    </div>
    <div class="inserir">
    </div>
    <asp:GridView ID="grdDetalhes" runat="server" SkinID="defaultGridView"
        DataSourceID="odsDetalhes" DataKeyNames="IdProdPedProducao">
        <Columns>
            <asp:BoundField DataField="FuncRepos" HeaderText="Funcionário" />
            <asp:BoundField DataField="SetorRepos" HeaderText="Setor" />
            <asp:BoundField DataField="DataRepos" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
            <asp:BoundField DataField="DadosLeituraProducao" HeaderText="Dados Leituras" />
        </Columns>
    </asp:GridView>
    <script type="text/javascript">
        $(document).ready(function () {
            var grid = document.getElementById("<%= grdDetalhes.ClientID %>");
            var dados = 3;
            
            for (var l = 1; l < grid.rows.length; l++) {
                var texto = grid.rows[l].cells[dados].innerText;
                grid.rows[l].cells[dados].innerHTML = texto;
            }
        });
    </script>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsDetalhes" runat="server" 
        DataObjectTypeName="Glass.PCP.Negocios.Entidades.DetalhesReposicaoPeca"
        SelectMethod="PesquisarDetalhesReposicaoPeca" 
        TypeName="Glass.PCP.Negocios.IDetalhesReposicaoPeca">
        <SelectParameters>
            <asp:QueryStringParameter Name="idProdPedProducao" QueryStringField="idProdPedProducao" Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>
