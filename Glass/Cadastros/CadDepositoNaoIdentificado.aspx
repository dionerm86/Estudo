<%@ Page Title="Cadastro de Deposito Não Identificado" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadDepositoNaoIdentificado.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadDepositoNaoIdentificado" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function onInsertUpdate() {
            var idContaBanco = FindControl("drpContaBanco", "select").value;
            var valorMov = FindControl("txtValorMov", "input").value;
            var dataMov = FindControl("ctrlDataMov_txtData", "input").value;

            if (idContaBanco == 0) {
                alert("Informe a conta bancaria.");
                return false;
            }

            if (valorMov == "") {
                alert("Informe o valor do deposito.");
                return false;
            }

            if (dataMov == "") {
                alert("Informe a data do deposito.");
                return false;
            }

            return true;
        }

    </script>

    <table style="width: 100%;">
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvDepositoNaoIdentificado" runat="server" AutoGenerateRows="False"
                    DataSourceID="odsDepositoNaoIdentificado" DefaultMode="Insert" GridLines="None"
                    DataKeyNames="IdDepositoNaoIdentificado">
                    <Fields>
                        <asp:TemplateField>
                            <InsertItemTemplate>
                                <table align="left" cellpadding="2" cellspacing="0" style="width: 100%">
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="lblContaBancaria" runat="server" Text="Conta Bancária"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" colspan="3" nowrap="nowrap">
                                            <asp:DropDownList ID="drpContaBanco" runat="server" AppendDataBoundItems="True" 
                                                DataSourceID="odsContaBanco" DataTextField="Descricao" DataValueField="IdContaBanco"
                                                SelectedValue='<%# Bind("IdContaBanco") %>'>
                                                <asp:ListItem Value="0" Text="" Selected="True" />
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="lblValorMov" runat="server" Text="Valor da Mov."></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtValorMov" runat="server" MaxLength="25" TabIndex="1" Text='<%# Bind("ValorMovString") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="lblDataMov" runat="server" Text="Data da Mov."></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <uc1:ctrlData ID="ctrlDataMov" runat="server" ReadOnly="ReadWrite" DataString='<%# Bind("DataMovString") %>'
                                                ExibirHoras="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label1" runat="server" Text="Obs."></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtObs" runat="server" Height="60px" TextMode="MultiLine" Text='<%# Bind("Obs") %>'
                                                Width="400px" />
                                        </td>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <table align="left" cellpadding="2" cellspacing="0" style="width: 100%">
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="lblContaBancaria" runat="server" Text="Conta Bancária"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" colspan="3" nowrap="nowrap">
                                            <asp:DropDownList ID="drpContaBanco" runat="server" AppendDataBoundItems="True"
                                                DataSourceID="odsContaBanco" DataTextField="Descricao" DataValueField="IdContaBanco"
                                                SelectedValue='<%# Bind("IdContaBanco") %>' Enabled="false">
                                                <asp:ListItem Value="0" Text="" Selected="True" />
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="lblValorMov" runat="server" Text="Valor da Mov."></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtValorMov" runat="server" MaxLength="25" TabIndex="1" Text='<%# Bind("ValorMovString") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="lblDataMov" runat="server" Text="Data da Mov."></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <uc1:ctrlData ID="ctrlDataMov" runat="server" ReadOnly="ReadWrite" DataString='<%# Bind("DataMovString") %>'
                                                ExibirHoras="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label1" runat="server" Text="Obs."></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtObs" runat="server" Height="60px" TextMode="MultiLine" Text='<%# Bind("Obs") %>'
                                                Width="400px" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField runat="server" ID="hdfSituacao" Value='<%# Bind("Situacao") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <InsertItemTemplate>
                                <br />
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="return onInsertUpdate();" />
                                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" />
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <br />
                                <asp:Button ID="btnAlterar" runat="server" CommandName="Update" Text="Atualizar" OnClientClick="return onInsertUpdate();" />
                                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" />
                            </EditItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsDepositoNaoIdentificado" runat="server" DataObjectTypeName="Glass.Data.Model.DepositoNaoIdentificado"
        InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.DepositoNaoIdentificadoDAO"
        UpdateMethod="Update" OnInserted="odsDepositoNaoIdentificado_Inserted" OnUpdated="odsDepositoNaoIdentificado_Updated">
        <SelectParameters>
            <asp:QueryStringParameter Name="idDepositoNaoIdentificado" QueryStringField="idDepositoNaoIdentificado"
                Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
        TypeName="Glass.Data.DAL.ContaBancoDAO">
    </colo:VirtualObjectDataSource>
</asp:Content>
