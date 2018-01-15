<%@ Page  Language="C#" AutoEventWireup="true" CodeBehind="SelMedicao.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SelMedicao" Title="Selecionar Medições" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

    function getCli(idCli) {
        if (idCli.value == "") {
            //openWindow(500, 800, 'SelCliente.aspx');
            return false;
        }

        var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idCli.value = "";
            FindControl("txtNomeCliente", "input").value = "";
            return false;
        }

        FindControl("txtNomeCliente", "input").value = retorno[1];
    }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Período"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtDataIni" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido0" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataIni', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataFim" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido1" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataFim', this)" ToolTip="Alterar" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label7" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" AppendDataBoundItems="True" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" ForeColor="#0066FF" Text="Cliente"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" onblur="getCli(this);" onkeypress="return soNumeros(event, true, true);"
                                Width="60px"></asp:TextBox>
                        </td>
                        <td>
                            &nbsp;<asp:TextBox ID="txtNomeCliente" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnFechar" runat="server" Text="Fechar" OnClientClick="closeWindow();" />
                <br />
                <asp:GridView GridLines="None" ID="grdMedicoes" runat="server" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsMedicao"
                    PageSize="15" DataKeyNames="IdMedicao" EmptyDataText="Nenhuma medição encontrada.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="window.opener.setMedicao('<%# Eval("IdMedicao") %>', '<%# Eval("NomeCliente").ToString().Replace("'", "") %>', '<%# Eval("DescrTurno") %>', '<%# Eval("DescrSituacao") %>', '<%# Eval("DataMedicao", "{0:d}") %>', self);">
                                    <img alt="Selecionar" border="0" src="../Images/insert.gif" title="Selecionar" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdMedicao" HeaderText="Cód." SortExpression="IdMedicao" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="TelCliente" HeaderText="Tel. Cliente" SortExpression="TelCliente" />
                        <asp:BoundField DataField="Endereco" HeaderText="Endereço" SortExpression="Endereco" />
                        <asp:BoundField DataField="Cidade" HeaderText="Cidade" SortExpression="Cidade" />
                        <asp:BoundField DataField="Bairro" HeaderText="Bairro" SortExpression="Bairro" />
                        <asp:BoundField DataField="DataMedicao" HeaderText="Data Medição" SortExpression="DataMedicao"
                            DataFormatString="{0:d}" />
                        <asp:BoundField DataField="DescrTurno" HeaderText="Turno" ReadOnly="True" SortExpression="DescrTurno" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="DescrSituacao" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMedicao" runat="server" SelectMethod="GetList" TypeName="Glass.Data.DAL.MedicaoDAO"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" >
                    <SelectParameters>
                        <asp:Parameter Name="idMedicao" Type="UInt32" />
                        <asp:Parameter Name="idOrcamento" Type="UInt32" />
                        <asp:Parameter Name="idPedido" Type="UInt32" />
                        <asp:Parameter Name="idMedidor" Type="UInt32" />
                        <asp:Parameter Name="nomeMedidor" Type="String" />
                        <asp:Parameter Name="idVendedor" Type="UInt32" />
                        <asp:Parameter DefaultValue="1" Name="situacao" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtDataIni" DefaultValue="" Name="dataIni" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataFim" Name="dataFim" PropertyName="Text" Type="String" />
                        <asp:Parameter Name="dataEfetuar" Type="String" />
                        <asp:ControlParameter ControlID="txtNomeCliente" Name="nomeCli" PropertyName="Text"
                            Type="String" />
                        <asp:Parameter Name="bairro" Type="String" />
                        <asp:Parameter Name="endereco" Type="String" />
                        <asp:Parameter Name="telefone" Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
