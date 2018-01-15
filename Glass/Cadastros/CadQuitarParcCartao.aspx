<%@ Page Title="Quitar Parcelas do Cartão" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadQuitarParcCartao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadQuitarParcCartao" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrltextboxfloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlLogCancPopup" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function validaData(val, args) {
            args.IsValid = isDataValida(args.Value);
        }

        function openRpt(exportarExcel) {
            var agrupar = FindControl("chkAgrupar", "input").checked;
            var idPedido = "", idAcerto = "", idLiberarPedido = "", dataFim = "", numCliente = "", nomeCliente = "",
                tipoEntrega = "", tipoCartao = "", idAcertoCheque = "", nAutorizacao = "", valorIni = "", valorFim = "", tipoRecebCartao = "",
                numAutCartao = "", numEstabCartao = "", ultDigCartao = "";

            if (!agrupar) {
                idPedido = FindControl("txtNumPedido", "input").value;
                idAcerto = FindControl("txtNumAcerto", "input").value;
                idLiberarPedido = FindControl("txtNumLiberacao", "input");
                idLiberarPedido = idLiberarPedido != null ? idLiberarPedido.value : "";
                dataFim = FindControl("ctrlDataFim_txtData", "input").value;
                numCliente = FindControl("txtNumCli", "input").value;
                nomeCliente = FindControl("txtNome", "input").value;
                tipoEntrega = FindControl("drpTipoEntrega", "select").value;
                idAcertoCheque = FindControl("txtAcertoCheque", "input").value;
                dataCadIni = FindControl("ctrlDataCadIni_txtData", "input").value;
                dataCadFim = FindControl("ctrlDataCadFim_txtData", "input").value;
                nCNI = FindControl("txtnumCNI", "input").value;
                valorIni = FindControl("txtValorIni", "input").value;
                valorFim = FindControl("txtValorFim", "input").value;
                tipoRecebCartao = FindControl("drpTipoRecebCartao", "select").value;
                numAutCartao = FindControl("txtNumAutCartao", "input").value;
                numEstabCartao = FindControl("txtNumEstabCartao", "input").value;
                ultDigCartao = FindControl("txtUltDigCartao", "input").value;
                tipoCartao = FindControl("drpTipoCartao", "select").value;
            }
            else {
                tipoCartao = FindControl("drpTipoCartao", "select").value;
            }

            var idLoja = FindControl("drpLoja", "select").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=QuitarParcCartao&idPedido=" + idPedido +
                "&idLiberarPedido=" + idLiberarPedido + "&idAcerto=" + idAcerto + "&dataIni=" + dataIni + "&dataFim=" + dataFim +
                "&idLoja=" + idLoja + "&idCliente=" + numCliente + "&nome=" + nomeCliente + "&tipoEntrega=" + tipoEntrega +
                "&tipoCartao=" + tipoCartao + "&idAcertoCheque=" + idAcertoCheque + "&agrupar=" + agrupar +
                "&dataCadIni=" + dataCadIni + "&dataCadFim=" + dataCadFim + "&nCNI" + nCNI +
                "&valorIni=" + valorIni + "&valorFim=" + valorFim + "&tipoRecebCartao=" + tipoRecebCartao +
                "&numAutCartao=" + numAutCartao + "&numEstabCartao=" + numEstabCartao + "&ultDigCartao=" + ultDigCartao +
                "&exportarExcel=" + exportarExcel);
        }

        function getCli(idCli) {
            if (idCli.value == "") {
                FindControl("txtNome", "input").value = "";
                return;
            }

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');
            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";

                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        var clicked = false;
        function quitarParcela(control) {
            if (clicked)
                return false;

            var conf = confirm("Tem certeza que deseja quitar esta parcela?");

            clicked = conf;

            return conf;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right" nowrap="nowrap" runat="server" id="pedidoTitulo">
                            <asp:Label ID="Label7" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap" runat="server" id="pedido">
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td align="right" nowrap="nowrap" runat="server" id="acertoTitulo">
                            <asp:Label ID="Label15" runat="server" Text="Acerto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap" runat="server" id="acerto">
                            <asp:TextBox ID="txtNumAcerto" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td align="right" nowrap="nowrap" runat="server" id="liberarPedidoTitulo">
                            <asp:Label ID="Label3" runat="server" Text="Liberação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap" runat="server" id="liberarPedido">
                            <asp:TextBox ID="txtNumLiberacao" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label10" runat="server" Text="Venc." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td nowrap="nowrap" runat="server" id="vencDataFim">
                            <uc3:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc4:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="false" />
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" />&nbsp;
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td nowrap="nowrap" runat="server" id="clienteTitulo">
                            <asp:Label ID="Label8" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap" runat="server" id="cliente">
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td runat="server" id="tipoEntregaTitulo">
                            <asp:Label ID="Label14" runat="server" Text="Tipo Entrega" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td runat="server" id="tipoEntrega">
                            <asp:DropDownList ID="drpTipoEntrega" runat="server" AppendDataBoundItems="true"
                                AutoPostBack="true" DataSourceID="odsTipoEntrega" DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem Text="Todas" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td runat="server" id="tipoCartaoTitulo">
                            <asp:Label ID="Label2" runat="server" Text="Tipo Cartão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td runat="server" id="tipoCartao">
                            <asp:DropDownList ID="drpTipoCartao" runat="server" DataSourceID="odsTipoCartao"
                                DataTextField="Descricao" DataValueField="IdTipoCartao"
                                AppendDataBoundItems="True" AutoPostBack="True"
                                OnTextChanged="drpTipoCartao_TextChanged">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgrupar" runat="server" OnCheckedChanged="chkAgrupar_CheckedChanged"
                                Text="Agrupar parcelas" AutoPostBack="True" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label16" runat="server" Text="Acerto de Cheque" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap" runat="server" id="Td1">
                            <asp:TextBox ID="txtAcertoCheque" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                        </td>

                        <td align="right">
                            <asp:Label ID="Label11" runat="server" Text="Data Cad." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataCadIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td nowrap="nowrap" runat="server" id="Td2">
                            <uc3:ctrlData ID="ctrlDataCadFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Valor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorIni" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>até
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorFim" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label17" runat="server" Text="Tipo Receb. Cartão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoRecebCartao" runat="server" DataSourceID="odsTipo"
                                DataTextField="Translation" DataValueField="Key" AppendDataBoundItems="true">
                                 <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td style='<%= Glass.Configuracoes.MenuConfig.ExibirCartaoNaoIdentificado ? "": "display: none" %>'>
                            <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" Text="Nº CNI" CssClass="exibir"></asp:Label>
                        </td>
                        <td style='<%= Glass.Configuracoes.MenuConfig.ExibirCartaoNaoIdentificado ? "": "display: none" %>'>
                            <asp:TextBox ID="txtnumCNI" runat="server" Width="160px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td style='<%= Glass.Configuracoes.MenuConfig.ExibirCartaoNaoIdentificado ? "": "display: none" %>'>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label19" runat="server" ForeColor="#0066FF" Text="Nº Aut."></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumAutCartao" runat="server" Width="70" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click"  />
                        </td>
                        <td style='<%= Glass.Configuracoes.MenuConfig.ExibirCartaoNaoIdentificado ? "": "display: none" %>'>
                            <asp:Label ID="Label20" runat="server" ForeColor="#0066FF" Text="Nº Estabelecimento"></asp:Label>
                        </td>
                        <td style='<%= Glass.Configuracoes.MenuConfig.ExibirCartaoNaoIdentificado ? "": "display: none" %>'>
                            <asp:TextBox ID="txtNumEstabCartao" runat="server" Width="70" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td style='<%= Glass.Configuracoes.MenuConfig.ExibirCartaoNaoIdentificado ? "": "display: none" %>'>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click"  />
                        </td>
                        <td style='<%= Glass.Configuracoes.MenuConfig.ExibirCartaoNaoIdentificado ? "": "display: none" %>'>
                            <asp:Label ID="Label21" runat="server" ForeColor="#0066FF" Text="Ult. Dig. Cartão"></asp:Label>
                        </td>
                        <td style='<%= Glass.Configuracoes.MenuConfig.ExibirCartaoNaoIdentificado ? "": "display: none" %>'>
                            <asp:TextBox ID="txtUltDigCartao" runat="server" Width="50" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td style='<%= Glass.Configuracoes.MenuConfig.ExibirCartaoNaoIdentificado ? "": "display: none" %>'>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click"  />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdConta" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataKeyNames="IdContaR" DataSourceID="odsParcCartao"
                    EmptyDataText="Nenhuma parcela a receber encontrada." OnRowCommand="grdConta_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <table class="pos" cellpadding="0" cellspacing="1">
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtDataQuitar" runat="server" Width="70px" OnLoad="txtDataQuitar_Load"
                                                onkeypress="return mascara_data(event, this), soNumeros(event, true, true);" MaxLength="10"></asp:TextBox>
                                            <asp:ImageButton ID="imgDataQuitar" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                                ToolTip="Alterar" OnLoad="imgDataQuitar_Load" Style="padding-right: 4px" />
                                            <asp:CustomValidator ID="ctvDataQuitar" runat="server" ErrorMessage="*" ClientValidationFunction="validaData"
                                                ControlToValidate="txtDataQuitar" Display="Dynamic" ValidateEmptyText="true"></asp:CustomValidator>
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:DropDownList ID="drpContaBanco" runat="server" DataSourceID="odsContaBanco" AppendDataBoundItems="true"
                                                DataTextField="Descricao" DataValueField="IdContaBanco" OnLoad="drpContaBanco_Load"
                                                SelectedValue='<%# Bind("IdContaBanco") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Button ID="btnQuitar" runat="server" CommandName="Quitar" OnClientClick="return quitarParcela(this);"
                                                Text="Quitar" OnDataBinding="btnQuitar_DataBinding" />
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif"
                                    ToolTip="Atualizar" CausesValidation="false" Height="16px" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referência" >
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Referencia") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Referencia") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Banco" SortExpression="ContaBanco">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("ContaBanco") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("ContaBanco") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cliente" SortExpression="NomeCli">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("IdNomeCli") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("IdNomeCli") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referente a" SortExpression="DescrPlanoConta">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("DescrPlanoConta") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("DescrPlanoConta") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorVec">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("ValorVec", "{0:C}") %>'></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotal" runat="server"></asp:Label>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("ValorVec", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Loja" SortExpression="NomeLoja">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("NomeLoja") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("NomeLoja") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Cartão" SortExpression="DescrTipoCartao">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("DescrTipoCartao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("DescrTipoCartao") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorAgrupado">
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("ValorAgrupado", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("ValorAgrupado", "{0:c}") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vencimento" SortExpression="DataVec">
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("DataVec", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDataVenc" runat="server" Text='<%# Bind("DataVecString") %>' MaxLength="10"
                                    Width="70px" onkeypress="return mascara_data(event, this), soNumeros(event, true, true);"></asp:TextBox>
                                <asp:ImageButton ID="imgDataVenc" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                    OnClientClick="return SelecionaData('txtDataVenc', this)" ToolTip="Alterar" />
                                <asp:CustomValidator ID="ctvData" runat="server" ErrorMessage="*" ClientValidationFunction="validaData"
                                    ControlToValidate="txtDataVenc" Display="Dynamic" ValidateEmptyText="true"></asp:CustomValidator>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs.">
                            <ItemTemplate>
                                <asp:Label ID="lblObs" runat="server" Text='<%# Eval("Obs") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" Text='<%# Bind("Obs") %>' MaxLength="300" Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Cad." SortExpression="DataCad">
                            <ItemTemplate>
                                <asp:Label ID="lblDataCad" runat="server" Text='<%# Bind("DataCad") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblDataCad" runat="server" Text='<%# Eval("DataCad") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Parcela" SortExpression="NumParc">
                            <ItemTemplate>
                                <asp:Label ID="lblNP" runat="server" Text='<%# Bind("NumParc") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblNP" runat="server" Text='<%# Eval("NumParc") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Taxa Juros" >
                            <ItemTemplate>
                                <asp:Label ID="lbltj" runat="server" Text='<%# Bind("TaxaJuros") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lbltj" runat="server" Text='<%# Eval("TaxaJuros") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição Cartão" >
                            <ItemTemplate>
                                <asp:Label ID="lbldc" runat="server" Text='<%# Bind("DescricaoCartao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lbldc" runat="server" Text='<%# Eval("DescricaoCartao") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc2:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" IdRegistro='<%# Eval("IdContaR") %>'
                                    Tabela="ContasReceber" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <br />
                <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="redirectUrl('../Cadastros/CadArquivoQuitacaoParcelaCartao.aspx'); return false">Quitar contas importando arquivo</asp:LinkButton>
                <br /> <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false">
                    <img src="../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoEntrega" runat="server" SelectMethod="GetTipoEntrega"
        TypeName="Glass.Data.Helper.DataSources">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsParcCartao" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetParcCartaoCount" SelectMethod="GetParcCartao" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ContasReceberDAO"
        DataObjectTypeName="Glass.Data.Model.ContasReceber" UpdateMethod="AtualizaObsDataVec">
        <SelectParameters>
            <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text" Type="UInt32" />
            <asp:ControlParameter ControlID="txtNumLiberacao" Name="idLiberarPedido" PropertyName="Text" Type="UInt32" />
            <asp:ControlParameter ControlID="txtNumAcerto" Name="idAcerto" PropertyName="Text" Type="UInt32" />
            <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" Type="UInt32" />
            <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
            <asp:ControlParameter ControlID="drpTipoEntrega" Name="tipoEntrega" PropertyName="SelectedValue" Type="UInt32" />
            <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="ctrlDataIni" Name="dtIni" PropertyName="DataString" Type="String" />
            <asp:ControlParameter ControlID="ctrlDataFim" Name="dtFim" PropertyName="DataString" Type="String" />
            <asp:ControlParameter ControlID="drpTipoCartao" Name="tipoCartao" PropertyName="SelectedValue" Type="UInt32" />
            <asp:ControlParameter ControlID="txtAcertoCheque" Name="idAcertoCheque" PropertyName="Text" Type="UInt32" />
            <asp:ControlParameter ControlID="chkAgrupar" Name="agrupar" PropertyName="Checked" Type="Boolean" />
            <asp:Parameter Name="recebidas" Type="Boolean" DefaultValue="false" />
            <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dtCadIni" PropertyName="DataString" Type="String" />
            <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dtCadFim" PropertyName="DataString" Type="String" />
            <asp:ControlParameter ControlID="txtnumCNI" Name="nCNI" PropertyName="Text" />
            <asp:ControlParameter ControlID="txtValorIni" Name="valorIni" PropertyName="Text" />
            <asp:ControlParameter ControlID="txtValorFim" Name="valorFim" PropertyName="Text" />
            <asp:ControlParameter ControlID="drpTipoRecebCartao" Name="tipoRecbCartao" PropertyName="SelectedValue" />
            <asp:ControlParameter ControlID="txtNumAutCartao" Name="numAutCartao" PropertyName="Text" />
            <asp:ControlParameter ControlID="txtNumEstabCartao" Name="numEstabCartao" PropertyName="Text" />
            <asp:ControlParameter ControlID="txtUltDigCartao" Name="ultDigCartao" PropertyName="Text" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoCartao" runat="server" SelectMethod="ObterListaTipoCartao"
        TypeName="Glass.Data.DAL.TipoCartaoCreditoDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource runat="server" ID="odsTipo"
        SelectMethod="GetTranslatesFromTypeName" EnableViewState="false"
        TypeName="Colosoft.Translator">
        <SelectParameters>
            <asp:Parameter Name="typeName" Type="String"
                DefaultValue="Glass.Data.Model.TipoCartaoEnum, Glass.Data" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetAll"
        TypeName="Glass.Data.DAL.ContaBancoDAO">
    </colo:VirtualObjectDataSource>
</asp:Content>
