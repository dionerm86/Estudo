<%@ Page Title="Cotação de Compra" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadCotacaoCompra.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCotacaoCompra" %>

<%@ Register Src="../Controls/ctrlSelProduto.ascx" TagName="ctrlSelProduto" TagPrefix="uc1" %>
<%@ Register src="../Controls/ctrlParcelasSelecionar.ascx" tagname="ctrlParcelasSelecionar" tagprefix="uc3" %>
<%@ Register src="../Controls/ctrlParcelas.ascx" tagname="ctrlParcelas" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcProd.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        function getTipoCalculoProd()
        {
            var tipoCalc = FindControl("ctrlSelProduto1_hdfTipoCalculo", "input");
            if (!tipoCalc) tipoCalc = FindControl("hdfTipoCalc", "input");
            return tipoCalc ? tipoCalc.value : 1;
        }

        function habilitaCamposProduto(nomeControle, idProd)
        {
            var tipoCalc = getTipoCalculoProd();
            var altura = FindControl("txtAltura", "input");
            var largura = FindControl("txtLargura", "input");

            altura.disabled = CalcProd_DesabilitarAltura(tipoCalc);
            largura.disabled = CalcProd_DesabilitarLargura(tipoCalc);
        }

        function validaCampoVazio(val, args)
        {
            var controle = document.getElementById(val.controltovalidate);
            args.IsValid = controle.disabled || args.Value != "";
        }

        function calcM2()
        {
            var idProd = FindControl("ctrlSelProdBuscar_hdfValor", "input").value;
            var altura = FindControl("txtAltura", "input").value;
            var largura = FindControl("txtLargura", "input").value;
            var qtde = FindControl("txtQtde", "input").value;
            var tipoCalc = getTipoCalculoProd();

            if (altura == "" || largura == "" || qtde == "" || altura == "0")
                return false;

            /*
            var adicVidroRedondoAte12mm = Glass.Configuracoes.Geral.AdicionalVidroRedondoAte12mm
            var adicVidroRedondoAcima12mm = Glass.Configuracoes.Geral.AdicionalVidroRedondoAcima12mm

            // Se a opção vidro redondo estiver marcado, adiciona 50mm na largura e na altura,
            // bisote e lapidação não cobram sobre este acréscimo
            if (FindControl("Redondo_chkSelecao", "input") != null && FindControl("Redondo_chkSelecao", "input").checked)
            {
                var esp = FindControl("txtEspessura", "input").value;
                var addValor = esp < 12 ? adicVidroRedondoAte12mm : adicVidroRedondoAcima12mm;

                altura = parseInt(altura) + parseInt(addValor);
                largura = parseInt(largura) + parseInt(addValor);
            }
            */

            if (tipoCalc == 2 || tipoCalc == 10)
                FindControl("lblTotM2", "span").innerHTML = CadCotacaoCompra.CalcM2(idProd, altura, largura, qtde, tipoCalc).value;
        }

        function habilitarTodos(check)
        {
            var tabela = check;
            while (tabela.nodeName.toLowerCase() != "table")
                tabela = tabela.parentNode;

            var inputs = tabela.getElementsByTagName("input");
            for (var c = 0; c < inputs.length; c++)
                if (inputs[c].type == "checkbox" && inputs[c].id != check.id && inputs[c].checked != check.checked)
                {
                    inputs[c].checked = check.checked;
                    habilitar(inputs[c]);
                }
        }

        function habilitar(codCotacaoCompra, codProduto, codFornecedor, controle)
        {
            if (typeof codCotacaoCompra == "object")
            {
                var linha = codCotacaoCompra;
                while (linha.nodeName.toLowerCase() != "tr")
                    linha = linha.parentNode;

                var controle = FindControl("chkHabilitar", "input", linha);
                controle.checked = !controle.checked;
                controle.click();
                    
                return;
            }

            var linha = controle;
            while (linha.nodeName.toLowerCase() != "tr")
                linha = linha.parentNode;

            var custo = FindControl("txtCusto", "input", linha).value;
            var prazo = FindControl("txtPrazo", "input", linha).value;
            var codParcela = FindControl("ctrlParcelas_drpParcelas", "select", linha).value;
            var datas = codParcela == -1 ? FindControl("ctrlParcelasCustom_tblParcelas", "table", linha).id : "";
            if (datas != "") datas = eval(datas.replace("_tblParcelas", "")).Datas(true);
            var custoTotal = FindControl("lblCustoTotal", "span", linha);
            
            var resposta = CadCotacaoCompra.Habilitar(codCotacaoCompra, codProduto, codFornecedor, custo, prazo, codParcela, datas, controle.checked).value.split("|");
            if (resposta[0] == "Erro")
            {
                controle.checked = !controle.checked;
                alert(resposta[1]);
                return;
            }
            else
                custoTotal.innerHTML = resposta[1];

            for (var i = 1; i < linha.cells.length; i++)
            {
                linha.cells[i].style.color = controle.checked ? "" : "silver";

                var inputs = linha.cells[i].getElementsByTagName("input");
                for (var j = 0; j < inputs.length; j++)
                {
                    if (inputs[j].id.indexOf("ctrlParcelas") == -1)
                        inputs[j].disabled = !controle.checked;
                }
                    
                var selects = linha.cells[i].getElementsByTagName("select");
                for (var j = 0; j < selects.length; j++)
                    selects[j].disabled = !controle.checked;
            }
        }

        function iniciarCalculo()
        {
            document.getElementById("botoes").style.display = "none";

            var prioridade = document.getElementById("prioridade");
            prioridade.style.display = "";

            FindControl("btnAbrirCalculo", "input").style.display = "";
            FindControl("btnAbrirFinalizar", "input").style.display = "none";
        }

        function iniciarFinalizar()
        {
            document.getElementById("botoes").style.display = "none";
            
            var prioridade = document.getElementById("prioridade");
            prioridade.style.display = "";

            FindControl("btnAbrirCalculo", "input").style.display = "none";
            FindControl("btnAbrirFinalizar", "input").style.display = "";
        }

        function abrirCalculo()
        {
            var id = '<%= Request["id"] %>';
            var tipo = FindControl("drpTipoCalculo", "select").value;
            
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=CotacaoCompraCalculada" +
                "&id=" + id + "&tipo=" + tipo + "&exibirValor=true");
            
            cancelarCalculoFinalizar();
        }

        function cancelarCalculoFinalizar()
        {
            document.getElementById("botoes").style.display = "";
            document.getElementById("prioridade").style.display = "none";
        }

        function imprimirFornecedor()
        {
            var id = '<%= Request["id"] %>';
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=CotacaoCompraFornecedor" + "&id=" + id);
        }

        function parcelasChanged(controleParcelas, campoParcelas)
        {
            Parc_visibilidadeParcelas(controleParcelas, null);
            
            var chk, linha = campoParcelas;
            while (linha.nodeName.toLowerCase() != 'tr' ||
                !(chk = FindControl("chkHabilitar", "input", linha)))
            {
                linha = linha.parentNode;
            }

            habilitar(chk);
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvCotacaoCompra" runat="server" AutoGenerateRows="False" DataSourceID="odsCotacaoCompra"
                    DataKeyNames="Codigo" CssClass="gridStyle detailsViewStyle" GridLines="None"
                    OnItemCommand="dtvCotacaoCompra_ItemCommand">
                    <FieldHeaderStyle CssClass="dtvHeader" />
                    <Fields>
                        <asp:TemplateField HeaderText="Código" InsertVisible="False" SortExpression="Codigo">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("Codigo") %>' Font-Bold="True"
                                    Font-Size="Medium"></asp:Label>
                                <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Bind("Situacao") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Codigo") %>' Font-Bold="True"
                                    Font-Size="Medium"></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Observação" SortExpression="Observacao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObservacao" runat="server" Rows="3" Text='<%# Bind("Observacao") %>'
                                    TextMode="MultiLine" Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Observacao") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Func. Cadastro" InsertVisible="False" SortExpression="NomeFuncCadastro">
                            <EditItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("NomeFuncCadastro") %>'></asp:Label>
                                <asp:HiddenField ID="hdfCodFuncCadastro" runat="server" Value='<%# Bind("CodFuncCadastro") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("NomeFuncCadastro") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Cadastro" InsertVisible="False" SortExpression="DataCadastro">
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" 
                                    Text='<%# Eval("DataCadastro", "{0:d}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("DataCadastro", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    Text="Cancelar" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" Text="Inserir" CommandName="Insert" />
                                <asp:Button ID="btnVoltar" runat="server" OnClick="btnVoltar_Click" Text="Voltar" />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <div id="botoes">
                                    <asp:Button ID="btnEditar" runat="server" Text="Editar" CommandName="Edit" CausesValidation="False" />
                                    <asp:Button ID="btnCalcular" runat="server" Text="Calcular cotação" CausesValidation="False" OnClientClick="iniciarCalculo(); return false" />
                                    <asp:Button ID="btnFinalizar" runat="server" Text="Finalizar cotação" CausesValidation="false" OnClientClick="iniciarFinalizar(); return false" />
                                </div>
                                <div id="prioridade" style="display: none; white-space: nowrap; text-align: left; padding: 8px; border: 1px solid silver; margin: 20px">
                                    <span>
                                        Prioridade:
                                    </span>
                                    <br />
                                    <asp:DropDownList ID="drpTipoCalculo" runat="server" 
                                        DataSourceID="odsTipoCalculo" DataTextField="Descr" DataValueField="Id">
                                    </asp:DropDownList>
                                    <asp:Button ID="btnAbrirCalculo" runat="server" Text="Calcular" OnClientClick="abrirCalculo(); return false" />
                                    <asp:Button ID="btnAbrirFinalizar" runat="server" Text="Finalizar" CausesValidation="false" CommandName="Finalizar" CommandArgument='<%# Eval("Codigo") %>'
                                        OnClientClick="bloquearPagina(); desbloquearPagina(false)" />
                                    <asp:Button ID="btnCancelar" runat="server" Text="Fechar" OnClientClick="cancelarCalculoFinalizar(); return false" />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:DetailsView>
            </td>
            <td rowspan="4" id="separador1" runat="server">
                &nbsp;
            </td>
            <td rowspan="4" id="separador2" runat="server" style="border-left: 1px solid #E2E2E4; padding-left: 3px">
                &nbsp;
            </td>
            <td align="center" valign="top" rowspan="4" id="fornecedorProduto" runat="server">
                <table class="pos">
                    <tr>
                        <td class="subtitle1">
                            Preço por Fornecedor
                        </td>
                    </tr>
                </table>
                <table class="pos">
                    <tr>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpProduto" runat="server" DataSourceID="odsProdutos" 
                                DataTextField="Descr" DataValueField="Id">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" 
                                ImageUrl="~/Images/Pesquisar.gif" onclick="imgPesq_Click" 
                                CausesValidation="False" />
                        </td>
                    </tr>
                </table>
                <table class="pos">
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFornecedor" runat="server" 
                                DataSourceID="odsFornecedor" DataTextField="Descr" DataValueField="Id">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" 
                                ImageUrl="~/Images/Pesquisar.gif" onclick="imgPesq_Click" 
                                CausesValidation="False" />
                        </td>
                    </tr>
                </table>
                <asp:GridView ID="grdProdutoFornecedorCotacaoCompra" runat="server" 
                    AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" 
                    CssClass="gridStyle" 
                    DataKeyNames="CodigoCotacaoCompra,CodigoProduto,DescricaoProduto" 
                    DataSourceID="odsProdutoFornecedorCotacaoCompra" GridLines="None" 
                    onrowdatabound="grdProdutoFornecedorCotacaoCompra_RowDataBound">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkHabilitar" runat="server" onclick="habilitarTodos(this)" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkHabilitar" runat="server" 
                                    Checked='<%# Eval("Cadastrado") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Produto" SortExpression="DescricaoProduto">
                            <EditItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("DescricaoProduto") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" 
                                    Text='<%# Bind("CodigoDescricaoProduto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fornecedor" SortExpression="NomeFornecedor">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("NomeFornecedor") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" 
                                    Text='<%# Bind("CodigoNomeFornecedor") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Preço de Compra Total" 
                            SortExpression="CustoTotal">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("CustoTotal") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblCustoTotal" runat="server" 
                                    Text='<%# Bind("CustoTotal", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Preço de Compra" SortExpression="CustoUnitario">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("CustoUnitario") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:TextBox ID="txtCusto" runat="server" Width="70px"
                                    onkeypress="return soNumeros(event, false, true)" 
                                    Text='<%# Bind("CustoUnitario") %>'
                                    onchange="habilitar(this)"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Prazo Entrega (Dias)" 
                            SortExpression="PrazoEntregaDias">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" 
                                    Text='<%# Bind("PrazoEntregaDias") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:TextBox ID="txtPrazo" runat="server" 
                                    onkeypress="return soNumeros(event, true, true)" Width="70px" 
                                    Text='<%# Bind("PrazoEntregaDias") %>'
                                    onchange="habilitar(this)"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Condição de Pagamento" 
                            SortExpression="CodigoParcela">
                            <ItemTemplate>
                                <asp:HiddenField ID="hdfCodigoFornecedor" runat="server" 
                                    Value='<%# Eval("CodigoFornecedor") %>' />
                                <uc3:ctrlParcelasSelecionar ID="ctrlParcelas" runat="server" 
                                    ExibirParcConfiguravel="True" OnLoad="ctrlParcelas_Load" 
                                    ParcelaPadraoInt='<%# Eval("CodigoParcela") %>' TipoConsulta="Todos" 
                                    CallbackSelecaoParcelas="parcelasChanged" NumeroParcelas='<%# Eval("NumeroParcelasConfiguradas") %>' />
                                <div>
                                    <uc2:ctrlParcelas ID="ctrlParcelasCustom" runat="server" OnLoad="ctrlParcelasCustom_Load"
                                        ReadOnly="True" ExibirValores="false" 
                                        CssClass="pos" DiasSomarDataVazia="30" NumParcelasLinha="3" Datas='<%# Eval("DatasParcelasConfiguradas") %>' />
                                    <asp:HiddenField ID="hdfDataBase" runat="server" />
                                    <asp:HiddenField ID="hdfCalcularParcelas" runat="server" Value='<%# (int)Eval("CodigoParcela") != -1 %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="imprimirFornecedor(); return false"> <img src="../images/Printer.png" border="0" /> Imprimir </asp:LinkButton>
            </td>
        </tr>
        <tr runat="server" id="separadorProdutos">
            <td>
                &nbsp;
            </td>
        </tr>
        <tr runat="server" id="tituloProdutos">
            <td class="subtitle1">
                Produtos
            </td>
        </tr>
        <tr runat="server" id="produtos">
            <td align="center" valign="top">
                <asp:GridView ID="grdProdutoCotacaoCompra" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataKeyNames="Codigo"
                    DataSourceID="odsProdutoCotacaoCompra" GridLines="None" ShowFooter="True" OnDataBound="grdProdutoCotacaoCompra_DataBound"
                    OnRowCommand="grdProdutoCotacaoCompra_RowCommand" 
                    onrowdeleted="grdProdutoCotacaoCompra_RowDeleted">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/Ok.gif" />
                                <asp:ImageButton ID="imgExcluir" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                                <asp:HiddenField ID="hdfCodigoCotacao" runat="server" 
                                    Value='<%# Bind("CodigoCotacaoCompra") %>' />
                                <asp:HiddenField ID="hdfCodigoProduto" runat="server" 
                                    Value='<%# Bind("CodigoProduto") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif"
                                    CausesValidation="False" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja excluir esse produto?&quot;)) return false"
                                    CausesValidation="False" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Produto" SortExpression="CodigoProduto">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" 
                                    Text='<%# Eval("CodigoDescricaoProduto") %>'></asp:Label>
                                <asp:HiddenField ID="hdfTipoCalc" runat="server" Value='<%# Eval("TipoCalc") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc1:ctrlSelProduto ID="ctrlSelProduto1" runat="server" PermitirVazio="false" Callback="habilitaCamposProduto"
                                    FazerPostBackBotaoPesquisar="False" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" 
                                    Text='<%# Bind("CodigoDescricaoProduto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Quantidade" SortExpression="Quantidade">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtQtde" runat="server" Text='<%# Bind("QuantidadeString") %>' onchange="calcM2()"
                                    Width="60px" onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(getTipoCalculoProd()), true)"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvQtde" runat="server" ControlToValidate="txtQtde"
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtQtde" runat="server" onchange="calcM2()" onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(getTipoCalculoProd()), true)"
                                    Width="60px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvQtde" runat="server" ControlToValidate="txtQtde"
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Quantidade") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura" runat="server" Text='<%# Bind("Largura") %>' onchange="calcM2()"
                                    Width="70px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                <asp:CustomValidator ID="ctvLargura" runat="server" ClientValidationFunction="validaCampoVazio"
                                    ControlToValidate="txtLargura" Display="Dynamic" ErrorMessage="*" ValidateEmptyText="True"></asp:CustomValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtLargura" runat="server" onchange="calcM2()" Width="70px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                <asp:CustomValidator ID="ctvLargura" runat="server" ClientValidationFunction="validaCampoVazio"
                                    ControlToValidate="txtLargura" Display="Dynamic" ErrorMessage="*" ValidateEmptyText="True"></asp:CustomValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura" runat="server" Text='<%# Bind("AlturaString") %>' Width="70px"
                                    onchange="calcM2()" onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(getTipoCalculoProd()), true)"></asp:TextBox>
                                <asp:CustomValidator ID="ctvAltura" runat="server" ClientValidationFunction="validaCampoVazio"
                                    ControlToValidate="txtAltura" Display="Dynamic" ErrorMessage="*" ValidateEmptyText="True"></asp:CustomValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAltura" runat="server" onchange="calcM2()" onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(getTipoCalculoProd()), true)"
                                    Width="70px"></asp:TextBox>
                                <asp:CustomValidator ID="ctvAltura" runat="server" ClientValidationFunction="validaCampoVazio"
                                    ControlToValidate="txtAltura" Display="Dynamic" ErrorMessage="*" ValidateEmptyText="True"></asp:CustomValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Altura") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total m²" SortExpression="TotalM2">
                            <EditItemTemplate>
                                <asp:Label ID="lblTotM2" runat="server" Text='<%# Eval("TotalM2") %>'></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotM2" runat="server">0</asp:Label>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("TotalM2") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAdd" runat="server" ImageUrl="~/Images/Insert.gif" OnClick="imgAdd_Click" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCotacaoCompra" runat="server" DataObjectTypeName="WebGlass.Business.CotacaoCompra.Entidade.CotacaoCompra"
        InsertMethod="InserirCotacaoCompra" SelectMethod="ObtemCotacaoCompra" TypeName="WebGlass.Business.CotacaoCompra.Fluxo.CRUD"
        UpdateMethod="AtualizarCotacaoCompra" oninserted="odsCotacaoCompra_Inserted" onupdated="odsCotacaoCompra_Updated">
        <SelectParameters>
            <asp:QueryStringParameter Name="codigo" QueryStringField="id" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutoCotacaoCompra" runat="server" InsertMethod="InserirProdutoCotacaoCompra"
        MaximumRowsParameterName="pageSize" SelectCountMethod="ObtemNumeroProdutosCotacaoCompra"
        SelectMethod="ObtemProdutosCotacaoCompra" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" TypeName="WebGlass.Business.CotacaoCompra.Fluxo.CRUD"
        UpdateMethod="AtualizarProdutoCotacaoCompra" DataObjectTypeName="WebGlass.Business.CotacaoCompra.Entidade.ProdutoCotacaoCompra"
        DeleteMethod="ExcluirProdutoCotacaoCompra" EnablePaging="True">
        <SelectParameters>
            <asp:QueryStringParameter Name="codigoCotacaoCompra" QueryStringField="id" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutoFornecedorCotacaoCompra" runat="server" 
        EnablePaging="True" MaximumRowsParameterName="pageSize" 
        SelectCountMethod="ObtemNumeroProdutosFornecedorCotacaoCompra" 
        SelectMethod="ObtemProdutosFornecedorCotacaoCompra" 
        SortParameterName="sortExpression" StartRowIndexParameterName="startRow" 
        TypeName="WebGlass.Business.CotacaoCompra.Fluxo.CRUD">
        <SelectParameters>
            <asp:QueryStringParameter Name="codigoCotacaoCompra" QueryStringField="id" 
                Type="UInt32" />
            <asp:ControlParameter ControlID="drpFornecedor" Name="codigoFornecedor" 
                PropertyName="SelectedValue" Type="UInt32" />
            <asp:ControlParameter ControlID="drpProduto" Name="codigoProduto" 
                PropertyName="SelectedValue" Type="UInt32" />
            <asp:Parameter Name="apenasCadastrados" Type="Boolean" DefaultValue="false" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutos" runat="server" 
        SelectMethod="GetProdutos" 
        TypeName="WebGlass.Business.CotacaoCompra.Fluxo.BuscarEValidar">
        <SelectParameters>
            <asp:QueryStringParameter Name="codigoCotacaoCompra" QueryStringField="id" 
                Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFornecedor" runat="server" SelectMethod="GetFornecedores" 
        TypeName="WebGlass.Business.CotacaoCompra.Fluxo.BuscarEValidar">
        <SelectParameters>
            <asp:QueryStringParameter Name="codigoCotacaoCompra" QueryStringField="id" 
                Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCalculo" runat="server" 
        SelectMethod="ObtemTiposCalculoCotacao" 
        TypeName="WebGlass.Business.CotacaoCompra.Fluxo.CalcularCotacao">
    </colo:VirtualObjectDataSource>
    <script type="text/javascript">
        $(document).ready(function()
        {
            habilitaCamposProduto("", "");

            var tabela = document.getElementById("<%= grdProdutoFornecedorCotacaoCompra.ClientID %>");
            
            if (tabela != null) {
                for (var i = 1; i < tabela.rows.length; i++)
                {
                    var calcular = FindControl("hdfCalcularParcelas", "input", tabela.rows[i]);
                    if (!!calcular)
                        calcular.value = "true";
                }
            }
        });
    </script>
</asp:Content>
