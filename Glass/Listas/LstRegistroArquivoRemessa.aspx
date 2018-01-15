<%@ Page Title="Registro de Importações do Arquivo Remessa" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstRegistroArquivoRemessa.aspx.cs" Inherits="Glass.UI.Web.Listas.LstRegistroArquivoRemessa" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function exibirRegistros(botao, idContaR) {

            var linha = document.getElementById("contaR_" + idContaR);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " Registros";
        }

        function openRpt(exportarExcel) {

            var idContaR = FindControl("txtIdContaR", "input").value;
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idLiberarPedido = FindControl("txtNumLiberarPedido", "input").value;
            var idAcerto = FindControl("txtAcerto", "input").value;
            var idAcertoParcial = FindControl("txtAcertoParc", "input").value;
            var idTrocaDev = FindControl("txtTrocaDev", "input").value;
            var numNfe = FindControl("txtNFe", "input").value;
            var idCli = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var lojaCliente = FindControl("chkLojaCliente", "input").checked;
            var numArquivoRemessa = FindControl("txtNumArqRemessa", "input").value;
            var idFormaPagto = FindControl("drpFormaPagto", "select").value;
            var obs = FindControl("txtSrcObs", "input").value;
            var dataIniVenc = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFimVenc = FindControl("ctrlDataFim_txtData", "input").value;
            var valorVecIni = FindControl("txtValorVecIni", "input").value;
            var valorVecFim = FindControl("txtValorVecFim", "input").value;
            var dataIniRec = FindControl("ctrlDataIniRec_txtData", "input").value;
            var dataFimRec = FindControl("ctrlDataFimRec_txtData", "input").value;
            var valorRecIni = FindControl("txtValorRecIni", "input").value;
            var valorRecFim = FindControl("txtValorRecFim", "input").value;
            var recebida = FindControl("ddlRecebida", "select").value;
            var codOcorrencia = FindControl("txtCodOcorrencia", "input").value;
            var nossoNumero = FindControl("txtNossoNumero", "input").value;
            var numDoc = FindControl("txtNumDoc", "input").value;
            var usoEmpresa = FindControl("txtUsoEmpresa", "input").value;
            var idContaBanco = FindControl("drpContaBanco", "select").value;

            var queryString = "&idContaR=" + idContaR;
            queryString += "&idPedido=" + idPedido;
            queryString += "&idLiberarPedido=" + idLiberarPedido;
            queryString += "&idAcerto=" + idAcerto;
            queryString += "&idAcertoParcial=" + idAcertoParcial;
            queryString += "&idTrocaDev=" + idTrocaDev;
            queryString += "&numNfe=" + numNfe;
            queryString += "&idCli=" + idCli;
            queryString += "&nomeCli=" + nomeCli;
            queryString += "&idLoja=" + idLoja;
            queryString += "&lojaCliente=" + lojaCliente;
            queryString += "&numArquivoRemessa=" + numArquivoRemessa;
            queryString += "&idFormaPagto=" + idFormaPagto;
            queryString += "&obs=" + obs;
            queryString += "&dataIniVenc=" + dataIniVenc;
            queryString += "&dataFimVenc=" + dataFimVenc;
            queryString += "&valorVecIni=" + valorVecIni;
            queryString += "&valorVecFim=" + valorVecFim;
            queryString += "&dataIniRec=" + dataIniRec;
            queryString += "&dataFimRec=" + dataFimRec;
            queryString += "&valorRecIni=" + valorRecIni;
            queryString += "&valorRecFim=" + valorRecFim;
            queryString += "&recebida=" + recebida;
            queryString += "&codOcorrencia=" + codOcorrencia;
            queryString += "&nossoNumero=" + nossoNumero;
            queryString += "&numDoc=" + numDoc;
            queryString += "&usoEmpresa=" + usoEmpresa;
            queryString += "&idContaBanco=" + idContaBanco;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=RegistroImpArqRemessa&ExportarExcel=" + exportarExcel +
                queryString);
        }
    
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="lblIdContaR" runat="server" Text="Cód." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtIdContaR" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton19" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label7" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                        </td>
                        <td align="right" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                            <asp:Label ID="Label1" runat="server" Text="Liberação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                            <asp:TextBox ID="txtNumLiberarPedido" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label2" runat="server" Text="Acerto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtAcerto" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label3" runat="server" Text="Acerto parcial" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAcertoParc" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label24" runat="server" Text="Troca/Dev." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtTrocaDev" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label22" runat="server" Text="Num. NF" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNFe" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
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
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="True" />
                            <asp:CheckBox ID="chkLojaCliente" runat="server" Text="Loja do Cliente?" AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label16" runat="server" Text="Forma Pagto." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFormaPagto" runat="server" AutoPostBack="True" DataSourceID="odsFormaPagto"
                                DataTextField="Descricao" DataValueField="IdFormaPagto">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label10" runat="server" Text="Período Venc." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Valor Venc." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorVecIni" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="até" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorVecFim" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" />
                        </td>
                        <td align="right">
                            <asp:Label ID="lblArquivoRemessa2" runat="server" Text="Num. Arq. Remessa" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtNumArqRemessa" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton11" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Período Rec." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIniRec" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFimRec" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label15" runat="server" Text="Valor Rec." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorRecIni" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="até" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorRecFim" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq5" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Contas" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlRecebida" runat="server">
                                <asp:ListItem Selected="True" Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Recebidas</asp:ListItem>
                                <asp:ListItem Value="2">A Receber</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton13" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Cód. Ocorrência" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodOcorrencia" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton15" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Nosso Número" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNossoNumero" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton16" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Núm. Documento" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumDoc" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton17" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label14" runat="server" Text="Uso da Empresa" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtUsoEmpresa" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton18" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblIdContaBancoCli" runat="server" ForeColor="#0066FF" Text="Conta Bancária"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContaBanco" runat="server" DataSourceID="odsContaBanco"
                                DataTextField="Descricao" DataValueField="IdContaBanco" AppendDataBoundItems="True"
                                 onchange="drpContaClienteChange(this);">
                                <asp:ListItem Text="" Value="0" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="ObterBancoAgrupado"
                                TypeName="Glass.Data.DAL.ContaBancoDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:Label ID="Label29" runat="server" Text="Obs.:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtSrcObs" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton14" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
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
                <asp:GridView GridLines="None" ID="grdContas" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    DataKeyNames="IdContaR" DataSourceID="odsContas" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhuma conta a receber \ recebida encontrada para o filtro informado.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton12" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirRegistros(this, " + Eval("IdContaR") + "); return false" %>'
                                    Width="10px" ToolTip="Exibir Registros" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/disk.gif" OnClientClick='<%# "redirectUrl(\"../Handlers/ArquivoRemessa.ashx?id=" + Eval("IdArquivoRemessa") + "\"); return false" %>'
                                    ToolTip="Download do Arquivo Remessa" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="idContaR" HeaderText="Cód." SortExpression="idContaR" />
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="Referencia" />
                        <asp:BoundField DataField="NumParcString" HeaderText="Parc." SortExpression="NumParcString" />
                        <asp:BoundField DataField="IdNomeCli" HeaderText="Cliente" SortExpression="IdNomeCli" />
                        <asp:BoundField DataField="ValorVec" HeaderText="Valor" SortExpression="ValorVec"
                            DataFormatString="{0:c}" />
                        <asp:BoundField DataField="DataVec" HeaderText="Data. Venc." SortExpression="DataVec"
                            DataFormatString="{0:d}" />
                        <asp:BoundField DataField="ValorRec" HeaderText="Valor Rec." SortExpression="ValorRec"
                            DataFormatString="{0:c}" />
                        <asp:BoundField DataField="DataRec" HeaderText="Data Rec." SortExpression="DataRec"
                            DataFormatString="{0:d}" />
                        <asp:BoundField DataField="DescrPlanoConta" HeaderText="Referente a" SortExpression="DescrPlanoConta" />
                        <asp:BoundField DataField="NumeroArquivoRemessaCnab" HeaderText="Num. Arq. Remessa"
                            SortExpression="NumeroArquivoRemessaCnab" />
                        <asp:TemplateField HeaderText="Obs" SortExpression="Obs">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Obs") %>'></asp:Label>
                                <asp:Label ID="Label32" runat="server" Text='<%# Bind("ObsDescAcresc") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                </td> </tr><asp:HiddenField ID="hdfIdContaR" runat="server" Value='<%# Eval("IdContaR") %>' />
                                <tr id="contaR_<%# Eval("IdContaR") %>" style="display: none;" class="<%= GetAlternateClass() %>">
                                    <td colspan="14">
                                        <asp:GridView GridLines="None" ID="grdRegistros" runat="server" AutoGenerateColumns="False"
                                            DataKeyNames="IdRegistroArquivoRemessa" DataSourceID="odsRegistros" CssClass="gridStyle"
                                            PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                                            EmptyDataText="Nenhuma registro encontrado para o filtro informado." Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/disk.gif" OnClientClick='<%# "redirectUrl(\"../Handlers/ArquivoRemessa.ashx?id=" + Eval("IdArquivoRemessa") + "\"); return false" %>'
                                                            ToolTip="Download do Arquivo Retorno" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="DataOcorrencia" HeaderText="Data da Ocorrência" SortExpression="DataOcorrencia"
                                                    DataFormatString="{0:d}" />
                                                <asp:BoundField DataField="CodOcorrenciaDescricao" HeaderText="Cód. da Ocorrência"
                                                    SortExpression="CodOcorrenciaDescricao" />
                                                <asp:BoundField DataField="NossoNumero" HeaderText="Nosso Número" SortExpression="NossoNumero" />
                                                <asp:BoundField DataField="NumeroDocumento" HeaderText="Número do Documento" SortExpression="NumeroDocumento" />
                                                <asp:BoundField DataField="UsoEmpresa" HeaderText="Uso da Empresa" SortExpression="UsoEmpresa" />
                                                <asp:BoundField DataField="ValorRecebido" HeaderText="Valor" SortExpression="ValorRecebido"
                                                    DataFormatString="{0:c}" />
                                                <asp:BoundField DataField="Juros" HeaderText="Juros" SortExpression="Juros" DataFormatString="{0:c}" />
                                                <asp:BoundField DataField="Multa" HeaderText="Multa" SortExpression="Multa" DataFormatString="{0:c}" />
                                            </Columns>
                                        </asp:GridView>
                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRegistros" runat="server" DataObjectTypeName="Glass.Data.Model.RegistroArquivoRemessa"
                                            SelectMethod="GetListRegistros" TypeName="WebGlass.Business.ArquivoRemessa.Fluxo.RegistroArquivoRemessaFluxo">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="hdfIdContaR" Name="idContaR" PropertyName="Value"
                                                    Type="UInt32" />
                                            </SelectParameters>
                                        </colo:VirtualObjectDataSource>
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
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false;"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContas" runat="server" DataObjectTypeName="Glass.Data.Model.ContasReceber"
                    SelectMethod="GetListWithExpression" TypeName="WebGlass.Business.ArquivoRemessa.Fluxo.RegistroArquivoRemessaFluxo"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetListWithExpressionCount"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                    <asp:ControlParameter ControlID="txtIdContaR" Name="idContaR" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumLiberarPedido" Name="idLiberarPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtAcerto" Name="idAcerto" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtAcertoParc" Name="idAcertoParcial" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtTrocaDev" Name="idTrocaDevolucao" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNFe" Name="numNFe" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="chkLojaCliente" Name="lojaCliente" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="txtNumArqRemessa" Name="numArqRemessa" PropertyName="text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpFormaPagto" Name="idFormaPagto" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtSrcObs" Name="obs" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dtIniVenc" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dtFimVenc" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniRec" Name="dtIniRec" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimRec" Name="dtFimRec" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtValorVecIni" Name="valorVecIni" PropertyName="Text"
                            Type="Decimal" />
                        <asp:ControlParameter ControlID="txtValorVecFim" Name="valorVecFim" PropertyName="Text"
                            Type="Decimal" />
                        <asp:ControlParameter ControlID="txtValorRecIni" Name="valorRecIni" PropertyName="Text"
                            Type="Decimal" />
                        <asp:ControlParameter ControlID="txtValorRecFim" Name="valorRecFim" PropertyName="Text"
                            Type="Decimal" />
                        <asp:ControlParameter ControlID="ddlRecebida" Name="recebida" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtCodOcorrencia" Name="codOcorrencia" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtNossoNumero" Name="nossoNumero" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtNumDoc" Name="numDoc" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtUsoEmpresa" Name="usoEmpresa" PropertyName="Text"
                            Type="String" />
                         <asp:ControlParameter ControlID="drpContaBanco" Name="idContaBanco" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForConsultaContasReceber"
                    TypeName="Glass.Data.DAL.FormaPagtoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
