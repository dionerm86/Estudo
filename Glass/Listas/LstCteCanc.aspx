<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstCteCanc.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstCteCanc" Title="Cancelamento de CTe" %>

<%@ Register Src="~/Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <link href="<%= ResolveUrl("~") %>Style/CTe/LstCTe.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript">

        function openMotivoInut(idCte) {
            var altura = 150;
            var largura = 400;
            var scrY = (screen.height - altura) / 2;
            var scrX = (screen.width - largura) / 2;
            var momentoAtual = new Date();

            var win = window.open("../Utils/SetMotivoCancCTe.aspx?idCte=" + idCte, "popup" + momentoAtual.getSeconds(), 'width=' + largura + ',height=' + altura + ',left=' + scrX + ',top=' + scrY);

            return false;
        }

    </script>

    <div class="filtro">
        <div class="linha-um-cte">
            <ul class="item">
                <li class="titulo-filtro">
                    <asp:Label ID="lblNumCte" runat="server" Text="Num. CTe" ForeColor="#0066FF"></asp:Label>
                </li>
                <li>
                    <asp:TextBox ID="txtNumCte" runat="server" MaxLength="10" Width="60px"></asp:TextBox>
                </li>
                <li>
                    <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                        OnClick="imgPesq_Click" OnClientClick="return openRota();" />
                </li>
            </ul>
            <ul class="item">
                <li class="titulo-filtro">
                    <asp:Label ID="lblSituacao" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                </li>
                <li>
                    <sync:CheckBoxListDropDown ID="cboSituacao" runat="server" CheckAll="False" Title="Todas"
                        Width="188px">
                        <asp:ListItem Value="2">Autorizada</asp:ListItem>
                        <asp:ListItem Value="8">Processo de cancelamento</asp:ListItem>
                        <asp:ListItem Value="11">Falha ao cancelar</asp:ListItem>
                    </sync:CheckBoxListDropDown>
                </li>
                <li>
                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                        ToolTip="Pesquisar" OnClick="imgPesq_Click" OnClientClick="return openRota();" />
                </li>
            </ul>
            <ul class="item">
                <li class="titulo-filtro">
                    <asp:Label ID="lblCfop" runat="server" Text="CFOP" ForeColor="#0066FF"></asp:Label>
                </li>
                <li>
                    <asp:DropDownList ID="drpCfop" runat="server" DataSourceID="odsCfop" DataTextField="CodInterno"
                        DataValueField="IdCfop" AppendDataBoundItems="True" AutoPostBack="True">
                        <asp:ListItem Value="0">Todos</asp:ListItem>
                    </asp:DropDownList>
                </li>
            </ul>
            <ul class="item">
                <li class="titulo-filtro">
                    <asp:Label ID="lblFormaPgto" runat="server" Text="Forma Pagto." ForeColor="#0066FF"></asp:Label>
                </li>
                <li>
                    <asp:DropDownList ID="drpFormaPagto" runat="server" Height="20px" Width="100px" AutoPostBack="true">
                        <asp:ListItem Value="3" Text="Todas"></asp:ListItem>
                        <asp:ListItem Value="0" Text="Pago"></asp:ListItem>
                        <asp:ListItem Value="1" Text="À pagar"></asp:ListItem>
                        <asp:ListItem Value="2" Text="Outros"></asp:ListItem>
                    </asp:DropDownList>
                </li>
            </ul>
        </div>
        <div class="linha-dois-cte">
            <ul class="item">
                <li class="titulo-filtro">
                    <asp:Label ID="lblTipoEmissao" runat="server" Text="Tipo Emissão" ForeColor="#0066FF"></asp:Label>
                </li>
                <li>
                    <asp:DropDownList ID="drpTipoEmissao" runat="server" Height="20px" Width="180px"
                        AutoPostBack="true">
                        <asp:ListItem Value="0" Text="Todos"></asp:ListItem>
                        <asp:ListItem Value="1" Text="Normal"></asp:ListItem>
                        <asp:ListItem Value="5" Text="Contingência FSDA"></asp:ListItem>
                        <asp:ListItem Value="7" Text="Autorização pela SVC-RS"></asp:ListItem>
                        <asp:ListItem Value="8" Text="Autorização pela SVC-SP"></asp:ListItem>
                    </asp:DropDownList>
                </li>
            </ul>
            <ul class="item">
                <li class="titulo-filtro">
                    <asp:Label ID="lblTipoCte" runat="server" Text="Tipo CTe" ForeColor="#0066FF"></asp:Label>
                </li>
                <li>
                    <asp:DropDownList ID="drpTipoCte" runat="server" Height="20px" Width="220px" AutoPostBack="true">
                        <asp:ListItem Value="4" Text="Todos"></asp:ListItem>
                        <asp:ListItem Value="0" Text="CT-e Normal"></asp:ListItem>
                        <asp:ListItem Value="1" Text="CT-e de Complemento de Valores"></asp:ListItem>
                        <asp:ListItem Value="2" Text="CT-e de Anulação de Valores"></asp:ListItem>
                        <asp:ListItem Value="3" Text="CT-e Substituto"></asp:ListItem>
                    </asp:DropDownList>
                </li>
            </ul>
            <ul class="item">
                <li class="titulo-filtro">
                    <asp:Label ID="lblTipoServico" runat="server" Text="Tipo Serviço" ForeColor="#0066FF"></asp:Label>
                </li>
                <li>
                    <asp:DropDownList ID="drpTipoServico" runat="server" Height="20px" Width="180px"
                        AutoPostBack="true">
                        <asp:ListItem Value="4" Text="Todos"></asp:ListItem>
                        <asp:ListItem Value="0" Text="Normal"></asp:ListItem>
                        <asp:ListItem Value="1" Text="Subcontratação"></asp:ListItem>
                        <asp:ListItem Value="2" Text="Redespacho"></asp:ListItem>
                        <asp:ListItem Value="3" Text="Redespacho Intermediário"></asp:ListItem>
                    </asp:DropDownList>
                </li>
            </ul>
        </div>
        <div class="linha-tres-cte">
            <ul class="item">
                <li class="titulo-filtro">
                    <asp:Label ID="lblPerEmissao" runat="server" Text="Período Emissão" ForeColor="#0066FF"></asp:Label>
                </li>
                <li>
                    <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                </li>
                <li>
                    <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                </li>
            </ul>
            <ul class="item">
                <li class="titulo-filtro">
                    <asp:Label ID="lblOrdenar" runat="server" Text="Ordenar" ForeColor="#0066FF"></asp:Label>
                </li>
                <li>
                    <asp:DropDownList ID="drpOrdenar" runat="server" AutoPostBack="True">
                        <asp:ListItem Value="0" Selected="True">Data de emissão (descresc.)</asp:ListItem>
                        <asp:ListItem Value="1">Data de emissão (cresc.)</asp:ListItem>
                        <asp:ListItem Value="2">Valor Total(cresc.)</asp:ListItem>
                        <asp:ListItem Value="3">Valor Total (descresc.)</asp:ListItem>
                    </asp:DropDownList>
                </li>
            </ul>
        </div>
    </div>
    <div class="grid">
        <asp:GridView GridLines="None" ID="grdCte" runat="server" AllowPaging="True" AllowSorting="True"
            AutoGenerateColumns="False" DataSourceID="odsCte" DataKeyNames="IdCte" EmptyDataText="Nenhum CTe encontrado."
            CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
            EditRowStyle-CssClass="edit">
            <PagerSettings PageButtonCount="20" />
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <a href="#" onclick="openMotivoInut('<%# Eval("IdCte") %>');return false;">Cancelar</a>
                    </ItemTemplate>
                    <HeaderStyle Wrap="False" />
                    <ItemStyle Wrap="False" />
                </asp:TemplateField>
                <asp:BoundField DataField="NumeroCte" HeaderText="Num." SortExpression="NumeroCte" />
                <asp:BoundField DataField="Modelo" HeaderText="Modelo" SortExpression="Modelo" />
                <asp:BoundField DataField="IdCfop" HeaderText="CFOP" SortExpression="IdCfop" />
                <asp:BoundField DataField="TipoCteString" HeaderText="Tipo Cte" SortExpression="TipoCte" />
                <asp:BoundField DataField="TipoEmissaoString" HeaderText="Tipo Emissão" SortExpression="TipoEmissao" />
                <asp:BoundField DataField="TipoServicoString" HeaderText="Tipo Serviço" SortExpression="TipoServico" />
                <asp:BoundField DataField="ValorTotal" HeaderText="Valor Tot." SortExpression="ValorTotal" />
                <asp:BoundField DataField="ValorReceber" HeaderText="Valor Rec." SortExpression="ValorReceber" />
                <asp:BoundField DataField="DataEmissao" DataFormatString="{0:D}" HeaderText="Data Emissão"
                    SortExpression="DataEmissao" />
                <asp:BoundField DataField="SituacaoString" HeaderText="Situação" SortExpression="Situacao" />
            </Columns>
            <PagerStyle />
            <EditRowStyle />
            <AlternatingRowStyle />
        </asp:GridView>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCte" runat="server" DataObjectTypeName="WebGlass.Business.ConhecimentoTransporte.Entidade.Cte"
            EnablePaging="True" MaximumRowsParameterName="pageSize" SelectMethod="GetList"
            SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="WebGlass.Business.ConhecimentoTransporte.Fluxo.BuscarCte"
            >
            <SelectParameters>
                <asp:ControlParameter ControlID="txtNumCte" Name="numeroCte" PropertyName="Text"
                    Type="Int32" />
                <asp:Parameter Name="idLoja" Type="Int32" />
                <asp:ControlParameter ControlID="cboSituacao" Name="situacao" PropertyName="SelectedValue"
                    Type="String" DefaultValue="2,8,11" />
                <asp:ControlParameter ControlID="drpCfop" Name="idCfop" PropertyName="SelectedValue"
                    Type="UInt32" />
                <asp:ControlParameter ControlID="drpFormaPagto" Name="formaPagto" PropertyName="SelectedValue"
                    Type="Int32" />
                <asp:ControlParameter ControlID="drpTipoEmissao" Name="tipoEmissao" PropertyName="SelectedValue"
                    Type="Int32" />
                <asp:ControlParameter ControlID="drpTipoCte" Name="tipoCte" PropertyName="SelectedValue"
                    Type="Int32" />
                <asp:ControlParameter ControlID="drpTipoServico" Name="tipoServico" PropertyName="SelectedValue"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctrlDataIni" Name="dataEmiIni" PropertyName="DataString"
                    Type="String" />
                <asp:ControlParameter ControlID="ctrlDataFim" Name="dataEmiFim" PropertyName="DataString"
                    Type="String" />
                <asp:Parameter Name="idTransportador" Type="UInt32" />
                <asp:ControlParameter ControlID="drpOrdenar" Name="ordenar" PropertyName="SelectedValue"
                    Type="Int32" />
                <asp:Parameter Name="tipoDestinatario" Type="UInt32" />
                <asp:Parameter Name="idDestinatario" Type="UInt32" />
                <asp:Parameter Name="tipoRecebedor" Type="UInt32" />
                <asp:Parameter Name="idRecebedor" Type="UInt32" />
            </SelectParameters>
        </colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCfop" runat="server" SelectMethod="GetSortedByCodInterno"
            TypeName="Glass.Data.DAL.CfopDAO">
        </colo:VirtualObjectDataSource>
    </div>
</asp:Content>
