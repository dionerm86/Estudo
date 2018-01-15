<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelExpressao.aspx.cs" Inherits="Glass.UI.Web.Utils.SelExpressao"
    Title="Expressão de Cálculo" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function setValue(control) {
            var txtExpressao = FindControl("txtExpressao", "textarea");
            var espressao =
                control.innerText == "SENO" ? "SENO( )" :
                control.innerText == "SECANTE" ? "SECANTE( )" :
                control.innerText == "COSSENO" ? "COSSENO( )" :
                control.innerText == "COSSECANTE" ? "COSSECANTE( )" :
                control.innerText == "TANGENTE" ? "TANGENTE( )" :
                control.innerText == "COTANGENTE" ? "COTANGENTE( )" :
                control.innerText == "CEILING" ? "CEILING( )" :
                control.innerText == "FLOOR" ? "FLOOR( )" :
                control.innerText == "RAIZ" ? "RAIZ( )" :
                control.innerText == "MULT" ? "MULT( )" :
                control.innerText == "IF" ? "IF( ; ; )" :
                control.innerText == "MAX" ? "MAX( ; )" :
                control.innerText == "MIN" ? "MIN( ; )" :
                control.innerText == "OR" ? " OR " :
                control.innerText == "AND" ? " AND " :
                control.innerText == ">" ? " > " :
                control.innerText == ">=" ? " >= " :
                control.innerText == "<" ? " < " :
                control.innerText == "<=" ? " <= " : control.innerText;
            var texto = txtExpressao.value;
            var inicio = txtExpressao.selectionEnd < texto.length ? txtExpressao.selectionStart + espressao.length : -1;
            texto = texto.substr(0, txtExpressao.selectionStart) + espressao +
            texto.substr(txtExpressao.selectionEnd);

            txtExpressao.value = texto;
            txtExpressao.focus();

            if (inicio > -1) {
                txtExpressao.selectionStart = inicio;
                txtExpressao.selectionEnd = inicio;
            }
        }

        function setExpressao() {

            var expressao = FindControl("txtExpressao", "textarea").value;
            var idProjetoModelo = <%= Request["idProjetoModelo"] != null ? Request["idProjetoModelo"] : "0" %>;
            var idFormulaExpreCalc = <%= Request["idFormulaExp"] != null ? Request["idFormulaExp"] : "0" %>;
            while(expressao.indexOf(" ") > -1)
                expressao = expressao.replace(" ", "");

            var validacaoExpressao = SelExpressao.ValidarExpressao(idProjetoModelo, idFormulaExpreCalc, expressao);
            
            if(validacaoExpressao.error != null){
                alertaPadrao("Etiqueta", camposMateriaPrima.error.description, 'erro', 280, 600);
                return false;
            }
            else if (validacaoExpressao.value.split('|')[0] == "Erro") {
                alert(validacaoExpressao.value.split('|')[1]);
                return false;
            }

            var item = GetQueryString("item");
            var primeiraExpressao = GetQueryString("primeiraExpressao");
            var segundaExpressao = GetQueryString("segundaExpressao");
            var pecaProjMod = GetQueryString("idPecaProjMod");

            if (expressao.indexOf("IETQ") != -1 &&
                item != undefined &&
                item != "90" && item != "91" && item != "92" && item != "93" && item != "94" && item != "95" && item != "96" && item != "97" && item != "98" && item != "99") {
                alert("A expressão Item da Etiqueta (IETQ) so pode ser usada para peças do modelo de projeto e item entre 90 e 99.");
                return false;
            }

            if (primeiraExpressao == "true") {
                window.opener.setPrimeiraExpressao(expressao.toString().toUpperCase());
            }
            else if (segundaExpressao == "true") {
                window.opener.setSegundaExpressao(expressao.toString().toUpperCase());
            }
            else
                window.opener.setExpressao(expressao.toString().toUpperCase());

            closeWindow();
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:Label ID="Label1" runat="server" Text="Monte a expressão considerando os seguintes termos/símbolos:"
                    Font-Size="Small"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center" id="itensFixos" runat="server">
                <table>
                    <tr>
                        <td align="center">
                            <a href='#' onclick='setValue(this);'>+</a> Soma, 
                            <a href='#' onclick='setValue(this);'>-</a> Subtração, 
                            <a href='#' onclick='setValue(this);'>*</a> Multiplicação,
                            <a href='#' onclick='setValue(this);'>/</a> Divisão, 
                            <a href='#' onclick='setValue(this);'>( </a>
                            <a href='#' onclick='setValue(this);'>)</a> Parênteses, 
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <a href='#' onclick='setValue(this);'>SENO</a> Calcula seno, 
                            <a href="#" onclick='setValue(this);'>COSSENO</a> Calcula cosseno,
                            <a href='#' onclick='setValue(this);'>TANGENTE</a> Calcula tangente,
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <a href='#' onclick='setValue(this);'>SECANTE</a> Calcula secante, 
                            <a href='#' onclick='setValue(this);'>COSSECANTE</a> Calcula cossecante, 
                            <a href='#' onclick='setValue(this);'>COTANGENTE</a> Calcula cotangente,
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <a href='#' onclick='setValue(this);'>CEILING</a> Arredonda para cima, 
                            <a href="#" onclick='setValue(this);'>FLOOR</a> Arredonda para baixo,
                            <a href='#' onclick='setValue(this);'>RAIZ</a> Raíz Quadrada, 
                            <a href='#' onclick='setValue(this);'>MULT</a> Múltiplo,
                            <a href='#' onclick='setValue(this);'>IF</a> Condicional,
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <a href='#' onclick='setValue(this);'>AND</a> E,
                            <a href='#' onclick='setValue(this);'>OR</a> Ou,
                            <a href='#' onclick='setValue(this);'>MAX</a> Valor máximo entre duas variáveis,
                            <a href='#' onclick='setValue(this);'>MIN</a> Valor mínimo entre duas variáveis,
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <a href='#' onclick='setValue(this);'>></a> Maior que,
                            <a href='#' onclick='setValue(this);'>>=</a> Maior ou igual,
                            <a href='#' onclick='setValue(this);'><</a> Menor que,
                            <a href='#' onclick='setValue(this);'><=</a> Menor ou igual
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblVariaveis" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblMedidasPecas" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblFormulaExpressaoCalculo" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblFolgasPecas" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:TextBox ID="txtExpressao" runat="server" Width="500px" onkeyup="this.value = this.value.toUpperCase()"
                    Rows="5" TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td align="center">&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClientClick="setExpressao();" />
            </td>
        </tr>
    </table>
</asp:Content>
