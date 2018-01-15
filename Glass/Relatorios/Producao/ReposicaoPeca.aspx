<%@ Page Title="Controle de Perdas" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ReposicaoPeca.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Producao.ReposicaoPeca" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(exportarExcel)
        {
            var idLoja = FindControl("drpLoja", "select").value;
            var idFuncPerda = FindControl("drpFuncPerda", "select").value;
            var idPedido = FindControl("txtPedido", "input").value;
            var numeroNFe = FindControl("txtNumeroNFe", "input").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idSetor = FindControl("drpSetor", "select").itens();
            var codInterno = FindControl("txtCodProd", "input").value;
            var descrProd = FindControl("txtDescrProd", "input").value;
            var idTurno = FindControl("ddlTurno", "select").value;
            var idTipoPerda = FindControl("drpTipoPerda", "select").itens();
            var idCorVidro = FindControl("drpCorVidro", "select").value;
            var espessuraVidro = FindControl("txtEspessura", "input").value;

            openWindow(600, 800, "RelBase.aspx?rel=PerdasRepos&idFuncPerda=" + idFuncPerda + "&idPedido=" + idPedido + "&idCliente=" + idCliente +
                "&nomeCliente=" + nomeCliente + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&idSetor=" + idSetor + "&codInterno=" + codInterno +
                "&descrProd=" + descrProd + "&exportarExcel=" + exportarExcel + "&turno=" + idTurno + "&idTipoPerda=" + idTipoPerda + 
                "&idCorVidro=" + idCorVidro + "&espessura=" + espessuraVidro + "&numeroNFe=" + numeroNFe + "&idLoja=" + idLoja);
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

        // Carrega dados do produto com base no código do produto passado
        function setProduto() {
            var codInterno = FindControl("txtCodProd", "input").value;

            if (codInterno == "")
                return false;

            try {
                var retorno = MetodosAjax.GetProd(codInterno).value.split(';');

                if (retorno[0] == "Erro") {
                    alert(retorno[1]);
                    FindControl("txtCodProd", "input").value = "";
                    return false;
                }

                FindControl("txtDescrProd", "input").value = retorno[2];
            }
            catch (err) {
                alert(err.value);
            }
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
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Setor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown runat="server" ID="drpSetor" AppendDataBoundItems="true" Title="Selecione os setores"
                                DataSourceID="odsSetor" DataTextField="Descricao" DataValueField="IdSetor" CheckAll="true">
                                <asp:ListItem Value="-1">Devolução</asp:ListItem>
                                <asp:ListItem Value="-2">Troca</asp:ListItem>
                                <asp:ListItem Value="-3">Chapa de Vidro</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton821" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Turno" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlTurno" runat="server" DataSourceID="odsTurno" DataTextField="Descricao"
                                DataValueField="IdTurno" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
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
                            <uc2:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true" />
                        </td>
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
                            <asp:TextBox ID="txtNome" runat="server" Width="120px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="setProduto();"></asp:TextBox>
                            <asp:TextBox ID="txtDescrProd" runat="server" Width="120px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Tipo Perda" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown runat="server" ID="drpTipoPerda" AppendDataBoundItems="true" Title="Selecione os tipos"
                                DataSourceID="odsTipoPerda" DataTextField="Descricao" DataValueField="IdTipoPerda">
                                <asp:ListItem Value="0" Selected="True">Todos</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Número NF-e" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumeroNFe" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Cor do Vidro" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpCorVidro" runat="server" AppendDataBoundItems="true" DataSourceID="odsCorVidro"
                                DataTextField="Descricao" DataValueField="IdCorVidro">
                                <asp:ListItem Value="0" Text="Selecione" />
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Espessura do Vidro" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtEspessura" runat="server" Width="35px" MaxLength="5" onkeypress="return soNumeros(event, false, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq18" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
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
                        <asp:BoundField DataField="NumeroNFe" HeaderText="Núm. NFe" 
                            SortExpression="NumeroNFe" />
                        <asp:BoundField DataField="SiglaTipoPedido" HeaderText="Tipo Ped." ReadOnly="True"
                            SortExpression="SiglaTipoPedido" />
                        <asp:BoundField DataField="CodCliente" HeaderText="Pedido Cli." SortExpression="CodCliente" />
                        <asp:BoundField DataField="IdNomeCliente" HeaderText="Cliente" ReadOnly="True" SortExpression="IdNomeCliente" />
                        <asp:BoundField DataField="CodInterno" HeaderText="Código"
                            SortExpression="CodInterno" />
                        <asp:BoundField DataField="Altura" HeaderText="Altura" 
                            SortExpression="Altura" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura" 
                            SortExpression="Largura" />
                        <asp:BoundField DataField="Cor" HeaderText="Cor" SortExpression="Cor" />
                        <asp:BoundField DataField="Espessura" HeaderText="Esp." 
                            SortExpression="Espessura" />
                        <asp:BoundField DataField="DescrSetorRepos" HeaderText="Setor Perda" SortExpression="DescrSetorRepos" />
                        <asp:BoundField DataField="DataRepos" HeaderText="Data Perda" SortExpression="DataRepos" />
                        <asp:BoundField DataField="NomeFuncPerda" HeaderText="Funcionário Perda" SortExpression="NomeFuncPerda" />
                        <asp:BoundField DataField="DescrTipoPerdaSemObs" HeaderText="Tipo Perda" ReadOnly="True"
                            SortExpression="DescrTipoPerdaSemObs" />
                        <asp:BoundField DataField="Obs" HeaderText="Motivo" SortExpression="Obs" />
                        <asp:BoundField DataField="TotM2" DataFormatString="{0:0.##}" HeaderText="Tot. M2"
                            SortExpression="TotM2" />
                        <asp:BoundField DataField="Qtde" HeaderText="Qtde." SortExpression="Qtde" />
                        <asp:BoundField DataField="NumEtiqueta" HeaderText="Etiqueta" SortExpression="NumEtiqueta" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false;"> <img src="../../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPerdas" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountPerdaReposPeca" SelectMethod="GetListPerdaReposPeca"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutoPedidoProducaoDAO"
                    >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpFuncPerda" Name="idFuncPerda" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtPedido" Name="idPedido" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInterno" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescrProd" Name="descrProd" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpSetor" Name="idSetor" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="ddlTurno" Name="idTurno" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpTipoPerda" Name="idTipoPerda" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpCorVidro" Name="idCorVidro" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtEspessura" Name="espessuraVidro" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="txtNumeroNFe" Name="numeroNFe" PropertyName="Text" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncPerda" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.FuncionarioDAO" >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSetor" runat="server" SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.SetorDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTurno" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.TurnoDAO"
                    >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoPerda" runat="server" SelectMethod="GetOrderedList"
                    TypeName="Glass.Data.DAL.TipoPerdaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCorVidro" runat="server" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" EnablePaging="true" MaximumRowsParameterName="pageSize"
                    SelectMethod="GetList" TypeName="Glass.Data.DAL.CorVidroDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
