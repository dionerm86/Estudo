<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstImpostoServico.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstImpostoServico" Title="Lançamento de Imposto/Serviços Avulsos" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlLogCancPopup" TagPrefix="uc4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function getFornec(idFornec) {
            if (idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function openFornec() {
            if (FindControl("txtFornecedor", "input").value != "")
                return true;

            openWindow(500, 700, "../Utils/SelFornec.aspx");

            return false;
        }

        function openRpt(idImpostoServ) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ImpostoServ&idImpostoServ=" + idImpostoServ);
        }

        function openRptLista(exportarExcel) {
            var idImpostoServ = FindControl("txtNumImpostoServ", "input").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var valorIni = FindControl("txtValorIni", "input").value;
            var valorFim = FindControl("txtValorFim", "input").value;
            var idFornec = FindControl("txtFornecedor", "input").value;
            var nomeFornec = FindControl("txtNome", "input").value;
            var contabil = FindControl("drpContabil", "select").value;
            var tipoPagto = FindControl("drpTipoPagto", "select").value;
            var centroCustoDivergente = FindControl("chkCentroCustoDivergente", "input").checked;

            var queryString =
                "&idImpostoServ=" + idImpostoServ +
                "&dataIni=" + dataIni +
                "&dataFim=" + dataFim +
                "&valorIni=" + valorIni +
                "&valorFim=" + valorFim +
                "&idFornec=" + idFornec +
                "&nomeFornec=" + nomeFornec +
                "&contabil=" + contabil +
                "&tipoPagto=" + tipoPagto +
                "&centroCustoDivergente=" + centroCustoDivergente +
                "&exportarexcel=" + exportarExcel;

            openWindow(600, 800, '../Relatorios/RelBase.aspx?rel=ListaImpostoServ' + queryString);
            return false;
        }

        function exibirCentroCusto(idImpostoServ) {

            openWindow(365, 700, '../Utils/SelCentroCusto.aspx?idImpostoServ=' + idImpostoServ);
            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Núm." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumImpostoServ" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq')"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Valor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorIni" runat="server" Width="70px"
                                onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                        <td>a
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorFim" runat="server" Width="70px"
                                onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtFornecedor" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Contábil" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContabil" runat="server" AutoPostBack="True">
                                <asp:ListItem Value=""></asp:ListItem>
                                <asp:ListItem Value="True">Sim</asp:ListItem>
                                <asp:ListItem Value="False">Não</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Tipo Pagto." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoPagto" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="2">À prazo</asp:ListItem>
                                <asp:ListItem Value="1">À vista</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="center">
                            <asp:CheckBox ID="chkCentroCustoDivergente" runat="server" Text="Imposto/Serviços Avulsos com valor do centro custo divergente" AutoPostBack="true" /></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">
                    Inserir imposto/serviço avulso</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                <asp:GridView ID="grdImpostoServ" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdImpostoServ" DataSourceID="odsImpostoServ"
                    GridLines="None" CssClass="gridStyle" OnRowCommand="grdImpostoServ_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" ImageUrl="~/Images/EditarGrid.gif"
                                    OnClientClick='<%# "redirectUrl(\"../Cadastros/CadImpostoServico.aspx?idImpostoServ=" + Eval("IdImpostoServ") + "\"); return false" %>'
                                    Visible='<%# Eval("EditarVisible") %>' />
                                <asp:ImageButton ID="imgCancelar" runat="server" CommandName="Cancelar" ImageUrl="~/Images/ExcluirGrid.gif"
                                    CommandArgument='<%# Eval("IdImpostoServ") %>' OnClientClick="if (!confirm(&quot;Deseja cancelar esse imposto/serviço avulso?&quot;)) return false;"
                                    Visible='<%# Eval("CancelarVisible") %>' />
                                <asp:ImageButton ID="ImageButton2" runat="server"
                                    ImageUrl="~/Images/Relatorio.gif"
                                    OnClientClick='<%# "openRpt(" + Eval("IdImpostoServ") + "); return false" %>' />
                                <asp:PlaceHolder ID="pchAnexos" runat="server"><a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=<%# Eval("IdImpostoServ") %>&tipo=impostoServ&#039;); return false;'>
                                    <img border="0px" src="../Images/Clipe.gif"></img></a></asp:PlaceHolder>

                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdImpostoServ" HeaderText="Núm." SortExpression="IdImpostoServ" />
                        <asp:BoundField DataField="NomeFornec" HeaderText="Fornecedor" SortExpression="NomeFornec" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="DescrPlanoConta" HeaderText="Referente a" SortExpression="DescrPlanoConta" />
                        <asp:BoundField DataField="DescrTipoPagto" HeaderText="Tipo Pagto." ReadOnly="True"
                            SortExpression="DescrTipoPagto" />
                        <asp:BoundField DataField="Nf" HeaderText="NF/Pedido" SortExpression="Nf" />
                        <asp:TemplateField HeaderText="Contabil" SortExpression="Contabil">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Contabil") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Eval("Contabil") %>' Enabled="False" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data Imposto/Serviço"
                            SortExpression="DataCad" />
                        <asp:BoundField DataField="DataFinalizada" DataFormatString="{0:d}" HeaderText="Data Finalização"
                            SortExpression="DataFinalizada" />
                        <asp:TemplateField HeaderText="Situação" SortExpression="DescrSituacao">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("DescrSituacao") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrSituacao") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="imgReabrir" runat="server" ImageUrl="../Images/Cadeado.gif"
                                                Visible='<%# Eval("ReabrirVisible") %>' ToolTip="Reabrir" CommandArgument='<%# Eval("IdImpostoServ") %>'
                                                CommandName="Reabrir" OnClientClick="if (!confirm(&quot;Deseja reabrir esse imposto/serviço?&quot;)) return false;" />
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:c}" />
                        <asp:BoundField DataField="Obs" HeaderText="Obs." SortExpression="Obs" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkFinalizar" runat="server" CommandArgument='<%# Eval("IdImpostoServ") %>'
                                    Visible='<%# Eval("EditarVisible") %>' CommandName="Finalizar" OnClientClick="if (!confirm(&quot;Deseja finalizar esse lançamento de imposto/serviço?&quot;)) return false">Finalizar</asp:LinkButton>
                                <asp:ImageButton ID="imbCentroCusto" runat="server" ImageUrl='<%# "~/Images/" + ((bool)Eval("CentroCustoCompleto") ? "cash_blue.png" : "cash_red.png") %>' Visible='<%# Eval("ExibirCentroCusto") %>'
                                    ToolTip="Exibir Centro de Custos" OnClientClick='<%# "exibirCentroCusto(" + Eval("IdImpostoServ") + "); return false" %>' />
                                <uc4:ctrlLogCancPopup ID="ctrlLogCancPopup2" runat="server" Tabela="ImpostoServ" IdRegistro='<%# Eval("IdImpostoServ") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>                        
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsImpostoServ" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ImpostoServDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumImpostoServ" Name="idImpostoServ" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtValorIni" Name="valorIni"
                            PropertyName="Text" Type="Single" />
                        <asp:ControlParameter ControlID="txtValorFim" Name="valorFim"
                            PropertyName="Text" Type="Single" />
                        <asp:ControlParameter ControlID="txtFornecedor" Name="idFornec"
                            PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeFornec" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpContabil" Name="contabil"
                            PropertyName="SelectedValue" Type="Boolean" />
                        <asp:ControlParameter ControlID="drpTipoPagto" Name="tipoPagto"
                            PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="chkCentroCustoDivergente" Name="centroCustoDivergente" PropertyName="Checked"
                            Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRptLista(false);"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                    <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="return openRptLista(true);"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
