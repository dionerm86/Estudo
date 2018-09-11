<%@ Page Title="Duplicar Produtos" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadDuplicarProduto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadDuplicarProduto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        function alteraTipo()
        {
            var tipo = FindControl("drpTipo", "select").value;
            document.getElementById("produto").style.display = tipo == 1 ? "" : "none";
            document.getElementById("grupoSubgrupo").style.display = tipo == 2 ? "" : "none";
        }

        function numeroProdutos()
        {
            var tabela = document.getElementById("tbProdutos");
            var retorno = 0;

            for (i = 1; i < tabela.rows.length; i++)
                retorno += tabela.rows[i].style.display != "none" ? 1 : 0;

            return retorno;
        }

        function addProd()
        {
            var codInterno = FindControl("txtCodProd", "input").value;
            if (codInterno == "")
                return;

            var dadosProd = CadDuplicarProduto.GetDadosProduto(codInterno).value.split("##");
            if (dadosProd[0] == "Erro")
            {
                alert(dadosProd[1]);
                return;
            }

            if (numeroProdutos() == 0)
            {
                FindControl("drpNovoGrupo", "select").value = dadosProd[5];
                FindControl("drpNovoGrupo", "select").onchange();
                FindControl("drpNovoSubgrupo", "select").value = dadosProd[6];
            }

            adicionar(dadosProd[1], codInterno, dadosProd[2], dadosProd[3], dadosProd[4]);
            atualizaPreview();

            FindControl("txtCodProd", "input").value = "";
        }

        function addGrupoSubgrupo()
        {
            var idGrupo = FindControl("drpGrupo", "select").value;
            var idSubgrupo = FindControl("drpSubgrupo", "select").value;

            var produtos = CadDuplicarProduto.GetProdutosGrupoSubgrupo(idGrupo, idSubgrupo).value.split("##");
            if (produtos[0] == "Erro")
            {
                alert(produtos[1]);
                return;
            }
            else
                produtos = produtos[1].split("^");

            for (iProd = 0; iProd < produtos.length; iProd++)
            {
                var dadosProd = produtos[iProd].split("~");

                if (iProd == 0 && numeroProdutos() == 0)
                {
                    FindControl("drpNovoGrupo", "select").value = dadosProd[5];
                    FindControl("drpNovoGrupo", "select").onchange();
                    FindControl("drpNovoSubgrupo", "select").value = dadosProd[6];
                }

                adicionar(dadosProd[0], dadosProd[1], dadosProd[2], dadosProd[3], dadosProd[4]);
            }

            atualizaPreview();
        }

        function adicionar(idProd, codInterno, descricao, grupo, subgrupo)
        {
            var idsProd = FindControl("hdfIdProd", "input").value.split(",");
            for (i = 0; i < idsProd.length; i++)
            {
                if (idsProd[i] == idProd)
                    return;
            }

            var titulos = new Array("Cód.", "Descrição", "Grupo/Subgrupo");
            var itens = new Array(codInterno, descricao, grupo + (subgrupo != "" ? " - " + subgrupo : ""));

            addItem(itens, titulos, "tbProdutos", idProd, "hdfIdProd", null, null, "atualizaPreview", false);
        }

        function substituiTexto(textoAtual, remover, substituir)
        {
            var retorno = textoAtual.toUpperCase();
            var pos = retorno.indexOf(remover.toUpperCase());

            while (pos > -1)
            {
		        retorno = retorno.substr(0, pos) + substituir.toUpperCase() + retorno.substr(pos + remover.length);
		        pos = retorno.indexOf(remover.toUpperCase(), pos + substituir.length);
	        }

            return retorno;
        }

        function getTextFromSelect(select)
        {
            for (i = 0; i < select.options.length; i++)
                if (select.options[i].value == select.value)
                    return select.options[i].text;

            return "";
        }

        var alterandoPreview = false;

        function atualizaPreview()
        {
            if (alterandoPreview)
                return;

            alterandoPreview = true;

            try
            {
                var tbProdutos = document.getElementById("tbProdutos");
                var tbPreview = document.getElementById("tbPreview");
                tbPreview.innerHTML = tbProdutos.innerHTML;

                var codInternoRemover = FindControl("txtTextoCodigoAtual", "input").value;
                var codInternoSubstituir = FindControl("txtTextoCodigoNovo", "input").value;
                var descricaoRemover = FindControl("txtTextoDescricaoAtual", "input").value;
                var descricaoSubstituir = FindControl("txtTextoDescricaoNovo", "input").value;
                var grupoNovo = getTextFromSelect(FindControl("drpNovoGrupo", "select"));
                var subgrupoNovo = FindControl("drpNovoSubgrupo", "select");
                subgrupoNovo = subgrupoNovo.value > 0 ? " - " + getTextFromSelect(subgrupoNovo) : "";

                if (tbPreview.rows.length > 0)
                    tbPreview.rows[0].cells[0].style.display = "none";

                for (i = 1; i < tbPreview.rows.length; i++)
                {
                    tbPreview.rows[i].cells[0].style.display = "none";

                    if (codInternoRemover != "")
                        tbPreview.rows[i].cells[1].innerHTML = substituiTexto(tbPreview.rows[i].cells[1].innerHTML, codInternoRemover, codInternoSubstituir);
                    else
                        tbPreview.rows[i].cells[1].innerHTML += codInternoSubstituir;

                    if (descricaoRemover != "")
                        tbPreview.rows[i].cells[2].innerHTML = substituiTexto(tbPreview.rows[i].cells[2].innerHTML, descricaoRemover, descricaoSubstituir);
                    else
                        tbPreview.rows[i].cells[2].innerHTML = Trim(tbPreview.rows[i].cells[2].innerHTML + " " + Trim(descricaoSubstituir));

                    tbPreview.rows[i].cells[3].innerHTML = grupoNovo + subgrupoNovo;
                }
            }
            finally
            {
                alterandoPreview = false;
            }
        }

        function carregaSubgrupos(nomeGrupo, nomeSubgrupo, textoVazio)
        {
            var idGrupo = FindControl(nomeGrupo, "select").value;
            var drpSubgrupo = FindControl(nomeSubgrupo, "select");
            drpSubgrupo.innerHTML = CadDuplicarProduto.GetSubgrupos(idGrupo, textoVazio).value;
        }

        function duplicar()
        {
            var idsProd = FindControl("hdfIdProd", "input").value.split(",");
            if (idsProd.length == 0 || idsProd[0] == "")
            {
                alert("Selecione um produto para ser duplicado.");
                return;
            }

            var codInternoSubstituir = FindControl("txtTextoCodigoNovo", "input").value;
            if (codInternoSubstituir == "")
            {
                alert("Digite o texto que será acrescido ao final do código do produto.");
                return;
            }

            var idNovoGrupo = FindControl("drpNovoGrupo", "select").value;
            var idNovoSubgrupo = FindControl("drpNovoSubgrupo", "select").value;
            var codInternoRemover = FindControl("txtTextoCodigoAtual", "input").value;
            var descricaoRemover = FindControl("txtTextoDescricaoAtual", "input").value;
            var descricaoSubstituir = FindControl("txtTextoDescricaoNovo", "input").value;
            var altura = FindControl("txtAltura", "input").value;
            var largura = FindControl("txtLargura", "input").value;
            var processo = FindControl("txtProc", "input").value;
            var aplicacao = FindControl("txtApl", "input").value;

            var resposta = CadDuplicarProduto.Duplicar(idsProd, idNovoGrupo, idNovoSubgrupo, codInternoRemover, codInternoSubstituir,
                descricaoRemover, descricaoSubstituir, altura, largura, processo, aplicacao).value.split("##");

            if (resposta[0] == "Erro")
                alert(resposta[1]);
            else
                eval(resposta[1]);
        }
    </script>

    <section>

        <section id="pesquisa">
            <div>
                <asp:Label runat="server" ID="lblTipo" Text="Tipo" ForeColor="#0066FF"></asp:Label>
                <asp:DropDownList ID="drpTipo" runat="server" onchange="alteraTipo()">
                    <asp:ListItem Value="1">Produto</asp:ListItem>
                    <asp:ListItem Value="2">Grupo/Subgrupo</asp:ListItem>
                </asp:DropDownList>
            </div>
            <br />
            <div id="produto">
                <asp:Label runat="server" ID="lblProduto" Text="Produto" ForeColor="#0066FF"></asp:Label>
                <asp:TextBox ID="txtCodProd" runat="server" Width="80px" onkeydown="if (isEnter(event)) cOnClick('imbAddProd', 'input')"></asp:TextBox>
                <asp:ImageButton ID="imbAddProd" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="addProd(); return false"
                    ImageAlign="Top" />
            </div>
            <div id="grupoSubgrupo">
                <asp:Label runat="server" ID="Label2" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                <asp:DropDownList ID="drpGrupo" runat="server" DataSourceID="odsGrupo" onchange="carregaSubgrupos('drpGrupo', 'drpSubgrupo', 'Todos');"
                    DataTextField="Descricao" DataValueField="IdGrupoProd">
                </asp:DropDownList>
                <asp:Label runat="server" ID="Label3" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                <asp:DropDownList ID="drpSubgrupo" runat="server">
                </asp:DropDownList>
                <asp:ImageButton ID="imbAddGrupoSubgrupo" runat="server" ImageUrl="~/Images/Insert.gif"
                    OnClientClick="addGrupoSubgrupo(); return false" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupo" runat="server" SelectMethod="GetForFilter" TypeName="Glass.Data.DAL.GrupoProdDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="false" Name="incluirTodos" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </div>
        </section>

        <br />

        <section id="produtos">
            <div style="display: table">
                <div style="float: left; width: 350px;">
                    <p>
                        Produtos</p>
                    <div>
                        <table id="tbProdutos">
                        </table>
                    </div>
                </div>
                <div style="margin-left: 360px; width: 350px;">
                    <p>
                        Resultado (pré-visualização)</p>
                    <div>
                        <table id="tbPreview">
                        </table>
                    </div>
                </div>
                <asp:HiddenField ID="hdfIdProd" runat="server" />
            </div>
        </section>

        <section id="dadosDuplicacao">

             <header>
                    <p>Salvar em:</p>
             </header>

             <div>
                    Grupo
                    <asp:DropDownList ID="drpNovoGrupo" runat="server" DataSourceID="odsGrupo" onchange="carregaSubgrupos('drpNovoGrupo', 'drpNovoSubgrupo', 'Nenhum'); atualizaPreview()"
                        DataTextField="Descricao" DataValueField="IdGrupoProd">
                    </asp:DropDownList>
                    Subgrupo
                    <asp:DropDownList ID="drpNovoSubgrupo" runat="server" onchange="atualizaPreview()">
                    </asp:DropDownList>
                </div>

              <br />

             <div>
                    <p>Substituir texto: </p>
                    <div>
                        Código: de <asp:TextBox ID="txtTextoCodigoAtual" runat="server" Width="50px" onchange="atualizaPreview()"></asp:TextBox>
                        para <asp:TextBox ID="txtTextoCodigoNovo" runat="server" Width="50px" onchange="atualizaPreview()"></asp:TextBox>
                    </div>
                    <br />
                    <div>
                        Descrição: de <asp:TextBox ID="txtTextoDescricaoAtual" runat="server" Width="150px"
                            onchange="atualizaPreview()"></asp:TextBox>
                        para <asp:TextBox ID="txtTextoDescricaoNovo" runat="server" Width="150px" onchange="atualizaPreview()"></asp:TextBox>
                    </div>
                    <br />
                    <div>
                        Altura  <asp:TextBox ID="txtAltura" runat="server" Width="50px"></asp:TextBox>
                        Largura <asp:TextBox ID="txtLargura" runat="server" Width="50px"></asp:TextBox>
                    </div>
                    <br />
                    <div>
                        Proc. <asp:TextBox ID="txtProc" runat="server" Width="50px"></asp:TextBox>
                        Apl. <asp:TextBox ID="txtApl" runat="server" Width="50px"></asp:TextBox>
                    </div>
                    <br />
                    <div style="font-style: italic">
                        se os campos "de" ficarem em branco o texto do campo "para" será acrescido ao final
                        do texto existente;
                        <br />
                        caso contrário o texto do campo "para" substitui o texto do campo "de" no texto
                        existente.
                        <br />
                        Se nada for definido nos campos Altura e Largura serão considerados os valores dos produtos originais.
                        <br />
                        Se nada for definido nos campos Proc e Apl serão considerados os valores dos produtos originais.
                        <br />
                        Nos campos de Proc. e Apl. devem ser inseridos códigos dos processos e aplicações que serão utilizados.
                    </div>
                    <br />
                    <div>
                        <asp:Button ID="btnDuplicar" runat="server" Text="Duplicar" OnClientClick="duplicar(); return false" />
                        <colo:VirtualObjectDataSource culture="pt-BR" ID="ObjectDataSource1" runat="server" SelectMethod="GetForFilter"
                            TypeName="Glass.Data.DAL.GrupoProdDAO">
                            <SelectParameters>
                                <asp:Parameter DefaultValue="false" Name="incluirTodos" Type="Boolean" />
                            </SelectParameters>
                        </colo:VirtualObjectDataSource>
                    </div>
                </div>

        </section>

     </section>

        <%--<table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="Label4" Text="Tipo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipo" runat="server" onchange="alteraTipo()">
                                <asp:ListItem Value="1">Produto</asp:ListItem>
                                <asp:ListItem Value="2">Grupo/Subgrupo</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table id="produto">
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="Label1" Text="Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="80px" onkeydown="if (isEnter(event)) cOnClick('imbAddProd', 'input')"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbAddProd" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="addProd(); return false" />
                        </td>
                    </tr>
                </table>
                <table id="grupoSubgrupo">
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="Label2" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupo" runat="server" DataSourceID="odsGrupo" onchange="carregaSubgrupos('drpGrupo', 'drpSubgrupo', 'Todos');"
                                DataTextField="Descricao" DataValueField="IdGrupoProd">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="Label3" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSubgrupo" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbAddGrupoSubgrupo" runat="server" ImageUrl="~/Images/Insert.gif"
                                OnClientClick="addGrupoSubgrupo(); return false" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <th align="center">
                            Produtos
                        </th>
                        <th>
                            &nbsp;&nbsp;&nbsp;
                            &nbsp;&nbsp;&nbsp;
                            &nbsp;&nbsp;&nbsp;
                        </th>
                        <th align="center">
                            Resultado (pré-visualização)
                        </th>
                    </tr>
                    <tr>
                        <td>
                            <table id="tbProdutos">
                            </table>
                        </td>
                        <td>
                        </td>
                        <td>
                            <table id="tbPreview">
                            </table>
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfIdProd" runat="server" />
                <br />
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <th align="center">
                            Salvar em:
                        </th>
                    </tr>
                    <tr>
                        <table>
                            <tr>
                                <td align="left">
                                    Grupo
                                </td>
                                <td align="left">
                                    <asp:DropDownList ID="drpNovoGrupo" runat="server" DataSourceID="odsGrupo" onchange="carregaSubgrupos('drpNovoGrupo', 'drpNovoSubgrupo', 'Nenhum'); atualizaPreview()"
                                        DataTextField="Descricao" DataValueField="IdGrupoProd">
                                    </asp:DropDownList>
                                </td>
                                <td align="left">
                                    &nbsp;
                                    Subgrupo
                                </td>
                                <td align="left">
                                    <asp:DropDownList ID="drpNovoSubgrupo" runat="server" onchange="atualizaPreview()">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </tr>
                    <tr>
                        <th align="center">
                            <br />
                            Substituir texto:
                        </th>
                    </tr>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td align="left">
                                        Código: de
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="txtTextoCodigoAtual" runat="server" Width="50px" onchange="atualizaPreview()"></asp:TextBox>
                                    </td>
                                    <td align="left">
                                        para
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="txtTextoCodigoNovo" runat="server" Width="50px" onchange="atualizaPreview()"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td align="left">
                                        Descrição: de
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="txtTextoDescricaoAtual" runat="server" Width="150px" onchange="atualizaPreview()"></asp:TextBox>
                                    </td>
                                    <td align="left">
                                        para
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="txtTextoDescricaoNovo" runat="server" Width="150px" onchange="atualizaPreview()"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="font-style: italic">
                            se os campos "de" ficarem em branco o texto do campo "para" será acrescido ao final do texto existente; <br />
                            caso contrário o texto do campo "para" substitui o texto do campo "de" no texto existente
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnDuplicar" runat="server" Text="Duplicar"
                    onclientclick="duplicar(); return false" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupo" runat="server" SelectMethod="GetForFilter" TypeName="Glass.Data.DAL.GrupoProdDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="false" Name="incluirTodos" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>--%>

        <script type="text/javascript">
        alteraTipo();
        FindControl("drpGrupo", "select").onchange();
        FindControl("drpNovoGrupo", "select").onchange();
        FindControl("txtCodProd", "input").focus();
        </script>
</asp:Content>
