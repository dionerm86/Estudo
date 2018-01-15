<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstSugestaoCliente.aspx.cs"
    EnableEventValidation="false" Inherits="Glass.UI.Web.Listas.LstSugestaoCliente" Title="Sugestões / Reclamações" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlTipoFuncionario.ascx" TagName="ctrlTipoFuncionario"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function getFunc(idFunc) {
            if (idFunc.value == "")
                return;

            var retorno = LstSugestaoCliente.GetFunc(idFunc.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFunc.value = "";
                FindControl("txtNomeFunc", "input").value = "";
                return false;
            }

            FindControl("txtNomeFunc", "input").value = retorno[1];
        }

        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = LstSugestaoCliente.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function openRpt(exportarExcel) {
            var idSug = FindControl("txtCodSug", "input").value;
            var idCli = FindControl("hdfIdCli", "input").value;
            var idPedido = FindControl("hdfIdPedido", "input").value;
            var idOrcamento = FindControl("hdfIdOrcamento", "input").value;
            var desc = FindControl("txtDescricao", "input").value;
            var idFunc = FindControl("drpFuncionario", "select").value;
            var dataIni = FindControl("ctrlDataCadIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataCadFim_txtData", "input").value;
            var tipo = FindControl("drpTipo", "select").value;
            var nomeCli = FindControl("txtNome", "input") != null ? FindControl("txtNome", "input").value : "";
            var situacao = FindControl("drpSituacao", "select").itens();
            var idRota = FindControl("drpRota", "select").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ListaSugestaoCliente&idSug=" + idSug + "&idCli=" + idCli + "&nomeCli=" + nomeCli +
                "&desc=" + desc + "&idFunc=" + idFunc + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&tipo=" + tipo + "&situacao=" + situacao +
                "&idRota=" + idRota + "&idPedido=" + idPedido + "&idOrcamento=" + idOrcamento +"&exportarExcel=" + exportarExcel);
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:Label ID="lblInfo" runat="server" Text="Cliente: "></asp:Label>
                <asp:Label ID="lblCliente" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HiddenField ID="hdfIdCli" runat="server" />
                <asp:HiddenField ID="hdfIdPedido" runat="server" />
                <asp:HiddenField ID="hdfIdOrcamento" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label3" runat="server" Text="Cód. Sugestão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodSug" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" Style="width: 16px" OnClick="imgPesq_click" />
                        </td>
                        <td>
                            <asp:Label ID="lblDescricao" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao" runat="server" Width="272px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                Style="width: 16px" OnClick="imgPesq_click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionario" runat="server" DataSourceID="odsFuncionario"
                                DataTextField="Name" DataValueField="Id" AppendDataBoundItems="True">
                                <asp:ListItem Value="">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="lblNumCli" runat="server" Text="Cód. Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lblNome" runat="server" Text="Nome/Apelido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label22" runat="server" ForeColor="#0066FF" Text="Período Cad."></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataCadIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                            <%--<asp:TextBox ID="txtDataCadIni" runat="server" onkeypress="return false;" Width="70px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataCadIni" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataCadIni', this)" ToolTip="Alterar" />--%>
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataCadFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                            <%--<asp:TextBox ID="txtDataCadFim" runat="server" onkeypress="return false;" Width="70px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataCadFim" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataCadFim', this)" ToolTip="Alterar" />--%>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_click" />
                        </td>
                        <td>
                            <asp:Label ID="Label23" runat="server" Text="Tipo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipo" runat="server">
                                <asp:ListItem Value="">Todos</asp:ListItem>
                                <asp:ListItem Value="Reclamacao">Reclamação</asp:ListItem>
                                <asp:ListItem Value="Sugestao">Sugestão</asp:ListItem>
                                <asp:ListItem Value="Negociacao">Negociação</asp:ListItem>
                                <asp:ListItem Value="Perfil">Perfil</asp:ListItem>
                                <asp:ListItem Value="Cobranca">Cobrança</asp:ListItem>
                                <asp:ListItem Value="Outro">Outro</asp:ListItem>
                            </asp:DropDownList>
                        </td>                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" Style="width: 16px" OnClick="imgPesq_click" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="11">
                            <table>
                                <tr>
                                    <td align="right">
                                        <asp:Label ID="lblPedido" runat="server" Text="Cód. Pedido" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPedido" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="btnPedido" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClick="imgPesq_click" />
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="lblOrcamento" runat="server" Text="Cód. Orçamento" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtOrcamento" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imbOrcamento" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" OnClick="imgPesq_click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label2" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <sync:CheckBoxListDropDown ID="drpSituacao" runat="server" CheckAll="False">
                                            <asp:ListItem Value="1" Selected="True">Ativas</asp:ListItem>
                                            <asp:ListItem Value="2">Canceladas</asp:ListItem>
                                        </sync:CheckBoxListDropDown>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            ToolTip="Pesquisar" Style="width: 16px" OnClick="imgPesq_click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label1" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpRota" runat="server" DataSourceID="odsRota"
                                            DataTextField="Name" DataValueField="Id" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text=""></asp:ListItem>                                            
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            ToolTip="Pesquisar" Style="width: 16px" OnClick="imgPesq_click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir Sugestão</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdSugestao" runat="server" SkinID="defaultGridView"
                    DataSourceID="odsSugestao"
                    DataKeyNames="IdSugestao" EmptyDataText="Nenhuma sugestão encontrada"
                    OnRowCommand="grdSugestao_RowCommand" OnRowDataBound="grdSugestao_RowDataBound">
                    <Columns>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# !(bool)Eval("Cancelada") && PodeApagar() %>'>
                                    <asp:LinkButton ID="lnkExcluir" runat="server" CausesValidation="False" CommandName="Cancelar"
                                        CommandArgument='<%# Eval("IdSugestao") %>' OnClientClick="return confirm(&quot;Tem certeza que deseja cancelar esta sugestão/reclamação?&quot;);" Text="">
                                        <img src="../Images/ExcluirGrid.gif" style="border:none" title="Cancelar" alt="Cancelar" />
                                    </asp:LinkButton>
                                    <asp:HiddenField ID="hdfIdFunc" runat="server" Value='<%# Eval("IdFunc") %>' />
                                </asp:PlaceHolder>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:PlaceHolder ID="pchAnexos" runat="server"><a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=<%# Eval("IdSugestao") %>&tipo=sugestao&#039;); return false;'>
                                    <img border="0px" src="../Images/Clipe.gif"></img></a></asp:PlaceHolder>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdSugestao" HeaderText="Cód." SortExpression="IdSugestao" />
                        <asp:TemplateField HeaderText="Cliente" SortExpression="IdCliente">
                            <ItemTemplate>
                                <%# Eval("IdCliente") + " - " + Eval("Cliente") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="DescricaoRota" HeaderText="Rota" SortExpression="DescricaoRota" />
                        <asp:BoundField DataField="DataCad" HeaderText="Data" SortExpression="DataCad" />
                        <asp:BoundField DataField="Funcionario" HeaderText="Funcionário" SortExpression="DescrUsuCad" />
                        <asp:TemplateField HeaderText="Tipo" SortExpression="TipoSugestao">
                            <ItemTemplate>
                                <%# Colosoft.Translator.Translate(Eval("TipoSugestao")).Format() %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Descricao" HeaderText="Descricao" SortExpression="Descricao" />
                        <asp:TemplateField HeaderText="Situação">
                            <ItemTemplate>
                                <%# ((bool)Eval("Cancelada")) ? "Cancelada" : "Ativa" %>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;<asp:Button ID="btnVoltar" runat="server" OnClick="btnVoltar_Click" Text="Voltar" />
                &nbsp;<br />
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);"><img alt="" border="0" src="../Images/printer.png" />Imprimir</asp:LinkButton>&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;">
                <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSugestao" runat="server" 
                    MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarSugestoes" SortParameterName="sortExpression"
                    TypeName="Glass.Global.Negocios.ISugestaoFluxo"
                    EnablePaging="True">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodSug" Name="idSugestao" PropertyName="Text" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text" />
                        <asp:ControlParameter ControlID="drpFuncionario" DefaultValue="" Name="idFunc" PropertyName="SelectedValue" />
                        <asp:Parameter Name="nomeFuncionario" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text" />
                        <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataInicio" PropertyName="DataString"
                            Type="DateTime" />
                        <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataFim" PropertyName="DataString"
                            Type="DateTime" />
                        <asp:ControlParameter ControlID="drpTipo" DefaultValue="" Name="tipo" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="txtDescricao" DefaultValue="" Name="descricao" PropertyName="Text" />
                        <asp:ControlParameter ControlID="drpSituacao" DefaultValue="" Name="situacoes" PropertyName="SelectedValues" />
                        <asp:ControlParameter ControlID="drpRota" DefaultValue="" Name="idRota" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="txtPedido" Name="idPedido" PropertyName="Text" />
                        <asp:ControlParameter ControlID="txtOrcamento" Name="idOrcamento" PropertyName="Text" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" 
                    SelectMethod="ObterFuncionariosSugestao"
                    TypeName="Glass.Global.Negocios.IFuncionarioFluxo">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsRota" runat="server" 
                    SelectMethod="ObtemRotas"
                    TypeName="Glass.Global.Negocios.IRotaFluxo">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
