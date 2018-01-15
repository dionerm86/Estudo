<%@ Page Title="Textos para Orçamento dos Projetos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadTextoOrcamento.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.CadTextoOrcamento" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <table align="center" style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Código" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodigo" runat="server" Width="60px" 
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                ToolTip="Pesquisar" onclick="imgPesq_Click" />
                                    </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao" runat="server" Width="200px" 
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                ToolTip="Pesquisar" onclick="imgPesq_Click" />
                                    </td>
                        <td>
                            <asp:Label ID="Grupo" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupoModelo" runat="server" 
                                DataSourceID="odsGrupoModelo" DataTextField="Descricao" 
                                DataValueField="IdGrupoModelo">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                ToolTip="Pesquisar" onclick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdModelos" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsProjetoModelo" AllowPaging="True" AllowSorting="True"
                    DataKeyNames="IdProjetoModelo"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">

                    <Columns>
                        <asp:TemplateField SortExpression="ModeloPath">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("ModeloPath") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Image ID="imgModelo" runat="server" ImageUrl='<%# Eval("ModeloPath") %>' />
                                <asp:HiddenField ID="hdfIdProjetoModelo" runat="server"
                                    Value='<%# Bind("IdProjetoModelo") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CodDescr" HeaderText="Descrição"
                            SortExpression="CodDescr" />
                        <asp:TemplateField HeaderText="Texto Orçamento" SortExpression="TextoOrcamento">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("TextoOrcamento") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:TextBox ID="txtTextoOrcamento" runat="server" MaxLength="500" Rows="2"
                                    Text='<%# Bind("TextoOrcamento") %>' TextMode="MultiLine" Width="320px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Texto Orcamento (Apenas vidros)"
                            SortExpression="TextoOrcamentoVidro">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server"
                                    Text='<%# Bind("TextoOrcamentoVidro") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:TextBox ID="txtTextoOrcamentoVidros" runat="server" MaxLength="500"
                                    Rows="2" Text='<%# Bind("TextoOrcamentoVidro") %>' TextMode="MultiLine"
                                    Width="320px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="btnAtualizar" runat="server" CommandName="Update"
                                    ImageUrl="~/Images/ok.gif" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>

                    <PagerStyle CssClass="pgr"></PagerStyle>

                    <EditRowStyle CssClass="edit"></EditRowStyle>

                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProjetoModelo" runat="server"
                    DataObjectTypeName="Glass.Data.Model.ProjetoModelo" SelectMethod="GetList"
                    TypeName="Glass.Data.DAL.ProjetoModeloDAO" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" MaximumRowsParameterName="pageSize"
                    UpdateMethod="AtualizaTextoOrcamento" EnablePaging="True"
                    OnUpdated="odsProjetoModelo_Updated" SelectCountMethod="GetCount">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodigo" Name="codigo" PropertyName="Text"  />
                        <asp:ControlParameter ControlID="txtDescricao" Name="descricao" PropertyName="Text"  />
                        <asp:ControlParameter ControlID="drpGrupoModelo" Name="idGrupoModelo"  PropertyName="SelectedValue" />
                        <asp:Parameter Name="situacao" DefaultValue="0" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                            <asp:Label ID="Label11" runat="server" Text="Códigos" 
                    ForeColor="#0066FF"></asp:Label>
                        &nbsp;<br />
                <table>
                    <tr>
                        <td align="left" class="dtvHeader">
                            #m2</td>
                        <td align="left">
                            Área quadrada de cada vão</td>
                    </tr>
                    <tr>
                        <td align="left" class="dtvHeader">
                            #vao_total</td>
                        <td align="left">
                            Área quadrada total</td>
                    </tr>
                    <tr>
                        <td align="left" class="dtvHeader">
                            #qtd</td>
                        <td align="left">
                            Quantidade de fechamentos</td>
                    </tr>
                    <tr>
                        <td align="left" class="dtvHeader">
                            #cor_v</td>
                        <td align="left">
                            Cor das peças de vidro</td>
                    </tr>
                    <tr>
                        <td class="dtvHeader" align="left">
                            #cor_al</td>
                        <td align="left">
                            Cor dos alumínios</td>
                    </tr>
                    <tr>
                        <td align="left" class="dtvHeader">
                            #cor_fr</td>
                        <td align="left">
                            Cor das ferragens</td>
                    </tr>
                    <tr>
                        <td align="left" class="dtvHeader">
                            #num_pc</td>
                        <td align="left">
                            Quantidade total de peças de vidro</td>
                    </tr>
                    <tr>
                        <td align="left" class="dtvHeader">
                            #esp_pc</td>
                        <td align="left">
                            Espessura das peças de vidro</td>
                    </tr>
                    <tr>
                        <td align="left" class="dtvHeader">
                            #qtd_pc_fchto</td>
                        <td align="left">
                            Quantidade de peças por fechamento</td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
        
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoModelo" runat="server" 
        SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.GrupoModeloDAO">
    </colo:VirtualObjectDataSource>
        
            </td>
        </tr>
    </table>
</asp:Content>

