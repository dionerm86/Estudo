<%@ Page Title="Vincular Fornecedores" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadFornecedorVinculos.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadFornecedorVinculos" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function getFornec(idFornec) {
            if (idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornec(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }    </script>

    <table style="width: 100%" align="center">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Cód. Fornec." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodFornec" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Nome" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server" onkeypress="if (isEnter(event)) return false;"
                                Width="170px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="CNPJ" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCnpj" runat="server" onkeypress="if (isEnter(event)) return false;"
                                Width="170px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Endereço" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtEndereco" runat="server" onkeypress="if (isEnter(event)) return false;"
                                Width="170px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AppendDataBoundItems="True" DataSourceID="odsSituacaoFornecedor"
                                DataTextField="Translation" DataValueField="Key">
                                <asp:ListItem Value="">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdFornecedor" runat="server" DataKeyNames="IdFornec" DataSourceID="odsFornecedor"
                    EmptyDataText="Não há Fornecedores Cadastrados" SkinID="defaultGridView">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick='openWindow(500, 750, &#039;../Utils/SetFornecedorVinculos.aspx?idFornec=<%# Eval("IdFornec") %>&#039;); return false;'>
                                    <img src="../Images/vinculos.gif" border="0" title="Vínculos" /></a>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdFornec" HeaderText="Cód." SortExpression="IdFornec" />
                        <asp:BoundField DataField="NomeFantasia" HeaderText="Nome Fantasia" SortExpression="NomeFantasia" />
                        <asp:BoundField DataField="CpfCnpj" HeaderText="CPF/CNPJ" SortExpression="CpfCnpj" />
                        <asp:BoundField DataField="RgInscEst" HeaderText="RG/Insc. Est." SortExpression="RgInscEst" />
                        <asp:BoundField DataField="TelCont" HeaderText="Tel. Cont." SortExpression="TelCont" />
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <%# Colosoft.Translator.Translate(Eval("Situacao")).Format() %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" IdRegistro='<%# (uint)(int)Eval("IdFornec") %>'
                                    Tabela="Fornecedor" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFornecedor" runat="server"
        DataObjectTypeName="Glass.Global.Negocios.Entidades.Fornecedor"
        DeleteMethod="ApagarFornecedor"
        DeleteStrategy="GetAndDelete"
        SelectMethod="PesquisarFornecedores"
        SelectByKeysMethod="ObtemFornecedor"
        TypeName="Glass.Global.Negocios.IFornecedorFluxo"
        EnablePaging="True" MaximumRowsParameterName="pageSize"
        SortParameterName="sortExpression">
        <SelectParameters>
            <asp:ControlParameter ControlID="txtCodFornec" Name="idFornec" PropertyName="Text" />
            <asp:ControlParameter ControlID="txtNome" Name="nomeFornec" PropertyName="Text" />
            <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue" />
            <asp:ControlParameter ControlID="txtCnpj" Name="cnpj" PropertyName="Text" />
            <asp:Parameter Name="comCredito" />
            <asp:Parameter Name="idPlanoConta" />
            <asp:Parameter Name="idTipoPagto" />
            <asp:ControlParameter ControlID="txtEndereco" Name="endereco" PropertyName="Text" />
            <asp:Parameter Name="vendedor" />
            <asp:Parameter Name="tipoPessoa" DefaultValue="" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacaoFornecedor" runat="server"
        SelectMethod="GetTranslatesFromTypeName"
        TypeName="Colosoft.Translator">
        <SelectParameters>
            <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.SituacaoFornecedor, Glass.Data" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>
