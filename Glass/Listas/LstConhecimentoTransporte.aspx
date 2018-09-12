<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstConhecimentoTransporte.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstConhecimentoTransporte" Title="Conhecimento Transporte" %>

<%@ Register Src="~/Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlBoleto.ascx" TagName="ctrlBoleto" TagPrefix="uc7" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/CTe/LstCTe.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>

    <script type="text/javascript">

    function salvarCte(idCte)
    {
        redirectUrl('<%= this.ResolveClientUrl("../Handlers/CteXml.ashx") %>?idCte=' + idCte);
    }
    
    function openRptDacte(idCte, lotacao) {
        if(lotacao)
            openWindow(600, 800, "../Relatorios/CTe/RelBase.aspx?rel=DacteLotacao&idCte=" + idCte);
        else
            openWindow(600, 800, "../Relatorios/CTe/RelBase.aspx?rel=DacteFracionada&idCte=" + idCte);
        
        return false;
    }

    function openRpt(exportarExcel) {

        var numCte = FindControl("txtNumCte", "input").value;
        var situacao = FindControl("cboSituacao", "select").itens();
        var cfop = FindControl("drpCfop", "select").value;
        var formaPagto = FindControl("drpFormaPagto", "select").value;
        var tipoEmissao = FindControl("drpTipoEmissao", "select").value;
        var tipoCte = FindControl("drpTipoCte", "select").value;
        var tipoServico = FindControl("drpTipoServico", "select").value;
        var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
        var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
        var idTransportador = FindControl("drpTransportador", "select").value;
        var ordenar = FindControl("drpOrdenar", "select").value;
        var tipoRemetente = FindControl("drpTipoRemetente", "select").value;
        var idRemetente = FindControl("drpRemetente", "select").value;
        var tipoDestinatario = FindControl("drpTipoDestinatario", "select").value;
        var idDestinatario = FindControl("drpDestinatario", "select").value;
        var tipoRecebedor = FindControl("drpTipoRecebedor", "select").value;
        var idRecebedor = FindControl("drpRecebedor", "select").value;

        var queryString = "../Relatorios/RelBase.aspx?Rel=ListaCTe";
        queryString += "&numCte=" + numCte;
        queryString += "&situacao=" + situacao;
        queryString += "&cfop=" + cfop;
        queryString += "&formaPagto=" + formaPagto;
        queryString += "&tipoEmissao=" + tipoEmissao;
        queryString += "&tipoCte=" + tipoCte;
        queryString += "&tipoServico=" + tipoServico;
        queryString += "&dataIni=" + dataIni;
        queryString += "&dataFim=" + dataFim;
        queryString += "&ordenar=" + ordenar;
        queryString += "&idTransportador=" + idTransportador;
        queryString += "&tipoRemetente=" + tipoRemetente;
        queryString += "&idRemetente=" + idRemetente;
        queryString += "&tipoDestinatario=" + tipoDestinatario;
        queryString += "&idDestinatario=" + idDestinatario;
        queryString += "&tipoRecebedor=" + tipoRecebedor;
        queryString += "&idRecebedor=" + idRecebedor;
        queryString += "&exportarExcel=" + exportarExcel;

        openWindow(600, 800, queryString);

        return false;
    }
    
    function openLoteCtes()
    {
        var numCte = FindControl("txtNumCte", "input").value;
        var idLoja = FindControl("drpLoja", "select").value;
        var situacao = FindControl("cboSituacao", "select").itens();
        var cfop = FindControl("drpCfop", "select").value;
        var formaPagto = FindControl("drpFormaPagto", "select").value;
        var tipoEmissao = FindControl("drpTipoEmissao", "select").value;
        var tipoCte = FindControl("drpTipoCte", "select").value;
        var tipoServico = FindControl("drpTipoServico", "select").value;
        var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
        var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
        var idTransportador = FindControl("drpTransportador", "select").value;
        var ordenar = FindControl("drpOrdenar", "select").value;
        var tipoRemetente = FindControl("drpTipoRemetente", "select").value;
        var idRemetente = FindControl("drpRemetente", "select").value;
        var tipoDestinatario = FindControl("drpTipoDestinatario", "select").value;
        var idDestinatario = FindControl("drpDestinatario", "select").value;
        var tipoRecebedor = FindControl("drpTipoRecebedor", "select").value;
        var idRecebedor = FindControl("drpRecebedor", "select").value;

        var queryString = "numCte=" + numCte;
        queryString += "&idLoja=" + idLoja;
        queryString += "&situacao=" + situacao;
        queryString += "&cfop=" + cfop;
        queryString += "&formaPagto=" + formaPagto;
        queryString += "&tipoEmissao=" + tipoEmissao;
        queryString += "&tipoCte=" + tipoCte;
        queryString += "&tipoServico=" + tipoServico;
        queryString += "&dataIni=" + dataIni;
        queryString += "&dataFim=" + dataFim;
        queryString += "&ordenar=" + ordenar;
        queryString += "&idTransportador=" + idTransportador;
        queryString += "&tipoRemetente=" + tipoRemetente;
        queryString += "&idRemetente=" + idRemetente;
        queryString += "&tipoDestinatario=" + tipoDestinatario;
        queryString += "&idDestinatario=" + idDestinatario;
        queryString += "&tipoRecebedor=" + tipoRecebedor;
        queryString += "&idRecebedor=" + idRecebedor;
         
        redirectUrl('<%= this.ResolveClientUrl("../Handlers/CteXmlLote.ashx") %>?' + queryString);
    }

    function openRptTerc(idCte) {
        openWindow(600, 800, "../Relatorios/CTe/RelBase.aspx?rel=CteTerceiros&idCte=" + idCte);
        return false;
    }

    function exibirCentroCusto(idCte) {
        var idLoja = LstConhecimentoTransporte.ObtemIdLoja(idCte).value;

        if (idLoja == "")
        {
            alert("Não foram encontradas contas a pagar associadas a esse CTE para associação de centro de custo.");
            return false;
        }

        openWindow(365, 700, '../Utils/SelCentroCusto.aspx?IdCte=' + idCte + "&compra=false" + "&idLoja=" + idLoja);
        return true;
    }
        
    </script>
    
    <table>
        <tr align="center">
            <td>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblNumCte" runat="server" Text="Num. CTe" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCte" runat="server" MaxLength="10" Width="60px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" OnClientClick="return openRota();" />
                        </td>
                        <td>
                            <asp:Label ID="lblSituacao" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cboSituacao" runat="server" CheckAll="False" Title="Todas"
                                Width="188px">
                                <asp:ListItem Value="1">Aberta</asp:ListItem>
                                <asp:ListItem Value="2">Autorizada</asp:ListItem>
                                <asp:ListItem Value="3">Não emitida</asp:ListItem>
                                <asp:ListItem Value="4">Cancelada</asp:ListItem>
                                <asp:ListItem Value="5">Inutilizada</asp:ListItem>
                                <asp:ListItem Value="6">Denegada</asp:ListItem>
                                <asp:ListItem Value="7">Processo de emissão</asp:ListItem>
                                <asp:ListItem Value="8">Processo de cancelamento</asp:ListItem>
                                <asp:ListItem Value="9">Processo de inutilização</asp:ListItem>
                                <asp:ListItem Value="10">Falha ao emitir</asp:ListItem>
                                <asp:ListItem Value="11">Falha ao cancelar</asp:ListItem>
                                <asp:ListItem Value="12">Falha ao inutilizar</asp:ListItem>
                                <asp:ListItem Value="13">Finalizada</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblCfop" runat="server" Text="CFOP" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpCfop" runat="server" DataSourceID="odsCfop" DataTextField="CodInterno"
                                DataValueField="IdCfop" AppendDataBoundItems="True" AutoPostBack="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblFormaPgto" runat="server" Text="Forma Pagto." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFormaPagto" runat="server" Height="20px" Width="100px" AutoPostBack="true">
                                <asp:ListItem Value="3" Text="Todas"></asp:ListItem>
                                <asp:ListItem Value="0" Text="Pago"></asp:ListItem>
                                <asp:ListItem Value="1" Text="À pagar"></asp:ListItem>
                                <asp:ListItem Value="2" Text="Outros"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblTipoEmissao" runat="server" Text="Tipo Emissão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoEmissao" runat="server" Height="20px" Width="180px"
                                AutoPostBack="true">
                                <asp:ListItem Value="0" Text="Todos"></asp:ListItem>
                                <asp:ListItem Value="1" Text="Normal"></asp:ListItem>
                                <asp:ListItem Value="5" Text="Contingência FSDA"></asp:ListItem>
                                <asp:ListItem Value="7" Text="Autorização pela SVC-RS"></asp:ListItem>
                                <asp:ListItem Value="8" Text="Autorização pela SVC-SP"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblTipoCte" runat="server" Text="Tipo CTe" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoCte" runat="server" Height="20px" Width="220px" AutoPostBack="true">
                                <asp:ListItem Value="4" Text="Todos"></asp:ListItem>
                                <asp:ListItem Value="0" Text="CT-e Normal"></asp:ListItem>
                                <asp:ListItem Value="1" Text="CT-e de Complemento de Valores"></asp:ListItem>
                                <asp:ListItem Value="2" Text="CT-e de Anulação de Valores"></asp:ListItem>
                                <asp:ListItem Value="3" Text="CT-e Substituto"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblTipoServico" runat="server" Text="Tipo Serviço" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoServico" runat="server" Height="20px" Width="180px"
                                AutoPostBack="true">
                                <asp:ListItem Value="4" Text="Todos"></asp:ListItem>
                                <asp:ListItem Value="0" Text="Normal"></asp:ListItem>
                                <asp:ListItem Value="1" Text="Subcontratação"></asp:ListItem>
                                <asp:ListItem Value="2" Text="Redespacho"></asp:ListItem>
                                <asp:ListItem Value="3" Text="Redespacho Intermediário"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblPerEmissao" runat="server" Text="Período Emissão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td class="titulo-filtro">
                            <asp:Label ID="Label1" runat="server" Text="Transportadora" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTransportador" runat="server" AppendDataBoundItems="True" AutoPostBack="true"
                                DataSourceID="odsTransportador" DataTextField="Nome" DataValueField="IdTransportador">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="lblOrdenar" runat="server" Text="Ordenar" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenar" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0" Selected="True">Data de emissão (descresc.)</asp:ListItem>
                                <asp:ListItem Value="1">Data de emissão (cresc.)</asp:ListItem>
                                <asp:ListItem Value="2">Valor Total(cresc.)</asp:ListItem>
                                <asp:ListItem Value="3">Valor Total (descresc.)</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblTipoRemetente" runat="server" Text="Tipo Remetente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoRemetente" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpTipoRemetente_SelectedIndexChanged">
                                <asp:ListItem Value="0" Selected="True">Loja</asp:ListItem>
                                <asp:ListItem Value="1">Fornecedor</asp:ListItem>
                                <asp:ListItem Value="2">Cliente</asp:ListItem>
                                <asp:ListItem Value="3">Transportador</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpRemetente" runat="server" AppendDataBoundItems="true">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton12" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblTipoDestinatario" runat="server" Text="Tipo Destinatário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoDestinatario" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpTipoDestinatario_SelectedIndexChanged">
                                <asp:ListItem Value="0" Selected="True">Loja</asp:ListItem>
                                <asp:ListItem Value="1">Fornecedor</asp:ListItem>
                                <asp:ListItem Value="2">Cliente</asp:ListItem>
                                <asp:ListItem Value="3">Transportador</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpDestinatario" runat="server" AppendDataBoundItems="true">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton11" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblTipoRecebedor" runat="server" Text="Tipo Recebedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoRecebedor" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpTipoRecebedor_SelectedIndexChanged">
                                <asp:ListItem Value="0" Selected="True">Loja</asp:ListItem>
                                <asp:ListItem Value="1">Fornecedor</asp:ListItem>
                                <asp:ListItem Value="2">Cliente</asp:ListItem>
                                <asp:ListItem Value="3">Transportador</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpRecebedor" runat="server" AppendDataBoundItems="true">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <div class="pagina">
        <div style="<%= String.IsNullOrEmpty(GetTipoContingenciaCte()) ? "display: none": "" %>">
            <asp:Label ID="lblContingenciaCTe" runat="server" Text="CTe em Contingência: " Font-Bold="true"
                Font-Size="Medium" ForeColor="Blue"></asp:Label>
        </div>
        <div style="<%= String.IsNullOrEmpty(GetTipoContingenciaCte()) ? "display: none": "" %>">
        </div>
        <div class="grid">
            <asp:GridView GridLines="None" ID="grdCte" runat="server" AllowPaging="True" AllowSorting="True"
                AutoGenerateColumns="False" DataSourceID="odsCte" DataKeyNames="IdCte" EmptyDataText="Nenhum CTe encontrado."
                OnRowCommand="grdCte_RowCommand" CssClass="gridStyle" PagerStyle-CssClass="pgr" EnableViewState="false"
                AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" OnRowDataBound="grdCte_RowDataBound" Width="900px">
                <PagerSettings PageButtonCount="20" />
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:HiddenField ID="hdfIdNf" runat="server" Value='<%# Eval("IdCte") %>' />
                            <asp:HyperLink ID="lnkEditar" runat="server" Visible="false" ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadConhecimentoTransporte.aspx?idCte=" + Eval("IdCte") + "&tipo=" + Eval("TipoDocumentoCte") %>'>
                                      <img border="0" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                            <asp:ImageButton ID="imgExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif" CommandName="Delete" CausesValidation="false"
                                Visible='<%# Eval("ExibirExcluir") %>' OnClientClick="if (!confirm('Deseja excluir esse CT-e?')) return false" />
                            <asp:PlaceHolder ID="phLog" runat="server" Visible="false">
                                <a href="#" onclick="openWindow(450, 700, '../Utils/ShowLogCte.aspx?IdCte=<%# Eval("IdCte") %>&situacao=<%# Eval("Situacao") %>&numero=<%# Eval("NumeroCte") %>'); return false;">
                                    <img src="../Images/blocodenotas.png" title="Log de eventos" border="0" /></a>
                            
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# Eval("PrintDacteVisible") %>'>
                                <a href="#" onclick="openRptDacte('<%# Eval("IdCte") %>', '<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.Cte)Page.GetDataItem()).ObjConhecimentoTransporteRodoviario == null ? false : true %>');">
                                    <img border="0" src="../Images/Relatorio.gif" border="0" /></a> </asp:PlaceHolder>
                            <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Eval("PrintCteTercVisible") %>'>
                                <a href="#" onclick="openRptTerc('<%# Eval("IdCte") %>');">
                                    <img border="0" src="../Images/Relatorio.gif" border="0" /></a></asp:PlaceHolder>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/clipe.gif" Visible='<%# Eval("ExibirDocRef") %>'
                                OnClientClick='<%# "openWindow(600, 800, \"../Utils/DocRefNotaFiscal.aspx?idCte=" + Eval("IdCte") + "\"); return false" %>'
                                ToolTip="Processos/Documentos Referenciados" />
                            <asp:LinkButton ID="lnkConsultaSitLote0" runat="server" CommandName="ConsultaSitLote"
                                Visible='<%# Eval("ConsSitVisible") %>' CommandArgument='<%# Eval("IdCte") %>'>
                                    <img border="0" src="../Images/ConsSitLoteNFe.gif" title="Consulta CTe" border="0" /></asp:LinkButton>
                            <asp:LinkButton ID="lnkConsultaSitCTe0" runat="server" CommandName="ConsultaSitCTe"
                                Visible='<%# Eval("ConsSitVisible") %>' CommandArgument='<%# Eval("IdCte") %>'>
                                    <img border="0" src="../Images/ConsSitNFe.gif" title="Consulta Situação do CTe" border="0" /></asp:LinkButton>
                            <asp:LinkButton ID="lnkSalvarXmlCte" runat="server" Visible='<%# Eval("BaixarXmlVisible") %>'
                                OnClientClick='<%# "salvarCte(\"" + Eval("IdCte") + "\"); return false;" %>'><img border="0" 
                                    src="../Images/disk.gif" title="Salvar arquivo do cte" /></asp:LinkButton>
                            <uc7:ctrlBoleto ID="ctrlBoleto1" runat="server" CodigoCte='<%# Eval("IdCte") != null ? Glass.Conversoes.StrParaInt(Eval("IdCte").ToString()) : (int?)null %>'
                                    Visible='<%# Eval("ExibirBoleto") %>' />
                            <asp:ImageButton ID="imgObsLancFiscal" runat="server" ImageUrl="~/Images/Nota.gif"
                                OnClientClick='<%# "openWindow(600, 800, \"../Utils/SetObsLancFiscal.aspx?idCte=" + Eval("IdCte") + "\"); return false" %>'
                                ToolTip="Observações do Lançamento Fiscal" />
                            <asp:ImageButton ID="imgAjustes" runat="server" ImageUrl="~/Images/dinheiro.gif"
                                OnClientClick='<%# Eval("IdCte", "openWindow(600, 950, \"../Listas/LstAjusteDocumentoFiscal.aspx?idCte={0}\"); return false;") %>'
                                ToolTip="Ajustes do Documento Fiscal" />
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="NumeroCte" HeaderText="Num." SortExpression="NumeroCte" />
                    <asp:BoundField DataField="Modelo" HeaderText="Modelo" SortExpression="Modelo" />
                    <asp:BoundField DataField="CodigoCfop" HeaderText="CFOP" SortExpression="IdCfop" />
                    <asp:BoundField DataField="TipoCteString" HeaderText="Tipo Cte" SortExpression="TipoCte" />
                    <asp:BoundField DataField="TipoEmissaoString" HeaderText="Tipo Emissão" SortExpression="TipoEmissao" />
                    <asp:BoundField DataField="TipoServicoString" HeaderText="Tipo Serviço" SortExpression="TipoServico" />
                    <asp:BoundField DataField="ValorTotal" HeaderText="Valor Tot." SortExpression="ValorTotal" />
                    <asp:BoundField DataField="ValorReceber" HeaderText="Valor Rec." SortExpression="ValorReceber" />
                    <asp:BoundField DataField="DataEmissao" HeaderText="Data Emissão" SortExpression="DataEmissao" />
                    <asp:BoundField DataField="EmitenteCte" HeaderText="Emitente"  />
                    <asp:BoundField DataField="RemetenteCte" HeaderText="Remetente"  />
                    <asp:BoundField DataField="DestinatarioCte" HeaderText="Destinatário"  />
                    <asp:BoundField DataField="ExpedidorCte" HeaderText="Expedidor"  />
                    <asp:BoundField DataField="RecebedorCte" HeaderText="Recebedor"  />
                    <asp:TemplateField HeaderText="Situação" SortExpression="SituacaoString">
                        <ItemTemplate>
                            <table class="pos" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:Label ID="lblSituacao" runat="server" Text='<%# Eval("SituacaoString") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgReabrir" runat="server" ToolTip="Reabrir" Visible='<%# Eval("ExibirReabrir") %>'
                                            ImageUrl="~/Images/Cadeado.gif" CommandName="Reabrir" CommandArgument='<%# Eval("IdCte") %>'
                                            OnClientClick="if (!confirm('Deseja reabrir esse CT-e?')) return false;" />
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="DescricaoTipoDocumentoCte" HeaderText="Tipo Documento CT-e"
                        SortExpression="DescricaoTipoDocumentoCte" />
                    <asp:TemplateField>
                        <ItemTemplate>
                             <asp:ImageButton ID="imbCentroCusto" runat="server" ImageUrl='<%# "~/Images/" + ((bool)Eval("CentroCustoCompleto") ? "cash_blue.png" : "cash_red.png") %>' Visible='<%# Eval("ExibirCentroCusto") %>' 
                                ToolTip="Exibir Centro de Custos" OnClientClick='<%# "exibirCentroCusto(" + Eval("IdCte") + "); return false" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerStyle />
                <EditRowStyle CssClass="edit"></EditRowStyle>
                <AlternatingRowStyle />
            </asp:GridView>
            <div runat="server" id="divContingencia" style="padding: 8px 0">
                <asp:LinkButton ID="lnkDesabilitarContingenciaCTe" runat="server" OnClick="lnkDesabilitarContingenciaCTe_Click"
                    OnClientClick="if (!confirm('Deseja desabilitar o modo de Contingência do CTe?')) return false"> Desabilitar Contingência do CTe</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkAlterarContingenciaCTe" runat="server" OnClick="lnkAlterarContingenciaCTe_Click"
                    OnClientClick="if (!confirm('Deseja habilitar o modo de Contingência SVC do CTe?')) return false"> Habilitar Contingência SVC do CTe</asp:LinkButton>
            </div>
            <div>
                <asp:LinkButton ID="lnkImprimir0" runat="server" CausesValidation="False" OnClientClick="openRpt(false); return false;"> <img alt="" border="0" 
                    src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </div>
            <br />
            <div>
                <asp:LinkButton ID="lkbLoteXml" runat="server" OnClientClick="openLoteCtes(); return false;"> <img border="0" src="../Images/disk.gif" /> Baixar XMLs em Lote</asp:LinkButton>
            </div>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCte" runat="server" DataObjectTypeName="WebGlass.Business.ConhecimentoTransporte.Entidade.Cte"
                EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" EnableViewState="false"
                SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                TypeName="WebGlass.Business.ConhecimentoTransporte.Fluxo.BuscarCte" DeleteMethod="Delete">
                <SelectParameters>
                    <asp:ControlParameter ControlID="txtNumCte" Name="numeroCte" PropertyName="Text"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="cboSituacao" Name="situacao" PropertyName="SelectedValue"
                        Type="String" />
                    <asp:ControlParameter ControlID="drpCfop" Name="idCfop" PropertyName="SelectedValue"
                        Type="UInt32" />
                    <asp:ControlParameter ControlID="drpFormaPagto" Name="formaPagto" PropertyName="SelectedValue"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="drpTipoEmissao" Name="tipoEmissao" PropertyName="SelectedValue"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="drpTipoCte" Name="tipoCte" PropertyName="SelectedValue"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="drpTipoServico" Name="tipoServico" PropertyName="SelectedValue"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="ctrlDataIni" Name="dataEmiIni" PropertyName="DataString"
                        Type="String" />
                    <asp:ControlParameter ControlID="ctrlDataFim" Name="dataEmiFim" PropertyName="DataString"
                        Type="String" />
                    <asp:ControlParameter ControlID="drpTransportador" Name="idTransportador" PropertyName="SelectedValue"
                        Type="UInt32" />
                    <asp:ControlParameter ControlID="drpOrdenar" Name="ordenar" PropertyName="SelectedValue"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="drpTipoRemetente" Name="tipoRemetente" PropertyName="SelectedValue" Type="UInt32" />
                    <asp:ControlParameter ControlID="drpRemetente" Name="idRemetente" PropertyName="SelectedValue" Type="UInt32" />
                    <asp:ControlParameter ControlID="drpTipoDestinatario" Name="tipoDestinatario" PropertyName="SelectedValue" Type="UInt32" />
                    <asp:ControlParameter ControlID="drpDestinatario" Name="idDestinatario" PropertyName="SelectedValue" Type="UInt32" />
                    <asp:ControlParameter ControlID="drpTipoRecebedor" Name="tipoRecebedor" PropertyName="SelectedValue" Type="UInt32" />
                    <asp:ControlParameter ControlID="drpRecebedor" Name="idRecebedor" PropertyName="SelectedValue" Type="UInt32" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCfop" runat="server" SelectMethod="GetSortedByCodInterno"
                TypeName="Glass.Data.DAL.CfopDAO">
            </colo:VirtualObjectDataSource>
            <sync:ObjectDataSource ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
            </sync:ObjectDataSource>
            <sync:ObjectDataSource ID="odsFornecedor" runat="server" EnableViewState="false"
                TypeName="Glass.Data.DAL.FornecedorDAO" SelectMethod="GetOrdered">
            </sync:ObjectDataSource>
            <sync:ObjectDataSource ID="odsCliente" runat="server" EnableViewState="false"
                TypeName="Glass.Data.DAL.ClienteDAO" SelectMethod="GetOrdered">
            </sync:ObjectDataSource>
            <sync:ObjectDataSource ID="odsTransportador" runat="server" SelectMethod="GetOrdered"
                TypeName="Glass.Data.DAL.TransportadorDAO" EnableViewState="false">
            </sync:ObjectDataSource>
        </div>
    </div>
</asp:Content>
