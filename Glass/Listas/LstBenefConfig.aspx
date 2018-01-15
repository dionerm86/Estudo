<%@ Page Title="Configurações de Beneficiamento" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstBenefConfig.aspx.cs" Inherits="Glass.UI.Web.Listas.LstBenefConfig" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        function openRpt(exportarExcel) {
            var descricao = FindControl("txtDescr", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=PrecoBeneficiamentos&descricao=" + descricao +
            "&exportarExcel=" + exportarExcel);
        }

        function substituirInput(tableCell) {

            var spans = tableCell.getElementsByTagName("span");
            if (spans.length > 0) {
                var span = spans[0];
                var input = document.createElement("input");

                if (span.getAttribute("hidden") == "true") {
                    input.type = "hidden";
                }
                input.setAttribute("onkeypress", "return soNumeros(event, false, true)");
                input.setAttribute("value", span.innerText);
                input.setAttribute("editable", true);
                input.style.width = "70px";
                tableCell.replaceChild(input, span);
            }
        }

        function limparGridInputs() {

            var div = document.getElementById("grdBenefConfig");
            
            var inputs = div.getElementsByTagName("input");
            for (var x = 0; x < inputs.length; x++) {
                var input = inputs[x];
                
                if (input.getAttribute("editable") != undefined) {
                    var span = document.createElement("span");
                    
                    if (input.type == "hidden") {
                        span.setAttribute("hidden", "true");
                    }

                    span.innerText = input.value;
                    input.parentNode.replaceChild(span, input);
                }
            }
        }

        function abrirGrid(id) {

            limparGridInputs(id);

            var div = document.getElementById("grid_" + id);
            var tabela = div.getElementsByTagName("table")[0];
            tabela = tabela.getElementsByTagName("table")[0];

            for (i = 1; i < tabela.rows.length; i++) {

                substituirInput(tabela.rows[i].cells[0]);
                substituirInput(tabela.rows[i].cells[3]);
                substituirInput(tabela.rows[i].cells[4]);
                substituirInput(tabela.rows[i].cells[5]);
                substituirInput(tabela.rows[i].cells[6]);
            }

            TagToTip('grid_' + id, FADEIN, 300, COPYCONTENT, false, TITLE, 'Subgrupo/Cor', CLOSEBTN, true, CLOSEBTNTEXT, 'Fechar', CLOSEBTNCOLORS,
                ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true);
        }

        function salvar(botao, idBenefConfig) {
            var div = document.getElementById("grid_" + idBenefConfig);
            var tabela = div.getElementsByTagName("table")[0];
            tabela = tabela.getElementsByTagName("table")[0];

            var dados = "";
            for (i = 1; i < tabela.rows.length; i++) {
                var idBenefConfigPreco = tabela.rows[i].cells[0].getElementsByTagName("input")[0].value;
                var custo = tabela.rows[i].cells[3].getElementsByTagName("input")[0].value;
                var valorAtacado = tabela.rows[i].cells[4].getElementsByTagName("input")[0].value;
                var valorBalcao = tabela.rows[i].cells[5].getElementsByTagName("input")[0].value;
                var valorObra = tabela.rows[i].cells[6].getElementsByTagName("input")[0].value;

                dados += "|" + idBenefConfigPreco + ";" + custo + ";" + valorAtacado + ";" + valorBalcao + ";" + valorObra;
            }

            if (dados.length > 0)
                dados = dados.substr(1);

            var retorno = LstBenefConfig.Salvar(idBenefConfig, dados).value.split(";");

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                return;
            }
            else {
                alert("Configuração salva com sucesso!");
                tt_HideInit();
            }
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescr" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
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
                <asp:GridView GridLines="None" ID="grdBenefConfig" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsBenefConfig" CssClass="gridStyle" PagerStyle-CssClass="pgr" ClientIDMode="Static"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataKeyNames="IdBenefConfigPreco"
                    AllowPaging="True" PageSize="20" EmptyDataText="Nenhum beneficiamento encontrado.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif" alt="Editar" /></asp:LinkButton>
                                <asp:ImageButton ID="imbGrid" runat="server" ImageUrl="~/Images/gear_add.gif" 
                                    OnClientClick='<%# "abrirGrid(" + Eval("IdBenefConfigPreco") + "); return false" %>'
                                    Visible='<%# ((ICollection)Eval("Precos")).Count > 0 %>' />
                                <div id='grid_<%# Eval("IdBenefConfigPreco") %>' style="display: none">
                                    <table>
                                        <tr>
                                            <td align="center">
                                                <asp:GridView ID="grdCor" runat="server" AutoGenerateColumns="False" CellPadding="3"
                                                    DataSource='<%# Eval("Precos") %>' GridLines="None">
                                                    <RowStyle BackColor="#EFF3FB" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Descrição">
                                                            <ItemTemplate>
                                                                <%# Eval("DescricaoComTipoCalculo") %>
                                                                <span hidden="true"><%# Eval("IdBenefConfigPreco") %></span>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="SubgrupoProd" HeaderText="Subgrupo">
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="CorVidro" HeaderText="Cor">
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="Custo" SortExpression="Custo">
                                                            <ItemTemplate>
                                                                <span><%# Eval("Custo") %></span>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Valor Atacado" SortExpression="ValorAtacado">
                                                            <ItemTemplate>
                                                                <span><%# Eval("ValorAtacado") %></span>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Valor Balcão" SortExpression="ValorBalcao">
                                                            <ItemTemplate>
                                                                <span><%# Eval("ValorBalcao") %></span>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Valor Obra" SortExpression="ValorObra">
                                                            <ItemTemplate>
                                                                <span><%# Eval("ValorObra") %></span>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                    <EditRowStyle BackColor="#2461BF" />
                                                    <AlternatingRowStyle BackColor="White" />
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClientClick='<%# "salvar(this, " + Eval("IdBenefConfigPreco") + "); return false" %>' />
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:HiddenField ID="hdfIdBenefConfig" runat="server" Value='<%# Eval("IdBenefConfig") %>' />
                                    <asp:HiddenField ID="hdfEspessura" runat="server" Value='<%# Eval("Espessura") %>' />
                                </div>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                                <asp:HiddenField ID="hdfEspessura" runat="server" Value='<%# Bind("Espessura") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("DescricaoComTipoCalculo") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("DescricaoComTipoCalculo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Custo" SortExpression="Custo">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorCusto" runat="server" MaxLength="10" onKeyPress="return soNumeros(event, false, true);"
                                    Text='<%# Bind("Custo") %>' Width="70px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Custo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Atacado" SortExpression="ValorAtacado">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorAtacado" Text='<%# Bind("ValorAtacado") %>' MaxLength="10"
                                    onKeyPress="return soNumeros(event, false, true);" runat="server" Width="70"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("ValorAtacado") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Balcao" SortExpression="ValorBalcao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorBalcao" Text='<%# Bind("ValorBalcao") %>' MaxLength="10"
                                    onKeyPress="return soNumeros(event, false, true);" runat="server" Width="70"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("ValorBalcao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Obra" SortExpression="ValorObra">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorObra" Text='<%# Bind("ValorObra") %>' MaxLength="10" onKeyPress="return soNumeros(event, false, true);"
                                    runat="server" Width="70"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("ValorObra") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" IdRegistro='<%# (uint)(int)Eval("IdBenefConfigPreco") %>'
                                    Tabela="BenefConfigPreco" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false;"><img border="0" 
                    src="../Images/Printer.png" alt="Imprimir" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" alt="Exporta para o Excel" /> Exportar para o Excel</asp:LinkButton>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsBenefConfig" runat="server"
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.BenefConfigPreco" EnablePaging="True" 
                    MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarPrecosPadraoBeneficiamentos" 
                    SortParameterName="sortExpression"
                    TypeName="Glass.Global.Negocios.IBeneficiamentoFluxo"
                    UpdateMethod="SalvarPrecoBeneficiamento"
                    UpdateStrategy="GetAndUpdate" 
                    SelectByKeysMethod="ObtemPrecoBeneficiamento">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtDescr" Name="descricao" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
