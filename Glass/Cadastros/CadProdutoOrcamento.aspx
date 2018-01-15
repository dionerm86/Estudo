<%@ Page Title="Produtos" Language="C#" AutoEventWireup="true" CodeBehind="CadProdutoOrcamento.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadProdutoOrcamento" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrllinkquerystring"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrlTextBoxFloat" TagPrefix="uc1" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        var editar = <%= (!String.IsNullOrEmpty(Request["editar"])).ToString().ToLower() %>;
        var idProduto = <%= !String.IsNullOrEmpty(Request["idProd"]) ? Request["idProd"] : "0" %>;
        var subProdutos = false;
        var mudarDescr = true;
        var tempoSetTimeout = 500;
        var cadastrando = false;
        var atualizarDescricaoPaiAoInserirOuRemoverProduto = <%= AtualizarDescricaoPaiAoInserirOuRemoverProduto().ToString().ToLower() %>;
        
        function validaAmbiente(val, args)
        {
            args.IsValid = subProdutos ? args.Value != "" : true;
        }
        
        function getOrcRapidoLocation()
        {
            return '../Listas/LstOrcamentoRapido.aspx?popup=true&IdOrca=<%= Request["IdOrca"] %>&PercComissao=<%= GetPercComissao() %>' +
                '&IdProdOrca=<%= !String.IsNullOrEmpty(Request["editar"]) ? Request["IdProd"] : "" %>&tipoEntrega=<%= Request["tipoEntrega"] %>&revenda=<%= GetRevenda() %>' +
                '&LiberarOrcamento=<%= Request["LiberarOrcamento"] %>&callbackIncluir=incluir&callbackExcluir=excluir' +
                '&idCliente=<%= Request["idCliente"] %>';
        }
    
        function inserirProdutos(link)
        {
            var orcRapido = document.getElementById("orcRapido");
            
            if (!subProdutos)
            {
                subProdutos = true;
                link.style.display = "none";
                
                var detailsView = document.getElementById("<%= dtvLoja.ClientID %>");
                detailsView.style.width = "900px";
                
                for (i = 0; i < 2; i++)
                    detailsView.rows[i].cells[0].style.paddingLeft = "95px";
                    
                for (i = 2; i < 4; i++)
                    detailsView.rows[i].style.display = "none";
        
                orcRapido.contentWindow.location = getOrcRapidoLocation();
                orcRapido.style.display = "inline";
            }
            
            if (orcRapido.contentWindow.location.href.indexOf("LstOrcamentoRapido.aspx") == -1 || orcRapido.contentDocument.body == null || orcRapido.contentDocument.body.scrollHeight <= 150)
            {
                setTimeout(function() { inserirProdutos(link) }, tempoSetTimeout);
                return;
            }
            
            orcRapido.style.height = (orcRapido.contentDocument.body.scrollHeight + 10) + "px";
            
            FindControl("txtQtd", "input").value = "1";
            if (FindControl("ctrlTextBoxFloat1_txtNumber", "input").value == "")
                FindControl("ctrlTextBoxFloat1_txtNumber", "input").value = "0,00";
            
            mudarDescr = atualizarDescricaoPaiAoInserirOuRemoverProduto;
        }
        
        function incluir(idLinha, dadosProduto)
        {
            var orcRapido = document.getElementById("orcRapido");
            var orcRapidoW = orcRapido.contentWindow;
            var orcRapidoD = orcRapido.contentDocument;
            var row = orcRapidoD.getElementById(idLinha);
            
            while (dadosProduto.indexOf('~') >= 0)
                dadosProduto = dadosProduto.replace('~', '\t');
                
            orcRapido.style.height = orcRapidoD.body.scrollHeight + "px";
            
            if (editar && idProduto > 0)
            {
                var resposta = orcRapidoW.LstOrcamentoRapido.IncluirProdutoOrcamento(idProduto, dadosProduto).value;
                var dadosResposta = resposta.split('\t');
                if (dadosResposta[0] == "Erro")
                {
                    alert(dadosResposta[1]);
                    return;
                }
                
                row.setAttribute("idProdOrcamento", resposta[2]);
            }
            else
            {
                var dadosProdutos = FindControl("hdfDadosProdutos", "input");
                dadosProdutos.value += dadosProduto + "\n";
            }
            
            if (mudarDescr)
            {
                if (FindControl("txtDescr", "textarea").value != "" && FindControl("txtDescr", "textarea").value.lastIndexOf(";") < (FindControl("txtDescr", "textarea").value.length - 2))
                    FindControl("txtDescr", "textarea").value += "; ";
            
                FindControl("txtDescr", "textarea").value += dadosProduto.split('\t')[9];
            }
            
            var valorAtual = parseFloat(FindControl("ctrlTextBoxFloat1_txtNumber", "input").value.replace(',', '.'));
            var valorProduto = parseFloat(dadosProduto.split('\t')[2].replace(',', '.'));
            FindControl("ctrlTextBoxFloat1_txtNumber", "input").value = (valorAtual + valorProduto).toFixed(2).replace('.', ',');
        }
        
        function excluir(idLinha, dadosProduto)
        {
            var orcRapido = document.getElementById("orcRapido");
            var orcRapidoW = orcRapido.contentWindow;
            var orcRapidoD = orcRapido.contentDocument;
            var row = orcRapidoD.getElementById(idLinha);
            
            orcRapido.style.height = orcRapidoD.body.scrollHeight + "px";
            
            while (dadosProduto.indexOf('~') >= 0)
                dadosProduto = dadosProduto.replace('~', '\t');
            
            if (editar && idProduto > 0)
            {
                var resposta = orcRapidoW.LstOrcamentoRapido.ExcluirProdutoOrcamento(row.getAttribute("idProdOrcamento")).value;
                var dadosResposta = resposta.split('\t');
                if (dadosResposta[0] == "Erro")
                {
                    alert(dadosResposta[1]);
                    return;
                }
            }
            else
            {
                var dadosProdutos = FindControl("hdfDadosProdutos", "input");
                var index = dadosProdutos.value.indexOf(dadosProduto + "\n");
                dadosProdutos.value = dadosProdutos.value.substr(0, index) + 
                    dadosProdutos.value.substr(index + (dadosProduto + "\n").length);
            }
            
            if (mudarDescr)
            {
                var index = FindControl("txtDescr", "textarea").value.indexOf(dadosProduto.split('\t')[9]);
                FindControl("txtDescr", "textarea").value = FindControl("txtDescr", "textarea").value.substr(0, index) + 
                    FindControl("txtDescr", "textarea").value.substr(index + dadosProduto.split('\t')[9].length);
                    
                if (FindControl("txtDescr", "textarea").value.charAt(index) == ';')
                {
                    FindControl("txtDescr", "textarea").value = FindControl("txtDescr", "textarea").value.substr(0, index) +
                        FindControl("txtDescr", "textarea").value.substr(index + 2);
                }
            }
            
            var valorAtual = parseFloat(FindControl("ctrlTextBoxFloat1_txtNumber", "input").value.replace(',', '.'));
            var valorProduto = parseFloat(dadosProduto.split('\t')[2].replace(',', '.'));
            FindControl("ctrlTextBoxFloat1_txtNumber", "input").value = (valorAtual - valorProduto).toFixed(2).replace('.', ',');
        }
        
        function cadastrarProdutos(idProduto, produtos, orcRapidoW)
        {
            cadastrando = true;
            document.getElementById("tabela").style.display = "none";
            
            bloquearPagina();
            desbloquearPagina(false);
            
            if (orcRapidoW == null || typeof orcRapidoW === undefined)
            {
                var orcRapido = document.getElementById("orcRapido");
                orcRapido.contentWindow.location = getOrcRapidoLocation();
                    
                var windowOrcRapido = orcRapido.contentWindow;
                setTimeout(function() { cadastrarProdutos(idProduto, produtos, windowOrcRapido) }, tempoSetTimeout);
                return;
            }
            
            if (orcRapidoW.LstOrcamentoRapido == null || typeof orcRapidoW.LstOrcamentoRapido === undefined)
            {
                setTimeout(function() { cadastrarProdutos(idProduto, produtos, orcRapidoW) }, tempoSetTimeout);
                return;
            }
            
            var resposta = orcRapidoW.LstOrcamentoRapido.IncluirProdutosOrcamento(idProduto, produtos).value;
            var dadosResposta = resposta.split('\t');
            if (dadosResposta[0] == "Erro")
            {
                alert(dadosResposta[1]);
                return;
            }
        
            window.opener.redirectUrl(window.opener.location.href + "&atualizar=1");
            closeWindow();
        }

        var atualizando = false;
        
        function validarInsert(botao)
        {
            if (atualizando)
                return false;

            atualizando = true;

            if (!validate()) {
                atualizando = false;
                return false;
            }
            
            if (subProdutos)
            {
                var numeroProdutos = 0;
                var tabelaProdutos = document.getElementById("orcRapido").contentDocument.getElementById("lstProd");
                for (i = 1; i < tabelaProdutos.rows.length - 1; i++)
                    numeroProdutos += tabelaProdutos.rows[i].style.display != "none" ? 1 : 0;
                
                if (numeroProdutos == 0)
                {
                    alert("Cadastre ao menos 1 produto para continuar.");
                    atualizando = false;
                    return false;
                }
            }
            
            return true;
        }
    </script>

    <table id="tabela">
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvLoja" runat="server" AutoGenerateRows="False" DataKeyNames="IdProd"
                    DataSourceID="odsProdOrc" DefaultMode="Insert" GridLines="None" CellPadding="4"
                    ForeColor="#333333" OnDataBound="dtvLoja_DataBound" OnItemCommand="dtvLoja_ItemCommand">
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
                    <RowStyle ForeColor="Black" />
                    <FieldHeaderStyle Font-Bold="False" Wrap="False" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <Fields>
                        <asp:TemplateField HeaderText="Ambiente" SortExpression="Ambiente">
                            <InsertItemTemplate>
                                <asp:DropDownList ID="ddlAmbiente" runat="server" Enabled="False" OnLoad="ddlAmbiente_Load"
                                    SelectedValue='<%# Bind("IdAmbienteOrca") %>'>
                                </asp:DropDownList>
                                <asp:TextBox ID="txtAmbiente" runat="server" MaxLength="50" Text='<%# Bind("Ambiente") %>'
                                    OnLoad="txtAmbiente_Load"></asp:TextBox>
                                <asp:CustomValidator ID="ctvAmbiente" runat="server" ClientValidationFunction="validaAmbiente"
                                    ControlToValidate="txtAmbiente" ErrorMessage="Ambiente não pode ser vazio." ValidateEmptyText="True">*</asp:CustomValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Ambiente") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="100%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtDescr" runat="server" Columns="70" MaxLength="1500" Rows="3"
                                    Text='<%# Bind("Descricao") %>' TextMode="MultiLine"></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtDescr"
                                    ErrorMessage="Descrição não pode ser vazia.">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                            <ItemStyle Wrap="False" Width="100%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtQtd" onkeypress="return soNumeros(event, true, true);" runat="server"
                                    Text='<%# Bind("Qtde") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemStyle Width="100%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Unitário" SortExpression="ValorProd">
                            <InsertItemTemplate>
                                <uc1:ctrlTextBoxFloat ID="ctrlTextBoxFloat1" runat="server" Value='<%# Bind("ValorProd") %>' />
                            </InsertItemTemplate>
                            <HeaderStyle Wrap="False" />
                            <ItemStyle Width="100%" />
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <InsertItemTemplate>
                                <table width="100%">
                                    <tr>
                                        <td align="center">
                                            <asp:LinkButton ID="lkbInserir" runat="server" OnClientClick="inserirProdutos(this); return false;"
                                                OnLoad="lkbInserir_Load">Inserir produtos</asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <iframe id="orcRapido" style="display: none" frameborder="0" width="100%"></iframe>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnLoad="btnInserir_Load"
                                                OnClientClick="if (!validarInsert(this)) return false;" />
                                            <asp:Button ID="btnCancelar" runat="server" OnClientClick="window.close();" Text="Cancelar"
                                                CausesValidation="false" />
                                            <uc2:ctrllinkquerystring ID="ctrlLinkQueryString1" runat="server" NameQueryString="IdOrca"
                                                Text='<%# Bind("IdOrcamento") %>' />
                                            <uc2:ctrllinkquerystring ID="ctrlLinkQueryString2" runat="server" NameQueryString="IdProd"
                                                Text='<%# Bind("IdProd") %>' />
                                            <asp:HiddenField ID="hdfDadosProdutos" runat="server" Value='<%# Bind("DadosProdutos") %>' />
                                            <asp:HiddenField ID="hdfNumSeq" runat="server" Value='<%# Bind("NumSeq") %>' />
                                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
                                                ShowSummary="False" />
                                        </td>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <InsertRowStyle HorizontalAlign="Left" />
                    <EditRowStyle HorizontalAlign="Left" BackColor="White" />
                    <AlternatingRowStyle BackColor="White" ForeColor="Black" />
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdOrc" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosOrcamento"
                    InsertMethod="Insert" OnInserted="odsProdOrc_Inserted" SelectMethod="GetElementByPrimaryKey"
                    TypeName="Glass.Data.DAL.ProdutosOrcamentoDAO" UpdateMethod="UpdateComTransacao" OnInserting="odsProdOrc_Inserting">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="key" QueryStringField="IdProd" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        if ((editar && idProduto > 0) || <%= Glass.Configuracoes.OrcamentoConfig.ItensProdutos.SempreInserirItensProdutosOrcamento.ToString().ToLower() %>)
            inserirProdutos(FindControl("lkbInserir", "a"));
    </script>

</asp:Content>
