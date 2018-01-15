<%@ Page Title="Descontos/Acréscimos sobre Grupos de Produtos" Language="C#" AutoEventWireup="true"
    CodeBehind="CadDescontoAcrescimoCliente.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadDescontoAcrescimoCliente"
    MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>

<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <script type="text/javascript">
        function iniciarCopia(botao)
        {
            document.getElementById("copiar").style.display = "";
            botao.style.display = "none";
        }

        function validaCopia()
        {
            var idCliente = FindControl("txtNumCli", "input").value;
            if (idCliente == "")
            {
                alert("Selecione o novo cliente.");
                return false;
            }

            return confirm('Deseja copiar os descontos/acréscimos desse cliente para o cliente ' + FindControl("lblNome", "span").innerHTML + '?');
        }

        function getCli(idCli)
        {
            if (idCli.value == "")
                return;

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro")
            {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("lblNome", "span").innerHTML = "";
                return false;
            }

            FindControl("lblNome", "span").innerHTML = retorno[1];
        }

        // Carrega dados do produto com base no código do produto passado
        function setProduto() {
            var codInterno = FindControl("txtCodProd", "input").value;

            if (codInterno == "")
                return false;

            try {
                var retorno = MetodosAjax.GetProd(codInterno).value.split(';');

                if (retorno[0] == "Erro") {
                    alert(retorno[1]);
                    FindControl("txtCodProd", "input").value = "";
                    return false;
                }

                FindControl("txtProduto", "input").value = retorno[2];
            }
            catch (err) {
                alert(err.value);
            }
        }

    </script>

    <div class="filtro">
        <div>
            <span>
                <asp:Label ID="lblTituloCliente" runat="server" Text="Cliente:" AssociatedControlID="lblCliente"></asp:Label>
                <asp:Label ID="lblCliente" runat="server"></asp:Label>
                <asp:ImageButton ID="imgIniciarCopia" runat="server" ImageUrl="~/Images/copy.png"
                    OnClientClick="iniciarCopia(this); return false" ToolTip="Copiar para..." CssClass="botaoPesquisar" />
            </span>
        </div>
        <div id="copiar" style="display: none">
            <span>
                <asp:Label ID="Label3" runat="server" Text="Novo Cliente:" AssociatedControlID="txtNumCli"></asp:Label>
                <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                    onblur="getCli(this);"></asp:TextBox>
                <asp:Label ID="lblNome" runat="server"></asp:Label>
                <asp:ImageButton ID="imgCopiar" runat="server" ImageUrl="~/Images/copy.png" OnClientClick="if (!validaCopia()) return false"
                    ToolTip="Copiar" OnClick="imgCopiar_Click" CssClass="botaoPesquisar" />
            </span>
        </div>
        <div runat="server" id="dadosGrupo">
            <br />
            <div>
                <span>
                    <asp:Label ID="Label2" runat="server" AssociatedControlID="lblGrupo" Text="Grupo:"></asp:Label>
                    <asp:Label ID="lblGrupo" runat="server"></asp:Label>
                </span>
                <span>
                    <asp:Label ID="Label4" runat="server" AssociatedControlID="lblSubgrupo" Text="Subgrupo:"></asp:Label>
                    <asp:Label ID="lblSubgrupo" runat="server"></asp:Label>
                </span>
            </div>
            <div>
                <span>
                    <asp:HyperLink ID="lnkVoltar" runat="server" OnLoad="lnkVoltar_Load" Style="font-size: small">Voltar para os grupos/subgrupos</asp:HyperLink>
                </span>
            </div>
            <br />
            <div>
                <span>
                    <asp:Label ID="Label9" runat="server" AssociatedControlID="txtProduto" Text="Produto"></asp:Label>
                    <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="setProduto();"></asp:TextBox>
                    <asp:TextBox ID="txtProduto" runat="server" Width="150px"></asp:TextBox>
                    <asp:ImageButton ID="imbPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" CssClass="botaoPesquisar" />
                </span>
                <span>
                    <asp:Label ID="Label10" runat="server" AssociatedControlID="drpSituacao" Text="Situação"></asp:Label>
                    <asp:DropDownList ID="drpSituacao" runat="server" AppendDataBoundItems="true" OnDataBound="drpSituacao_DataBound"
                        DataSourceID="odsSituacao" DataValueField="Key" DataTextField="Translation">
                        <asp:ListItem Value="">Todas</asp:ListItem>
                    </asp:DropDownList>
                    <asp:ImageButton ID="imbPesq0" runat="server" 
                        ImageUrl="~/Images/Pesquisar.gif" CssClass="botaoPesquisar" />
                </span>
            </div>
        </div>
    </div>
    <asp:GridView GridLines="None" ID="grdDesconto" runat="server" SkinID="defaultGridView" 
        DataSourceID="odsDesconto" DataKeyNames="IdDesconto" OnRowCommand="grdDesconto_RowCommand"
        AllowSorting="false" PageSize="100">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:ImageButton ID="imbProdutosGrupo" runat="server" ImageUrl="~/Images/subgrupo.png"
                        OnClick="imbProdutosGrupo_Click" ToolTip="Produtos desse grupo" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField Visible="False">
                <ItemTemplate>
                    <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Atualizar" ImageUrl="~/Images/ok.gif" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Desconto (%)" SortExpression="Desconto">
                <ItemTemplate>
                    <asp:TextBox ID="txtDesc" runat="server" MaxLength="7" Width="50px" onkeypress="return soNumeros(event, false, true);"
                        Text='<%# Bind("Desconto", "{0:#0.0000}") %>'></asp:TextBox>
                    <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Bind("IdCliente") %>' />
                    <asp:HiddenField ID="hdfIdTabelaDesconto" runat="server" Value='<%# Bind("IdTabelaDesconto") %>' />
                    <asp:HiddenField ID="hdfIdGrupo" runat="server" Value='<%# Bind("IdGrupoProd") %>' />
                    <asp:HiddenField ID="hdfIdSubgrupo" runat="server" Value='<%# Bind("IdSubgrupoProd") %>' />
                    <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Bind("IdProduto") %>' />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Acréscimo (%)" SortExpression="Acrescimo">
                <ItemTemplate>
                    <asp:TextBox ID="txtAcresc" runat="server" MaxLength="7" onkeypress="return soNumeros(event, false, true);"
                        Text='<%# Bind("Acrescimo", "{0:#0.0000}") %>' Width="50px"></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Grupo">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("GrupoSubgrupo") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Produto" SortExpression="Descricao" Visible="False">
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Eval("Produto") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Aplicar bis./lap.?" SortExpression="AplicarBeneficiamentos"
                Visible="False">
                <ItemTemplate>
                    <asp:CheckBox ID="chkAplicarBenef" runat="server" Checked='<%# Bind("AplicarBeneficiamentos") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="DescontoAcrescimoCliente"
                        IdRegistro='<%# (uint)(int)Eval("IdDesconto") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <PagerStyle CssClass="pgr"></PagerStyle>
        <EditRowStyle CssClass="edit"></EditRowStyle>
        <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
    </asp:GridView>
    <div class="inserir">
        <asp:Button ID="btnAtualizar" runat="server" OnClick="btnAtualizar_Click" Text="Atualizar" />
    </div>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsDesconto" runat="server" 
        DataObjectTypeName="Glass.Global.Negocios.Entidades.DescontoAcrescimoCliente"
        SelectMethod="PesquisarDescontosAcrescimos" 
        SelectByKeysMethod="ObtemDescontoAcrescimo"
        TypeName="Glass.Global.Negocios.IClienteFluxo"
        EnablePaging="True" MaximumRowsParameterName="pageSize" OnUpdated="odsDesconto_Updated"
        SortParameterName="sortExpression"
        UpdateMethod="SalvarDescontoAcrescimo"
        UpdateStrategy="GetAndUpdate">
        <SelectParameters>
            <asp:QueryStringParameter Name="idCliente" QueryStringField="idCliente" Type="Int32" />
            <asp:QueryStringParameter Name="idTabelaDesconto" QueryStringField="idTabelaDesconto" Type="Int32" />
            <asp:QueryStringParameter Name="idGrupoProd" QueryStringField="idGrupo" Type="Int32" />
            <asp:QueryStringParameter Name="idSubgrupoProd" QueryStringField="idSubgrupo" Type="Int32" />
            <asp:ControlParameter Name="codProduto" ControlID="txtCodProd" PropertyName="Text" Type="String" />
            <asp:ControlParameter Name="produto" ControlID="txtProduto" PropertyName="Text" Type="String" />
            <asp:ControlParameter Name="situacao" ControlID="drpSituacao" PropertyName="SelectedValue" Type="Object" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:virtualobjectdatasource Culture="pt-BR" ID="odsSituacao" runat="server"
        SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
        <SelectParameters>
            <asp:Parameter Name="typeName" DefaultValue="Glass.Situacao, Glass.Comum" />
        </SelectParameters>
    </colo:virtualobjectdatasource>
</asp:Content>
