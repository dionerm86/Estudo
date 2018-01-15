<%@ Page Title="Arquivos de FCI" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstArquivoFci.aspx.cs" Inherits="Glass.UI.Web.Listas.LstArquivoFci" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function atualiza(msg) {
            if (msg != null && msg != "")
                alert(msg);

            window.location.href = window.location.href;
        }

        function exibirProdutos(botao, idArquivoFCI) {

            var linha = document.getElementById("arquivoFci_" + idArquivoFCI);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " produtos";
        }

        function BaixarArquivoFci(idArquivoFci, retorno) {
            window.location.href = "../Handlers/ArquivoFCI.ashx?idArquivoFci=" + idArquivoFci + "&retorno=" + retorno;
        }

    </script>

    <table>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Gerar Arquivo 
                FCI</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdArquivoFci" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataKeyNames="IdArquivoFCI"
                    DataSourceID="odsArquivoFci" GridLines="None" OnRowCommand="grdArquivoFci_RowCommand"
                    EmptyDataText="Nenhum arquivo FCI foi encontrado." 
                    Style="min-width: 1000px;">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirProdutos(this, " + Eval("IdArquivoFCI") + "); return false" %>'
                                    Width="10px" ToolTip="Exibir produtos" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="ImportarArqRetorno"
                                    ImageUrl="~/Images/Ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgDeletar" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    Visible='<%# Eval("EditarVisible") %>' OnClientClick="return confirm('Deseja realmente deletar este arquivo da FCI?');" />
                                <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif"
                                    Visible='<%# Eval("EditarVisible") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód." SortExpression="IdArquivoFCI">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("IdArquivoFCI") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data geração" SortExpression="DataCad">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("DataCad") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Funcionário geração" SortExpression="NomeUsuCad">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("NomeUsuCad") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data importação" SortExpression="DataImportacao">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("DataImportacao") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Funcionário importação" SortExpression="UsuImportacao">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("NomeUsuImportacao") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Protoco de Envio">
                            <EditItemTemplate>
                                <asp:FileUpload ID="fupArqRetorno" runat="server" />
                                <asp:HiddenField runat="server" ID="hdfIdArquivoFci" Value='<%# Bind("IdArquivoFCI") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("Protocolo") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Arq. FCI">
                            <ItemTemplate>
                                <asp:ImageButton ID="imgArquivoFci" runat="server" ImageUrl="~/Images/blocodenotas.png"
                                    ToolTip="Arquivo da FCI" OnClientClick='<%# "BaixarArquivoFci(" + Eval("IdArquivoFCI") + ", false); return false" %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Arq. Ret. FCI">
                            <ItemTemplate>
                                <asp:ImageButton ID="imgArquivoRetornoFci" runat="server" ImageUrl="~/Images/blocodenotas.png"
                                    ToolTip="Arquivo de retorno da FCI" Visible='<%# !(Boolean)Eval("EditarVisible") %>' 
                                    OnClientClick='<%# "BaixarArquivoFci(" + Eval("IdArquivoFCI") + ", true); return false" %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                </td> </tr>
                                <tr id="arquivoFci_<%# Eval("IdArquivoFCI") %>" style="display: none;" class="<%= GetAlternateClass() %>">
                                    <td colspan="11">
                                        <asp:HiddenField runat="server" ID="hdfIdArquivoFci2" Value='<%# Bind("IdArquivoFCI") %>' />
                                        <asp:GridView ID="grdProdutosArquivoFci" runat="server" AllowPaging="True" AllowSorting="True"
                                            AutoGenerateColumns="False" CssClass="gridStyle" DataKeyNames="IdArquivoFCI"
                                            DataSourceID="odsProdutosArquivoFci" GridLines="None" EmptyDataText="Nenhum produto do arquivo FCI foi encontrado."
                                            Width="100%">
                                            <Columns>
                                                <asp:BoundField HeaderText="Produto" DataField="CodInternoDescrProduto" />
                                                <asp:BoundField HeaderText="Parcela Importada" DataField="ParcelaImportada" />
                                                <asp:BoundField HeaderText="Saída Interestadual" DataField="SaidaInterestadual" />
                                                <asp:BoundField HeaderText="Conteúdo de Importacao" DataField="ConteudoImportacao" />
                                                <asp:BoundField HeaderText="Núm. Controle FCI" DataField="NumControleFciStr" />
                                            </Columns>
                                        </asp:GridView>
                                        <sync:ObjectDataSource ID="odsProdutosArquivoFci" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosArquivoFCI"
                                            EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="ObterListaCount"
                                            SelectMethod="ObterLista" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                                            TypeName="WebGlass.Business.FCI.Fluxo.ProdutosArquivoFCIFluxo" UseDAOInstance="True">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="hdfIdArquivoFci2" PropertyName="Value" Name="idArquivoFCI"
                                                    Type="UInt32" />
                                            </SelectParameters>
                                        </sync:ObjectDataSource>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <sync:ObjectDataSource ID="odsArquivoFci" runat="server" DataObjectTypeName="Glass.Data.Model.ArquivoFCI"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="ObterListaCount"
                    SelectMethod="ObterLista" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="WebGlass.Business.FCI.Fluxo.ArquivoFCIFluxo" UseDAOInstance="True"
                    DeleteMethod="Delete" OnDeleted="odsArquivoFci_Deleted">
                </sync:ObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

