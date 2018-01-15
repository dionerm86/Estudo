<%@ Page Title="Lista de MDF-e" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstManifestoEletronico.aspx.cs" Inherits="Glass.UI.Web.Listas.LstManifestoEletronico" %>

<%@ Register Src="~/Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript">

        function openRptDAMDFE(idMDFe) {
            openWindow(600, 800, "../Relatorios/MDFe/RelBase.aspx?rel=Damdfe&IdMDFe=" + idMDFe);
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
                            <sync:CheckBoxListDropDown ID="cbdSituacao" runat="server" DataSourceID="odsSituacao"
                                DataTextField="Translation" DataValueField="Value">
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
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblEmitente" runat="server" Text="Emitente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoEmitente" runat="server" AutoPostBack="true" Enabled="false">
                                <asp:ListItem Value="0" Selected="True">Loja</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpEmitente" runat="server" AppendDataBoundItems="true"
                                DataSourceID="odsLoja" DataValueField="IdLoja" DataTextField="RazaoSocial">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblContratante" runat="server" Text="Contratante" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoContratante" runat="server" AutoPostBack="true"
                                OnSelectedIndexChanged="drpTipoContratante_SelectedIndexChanged">
                                <asp:ListItem Value="0" Selected="True">Loja</asp:ListItem>
                                <asp:ListItem Value="1">Fornecedor</asp:ListItem>
                                <asp:ListItem Value="2">Cliente</asp:ListItem>
                                <asp:ListItem Value="3">Transportador</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContratante" runat="server" AppendDataBoundItems="true">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
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
                    DataSourceID="odsManifestoEletronico" DataKeyNames="IdManifestoEletronico" OnRowCommand="grdManifestoEletronico_RowCommand"
                    OnRowDataBound="grdManifestoEletronico_RowDataBound" EmptyDataText="Nenhum MDF-e Cadastrado">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" ImageUrl="~/Images/EditarGrid.gif" ToolTip="Editar" Visible='<%# Eval("EditarVisible") %>'
                                     NavigateUrl='<%# "~/Cadastros/CadManifestoEletronico.aspx?IdMDFe=" + Eval("IdManifestoEletronico") %>'></asp:HyperLink>
                                <asp:ImageButton ID="imbLogMDFe" runat="server" ImageUrl="~/Images/blocodenotas.png" ToolTip="Log de eventos" Visible="true"
                                    OnClientClick='<%# "openWindow(450, 700, \"../Utils/ShowLogMDFe.aspx?IdManifestoEletronico=" + Eval("IdManifestoEletronico") +
                                        "&Situacao=" + Eval("Situacao") + "&Numero=" + Eval("NumeroManifestoEletronico") + "\"); return false" %>' />
                                <asp:ImageButton ID="imbImprimir" runat="server" ImageUrl="~/Images/Relatorio.gif" ToolTip="Imprimir" Visible='<%# Eval("ImprimirDAMDFEVisible") %>'
                                    OnClientClick='<%# "openRptDAMDFE(" + Eval("IdManifestoEletronico") + "); return false;" %>' />
                                <asp:ImageButton ID="imbConsultaSitLoteMDFe" runat="server" ImageUrl="~/Images/ConsSitNFe.gif" ToolTip="Consulta Situação Lote MDFe"
                                    Visible='<%# Eval("ConsultarSituacaoVisible") %>'
                                    CommandName="ConsultaSitLoteMDFe" CommandArgument='<%# Eval("IdManifestoEletronico") %>' />
                                <asp:ImageButton ID="imbConsultaSituacaoMDFe" runat="server" ImageUrl="~/Images/ConsSitNFe.gif" ToolTip="Consulta Situação MDFe"
                                    Visible='<%# Eval("ConsultarSituacaoVisible") %>'
                                    CommandName="ConsultaSitMDFe" CommandArgument='<%# Eval("IdManifestoEletronico") %>' />
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
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEmitirMDFeOffline" runat="server" ImageUrl="~/Images/arrow_right.gif"
                                    ToolTip="Emitir MDF-e" Visible='<%# Eval("EmitirMDFeOfflineVisible") %>' CommandArgument='<%# Eval("IdManifestoEletronico") %>'
                                    CommandName="EmitirMDFeOffline"></asp:ImageButton>
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
            <td align="center">
                <asp:Label ID="lblConsultaNaoEncerrados" runat="server" Text="Consulta MDFe não Encerrados"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:DropDownList ID="drpLojaConsultaNaoEncerrados" runat="server" AppendDataBoundItems="true"
                    DataSourceID="odsLoja" DataValueField="IdLoja" DataTextField="NomeFantasia">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <asp:LinkButton ID="lnkConsultaNaoEncerrados" runat="server" Text="Consultar Não Encerrados"
                    OnClick="lnkConsultaNaoEncerrados_Click"></asp:LinkButton>
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
                        <asp:ControlParameter ControlID="cbdSituacao" Name="situacoes" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="drpUfInicio" Name="uFInicio" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="drpUfFim" Name="uFFim" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataEmissaoIni" PropertyName="DataString" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataEmissaoFim" PropertyName="DataString" />
                        <asp:ControlParameter ControlID="drpEmitente" Name="idLoja" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="drpTipoContratante" Name="tipoContratante" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="drpContratante" Name="idContratante" PropertyName="SelectedValue" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacao" runat="server"
                    TypeName="Colosoft.Translator" SelectMethod="GetTranslatesFromTypeName">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.SituacaoEnum, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:ObjectDataSource ID="odsUFPercurso" runat="server"
                    TypeName="Glass.Data.DAL.CidadeDAO" DataObjectTypeName="Glass.Data.Model.Cidade"
                    SelectMethod="ObterUF">
                </asp:ObjectDataSource>
                <sync:ObjectDataSource ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </sync:ObjectDataSource>
                <sync:ObjectDataSource ID="odsFornecedor" runat="server" EnableViewState="false"
                    TypeName="Glass.Data.DAL.FornecedorDAO" SelectMethod="GetOrdered">
                </sync:ObjectDataSource>
                <sync:ObjectDataSource ID="odsCliente" runat="server" EnableViewState="false"
                    TypeName="Glass.Data.DAL.ClienteDAO" SelectMethod="GetOrdered">
                </sync:ObjectDataSource>
                <sync:ObjectDataSource ID="odsTransportador" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.TransportadorDAO" EnableViewState="false">
                </sync:ObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">

        $( document ).ready(function() {
            var falhaEmitirMDFe = GetQueryString('FalhaEmitirMDFe');
            var idMDFe = GetQueryString('IdMDFe');

            var txtConfirmacao = "Houve uma falha de conexão ao tentar emitir o MDF-e.";
            txtConfirmacao+="\n\nNesse caso é possível realizar a emissão em Contingência Offline, porém a mesma deverá ser posteriormente autorizada.";
            txtConfirmacao+="\nA não autorização em 168 hrs, seja por inconsistência ou persistência do problema, poderá resultar em custos e riscos adicionais.";
            txtConfirmacao+="\n\nDeseja prosseguir?";

            if (falhaEmitirMDFe == "true" && confirm(txtConfirmacao)) {
                var ret = LstManifestoEletronico.ImprimirMDFeContingencia(idMDFe);

                if (ret.error != null) {
                    alert(ret.error.description);
                    return false;
                }

                window.location.href = window.location.origin + "/Listas/LstManifestoEletronico.aspx?Retorno=" + ret.value;
            }
        });

    </script>

</asp:Content>
