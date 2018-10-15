<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CadValidacaoPecaModelo.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.Projeto.CadValidacaoPecaModelo" Title="Validações da Peça"
    MasterPageFile="~/Layout.master" %>

<%@ Register Src="../../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <script type="text/javascript">
        function setPrimeiraExpressao(expressao) {
            FindControl("txtPrimeiraExpressaoValidacao", "input").value = expressao;
        }

        function setSegundaExpressao(expressao) {
            FindControl("txtSegundaExpressaoValidacao", "input").value = expressao;
        }

        // Troca os sinais de + das expressões de cálculo para que ao editar a mesma o + não suma
        function trocaSinalMais(descricao) {
            while (descricao.indexOf("+") > 0)
                descricao = descricao.replace("+", "@");

            return descricao;
        }

        // Validações para salvar a validação da peça.
        function salvarValidacaoPecaModelo() {
            var primeiraExpressaoValidacao = FindControl("txtPrimeiraExpressaoValidacao", "input");
            var segundaExpressaoValidacao = FindControl("txtSegundaExpressaoValidacao", "input");
            var tipoComparador = FindControl("drpTipoComparador", "select");
            var mensagem = FindControl("txtMensagem", "input");
            var tipoValidacao = FindControl("drpTipoValidacao", "select");

            if (primeiraExpressaoValidacao == null || primeiraExpressaoValidacao.value == "") {
                alert("Informe a primeira expressão da validação.");
                return false;
            }

            if (segundaExpressaoValidacao == null || segundaExpressaoValidacao.value == "") {
                alert("Informe a segunda expressão da validação.");
                return false;
            }

            if (tipoComparador == null || tipoComparador.value == "") {
                alert("Informe o tipo de comparador da expressão de validação.");
                return false;
            }

            if (mensagem == null || mensagem.value == "") {
                alert("Informe a mensagem da validação.");
                return false;
            }

            if (tipoValidacao == null || tipoValidacao.value == "") {
                alert("Informe o tipo da validação.");
                return false;
            }

            return true;
        }
    </script>
    <table>
        <tr>
            <td align="center">                
                <asp:GridView GridLines="None" ID="grdValidacaoPecaModelo" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsValidacaoPecaModelo" ShowFooter="True" DataKeyNames="IdValidacaoPecaModelo"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" OnRowCommand="grdValidacaoPecaModelo_RowCommand"
                    OnPreRender="grdValidacaoPecaModelo_PreRender" >
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../../Images/Edit.gif" /></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="!confirm('Tem certeza que deseja excluir esta validação?') return;" CommandName="Delete" />
                                <asp:HiddenField ID="hdfIdPecaProjMod" runat="server" Value='<%# Bind("IdPecaProjMod") %>' />
                                <asp:HiddenField ID="hdfIdValidacaoPecaModelo" runat="server" Value='<%# Bind("IdValidacaoPecaModelo") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="return salvarValidacaoPecaModelo();" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" CausesValidation="False" />
                                <asp:HiddenField ID="hdfIdValidacaoPecaModelo" runat="server" Value='<%# Bind("IdValidacaoPecaModelo") %>' />
                                <asp:HiddenField ID="hdfIdPecaProjMod" runat="server" Value='<%# Bind("IdPecaProjMod") %>' />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Primeira exp. validação" SortExpression="PrimeiraExpressaoValidacao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtPrimeiraExpressaoValidacao" runat="server" MaxLength="300" onpaste="return false;" onkeydown="return false;" onkeyup="return false;"
                                    Text='<%# Bind("PrimeiraExpressaoValidacao") %>' Width="200px">
                                </asp:TextBox>
                                <a href="#" onclick='txtPrimeiraExpressaoValidacaoSel=FindControl(&#039;txtPrimeiraExpressaoValidacao&#039;, &#039;input&#039;); openWindow(500, 700, &#039;../../Utils/SelExpressao.aspx?tipo=validacao&amp;primeiraExpressao=true&amp;idProjetoModelo=<%= Request["idProjetoModelo"] %>&amp;expr=&#039; + trocaSinalMais(FindControl(&#039;txtPrimeiraExpressaoValidacao&#039;, &#039;input&#039;).value));'>
                                    <img border="0" src="../../Images/Pesquisar.gif" /></a>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtPrimeiraExpressaoValidacao" runat="server" onpaste="return false;" onkeydown="return false;" onkeyup="return false;" MaxLength="300"
                                    Width="200px" Text='<%# Bind("PrimeiraExpressaoValidacao") %>'>
                                </asp:TextBox>
                                <a href="#" onclick='txtPrimeiraExpressaoValidacaoSel=FindControl(&#039;txtPrimeiraExpressaoValidacao&#039;, &#039;input&#039;); openWindow(500, 700, &#039;../../Utils/SelExpressao.aspx?tipo=validacao&amp;primeiraExpressao=true&amp;idProjetoModelo=<%= Request["idProjetoModelo"] %>&amp;expr=&#039; + trocaSinalMais(FindControl(&#039;txtPrimeiraExpressaoValidacao&#039;, &#039;input&#039;).value));'>
                                    <img border="0" src="../../Images/Pesquisar.gif" /></a>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblPrimeiraExpressaoValidacao" runat="server" Text='<%# Bind("PrimeiraExpressaoValidacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Comparador" SortExpression="TipoComparador">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoComparador" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsTipoComparador" DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoComparador") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipoComparador" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsTipoComparador" DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoComparador") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblTipoComparador" runat="server" Text='<%# Colosoft.Translator.Translate((Glass.Data.Model.ValidacaoPecaModelo.TipoComparadorExpressaoValidacao)Eval("TipoComparador")).Format() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Segunda exp. validação" SortExpression="SegundaExpressaoValidacao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtSegundaExpressaoValidacao" runat="server" MaxLength="300" onpaste="return false;" onkeydown="return false;" onkeyup="return false;"
                                    Text='<%# Bind("SegundaExpressaoValidacao") %>' Width="200px">
                                </asp:TextBox>
                                <a href="#" onclick='txtSegundaExpressaoValidacaoSel=FindControl(&#039;txtSegundaExpressaoValidacao&#039;, &#039;input&#039;); openWindow(500, 700, &#039;../../Utils/SelExpressao.aspx?tipo=validacao&amp;segundaExpressao=true&amp;idProjetoModelo=<%= Request["idProjetoModelo"] %>&amp;expr=&#039; + trocaSinalMais(FindControl(&#039;txtSegundaExpressaoValidacao&#039;, &#039;input&#039;).value));'>
                                    <img border="0" src="../../Images/Pesquisar.gif" /></a>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtSegundaExpressaoValidacao" runat="server" onpaste="return false;" onkeydown="return false;" onkeyup="return false;" MaxLength="300"
                                    Width="200px" Text='<%# Bind("SegundaExpressaoValidacao") %>'>
                                </asp:TextBox>
                                <a href="#" onclick='txtSegundaExpressaoValidacaoSel=FindControl(&#039;txtSegundaExpressaoValidacao&#039;, &#039;input&#039;); openWindow(500, 700, &#039;../../Utils/SelExpressao.aspx?tipo=validacao&amp;segundaExpressao=true&amp;idProjetoModelo=<%= Request["idProjetoModelo"] %>&amp;expr=&#039; + trocaSinalMais(FindControl(&#039;txtSegundaExpressaoValidacao&#039;, &#039;input&#039;).value));'>
                                    <img border="0" src="../../Images/Pesquisar.gif" /></a>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblSegundaExpressaoValidacao" runat="server" Text='<%# Bind("SegundaExpressaoValidacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Mensagem" SortExpression="Mensagem">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtMensagem" runat="server" Text='<%# Bind("Mensagem") %>'
                                    MaxLength="300" Width="300px">
                                </asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtMensagem" runat="server" MaxLength="300" Text='<%# Bind("Mensagem") %>'
                                    Width="300px">
                                </asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblMensagem" runat="server" Text='<%# Eval("Mensagem") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="TipoValidacao">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoValidacao" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsTipoValidacao" DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoValidacao") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipoValidacao" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsTipoValidacao" DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoValidacao") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblTipoValidacao" runat="server" Text='<%# Colosoft.Translator.Translate((Glass.Data.Model.ValidacaoPecaModelo.TipoValidacaoPecaModelo)Eval("TipoValidacao")).Format() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInsValidacaoPecaModelo" runat="server" OnClientClick="return salvarValidacaoPecaModelo();"
                                    OnClick="lnkInsValidacaoPecaModelo_Click">
                                    <img border="0" src="../../Images/insert.gif" /></asp:LinkButton>
                            </FooterTemplate>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" IdRegistro='<%# Glass.Conversoes.StrParaUint(Eval("IdValidacaoPecaModelo").ToString()) %>'
                                    Tabela="ValidacaoPecaModelo" />
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
                <asp:Button ID="btnFechar" runat="server" Text="Fechar" OnClientClick="closeWindow();" />
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsValidacaoPecaModelo" runat="server"
                    DataObjectTypeName="Glass.Data.Model.ValidacaoPecaModelo" DeleteMethod="Delete" SelectMethod="ObtemValidacoes"
                    TypeName="Glass.Data.DAL.ValidacaoPecaModeloDAO" UpdateMethod="Update">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfIdPecaProjMod" Name="idPecaProjMod" PropertyName="Value"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoComparador" runat="server" SelectMethod="GetTipoComparadorValidacaoPecaProjetoModelo"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoValidacao" runat="server" SelectMethod="GetTipoValidacaoPecaProjetoModelo"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdfIdPecaProjMod" runat="server" />
    <asp:HiddenField ID="hdfIdProjetoModelo" runat="server" />
</asp:Content>
