<%@ Page Title="Cadastrar Administradora de Cartão" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadAdminCartao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadAdminCartao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <script type="text/javascript">
        function setCidade(idCidade, nomeCidade)
        {
            FindControl('hdfCidade', 'input').value = idCidade;
            FindControl('txtCidade', 'input').value = nomeCidade;
        }
    </script>
    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvAdminCartao" runat="server" AutoGenerateRows="False" 
                    DataKeyNames="IdAdminCartao" DataSourceID="odsAdminCartao" DefaultMode="Insert" 
                    GridLines="None">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <table id="tbAdminCartao">
                                    <tr>
                                        <td class="dtvHeader">
                                            Cód.
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("IdAdminCartao") %>'></asp:Label>
                                        </td>
                                        <td class="dtvHeader">
                                            Nome
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtNome" runat="server" Text='<%# Bind("Nome") %>' Width="200px" MaxLength="30"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            CNPJ
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtCnpj" runat="server" MaxLength="18" Text='<%# Bind("Cnpj") %>'
                                                Width="150px" onkeypress="maskCNPJ(event, this)"></asp:TextBox>
                                            <asp:CustomValidator ID="valCnpj" runat="server" ClientValidationFunction="validarCnpj"
                                                ControlToValidate="txtCnpj" ErrorMessage="CNPJ Inválido" Display="Dynamic"></asp:CustomValidator>
                                        </td>
                                        <td class="dtvHeader">
                                            Inscr. Estadual
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtInscrEst" runat="server" MaxLength="14" 
                                                Text='<%# Bind("InscrEst") %>' Width="120px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Endereço
                                        </td>
                                        <td align="left" nowrap="nowrap" colspan="3">
                                            <asp:TextBox ID="txtEndereco" runat="server" MaxLength="100" Text='<%# Bind("Endereco") %>'
                                                Width="230px"></asp:TextBox>
                                            N.º
                                            <asp:TextBox ID="txtNum" runat="server" Width="50px" 
                                                Text='<%# Bind("Numero") %>'></asp:TextBox>
                                            Complemento
                                            <asp:TextBox ID="txtCompl" runat="server" MaxLength="30" Text='<%# Bind("Compl") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Bairro
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtBairro" runat="server" MaxLength="30" Text='<%# Bind("Bairro") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                        <td class="dtvHeader">
                                            Cidade
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtCidade" runat="server" MaxLength="50" Text='<%# Eval("Cidade") %>'
                                                Width="200px" ReadOnly="True"></asp:TextBox>
                                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx'); return false;" />
                                            <asp:HiddenField ID="hdfCidade" runat="server" Value='<%# Bind("IdCidade") %>' />
                                        </td>
                                    </tr>
                                </table>
                                <script type="text/javascript">
                                    var editando = <%= (!String.IsNullOrEmpty(Request["idAdminCartao"])).ToString().ToLower() %>;
                                    if (!editando)
                                    {
                                        var tabela = document.getElementById("tbAdminCartao");
                                        tabela.rows[0].cells[0].style.display = "none";
                                        tabela.rows[0].cells[1].style.display = "none";
                                    }
                                </script>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" 
                                    Text="Atualizar" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" 
                                    CommandName="Cancel" onclick="btnCancelar_Click" Text="Cancelar" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" 
                                    Text="Inserir" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" 
                                    CommandName="Cancel" onclick="btnCancelar_Click" Text="Cancelar" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAdminCartao" runat="server" 
                    DataObjectTypeName="Glass.Data.Model.AdministradoraCartao" 
                    InsertMethod="Insert" SelectMethod="GetElement" 
                    TypeName="Glass.Data.DAL.AdministradoraCartaoDAO" UpdateMethod="Update" 
                    OnInserted="odsAdminCartao_Inserted" OnUpdated="odsAdminCartao_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idAdminCartao" QueryStringField="idAdminCartao" 
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

