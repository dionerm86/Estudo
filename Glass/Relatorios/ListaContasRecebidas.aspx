<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaContasRecebidas.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.ListaContasRecebidas" Title="Contas Recebidas" %>

<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlLogCancPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel, exportarGCON, exportarProsoft, exportarDominio) {
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idLiberarPedido = FindControl("txtNumLiberarPedido", "input").value;
            var idAcerto = FindControl("txtAcerto", "input").value;
            var idAcertoParcial = FindControl("txtAcertoParc", "input").value;
            var idTrocaDev = FindControl("txtTrocaDev", "input").value;
            var idCli = FindControl("txtNumCli", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var idFunc = FindControl("drpFuncionario", "select").value;
            var idVendedorObra = FindControl("drpVendedorObra", "select").value;
            var idFuncRecebido = FindControl("drpRecebidaPor", "select").value;
            var idComissionado = FindControl("drpComissionado", "select").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var dtIniVenc = FindControl("ctrlDataIniVenc_txtData", "input").value;
            var dtFimVenc = FindControl("ctrlDataFimVenc_txtData", "input").value;
            var dtIniRec = FindControl("ctrlDataIniRec_txtData", "input").value;
            var dtFimRec = FindControl("ctrlDataFimRec_txtData", "input").value;
            var dataIniCad = FindControl("ctrlDataIniCad_txtData", "input").value;
            var dataFimCad = FindControl("ctrlDataFimCad_txtData", "input").value;
            var formaPagto = FindControl("cblFormaPagto", "select").itens();
            var tipoBoleto = FindControl("drpTipoBoleto", "select") != null ? FindControl("drpTipoBoleto", "select").value : 0;
            var valorInicial = FindControl("txtPrecoInicial", "input").value;
            var valorFinal = FindControl("txtPrecoFinal", "input").value;
            var renegociadas = FindControl("chkRenegociadas", "input").checked;
            var numeroNFe = FindControl("txtNFe", "input").value;
            var idRota = FindControl("drpRota", "select").value;
            var Obs = FindControl("txtSrcObs", "input").value;
            var exibirAReceber = FindControl("chkExibirAReceber", "input").checked;
            var ordenar = FindControl("drpOrdenar", "select").value;
            var tipoConta = FindControl("drpTipoConta", "select").itens();
            var numArqRemessa = FindControl("txtNumArqRemessa", "input").value
            var contasCnab = FindControl("drpArquivoRemessa", "select").value;
            var refObra = FindControl("chkRefObra", "input").checked;
            var idVendedorAssociado = FindControl("drpVendedorAssociado", "select").value;
            var idComissao = FindControl("txtIdComissao", "input") != null ? FindControl("txtIdComissao", "input").value : "";
            var idSinal = FindControl("txtIdSinal", "input") != null ? FindControl("txtIdSinal", "input").value : "";
            var numCte = FindControl("txtNumCte", "input").value;
            var protestadas = FindControl("chkProtestadas", "input").checked;
            var contasVinculadas = FindControl("chkExibirContasVinculadas", "input").checked;
            var tipoContasBuscar = FindControl("cblBuscarContas", "select").itens();
            var numAutCartao = FindControl("txtNumAutorizacao", "input").value

            var queryString = idPedido == "" ? "&idPedido=0" : "&idPedido=" + idPedido;
            queryString += idLiberarPedido == "" ? "&idLiberarPedido=0" : "&idLiberarPedido=" + idLiberarPedido;
            queryString += idAcerto == "" ? "&idAcerto=0" : "&idAcerto=" + idAcerto;
            queryString += idAcertoParcial == "" ? "&idAcertoParcial=0" : "&idAcertoParcial=" + idAcertoParcial;
            queryString += "&idTrocaDev=" + idTrocaDev;
            queryString += idCli == "" ? "&idCli=0" : "&idCli=" + idCli;
            queryString += numeroNFe == "" ? "&numeroNFe=0" : "&numeroNFe=" + numeroNFe;
            queryString += "&Obs=" + Obs;
            queryString += "&numArqRemessa=" + numArqRemessa;
            queryString += "&contasCnab=" + contasCnab;
            queryString += "&refObra=" + refObra;
            queryString += "&idVendedorAssociado=" + idVendedorAssociado;
            queryString += "&idVendedorObra=" + idVendedorObra;
            queryString += "&idComissao=" + idComissao;
            queryString += "&idSinal=" + idSinal;
            queryString += "&numCte=" + numCte;
            queryString += "&protestadas=" + protestadas;
            queryString += "&contasVinculadas=" + contasVinculadas;
            queryString += "&tipoContasBuscar=" + tipoContasBuscar;
            queryString += "&numAutCartao=" + numAutCartao;

            if (!exportarGCON && !exportarProsoft && !exportarDominio) {
                openWindow(600, 800, "RelBase.aspx?Rel=ContasRecebidas&nomeCli=" + nomeCli + "&dtIniRec=" + dtIniRec + "&dtFimRec=" + dtFimRec +
                    "&dtIniVenc=" + dtIniVenc + "&dtFimVenc=" + dtFimVenc + "&dataIniCad=" + dataIniCad + "&dataFimCad=" + dataFimCad + "&renegociadas=" + renegociadas + "&idLoja=" + idLoja +
                    "&idsFormaPagto=" + formaPagto + "&valorInicial=" + valorInicial + "&valorFinal=" + valorFinal +
                    "&tipoBoleto=" + tipoBoleto + "&idFunc=" + idFunc + "&idFuncRecebido=" + idFuncRecebido + "&idComissionado=" + idComissionado +
                    "&exibirAReceber=" + exibirAReceber + queryString + "&idRota=" + idRota + "&ordenar=" + ordenar + "&exportarExcel=" + exportarExcel +
                    "&tipoConta=" + tipoConta);
            }
            else if (exportarGCON) {
                window.open("../Handlers/ArquivoGCon.ashx?nomeCli=" + nomeCli + "&dtIniRec=" + dtIniRec + "&dtFimRec=" + dtFimRec +
                    "&dtIniVenc=" + dtIniVenc + "&dtFimVenc=" + dtFimVenc + "&dataIniCad=" + dataIniCad + "&dataFimCad=" + dataFimCad + "&renegociadas=" + renegociadas + "&idLoja=" + idLoja +
                    "&idFormaPagto=" + formaPagto + "&valorInicial=" + valorInicial + "&valorFinal=" + valorFinal +
                    "&tipoBoleto=" + tipoBoleto + "&idFunc=" + idFunc + "&idFuncRecebido=" + idFuncRecebido + "&idComissionado=" + idComissionado +
                    "&exibirAReceber=" + exibirAReceber + queryString + "&idRota=" + idRota + "&ordenar=" + ordenar + "&exportarExcel=" + exportarExcel +
                    "&tipoConta=" + tipoConta + "&receber=true");
            }
            else if (exportarProsoft) {
                window.open("../Handlers/ArquivoProsoft.ashx?nomeCli=" + nomeCli + "&dtIniRec=" + dtIniRec + "&dtFimRec=" + dtFimRec +
                    "&dtIniVenc=" + dtIniVenc + "&dtFimVenc=" + dtFimVenc + "&dataIniCad=" + dataIniCad + "&dataFimCad=" + dataFimCad + "&renegociadas=" + renegociadas + "&idLoja=" + idLoja +
                    "&idFormaPagto=" + formaPagto + "&valorInicial=" + valorInicial + "&valorFinal=" + valorFinal +
                    "&tipoBoleto=" + tipoBoleto + "&idFunc=" + idFunc + "&idFuncRecebido=" + idFuncRecebido + "&idComissionado=" + idComissionado +
                    "&exibirAReceber=" + exibirAReceber + queryString + "&idRota=" + idRota + "&ordenar=" + ordenar + "&exportarExcel=" + exportarExcel +
                    "&tipoConta=" + tipoConta + "&receber=true");
            }
            else if (exportarDominio) {
                window.open("../Handlers/ArquivoDominio.ashx?nomeCli=" + nomeCli + "&dtIniRec=" + dtIniRec + "&dtFimRec=" + dtFimRec +
                    "&dtIniVenc=" + dtIniVenc + "&dtFimVenc=" + dtFimVenc + "&dataIniCad=" + dataIniCad + "&dataFimCad=" + dataFimCad + "&renegociadas=" + renegociadas + "&idLoja=" + idLoja +
                    "&idFormaPagto=" + formaPagto + "&valorInicial=" + valorInicial + "&valorFinal=" + valorFinal +
                    "&tipoBoleto=" + tipoBoleto + "&idFunc=" + idFunc + "&idFuncRecebido=" + idFuncRecebido + "&idComissionado=" + idComissionado +
                    "&exibirAReceber=" + exibirAReceber + queryString + "&idRota=" + idRota + "&ordenar=" + ordenar + "&exportarExcel=" + exportarExcel +
                    "&tipoConta=" + tipoConta+ "&receber=true");
            }

            return false;
        }

        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function openRptUnico(url) {
            openWindow(600, 800, url);
            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label7" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td align="right" nowrap="nowrap" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                            <asp:Label ID="Label1" runat="server" Text="Liberação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                            <asp:TextBox ID="txtNumLiberarPedido" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label2" runat="server" Text="Acerto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtAcerto" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label3" runat="server" Text="Acerto parcial" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtAcertoParc" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label24" runat="server" Text="Troca/Dev." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtTrocaDev" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label22" runat="server" Text="Num. NF" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtNFe" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Sinal/Pagto. Antecipado" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtIdSinal" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label35" runat="server" ForeColor="#0066FF" Text="CT-e"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNumCte" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label23" runat="server" Text="Período Venc." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIniVenc" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFimVenc" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq6" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Período Rec." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIniRec" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFimRec" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Período Cad." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIniCad" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFimCad" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Recebida Por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpRecebidaPor" runat="server" DataSourceID="odsFuncFinanceiro"
                                DataTextField="Nome" DataValueField="IdFunc" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <uc3:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="True" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq8" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label25" runat="server" Text="Vendedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:DropDownList ID="drpFuncionario" runat="server" DataSourceID="odsFuncionario"
                                DataTextField="Nome" DataValueField="IdFunc" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label16" runat="server" Text="Vendedor Associado" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpVendedorAssociado" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsVendedorAssociado" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsVendedorAssociado" runat="server"
                                SelectMethod="GetAtivosAssociadosCliente" TypeName="Glass.Data.DAL.FuncionarioDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label19" runat="server" Text="Vendedor Obra" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpVendedorObra" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsVendedorObra" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsVendedorObra" runat="server"
                                SelectMethod="GetVendedores" TypeName="Glass.Data.DAL.FuncionarioDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Forma Pagto." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblFormaPagto" runat="server" DataSourceID="odsFormaPagto"
                                DataTextField="Descricao" DataValueField="IdFormaPagto" AutoPostBack="True" OnSelectedIndexChanged="cblFormaPagto_SelectedIndexChanged">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoBoleto" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoBoleto"
                                DataTextField="Descricao" DataValueField="IdTipoBoleto" Visible="False" AutoPostBack="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" />
                        </td>
                        <td>
                            <asp:Label ID="Label14" runat="server" Text="Tipo Entrega" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoEntrega" runat="server" AppendDataBoundItems="true" OnLoad="drpTipoEntrega_Load"
                                DataSourceID="odsTipoEntrega" DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem Text="Todas" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq9" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" />
                        </td>
                        <td>
                            <asp:Label ID="Label15" runat="server" Text="Valor Rec." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPrecoInicial" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>até
                        </td>
                        <td>
                            <asp:TextBox ID="txtPrecoFinal" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq5" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label17" runat="server" Text="Ordenar por:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenar" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="1">Vencimento</asp:ListItem>
                                <asp:ListItem Value="2">Cliente</asp:ListItem>
                                <asp:ListItem Value="3">Valor</asp:ListItem>
                                <asp:ListItem Value="4">Num. NFe</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblRota" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpRota" runat="server" AppendDataBoundItems="True" DataSourceID="odsRota"
                                DataTextField="CodInterno" DataValueField="IdRota" OnLoad="drpRota_Load">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqRota" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label31" runat="server" Text="Tipo Conta:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown runat="server" ID="drpTipoConta" DataSourceID="odsTiposContas"
                                DataValueField="Id" DataTextField="Descr" Title="Selecione o tipo de conta">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblComissionado" runat="server" Text="Comissionado" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpComissionado" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsComissionado" DataTextField="Nome" DataValueField="IdComissionado">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq10" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Obs.:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtSrcObs" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label20" runat="server" Text="Número de Autorização" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNumAutorizacao" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="lblArquivoRemessa2" runat="server" Text="Num. Arquivo Remessa	" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNumArqRemessa" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="lblArquivoRemessa" runat="server" Text="Arquivo Remessa:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpArquivoRemessa" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="1">Não incluir contas de arquivo de remessa</asp:ListItem>
                                <asp:ListItem Value="2" Selected="True">Incluir contas de arquivo de remessa</asp:ListItem>
                                <asp:ListItem Value="3">Somente contas de arquivo de remessa</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="lblComissao" runat="server" Text="Cód Comissão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtIdComissao" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbComissao" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="lblBuscarContas" runat="server" Text="Buscar contas" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblBuscarContas" runat="server" CheckAll="True" Title="Selecione o tipo das contas"
                                Width="200px">
                                <asp:ListItem Value="1">Contas com NF-e geradas</asp:ListItem>
                                <asp:ListItem Value="2" Style="color: red">Contas sem NF-e geradas</asp:ListItem>
                                <asp:ListItem Value="3">Demais contas</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbBuscarContas" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                            <asp:Image ID="imgBuscarContas" runat="server" ImageUrl="~/Images/help.gif" AlternateText=" " 
                                ToolTip=
                                    "Contas com NFe gerada: cliente com percentual de redução em NFe E conta com referência direta com alguma liberação E liberação associada à alguma nota fiscal que não esteja cancelada, denegada ou inutilizada.
Sem NFe gerada: cliente com percentual de redução em NFe E conta com referência direta com alguma liberação E liberação NÃO associada à alguma nota fiscal que não esteja cancelada, denegada ou inutilizada.
Demais contas: cliente sem percentual de redução em NFe ou conta sem referência direta com alguma liberação."/>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkExibirAReceber" runat="server" AutoPostBack="True" Text="Exibir contas a receber"
                                OnCheckedChanged="chkExibirAReceber_CheckedChanged" />
                            <asp:HiddenField runat="server" ID="hdfExibirAReceber" Value="True" />
                        </td>
                        <td>
                            <td>
                                <asp:CheckBox ID="chkRenegociadas" runat="server" AutoPostBack="False" Text="Visualizar contas renegociadas" />
                            </td>
                            <td>
                                <asp:CheckBox ID="chkRefObra" runat="server" Text="Incluir contas referência Obra" Checked="true" AutoPostBack="True" />
                            </td>
                            <td>
                            <asp:CheckBox ID="chkProtestadas" runat="server" Text="Apenas contas em cartório" Checked="false" AutoPostBack="True" />&nbsp;
                            </td>
                            <td>
                                <asp:CheckBox ID="chkExibirContasVinculadas" runat="server" Text="Incluir contas vinculadas" AutoPostBack="True" />
                            </td>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdConta" runat="server" AutoGenerateColumns="False"
                    DataKeyNames="IdContaR" DataSourceID="odsContasReceber" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    EmptyDataText="Nenhuma conta recebida encontrada." AllowPaging="True" AllowSorting="True"
                    OnRowDataBound="grdConta_RowDataBound">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="False" CommandName="Edit"
                                    Visible='<%# !(bool)Eval("IsParcelaCartao") %>'>
                                      <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                <asp:LinkButton ID="lnkCancelar" runat="server" OnClientClick='<%# "openWindow(200, 500, \"../Utils/SetMotivoCancReceb.aspx?tipo=contaR&id=" + Eval("IdContaR") + "\"); return false" %>'
                                    Visible='<%# Eval("DeleteVisible") %>'>
                                    <img src="../Images/ExcluirGrid.gif" border="0" /></asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CausesValidation="false" CommandName="Update"
                                    Height="16px" ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:PlaceHolder runat="server" ID="rptRef" Visible='<%# !String.IsNullOrEmpty(Eval("RelatorioPedido") as string) %>'>
                                    <a href="#" onclick="openRptUnico('<%# Eval("RelatorioPedido") %>');">
                                        <img border="0" src="../Images/Relatorio.gif" /></a> </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" ID="rptRec" Visible='<%# Eval("IdSinal") == null %>'>
                                    <a href="#" onclick="openRptUnico('<%# Eval("UrlRelatorio") %>');">
                                        <img border="0" src="../Images/script_go.gif" title="Dados do recebimento" /></a>
                                </asp:PlaceHolder>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referência" SortExpression="Referencia">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Referencia") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("Referencia") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Comissão" SortExpression="IdComissao">
                            <ItemTemplate>
                                <asp:Label ID="Label22" runat="server" Text='<%# Bind("IdComissao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label22" runat="server" Text='<%# Eval("IdComissao") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Parc." SortExpression="NumParcString">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("NumParcString") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("NumParcString") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cliente" SortExpression="IdNomeCli">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("IdNomeCli") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("IdNomeCli") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Forma Pagto." SortExpression="DescrFormaPagto">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("DescrFormaPagto") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("DescrFormaPagto") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorVec">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("ValorVec", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("ValorVec", "{0:C}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data. Venc." SortExpression="DataVec">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("DataVec", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("DataVec", "{0:d}") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Rec." SortExpression="ValorRec">
                            <ItemTemplate>
                                <asp:Label ID="Label71" runat="server" Text='<%# Eval("ValorRec", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label72" runat="server" Text='<%# Eval("ValorRec", "{0:C}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Rec." SortExpression="DataRec">
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("DataRec", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("DataRec", "{0:d}") %>'></asp:Label>
                            </EditItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Recebida por" SortExpression="NomeFunc">
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("NomeFunc") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("NomeFunc") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Num. NF" SortExpression="NumeroNFe">
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("NumeroNFe") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("NumeroNFe") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Localização" SortExpression="DestinoRec">
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("DestinoRec") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("DestinoRec") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Num. Arquivo Remessa" SortExpression="NumeroArquivoRemessaCnab">
                            <EditItemTemplate>
                                <asp:Label ID="Label78" runat="server" Text='<%# Bind("NumeroArquivoRemessaCnab") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label72" runat="server" Text='<%# Bind("NumeroArquivoRemessaCnab") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs" SortExpression="Obs">
                            <ItemTemplate>
                                <asp:Label ID="lblObs" runat="server" Text='<%# Bind("Obs") %>'></asp:Label>
                                <asp:Label ID="lblObsDescAcresc" runat="server" Text='<%# Bind("ObsDescAcresc") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Text='<%# Bind("Obs") %>'
                                    Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescrComissao" HeaderText="Comissão" SortExpression="DescrComissao">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="DescricaoContaContabil">
                            <ItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("DescricaoContaContabil") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("DescricaoContaContabil") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" IdRegistro='<%# Eval("IdContaR") %>'
                                    Tabela="ContasReceber" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc4:ctrlLogPopup ID="ctrlLogContasReceber" runat="server" Tabela="ContasReceber" IdRegistro='<%# Eval("IdContaR") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false, false, false, false);">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true, false, false, false); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkExportarGCON" runat="server" OnClientClick="openRpt(false, true, false, false); return false;"><img border="0" 
                    src="../Images/blocodenotas.png" /> Exportar para o GCON</asp:LinkButton>
                <asp:LinkButton ID="lnkExportarProsoft" runat="server" OnClientClick="openRpt(false, false, true, false); return false;"><img border="0" 
                    src="../Images/blocodenotas.png" /> Exportar para PROSOFT</asp:LinkButton>
                <asp:LinkButton ID="lnkExportarDominio" runat="server" OnClientClick="openRpt(false, false, false, true); return false;"><img border="0" 
                    src="../Images/blocodenotas.png" /> Exportar para DOMÍNIO SISTEMAS</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContasReceber" runat="server"
                    MaximumRowsParameterName="pageSize" SelectMethod="GetForListRpt" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.ContasReceberDAO" EnablePaging="True" SelectCountMethod="GetRptCount"
                    SortParameterName="sortExpression" DataObjectTypeName="Glass.Data.Model.ContasReceber"
                    UpdateMethod="AtualizaObsDataVec">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumLiberarPedido" Name="idLiberarPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtAcerto" Name="idAcerto" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtAcertoParc" Name="idAcertoParcial" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtTrocaDev" Name="idTrocaDevolucao" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNFe" Name="numeroNFe" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpRecebidaPor" Name="idFuncRecebido" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpTipoEntrega" Name="tipoEntrega" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniVenc" Name="dtIniVenc" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimVenc" Name="dtFimVenc" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniRec" Name="dtIniRec" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimRec" Name="dtFimRec" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniCad" Name="dataIniCad" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimCad" Name="dataFimCad" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="cblFormaPagto" Name="idsFormaPagto" PropertyName="SelectedValue"
                            Type="string" />
                        <asp:ControlParameter ControlID="drpTipoBoleto" Name="idTipoBoleto" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtPrecoInicial" Name="precoInicial" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="txtPrecoFinal" Name="precoFinal" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="chkRenegociadas" Name="renegociadas" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="hdfExibirAReceber" Name="recebida" Type="Boolean"
                            PropertyName="Value" ConvertEmptyStringToNull="true" />
                        <asp:ControlParameter ControlID="drpComissionado" Name="idComissionado" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpRota" Name="idRota" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtSrcObs" Name="obs" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpOrdenar" PropertyName="SelectedValue" Name="ordenacao"
                            Type="Int32"></asp:ControlParameter>
                        <asp:ControlParameter ControlID="drpTipoConta" Name="tipoContaContabil" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtNumArqRemessa" Name="numArqRemessa" PropertyName="text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpArquivoRemessa" Name="contasCnab" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="chkRefObra" Name="refObra" PropertyName="Checked" Type="Boolean" />
                        <asp:ControlParameter ControlID="drpVendedorAssociado" Name="idVendedorAssociado" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpVendedorObra" Name="idVendedorObra" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtIdComissao" Name="idComissao" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtIdSinal" Name="idSinal" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtNumCte" Name="numCte" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="chkProtestadas" Name="protestadas" PropertyName="Checked" Type="Boolean" />
                        <asp:ControlParameter ControlID="chkExibirContasVinculadas" Name="contasVinculadas" PropertyName="Checked" Type="Boolean" />
                        <asp:ControlParameter ControlID="cblBuscarContas" Name="tipoContasBuscar" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtNumAutorizacao" Name="numAutCartao" PropertyName="text"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoEntrega" runat="server"
                    SelectMethod="GetTipoEntrega" TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
                    SelectMethod="GetVendedores" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncFinanceiro" runat="server"
                    SelectMethod="GetCaixaDiario" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForConsultaConta"
                    TypeName="Glass.Data.DAL.FormaPagtoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoBoleto" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.TipoBoletoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsComissionado" runat="server"
                    SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.ComissionadoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource ID="odsTiposContas" runat="server" TypeName="Glass.Data.DAL.ContasReceberDAO"
                    SelectMethod="ObtemTiposContas">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource ID="odsRota" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.RotaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
