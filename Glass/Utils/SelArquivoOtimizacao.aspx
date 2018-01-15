<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelArquivoOtimizacao.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SelArquivoOtimizacao" Title="Gerar Arquivo de Otimização" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function load()
        {
            if (<%= IsPostBack.ToString().ToLower() %>)
                return;
            
            var campoEtiquetasOpener = window.opener.document.getElementById("campoEtiquetas");
            var etiquetas = document.getElementById("<%= hdfEtiquetas.ClientID %>");
            etiquetas.value = campoEtiquetasOpener.value;
            atualizarPagina();
        }
        
        function showArquivoOtimizacao(botao, material)
        {
            var celula = botao;
            while (celula.nodeName.toLowerCase() != "td")
                celula = celula.parentNode;
            
            var hdfEtiquetas = celula.getElementsByTagName("input")[0];
            
            var campoEtiquetas = document.getElementById("campoEtiquetas");
            if (campoEtiquetas == null)
            {
                campoEtiquetas = document.createElement("input");
                campoEtiquetas.id = "campoEtiquetas";
                campoEtiquetas.name = "etiquetas";
                document.formPost.appendChild(campoEtiquetas);
            }

            campoEtiquetas.value = hdfEtiquetas.value;
            document.formPost.action = "../Handlers/ArquivoOtimizacao.ashx?material=" + material;
            document.formPost.submit();
        }
        
        window.onload = load;
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsProdutos" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:BoundField DataField="Material" HeaderText="Material" ReadOnly="True" SortExpression="Material" />
                        <asp:TemplateField HeaderText="Arquivo de otimização">
                            <ItemTemplate>
                                <a href="#" onclick='showArquivoOtimizacao(this, "<%# Eval("Material") %>"); return false;'>
                                    <img alt="Arquivo de Otimização" title="Arquivo de Otimização" border="0" src="../Images/blocodenotas.png" /></a>
                                <asp:HiddenField ID="hdfEtiquetas" runat="server" Value='<%# Eval("Etiquetas") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutos" runat="server" SelectMethod="GetForArquivoOtimizacao"
                    TypeName="Glass.Data.RelDAL.ArquivoOtimizacaoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfEtiquetas" Name="dadosEtiquetas" PropertyName="Value"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfEtiquetas" runat="server" />
                <asp:LinkButton ID="LinkButton1" runat="server" Style="display: none"></asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
