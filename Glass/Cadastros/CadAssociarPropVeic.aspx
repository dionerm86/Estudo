<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadAssociarPropVeic.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadAssociarPropVeic" Title="Associar Proprietário Veículo" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Conteudo" runat="Server">
    <link href="<%= ResolveUrl("~") %>Style/CTe/CadCTe" rel="stylesheet" type="text/css" />
    <div class="pagina">
        <div class="dtvCadAssociarProprietarioVeiculo">
            <asp:DetailsView ID="dtvCadAssociarProprietarioVeiculo" DataKeyNames="IdPropVeic,Placa"
                DataSourceID="odsAssPropVeiculo" runat="server" AutoGenerateRows="False" DefaultMode="Insert"
                GridLines="None" Width="520px">
                <Fields>
                    <asp:TemplateField>
                        <InsertItemTemplate>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label1" runat="server" Text="Proprietario"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpProprietario" runat="server" Height="20px" Width="300px"
                                        AppendDataBoundItems="True" Enabled="true" Visible="true" SelectedValue='<%# Bind("IdPropVeic") %>'
                                        DataSourceID="odsProprietarioVeiculo" DataTextField="Nome" DataValueField="IDPROPVEIC">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label2" runat="server" Text="Veiculo"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpVeiculo" runat="server" Height="20px" Width="100px" AppendDataBoundItems="True"
                                        Enabled="true" Visible="true" SelectedValue='<%# Bind("Placa") %>' DataSourceID="odsVeiculo"
                                        DataTextField="PLACA" DataValueField="PLACA">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </InsertItemTemplate>
                        <EditItemTemplate>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label1" runat="server" Text="Proprietario"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpProprietario" runat="server" Height="20px" Width="300px"
                                        AppendDataBoundItems="True" Enabled="true" Visible="true" SelectedValue='<%# Bind("IdPropVeic") %>'
                                        DataSourceID="odsProprietarioVeiculo" DataTextField="Nome" DataValueField="IDPROPVEIC">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label2" runat="server" Text="Veiculo"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpVeiculo" runat="server" Height="20px" Width="100px" AppendDataBoundItems="True"
                                        Enabled="true" Visible="true" SelectedValue='<%# Bind("Placa") %>' DataSourceID="odsVeiculo"
                                        DataTextField="PLACA" DataValueField="PLACA">
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
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAssPropVeiculo" runat="server" DataObjectTypeName="Glass.Data.Model.CTe.ProprietarioVeiculo_Veiculo"
            SelectMethod="GetElement" TypeName="Glass.Data.DAL.CTe.ProprietarioVeiculo_VeiculoDAO"
            OnInserted="odsAssPropVeiculo_Inserted" OnUpdated="odsAssPropVeiculo_Updated" OnUpdating="odsAssPropVeiculo_Updating"
            InsertMethod="Insert" UpdateMethod="Update">
            <SelectParameters>
                <asp:QueryStringParameter Name="idProprietario" QueryStringField="IdPropVeiculo" Type="UInt32"
                    DefaultValue="" />
                <asp:QueryStringParameter Name="placa" QueryStringField="placa" Type="String"
                    DefaultValue="" />
            </SelectParameters>
        </colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsVeiculo" runat="server" SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.VeiculoDAO">
        </colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProprietarioVeiculo" runat="server" DataObjectTypeName="Glass.Data.Model.CTe.ProprietarioVeiculo"
            SelectMethod="GetList" TypeName="Glass.Data.DAL.CTe.ProprietarioVeiculoDAO" EnablePaging="True"
            MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" SortParameterName="sortExpression"
            StartRowIndexParameterName="startRow">
        </colo:VirtualObjectDataSource>
    </div>
</asp:Content>
