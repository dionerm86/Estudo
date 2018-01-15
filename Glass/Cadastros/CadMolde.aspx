<%@ Page Title="Cadastro de Execução de Molde" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadMolde.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadMolde" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    
    <script type="text/javascript">
        function selPedido()
        {
            openWindow(600, 800, "../Utils/SelPedido.aspx");
        }
        
        function setPedido(idPedido)
        {
            if (idPedido == "")
                return;
            
            FindControl("txtPedido", "input").value = idPedido;
            var dados = CadMolde.GetDadosPedido(idPedido).value.split(';');
            
            if (dados[0] == "Erro")
                alert(dados[1]);
            else
            {
                FindControl("lblNomeCliente", "span").innerHTML = dados[1];
                FindControl("lblTelefoneCliente", "span").innerHTML = dados[2];
                FindControl("lblEnderecoObra", "span").innerHTML = dados[3];
                FindControl("lblBairroObra", "span").innerHTML = dados[4];
                FindControl("lblCidadeObra", "span").innerHTML = dados[5];
            }
        }
        
        function validaPedido(val, args)
        {
            args.IsValid = args.Value != "";
        }
        
        function openRpt()
        {
            var idMolde = '<%= Request["idMolde"] %>';
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Molde&idMolde=" + idMolde);
        }
    </script>
    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvMolde" runat="server" AutoGenerateRows="False" 
                    DataSourceID="odsMolde" GridLines="None" DefaultMode="Insert" 
                    DataKeyNames="IdMolde">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <table cellpadding="2" cellspacing="0">
                                    <tr>
                                        <td id="tituloMolde" align="left" class="dtvHeader">
                                            Molde
                                        </td>
                                        <td id="molde" align="left" style="font-size: medium; padding: 4px">
                                            <asp:Label ID="lblIdMolde" runat="server" Text='<%# Eval("IdMolde") %>'></asp:Label>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            Pedido
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtPedido" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"
                                                onblur="setPedido(this.value)" Text='<%# Bind("IdPedido") %>'></asp:TextBox>
                                            <asp:ImageButton ID="imgPedido" runat="server" 
                                                ImageUrl="~/Images/Pesquisar.gif" 
                                                onclientclick="selPedido(); return false" />
                                            <asp:CustomValidator ID="rfvPedido" runat="server" 
                                                ClientValidationFunction="validaPedido" ControlToValidate="txtPedido" 
                                                Display="Dynamic" ErrorMessage="*" ValidateEmptyText="True"></asp:CustomValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" style="padding-top: 4px; padding-bottom: 4px">
                                            Cliente
                                        </td>
                                        <td align="left" style="padding: 4px">
                                            <asp:Label ID="lblNomeCliente" runat="server" Text='<%# Eval("NomeCliente") %>'></asp:Label>
                                        </td>
                                        <td align="left" class="dtvHeader" style="padding-top: 4px; padding-bottom: 4px">
                                            Telefone Cliente
                                        </td>
                                        <td align="left" style="padding: 4px">
                                            <asp:Label ID="lblTelefoneCliente" runat="server" 
                                                Text='<%# Eval("TelefoneCliente") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" style="padding-top: 4px; padding-bottom: 4px">
                                            Endereço Obra
                                        </td>
                                        <td align="left" colspan="3" style="padding: 4px">
                                            <asp:Label ID="lblEnderecoObra" runat="server" 
                                                Text='<%# Eval("EnderecoObra") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" style="padding-top: 4px; padding-bottom: 4px">
                                            Bairro Obra
                                        </td>
                                        <td align="left" style="padding: 4px">
                                            <asp:Label ID="lblBairroObra" runat="server" Text='<%# Eval("BairroObra") %>'></asp:Label>
                                        </td>
                                        <td align="left" class="dtvHeader" style="padding-top: 4px; padding-bottom: 4px">
                                            Cidade Obra
                                        </td>
                                        <td align="left" style="padding: 4px">
                                            <asp:Label ID="lblCidadeObra" runat="server" Text='<%# Eval("CidadeObra") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" style="padding-top: 4px; padding-bottom: 4px">
                                            Telefone Obra
                                        </td>
                                        <td align="left" style="padding: 4px">
                                            <asp:Label ID="lblTelefoneObra" runat="server" 
                                                Text='<%# Eval("TelefoneObra") %>'></asp:Label>
                                        </td>
                                        <td align="left" class="dtvHeader" style="padding-top: 4px; padding-bottom: 4px">
                                            Celular Obra
                                        </td>
                                        <td align="left" style="padding: 4px">
                                            <asp:Label ID="lblCelularObra" runat="server" Text='<%# Eval("CelularObra") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            Tipo de serviço
                                        </td>
                                        <td align="left" colspan="3">
                                            <asp:CheckBox ID="chkCortar" runat="server" Text="Cortar" 
                                                Checked='<%# Bind("Cortar") %>' />
                                            <asp:CheckBox ID="chkBisotar" runat="server" Text="Bisotar" 
                                                Checked='<%# Bind("Bisotar") %>' />
                                            <asp:CheckBox ID="chkTemperar" runat="server" Text="Temperar" 
                                                Checked='<%# Bind("Temperar") %>' />
                                            <asp:CheckBox ID="chkJatear" runat="server" Text="Jatear" 
                                                Checked='<%# Bind("Jatear") %>' />
                                            <asp:CheckBox ID="chkLaquear" runat="server" Text="Laquear" 
                                                Checked='<%# Bind("Laquear") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            Prazo Entrega
                                        </td>
                                        <td align="left" colspan="3">
                                            <asp:TextBox ID="txtPrazoEntrega" runat="server" Width="100%" MaxLength="200"
                                                Text='<%# Bind("PrazoEntrega") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            Especificações<br />da MDF
                                        </td>
                                        <td align="left" colspan="3">
                                            <asp:TextBox ID="txtMdf" runat="server" TextMode="MultiLine" Width="100%" 
                                                Rows="5" Text='<%# Bind("EspecificacaoMdf") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            Especificações<br />dos Vidros
                                        </td>
                                        <td align="left" colspan="3">
                                            <asp:TextBox ID="txtVidros" runat="server" TextMode="MultiLine" Width="100%" 
                                                Rows="5" Text='<%# Bind("EspecificacaoVidro") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfSituacao" runat="server" 
                                    Value='<%# Bind("Situacao") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <table cellpadding="2" cellspacing="2">
                                    <tr>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Molde
                                        </td>
                                        <td align="left" nowrap="nowrap" style="font-size: medium">
                                            <asp:Label ID="lblIdMolde" runat="server" Text='<%# Eval("IdMolde") %>'></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Cliente
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblCliente" runat="server" Text='<%# Eval("NomeCliente") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Pedido</td>
                                        <td align="left" nowrap="nowrap" style="font-size: medium">
                                            <asp:Label ID="lblIdPedido" runat="server" Text='<%# Eval("IdPedido") %>'></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Telefone cliente
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblTelefoneCliente" runat="server" Text='<%# Eval("TelefoneCliente") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Endereço Obra
                                        </td>
                                        <td align="left" colspan="3">
                                            <asp:Label ID="lblEnderecoObra" runat="server" Text='<%# Eval("EnderecoComplObra") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Tipo de serviço
                                        </td>
                                        <td colspan="3" align="left" nowrap="nowrap">
                                            <asp:Label ID="lblServicos" runat="server" Text='<%# Eval("DescrTipoServico") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Prazo Entrega
                                        </td>
                                        <td colspan="3" align="left" nowrap="nowrap">
                                            <asp:Label ID="lblPrazoEntrega" runat="server" Text='<%# Eval("PrazoEntrega") %>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <br />
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" 
                                    Text="Atualizar" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" 
                                    CommandName="Cancel" Text="Cancelar" onclick="btnCancelar_Click" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <br />
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" 
                                    Text="Inserir" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" 
                                    CommandName="Cancel" Text="Cancelar" onclick="btnCancelar_Click" />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <br />
                                <asp:Button ID="btnEditar" runat="server" CommandName="Edit" Text="Editar" />
                                <asp:Button ID="btnVoltar" runat="server" 
                                    onclientclick="redirectUrl('../Listas/LstMolde.aspx'); return false" 
                                    Text="Voltar" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </td>
        </tr>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMolde" runat="server" 
                    SelectMethod="GetElement" TypeName="Glass.Data.DAL.MoldeDAO" 
                    DataObjectTypeName="Glass.Data.Model.Molde" InsertMethod="Insert" 
                    oninserted="odsMolde_Inserted" onupdated="odsMolde_Updated" 
                    UpdateMethod="Update">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idMolde" QueryStringField="idMolde" 
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
    </table>
    <script type="text/javascript">
        if (<%= (dtvMolde.CurrentMode == DetailsViewMode.Insert).ToString().ToLower() %>)
        {
            document.getElementById("tituloMolde").style.display = "none";
            document.getElementById("molde").style.display = "none";
        }
        
        if (FindControl("hdfAba", "input") != null)
            mudaAba(FindControl("hdfAba", "input").value, false);
        
        if (<%= (Request["relatorio"] != null).ToString().ToLower() %>)
        {
            openRpt();
            redirectUrl("../Listas/LstMolde.aspx");
        }
    </script>
</asp:Content>

