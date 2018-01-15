<%@ Page Title="Novo Modelo de Projeto" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadModeloProjeto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.CadModeloProjeto" %>

<%@ Register Src="../../Controls/ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc1" %>
<%@ Register Src="../../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc2" %>
<%@ Register src="../../Controls/ctrlSelProduto.ascx" tagname="ctrlSelProduto" tagprefix="uc3" %>
<%@ Register src="../../Controls/ctrlSelAplicacao.ascx" tagname="ctrlSelAplicacao" tagprefix="uc4" %>
<%@ Register src="../../Controls/ctrlSelProcesso.ascx" tagname="ctrlSelProcesso" tagprefix="uc5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CallbackItem_ctrlBenef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
    var txtCalcSel = null;
    
    function excluir(tipo, id)
    {
        var perguntar = <%= Glass.Data.Helper.UserInfo.GetUserInfo.IsAdminSync.ToString().ToLower() %>;
        if (perguntar && !confirm("Tem certeza que deseja excluir est" + (tipo == 1 ? "a peça" : "e material") + " do modelo de projeto?"))
            return;
        
        openWindow(150, 420, "../../Utils/SetMotivoCancProj.aspx?tipo=" + tipo + "&id=" + id);
    }
    
    function validacao(idPecaProjetoModelo, idProjetoModelo)
    {
        debugger;
        openWindow(500, 1000, "CadValidacaoPecaModelo.aspx?idPecaProjMod=" + idPecaProjetoModelo + "&idProjetoModelo=" + idProjetoModelo);
    }

    function exibirBenef(botao)
    {
        for (iTip = 0; iTip < 2; iTip++)
        {
            // O controle de beneficiamentos deve ser exibido abaixo das peças, pois se for exibido acima e for muito grande,
            // esconde o botão aplicar
            TagToTip('tbConfigVidro', FADEIN, 300, COPYCONTENT, false, TITLE, 'Beneficiamento', CLOSEBTN, true,
                CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                FIX, [botao, 9 - getTableWidth('tbConfigVidro'), 17]);
        }
    }

    // Abre tela para edição da posição das peças do modelo
    function openPos(idProjetoModelo) {
        openWindow(600, 800, "CadPosicaoPecaModelo.aspx?idProjetoModelo=" + idProjetoModelo);

        return false;
    }

    function setExpressao(expressao) {
        txtCalcSel.value = expressao;
    }

    // Validações para salvar peça
    function salvarPeca() {
        if (!validate("produto"))
            return false;

        var tipoPeca = FindControl("drpTipo", "select");
        var qtd = FindControl("txtCalcPecaQtde", "input").value;
        var item = FindControl("txtItem", "input").value;
        var calcAltura = FindControl("txtCalcAlt", "input").value;
        var calcLargura = FindControl("txtCalcLarg", "input").value;
        var arquivoMarcacao = FindControl("drpArquivo", "select");
        var tipoArquivo = FindControl("drpTipoArquivo", "select");

        if (qtd != undefined && qtd != null && qtd.value == "") {
            alert("Informe a quantidade.");
            return false;
        }

        if (item != undefined && item != null && item.value == "") {
            alert("Informe o item.");
            return false;
        }
        
        if (calcAltura != undefined && calcAltura != null && calcAltura.value == "") {
            alert("Informe o cálculo da altura.");
            return false;
        }
        
        if (calcLargura != undefined && calcLargura != null && calcLargura.value == ""){
            alert("Informe o cálculo da largura.");
            return false;
        }
        
        if ((tipoArquivo == undefined || tipoArquivo == null || tipoArquivo.value == "" || tipoArquivo.value == "0") &&
            tipoPeca != undefined && tipoPeca != null && tipoPeca.value == "Instalacao" &&
            arquivoMarcacao != undefined && arquivoMarcacao != null && arquivoMarcacao.value != "") {
            alert("Informe o tipo do arquivo.");
            return false;
        }
        else if (arquivoMarcacao != undefined && arquivoMarcacao != null && arquivoMarcacao.value == "" &&
            tipoArquivo != undefined && tipoArquivo != null)
            tipoArquivo.value = "";

        return true;
    }

    // Validações para salvar material
    function salvaMaterial() {
        var idProdProj = FindControl("hdfIdProdProj", "input").value;

        if (qtd == "") {
            alert("Informe a quantidade.");
            return false;
        }
    }

    // Função chamada após selecionar produto pelo popup
    function setProduto(codInterno) {
        FindControl("txtIdProdProj", "input").value = codInterno;
        loadProduto(codInterno);
    }

    // Carrega dados do produto com base no código do produto passado
    function loadProduto(codInterno) {
        if (codInterno == "")
            return false;

        try {
            var retorno = CadModeloProjeto.GetProduto(codInterno).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                FindControl("txtIdProdProj", "input").value = "";
                FindControl("lblDescrProd", "span").innerHTML = "";
                FindControl("hdfIdProdProj", "input").value = "";
                return false;
            }
            
            FindControl("hdfIdProdProj", "input").value = retorno[1];
            FindControl("lblDescrProd", "span").innerHTML = retorno[2];
        }
        catch (err) {
            alert(err);
        }
    }

    // Troca os sinais de + das expressões de cálculo para que ao editar a mesma o + não suma
    function trocaSinalMais(descricao) {
        while (descricao.indexOf("+") > 0)
            descricao = descricao.replace("+", "@");

        return descricao;
    }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:DetailsView ID="dtvProjetoModelo" runat="server" DataKeyNames="IdProjetoModelo"
                                DataSourceID="odsProjetoModelo" DefaultMode="Insert" Height="50px" Width="125px"
                                AutoGenerateRows="False" GridLines="None" OnDataBound="dtvProjetoModelo_DataBound">
                                <Fields>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <table>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="Label1" runat="server" Text="Código"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtCodigo" runat="server" MaxLength="50" Text='<%# Bind("Codigo") %>'
                                                            Width="70px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="Label2" runat="server" Text="Descrição"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtDescricao" runat="server" MaxLength="150" Text='<%# Bind("Descricao") %>'
                                                            Width="270px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="Label12" runat="server" Text="Produto para NF-e"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <uc3:ctrlSelProduto ID="selProdNfe" runat="server" 
                                                            IdProd='<%# Bind("IdProdParaNf") %>' PermitirVazio="True" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="lblGrupo" runat="server" Text="Grupo"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="drpGrupo" runat="server" DataSourceID="odsGrupoModelo" DataTextField="Descricao"
                                                            DataValueField="IdGrupoModelo" SelectedValue='<%# Bind("IdGrupoModelo") %>'>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="lblFiguraEngenharia" runat="server" Text="Figura (Engenharia)"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:FileUpload ID="fluFiguraEngenharia" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="Label5" runat="server" Text="Figura (Modelo)"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:FileUpload ID="fluFiguraModelo" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="Label6" runat="server" Text="Espessura"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtEspessura" runat="server" MaxLength="2" onkeypress="return soNumeros(event, false, true)"
                                                            Text='<%# Bind("Espessura") %>' Width="70px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="Label3" runat="server" Text="Cor"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <sync:CheckBoxListDropDown ID="cbdCorVidro" runat="server" DataSourceID="odsCorVidro"
                                                            DataTextField="Descricao"  DataValueField="IdCorVidro" OpenOnStart="False" Title="Selecione uma cor"
                                                            SelectedValue='<%# Bind("CorVidro") %>' Width="250px">
                                                        </sync:CheckBoxListDropDown>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="Label7" runat="server" Text="Medidas"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:CheckBoxList ID="cblMedidas" runat="server" DataSourceID="odsMedidaProjeto"
                                                            DataTextField="Descricao" DataValueField="IdMedidaProjeto" RepeatColumns="3">
                                                        </asp:CheckBoxList>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="hdfMedidas" runat="server" Value='<%# Eval("MedidasProjMod") %>' />
                                            <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Bind("Situacao") %>' />
                                            <asp:HiddenField ID="hdfFiguraEng" runat="server" Value='<%# Bind("NomeFiguraAssociada") %>' />
                                            <asp:HiddenField ID="hdfFiguraMod" runat="server" Value='<%# Bind("NomeFigura") %>' />
                                            <asp:HiddenField ID="hdfAlturaFigura" runat="server" Value='<%# Bind("AlturaFigura") %>' />
                                            <asp:HiddenField ID="hdfLarguraFigura" runat="server" Value='<%# Bind("LarguraFigura") %>' />
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <table>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="Label8" runat="server" Font-Bold="True" Text="Código"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:Label ID="lblCodigo" runat="server" Text='<%# Eval("Codigo") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="Label9" runat="server" Font-Bold="True" Text="Descrição"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblDescricao" runat="server" Text='<%# Eval("Descricao") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="Label10" runat="server" Font-Bold="True" Text="Grupo"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblGrupo" runat="server" Text='<%# Eval("DescrGrupo") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="Label11" runat="server" Font-Bold="True" Text="Espessura"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:Label ID="lblEspessura" runat="server" Text='<%# Eval("Espessura") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                 <tr>
                                                    <td align="left">
                                                        <asp:Label ID="Label4" runat="server" Font-Bold="True" Text="Cor"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:Label ID="Label13" runat="server" Text='<%# Eval("DescrCorVidro") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar" />
                                            <asp:Button ID="btnCancelar" runat="server" OnClick="btnVoltar_Click" Text="Cancelar" />
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Button ID="btnEditar" runat="server" CommandName="Edit" Text="Editar" />
                                            <asp:Button ID="btnVoltar" runat="server" OnClick="btnVoltar_Click" Text="Voltar" />
                                            <asp:LinkButton ID="lnkSetPos" runat="server" Visible="true" ToolTip="Configurar posição das informações na figura"
                                                OnClientClick='<%# "return openPos(\"" + Eval("IdProjetoModelo") + "\");" %>'>
                                                <img border="0" src="../../Images/Coord.gif" /></asp:LinkButton>
                                        </ItemTemplate>
                                        <InsertItemTemplate>
                                            <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" />
                                            <asp:Button ID="btnCancelar" runat="server" OnClick="btnVoltar_Click" Text="Cancelar" />
                                        </InsertItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Wrap="True" />
                                    </asp:TemplateField>
                                </Fields>
                            </asp:DetailsView>
                        </td>
                        <td style="padding-left: 8px">
                            <asp:Image ID="imgModelo" runat="server" onload="imgModelo_Load" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" class="title">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center" class="subtitle1">
                <asp:Label ID="lblPecas" runat="server" Text="Peças"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdPecaProjetoModelo" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsPecaProjetoModelo" ShowFooter="True" DataKeyNames="IdPecaProjMod"
                    OnPreRender="grdPecaProjetoModelo_PreRender" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" OnRowCommand="grdPecaProjetoModelo_RowCommand"
                    OnDataBound="grdPecaProjetoModelo_DataBound">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../../Images/Edit.gif" /></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick='<%# "excluir(1, " + Eval("IdPecaProjMod") + "); return false" %>' />
                                <asp:ImageButton ID="imbValidacao" runat="server" ImageUrl="~/Images/validacao.gif" OnClientClick='<%# "validacao(" + Eval("IdPecaProjMod") + "," + Eval("IdProjetoModelo") + "); return false;" %>' ToolTip="Validações" />
                                <asp:HiddenField ID="hdfIdPecaProjMod" runat="server" Value='<%# Bind("IdPecaProjMod") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="return salvarPeca();" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" CausesValidation="False" />
                                <asp:HiddenField ID="hdfIdProjetoModelo" runat="server" Value='<%# Bind("IdProjetoModelo") %>' />
                                <asp:HiddenField ID="hdfIdPecaProjMod" runat="server" Value='<%# Bind("IdPecaProjMod") %>' />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="DescrTipoSigla">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server" SelectedValue='<%# Bind("Tipo") %>'>
                                    <asp:ListItem Value="1">Instalação</asp:ListItem>
                                    <asp:ListItem Value="2">Caixilho</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipoIns" runat="server">
                                    <asp:ListItem Value="1">Instalação</asp:ListItem>
                                    <asp:ListItem Value="2">Caixilho</asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("DescrTipo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCalcPecaQtde" runat="server" Text='<%# Bind("CalculoQtde") %>' onpaste="return false;" onkeydown="return false;" onkeyup="return false;"
                                    MaxLength="150" Width="100px">
                                </asp:TextBox>
                                <a href="#" onclick='txtCalcSel=FindControl(&#039;txtCalcPecaQtde&#039;, &#039;input&#039;); openWindow(500, 700, &#039;../../Utils/SelExpressao.aspx?tipo=peca&amp;idProjetoModelo=<%= Request["idProjetoModelo"] %>&amp;expr=&#039; + trocaSinalMais(FindControl(&#039;txtCalcPecaQtde&#039;, &#039;input&#039;).value));'>
                                    <img border="0" src="../../Images/Pesquisar.gif" /></a>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCalcPecaQtde" runat="server" MaxLength="150" Text='<%# Bind("CalculoQtde") %>' onpaste="return false;" onkeydown="return false;" onkeyup="return false;"
                                    Width="100px">
                                </asp:TextBox>
                                <a href="#" onclick='txtCalcSel=FindControl(&#039;txtCalcPecaQtde&#039;, &#039;input&#039;); openWindow(500, 700, &#039;../../Utils/SelExpressao.aspx?tipo=peca&amp;idProjetoModelo=<%= Request["idProjetoModelo"] %>&amp;expr=&#039; + trocaSinalMais(FindControl(&#039;txtCalcPecaQtde&#039;, &#039;input&#039;).value));'>
                                    <img border="0" src="../../Images/Pesquisar.gif" /></a>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("CalculoQtde") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Folga (Alt.)" SortExpression="Altura">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura" runat="server" Text='<%# Bind("Altura") %>' onkeypress="return soNumeros(event, true, false)"
                                    MaxLength="5" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAlturaIns" runat="server" MaxLength="5" onkeypress="return soNumeros(event, true, false)"
                                    Text='<%# Bind("Altura") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAltura" runat="server" Text='<%# Bind("Altura") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Folga (Larg.)" SortExpression="Largura">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura" runat="server" Text='<%# Bind("Largura") %>' onkeypress="return soNumeros(event, true, false)"
                                    MaxLength="5" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtLarguraIns" runat="server" MaxLength="5" onkeypress="return soNumeros(event, true, false)"
                                    Text='<%# Bind("Largura") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLargura" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Folga (Alt. 03MM)" SortExpression="Altura 03MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura03MM" runat="server" Text='<%# Bind("Altura03MM") %>' onkeypress="return soNumeros(event, true, false)"
                                    MaxLength="5" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAltura03MMIns" runat="server" MaxLength="5" onkeypress="return soNumeros(event, true, false)"
                                    Text='<%# Bind("Altura03MM") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAltura03MM" runat="server" Text='<%# Bind("Altura03MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Folga (Larg. 03MM)" SortExpression="Largura03MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura03MM" runat="server" Text='<%# Bind("Largura03MM") %>' onkeypress="return soNumeros(event, true, false)"
                                    MaxLength="5" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtLargura03MMIns" runat="server" MaxLength="5" onkeypress="return soNumeros(event, true, false)"
                                    Text='<%# Bind("Largura03MM") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLargura03MM" runat="server" Text='<%# Bind("Largura03MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Folga (Alt. 04MM)" SortExpression="Altura 04MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura04MM" runat="server" Text='<%# Bind("Altura04MM") %>' onkeypress="return soNumeros(event, true, false)"
                                    MaxLength="5" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAltura04MMIns" runat="server" MaxLength="5" onkeypress="return soNumeros(event, true, false)"
                                    Text='<%# Bind("Altura04MM") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAltura04MM" runat="server" Text='<%# Bind("Altura04MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Folga (Larg. 04MM)" SortExpression="Largura04MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura04MM" runat="server" Text='<%# Bind("Largura04MM") %>' onkeypress="return soNumeros(event, true, false)"
                                    MaxLength="5" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtLargura04MMIns" runat="server" MaxLength="5" onkeypress="return soNumeros(event, true, false)"
                                    Text='<%# Bind("Largura04MM") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLargura04MM" runat="server" Text='<%# Bind("Largura04MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Folga (Alt. 05MM)" SortExpression="Altura 05MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura05MM" runat="server" Text='<%# Bind("Altura05MM") %>' onkeypress="return soNumeros(event, true, false)"
                                    MaxLength="5" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAltura05MMIns" runat="server" MaxLength="5" onkeypress="return soNumeros(event, true, false)"
                                    Text='<%# Bind("Altura05MM") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAltura05MM" runat="server" Text='<%# Bind("Altura05MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Folga (Larg. 05MM)" SortExpression="Largura05MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura05MM" runat="server" Text='<%# Bind("Largura05MM") %>' onkeypress="return soNumeros(event, true, false)"
                                    MaxLength="5" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtLargura05MMIns" runat="server" MaxLength="5" onkeypress="return soNumeros(event, true, false)"
                                    Text='<%# Bind("Largura05MM") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLargura05MM" runat="server" Text='<%# Bind("Largura05MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Folga (Alt. 06MM)" SortExpression="Altura 06MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura06MM" runat="server" Text='<%# Bind("Altura06MM") %>' onkeypress="return soNumeros(event, true, false)"
                                    MaxLength="5" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAltura06MMIns" runat="server" MaxLength="5" onkeypress="return soNumeros(event, true, false)"
                                    Text='<%# Bind("Altura06MM") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAltura06MM" runat="server" Text='<%# Bind("Altura06MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Folga (Larg. 06MM)" SortExpression="Largura06MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura06MM" runat="server" Text='<%# Bind("Largura06MM") %>' onkeypress="return soNumeros(event, true, false)"
                                    MaxLength="5" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtLargura06MMIns" runat="server" MaxLength="5" onkeypress="return soNumeros(event, true, false)"
                                    Text='<%# Bind("Largura06MM") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLargura06MM" runat="server" Text='<%# Bind("Largura06MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Folga (Alt. 08MM)" SortExpression="Altura08MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura08MM" runat="server" Text='<%# Bind("Altura08MM") %>' onkeypress="return soNumeros(event, true, false)"
                                    MaxLength="5" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAltura08MMIns" runat="server" MaxLength="5" onkeypress="return soNumeros(event, true, false)"
                                    Text='<%# Bind("Altura08MM") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAltura08MM" runat="server" Text='<%# Bind("Altura08MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Folga (Larg. 08MM)" SortExpression="Largura08MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura08MM" runat="server" Text='<%# Bind("Largura08MM") %>' onkeypress="return soNumeros(event, true, false)"
                                    MaxLength="5" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtLargura08MMIns" runat="server" MaxLength="5" onkeypress="return soNumeros(event, true, false)"
                                    Text='<%# Bind("Largura08MM") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLargura08MM" runat="server" Text='<%# Bind("Largura08MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Folga (Alt. 10MM)" SortExpression="Altura10MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura10MM" runat="server" Text='<%# Bind("Altura10MM") %>' onkeypress="return soNumeros(event, true, false)"
                                    MaxLength="5" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAltura10MMIns" runat="server" MaxLength="5" onkeypress="return soNumeros(event, true, false)"
                                    Text='<%# Bind("Altura10MM") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAltura10MM" runat="server" Text='<%# Bind("Altura10MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Folga (Larg. 10MM)" SortExpression="Largura10MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura10MM" runat="server" Text='<%# Bind("Largura10MM") %>' onkeypress="return soNumeros(event, true, false)"
                                    MaxLength="5" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtLargura10MMIns" runat="server" MaxLength="5" onkeypress="return soNumeros(event, true, false)"
                                    Text='<%# Bind("Largura10MM") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLargura10MM" runat="server" Text='<%# Bind("Largura10MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Folga (Alt. 12MM)" SortExpression="Altura12MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura12MM" runat="server" Text='<%# Bind("Altura12MM") %>' onkeypress="return soNumeros(event, true, false)"
                                    MaxLength="5" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAltura12MMIns" runat="server" MaxLength="5" onkeypress="return soNumeros(event, true, false)"
                                    Text='<%# Bind("Altura12MM") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAltura12MM" runat="server" Text='<%# Bind("Altura12MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Folga (Larg. 12MM)" SortExpression="Largura12MM">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura12MM" runat="server" Text='<%# Bind("Largura12MM") %>' onkeypress="return soNumeros(event, true, false)"
                                    MaxLength="5" Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtLargura12MMIns" runat="server" MaxLength="5" onkeypress="return soNumeros(event, true, false)"
                                    Text='<%# Bind("Largura12MM") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLargura12MM" runat="server" Text='<%# Bind("Largura12MM") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Item" SortExpression="Item">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtItem" runat="server" MaxLength="20" Text='<%# Bind("Item") %>'
                                    Width="70px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtItemIns" runat="server" MaxLength="20" Text='<%# Bind("Item") %>'
                                    Width="70px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Item") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cálculo Altura" SortExpression="CalculoAltura">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCalcAlt" runat="server" MaxLength="150" onpaste="return false;" onkeydown="return false;" onkeyup="return false;"
                                    Text='<%# Bind("CalculoAltura") %>' Width="100px"></asp:TextBox>
                                <a href="#" onclick="txtCalcSel=FindControl('txtCalcAlt', 'input'); openWindow(500, 700, '../../Utils/SelExpressao.aspx?item=' + FindControl('txtItem', 'input').value + '&tipo=peca&idProjetoModelo=<%= Request["idProjetoModelo"] %>&expr=' + trocaSinalMais(FindControl('txtCalcAlt', 'input').value));">
                                    <img src="../../Images/Pesquisar.gif" border="0" /></a>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCalcAltIns" runat="server" onpaste="return false;" onkeydown="return false;" onkeyup="return false;" MaxLength="150"
                                    Width="100px"></asp:TextBox>
                                <a href="#" onclick="txtCalcSel=FindControl('txtCalcAlt', 'input'); openWindow(500, 700, '../../Utils/SelExpressao.aspx?item=' + FindControl('txtItemIns', 'input').value + '&tipo=peca&idProjetoModelo=<%= Request["idProjetoModelo"] %>&expr=' + trocaSinalMais(FindControl('txtCalcAlt', 'input').value));">
                                    <img src="../../Images/Pesquisar.gif" border="0" /></a>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("CalculoAltura") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cálculo Largura" SortExpression="CalculoLargura">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCalcLarg" runat="server" MaxLength="150" onpaste="return false;" onkeydown="return false;" onkeyup="return false;"
                                    Text='<%# Bind("CalculoLargura") %>' Width="100px"></asp:TextBox>
                                <a href="#" onclick="txtCalcSel=FindControl('txtCalcLarg', 'input'); openWindow(500, 700, '../../Utils/SelExpressao.aspx?item=' + FindControl('txtItem', 'input').value + '&tipo=peca&idProjetoModelo=<%= Request["idProjetoModelo"] %>&expr=' + trocaSinalMais(FindControl('txtCalcLarg', 'input').value));">
                                    <img src="../../Images/Pesquisar.gif" border="0" /></a>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCalcLargIns" runat="server" onpaste="return false;" onkeydown="return false;" onkeyup="return false;" MaxLength="150"
                                    Width="100px"></asp:TextBox>
                                <a href="#" onclick="txtCalcSel=FindControl('txtCalcLarg', 'input'); openWindow(500, 700, '../../Utils/SelExpressao.aspx?item=' + FindControl('txtItemIns', 'input').value + '&tipo=peca&idProjetoModelo=<%= Request["idProjetoModelo"] %>&expr=' + trocaSinalMais(FindControl('txtCalcLarg', 'input').value));">
                                    <img src="../../Images/Pesquisar.gif" border="0" /></a>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("CalculoLargura") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs" SortExpression="Obs">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("Obs") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" MaxLength="100" Text='<%# Bind("Obs") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtObs" runat="server" MaxLength="100" Text='<%# Bind("Obs") %>'></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Arquivo Mesa" SortExpression="IdArquivoMesaCorte">
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("CodArqMesa") %>'></asp:Label>
                                <asp:HiddenField ID="hdfIdArquivoMesaCorte" runat="server" Value='<%# Eval("IdArquivoMesaCorte") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpArquivo" runat="server" AppendDataBoundItems="True" DataSourceID="odsArquivoMesaCorte"
                                    DataTextField="Codigo" DataValueField="IdArquivoMesaCorte" SelectedValue='<%# Bind("IdArquivoMesaCorte") %>'
                                    AutoPostBack="true" OnSelectedIndexChanged="drpArquivo_SelectedIndexChanged" OnDataBound="drpArquivo_DataBound" >
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpArquivo" runat="server" AppendDataBoundItems="True" DataSourceID="odsArquivoMesaCorte"
                                    DataTextField="Codigo" DataValueField="IdArquivoMesaCorte" AutoPostBack="true"
                                    OnSelectedIndexChanged="drpArquivo_SelectedIndexChanged" OnDataBound="drpArquivo_DataBound" >
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Arquivo" SortExpression="TipoArquivoMesaCorte">
                            <ItemTemplate>
                                <asp:Label ID="lblTipoArquivo" runat="server" Text='<%# Bind("TipoArquivo") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoArquivo" runat="server" AppendDataBoundItems="true" DataSourceID="odsTipoArquivoMesaCorte"
                                    DataTextField="Translation" DataValueField="Key" SelectedValue='<%# Bind("TipoArquivo") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipoArquivo" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoArquivoMesaCorte"
                                    DataTextField="Translation" DataValueField="Key">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Flag">
                            <ItemTemplate>
                                <asp:Label ID="Label66" runat="server" Text='<%# Eval("FlagsArqMesaDescricao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                               <sync:CheckBoxListDropDown ID="drpFlagArqMesa" runat="server" DataSourceID="odsFlagArqMesa" AppendDataBoundItems="true"
                                    DataTextField="Name" DataValueField="Id" Title="Flag Arq. de Mesa" SelectedValues='<%# Bind("FlagsArqMesa") %>'>
                                </sync:CheckBoxListDropDown>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <sync:CheckBoxListDropDown ID="drpFlagArqMesa" runat="server" DataSourceID="odsFlagArqMesa" AppendDataBoundItems="true"
                                    DataTextField="Name" DataValueField="Id" Title="" SelectedValues='<%# Bind("FlagsArqMesa") %>'>
                                </sync:CheckBoxListDropDown>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Aplicação" SortExpression="IdAplicacao">
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("CodAplicacao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc4:ctrlSelAplicacao ID="ctrlSelAplicacao1" runat="server" 
                                    CodigoAplicacao='<%# Bind("IdAplicacao") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc4:ctrlSelAplicacao ID="ctrlSelAplicacao" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Processo" SortExpression="IdProcesso">
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc5:ctrlSelProcesso ID="ctrlSelProcesso1" runat="server"
                                    CodigoProcesso='<%# Bind("IdProcesso") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc5:ctrlSelProcesso ID="ctrlSelProcesso" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:LinkButton ID="lnkBenef" runat="server" OnClientClick="exibirBenef(this); return false;">
                                    <img border="0" src="../../Images/gear_add.gif" />
                                </asp:LinkButton>
                                <table id="tbConfigVidro" cellspacing="0" style="display: none;">
                                    <tr>
                                        <td>
                                            <uc1:ctrlBenef ID="ctrlBenef1" runat="server" OnLoad="ctrlBenef1_Load" Redondo='<%# Bind("Redondo") %>'
                                                ValidationGroup="produto" Beneficiamentos='<%# Bind("Beneficiamentos") %>' CarregarBenefPadrao="false"
                                                CalcularBeneficiamentoPadrao="true" />
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkBenef" runat="server" OnClientClick="exibirBenef(this); return false;">
                                    <img border="0" src="../../Images/gear_add.gif" />
                                </asp:LinkButton>
                                <table id="tbConfigVidro" cellspacing="0" style="display: none;">
                                    <tr>
                                        <td>
                                            <uc1:ctrlBenef ID="ctrlBenef1" runat="server" OnLoad="ctrlBenef1_Load" ValidationGroup="produto"
                                                CarregarBenefPadrao="false" />
                                        </td>
                                    </tr>
                                </table>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInsPeca" runat="server" OnClientClick="return salvarPeca();"
                                    OnClick="lnkInsPeca_Click">
                                    <img border="0" src="../../Images/insert.gif" /></asp:LinkButton>
                            </FooterTemplate>
                            <ItemTemplate>
                                <uc2:ctrlLogPopup ID="ctrlLogPopup1" runat="server" IdRegistro='<%# Eval("IdPecaProjMod") %>'
                                    Tabela="PecaProjetoModelo" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center" class="subtitle1">
                <asp:Label ID="lblMateriais" runat="server" Text="Materiais"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdMaterialProjetoModelo" runat="server" ShowFooter="True"
                    AutoGenerateColumns="False" DataKeyNames="IdMaterProjMod" DataSourceID="odsMaterialProjetoModelo"
                    OnPreRender="grdMaterialProjetoModelo_PreRender" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../../Images/Edit.gif" /></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" OnClientClick='<%# "excluir(2, " + Eval("IdMaterProjMod") + "); return false" %>'
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                                <asp:HiddenField ID="hdfIdProjetoModelo" runat="server" Value='<%# Bind("IdProjetoModelo") %>' />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProdProj">
                            <EditItemTemplate>
                                <asp:Label ID="lblDescrProd" runat="server" Text='<%# Eval("DescrProdProj") %>'></asp:Label>&nbsp;
                                <asp:HiddenField ID="hdfIdProdProj" runat="server" Value='<%# Bind("IdProdProj") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtIdProdProj" runat="server" onblur="loadProduto(this.value);"
                                    onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                    Width="50px"></asp:TextBox>
                                <asp:Label ID="lblDescrProd" runat="server"></asp:Label>
                                <a href="#" onclick='openWindow(500, 700, &#039;../../Utils/SelProdProj.aspx&#039;); return false;'>
                                    <img border="0" src="../../Images/Pesquisar.gif" /></a>
                                <asp:HiddenField ID="hdfIdProdProj" runat="server" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DescrProdProj") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cálculo Qtde" SortExpression="CalculoQtde">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCalcQtd" runat="server" Text='<%# Bind("CalculoQtde") %>' MaxLength="150"
                                    onpaste="return false;" onkeydown="return false;" onkeyup="return false;" Width="100px"></asp:TextBox>
                                <a href="#" onclick='txtCalcSel=FindControl(&#039;txtCalcQtd&#039;, &#039;input&#039;); openWindow(500, 700, &#039;../../Utils/SelExpressao.aspx?tipo=material&amp;idProjetoModelo=<%= Request["idProjetoModelo"] %>&amp;expr=&#039; + trocaSinalMais(FindControl(&#039;txtCalcQtd&#039;, &#039;input&#039;).value));'>
                                    <img border="0" src="../../Images/Pesquisar.gif" /></a>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCalcQtd" runat="server" MaxLength="150" onpaste="return false;" onkeydown="return false;" onkeyup="return false;"
                                    Width="100px"></asp:TextBox>
                                <a href="#" onclick='txtCalcSel=FindControl(&#039;txtCalcQtd&#039;, &#039;input&#039;); openWindow(500, 700, &#039;../../Utils/SelExpressao.aspx?tipo=material&amp;idProjetoModelo=<%= Request["idProjetoModelo"] %>&amp;expr=&#039; + trocaSinalMais(FindControl(&#039;txtCalcQtd&#039;, &#039;input&#039;).value));'>
                                    <img border="0" src="../../Images/Pesquisar.gif" /></a>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblCalculoQtde" runat="server" Text='<%# Bind("CalculoQtde") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cálculo Alt. (Alumínio)" SortExpression="CalculoAltura">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCalcMaterAlt" runat="server" Text='<%# Bind("CalculoAltura") %>'
                                    MaxLength="150" onpaste="return false;" onkeydown="return false;" onkeyup="return false;" Width="100px"></asp:TextBox>
                                <a href="#" onclick='txtCalcSel=FindControl(&#039;txtCalcMaterAlt&#039;, &#039;input&#039;); openWindow(500, 700, &#039;../../Utils/SelExpressao.aspx?tipo=material&amp;idProjetoModelo=<%= Request["idProjetoModelo"] %>&amp;expr=&#039; + trocaSinalMais(FindControl(&#039;txtCalcMaterAlt&#039;, &#039;input&#039;).value));'>
                                    <img border="0" src="../../Images/Pesquisar.gif" /></a>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCalcMaterAlt" runat="server" MaxLength="150" onpaste="return false;" onkeydown="return false;" onkeyup="return false;"
                                    Width="100px"></asp:TextBox>
                                <a href="#" onclick='txtCalcSel=FindControl(&#039;txtCalcMaterAlt&#039;, &#039;input&#039;); openWindow(500, 700, &#039;../../Utils/SelExpressao.aspx?tipo=material&amp;idProjetoModelo=<%= Request["idProjetoModelo"] %>&amp;expr=&#039; + trocaSinalMais(FindControl(&#039;txtCalcMaterAlt&#039;, &#039;input&#039;).value));'>
                                    <img border="0" src="../../Images/Pesquisar.gif" /></a>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("CalculoAltura") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Espessuras" SortExpression="Espessuras">
                            <EditItemTemplate>
                                <sync:CheckBoxListDropDown ID="drpEspessuras" runat="server" Title="Selecione a Espessura"
                                    SelectedValue='<%# Bind("Espessuras") %>'>
                                        <asp:ListItem Value="3" Text="03MM"></asp:ListItem>
                                        <asp:ListItem Value="4" Text="04MM"></asp:ListItem>
                                        <asp:ListItem Value="5" Text="05MM"></asp:ListItem>
                                        <asp:ListItem Value="6" Text="06MM"></asp:ListItem>
                                        <asp:ListItem Value="8" Text="08MM"></asp:ListItem>
                                        <asp:ListItem Value="10" Text="10MM"></asp:ListItem>
                                        <asp:ListItem Value="12" Text="12MM"></asp:ListItem>
                                </sync:CheckBoxListDropDown>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <sync:CheckBoxListDropDown ID="drpEspessuras" runat="server" Title="Selecione a Espessura"
                                    SelectedValue='<%# Bind("Espessuras") %>'>
                                        <asp:ListItem Value="3" Text="03MM"></asp:ListItem>
                                        <asp:ListItem Value="4" Text="04MM"></asp:ListItem>
                                        <asp:ListItem Value="5" Text="05MM"></asp:ListItem>
                                        <asp:ListItem Value="6" Text="06MM"></asp:ListItem>
                                        <asp:ListItem Value="8" Text="08MM"></asp:ListItem>
                                        <asp:ListItem Value="10" Text="10MM"></asp:ListItem>
                                        <asp:ListItem Value="12" Text="12MM"></asp:ListItem>
                                </sync:CheckBoxListDropDown>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblEspessuras" runat="server" Text='<%# Bind("DescricaoEspessura") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Grau" SortExpression="GrauCorte">
                            <ItemTemplate>
                                <asp:Label ID="lblGrauCorte" runat="server"
                                     Text='<%# Eval("GrauCorte") == null ? "" : Colosoft.Translator.Translate(Glass.Conversoes.StrParaEnum<Glass.Data.Model.GrauCorteEnum>(Eval("GrauCorte").ToString())).Format() %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpGrauCorte" runat="server" AppendDataBoundItems="true" DataSourceID="odsGrauCorte"
                                    DataTextField="Translation" DataValueField="Key" SelectedValue='<%# Bind("GrauCorte") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpGrauCorte" runat="server" AppendDataBoundItems="True" DataSourceID="odsGrauCorte"
                                    DataTextField="Translation" DataValueField="Key">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInsMaterial" runat="server" OnClick="lnkInsMaterial_Click">
                                    <img border="0" src="../../Images/insert.gif" /></asp:LinkButton>
                            </FooterTemplate>
                            <ItemTemplate>
                                <uc2:ctrlLogPopup ID="ctrlLogPopup2" runat="server" IdRegistro='<%# Eval("IdMaterProjMod") %>'
                                    Tabela="MaterialProjetoModelo" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="hdfIdProjetoModelo" runat="server" />
                <asp:HiddenField ID="hdfIdProd" runat="server" />
                <asp:HiddenField ID="hdfIdArquivoMesaCorte" runat="server" />
                <colo:virtualobjectdatasource culture="pt-BR" ID="odsProjetoModelo" runat="server" DataObjectTypeName="Glass.Data.Model.ProjetoModelo"
                    InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.ProjetoModeloDAO"
                    UpdateMethod="Update" OnInserted="odsProjetoModelo_Inserted" OnInserting="odsProjetoModelo_Inserting"
                    OnUpdated="odsProjetoModelo_Updated" OnUpdating="odsProjetoModelo_Updating">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idProjetoModelo" QueryStringField="idProjetoModelo"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:virtualobjectdatasource>
                <colo:virtualobjectdatasource culture="pt-BR" ID="odsGrupoModelo" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.GrupoModeloDAO"></colo:virtualobjectdatasource>
                <colo:virtualobjectdatasource culture="pt-BR" ID="odsMedidaProjeto" runat="server" SelectMethod="GetMedidas"
                    TypeName="Glass.Data.DAL.MedidaProjetoDAO"></colo:virtualobjectdatasource>
                <colo:virtualobjectdatasource culture="pt-BR" ID="odsPecaProjetoModelo" runat="server" DataObjectTypeName="Glass.Data.Model.PecaProjetoModelo"
                    DeleteMethod="Delete" SelectMethod="GetListIns" TypeName="Glass.Data.DAL.PecaProjetoModeloDAO"
                    UpdateMethod="Update" OnDeleted="odsPecaProjetoModelo_Deleted" OnUpdated="odsPecaProjetoModelo_Updated">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfIdProjetoModelo" Name="idProjetoModelo" PropertyName="Value"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:virtualobjectdatasource>
                <colo:virtualobjectdatasource culture="pt-BR" ID="odsMaterialProjetoModelo" runat="server" DataObjectTypeName="Glass.Data.Model.MaterialProjetoModelo"
                    DeleteMethod="Delete" InsertMethod="Insert" SelectMethod="GetListIns" TypeName="Glass.Data.DAL.MaterialProjetoModeloDAO"
                    UpdateMethod="Update" OnInserted="odsMaterialProjetoModelo_Inserted" OnUpdated="odsMaterialProjetoModelo_Updated">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfIdProjetoModelo" Name="idProjetoModelo" PropertyName="Value"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:virtualobjectdatasource>
                <colo:virtualobjectdatasource culture="pt-BR" ID="odsArquivoMesaCorte" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ArquivoMesaCorteDAO"></colo:virtualobjectdatasource>
                <colo:virtualobjectdatasource culture="pt-BR" ID="odsTipoArquivoMesaCorte" runat="server" 
                    SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.TipoArquivoMesaCorte, Glass.Data" />
                    </SelectParameters>
                </colo:virtualobjectdatasource>
                <colo:virtualobjectdatasource culture="pt-BR" ID="odsFlagArqMesa" runat="server" 
                    SelectMethod="ObtemFlagsArqMesaArqCalcengine" 
                    TypeName="Glass.Projeto.Negocios.IFlagArqMesaFluxo">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfIdArquivoMesaCorte" Name="idArquivoMesaCorte" PropertyName="Value" />
                    </SelectParameters>
                </colo:virtualobjectdatasource>
                <colo:virtualobjectdatasource culture="pt-BR" ID="odsGrauCorte" runat="server" 
                    SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.GrauCorteEnum, Glass.Data" />
                    </SelectParameters>
                </colo:virtualobjectdatasource>
                 <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCorVidro" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.CorVidroDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
