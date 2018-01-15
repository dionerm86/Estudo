<%@ Page Title="Alterar Dados Fiscais" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadAlterarDadosFiscais.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadAlterarDadosFiscais" %>

<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc5" %>

<%@ Register src="../Controls/ctrlMvaProdutoPorUf.ascx" tagname="ctrlMvaProdutoPorUf" tagprefix="uc1" %>
<%@ Register src="../Controls/ctrlIcmsProdutoPorUf.ascx" tagname="ctrlIcmsProdutoPorUf" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        function adicionar() {
            var idGrupo = FindControl("drpGrupo", "select").value;
            var idSubgrupo = FindControl("drpSubgrupo", "select").value;
            var codProd = FindControl("txtCod", "input").value;
            var descrProd = FindControl("txtDescr", "input").value;
            
            var ncmIni = FindControl("txtNcmIni", "input").value;
            var ncmFim = FindControl("txtNcmFim", "input").value;

            if (idGrupo == 0 && idSubgrupo == 0 && codProd == "" && descrProd == "" && ncmIni == "" && ncmFim == "") {
                alert("Selecione um filtro para adicionar os produtos.");
                return;
            }
            
            var retorno = CadAlterarDadosFiscais.BuscarProdutos(codProd, descrProd, idGrupo, idSubgrupo, ncmIni, ncmFim).value.split('~');
            
            if (retorno[0] != "Erro")
            {
                var produtos = retorno[1].split('|');
                for (var j = 0; j < produtos.length; j++) {
                    var prod = produtos[j].split("#");
                    addProd(prod[0], prod[1], prod[2], prod[3], prod[4], prod[5], prod[6], prod[7], prod[8], prod[9], prod[10], prod[11], prod[12], prod[13], prod[14], prod[15]);
                }
            }
            else
                alert(retorno[1]);
        }

        function addProd(idProd, codInterno, descricao, grupo, aliqicms, aliqicmsst, aliqipi, mva, ncm, cst, cstIpi, csosn, codEx, genProd, tipoMerc, planoContab) {
            var idProdExiste = FindControl("hdfIdProd", "input").value.split(',');
            for (var l = 0; l < idProdExiste.length; l++)
                if (idProdExiste[l] == idProd)
                return;
        
            var titulos = new Array("Cód.", "Descrição", "Grupo", "Aliq. ICMS", "Aliq. ICMS-ST", "Aliq. IPI", "MVA", "NCM", "CST", "CST IPI", "CSOSN", "Cód. EX", "Gênero Prod.", "Tipo Mercadoria", "Plano Contáb.");
            var itens = new Array(codInterno, descricao, grupo, aliqicms, aliqicmsst, aliqipi, mva, ncm, cst, cstIpi, csosn, codEx, genProd, tipoMerc, planoContab);

            addItem(itens, titulos, "tbProdutos", idProd, "hdfIdProd", null, null, null, false);
        }
        
        function formataStringControle(texto, texto0, texto1)
        {
            var nova = [];
            var temp = texto.split("/");
            
            for (var i = 0; i < temp.length; i++)
                if (temp[i] != "")
                    nova.push(temp[i]);
            
            texto = texto0 + ": " + nova[0] + " / " + texto1 + ": " + nova[1];
            if (nova.length > 2)
            {
                nova.shift();
                nova.shift();
                texto += " / Exceções: " + nova.join(" / ");
            }
            
            nova = [];
            temp = texto.split("|");
            
            for (var i = 0; i < temp.length; i++)
                if (temp[i] != "")
                    nova.push(temp[i]);
            
            return nova.join(", ");
        }

        function alterar() {
            var idsProd = FindControl("hdfIdProd", "input").value;
            if (idsProd == "") {
                alert("Selecione um produto para ser alterado.");
                return;
            }
            
            bloquearPagina();

            /* 
                Checa se alguma das textboxes estão habilitadas porém vazias
                As que estiverem vazias retornarão o valor string "-1" para facilitar a verificação no método ajax AlterarDadosFiscais
            */
            
            var icms = <%= ctrlIcmsProdutoPorUf.ClientID %>;
            var mva = <%= ctrlMvaProdutoPorUf.ClientID %>;
            
            var novaAliqICMS = FindControl("cbxICMS", "input").checked ? icms.DadosParaAjax() : "-1";
            var novaAliqICMSST = FindControl("txtAliqICMSST", "input").value != "" ? FindControl("txtAliqICMSST", "input").value  : "-1";
            var novaAliqIPI = FindControl("txtAliqIPI", "input").value != "" ? FindControl("txtAliqIPI", "input").value : "-1";
            var novaMVA = FindControl("cbxMVA", "input").checked ? mva.DadosParaAjax() : "-1";
            var novaNCM = FindControl("txtNCM", "input").value != "" ? FindControl("txtNCM", "input").value : "-1";

            var novaCST = FindControl("drpCst", "select").value != "" ? FindControl("drpCst", "select").value : "-1";
            var novaCSTIPI = FindControl("selCstIpi_hdfValor", "input").value != "" ? FindControl("selCstIpi_hdfValor", "input").value : "-1";
            var novaCSOSN = FindControl("drpCsosn", "select").value != "" ? FindControl("drpCsosn", "select").value : "-1";
            var novaCodEx = FindControl("txtCodEx", "input").value != "" ? FindControl("txtCodEx", "input").value : "-1";
            var novaGenProd = FindControl("drpGeneroProduto", "select").value != "" ? FindControl("drpGeneroProduto", "select").value : "-1";
            var novaTipoMerc = FindControl("drpTipoMercadoria", "select").value != "" ? FindControl("drpTipoMercadoria", "select").value : "-1";
            var novaPlanoContabil = FindControl("drpPlanoContabil", "select").value != "" ? FindControl("drpPlanoContabil", "select").value : "-1";
            var novoCest = FindControl("chkCest", "input").checked ? FindControl("hdfIdCest", "input").value : "-1";

            var substituirICMS = FindControl("cbSubstituirICMS", "input").checked;
            var substituirMVA = FindControl("cbSubstituirMVA", "input").checked;

            var alterarICMS = FindControl("chkAlterarICMS", "input").checked;
            var alterarMVA = FindControl("chkAlterarMVA", "input").checked;

            // Recupera o texto que estiver selecionado no gênero do produto
            var drpGenero = FindControl("drpGeneroProduto", "select");
            var novaGenProdDescr = drpGenero.options[drpGenero.selectedIndex].text;
            
            // Verifica se o usuário selecionou ao menos um campo para alterar antes de chamar o método de alteração
            if (novaAliqICMS == "-1" && novaAliqICMSST == "-1" && novaAliqIPI == "-1"
                && novaMVA == "-1" && novaNCM == "-1" && novaCST == "-1" && novaCSTIPI == "-1"
                && novaCSOSN == "-1" && novaCodEx == "-1" && novaGenProd == "-1" && novaTipoMerc == "-1"
                && novaPlanoContabil == "-1" && novoCest == "-1") {
                alert("Selecione ao menos um dado fiscal para ser alterado.");
                desbloquearPagina(true);
                return;
            }

            // Constrói a mensagem detalhando as alterações que serão realizadas e solicita confirmação do usuário
            var msgAlteracoes = "\n" + (novaAliqICMS == -1 ? "" : "Nova Alíquota ICMS: " + formataStringControle(novaAliqICMS, "Aliq. Intra.", "Aliq. Inter.") + "\n")
                                     + (novaAliqICMSST == -1 ? "" : "Nova Alíquota ICMS-ST: " + novaAliqICMSST + "\n")
                                     + (novaAliqIPI == -1 ? "" : "Nova Alíquota IPI: " + novaAliqIPI + "\n")
                                     + (novaMVA == -1 ? "" : "Nova MVA: " + formataStringControle(novaMVA, "Original", "Simples") + "\n")
                                     + (novaNCM == -1 ? "" : "Nova NCM: " + novaNCM + "\n")
                                     + (novaCST == -1 ? "" : "Novo CST: " + novaCST + "\n")
                                     + (novaCSTIPI == -1 ? "" : "Novo CST IPI: " + novaCSTIPI + "\n")
                                     + (novaCSOSN == -1 ? "" : "Nova CSOSN: " + novaCSOSN + "\n")
                                     + (novaCodEx == -1 ? "" : "Novo Cód. Ex.: " + novaCodEx + "\n")
                                     + (novaGenProd == -1 ? "" : "Novo Gênero do Produto: " + novaGenProdDescr + "\n")
                                     + (novaTipoMerc == -1 ? "" : "Novo Tipo de Mercadoria: " + novaTipoMerc + "\n")
                                     + (novaPlanoContabil == -1 ? "" : "Novo Plano Contábil: " + novaPlanoContabil + "\n")
                                     + (novoCest == -1 ? "" : "Novo CEST: " + FindControl("txtCest", "input").value + "\n");

            if (!confirm("Deseja alterar os dados fiscais destes produtos para os valores abaixo?" + msgAlteracoes))
            {
                desbloquearPagina(true);
                return;
            }
            
            // Invoca o método para alterar os valores e reporta se houve sucesso ou erro no procedimento
            var resposta = CadAlterarDadosFiscais.AlterarDados(idsProd, novaAliqICMS, novaAliqICMSST, novaAliqIPI, novaMVA, novaNCM, novaCST, novaCSTIPI,
                novaCSOSN, novaCodEx, novaGenProd, novaTipoMerc, novaPlanoContabil, substituirICMS, substituirMVA, alterarICMS, alterarMVA, novoCest).value.split("#");
            
            desbloquearPagina(true);
            alert(resposta[1]);

            if (resposta[0] == "Erro")
                return;

            redirectUrl(window.location.href);
        }

        function loadSubgrupo(idGrupoProd, nomeControleSubgrupo) {
            var subgrupos = CadAlterarDadosFiscais.LoadSubgrupos(idGrupoProd, nomeControleSubgrupo.indexOf("Novo") > -1).value;
            FindControl(nomeControleSubgrupo, "select").innerHTML = subgrupos;
        }

        function enableTextBox(checkBoxName, textBoxName) {
            FindControl(textBoxName, "input").disabled = !FindControl(checkBoxName, "input").checked;
            FindControl(textBoxName, "input").value = "";
        }
        
        function enableImageButton(checkBoxName, imageName) {
            FindControl(imageName, "input").disabled = !FindControl(checkBoxName, "input").checked;
        }

        function enableDropDownList(checkBoxName, dropDownListName) {
            FindControl(dropDownListName, "select").disabled = !FindControl(checkBoxName, "input").checked;
        }

        function enableSelPopup(checkBoxName, nameSelPopup) {
            FindControl(nameSelPopup + "_txtDescr", "input").disabled = !FindControl(checkBoxName, "input").checked;
            FindControl(nameSelPopup + "_imgPesq", "input").disabled = !FindControl(checkBoxName, "input").checked;
            FindControl(nameSelPopup, "input").value = "";
        }
        
        function enableControleTabela(checkBoxName, controle) {
            controle.Habilitar(FindControl(checkBoxName, "input").checked);
            controle.Limpar();
        }

        function mvaChanged(control){

            var chk = FindControl(control, "input");
            var checked = chk.checked;

            if(checked){
                FindControl("cbSubstituirMVA", "input").checked = false;
                FindControl("chkAlterarMVA", "input").checked = false;
            }

            chk.checked = checked;
        }

        function icmsChanged(control){

            var chk = FindControl(control, "input");
            var checked = chk.checked;

            if(checked){
                FindControl("cbSubstituirICMS", "input").checked = false;
                FindControl("chkAlterarICMS", "input").checked = false;
            }

            chk.checked = checked;
        }

        function setCest(idCest, Codigo){
            if (!FindControl("chkCest", "input").checked){
                alert("Marque o campo CEST para definir o novo valor.");
                return false;
            }

            FindControl("txtCest", "input").value = Codigo;
            FindControl("hdfIdCest", "input").value = idCest;
        }

    </script>

    <section>
    
        <div class="filtro">        
            <div>
                <span>
                    <asp:Label ID="Label1" runat="server" Text="Grupo" AssociatedControlID="drpGrupo"></asp:Label>
                    <asp:DropDownList ID="drpGrupo" runat="server" DataSourceID="odsGrupo" DataTextField="Descricao"
                        DataValueField="IdGrupoProd" onchange="loadSubgrupo(this.value, 'drpSubgrupo')">
                    </asp:DropDownList>
                </span>
                <span>
                    <asp:Label ID="Label2" runat="server" Text="Subgrupo" AssociatedControlID="drpSubgrupo"></asp:Label>
                    <asp:DropDownList ID="drpSubgrupo" runat="server">
                        <asp:ListItem Value="0">Todos</asp:ListItem>
                    </asp:DropDownList>
                </span>
            </div>
            
            <div>
                <span>
                    <asp:Label ID="Label3" runat="server" Text="Cód." AssociatedControlID="txtCod"></asp:Label>
                    <asp:TextBox ID="txtCod" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('lnkAdicionarProd', 'a');"></asp:TextBox>
                </span>
                <span>
                    <asp:Label ID="Label4" runat="server" Text="Descrição" AssociatedControlID="txtDescr"></asp:Label>
                    <asp:TextBox ID="txtDescr" runat="server" onkeydown="if (isEnter(event)) cOnClick('lnkAdicionarProd', 'a');"
                        Width="200px"></asp:TextBox>
                </span>
            </div>
            
            <div>
                <span>
                    <asp:Label ID="Label5" runat="server" Text="Faixa de NCM" AssociatedControlID="txtNcmIni"></asp:Label>
                    <asp:TextBox ID="txtNcmIni" runat="server" MaxLength="8" Columns="9"
                        onkeypress="return soNumeros(event, true, true)"
                        onkeydown="if (isEnter(event)) cOnClick('lnkAdicionarProd', 'a');"></asp:TextBox>
                    a
                    <asp:TextBox ID="txtNcmFim" runat="server" MaxLength="8" Columns="9"
                        onkeypress="return soNumeros(event, true, true)"
                        onkeydown="if (isEnter(event)) cOnClick('lnkAdicionarProd', 'a');"></asp:TextBox>
                </span>
            </div>
        </div>
                
        <div class="inserir">
            <span>
                <asp:LinkButton ID="lnkAdicionarProd" runat="server" OnClientClick="adicionar(); return false" CausesValidation="false"> 
                    <img src="../Images/Insert.gif" alt="Inserir" /> Adicionar Produtos
                </asp:LinkButton>
            </span>
        </div>
        
        <section id="produtos">
             <table id="tbProdutos">
             </table>
             <asp:HiddenField ID="hdfIdProd" runat="server" />
              <br /> <br />
        </section>
        
        <section id="dados">
            <div>
                <table class="gridStyle" cellpadding="0" cellspacing="0">
                    <tr>
                        <th>
                        </th>
                        <th>
                            Dado Fiscal
                        </th>
                        <th>
                            Novo Valor
                        </th>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td> 
                                         <asp:CheckBox ID="cbxICMS" runat="server" AutoPostBack="False" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                         <asp:CheckBox ID="cbSubstituirICMS" runat="server" AutoPostBack="False" onclick="icmsChanged(this.id);" />
                                    </td>
                                </tr>
                                 <tr>
                                    <td>
                                         <asp:CheckBox ID="chkAlterarICMS" runat="server" AutoPostBack="False" onclick="icmsChanged(this.id);" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td> 
                                         Alíquota ICMS
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                         Substiuir Original ?
                                    </td>
                                </tr>
                                 <tr>
                                    <td>
                                         Alterar Existente ?
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <uc2:ctrlIcmsProdutoPorUf ID="ctrlIcmsProdutoPorUf" runat="server" />
                            <script type="text/javascript">
                                FindControl("cbxICMS", "input").onclick();
                            </script>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>
                            <asp:CheckBox ID="cbxICMSST" runat="server" AutoPostBack="False" OnClick="enableTextBox('cbxICMSST','txtAliqICMSST')" />
                        </td>
                        <td>
                            Alíquota ICMS-ST
                        </td>
                        <td>
                            <asp:TextBox ID="txtAliqICMSST" runat="server" Enabled="False" OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBox ID="cbxIPI" runat="server" AutoPostBack="False" OnClick="enableTextBox('cbxIPI','txtAliqIPI')" />
                        </td>
                        <td>
                            Alíquota IPI
                        </td>
                        <td>
                            <asp:TextBox ID="txtAliqIPI" runat="server" Enabled="False" OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>
                            <table>
                                <tr>
                                    <td> 
                                        <asp:CheckBox ID="cbxMVA" runat="server" AutoPostBack="False" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                         <asp:CheckBox ID="cbSubstituirMVA" runat="server" AutoPostBack="False" ClientIDMode="Static" onclick="mvaChanged(this.id);"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                         <asp:CheckBox ID="chkAlterarMVA" runat="server" AutoPostBack="False" ClientIDMode="Static" onclick="mvaChanged(this.id);" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                           <table>
                                <tr>
                                    <td> 
                                        MVA
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                         Substiuir Original ?
                                    </td>
                                </tr>
                               <tr>
                                    <td>
                                         Alterar Existente ?
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <uc1:ctrlMvaProdutoPorUf ID="ctrlMvaProdutoPorUf" runat="server" />
                            <script type="text/javascript">
                                FindControl("cbxMVA", "input").onclick();
                            </script>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBox ID="cbxCST" runat="server" AutoPostBack="False" OnClick="enableDropDownList('cbxCST','drpCst')" />
                        </td>
                        <td>
                            CST
                        </td>
                        <td>
                            <asp:DropDownList ID="drpCst" runat="server" Enabled="False">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem>60</asp:ListItem>
                                <asp:ListItem>00</asp:ListItem>
                                <asp:ListItem>10</asp:ListItem>
                                <asp:ListItem>20</asp:ListItem>
                                <asp:ListItem>30</asp:ListItem>
                                <asp:ListItem>40</asp:ListItem>
                                <asp:ListItem>41</asp:ListItem>
                                <asp:ListItem>50</asp:ListItem>
                                <asp:ListItem>51</asp:ListItem>
                                <asp:ListItem>70</asp:ListItem>
                                <asp:ListItem>90</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td style="height: 23px">
                            <asp:CheckBox ID="cbxCSTIPI" runat="server" AutoPostBack="False" OnClick="enableSelPopup('cbxCSTIPI','selCstIpi')" />
                        </td>
                        <td style="height: 23px">
                            CST IPI&nbsp;
                        </td>
                        <td style="height: 23px">
                            <uc5:ctrlSelPopup ID="selCstIpi" runat="server" DataSourceID="odsCstIpi" DataTextField="Descr"
                                DataValueField="Id" ExibirIdPopup="True" FazerPostBackBotaoPesquisar="False"
                                TextWidth="200px" TituloTela="Selecione o CST do IPI" />
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 23px">
                            <asp:CheckBox ID="cbxCSOSN" runat="server" AutoPostBack="False" OnClick="enableDropDownList('cbxCSOSN','drpCsosn')" />
                        </td>
                        <td style="height: 23px">
                            CSOSN
                        </td>
                        <td style="height: 23px">
                            <asp:DropDownList ID="drpCsosn" runat="server" AppendDataBoundItems="True" DataSourceID="odsCsosn"
                                DataTextField="Descr" DataValueField="Id" Enabled="False">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>
                            <asp:CheckBox ID="cbxNCM" runat="server" AutoPostBack="False" OnClick="enableTextBox('cbxNCM','txtNCM')" />
                        </td>
                        <td>
                            NCM
                        </td>
                        <td>
                            <asp:TextBox ID="txtNCM" runat="server" Enabled="False" OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBox ID="cbxCodEx" runat="server" AutoPostBack="False" OnClick="enableTextBox('cbxCodEx','txtCodEx')" />
                        </td>
                        <td>
                            CÓD. EX.
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodEx" runat="server" Enabled="False" OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>
                            <asp:CheckBox ID="cbxGenProd" runat="server" AutoPostBack="False" OnClick="enableDropDownList('cbxGenProd','drpGeneroProduto')" />
                        </td>
                        <td>
                            Gênero Prod.
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGeneroProduto" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsGeneroProduto" DataTextField="CodigoDescricao" DataValueField="IdGeneroProduto"
                                Enabled="False">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBox ID="cbxTipoMercadoria" runat="server" AutoPostBack="False" OnClick="enableDropDownList('cbxTipoMercadoria','drpTipoMercadoria')" />
                        </td>
                        <td>
                            Tipo de Mercadoria
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoMercadoria" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsTipoMercadoria" DataTextField="Descr" DataValueField="Id" Enabled="False">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>
                            <asp:CheckBox ID="cbxPlanoContabil" runat="server" AutoPostBack="False" OnClick="enableDropDownList('cbxPlanoContabil','drpPlanoContabil')" />
                        </td>
                        <td>
                            Plano Contábil
                        </td>
                        <td>
                            <asp:DropDownList ID="drpPlanoContabil" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsPlanoContaContabil" DataTextField="Descricao" DataValueField="IdContaContabil"
                                Enabled="False">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td style="height: 23px">
                            <asp:CheckBox ID="chkCest" runat="server" AutoPostBack="False" OnClick="enableImageButton('chkCest','imbOpenCest'); FindControl('txtCest', 'input').value = '';" />
                        </td>
                        <td style="height: 23px">
                            CEST&nbsp;
                        </td>
                        <td style="height: 23px">
                            <asp:TextBox ID="txtCest" runat="server" MaxLength="7" style="float:left" Enabled="false"></asp:TextBox>
                            <asp:HiddenField ID="hdfIdCest" runat="server" />
                            <asp:ImageButton ID="imbOpenCest" style="float:left" runat="server" Enabled="true"
                                ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(300, 400, '../Utils/SelCest.aspx?'); return false;" />
                        </td>
                    </tr>
                </table>
                <asp:Button ID="btnAlterar" runat="server" Text="Alterar" OnClientClick="alterar(); return false"
                    UseSubmitBehavior="False" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupo" runat="server" SelectMethod="GetForFilter" TypeName="Glass.Data.DAL.GrupoProdDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCstIpi" runat="server" SelectMethod="GetCstIpi" TypeName="Glass.Data.EFD.DataSourcesEFD">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCsosn" runat="server" SelectMethod="GetCSOSN" TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGeneroProduto" runat="server" 
                    SelectMethod="ObtemGenerosProduto" TypeName="Glass.Global.Negocios.IProdutoFluxo">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoMercadoria" runat="server" SelectMethod="GetTipoMercadoria"
                    TypeName="Glass.Data.EFD.DataSourcesEFD">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoContaContabil" runat="server" SelectMethod="GetSorted"
                    TypeName="Glass.Data.DAL.PlanoContaContabilDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="0" Name="natureza" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </div>
        </section>
        
    </section>

    <script type="text/javascript">
        FindControl("selCstIpi_txtDescr", "input").disabled = true;
        FindControl("selCstIpi_imgPesq", "input").disabled = true;
    </script>

</asp:Content>
