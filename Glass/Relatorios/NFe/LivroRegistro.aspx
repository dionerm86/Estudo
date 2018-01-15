<%@ Page Title="Livro de Registro" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LivroRegistro.aspx.cs" Inherits="Glass.UI.Web.Relatorios.NFe.LivroRegistro" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        var data;

        function openRpt() {
            var tipo = FindControl("hdfTipo", "input").value;
            if (validar()) {

                var rel = openWindow(600, 800, "RelBase.aspx?postData=getPostData()");
                data = new Object();
                data["rel"] = "LivroRegistro";
                data["loja"] = FindControl("drpLoja", "select").value;
                data["mes"] = FindControl("ddlMes", "select").value;
                data["ano"] = FindControl("ddlAno", "select").value;
                data["ultimoLancamento"] = FindControl("txtUltimoLancamento_txtData", "input").value;
                data["numeroOrdem"] = FindControl("txtNumeroOrdem", "input").value;
                data["localData"] = FindControl("txtLocalData", "input").value;
                data["termo"] = FindControl("rbJucemg", "input").checked == true ? "jucemg" : FindControl("rbSef", "input").checked == true ? "sef" : "";
                data["contador"] = FindControl("ddlContador", "select").value;
                data["funcionario"] = FindControl("ddlFuncionario", "select").value;
                data["tipo"] = FindControl("hdfTipo", "input").value;
                data["exibeST"] = FindControl("cboExibirST", "input").checked;
                data["exibeIPI"] = FindControl("cboExibirIPI", "input").checked;

                if (tipo == 3) {
                    data["outrosDebitosDesc"] = FindControl("txtOutrosDebitosDesc", "input").value;
                    data["outrosDebitosValor"] = FindControl("txtOutrosDebitosValor", "input").value;
                    data["estornoCreditoDesc"] = FindControl("txtEstornoCreditoDesc", "input").value;
                    data["estornoCreditoValor"] = FindControl("txtEstornoCreditoValor", "input").value;
                    data["outrosCreditosDesc"] = FindControl("txtOutrosCreditosDesc", "input").value;
                    data["outrosCreditosValor"] = FindControl("txtOutrosCreditosValor", "input").value;
                    data["estornoDebitoDesc"] = FindControl("txtEstornoDebitoDesc", "input").value;
                    data["estornoDebitoValor"] = FindControl("txtEstornoDebitoValor", "input").value;
                    data["deducaoDesc"] = FindControl("txtDeducaoDesc", "input").value;
                    data["deducaoValor"] = FindControl("txtDeducaoValor", "input").value;
                    data["saldoCredor"] = FindControl("txtSaldoCredor", "input").value;
                }
            }
        }

        function getPostData() {
            return data;
        }

        function validar() {
            var tipo = FindControl("hdfTipo", "input").value;
            var loja = FindControl("drpLoja", "select").value;
            var mes = FindControl("ddlMes", "select").value;
            var ano = FindControl("ddlAno", "select").value;
            var ultimoLancamento = FindControl("txtUltimoLancamento_txtData", "input").value;
            var numeroOrdem = FindControl("txtNumeroOrdem", "input").value;
            var localData = FindControl("txtLocalData", "input").value;
            var termo = FindControl("rbJucemg", "input").checked == true ? "jucemg" : FindControl("rbSef", "input").checked == true ? "sef" : "";
            var contador = FindControl("ddlContador", "select").value;
            var funcionario = FindControl("ddlFuncionario", "select").value;

            if (loja == "0") {
                alert("Selecione uma loja.");
                return false;
            }
            if (mes == "0" || ano == "0") {
                alert("Informe um período válido.");
                return false;
            }
            if (numeroOrdem == "") {
                alert("Informe o número de ordem.");
                return false;
            }
            if (localData == "") {
                alert("Informe Local e data.");
                return false;
            }
            if (ultimoLancamento == "") {
                alert("Informe a data do último lançamento.");
                return false;
            }
            if (termo == "") {
                alert("Selecione o tipo de termo abertura/encerramento.");
                return false;
            }
            if (funcionario == "0") {
                alert("Selecione um responsável.");
                return false;
            }
            if (contador == "0") {
                alert("Selecione um contador.");
                return false;
            }
            /*if (tipo == 3) {
                var outrosDebitosDesc = FindControl("txtOutrosDebitosDesc", "input").value;
                var outrosDebitosValor = FindControl("txtOutrosDebitosValor", "input").value;
                var estornoCreditoDesc = FindControl("txtEstornoCreditoDesc", "input").value;
                var estornoCreditoValor = FindControl("txtEstornoCreditoValor", "input").value;
                var outrosCreditosDesc = FindControl("txtOutrosCreditosDesc", "input").value;
                var outrosCreditosValor = FindControl("txtOutrosCreditosValor", "input").value;
                var estornoDebitoDesc = FindControl("txtEstornoDebitoDesc", "input").value;
                var estornoDebitoValor = FindControl("txtEstornoDebitoValor", "input").value;
                var deducaoDesc = FindControl("txtDeducaoDesc", "input").value;
                var deducaoValor = FindControl("txtDeducaoValor", "input").value;

                if (outrosDebitosDesc == "") {
                    alert("Informe uma descrição para \"Outros Débitos\".");
                    return false;
                }
                if (outrosDebitosValor == "") {
                    alert("Informe um valor para \"Outros Débitos\".");
                    return false;
                }
                if (estornoCreditoDesc == "") {
                    alert("Informe uma descrição para \"Estorno Crédito\".");
                    return false;
                }
                if (estornoCreditoValor == "") {
                    alert("Informe um valor para \"Estorno Crédito\".");
                    return false;
                }
                if (outrosCreditosDesc == "") {
                    alert("Informe uma descrição para \"Outros Créditos\".");
                    return false;
                }
                if (outrosCreditosValor == "") {
                    alert("Informe um valor para \"Outros Créditos\".");
                    return false;
                }
                if (estornoDebitoDesc == "") {
                    alert("Informe uma descrição para \"Estorno Débito\".");
                    return false;
                }
                if (estornoDebitoValor == "") {
                    alert("Informe um valor para \"Estorno Débito\".");
                    return false;
                }
                if (deducaoDesc == "") {
                    alert("Informe uma descrição para \"Deduções\".");
                    return false;
                }
                if (deducaoValor == "") {
                    alert("Informe um valor para \"Deduções\".");
                    return false;
                }
            }*/

            return true;
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table style="margin-bottom: 15px">
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Selecione</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlMes" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlAno" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table style="margin-bottom: 15px">
                    <tr>
                        <td align="center">
                            <table style="margin-bottom: 15px">
                                <tr>
                                    <td>
                                        <asp:Label ID="Label3" runat="server" Text="Número de Ordem" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtNumeroOrdem" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label4" runat="server" Text="Local/Data" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLocalData" runat="server" Style="width: 300px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label2" runat="server" Text="Último Laçamento" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="txtUltimoLancamento" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                </tr>
                            </table>
                            <table style="margin-bottom: 15px">
                                <tr>
                                    <td>
                                        <asp:Label ID="Label1" runat="server" Text="Termos Abertura/Encerramento" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td style="padding-left: 10px">
                                        <asp:RadioButton ID="rbJucemg" runat="server" Text="JUCEMG" GroupName="termo" />
                                    </td>
                                    <td style="padding-left: 10px">
                                        <asp:RadioButton ID="rbSef" runat="server" Text="SEF" GroupName="termo" />
                                    </td>
                                </tr>
                            </table>
                            <table style="margin-bottom: 15px">
                                <tr>
                                    <td>
                                        <asp:Label ID="Label5" runat="server" Text="Responsável" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td style="padding-left: 10px">
                                        <asp:DropDownList ID="ddlFuncionario" runat="server" AppendDataBoundItems="True"
                                            DataSourceID="odsFuncionario" DataTextField="Nome" DataValueField="IdFunc">
                                            <asp:ListItem Value="0">Selecione</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label6" runat="server" Text="Contador" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td style="padding-left: 10px">
                                        <asp:DropDownList ID="ddlContador" runat="server" DataSourceID="odsContabilista"
                                            DataTextField="Nome" DataValueField="IdContabilista" AppendDataBoundItems="True">
                                            <asp:ListItem Value="0">Selecione</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                            <table style="margin-bottom: 15px; display:<%= Request["tipo"] == "1" ? "inherit" : "none" %>">
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="cboExibirST" runat="server" />
                                    </td>
                                    <td align="left">
                                        Exibe ICMS ST em Observações
                                    </td>
                                     <td style="padding-left:10px">
                                        <asp:CheckBox ID="cboExibirIPI" runat="server" Checked="True" />
                                    </td>

                                    <td align="left">
                                        Exibe IPI
                                    </td>
                                </tr>
                            </table>
                            <table id="dadosApuracao" style="margin-bottom: 15px; display: none; text-align: left">
                                <tr>
                                    <td>
                                        <p style="color: #000000; font-size: 12px">
                                            Dados para Resumo Apuração ICMS/IPI </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <table>
                                            <tr>
                                                <td>
                                                    <fieldset>
                                                        <legend>Outros Débitos</legend>
                                                        <table style="margin: 10px">
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="Label8" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtOutrosDebitosDesc" Style="width: 300px;" runat="server"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="Label9" runat="server" Text="Valor" ForeColor="#0066FF"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtOutrosDebitosValor" onkeypress="return soNumeros(event, false, true);"
                                                                        runat="server"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </fieldset>
                                                    <fieldset>
                                                        <legend>Estorno de Créditos</legend>
                                                        <table style="margin: 10px">
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="Label11" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtEstornoCreditoDesc" Style="width: 300px;" runat="server"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="Label12" runat="server" Text="Valor" ForeColor="#0066FF"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtEstornoCreditoValor" onkeypress="return soNumeros(event, false, true);"
                                                                        runat="server"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </fieldset>
                                                    <fieldset>
                                                        <legend>Outros Créditos</legend>
                                                        <table style="margin: 10px">
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="Label13" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtOutrosCreditosDesc" Style="width: 300px;" runat="server"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="Label14" runat="server" Text="Valor" ForeColor="#0066FF"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtOutrosCreditosValor" onkeypress="return soNumeros(event, false, true);"
                                                                        runat="server"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </fieldset>
                                                    <fieldset>
                                                        <legend>Estorno de Débitos</legend>
                                                        <table style="margin: 10px">
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="Label15" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtEstornoDebitoDesc" Style="width: 300px;" runat="server"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="Label16" runat="server" Text="Valor" ForeColor="#0066FF"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtEstornoDebitoValor" onkeypress="return soNumeros(event, false, true);"
                                                                        runat="server"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </fieldset>
                                                    <fieldset>
                                                        <legend>Deduções</legend>
                                                        <table style="margin: 10px">
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="Label17" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtDeducaoDesc" Style="width: 300px;" runat="server"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="Label19" runat="server" Text="Valor" ForeColor="#0066FF"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtDeducaoValor" onkeypress="return soNumeros(event, false, true);" runat="server"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </fieldset>
                                                </td>
                                            </tr>
                                        </table>
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
                        <td align="center" valign="middle" colspan="10">
                            <a href="#" id="lnkImprimir" onclick="openRpt()" title="Gerar" style="padding: 5px">
                                <img alt="" border="0" src="../../Images/printer.png" />
                                Gerar</a>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                            </colo:VirtualObjectDataSource>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContabilista" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.ContabilistaDAO"
                                >
                            </colo:VirtualObjectDataSource>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetAdministradores"
                                TypeName="Glass.Data.DAL.FuncionarioDAO" >
                                <SelectParameters>
                                    <asp:Parameter DefaultValue="false" Name="incluirAdminSync" Type="Boolean" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                            <asp:HiddenField ID="hdfTipo" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        if (FindControl("hdfTipo", "input").value == "3" || FindControl("hdfTipo", "input").value == "4") {
            FindControl("dadosApuracao", "table").style.display = "table";
        }
        else {
            FindControl("dadosApuracao", "table").style.display = "none";
        }
    </script>

</asp:Content>
