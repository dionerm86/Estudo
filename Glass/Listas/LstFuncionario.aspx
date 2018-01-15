<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstFuncionario.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstFuncionario" Title="Funcionários" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlTipoFuncionario.ascx" TagName="ctrlTipoFuncionario"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc5" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt() {
            var idLoja = FindControl("drpLoja", "select").value;
            var nome = FindControl("txtNome", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var registrado = FindControl("chkRegistrados", "input").checked;
            var idTipoFunc = FindControl("hdfTipoFuncionario", "input").value;
            var idSetorFunc = FindControl("hdfIdSetorFuncionario", "input").value;
            var dataNascIni = FindControl("ctrlDataNascIni_txtData", "input").value;
            var dataNascFim = FindControl("ctrlDataNascFim_txtData", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ListaFuncionarios&idLoja=" + idLoja + "&nome=" + nome + "&situacao=" + situacao +
                "&registrado=" + registrado + "&idTipoFunc=" + idTipoFunc + "&idSetorFunc=" + idSetorFunc +
                "&dtNascIni=" + dataNascIni + "&dtNascFim=" + dataNascFim);

            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc5:ctrlLoja runat="server" ID="drpLoja" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Nome" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="">Todas</asp:ListItem>
                                <asp:ListItem Selected="True" Value="Ativo">Ativo</asp:ListItem>
                                <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkRegistrados" runat="server" Text="Apenas Registrados" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Tipo de Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlTipoFuncionario ID="ctrlTipoFuncionario1" runat="server" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Data de Nasc Inicial." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc4:ctrlData ID="ctrlDataNascIni" runat="server" ExibirHoras="false" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Data de Nasc Final." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc4:ctrlData ID="ctrlDataNascFim" runat="server" ExibirHoras="false" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
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
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir 
    Funcionário</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdFunc" runat="server" DataSourceID="odsFunc"
                    DataKeyNames="IdFunc" SkinID="defaultGridView" EnableViewState="false"
                    EmptyDataText="Nenhum funcionário encontrado.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" Visible='<%# PodeEditar((bool)Eval("AdminSync")) %>'
                                    ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadFuncionario.aspx?idFunc=" + Eval("IdFunc") %>'>
                                <img border="0" src="../Images/EditarGrid.gif" alt="Editar" /></asp:HyperLink>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este Funcionário?&quot;);"
                                    ToolTip="Excluir" Visible='<%# PodeApagar((bool)Eval("AdminSync")) %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                        <asp:BoundField DataField="Loja" HeaderText="Loja" SortExpression="Loja" />
                        <asp:TemplateField HeaderText="Tipo Func." SortExpression="TipoFuncionario">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("TipoFuncionario") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Cpf" HeaderText="CPF" SortExpression="Cpf" />
                        <asp:BoundField DataField="Rg" HeaderText="RG" SortExpression="Rg" />
                        <asp:BoundField DataField="TelRes" HeaderText="Tel Res" SortExpression="TelRes">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TelCel" HeaderText="Cel" SortExpression="TelCel">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc3:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" ImageUrl='<%# Glass.Global.UI.Web.Process.Funcionarios.FuncionarioRepositorioImagens.Instance.ObtemUrl((int)Eval("IdFunc")) %>' />
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="Funcionario" IdRegistro='<%# (uint)(int)Eval("IdFunc") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt();"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFunc" runat="server" 
        DataObjectTypeName="Glass.Global.Negocios.Entidades.Funcionario"
        DeleteMethod="ApagarFuncionario" DeleteStrategy="GetAndDelete"
        EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectMethod="PesquisarFuncionarios" SortParameterName="sortExpression"
        SelectByKeysMethod="ObtemFuncionario"
        TypeName="Glass.Global.Negocios.IFuncionarioFluxo">
        <SelectParameters>
            <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                Type="String" />
            <asp:ControlParameter ControlID="txtNome" Name="nomeFuncionario" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue" />
            <asp:ControlParameter ControlID="chkRegistrados" Name="apenasRegistrados" PropertyName="Checked"
                Type="Boolean" />
            <asp:ControlParameter ControlID="ctrlTipoFuncionario1" Name="idTipoFunc" PropertyName="IdTipoFunc"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctrlTipoFuncionario1" Name="idSetor" PropertyName="IdSetorFunc"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctrlDataNascIni" Name="dataNascInicio" PropertyName="DataString"
                Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataNascFim" Name="dataNascFim" PropertyName="DataString"
                Type="DateTime" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>
