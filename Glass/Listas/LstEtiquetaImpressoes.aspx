<%@ Page Title="Consulta de Impressões de Etiquetas" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstEtiquetaImpressoes.aspx.cs" Inherits="Glass.UI.Web.Listas.LstEtiquetaImpressoes" %>

<%@ Register src="../Controls/ctrlLogCancPopup.ascx" tagname="ctrlLogCancPopup" tagprefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function cancelarImpressao(idImpressao, tipoImpressao) {
            /*if (!confirm("Deseja cancelar essa impressão?"))
                return false;

            var idPedido = prompt("Informe o número do pedido que será cancelado desta impressão ou digite \"0\" para cancelar toda a impressão.");

            if (idPedido == "" || isNaN(parseInt(idPedido, 10))) {
                alert("Valor inválido.");
                return false;
            }
            
            bloquearPagina();

            var retorno = LstEtiquetaImpressoes.CancelarImpressao(idImpressao, idPedido).value;
            
            desbloquearPagina(true);

            if (retorno == null) {
                alert("Erro na requisição AJAX.");
                return false;
            }

            retorno = retorno.split('|');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                return false;
            }

            alert("Impressão cancelada.");
            cOnClick("imgPesq");*/

            openWindow(350, 600, "../Utils/SetMotivoCancEtiqueta.aspx?idImpressao=" + idImpressao + "&tipo=" + tipoImpressao);
        }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="NF-e" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumeroNFe" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Impressão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdImpressao" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Plano de Corte" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPlanoCorte" runat="server"
                                Width="80px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Lote" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtLote" runat="server"
                                Width="80px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
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
                            <asp:Label ID="Label6" runat="server" Text="Etiqueta" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtEtiqueta" runat="server"
                                Width="80px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Tipo de Impressão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoImpressao" runat="server" 
                                AppendDataBoundItems="True" DataSourceID="odsTipoImpressao" 
                                DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
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
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdImpressao" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsImpressao"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataKeyNames="IdImpressao" EmptyDataText="Nenhuma impressão encontrada."
                    OnRowDataBound="grdImpressao_RowDataBound">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# (int)Eval("Situacao") >= 1 %>'>
                                    <a href="#" onclick="openWindow(550, 750, '../Listas/LstEtiquetaProdImp.aspx?IdImpressao=<%# Eval("IdImpressao") %>'); return false;">
                                        <img src="../Images/relatorio.gif" border="0" alt="Itens Impressos" title="Itens Impressos"></a>
                                </asp:PlaceHolder>
                                <span style='<%# (int)Eval("Situacao") != 1 || (bool)Glass.Configuracoes.PCPConfig.Etiqueta.UsarPlanoCorte ? "display: none": "" %>'>
                                    <a href="#" onclick='location.href=&quot;../Handlers/ArquivoOtimizacao.ashx?idImpressao=<%# Eval("IdImpressao") %>&quot;'>
                                        <img src="../Images/blocodenotas.png" title="Arquivo de Otimização" border="0" /></a>
                                </span>
                                <asp:PlaceHolder ID="pchSituacao" runat="server" Visible='<%# ((int)Eval("Situacao") <= 1) && PermitirCancelar(Eval("IdFunc")) %>'>
                                    <a href="#" onclick='cancelarImpressao(<%# Eval("IdImpressao") %>, <%# (long)Eval("TipoImpressao") %>); return false'>
                                        <img src="../Images/ExcluirGrid.gif" border="0" title="Cancelar Impressão" /></a>
                                </asp:PlaceHolder>
                                <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Eval("Situacao") %>' />
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdImpressao" HeaderText="Impressão" SortExpression="IdImpressao" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="Data" HeaderText="Data" SortExpression="Data" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" 
                            SortExpression="DescrSituacao" />
                        <asp:BoundField DataField="DescrTipoImpressao" HeaderText="Tipo de Impressão" 
                            SortExpression="TipoImpressao" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" 
                                    IdRegistro='<%# Eval("IdImpressao") %>' 
                                    Tabela="ImpressaoEtiqueta" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <span style="color: Red">Impressões em vermelho estão canceladas.</span>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsImpressao" runat="server" SelectMethod="GetList" TypeName="Glass.Data.DAL.ImpressaoEtiquetaDAO"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" 
                    >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtIdPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumeroNFe" Name="numeroNFe" 
                            PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtIdImpressao" Name="idImpressao" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtPlanoCorte" Name="planoCorte" 
                            PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtLote" Name="lote" 
                            PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="txtEtiqueta" Name="etiqueta" 
                            PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpTipoImpressao" Name="tipoImpressao" 
                            PropertyName="SelectedValue" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoImpressao" runat="server" 
                    SelectMethod="GetTiposEtiquetas" TypeName="Glass.Data.DAL.ProdutoImpressaoDAO" 
                    >
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
