<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelCheque.aspx.cs" Inherits="Glass.UI.Web.Utils.SelCheque"
    Title="Selecione os Cheques" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        
        function setCheque(idCheque, idCliente, num, titular, banco, agencia, conta, valor, valorRestante, dataVenc, obs, janela)
        {        
            var tipo = <%= Request["tipo"] %>;
            if (tipo == 1 || tipo == 2)
                window.opener.setCheque(idCheque, num, titular, banco, agencia, conta, valor, dataVenc, obs, janela);
            else if (tipo == 3 || tipo == 5)
                window.opener.setChequeReceb(idCheque, num, titular, banco, agencia, conta, valorRestante, dataVenc, obs, janela, idCliente);
            else
            {
                var nomeTabelaCheques = <%= !String.IsNullOrEmpty(Request["tabelaCheque"]) ? Request["tabelaCheque"] : "'tbChequePagto'" %>;
                var situacao = FindControl("hdfSituacao", "input").value;
                var controlPagto = FindControl("hdfControlPagto", "input").value;
                var tipoCheque = FindControl("drpTipo", "select").value;
                window.opener.setCheque(nomeTabelaCheques, idCheque, null, num, null, titular, valor, dataVenc, banco, agencia, conta, 
                    situacao, obs, window, tipoCheque, 0, 0, 0, 0, 0, 0, 0, 0, controlPagto, "");
            }
            
            var unico = <%= (Request["unico"] == "1").ToString().ToLower() %>;
            if (unico)
                closeWindow();
        }
        
        function adicionaTodos()
        {
            var idPedido = FindControl("txtNumPedido", "input").value == "" ? "0" : FindControl("txtNumPedido", "input").value;
            var idLiberarPedido = FindControl("txtNumLiberarPedido", "input") == null || FindControl("txtNumLiberarPedido", "input").value == "" ? "0" : FindControl("txtNumLiberarPedido", "input").value;
            var idAcerto = FindControl("txtNumAcerto", "input").value == "" ? "0" : FindControl("txtNumAcerto", "input").value;
            var numeroNfe = FindControl("txtNumeroNfe", "input").value == "" ? "0" : FindControl("txtNumeroNfe", "input").value;
            var numCheque = FindControl("txtNumCheque", "input").value == "" ? "0" : FindControl("txtNumCheque", "input").value;
            var reapresentado = (FindControl("chkIncluirReapresentados", "checkbox") == null ? "" : FindControl("chkIncluirReapresentados", "checkbox").checked) ||
                <%= (Request["reapresentados"] == "1").ToString().ToLower() %>;
            var titular = FindControl("txtTitular", "input").value;
            var agencia = FindControl("txtAgencia", "input").value;
            var conta = FindControl("txtConta", "input").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idCli = FindControl("txtNumCli", "input").value == "" ? "0" : FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var idFornec = FindControl("txtIdFornec", "input").value == "" ? "0" : FindControl("txtIdFornec", "input").value;
            var nomeFornec = FindControl("txtNomeFornec", "input").value;
            var valorInicial = FindControl("txtValorInicial", "input").value == "" ? "0" : FindControl("txtValorInicial", "input").value;
            var valorFinal = FindControl("txtValorFinal", "input").value == "" ? "0" : FindControl("txtValorFinal", "input").value;
            var situacao = FindControl("drpSituacao", "select") == null ? <%= drpSituacao.SelectedValue %> :  FindControl("drpSituacao", "select").value;
            var ordenacao = "0";
            
            var tipo = <%= Request["tipo"] %>;

            if (tipo == "5") {
                reapresentado = <%= IsBuscarReapresentados().ToString().ToLower() %> || <%= (Request["reapresentados"] == "1").ToString().ToLower() %>;
                tipo = <%= IsFinanceiroPagto().ToString().ToLower() %> ? "1" : "2";
            }
            else if (tipo != "2")
                tipo = FindControl("drpTipo", "select") != null ? FindControl("drpTipo", "select").value == "" ? "0" : FindControl("drpTipo", "select").value : "0";
                    
            var retorno = SelCheque.ObterLista(idPedido, idLiberarPedido, idAcerto, numeroNfe, tipo, numCheque, situacao, reapresentado, titular, agencia, 
                conta, dataIni, dataFim, idCli, nomeCli, idFornec, nomeFornec, valorInicial, valorFinal, ordenacao).value;
            
            var buffer = retorno.split("|");
            
            for(i = 0; i < buffer.length; i++)
            {
                var aux = buffer[i].split(",");

                if (aux != null && aux.length == 11)
                    setCheque(aux[0], aux[1], aux[2], aux[3], aux[4], aux[5], aux[6], aux[7].replace('.', ','), aux[8].replace('.', ','), aux[9], aux[10], window);
            }
            
            closeWindow();
        }
        
        function limpar()
        {
            // Não faz nada.
            // Função para compatibilidade com o controle de forma de pagamento.
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
        
        function getFornec(idFornec) {
            if (idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
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
                            <asp:Label ID="Label21" runat="server" ForeColor="#0066FF" Text="Liberação"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumLiberarPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
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
                            <asp:Label ID="Label23" runat="server" ForeColor="#0066FF" Text="Nota Fiscal"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumeroNfe" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
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
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Cliente"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label16" runat="server" ForeColor="#0066FF" Text="Titular"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtTitular" runat="server" MaxLength="40" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label17" runat="server" ForeColor="#0066FF" Text="Agência"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAgencia" runat="server" MaxLength="25" Width="100px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" ForeColor="#0066FF" Text="Conta"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtConta" runat="server" MaxLength="20" Width="100px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label11" runat="server" ForeColor="#0066FF" Text="Fornecedor"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtIdFornec" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeFornec" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('ImageButton1', null);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label19" runat="server" ForeColor="#0066FF" Text="Período Venc."></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
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
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label15" runat="server" ForeColor="#0066FF" Text="Situação"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpSituacao_SelectedIndexChanged">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Em Aberto</asp:ListItem>
                                <asp:ListItem Value="2">Compensado</asp:ListItem>
                                <asp:ListItem Value="3">Devolvido</asp:ListItem>
                                <asp:ListItem Value="4">Quitado</asp:ListItem>
                                <asp:ListItem Value="5">Cancelado</asp:ListItem>
                                <asp:ListItem Value="6">Trocado</asp:ListItem>
                                <asp:ListItem Value="7">Protestado</asp:ListItem>
                                <asp:ListItem Value="10">Em Aberto/Devolvido</asp:ListItem>
                                <asp:ListItem Enabled="False" Value="11">Reapresentados</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkIncluirReapresentados" runat="server" Text="Incluir cheques reapresentados"
                                AutoPostBack="True" />
                        </td>
                        <td>
                            <asp:Label ID="Label22" runat="server" ForeColor="#0066FF" Text="Valor"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorInicial" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            até
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
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnFechar" runat="server" Text="Fechar" OnClientClick="closeWindow();" />
            </td>
        </tr>
        <tr>
            <td align="center">
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdCheque" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdCheque" DataSourceID="odsCheques"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="Nenhum cheque encontrado.">
                    <PagerSettings PageButtonCount="30" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="setCheque(<%# Eval("IdCheque") %>, <%# Eval("IdCliente") != null ? Eval("IdCliente") : "0" %>, <%# Eval("Num") %>, '<%# Eval("Titular").ToString().Replace("'", "") %>', '<%# Eval("Banco").ToString().Replace("'", "") %>', '<%# Eval("Agencia").ToString().Replace("'", "") %>', '<%# Eval("Conta").ToString().Replace("'", "") %>', '<%# Eval("Valor", "{0:C}") %>', '<%# Eval("ValorRestante", "{0:C}") %>', '<%# Eval("DataVenc", "{0:d}") %>', '<%# (Eval("Obs") != null ? Eval("Obs") : String.Empty).ToString().Replace("'", "").Replace("\"","").Replace("\r", "").Replace("\n", "") %>', window);">
                                    <img alt="Selecionar" border="0" src="../Images/insert.gif" title="Selecionar" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referência" SortExpression="Referencia">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("Referencia") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("Referencia") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cliente" SortExpression="IdNomeCliente">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("IdNomeCliente") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("IdNomeCliente") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fornecedor" SortExpression="IdNomeFornecedor">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("IdNomeFornecedor") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("IdNomeFornecedor") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Num." SortExpression="Num">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Num") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Banco" SortExpression="Banco">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("Banco") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Agência" SortExpression="Agencia">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Agencia") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Conta" SortExpression="Conta">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Conta") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Titular" SortExpression="Titular">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Titular") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="Valor">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Valor", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ValorReceb" DataFormatString="{0:C}" HeaderText="Valor Recebido"
                            SortExpression="ValorReceb">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DescontoReceb" DataFormatString="{0:C}" HeaderText="Desconto" SortExpression="DescontoReceb">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="DescrSituacao">
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("DescrSituacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Venc." SortExpression="DataVenc">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DataVenc", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Observação." SortExpression="Obs">
                            <ItemTemplate>
                                <asp:Label ID="labelObs" runat="server" Text='<%# Eval("Obs") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCheques" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountSel" SelectMethod="GetForSel" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ChequesDAO" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumLiberarPedido" Name="idLiberarPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumAcerto" Name="idAcerto" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumeroNfe" Name="numeroNfe" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpTipo" DefaultValue="0" Name="tipo" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtNumCheque" Name="numCheque" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="chkIncluirReapresentados" Name="reapresentado" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="txtTitular" Name="titular" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtAgencia" Name="agencia" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtConta" Name="conta" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
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
                        <asp:Parameter Name="ordenacao" Type="Int32" DefaultValue="" />
                        <asp:Parameter Name="chequesCaixaDiario" Type="Boolean" DefaultValue="false" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfSituacao" runat="server" />
                <asp:HiddenField ID="hdfControlPagto" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkAddAll" runat="server" Font-Size="Small" OnClientClick="adicionaTodos();">
                    <img src="../Images/addMany.gif" border="0"> Adicionar Todos</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
