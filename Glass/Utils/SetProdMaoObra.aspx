<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetProdMaoObra.aspx.cs" Inherits="Glass.UI.Web.Utils.SetProdMaoObra"
    Title="Inserir várias peças de vidro com a mesma mão de obra" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        var produtoAmbiente = 0;

        function validate() {
            // Garante que há uma mão de obra selecionada
            if (FindControl("hdfIdProdMaoObra", "input").value == "") {
                alert("Escolha uma mão de obra.");
                return false;
            }
            else {
                var qtde = parseInt(FindControl("txtQtdeMaoObra", "input").value, 10);
                if (isNaN(qtde) || qtde <= 0) {
                    alert("Escolha a quantidade da mão de obra.");
                    return false;
                }

                var valor = parseFloat(FindControl("txtValorUnitMaoObra", "input").value.replace(",", "."));
                if (isNaN(valor) || valor <= 0) {
                    alert("Digite o valor vendido da mão de obra.");
                    return false;
                }
            }

            var inserir = false;

            // Garante que há pelo menos 1 peça de vidro e que todas as informações das
            // peças de vidro informadas estejam preenchidas
            for (i = 1; i <= 10; i++) {
                var idProd = parseInt(FindControl("hdfAmbIdProd" + i, "input").value, 10);
                if (!isNaN(idProd) && idProd > 0) {
                    inserir = true;
                    var qtde = parseInt(FindControl("txtQtde" + i, "input").value, 10);
                    if (isNaN(qtde) || qtde <= 0) {
                        alert("Digite a quantidade da peça de vidro " + i + ".");
                        return false;
                    }

                    var altura = parseInt(FindControl("txtAltura" + i, "input").value, 10);
                    if (isNaN(altura) || altura <= 0) {
                        alert("Digite a altura da peça de vidro " + i + ".");
                        return false;
                    }

                    var largura = parseInt(FindControl("txtLargura" + i, "input").value, 10);
                    if (isNaN(largura) || largura <= 0) {
                        alert("Digite a largura da peça de vidro " + i + ".");
                        return false;
                    }
                }
            }

            if (!inserir) {
                alert("Escolha pelo menos 1 peça de vidro.");
                return false;
            }

            if (!confirm("Inserir produtos?"))
                return false;

            FindControl("txtValorUnitMaoObra", "input").disabled = false;
        }

        function getProduto() {
            openWindow(450, 700, '../Utils/SelProd.aspx?IdPedido=<%= Request["IdPedido"] %>' + (produtoAmbiente ? "&ambiente=true" : ""));
        }

        // Função chamada após selecionar produto pelo popup
        function setProduto(codInterno) {
            try {
                if (produtoAmbiente > 0)
                    FindControl("txtCodAmb" + produtoAmbiente, "input").value = codInterno;
                else
                    FindControl("txtCodProd", "input").value = codInterno;

                loadProduto(codInterno);
            }
            catch (err) {

            }
        }

        // Retorna o percentual de comissão
        function getPercComissao() {
            var percComissao = 0;

            if (FindControl("hdfPercComissao", "input") != null)
                percComissao = parseFloat(FindControl("hdfPercComissao", "input").value.replace(',', '.'));

            return percComissao != null ? percComissao : 0;
        }

        // Carrega dados do produto com base no código do produto passado
        function loadProduto(codInterno) {
            if (codInterno == "") {
                if (produtoAmbiente > 0) {
                    FindControl("hdfAmbIdProd" + produtoAmbiente, "input").value = "";
                    FindControl("lblDescrAmb" + produtoAmbiente, "span").innerHTML = "";
                    FindControl("hdfDescrAmb" + produtoAmbiente, "input").value = "";
                }

                return false;
            }

            try {
                var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
                var cliRevenda = FindControl("hdfCliRevenda", "input").value;
                var idCliente = FindControl("hdfIdCliente", "input").value;
                var reposicao = FindControl("hdfIsReposicao", "input").value;
                var pedidoMaoDeObra = true;
                var percComissao = getPercComissao();

                var retorno = SetProdMaoObra.GetProduto(codInterno, tipoEntrega, cliRevenda, reposicao, idCliente, percComissao == null ? 0 : percComissao.toString().replace('.', ','), produtoAmbiente > 0).value.split(';');

                if (retorno[0] == "Erro") {
                    alert(retorno[1]);
                    if (!produtoAmbiente)
                        FindControl("txtCodProd", "input").value = "";
                    else
                        FindControl("txtCodAmb", "input").value = "";

                    return false;
                }

                else if (produtoAmbiente == 0) {
                    if (retorno[0] == "Prod") {
                        FindControl("hdfIdProdMaoObra", "input").value = retorno[1];
                        FindControl("txtValorUnitMaoObra", "input").value = retorno[3]; // Exibe no cadastro o valor mínimo do produto
                        FindControl("hdfTipoCalcMaoObra", "input").value = retorno[4]; // Verifica como deve ser calculado o produto
                    }

                    FindControl("lblDescrMaoObra", "span").innerHTML = retorno[2];
                }
                else {
                    FindControl("hdfAmbIdProd" + produtoAmbiente, "input").value = retorno[1];
                    FindControl("lblDescrAmb" + produtoAmbiente, "span").innerHTML = retorno[2];
                    FindControl("hdfDescrAmb" + produtoAmbiente, "input").value = retorno[2];
                }
            }
            catch (err) {
                alert(err);
            }

            produtoAmbiente = 0;
        }

        // Função chamada pelo popup de escolha da Aplicação do produto
        function setApl(idAplicacao, codInterno) {
            FindControl("txtAplIns" + produtoAmbiente, "input").value = codInterno;
            FindControl("hdfIdAplicacao" + produtoAmbiente, "input").value = idAplicacao;
            produtoAmbiente = 0;
        }

        function loadApl(codInterno) {
            if (codInterno == undefined || codInterno == "") {
                setApl("", "");
                return false;
            }

            try {
                var response = MetodosAjax.GetEtiqAplicacao(codInterno).value;

                if (response == null || response == "") {
                    alert("Falha ao buscar Aplicação. Ajax Error.");
                    setApl("", "");
                    return false
                }

                response = response.split("\t");

                if (response[0] == "Erro") {
                    alert(response[1]);
                    setApl("", "");
                    return false;
                }

                setApl(response[1], response[2]);
            }
            catch (err) {
                alert(err);
            }
        }

        // Função chamada pelo popup de escolha do Processo do produto
        function setProc(idProcesso, codInterno, codAplicacao) {
            FindControl("txtProcIns" + produtoAmbiente, "input").value = codInterno;
            FindControl("hdfIdProcesso" + produtoAmbiente, "input").value = idProcesso;

            if (codAplicacao != "")
                loadApl(codAplicacao, produtoAmbiente);

            produtoAmbiente = 0;
        }

        function loadProc(codInterno) {
            if (codInterno == "") {
                setProc("", "", "");
                return false;
            }

            try {
                var response = MetodosAjax.GetEtiqProcesso(codInterno).value;

                if (response == null || response == "") {
                    alert("Falha ao buscar Processo. Ajax Error.");
                    setProc("", "");
                    return false
                }

                response = response.split("\t");

                if (response[0] == "Erro") {
                    alert(response[1]);
                    setProc("", "", "");
                    return false;
                }

                setProc(response[1], response[2], response[3]);
            }
            catch (err) {
                alert(err);
            }
        }
    </script>

    <table cellpadding="0" cellspacing="0" >
        <tr>
            <td align="center">
                <table cellpadding="4" cellspacing="0">
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td style="font-weight: bold">Mão-de-obra:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCodProd" runat="server" onblur="loadProduto(this.value);" onkeydown="if (isEnter(event)) loadProduto(this.value);"
                                            onkeypress="return !(isEnter(event));" Width="50px"></asp:TextBox>
                                        <asp:Label ID="lblDescrMaoObra" runat="server"></asp:Label>
                                        <a href="#" onclick="getProduto(); return false;" tabindex="-1">
                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                    </td>
                                    <td style="font-weight: bold; padding-left: 8px">Qtde
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtdeMaoObra" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td style="font-weight: bold; padding-left: 8px">Valor Vendido
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtValorUnitMaoObra" runat="server" onkeypress="return soNumeros(event, false, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                    <td style="font-weight: bold; padding-left: 8px; text-align: center">
                                        <span style="position: relative; top: 6px">Altura, largura e espessura<br />
                                            <span style="font-weight: normal; font-size: 85%">(usado para ML)</span> </span>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:DropDownList ID="drpAltBenef" runat="server">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem>0</asp:ListItem>
                                            <asp:ListItem>1</asp:ListItem>
                                            <asp:ListItem>2</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:DropDownList ID="drpLargBenef" runat="server">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem>0</asp:ListItem>
                                            <asp:ListItem>1</asp:ListItem>
                                            <asp:ListItem>2</asp:ListItem>
                                        </asp:DropDownList>
                                        Esp.:
                                        <asp:TextBox ID="txtEspBenef" Width="30px" runat="server" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <table class="gridStyle" cellpadding="1" cellspacing="0" rules="all" id="tbProdutos">
                                <tr>
                                    <td nowrap="nowrap">Peça de vidro 1:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCodAmb1" runat="server" onblur="produtoAmbiente=1; loadProduto(this.value);"
                                            onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                            Width="50px"></asp:TextBox>
                                        <a href="#" onclick="produtoAmbiente=1; getProduto(); return false;" tabindex="-1">
                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        <br />
                                        <asp:Label ID="lblDescrAmb1" runat="server"></asp:Label>
                                        <asp:HiddenField ID="hdfAmbIdProd1" runat="server" />
                                        <asp:HiddenField ID="hdfDescrAmb1" runat="server" />
                                    </td>
                                    <td>Qtde:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtde1" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="30px"></asp:TextBox>
                                    </td>
                                    <td>Altura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAltura1" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                    <td>Largura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLargura1" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>

                                    <td>Proc.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtProcIns1" runat="server" onblur="produtoAmbiente=1; loadProc(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=1; loadProc(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=1; openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdProcesso1" runat="server" />
                                    </td>
                                    <td>Apl.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtAplIns1" runat="server" onblur="produtoAmbiente=1; loadApl(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=1; loadApl(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=1; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdAplicacao1" runat="server" />
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:CheckBox ID="chkRedondo1" runat="server" Text="Redondo" />
                                    </td>
                                </tr>
                                <tr class="alt">
                                    <td nowrap="nowrap">Peça de vidro 2:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCodAmb2" runat="server" onblur="produtoAmbiente=2; loadProduto(this.value);"
                                            onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                            Width="50px"></asp:TextBox>
                                        <a href="#" onclick="produtoAmbiente=2; getProduto(); return false;" tabindex="-1">
                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        <br />
                                        <asp:Label ID="lblDescrAmb2" runat="server"></asp:Label>
                                        <asp:HiddenField ID="hdfAmbIdProd2" runat="server" />
                                        <asp:HiddenField ID="hdfDescrAmb2" runat="server" />
                                    </td>
                                    <td>Qtde:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtde2" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="30px"></asp:TextBox>
                                    </td>
                                    <td>Altura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAltura2" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                    <td>Largura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLargura2" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>

                                    <td>Proc.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtProcIns2" runat="server" onblur="produtoAmbiente=2; loadProc(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=2; loadProc(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=2; openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdProcesso2" runat="server" />
                                    </td>
                                    <td>Apl.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtAplIns2" runat="server" onblur="produtoAmbiente=2; loadApl(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=2; loadApl(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=2; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdAplicacao2" runat="server" />
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:CheckBox ID="chkRedondo2" runat="server" Text="Redondo" />
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap">Peça de vidro 3:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCodAmb3" runat="server" onblur="produtoAmbiente=3; loadProduto(this.value);"
                                            onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                            Width="50px"></asp:TextBox>
                                        <a href="#" onclick="produtoAmbiente=3; getProduto(); return false;" tabindex="-1">
                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        <br />
                                        <asp:Label ID="lblDescrAmb3" runat="server"></asp:Label>
                                        <asp:HiddenField ID="hdfAmbIdProd3" runat="server" />
                                        <asp:HiddenField ID="hdfDescrAmb3" runat="server" />
                                    </td>
                                    <td>Qtde:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtde3" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="30px"></asp:TextBox>
                                    </td>
                                    <td>Altura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAltura3" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                    <td>Largura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLargura3" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>

                                    <td>Proc.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtProcIns3" runat="server" onblur="produtoAmbiente=3; loadProc(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=3; loadProc(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=3; openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdProcesso3" runat="server" />
                                    </td>
                                    <td>Apl.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtAplIns3" runat="server" onblur="produtoAmbiente=3; loadApl(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=3; loadApl(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=3; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdAplicacao3" runat="server" />
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:CheckBox ID="chkRedondo3" runat="server" Text="Redondo" />
                                    </td>
                                </tr>
                                <tr class="alt">
                                    <td nowrap="nowrap">Peça de vidro 4:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCodAmb4" runat="server" onblur="produtoAmbiente=4; loadProduto(this.value);"
                                            onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                            Width="50px"></asp:TextBox>
                                        <a href="#" onclick="produtoAmbiente=4; getProduto(); return false;" tabindex="-1">
                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        <br />
                                        <asp:Label ID="lblDescrAmb4" runat="server"></asp:Label>
                                        <asp:HiddenField ID="hdfAmbIdProd4" runat="server" />
                                        <asp:HiddenField ID="hdfDescrAmb4" runat="server" />
                                    </td>
                                    <td>Qtde:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtde4" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="30px"></asp:TextBox>
                                    </td>
                                    <td>Altura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAltura4" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                    <td>Largura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLargura4" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>

                                    <td>Proc.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtProcIns4" runat="server" onblur="produtoAmbiente=4; loadProc(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=4; loadProc(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=4; openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdProcesso4" runat="server" />
                                    </td>
                                    <td>Apl.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtAplIns4" runat="server" onblur="produtoAmbiente=4; loadApl(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=4; loadApl(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=4; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdAplicacao4" runat="server" />
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:CheckBox ID="chkRedondo4" runat="server" Text="Redondo" />
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap">Peça de vidro 5:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCodAmb5" runat="server" onblur="produtoAmbiente=5; loadProduto(this.value);"
                                            onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                            Width="50px"></asp:TextBox>
                                        <a href="#" onclick="produtoAmbiente=5; getProduto(); return false;" tabindex="-1">
                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        <br />
                                        <asp:Label ID="lblDescrAmb5" runat="server"></asp:Label>
                                        <asp:HiddenField ID="hdfAmbIdProd5" runat="server" />
                                        <asp:HiddenField ID="hdfDescrAmb5" runat="server" />
                                    </td>
                                    <td>Qtde:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtde5" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="30px"></asp:TextBox>
                                    </td>
                                    <td>Altura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAltura5" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                    <td>Largura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLargura5" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>

                                    <td>Proc.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtProcIns5" runat="server" onblur="produtoAmbiente=5; loadProc(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=5; loadProc(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=5; openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdProcesso5" runat="server" />
                                    </td>
                                    <td>Apl.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtAplIns5" runat="server" onblur="produtoAmbiente=5; loadApl(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=5; loadApl(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=5; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdAplicacao5" runat="server" />
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:CheckBox ID="chkRedondo5" runat="server" Text="Redondo" />
                                    </td>
                                </tr>
                                <tr class="alt">
                                    <td nowrap="nowrap">Peça de vidro 6:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCodAmb6" runat="server" onblur="produtoAmbiente=6; loadProduto(this.value);"
                                            onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                            Width="50px"></asp:TextBox>
                                        <a href="#" onclick="produtoAmbiente=6; getProduto(); return false;" tabindex="-1">
                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        <br />
                                        <asp:Label ID="lblDescrAmb6" runat="server"></asp:Label>
                                        <asp:HiddenField ID="hdfAmbIdProd6" runat="server" />
                                        <asp:HiddenField ID="hdfDescrAmb6" runat="server" />
                                    </td>
                                    <td>Qtde:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtde6" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="30px"></asp:TextBox>
                                    </td>
                                    <td>Altura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAltura6" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                    <td>Largura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLargura6" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>

                                    <td>Proc.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtProcIns6" runat="server" onblur="produtoAmbiente=6; loadProc(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=6; loadProc(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=6; openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdProcesso6" runat="server" />
                                    </td>
                                    <td>Apl.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtAplIns6" runat="server" onblur="produtoAmbiente=6; loadApl(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=6; loadApl(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=6; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdAplicacao6" runat="server" />
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:CheckBox ID="chkRedondo6" runat="server" Text="Redondo" />
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap">Peça de vidro 7:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCodAmb7" runat="server" onblur="produtoAmbiente=7; loadProduto(this.value);"
                                            onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                            Width="50px"></asp:TextBox>
                                        <a href="#" onclick="produtoAmbiente=7; getProduto(); return false;" tabindex="-1">
                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        <br />
                                        <asp:Label ID="lblDescrAmb7" runat="server"></asp:Label>
                                        <asp:HiddenField ID="hdfAmbIdProd7" runat="server" />
                                        <asp:HiddenField ID="hdfDescrAmb7" runat="server" />
                                    </td>
                                    <td>Qtde:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtde7" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="30px"></asp:TextBox>
                                    </td>
                                    <td>Altura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAltura7" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                    <td>Largura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLargura7" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>

                                    <td>Proc.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtProcIns7" runat="server" onblur="produtoAmbiente=7; loadProc(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=7; loadProc(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=7; openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdProcesso7" runat="server" />
                                    </td>
                                    <td>Apl.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtAplIns7" runat="server" onblur="produtoAmbiente=7; loadApl(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=7; loadApl(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=7; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdAplicacao7" runat="server" />
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:CheckBox ID="chkRedondo7" runat="server" Text="Redondo" />
                                    </td>
                                </tr>
                                <tr class="alt">
                                    <td nowrap="nowrap">Peça de vidro 8:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCodAmb8" runat="server" onblur="produtoAmbiente=8; loadProduto(this.value);"
                                            onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                            Width="50px"></asp:TextBox>
                                        <a href="#" onclick="produtoAmbiente=8; getProduto(); return false;" tabindex="-1">
                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        <br />
                                        <asp:Label ID="lblDescrAmb8" runat="server"></asp:Label>
                                        <asp:HiddenField ID="hdfAmbIdProd8" runat="server" />
                                        <asp:HiddenField ID="hdfDescrAmb8" runat="server" />
                                    </td>
                                    <td>Qtde:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtde8" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="30px"></asp:TextBox>
                                    </td>
                                    <td>Altura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAltura8" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                    <td>Largura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLargura8" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>

                                    <td>Proc.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtProcIns8" runat="server" onblur="produtoAmbiente=8; loadProc(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=8; loadProc(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=8; openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdProcesso8" runat="server" />
                                    </td>
                                    <td>Apl.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtAplIns8" runat="server" onblur="produtoAmbiente=8; loadApl(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=8; loadApl(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=8; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdAplicacao8" runat="server" />
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:CheckBox ID="chkRedondo8" runat="server" Text="Redondo" />
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap">Peça de vidro 9:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCodAmb9" runat="server" onblur="produtoAmbiente=9; loadProduto(this.value);"
                                            onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                            Width="50px"></asp:TextBox>
                                        <a href="#" onclick="produtoAmbiente=9; getProduto(); return false;" tabindex="-1">
                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        <br />
                                        <asp:Label ID="lblDescrAmb9" runat="server"></asp:Label>
                                        <asp:HiddenField ID="hdfAmbIdProd9" runat="server" />
                                        <asp:HiddenField ID="hdfDescrAmb9" runat="server" />
                                    </td>
                                    <td>Qtde:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtde9" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="30px"></asp:TextBox>
                                    </td>
                                    <td>Altura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAltura9" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                    <td>Largura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLargura9" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>

                                    <td>Proc.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtProcIns9" runat="server" onblur="produtoAmbiente=9; loadProc(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=9; loadProc(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=9; openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdProcesso9" runat="server" />
                                    </td>
                                    <td>Apl.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtAplIns9" runat="server" onblur="produtoAmbiente=9; loadApl(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=9; loadApl(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=9; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdAplicacao9" runat="server" />
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:CheckBox ID="chkRedondo9" runat="server" Text="Redondo" />
                                    </td>
                                </tr>
                                <tr class="alt">
                                    <td nowrap="nowrap">Peça de vidro 10:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCodAmb10" runat="server" onblur="produtoAmbiente=10; loadProduto(this.value);"
                                            onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                            Width="50px"></asp:TextBox>
                                        <a href="#" onclick="produtoAmbiente=10; getProduto(); return false;" tabindex="-1">
                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        <br />
                                        <asp:Label ID="lblDescrAmb10" runat="server"></asp:Label>
                                        <asp:HiddenField ID="hdfAmbIdProd10" runat="server" />
                                        <asp:HiddenField ID="hdfDescrAmb10" runat="server" />
                                    </td>
                                    <td>Qtde:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtde10" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="30px"></asp:TextBox>
                                    </td>
                                    <td>Altura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAltura10" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                    <td>Largura:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLargura10" runat="server" onkeypress="return soNumeros(event, true, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>

                                    <td>Proc.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtProcIns10" runat="server" onblur="produtoAmbiente=10; loadProc(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=10; loadProc(this.value) }"
                                                        onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=10; openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdProcesso10" runat="server" />
                                    </td>
                                    <td>Apl.:
                                    </td>
                                    <td>
                                        <table class="pos">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtAplIns10" runat="server" onblur="produtoAmbiente=10; loadApl(this.value);"
                                                        onkeydown="if (isEnter(event)) { produtoAmbiente=10; loadApl(this.value) }" onkeypress="return !(isEnter(event));"
                                                        Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <a href="#" onclick="produtoAmbiente=10; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdfIdAplicacao10" runat="server" />
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:CheckBox ID="chkRedondo10" runat="server" Text="Redondo" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" OnClick="btnConfirmar_Click"
                    OnClientClick="return validate();" Style="margin: 4px" />
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        document.getElementById("<%= txtCodProd.ClientID %>").focus();

        var usarAltLarg = '<%= !Glass.Configuracoes.PedidoConfig.EmpresaTrabalhaAlturaLargura %>'.toLowerCase();

        // Troca a posição da altura com a largura
        if (usarAltLarg == "true") {

            var tbProds = FindControl("tbProdutos", "table");

            if (tbProds != null) {

                for (var i = 0; i < tbProds.rows.length; i++) {
                    var row = tbProds.rows[i];

                    var lblAlt = row.cells[4].innerHTML;
                    var txtAlt = row.cells[5].innerHTML;

                    row.cells[4].innerHTML = row.cells[6].innerHTML;
                    row.cells[5].innerHTML = row.cells[7].innerHTML;

                    row.cells[6].innerHTML = lblAlt;
                    row.cells[7].innerHTML = txtAlt;
                }
            }
        }

    </script>

    <asp:HiddenField ID="hdfIdProdMaoObra" runat="server" />
    <asp:HiddenField ID="hdfTipoCalcMaoObra" runat="server" />
    <asp:HiddenField ID="hdfTipoEntrega" runat="server" />
    <asp:HiddenField ID="hdfCliRevenda" runat="server" />
    <asp:HiddenField ID="hdfIdCliente" runat="server" />
    <asp:HiddenField ID="hdfPercComissao" runat="server" />
    <asp:HiddenField ID="hdfIsReposicao" runat="server" />
</asp:Content>
