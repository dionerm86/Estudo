<%@ Page Title="Arquivos de Quitação Parcelas de Cartão" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="LstArquivoQuitacaoParcelaCartao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstArquivoQuitacaoParcelaCartao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    
    <script type="text/javascript">

        function openMotivoCanc(idArquivoQuitacaoParcelaCartao) {
            openWindow(400, 700, "../Utils/SetMotivoCancPagto.aspx?idArquivoQuitacaoParcelaCartao=" + idArquivoQuitacaoParcelaCartao);
            return false;
        }

    </script>
    <table>
        <tr>
            <td>

            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdArquivoQuitacaoParcelaCartao" runat="server" GridLines="None" AllowPaging="true" AllowSorting="true" AutoGenerateColumns="false"
                    DataSourceID="odsArquivoQuitacaoParcelaCartao" DataKeyNames="IdArquivoQuitacaoParcelaCartao" CssClass="gridStyle" EmptyDataText="Nenhum arquivo quitação parcela cartão encontrado.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>                               
                                <asp:ImageButton ID="imbCancelar" runat="server" ToolTip="Cancelar" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick='<%# "return openMotivoCanc(" + Eval("IdArquivoQuitacaoParcelaCartao") + ");" %>'
                                    Visible='<%# Eval("PodeCancelar") %>' />
                                 <asp:ImageButton ID="imbDownLoad" runat="server" ToolTip="Download do Arquivo" ImageUrl="~/Images/disk.gif" 
                                    onclientclick='<%# "redirectUrl(\"../Handlers/Download.ashx?filePath=~/Upload/ArquivoQuitacaoParcelaCartao/" + Eval("NomeArquivo") + "&fileName=" + Eval("NomeArquivo") + "\"); return false" %>' />
                                <asp:ImageButton ID="imbDetalhes" runat="server" ToolTip="Detalhes" ImageUrl="~/Images/Nota.gif"
                                    OnClientClick='<%# "redirectUrl(\"../Cadastros/CadArquivoQuitacaoParcelaCartao.aspx?IdArquivoQuitacaoParcelaCartao=" + Eval("IdArquivoQuitacaoParcelaCartao") + "\"); return false" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdArquivoQuitacaoParcelaCartao" HeaderText="Cód. Arquivo" SortExpression="IdArquivoQuitacaoParcelaCartao" />
                        <asp:BoundField DataField="DataCad" HeaderText="Data de Importação" SortExpression="DataCad" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="Situacao" HeaderText="Situação" />
                        <asp:BoundField DataField="NomeFuncionarioCadastro" HeaderText="Func. Cad" SortExpression="NomeFuncionarioCadastro"  DataFormatString="{0:C}" />   
                        <asp:TemplateField>
                            <ItemTemplate>

                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                    <EditRowStyle CssClass="edit" />
                    <PagerStyle CssClass="pgr" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource ID="odsArquivoQuitacaoParcelaCartao" runat="server" Culture="pt-BR" EnablePaging="true"
                    TypeName="Glass.Financeiro.Negocios.IQuitacaoParcelaCartaoFluxo"
                    SelectMethod="PesquisarArquivoQuitacaoParcelaCartao">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

</asp:Content>
