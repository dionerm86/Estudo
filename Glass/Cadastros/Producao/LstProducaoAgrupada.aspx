<%@ Page Title="Consulta Produção" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstProducaoAgrupada.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Producao.LstProducaoAgrupada" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register src="../../Controls/ctrlLoja.ascx" tagname="ctrlLoja" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script>

        function openRpt(exportarExcel) {
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var idFunc = FindControl("drpFuncionario", "select").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var setor = FindControl("drpSetor", "select").value;
            var idsSubgrupo = FindControl("drpSubgrupo", "select").itens();
            var agruparDia = FindControl("chkAgruparDia", "input").checked;
            var tipoCliente = FindControl("drpTipoCliente", "select").itens();
            var idRota = FindControl("drpRota", "select").value;

            openWindow(600, 800, "../../Relatorios/RelBase.aspx?rel=ProducaoAgrupada&idPedido=" + idPedido +
                "&idLoja=" + idLoja + "&idCliente=" + idCliente + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&situacao=" + situacao +
                "&idSetor=" + setor + "&tiposSituacoes=0&tipoPedido=0&idsSubgrupo=" + idsSubgrupo + "&idFunc=" + idFunc + "&agruparDia=" + agruparDia +
                "&tipoCliente=" + tipoCliente + "&idRota=" + idRota + "&exportarExcel=" + exportarExcel);
        }

        function getCli(idCli) {
            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

    </script>

    <table width="100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Num. Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="45px" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" MaxLength="6"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td align="left">
                            <asp:Label ID="Label2" runat="server" ForeColor="#0066FF" Text="Tipo de Cliente"></asp:Label>
                        </td>
                        <td align="left">
                            <sync:CheckBoxListDropDown ID="drpTipoCliente" runat="server" CheckAll="False" DataSourceID="odsTipoCliente"
                                DataTextField="Name" DataValueField="Id" ImageURL="~/Images/DropDown.png"
                                OpenOnStart="False" Title="">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td align="left">
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgruparDia" runat="server" Text="Agrupar impressão por dia" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label21" runat="server" Text="Loja (Produção)" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" ForeColor="#0066FF" Text="Situação"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpSetor_SelectedIndexChanged">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Produção</asp:ListItem>
                                <asp:ListItem Value="2">Perda</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label20" runat="server" ForeColor="#0066FF" Text="Setor"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSetor" runat="server" AutoPostBack="True" DataSourceID="odsSetor"
                                DataTextField="Descricao" DataValueField="IdSetor" OnSelectedIndexChanged="drpSetor_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Subgrupo"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpSubgrupo" runat="server" DataSourceID="odsSubgrupo"
                                DataTextField="Descricao" DataValueField="IdSubgrupoProd">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label22" runat="server" ForeColor="#0066FF" Text="Funcionário Setor"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpFuncionario" runat="server" AutoPostBack="True" DataSourceID="odsFuncionario"
                                DataTextField="Nome" DataValueField="IdFunc">
                            </asp:DropDownList>
                        </td>
                        <td align="left">
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td align="left">
                            <asp:Label ID="Label23" runat="server" ForeColor="#0066FF" Text="Rota"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpRota" runat="server" AppendDataBoundItems="True" DataSourceID="odsRota"
                                DataTextField="Descricao" DataValueField="IdRota">
                                <asp:ListItem Value="0">Selecione uma Rota</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="left">
                            <asp:ImageButton ID="imgPesq5" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td align="left">
                            <asp:Label ID="lblPeriodoSit" runat="server" ForeColor="#0066FF" Text="Período (Impr. Etiqueta)"></asp:Label>&nbsp;
                        </td>
                        <td align="left" nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" CausesValidation="False" />
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
                <asp:GridView GridLines="None" ID="grdPecas" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    DataKeyNames="IdProdPedProducao" DataSourceID="odsPecas" EmptyDataText="Nenhuma peça encontrada."
                    PageSize="12" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                        <asp:BoundField DataField="Qtde" HeaderText="Qtde" SortExpression="Qtde" />
                        <asp:BoundField DataField="Peso" HeaderText="Peso" SortExpression="Peso" />
                        <asp:BoundField DataField="TotM2" HeaderText="Tot M²" SortExpression="TotM2" DataFormatString="{0:N}" />
                        <asp:BoundField DataField="Total" HeaderText="Total (R$)" SortExpression="Total"
                            DataFormatString="{0:N}" />
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPecas" runat="server" SelectMethod="GetListConsultaAgrupada"
                    TypeName="Glass.Data.DAL.ProdutoPedidoProducaoDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountConsultaAgrupada" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" DataObjectTypeName="Glass.Data.Model.ProdutoPedidoProducao"
                    DeleteMethod="Delete" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpRota" Name="idRota" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpSetor" Name="idSetor" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSubgrupo" Name="idsSubgrupo" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:Parameter DefaultValue="true" Name="setoresPosteriores" Type="Boolean" />
                        <asp:ControlParameter ControlID="drpTipoCliente" Name="tipoCliente" PropertyName="SelectedValue"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSubgrupo" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.SubgrupoProdDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="1" Name="idGrupo" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSetor" runat="server" SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.SetorDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetProducaoFiltro"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpSetor" Name="idSetor" PropertyName="SelectedValue"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCliente" runat="server" SelectMethod="ObtemDescritoresTipoCliente" 
                    TypeName="Glass.Global.Negocios.IClienteFluxo">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.RotaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false;">
                                <img border="0" src="../../Images/Printer.png" /> Imprimir</asp:LinkButton>
                            &nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                                src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
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
    </table>
</asp:Content>
