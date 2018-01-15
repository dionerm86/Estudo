<%@ Page Title="Bens/Componentes Ativo Imobilizado" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstBemAtivoImobilizado.aspx.cs" Inherits="Glass.UI.Web.Listas.LstBemAtivoImobilizado" EnableEventValidation="false" %>

<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc1" %>
<%@ Register src="../Controls/ctrlSelPopup.ascx" tagname="ctrlSelPopup" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript">
        function getProduto()
        {
            openWindow(600, 800, "../Utils/SelProd.aspx");
        }

        function setProduto(codInterno)
        {
            FindControl("txtCodProd", "input").value = codInterno;
            loadProduto(codInterno);
        }
        
        function loadProduto(codInterno)
        {
            if (codInterno == "")
            {
                FindControl("lblDescrProd", "span").innerHTML = "";
                FindControl("hdfIdProd", "input").value = "";
                return;
            }

            var resposta = MetodosAjax.GetProd(codInterno).value.split(';');
            if (resposta[0] == "Erro")
            {
                FindControl("txtCodProd", "input").value = "";
                FindControl("lblDescrProd", "span").innerHTML = "";
                FindControl("hdfIdProd", "input").value = "";
                alert(resposta[1]);
                return;
            }

            FindControl("lblDescrProd", "span").innerHTML = resposta[2];
            FindControl("hdfIdProd", "input").value = resposta[1];
        }

        function getBem()
        {
            openWindow(600, 800, "../Utils/SelBemAtivoImob.aspx?tipo=1");
        }

        function setBem(idBemAtivoImobilizado)
        {
            FindControl("txtCodBem", "input").value = idBemAtivoImobilizado;
        }

        function validaProd(val, args)
        {
            args.IsValid = FindControl("hdfIdProd", "input").value != "";
        }

        function validarDados(inserindo)
        {
            if (!validate())
                return false;
            
            var idLoja = FindControl("drpLoja", "select").value;
            var idProd = FindControl("hdfIdProd", "input").value;
            var idContaContabil = FindControl("drpPlanoContaContabil", "select").value;
            var idBemPrinc = FindControl("txtCodBem", "input").value;
            var idBemAtivoImobilizado = FindControl("lblIdBemAtivoImobilizado", "span");
            idBemAtivoImobilizado = idBemAtivoImobilizado != null ? idBemAtivoImobilizado.innerHTML : "";

            var resposta = LstBemAtivoImobilizado.Validar(idLoja, idProd, idContaContabil, idBemPrinc, idBemAtivoImobilizado).value.split(';');
            if (resposta[0] == "Erro")
            {
                alert(resposta[1]);
                return false;
            }
            
            if (!inserindo)
            {
                if (idBemAtivoImobilizado == idBemPrinc)
                {
                    alert("Não é possível atribuir como bem principal o próprio item.");
                    return false;
                }
            }
            
            return true;
        }

        var inicial = null;

        function atualizaCentroCustos()
        {
            var idLoja = FindControl("drpLoja", "select").value;
            var drpCentroCusto = FindControl("drpCentroCusto", "select");

            var atual = drpCentroCusto.value;
            drpCentroCusto.innerHTML = LstBemAtivoImobilizado.GetCentrosCustos(idLoja).value;
            drpCentroCusto.value = atual;
        }
    </script>
    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdBemAtivoImobilizado" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" 
                    DataKeyNames="IdBemAtivoImobilizado" DataSourceID="odsBemAtivoImobilizado" 
                    GridLines="None" ShowFooter="True" 
                    ondatabound="grdBemAtivoImobilizado_DataBound" 
                    onrowcommand="grdBemAtivoImobilizado_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" 
                                    ImageUrl="~/Images/ok.gif" 
                                    onclientclick="if (!validarDados(false)) return false" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CommandName="Cancel" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" 
                                    ImageUrl="~/Images/EditarGrid.gif" CausesValidation="false" />
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" 
                                    onclientclick="if (!confirm(&quot;Deseja excluir esse bem/componente?&quot;)) return false" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód." SortExpression="IdBemAtivoImobilizado">
                            <EditItemTemplate>
                                <asp:Label ID="lblIdBemAtivoImobilizado" runat="server" 
                                    Text='<%# Bind("IdBemAtivoImobilizado") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" 
                                    Text='<%# Bind("IdBemAtivoImobilizado") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Loja" SortExpression="NomeLoja">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" 
                                    DataTextField="NomeFantasia" DataValueField="IdLoja" 
                                    SelectedValue='<%# Bind("IdLoja") %>' onchange="atualizaCentroCustos()">
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpLoja" runat="server" AppendDataBoundItems="True" 
                                    DataSourceID="odsLoja" DataTextField="NomeFantasia" DataValueField="IdLoja" 
                                    SelectedValue='<%# Bind("IdLoja") %>' onchange="atualizaCentroCustos()">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvLoja" runat="server" 
                                    ControlToValidate="drpLoja" Display="Dynamic" ErrorMessage="*" 
                                    ValidationGroup="produto"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("NomeLoja") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProd">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCodProd" runat="server" 
                                    onblur="loadProduto(this.value);" 
                                    onkeydown="if (isEnter(event)) loadProduto(this.value);" 
                                    onkeypress="return !(isEnter(event));" Width="50px" 
                                    Text='<%# Eval("CodInternoProd") %>'></asp:TextBox>
                                <asp:Label ID="lblDescrProd" runat="server" Text='<%# Eval("DescrProd") %>'></asp:Label>
                                <a href="#" onclick="getProduto(); return false;">
                                    <img border="0" src="../Images/Pesquisar.gif" /></a>
                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                                <asp:CustomValidator ID="ctvProduto" runat="server" 
                                    ClientValidationFunction="validaProd" Display="Dynamic" ErrorMessage="*" 
                                    ValidateEmptyText="True" ValidationGroup="produto"></asp:CustomValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCodProd" runat="server" 
                                    onblur="loadProduto(this.value);" 
                                    onkeydown="if (isEnter(event)) loadProduto(this.value);" 
                                    onkeypress="return !(isEnter(event));" Text='<%# Eval("CodInternoProd") %>' 
                                    Width="50px"></asp:TextBox>
                                <asp:Label ID="lblDescrProd" runat="server" Text='<%# Eval("DescrProd") %>'></asp:Label>
                                <a href="#" onclick="getProduto(); return false;">
                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                                <asp:CustomValidator ID="ctvProduto" runat="server" 
                                    ClientValidationFunction="validaProd" Display="Dynamic" ErrorMessage="*" 
                                    ValidateEmptyText="True" ValidationGroup="produto"></asp:CustomValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("CodInternoProd") %>'></asp:Label>
                                -
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DescrProd") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Grupo" SortExpression="Grupo">
                            <EditItemTemplate>
                                <uc2:ctrlSelPopup ID="selGrupo" runat="server" DataSourceID="odsGrupos" Valor='<%# Bind("Grupo") %>'
                                    DataTextField="Descr" DataValueField="Id" ExibirIdPopup="false" TituloTela="Selecione o Grupo" 
                                    Descricao='<%# Eval("DescrGrupo") %>' FazerPostBackBotaoPesquisar="False" 
                                    PermitirVazio="False" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc2:ctrlSelPopup ID="selGrupo" runat="server" DataSourceID="odsGrupos" 
                                    DataTextField="Descr" DataValueField="Id" Descricao='<%# Eval("DescrGrupo") %>' 
                                    ExibirIdPopup="false" FazerPostBackBotaoPesquisar="False" 
                                    Valor='<%# Bind("Grupo") %>' TituloTela="Selecione o Grupo" 
                                    PermitirVazio="False" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("DescrGrupo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Plano de Conta Contábil" 
                            SortExpression="DescrPlanoContaContabil">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpPlanoContaContabil" runat="server" 
                                    AppendDataBoundItems="True" DataSourceID="odsPlanoContaContabil" 
                                    DataTextField="Descricao" DataValueField="IdContaContabil" 
                                    SelectedValue='<%# Bind("IdContaContabil") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvPlanoContaContabil" runat="server" 
                                    ControlToValidate="drpPlanoContaContabil" Display="Dynamic" 
                                    ErrorMessage="*" ValidationGroup="produto"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpPlanoContaContabil" runat="server" 
                                    AppendDataBoundItems="True" DataSourceID="odsPlanoContaContabil" 
                                    DataTextField="Descricao" DataValueField="IdContaContabil" 
                                    SelectedValue='<%# Bind("IdContaContabil") %>' ValidationGroup="produto">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvPlanoContaContabil" runat="server" 
                                    ControlToValidate="drpPlanoContaContabil" Display="Dynamic" 
                                    ErrorMessage="*" ValidationGroup="produto"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" 
                                    Text='<%# Bind("DescrPlanoContaContabil") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="DescrTipo">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server" DataSourceID="odsTipo" onchange="alteraTipo()"
                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("Tipo") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server" DataSourceID="odsTipo" onchange="alteraTipo()"
                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("Tipo") %>'>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("DescrTipo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Bem Principal" 
                            SortExpression="IdBemAtivoImobilizadoPrinc">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCodBem" runat="server" 
                                    Text='<%# Bind("IdBemAtivoImobilizadoPrinc") %>' Width="50px"></asp:TextBox>
                                <a href="#" onclick="getBem(); return false;">
                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" 
                                    Text='<%# Eval("IdBemAtivoImobilizadoPrinc") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCodBem" runat="server" 
                                    Text='<%# Bind("IdBemAtivoImobilizadoPrinc") %>' Width="50px"></asp:TextBox>
                                <a href="#" onclick="getBem(); return false;">
                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                            </FooterTemplate>
                            <FooterStyle Wrap="False" />
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Núm. Parcelas Apropriadas" 
                            SortExpression="NumParc">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumParc" runat="server" Text='<%# Bind("NumParc") %>' 
                                    Width="50px" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvNumParc" runat="server" 
                                    ControlToValidate="txtNumParc" Display="Dynamic" ErrorMessage="*" 
                                    ValidationGroup="produto"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNumParc" runat="server" 
                                    onkeypress="return soNumeros(event, true, true)" Text='<%# Bind("NumParc") %>' 
                                    Width="50px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvNumParc" runat="server" 
                                    ControlToValidate="txtNumParc" Display="Dynamic" ErrorMessage="*" 
                                    ValidationGroup="produto"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("NumParc") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="True" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Centro de Custos" 
                            SortExpression="DescrCentroCusto">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpCentroCusto" runat="server" 
                                    AppendDataBoundItems="True" onchange="FindControl('hdfIdCentroCusto', 'input').value = this.value">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <asp:HiddenField ID="hdfIdCentroCusto" runat="server" 
                                    Value='<%# Bind("IdCentroCusto") %>' />
                                <script type="text/javascript">
                                    inicial = '<%# Eval("IdCentroCusto") %>';
                                </script>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpCentroCusto" runat="server" 
                                    AppendDataBoundItems="True" onchange="FindControl('hdfIdCentroCusto', 'input').value = this.value"
                                    SelectedValue='<%# Eval("IdCentroCusto") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <asp:HiddenField ID="hdfIdCentroCusto" runat="server" 
                                    Value='<%# Bind("IdCentroCusto") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("DescrCentroCusto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Rows="3" 
                                    Text='<%# Bind("Descricao") %>' TextMode="MultiLine" Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Rows="3" 
                                    Text='<%# Bind("Descricao") %>' TextMode="MultiLine" Width="200px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vida útil" SortExpression="VidaUtil">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtVidaUtil" runat="server" Text='<%# Bind("VidaUtil") %>'
                                    onkeypress="return soNumeros(event, true, true)" Width="70px"></asp:TextBox>
                                meses
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtVidaUtil" runat="server" 
                                    onkeypress="return soNumeros(event, true, true)" Text='<%# Bind("VidaUtil") %>' 
                                    Width="70px"></asp:TextBox>
                                meses
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("VidaUtil") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="True" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Cadastro" SortExpression="DataCad">
                            <EditItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("DataCad", "{0:d}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("DataCad", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAdd" runat="server" ImageUrl="~/Images/Insert.gif" 
                                    onclick="imgAdd_Click" ValidationGroup="produto" 
                                    onclientclick="if (!validarDados(true)) return false" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" 
                                    IdRegistro='<%# Eval("IdBemAtivoImobilizado") %>' 
                                    Tabela="BemAtivoImobilizado" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsBemAtivoImobilizado" runat="server" 
                    DataObjectTypeName="Glass.Data.Model.BemAtivoImobilizado" DeleteMethod="Delete" 
                    EnablePaging="True" InsertMethod="Insert" MaximumRowsParameterName="pageSize" 
                    SelectCountMethod="GetCount" SelectMethod="GetList" 
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.BemAtivoImobilizadoDAO" UpdateMethod="Update" 
                    ondeleted="odsBemAtivoImobilizado_Deleted" 
                    oninserted="odsBemAtivoImobilizado_Inserted" 
                    onupdated="odsBemAtivoImobilizado_Updated">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="0" Name="tipo" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipo" runat="server" SelectMethod="GetTiposBemAtivoImobilizado" 
                    TypeName="Glass.Data.EFD.DataSourcesEFD"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoContaContabil" runat="server" 
                    SelectMethod="GetSorted" TypeName="Glass.Data.DAL.PlanoContaContabilDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="1" Name="natureza" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" 
                    TypeName="Glass.Data.DAL.LojaDAO"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupos" runat="server" 
                    SelectMethod="GetGruposBemAtivoImobilizado" 
                    TypeName="Glass.Data.EFD.DataSourcesEFD"></colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
    <script type="text/javascript">
        atualizaCentroCustos();
        if (inicial != null)
            FindControl("drpCentroCusto", "select").value = inicial;
    </script>
</asp:Content>

