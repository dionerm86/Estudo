<%@ Page Title="Alterar Grupo/Subgrupo" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadAlterarGrupo.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadAlterarGrupo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        function adicionar()
        {
            var idGrupo = FindControl("drpGrupo", "select").value;
            var idSubgrupo = FindControl("drpSubgrupo", "select").value;
            var codProd = FindControl("txtCod", "input").value;
            var descrProd = FindControl("txtDescr", "input").value;

            if (idGrupo == 0 && idSubgrupo == 0 && codProd == "" && descrProd == "")
            {
                alert("Selecione um filtro para adicionar os produtos.");
                return;
            }

            var produtos = CadAlterarGrupo.BuscarProdutos(codProd, descrProd, idGrupo, idSubgrupo).value.split('~');
            for (i = 0; i < produtos.length; i++)
            {
                var prod = produtos[i].split("#");
                addProd(prod[0], prod[1], prod[2], prod[3]);
            }
        }
        
        function addProd(idProd, codInterno, descricao, grupo)
        {
            var idProdExiste = FindControl("hdfIdProd", "input").value.split(',');
            for (i = 0; i < idProdExiste.length; i++)
                if (idProdExiste[i] == idProd)
                    return;
            
            var titulos = new Array("Cód.", "Descrição", "Grupo");
            var itens = new Array(codInterno, descricao, grupo);

            addItem(itens, titulos, "tbProdutos", idProd, "hdfIdProd", null, null, null, false);
           
        }

        function alterar()
        {
            var idsProd = FindControl("hdfIdProd", "input").value;
            if (idsProd == "")
            {
                alert("Selecione um produto para ser alterado.");
                return;
            }
            
            var grupo = FindControl("drpNovoGrupo", "select");
            var subgrupo = FindControl("drpNovoSubgrupo", "select");
            
            var msgSubgrupo = parseInt(subgrupo.value, 10) > 0 ? " e subgrupo '" + subgrupo.options[subgrupo.selectedIndex].text + "'" : "";
            if (!confirm("Deseja alterar esses produtos para o grupo '" + grupo.options[grupo.selectedIndex].text + "'" + msgSubgrupo + "?"))
                return;
            
            var resposta = CadAlterarGrupo.Alterar(idsProd, grupo.value, subgrupo.value).value.split("#");
            alert(resposta[1]);
            
            if (resposta[0] == "Erro")
                return;

            redirectUrl(window.location.href);
        }

        function loadSubgrupo(idGrupoProd, nomeControleSubgrupo)
        {
            var subgrupos = CadAlterarGrupo.LoadSubgrupos(idGrupoProd, nomeControleSubgrupo.indexOf("Novo") > -1).value;
            FindControl(nomeControleSubgrupo, "select").innerHTML = subgrupos;
        }
    </script>

    <section>
        <section id="pesquisa">
            <div class="boxLinha">
                <asp:Label ID="Label1" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                <asp:DropDownList ID="drpGrupo" runat="server" DataSourceID="odsGrupo" DataTextField="Descricao"
                    DataValueField="IdGrupoProd" onchange="loadSubgrupo(this.value, 'drpSubgrupo')">
                </asp:DropDownList>
                <asp:Label ID="Label2" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                <asp:DropDownList ID="drpSubgrupo" runat="server">
                    <asp:ListItem Value="0">Todos</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="boxLinha">
                <asp:Label ID="Label3" runat="server" Text="Cód." ForeColor="#0066FF"></asp:Label>
                <asp:TextBox ID="txtCod" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('lnkAdicionarProd', 'a');"></asp:TextBox>
                <asp:Label ID="Label4" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                <asp:TextBox ID="txtDescr" runat="server" onkeydown="if (isEnter(event)) cOnClick('lnkAdicionarProd', 'a');"
                    Width="200px"></asp:TextBox>
            </div>
            <div class="boxLinha">
                <asp:LinkButton ID="lnkAdicionarProd" runat="server" OnClientClick="adicionar(); return false">
                    <img src="../Images/Insert.gif" border="0" /> Adicionar Produtos
                </asp:LinkButton>
            </div>
        </section>
        <section id="produtos">
            <table id="tbProdutos">
            </table>
            <asp:HiddenField ID="hdfIdProd" runat="server" />
        </section>
        <section id="dadosAlteracao">
            <div class="boxLinha">
               <table>
                    <tr>
                        <td>
                            Novo Grupo
                        </td>
                        <td>
                            <asp:DropDownList ID="drpNovoGrupo" runat="server" DataSourceID="odsGrupo" 
                                DataTextField="Descricao" DataValueField="IdGrupoProd"
                                onchange="loadSubgrupo(this.value, 'drpNovoSubgrupo')" 
                                ondatabound="drpNovoGrupo_DataBound">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Novo Subgrupo
                        </td>
                        <td>
                            <asp:DropDownList ID="drpNovoSubgrupo" runat="server">
                                <asp:ListItem Value="0">Nenhum</asp:ListItem>
                            </asp:DropDownList>
                            <asp:HiddenField ID="hdfNovoSubgrupo" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
            <div class="boxLinha">
                <asp:Button ID="btnAlterar" runat="server" Text="Alterar" OnClientClick="alterar(); return false"
                    UseSubmitBehavior="False" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupo" runat="server" SelectMethod="GetForFilter" TypeName="Glass.Data.DAL.GrupoProdDAO">
                </colo:VirtualObjectDataSource>
                <br />
                <br />
                <asp:Label ID="lbl1" runat="server" Text="Ao alterar o grupo/subgrupo do produto, caso o produto tenha um desconto/acréscimo individual, este será mantido,"
                    ForeColor="Red" Font-Bold="True"></asp:Label>
                <br />
                <asp:Label ID="lbl2" runat="server" Text="mas caso o desconto esteja no gupo/subgrupo atual, ele assumirá o desconto/acréscimo deste novo grupo/subgrupo."
                    ForeColor="Red" Font-Bold="True"></asp:Label>
            </div>
        </section>
    </section>
</asp:Content>
