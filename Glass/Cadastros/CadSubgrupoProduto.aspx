<%@ Page Title="Subgrupos de Produto" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadSubgrupoProduto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadSubgrupoProduto" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString"
    TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        function exibirLog(idGrupo) {
            TagToTip("log_" + idGrupo, FADEIN, 300, COPYCONTENT, false, TITLE, 'Alterações no tipo cálculo', CLOSEBTN, true, CLOSEBTNTEXT, 'Fechar',
                CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true)
        }

        function onAtualizar() {
            try{
                var tipoCalc = parseInt(FindControl("hdfTipoCalc", "input").value);
                var tipoCalcNf = parseInt(FindControl("hdfTipoCalcNf", "input").value);
                
                var tipoCalcNovo = parseInt(FindControl("drpCalculo", "select").value);
                var tipoCalcNfNovo = parseInt(FindControl("drpCalculoNf", "select").value);
            
                var idGrupoVidro = <%= (int)Glass.Data.Model.NomeGrupoProd.Vidro %>;
                var idGrupoProd = parseInt(GetQueryString("IdGrupoProd"));
                
                var tipoCalcQtd = <%= (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd %>;
                var tipoCalcM2 = <%= (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 %>;
                
                if(idGrupoVidro == idGrupoProd && ((tipoCalc == tipoCalcQtd && tipoCalcNovo == tipoCalcM2) || (tipoCalcNf == tipoCalcQtd && tipoCalcNfNovo == tipoCalcM2)) &&
                 !confirm('Ao alterar o cálculo deste subgrupo, os dados Altura, largura, processo e aplicação serão perdidos ao atualizar os produtos.\n'+
                            'Deseja realmente efetuar alteração? ')){   
                    return false;
                }
            }
            catch(err){
                alert(err);
            }
        }        

        function PesquisaCliente()
        {
            openWindow(570, 760, '../Utils/SelCliente.aspx');
        }

        function naoGerarVolume(control) {

            var tr = control.parentElement.parentElement;
            var chkPermitirItemRevendaNaVenda = FindControl("chkPermitirItemRevendaNaVenda", "input", tr);
            var chkVolume = FindControl("chkGeraVolume", "input", tr);
            
            if(chkPermitirItemRevendaNaVenda.checked && chkVolume.checked)
            {
                chkVolume.checked = false;
                alert("Para habilitar a permissão de item de revenda na venda, a opção Gera Volume deve estar desmarcada.");
            }
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdSubgrupoProd" runat="server" SkinID="gridViewEditable" EnableViewState="false"
                    DataSourceID="odsSubgrupoProd" DataKeyNames="IdSubgrupoProd" OnRowDataBound="grdSubgrupoProd_RowDataBound"
                    OnRowCommand="grdSubgrupoProd_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif" alt="Editar" /></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir0" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" Visible='<%# !(bool)Eval("SubgrupoSistema") %>' OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este Subgrupo de Produto?&quot;);" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="return onAtualizar();" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="btnAtivarProdutos" runat="server" CommandName="AtivarProdutos" CommandArgument='<%# Eval("IdSubgrupoProd") %>' ImageUrl="~/Images/Ok.gif"
                                    ToolTip="Ativar todos os produtos deste grupo" OnClientClick="return confirm(&quot;Tem certeza que deseja ativar todos os produtos deste Grupo?&quot;);" />
                                <asp:ImageButton ID="btnInvativarProdutos" runat="server" CommandName="InativarProdutos" CommandArgument='<%# Eval("IdSubgrupoProd") %>' ImageUrl="~/Images/Inativar.gif"
                                    ToolTip="Inativar todos os produtos deste grupo" OnClientClick="return confirm(&quot;Tem certeza que deseja inativar todos os produtos deste Grupo?&quot;);" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código" SortExpression="IdSubgrupoProd">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdSubgrupoProd") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdSubgrupoProd") %>'></asp:Label>
                                <asp:HiddenField ID="hdfIdGrupoProd" runat="server" Value='<%# Bind("IdGrupoProd") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricaoIns" runat="server" MaxLength="150" Text='<%# Bind("Descricao") %>'
                                    Width="150px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cálculo" SortExpression="TipoCalculo">
                            <FooterTemplate>
                                <asp:DropDownList ID="drpCalculoIns" runat="server" DataSourceID="odsTipoCalc"
                                    DataTextField="Translation" DataValueField="Key">
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("TipoCalculo")).Format() %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:HiddenField runat="server" ID="hdfTipoCalc" Value='<%# Bind("TipoCalculo") %>' />
                                <asp:DropDownList ID="drpCalculo" runat="server" SelectedValue='<%# Bind("TipoCalculo") %>'
                                    AppendDataBoundItems="True" DataSourceID="odsTipoCalc" DataTextField="Translation"
                                    DataValueField="Key">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cálculo NF" SortExpression="TipoCalculoNf">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("TipoCalculoNf")).Format() %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:HiddenField runat="server" ID="hdfTipoCalcNf" Value='<%# Bind("TipoCalculoNf") %>' />
                                <asp:DropDownList ID="drpCalculoNf" runat="server" SelectedValue='<%# Bind("TipoCalculoNf") %>'
                                    AppendDataBoundItems="True" DataSourceID="odsTipoCalcNf" DataTextField="Translation"
                                    DataValueField="Key">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpCalculoNfIns" runat="server" DataSourceID="odsTipoCalcNf"
                                    DataTextField="Translation" DataValueField="Key">
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Produtos para Estoque" SortExpression="ProdutosEstoque">
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Bind("ProdutosEstoque") %>'
                                    Visible='<%# Eval("ExibirProdutosEstoque") %>' Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Bind("ProdutosEstoque") %>'
                                    Visible='<%# Eval("ExibirProdutosEstoque") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkProdutosEstoque" runat="server" Checked="True" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vidro Temperado" SortExpression="IsVidroTemperado">
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBox3" runat="server" Checked='<%# Bind("IsVidroTemperado") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox3" runat="server" Checked='<%# Bind("IsVidroTemperado") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkVidroTemperado" runat="server" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Bloquear Estoque *" SortExpression="BloquearEstoque">
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkBloquearEstoque" runat="server" Checked='<%# Bind("BloquearEstoque") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkBloquearEstoque" runat="server" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkBloquearEstoque" runat="server" Checked='<%# Bind("BloquearEstoque") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Alterar Estoque" SortExpression="NaoAlterarEstoque">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkAlterarEstoque" runat="server" Enabled="false" Checked='<%# Eval("AlterarEstoque") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkAlterarEstoque" runat="server" Checked='<%# Bind("AlterarEstoque") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkAlterarEstoque" runat="server" Checked="true" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Alterar Estoque Fiscal" SortExpression="NaoAlterarEstoqueFiscal">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkAlterarEstoqueFiscal" runat="server" Enabled="false" Checked='<%# Eval("AlterarEstoqueFiscal") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkAlterarEstoqueFiscal" runat="server" Checked='<%# Bind("AlterarEstoqueFiscal") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkAlterarEstoqueFiscal" runat="server" Checked="true" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Exibir Mensagem Estoque" SortExpression="ExibirMensagemEstoque">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkExibirMensagemEstoque" runat="server" Checked='<%# Bind("ExibirMensagemEstoque") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkExibirMensagemEstoque" runat="server" Checked='<%# Bind("ExibirMensagemEstoque") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkExibirMensagemEstoque" runat="server" Checked="True" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Gera Volume?" SortExpression="GeraVolume">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkGeraVolume" runat="server" onchange="naoGerarVolume(this);" Checked='<%# Bind("GeraVolume") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkGeraVolume" runat="server" onchange="naoGerarVolume(this);" Checked='<%# Bind("GeraVolume") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkGeraVolume" runat="server" Checked="False" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Lib. pendende produção?" SortExpression="LiberarPendenteProducao">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkLibPendenteProducao" runat="server" Checked='<%# Bind("LiberarPendenteProducao") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkLibPendenteProducao" runat="server" Checked='<%# Bind("LiberarPendenteProducao") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkLibPendenteProducao" runat="server" Checked="False" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Permitir Item Revenda Na Venda?" SortExpression="PermitirItemRevendaNaVenda">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkPermitirItemRevendaNaVenda" runat="server" Checked='<%# Bind("PermitirItemRevendaNaVenda") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkPermitirItemRevendaNaVenda" runat="server" onchange="naoGerarVolume(this);" Checked='<%# Bind("PermitirItemRevendaNaVenda") %>'  />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkPermitirItemRevendaNaVenda" runat="server" onchange="naoGerarVolume(this);" Checked="False" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Núm. Dias Mín. Entrega" SortExpression="NumeroDiasMinimoEntrega">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("NumeroDiasMinimoEntrega") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumDiasMinEntrega" runat="server" Text='<%# Bind("NumeroDiasMinimoEntrega") %>'
                                    Width="50px" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNumDiasMinEntrega" runat="server" onkeypress="return soNumeros(event, true, true)"
                                    Text='<%# Bind("NumeroDiasMinimoEntrega") %>' Width="50px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Dia Semana Entrega" SortExpression="DiaSemanaEntrega">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Glass.FuncoesData.ObtemNomeDiaSemana(((int?)Eval("DiaSemanaEntrega")).GetValueOrDefault()).Format() %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpDiaSemana" runat="server" SelectedValue='<%# Bind("DiaSemanaEntrega") %>'>
                                    <asp:ListItem></asp:ListItem>
                                    <asp:ListItem Value="0">Domingo</asp:ListItem>
                                    <asp:ListItem Value="1">Segunda-feira</asp:ListItem>
                                    <asp:ListItem Value="2">Terça-feira</asp:ListItem>
                                    <asp:ListItem Value="3">Quarta-feira</asp:ListItem>
                                    <asp:ListItem Value="4">Quinta-feira</asp:ListItem>
                                    <asp:ListItem Value="5">Sexta-feira</asp:ListItem>
                                    <asp:ListItem Value="6">Sábado</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpDiaSemana" runat="server" SelectedValue='<%# Bind("DiaSemanaEntrega") %>'>
                                    <asp:ListItem></asp:ListItem>
                                    <asp:ListItem Value="0">Domingo</asp:ListItem>
                                    <asp:ListItem Value="1">Segunda-feira</asp:ListItem>
                                    <asp:ListItem Value="2">Terça-feira</asp:ListItem>
                                    <asp:ListItem Value="3">Quarta-feira</asp:ListItem>
                                    <asp:ListItem Value="4">Quinta-feira</asp:ListItem>
                                    <asp:ListItem Value="5">Sexta-feira</asp:ListItem>
                                    <asp:ListItem Value="6">Sábado</asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo de Subgrupo" SortExpression="TipoSubgrupo">
                            <ItemTemplate>
                                <asp:Label ID="Label50" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("TipoSubgrupo")).Format() %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoSubgrupo" runat="server" SelectedValue='<%# Bind("TipoSubgrupo") %>'
                                                  DataSourceID="odsTiposSubgrupo" DataTextField="Translation" DataValueField="Key">
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipoSubgrupo" runat="server" 
                                                  DataSourceID="odsTiposSubgrupo" DataTextField="Translation" DataValueField="Key">
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Cliente" SortExpression="TipoSubgrupo">
                            <ItemTemplate>
                                <asp:Label ID="LabelCliente" runat="server" Text='<%# Bind("NomeCliente") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                Text='<%# Bind("IdCli") %>'></asp:TextBox>
                                <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="return PesquisaCliente();" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                Text='<%# Bind("IdCli") %>'></asp:TextBox>
                                <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="return PesquisaCliente();" />
                            </FooterTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Loja" SortExpression="IdLoja">
                            <ItemTemplate>
                                <asp:Label ID="LabelLoja" runat="server" Text='<%# Bind("Loja") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpLoja" runat="server" AppendDataBoundItems="True" DataSourceID="odsLoja"
                                    DataTextField="Name" DataValueField="Id" SelectedValue='<%# Bind("IdLoja") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpLoja" runat="server" AppendDataBoundItems="True" DataSourceID="odsLoja"
                                    DataTextField="Name" DataValueField="Id">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc2:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="SubgrupoProduto" IdRegistro='<%# (uint)(int)Eval("IdSubgrupoProd") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                </td> </tr>
                                <tr>
                                    <td colspan="3">
                                    </td>
                                    <td colspan="12" align="left" style="color: Red">
                                        os valores entre parênteses do "<%# Glass.Global.CalculosFluxo.NOME_MLAL %>" são valores
                                        de arredondamento para cálculo do valor
                                    </td>
                                </tr>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">
                                    <img border="0" src="../Images/insert.gif" alt="Inserir" /></asp:LinkButton>
                                </td> </tr>
                                <tr>
                                    <td colspan="3">
                                    </td>
                                    <td colspan="12" align="left" style="color: Red">
                                        os valores entre parênteses do "<%# Glass.Global.CalculosFluxo.NOME_MLAL %>" são valores
                                        de arredondamento para cálculo do valor
                                    </td>
                                </tr>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

                <colo:VirtualObjectDataSource runat="server" ID="odsTiposSubgrupo"
                    TypeName="Colosoft.Translator" SelectMethod="GetTranslatesFromTypeName">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.TipoSubgrupoProd, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>

                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSubgrupoProd" runat="server"
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.SubgrupoProd" EnableViewState="false"
                    DeleteMethod="ApagarSubgrupoProduto" EnablePaging="True" EnableCaching="false"
                    DeleteStrategy="GetAndDelete"
                    MaximumRowsParameterName="pageSize" 
                    SelectMethod="PesquisarSubgruposProduto"
                    SelectByKeysMethod="ObtemSubgrupoProduto"
                    SortParameterName="sortExpression"
                    TypeName="Glass.Global.Negocios.IGrupoProdutoFluxo"
                    UpdateMethod="SalvarSubgrupoProduto"
                    UpdateStrategy="GetAndUpdate">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idGrupoProd" QueryStringField="idGrupoProd" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                 <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCalc" runat="server" 
                    SelectMethod="ObtemTiposCalculo"
                    TypeName="Glass.Global.UI.Web.Process.GrupoProdutoDataSource">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="true" Name="exibirDecimal" Type="Boolean" />
                        <asp:Parameter DefaultValue="false" Name="notaFiscal" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCalcNf" runat="server" 
                    SelectMethod="ObtemTiposCalculo"
                    TypeName="Glass.Global.UI.Web.Process.GrupoProdutoDataSource">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="true" Name="exibirDecimal" Type="Boolean" />
                        <asp:Parameter DefaultValue="true" Name="notaFiscal" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>

                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="ObtemLojas"
                    TypeName="Glass.Global.Negocios.ILojaFluxo">
                </colo:VirtualObjectDataSource>

            </td>
        </tr>
        <tr>
            <td align="center">
                * Bloqueia venda dos produtos deste subgrupo se não houver em estoque<br />
                <asp:Label ID="lblDadosPerdidos" runat="server" Text="Ao alterar o cálculo dos subgrupos alguns dados poderão ser perdidos."
                    ForeColor="Red" Visible="False" Font-Bold="True"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
