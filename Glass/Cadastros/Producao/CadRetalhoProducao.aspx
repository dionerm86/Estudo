<%@ Page Title="Cadastro de Retalhos de Produção" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadRetalhoProducao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Producao.CadRetalhoProducao" %>

<%@ Register Src="../../Controls/ctrlSelProduto.ascx" TagName="ctrlSelProduto" TagPrefix="uc1" %>
<%@ Register Src="../../Controls/ctrlRetalhoProducao.ascx" TagName="ctrlRetalhoProducao"
    TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script src="../../Scripts/jquery/jquery-1.9.0.js" type="text/javascript"></script>

    <script src="../../Scripts/jquery/jquery-1.9.0.min.js" type="text/javascript"></script>

    <script type="text/javascript">

        function validar() {

            var retalhos = $("table[id$=ctrlRetalhoProducao1_tblRetalhos]")[0];

            if ($("input:hidden[id$=hdfIdProdNF]").val() == "") {
                alert("É necessário selecionar um produto.");
                return false;
            }

            for (i = 0; i < retalhos.rows.length; i++) {
                var txtAltura = FindControl('txtAltura', 'input', retalhos.rows[i]).value;
                var txtLargura = FindControl('txtLargura', 'input', retalhos.rows[i]).value;
                var txtQuantidade = FindControl('txtQuantidade', 'input', retalhos.rows[i]).value;

                if (txtAltura == "" || txtLargura == "" || txtQuantidade == "") {
                    alert("É necessário informar os dados do retalho.");
                    return false;
                }
            }

            return true;
        }

        $(document).ready(function() {

            $(".radioGrid").click(function() {
                if ($(this).is(":checked")) {
                    $("input:hidden[id$=hdfIdProdNF]").val($(this).val());
                }
            });
        });

        function imprimirRetalhos(idProd) {
            openWindow(500, 700, '../../Relatorios/RelEtiquetas.aspx?callbackPronto=impressaoPronta()&apenasPlano=false&idProd=' + idProd,
                null, false, true);
        }

        function impressaoPronta()
        {
            redirectUrl(window.location.href);
        }
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table id="nf" style="padding-top: 20px">
                    <tr>
                        <td colspan="3" align="center">
                            <table>
                                <tr>
                                    <td align="center" style="padding-left: 10px">
                                        Número NFe:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtNumeroNFE" onkeypress="return soNumeros(event, true, true);"
                                            runat="server" onkeydown="if (isEnter(event)) cOnClick('imbPesq', null);" Width="70px"
                                            TabIndex="1"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imbPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imbPesq_Click"
                                            TabIndex="2" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:GridView ID="grdProduto" GridLines="None" runat="server" AutoGenerateColumns="False"
                                CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EditRowStyle-CssClass="edit" DataKeyNames="IdProd" DataSourceID="odsProdutos"
                                EmptyDataText="Nenhum produto do grupo vidro encontrado">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <input type="radio" id="rbProduto" name="radioProduto" class="radioGrid" value='<%# string.Format("{0};{1}", Eval("IdProdNf"), Eval("IdProd")) %>' />
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="CodInterno" HeaderText="Cód." SortExpression="CodInterno" />
                                    <asp:BoundField DataField="DescrProduto" HeaderText="Descrição" SortExpression="DescrProduto" />
                                    <asp:BoundField DataField="DescrTipoProduto" HeaderText="Tipo" SortExpression="DescrTipoProduto" />
                                    <asp:BoundField DataField="Largura" HeaderText="Largura" 
                                        SortExpression="Largura" />
                                    <asp:BoundField DataField="Altura" HeaderText="Altura" 
                                        SortExpression="Altura" />
                                    <asp:BoundField DataField="Qtde" HeaderText="Qtde" SortExpression="Qtde" />
                                    <asp:BoundField DataField="TotM" HeaderText="Tot. M²" SortExpression="TotM" />
                                    <asp:BoundField DataField="Lote" HeaderText="Lote" SortExpression="Lote" />
                                </Columns>
                                <PagerStyle CssClass="pgr"></PagerStyle>
                                <EditRowStyle CssClass="edit"></EditRowStyle>
                                <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                            </asp:GridView>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutos" runat="server" SelectMethod="ObterProdutosNota"
                                TypeName="Glass.Data.DAL.ProdutosNfDAO"  OnSelecting="odsProdutos_Selecting">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtNumeroNFE" Name="numeroNfe" PropertyName="Text"
                                        Type="UInt32" />
                                    <asp:Parameter DefaultValue="true" Name="apenasEntradas" Type="Boolean" />
                                    <asp:Parameter DefaultValue="true" Name="apenasVidros" Type="Boolean" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <uc2:ctrlRetalhoProducao ID="ctrlRetalhoProducao1" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding-top: 20px">
                            <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click"
                                OnClientClick="return validar();" Width="47px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdfIdProdNF" runat="server" />
</asp:Content>
