 <%@ Page Title="Medi��es" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstMedicao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstMedicao" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<%@ Register src="../Controls/ctrlLogCancPopup.ascx" tagname="ctrlLogCancPopup" tagprefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript">
        function cancelar(idMedicao) {
            openWindow(150, 420, "../Utils/SetMotivoCancMedicao.aspx?idMedicao=" + idMedicao);
        }

        function getMedidor(idMedidor) {
            if (idMedidor.value == "") {
                openWindow(500, 700, "../Utils/SelMedidor.aspx");
                return false;
            }

            var retorno = MetodosAjax.GetMedidor(idMedidor.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idMedidor.value = "";
                FindControl("txtNomeMedidor", "input").value = "";
                return false;
            }

            FindControl("txtNomeMedidor", "input").value = retorno[1];
        }

        // Fun��o utilizada ap�s selecionar medidor no popup, para preencher o id e o nome do mesmo
        // Nas respectivas textboxes deste form
        function setMedidor(id, nome) {
            FindControl("txtNumMedidor", "input").value = id;
            FindControl("txtNomeMedidor", "input").value = nome;
            return false;
        }

        function openRptMedicao(idMedicao) {
            openWindow(600, 800, "../Relatorios/RelMedicao.aspx?idMedicao=" + idMedicao);
            return false;
        }

        function openRpt(exportarExcel) {
            var idMedicao = FindControl("txtNumMedicao", "input").value;
            var idOrcamento = FindControl("txtIdOrcamento", "input").value;
            var idPedido = FindControl("txtIdPedido", "input").value;
            var idMedidor = FindControl("txtNumMedidor", "input").value;
            var nomeMedidor = FindControl("txtNomeMedidor", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNomeCli", "input").value;
            var telefone = FindControl("txtTelefone", "input").value;
            var dtIni = FindControl("txtDataIni", "input").value;
            var dtFim = FindControl("txtDataFim", "input").value;
            var dtEfetuar = FindControl("txtEfetuarEm", "input").value;
            var bairro = FindControl("txtBairro", "input").value;
            var endereco = FindControl("txtEndereco", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var idVendedor = FindControl("drpVendedor", "select").value;
            var obs = FindControl("txtObs", "input").value;
            var agrupar = FindControl("drpAgrupar", "select").value;
            var ordenarPor = FindControl("drpOrdenar", "select").value;

            if (idMedicao == "") idMedicao = 0; 
            if (idMedidor == "") idMedidor = 0;
            if (idVendedor == "") idVendedor = 0;           

            var queryString = "?Rel=ListaMedicao&IdMedicao=" + idMedicao + "&idOrcamento=" + idOrcamento + "&idPedido=" + idPedido +
                "&IdMedidor=" + idMedidor + "&NomeMedidor=" + nomeMedidor + "&idCliente=" + idCliente + "&NomeCliente=" + nomeCliente + "&situacao=" + situacao +
                "&dataIni=" + dtIni + "&dataFim=" + dtFim + "&dataEfetuar=" + dtEfetuar + "&endereco=" + endereco + "&bairro=" + bairro +
                "&idloja=" + idLoja + "&IdVendedor=" + idVendedor + "&telefone=" + telefone + "&obs=" + obs + "&agrupar=" + agrupar + "&ordenarPor=" + ordenarPor + "&exportarExcel=" + exportarExcel;

            openWindow(600, 800, "../Relatorios/RelBase.aspx" + queryString);
            return false;
        }

        // Finaliza a medi��o passada e abre or�amento criado pela mesma
        function finalizar(idMedicao) {
            if (confirm("Tem certeza que deseja finalizar esta medi��o?") == false)
                return false;

            var retorno = LstMedicao.Finalizar(idMedicao).value.split('\t');

            if (retorno[0] == "ok") {
                /* Chamado 50817. */
                if (parseInt(retorno[1]) > 0)
                    openRptOrca(retorno[1]);
                else
                    openRptMedicao(idMedicao);

                redirectUrl(window.location.href);
            }
            else {
                alert(retorno[1]);
                return false;
            }
        }

        // Abre o relat�rio do or�amento gerado ap�s finalizada determinada medi��o
        function openRptOrca(idOrca) {
            openWindow(600, 800, "../Relatorios/RelOrcamento.aspx?idOrca=" + idOrca);
        }

        function getCli(abrirPopup) {
            var idCliente = FindControl("txtNumCli", "input");
            var nomeCliente = FindControl("txtNomeCli", "input");

            if (idCliente.value == "" && nomeCliente.value == "" && abrirPopup) {
                openWindow(600, 800, "../Utils/SelCliente.aspx?custom=1");
                return false;
            }

            if (idCliente.value == "")
                return false;

            var retorno = MetodosAjax.GetCli(idCliente.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCliente.value = "";
                nomeCliente.value = "";
                return false;
            }

            nomeCliente.value = retorno[1];

            return false;
        }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label12" runat="server" ForeColor="#0066FF" Text="Num. Medi��o"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtNumMedicao" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="imgPesqMedidor0" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right">
                            <asp:Label ID="lblIdOrcamento" runat="server" ForeColor="#0066FF" Text="Num. Or�amento"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtIdOrcamento" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right">
                            <asp:Label ID="lblIdPedido" runat="server" ForeColor="#0066FF" Text="Num. Pedido"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtIdPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label11" runat="server" ForeColor="#0066FF" Text="Situa��o"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Aberta</asp:ListItem>
                                <asp:ListItem Value="2">Em Andamento</asp:ListItem>
                                <asp:ListItem Value="3">Finalizada</asp:ListItem>
                                <asp:ListItem Value="4">Remarcada</asp:ListItem>
                                <asp:ListItem Value="5">Cancelada</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Per�odo"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtDataIni" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido0" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataIni', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataFim" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido1" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataFim', this)" ToolTip="Alterar" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Medidor"></asp:Label>&nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumMedidor" runat="server" Width="50px" onblur="getMedidor(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeMedidor" runat="server" Width="150px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqMedidor" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="getMedidor(FindControl('txtNumMedidor', 'input'));" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(false);"></asp:TextBox>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtNomeCli" runat="server" Width="180px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td align="left">
                            <asp:LinkButton ID="lnkPesquisar0" runat="server" OnClick="lnkPesquisar_Click"><img border="0" 
                                src="../Images/Pesquisar.gif" onclick="getCli(true);" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label17" runat="server" ForeColor="#0066FF" Text="Telefone"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtTelefone" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                Width="80px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq5" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" Text="A Efetuar em"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtEfetuarEm" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataEfetuar" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtEfetuarEm', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" ForeColor="#0066FF" Text="Vendedor"></asp:Label>
                        </td>
                        <td>
                            
                            <asp:DropDownList ID="drpVendedor" runat="server" DataSourceID="odsVendedor" DataTextField="Nome"
                                DataValueField="IdFunc" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                            
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label16" runat="server" ForeColor="#0066FF" Text="Loja"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:Label ID="Label14" runat="server" ForeColor="#0066FF" Text="Bairro"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtBairro" runat="server" MaxLength="30" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label15" runat="server" ForeColor="#0066FF" Text="Endere�o"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtEndereco" runat="server" MaxLength="50" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                                    </table>
                <table>
                    <tr align="center">
                        <td align="left">
                            <asp:Label ID="lblObs" runat="server" ForeColor="#0066FF" Text="Observa��o:"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtObs" runat="server"></asp:TextBox>
                        </td>
                        <td align="left">
                            <asp:ImageButton ID="ImageButton13" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                         <td>
                            <asp:Label ID="Label6" runat="server" Text="Agrupar Relat�rio por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpAgrupar" runat="server">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Value="1">Data Medi��o</asp:ListItem>                                
                            </asp:DropDownList>
                        </td>
                         <td align="left">
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Ordenar por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenar" runat="server">
                                <asp:ListItem Value=""></asp:ListItem>
                                <asp:ListItem Value="1">C�d. Medi��o Cresc</asp:ListItem>
                                <asp:ListItem Value="2">C�d. Medi��o Decr</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="left">
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
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
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir Medi��o</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdMedicao" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsMedicao"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataKeyNames="IdMedicao" 
                    EmptyDataText="Nenhuma medi��o encontrada.">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadMedicao.aspx?idMedicao=" + Eval("IdMedicao") %>'
                                    Visible='<%# Eval("EditVisible") %>'>
                                <img border="0" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                                <asp:LinkButton ID="lnkExcluir" runat="server" ToolTip="Cancelar" Visible='<%# Eval("DeleteVisible") %>'
                                    OnClientClick='<%# "cancelar(" + Eval("IdMedicao") + "); return false;" %>'>
                                <img src="../Images/ExcluirGrid.gif" border="0"></asp:LinkButton>
                                <asp:PlaceHolder ID="pchFotos" runat="server" Visible='<%# Eval("FotosVisible") %>'>
                                    <a href="#" onclick="openWindow(600, 700, '../Cadastros/CadFotos.aspx?id=<%# Eval("IdMedicao") %>&tipo=medicao'); return false;">
                                        <img src="../Images/Clipe.gif" border="0px"></a></asp:PlaceHolder>
                                <asp:PlaceHolder ID="pchRptMedicao" runat="server"><a href="#" onclick="openWindow(600, 700, '../Relatorios/RelMedicao.aspx?IdMedicao=<%# Eval("IdMedicao") %>'); return false;">
                                    <img src="../Images/Relatorio.gif" border="0px"></a></asp:PlaceHolder>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdMedicao" HeaderText="C�d." SortExpression="IdMedicao" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="IdOrcamento" HeaderText="Or�a." SortExpression="IdOrcamento" />
                        <asp:BoundField DataField="IdPedido" HeaderText="Ped." SortExpression="IdPedido" />
                        <asp:BoundField DataField="NomeMedidor" HeaderText="Medidor" SortExpression="NomeMedidor" />
                        <asp:BoundField DataField="NomeVendedor" HeaderText="Vendedor" SortExpression="NomeVendedor" />
                        <asp:BoundField DataField="IdNomeCliente" HeaderText="Cliente" SortExpression="IdNomeCliente" />
                        <asp:BoundField DataField="EnderecoCompleto" HeaderText="Endere�o" SortExpression="EnderecoCompleto" />
                        <asp:BoundField DataField="TelCliente" HeaderText="Tel. Cliente" SortExpression="TelCliente" />
                        <asp:BoundField DataField="DataMedicao" HeaderText="Data Medi��o" SortExpression="DataMedicao"
                            DataFormatString="{0:d}"></asp:BoundField>
                        <asp:BoundField DataField="DataEfetuar" DataFormatString="{0:d}" HeaderText="Data Efetuar"
                            SortExpression="DataEfetuar" />
                        <asp:BoundField DataField="DescrTurno" HeaderText="Turno" SortExpression="DescrTurno">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Hora" HeaderText="Hora" SortExpression="Hora" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situa��o" SortExpression="DescrSituacao" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick='TagToTip("medicao_<%# Eval("IdMedicao") %>", FADEIN, 300, COPYCONTENT, false, TITLE, "Detalhes", CLOSEBTN, true, CLOSEBTNTEXT, "Fechar", CLOSEBTNCOLORS, ["#cc0000", "#ffffff", "#D3E3F6", "#0000cc"], STICKY, true, FIX, [this, 10, 0]); return false;'>
                                    <img src="../Images/user_comment.gif" border="0" alt="Detalhes" /></a>
                                <div id="medicao_<%# Eval("IdMedicao") %>" style="display: none">
                                    <asp:Label ID="Label1" runat="server" Text='<%# "Data de cadastro: " + Eval("DataCad", "{0:d}") %>'></asp:Label><br />
                                    <asp:Label ID="Label2" runat="server" Text='<%# "Usu�rio Cadastro: " + Eval("NomeFuncCad") %>'></asp:Label><br />
                                </div>

                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# Eval("FinalizarVisible") %>'>
                                    <a href="#" onclick="return finalizar('<%# Eval("IdMedicao") %>');">Finalizar</a></asp:PlaceHolder>
                            <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="Medicao" IdRegistro='<%# Eval("IdMedicao") %>' />
                            <uc1:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" IdRegistro='<%# Eval("IdMedicao") %>' Tabela="Medicao" />
                            
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);"> <img alt="" border="0" src="../Images/printer.png" />Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"> <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                &nbsp;
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMedicao" runat="server"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" OnDeleted="odsMedicao_Deleted"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.MedicaoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumMedicao" Name="idMedicao" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtIdOrcamento" Name="idOrcamento" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtIdPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumMedidor" Name="idMedidor" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeMedidor" Name="nomeMedidor" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpVendedor" Name="idVendedor" 
                            PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSituacao" DefaultValue="" Name="situacao" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtDataIni" DefaultValue="" Name="dataIni" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataFim" Name="dataFim" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtEfetuarEm" Name="dataEfetuar" PropertyName="Text"
                            Type="String" />
                         <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeCli" Name="nomeCli" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtBairro" Name="bairro" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtEndereco" Name="endereco" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtTelefone" Name="telefone" 
                            PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtObs" Name="obs" PropertyName="Text" />      
                        <asp:ControlParameter ControlID="drpOrdenar" Name="ordenarPor"
                            PropertyName="SelectedValue" Type="UInt32" />                
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsVendedor" runat="server" 
                    SelectMethod="GetVendedoresEMedicao" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
