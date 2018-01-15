<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlDadosCnab.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlDadosCnab" %>

<style type="text/css">
    #details-view {
        display: table;
        border-spacing: 2px;
        border: solid 1px #ccc;
        border-collapse: separate;
        min-width: 800px;
    }

    .details-view-title {
        background-color: #E9ECF1;
        text-align: left;
        font-weight: bold;
        padding-left: 10px;
        padding-right: 5px;
    }
</style>

<table id="details-view">
    <tr id="tipoCnab" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label3" runat="server" Text="Tipo de CNAB"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlTipoCnab" runat="server" AutoPostBack="true">
                <asp:ListItem Text="CNAB 240" Value="1"></asp:ListItem>
                <asp:ListItem Text="CNAB 400" Value="2"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="codigo-ocorrencia" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label16" runat="server" Text="Cód. de Ocorrência"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlCodigoOcorrencia" runat="server" AppendDataBoundItems="true"
                DataSourceID="odsCodOcorrencia" DataTextField="Value" DataValueField="Key">
            </asp:DropDownList>
        </td>
    </tr>
    <%--<tr id="instrucao-alegacao" class="details-view-row" style="display: none">
        <td class="details-view-title">
            <asp:Label ID="Label17" runat="server" Text="Instrução/Alegação"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlCodigoInstAleg" runat="server" AppendDataBoundItems="true">
            </asp:DropDownList>
        </td>
    </tr>--%>
    <tr id="codigo-movimento" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label5" runat="server" Text="Cód. de Mov. Remessa"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlCodigoMovimento" runat="server" AppendDataBoundItems="true"
                DataSourceID="odsCodMovRemessa" DataTextField="Value" DataValueField="Key">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="carteira" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label4" runat="server" Text="Carteira"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlCarteira" runat="server" AppendDataBoundItems="True"
                DataSourceID="odsCodCarteira" DataTextField="Value" DataValueField="Key">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="cadastramento" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label51" runat="server" Text="Cadastramento"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlCadastramento" runat="server" AppendDataBoundItems="true"
                DataSourceID="odsCodCadastramento" DataTextField="Value" DataValueField="Key">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="especie-documento" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label6" runat="server" Text="Espécie do Documento"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlEspecie" runat="server" AppendDataBoundItems="true"
                DataSourceID="odsCodEspDocumento" DataTextField="Value" DataValueField="Key">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="aceite" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label9" runat="server" Text="Aceite"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlAceite" runat="server" AppendDataBoundItems="true"
                DataSourceID="odsAceite" DataTextField="Value" DataValueField="Key">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="juros-mora" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label10" runat="server" Text="Juros de Mora"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlCodigoJurosMora" runat="server" AppendDataBoundItems="true"
                DataSourceID="odsCodJuros" DataTextField="Value" DataValueField="Key">
            </asp:DropDownList>
            <table>
                <tr>
                    <td>Nº Dias:
                    </td>
                    <td>
                        <asp:TextBox ID="txtDataJuroMora" Text="0" onkeypress="return soNumeros(event, false, true);"
                            runat="server"></asp:TextBox>
                    </td>
                    <td>Valor:
                    </td>
                    <td>
                        <asp:TextBox ID="txtValorJuroMora" Text="0" onkeypress="return soNumeros(event, false, true);"
                            runat="server"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr id="desconto" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label11" runat="server" Text="Desconto"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlCodigoDesconto" runat="server" AppendDataBoundItems="true"
                DataSourceID="odsCodDesconto" DataTextField="Value" DataValueField="Key">
            </asp:DropDownList>
            <table>
                <tr>
                    <td>Nº Dias:
                    </td>
                    <td>
                        <asp:TextBox ID="txtDataDesconto" Text="0" onkeypress="return soNumeros(event, false, true);"
                            runat="server"></asp:TextBox>
                    </td>
                    <td>Valor:
                    </td>
                    <td>
                        <asp:TextBox ID="txtValorDesconto" Text="0" onkeypress="return soNumeros(event, false, true);"
                            runat="server"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr id="codigo-protesto" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label12" runat="server" Text="Cód. Protesto"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlCodigoProtesto" runat="server" AppendDataBoundItems="true"
                DataSourceID="odsCodProtesto" DataTextField="Value" DataValueField="Key">
            </asp:DropDownList>
            <table>
                <tr>
                    <td>Nº Dias:
                    </td>
                    <td>
                        <asp:TextBox ID="txtDiasProtesto" Text="0" onkeypress="return soNumeros(event, false, true);"
                            runat="server"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr id="multa" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label23" runat="server" Text="Multa"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlCodigoMulta" runat="server" AppendDataBoundItems="true"
                DataSourceID="odsCodMulta" DataTextField="Value" DataValueField="Key">
            </asp:DropDownList>
            <table>
                <tr>
                    <td>Percentual de Multa por Atraso:
                    </td>
                    <td>
                        <asp:TextBox ID="txtMultaValor" Text="0" onkeypress="return soNumeros(event, false, true);"
                            runat="server"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr id="codigo-baixa-devolucao" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label13" runat="server" Text="Cód. Baixa/Devolução"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlCodigoBaixaDevolucao" runat="server" AppendDataBoundItems="true"
                DataSourceID="odsCodBaixaDevolucao" DataTextField="Value" DataValueField="Key">
            </asp:DropDownList>
            <table>
                <tr>
                    <td>Nº Dias:
                    </td>
                    <td>
                        <asp:TextBox ID="txtDiasBaixaDevolucao" Text="0" onkeypress="return soNumeros(event, false, true);"
                            runat="server"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr id="iof" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label121" runat="server" Text="IOF"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:TextBox ID="txtValorIOF" Text="0" onkeypress="return soNumeros(event, false, true);"
                runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr id="abatimento" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label14" runat="server" Text="Abatimento"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:TextBox ID="txtAbatimento" Text="0" onkeypress="return soNumeros(event, false, true);"
                runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr id="instrucao1" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label15" runat="server" Text="Instrução 1"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlInstrucao" runat="server" AppendDataBoundItems="true" Width="500px"
                DataSourceID="odsInstrucao" DataTextField="Value" DataValueField="Key">
            </asp:DropDownList>
            <asp:Image ID="imgAlertaInstrucao1" ImageUrl="~/Images/alerta.png" ToolTip="Valor padrão não carregado, favor rever a informação deste campo" runat="server" Visible="false" />
        </td>
    </tr>
    <tr id="instrucao2" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label1" runat="server" Text="Instrução 2"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlInstrucao2" runat="server" AppendDataBoundItems="true" Width="500px"
                DataSourceID="odsInstrucao" DataTextField="Value" DataValueField="Key">
            </asp:DropDownList>
            <asp:Image ID="imgAlertaInstrucao2" ImageUrl="~/Images/alerta.png" ToolTip="Valor padrão não carregado, favor rever a informação deste campo" runat="server" Visible="false" />
        </td>
    </tr>
    <tr id="mensagem" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label2" runat="server" Text="Mensagem"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:TextBox ID="txtMensagem" runat="server" TextMode="MultiLine" Rows="2" Columns="20" Width="480px" Height="58px"></asp:TextBox>
        </td>
    </tr>
    <tr id="emissao-bloqueto" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label18" runat="server" Text="Emissão Bloqueto"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlEmissaoBloqueto" runat="server" AppendDataBoundItems="true"
                DataSourceID="odsEmissaoBloqueto" DataTextField="Value" DataValueField="Key">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="distribuicao-bloqueto" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label19" runat="server" Text="Distribuição Bloqueto"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlDistribuicaoBloqueto" runat="server" AppendDataBoundItems="true"
                DataSourceID="odsDistribuicaoBloqueto" DataTextField="Value" DataValueField="Key">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="tipo-documento" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label20" runat="server" Text="Tipo de Documento"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlTipoDocumento" runat="server" AppendDataBoundItems="true"
                DataSourceID="odsTipoDocumento" DataTextField="Value" DataValueField="Key">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="codigo-moeda" class="details-view-row">
        <td class="details-view-title">
            <asp:Label ID="Label22" runat="server" Text="Código Moeda"></asp:Label>
        </td>
        <td class="details-view-data">
            <asp:DropDownList ID="ddlCodigoMoeda" runat="server" AppendDataBoundItems="true"
                DataSourceID="odsCodMoeda" DataTextField="Value" DataValueField="Key">
            </asp:DropDownList>
        </td>
    </tr>
