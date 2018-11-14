<%@ Page Title="Troca/Devolução" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstTrocaDev.aspx.cs" Inherits="Glass.UI.Web.Listas.LstTrocaDev" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlTipoPerda.ascx" TagName="ctrlTipoPerda" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
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

            if (codInterno == "") {
                FindControl("hdfCodProduto", "input").value = "";
                return false;
            }

            try {
                var retorno = MetodosAjax.GetProd(codInterno).value.split(';');

                if (retorno[0] == "Erro") {
                    alert(retorno[1]);
                    FindControl("txtCodProd", "input").value = "";
                    return false;
                }
                else {
                    FindControl("hdfCodProduto", "input").value = retorno[1];
                }
            }
            catch (err) {
                alert(err.value);
            }
        }

        function openRpt(idTrocaDevolucao) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=TrocaDevolucao&idTrocaDevolucao=" + idTrocaDevolucao);
        }

        function openRptLista(exportarExcel) {
            var idTrocaDevolucao = FindControl("txtCodigo", "input").value;
            var idPedido = FindControl("txtPedido", "input").value;
            var tipo = FindControl("drpTipo", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var agrupar = FindControl("chkAgruparFunc", "input").checked;
            var agruparFuncionarioAssociadoCliente = FindControl("chkAgruparFuncionarioAssociadoCliente", "input").checked;
            var idCli = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idsFuncionario = FindControl("cblVendedor", "select").itens();
            var idsFuncionarioAssociadoCliente = FindControl("cblVendedorAssociadoCliente", "select").itens();
            var idProduto = FindControl("hdfCodProduto", "input").value;
            var alturaMin = FindControl("txtAlturaMin", "input").value;
            var alturaMax = FindControl("txtAlturaMax", "input").value;
            var larguraMin = FindControl("txtLarguraMin", "input").value;
            var larguraMax = FindControl("txtLarguraMax", "input").value;
            var idOrigemTrocaDevolucao = FindControl("ddlOrigem", "select") != null ? FindControl("ddlOrigem", "select").value : 0;
            var idTipoPerda = FindControl("ctrlTipoPerda1", "select") != null ? FindControl("ctrlTipoPerda1", "select").value : 0;
            var idSetor = FindControl("drpSetor", "select") != null ? FindControl("drpSetor", "select").value : 0;
            var tipoPedido = FindControl("cblTipoPedido", "select").itens();
            var idGrupo = FindControl("drpGrupo", "select").value;
            var idSubgrupo = FindControl("drpSubgrupo", "select").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ListaTrocaDevolucao&idTrocaDevolucao=" + idTrocaDevolucao +
                "&idPedido=" + idPedido + "&tipo=" + tipo + "&situacao=" + situacao + "&agrupar=" + agrupar + "&idCli=" + idCli +
                "&nomeCli=" + nomeCli + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&idsFunc=" + idsFuncionario +
                "&idsFuncionarioAssociadoCliente=" + idsFuncionarioAssociadoCliente + "&idProduto=" + idProduto +
                "&alturaMin=" + alturaMin + "&alturaMax=" + alturaMax + "&larguraMin=" + larguraMin + "&larguraMax=" + larguraMax +
                "&agruparFuncionarioAssociadoCliente=" + agruparFuncionarioAssociadoCliente + "&idOrigemTrocaDevolucao=" + idOrigemTrocaDevolucao +
                "&idTipoPerda=" + idTipoPerda + "&idSetor=" + idSetor + "&exportarExcel=" + exportarExcel + "&tipoPedido=" + tipoPedido+
                "&idGrupo=" + idGrupo + "&idSubgrupo=" + idSubgrupo);
        }

        function openRptPerdas() {
            var idTrocaDevolucao = FindControl("txtCodigo", "input").value;
            var idPedido = FindControl("txtPedido", "input").value;
            var tipo = FindControl("drpTipo", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var agruparFunc = FindControl("chkAgruparFunc", "input").checked;
            var agruparFuncionarioAssociadoCliente = FindControl("chkAgruparFuncionarioAssociadoCliente", "input").checked;
            var idCli = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idsFuncionario = FindControl("cblVendedor", "select").itens();
            var idsFuncionarioAssociadoCliente = FindControl("cblVendedorAssociadoCliente", "select").itens();
            var idProduto = FindControl("hdfCodProduto", "input").value;
            var alturaMin = FindControl("txtAlturaMin", "input").value;
            var alturaMax = FindControl("txtAlturaMax", "input").value;
            var larguraMin = FindControl("txtLarguraMin", "input").value;
            var larguraMax = FindControl("txtLarguraMax", "input").value;
            var idOrigemTrocaDevolucao = FindControl("ddlOrigem", "select") != null ? FindControl("ddlOrigem", "select").value : 0;
            var idTipoPerda = FindControl("ctrlTipoPerda1", "select") != null ? FindControl("ctrlTipoPerda1", "select").value : 0;
            var idSetor = FindControl("drpSetor", "select") != null ? FindControl("drpSetor", "select").value : 0;
            var tipoPedido = FindControl("cblTipoPedido", "select").itens();
            var idGrupo = FindControl("drpGrupo", "select").value;
            var idSubgrupo = FindControl("drpSubgrupo", "select").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ControlePerdasExternas&idTrocaDevolucao=" + idTrocaDevolucao +
                "&idPedido=" + idPedido + "&tipo=" + tipo + "&situacao=" + situacao + "&agruparFunc=" + agruparFunc +
                "&idCli=" + idCli + "&nomeCli=" + nomeCli + "&dataIni=" + dataIni + "&dataFim=" + dataFim +
                "&idsFunc=" + idsFuncionario + "&idsFuncionarioAssociadoCliente=" + idsFuncionarioAssociadoCliente + "&idProduto=" + idProduto +
                "&alturaMin=" + alturaMin + "&alturaMax=" + alturaMax + "&agruparFuncionarioAssociadoCliente=" + agruparFuncionarioAssociadoCliente +
                "&larguraMin=" + larguraMin + "&larguraMax=" + larguraMax + "&idOrigemTrocaDevolucao=" + idOrigemTrocaDevolucao +
                "&idTipoPerda=" + idTipoPerda + "&idSetor=" + idSetor + "&tipoPedido=" + tipoPedido +
                "&idGrupo=" + idGrupo + "&idSubgrupo=" + idSubgrupo);
        }

        function cancelar(idTrocaDevolucao) {
            openWindow(150, 420, "../Utils/SetMotivoCancTroca.aspx?idTrocaDev=" + idTrocaDevolucao);
        }
        
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Código"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodigo" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" ForeColor="#0066FF" Text="Pedido"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPedido" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" ForeColor="#0066FF" Text="Tipo"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipo" runat="server">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Troca</asp:ListItem>
                                <asp:ListItem Value="2">Devolução</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" ForeColor="#0066FF" Text="Situação"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Aberta</asp:ListItem>
                                <asp:ListItem Value="2">Finalizada</asp:ListItem>
                                <asp:ListItem Value="3">Cancelada</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" ForeColor="#0066FF" Text="Setor"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSetor" runat="server" AppendDataBoundItems="True" DataSourceID="odsSetor"
                                DataTextField="Descricao" DataValueField="IdSetor">
                                <asp:ListItem Value="0" Selected="True">Todos</asp:ListItem>
                            </asp:DropDownList></td>
                        <td>
                            <asp:ImageButton ID="ImageButton11" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Tipo Perda" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                           <uc3:ctrlTipoPerda ID="ctrlTipoPerda1" runat="server" ExibirItemVazio="true" 
                                                PermitirVazio="True" />

                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Cód. Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="setProduto();" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblVendedor" runat="server" DataSourceID="odsVendedor" DataTextField="Nome"
                                DataValueField="IdFunc" AppendDataBoundItems="True" Title="Selecione um vendedor">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Funcionário Assoc. Cli." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblVendedorAssociadoCliente" runat="server"
                                DataSourceID="odsVendedorAssociadoCliente" DataTextField="Nome"
                                DataValueField="IdFunc" AppendDataBoundItems="True" Title="Selecione um vendedor">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Origem Troca/Devolução" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlOrigem" runat="server" DataSourceID="odsOrigem" DataTextField="Descricao"
                                DataValueField="idOrigemTrocaDesconto" AppendDataBoundItems=true>
                                <asp:ListItem Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource ID="odsOrigem" runat="server" Culture="pt-BR" SelectMethod="GetList"
                                TypeName="Glass.Data.DAL.OrigemTrocaDescontoDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click"/>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td style="color: #0066FF">
                            Altura
                        </td>
                        <td>
                            mín.:
                        </td>
                        <td>
                            <asp:TextBox ID="txtAlturaMin" runat="server" Width="50px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                        <td>
                            máx.:
                        </td>
                        <td>
                            <asp:TextBox ID="txtAlturaMax" runat="server" Width="50px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                        <td style="color: #0066FF">
                            Largura
                        </td>
                        <td>
                            mín.:
                        </td>
                        <td>
                            <asp:TextBox ID="txtLarguraMin" runat="server" Width="50px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                        <td>
                            máx.:
                        </td>
                        <td>
                            <asp:TextBox ID="txtLarguraMax" runat="server" Width="50px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblTipoPedido" runat="server" ForeColor="#0066FF" Text="Tipo Pedido"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblTipoPedido" runat="server" Width="110px" CheckAll="False" Title="Selecione o tipo"
                                DataSourceID="odsTipoPedido" DataTextField="Descr" DataValueField="Id" ImageURL="~/Images/DropDown.png"
                                JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" OpenOnStart="False">
                            </sync:CheckBoxListDropDown>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoPedido" runat="server" SelectMethod="GetTipoPedidoFilter"
                                TypeName="Glass.Data.Helper.DataSources">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupo" runat="server" AutoPostBack="True" DataSourceID="odsGrupo"
                                DataTextField="Name" DataValueField="Id" AppendDataBoundItems="true"
                                EnableViewState="false">
                                <asp:ListItem Text="Todos" Value="" />
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsGrupo" runat="server"
                                SelectMethod="ObtemGruposProduto"
                                TypeName="Glass.Global.Negocios.IGrupoProdutoFluxo">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:Label ID="Label14" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSubgrupo" runat="server" AutoPostBack="True" DataSourceID="odsSubgrupo"
                                DataTextField="Name" DataValueField="Id" AppendDataBoundItems="true"
                                EnableViewState="false">
                                <asp:ListItem Text="Todos" Value="" />
                            </asp:DropDownList>

                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSubgrupo" runat="server"
                                SelectMethod="ObtemSubgruposProduto"
                                TypeName="Glass.Global.Negocios.IGrupoProdutoFluxo">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="drpGrupo" Name="idGrupoProd" PropertyName="SelectedValue" Type="Int32" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td nowrap="nowrap">
                            <asp:CheckBox ID="chkAgruparFunc" runat="server" AutoPostBack="true"
                                Text="Agrupar relatório por funcionário" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:CheckBox ID="chkAgruparFuncionarioAssociadoCliente" runat="server" AutoPostBack="true"
                                Text="Agrupar relatório por funcionário associado ao cliente" />
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
                <asp:LinkButton ID="lkbInserir" runat="server" OnClick="lkbInserir_Click">Inserir troca/devolução</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdTroca" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdTrocaDevolucao" DataSourceID="odsTroca"
                    EmptyDataText="Não há trocas cadastradas." CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" ImageUrl="~/Images/EditarGrid.gif"
                                    OnClientClick='<%# "redirectUrl(\"../Cadastros/CadTrocaDev.aspx?idTrocaDev=" + Eval("IdTrocaDevolucao") + "\"); return false" %>'
                                    Visible='<%# Eval("EditEnabled") %>' />
                                <asp:ImageButton ID="imgCancelar" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick='<%# "cancelar(" + Eval("IdTrocaDevolucao") + "); return false;" %>'
                                    Visible='<%# Eval("CancelEnabled") %>' />
                                <asp:ImageButton ID="imbRelatorio" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    OnClientClick='<%# "openRpt(" + Eval("IdTrocaDevolucao") + "); return false" %>' />
                                <asp:PlaceHolder ID="pchAnexos" runat="server"><a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=<%# Eval("IdTrocaDevolucao") %>&tipo=trocaDevolucao&#039;); return false;'>
                                    <img border="0px" src="../Images/Clipe.gif"></img></a></asp:PlaceHolder>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdTrocaDevolucao" HeaderText="Cód." SortExpression="IdTrocaDevolucao" />
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Func. Solic." SortExpression="NomeFunc" />
                        <asp:BoundField DataField="IdNomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="DescrTipo" HeaderText="Tipo" SortExpression="DescrTipo" />
                        <asp:BoundField DataField="DataTroca" DataFormatString="{0:d}" HeaderText="Data"
                            SortExpression="DataTroca" />
                        <asp:BoundField DataField="DataErro" DataFormatString="{0:d}" HeaderText="Data Erro"
                            SortExpression="DataErro" />
                        <asp:BoundField DataField="Loja" HeaderText="Loja" SortExpression="Loja" />
                        <asp:TemplateField HeaderText="Crédito Gerado" SortExpression="CreditoGerado">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" ForeColor="Green" Text='<%# Bind("CreditoGerado", "{0:c}") %>'
                                    Visible='<%# float.Parse(Eval("CreditoGerado").ToString()) > 0 %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("CreditoGerado") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Excedente" SortExpression="ValorExcedente">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" ForeColor="Red" Text='<%# Bind("ValorExcedente", "{0:c}") %>'
                                    Visible='<%# float.Parse(Eval("ValorExcedente").ToString()) > 0 %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("ValorExcedente") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="DescrSituacao" />
                        <asp:BoundField DataField="DescrOrigemTrocaDevolucao" HeaderText="Origem" SortExpression="DescrOrigemTrocaDevolucao" />
                        <asp:BoundField DataField="Setor" HeaderText="Setor" SortExpression="Setor" />
                        <asp:TemplateField HeaderText="Obs" SortExpression="Descricao">
                            <ItemTemplate>
                                <asp:Label ID="lblDescricaoObs" runat="server" Text='<%# string.Format("{0}{1}", Eval("Descricao"), Eval("Obs")) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="NomeUsuCad" HeaderText="Func. Cad." SortExpression="NomeUsuCad" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc2:ctrlLogPopup ID="ctrlLogPopup1" runat="server" IdRegistro='<%# Eval("IdTrocaDevolucao") %>'
                                    Tabela="TrocaDev" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">

                <table>
                    <tr>
                        <td><asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRptLista(false); return false"> <img src="../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
                            &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRptLista(true); return false"><img border="0" 
                    src="../Images/Excel.gif" alt="Exportar para o Excel" /> Exportar para o Excel</asp:LinkButton></td>
                    </tr>
                    <tr>
                        <td><asp:LinkButton ID="lnkPerdas" runat="server" OnClientClick="openRptPerdas(); return false"> <img src="../Images/Printer.png" border="0" /> Controle de perdas externas</asp:LinkButton></td>
                    </tr>
                </table>
                
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTroca" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" SelectMethod="GetList"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.TrocaDevolucaoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodigo" Name="idTrocaDevolucao" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtPedido" Name="idPedido" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpTipo" Name="tipo" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="cblVendedor" Name="idsFuncionario" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="cblVendedorAssociadoCliente" Name="idsFuncionarioAssociadoCliente" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="hdfCodProduto" Name="idProduto" PropertyName="Value"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtAlturaMin" Name="alturaMin" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtAlturaMax" Name="alturaMax" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtLarguraMin" Name="larguraMin" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtLarguraMax" Name="larguraMax" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="ddlOrigem" Name="idOrigemTrocaDevolucao" PropertyName="Text"
                            Type="Uint32" />
                        <asp:ControlParameter ControlID="ctrlTipoPerda1" Name="idTipoPerda" PropertyName="IdTipoPerda"
                            Type="Uint32" />
                        <asp:ControlParameter ControlID="drpSetor" Name="idSetor" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="cblTipoPedido" Name="tipoPedido" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpGrupo" Name="idGrupo" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="drpSubgrupo" Name="idSubgrupo" PropertyName="SelectedValue" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsVendedor" runat="server" SelectMethod="GetVendedoresOrca"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="0" Name="idOrcamento" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsVendedorAssociadoCliente" runat="server" SelectMethod="GetVendedoresOrca"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="0" Name="idOrcamento" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSetor" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.SetorDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdfCodProduto" runat="server" />
</asp:Content>
