<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExecScript.aspx.cs" Inherits="Glass.UI.Web.Utils.ExecScript" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .style1
        {
            height: 41px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fpImportarClientes" runat="server" />
                </td>
                <td>
                    Loja:
                </td>
                <td>
                    <uc3:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="false" MostrarTodas="false" />
                </td>
                <td>
                    <asp:Button ID="btnImportarClientes" runat="server" Text="Importar Clientes" OnClick="btnImportarClientes_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogImportaçãoCliente" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fpImportarFornecedores" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnImportarFornec" runat="server" Text="Importar Fornecedores" OnClick="btnImportarFornecedores_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogImportacaoFornec" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fupImportarProdutos" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnImportarProdutos" runat="server" Text="Importar Produtos" OnClick="btnImportarProdutos_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtImportarProdutos" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fupImportarSubgrupo" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnImportarSubgrupo" runat="server" Text="Importar Subgrupos" OnClick="btnImportarSubgrupo_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogImportarSubgrupo" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fupAtualizaIdCliente" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnAtualizaIdCliente" runat="server" Text="Atualizar Id Cliente"
                        OnClick="btnAtualizaIdCliente_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogAtualizaIdCliente" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fupAtualizarTipoCliente" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnAtualizarTipoCliente" runat="server" Text="Atualizar Tipo Cliente"
                        OnClick="btnAtualizarTipoCliente_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogAtualizarTipoCliente" runat="server" TextMode="MultiLine"
                        Height="100px" Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fupAtualizarEndereco" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnAtualizaroEndereco" runat="server" Text="Atualizar Endereço" OnClick="btnAtualizaroEndereco_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogAtualizarEndereco" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="filImportarContasReceber" runat="server" />
                </td>
                <td>
                    Loja:
                </td>
                <td>
                    <uc3:ctrlLoja runat="server" ID="drpLojasContasReceber" AutoPostBack="false" MostrarTodas="false" />
                </td>
                <td>
                    <asp:Button ID="btnImportarContasReceber" runat="server" Text="Importar contas a receber"
                        OnClick="btnImportarContasReceber_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogImportarContasReceber" runat="server" TextMode="MultiLine"
                        Height="100px" Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fupApagarContas" runat="server" />
                </td>
                <td>
                    Loja:
                </td>
                <td>
                    <uc3:ctrlLoja runat="server" ID="drpIdLojaApagarContas" AutoPostBack="false" MostrarTodas="false" />
                </td>
                <td>
                    <asp:Button ID="btnApagarContas" runat="server" Text="Apagar contas a receber MS Vidros" OnClick="btnApagarContas_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogApagarContas" runat="server" TextMode="MultiLine"
                        Height="100px" Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="filImportarContasPagar" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnImportarContasPagar" runat="server" Text="Importar contas a pagar"
                        OnClick="btnImportarContasPagar_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogImportarContasPagar" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fupDadosCliente" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnDadosCliente" runat="server" Text="Importar limite, simples, data fundação"
                        OnClick="btnDadosCliente_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogImportarDadosCliente" runat="server" TextMode="MultiLine"
                        Height="100px" Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fupImportarPrecoProduto" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btmImportarPrecoProduto" runat="server" Text="Importar preço do produto"
                        OnClick="btmImportarPrecoProduto_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogImportarPrecoProduto" runat="server" TextMode="MultiLine"
                        Height="100px" Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fupImportarTipoMercadoria" runat="server" />
                </td>
                <td>
                    <asp:Button ID="Button1" runat="server" Text="Importar Tipo Mercadoria, Cod. Otimização"
                        OnClick="btmImportarTipoMercadoria_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogImportarTipoMercadoria" runat="server" TextMode="MultiLine"
                        Height="100px" Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fupImportarDescontoCliente" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnImportarDescontoCliente" runat="server" Text="Importar desconto cliente"
                        OnClick="btnImportarDescontoCliente_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogImportarDescontoCliente" runat="server" TextMode="MultiLine"
                        Height="100px" Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fupImportarRotaCliente" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnImportarRotaCliente" runat="server" Text="Importar rota cliente"
                        OnClick="btnImportarRotaCliente_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtImportarRotaCliente" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fupDescEsp" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnDescEsp" runat="server" Text="Importar desconto esp." OnClick="btnDescEsp_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtDescEsp" runat="server" TextMode="MultiLine" Height="100px" Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:Button ID="btnPrecoTransferencia" runat="server" Text="Gerar preço transfêrencia"
                        OnClick="btnPrecoTransferencia_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtPrecoTransferencia" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fupImportarNomeFantasia" runat="server" />
                </td>
                <td>
                    <asp:Button ID="Button2" runat="server" Text="Importar nome fantasia, vendedor do cliente"
                        OnClick="Button2_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtImportarNomeFantasia" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fupImportarAltLarBox" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnImportarAltLarBox" runat="server" Text="Importar altura, largura box"
                        OnClick="btnImportarAltLarBox_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtImportarAltLarBox" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:Button ID="btnImpProdBase" runat="server" Text="Importar prod. base" OnClick="btnImpProdBase_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtImpProdBase" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:Button ID="btnImpMatPri" runat="server" Text="Importar matéria-prima" OnClick="btnImpMatPri_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtImpMatPri" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="filImpEstoque" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnImpEstoque" runat="server" Text="Importar estoque" OnClick="btnImpEstoque_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogImpEstoque" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="filImpCheques" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnImpCheques" runat="server" Text="Importar cheques" OnClick="btnImpCheques_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogImpCheques" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="filImportarCreditoFornec" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnImportarCreditoFornec" runat="server" Text="Importar crédito fornecedor"
                        OnClick="btnImportarCreditoFornec_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogImportarCreditoFornec" runat="server" TextMode="MultiLine"
                        Height="100px" Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="filImportarCreditoCliente" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnImportarCreditoCliente" runat="server" Text="Importar crédito cliente"
                        OnClick="btnImportarCreditoCliente_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogImportarCreditoCliente" runat="server" TextMode="MultiLine"
                        Height="100px" Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:Button ID="btnApagarPlanoConta" runat="server" Text="Apagar Plano de Contas"
                        OnClick="btnApagarPlanoConta_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtApagarPlanoContaLog" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:Button ID="btnApagarGrupoConta" runat="server" Text="Apagar Grupo de Contas"
                        OnClick="btnApagarGrupoConta_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogApagarGrupoConta" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fupImgGrupoConta" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btmImpGrupoConta" runat="server" Text="Importar Grupo Conta" OnClick="btmImpGrupoConta_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogImpGrupoConta" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fupImpPlanoConta" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnImpPlanoConta" runat="server" Text="Importar Plano Conta" OnClick="btnImpPlanoConta_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogImpPlanoConta" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <asp:Button ID="btnCorrigirMovEstoqueFiscalChapas" runat="server" Text="Corrigir mov. estoque Fiscal Chapas"
            OnClick="btnCorrigirMovEstoqueFiscalChapas_Click" />
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fupImpMateriaPrimaLaminado" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnImpMateriaPrimaLaminado" runat="server" Text="Importar Materia Prima Laminado"
                        OnClick="btnImpMateriaPrimaLaminado_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogMateriaPrimaLaminado" runat="server" TextMode="MultiLine"
                        Height="100px" Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <asp:Button ID="btnApagarMovBancoEstorno" runat="server" Text="Apagar mov. banco estorno"
            OnClick="btnApagarMovBancoEstorno_Click" />
        <br />
        <br />
        <asp:Button ID="btnCorrigirPecasRoteiro" runat="server" Text="Corrigir peças de roteiro"
            OnClick="btnCorrigirPecasRoteiro_Click" />
        <br />
        <br />
        <table>
            <tr>
                <td>
                    <asp:TextBox ID="txtIdsPedidosAtuSituacao" runat="server" Width="338px"></asp:TextBox>&nbsp;&nbsp;
                    <asp:Button ID="btnAtualizaSituacaoProducaoPedido" runat="server" Text="Atualizar situação produção nos pedidos"
                        OnClick="btnAtualizaSituacaoProducaoPedido_Click" Width="250px" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogAtualizaSituacaoProducaoPedido" runat="server" TextMode="MultiLine" Height="100px" Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:Label ID="lbl1" runat="server" Text="Ids dos Pedidos para marcar peça pronta"></asp:Label>
                    <asp:TextBox ID="txtIdsPedidosMarcarPecaPronta" runat="server"></asp:TextBox>
                </td>
                <td>
                    <asp:Button ID="Button3" runat="server" Text="Marcar peça pronta" OnClick="btnMarcarPecaPronta_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogMarcarPecaPronta" runat="server" TextMode="MultiLine" Height="100px"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    <asp:TextBox ID="txtEmailCarregamento" runat="server"></asp:TextBox>
                </td>
                <td class="style1">
                    <asp:Button ID="btnEnviarEmailCarregamento" runat="server" Text="Enviar Email carregamento"
                        OnClick="btnEnviarEmailCarregamento_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:TextBox ID="txtLogEnvioEmailCarregamento" runat="server" Height="100px" Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    <asp:FileUpload ID="fupImpPrecoFornec" runat="server" />
                </td>
                <td class="style1">
                    <asp:Button ID="btnImpPrecoFornec" runat="server" Text="Importar preço do fornec."
                        OnClick="btnImpPrecoFornec_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:TextBox ID="txtLogImpPrecoFornec" runat="server" Height="100px" Width="600px"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    <asp:TextBox ID="txtIdsPedidos" runat="server"></asp:TextBox>
                </td>
                <td class="style1">
                    <asp:Button ID="btnProduzirPedidos" runat="server" Text="Confirmar PCP." OnClick="btnProduzirPedidos_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:TextBox ID="txtLogProduzirPedidos" runat="server" Height="100px" Width="600px"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <asp:Button ID="btnApagarMovEstoqueTrocaDev" runat="server" Text="Apagar movimentações troca/devolução Personal"
            OnClick="btnApagarMovEstoqueTrocaDev_Click" />
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    <asp:TextBox ID="txtGerarEstoqueRealNotas" runat="server"></asp:TextBox>
                </td>
                <td class="style1">
                    <asp:Button ID="btnGerarEstoqueRealNotas" runat="server" Text="Gerar movimentações estoque notas"
                        OnClick="btnGerarEstoqueRealNotas_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:TextBox ID="txtLogGerarEstoqueRealNotas" runat="server" Height="100px" Width="600px"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    Num. Nota Fiscal
                    <asp:TextBox ID="txtNumNf" runat="server"></asp:TextBox>
                </td>
                <td class="style1">
                    <asp:Button ID="btnSepararValores" runat="server" Text="Separar valores" OnClick="btnSepararValores_Click" />
                    <asp:Button ID="btnCancelarSepararValores" runat="server" Text="Cancelar separação valores"
                        OnClick="btnCancelarSepararValores_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:TextBox ID="txtLogSepararValores" runat="server" Height="100px" Width="600px"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    Pedidos:
                    <asp:TextBox ID="txtIdAtualizaValorPedido" runat="server"></asp:TextBox>
                </td>
                <td class="style1">
                    <asp:Button ID="btnAtualizaValorPedido" runat="server" Text="Atualizar valor do pedido"
                        OnClick="btnAtualizaValorPedido_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:TextBox ID="txtLogAtualizaValorPedido" runat="server" Height="100px" Width="600px"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    <asp:Button ID="btnBuscarPecasErradas" runat="server" Text="Buscar peças erradas"
                        OnClick="btnBuscarPecasErradas_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtLogBuscarPecasErradas" runat="server" Height="100px" Width="600px"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    Produtos Pedido Produção:
                    <asp:TextBox ID="txtIdsProdPedProducao" runat="server"></asp:TextBox>
                </td>
                <td class="style1">
                    <asp:Button ID="btnVoltarSituaçãoPeca" runat="server" Text="Voltar situação peça"
                        OnClick="btnVoltarSituaçãoPeca_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:TextBox ID="txtLogVoltarSituacaoPeca" runat="server" Height="100px" Width="600px"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    <asp:Button ID="btnRegistroArquivoRemessa" runat="server" Text="RegistroArquivoRemessa"
                        OnClick="btnRegistroArquivoRemessa_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtLogRegistroArquivoRemessa" runat="server" Height="100px" Width="600px"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    <asp:Button ID="btnAtualizarProducaoPedido" runat="server" 
                        Text="Atualizar setor e situação do produto na produção" 
                        onclick="btnAtualizarProducaoPedido_Click"  />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtLogAtualizarProducaoPedido" runat="server" Height="100px" Width="600px"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        </table>
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:Label ID="lbl2" runat="server" 
                        Text="Id NF dar saída/entrada estoque (Autorizadas na Sefaz em outro PC)"></asp:Label>
                    <asp:TextBox ID="txtIdNf" runat="server"></asp:TextBox>
                </td>
                <td>
                    <asp:Button ID="btnAlterarEstoque" runat="server" Text="Alterar Estoque" 
                        OnClick="btnAlterarEstoque_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    &nbsp;</td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    <asp:Button ID="btnAtualizaItensRevendaCarregamento" runat="server" 
                        Text="Atualizar itens revenda carregamento" 
                        onclick="btnAtualizaItensRevendaCarregamento_Click"   />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtLogAtualizaItensRevendaCarregamento" runat="server" Height="100px" Width="600px"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    <asp:Button ID="btnCorrigirProducaoCarregamento" runat="server" 
                        Text="Corrigir Produção Carregamento" OnClick="btnCorrigirProducaoCarregamento_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtLogCorrigirProducaoCarregamento" runat="server" Height="100px" Width="600px"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    Pedido:
                    <asp:TextBox ID="txtPedidoCorrigirSituacaoPecas" runat="server"></asp:TextBox>
                </td>
                <td class="style1">
                    <asp:Button ID="btnCorrigirSituacaoPecas" runat="server" Text="Corrigir situação peças pedido"
                        OnClick="btnCorrigirSituacaoPecas_Click" />
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    <asp:Button ID="btnEstoqueInicialCliente" runat="server" 
                        Text="Corrigir Estoque Inicial Cliente" OnClick="btnEstoqueInicialCliente_Click"  />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtLogEstoqueInicialCliente" runat="server" Height="100px" Width="600px"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    <asp:Button ID="btnEstoquePosicaoMateriaPrima" runat="server" 
                        Text="Ajustar estoque com posição da materia-prima" OnClick="btnEstoquePosicaoMateriaPrima_Click"   />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtLogEstoquePosicaoMateriaPrima" runat="server" Height="100px" Width="600px"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    <asp:Button ID="btnCriarPagtoContasRecebidas" runat="server" 
                        Text="Criar pagamento contas recebidas" OnClick="btnCriarPagtoContasRecebidas_Click"    />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtImpCriarPagtoContasRecebeidas" runat="server" Height="100px" Width="600px"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td>
                    <asp:FileUpload ID="fupImportarValorCentroCusto" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnImporarValorCentroCusto" runat="server" Text="Importar preço do centro de custo do produto" OnClick="btnImporarValorCentroCusto_Click"
                     />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:TextBox ID="txtLogImportarValorCentroCusto" runat="server" TextMode="MultiLine"
                        Height="100px" Width="600px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    <asp:Button ID="btnClientesCreditoIncorreto" runat="server" 
                        Text="Buscar clientes com crédito incorreto" OnClick="btnClientesCreditoIncorreto_Click"    />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtClientesCreditoIncorreto" runat="server" Height="100px" Width="600px"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    <asp:Button ID="btnGerarSqlLimparBanco" runat="server" 
                        Text="Gerar SQL para Limpar o Banco" OnClick="btnGerarSqlLimparBanco_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtGerarSqlLimparBanco" runat="server" Height="100px" Width="600px"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    <asp:Button ID="btnAtualizarNomeImagensProjetoModelo" runat="server"
                        Text="Atualizar nome das imagens do projeto modelo" OnClick="btnAtualizarNomeImagensProjetoModelo_Click" />
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    <asp:Button ID="btnAjustePagtoContasReceberDeAcerto" runat="server" 
                        Text="Ajustar pagamento contas recebidas, de acerto, do dia 09/12/2016 em diante (chamado 50768)" OnClick="btnAjustePagtoContasReceberDeAcerto_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtAjustePagtoContasReceberDeAcerto" runat="server" Height="100px" Width="600px"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table cellspacing="5" cellpadding="5">
            <tr>
                <td class="style1">
                    <asp:Button ID="btnLocalizacaoContas" runat="server" 
                        Text="Ajustar Localização das contas pagas" OnClick="btnLocalizacaoContas_Click" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
