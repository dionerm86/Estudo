<%@ Page Title="Arquivos de EFD" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="EFD.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Fiscal.EFD" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <style type="text/css">
        .aba, .painel
        {
            width: 600px;
        }
        .aba
        {
            position: relative;
            left: -9px;
            padding-bottom: 6px;
        }
        .aba span
        {
            padding: 6px;
            margin-right: 3px;
            cursor: pointer;
        }
        .painel
        {
            border: 1px solid gray;
            height: 330px;
            vertical-align: top;
            padding: 8px;
            overflow: auto;
        }
    </style>

    <script type="text/javascript">
        // -----------------------------------
        // Função que muda a aba ativa na tela
        // -----------------------------------
        function mudaAba(nomeAba) {
            // variável que contém os identificadores das abas
            var abas = new Array("icmsIpi", "pisCofins", "fci");

            // percorre todas as abas
            for (i = 0; i < abas.length; i++) {
                // recupera o título de aba atual e altera dependendo do parâmetro
                var aba = document.getElementById("aba_" + abas[i]);
                var borda = (abas[i] == nomeAba) ? "1px solid gray" : "1px solid silver";
                var bordaInferior = (abas[i] == nomeAba) ? "1px solid white" : "";

                if (aba == null)
                    continue;
                
                aba.style.fontWeight = (abas[i] == nomeAba) ? "bold" : "normal";
                aba.style.borderTop = borda;
                aba.style.borderLeft = borda;
                aba.style.borderRight = borda;
                aba.style.borderBottom = bordaInferior;

                // recupera a aba atual e exibe/esconde dependendo do parâmetro
                var aba = document.getElementById(abas[i]);
                aba.style.display = (abas[i] == nomeAba) ? "block" : "none";
            }

            // altera o hiddenfield que guarda a aba atual
            document.getElementById("<%= hdfAba.ClientID %>").value = nomeAba;
        }

        // ------------------------------------------------------
        // Função que disponibiliza o download do arquivo do EFD.
        // ------------------------------------------------------
        function baixarArquivo() {
            // recupera os valores selecionados nos campos
            var aba = document.getElementById("<%= hdfAba.ClientID %>").value;

            if (!validate(aba))
                return;

            var idLoja_icmsIpi = document.getElementById("<%= drpLoja.ClientID %>").value;
            var idLoja_pisCofins = document.getElementById("<%= cblLoja.ClientID %>_Selecao").itens();
            var idContabilista_icmsIpi = document.getElementById("<%= drpContabilista_icmsIpi.ClientID %>").value;
            var idContabilista_pisCofins = document.getElementById("<%= drpContabilista_pisCofins.ClientID %>").value;
            var mes_icmsIpi = document.getElementById("<%= drpMes_icmsIpi.ClientID %>").value;
            var ano_icmsIpi = document.getElementById("<%= txtAno_icmsIpi.ClientID %>").value;
            var mes_pisCofins = document.getElementById("<%= drpMes_pisCofins.ClientID %>").value;
            var ano_pisCofins = document.getElementById("<%= txtAno_pisCofins.ClientID %>").value;
            var idLoja = aba == "icmsIpi" ? idLoja_icmsIpi : idLoja_pisCofins;
            var idContabilista = aba == "icmsIpi" ? idContabilista_icmsIpi : idContabilista_pisCofins;
            var mes = aba == "icmsIpi" ? mes_icmsIpi : mes_pisCofins;
            var ano = aba == "icmsIpi" ? ano_icmsIpi : ano_pisCofins;

            // verifica se há um contabilista selecionado
            if (idContabilista == "") {
                alert("Selecione um contabilista.");
                return;
            }

            // garante que os campos de data não estejam vazios
            if (ano == "" || ano.length < 4) {
                alert("Preencha o ano com 4 dígitos antes de continuar.");
                return;
            }

            var intervalo = FindControl("hdfIntervaloData", "input").value;
            if (intervalo.indexOf(",,") > -1 || intervalo.indexOf(",") == 0 || (intervalo.length > 0 && intervalo.indexOf(",") == (intervalo.length - 1))) {
                alert("Informe todas as datas do intervalo de apuração.");
                return;
            }

            var arquivoRetificador = document.getElementById("<%= chkArquivoRetificador.ClientID %>").checked;
            var numReciboOriginal = document.getElementById("<%= txtNumReciboOriginal.ClientID %>").value;

            var blocoH = document.getElementById("<%= chkGerarBlocoH.ClientID %>").checked;
            var blocoK = document.getElementById("<%= chkGerarBlocoK.ClientID %>").checked;
            var aidf = document.getElementById("<%= txtAidf.ClientID %>").value;
            var codIncTrib = document.getElementById("<%= ctrlCodIncTrib.ClientID %>_hdfValor").value;
            var indAproCred = document.getElementById("<%= ctrlIndAproCred.ClientID %>_hdfValor").value;
            var codTipoCont = document.getElementById("<%= ctrlCodTipoCont.ClientID %>_hdfValor").value;

            bloquearPagina();
            desbloquearPagina(false);

            var token = new Date().getTime();
            redirectUrl("../../Handlers/Fiscal.ashx?tipo=EFD&tipoEFD=" + aba + "&arquivoRetificador=" + arquivoRetificador +
                "&numReciboOriginal=" + numReciboOriginal + "&loja=" + idLoja + "&contabilista=" + idContabilista + "&mes=" + mes +
                "&ano=" + ano + "&intervalo=" + intervalo + "&blocoH=" + blocoH + "&aidf=" + aidf + "&codIncTrib=" + codIncTrib +
                "&indAproCred=" + indAproCred + "&codTipoCont=" + codTipoCont + "&token=" + token + "&blocoK=" + blocoK);

            var idInt = setInterval(function() {
                var cookies = document.cookie.split(";");
                for (i = 0; i < cookies.length; i++) {
                    var c = cookies[i].split("=");
                    if (c[0].indexOf("token") > -1 && c[1] == token) {
                        clearInterval(idInt);
                        desbloquearPagina(true);
                    }
                }
            }, 200);
        }

        var primeiraLinhaIntervalo = 5;

        function atualizaBotoes() {
            var compl = "_" + document.getElementById("<%= hdfAba.ClientID %>").value;
            var tabela = document.getElementById("tabela" + compl);
            for (i = primeiraLinhaIntervalo; i < tabela.rows.length; i++) {
                var isUltimaLinha = (i + 1) == tabela.rows.length;
                FindControl("imgAdicionar" + compl, "input", tabela.rows[i].cells[1]).style.display = isUltimaLinha ? "" : "none";
                FindControl("imgRemover" + compl, "input", tabela.rows[i].cells[1]).style.display = i > primeiraLinhaIntervalo && isUltimaLinha ? "" : "none";
            }

            atualizaIntervaloData();
        }

        function adicionar() {
            var compl = "_" + document.getElementById("<%= hdfAba.ClientID %>").value;
            var tabela = document.getElementById("tabela" + compl);
            tabela.insertRow(tabela.rows.length);
            tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[primeiraLinhaIntervalo].innerHTML;
            tabela.rows[tabela.rows.length - 1].cells[0].innerHTML = "";
            var txtData = FindControl("ctrlDataInt" + compl + "_txtData", "input", tabela.rows[tabela.rows.length - 1].cells[1]);
            var imgData = FindControl("ctrlDataInt" + compl + "_imgData", "input", tabela.rows[tabela.rows.length - 1].cells[1]);
            txtData.value = "";
            txtData.id += (tabela.rows.length - 1).toString();
            imgData.setAttribute("onclick", imgData.getAttribute("onclick").replace("txtData", "txtData" + (tabela.rows.length - 1)));
            atualizaBotoes();
        }

        function remover() {
            var compl = "_" + document.getElementById("<%= hdfAba.ClientID %>").value;
            var tabela = document.getElementById("tabela" + compl);
            tabela.deleteRow(tabela.rows.length - 1);
            atualizaBotoes();
        }

        function atualizaIntervaloData() {
            var compl = "_" + document.getElementById("<%= hdfAba.ClientID %>").value;
            var intervalo = new Array();
            var tabela = document.getElementById("tabela" + compl);
            for (i = primeiraLinhaIntervalo; i < tabela.rows.length; i++)
                intervalo.push(FindControl("ctrlDataInt" + compl + "_txtData", "input", tabela.rows[i].cells[1]).value);

            FindControl("hdfIntervaloData" + compl, "input").value = intervalo.join(",");
        }

        function alteraArquivoRetificador(retificar) {
            document.getElementById("<%= txtNumReciboOriginal.ClientID %>").disabled = !retificar;
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <div align="left" class="aba">
                    <span id="aba_icmsIpi" onclick="mudaAba('icmsIpi')">EFD ICMS/IPI</span> <span id="aba_pisCofins"
                        onclick="mudaAba('pisCofins')">EFD PIS/COFINS</span>
                    <%= ExibeSped() %>
                </div>
                <div class="painel">
                    <div id="icmsIpi">
                        <table id="tabela_icmsIpi" class="gridStyle" style="border-collapse: collapse; border: none">
                            <tr>
                                <td align="right">
                                    <asp:Label ID="lblLoja_icmsIpi" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                        DataValueField="IdLoja" OnDataBound="drpLoja_DataBound">
                                    </asp:DropDownList>
                                    <asp:HiddenField ID="hdfIdLoja" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label1" runat="server" Text="Contabilista" ForeColor="#0066FF"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:DropDownList ID="drpContabilista_icmsIpi" runat="server" DataSourceID="odsContabilista"
                                        DataTextField="Nome" DataValueField="IdContabilista">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="lblPeriodo" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                                </td>
                                <td nowrap="nowrap" align="left">
                                    <table class="pos" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td width="1%">
                                                <asp:DropDownList ID="drpMes_icmsIpi" runat="server" DataSourceID="odsMes" DataTextField="Descr"
                                                    DataValueField="Id">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtAno_icmsIpi" runat="server" MaxLength="4" Columns="4" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label3" runat="server" Text="Contabilizar Inventário Fiscal" ForeColor="#0066FF"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:CheckBox ID="chkGerarBlocoH" runat="server" Checked="True" />
                                </td>
                            </tr>
                            <tr id="trBlocoK">
                                <td align="right">
                                    <asp:Label ID="Label5" runat="server" Text="Contabilizar Produção (Bloco K)" ForeColor="#0066FF"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:CheckBox ID="chkGerarBlocoK" runat="server" Checked="false"/>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label4" runat="server" Text="Autorização para Impressão de Documento Fiscal"
                                        ForeColor="#0066FF"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtAidf" runat="server" onkeypress="return soNumeros(event, false, false)"
                                        MaxLength="20" Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label2" runat="server" Text="Intervalos apuração ICMS" ForeColor="#0066FF"></asp:Label>
                                </td>
                                <td align="left">
                                    <uc1:ctrlData ID="ctrlDataInt_icmsIpi" runat="server" onchange="atualizaIntervaloData()"
                                        ReadOnly="ReadWrite" />
                                    <asp:ImageButton ID="imgAdicionar_icmsIpi" runat="server" ImageAlign="AbsMiddle"
                                        ImageUrl="~/Images/Insert.gif" OnClientClick="adicionar(); return false" />
                                    <asp:ImageButton ID="imgRemover_icmsIpi" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/ExcluirGrid.gif"
                                        Style="display: none" OnClientClick="remover(); return false" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="pisCofins">
                        <table id="tabela_pisCofins" class="gridStyle" style="border-collapse: collapse;
                            border: none">
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label9" runat="server" Text="Lojas" ForeColor="#0066FF"></asp:Label>
                                </td>
                                <td align="left">
                                    <sync:CheckBoxListDropDown ID="cblLoja" runat="server" CheckAll="True" DataSourceID="odsLoja"
                                        DataTextField="NomeFantasia" DataValueField="IdLoja" Title="Selecione as Lojas">
                                    </sync:CheckBoxListDropDown>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label10" runat="server" Text="Contabilista" ForeColor="#0066FF"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:DropDownList ID="drpContabilista_pisCofins" runat="server" DataSourceID="odsContabilista"
                                        DataTextField="Nome" DataValueField="IdContabilista">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label11" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                                </td>
                                <td nowrap="nowrap" align="left">
                                    <table class="pos" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td width="1%">
                                                <asp:DropDownList ID="drpMes_pisCofins" runat="server" DataSourceID="odsMes" DataTextField="Descr"
                                                    DataValueField="Id">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtAno_pisCofins" runat="server" MaxLength="4" Columns="4" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label13" runat="server" Text="Indicador Incidência Tributária" ForeColor="#0066FF"></asp:Label>
                                </td>
                                <td align="left">
                                    <uc2:ctrlSelPopup ID="ctrlCodIncTrib" runat="server" DataSourceID="odsCodIncTrib"
                                        DataTextField="Descr" DataValueField="Id" ExibirIdPopup="False" FazerPostBackBotaoPesquisar="False"
                                        PermitirVazio="False" ValidationGroup="pisCofins" TituloTela="Selecione o Indicador Incidência Tributária"
                                        TextWidth="200px" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label12" runat="server" Text="Método de Apropriação de Créditos" ForeColor="#0066FF"></asp:Label>
                                </td>
                                <td align="left">
                                    <uc2:ctrlSelPopup ID="ctrlIndAproCred" runat="server" DataSourceID="odsIndAproCred"
                                        DataTextField="Descr" DataValueField="Id" ExibirIdPopup="False" FazerPostBackBotaoPesquisar="False"
                                        PermitirVazio="False" ValidationGroup="pisCofins" TituloTela="Selecione o Método de Apropriação de Créditos"
                                        TextWidth="200px" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label14" runat="server" Text="Tipo de Contribuição Apurada" ForeColor="#0066FF"></asp:Label>
                                </td>
                                <td align="left">
                                    <uc2:ctrlSelPopup ID="ctrlCodTipoCont" runat="server" DataSourceID="odsCodTipoCont"
                                        DataTextField="Descr" DataValueField="Id" ExibirIdPopup="False" FazerPostBackBotaoPesquisar="False"
                                        PermitirVazio="False" ValidationGroup="pisCofins" TituloTela="Selecione o Tipo de Contribuição Apurada"
                                        TextWidth="200px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <br />
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkArquivoRetificador" runat="server" Text="Arquivo retificador"
                                onclick="alteraArquivoRetificador(this.checked)" />
                        </td>
                        <td>
                            &nbsp;&nbsp;
                            <asp:Label ID="Label16" runat="server" Text="Número do recibo do EFD original" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumReciboOriginal" runat="server" Enabled="False"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="3">
                            <asp:Button ID="btnBaixar" runat="server" Text="Gerar arquivo" 
                                OnClientClick="baixarArquivo(); return false" />
                        </td>
                    </tr>
                </table>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContabilista" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.ContabilistaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodIncTrib" runat="server" SelectMethod="GetCodIncTrib"
                    TypeName="Glass.Data.EFD.DataSourcesEFD">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsIndAproCred" runat="server" SelectMethod="GetIndAproCred"
                    TypeName="Glass.Data.EFD.DataSourcesEFD">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodTipoCont" runat="server" SelectMethod="GetCodTipoCont"
                    TypeName="Glass.Data.EFD.DataSourcesEFD">
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfAba" runat="server" Value="icmsIpi" />
                <asp:HiddenField ID="hdfIntervaloData_icmsIpi" runat="server" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMes" runat="server" SelectMethod="GetMeses" TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        document.getElementById("<%= hdfAba.ClientID %>").value = "pisCofins";
         mudaAba(document.getElementById("<%= hdfAba.ClientID %>").value);

        $(document).ready(function() {
            var iniciar = function() {
                if (FindControl("cblLoja", "select").style.display != "none")
                    setTimeout(function() { iniciar(); }, 100);
                else {
                    document.getElementById("<%= hdfAba.ClientID %>").value = "icmsIpi";
                    mudaAba(document.getElementById("<%= hdfAba.ClientID %>").value);
                }
            };

            iniciar();
        });
    </script>

</asp:Content>
