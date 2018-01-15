<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelTipoParticipante.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SelTipoParticipante" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Selecione o tipo de participante</title>
    
    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/StyleProd.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>
    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/GridView.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>
    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/dhtmlgoodies_calendar.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/dhtmlgoodies_calendar.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/ecmascript">    

        function setTipoParticipante(nome, controle) {

            window.opener.setTipoParticipante(nome, controle);
            
            window.close();
        }
    
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <table style="width: 100%">
        <div>
            Selecione o tipo de participante
        </div>
        
        <div>
            <asp:RadioButtonList ID="rblTipoParticipante" runat="server" OnSelectedIndexChanged="rblTipoParticipante_SelectedIndexChanged"
                AutoPostBack="True" RepeatDirection="Horizontal" >
                <asp:ListItem Value="0" Text="Loja"></asp:ListItem>
                <asp:ListItem Value="1" Text="Fornecedor"></asp:ListItem>
                <asp:ListItem Value="2" Text="Cliente"></asp:ListItem>
                <asp:ListItem Value="3" Text="Transportador"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div>
            
            <asp:GridView GridLines="None" ID="grdLoja" runat="server" AllowPaging="True" AllowSorting="True"
                AutoGenerateColumns="False" DataSourceID="odsLoja" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataKeyNames="IdLoja"
                EmptyDataText="Nenhuma loja encontrada." Visible="false">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <a href="#" onclick="setTipoParticipante('hdfPartLoja', '<%# Eval("NomeFantasia") %>' );">
                                <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="NomeFantasia" HeaderText="Nome" SortExpression="NomeFantasia" />
                </Columns>
                <PagerStyle />
                <EditRowStyle />
                <AlternatingRowStyle />
            </asp:GridView>
            <asp:GridView GridLines="None" ID="grdFornecedor" runat="server" AllowPaging="True"
                AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsFornecedor"
                CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                EditRowStyle-CssClass="edit" DataKeyNames="IdFornec" EmptyDataText="Nenhum fornecedor encontrado."
                Visible="false">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <a href="#" onclick="setTipoParticipante('hdfPartFornec', '<%# Eval("NomeFantasia") %>');">
                                <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="NomeFantasia" HeaderText="Nome" SortExpression="NomeFantasia" />
                </Columns>
                <PagerStyle />
                <EditRowStyle />
                <AlternatingRowStyle />
            </asp:GridView>
            <asp:GridView GridLines="None" ID="grdCliente" runat="server" AllowPaging="True"
                AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsCliente" CssClass="gridStyle"
                PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                DataKeyNames="IdCli" EmptyDataText="Nenhum cliente encontrado." Visible="false">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <a href="#" onclick="setTipoParticipante('hdfPartCliente', '<%# Eval("Nome") %>');">
                                <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                </Columns>
                <PagerStyle />
                <EditRowStyle />
                <AlternatingRowStyle />
            </asp:GridView>
            <asp:GridView GridLines="None" ID="grdtransportador" runat="server" AllowPaging="True"
                AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsTransportador"
                CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                EditRowStyle-CssClass="edit" DataKeyNames="IdTransportador" EmptyDataText="Nenhum funcionário encontrado."
                Visible="false">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <a href="#" onclick="setTipoParticipante('hdfPartTransportador','<%# Eval("Nome") %>');">
                                <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                </Columns>
                <PagerStyle />
                <EditRowStyle />
                <AlternatingRowStyle />
            </asp:GridView>
            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" DataObjectTypeName="Glass.Data.Model.Loja"
                SelectMethod="GetList" TypeName="Glass.Data.DAL.LojaDAO" EnablePaging="True"
                MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" SortParameterName="sortExpression"
                StartRowIndexParameterName="startRow">
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFornecedor" runat="server" SelectMethod="GetOrdered"
                TypeName="Glass.Data.DAL.FornecedorDAO" />
            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCliente" runat="server" DataObjectTypeName="Glass.Data.Model.Cliente"
                SelectMethod="GetList" TypeName="Glass.Data.DAL.ClienteDAO" EnablePaging="True"
                MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" SortParameterName="sortExpression"
                StartRowIndexParameterName="startRow">
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTransportador" runat="server" SelectMethod="GetOrdered"
                TypeName="Glass.Data.DAL.TransportadorDAO" />
            
            <br />
            <br />    
                <asp:Button ID="btnFechar" runat="server" Text="Fechar" OnClientClick="window.close();" />
            
        </div>
    </table>
    </form>
</body>
</html>
