<%@ Page Title="Estoque de Produtos" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstEstoque.aspx.cs" Inherits="Glass.UI.Web.Listas.LstEstoque" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlSelParticipante.ascx" TagName="ctrlSelParticipante"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlSelCorProd.ascx" TagName="ctrlSelCorProd" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
    
        // Carrega dados do produto com base no código do produto passado
        function setProduto() {
            var codInterno = FindControl("txtCodProd", "input").value;

            if (codInterno == "")
                return false;

            try {
                var retorno = MetodosAjax.ObtemProdutoParaListagem(codInterno).value.split(';');

                if (retorno[0] == "Erro") {
                    alert(retorno[1]);
                    FindControl("txtCodProd", "input").value = "";
                    return false;
                }
            }
            catch (err) {
                alert(err.value);
            }
        }
        
        // Esconde uma coluna da tabela
        function escondeColuna(numColuna)
        {
            var tabela = document.getElementById("<%= grdEstoque.ClientID %>");
            for (i = 0; i < tabela.rows.length; i++)
            {
                if (i == (tabela.rows.length - 1) && tabela.rows[i].cells.length == 1)
                    break;
                
                tabela.rows[i].cells[numColuna].style.display = "none";
            }
        }
        
        function openRpt(exportarExcel) {
            var idLoja = FindControl("drpLoja", "select").value;
            var codProd = FindControl("txtCodProd", "input").value;
            var descrProd = FindControl("txtDescr", "input").value;
            var idGrupo = FindControl("drpGrupo", "select").value;
            var idSubgrupo = FindControl("drpSubgrupo", "select").value;
            var controleCor = <%= ctrlSelCorProd1.ClientID %>;
            var apenasPosseTerceiros = FindControl("chkApenasPosseTerceiros", "input");
            apenasPosseTerceiros = apenasPosseTerceiros != null ? apenasPosseTerceiros.checked : false;
            var apenasProdutosProjeto = FindControl("chkApenasProdutosProjeto", "input");
            apenasProdutosProjeto = apenasProdutosProjeto != null ? apenasProdutosProjeto.checked : false;
            var apenasComEstoque = FindControl("chkComEstoque", "input").checked;
            var aguardSaidaEstoque = FindControl("chkAguardSaidaEstoque", "input").checked;
            var ordenacao = FindControl("drpOrdenar", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;

            var rel = "Estoque" + (<%= (Request["fiscal"] != "1").ToString().ToLower() %> ? "Real" : "Fiscal");
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=" + rel + "&idLoja=" + idLoja + "&codProd=" + codProd +
                "&descr=" + descrProd + "&idGrupo=" + idGrupo + "&idSubgrupo=" + idSubgrupo +
                "&apenasPosseTerceiros=" + apenasPosseTerceiros + "&apenasProdutosProjeto=" + apenasProdutosProjeto +
                "&idCorVidro=" + controleCor.idCorVidro() +  "&idCorFerragem=" + controleCor.idCorFerragem() +
                "&idCorAluminio=" + controleCor.idCorAluminio() + "&apenasEstoqueFiscal=" + apenasComEstoque +
                "&aguardSaidaEstoque=" + aguardSaidaEstoque +  "&situacao=" + situacao + "&ordenacao=" + ordenacao + "&exportarExcel=" + exportarExcel);
        }

        function limparEstoque()
        {
            var temSubgrupo = FindControl("drpSubgrupo", "select").value != "0";
            if (!confirm("Deseja limpar o a quantidade e o m² em estoque dos produtos dessa loja" +
                (temSubgrupo ? "," : " e") + " grupo" + (temSubgrupo ? " e subgrupo" : "") + "?"))
                return false;

            return true;
        }
        
        function openRptEstoque(idProduto, tipo)
        {
            var idLoja = FindControl("drpLoja", "select").value;
        
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=EstoqueProdutos&TipoColunas=" + tipo + "&idProd=" + idProduto + 
                "&idLoja=" + idLoja + "&agrupar=false");
        }
        
        function abrirReserva(idProduto)
        {
            openRptEstoque(idProduto, 1);
        }

        function abrirLiberacao(idProduto)
        {
            openRptEstoque(idProduto, 2);
        }
        
        function insercaoRapidaEstoque()
        {
            bloquearPagina();
            desbloquearPagina(true);
        }
       
                
    </script>

    <table align="center">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc4:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true" MostrarTodas="false"/>
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Cód." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="setProduto();" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescr" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                Width="200px"></asp:TextBox>
                            <asp:LinkButton ID="lnkPesq0" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label23" runat="server" ForeColor="#0066FF" Text="Situação"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Selected="True" Value="1">Ativo</asp:ListItem>
                                <asp:ListItem Value="2">Inativo</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq2" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkApenasProdutosProjeto" runat="server" Text="Exibir apenas produtos associados a projetos"
                                AutoPostBack="True" /></td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label6" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupo" runat="server" AutoPostBack="True" DataSourceID="odsGrupoProd"
                                DataTextField="Descricao" DataValueField="IdGrupoProd">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label7" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSubgrupo" runat="server" AutoPostBack="True" DataSourceID="odsSubgrupoProd"
                                DataTextField="Descricao" DataValueField="IdSubgrupoProd">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq" runat="server" OnClick="lnkPesq_Click"><img border="0\" 
                                src="../images/pesquisar.gif"></img></asp:LinkButton>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label13" runat="server" Text="Cor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlSelCorProd ID="ctrlSelCorProd1" runat="server" OnLoad="ctrlSelCorProd1_Load" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkPesq_Click"><img border="0\" 
                                src="../images/pesquisar.gif"></img></asp:LinkButton>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkApenasPosseTerceiros" runat="server" Text="Exibir apenas produtos em posse de terceiros"
                                AutoPostBack="True" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkComEstoque" runat="server" Text="Apenas produtos com estoque" />
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq1" runat="server" OnClick="lnkPesq_Click"><img border="0\" 
                                src="../images/pesquisar.gif"></img></asp:LinkButton>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAguardSaidaEstoque" runat="server" Text="Produtos aguardando saída de estoque" />
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesqSaidaEstoque" runat="server" OnClick="lnkPesq_Click"><img border="0\" 
                                src="../images/pesquisar.gif"></img></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="lblOrdenar" runat="server" ForeColor="#0066FF" Text="Ordenar por"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenar" runat="server">
                                <asp:ListItem Value="1">Produto</asp:ListItem>
                                <asp:ListItem Value="2">Qtd. Estoque (Cresc.)</asp:ListItem>
                                <asp:ListItem Value="3">Qtd. Estoque (Decresc.)</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesqSaidaEstoque0" runat="server" OnClick="lnkPesq_Click"><img border="0\" 
                                src="../images/pesquisar.gif"></img></asp:LinkButton>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkInsercaoRapidaEstoque" runat="server" Text="Inserção Rápida de Estoque"
                                AutoPostBack="true" OnCheckedChanged="chkInsercaoRapidaEstoque_CheckedChanged" />
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
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdEstoque" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdProd,IdLoja"
                    DataSourceID="odsEstoque" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" PageSize="20" OnDataBound="grdEstoque_DataBound">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif"
                                    CausesValidation="false" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbEditar" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif"
                                    Visible='<%# Eval("EditVisible") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód." SortExpression="CodInternoProd">
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("CodInternoProd") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("CodInternoProd") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Grupo/Subgrupo" SortExpression="DescrGrupoProd, DescrSubgrupoProd">
                            <EditItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("DescrGrupoSubgrupoProd") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("DescrGrupoSubgrupoProd") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Estoque mínimo" SortExpression="EstMinimo">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEstMinimo" runat="server" onkeypress="return soNumeros(event, true, true)"
                                    Text='<%# Bind("EstMinimo") %>' Width="70px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("EstoqueMinimoString") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="M² em Estoque" SortExpression="M2">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# (Eval("M2") ?? "").ToString() + (Eval("DescrEstoque") ?? "").ToString() %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtM2Estoque" runat="server" Text='<%# Bind("M2") %>' Width="70px"
                                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                <asp:Label ID="Label11" runat="server" Text='<%# Eval("DescrEstoque") %>'></asp:Label>
                                <asp:RequiredFieldValidator ID="rfvM2Estoque" runat="server" ControlToValidate="txtM2Estoque"
                                    Display="Dynamic" ErrorMessage="Campo não pode ser vazio."></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Reserva" SortExpression="Reserva">
                            <EditItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("Reserva") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkReserva" runat="server" OnClientClick='<%# "abrirReserva(" + Eval("IdProd") + "); return false" %>'
                                    Text='<%# Eval("ReservaString") %>' Visible='<%# Eval("TipoCalc").ToString() == "1" %>'></asp:LinkButton>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Liberação" SortExpression="Liberacao">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkLiberacao" runat="server" OnClientClick='<%# "abrirLiberacao(" + Eval("IdProd") + "); return false" %>'
                                    Text='<%# Eval("LiberacaoString") %>' Visible='<%# Eval("TipoCalc").ToString() == "1" %>'></asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("Liberacao") %>'></asp:Label>
                            </EditItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtd. em Estoque" SortExpression="QtdEstoque">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtQtdEstoque" runat="server" Text='<%# Bind("QtdEstoque") %>'
                                    Width="70px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("DescrEstoque") %>'></asp:Label>
                                <asp:RequiredFieldValidator ID="rfvQtdEstoque" runat="server" ControlToValidate="txtQtdEstoque"
                                    Display="Dynamic" ErrorMessage="Campo não pode ser vazio."></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("QtdEstoqueStringLabel") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Disponível">
                            <EditItemTemplate>
                                <asp:Label ID="Label20" runat="server" Text='<%# Eval("EstoqueDisponivel") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label20" runat="server" Text='<%# Bind("EstoqueDisponivel") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtd. em Estoque Fiscal" SortExpression="EstoqueFiscal">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtQtdEstoqueFiscal" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("EstoqueFiscal") %>' Width="70px"></asp:TextBox>
                                <asp:Label ID="Label10" runat="server" Text='<%# Eval("DescrEstoque") %>'></asp:Label>
                                <asp:RequiredFieldValidator ID="rfvQtdEstoqueFiscal" runat="server" ControlToValidate="txtQtdEstoqueFiscal"
                                    Display="Dynamic" ErrorMessage="Campo não pode ser vazio."></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("EstoqueFiscal") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtd. com Defeito" SortExpression="Defeito">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtQtdDefeito" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("Defeito") %>' Width="70px"></asp:TextBox>
                                <asp:Label ID="Label12" runat="server" Text='<%# Eval("DescrEstoque") %>'></asp:Label>
                                <asp:RequiredFieldValidator ID="rfvQtdDefeito" runat="server" ControlToValidate="txtQtdDefeito"
                                    Display="Dynamic" ErrorMessage="Campo não pode ser vazio."></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("Defeito") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtde. Posse Terceiros" SortExpression="QtdePosseTerceiros">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtQtdePosseTerc" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("QtdePosseTerceiros") %>' Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("QtdePosseTerceiros") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Item em posse de">
                            <EditItemTemplate>
                                <uc2:ctrlSelParticipante ID="ctrlSelParticipante1" runat="server" IdCliente='<%# Bind("IdCliente") %>'
                                    IdFornec='<%# Bind("IdFornec") %>' IdLoja='<%# Bind("IdLojaTerc") %>' IdTransportador='<%# Bind("IdTransportador") %>'
                                    IdAdminCartao='<%# Bind("IdAdminCartao") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label21" runat="server" Font-Italic="True" Text='<%# Eval("DescrTipoPart") %>'></asp:Label>
                                <asp:Label ID="Label22" runat="server" Text='<%# Eval("DescrPart") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <%-- <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" IdRegistro='<%# Eval("IdLog") %>'
                                    Tabela="ProdutoLoja" /> --%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HiddenField ID="hdfCodProduto" runat="server" Value='<%# Eval("IdLoja") + "," + Eval("IdProd") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtd. em Estoque" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtQtdEstoqueInsercaoRapida" runat="server" Text='<%# Bind("QtdEstoque") %>'
                                    Width="70px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvQtdEstoqueInsercaoRapida" runat="server" ControlToValidate="txtQtdEstoqueInsercaoRapida"
                                    Display="Dynamic" ErrorMessage="Campo deve ser preenchido."></asp:RequiredFieldValidator>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtd. em Estoque Fiscal" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtQtdEstoqueFiscalInsercaoRapida" runat="server" Text='<%# Bind("EstoqueFiscal") %>'
                                    Width="70px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvQtdEstoqueFiscalInsercaoRapida" runat="server"
                                    ControlToValidate="txtQtdEstoqueFiscalInsercaoRapida" Display="Dynamic" ErrorMessage="Campo deve ser preenchido."></asp:RequiredFieldValidator>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="ProdutoLoja" Campo="Qtde. Posse Terceiros" IdRegistro='<%# Eval("IdLog") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button runat="server" ID="btnSalvarInsercaoRapida" Text="Salvar" Width="100"
                    Visible="false" OnClick="SalvarInsercaoRapida" OnClientClick="bloquearPagina(); desbloquearPagina(false);" />
                <div id="blanket" style="display: none; position: fixed; left: 0px; top: 0px; z-index: 99999;
                    width: 100%; height: 100%">
                    <iframe frameborder="0" style="position: absolute; left: 0px; top: 0px; width: 100%;
                        height: 100%"></iframe>
                    <div style="width: 100%; height: 100%; opacity: 0.8; background-color: white; position: absolute;
                        left: 0; top: 0">
                    </div>
                    <div style="text-align: center; top: 40%; position: relative">
                        <span>
                            <img src="<%= this.ResolveClientUrl("~/Images/Load.gif") %>" height="96px" />
                            <br />
                            <span style="font-size: xx-large">Aguarde </span>
                            <br />
                            <span style="font-size: medium">Processando suas informações... </span></span>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false">
                    <img src="../Images/printer.png" border="0" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkLimparEstoque" runat="server" OnClientClick="if (!limparEstoque()) return false"
                    OnClick="lnkLimparEstoque_Click">
                    <img src="../Images/ExcluirGrid.gif" border="0" /> Limpar Estoque</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEstoque" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectMethod="GetForEstoque" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
        TypeName="Glass.Data.DAL.ProdutoLojaDAO" DataObjectTypeName="Glass.Data.Model.ProdutoLoja"
        SelectCountMethod="GetForEstoqueCount" UpdateMethod="AtualizaEstoque" 
        OnUpdated="odsEstoque_Updated" >
        <SelectParameters>
            <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                Type="UInt32" />
            <asp:ControlParameter ControlID="txtCodProd" Name="codInternoProd" PropertyName="Text"
                Type="String" />
            <asp:ControlParameter ControlID="txtDescr" Name="descricao" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpGrupo" Name="idGrupoProd" PropertyName="SelectedValue"
                Type="UInt32" />
            <asp:ControlParameter ControlID="drpSubgrupo" Name="idSubgrupoProd" PropertyName="SelectedValue"
                Type="UInt32" />
            <asp:ControlParameter ControlID="chkComEstoque" DefaultValue="false" Name="exibirApenasComEstoque"
                PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="chkApenasPosseTerceiros" DefaultValue="" Name="exibirApenasPosseTerceiros"
                PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="chkApenasProdutosProjeto" 
                Name="exibirApenasProdutosProjeto" PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="ctrlSelCorProd1" Name="idCorVidro" PropertyName="IdCorVidro"
                Type="UInt32" />
            <asp:ControlParameter ControlID="ctrlSelCorProd1" Name="idCorFerragem" PropertyName="IdCorFerragem"
                Type="UInt32" />
            <asp:ControlParameter ControlID="ctrlSelCorProd1" Name="idCorAluminio" PropertyName="IdCorAluminio"
                Type="UInt32" />
            <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                Type="Int32" />
            <asp:QueryStringParameter Name="estoqueFiscal" QueryStringField="fiscal" Type="Int32" />
            <asp:ControlParameter ControlID="chkAguardSaidaEstoque" Name="aguardandoSaidaEstoque"
                PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="drpOrdenar" Name="ordenacao" PropertyName="SelectedValue"
                Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoProd" runat="server" SelectMethod="GetForFilter"
        TypeName="Glass.Data.DAL.GrupoProdDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSubgrupoProd" runat="server" SelectMethod="GetForFilter"
        TypeName="Glass.Data.DAL.SubgrupoProdDAO">
        <SelectParameters>
            <asp:ControlParameter ControlID="drpGrupo" Name="idGrupo" PropertyName="SelectedValue"
                Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>

    <script type="text/javascript">
        <% if (!Glass.Configuracoes.PedidoConfig.LiberarPedido) 
           { %>
               escondeColuna(7);
         <% }
           
           if (Glass.Configuracoes.Geral.NaoVendeVidro())
           { %>
               escondeColuna(5);
        <% }
        
           if (Request["fiscal"] == "1") 
           { %>
               escondeColuna(5);
               escondeColuna(6);
               escondeColuna(7);
               escondeColuna(8);
               escondeColuna(9);
               escondeColuna(11);
        <% }
           else 
           { %>
               escondeColuna(10);
               escondeColuna(12);
               escondeColuna(13);
        <% } %>
    </script>

</asp:Content>
