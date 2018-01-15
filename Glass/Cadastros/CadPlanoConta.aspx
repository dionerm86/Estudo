<%@ Page Title="Planos de Conta" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadPlanoConta.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadPlanoConta" %>

<%@ Register Src="~/Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(exportarExcel) {
            var idGrupo = FindControl("drpGrupoContaFiltro", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ListaPlanoContasDesc&idGrupo=" + (idGrupo != "" ? idGrupo : "0") +
                "&situacao=" + situacao + "&exportarExcel=" + exportarExcel);

            return false;
        }

        function onSave() {
            if (FindControl("drpGrupoConta", "select").value == "") {
                alert("Selecione o grupo de conta.");
                return false;
            }

            if (FindControl("txtDescricao", "input").value == "") {
                alert("Informe a descrição do plano de conta.");
                return false;
            }
        }
        
        function SetExibirDre(idConta, controle) {

            var retorno = CadPlanoConta.SetExibirDre(idConta, controle.checked);

            if (retorno.error != null) {
                alert(retorno.error.description);
                return false;
            }
            else if (retorno.value.split('|')[0] == "Erro") {
                alert(retorno.value.split('|')[1]);
                controle.checked = !controle.checked;
                return false;
            }

            alert(retorno.value.split('|')[0]);
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupoContaFiltro" runat="server" DataSourceID="odsGrupoConta"
                                DataTextField="Name" DataValueField="Id" AppendDataBoundItems="True"
                                OnDataBound="drpGrupoContaFiltro_DataBound">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server">
                                <asp:ListItem Value="">Todas</asp:ListItem>
                                <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
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
                <asp:GridView SkinID="gridViewEditable" ID="grdPlanoConta" runat="server"
                              DataKeyNames="IdConta" DataSourceID="odsPlanoConta" EnableViewState="false">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" Visible='<%# Eval("PlanoContasSistema") %>'>
                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" 
                                    CommandName="Delete" CommandArgument='<%# Eval("IdConta") %>' ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este Plano de Conta?&quot;);"
                                    ToolTip="Excluir" Visible='<%# Eval("PlanoContasSistema") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Num. Conta" SortExpression="IdContaGrupo">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdContaGrupo") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdContaGrupo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Grupo" SortExpression="Grupo">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Grupo") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpGrupoConta" runat="server" DataSourceID="odsGrupoConta"
                                    DataTextField="Name" DataValueField="Id" SelectedValue='<%# Bind("IdGrupo") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblDescrGrupo" runat="server" OnLoad="lblDescrGrupo_Load"></asp:Label>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="labelDescricao" ClientIDMode="Static" runat="server" Text='<%# Eval("Descricao") %>' style="display: none;"></asp:Label>
                                <asp:TextBox ID="txtEditarDescricao" runat="server" MaxLength="60" ClientIDMode="Static"
                                    Text='<%# Bind("Descricao") %>' Visible='<%# !PlanoContasFluxo.PlanoContasEmUso((int)Eval("IdConta"), false) %>'
                                    Width="250px"></asp:TextBox>
                                <script type="text/javascript">
                                    if (document.getElementById('txtEditarDescricao') == null) {
                                        document.getElementById('labelDescricao').style.display = 'block';
                                    }
                                </script>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="60" Text='<%# Bind("Descricao") %>'
                                    Width="250px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Exibir no DRE?" SortExpression="ExibirDre">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkExibirDre" runat="server" Checked='<%# Bind("ExibirDre") %>'
                                    onclick='<%# "SetExibirDre(" + Eval("IdConta") + ", this);" %>' />
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server"
                                    SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server">
                                    <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Colosoft.Translator.Translate(Eval("Situacao")).Format() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="PlanoContas" IdRegistro='<%# (int)Eval("IdConta") %>' />
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click" OnClientClick="return onSave();"><img border="0" 
                                    src="../Images/ok.gif" /></asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsGrupoConta" runat="server"
                    SelectMethod="ObtemGruposContaCadastro"
                    TypeName="Glass.Financeiro.Negocios.IPlanoContasFluxo">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPlanoConta" runat="server" 
                    DataObjectTypeName="Glass.Financeiro.Negocios.Entidades.PlanoContas"
                    DeleteMethod="ApagarPlanoContas" EnablePaging="True" 
                    DeleteStrategy="GetAndDelete"
                    MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarPlanosContas" 
                    SelectByKeysMethod="ObtemPlanoContas"
                    SortParameterName="sortExpression"
                    TypeName="Glass.Financeiro.Negocios.IPlanoContasFluxo"
                    UpdateMethod="SalvarPlanoContas"
                    UpdateStrategy="GetAndUpdate">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpGrupoContaFiltro" Name="idGrupo" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao"
                            PropertyName="SelectedValue" />
                    </SelectParameters>
                    <DeleteParameters>
                        <asp:Parameter Name="idConta" Type="Int32" />
                    </DeleteParameters>
                </colo:VirtualObjectDataSource>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false);">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
