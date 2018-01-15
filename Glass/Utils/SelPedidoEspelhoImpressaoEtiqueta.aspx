<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelPedidoEspelhoImpressaoEtiqueta.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SelPedidoEspelhoImpressaoEtiqueta" Title="Selecione o Pedido"
    MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlSelCliente.ascx" TagName="ctrlSelCliente" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function adicionar(idPedido, mostrarErro)
        {
            var campoNumero = FindControl("txtNumero", "input", window.opener.document);

            var idCorVidro = FindControl("drpCorVidro", "select").value;
            var espessura = FindControl("txtEspessura", "input").value;
            var idSubgrupoProd = FindControl("drpSubgrupoProd", "select").value;
            var alturaMin = FindControl("txtAlturaMin", "input").value;
            var alturaMax = FindControl("txtAlturaMax", "input").value;
            var larguraMin = FindControl("txtLarguraMin", "input").value;
            var larguraMax = FindControl("txtLarguraMax", "input").value;
            
            if (campoNumero != null)
            {
                campoNumero.value = idPedido;
                window.opener.getProduto(null, mostrarErro, idCorVidro, espessura, idSubgrupoProd, alturaMin, alturaMax, larguraMin, larguraMax);

                campoNumero.value = "";
            }
        }

    </script>

    <div class="filtro">
        <div>
            <span>
                <asp:Label ID="Label4" runat="server" Text="Cliente" AssociatedControlID="ctrlSelCliente"></asp:Label>
                <uc1:ctrlSelCliente ID="ctrlSelCliente" runat="server" FazerPostBackBotaoPesquisar="True" />
            </span>
            <span>
                <asp:Label ID="Label3" runat="server" Text="Rota" AssociatedControlID="drpRota"></asp:Label>
                <asp:DropDownList ID="drpRota" runat="server" AppendDataBoundItems="True" 
                DataSourceID="odsRota" DataTextField="Descricao" DataValueField="IdRota">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    CssClass="botaoPesquisar" OnClick="imgPesq_Click" />
            </span>
            <span>
                <asp:Label ID="Label5" runat="server" Text="Ped. Cli." AssociatedControlID="txtPedCli"></asp:Label>
                <asp:TextBox ID="txtPedCli" runat="server" Width="100px"></asp:TextBox>
                <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    CssClass="botaoPesquisar" OnClick="imgPesq_Click" />
            </span>
        </div>
        <div>
            <span>
                <asp:Label ID="Label1" runat="server" Text="Data de Entrega" AssociatedControlID="ctrlDataEntregaIni"></asp:Label>
                <uc2:ctrlData ID="ctrlDataEntregaIni" runat="server" />
                a
                <uc2:ctrlData ID="ctrlDataEntregaFim" runat="server" />
                <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" CssClass="botaoPesquisar"
                    OnClick="imgPesq_Click" />
            </span>
            <span>
                <asp:Label ID="Label2" runat="server" Text="Data de Fábrica" AssociatedControlID="ctrlDataFabricaIni"></asp:Label>
                <uc2:ctrlData ID="ctrlDataFabricaIni" runat="server" />
                a
                <uc2:ctrlData ID="ctrlDataFabricaFim" runat="server" />
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    CssClass="botaoPesquisar" OnClick="imgPesq_Click" />
            </span>
        </div>
        <div>
            <span>
                <asp:Label ID="Label6" runat="server" Text="Tipo de Pedido" AssociatedControlID="drpTipoPedido"></asp:Label>
                <asp:DropDownList ID="drpTipoPedido" runat="server" 
                    DataSourceID="odsTipoPedido" DataTextField="Descr" DataValueField="Id">
                </asp:DropDownList>
                <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    CssClass="botaoPesquisar" OnClick="imgPesq_Click" />
            </span>
            <span>
                <asp:Label ID="Label7" runat="server" Text="Loja" AssociatedControlID="drpLoja"></asp:Label>
                <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" 
                    DataTextField="NomeFantasia" DataValueField="IdLoja" AppendDataBoundItems="true">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    CssClass="botaoPesquisar" OnClick="imgPesq_Click" />
            </span>
        </div>
        <div>
            <span>
                <asp:Label ID="Label8" runat="server" Text="Cor" AssociatedControlID="drpCorVidro"></asp:Label>
                <asp:DropDownList ID="drpCorVidro" runat="server" AppendDataBoundItems="True" 
                    DataSourceID="odsCorVidro" DataTextField="Descricao" 
                    DataValueField="IdCorVidro">
                    <asp:ListItem Value="0">Todas</asp:ListItem>
                </asp:DropDownList>
                <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    CssClass="botaoPesquisar" OnClick="imgPesq_Click" />
            </span>
            <span>
                <asp:Label ID="Label9" runat="server" Text="Espessura" AssociatedControlID="txtEspessura"></asp:Label>
                <asp:TextBox ID="txtEspessura" runat="server" Width="50px"
                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    CssClass="botaoPesquisar" OnClick="imgPesq_Click" />
            </span>
            <span>
                <asp:Label ID="Label10" runat="server" Text="Subgrupo" AssociatedControlID="drpSubgrupoProd"></asp:Label>
                <asp:DropDownList ID="drpSubgrupoProd" runat="server" DataSourceID="odsSubgrupo" 
                    DataTextField="Descricao" DataValueField="IdSubgrupoProd">
                </asp:DropDownList>
                <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    CssClass="botaoPesquisar" OnClick="imgPesq_Click" />
            </span>
        </div>
        <div>
            <span>
                <asp:Label ID="Label11" runat="server" Text="Altura" AssociatedControlID="txtAlturaMin"></asp:Label>
                mín.:
                <asp:TextBox ID="txtAlturaMin" runat="server" Width="50px"
                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                máx.:
                <asp:TextBox ID="txtAlturaMax" runat="server" Width="50px"
                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
            </span>
            <span>
                <asp:Label ID="Label12" runat="server" Text="Largura" AssociatedControlID="txtLarguraMin"></asp:Label>
                mín.:
                <asp:TextBox ID="txtLarguraMin" runat="server" Width="50px"
                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                máx.:
                <asp:TextBox ID="txtLarguraMax" runat="server" Width="50px"
                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
            </span>
        </div>
    </div>
    <asp:GridView GridLines="None" ID="grdPedido" runat="server" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" DataKeyNames="IdPedido" DataSourceID="odsPedidoEspelho"
        CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt">
        <PagerSettings PageButtonCount="20" />
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <a href="#" onclick="adicionar('<%# Eval("IdPedido") %>', true);">
                        <img src="../Images/Insert.gif" border="0" title="Adicionar" 
                        alt="Selecionar" /></a>
                </ItemTemplate>
                <ItemStyle Wrap="false" />
            </asp:TemplateField>
            <asp:BoundField DataField="IdPedido" HeaderText="Num" SortExpression="IdPedido" />
            <asp:BoundField DataField="IdProjeto" HeaderText="Projeto" SortExpression="IdProjeto" />
            <asp:BoundField DataField="NomeCli" HeaderText="Cliente" SortExpression="NomeCli" />
            <asp:BoundField DataField="NomeFunc" HeaderText="Vendedor" SortExpression="NomeFunc" />
            <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:C}" />
            <asp:BoundField DataField="DataEspelho" HeaderText="Data" SortExpression="DataEspelho"
                DataFormatString="{0:d}"></asp:BoundField>
            <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="DescrSituacao" />
        </Columns>
        <PagerStyle />
        <EditRowStyle />
        <AlternatingRowStyle />
    </asp:GridView>
    <asp:LinkButton ID="lnkAddAll" runat="server" Font-Size="Small" OnClick="lnkAddAll_Click"><img src="../Images/addMany.gif" border="0"> Adicionar Todas</asp:LinkButton>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedidoEspelho" runat="server"
        EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="ObtemNumeroPedidosEspelhoImpressaoEtiqueta"
        SelectMethod="ObtemPedidosEspelhoImpressaoEtiqueta" 
        SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
        TypeName="Glass.Data.DAL.PedidoEspelhoDAO" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" SkinID="">
        <SelectParameters>
            <asp:ControlParameter Name="idCliente" ControlID="ctrlSelCliente" 
                PropertyName="IdCliente" Type="UInt32" />
            <asp:ControlParameter Name="idRota" ControlID="drpRota" PropertyName="SelectedValue"
                Type="UInt32" />
            <asp:ControlParameter Name="pedCliente" ControlID="txtPedCli" 
                PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="ctrlDataEntregaIni" Name="dataEntregaIni" 
                PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataEntregaFim" Name="dataEntregaFim" 
                PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataFabricaIni" Name="dataFabricaIni" 
                PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataFabricaFim" Name="dataFabricaFim" 
                PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="drpTipoPedido" Name="tipoPedido" 
                PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="drpLoja" Name="idLoja" 
                PropertyName="SelectedValue" Type="UInt32" />
            <asp:ControlParameter ControlID="drpCorVidro" Name="idCorVidro" 
                PropertyName="SelectedValue" Type="UInt32" />
            <asp:ControlParameter ControlID="txtEspessura" Name="espessura" 
                PropertyName="Text" Type="Single" />
            <asp:ControlParameter ControlID="drpSubgrupoProd" Name="idSubgrupoProd" 
                PropertyName="SelectedValue" Type="UInt32" />
            <asp:ControlParameter ControlID="txtAlturaMin" Name="alturaMin" 
                PropertyName="Text" Type="Single" />
            <asp:ControlParameter ControlID="txtAlturaMax" Name="alturaMax" 
                PropertyName="Text" Type="Single" />
            <asp:ControlParameter ControlID="txtLarguraMin" Name="larguraMin" 
                PropertyName="Text" Type="Int32" />
            <asp:ControlParameter ControlID="txtLarguraMax" Name="larguraMax" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoPedido" runat="server" 
        SelectMethod="GetTipoPedido" TypeName="Glass.Data.Helper.DataSources">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server"
        SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server"
        SelectMethod="GetAll" TypeName="Glass.Data.DAL.RotaDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCorVidro" runat="server" 
        SelectMethod="GetAll" TypeName="Glass.Data.DAL.CorVidroDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSubgrupo" runat="server" 
        SelectMethod="GetForFilter" TypeName="Glass.Data.DAL.SubgrupoProdDAO">
        <SelectParameters>
            <asp:Parameter DefaultValue="1" Name="idGrupo" Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>
