<%@ Page Title="Parcelas" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstParcelas.aspx.cs" Inherits="Glass.UI.Web.Listas.LstParcelas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Parcelas&exportarExcel=" + exportarExcel);
            return false;
        }

    </script>

    <section>
        <div>
            <asp:LinkButton ID="lkbInserir" runat="server" PostBackUrl="~/Cadastros/CadParcelas.aspx">Inserir parcela</asp:LinkButton></div>
        <div>
            <asp:GridView ID="grdParcelas" runat="server" SkinID="defaultGridView"
                 DataSourceID="odsParcelas" DataKeyNames="IdParcela" OnLoad="grdParcelas_Load">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton ID="imgEditar" runat="server" ImageUrl="~/Images/EditarGrid.gif"
                                PostBackUrl='<%# "~/Cadastros/CadParcelas.aspx?idParcela=" + Eval("IdParcela") %>' />
                            <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                OnClientClick="if (!confirm(&quot;Tem certeza que deseja excluir essa parcela?&quot;)) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                    <asp:TemplateField HeaderText="Núm. Parcelas" SortExpression="NumParcelas">
                        <ItemTemplate>
                            <%# ObtemDescricaoNumeroParcelas((int)Eval("NumParcelas")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Dias" HeaderText="Dias" SortExpression="Dias" />
                    <asp:BoundField DataField="Desconto" HeaderText="Desconto" SortExpression="Desconto" />
                    <asp:TemplateField HeaderText="Exibir marcado como padrão?" SortExpression="ParcelaPadrao">
                        <ItemTemplate>
                            <%# (bool)Eval("ParcelaPadrao") ? "Sim" : "Não" %>
                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <%# Colosoft.Translator.Translate(Eval("Situacao")).Format() %>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;">
                    <img border="0" src="../Images/Excel.gif" alt="Exportar para o Excel" /> Exportar para o Excel</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsParcelas" runat="server" 
                EnablePaging="True" MaximumRowsParameterName="pageSize"
                SelectMethod="PesquisarParcelas" SortParameterName="sortExpression"
                SelectByKeysMethod="ObtemParcela"
                TypeName="Glass.Financeiro.Negocios.IParcelasFluxo" 
                DeleteMethod="ApagarParcela" DeleteStrategy="GetAndDelete"
                DataObjectTypeName="Glass.Financeiro.Negocios.Entidades.Parcelas">
            </colo:VirtualObjectDataSource>
        </div>
    </section>
    
</asp:Content>
