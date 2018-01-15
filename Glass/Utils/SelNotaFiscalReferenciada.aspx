<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelNotaFiscalReferenciada.aspx.cs" Inherits="Glass.UI.Web.Utils.SelNotaFiscalReferenciada"
    Title="Selecione a Nota Fiscal" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlTextoTooltip.ascx" TagName="ctrlTextoTooltip" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc4" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function setNf(idNf, numNf)
        {
            window.opener.setNfReferenciada(idNf, numNf);
        }       

        function ObtemNumeroNFGrid(numeroNF, idNfe) {
            var dado = SelNotaFiscalReferenciada.ObtemNumeroNF(numeroNF, idNfe).value;
            var notas = '';
            if (dado != '' && dado != null)
                notas = dado.split(",")

            if (notas == null || notas == '')
                window.opener.setNfReferenciada('', '');
            else {                
                var numeros = '';
                var ids = '';
                for (var i = 0; i < notas.length; i++) {
                    if (notas[i] != "") {
                        ids += notas[i].split(";")[0] + (i == notas.length - 1 ? "" : ",");
                        numeros += notas[i].split(";")[1] + (i == notas.length - 1 ? "" : ",");
                    }
                }
                window.opener.setNfReferenciada(ids, numeros);
            }
        }
    </script>
    <asp:HiddenField runat="server" ID="hdfNumeroNotaFiscal" />
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Num. NF" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumNf" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Num. Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc4:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="true" OnSelectedIndexChanged="drpLoja_SelectedIndexChanged" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="CFOP" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpCfop" runat="server" DataSourceID="odsCfop" DataTextField="CodInterno"
                                DataValueField="IdCfop">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:Label ID="Label15" runat="server" Text="Tipo CFOP" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbxdrpTipoCFOP" runat="server" CheckAll="False" DataSourceID="odsTipoCfop"
                                DataTextField="Descricao" DataValueField="IdTipoCfop" ImageURL="~/Images/DropDown.png"
                                jqueryurl="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" OpenOnStart="False"
                                Title="Selecione os tipos de CFOP">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label25" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtRota" runat="server" MaxLength="20" Width="80px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" OnClientClick="return openRota();" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label14" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtIdCliente" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Tipo Fiscal" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoFiscal" runat="server">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Consumidor Final</asp:ListItem>
                                <asp:ListItem Value="2">Revenda</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtIdFornec" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeFornecedor" runat="server" Width="170px" onkeypress="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label8" runat="server" Text="Período Emissão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Período Entrada/Saída" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataEntSaiIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataEntSaiFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label17" runat="server" Text="Forma de emissão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFormaEmissao" runat="server">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Normal</asp:ListItem>
                                <asp:ListItem Value="3">Contingência com SCAN</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label16" runat="server" Text="Finalidade" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFinalidade" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Normal</asp:ListItem>
                                <asp:ListItem Value="2">Complementar</asp:ListItem>
                                <asp:ListItem Value="3">Ajuste</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label26" runat="server" Text="Forma Pagto." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFormaPagto" runat="server">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">À Vista</asp:ListItem>
                                <asp:ListItem Value="2">À Prazo</asp:ListItem>
                                <asp:ListItem Value="3">Outros</asp:ListItem>
                            </asp:DropDownList>
                            <asp:DropDownList ID="drpIdFormaPagto" runat="server" DataSourceID="odsFormasPagto"
                                DataTextField="Descr" DataValueField="Id" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Ordenar por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenar" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="1">Número NF (decresc.)</asp:ListItem>
                                <asp:ListItem Value="2">Número NF (cresc.)</asp:ListItem>
                                <asp:ListItem Value="3" Selected="True">Data de emissão (descresc.)</asp:ListItem>
                                <asp:ListItem Value="4">Data de emissão (cresc.)</asp:ListItem>
                                <asp:ListItem Value="5">Data de entrada/saída (descresc.)</asp:ListItem>
                                <asp:ListItem Value="6">Data de entrada/saída (cresc.)</asp:ListItem>
                                <asp:ListItem Value="7">Valor Total(cresc.)</asp:ListItem>
                                <asp:ListItem Value="8">Valor Total (descresc.)</asp:ListItem>
                            </asp:DropDownList>
                        </td>

                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Tipo NFe" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoNFe" runat="server" AutoPostBack="True">                                
                                <asp:ListItem Value="1">Entrada</asp:ListItem>
                                <asp:ListItem Value="2">Saída</asp:ListItem>
                                <asp:ListItem Value="3" Selected="True">Entrada (terceiros)</asp:ListItem>
                                <asp:ListItem Value="4">Transporte</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="loadProduto(this);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:TextBox ID="txtDescr" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label27" runat="server" Text="Total Nota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorInicial" runat="server" onkeypress="return soNumeros(event, false, true);"
                                Width="60px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label28" runat="server" Text="a" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorFinal" runat="server" onkeypress="return soNumeros(event, false, true);"
                                Width="60px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton11" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Inf. Compl./Obs." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtInfCompl" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" Style="width: 16px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdNf" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    DataSourceID="odsNf" DataKeyNames="IdNf" EmptyDataText="Nenhum Nota Fiscal encontrada."
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" 
                    EditRowStyle-CssClass="edit" OnRowCommand="grdNf_RowCommand" OnRowDataBound="grdNf_RowDataBound"
                    Width="100%" OnPageIndexChanging="OnPaging" OnRowCreated="grdNf_RowCreated" OnSelectedIndexChanged="grdNf_SelectedIndexChanged" OnSelectedIndexChanging="grdNf_SelectedIndexChanging">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox runat="server" ID="chkItem" onchange='<%# "ObtemNumeroNFGrid(" + Eval("NumeroNFe") +"," + Eval("IdNf") +")" %>' OnCheckedChanged="chkItem_CheckedChanged">
                                    </asp:CheckBox>
                                <asp:HiddenField ID="hdfIdNf" runat="server" Value='<%# Eval("IdNf") %>' />
                                <asp:HiddenField ID="hdfCorLinha" runat="server" Value='<%# Eval("CorLinhaGrid") %>' />
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <%--<a href="#" onclick='setNf(&#039;<%# Eval("IdNf") %>&#039;,&#039;<%# Eval("NumeroNFe") %>&#039;);closeWindow();return false;'>
                                    <img alt="Selecionar" border="0" src="../Images/ok.gif" title="Selecionar" /></a>--%>
                                
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="NumeroNFe" HeaderText="Num. NF" SortExpression="NumeroNFe" />
                        <asp:BoundField DataField="Modelo" HeaderText="Modelo" SortExpression="Modelo" />
                        <asp:BoundField DataField="CodCfop" HeaderText="CFOP" SortExpression="CodCfop" />
                        <asp:BoundField DataField="TipoDocumentoString" HeaderText="Tipo" SortExpression="TipoDocumentoString" />
                        <asp:BoundField DataField="DescrUsuCad" HeaderText="Funcionário" SortExpression="DescrUsuCad" />
                        <asp:BoundField DataField="NomeEmitente" HeaderText="Emitente" SortExpression="NomeEmitente" />
                        <asp:BoundField DataField="NomeDestRem" HeaderText="Destinatário/Remetente" SortExpression="NomeDestRem" />
                        <asp:BoundField DataField="TotalNota" HeaderText="Total" SortExpression="TotalNota"
                            DataFormatString="{0:C}" />
                        <asp:BoundField DataField="DataEmissao" DataFormatString="{0:d}" HeaderText="Data Emissão"
                            SortExpression="DataEmissao" />
                        <asp:BoundField DataField="SituacaoString" HeaderText="Situação" SortExpression="SituacaoString" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsNf" runat="server" DataObjectTypeName="Glass.Data.Model.NotaFiscal"
                    DeleteMethod="Delete" SelectMethod="GetListPorSituacao" TypeName="Glass.Data.DAL.NotaFiscalDAO"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCountPorSituacao"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumNf" Name="numeroNFe" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:Parameter Name="modelo" Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtIdCliente" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeCliente" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpTipoFiscal" Name="tipoFiscal" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtIdFornec" Name="idFornec" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeFornecedor" Name="nomeFornec" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtRota" Name="codRota" PropertyName="Text" Type="String" />
                        <asp:Parameter Name="tipoDoc" Type="Int32" />
                        <asp:Parameter Name="situacao" Type="String" DefaultValue="2,13" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpCfop" Name="idsCfop" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="cbxdrpTipoCFOP" Name="idsTiposCfop" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="ctrlDataEntSaiIni" Name="dataEntSaiIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataEntSaiFim" Name="dataEntSaiFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpFormaPagto" Name="formaPagto" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpIdFormaPagto" Name="idFormaPagto" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter Name="tipoNf" ControlID="drpTipoNFe" Type="Int32" />
                        <asp:ControlParameter ControlID="drpFinalidade" Name="finalidade" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpFormaEmissao" Name="formaEmissao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtInfCompl" Name="infCompl" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInternoProd" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescr" Name="descrProd" PropertyName="Text" Type="String" />
                         <asp:ControlParameter ControlID="txtValorInicial" Name="valorInicial" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtValorFinal" Name="valorFinal" PropertyName="Text"
                            Type="String" />
                        <asp:Parameter Name="cnpjFornecedor" DefaultValue="" />
                        <asp:ControlParameter ControlID="drpOrdenar" Name="ordenar" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:Parameter  Name="lote" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCfop" runat="server" SelectMethod="GetSortedByCodInterno"
                    TypeName="Glass.Data.DAL.CfopDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCfop" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.TipoCfopDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormasPagto" runat="server" SelectMethod="GetFormasPagtoNf"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
