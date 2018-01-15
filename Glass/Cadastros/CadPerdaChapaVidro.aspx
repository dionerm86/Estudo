<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadPerdaChapaVidro.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadPerdaChapaVidro" Title="Marcar Perda de Chapa de Vidro" %>

<%@ Register Src="../Controls/ctrlTipoPerda.ascx" TagName="ctrlTipoPerda" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlRetalhoProducao.ascx" TagName="ctrlRetalhoProducao"
    TagPrefix="uc2" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function addEtiqueta(numEtiqueta){
        
            var numEtiqueta = FindControl("txtEtiqueta", "input");
            if(numEtiqueta == null || numEtiqueta.value == ""){
                alert("Informe o número da etiqueta.");
                return false;
            }
            
            var retorno = CadPerdaChapa.ValidaEtiqueta(numEtiqueta.value).value.split(';');
            if(retorno[0]!="ok"){
                alert(retorno[1]);
                return false;
            }
            
            return true;
        }
        
        function confirmar()
        {                
            // Verifica se a opção de retornar ao estoque foi marcada
            if (FindControl("drpRetornarEstoque", "select") != null && 
                FindControl("drpRetornarEstoque", "select").value == "")
            {
                alert("Informe se a peça deverá retornar ao estoque.");
                return false;
            }
        
            return confirm("Deseja marcar essa peça como perda?");
        }
        
       function imprimirRetalhos(idProd) {
            openWindow(500, 700, '../Relatorios/RelEtiquetas.aspx?callbackPronto=impressaoPronta()&apenasPlano=false&idProd=' + idProd);
        }
        
        function impressaoPronta()
        {
           redirectUrl(window.location.href);
        }
        
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            Número da Etiqueta da Chapa:
                        </td>
                        <td>
                            <asp:TextBox ID="txtEtiqueta" runat="server" Width="70px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valNumEtiqueta" runat="server" ControlToValidate="txtEtiqueta"
                                ErrorMessage="*" ValidationGroup="c"></asp:RequiredFieldValidator>
                            <asp:HiddenField ID="hdfIdImpressao" runat="server" Value="" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imbPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imbPesq_Click"
                                ValidationGroup="c" Width="16px" OnClientClick='if(!addEtiqueta())return false;' />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>
    <table id="tbSaida" runat="server" visible="false">
        <tr runat="server" id="tipoPerda" align="center">
            <td>
                <table>
                    <tr>
                        <td align="left">
                            Tipo de perda
                        </td>
                        <td align="left">
                            <uc1:ctrlTipoPerda ID="ctrlTipoPerda1" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label4" runat="server" Text="Retornar peça ao estoque?" Visible="False"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpRetornarEstoque" runat="server" Visible="False">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Value="1">Sim</asp:ListItem>
                                <asp:ListItem Value="2">Não</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            Motivo perda
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtObsPerda" runat="server" Rows="4" Columns="50" TextMode="MultiLine"></asp:TextBox>
                            <asp:CustomValidator ID="ctvObsPerda" runat="server" ClientValidationFunction="validarObsPerda"
                                ControlToValidate="txtObsPerda" Display="None" ErrorMessage="O motivo da perda não pode ser vazio."
                                ValidateEmptyText="True"></asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2">
                            <uc2:ctrlRetalhoProducao ID="ctrlRetalhoProducao1" runat="server" />
                        </td>
                    </tr>
                </table>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoPerda" runat="server" SelectMethod="GetBySetor"
                    TypeName="Glass.Data.DAL.TipoPerdaDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfIdSetorCorte" Name="idSetor" PropertyName="Value"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfIdSetorCorte" runat="server" />
                <asp:ValidationSummary ID="vsuSumario" runat="server" ShowMessageBox="True" ShowSummary="False"
                    DisplayMode="List" />
            </td>
        </tr>
        <tr>
            <td>
                <table>
                    <tr align="center">
                        <td>
                            <asp:GridView GridLines="None" ID="grdProduto" runat="server" AllowPaging="True"
                                AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdProdImpressao"
                                DataSourceID="odsProduto" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EditRowStyle-CssClass="edit" PageSize="15" OnRowCommand="grdProduto_RowCommand">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:ImageButton ID="imbMarcar" runat="server" ToolTip="Marcar chapa" CommandArgument='<%# Eval("NumEtiqueta") + ";" + Eval("IdProd")+ ";" + Eval("IdProdNf") %>'
                                                            CommandName="Marcar" ImageUrl="~/Images/ok.gif" OnClientClick="if (!confirmar()) return false;" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="NumeroNFe" HeaderText="NF-e" SortExpression="NumeroNFe" />
                                    <asp:BoundField DataField="NomeFornec" HeaderText="Fornecedor" SortExpression="NomeFornec" />
                                    <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                                    <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura" />
                                    <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura">
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Obs" HeaderText="Obs" SortExpression="Obs" />
                                    <asp:BoundField DataField="NumEtiqueta" HeaderText="Etiqueta" SortExpression="concat(pi.IdPedido, pi.PosicaoProd, pi.ItemEtiqueta)" />
                                </Columns>
                                <PagerStyle />
                                <EditRowStyle />
                                <AlternatingRowStyle />
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr align="center">
                        <td align="center">
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProduto" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                SelectCountMethod="GetCountImpressao" SelectMethod="GetListImpressao" SortParameterName="sortExpression"
                                StartRowIndexParameterName="startRow" 
                                TypeName="Glass.Data.DAL.ProdutoImpressaoDAO">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="hdfIdImpressao" Name="idImpressao" PropertyName="Value"
                                        Type="UInt32" />
                                    <asp:Parameter Name="planoCorte" Type="String" />
                                    <asp:Parameter Name="numeroNFe" Type="UInt32" DefaultValue="0" />
                                    <asp:Parameter Name="idPedido" Type="UInt32" DefaultValue="0" />
                                    <asp:Parameter Name="descrProduto" Type="String" DefaultValue="" />
                                    <asp:ControlParameter ControlID="txtEtiqueta" Name="etiqueta" PropertyName="Text"
                                        Type="String" />
                                    <asp:Parameter Name="altura" Type="Single" />
                                    <asp:Parameter Name="largura" Type="Int32" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
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
        <tr align="center">
            <td>
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
