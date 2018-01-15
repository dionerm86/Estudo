<%@ Page Title="Cadastro de Arquivo CalcEngine" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadArquivoCalcEngine.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadArquivoCalcEngine" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        var clicked = false;

        // Validações realizadas ao inserir ou atualizar o arquivo.
        function onInsertUpdate(insert) {
            if (clicked)
                return false;
        
            clicked = true;

            // Recupera os controles.
            var fileUpload = FindControl("fluArquivoCalcEngine", "input");

            if (insert) {
                // Verifica se algum arquivo foi selecionado.
                if (fileUpload == null || fileUpload.files == null || fileUpload.files.length == 0) {
                    alert("Selecione o arquivo do CalcEngine a ser incluído no sistema");
                    clicked = false;
                    return false;
                }
            }

            // Verifica se o arquivo selecionado possui a extensão correta.
            if (fileUpload.files[0].name.toString().toLowerCase().indexOf(".calcpackage") == -1) {
                alert("São permitidos apenas arquivos de extensão .calcpackage.");
                clicked = false;
                return false;
            }

            if (insert)
                FindControl("hdfNome", "input").value = fileUpload.files[0].name.toString().toUpperCase().substring(0,
                    fileUpload.files[0].name.toString().toUpperCase().indexOf(".CALCPACKAGE"));
    
            return true;
        }

        function formataCampoValor() {
            debugger;
            FindControl("txtValorPadraoIns", "input").value = FindControl("txtValorPadraoIns", "input").value.toString().replace(',', '.');
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvArquivoCalcEngine" runat="server" AutoGenerateRows="False" DataKeyNames="IdArquivoCalcEngine"
                    DataSourceID="odsArquivoCalcEngine" DefaultMode="Insert" GridLines="None">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <InsertItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:HiddenField ID="hdfNome" runat="server" Value='<%# Bind("Nome") %>'></asp:HiddenField>
                                        </td>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            Nome: 
                                        </td>
                                        <td>
                                            <asp:Label ID="lblNome" runat="server" Text='<%# Bind("Nome") %>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <InsertItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            Descrição: 
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtDescricao" runat="server" Width="315px"
                                                TextMode="MultiLine" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            Descrição: 
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtDescricao" runat="server" Width="315px"
                                                TextMode="MultiLine" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            Arquivo CalcEngine
                                        </td>
                                        <td>
                                            <asp:FileUpload ID="fluArquivoCalcEngine" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                         <asp:TemplateField>
                            <EditItemTemplate>
                                <table>
                                    <tr>
                                        <td>                                         
                                            <asp:Button ID="btnDownload" runat="server" Text="Baixar Arquivo" onclick="btnDownload_Click"  />                                   
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <table>
                                    <tr>
                                        <td align="center">
                                            <asp:GridView GridLines="None" ID="grdVariaveisCalcEngine" runat="server" AllowPaging="True" AllowSorting="True"
                                                AutoGenerateColumns="False" DataSourceID="odsVariavelCalcEngine" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                                                AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataKeyNames="IdArquivoCalcEngineVar"
                                                EmptyDataText="Nenhuma variável encontrada.">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" CausesValidation="False">
                                                                <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:ImageButton ID="imbAtualizar" runat="server" OnClientClick="formataCampoValor();"
                                                                CommandName="Update" Height="16px"
                                                                ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                                            <asp:ImageButton ID="imbCancelar" runat="server" 
                                                                ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" CausesValidation="False" />
                                                        </EditItemTemplate>
                                                        <HeaderStyle Wrap="False" />
                                                        <ItemStyle Wrap="False" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:HiddenField ID="hdfIdArquivoCalcEngine" runat="server" Value='<%# Bind("IdArquivoCalcEngine") %>'></asp:HiddenField>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:HiddenField ID="hdfIdArquivoCalcEngine" runat="server" Value='<%# Bind("IdArquivoCalcEngine") %>'></asp:HiddenField>
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Var. CalcEngine">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblVariavelCalcEngine" runat="server" Text='<%# Bind("VariavelCalcEngine") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="txtVariavelCalcEngineIns" runat="server" Text='<%# Bind("VariavelCalcEngine") %>'></asp:TextBox>
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Var. Sistema">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblVariavelSistema" runat="server" Text='<%# Bind("VariavelSistema") %>' >
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:DropDownList ID="drpGrupoMedProj" runat="server" DataSourceID="odsGrupoMedProj"
                                                                DataTextField="Descricao" DataValueField="Descricao" AppendDataBoundItems="true"
                                                                SelectedValue='<%# Bind("VariavelSistema") %>' >
                                                                <asp:ListItem></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Valor Padrão">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblValorPadrao" runat="server" Text='<%# Bind("ValorPadrao") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="txtValorPadraoIns" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                Text='<%# Bind("ValorPadrao") %>'></asp:TextBox>
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <PagerStyle />
                                                <EditRowStyle />
                                                <AlternatingRowStyle />
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <InsertItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            Flag Arquivo de Mesa disponível
                                        </td>
                                        <td>
                                            <sync:CheckBoxListDropDown ID="drpFlagArqMesaInsert" runat="server" DataSourceID="odsFlagArqMesa"
                                                DataTextField="Name" DataValueField="Id" SelectedValues='<%# Bind("FlagsArqMesa") %>'>
                                            </sync:CheckBoxListDropDown>
                                        </td>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            Flag Arquivo de Mesa disponível
                                        </td>
                                        <td>
                                            <sync:CheckBoxListDropDown ID="drpFlagArqMesaEdit" runat="server" DataSourceID="odsFlagArqMesa"
                                                DataTextField="Name" DataValueField="Id" SelectedValues='<%# Bind("FlagsArqMesa") %>'>
                                            </sync:CheckBoxListDropDown>
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <br />
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                    OnClientClick="if (!onInsertUpdate(false)) return false;" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False"
                                    Text="Voltar" onclick="btnCancelar_Click" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <br />
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="return onInsertUpdate(true);" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    Text="Voltar" onclick="btnCancelar_Click" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsArquivoCalcEngine" runat="server" DataObjectTypeName="Glass.Data.Model.ArquivoCalcEngine"
                    SelectMethod="ObtemArquivoCalcEngine" TypeName="Glass.Data.DAL.ArquivoCalcEngineDAO"
                    UpdateMethod="Update" InsertMethod="Insert" OnInserted="odsArquivoCalcEngine_Inserted" OnInserting="odsArquivoCalcEngine_Inserting"
                    OnUpdating="odsArquivoCalcEngine_Updating" onupdated="odsArquivoCalcEngine_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="IdArquivoCalcEngine" QueryStringField="IdArquivoCalcEngine" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource ID="odsGrupoMedProj" runat="server" SelectMethod="ObtemOrdenado" TypeName="Glass.Data.DAL.GrupoMedidaProjetoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource ID="odsVariavelCalcEngine" runat="server" SelectMethod="ObtemPeloIdArquivoCalcEngine"
                    UpdateMethod="Update" TypeName="Glass.Data.DAL.ArquivoCalcEngineVariavelDAO"
                    DataObjectTypeName="Glass.Data.Model.ArquivoCalcEngineVariavel" >
                    <SelectParameters>
                        <asp:QueryStringParameter Name="IdArquivoCalcEngine" QueryStringField="IdArquivoCalcEngine" Type="UInt32" />
                        <asp:Parameter Name="buscarAlturaLargura" DefaultValue="false" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:virtualobjectdatasource culture="pt-BR" ID="odsFlagArqMesa" runat="server" 
                    SelectMethod="ObtemFlagsArqMesa" 
                    TypeName="Glass.Projeto.Negocios.IFlagArqMesaFluxo">
                </colo:virtualobjectdatasource>
            </td>
        </tr>
    </table>
</asp:Content>