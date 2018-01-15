<%@ Page Title="Perdas" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="Perdas.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Producao.Perdas" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(exportarExcel)
        {
            var idFuncPerda = FindControl("drpFuncPerda", "select").value;
            var idPedido = FindControl("txtPedido", "input").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idsSetor = FindControl("cbxdrpSetorPerda", "select").itens();
            var agrupar = FindControl("drpAgrupar", "select").value;

            openWindow(600, 800, "RelBase.aspx?rel=Perdas&idFuncPerda=" + idFuncPerda + "&idPedido=" + idPedido + 
                "&idCliente=" + idCliente + "&nomeCliente=" + nomeCliente + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&idsSetor=" + idsSetor +
                "&agrupar=" + agrupar + "&exportarExcel=" + exportarExcel);
        }
        
        function getCli(idCli)
        {
            if (idCli.value == "")
                return;

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro")
            {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Funcionário Perda" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncPerda" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsFuncPerda" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Período Perda" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td style="width: 73px">
                            <asp:Label ID="Label12" runat="server" Text="Setor Perda" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbxdrpSetorPerda" runat="server" CheckAll="False" DataSourceID="odsSetor"
                                DataTextField="Descricao" DataValueField="IdSetor" ImageURL="~/Images/DropDown.png"
                                JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" OpenOnStart="False"
                                Title="">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPerda" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Departamento" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbxdrpDepartamento" runat="server" CheckAll="False"
                                DataSourceID="odsDepartamento" DataTextField="Descricao" DataValueField="IdDepartamento"
                                ImageURL="~/Images/DropDown.png" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"
                                OpenOnStart="False" Title="">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageDepart" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPedido" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Agrupar por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpAgrupar" runat="server">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Value="1">Espessura</asp:ListItem>
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
                <asp:GridView ID="grdPerda" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsPerdas" GridLines="None"
                    PageSize="15" EmptyDataText="Não há perdas para esse filtro.">
                    <Columns>
                        <asp:BoundField DataField="IdPedidoExibir" HeaderText="Pedido" SortExpression="IdPedidoExibir" />
                        <asp:BoundField DataField="SiglaTipoPedido" HeaderText="Tipo Ped." ReadOnly="True"
                            SortExpression="SiglaTipoPedido" />
                        <asp:BoundField DataField="CodCliente" HeaderText="Pedido Cli." SortExpression="CodCliente" />
                        <asp:BoundField DataField="IdNomeCliente" HeaderText="Cliente" ReadOnly="True" SortExpression="IdNomeCliente" />
                        <asp:BoundField DataField="DescrProdLargAlt" HeaderText="Produto" ReadOnly="True"
                            SortExpression="DescrProdLargAlt" />
                        <asp:BoundField DataField="DataPerda" HeaderText="Data Perda" SortExpression="DataPerda" />
                        <asp:BoundField DataField="NomeFuncPerda" HeaderText="Funcionário Perda" SortExpression="NomeFuncPerda" />
                        <asp:BoundField DataField="DescrTipoPerdaSemObs" HeaderText="Tipo Perda" ReadOnly="True"
                            SortExpression="DescrTipoPerdaSemObs" />
                        <asp:BoundField DataField="Obs" HeaderText="Motivo" SortExpression="Obs" />
                        <asp:BoundField DataField="TotM2" DataFormatString="{0:0.##}" HeaderText="Tot. M2"
                            SortExpression="TotM2" />
                        <asp:BoundField DataField="NumEtiqueta" HeaderText="Etiqueta" SortExpression="NumEtiqueta" />
                        <asp:BoundField DataField="DescrSetor" HeaderText="Setor" SortExpression="DescrSetor" />
                        <asp:BoundField DataField="DescrDepart" HeaderText="Departamento" SortExpression="DescrDepart" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false;">
                    <img src="../../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPerdas" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountPerda" SelectMethod="GetListPerda" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutoPedidoProducaoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpFuncPerda" Name="idFuncPerda" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtPedido" Name="idPedido" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="cbxdrpSetorPerda" Name="idsSetor" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="cbxdrpDepartamento" Name="idsDepartamento" PropertyName="SelectedValue" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncPerda" runat="server" SelectMethod="GetProducao"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="true" Name="apenasFuncPerda" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSetor" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.SetorDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsDepartamento" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.DepartamentoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
