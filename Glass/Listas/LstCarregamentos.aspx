<%@ Page Title="Carregamentos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstCarregamentos.aspx.cs" Inherits="Glass.UI.Web.Listas.LstCarregamentos" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlLogCancPopup" TagPrefix="uc4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRptInd(idCarregamento, exportarExcel) {
            if (idCarregamento == null || idCarregamento == "") {
                alert("Informe o carregamento.");
                return false;
            }

            var ordenar = FindControl("drpOrdenar", "select").value;

            openWindow(500, 700, "../Relatorios/RelBase.aspx?rel=Carregamento&idCarregamento=" + idCarregamento + "&Ordenar=" + ordenar + "&exportarExcel=" + exportarExcel,
                null, true, true); 
             
            return false;
        }

        function openRpt(exportarExcel) {

            var carregamento = FindControl("txtCodCarregamento", "input").value;
            var motorista = FindControl("drpMotorista", "select").value;
            var placa = FindControl("drpVeiculo", "select").value;
            var situacao = FindControl("drpSituacaoCarregamento", "select").itens();
            var dtPrevSaidaIni = FindControl("txtDataPrevSaidaIni", "input").value;
            var dtPrevSaidaFim = FindControl("txtDataPrevSaidaFim", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var idRota = FindControl("drpRota", "select").value;
            var idOC = FindControl("txtCodOC", "input").value;
            var idPedido = FindControl("txtIdPedido", "input").value;

            var queryString = "&carregamento=" + carregamento;
            queryString += "&motorista=" + motorista;
            queryString += "&placa=" + placa;
            queryString += "&situacao=" + situacao;
            queryString += "&dtPrevSaidaIni=" + dtPrevSaidaIni;
            queryString += "&dtPrevSaidaFim=" + dtPrevSaidaFim;
            queryString += "&idLoja=" + idLoja;
            queryString += "&idRota=" + idRota;
            queryString += "&idOC=" + idOC;
            queryString += "&idPedido=" + idPedido;
            queryString += "&exportarExcel=" + exportarExcel;

            openWindow(500, 700, "../Relatorios/RelBase.aspx?rel=ListaCarregamento" + queryString);
            return false;
        }

        function exibirOCs(botao, idCarregamento) {

            var linha = document.getElementById("carregamento_" + idCarregamento);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " OC's";
        }

        function openRptIndOC(idOrdemCarga) {
            if (idOrdemCarga == null || idOrdemCarga == "") {
                alert("Informe a OC.");
                return false;
            }

            openWindow(500, 700, "../Relatorios/RelBase.aspx?rel=OrdemCarga&idOrdemCarga=" + idOrdemCarga);
            return false;
        }

        function adicionarOC(idCarregamento) {

            var queryString = "popup=true";
            queryString += "&idCarregamento=" + idCarregamento;

            openWindow(500, 800, "../Cadastros/CadItensCarregamento.aspx?" + queryString);

            return false;
        }

        function download(filename, text) {
            var pom = document.createElement('a');
            pom.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(text));
            pom.setAttribute('download', filename);

            if (document.createEvent) {
                var event = document.createEvent('MouseEvents');
                event.initEvent('click', true, true);
                pom.dispatchEvent(event);
            }
            else {
                pom.click();
            }
        }

        function faturar(idCarregamento) {                   
            if (!confirm("Faturar carregamento?"))
                return false;

            var retorno = LstCarregamentos.Faturar(idCarregamento);

            if (retorno == null) {
                alert("Erro ao processar ajax.");
                return false;
            }

            if (retorno.error != null) {
                alert(retorno.error.description);
                return false;
            }

            var ret = retorno.value.split("||");

            FindControl("popupAguardar", "div").style.display = "none";
            
            if (ret[0] == "ok") {
                imprimirFaturamento(idCarregamento);
                return true;
            }

            download("Carregamento(" + idCarregamento + ').txt', ret[1]);

            if (ret.length == 3)
                imprimirFaturamento(idCarregamento);

            return true;
        }

        function baixarErrosFaturamento(idCarregamento)
        {
            var retorno = LstCarregamentos.BuscarErrosFaturamentoCarregamento(idCarregamento).value;
            if(retorno != null && retorno != "")
                download("Carregamento(" + idCarregamento + ').txt', retorno);

            return false;
        }

        function imprimirFaturamento(idCarregamento)
        {
            var retorno = LstCarregamentos.BuscarFaturamento(idCarregamento).value;

            str = retorno.split("||");

            if (str[0] == "Erro")
                return alert(str[1]);

            abrirFaturamento(str[1]);
        }

        function abrirFaturamento(lista) {
            openWindow(600, 800, '<%= this.ResolveClientUrl("../Handlers/FaturamentoCarregamento.ashx") %>?idsImprimir=' + lista);
            return false;
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Carregamento" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodCarregamento" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Cód. OC" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodOC" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label16" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label17" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpRota" runat="server" DataSourceID="odsRota" DataTextField="Descricao"
                                DataValueField="IdRota" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Selecione uma Rota</asp:ListItem>
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll"
                                TypeName="Glass.Data.DAL.RotaDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="False"
                                MostrarTodas="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqLoja" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Motorista"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpMotorista" runat="server" DataSourceID="odsMotorista" DataValueField="IdFunc"
                                DataTextField="Nome" AppendDataBoundItems="true">
                                <asp:ListItem Selected="True" Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsMotorista" runat="server" SelectMethod="GetMotoristas"
                                TypeName="Glass.Data.DAL.FuncionarioDAO">
                                <SelectParameters>
                                    <asp:Parameter Name="nome" Type="String" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Veículo" runat="server" ForeColor="#0066FF" Text="Veículo"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpVeiculo" runat="server" DataSourceID="odsVeiculo" DataValueField="Placa"
                                DataTextField="DescricaoCompleta" AppendDataBoundItems="true">
                                <asp:ListItem Value="0" Selected="True">Todos</asp:ListItem>
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsVeiculo" runat="server" SelectMethod="GetOrdered"
                                TypeName="Glass.Data.DAL.VeiculoDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" ForeColor="#0066FF" Text="Situação"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpSituacaoCarregamento" runat="server">
                                <asp:ListItem Value="1">Pedente de Carregamento</asp:ListItem>
                                <asp:ListItem Value="2">Carregado</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" Style="width: 16px" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Data Prev. Saída" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="txtDataPrevSaidaIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="true" />
                        </td>
                        <td nowrap="nowrap">
                            &nbsp;à&nbsp;
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="txtDataPrevSaidaFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label19" runat="server" Text="Ordernar por: " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenar" runat="server">
                                <asp:ListItem Value="1" Selected="True">Ordem de Carga</asp:ListItem>
                                <asp:ListItem Value="2">Cliente</asp:ListItem>
                            </asp:DropDownList>
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
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click"> Gerar Carregamento</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdCarregamento" runat="server" AutoGenerateColumns="False"
                    DataKeyNames="IdCarregamento" DataSourceID="odsCarregamento" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    EmptyDataText="Nenhum carregamento encontrado para o filtro informado." Style="min-width: 1000px;"
                    AllowPaging="True">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif" alt=""></img></asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="return onSave(false);" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir Carregamento" OnClientClick="if(!confirm('Deseja realmente excluir este carregamento?')) return false;" />
                            </ItemTemplate>
                            <EditItemTemplate>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirOCs(this, " + Eval("IdCarregamento") + "); return false" %>'
                                    Width="10px" ToolTip="Exibir OC's" />
                            </ItemTemplate>
                            <EditItemTemplate>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Carregamento">
                            <ItemTemplate>
                                <asp:Label ID="Label15" runat="server" Text='<%# Eval("IdCarregamento") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Motorista">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpMotorista" runat="server" DataSourceID="odsMotorista" DataValueField="IdFunc"
                                    DataTextField="Nome" SelectedValue='<%# Bind("IdMotorista") %>'>
                                </asp:DropDownList>
                                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsMotorista" runat="server" SelectMethod="GetMotoristas"
                                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                                    <SelectParameters>
                                        <asp:Parameter Name="nome" Type="String" />
                                    </SelectParameters>
                                </colo:VirtualObjectDataSource>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Eval("NomeMotorista") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Veículo">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpVeiculo" runat="server" DataSourceID="odsVeiculo" DataValueField="Placa"
                                    DataTextField="DescricaoCompleta" SelectedValue='<%# Bind("Placa") %>'>
                                </asp:DropDownList>
                                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsVeiculo" runat="server" SelectMethod="GetOrdered"
                                    TypeName="Glass.Data.DAL.VeiculoDAO">
                                </colo:VirtualObjectDataSource>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Eval("Veiculo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Prev. Saída">
                            <EditItemTemplate>
                                <uc1:ctrlData ID="txtDtPrevSaida" runat="server" ReadOnly="ReadWrite" ExibirHoras="true"
                                    DataString='<%# Bind("DataPrevistaSaida") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label14" runat="server" Text='<%# Eval("DataPrevistaSaida") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Loja">
                            <ItemTemplate>
                                <asp:Label ID="Label18" runat="server" Text='<%# Eval("NomeLoja") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situacao">
                            <ItemTemplate>
                                <asp:Label ID="Label17" runat="server" Text='<%# Eval("SituacaoStr") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Peso">
                            <ItemTemplate>
                                <asp:Label ID="Label16" runat="server" Text='<%# Eval("Peso") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total">
                            <ItemTemplate>
                                <asp:Label ID="lblValorTotalPedidos" runat="server" Text='<%# ((decimal?)Eval("ValorTotalPedidos")).GetValueOrDefault().ToString("C") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação Faturamento">
                            <ItemTemplate>
                                <asp:Label ID="lblSitFaturamento" runat="server" Visible=<%# Glass.Configuracoes.PCPConfig.HabilitarFaturamentoCarregamento %> 
                                    Text='<%# ((Glass.Data.Model.SituacaoFaturamentoEnum)(int)Eval("SituacaoFaturamento")).ToString() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Faturamento.gif"  Visible=<%# Glass.Configuracoes.PCPConfig.HabilitarFaturamentoCarregamento && ((Glass.Data.Model.SituacaoFaturamentoEnum)(int)Eval("SituacaoFaturamento") != Glass.Data.Model.SituacaoFaturamentoEnum.Faturado)  %> 
                                    ToolTip="Faturar Carregamento" OnClientClick='<%# "faturar("+ Eval("IdCarregamento") +");" %>' />
                                <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="~/Images/printer.png"  Visible=<%# Glass.Configuracoes.PCPConfig.HabilitarFaturamentoCarregamento && ((Glass.Data.Model.SituacaoFaturamentoEnum)(int)Eval("SituacaoFaturamento") == Glass.Data.Model.SituacaoFaturamentoEnum.Faturado)  %> 
                                    ToolTip="Imprimir Faturamento" OnClientClick='<%# "imprimirFaturamento("+ Eval("IdCarregamento") +");" %>' />    
                                <asp:ImageButton ID="ImageButton11" runat="server" ImageUrl="~/Images/ErrosProcesso.png"  Visible=<%# Glass.Configuracoes.PCPConfig.HabilitarFaturamentoCarregamento && ((Glass.Data.Model.SituacaoFaturamentoEnum)(int)Eval("SituacaoFaturamento") == Glass.Data.Model.SituacaoFaturamentoEnum.FaturadoParcialmente)  %> 
                                    ToolTip="Pendências Faturamento" OnClientClick='<%# "baixarErrosFaturamento("+ Eval("IdCarregamento") +");" %>' />  
                                <asp:ImageButton ID="imbRelInd" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    ToolTip="Visualizar Carregamento" OnClientClick='<%# "openRptInd(" + Eval("idCarregamento") + ", false); return false;" %>' />
                                <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Excel.gif"
                                    ToolTip="Visualizar Carregamento" OnClientClick='<%# "openRptInd(" + Eval("idCarregamento") + ", true); return false;" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc3:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="Carregamento" IdRegistro='<%# Eval("idCarregamento") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc4:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" IdRegistro='<%# Eval("idCarregamento") %>'
                                    Tabela="OrdemCarga" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                </td> </tr><asp:HiddenField ID="hdfIdCarregamento" runat="server" Value='<%# Eval("IdCarregamento") %>' />
                                <tr id="carregamento_<%# Eval("IdCarregamento") %>" style="display: none;" class="<%= GetAlternateClass() %>">
                                    <td colspan="13">
                                        <br />
                                        &nbsp; 
                                        <asp:LinkButton ID="lnkAdicionarOC" runat="server" OnClientClick='<%# "return adicionarOC(" + Eval("IdCarregamento") + ");" %>'>Adicionar OC</asp:LinkButton>
                                        <br />
                                        <br />
                                        <asp:GridView ID="grdOrdemCarga" runat="server" AutoGenerateColumns="False" DataKeyNames="IdOrdemCarga"
                                            DataSource='<%# Eval("OCs") %>'
                                            GridLines="None" Width="100%" class="pos" ShowFooter="True"
                                            CellPadding="0" EmptyDataText="Nenhuma ordem de carga encontrada." OnRowDataBound="grdOrdemCarga_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdfIdOC" runat="server" Value='<%# Eval("IdOrdemCarga") %>' />
                                                        <asp:ImageButton ID="imgExcluir" runat="server" CommandName="RetiraOcCarregamento"
                                                            ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Remover OC" OnClientClick="if(!confirm('Deseja realmente remover esta OC do Carregamento?')) return false;"
                                                            OnPreRender="imgExcluir_PreRender" OnCommand="imgExcluir_Command" />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cód. OC">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdOrdemCarga") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Loja">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label9" runat="server" Text='<%# Bind("NomeLoja") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        Total :
                                                    </FooterTemplate>
                                                    <FooterStyle HorizontalAlign="Right" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Peso">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("Peso") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Peso Pendente">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label3" runat="server" Text='<%# Bind("PesoPendenteProducao") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Total M²">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label4" runat="server" Text='<%# Bind("TotalM2") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Itens">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label5" runat="server" Text='<%# Bind("QtdePecasVidro") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Total M² Pendente">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label6" runat="server" Text='<%# Bind("TotalM2PendenteProducao") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Itens Pendentes">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label7" runat="server" Text='<%# Bind("QtdePecaPendenteProducao") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Volumes">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label8" runat="server" Text='<%# Bind("QtdeVolumes") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tipo OC">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label10" runat="server" Text='<%# Bind("TipoOrdemCargaStr") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Situação">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label11" runat="server" Text='<%# Bind("SituacaoStr") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imbRelInd" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                                            ToolTip="Visualizar OC" OnClientClick='<%# "openRptIndOC(" + Eval("IdOrdemCarga") + "); return false;" %>' />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                            </Columns>
                                            <FooterStyle Font-Bold="true" Height="25px" />
                                        </asp:GridView>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCarregamento" runat="server"
                    DataObjectTypeName="Glass.Data.Model.Carregamento" DeleteMethod="Delete" SelectMethod="GetListWithExpression"
                    TypeName="WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetListWithExpressionCount"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" OnDeleted="odsCarregamento_Deleted"
                    OnUpdated="odsCarregamento_Updated" UpdateMethod="Update">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodCarregamento" PropertyName="Text" Name="idCarregamento"
                            DbType="UInt32" />
                        <asp:ControlParameter ControlID="drpRota" Name="idRota" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpMotorista" PropertyName="SelectedValue" Name="idMotorista"
                            DbType="UInt32" />
                        <asp:ControlParameter ControlID="drpVeiculo" PropertyName="SelectedValue" Name="placa"
                            DbType="String" />
                        <asp:ControlParameter ControlID="drpSituacaoCarregamento" PropertyName="SelectedValue"
                            Name="situacao" DbType="String" />
                        <asp:ControlParameter ControlID="txtDataPrevSaidaIni" PropertyName="DataString" Name="dtSaidaIni"
                            DbType="String" />
                        <asp:ControlParameter ControlID="txtDataPrevSaidaFim" PropertyName="DataString" Name="dtSaidaFim"
                            DbType="String" />
                        <asp:ControlParameter ControlID="drpLoja" PropertyName="SelectedValue" Name="idLoja"
                            DbType="Uint32" />
                        <asp:ControlParameter ControlID="txtCodOC" Name="idOC" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtIdPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);"> <img alt="" border="0" 
                    src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"> <img 
                    border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
