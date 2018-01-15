<%@ Page Title="Vendas por Cliente" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ListaVendasCliente.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaVendasCliente" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = ListaVendasCliente.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function getFunc(idFunc) {
            if (idFunc.value == "")
                return;

            var retorno = ListaVendasCliente.GetFunc(idFunc.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFunc.value = "";
                FindControl("txtNomeFunc", "input").value = "";
                return false;
            }

            FindControl("txtNomeFunc", "input").value = retorno[1];
        }

        function openRpt(exportarExcel) {
            if (!validate())
                return;

            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var idsRota = FindControl("drpRota", "select").itens();
            var revenda = FindControl("chkRevenda", "input").checked;
            var mesInicio = FindControl("drpInicio", "select").value;
            var anoInicio = FindControl("txtInicio", "input").value;
            var mesFim = FindControl("drpFim", "select").value;
            var anoFim = FindControl("txtFim", "input").value;
            var ordenar = FindControl("drpOrdenar", "select").value;
            var tipoMedia = FindControl("cblTipoMedia", "select").itens();
            var idsFunc = FindControl("cblFuncionario", "select").itens();
            var valorMinimo = FindControl("txtValorMin", "input").value;
            var valorMaximo = FindControl("txtValorMax", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var lojaCliente = FindControl("chkLojaCliente", "input").checked;
            var idsFuncAssociaCliente = FindControl("cblFuncAssociCliente", "select").itens();
            var tipoCliente = FindControl("ddlGrupoCliente", "select").itens();
            var situacaoCliente = FindControl("drpSituacaoCli", "select").value;

            openWindow(600, 800, "RelBase.aspx?rel=VendasCliente&idCliente=" + idCliente + "&nomeCliente=" + nomeCliente + "&idsRota=" + idsRota +
                "&revenda=" + revenda + "&mesInicio=" + mesInicio + "&anoInicio=" + anoInicio + "&mesFim=" + mesFim +
                "&anoFim=" + anoFim + "&tipoMedia=" + tipoMedia + "&ordenar=" + ordenar + "&idsFunc=" + idsFunc + "&idsFuncAssociaCliente=" + idsFuncAssociaCliente +
                "&tipoVendas=0&exportarExcel=" + exportarExcel + "&valorMinimo=" + valorMinimo + "&valorMaximo=" + valorMaximo + 
                "&idLoja=" + idLoja + "&lojaCliente=" + lojaCliente + "&tipoCliente=" + tipoCliente + "&situacaoCliente=" + situacaoCliente);
        }
    </script>

    <table>
        <tr>
            <td align="center" style="height: 144px">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Vendedor associado ao cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblFuncAssociCliente" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsFuncAssociaCliente" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Situação Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacaoCli" runat="server">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Ativo</asp:ListItem>
                                <asp:ListItem Value="2">Inativo</asp:ListItem>
                                <asp:ListItem Value="3">Cancelado</asp:ListItem>
                                <asp:ListItem Value="4">Bloqueado</asp:ListItem>
                            </asp:DropDownList>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>                        
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" />
                            <asp:CheckBox ID="chkLojaCliente" runat="server" Text="Loja do Cliente?" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Vendedor associado ao pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblFuncionario" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsFuncionario" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:DropDownList ID="drpInicio" runat="server">
                                <asp:ListItem Value="1">Janeiro</asp:ListItem>
                                <asp:ListItem Value="2">Fevereiro</asp:ListItem>
                                <asp:ListItem Value="3">Março</asp:ListItem>
                                <asp:ListItem Value="4">Abril</asp:ListItem>
                                <asp:ListItem Value="5">Maio</asp:ListItem>
                                <asp:ListItem Value="6">Junho</asp:ListItem>
                                <asp:ListItem Value="7">Julho</asp:ListItem>
                                <asp:ListItem Value="8">Agosto</asp:ListItem>
                                <asp:ListItem Value="9">Setembro</asp:ListItem>
                                <asp:ListItem Value="10">Outubro</asp:ListItem>
                                <asp:ListItem Value="11">Novembro</asp:ListItem>
                                <asp:ListItem Value="12">Dezembro</asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="txtInicio" runat="server" MaxLength="4" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)" Columns="5"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvInicio" runat="server" ErrorMessage="*" ControlToValidate="txtInicio"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="a" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFim" runat="server">
                                <asp:ListItem Value="1">Janeiro</asp:ListItem>
                                <asp:ListItem Value="2">Fevereiro</asp:ListItem>
                                <asp:ListItem Value="3">Março</asp:ListItem>
                                <asp:ListItem Value="4">Abril</asp:ListItem>
                                <asp:ListItem Value="5">Maio</asp:ListItem>
                                <asp:ListItem Value="6">Junho</asp:ListItem>
                                <asp:ListItem Value="7">Julho</asp:ListItem>
                                <asp:ListItem Value="8">Agosto</asp:ListItem>
                                <asp:ListItem Value="9">Setembro</asp:ListItem>
                                <asp:ListItem Value="10">Outubro</asp:ListItem>
                                <asp:ListItem Value="11">Novembro</asp:ListItem>
                                <asp:ListItem Value="12">Dezembro</asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="txtFim" runat="server" MaxLength="4" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)" Columns="5"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvFim" runat="server" ErrorMessage="*" ControlToValidate="txtFim"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>                        
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Total Vendas" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorMin" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true)" Width="70px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="a" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorMax" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true)" Width="70px"></asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Tipo Fiscal Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoFiscalCli" runat="server">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Consumidor Final</asp:ListItem>
                                <asp:ListItem Value="2">Revenda</asp:ListItem>
                            </asp:DropDownList>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkRevenda" runat="server" ForeColor="#0066FF" Text="Revenda" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="lblRota" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpRota" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsRota" DataTextField="Descricao" DataValueField="IdRota">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqRota" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Média de compra" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblTipoMedia" runat="server" Title="Selecione uma opção"
                                Font-Names="Arial">
                                <asp:ListItem Value="1">Acima da média</asp:ListItem>
                                <asp:ListItem Value="2">Dentro da média</asp:ListItem>
                                <asp:ListItem Value="3">Abaixo da média</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td nowrap="nowrap" style="height: 30px">
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>                        
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Grupo de Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="ddlGrupoCliente" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsGrupoCliente" DataTextField="Name" DataValueField="Id">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td nowrap="nowrap" style="height: 30px">
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Ordenar por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenar" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="1">Total (cresc)</asp:ListItem>
                                <asp:ListItem Value="2">Total (decresc)</asp:ListItem>
                                <asp:ListItem Value="3">Cliente (cresc)</asp:ListItem>
                                <asp:ListItem Value="4">Cliente (decresc)</asp:ListItem>
                            </asp:DropDownList>
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
                <asp:GridView GridLines="None" ID="grdVendas" runat="server" AutoGenerateColumns="False"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" OnRowDataBound="grdVendas_RowDataBound" EmptyDataText="Não foram encontradas vendas para esse filtro específico."
                    AllowPaging="True" DataSourceID="odsVendas" PageSize="20">
                    <Columns>
                        <asp:BoundField DataField="IdCliente" HeaderText="Cód." SortExpression="IdCliente" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Nome" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="TresPrimeirosNomesVendedor" HeaderText="Vendedor" SortExpression="TresPrimeirosNomesVendedor" />
                        <asp:BoundField DataField="MediaCompraCliente" HeaderText="Média" SortExpression="MediaCompraCliente" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:c}" />
                        <asp:BoundField DataField="TotM2" HeaderText="Total M²" SortExpression="TotM2" />
                        <asp:BoundField DataField="TotalItens" HeaderText="Total Itens" SortExpression="TotalItens" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsVendas" runat="server" SelectCountMethod="GetListCount"
                    SelectMethod="GetList" TypeName="Glass.Data.RelDAL.VendasDAO" OnSelected="odsVendas_Selected"
                    EnablePaging="true" SortParameterName="sortExpression" MaximumRowsParameterName="pageSize"
                    StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpRota" Name="idsRotas" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="chkRevenda" Name="revenda" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="drpInicio" Name="mesInicio" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtInicio" Name="anoInicio" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpFim" Name="mesFim" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtFim" Name="anoFim" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="drpOrdenar" Name="ordenar" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="cblTipoMedia" Name="tipoMedia" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="cblFuncionario" DefaultValue="" Name="idsFuncionario"
                            PropertyName="SelectedValue" Type="String" />
                        <asp:Parameter Name="nomeFuncionario" Type="String" />
                        <asp:ControlParameter ControlID="txtValorMin" Name="valorMinimo" PropertyName="Text"
                            Type="Decimal" />
                        <asp:ControlParameter ControlID="cblFuncAssociCliente" DefaultValue="" Name="idsFuncAssociaCliente"
                            PropertyName="SelectedValue" Type="String" />                        
                        <asp:ControlParameter ControlID="txtValorMax" Name="valorMaximo" PropertyName="Text"
                            Type="Decimal" />
                        <asp:ControlParameter ControlID="drpSituacaoCli" Name="situacaoCliente" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpTipoFiscalCli" Name="tipoFiscalCliente" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="chkLojaCliente" Name="lojaCliente" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="ddlGrupoCliente" Name="tipoCliente" PropertyName="SelectedValue"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
                    SelectMethod="GetVendedores" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncAssociaCliente" runat="server"
                    SelectMethod="GetVendedoresDropAssociaCliente" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>                
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.RotaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsGrupoCliente" runat="server"
                    SelectMethod="ObtemDescritoresTipoCliente" TypeName="Glass.Global.Negocios.IClienteFluxo">
                </colo:VirtualObjectDataSource>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;"><img border="0" 
                    src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
