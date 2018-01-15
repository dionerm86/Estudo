<%@ Page Title="Cadastro de Turno" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadTurno.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadTurno" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=DadosTurno&exportarExcel=" + exportarExcel);
        }

        function onSave() {

            var inicio = FindControl("txtInicio", "input").value;
            var termino = FindControl("txtTermino", "input").value;

            if (!verifica_hora(inicio) || !verifica_hora(termino))
                return false;
            else
                return true;
        }

    </script>

    <section>
        <div>
            <asp:DetailsView ID="dtvTurno" runat="server" CellPadding="4" ForeColor="#333333"
                GridLines="None" AutoGenerateRows="False" DataSourceID="odsTurno" DataKeyNames="IdTurno"
                DefaultMode="Insert">
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <CommandRowStyle BackColor="#D1DDF1" Font-Bold="True" />
                <RowStyle BackColor="White" />
                <FieldHeaderStyle BackColor="#DEE8F5" Font-Bold="True" />
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <Fields>
                    <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDescricao" runat="server" Width="300px" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDescricao"
                                ErrorMessage="Informe a descrição" SetFocusOnError="True" ToolTip="Informe a descrição"
                                ValidationGroup="c">*</asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtDescricao" runat="server" Width="300px" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDescricao"
                                ErrorMessage="Informe a descrição" SetFocusOnError="True" ToolTip="Informe a descrição"
                                ValidationGroup="c">*</asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Turno" SortExpression="NumSeq">
                        <ItemTemplate>
                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("NumSeq") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="odsTurnoSequencia" 
                                DataValueField="Key" DataTextField="Translation" SelectedValue='<%# Bind("NumSeq") %>'
                                AppendDataBoundItems="true">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="DropDownList1"
                                ErrorMessage="Selecione um turno" ToolTip="Selecione um turno" ValidationGroup="c">*</asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="odsTurnoSequencia"
                                DataValueField="Key" DataTextField="Translation" SelectedValue='<%# Bind("NumSeq") %>'
                                AppendDataBoundItems="true">
                                <asp:ListItem Value="">Selecione</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="DropDownList1"
                                ErrorMessage="Selecione um turno" ToolTip="Selecione um turno" ValidationGroup="c">*</asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Início" SortExpression="Inicio">
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("Inicio") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtInicio" Width="50px" OnKeyUp="mascara_hora(event, this)" onkeypress="return soHora(event);"
                                MaxLength="5" runat="server" Text='<%# Bind("Inicio") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtInicio"
                                ErrorMessage="Informe o horário de início" SetFocusOnError="True" ToolTip="Informe o horário de início"
                                ValidationGroup="c">*</asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtInicio" Width="50px" onblur="verifica_hora(this.value);" OnKeyUp="mascara_hora(event, this)"
                                onkeypress="return soHora(event);" MaxLength="5" runat="server" Text='<%# Bind("Inicio") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtInicio"
                                ErrorMessage="Informe o horário de início" SetFocusOnError="True" ToolTip="Informe o horário de início"
                                ValidationGroup="c">*</asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Término" SortExpression="Termino">
                        <ItemTemplate>
                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("Termino") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtTermino" Width="50px" OnKeyUp="mascara_hora(event, this)" onkeypress="return soHora(event);"
                                MaxLength="5" runat="server" Text='<%# Bind("Termino") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtTermino"
                                ErrorMessage="Informe o horário de término" SetFocusOnError="True" ToolTip="Informe o horário de término"
                                ValidationGroup="c">*</asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtTermino" Width="50px" OnKeyUp="mascara_hora(event, this)" onkeypress="return soHora(event);"
                                MaxLength="5" runat="server" Text='<%# Bind("Termino") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtTermino"
                                ErrorMessage="Informe o horário de término" SetFocusOnError="True" ToolTip="Informe o horário de término"
                                ValidationGroup="c">*</asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="False">
                        <EditItemTemplate>
                            <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" OnClientClick="return onSave();"
                                Text="Atualizar" ValidationGroup="c" />
                            <asp:Button ID="btnCancelar" runat="server" CommandName="Cancel" OnClick="btnVoltar_Click"
                                Text="Cancelar" />
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:Button ID="btnInserir" runat="server" CommandName="Insert" OnClientClick="return onSave();"
                                Text="Inserir" ValidationGroup="c" />
                            <asp:Button ID="btnVoltar" runat="server" OnClick="btnVoltar_Click" Text="Voltar" />
                        </InsertItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Wrap="False" />
                    </asp:TemplateField>
                </Fields>
                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <InsertRowStyle BackColor="White" HorizontalAlign="Left" />
                <EditRowStyle BackColor="White" HorizontalAlign="Left" />
                <AlternatingRowStyle BackColor="White" />
            </asp:DetailsView>
            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTurno" runat="server" 
                DataObjectTypeName="Glass.Global.Negocios.Entidades.Turno"
                InsertMethod="SalvarTurno" 
                SelectMethod="ObtemTurno" 
                TypeName="Glass.Global.Negocios.ITurnoFluxo"
                UpdateMethod="SalvarTurno"
                UpdateStrategy="GetAndUpdate">
                <SelectParameters>
                    <asp:QueryStringParameter Name="idTurno" QueryStringField="idTurno" Type="Int32" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource runat="server" ID="odsTurnoSequencia"
                SelectMethod="GetTranslatesFromTypeName"
                TypeName="Colosoft.Translator">
                <SelectParameters>
                    <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.TurnoSequencia, Glass.Data" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
                ShowSummary="False" ValidationGroup="c" />
        </div>
    </section>
   
</asp:Content>
