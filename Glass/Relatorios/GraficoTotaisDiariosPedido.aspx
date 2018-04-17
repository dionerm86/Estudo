<%@ Page Title="Gráfico Totais Diários dos Pedidos" Language="C#" MasterPageFile="~/Layout.master" AutoEventWireup="true" CodeBehind="GraficoTotaisDiariosPedido.aspx.cs" 
    Inherits="Glass.UI.Web.Relatorios.GraficoTotaisDiariosPedido" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Menu" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Pagina" Runat="Server">

    <div>
        <label>Agrupar por</label>
        <asp:DropDownList runat="server" ID="groupBy" AutoPostBack="true">
            <asp:ListItem Text="Pedido Prontos" Value="DataPronto" />
            <asp:ListItem Text="Confirmado PCP" Value="DataConf" />
            <asp:ListItem Text="Liberado" Value="DataLiberacao" />
        </asp:DropDownList>
    </div>
    <asp:Chart runat="server" id="grafico" Width="800" Height="500" DataSourceID="odsPedido">
        <Titles>
            <asp:Title Text="Totais Diários" Docking="Top" Font="Arial, 14pt, style=Bold" IsDockedInsideChartArea="false">
            </asp:Title>
        </Titles>
        <Series>
            <asp:Series Name="Pedido" ChartType="Column"
                        ToolTip="#VALX (#VALY{C})"
                        LabelToolTip="#VALX{d}"
                        XValueMember="Data" XValueType="DateTime" 
                        YValueType="Double"
                        YValueMembers="Total">
            </asp:Series>
        </Series>
        <ChartAreas>
            <asp:ChartArea>
                <AxisX IsMarginVisible="true" Title="Dias"
                       TitleAlignment="Center" IsLabelAutoFit="false">
                    <LabelStyle Angle="-90" Interval="1" />
                </AxisX>
                <AxisY>
                    <LabelStyle Format="{C}" />
                </AxisY>
            </asp:ChartArea>
        </ChartAreas>
    </asp:Chart>
    <colo:VirtualObjectDataSource Culture="pt-BR" 
                    ID="odsPedido" runat="server"
                    SelectMethod="ObtemTotaisDiarios"
                    TypeName="Glass.Data.DAL.PedidoDAO">
        <SelectParameters>
            <asp:QueryStringParameter QueryStringField="idPedido" Name="idPedido" Type="UInt32" />
            <asp:QueryStringParameter QueryStringField="idLoja" Name="idLoja" Type="UInt32" />
            <asp:QueryStringParameter QueryStringField="idCli" Name="idCli" Type="UInt32" />
            <asp:QueryStringParameter QueryStringField="nomeCli" Name="nomeCli" Type="String" />
            <asp:QueryStringParameter QueryStringField="codCliente" Name="codCliente" Type="String" />
            <asp:QueryStringParameter QueryStringField="idCidade" Name="idCidade" Type="UInt32" />
            <asp:QueryStringParameter QueryStringField="endereco" Name="endereco" Type="String" />
            <asp:QueryStringParameter QueryStringField="bairro" Name="bairro" Type="String" />
            <asp:QueryStringParameter QueryStringField="complemento" Name="complemento" Type="String" />
            <asp:QueryStringParameter QueryStringField="situacao" Name="situacao" Type="String" />
            <asp:QueryStringParameter QueryStringField="situacaoProd" Name="situacaoProd" Type="String" />
            <asp:QueryStringParameter QueryStringField="ByVend" Name="byVend" Type="String" />
            <asp:QueryStringParameter QueryStringField="maoObra"  Name="maoObra" Type="String" />
            <asp:QueryStringParameter QueryStringField="maoObraEspecial" Name="maoObraEspecial" Type="String" />
            <asp:QueryStringParameter QueryStringField="producao" Name="producao" Type="String" />
            <asp:QueryStringParameter QueryStringField="numOrca" Name="idOrcamento" Type="UInt32" />
            <asp:QueryStringParameter QueryStringField="altura" Name="altura" Type="Single" />
            <asp:QueryStringParameter QueryStringField="largura" Name="largura" Type="Int32" />
            <asp:QueryStringParameter QueryStringField="diasProntoLib" Name="numeroDiasDiferencaProntoLib" Type="Int32" />
            <asp:QueryStringParameter QueryStringField="valorDe" Name="valorDe" Type="Single" />
            <asp:QueryStringParameter QueryStringField="valorAte" Name="valorAte" Type="Single" />
            <asp:QueryStringParameter QueryStringField="dataCadIni" Name="dataCadIni" Type="String" />
            <asp:QueryStringParameter QueryStringField="dataCadFim" Name="dataCadFim" Type="String" />
            <asp:QueryStringParameter QueryStringField="dataFinIni" Name="dataFinIni" Type="String" />
            <asp:QueryStringParameter QueryStringField="dataFinFim" Name="dataFinFim" Type="String" />
            <asp:QueryStringParameter QueryStringField="funcFinalizacao" Name="funcFinalizacao" Type="String" />
            <asp:QueryStringParameter QueryStringField="tipo" Name="tipo" Type="String" />
            <asp:QueryStringParameter QueryStringField="fastDelivery" Name="fastDelivery" Type="Int32" />
            <asp:QueryStringParameter QueryStringField="origemPedido" Name="origemPedido" Type="Int32" />
            <asp:QueryStringParameter QueryStringField="obs" Name="obs" Type="String" />
            <asp:ControlParameter ControlID="groupBy" Name="groupBy" Type="String" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>