</table>
<table>
    <tr>
        <td>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCodOcorrencia" runat="server" SelectMethod="ObterCodigoOcorrencia"
                TypeName="Sync.Utils.Boleto.DataSourceHelper">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdfCodBanco" DbType="Object" Name="codBanco" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCodEspDocumento" runat="server" SelectMethod="ObterEspecieDocumento"
                TypeName="Sync.Utils.Boleto.DataSourceHelper">
                <SelectParameters>
                    <asp:ControlParameter ControlID="ddlTipoCnab" DbType="Object" Name="tipoArquivo" />
                    <asp:ControlParameter ControlID="hdfCodBanco" DbType="Object" Name="codBanco" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCodMovRemessa" runat="server" SelectMethod="ObterCodigoMovimentoRemessa"
                TypeName="Sync.Utils.Boleto.DataSourceHelper">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdfCodBanco" DbType="Object" Name="codBanco" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCodCarteira" runat="server" SelectMethod="ObterCarteiras"
                TypeName="Sync.Utils.Boleto.DataSourceHelper">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdfCodBanco" DbType="Object" Name="codBanco" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCodCadastramento" runat="server" SelectMethod="ObterCadastramento"
                TypeName="Sync.Utils.Boleto.DataSourceHelper">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdfCodBanco" DbType="Object" Name="codBanco" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsAceite" runat="server" SelectMethod="ObterAceite"
                TypeName="Sync.Utils.Boleto.DataSourceHelper">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdfCodBanco" DbType="Object" Name="codBanco" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCodJuros" runat="server" SelectMethod="ObterCodigoJurosMora"
                TypeName="Sync.Utils.Boleto.DataSourceHelper">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdfCodBanco" DbType="Object" Name="codBanco" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCodDesconto" runat="server" SelectMethod="ObterCodigoDesconto"
                TypeName="Sync.Utils.Boleto.DataSourceHelper">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdfCodBanco" DbType="Object" Name="codBanco" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCodProtesto" runat="server" SelectMethod="ObterCodigoProtesto"
                TypeName="Sync.Utils.Boleto.DataSourceHelper">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdfCodBanco" DbType="Object" Name="codBanco" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCodMulta" runat="server" SelectMethod="ObterCodigoMulta"
                TypeName="Sync.Utils.Boleto.DataSourceHelper">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdfCodBanco" DbType="Object" Name="codBanco" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCodBaixaDevolucao" runat="server" SelectMethod="ObterBaixaDevolucao"
                TypeName="Sync.Utils.Boleto.DataSourceHelper">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdfCodBanco" DbType="Object" Name="codBanco" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsInstrucao" runat="server" SelectMethod="ObterInstrucoes"
                TypeName="Sync.Utils.Boleto.DataSourceHelper">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdfCodBanco" DbType="Object" Name="codBanco" />
                    <asp:ControlParameter ControlID="ddlTipoCnab" DbType="Object" Name="tipoArquivo" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsEmissaoBloqueto" runat="server" SelectMethod="ObterIdentificaoEmissaoBloqueto"
                TypeName="Sync.Utils.Boleto.DataSourceHelper">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdfCodBanco" DbType="Object" Name="codBanco" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsDistribuicaoBloqueto" runat="server" SelectMethod="ObterIdentificacaoDistribuicao"
                TypeName="Sync.Utils.Boleto.DataSourceHelper">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdfCodBanco" DbType="Object" Name="codBanco" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoDocumento" runat="server" SelectMethod="ObterTipoDocumento"
                TypeName="Sync.Utils.Boleto.DataSourceHelper">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdfCodBanco" DbType="Object" Name="codBanco" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCodMoeda" runat="server" SelectMethod="ObterCodigoMoeda"
                TypeName="Sync.Utils.Boleto.DataSourceHelper">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdfCodBanco" DbType="Object" Name="codBanco" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
        </td>
    </tr>
</table>

<asp:HiddenField runat="server" ID="hdfCodBanco" />
