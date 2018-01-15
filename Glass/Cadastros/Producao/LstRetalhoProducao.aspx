<%@ Page Title="Lista de Retalhos de Produção" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstRetalhoProducao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Producao.LstRetalhoProducao" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../../Controls/ctrlLogCancPopup.ascx" TagName="ctrlLogCancPopup" TagPrefix="uc2" %>

<%@ Register Src="../../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        // Carrega dados do produto com base no código do produto passado
        function setProduto() {
            var codInterno = FindControl("txtCodProd", "input").value;

            if (codInterno == "")
                return false;

            try {
                var retorno = MetodosAjax.GetProd(codInterno).value.split(';');

                if (retorno[0] == "Erro") {
                    alert(retorno[1]);
                    FindControl("txtCodProd", "input").value = "";
                    return false;
                }

                FindControl("txtDescr", "input").value = retorno[2];
                FindControl("hdfIdProd", "input").value = retorno[1];
            }
            catch (err) {
                alert(err.value);
            }
        }

        function imprimir(exportarExcel) {
            var codInterno = FindControl("txtCodProd", "input").value;
            var descrProd = FindControl("txtDescr", "input").value;
            var dataIni = FindControl("ctrlDataInicio_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var dataUsoIni = FindControl("ctrlDataUsoInicio_txtData", "input").value;
            var dataUsoFim = FindControl("ctrlDataUsoFim_txtData", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var espessura = FindControl("txtEspessura", "input").value;
            var idsCores = FindControl("cbdCor", "select").itens();
            var alturaInicio = FindControl("txtAlturaInicio", "input").value;
            var alturaFim = FindControl("txtAlturaFim", "input").value;
            var larguraInicio = FindControl("txtLarguraInicio", "input").value;
            var larguraFim = FindControl("txtLarguraFim", "input").value;
            var numEtiqueta = FindControl("txtNumEtiqueta", "input").value;
            var observacao = FindControl("txtObservacao", "input").value;

            openWindow(600, 800, "../../Relatorios/RelBase.aspx?rel=RetalhosProducao" +
                "&codInterno=" + codInterno + "&descrProduto=" + descrProd + "&dataIni=" + dataIni +
                "&dataUsoIni=" + dataUsoIni + "&dataUsoFim=" + dataUsoFim +
                "&dataFim=" + dataFim + "&situacao=" + situacao + "&idsCores=" + idsCores +
                "&espessura=" + espessura + "&alturaInicio=" + alturaInicio +
                "&alturaFim=" + alturaFim + "&larguraInicio=" + larguraInicio +
                "&larguraFim=" + larguraFim + "&numEtiqueta=" + numEtiqueta +
                "&observacao=" + observacao + "&exportarExcel=" + exportarExcel);
        }

        function disponibilizar() {
            openWindow(600, 800, "../../Utils/DisponibilizarRetalhos.aspx");
        }

        function exibirMensagem(mensagem) {
            alert(mensagem);
        }

        function exibirDados(botao, titulo, msg) {

            var boxObs = FindControl("boxObs", "div");
            var lblObs = FindControl("lblObs", "span");

            lblObs.innerHTML = msg;

            TagToTip('boxObs', FADEIN, 300, COPYCONTENT, false, TITLE, titulo, CLOSEBTN, true,
                CLOSEBTNTEXT, 'Fechar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                FIX, [botao, 9 - getTableWidth('boxObs'), -41 - getTableHeight('boxObs')]);
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Produto"
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="setProduto();"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescr" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq0" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> <img border="0" src="../../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Período cad.:"
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataInicio" runat="server" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> <img border="0" src="../../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Período uso:"
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataUsoInicio" runat="server" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataUsoFim" runat="server" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton6" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> <img border="0" src="../../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                    </tr>

                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Num. Etiqueta"
                                ForeColor="#0066FF"></asp:Label>
                        </td>

                        <td>
                            <asp:TextBox ID="txtNumEtiqueta" runat="server" Width="80px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton5" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> 
                                <img border="0" src="../../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Situação"
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="true">
                                <asp:ListItem Value="0" Text="Todas"></asp:ListItem>
                                <asp:ListItem Value="1" Text="Disponíveis"></asp:ListItem>
                                <asp:ListItem Value="3" Text="Em uso"></asp:ListItem>
                                <asp:ListItem Value="4" Text="Em Estoque"></asp:ListItem>
                                <asp:ListItem Value="5" Text="Vendido"></asp:ListItem>
                                <asp:ListItem Text="Cancelado" Value="2"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="lblCor" runat="server" Text="Cor"
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdCor" runat="server" Width="110px" CheckAll="False" Title="Selecione o tipo"
                                DataSourceID="odsCorVidro" DataTextField="Descricao" DataValueField="IdCorVidro" ImageURL="~/Images/DropDown.png"
                                JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" OpenOnStart="False">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton22" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> 
                                <img border="0" src="../../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="lblEspessura" runat="server" Text="Espessura"
                                ForeColor="#0066FF"></asp:Label>
                        </td>

                        <td>
                            <asp:TextBox ID="txtEspessura" runat="server" Width="40px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton2" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> 
                                <img border="0" src="../../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="lblAltura" runat="server" Text="Altura"
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAlturaInicio" runat="server" Width="40px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="até"
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAlturaFim" runat="server" Width="40px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton3" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> 
                                <img border="0" src="../../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="lblLargura" runat="server" Text="Largura"
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtLarguraInicio" runat="server" Width="40px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="até"
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtLarguraFim" runat="server" Width="40px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton4" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> 
                                <img border="0" src="../../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Obs.:"
                                ForeColor="#0066FF"></asp:Label>
                        </td>

                        <td>
                            <asp:TextBox ID="txtObservacao" runat="server" Width="300px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton7" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> 
                                <img border="0" src="../../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">&nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <asp:HyperLink ID="lnkInserir" runat="server"
                    NavigateUrl="~/Cadastros/Producao/CadRetalhoProducao.aspx">Inserir Retalho Avulso</asp:HyperLink>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkDisponibilizar" runat="server" OnClientClick="disponibilizar(); return false">Disponibilizar Retalhos</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdRetalho" runat="server" AutoGenerateColumns="False" GridLines="None"
                    AllowPaging="True" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataSourceID="odsRetalho" AllowSorting="True" EmptyDataText="Nenhum registro encontrado">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgRelatorio" runat="server"
                                    ImageUrl="~/Images/Relatorio.gif"
                                    OnClientClick='<%# Eval("IdRetalhoProducao", "openWindow(600, 800, \"../../Relatorios/RelEtiquetas.aspx?idRetalhoProducao={0}&ind=1\"); return false") %>'
                                    Visible='<%# ExibirImpressao(Eval("CancelarVisible")) %>' />
                                <asp:ImageButton ID="imgCancelar" runat="server"
                                    ImageUrl="~/Images/ExcluirGrid.gif"
                                    Visible='<%# Eval("CancelarVisible") %>'
                                    OnClientClick='<%# Eval("IdRetalhoProducao", "openWindow(180, 550, \"../../Utils/SetMotivoCancRetalho.aspx?idRetalhoProducao={0}\"); return false") %>' />
                                <asp:ImageButton ID="imgPerda" runat="server"
                                    ImageUrl="~/Images/perda.png" Width="16" Height="16" ToolTip="Marcar perda"
                                    Visible='<%# Eval("PerdaVisible") %>'
                                    OnClientClick='<%# Eval("IdRetalhoProducao", "openWindow(180, 550, \"../../Utils/SetMotivoPerdaRetalho.aspx?idRetalhoProducao={0}\"); return false") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CodInterno" HeaderText="Cód." SortExpression="CodInterno" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura"
                            SortExpression="Largura" />
                        <asp:BoundField DataField="Altura" HeaderText="Altura"
                            SortExpression="Altura" />
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data Cad."
                            SortExpression="DataCad" />
                        <asp:BoundField DataField="SituacaoString" HeaderText="Situação" SortExpression="SituacaoString" />
                        <asp:BoundField DataField="NumeroEtiqueta" HeaderText="Número de Etiqueta" />
                        <asp:BoundField DataField="NumeroNFe" HeaderText="Número NF-e" />
                        <asp:BoundField DataField="Lote" HeaderText="Lote" />
                        <asp:BoundField DataField="TotM" DataFormatString="{0:0.##} m²"
                            HeaderText="M² total" />
                        <asp:TemplateField HeaderText="Etiquetas">
                            <ItemTemplate>
                                <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Eval("EtiquetaUsando") != null && ((string)Eval("EtiquetaUsando")) != "" %>'>
                                    <a href="#" onclick='exibirDados(this, &#039Leituras do retalho: <%# Eval("NumeroEtiqueta") %>&#039,&#039;<%# Eval("EtiquetaUsando") != null ? ((string)Eval("EtiquetaUsando")).Replace("\r\n"," ") : "" %>&#039;); return false;'>
                                        <img alt="" border="0" src="../../Images/blocodenotas.png" title="Ver leituras" /></a>
                                </asp:PlaceHolder>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DataUso" DataFormatString="{0:d}" HeaderText="Data Uso" SortExpression="DataUso" />
                        <asp:TemplateField HeaderText="M² aproveitado">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server"
                                    Text='<%# (double)Eval("TotMUsando") > 0 ? Eval("TotMUsando", "{0:0.##} m²") : "" %>'></asp:Label>
                                <asp:Label ID="Label6" runat="server"
                                    Text='<%# (double)Eval("TotMUsando") > 0 ? "(" + String.Format("{0:p}", (double)Eval("TotMUsando") / (float)Eval("TotM")) + ")" : "" %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Funcionário">
                            <ItemTemplate>
                                <asp:Label ID="Label50" runat="server" Text='<%# Eval("NomeFunc") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Obs" HeaderText="Obs" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc3:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="RetalhoProducao"
                                    IdRegistro='<%# Eval("IdRetalhoProducao") %>' />
                                <uc2:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server"
                                    IdRegistro='<%# Eval("IdRetalhoProducao") %>' Tabela="RetalhoProducao" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRetalho" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    DataObjectTypeName="Glass.Data.Model.RetalhoProducao" DeleteMethod="Delete" SelectMethod="ObterLista"
                    SelectCountMethod="ObterCount" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.RetalhoProducaoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInterno" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescr" Name="descrProduto" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataInicio" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataUsoInicio" Name="dataUsoIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataUsoFim" Name="dataUsoFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="cbdCor" Name="idsCores" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtEspessura" Name="espessura" PropertyName="Text"
                            Type="Double" />
                        <asp:ControlParameter ControlID="txtAlturaInicio" Name="alturaInicio" PropertyName="Text"
                            Type="Double" />
                        <asp:ControlParameter ControlID="txtAlturaFim" Name="alturaFim" PropertyName="Text"
                            Type="Double" />
                        <asp:ControlParameter ControlID="txtLarguraInicio" Name="larguraInicio" PropertyName="Text"
                            Type="Double" />
                        <asp:ControlParameter ControlID="txtLarguraFim" Name="larguraFim" PropertyName="Text"
                            Type="Double" />
                        <asp:ControlParameter ControlID="txtNumEtiqueta" Name="numEtiqueta" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtObservacao" Name="observacao" PropertyName="Text"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCorVidro" runat="server"
                    SelectMethod="GetForFiltro" TypeName="Glass.Data.DAL.CorVidroDAO">
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfIdProd" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="imprimir(false); return false">
                    <img src="../../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="imprimir(true); return false;"><img border="0" 
                    src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>

    <div id="boxObs" style="display: none; width: 350px;">
        <asp:Label ID="lblObs" runat="server" Text="Label"></asp:Label>
    </div>
</asp:Content>
