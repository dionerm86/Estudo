<%@ Page Title="Cheques" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstCheque.aspx.cs" Inherits="Glass.UI.Web.Listas.LstCheque" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrltextboxfloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(exportarExcel) {
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idLiberarPedido = FindControl("txtNumLiberarPedido", "input");
            idLiberarPedido = idLiberarPedido != null ? idLiberarPedido.value : "0";
            var idAcerto = FindControl("txtNumAcerto", "input").value;
            var numeroNfe = FindControl("txtNumeroNfe", "input").value;
            var numCheque = FindControl("txtNumCheque", "input").value;
            var situacao = FindControl("cbdSituacao", "select").itens();
            var reapresentado = FindControl("chkReapresentado", "input").checked;
            var advogado = FindControl("drpAdvogados", "select").value;
            var tipo = FindControl("drpTipo", "select").value;
            var titular = FindControl("txtTitular", "input").value;
            var agencia = FindControl("txtAgencia", "input").value;
            var conta = FindControl("txtConta", "input").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var dataCadIni = FindControl("ctrlDataCadIni_txtData", "input").value;
            var dataCadFim = FindControl("ctrlDataCadFim_txtData", "input").value;
            var cpfCnpj = FindControl("txtCpfCnpj", "input").value;
            var idCli = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var idFornec = FindControl("txtIdFornec", "input").value;
            var nomeFornec = FindControl("txtNomeFornec", "input").value;
            var valorInicial = FindControl("txtValorInicial", "input").value;
            var valorFinal = FindControl("txtValorFinal", "input").value;
            var ordenacao = FindControl("drpOrdenar", "select").itens();
            var agrupar = FindControl("chkAgrupar", "input").checked;
            var loja = FindControl("ctrlLoja_drpLoja", "select").value;
            var nomeUsuCad = FindControl("txtNomeUsuCad", "input").value;
            var caixaDiario = <%= Request["caixaDiario"] ?? "false" %>;
            var idsRotas = FindControl("cblRota", "select").itens();
            var obs = FindControl("txtObs", "input").value;

        var queryString = idPedido == "" ? "&idPedido=0" : "&idPedido=" + idPedido;
        queryString += idLiberarPedido == "" ? "&idLiberarPedido=0" : "&idLiberarPedido=" + idLiberarPedido;
        queryString += idAcerto == "" ? "&idAcerto=0" : "&idAcerto=" + idAcerto;
        queryString += numeroNfe == "" ? "&numeroNfe=0" : "&numeroNfe=" + numeroNfe;
        queryString += numCheque == "" ? "&numCheque=0" : "&numCheque=" + numCheque;
        queryString += "&situacao=" + situacao + "&tipo=" + tipo + "&titular=" + titular + "&agencia=" + agencia +
            "&conta=" + conta + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&cpfCnpj=" + cpfCnpj + "&idCli=" + idCli + "&nomeCli=" + nomeCli +
            "&idFornec=" + idFornec + "&nomeFornec=" + nomeFornec + "&valorInicial=" + valorInicial + "&valorFinal=" + valorFinal +
            "&dataCadIni=" + dataCadIni + "&dataCadFim=" + dataCadFim + "&reapresentado=" + reapresentado + "&advogado=" + advogado +
            "&ordenacao=" + ordenacao + "&agrupar=" + agrupar + "&idLoja=" + loja + "&nomeUsuCad=" + nomeUsuCad + "&idsRotas=" 
            + idsRotas + "&exportarExcel=" + exportarExcel + "&chequesCaixaDiario=" + caixaDiario + "&obs=" + obs;

        openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=ListaCheque" + queryString);

        return false;
    }

    function localizacaoCheque(idCheque) {
        openWindow(150, 500, "../Utils/LocalizacaoCheque.aspx?idCheque=" + idCheque);
        return false;
    }
    
    function getCli(idCli) {
        if (idCli.value == "")
            return;

        var retorno = LstCheque.GetCli(idCli.value).value.split(';');
        
        if (retorno[0] == "Erro")
        {
            alert(retorno[1]);
            idCli.value = "";
            FindControl("txtNome", "input").value = "";
            return false;
        }
        
        FindControl("txtNome", "input").value = retorno[1];
    }
    
    function getFornec(idFornec) {
        if (idFornec.value == "")
            return;

        var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');
        
        if (retorno[0] == "Erro")
        {
            alert(retorno[1]);
            idFornec.value = "";
            FindControl("txtNomeFornec", "input").value = "";
            return false;
        }
        
        FindControl("txtNomeFornec", "input").value = retorno[1];
    }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label14" runat="server" ForeColor="#0066FF" Text="Num. Cheque"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCheque" runat="server" Width="100px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" Text="Pedido"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblLiberacao" runat="server" ForeColor="#0066FF" Text="Liberação"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumLiberarPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqLib" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" ForeColor="#0066FF" Text="Num. Acerto"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumAcerto" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblNotaFiscal" runat="server" ForeColor="#0066FF" Text="Nota Fiscal"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumeroNfe" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label20" runat="server" ForeColor="#0066FF" Text="Tipo"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipo" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Próprio</asp:ListItem>
                                <asp:ListItem Selected="True" Value="2">Terceiros</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton16" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label15" runat="server" ForeColor="#0066FF" Text="Situação"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdSituacao" runat="server" CheckAll="False" DataSourceID="odsSituacaoCheque"
                                DataTextField="Descr" DataValueField="Id" ImageURL="~/Images/DropDown.png" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"
                                OpenOnStart="False" Title="Selecione a situação" Width="135px">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label23" runat="server" ForeColor="#0066FF" Text="Loja"></asp:Label>
                        </td>
                        <td>
                            <uc4:ctrlLoja ID="ctrlLoja" runat="server" MostrarTodas="True" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblRota" runat="server" Width="110px" CheckAll="False"
                                Title="Selecione a rota" DataSourceID="odsRota" DataTextField="Descricao" DataValueField="IdRota"
                                ImageURL="~/Images/DropDown.png" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"
                                OpenOnStart="False">
                            </sync:CheckBoxListDropDown>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll"
                                TypeName="Glass.Data.DAL.RotaDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label26" runat="server" ForeColor="#0066FF" Text="CPF/CNPJ"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCpfCnpj" runat="server" MaxLength="20" Width="100px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton15" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Cliente"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" ForeColor="#0066FF" Text="Fornecedor"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtIdFornec" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeFornec" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('ImageButton1', null);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label16" runat="server" ForeColor="#0066FF" Text="Titular"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtTitular" runat="server" MaxLength="40" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton12" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label17" runat="server" ForeColor="#0066FF" Text="Agência"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAgencia" runat="server" MaxLength="25" Width="100px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton13" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" ForeColor="#0066FF" Text="Conta"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtConta" runat="server" MaxLength="20" Width="100px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton14" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label21" runat="server" ForeColor="#0066FF" Text="Valor"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorInicial" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>até
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorFinal" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label25" runat="server" ForeColor="#0066FF" Text="Usuário Cad."></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeUsuCad" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton11" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label22" runat="server" ForeColor="#0066FF" Text="Período Cad."></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataCadIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataCadFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label19" runat="server" ForeColor="#0066FF" Text="Período Venc."></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Label ID="lblObs" runat="server" ForeColor="#0066FF" Text="Observação:"></asp:Label>
                        </td>
                        <td align="center">
                            <asp:TextBox ID="txtObs" runat="server"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:ImageButton ID="ImageButton17" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Cheques Advogados"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpAdvogados" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0" Selected="True">Incluir Advogados</asp:ListItem>
                                <asp:ListItem Value="1">Apenas Advogados</asp:ListItem>
                                <asp:ListItem Value="2">Não Advogados</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkReapresentado" runat="server" Text="Incluir Reapresentados" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Ordernar relatório por:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpOrdenar" runat="server" CheckAll="False" ImageURL="~/Images/DropDown.png" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"
                                OpenOnStart="False" Title="Selecione a ordenação" Width="135px">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Value="1">Titular</asp:ListItem>
                                <asp:ListItem Value="2">Data venc.</asp:ListItem>
                                <asp:ListItem Value="3">Banco</asp:ListItem>
                                <asp:ListItem Value="4">Pedido</asp:ListItem>
                                <asp:ListItem Value="5">Valor</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgrupar" runat="server" Text="Agrupar impressão por cliente."
                                Checked="True" />
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
                <asp:GridView GridLines="None" ID="grdCheque" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    DataKeyNames="IdCheque" DataSourceID="odsCheques" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhum cheque encontrado."
                    OnRowCommand="grdCheque_RowCommand">
                    <PagerSettings PageButtonCount="30" />
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="ImageButton4" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="ImageButton5" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                                <asp:HiddenField ID="hdfTipo" runat="server" Value='<%# Bind("Tipo") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEditar" runat="server" CommandName="Edit" ToolTip="Editar"><img border="0" src="../Images/EditarGrid.gif" /></asp:LinkButton>
                                <asp:PlaceHolder ID="pchAnexos" runat="server"><a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=<%# Eval("IdCheque") %>&tipo=cheque&#039;); return false;'>
                                    <img border="0px" src="../Images/Clipe.gif"></img></a></asp:PlaceHolder>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referência" SortExpression="Referencia">
                            <EditItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Eval("Referencia") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("Referencia") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Loja" SortExpression="IdLoja">
                            <ItemTemplate>
                                <asp:Label ID="Label24" runat="server" Text='<%# Eval("NomeLoja") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label24" runat="server" Text='<%# Eval("NomeLoja") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cliente">
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("IdNomeCliente") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("IdNomeCliente") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fornecedor" SortExpression="IdFornecedor">
                            <EditItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Eval("IdNomeFornecedor") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("IdNomeFornecedor") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Num." SortExpression="Num">
                            <EditItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("Num") %>' Visible='<%# !(bool)Eval("EditarAgenciaConta") %>'></asp:Label>
                                <asp:TextBox ID="txtNum" runat="server" onchange="FindControl('hdfNum', 'input').value = this.value"
                                    Text='<%# Eval("Num") %>' Width="50px"></asp:TextBox>
                                <asp:HiddenField ID="hdfNum" runat="server" Value='<%# Bind("Num") %>' />
                                <asp:TextBox ID="txtDigitoNum" runat="server" onchange="FindControl('hdfDigitoNum', 'input').value = this.value"
                                    Text='<%# Eval("DigitoNum") %>' Width="50px"></asp:TextBox>
                                <asp:HiddenField ID="hdfDigitoNum" runat="server" Value='<%# Bind("DigitoNum") %>' />
                                <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Bind("IdCliente") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("NumChequeComDig") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Banco" SortExpression="Banco">
                            <EditItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("Banco") %>' Visible='<%# !(bool)Eval("EditarAgenciaConta") %>'></asp:Label>
                                  <asp:TextBox ID="txtBanco" runat="server" onchange="FindControl('hdfBanco', 'input').value = this.value"
                                       Text='<%# Eval("Banco") %>'  Width="50px"></asp:TextBox>
                                <asp:HiddenField ID="hdfBanco" runat="server" Value='<%# Bind("Banco") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("Banco") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Agência" SortExpression="Agencia">
                            <EditItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("Agencia") %>' Visible='<%# !(bool)Eval("EditarAgenciaConta") %>'></asp:Label>
                                <asp:TextBox ID="txtAgencia" runat="server" onchange="FindControl('hdfAgencia', 'input').value = this.value"
                                    Text='<%# Eval("Agencia") %>' Visible='<%# Eval("EditarAgenciaConta") %>' Width="50px"></asp:TextBox>
                                <asp:HiddenField ID="hdfAgencia" runat="server" Value='<%# Bind("Agencia") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("Agencia") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Conta" SortExpression="Conta">
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("Conta") %>' Visible='<%# !(bool)Eval("EditarAgenciaConta") %>'></asp:Label>
                                <asp:TextBox ID="txtConta" runat="server" onchange="FindControl('hdfConta', 'input').value = this.value"
                                    Text='<%# Eval("Conta") %>' Visible='<%# Eval("EditarAgenciaConta") %>' Width="70px"></asp:TextBox>
                                <asp:HiddenField ID="hdfConta" runat="server" Value='<%# Bind("Conta") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("Conta") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Titular" SortExpression="Titular">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtTitular" runat="server" MaxLength="45" Text='<%# Bind("Titular") %>'
                                    Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("Titular") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CPF/CNPJ" SortExpression="CpfCnpj">
                            <EditItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("CpfCnpjFormatado") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("CpfCnpjFormatado") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="Valor">
                            <EditItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("ValorRecebido") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("ValorRecebido") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Venc." SortExpression="DataVenc">
                            <ItemTemplate>
                                <asp:Label ID="lblDataVenc" runat="server" Text='<%# Eval("DataVencLista") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDataVencGrid" runat="server" onkeypress="return false;" Text='<%# Bind("DataVenc") %>'
                                    Width="70px" Visible='<%# Eval("AlterarDataVenc") %>'></asp:TextBox>
                                <asp:ImageButton ID="imgDataVencGrid" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                    ToolTip="Alterar" OnClientClick="return SelecionaData('txtDataVencGrid', this)"
                                    Visible='<%# Eval("AlterarDataVenc") %>' />
                                <asp:Label ID="lblDataVenc" runat="server" Text='<%# Eval("DataVencLista") %>' Visible='<%# !(bool)Eval("AlterarDataVenc") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs" SortExpression="Obs">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Text='<%# Bind("Obs") %>'
                                    Width="200px" Rows="2" TextMode="MultiLine"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("Obs") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="DescrSituacao">
                            <EditItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("DescrSituacao") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("DescrSituacao") %>'></asp:Label>
                                <br />
                                <asp:LinkButton ID="lnkReapresentado" runat="server" CommandArgument='<%# Eval("IdCheque") %>'
                                    CommandName="CancelarReapresentado" OnClientClick='<%# "return confirm(\"Tem certeza que deseja cancelar a reapresentação?\");"%>'
                                    Visible='<%# Eval("CancelarReapresentadoVisible") %>'>Cancelar Reapresentado ?</asp:LinkButton>
                                <asp:LinkButton ID="LinkButton1" runat="server" Style="white-space: nowrap" CommandArgument='<%# Eval("IdCheque") %>'
                                    CommandName="CancelarDevolucao" Visible='<%# Eval("ExibirCancelarDevolucao") %>'
                                    OnClientClick="if (!confirm(&quot;Deseja cancelar a devolução desse cheque?&quot;)) return false">Cancelar Devolução</asp:LinkButton>
                                <asp:LinkButton ID="LinkButton2" runat="server" Style="white-space: nowrap" CommandArgument='<%# Eval("IdCheque") %>'
                                    CommandName="DesmarcarProtestado" Visible='<%# Eval("ExibirDesmarcarProtestado") %>'
                                    OnClientClick="if (!confirm(&quot;Deseja cancelar o protesto desse cheque?&quot;)) return false">Cancelar Protesto</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:PlaceHolder ID="pchLocalizacao" runat="server"><a href="#" onclick='localizacaoCheque(<%# Eval("IdCheque") %>);'>
                                    <img src="../Images/environment.gif" border="0" title="Localização" /></a> </asp:PlaceHolder>
                                <uc2:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="Cheque" IdRegistro='<%# Eval("IdCheque") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCheques" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountFilter" SelectMethod="GetByFilter" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ChequesDAO" DataObjectTypeName="Glass.Data.Model.Cheques"
                    UpdateMethod="AlterarDados">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumLiberarPedido" Name="idLiberarPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumAcerto" Name="idAcerto" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumeroNfe" Name="numeroNfe"
                            PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpTipo" DefaultValue="0" Name="tipo" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtNumCheque" Name="numCheque" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="cbdSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="chkReapresentado" Name="reapresentado" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="drpAdvogados" Name="advogado" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtTitular" Name="titular" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtAgencia" Name="agencia" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtConta" Name="conta" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtCpfCnpj" Name="cpfCnpj" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtIdFornec" Name="idFornec" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeFornec" Name="nomeFornec" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtValorInicial" Name="valorInicial" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="txtValorFinal" Name="valorFinal" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="drpOrdenar" Name="ordenacao" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="txtNomeUsuCad" Name="nomeUsuCad" PropertyName="Text"
                            Type="String" />
                        <asp:QueryStringParameter Name="chequesCaixaDiario" Type="Boolean" QueryStringField="caixaDiario" DefaultValue="false" />
                        <asp:ControlParameter ControlID="cblRota" Name="idsRotas" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtObs" Name="obs" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacaoCheque" runat="server" SelectMethod="GetSituacaoCheque"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfAdvogado" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
