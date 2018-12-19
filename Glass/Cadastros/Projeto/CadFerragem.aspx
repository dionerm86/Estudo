<%@ Page Title="Cadastro de Ferragem" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadFerragem.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.CadFerragem" %>

<%@ Register Src="~/Controls/ctrlFilhoFerragem.ascx" TagName="ctrlFilhoFerragem" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript">

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center" style="vertical-align:top">
                <asp:DetailsView ID="dtvFerragem" runat="server" AutoGenerateRows="false" GridLines="None" SkinID="defaultDetailsView"
                    DataSourceID="odsFerragem" DataKeyNames="IdFerragem" DefaultMode="Insert" EnableViewState="false">
                    <Fields>
                        <asp:TemplateField HeaderText="Fabricante">
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpFabricante" runat="server" SelectedValue='<%# Bind("IdFabricanteFerragem") %>'
                                    DataSourceID="odsFabricantesFerragem" DataTextField="Name" DataValueField="Id">
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpFabricante" runat="server" SelectedValue='<%# Bind("IdFabricanteFerragem") %>'
                                    DataSourceID="odsFabricantesFerragem" DataTextField="Name" DataValueField="Id">
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Nome Ferragem">
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtNomeFerragem" runat="server" Text='<%# Bind("Nome") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNomeFerragem" runat="server" Text='<%# Bind("Nome") %>'
                                    Enabled='<%# Glass.Data.Helper.UserInfo.GetUserInfo != null && Glass.Data.Helper.UserInfo.GetUserInfo.IsAdminSync %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Calc. Pkg.">
                            <InsertItemTemplate>
                                <asp:FileUpload ID="filCalcPkg" runat="server" />
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <asp:FileUpload ID="filCalcPkg" runat="server" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Imagem">
                            <InsertItemTemplate>
                                <asp:FileUpload ID="filImagem" runat="server" />
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <asp:FileUpload ID="filImagem" runat="server" />
                                <uc2:ctrlImagemPopup ID="ctrlImagemPopup" runat="server" src=".jpg" MaxSize="600px" ImageUrl='<%# Glass.Global.UI.Web.Process.Ferragem.FerragemRepositorioImagens.Instance.ObterUrl((int)Eval("IdFerragem")) %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Medidas Estaticas">
                            <InsertItemTemplate>
                                <asp:CheckBox ID="cbxMedidasEstaticas" runat="server" Checked='<%# Bind("MedidasEstaticas") %>' />
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="cbxMedidasEstaticas" runat="server" Checked='<%# Bind("MedidasEstaticas") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pode Rotacionar">
                            <InsertItemTemplate>
                                <asp:CheckBox ID="cbxPodeRotacionar" runat="server" Checked='<%# Bind("PodeRotacionar") %>' />
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="cbxPodeRotacionar" runat="server" Checked='<%# Bind("PodeRotacionar") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pode Espelhar">
                            <InsertItemTemplate>
                                <asp:CheckBox ID="cbxPodeEspelhar" runat="server" Checked='<%# Bind("PodeEspelhar") %>' />
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="cbxPodeEspelhar" runat="server" Checked='<%# Bind("PodeEspelhar") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Estilo Ancoragem">
                            <InsertItemTemplate>
                                <table id="tabela" runat="server">
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td>
                                            <asp:CheckBox ID="cbxTopo" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:CheckBox ID="cbxTopoEsquerda" runat="server" />
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="cbxTopoCentro" runat="server" />
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="cbxTopoDireita" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="cbxEsquerda" runat="server" />
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="cbxCentroEsquerda" runat="server" />
                                        </td>
                                        <td></td>
                                        <td>
                                            <asp:CheckBox ID="cbxCentroDireita" runat="server" />
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="cbxDireita" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:CheckBox ID="cbxBaseEsquerda" runat="server" />
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="cbxBaseCentro" runat="server" />
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="cbxBaseDireita" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td>
                                            <asp:CheckBox ID="cbxBase" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <table id="tabela" runat="server">
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td>
                                            <asp:CheckBox ID="cbxTopo" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:CheckBox ID="cbxTopoEsquerda" runat="server" />
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="cbxTopoCentro" runat="server" />
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="cbxTopoDireita" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="cbxEsquerda" runat="server" />
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="cbxCentroEsquerda" runat="server" />
                                        </td>
                                        <td></td>
                                        <td>
                                            <asp:CheckBox ID="cbxCentroDireita" runat="server" />
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="cbxDireita" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:CheckBox ID="cbxBaseEsquerda" runat="server" />
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="cbxBaseCentro" runat="server" />
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="cbxBaseDireita" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td>
                                            <asp:CheckBox ID="cbxBase" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Constante da Ferragem">
                            <InsertItemTemplate>
                                <uc1:ctrlFilhoFerragem ID="ctrlConstanteFerragem" runat="server" IdFerragem='<%# Bind("IdFerragem") %>'
                                    PodeAlterar="<%# Glass.Data.Helper.UserInfo.GetUserInfo.IsAdminSync %>" ConstanteFerragem='<%# Bind("Constantes") %>' />
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <uc1:ctrlFilhoFerragem ID="ctrlConstanteFerragem" runat="server" IdFerragem='<%# Bind("IdFerragem") %>'
                                    PodeAlterar="<%# Glass.Data.Helper.UserInfo.GetUserInfo.IsAdminSync %>" ConstanteFerragem='<%# Bind("Constantes") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código da Ferragem">
                            <InsertItemTemplate>
                                <uc1:ctrlFilhoFerragem ID="ctrlCodigoFerragem" runat="server" IdFerragem='<%# Bind("IdFerragem") %>' PodeAlterar="true"  CodigoFerragem='<%# Bind("Codigos") %>' />
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <uc1:ctrlFilhoFerragem ID="ctrlCodigoFerragem" runat="server" IdFerragem='<%# Bind("IdFerragem") %>' PodeAlterar="true" CodigoFerragem='<%# Bind("Codigos") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="false">
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" Text="Inserir" CommandName="Insert" />
                                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" />
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" Text="Atualizar" CommandName="Update" />
                                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" />
                            </EditItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </td>
            <td style="vertical-align:top">
                <table style="vertical-align:baseline">
                    <tr>
                        <td>
                            <div id="divCanvas" style="position: relative; width:500px; height:800px;"
                                align="center">
                                <asp:Image ID="imgFigura" runat="server" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">&nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource ID="odsFerragem" runat="server" Culture="pt-BR"
                    TypeName="Glass.Projeto.Negocios.IFerragemFluxo" DataObjectTypeName="Glass.Projeto.Negocios.Entidades.Ferragem"
                    CreateDataObjectMethod="CriarFerragem" SelectMethod="ObterFerragem"
                    UpdateStrategy="GetAndUpdate" UpdateMethod="SalvarFerragem"
                    InsertMethod="SalvarFerragem"
                    OnInserting="odsFerragem_Inserting" OnUpdating="odsFerragem_Updating"
                    OnInserted="odsFerragem_Inserted" OnUpdated="odsFerragem_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idFerragem" QueryStringField="idFerragem" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource ID="odsFabricantesFerragem" runat="server" Culture="pt-BR"
                    TypeName="Glass.Projeto.Negocios.IFerragemFluxo" SelectMethod="ObterFabricantesFerragem">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsEstiloAncoragem" runat="server"
                    SelectMethod="GetTranslatesFromTypeName"
                    TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" Type="String" DefaultValue="Glass.Data.Model.EstiloAncoragemEnum, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

</asp:Content>
