<%@ Page Title="Relatório dinâmico" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadRelatorioDinamico.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Dinamicos.CadRelatorioDinamico" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvRelatorioDinamico" runat="server" SkinID="defaultDetailsView"
                    DataSourceID="odsRelatorioDinamico" DataKeyNames="IdRelatorioDinamico">
                    <Fields>
                        <asp:TemplateField HeaderText="Nome relatório" SortExpression="NomeRelatorio">
                            <ItemTemplate>
                                <asp:Label ID="lblNomeRelatorio" runat="server" Text='<%# Bind("NomeRelatorio") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("NomeRelatorio") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Arquivo rdlc">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkRptBase" runat="server" PostBackUrl='<%# string.Format("../../Handlers/Download.ashx?filePath={0}&fileName=rptDinamico.rdlc", ObterCaminhoArquivoRdlc((int)Eval("IdRelatorioDinamico"))) %>'>
                                <img src="../../Images/Clipe.gif" />
                                </asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:FileUpload runat="server" ID="fluArquivoRdlc"></asp:FileUpload>
                                <br />
                                <asp:LinkButton ID="lnkRptBase" runat="server" PostBackUrl='<%# "../../Handlers/Download.ashx?filePath=" + Server.MapPath("~/Relatorios/Dinamicos/rptDinamico.rdlc") + "&fileName=rptDinamico.rdlc" %>'>Relatório base</asp:LinkButton>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Link de inserção" SortExpression="LinkInsercaoNome">
                            <ItemTemplate>
                                <asp:Label ID="lblLinkInsercaoNome" runat="server" Text='<%# Bind("LinkInsercaoNome") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtLinkInsercaoNome" Text='<%# Bind("LinkInsercaoNome") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="URL Link de inserção" SortExpression="LinkInsersaoUrl">
                            <ItemTemplate>
                                <asp:Label ID="lblLinkInsersaoUrl" runat="server" Text='<%# Bind("LinkInsersaoUrl") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtLinkInsersaoUrl" Text='<%# Bind("LinkInsersaoUrl") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtde. de registros por pagina" SortExpression="QuantidadeRegistrosPorPagina">
                            <ItemTemplate>
                                <asp:Label ID="lblQuantidadeRegistrosPorPagina" runat="server" Text='<%# Bind("QuantidadeRegistrosPorPagina") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtQuantidadeRegistrosPorPagina" Text='<%# Bind("QuantidadeRegistrosPorPagina") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <asp:Label ID="lblSituacao" runat="server" Text='<%# Bind("Situacao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Comando SQL" SortExpression="ComandoSql">
                            <ItemTemplate>
                                <pre><asp:Label ID="lblComandoSql" runat="server" Text='<%# Eval("ComandoSql").ToString() %>'></asp:Label></pre>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtComandoSql" runat="server" Rows="35" Width="600" TextMode="MultiLine" Text='<%# Bind("ComandoSql") %>'></asp:TextBox>
                                <br />
                                <asp:Label ID="lblAvisoSql" runat="server" ForeColor="Red" Text='É necessário que o sql tenha um "Where" pré-definido e a tag [where] logo após a última cláusula. Ex.: Select * From cliente Where 1 [where]'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:Button ID="btnEditar" runat="server" CommandName="Edit" Text="Editar" />
                                <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar" CausesValidation="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Button ID="btnUpdate" runat="server" CommandName="Update" Text="Atualizar" />
                                <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar" CausesValidation="False" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" />
                                <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar" CausesValidation="False" />
                            </InsertItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" Wrap="False" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRelatorioDinamico" runat="server"
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.RelatorioDinamico"
                    InsertMethod="SalvarRelatorioDinamico" CreateDataObjectMethod="CriarRelatorioDinamico"
                    SelectMethod="get_RelatorioDinamico"
                    TypeName="Glass.Global.UI.Web.Process.RelatorioDinamico.CadastroRelatorioDinamicoFluxo"
                    UpdateMethod="SalvarRelatorioDinamico"
                    UpdateStrategy="GetAndUpdate">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center"></td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblIcones" runat="server" CssClass="subtitle" Text="Ícones"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdIcones" runat="server" AllowSorting="false" DataKeyNames="IdRelatorioDinamicoIcone"
                    DataSourceID="odsIcone" ShowFooter="true" SkinID="gridViewEditable" AutoGenerateColumns="false">
                    <Columns>
                        <asp:TemplateField>
                            <FooterTemplate>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="False" CommandName="Edit">
                                <img border="0" src="../../Images/Edit.gif" alt="Editar" /></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir0" runat="server" CausesValidation="False" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif" OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este ícone?&quot;)" ToolTip="Excluir" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px" ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CausesValidation="False" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <HeaderStyle Wrap="False" />
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ícone">
                            <ItemTemplate>
                                <asp:Image runat="server" ImageUrl='<%# Eval("ImagemIcone") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Image runat="server" ImageUrl='<%# Eval("ImagemIcone") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:FileUpload ID="fluIcone" runat="server" />
                            </FooterTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição">
                            <ItemTemplate>
                                <asp:Label ID="lblDescricao0" runat="server" Text='<%# Eval("NomeIcone") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao0" runat="server" Text='<%# Bind("NomeIcone") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Função JavaSript">
                            <ItemTemplate>
                                <asp:Label ID="lblFuncaoJavascript" runat="server" Text='<%# Eval("FuncaoJavaScript") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtFuncaoJavascript" runat="server" TextMode="MultiLine" Text='<%# Bind("FuncaoJavaScript") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtFuncaoJavascript" TextMode="MultiLine" runat="server"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Método de Visibilidade">
                            <ItemTemplate>
                                <asp:Label ID="lblMetodoVisibilidade" runat="server" Text='<%# Eval("MetodoVisibilidade") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtMetodoVisibilidade" runat="server" TextMode="MultiLine" Text='<%# Bind("MetodoVisibilidade") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtMetodoVisibilidade" TextMode="MultiLine" runat="server"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Mostrar no final?">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkMostrarFinalGrid" runat="server" Checked='<%# Eval("MostrarFinalGrid") %>'></asp:CheckBox>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkMostrarFinalGrid" runat="server" Checked='<%# Bind("MostrarFinalGrid") %>'></asp:CheckBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkMostrarFinalGrid" runat="server"></asp:CheckBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Seq.">
                            <ItemTemplate>
                                <asp:Label ID="lblNumSeq" runat="server" Text='<%# Eval("NumSeq") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumSeq" runat="server" Width="40" Text='<%# Bind("NumSeq") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNumSeq" runat="server" Width="40"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInsIcone" runat="server" OnClick="lnkInsIcone_Click">
                        <img border="0" src="../../Images/ok.gif" alt="Ok" /></asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
        </tr>
         <tr>
            <td>
                <span>
                    <b>Método de visibilidade:</b><br />
                     <i> - Deve seguir o seguinte padrão:</i> namespace; assembly; método. Ex:. Glass.Global.Negocios.IRelatorioDinamicoFluxo;Glass.Negocios;VerificarPodeEditarOrcamento([Num], {@IdFunc})
                </span>
                <br /><br />
                <span>
                    <b>Função JavaSript:</b><br />
                     <i> - Para retornar ao relatório dinâmico informar:</i> &IdRelDinamico={@IdRelDinamico}
                </span>
            </td>
        </tr>
        <tr>
            <td align="center">&nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblFiltros" runat="server" CssClass="subtitle" Text="Filtros"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdFiltros" runat="server" AllowSorting="false" DataKeyNames="IdRelatorioDinamicoFiltro"
                    DataSourceID="odsFiltro" ShowFooter="true" SkinID="gridViewEditable" AutoGenerateColumns="false">
                    <Columns>
                        <asp:TemplateField>
                            <FooterTemplate>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="False" CommandName="Edit">
                                <img border="0" src="../../Images/Edit.gif" alt="Editar" /></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CausesValidation="False" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif" OnClientClick="return confirm(&quot;Excluir este ambiente fará com que todos os produtos do mesmo sejam excluídos também, confirma exclusão?&quot;)" ToolTip="Excluir" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px" ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" ValidationGroup="ambiente" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CausesValidation="False" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <HeaderStyle Wrap="False" />
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição">
                            <ItemTemplate>
                                <asp:Label ID="lblDescricao" runat="server" Text='<%# Eval("NomeFiltro") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Text='<%# Bind("NomeFiltro") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Nome coluna SQL">
                            <ItemTemplate>
                                <asp:Label ID="lblNomeColunaSql" runat="server" Text='<%# Eval("NomeColunaSql") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNomeColunaSql" runat="server" Text='<%# Bind("NomeColunaSql") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNomeColunaSql" runat="server"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo controle">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("TipoControle")) %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoControle" runat="server" DataSourceID="odsTipoControle"
                                    DataTextField="Translation" DataValueField="Key" AppendDataBoundItems="True" SelectedValue='<%# Bind("TipoControle") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipoControle" runat="server" DataSourceID="odsTipoControle"
                                    DataTextField="Translation" DataValueField="Key" AppendDataBoundItems="True">
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Opções (Lista seleção e múltipla seleção) *">
                            <ItemTemplate>
                                <asp:Label ID="lblOpcoes" runat="server" Text='<%# Eval("Opcoes") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtOpcoes" runat="server" Text='<%# Bind("Opcoes") %>' Width="300"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtOpcoes" runat="server" Width="300"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor padrão">
                            <ItemTemplate>
                                <asp:Label ID="lblValorPadrao" runat="server" Text='<%# Eval("ValorPadrao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorPadrao" runat="server" Text='<%# Bind("ValorPadrao") %>' Width="100"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValorPadrao" runat="server" Width="100"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Seq.">
                            <ItemTemplate>
                                <asp:Label ID="lblNumSeq" runat="server" Text='<%# Eval("NumSeq") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumSeq" runat="server" Width="40" Text='<%# Bind("NumSeq") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNumSeq" runat="server" Width="40"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInsFiltro" runat="server" OnClick="lnkInsFiltro_Click"
                                    ValidationGroup="ambiente">
                        <img border="0" src="../../Images/ok.gif" alt="Ok" /></asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label runat="server" ID="lblAviso" Text="* As opções devem ser lançadas no seguinte formato: ValorExibido1,ValorFiltrado1|ValorExibido2,ValorFiltrado2|ValorExibido3,ValorFiltrado3 assim em diante" ForeColor="Red"></asp:Label>
                <br />
                <asp:Label runat="server" ID="Label2" Text="Caso queira utilizar sql, começar a sentença com 'sql:' sem as aspas e o sql logo em seguida, que deverá recuperar dois campos: o primeiro sendo a descrição e o segundo o valor a ser filtrado." ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                    <table class="gridStyle">
                        <tbody>
                            <tr>
                                <th scope="col" colspan="2">Variáveis disponíveis</th>
                            </tr>
                            <tr>
                                <td><b>Cód. do relatório dinâmico</b></td>
                                <td>{@IdRelDinamico}</td>
                            </tr>
                            <tr class="alt">
                                <td><b>Funcionário Atual</b></td>
                                <td>{@IdFunc}</td>
                            </tr>
                        </tbody>
                    </table>
                 <table class="gridStyle">
                        <tbody>
                            <tr>
                                <th scope="col" colspan="2">Valor padrão para datas</th>
                                <th scope="col">Ex:. -2m,+2m</th>
                            </tr>
                            <tr>
                                <td><b>Primeiro dia do mês</b></td>
                                <td>PrimeiroDiaMes</td>
                                <td>PrimeiroDiaMes, UltimoDiaMes</td>
                            </tr>
                            <tr class="alt">
                                <td><b>Ultimo dia do mês</b></td>
                                <td>UltimoDiaMes</td>
                                <td>PrimeiroDiaMes, UltimoDiaMes</td>
                            </tr>
                            <tr>
                                <td><b>Dia</b></td>
                                <td>d</td>
                                <td>-4d, +1s</td>
                            </tr>
                            <tr class="alt">
                                <td><b>Semana</b></td>
                                <td>s</td>
                                <td>-1d, +1s</td>
                            </tr>
                            <tr>
                                <td><b>Mês</b></td>
                                <td>m</td>
                                <td>-3m, +3m</td>
                            </tr>
                            <tr class="alt">
                                <td><b>Ano</b></td>
                                <td>a</td>
                                <td>-1a, +1a</td>
                            </tr>
                        </tbody>
                    </table>
            </td>
        </tr>

        <tr>
            <td>
                <colo:VirtualObjectDataSource ID="odsIcone" runat="server" Culture="pt-BR"
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.RelatorioDinamicoIcone"
                    DeleteMethod="ApagarIcone"
                    DeleteStrategy="GetAndDelete"
                    EnablePaging="True"
                    SelectByKeysMethod="ObterIcone"
                    SelectMethod="ObterIcones"
                    TypeName="Glass.Global.UI.Web.Process.RelatorioDinamico.CadastroRelatorioDinamicoFluxo"
                    UpdateMethod="SalvarIcone"
                    UpdateStrategy="GetAndUpdate">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource ID="odsFiltro" runat="server" Culture="pt-BR"
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.RelatorioDinamicoFiltro"
                    DeleteMethod="ApagarFiltro"
                    DeleteStrategy="GetAndDelete"
                    EnablePaging="True"
                    SelectByKeysMethod="ObterFiltro"
                    SelectMethod="ObterFiltros"
                    TypeName="Glass.Global.UI.Web.Process.RelatorioDinamico.CadastroRelatorioDinamicoFluxo"
                    UpdateMethod="SalvarFiltro"
                    UpdateStrategy="GetAndUpdate">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoControle" runat="server"
                    SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.TipoControle, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
