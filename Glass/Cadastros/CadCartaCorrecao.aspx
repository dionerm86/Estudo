<%@ Page Title="Criação de Carta de Correção para NF-e" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadCartaCorrecao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCartaCorrecao" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrllogpopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        //Utilizado no CustomValidator
        function TextoValidator(source, args) {
            if (args.Value == "")
                args.IsValid = false;
        }

        function CheckLength() {
            var texto = FindControl("TextBox1", "textarea");

            if (texto.value.length < 15 || texto.value.length > 1000) {
                alert("O texto a ser enviado deve estar entre 15 e 1000 caracteres para ser aceito.");
                return;
            }
        }

        function openRpt(idCarta, exportarExcel) {
            
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=RetCartaCorrecao&idCarta=" + idCarta + "&exportarExcel=" + exportarExcel);

            return false;

        }
 
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center" style="height: 220px">
                <asp:DetailsView ID="dtvNf" runat="server" AutoGenerateRows="False" DefaultMode="Insert"
                    GridLines="None" DataSourceID="odsCarta" OnItemInserting="dtvNf_ItemInserting"
                    OnItemInserted="dtvNf_ItemInserted" DataKeyNames="IdCarta" OnItemUpdated="dtvNf_ItemUpdated">
                    <Fields>
                        <asp:TemplateField HeaderText="Correção" SortExpression="Correcao" ShowHeader="False">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Correcao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <table id="tblDadosCadastrais" width="100%">
                                                <tr>
                                                    <td colspan="2" align="center" bgcolor="#D2D2D2">
                                                        <asp:Label ID="Label20" runat="server" Text="Correção" Font-Bold="True"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="width: 380px">
                                                        <asp:TextBox ID="TextBox1" runat="server" Height="135px" onblur="CheckLength();" Text='<%# Bind("Correcao") %>'
                                                            TextMode="MultiLine" Width="500px"></asp:TextBox><br />
                                                        <asp:CustomValidator ID="correcaoValidator" runat="server" ControlToValidate="TextBox1"
                                                            ClientValidationFunction="TextoValidator" Display="Dynamic" ErrorMessage="Texto informado é inválido."
                                                            OnServerValidate="textoValidator_ServerValidate" SetFocusOnError="True" ValidateEmptyText="True"
                                                            ValidationGroup="c">O texto a ser enviado deve estar entre 15 e 1000 caracteres para ser aceito.</asp:CustomValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <table>
                                    <tr>
                                        <td align="center" bgcolor="#D2D2D2">
                                            <asp:Label ID="Label20" runat="server" Text="Correção" Font-Bold="True"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table id="tblDadosCadastrais" width="100%">
                                                <tr>
                                                    <td align="left"  nowrap="nowrap" style="width: 380px">
                                                        <asp:TextBox ID="TextBox1" runat="server" Height="135px" onblur="CheckLength();" Text='<%# Bind("Correcao") %>'
                                                            TextMode="MultiLine" Width="500px"></asp:TextBox><br />
                                                        <asp:CustomValidator ID="correcaoValidator" runat="server" ControlToValidate="TextBox1"
                                                            ClientValidationFunction="TextoValidator" Display="Dynamic" ErrorMessage="Texto informado é inválido."
                                                            OnServerValidate="textoValidator_ServerValidate" SetFocusOnError="True" ValidateEmptyText="True"
                                                            ValidationGroup="c">O texto a ser enviado deve estar entre 15 e 1000 caracteres para ser aceito.</asp:CustomValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdNf" HeaderText="IdNf" SortExpression="IdNf" 
                            Visible="False" ShowHeader="False" />
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:Button ID="Button1" runat="server" CausesValidation="False" CommandName="Edit"
                                    Text="Editar" />
                                &nbsp;<asp:Button ID="Button2" runat="server" CausesValidation="False" CommandName="New"
                                    Text="Novo" />
                                &nbsp;<asp:Button ID="Button3" runat="server" CausesValidation="False" CommandName="Delete"
                                    Text="Excluir" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Button ID="Button1" runat="server" CausesValidation="True" CommandName="Update"
                                    Text="Atualizar" ValidationGroup="c" />
                                &nbsp;<asp:Button ID="Button2" runat="server" CausesValidation="False" CommandName="Cancel"
                                    OnClientClick="closeWindow();" Text="Cancelar" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="Button1" runat="server" CausesValidation="True" CommandName="Insert"
                                    Text="Inserir" ValidationGroup="c" />
                                &nbsp;<asp:Button ID="Button2" runat="server" CausesValidation="False" CommandName="Cancel"
                                    OnClientClick="closeWindow();" Text="Cancelar" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCarta" runat="server" DataObjectTypeName="Glass.Data.Model.CartaCorrecao"
                    DeleteMethod="Delete" InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.CartaCorrecaoDAO"
                    UpdateMethod="Update">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="grdCarta" Name="idCarta" PropertyName="SelectedValue"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <p>
                    § 1º-A Fica permitida a utilização de carta de correção, para regularização de erro
                    ocorrido na emissão de documento fiscal, desde que o erro não esteja relacionado
                    com:</p>
                <b>I</b> - as variáveis que determinam o valor do imposto tais como: base de cálculo,
                alíquota, diferença de preço, quantidade, valor da operação ou da prestação;
                <br />
                <b>II</b> - a correção de dados cadastrais que implique mudança do remetente ou
                do destinatário;
                <br />
                <b>III</b> - a data de emissão ou de saída.”
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdCarta" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsListaCarta" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    EmptyDataText="Nenhum registro encontrado." DataKeyNames="IdCarta"
                    OnRowCommand="grdCarta_RowCommand" 
                    OnSelectedIndexChanged="grdCarta_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbEditar" runat="server" CommandName="Select" ImageUrl="~/Images/EditarGrid.gif"
                                    ToolTip="Editar" Visible='<%# Eval("EditarExcluirVisible") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este registro?&quot;);"
                                    Visible='<%# Eval("EditarExcluirVisible") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbEnvia" runat="server" ImageUrl="~/Images/carta_envia.gif"
                                    ToolTip="Enviar carta de correção." CommandName="EnviarCarta" CommandArgument='<%# Eval("IdCarta") %>'
                                    OnClientClick="return confirm(&quot;Tem certeza que deseja enviar esta correção?&quot;);"
                                    Visible='<%# Eval("EnviarVisible") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                                          <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbImprime" runat="server" ImageUrl="~/Images/printer.png"
                                    ToolTip="Imprimir carta de correção." OnClientClick='<%# "openRpt(" + Eval("IdCarta") + ", false); return false" %>'  CommandName="Imprimir" CommandArgument='<%# Eval("IdCarta") %>'
                                    Visible='<%# Eval("Imprimir")%>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Correcao" HeaderText="Correção" SortExpression="Correcao" />
                        <asp:BoundField DataField="DataCadastro" DataFormatString="{0:d}" HeaderText="Data Cadastro"
                            SortExpression="DataCadastro">
                            <HeaderStyle HorizontalAlign="Right" />
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" 
                            SortExpression="DescrSituacao" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsListaCarta" runat="server" SelectMethod="GetList" TypeName="Glass.Data.DAL.CartaCorrecaoDAO"
                    DataObjectTypeName="Glass.Data.Model.CartaCorrecao" DeleteMethod="Delete">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
