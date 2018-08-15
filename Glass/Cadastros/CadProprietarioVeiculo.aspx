<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadProprietarioVeiculo.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadProprietarioVeiculo" Title="Cadastro Proprietário" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <link href="<%= ResolveUrl("~") %>Style/CTe/CadCTe" rel="stylesheet" type="text/css" />
    <div class="pagina">
        <div class="dtvCadProprietario">
            <asp:DetailsView ID="dtvCadProprietario" DataKeyNames="IdPropVeic" DataSourceID="odsPropVeiculo"
                runat="server" AutoGenerateRows="False" DefaultMode="Insert" GridLines="None"
                Width="520px">
                <Fields>
                    <asp:TemplateField>
                        <InsertItemTemplate>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label54" runat="server" Text="Nome"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtNomeProprietario" runat="server" MaxLength="50" Width="300px"
                                        Text='<%# Bind("Nome") %>'></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label17" runat="server" Text="Cpf"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtCpf" CssClass="cpf" runat="server" MaxLength="50" Width="120px"
                                        Text='<%# Bind("Cpf") %>' onkeypress="maskCPF(event, this)"></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label18" runat="server" Text="Cnpj"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtCnpj" CssClass="cnpj" runat="server" MaxLength="50" Width="120px"
                                        Text='<%# Bind("Cnpj") %>' onkeypress="maskCNPJ(event, this)"></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label36" runat="server" Text="RNTRC"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtRntrc" runat="server" MaxLength="50" Width="200px" Text='<%# Bind("RNTRC") %>'></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label69" runat="server" Text="Inscrição Estadual"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtInscricaoEstadual" runat="server" MaxLength="50" Width="200px"
                                        Text='<%# Bind("IE") %>'></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label1" runat="server" Text="Uf"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpCFOP" runat="server" Height="20px" Width="45px" AppendDataBoundItems="True"
                                        Enabled="true" Visible="true" SelectedValue='<%# Bind("UF") %>'>
                                        <asp:ListItem>AC</asp:ListItem>
                                        <asp:ListItem>AL</asp:ListItem>
                                        <asp:ListItem>AM</asp:ListItem>
                                        <asp:ListItem>AP</asp:ListItem>
                                        <asp:ListItem>BA</asp:ListItem>
                                        <asp:ListItem>CE</asp:ListItem>
                                        <asp:ListItem>DF</asp:ListItem>
                                        <asp:ListItem>ES</asp:ListItem>
                                        <asp:ListItem>GO</asp:ListItem>
                                        <asp:ListItem>MA</asp:ListItem>
                                        <asp:ListItem>MG</asp:ListItem>
                                        <asp:ListItem>MS</asp:ListItem>
                                        <asp:ListItem>MT</asp:ListItem>
                                        <asp:ListItem>PB</asp:ListItem>
                                        <asp:ListItem>PA</asp:ListItem>
                                        <asp:ListItem>PE</asp:ListItem>
                                        <asp:ListItem>PI</asp:ListItem>
                                        <asp:ListItem>PR</asp:ListItem>
                                        <asp:ListItem>RJ</asp:ListItem>
                                        <asp:ListItem>RN</asp:ListItem>
                                        <asp:ListItem>RO</asp:ListItem>
                                        <asp:ListItem>RR</asp:ListItem>
                                        <asp:ListItem>RS</asp:ListItem>
                                        <asp:ListItem>SC</asp:ListItem>
                                        <asp:ListItem>SP</asp:ListItem>
                                        <asp:ListItem>SE</asp:ListItem>
                                        <asp:ListItem>TO</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label2" runat="server" Text="Tipo Proprietário"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="DropDownList1" runat="server" Height="20px" Width="80px" AppendDataBoundItems="True"
                                        Enabled="true" Visible="true" SelectedValue='<%# Bind("TipoProp") %>'>
                                        <asp:ListItem Text="Próprio" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Terceiro" Value="2"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </InsertItemTemplate>
                        <EditItemTemplate>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label54" runat="server" Text="Nome"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtNomeProprietario" runat="server" MaxLength="50" Width="300px"
                                        Text='<%# Bind("Nome") %>'></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label17" runat="server" Text="Cpf"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtCpf" CssClass="cpf" runat="server" MaxLength="50" Width="120px"
                                        onkeypress="maskCPF(event, this)" Text='<%# Bind("Cpf") %>'></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label18" runat="server" Text="Cnpj"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtCnpj" CssClass="cnpj" runat="server" MaxLength="50" Width="120px"
                                        onkeypress="maskCNPJ(event, this)" Text='<%# Bind("Cnpj") %>'></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label36" runat="server" Text="RNTRC"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtRntrc" runat="server" MaxLength="50" Width="200px" Text='<%# Bind("RNTRC") %>'></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label69" runat="server" Text="Inscrição Estadual"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtInscricaoEstadual" runat="server" MaxLength="50" Width="200px"
                                        Text='<%# Bind("IE") %>'></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label1" runat="server" Text="Uf"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpCFOP" runat="server" Height="20px" Width="45px" AppendDataBoundItems="True"
                                        Enabled="true" Visible="true" SelectedValue='<%# Bind("UF") %>'>
                                        <asp:ListItem>AC</asp:ListItem>
                                        <asp:ListItem>AL</asp:ListItem>
                                        <asp:ListItem>AM</asp:ListItem>
                                        <asp:ListItem>AP</asp:ListItem>
                                        <asp:ListItem>BA</asp:ListItem>
                                        <asp:ListItem>CE</asp:ListItem>
                                        <asp:ListItem>DF</asp:ListItem>
                                        <asp:ListItem>ES</asp:ListItem>
                                        <asp:ListItem>GO</asp:ListItem>
                                        <asp:ListItem>MA</asp:ListItem>
                                        <asp:ListItem>MG</asp:ListItem>
                                        <asp:ListItem>MS</asp:ListItem>
                                        <asp:ListItem>MT</asp:ListItem>
                                        <asp:ListItem>PB</asp:ListItem>
                                        <asp:ListItem>PA</asp:ListItem>
                                        <asp:ListItem>PE</asp:ListItem>
                                        <asp:ListItem>PI</asp:ListItem>
                                        <asp:ListItem>PR</asp:ListItem>
                                        <asp:ListItem>RJ</asp:ListItem>
                                        <asp:ListItem>RN</asp:ListItem>
                                        <asp:ListItem>RO</asp:ListItem>
                                        <asp:ListItem>RR</asp:ListItem>
                                        <asp:ListItem>RS</asp:ListItem>
                                        <asp:ListItem>SC</asp:ListItem>
                                        <asp:ListItem>SP</asp:ListItem>
                                        <asp:ListItem>SE</asp:ListItem>
                                        <asp:ListItem>TO</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label2" runat="server" Text="Tipo Proprietário"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="DropDownList1" runat="server" Height="20px" Width="80px" AppendDataBoundItems="True"
                                        Enabled="true" Visible="true" SelectedValue='<%# Bind("TipoProp") %>'>
                                        <asp:ListItem Text="Próprio" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Terceiro" Value="2"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <EditItemTemplate>
                            <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                OnClientClick="if (!onUpdate()) return false;" ValidationGroup="c" />
                            <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar"
                                CausesValidation="false" />
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="if (!onInsert()) return false;"
                                ValidationGroup="c" />
                            <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" OnClick="btnCancelar_Click"
                                Text="Cancelar" />
                        </InsertItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
        </div>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPropVeiculo" runat="server" DataObjectTypeName="Glass.Data.Model.CTe.ProprietarioVeiculo"
            SelectMethod="GetElement" TypeName="Glass.Data.DAL.CTe.ProprietarioVeiculoDAO"
            OnInserted="odsProprietarioVeiculo_Inserted" OnUpdated="odsProprietarioVeiculo_Updated"
            InsertMethod="Insert" UpdateMethod="Update" OnInserting="odsPropVeiculo_Inserting" OnUpdating="odsPropVeiculo_Updating">
            <SelectParameters>
                <asp:QueryStringParameter Name="idProprietario" QueryStringField="idPropVeiculo" Type="UInt32" DefaultValue="" />
            </SelectParameters>
        </colo:VirtualObjectDataSource>
    </div>
</asp:Content>
