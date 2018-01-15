<%@ Page Title="Cancelamento de MDFe" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstManifestoEletronicoCancelamento.aspx.cs" Inherits="Glass.UI.Web.Listas.LstManifestoEletronicoCancelamento" %>

<%@ Register Src="~/Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript">

        function openMotivoCancelamento(idMDFe) {
            var altura = 300;
            var largura = 550;
            var scrY = (screen.height - altura) / 2;
            var scrX = (screen.width - largura) / 2;
            var momentoAtual = new Date();

            var win = window.open("../Utils/SetMotivoCancMDFe.aspx?IdMDFe=" + idMDFe, "popup" + momentoAtual.getSeconds(), 'width=' + largura + ',height=' + altura + ',left=' + scrX + ',top=' + scrY);

            return false;
        }

    </script>
    
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblNumeroMDFe" runat="server" Text="Num. MDFe" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumeroMDFe" runat="server" MaxLength="9" Width="60px"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblSituacao" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdSituacao" runat="server" CheckAll="False" Title="Todas">
                                <asp:ListItem Value="2">Autorizado</asp:ListItem>
                                <asp:ListItem Value="6">Processo de cancelamento</asp:ListItem>
                                <asp:ListItem Value="9">Falha ao cancelar</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblUFInicio" runat="server" Text="UF Início" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpUfInicio" runat="server" AppendDataBoundItems="true" DataSourceID="odsUFPercurso"
                                DataTextField="NomeUf" DataValueField="NomeUf">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblUFFim" runat="server" Text="UF Fim" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpUfFim" runat="server" AppendDataBoundItems="true" DataSourceID="odsUFPercurso"
                                DataTextField="NomeUf" DataValueField="NomeUf">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblPerEmissao" runat="server" Text="Período Emissão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdManifestoEletronico" runat="server" GridLines="None" AllowPaging="true" AllowSorting="true" AutoGenerateColumns="false" CssClass="gridStyle"
                    DataSourceID="odsManifestoEletronico" DataKeyNames="IdManifestoEletronico"
                    OnRowDataBound="grdManifestoEletronico_RowDataBound">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkCancelar" runat="server" Text="Cancelar" Visible='<%# Eval("CancelarVisible") %>'
                                    OnClientClick='<%# "openMotivoCancelamento(" + Eval("IdManifestoEletronico") + "); return false;" %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="NumeroManifestoEletronico" HeaderText="Num." SortExpression="NumeroManifestoEletronico" />
                        <asp:BoundField DataField="Modelo" HeaderText="Modelo" SortExpression="Modelo" />
                        <asp:BoundField DataField="UFInicio" HeaderText="UF Início" SortExpression="UFInicio" />
                        <asp:BoundField DataField="UFFim" HeaderText="UF Fim" SortExpression="UFFim" />
                        <asp:BoundField DataField="ValorCarga" HeaderText="Valor Tot." SortExpression="ValorCarga" />
                        <asp:BoundField DataField="DataEmissao" HeaderText="Data Emissão" SortExpression="DataEmissao" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="Emitente" HeaderText="Emitente"  />
                        <asp:BoundField DataField="Contratante" HeaderText="Contratante" ItemStyle-Wrap="true" />
                        <asp:TemplateField HeaderText="Situação">
                            <ItemTemplate>
                                <asp:Label ID="lblSituacao" runat="server" Text='<%# Eval("SituacaoString") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                    <EditRowStyle CssClass="edit" />
                    <PagerStyle CssClass="pgr" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource ID="odsManifestoEletronico" runat="server" Culture="pt-BR"
                    EnablePaging="true" EnableViewState="false" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" MaximumRowsParameterName="pageSize"
                    TypeName="Glass.Data.DAL.ManifestoEletronicoDAO" DataObjectTypeName="Glass.Data.Model.ManifestoEletronico"
                    SelectMethod="GetList" SelectCountMethod="GetCount">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumeroMDFe" Name="numeroManifestoEletronico" PropertyName="Text" />
                        <asp:ControlParameter ControlID="cbdSituacao" Name="situacoes" PropertyName="SelectedValue" DefaultValue="2,6,9" />
                        <asp:ControlParameter ControlID="drpUfInicio" Name="uFInicio" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="drpUfFim" Name="uFFim" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataEmissaoIni" PropertyName="DataString" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataEmissaoFim" PropertyName="DataString" />
                        <asp:Parameter Name="idLoja" DefaultValue="0" />
                        <asp:Parameter Name="tipoContratante" DefaultValue="0" />
                        <asp:Parameter Name="idContratante" DefaultValue="0" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:ObjectDataSource ID="odsUFPercurso" runat="server"
                    TypeName="Glass.Data.DAL.CidadeDAO" DataObjectTypeName="Glass.Data.Model.Cidade"
                    SelectMethod="ObterUF">
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>

</asp:Content>
