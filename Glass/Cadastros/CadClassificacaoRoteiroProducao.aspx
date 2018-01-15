<%@ Page Title="Cadastro de Classificação - Roteiro da Produção" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadClassificacaoRoteiroProducao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadClassificacaoRoteiroProducao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript">

        function onSave() {
            if (FindControl("txtDescricao", "input").value == "") {
                alert("Informe a descrição da Classificação.");
                return false;
            }

            return true;
        }

        function setRoteiro(idRoteiro) {

            var ret = CadClassificacaoRoteiroProducao.AssociaRoteiro(idRoteiro, FindControl("hdfIdClassificacao", "input").value);

            if (ret.error != null) {
                alert(ret.error.description);
                return false;
            }

            redirectUrl(window.location.href + "&ref" + Math.random() + "=1");
        }

        function setSubgrupo(idSubgrupo) {

            var ret = CadClassificacaoRoteiroProducao.AssociaSubgrupo(idSubgrupo, FindControl("hdfIdClassificacao", "input").value);

            if (ret.error != null) {
                alert(ret.error.description);
                return false;
            }

            redirectUrl(window.location.href + "&ref" + Math.random() + "=1");
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvClassificacao" runat="server" CellPadding="4" ForeColor="#333333"
                    GridLines="None" AutoGenerateRows="False" DataSourceID="odsClassificacao">
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <CommandRowStyle BackColor="#D1DDF1" Font-Bold="True" />
                    <RowStyle BackColor="White" />
                    <FieldHeaderStyle BackColor="#DEE8F5" Font-Bold="True" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <Fields>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Descricao") %>' Width="250px"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="100" Text='<%# Bind("Descricao") %>'
                                    Width="250px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="100" Text='<%# Bind("Descricao") %>'
                                    Width="250px"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Capacidade Diária Padrão" SortExpression="CapacidadeDiaria">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("CapacidadeDiaria") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"
                                    Text='<%# Bind("CapacidadeDiaria") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"
                                    Text='<%# Bind("CapacidadeDiaria") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Meta Diária Padrão" SortExpression="MetaDiaria">
                            <ItemTemplate>
                                <asp:Label ID="lblMeta" runat="server" Text='<%# Bind("MetaDiaria") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtMeta" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"
                                    Text='<%# Bind("MetaDiaria") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtMeta" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"
                                    Text='<%# Bind("MetaDiaria") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:Button ID="btnEditar" runat="server" CommandName="Edit" Text="Editar" />
                                <asp:Button ID="btnVoltar" runat="server" OnClick="btnVoltar_Click" Text="Voltar" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" OnClientClick="return onSave();"
                                    Text="Atualizar" />
                                <asp:Button ID="btnCancelar" runat="server" CommandName="Cancel" OnClick="btnVoltar_Click"
                                    Text="Cancelar" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" OnClientClick="return onSave();"
                                    Text="Inserir" />
                                <asp:Button ID="btnVoltar" runat="server" OnClick="btnVoltar_Click" Text="Voltar" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" Wrap="False" />
                        </asp:TemplateField>
                    </Fields>
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <InsertRowStyle BackColor="White" HorizontalAlign="Left" />
                    <EditRowStyle BackColor="White" HorizontalAlign="Left" />
                    <AlternatingRowStyle BackColor="White" />
                </asp:DetailsView>
                <asp:HiddenField ID="hdfIdClassificacao" runat="server" Value='<%# Eval("IdClassificacaoRoteiroProducao") %>' />
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsClassificacao" runat="server"
                    EnablePaging="false"
                    SelectMethod="ObtemClassificacao"
                    TypeName="Glass.PCP.Negocios.IClassificacaoRoteiroProducaoFluxo"
                    DataObjectTypeName="Glass.PCP.Negocios.Entidades.ClassificacaoRoteiroProducao"
                    InsertMethod="SalvarClassificacao"
                    UpdateMethod="SalvarClassificacao" UpdateStrategy="GetAndUpdate"
                    OnInserting="odsClassificacao_Inserting" OnInserted="odsClassificacao_Inserted">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="IdClassificacaoRoteiroProducao" QueryStringField="idClassificacao" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td class="subtitle1" align="center">
                <asp:Label ID="lblSubtitle" runat="server" Text="Roteiros Associados"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkAssociarRoteiro" runat="server"
                    OnClientClick="openWindow(600, 800, '../Utils/SelRoteiroProducao.aspx'); return false;">Associar Roteiro</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdRoteiroProducao" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle"
                    DataKeyNames="Codigo"
                    DataSourceID="odsRoteiroProducao"
                    EmptyDataText="Não há roteiros associados." GridLines="None">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete"
                                    ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja desassociar esse roteiro de produção?&quot;)) return false;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CodigoInternoProcesso" HeaderText="Processo"
                            SortExpression="CodigoInternoProcesso" />
                        <asp:BoundField DataField="DescricaoSetores" HeaderText="Setores"
                            SortExpression="DescricaoSetores" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource ID="odsRoteiroProducao" runat="server"
                    Culture="pt-BR"
                    DataObjectTypeName="WebGlass.Business.RoteiroProducao.Entidade.RoteiroProducao"
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="ObtemListaCount" SelectMethod="ObtemLista"
                    StartRowIndexParameterName="startRow"
                    TypeName="WebGlass.Business.RoteiroProducao.Fluxo.RoteiroProducao"
                    SortParameterName="sortExpression"
                    DeleteMethod="DesassociarRoteiroClassificacao">
                    <SelectParameters>
                        <asp:ControlParameter Name="idClassificacaoRoteiroProducao" ControlID="hdfIdClassificacao" PropertyName="Value" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="LinkButton1" runat="server"
                    OnClientClick="openWindow(600, 800, '../Utils/SelSubgrupoRoteiroProducao.aspx'); return false;">Associar Subgrupo</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdSubgrupo" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle"
                    DataKeyNames="ChaveComposta"
                    DataSourceID="odsSubgrupo"
                    EmptyDataText="Não há Subgrupos associados." GridLines="None">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja desassociar esse subgrupo?&quot;)) return false;" />
                            </ItemTemplate>
                        </asp:TemplateField>                       
                        <asp:BoundField DataField="Descricao" HeaderText="Subgrupos" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource ID="odsSubgrupo" runat="server" Culture="pt-BR"             
                    SelectMethod="ObterSubgruposAssociadosClassificacaoRoteiroProducao" 
                    SelectByKeysMethod="ObterSubgrupoAssociadoClassificacao" 
                    DeleteMethod="DesassociarSubgrupo"  DeleteStrategy="GetAndDelete" 
                    TypeName="Glass.Data.Dal.ClassificacaoSubgrupoDAO">
                    <SelectParameters>
                        <asp:ControlParameter Name="idClassificacaoRoteiroProducao" ControlID="hdfIdClassificacao" PropertyName="Value" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
