using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.Helper
{
    public static class UtilsPlanoConta
    {
        #region Enumeradores

        public enum PlanoContas
        {
            // Vendas para funcionários: Grupo 52
            FuncBoleto,
            FuncCartao,
            FuncDinheiro,
            FuncCheque,
            FuncDeposito,
            FuncConstrucard,
            FuncPermuta,
            FuncPrazo,
            FuncCredito,
            FuncRecebimento,

            // Faturamento sobre vendas: Grupo 51
            EntradaDinheiro,
            EntradaBoleto,
            EntradaCheque,
            EntradaCartao,
            EntradaConstrucard,
            EntradaDeposito,
            EntradaPermuta,
            EntradaCredito,

            EstornoBoleto,
            EstornoCartao,
            EstornoCheque,
            EstornoDeposito,
            EstornoDinheiro,
            EstornoCredito,
            EstornoPermuta,
            EstornoConstrucard,
            EstornoBoletoLumen,
            EstornoBoletoSantander,
            EstornoBoletoBancoBrasil,
            EstornoBoletoOutros,
            EstornoRetiradaDinheiro,
            EstornoRetiradaCheque,
            EstornoCreditoDinheiro,
            EstornoCreditoCheque,
            EstornoJurosVendaCartao,
            EstornoJurosVendaConstrucard,

            EstornoRecPrazoDinheiro,
            EstornoRecPrazoCheque,
            EstornoRecPrazoCartao,
            EstornoRecPrazoDeposito,
            EstornoRecPrazoCredito,
            EstornoRecPrazoConstrucard,
            EstornoRecPrazoPermuta,
            EstornoRecPrazoBoleto,
            EstornoRecPrazoBoletoLumen,
            EstornoRecPrazoBoletoSantander,
            EstornoRecPrazoBoletoBancoBrasil,
            EstornoRecPrazoBoletoOutros,

            EstornoEntradaDinheiro,
            EstornoEntradaCheque,
            EstornoEntradaCartao,
            EstornoEntradaDeposito,
            EstornoEntradaCredito,
            EstornoEntradaConstrucard,
            EstornoEntradaPermuta,
            EstornoEntradaBoleto,
            EstornoEntradaBoletoLumen,
            EstornoEntradaBoletoSantander,
            EstornoEntradaBoletoBancoBrasil,
            EstornoEntradaBoletoOutros,

            EstornoChequeDevDinheiro,
            EstornoChequeDevCheque,
            EstornoChequeDevCartao,
            EstornoChequeDevBoleto,
            EstornoChequeDevDeposito,
            EstornoChequeDevCredito,
            EstornoChequeDevConstrucard,
            EstornoChequeDevPermuta,
            EstornoChequeDevBoletoLumen,
            EstornoChequeDevBoletoSantander,
            EstornoChequeDevBoletoBancoBrasil,
            EstornoChequeDevBoletoOutros,

            DevolucaoPagtoDinheiro,
            DevolucaoPagtoCheque,
            DevolucaoPagtoCartao,
            DevolucaoPagtoDeposito,
            DevolucaoPagtoCredito,
            DevolucaoPagtoConstrucard,
            DevolucaoPagtoPermuta,
            DevolucaoPagtoBoleto,
            DevolucaoPagtoBoletoLumen,
            DevolucaoPagtoBoletoSantander,
            DevolucaoPagtoBoletoBancoBrasil,
            DevolucaoPagtoBoletoOutros,

            EstornoDevolucaoPagtoDinheiro,
            EstornoDevolucaoPagtoCheque,
            EstornoDevolucaoPagtoCartao,
            EstornoDevolucaoPagtoDeposito,
            EstornoDevolucaoPagtoCredito,
            EstornoDevolucaoPagtoConstrucard,
            EstornoDevolucaoPagtoPermuta,
            EstornoDevolucaoPagtoBoleto,
            EstornoDevolucaoPagtoBoletoLumen,
            EstornoDevolucaoPagtoBoletoSantander,
            EstornoDevolucaoPagtoBoletoBancoBrasil,
            EstornoDevolucaoPagtoBoletoOutros,

            PrazoBoleta,
            PrazoCartao,
            PrazoCheque,
            PrazoConstrucard,
            PrazoDeposito,
            PrazoPermuta,
            Prazo,

            VistaDinheiro,
            VistaDeposito,
            VistaCheque,
            VistaCartao,
            VistaConstrucard,
            VistaPermuta,
            VistaCredito,

            RecPrazoCheque,
            RecPrazoDinheiro,
            RecPrazoCartao,
            RecPrazoBoleto,
            RecPrazoConstrucard,
            RecPrazoDeposito,
            RecPrazoPermuta,
            RecPrazoCredito,
            RecPrazoBoletoLumen,
            RecPrazoBoletoSantander,
            RecPrazoBoletoBancoBrasil,
            RecPrazoBoletoOutros,

            RecChequeDevDinheiro,
            RecChequeDevDeposito,
            RecChequeDevCheque,
            RecChequeDevCartao,
            RecChequeDevBoleto,
            RecChequeDevConstrucard,
            RecChequeDevCredito,
            RecChequeDevPermuta,

            PagtoDinheiro,
            PagtoChequeProprio,
            PagtoChequeTerceiros,
            PagtoTransfBanco,
            PagtoBoleto,
            PagtoCreditoFornecedor,
            PagtoRenegociacao,
            PagtoPermuta,
            PagtoAntecipacaoFornecedor,

            EstornoPagtoDinheiro,
            EstornoPagtoChequeProprio,
            EstornoPagtoChequeTerceiros,
            EstornoPagtoTransfBancaria,
            EstornoPagtoPermuta,
            EstornoPagtoBoleto,
            EstornoPagtoCreditoFornecedor,
            EstornoPagtoAntecipacaoFornecedor,
            EstornoVenda,

            CreditoFornecDinheiro,
            CreditoFornecChequeProprio,
            CreditoFornecChequeTerceiros,
            CreditoFornecTransfBanco,
            CreditoFornecBoleto,
            CreditoFornecPermuta,

            EstornoCreditoFornecDinheiro,
            EstornoCreditoFornecChequeProprio,
            EstornoCreditoFornecChequeTerceiros,
            EstornoCreditoFornecTransfBancaria,
            EstornoCreditoFornecPermuta,
            EstornoCreditoFornecBoleto,

            TransfCaixaGeral,
            TransfDeCaixaGeralCheques,
            TransfBancoCheques,
            TransfCaixaGeralParaDiario,
            TransfContaBancaria,
            TransfDeCxDiarioDinheiro,
            TransfDeCxDiarioCheque,
            TransfDeCxDiarioCartao,
            TransfDeCxDiarioDeposito,
            TransfDeCxDiarioBoleto,
            TransfDeCxDiarioConstrucard,
            TransfDeCxDiarioPermuta,
            TransfParaCxGeralDinheiro,
            TransfParaCxGeralCheque,
            TransfContaBancariaParaCxGeralDinheiro,
            TransfCxGeralParaContaBancariaDinheiro,

            DepositoCheque,
            EstornoDepositoCheque,
            AntecipBoleto,
            EstornoAntecipBoleto,
            TaxaAntecipBoleto,
            ChequeDevolvido,
            ChequeTrocado,
            EstornoChequeTrocado,
            ChequeQuitado,

            CreditoVendaGerado,
            CreditoRecPrazoGerado,
            CreditoEntradaGerado,
            EstornoCreditoVendaGerado,
            EstornoCreditoRecPrazoGerado,
            EstornoCreditoEntradaGerado,
            CreditoCompraGerado,
            EstornoCreditoCompraGerado,

            JurosVendaCartao,
            JurosVendaConstrucard,
            ValorExcedente,
            SaldoRemanescente,
            Comerciais,
            Industriais,
            Frete,
            Salario,
            EstornoTaxaAntecipDepositoCheque,
            PagtoTaxaAntecipDepositoCheque,

            ValorRestantePagto,
            ParcelamentoObra,
            Renegociacao,
            PagamentoComisssao,
            TrocaDevolucao,

            CompraPrazoBoleto,
            CompraVistaBoleto,
            CompraPrazoCheque,
            CompraVistaCheque,
            CompraPrazoDeposito,
            CompraVistaDeposito,
            CompraPrazoDinheiro,
            CompraVistaDinheiro,

            RecObraCredito,
            EstornoRecObraCredito,
            CreditoObraGerado,
            CreditoObraEstorno,

            PagtoAntecipFornecBoleto,
            PagtoAntecipFornecChequePropio,
            PagtoAntecipFornecChequeTerceiros,
            PagtoAntecipFornecDeposito,
            PagtoAntecipFornecDinheiro,
            PagtoAntecipFornecPermuta,

            EstornoPagtoAntecipFornecBoleto,
            EstornoPagtoAntecipFornecChequePropio,
            EstornoPagtoAntecipFornecChequeTerceiros,
            EstornoPagtoAntecipFornecDeposito,
            EstornoPagtoAntecipFornecDinheiro,
            EstornoPagtoAntecipFornecPermuta,

            ParcelamentoPagtoAntecipFornec,

            CreditoAntecipFornecGerado,
            CreditoAntencipFornecEstorno,

            ValorExcedenteEncontroContas,

            DepositoNaoIdentificado
        }

        #endregion

        #region Variáveis Estáticas

        static uint _funcBoleto;
        static uint _funcCartao;
        static uint _funcDinheiro;
        static uint _funcCheque;
        static uint _funcDeposito;
        static uint _funcConstrucard;
        static uint _funcPermuta;
        static uint _funcPrazo;
        static uint _funcCredito;
        static uint _funcRecebimento;

        static uint _entradaCheque = 0;
        static uint _entradaDinheiro;
        static uint _entradaBoleto;
        static uint _entradaCartao;
        static uint _entradaConstrucard;
        static uint _entradaDeposito;
        static uint _entradaPermuta;
        static uint _entradaCredito;

        static uint _estornoBoleto;
        static uint _estornoCartao;
        static uint _estornoCheque;
        static uint _estornoDeposito;
        static uint _estornoDinheiro;
        static uint _estornoCredito;
        static uint _estornoPermuta;
        static uint _estornoConstrucard;
        static uint _estornoBoletoLumen;
        static uint _estornoBoletoSantander;
        static uint _estornoBoletoBancoBrasil;
        static uint _estornoBoletoOutros;
        static uint _estornoCreditoVendaGerado;
        static uint _estornoCreditoCompraGerado;
        static uint _estornoRetiradaDinheiro;
        static uint _estornoRetiradaCheque;
        static uint _estornoCreditoDinheiro;
        static uint _estornoCreditoCheque;
        static uint _estornoJurosVendaCartao;
        static uint _estornoJurosVendaConstrucard;

        static uint _estornoRecPrazoDinheiro;
        static uint _estornoRecPrazoCheque;
        static uint _estornoRecPrazoCartao;
        static uint _estornoRecPrazoDeposito;
        static uint _estornoRecPrazoCredito;
        static uint _estornoRecPrazoConstrucard;
        static uint _estornoRecPrazoPermuta;
        static uint _estornoRecPrazoBoleto;
        static uint _estornoRecPrazoBoletoLumen;
        static uint _estornoRecPrazoBoletoSantander;
        static uint _estornoRecPrazoBoletoBancoBrasil;
        static uint _estornoRecPrazoBoletoOutros;

        static uint _estornoEntradaDinheiro;
        static uint _estornoEntradaCheque;
        static uint _estornoEntradaCartao;
        static uint _estornoEntradaDeposito;
        static uint _estornoEntradaCredito;
        static uint _estornoEntradaConstrucard;
        static uint _estornoEntradaPermuta;
        static uint _estornoEntradaBoleto;
        static uint _estornoEntradaBoletoLumen;
        static uint _estornoEntradaBoletoSantander;
        static uint _estornoEntradaBoletoBancoBrasil;
        static uint _estornoEntradaBoletoOutros;

        static uint _estornoChequeDevDinheiro;
        static uint _estornoChequeDevCheque;
        static uint _estornoChequeDevCartao;
        static uint _estornoChequeDevBoleto;
        static uint _estornoChequeDevDeposito;
        static uint _estornoChequeDevCredito;
        static uint _estornoChequeDevConstrucard;
        static uint _estornoChequeDevPermuta;
        static uint _estornoChequeDevBoletoLumen;
        static uint _estornoChequeDevBoletoSantander;
        static uint _estornoChequeDevBoletoBancoBrasil;
        static uint _estornoChequeDevBoletoOutros;

        static uint _devolucaoPagtoDinheiro;
        static uint _devolucaoPagtoCheque;
        static uint _devolucaoPagtoCartao;
        static uint _devolucaoPagtoDeposito;
        static uint _devolucaoPagtoCredito;
        static uint _devolucaoPagtoConstrucard;
        static uint _devolucaoPagtoPermuta;
        static uint _devolucaoPagtoBoleto;
        static uint _devolucaoPagtoBoletoLumen;
        static uint _devolucaoPagtoBoletoSantander;
        static uint _devolucaoPagtoBoletoBancoBrasil;
        static uint _devolucaoPagtoBoletoOutros;

        static uint _estornoDevolucaoPagtoDinheiro;
        static uint _estornoDevolucaoPagtoCheque;
        static uint _estornoDevolucaoPagtoCartao;
        static uint _estornoDevolucaoPagtoDeposito;
        static uint _estornoDevolucaoPagtoCredito;
        static uint _estornoDevolucaoPagtoConstrucard;
        static uint _estornoDevolucaoPagtoPermuta;
        static uint _estornoDevolucaoPagtoBoleto;
        static uint _estornoDevolucaoPagtoBoletoLumen;
        static uint _estornoDevolucaoPagtoBoletoSantander;
        static uint _estornoDevolucaoPagtoBoletoBancoBrasil;
        static uint _estornoDevolucaoPagtoBoletoOutros;

        static uint _prazo;
        static uint _prazoCartao;
        static uint _prazoCheque;
        static uint _prazoConstrucard;
        static uint _prazoDeposito;
        static uint _prazoBoleta;
        static uint _prazoPermuta;

        static uint _vistaDinheiro;
        static uint _vistaCartao;
        static uint _vistaCheque;
        static uint _vistaConstrucard;
        static uint _vistaDeposito;
        static uint _vistaPermuta;
        static uint _vistaCredito;

        static uint _recPrazoBoleto;
        static uint _recPrazoCartao;
        static uint _recPrazoCheque;
        static uint _recPrazoConstrucard;
        static uint _recPrazoDinheiro;
        static uint _recPrazoDeposito;
        static uint _recPrazoPermuta;
        static uint _recPrazoCredito;
        static uint _recPrazoBoletoLumen;
        static uint _recPrazoBoletoSantander;
        static uint _recPrazoBoletoBancoBrasil;
        static uint _recPrazoBoletoOutros;

        static uint _depositoCheque;
        static uint _estornoDepositoCheque;
        static uint _antecipBoleto;
        static uint _estornoAntecipBoleto;
        static uint _taxaAntecipBoleto;
        static uint _chequeDevolvido;
        static uint _chequeTrocado;
        static uint _estornoChequeTrocado;
        static uint _chequeQuitado;

        static uint _recChequeDevDinheiro;
        static uint _recChequeDevDeposito;
        static uint _recChequeDevCheque;
        static uint _recChequeDevCartao;
        static uint _recChequeDevBoleto;
        static uint _recChequeDevConstrucard;
        static uint _recChequeDevCredito;
        static uint _recChequeDevPermuta;

        static uint _transfCaixaGeral;
        static uint _transfBancoCheques;
        static uint _transfDeCaixaGeralCheques;
        static uint _transfCaixaGeralParaDiario;
        static uint _transfContaBancaria;
        static uint _transfDeCxDiarioDinheiro;
        static uint _transfDeCxDiarioCheque;
        static uint _transfDeCxDiarioCartao;
        static uint _transfDeCxDiarioDeposito;
        static uint _transfDeCxDiarioBoleto;
        static uint _transfDeCxDiarioConstrucard;
        static uint _transfDeCxDiarioPermuta;
        static uint _transfParaCxGeralDinheiro;
        static uint _transfParaCxGeralCheque;
        static uint _transfContaBancariaParaCxGeralDinheiro;
        static uint _transfCxGeralParaContaBancariaDinheiro;

        static uint _pagtoDinheiro;
        static uint _pagtoChequeProprio;
        static uint _pagtoChequeTerceiros;
        static uint _pagtoBanco;
        static uint _pagtoBoleto;
        static uint _pagtoCreditoFornecedor;
        static uint _pagtoPermuta;
        static uint _pagtoRenegociacao;
        static uint _pagtoAntecipacaoFornecedor;

        static uint _estornoPagtoDinheiro;
        static uint _estornoPagtoChequeProprio;
        static uint _estornoPagtoChequeTerceiros;
        static uint _estornoPagtoBancario;
        static uint _estornoPagtoPermuta;
        static uint _estornoPagtoBoleto;
        static uint _estornoPagtoCreditoFornecedor;
        static uint _estornoPagtoAntecipacaoFornecedor;

        static uint _creditoFornecDinheiro;
        static uint _creditoFornecChequeProprio;
        static uint _creditoFornecChequeTerceiros;
        static uint _creditoFornecBanco;
        static uint _creditoFornecBoleto;
        static uint _creditoFornecPermuta;

        static uint _estornoCreditoFornecDinheiro;
        static uint _estornoCreditoFornecChequeProprio;
        static uint _estornoCreditoFornecChequeTerceiros;
        static uint _estornoCreditoFornecBancario;
        static uint _estornoCreditoFornecPermuta;
        static uint _estornoCreditoFornecBoleto;

        static uint _creditoVendaGerado;
        static uint _creditoRecPrazoGerado;
        static uint _creditoEntradaGerado;
        static uint _creditoCompraGerado;
        static uint _estornoCreditoRecPrazoGerado;
        static uint _estornoCreditoEntradaGerado;
        static uint _comerciais;
        static uint _industriais;
        static uint _salario = 0;
        static uint _saldoRemanescente;

        static uint _valorExcedente;
        static uint _jurosVendaCartao;
        static uint _jurosVendaConstrucard;
        static uint _estornoTaxaAntecipDepositoCheque;
        static uint _pagtoTaxaAntecipDepositoCheque;

        static uint _valorRestantePagto;
        static uint _parcelamentoObra;
        static uint _renegociacao;
        static uint _pagamentoComissao;
        static uint _trocaDevolucao;

        static uint _compraPrazoBoleto;
        static uint _compraVistaBoleto;
        static uint _compraPrazoCheque;
        static uint _compraVistaCheque;
        static uint _compraPrazoDeposito;
        static uint _compraVistaDeposito;
        static uint _compraPrazoDinheiro;
        static uint _compraVistaDinheiro;

        static uint _recObraCredito;
        static uint _estornoRecObraCredito;
        static uint _creditoObraGerado;
        static uint _creditoObraEstorno;

        static uint _pagtoAntecipFornecBoleto;
        static uint _pagtoAntecipFornecChequePropio;
        static uint _pagtoAntecipFornecChequeTerceiros;
        static uint _pagtoAntecipFornecDeposito;
        static uint _pagtoAntecipFornecDinheiro;
        static uint _pagtoAntecipFornecPermuta;

        static uint _estornoPagtoAntecipFornecBoleto;
        static uint _estornoPagtoAntecipFornecChequePropio;
        static uint _estornoPagtoAntecipFornecChequeTerceiros;
        static uint _estornoPagtoAntecipFornecDeposito;
        static uint _estornoPagtoAntecipFornecDinheiro;
        static uint _estornoPagtoAntecipFornecPermuta;

        static uint _parcelamentoPagtoAntecipFornec;

        static uint _creditoAntecipFornecGerado;
        static uint _creditoAntencipFornecEstorno;

        static uint _valorExcedenteEncontroContas;

        static uint _depositoNaoIdentificado;

        private static IList<TipoCartaoCredito> _contasCartoes;

        public static void initPlanoContas()
        {
            _funcBoleto = PlanoContasDAO.Instance.GetId(1, 7);
            _funcCartao = PlanoContasDAO.Instance.GetId(2, 7);
            _funcDinheiro = PlanoContasDAO.Instance.GetId(3, 7);
            _funcCheque = PlanoContasDAO.Instance.GetId(4, 7);
            _funcDeposito = PlanoContasDAO.Instance.GetId(5, 7);
            _funcConstrucard = PlanoContasDAO.Instance.GetId(6, 7);
            _funcPermuta = PlanoContasDAO.Instance.GetId(7, 7);
            _funcPrazo = PlanoContasDAO.Instance.GetId(8, 7);
            _funcCredito = PlanoContasDAO.Instance.GetId(9, 7);
            _funcRecebimento = PlanoContasDAO.Instance.GetId(16, 7);

            _entradaCheque = PlanoContasDAO.Instance.GetId(11, 51);
            _entradaBoleto = PlanoContasDAO.Instance.GetId(103, 51);
            _entradaDinheiro = PlanoContasDAO.Instance.GetId(10, 51);
            _entradaCartao = PlanoContasDAO.Instance.GetId(24, 51);
            _entradaConstrucard = PlanoContasDAO.Instance.GetId(25, 51);
            _entradaDeposito = PlanoContasDAO.Instance.GetId(26, 51);
            _entradaPermuta = PlanoContasDAO.Instance.GetId(27, 51);
            _entradaCredito = PlanoContasDAO.Instance.GetId(50, 51);

            _vistaDinheiro = PlanoContasDAO.Instance.GetId(1, 51);
            _vistaCartao = PlanoContasDAO.Instance.GetId(6, 51);
            _vistaCheque = PlanoContasDAO.Instance.GetId(4, 51);
            _vistaConstrucard = PlanoContasDAO.Instance.GetId(30, 51);
            _vistaDeposito = PlanoContasDAO.Instance.GetId(29, 51);
            _vistaPermuta = PlanoContasDAO.Instance.GetId(28, 51);
            _vistaCredito = PlanoContasDAO.Instance.GetId(49, 51);

            _recPrazoBoleto = PlanoContasDAO.Instance.GetId(34, 50);
            _recPrazoCartao = PlanoContasDAO.Instance.GetId(33, 50);
            _recPrazoCheque = PlanoContasDAO.Instance.GetId(5, 50);
            _recPrazoConstrucard = PlanoContasDAO.Instance.GetId(35, 50);
            _recPrazoDinheiro = PlanoContasDAO.Instance.GetId(3, 50);
            _recPrazoDeposito = PlanoContasDAO.Instance.GetId(36, 50);
            _recPrazoPermuta = PlanoContasDAO.Instance.GetId(37, 50);
            _recPrazoCredito = PlanoContasDAO.Instance.GetId(51, 50);
            _recPrazoBoletoSantander = PlanoContasDAO.Instance.GetId(86, 50);
            _recPrazoBoletoBancoBrasil = PlanoContasDAO.Instance.GetId(87, 50);
            _recPrazoBoletoOutros = PlanoContasDAO.Instance.GetId(88, 50);

            _prazo = PlanoContasDAO.Instance.GetId(31, 50);
            _prazoCartao = PlanoContasDAO.Instance.GetId(17, 50);
            _prazoCheque = PlanoContasDAO.Instance.GetId(2, 50);
            _prazoConstrucard = PlanoContasDAO.Instance.GetId(188, 50);
            _prazoDeposito = PlanoContasDAO.Instance.GetId(19, 50);
            _prazoBoleta = PlanoContasDAO.Instance.GetId(32, 50);
            _prazoPermuta = PlanoContasDAO.Instance.GetId(54, 50);

            _depositoNaoIdentificado = PlanoContasDAO.Instance.GetId(190, 50);

            _recChequeDevDinheiro = PlanoContasDAO.Instance.GetId(45, 48);
            _recChequeDevDeposito = PlanoContasDAO.Instance.GetId(46, 48);
            _recChequeDevCheque = PlanoContasDAO.Instance.GetId(47, 48);
            _recChequeDevCartao = PlanoContasDAO.Instance.GetId(48, 48);
            _recChequeDevBoleto = PlanoContasDAO.Instance.GetId(127, 48);
            _recChequeDevConstrucard = PlanoContasDAO.Instance.GetId(94, 48);
            _recChequeDevCredito = PlanoContasDAO.Instance.GetId(126, 48);
            _recChequeDevPermuta = PlanoContasDAO.Instance.GetId(7, 48);

            _estornoBoleto = PlanoContasDAO.Instance.GetId(84, 49);
            _estornoCartao = PlanoContasDAO.Instance.GetId(21, 49);
            _estornoCheque = PlanoContasDAO.Instance.GetId(13, 49);
            _estornoDeposito = PlanoContasDAO.Instance.GetId(23, 49);
            _estornoDinheiro = PlanoContasDAO.Instance.GetId(12, 49);
            _estornoCredito = PlanoContasDAO.Instance.GetId(52, 49);
            _estornoPermuta = PlanoContasDAO.Instance.GetId(83, 49);
            _estornoConstrucard = PlanoContasDAO.Instance.GetId(82, 49);

            _estornoBoletoSantander = PlanoContasDAO.Instance.GetId(90, 49);
            _estornoBoletoBancoBrasil = PlanoContasDAO.Instance.GetId(91, 49);
            _estornoBoletoOutros = PlanoContasDAO.Instance.GetId(92, 49);
            _estornoCreditoVendaGerado = PlanoContasDAO.Instance.GetId(93, 5);
            _estornoCreditoRecPrazoGerado = PlanoContasDAO.Instance.GetId(139, 5);
            _estornoCreditoEntradaGerado = PlanoContasDAO.Instance.GetId(140, 5);
            _estornoCreditoCompraGerado = PlanoContasDAO.Instance.GetId(97, 5);
            _estornoRetiradaDinheiro = PlanoContasDAO.Instance.GetId(99, 5);
            _estornoRetiradaCheque = PlanoContasDAO.Instance.GetId(100, 5);
            _estornoCreditoDinheiro = PlanoContasDAO.Instance.GetId(177, 5);
            _estornoCreditoCheque = PlanoContasDAO.Instance.GetId(178, 5);
            _estornoJurosVendaCartao = PlanoContasDAO.Instance.GetId(102, 5);
            _estornoJurosVendaConstrucard = PlanoContasDAO.Instance.GetId(143, 5);

            _estornoRecPrazoDinheiro = PlanoContasDAO.Instance.GetId(132, 49);
            _estornoRecPrazoCheque = PlanoContasDAO.Instance.GetId(133, 49);
            _estornoRecPrazoCartao = PlanoContasDAO.Instance.GetId(134, 49);
            _estornoRecPrazoDeposito = PlanoContasDAO.Instance.GetId(135, 49);
            _estornoRecPrazoCredito = PlanoContasDAO.Instance.GetId(136, 49);
            _estornoRecPrazoConstrucard = PlanoContasDAO.Instance.GetId(143, 49);
            _estornoRecPrazoPermuta = PlanoContasDAO.Instance.GetId(144, 49);
            _estornoRecPrazoBoleto = PlanoContasDAO.Instance.GetId(145, 49);
            _estornoRecPrazoBoletoSantander = PlanoContasDAO.Instance.GetId(147, 49);
            _estornoRecPrazoBoletoBancoBrasil = PlanoContasDAO.Instance.GetId(148, 49);
            _estornoRecPrazoBoletoOutros = PlanoContasDAO.Instance.GetId(149, 49);

            _estornoEntradaDinheiro = PlanoContasDAO.Instance.GetId(150, 49);
            _estornoEntradaCheque = PlanoContasDAO.Instance.GetId(151, 49);
            _estornoEntradaCartao = PlanoContasDAO.Instance.GetId(152, 49);
            _estornoEntradaDeposito = PlanoContasDAO.Instance.GetId(153, 49);
            _estornoEntradaCredito = PlanoContasDAO.Instance.GetId(154, 49);
            _estornoEntradaConstrucard = PlanoContasDAO.Instance.GetId(161, 49);
            _estornoEntradaPermuta = PlanoContasDAO.Instance.GetId(162, 49);
            _estornoEntradaBoleto = PlanoContasDAO.Instance.GetId(163, 49);
            _estornoEntradaBoletoSantander = PlanoContasDAO.Instance.GetId(165, 49);
            _estornoEntradaBoletoBancoBrasil = PlanoContasDAO.Instance.GetId(166, 49);
            _estornoEntradaBoletoOutros = PlanoContasDAO.Instance.GetId(167, 49);

            _estornoChequeDevDinheiro = PlanoContasDAO.Instance.GetId(170, 49);
            _estornoChequeDevCheque = PlanoContasDAO.Instance.GetId(171, 49);
            _estornoChequeDevCartao = PlanoContasDAO.Instance.GetId(172, 49);
            _estornoChequeDevBoleto = PlanoContasDAO.Instance.GetId(183, 49);
            _estornoChequeDevDeposito = PlanoContasDAO.Instance.GetId(173, 49);
            _estornoChequeDevCredito = PlanoContasDAO.Instance.GetId(174, 49);
            _estornoChequeDevConstrucard = PlanoContasDAO.Instance.GetId(181, 49);
            _estornoChequeDevPermuta = PlanoContasDAO.Instance.GetId(182, 49);
            _estornoChequeDevBoletoSantander = PlanoContasDAO.Instance.GetId(185, 49);
            _estornoChequeDevBoletoBancoBrasil = PlanoContasDAO.Instance.GetId(186, 49);
            _estornoChequeDevBoletoOutros = PlanoContasDAO.Instance.GetId(187, 49);

            _devolucaoPagtoDinheiro = PlanoContasDAO.Instance.GetId(188, 49);
            _devolucaoPagtoCheque = PlanoContasDAO.Instance.GetId(189, 49);
            _devolucaoPagtoCartao = PlanoContasDAO.Instance.GetId(190, 49);
            _devolucaoPagtoDeposito = PlanoContasDAO.Instance.GetId(191, 49);
            _devolucaoPagtoCredito = PlanoContasDAO.Instance.GetId(192, 49);
            _devolucaoPagtoConstrucard = PlanoContasDAO.Instance.GetId(199, 49);
            _devolucaoPagtoPermuta = PlanoContasDAO.Instance.GetId(200, 49);
            _devolucaoPagtoBoleto = PlanoContasDAO.Instance.GetId(201, 49);
            _devolucaoPagtoBoletoSantander = PlanoContasDAO.Instance.GetId(203, 49);
            _devolucaoPagtoBoletoBancoBrasil = PlanoContasDAO.Instance.GetId(204, 49);
            _devolucaoPagtoBoletoOutros = PlanoContasDAO.Instance.GetId(205, 49);

            _estornoDevolucaoPagtoDinheiro = PlanoContasDAO.Instance.GetId(104, 51);
            _estornoDevolucaoPagtoCheque = PlanoContasDAO.Instance.GetId(105, 51);
            _estornoDevolucaoPagtoCartao = PlanoContasDAO.Instance.GetId(106, 51);
            _estornoDevolucaoPagtoDeposito = PlanoContasDAO.Instance.GetId(107, 51);
            _estornoDevolucaoPagtoCredito = PlanoContasDAO.Instance.GetId(108, 51);
            _estornoDevolucaoPagtoConstrucard = PlanoContasDAO.Instance.GetId(115, 51);
            _estornoDevolucaoPagtoPermuta = PlanoContasDAO.Instance.GetId(116, 51);
            _estornoDevolucaoPagtoBoleto = PlanoContasDAO.Instance.GetId(117, 51);
            _estornoDevolucaoPagtoBoletoSantander = PlanoContasDAO.Instance.GetId(119, 51);
            _estornoDevolucaoPagtoBoletoBancoBrasil = PlanoContasDAO.Instance.GetId(120, 51);
            _estornoDevolucaoPagtoBoletoOutros = PlanoContasDAO.Instance.GetId(121, 51);

            _depositoCheque = PlanoContasDAO.Instance.GetId(42, 5);
            _estornoDepositoCheque = PlanoContasDAO.Instance.GetId(43, 5);
            _antecipBoleto = PlanoContasDAO.Instance.GetId(133, 5);
            _estornoAntecipBoleto = PlanoContasDAO.Instance.GetId(134, 5);
            _taxaAntecipBoleto = PlanoContasDAO.Instance.GetId(135, 5);
            _chequeDevolvido = PlanoContasDAO.Instance.GetId(44, 5);
            _chequeTrocado = PlanoContasDAO.Instance.GetId(122, 5);
            _estornoChequeTrocado = PlanoContasDAO.Instance.GetId(141, 5);
            _chequeQuitado = PlanoContasDAO.Instance.GetId(123, 5);

            _transfCaixaGeral = PlanoContasDAO.Instance.GetId(4, 5);
            _transfBancoCheques = PlanoContasDAO.Instance.GetId(5, 5);
            _transfDeCaixaGeralCheques = PlanoContasDAO.Instance.GetId(8, 5);
            _transfCaixaGeralParaDiario = PlanoContasDAO.Instance.GetId(7, 5);
            _transfContaBancaria = PlanoContasDAO.Instance.GetId(98, 5);
            _transfDeCxDiarioDinheiro = PlanoContasDAO.Instance.GetId(6, 5);
            _transfDeCxDiarioCheque = PlanoContasDAO.Instance.GetId(9, 5);
            _transfDeCxDiarioCartao = PlanoContasDAO.Instance.GetId(127, 5);
            _transfDeCxDiarioDeposito = PlanoContasDAO.Instance.GetId(128, 5);
            _transfDeCxDiarioBoleto = PlanoContasDAO.Instance.GetId(129, 5);
            _transfDeCxDiarioConstrucard = PlanoContasDAO.Instance.GetId(130, 5);
            _transfDeCxDiarioPermuta = PlanoContasDAO.Instance.GetId(131, 5);
            _transfParaCxGeralDinheiro = PlanoContasDAO.Instance.GetId(11, 5);
            _transfParaCxGeralCheque = PlanoContasDAO.Instance.GetId(12, 5);
            _transfContaBancariaParaCxGeralDinheiro = PlanoContasDAO.Instance.GetId(108, 5);
            _transfCxGeralParaContaBancariaDinheiro = PlanoContasDAO.Instance.GetId(118, 5);

            _pagtoDinheiro = PlanoContasDAO.Instance.GetId(38, 5);
            _pagtoChequeProprio = PlanoContasDAO.Instance.GetId(39, 5);
            _pagtoChequeTerceiros = PlanoContasDAO.Instance.GetId(40, 5);
            _pagtoBanco = PlanoContasDAO.Instance.GetId(55, 5);
            _pagtoBoleto = PlanoContasDAO.Instance.GetId(80, 5);
            _pagtoCreditoFornecedor = PlanoContasDAO.Instance.GetId(95, 5);
            _pagtoPermuta = PlanoContasDAO.Instance.GetId(144, 5);
            _pagtoRenegociacao = PlanoContasDAO.Instance.GetId(145, 5);
            _pagtoAntecipacaoFornecedor = PlanoContasDAO.Instance.GetId(179, 5);

            _estornoPagtoDinheiro = PlanoContasDAO.Instance.GetId(41, 5);
            _estornoPagtoChequeProprio = PlanoContasDAO.Instance.GetId(107, 5);
            _estornoPagtoChequeTerceiros = PlanoContasDAO.Instance.GetId(121, 5);
            _estornoPagtoBancario = PlanoContasDAO.Instance.GetId(109, 5);
            _estornoPagtoPermuta = PlanoContasDAO.Instance.GetId(146, 5);
            _estornoPagtoBoleto = PlanoContasDAO.Instance.GetId(147, 5);
            _estornoPagtoCreditoFornecedor = PlanoContasDAO.Instance.GetId(160, 5);
            _estornoPagtoAntecipacaoFornecedor = PlanoContasDAO.Instance.GetId(180, 5);

            _creditoFornecDinheiro = PlanoContasDAO.Instance.GetId(148, 46);
            _creditoFornecChequeProprio = PlanoContasDAO.Instance.GetId(149, 46);
            _creditoFornecChequeTerceiros = PlanoContasDAO.Instance.GetId(150, 46);
            _creditoFornecBanco = PlanoContasDAO.Instance.GetId(151, 46);
            _creditoFornecBoleto = PlanoContasDAO.Instance.GetId(152, 46);
            _creditoFornecPermuta = PlanoContasDAO.Instance.GetId(153, 46);

            _estornoCreditoFornecDinheiro = PlanoContasDAO.Instance.GetId(154, 47);
            _estornoCreditoFornecChequeProprio = PlanoContasDAO.Instance.GetId(155, 47);
            _estornoCreditoFornecChequeTerceiros = PlanoContasDAO.Instance.GetId(156, 47);
            _estornoCreditoFornecBancario = PlanoContasDAO.Instance.GetId(157, 47);
            _estornoCreditoFornecBoleto = PlanoContasDAO.Instance.GetId(158, 47);
            _estornoCreditoFornecPermuta = PlanoContasDAO.Instance.GetId(159, 47);

            _valorExcedente = PlanoContasDAO.Instance.GetId(189, 50);
            _creditoVendaGerado = PlanoContasDAO.Instance.GetId(132, 5);
            _creditoRecPrazoGerado = PlanoContasDAO.Instance.GetId(137, 5);
            _creditoEntradaGerado = PlanoContasDAO.Instance.GetId(138, 5);
            _creditoCompraGerado = PlanoContasDAO.Instance.GetId(96, 5);
            _comerciais = PlanoContasDAO.Instance.GetId(1, 8);
            _industriais = PlanoContasDAO.Instance.GetId(2, 8);
            _saldoRemanescente = PlanoContasDAO.Instance.GetId(20, 5);

            _jurosVendaCartao = PlanoContasDAO.Instance.GetId(101, 5);
            _jurosVendaConstrucard = PlanoContasDAO.Instance.GetId(142, 5);
            _estornoTaxaAntecipDepositoCheque = PlanoContasDAO.Instance.GetId(124, 5);
            _pagtoTaxaAntecipDepositoCheque = PlanoContasDAO.Instance.GetId(125, 5);

            _parcelamentoObra = PlanoContasDAO.Instance.GetId(104, 5);
            _renegociacao = PlanoContasDAO.Instance.GetId(105, 5);
            _valorRestantePagto = PlanoContasDAO.Instance.GetId(106, 5);
            _pagamentoComissao = PlanoContasDAO.Instance.GetId(136, 5);
            _trocaDevolucao = PlanoContasDAO.Instance.GetId(122, 51);

            _compraPrazoBoleto = PlanoContasDAO.Instance.GetId(110, 5);
            _compraVistaBoleto = PlanoContasDAO.Instance.GetId(111, 5);
            _compraPrazoCheque = PlanoContasDAO.Instance.GetId(112, 5);
            _compraVistaCheque = PlanoContasDAO.Instance.GetId(113, 5);
            _compraPrazoDeposito = PlanoContasDAO.Instance.GetId(114, 5);
            _compraVistaDeposito = PlanoContasDAO.Instance.GetId(115, 5);
            _compraPrazoDinheiro = PlanoContasDAO.Instance.GetId(116, 5);
            _compraVistaDinheiro = PlanoContasDAO.Instance.GetId(117, 5);

            _recObraCredito = PlanoContasDAO.Instance.GetId(181, 5);
            _estornoRecObraCredito = PlanoContasDAO.Instance.GetId(182, 5);
            _creditoObraGerado = PlanoContasDAO.Instance.GetId(119, 5);
            _creditoObraEstorno = PlanoContasDAO.Instance.GetId(120, 5);

            _pagtoAntecipFornecBoleto = PlanoContasDAO.Instance.GetId(161, 5);
            _pagtoAntecipFornecChequePropio = PlanoContasDAO.Instance.GetId(162, 5);
            _pagtoAntecipFornecChequeTerceiros = PlanoContasDAO.Instance.GetId(163, 5);
            _pagtoAntecipFornecDeposito = PlanoContasDAO.Instance.GetId(164, 5);
            _pagtoAntecipFornecDinheiro = PlanoContasDAO.Instance.GetId(165, 5);
            _pagtoAntecipFornecPermuta = PlanoContasDAO.Instance.GetId(166, 5);

            _estornoPagtoAntecipFornecBoleto = PlanoContasDAO.Instance.GetId(167, 5);
            _estornoPagtoAntecipFornecChequePropio = PlanoContasDAO.Instance.GetId(168, 5);
            _estornoPagtoAntecipFornecChequeTerceiros = PlanoContasDAO.Instance.GetId(169, 5);
            _estornoPagtoAntecipFornecDeposito = PlanoContasDAO.Instance.GetId(170, 5);
            _estornoPagtoAntecipFornecDinheiro = PlanoContasDAO.Instance.GetId(171, 5);
            _estornoPagtoAntecipFornecPermuta = PlanoContasDAO.Instance.GetId(172, 5);

            _parcelamentoPagtoAntecipFornec = PlanoContasDAO.Instance.GetId(173, 5);

            _creditoAntecipFornecGerado = PlanoContasDAO.Instance.GetId(174, 5);
            _creditoAntencipFornecEstorno = PlanoContasDAO.Instance.GetId(175, 5);

            // Se adicionar algum plano de conta novo abaixo deste, será necessário mudar o plano de conta 
            // que verifica se todos foram carregados no método ObtemPlanoConta(PlanoContas plano)
            _valorExcedenteEncontroContas = PlanoContasDAO.Instance.GetId(176, 5);

            if (Configuracoes.FinanceiroConfig.UsarPlanoContaBoletoLumen)
            {
                _recPrazoBoletoLumen = PlanoContasDAO.Instance.GetId(85, 50);
                _estornoDevolucaoPagtoBoletoLumen = PlanoContasDAO.Instance.GetId(118, 51);
                _devolucaoPagtoBoletoLumen = PlanoContasDAO.Instance.GetId(202, 49);
                _estornoBoletoLumen = PlanoContasDAO.Instance.GetId(89, 49);
                _estornoChequeDevBoletoLumen = PlanoContasDAO.Instance.GetId(184, 49);
                _estornoEntradaBoletoLumen = PlanoContasDAO.Instance.GetId(164, 49);
                _estornoRecPrazoBoletoLumen = PlanoContasDAO.Instance.GetId(146, 49);
            }
        }

        #endregion

        #region Retorna idConta a partir de enumerator

        /// <summary>
        /// Retorna o código do plano de conta informado
        /// </summary>
        /// <param name="planoConta"></param>
        /// <returns></returns>
        public static uint GetPlanoConta(PlanoContas plano)
        {
            try
            {
                // Verifica se todos os planos de conta foram carregados, caso não tenham sido, carrega novamente.
                if (_valorExcedenteEncontroContas == 0)
                    initPlanoContas();

                switch (plano)
                {
                    // Vendas para funcionários
                    case PlanoContas.FuncBoleto: return _funcBoleto;
                    case PlanoContas.FuncCartao: return _funcCartao;
                    case PlanoContas.FuncDinheiro: return _funcDinheiro;
                    case PlanoContas.FuncCheque: return _funcCheque;
                    case PlanoContas.FuncDeposito: return _funcDeposito;
                    case PlanoContas.FuncConstrucard: return _funcConstrucard;
                    case PlanoContas.FuncPermuta: return _funcPermuta;
                    case PlanoContas.FuncPrazo: return _funcPrazo;
                    case PlanoContas.FuncCredito: return _funcCredito;
                    case PlanoContas.FuncRecebimento: return _funcRecebimento;

                    // Faturamento sobre vendas
                    case PlanoContas.EntradaCheque: return _entradaCheque;
                    case PlanoContas.EntradaBoleto: return _entradaBoleto;
                    case PlanoContas.EntradaDinheiro: return _entradaDinheiro;
                    case PlanoContas.EntradaCartao: return _entradaCartao;
                    case PlanoContas.EntradaConstrucard: return _entradaConstrucard;
                    case PlanoContas.EntradaDeposito: return _entradaDeposito;
                    case PlanoContas.EntradaPermuta: return _entradaPermuta;
                    case PlanoContas.EntradaCredito: return _entradaCredito;

                    case PlanoContas.EstornoBoleto: return _estornoBoleto;
                    case PlanoContas.EstornoCartao: return _estornoCartao;
                    case PlanoContas.EstornoCheque: return _estornoCheque;
                    case PlanoContas.EstornoDeposito: return _estornoDeposito;
                    case PlanoContas.EstornoDinheiro: return _estornoDinheiro;
                    case PlanoContas.EstornoCredito: return _estornoCredito;
                    case PlanoContas.EstornoPermuta: return _estornoPermuta;
                    case PlanoContas.EstornoConstrucard: return _estornoConstrucard;
                    case PlanoContas.EstornoBoletoLumen: return _estornoBoletoLumen;
                    case PlanoContas.EstornoBoletoSantander: return _estornoBoletoSantander;
                    case PlanoContas.EstornoBoletoBancoBrasil: return _estornoBoletoBancoBrasil;
                    case PlanoContas.EstornoBoletoOutros: return _estornoBoletoOutros;
                    case PlanoContas.EstornoRetiradaDinheiro: return _estornoRetiradaDinheiro;
                    case PlanoContas.EstornoRetiradaCheque: return _estornoRetiradaCheque;
                    case PlanoContas.EstornoCreditoDinheiro: return _estornoCreditoDinheiro;
                    case PlanoContas.EstornoCreditoCheque: return _estornoCreditoCheque;
                    case PlanoContas.EstornoJurosVendaCartao: return _estornoJurosVendaCartao;
                    case PlanoContas.EstornoJurosVendaConstrucard: return _estornoJurosVendaConstrucard;

                    case PlanoContas.EstornoRecPrazoDinheiro: return _estornoRecPrazoDinheiro;
                    case PlanoContas.EstornoRecPrazoCheque: return _estornoRecPrazoCheque;
                    case PlanoContas.EstornoRecPrazoCartao: return _estornoRecPrazoCartao;
                    case PlanoContas.EstornoRecPrazoDeposito: return _estornoRecPrazoDeposito;
                    case PlanoContas.EstornoRecPrazoCredito: return _estornoRecPrazoCredito;
                    case PlanoContas.EstornoRecPrazoConstrucard: return _estornoRecPrazoConstrucard;
                    case PlanoContas.EstornoRecPrazoPermuta: return _estornoRecPrazoPermuta;
                    case PlanoContas.EstornoRecPrazoBoleto: return _estornoRecPrazoBoleto;
                    case PlanoContas.EstornoRecPrazoBoletoLumen: return _estornoRecPrazoBoletoLumen;
                    case PlanoContas.EstornoRecPrazoBoletoSantander: return _estornoRecPrazoBoletoSantander;
                    case PlanoContas.EstornoRecPrazoBoletoBancoBrasil: return _estornoRecPrazoBoletoBancoBrasil;
                    case PlanoContas.EstornoRecPrazoBoletoOutros: return _estornoRecPrazoBoletoOutros;

                    case PlanoContas.EstornoEntradaDinheiro: return _estornoEntradaDinheiro;
                    case PlanoContas.EstornoEntradaCheque: return _estornoEntradaCheque;
                    case PlanoContas.EstornoEntradaCartao: return _estornoEntradaCartao;
                    case PlanoContas.EstornoEntradaDeposito: return _estornoEntradaDeposito;
                    case PlanoContas.EstornoEntradaCredito: return _estornoEntradaCredito;
                    case PlanoContas.EstornoEntradaConstrucard: return _estornoEntradaConstrucard;
                    case PlanoContas.EstornoEntradaPermuta: return _estornoEntradaPermuta;
                    case PlanoContas.EstornoEntradaBoleto: return _estornoEntradaBoleto;
                    case PlanoContas.EstornoEntradaBoletoLumen: return _estornoEntradaBoletoLumen;
                    case PlanoContas.EstornoEntradaBoletoSantander: return _estornoEntradaBoletoSantander;
                    case PlanoContas.EstornoEntradaBoletoBancoBrasil: return _estornoEntradaBoletoBancoBrasil;
                    case PlanoContas.EstornoEntradaBoletoOutros: return _estornoEntradaBoletoOutros;

                    case PlanoContas.EstornoChequeDevDinheiro: return _estornoChequeDevDinheiro;
                    case PlanoContas.EstornoChequeDevCheque: return _estornoChequeDevCheque;
                    case PlanoContas.EstornoChequeDevCartao: return _estornoChequeDevCartao;
                    case PlanoContas.EstornoChequeDevBoleto: return _estornoChequeDevBoleto;
                    case PlanoContas.EstornoChequeDevDeposito: return _estornoChequeDevDeposito;
                    case PlanoContas.EstornoChequeDevCredito: return _estornoChequeDevCredito;
                    case PlanoContas.EstornoChequeDevConstrucard: return _estornoChequeDevConstrucard;
                    case PlanoContas.EstornoChequeDevPermuta: return _estornoChequeDevPermuta;
                    case PlanoContas.EstornoChequeDevBoletoLumen: return _estornoChequeDevBoletoLumen;
                    case PlanoContas.EstornoChequeDevBoletoSantander: return _estornoChequeDevBoletoSantander;
                    case PlanoContas.EstornoChequeDevBoletoBancoBrasil: return _estornoChequeDevBoletoBancoBrasil;
                    case PlanoContas.EstornoChequeDevBoletoOutros: return _estornoChequeDevBoletoOutros;

                    case PlanoContas.DevolucaoPagtoDinheiro: return _devolucaoPagtoDinheiro;
                    case PlanoContas.DevolucaoPagtoCheque: return _devolucaoPagtoCheque;
                    case PlanoContas.DevolucaoPagtoCartao: return _devolucaoPagtoCartao;
                    case PlanoContas.DevolucaoPagtoDeposito: return _devolucaoPagtoDeposito;
                    case PlanoContas.DevolucaoPagtoCredito: return _devolucaoPagtoCredito;
                    case PlanoContas.DevolucaoPagtoConstrucard: return _devolucaoPagtoConstrucard;
                    case PlanoContas.DevolucaoPagtoPermuta: return _devolucaoPagtoPermuta;
                    case PlanoContas.DevolucaoPagtoBoleto: return _devolucaoPagtoBoleto;
                    case PlanoContas.DevolucaoPagtoBoletoLumen: return _devolucaoPagtoBoletoLumen;
                    case PlanoContas.DevolucaoPagtoBoletoSantander: return _devolucaoPagtoBoletoSantander;
                    case PlanoContas.DevolucaoPagtoBoletoBancoBrasil: return _devolucaoPagtoBoletoBancoBrasil;
                    case PlanoContas.DevolucaoPagtoBoletoOutros: return _devolucaoPagtoBoletoOutros;

                    case PlanoContas.EstornoDevolucaoPagtoDinheiro: return _estornoDevolucaoPagtoDinheiro;
                    case PlanoContas.EstornoDevolucaoPagtoCheque: return _estornoDevolucaoPagtoCheque;
                    case PlanoContas.EstornoDevolucaoPagtoCartao: return _estornoDevolucaoPagtoCartao;
                    case PlanoContas.EstornoDevolucaoPagtoDeposito: return _estornoDevolucaoPagtoDeposito;
                    case PlanoContas.EstornoDevolucaoPagtoCredito: return _estornoDevolucaoPagtoCredito;
                    case PlanoContas.EstornoDevolucaoPagtoConstrucard: return _estornoDevolucaoPagtoConstrucard;
                    case PlanoContas.EstornoDevolucaoPagtoPermuta: return _estornoDevolucaoPagtoPermuta;
                    case PlanoContas.EstornoDevolucaoPagtoBoleto: return _estornoDevolucaoPagtoBoleto;
                    case PlanoContas.EstornoDevolucaoPagtoBoletoLumen: return _estornoDevolucaoPagtoBoletoLumen;
                    case PlanoContas.EstornoDevolucaoPagtoBoletoSantander: return _estornoDevolucaoPagtoBoletoSantander;
                    case PlanoContas.EstornoDevolucaoPagtoBoletoBancoBrasil: return _estornoDevolucaoPagtoBoletoBancoBrasil;
                    case PlanoContas.EstornoDevolucaoPagtoBoletoOutros: return _estornoDevolucaoPagtoBoletoOutros;

                    case PlanoContas.Prazo: return _prazo;
                    case PlanoContas.PrazoCartao: return _prazoCartao;
                    case PlanoContas.PrazoCheque: return _prazoCheque;
                    case PlanoContas.PrazoConstrucard: return _prazoConstrucard;
                    case PlanoContas.PrazoDeposito: return _prazoDeposito;
                    case PlanoContas.PrazoBoleta: return _prazoBoleta;
                    case PlanoContas.PrazoPermuta: return _prazoPermuta;

                    case PlanoContas.VistaDinheiro: return _vistaDinheiro;
                    case PlanoContas.VistaCartao: return _vistaCartao;
                    case PlanoContas.VistaCheque: return _vistaCheque;
                    case PlanoContas.VistaConstrucard: return _vistaConstrucard;
                    case PlanoContas.VistaDeposito: return _vistaDeposito;
                    case PlanoContas.VistaPermuta: return _vistaPermuta;
                    case PlanoContas.VistaCredito: return _vistaCredito;

                    case PlanoContas.RecPrazoBoleto: return _recPrazoBoleto;
                    case PlanoContas.RecPrazoCartao: return _recPrazoCartao;
                    case PlanoContas.RecPrazoCheque: return _recPrazoCheque;
                    case PlanoContas.RecPrazoConstrucard: return _recPrazoConstrucard;
                    case PlanoContas.RecPrazoDinheiro: return _recPrazoDinheiro;
                    case PlanoContas.RecPrazoDeposito: return _recPrazoDeposito;
                    case PlanoContas.RecPrazoPermuta: return _recPrazoPermuta;
                    case PlanoContas.RecPrazoCredito: return _recPrazoCredito;
                    case PlanoContas.RecPrazoBoletoLumen: return _recPrazoBoletoLumen;
                    case PlanoContas.RecPrazoBoletoSantander: return _recPrazoBoletoSantander;
                    case PlanoContas.RecPrazoBoletoBancoBrasil: return _recPrazoBoletoBancoBrasil;
                    case PlanoContas.RecPrazoBoletoOutros: return _recPrazoBoletoOutros;

                    case PlanoContas.DepositoCheque: return _depositoCheque;
                    case PlanoContas.EstornoDepositoCheque: return _estornoDepositoCheque;
                    case PlanoContas.AntecipBoleto: return _antecipBoleto;
                    case PlanoContas.EstornoAntecipBoleto: return _estornoAntecipBoleto;
                    case PlanoContas.TaxaAntecipBoleto: return _taxaAntecipBoleto;
                    case PlanoContas.ChequeDevolvido: return _chequeDevolvido;
                    case PlanoContas.ChequeTrocado: return _chequeTrocado;
                    case PlanoContas.EstornoChequeTrocado: return _estornoChequeTrocado;
                    case PlanoContas.ChequeQuitado: return _chequeQuitado;

                    case PlanoContas.RecChequeDevDinheiro: return _recChequeDevDinheiro;
                    case PlanoContas.RecChequeDevDeposito: return _recChequeDevDeposito;
                    case PlanoContas.RecChequeDevCheque: return _recChequeDevCheque;
                    case PlanoContas.RecChequeDevCartao: return _recChequeDevCartao;
                    case PlanoContas.RecChequeDevBoleto: return _recChequeDevBoleto;
                    case PlanoContas.RecChequeDevConstrucard: return _recChequeDevConstrucard;
                    case PlanoContas.RecChequeDevCredito: return _recChequeDevCredito;
                    case PlanoContas.RecChequeDevPermuta: return _recChequeDevPermuta;

                    case PlanoContas.TransfCaixaGeral: return _transfCaixaGeral;
                    case PlanoContas.TransfBancoCheques: return _transfBancoCheques;
                    case PlanoContas.TransfDeCaixaGeralCheques: return _transfDeCaixaGeralCheques;
                    case PlanoContas.TransfCaixaGeralParaDiario: return _transfCaixaGeralParaDiario;
                    case PlanoContas.TransfContaBancaria: return _transfContaBancaria;
                    case PlanoContas.TransfDeCxDiarioDinheiro: return _transfDeCxDiarioDinheiro;
                    case PlanoContas.TransfDeCxDiarioCheque: return _transfDeCxDiarioCheque;
                    case PlanoContas.TransfDeCxDiarioCartao: return _transfDeCxDiarioCartao;
                    case PlanoContas.TransfDeCxDiarioDeposito: return _transfDeCxDiarioDeposito;
                    case PlanoContas.TransfDeCxDiarioBoleto: return _transfDeCxDiarioBoleto;
                    case PlanoContas.TransfDeCxDiarioConstrucard: return _transfDeCxDiarioConstrucard;
                    case PlanoContas.TransfDeCxDiarioPermuta: return _transfDeCxDiarioPermuta;
                    case PlanoContas.TransfParaCxGeralDinheiro: return _transfParaCxGeralDinheiro;
                    case PlanoContas.TransfParaCxGeralCheque: return _transfParaCxGeralCheque;
                    case PlanoContas.TransfContaBancariaParaCxGeralDinheiro: return _transfContaBancariaParaCxGeralDinheiro;
                    case PlanoContas.TransfCxGeralParaContaBancariaDinheiro: return _transfCxGeralParaContaBancariaDinheiro;

                    // Pagamento de contas
                    case PlanoContas.PagtoDinheiro: return _pagtoDinheiro;
                    case PlanoContas.PagtoChequeProprio: return _pagtoChequeProprio;
                    case PlanoContas.PagtoChequeTerceiros: return _pagtoChequeTerceiros;
                    case PlanoContas.PagtoTransfBanco: return _pagtoBanco;
                    case PlanoContas.PagtoBoleto: return _pagtoBoleto;
                    case PlanoContas.PagtoCreditoFornecedor: return _pagtoCreditoFornecedor;
                    case PlanoContas.PagtoPermuta: return _pagtoPermuta;
                    case PlanoContas.PagtoRenegociacao: return _pagtoRenegociacao;
                    case PlanoContas.PagtoAntecipacaoFornecedor: return _pagtoAntecipacaoFornecedor;

                    // Estorno de conta
                    case PlanoContas.EstornoPagtoDinheiro: return _estornoPagtoDinheiro;
                    case PlanoContas.EstornoPagtoChequeProprio: return _estornoPagtoChequeProprio;
                    case PlanoContas.EstornoPagtoChequeTerceiros: return _estornoPagtoChequeTerceiros;
                    case PlanoContas.EstornoPagtoTransfBancaria: return _estornoPagtoBancario;
                    case PlanoContas.EstornoPagtoBoleto: return _estornoPagtoBoleto;
                    case PlanoContas.EstornoPagtoPermuta: return _estornoPagtoPermuta;
                    case PlanoContas.EstornoPagtoCreditoFornecedor: return _estornoPagtoCreditoFornecedor;
                    case PlanoContas.EstornoPagtoAntecipacaoFornecedor: return _estornoPagtoAntecipacaoFornecedor;

                    case PlanoContas.CreditoFornecDinheiro: return _creditoFornecDinheiro;
                    case PlanoContas.CreditoFornecChequeProprio: return _creditoFornecChequeProprio;
                    case PlanoContas.CreditoFornecChequeTerceiros: return _creditoFornecChequeTerceiros;
                    case PlanoContas.CreditoFornecTransfBanco: return _creditoFornecBanco;
                    case PlanoContas.CreditoFornecBoleto: return _creditoFornecBoleto;
                    case PlanoContas.CreditoFornecPermuta: return _creditoFornecPermuta;

                    case PlanoContas.EstornoCreditoFornecDinheiro: return _estornoCreditoFornecDinheiro;
                    case PlanoContas.EstornoCreditoFornecChequeProprio: return _estornoCreditoFornecChequeProprio;
                    case PlanoContas.EstornoCreditoFornecChequeTerceiros: return _estornoCreditoFornecChequeTerceiros;
                    case PlanoContas.EstornoCreditoFornecTransfBancaria: return _estornoCreditoFornecBancario;
                    case PlanoContas.EstornoCreditoFornecPermuta: return _estornoCreditoFornecPermuta;
                    case PlanoContas.EstornoCreditoFornecBoleto: return _estornoCreditoFornecBoleto;

                    // Velor excedente do pedido
                    case PlanoContas.ValorExcedente: return _valorExcedente;

                    // Credito gerado a partir de venda/compra
                    case PlanoContas.CreditoVendaGerado: return _creditoVendaGerado;
                    case PlanoContas.CreditoRecPrazoGerado: return _creditoRecPrazoGerado;
                    case PlanoContas.CreditoEntradaGerado: return _creditoEntradaGerado;
                    case PlanoContas.CreditoCompraGerado: return _creditoCompraGerado;
                    case PlanoContas.EstornoCreditoVendaGerado: return _estornoCreditoVendaGerado;
                    case PlanoContas.EstornoCreditoRecPrazoGerado: return _estornoCreditoRecPrazoGerado;
                    case PlanoContas.EstornoCreditoEntradaGerado: return _estornoCreditoEntradaGerado;
                    case PlanoContas.EstornoCreditoCompraGerado: return _estornoCreditoCompraGerado;

                    case PlanoContas.Comerciais: return _comerciais;
                    //case PlanoContas.Industriais: return _industriais;
                    //case PlanoContas.Frete: return _frete;
                    case PlanoContas.Salario: return _salario;
                    case PlanoContas.SaldoRemanescente: return _saldoRemanescente;

                    case PlanoContas.JurosVendaCartao: return _jurosVendaCartao;
                    case PlanoContas.JurosVendaConstrucard: return _jurosVendaConstrucard;
                    case PlanoContas.EstornoTaxaAntecipDepositoCheque: return _estornoTaxaAntecipDepositoCheque;
                    case PlanoContas.PagtoTaxaAntecipDepositoCheque: return _pagtoTaxaAntecipDepositoCheque;

                    case PlanoContas.ParcelamentoObra: return _parcelamentoObra;
                    case PlanoContas.Renegociacao: return _renegociacao;
                    case PlanoContas.ValorRestantePagto: return _valorRestantePagto;
                    case PlanoContas.PagamentoComisssao: return _pagamentoComissao;
                    case PlanoContas.TrocaDevolucao: return _trocaDevolucao;

                    case PlanoContas.CompraPrazoBoleto: return _compraPrazoBoleto;
                    case PlanoContas.CompraVistaBoleto: return _compraVistaBoleto;
                    case PlanoContas.CompraPrazoCheque: return _compraPrazoCheque;
                    case PlanoContas.CompraVistaCheque: return _compraVistaCheque;
                    case PlanoContas.CompraPrazoDeposito: return _compraPrazoDeposito;
                    case PlanoContas.CompraVistaDeposito: return _compraVistaDeposito;
                    case PlanoContas.CompraPrazoDinheiro: return _compraPrazoDinheiro;
                    case PlanoContas.CompraVistaDinheiro: return _compraVistaDinheiro;

                    case PlanoContas.RecObraCredito: return _recObraCredito;
                    case PlanoContas.EstornoRecObraCredito: return _estornoRecObraCredito;
                    case PlanoContas.CreditoObraGerado: return _creditoObraGerado;
                    case PlanoContas.CreditoObraEstorno: return _creditoObraEstorno;

                    case PlanoContas.PagtoAntecipFornecBoleto: return _pagtoAntecipFornecBoleto;
                    case PlanoContas.PagtoAntecipFornecChequePropio: return _pagtoAntecipFornecChequePropio;
                    case PlanoContas.PagtoAntecipFornecChequeTerceiros: return _pagtoAntecipFornecChequeTerceiros;
                    case PlanoContas.PagtoAntecipFornecDeposito: return _pagtoAntecipFornecDeposito;
                    case PlanoContas.PagtoAntecipFornecDinheiro: return _pagtoAntecipFornecDinheiro;
                    case PlanoContas.PagtoAntecipFornecPermuta: return _pagtoAntecipFornecPermuta;

                    case PlanoContas.EstornoPagtoAntecipFornecBoleto: return _estornoPagtoAntecipFornecBoleto;
                    case PlanoContas.EstornoPagtoAntecipFornecChequePropio: return _estornoPagtoAntecipFornecChequePropio;
                    case PlanoContas.EstornoPagtoAntecipFornecChequeTerceiros: return _estornoPagtoAntecipFornecChequeTerceiros;
                    case PlanoContas.EstornoPagtoAntecipFornecDeposito: return _estornoPagtoAntecipFornecDeposito;
                    case PlanoContas.EstornoPagtoAntecipFornecDinheiro: return _estornoPagtoAntecipFornecDinheiro;
                    case PlanoContas.EstornoPagtoAntecipFornecPermuta: return _estornoPagtoAntecipFornecPermuta;

                    case PlanoContas.ParcelamentoPagtoAntecipFornec: return _parcelamentoPagtoAntecipFornec;

                    case PlanoContas.CreditoAntecipFornecGerado: return _creditoAntecipFornecGerado;
                    case PlanoContas.CreditoAntencipFornecEstorno: return _creditoAntencipFornecEstorno;

                    case PlanoContas.ValorExcedenteEncontroContas: return _valorExcedenteEncontroContas;

                    case PlanoContas.DepositoNaoIdentificado: return _depositoNaoIdentificado;

                    default:
                        throw new Exception("Plano de contas não mapeado.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Plano de Conta para vendas à Vista

        /// <summary>
        /// Pega o Plano de Conta referente à venda à Vista
        /// </summary>
        /// <param name="idFormaPagto"></param>
        /// <returns></returns>
        public static uint GetPlanoVista(uint idFormaPagto)
        {
            switch (idFormaPagto)
            {
                case (uint)Pagto.FormaPagto.Dinheiro:
                    return GetPlanoConta(PlanoContas.VistaDinheiro);
                case (uint)Pagto.FormaPagto.ChequeProprio:
                case (uint)Pagto.FormaPagto.ChequeTerceiro:
                    return GetPlanoConta(PlanoContas.VistaCheque);
                case (uint)Pagto.FormaPagto.Construcard:
                    return GetPlanoConta(PlanoContas.VistaConstrucard);
                case (uint)Pagto.FormaPagto.Cartao:
                case (uint)Pagto.FormaPagto.CartaoNaoIdentificado:
                    return GetPlanoConta(PlanoContas.VistaCartao);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito:
                case (uint)Glass.Data.Model.Pagto.FormaPagto.DepositoNaoIdentificado:
                    return GetPlanoConta(PlanoContas.VistaDeposito);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta:
                    return GetPlanoConta(PlanoContas.VistaPermuta);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Credito:
                    return GetPlanoConta(PlanoContas.VistaCredito);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto:
                    throw new Exception("Boleto não é aceito como pagamento à vista.");
                default:
                    if (idFormaPagto > 0)
                        throw new Exception("A forma de pagamento " + FormaPagtoDAO.Instance.GetDescricao(idFormaPagto) + " não é permitida neste procedimento.");
                    else
                        throw new Exception("A forma de pagamento não foi informada.");
            }
        }

        /// <summary>
        /// Pega o Plano de Conta referente ao tipo de cartão da venda à Vista
        /// </summary>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public static uint GetPlanoVistaTipoCartao(uint idTipoCartao)
        {
            var tipoCartao = ContasCartoes.Where(f => f.IdTipoCartao == idTipoCartao).FirstOrDefault();

            if(tipoCartao == null)
                throw new Exception("Tipo de cartão não encontrado.");

            return (uint)tipoCartao.IdContaVista;
        }

        #endregion

        #region Plano de Conta para vendas à Prazo

        /// <summary>
        /// Pega o Plano de Conta referente à venda à Prazo
        /// </summary>
        /// <param name="idFormaPagto"></param>
        /// <returns></returns>
        public static uint GetPlanoPrazo(uint idFormaPagto)
        {
            switch (idFormaPagto)
            {
                case (uint)Pagto.FormaPagto.Construcard:
                    return GetPlanoConta(PlanoContas.PrazoConstrucard);
                case (uint)Pagto.FormaPagto.Cartao:
                case (uint)Pagto.FormaPagto.CartaoNaoIdentificado:
                    return GetPlanoConta(PlanoContas.PrazoCartao);
                case (uint)Pagto.FormaPagto.ChequeProprio:
                case (uint)Pagto.FormaPagto.ChequeTerceiro:
                    return GetPlanoConta(PlanoContas.PrazoCheque);
                case (uint)Pagto.FormaPagto.Deposito:
                    return GetPlanoConta(PlanoContas.PrazoDeposito);
                case (uint)Pagto.FormaPagto.Boleto:
                    return GetPlanoConta(PlanoContas.PrazoBoleta);
                case (uint)Pagto.FormaPagto.Prazo:
                    return GetPlanoConta(PlanoContas.Prazo);
                case (uint)Pagto.FormaPagto.Permuta:
                    return GetPlanoConta(PlanoContas.PrazoPermuta);
                default:
                    throw new Exception("Forma de pagamento do pedido não permitida.");
            }
        }

        #endregion

        #region Plano de Conta para recebimento do Sinal

        /// <summary>
        /// Pega o Plano de Conta referente ao tipo de recebimento do sinal
        /// </summary>
        /// <param name="idFormaPagto"></param>
        /// <returns></returns>
        public static uint GetPlanoSinal(uint idFormaPagto)
        {
            switch (idFormaPagto)
            {
                case (uint)Pagto.FormaPagto.Boleto:
                    return GetPlanoConta(PlanoContas.EntradaBoleto);
                case (uint)Pagto.FormaPagto.Dinheiro:
                    return GetPlanoConta(PlanoContas.EntradaDinheiro);
                case (uint)Pagto.FormaPagto.ChequeProprio:
                case (uint)Pagto.FormaPagto.ChequeTerceiro:
                    return GetPlanoConta(PlanoContas.EntradaCheque);
                case (uint)Pagto.FormaPagto.Construcard:
                    return GetPlanoConta(PlanoContas.EntradaConstrucard);
                case (uint)Pagto.FormaPagto.Cartao:
                case (uint)Pagto.FormaPagto.CartaoNaoIdentificado:
                    return GetPlanoConta(PlanoContas.EntradaCartao);
                case (uint)Pagto.FormaPagto.Deposito:
                    return GetPlanoConta(PlanoContas.EntradaDeposito);
                case (uint)Pagto.FormaPagto.Permuta:
                    return GetPlanoConta(PlanoContas.EntradaPermuta);
                case (uint)Pagto.FormaPagto.Credito:
                    return GetPlanoConta(PlanoContas.EntradaCredito);
                default:
                    throw new Exception("Forma de pagamento de sinal não permitida.");
            }
        }

        /// <summary>
        /// Pega o Plano de Conta referente ao tipo cartao utilizado no recebimento do sinal
        /// </summary>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public static uint GetPlanoSinalTipoCartao(uint idTipoCartao)
        {
            var tipoCartao = ContasCartoes.Where(f => f.IdTipoCartao == idTipoCartao).FirstOrDefault();

            if(tipoCartao == null)
                throw new Exception("Tipo de cartão não encontrado.");

            return (uint)tipoCartao.IdContaEntrada;
        }

        #endregion

        #region Plano de conta para recebimento de conta

        /// <summary>
        /// Pega o Plano de Conta referente ao tipo de pagamento da conta
        /// </summary>
        /// <param name="idFormaPagto"></param>
        /// <returns></returns>
        public static uint GetPlanoReceb(uint idFormaPagto)
        {
            switch (idFormaPagto)
            {
                case (uint)Pagto.FormaPagto.Boleto:
                    return GetPlanoConta(PlanoContas.RecPrazoBoleto);
                case (uint)Pagto.FormaPagto.Cartao:
                case (uint)Pagto.FormaPagto.CartaoNaoIdentificado:
                    return GetPlanoConta(PlanoContas.RecPrazoCartao);
                case (uint)Pagto.FormaPagto.Dinheiro:
                    return GetPlanoConta(PlanoContas.RecPrazoDinheiro);
                case (uint)Pagto.FormaPagto.ChequeProprio:
                case (uint)Pagto.FormaPagto.ChequeTerceiro:
                    return GetPlanoConta(PlanoContas.RecPrazoCheque);
                case (uint)Pagto.FormaPagto.Construcard:
                    return GetPlanoConta(PlanoContas.RecPrazoConstrucard);
                case (uint)Pagto.FormaPagto.Permuta:
                    return GetPlanoConta(PlanoContas.RecPrazoPermuta);
                case (uint)Pagto.FormaPagto.Deposito:
                    return GetPlanoConta(PlanoContas.RecPrazoDeposito);
                case (uint)Pagto.FormaPagto.Prazo:
                    return GetPlanoConta(PlanoContas.Prazo);
                default:
                    throw new Exception("Forma de pagamento de conta não permitida.");
            }
        }

        /// <summary>
        /// Pega o Plano de Conta referente ao tipo cartao utilizado no pagamento da conta
        /// </summary>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public static uint GetPlanoRecebTipoCartao(uint idTipoCartao)
        {
            var tipoCartao = ContasCartoes.Where(f => f.IdTipoCartao == idTipoCartao).FirstOrDefault();

            if(tipoCartao == null)
                throw new Exception("Tipo de cartão não encontrado.");

            return (uint)tipoCartao.IdContaRecPrazo;
        }

        /// <summary>
        /// Pega o Plano de Conta referente ao tipo cartao utilizado no pagamento da conta de sinal
        /// </summary>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public static uint GetPlanoRecebTipoCartaoEntrada(uint idTipoCartao)
        {
            var tipoCartao = ContasCartoes.Where(f => f.IdTipoCartao == idTipoCartao).FirstOrDefault();

            if(tipoCartao == null)
                throw new Exception("Tipo de cartão não encontrado.");

            return (uint)tipoCartao.IdContaEntrada;
        }

        /// <summary>
        /// Pega o Plano de Conta referente ao tipo boleto utilizado no pagamento da conta
        /// </summary>
        /// <param name="tipoBoleto"></param>
        /// <returns></returns>
        public static uint GetPlanoRecebTipoBoleto(uint tipoBoleto)
        {
            switch (tipoBoleto)
            {
                case (uint)Utils.TipoBoleto.Lumen:
                    return GetPlanoConta(PlanoContas.RecPrazoBoletoLumen);
                case (uint)Utils.TipoBoleto.Santander:
                    return GetPlanoConta(PlanoContas.RecPrazoBoletoSantander);
                case (uint)Utils.TipoBoleto.BancoBrasil:
                    return GetPlanoConta(PlanoContas.RecPrazoBoletoBancoBrasil);
                case (uint)Utils.TipoBoleto.Outros:
                    return GetPlanoConta(PlanoContas.RecPrazoBoletoOutros);
                default:
                    return GetPlanoConta(PlanoContas.RecPrazoBoleto);
            }
        }

        /// <summary>
        /// Retorna a forma de pagto do plano de conta passado
        /// </summary>
        public static string GetFormaPagtoFromPlanoReceb(uint idConta)
        {
            if (GetPlanoConta(PlanoContas.RecPrazoDinheiro) == idConta)
                return "Dinheiro";
            else if (GetPlanoConta(PlanoContas.RecPrazoCheque) == idConta)
                return "Cheque";
            else if (GetPlanoConta(PlanoContas.RecPrazoCartao) == idConta)
                return "Cartão";
            else if (ContasCartoes.Any(f => f.PossuiPlanoConta(idConta)))
                return string.Join(", ", ContasCartoes.Where(f => f.PossuiPlanoConta(idConta)).Select(f => f.Descricao));
            else if (GetPlanoConta(PlanoContas.RecPrazoPermuta) == idConta)
                return "Permuta";
            else if (GetPlanoConta(PlanoContas.RecPrazoDeposito) == idConta)
                return "Depósito";
            else if (GetPlanoConta(PlanoContas.RecPrazoConstrucard) == idConta)
                return "Construcard";
            else if (GetPlanoConta(PlanoContas.RecPrazoBoleto) == idConta)
                return "Boleto";
            else if (GetPlanoConta(PlanoContas.RecPrazoBoletoLumen) == idConta)
                return "Boleto Lumen";
            else if (GetPlanoConta(PlanoContas.RecPrazoBoletoSantander) == idConta)
                return "Boleto Santander";
            else if (GetPlanoConta(PlanoContas.RecPrazoBoletoBancoBrasil) == idConta)
                return "Boleto Banco do Brasil";
            else if (GetPlanoConta(PlanoContas.RecPrazoBoletoOutros) == idConta)
                return "Boleto Outros";
            else if (GetPlanoConta(PlanoContas.RecPrazoCredito) == idConta)
                return "Crédito";
            else if (GetPlanoConta(PlanoContas.CreditoVendaGerado) == idConta)
                return "Crédito venda gerado";
            else if (GetPlanoConta(PlanoContas.CreditoRecPrazoGerado) == idConta)
                return "Crédito rec. prazo gerado";
            else if (GetPlanoConta(PlanoContas.CreditoEntradaGerado) == idConta)
                return "Crédito entrada gerado";
            else if (idConta > 0)
                return PlanoContasDAO.Instance.GetByIdConta(idConta).Descricao;
            else
                return "N/D";
        }

        #endregion

        #region Plano de conta para pagamento de conta

        /// <summary>
        /// Retorna os planos de conta para pagamento.
        /// </summary>
        /// <returns></returns>
        public static string ContasPagto()
        {
            return GetPlanoConta(PlanoContas.PagtoDinheiro) + "," + GetPlanoConta(PlanoContas.PagtoChequeProprio) + "," +
                GetPlanoConta(PlanoContas.PagtoChequeTerceiros) + "," + GetPlanoConta(PlanoContas.PagtoTransfBanco) + "," +
                GetPlanoConta(PlanoContas.PagtoBoleto) + "," + GetPlanoConta(PlanoContas.PagtoCreditoFornecedor) + "," +
                GetPlanoConta(PlanoContas.PagtoPermuta) + "," + GetPlanoConta(PlanoContas.PagtoAntecipacaoFornecedor);
        }

        /// <summary>
        /// Pega o Plano de Conta referente ao tipo de pagamento da conta
        /// </summary>
        /// <param name="idFormaPagto">1-Dinheiro, 2-Cheque Proprio, 3-Cheque Terceiros, 4-Pagto. Bancário, 5-Boleto</param>
        /// <returns></returns>
        public static uint GetPlanoContaPagto(uint idFormaPagto)
        {
            switch (idFormaPagto)
            {
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro:
                    return GetPlanoConta(PlanoContas.PagtoDinheiro);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio:
                    return GetPlanoConta(PlanoContas.PagtoChequeProprio);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro:
                    return GetPlanoConta(PlanoContas.PagtoChequeTerceiros);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito:
                    return GetPlanoConta(PlanoContas.PagtoTransfBanco);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto:
                    return GetPlanoConta(PlanoContas.PagtoBoleto);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Credito:
                    return GetPlanoConta(PlanoContas.PagtoCreditoFornecedor);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta:
                    return GetPlanoConta(PlanoContas.PagtoPermuta);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.AntecipFornec:
                    return GetPlanoConta(PlanoContas.PagtoAntecipacaoFornecedor);
                default:
                    throw new Exception("Forma de pagamento de conta não permitida.");
            }
        }

        /// <summary>
        /// Pega o Plano de Conta de estorno referente ao tipo de pagamento da conta
        /// </summary>
        /// <param name="idFormaPagto">1-Dinheiro, 2-Cheque Proprio, 3-Cheque Terceiros, 4-Pagto. Bancário, 5-Boleto</param>
        /// <returns></returns>
        public static uint GetPlanoContaEstornoPagto(uint idFormaPagto)
        {
            switch (idFormaPagto)
            {
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro:
                    return GetPlanoConta(PlanoContas.EstornoPagtoDinheiro);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio:
                    return GetPlanoConta(PlanoContas.EstornoPagtoChequeProprio);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro:
                    return GetPlanoConta(PlanoContas.EstornoPagtoChequeTerceiros);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito:
                    return GetPlanoConta(PlanoContas.EstornoPagtoTransfBancaria);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto:
                    return GetPlanoConta(PlanoContas.EstornoPagtoBoleto);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta:
                    return GetPlanoConta(PlanoContas.EstornoPagtoPermuta);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.AntecipFornec:
                    return GetPlanoConta(PlanoContas.EstornoPagtoAntecipacaoFornecedor);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Credito:
                    return GetPlanoConta(PlanoContas.EstornoPagtoCreditoFornecedor);
                default:
                    throw new Exception("Forma de pagamento de conta não permitida.");
            }
        }

        #endregion

        #region Plano de conta para geração de crédito para fornecedor

        /// <summary>
        /// Retorna os planos de conta para crédito de fornecedor.
        /// </summary>
        /// <returns></returns>
        public static string ContasCreditoFornec()
        {
            return GetPlanoConta(PlanoContas.CreditoFornecDinheiro) + "," + GetPlanoConta(PlanoContas.CreditoFornecChequeProprio) + "," +
                GetPlanoConta(PlanoContas.CreditoFornecChequeTerceiros) + "," + GetPlanoConta(PlanoContas.CreditoFornecTransfBanco) + "," +
                GetPlanoConta(PlanoContas.CreditoFornecBoleto) + "," + GetPlanoConta(PlanoContas.CreditoFornecPermuta);
        }

        /// <summary>
        /// Retorna os planos de conta de estorno para crédito de fornecedor.
        /// </summary>
        /// <returns></returns>
        public static string ContasEstornoCreditoFornec()
        {
            return GetPlanoConta(PlanoContas.EstornoCreditoFornecDinheiro) + "," + GetPlanoConta(PlanoContas.EstornoCreditoFornecChequeProprio) + "," +
                GetPlanoConta(PlanoContas.EstornoCreditoFornecChequeTerceiros) + "," + GetPlanoConta(PlanoContas.EstornoCreditoFornecTransfBancaria) + "," +
                GetPlanoConta(PlanoContas.EstornoCreditoFornecBoleto) + "," + GetPlanoConta(PlanoContas.EstornoCreditoFornecPermuta);
        }

        /// <summary>
        /// Pega o Plano de Conta referente ao tipo de pagamento de crédito fornecedor
        /// </summary>
        /// <param name="idFormaPagto">1-Dinheiro, 2-Cheque Proprio, 3-Cheque Terceiros, 4-Pagto. Bancário, 5-Boleto</param>
        /// <returns></returns>
        public static uint GetPlanoContaCreditoFornec(uint? idFormaPagto)
        {
            switch (idFormaPagto)
            {
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro:
                    return GetPlanoConta(PlanoContas.CreditoFornecDinheiro);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio:
                    return GetPlanoConta(PlanoContas.CreditoFornecChequeProprio);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro:
                    return GetPlanoConta(PlanoContas.CreditoFornecChequeTerceiros);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito:
                    return GetPlanoConta(PlanoContas.CreditoFornecTransfBanco);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto:
                    return GetPlanoConta(PlanoContas.CreditoFornecBoleto);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta:
                    return GetPlanoConta(PlanoContas.CreditoFornecPermuta);
                default:
                    throw new Exception("Forma de pagamento de conta não permitida.");
            }
        }

        /// <summary>
        /// Pega o Plano de Conta de estorno referente ao tipo de pagamento de crédito fornecedor
        /// </summary>
        /// <param name="idFormaPagto">1-Dinheiro, 2-Cheque Proprio, 3-Cheque Terceiros, 4-Pagto. Bancário, 5-Boleto</param>
        /// <returns></returns>
        public static uint GetPlanoContaEstornoCreditoFornec(uint? idFormaPagto)
        {
            switch (idFormaPagto)
            {
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro:
                    return GetPlanoConta(PlanoContas.EstornoCreditoFornecDinheiro);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio:
                    return GetPlanoConta(PlanoContas.EstornoCreditoFornecChequeProprio);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro:
                    return GetPlanoConta(PlanoContas.EstornoCreditoFornecChequeTerceiros);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito:
                    return GetPlanoConta(PlanoContas.EstornoCreditoFornecTransfBancaria);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto:
                    return GetPlanoConta(PlanoContas.EstornoCreditoFornecBoleto);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta:
                    return GetPlanoConta(PlanoContas.EstornoCreditoFornecPermuta);
                default:
                    throw new Exception("Forma de pagamento de conta não permitida.");
            }
        }

        #endregion

        #region Plano de conta para antecipação de pagto. de fornecedor

        /// <summary>
        /// Retorna os planos de conta para antecipação de pagto. de fornecedor.
        /// </summary>
        /// <returns></returns>
        public static string ContasAntecipFornec()
        {
            return GetPlanoConta(PlanoContas.PagtoAntecipFornecBoleto) + "," + GetPlanoConta(PlanoContas.PagtoAntecipFornecChequePropio) + "," +
                GetPlanoConta(PlanoContas.PagtoAntecipFornecChequeTerceiros) + "," + GetPlanoConta(PlanoContas.PagtoAntecipFornecDeposito) + "," +
                GetPlanoConta(PlanoContas.PagtoAntecipFornecDinheiro) + "," + GetPlanoConta(PlanoContas.PagtoAntecipFornecPermuta);
        }

        /// <summary>
        /// Retorna os planos de conta de estorno para crédito de fornecedor.
        /// </summary>
        /// <returns></returns>
        public static string ContasEstornoAntecipFornec()
        {
            return GetPlanoConta(PlanoContas.EstornoPagtoAntecipFornecBoleto) + "," + GetPlanoConta(PlanoContas.EstornoPagtoAntecipFornecChequePropio) + "," +
                GetPlanoConta(PlanoContas.EstornoPagtoAntecipFornecChequeTerceiros) + "," + GetPlanoConta(PlanoContas.EstornoPagtoAntecipFornecDeposito) + "," +
                GetPlanoConta(PlanoContas.EstornoPagtoAntecipFornecDinheiro) + "," + GetPlanoConta(PlanoContas.EstornoPagtoAntecipFornecPermuta);
        }

        /// <summary>
        /// Pega o Plano de Conta referente ao tipo de pagamento de crédito fornecedor
        /// </summary>
        /// <param name="idFormaPagto">1-Dinheiro, 2-Cheque Proprio, 3-Cheque Terceiros, 4-Pagto. Bancário, 5-Boleto</param>
        /// <returns></returns>
        public static uint GetPlanoContaAntecipFornec(uint idFormaPagto)
        {
            switch (idFormaPagto)
            {
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro:
                    return GetPlanoConta(PlanoContas.PagtoAntecipFornecDinheiro);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio:
                    return GetPlanoConta(PlanoContas.PagtoAntecipFornecChequePropio);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro:
                    return GetPlanoConta(PlanoContas.PagtoAntecipFornecChequeTerceiros);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito:
                    return GetPlanoConta(PlanoContas.PagtoAntecipFornecDeposito);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto:
                    return GetPlanoConta(PlanoContas.PagtoAntecipFornecBoleto);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta:
                    return GetPlanoConta(PlanoContas.PagtoAntecipFornecPermuta);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Credito:
                    return GetPlanoConta(PlanoContas.CreditoAntecipFornecGerado);
                default:
                    throw new Exception("Forma de pagamento de conta não permitida.");
            }
        }

        /// <summary>
        /// Pega o Plano de Conta de estorno referente ao tipo de pagamento de crédito fornecedor
        /// </summary>
        /// <param name="idFormaPagto">1-Dinheiro, 2-Cheque Proprio, 3-Cheque Terceiros, 4-Pagto. Bancário, 5-Boleto</param>
        /// <returns></returns>
        public static uint GetPlanoContaEstornoAntecipFornec(uint idFormaPagto)
        {
            switch (idFormaPagto)
            {
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro:
                    return GetPlanoConta(PlanoContas.EstornoPagtoAntecipFornecDinheiro);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio:
                    return GetPlanoConta(PlanoContas.EstornoPagtoAntecipFornecChequePropio);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro:
                    return GetPlanoConta(PlanoContas.EstornoPagtoAntecipFornecChequeTerceiros);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito:
                    return GetPlanoConta(PlanoContas.EstornoPagtoAntecipFornecDeposito);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto:
                    return GetPlanoConta(PlanoContas.EstornoPagtoAntecipFornecBoleto);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta:
                    return GetPlanoConta(PlanoContas.EstornoPagtoAntecipFornecPermuta);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Credito:
                    return GetPlanoConta(PlanoContas.CreditoAntencipFornecEstorno);
                default:
                    throw new Exception("Forma de pagamento de conta não permitida.");
            }
        }

        #endregion

        #region Plano de conta para quitar cheque devolvido

        /// <summary>
        /// Pega o Plano de Conta referente ao tipo de pagamento do recebimento de cheque devolvido
        /// </summary>
        /// <param name="idFormaPagto"></param>
        /// <returns></returns>
        public static uint GetPlanoContaRecebChequeDev(uint idFormaPagto)
        {
            switch (idFormaPagto)
            {
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro:
                    return GetPlanoConta(PlanoContas.RecChequeDevDinheiro);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Cartao:
                    return GetPlanoConta(PlanoContas.RecChequeDevCartao);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio:
                case (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro:
                    return GetPlanoConta(PlanoContas.RecChequeDevCheque);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito:
                    return GetPlanoConta(PlanoContas.RecChequeDevDeposito);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Construcard:
                    return GetPlanoConta(PlanoContas.RecChequeDevConstrucard);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto:
                    return GetPlanoConta(PlanoContas.RecChequeDevBoleto);
                case (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta:
                    return GetPlanoConta(PlanoContas.RecChequeDevPermuta);
                default:
                    throw new Exception("Forma de pagamento de cheque devolvido não permitida.");
            }
        }

        /// <summary>
        /// Pega o Plano de Conta referente ao tipo de cartão da rec. de cheque devolvido
        /// </summary>
        /// <param name="tipoCartao"></param>
        /// <returns></returns>
        public static uint GetPlanoChequeDevTipoCartao(uint idTipoCartao)
        {
            var tipoCartao = ContasCartoes.Where(f => f.IdTipoCartao == idTipoCartao).FirstOrDefault();

            if(tipoCartao == null)
                throw new Exception("Tipo de cartão não encontrado.");

            return (uint)tipoCartao.IdContaRecChequeDev;
        }

        /// <summary>
        /// Pega o Plano de Conta referente ao tipo de pagamento do recebimento de cheque devolvido
        /// </summary>
        public static uint GetPlanoContaEstornoChequeDev(uint idConta)
        {
            if (idConta == GetPlanoConta(PlanoContas.RecChequeDevDinheiro))
                return GetPlanoConta(PlanoContas.EstornoChequeDevDinheiro);
            else if (idConta == GetPlanoConta(PlanoContas.RecChequeDevCartao))
                return GetPlanoConta(PlanoContas.EstornoChequeDevCartao);
            else if (idConta == GetPlanoConta(PlanoContas.RecChequeDevBoleto))
                return GetPlanoConta(PlanoContas.EstornoChequeDevBoleto);
            else if (idConta == GetPlanoConta(PlanoContas.RecChequeDevCheque))
                return GetPlanoConta(PlanoContas.EstornoChequeDevCheque);
            else if (idConta == GetPlanoConta(PlanoContas.RecChequeDevDeposito))
                return GetPlanoConta(PlanoContas.EstornoChequeDevDeposito);
            else if (idConta == GetPlanoConta(PlanoContas.RecChequeDevConstrucard))
                return GetPlanoConta(PlanoContas.EstornoChequeDevConstrucard);
            else if (idConta == GetPlanoConta(PlanoContas.RecChequeDevCredito))
                return GetPlanoConta(PlanoContas.EstornoChequeDevCredito);
            else if (ContasCartoes.Any(f => f.PossuiPlanoConta(idConta)))
                return (uint)ContasCartoes.Where(f => f.PossuiPlanoConta(idConta)).FirstOrDefault().ObterContaEstorno(idConta);
            else if (idConta == GetPlanoConta(PlanoContas.CreditoVendaGerado))
                return GetPlanoConta(PlanoContas.EstornoCreditoVendaGerado);
            else if (idConta == GetPlanoConta(PlanoContas.RecChequeDevPermuta))
                return GetPlanoConta(PlanoContas.EstornoChequeDevPermuta);
            else
                throw new Exception("Plano de conta de estorno de cheque devolvido não encontrado.");
        }

        #endregion

        #region Planos de contas separados por vírgula

        /// <summary>
        /// Retorna os ids de todos os planos de contas relacionados ao pagamento de pedido à vista, separados por vírgula
        /// </summary>
        /// <returns></returns>
        public static string ContasAVista()
        {
            return GetPlanoConta(PlanoContas.VistaCartao) + "," + GetPlanoConta(PlanoContas.VistaCheque) + "," +
                GetPlanoConta(PlanoContas.VistaConstrucard) + "," + GetPlanoConta(PlanoContas.VistaDeposito) + "," +
                GetPlanoConta(PlanoContas.VistaDinheiro) + "," + GetPlanoConta(PlanoContas.VistaPermuta) + "," +
                GetPlanoConta(PlanoContas.VistaCredito) +

                /* Chamado 45147. */
                (!string.IsNullOrEmpty(ContasTipoVistaCartao()) ? "," + ContasTipoVistaCartao() : string.Empty);
        }

        /// <summary>
        /// Retorna os ids de todos os planos de contas relacionados ao quitamento de cheques devolvidos, separados por vírgula
        /// </summary>
        /// <returns></returns>
        public static string ContasChequeDev()
        {
            return GetPlanoConta(PlanoContas.RecChequeDevBoleto) + "," + GetPlanoConta(PlanoContas.RecChequeDevCartao) + "," +
                GetPlanoConta(PlanoContas.RecChequeDevCheque) + "," + GetPlanoConta(PlanoContas.RecChequeDevConstrucard) + "," +
                GetPlanoConta(PlanoContas.RecChequeDevCredito) + "," + GetPlanoConta(PlanoContas.RecChequeDevDeposito) + "," +
                GetPlanoConta(PlanoContas.RecChequeDevDinheiro) + "," + GetPlanoConta(PlanoContas.RecChequeDevPermuta) + "," +
                GetPlanoConta(PlanoContas.RecChequeDevPermuta) +

                /* Chamado 45147. */
                (!string.IsNullOrEmpty(ContasTipoRecChequeDevCartao()) ? "," + ContasTipoRecChequeDevCartao() : string.Empty); 
        }

        /// <summary>
        /// Retorna os ids de todos os planos de contas relacionados ao pagamento de parcelas, separados por vírgula
        /// </summary>
        /// <returns></returns>
        public static string ContasAPrazo()
        {
            return GetPlanoConta(PlanoContas.RecPrazoBoleto) + "," +
                GetPlanoConta(PlanoContas.RecPrazoCartao) + "," + GetPlanoConta(PlanoContas.RecPrazoCheque) + "," +
                GetPlanoConta(PlanoContas.RecPrazoConstrucard) + "," + GetPlanoConta(PlanoContas.RecPrazoDeposito) + "," +
                GetPlanoConta(PlanoContas.RecPrazoDinheiro) + "," + GetPlanoConta(PlanoContas.RecPrazoPermuta) + "," +
                GetPlanoConta(PlanoContas.RecPrazoCredito) + "," + GetPlanoConta(PlanoContas.RecPrazoBoletoLumen) + "," +
                GetPlanoConta(PlanoContas.RecPrazoBoletoSantander) + "," + GetPlanoConta(PlanoContas.RecPrazoBoletoBancoBrasil) + "," +
                GetPlanoConta(PlanoContas.RecPrazoBoletoOutros) +

                /* Chamado 45147. */
                (!string.IsNullOrEmpty(ContasTipoPrazoCartao()) ? "," + ContasTipoPrazoCartao() : string.Empty);
        }

        /// <summary>
        /// Retorna is ods de todos os planos de contas relacionados à geração de contas a receber.
        /// </summary>
        /// <returns></returns>
        public static string ContasAPrazoContasReceber()
        {
            return GetPlanoConta(PlanoContas.Prazo) + "," + GetPlanoConta(PlanoContas.PrazoBoleta) + "," +
                GetPlanoConta(PlanoContas.PrazoCartao) + "," + GetPlanoConta(PlanoContas.PrazoCheque) + "," +
                GetPlanoConta(PlanoContas.PrazoConstrucard) + "," + GetPlanoConta(PlanoContas.PrazoDeposito) + "," +
                GetPlanoConta(PlanoContas.PrazoPermuta);
        }

        /// <summary>
        /// Retorna os ids de todos os planos de contas relacionados ao pagamento de sinal de pedido, separados por vírgula
        /// </summary>
        /// <returns></returns>
        public static string ContasSinalPedido()
        {
            return GetPlanoConta(PlanoContas.EntradaCartao) + "," + GetPlanoConta(PlanoContas.EntradaCheque) + "," +
                GetPlanoConta(PlanoContas.EntradaConstrucard) + "," + GetPlanoConta(PlanoContas.EntradaDeposito) + "," +
                GetPlanoConta(PlanoContas.EntradaDinheiro) + "," + GetPlanoConta(PlanoContas.EntradaPermuta) + "," +
                GetPlanoConta(PlanoContas.EntradaCredito) + "," + GetPlanoConta(PlanoContas.EntradaBoleto) +

                /* Chamado 45147. */
                (!string.IsNullOrEmpty(ContasTipoEntradaCartao()) ? "," + ContasTipoEntradaCartao() : string.Empty);
        }

        /// <summary>
        /// Retorna os ids de todos os planos de contas relacionados ao estorno de pagamento de sinal de pedido, separados por vírgula
        /// </summary>
        /// <returns></returns>
        public static string ContasEstornoSinalPedido()
        {
            return GetPlanoConta(PlanoContas.EstornoEntradaCartao) + "," + GetPlanoConta(PlanoContas.EstornoEntradaCheque) + "," +
                GetPlanoConta(PlanoContas.EstornoEntradaConstrucard) + "," + GetPlanoConta(PlanoContas.EstornoEntradaDeposito) + "," +
                GetPlanoConta(PlanoContas.EstornoEntradaDinheiro) + "," + GetPlanoConta(PlanoContas.EstornoEntradaPermuta) + "," +
                GetPlanoConta(PlanoContas.EstornoEntradaCredito) + "," + GetPlanoConta(PlanoContas.EstornoEntradaBoleto) +

                /* Chamado 45147. */
                (!string.IsNullOrEmpty(ContasTipoEstornoEntradaCartao()) ? "," + ContasTipoEstornoEntradaCartao() : string.Empty);
        }

        /// <summary>
        /// Retorna as contas de entrada/saída do faturamento sobre vendas por forma de pagamento
        /// </summary>
        /// <param name="formaPagto"></param>
        /// <returns></returns>
        public static string ContasTipoPagto(Glass.Data.Model.Pagto.FormaPagto formaPagto)
        {
            switch (formaPagto)
            {
                case Glass.Data.Model.Pagto.FormaPagto.Boleto:
                    return ContasTodosTiposBoleto();
                case Glass.Data.Model.Pagto.FormaPagto.Cartao:
                    return ContasTodosTiposCartao();
                case Glass.Data.Model.Pagto.FormaPagto.ChequeProprio:
                case Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro:
                    return ContasTodosTiposCheque();
                case Glass.Data.Model.Pagto.FormaPagto.Construcard:
                    return ContasTodosTiposConstrucard();
                case Glass.Data.Model.Pagto.FormaPagto.Deposito:
                    return ContasTodosTiposDeposito();
                case Glass.Data.Model.Pagto.FormaPagto.Dinheiro:
                    return ContasTodosTiposDinheiro();
                case Glass.Data.Model.Pagto.FormaPagto.Permuta:
                    return ContasTodosTiposPermuta();
                case Glass.Data.Model.Pagto.FormaPagto.Credito:
                    return ContasTodosTiposCredito();
                case Glass.Data.Model.Pagto.FormaPagto.Prazo:
                    return String.Empty;
                default:
                    throw new Exception("Forma de pagamento não esperada.");
            }
        }

        public static string ContasTipoCartao(uint tipoCartao)
        {
            var ids = new List<uint>();

            if (ContasCartoes.Where(f => f.IdTipoCartao == tipoCartao).Any(f => f.IdsContasRecebimento != null && f.IdsContasRecebimento.Count > 0))
                foreach (var c in ContasCartoes.Where(f => f.IdTipoCartao == tipoCartao).Select(f => f.IdsContasRecebimento))
                    ids.AddRange(c);

            if (ContasCartoes.Where(f => f.IdTipoCartao == tipoCartao).Any(f => f.IdsContasEstorno != null && f.IdsContasEstorno.Count > 0))
                foreach (var c in ContasCartoes.Where(f => f.IdTipoCartao == tipoCartao).Select(f => f.IdsContasEstorno))
                    ids.AddRange(c);

            return ids.Count > 0 ? string.Join(",", ids.Distinct()) : string.Empty;
        }

        #region Boletos

        /// <summary>
        /// Retorna contas de recebimento de boletos
        /// </summary>
        /// <returns></returns>
        public static string ContasRecebimentoBoleto()
        {
            return GetPlanoConta(PlanoContas.RecPrazoBoleto) + "," + GetPlanoConta(PlanoContas.RecPrazoBoletoBancoBrasil) + "," +
                GetPlanoConta(PlanoContas.RecPrazoBoletoLumen) + "," + GetPlanoConta(PlanoContas.RecPrazoBoletoOutros) + "," +
                GetPlanoConta(PlanoContas.RecPrazoBoletoSantander) + "," + GetPlanoConta(PlanoContas.EntradaBoleto) + "," +
                GetPlanoConta(PlanoContas.PrazoBoleta) + "," + GetPlanoConta(PlanoContas.DevolucaoPagtoBoleto) + "," +
                GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoBancoBrasil) + "," + GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoLumen) + "," +
                GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoOutros) + "," + GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoSantander) +
                "," + GetPlanoConta(PlanoContas.RecChequeDevBoleto);
        }

        /// <summary>
        /// Retorna contas de estorno de boletos
        /// </summary>
        /// <returns></returns>
        public static string ContasEstornoBoleto()
        {
            return GetPlanoConta(PlanoContas.EstornoBoleto) + "," + GetPlanoConta(PlanoContas.EstornoBoletoBancoBrasil) + "," +
                GetPlanoConta(PlanoContas.EstornoBoletoLumen) + "," + GetPlanoConta(PlanoContas.EstornoBoletoOutros) + "," +
                GetPlanoConta(PlanoContas.EstornoBoletoSantander) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoBoleto) + "," +
                GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoBancoBrasil) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoLumen) + "," +
                GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoOutros) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoSantander) + "," +
                GetPlanoConta(PlanoContas.EstornoEntradaBoleto) + "," + GetPlanoConta(PlanoContas.EstornoEntradaBoletoBancoBrasil) + "," +
                GetPlanoConta(PlanoContas.EstornoEntradaBoletoLumen) + "," + GetPlanoConta(PlanoContas.EstornoEntradaBoletoOutros) + "," +
                GetPlanoConta(PlanoContas.EstornoEntradaBoletoSantander) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevBoleto) + "," +
                GetPlanoConta(PlanoContas.EstornoChequeDevBoletoBancoBrasil) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevBoletoLumen) + "," +
                GetPlanoConta(PlanoContas.EstornoChequeDevBoletoOutros) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevBoletoSantander) + "," +
                GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoBoleto) + "," + GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoBoletoBancoBrasil) + "," +
                GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoBoletoLumen) + "," + GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoBoletoOutros) + "," +
                GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoBoletoSantander);
        }

        /// <summary>
        /// Retorna todos os tipos de contas de boletos
        /// </summary>
        /// <returns></returns>
        public static string ContasTodosTiposBoleto()
        {
            return ContasRecebimentoBoleto() + "," + ContasEstornoBoleto();
        }

        #endregion

        #region Cartão

        /// <summary>
        /// Retorna contas de recebimento de cartão
        /// </summary>
        public static string ContasRecebimentoCartao()
        {
            var ids = new List<uint>();

            if (ContasCartoes.Any(f => f.IdsContasRecebimento != null && f.IdsContasRecebimento.Count > 0))
                foreach (var c in ContasCartoes.Select(f => f.IdsContasRecebimento))
                    ids.AddRange(c);

            return ids.Count > 0 ? string.Join(",", ids) : string.Empty;
        }

        /// <summary>
        /// Retorna contas de estorno de cartão
        /// </summary>
        public static string ContasEstornoCartao()
        {
            var ids = new List<uint>();

            if (ContasCartoes.Any(f => f.IdsContasEstorno != null && f.IdsContasEstorno.Count > 0))
                foreach (var c in ContasCartoes.Select(f => f.IdsContasEstorno))
                    ids.AddRange(c);

            return ids.Count > 0 ? string.Join(",", ids) : string.Empty;
        }

        /// <summary>
        /// Retorna todas as contas de cartão
        /// </summary>
        public static string ContasTodosTiposCartao()
        {
            var ids = new List<uint>();

            if (ContasCartoes.Any(f => f.IdsContasRecebimento != null && f.IdsContasRecebimento.Count > 0))
                foreach (var c in ContasCartoes.Select(f => f.IdsContasRecebimento))
                    ids.AddRange(c);

            if (ContasCartoes.Any(f => f.IdsContasEstorno != null && f.IdsContasEstorno.Count > 0))
                foreach (var c in ContasCartoes.Select(f => f.IdsContasEstorno))
                    ids.AddRange(c);

            return ids.Count > 0 ? string.Join(",", ids) : string.Empty;
        }

        public static IList<uint> LstContasTodosTiposCartao()
        {
            var ids = new List<uint>();

            if (ContasCartoes.Any(f => f.IdsContasRecebimento != null && f.IdsContasRecebimento.Count > 0))
                foreach (var c in ContasCartoes.Select(f => f.IdsContasRecebimento))
                    ids.AddRange(c);

            if (ContasCartoes.Any(f => f.IdsContasEstorno != null && f.IdsContasEstorno.Count > 0))
                foreach (var c in ContasCartoes.Select(f => f.IdsContasEstorno))
                    ids.AddRange(c);

            return ids.Count > 0 ? ids.Distinct().ToList() : new List<uint>();
        }

        public static uint ContasTipoFuncCartao(uint? idTipoCartao)
        {
            var tipoCartao = ContasCartoes.Where(f => f.IdTipoCartao == idTipoCartao).FirstOrDefault();

            return tipoCartao != null ? (uint)tipoCartao.IdContaFunc : GetPlanoConta(PlanoContas.FuncCartao);
        }

        public static string ContasTipoVistaCartao()
        {
            return ContasCartoes.Any(f => f.IdContaVista > 0) ?
                string.Join(",", ContasCartoes.Select(f => f.IdContaVista).Distinct()) : string.Empty;
        }

        public static string ContasTipoEstornoCartao()
        {
            return ContasCartoes.Any(f => f.IdContaEstorno > 0) ?
                string.Join(",", ContasCartoes.Select(f => f.IdContaEstorno).Distinct()) : string.Empty;
        }

        public static string ContasTipoEstornoEntradaCartao()
        {
            return ContasCartoes.Any(f => f.IdContaEstornoEntrada > 0) ?
                string.Join(",", ContasCartoes.Select(f => f.IdContaEstornoEntrada).Distinct()) : string.Empty;
        }

        public static string ContasTipoEstornoChequeDevCartao()
        {
            return ContasCartoes.Any(f => f.IdContaEstornoChequeDev > 0) ?
                string.Join(",", ContasCartoes.Select(f => f.IdContaEstornoChequeDev).Distinct()) : string.Empty;
        }

        public static string ContasTipoPrazoCartao()
        {
            return ContasCartoes.Any(f => f.IdContaRecPrazo > 0) ?
                string.Join(",", ContasCartoes.Select(f => f.IdContaRecPrazo).Distinct()) : string.Empty;
        }

        public static string ContasTipoCartao(TipoCartaoEnum tipo)
        {
            var lst = new List<uint>();

            if (ContasCartoes.Where(f => f.Tipo == tipo).Any(f => f.IdsContasRecebimento != null && f.IdsContasRecebimento.Count > 0))
                foreach (var c in ContasCartoes.Where(f => f.Tipo == tipo).Select(f => f.IdsContasRecebimento))
                    lst.AddRange(c);

            if (ContasCartoes.Where(f => f.Tipo == tipo).Any(f => f.IdsContasEstorno != null && f.IdsContasEstorno.Count > 0))
                foreach (var c in ContasCartoes.Where(f => f.Tipo == tipo).Select(f => f.IdsContasEstorno))
                    lst.AddRange(c);

            return lst.Count > 0 ? string.Join(",", lst) : string.Empty;
        }

        public static string ContasTipoRecChequeDevCartao()
        {
            return ContasCartoes.Any(f => f.IdContaRecChequeDev > 0) ?
                string.Join(",", ContasCartoes.Select(f => f.IdContaRecChequeDev)) : string.Empty;
        }

        public static string ContasTipoChequeDevCartao()
        {
            var lst = new List<uint>();

            if (ContasCartoes.Any(f => f.IdContaRecChequeDev > 0))
                lst.AddRange(ContasCartoes.Select(f => (uint)f.IdContaRecChequeDev));

            if (ContasCartoes.Any(f => f.IdContaEstornoChequeDev > 0))
                lst.AddRange(ContasCartoes.Select(f => (uint)f.IdContaEstornoChequeDev));
            
            return lst.Count > 0 ? string.Join(",", lst) : string.Empty;
        }

        public static string ContasTipoEntradaCartao()
        {
            return ContasCartoes.Any(f => f.IdContaEntrada > 0) ?
                string.Join(",", ContasCartoes.Select(f => f.IdContaEntrada).Distinct()) : string.Empty;
        }

        #endregion

        #region Cheque

        /// <summary>
        /// Retorna contas de recebimento de cheque
        /// </summary>
        /// <returns></returns>
        public static string ContasRecebimentoCheque()
        {
            return GetPlanoConta(PlanoContas.EntradaCheque) + "," + GetPlanoConta(PlanoContas.RecPrazoCheque) + "," +
                GetPlanoConta(PlanoContas.PrazoCheque) + "," + GetPlanoConta(PlanoContas.VistaCheque) + "," +
                GetPlanoConta(PlanoContas.RecChequeDevCheque) + "," + GetPlanoConta(PlanoContas.DevolucaoPagtoCheque) + "," +
                GetPlanoConta(PlanoContas.ChequeDevolvido) + "," + GetPlanoConta(PlanoContas.ChequeQuitado) + "," +
                GetPlanoConta(PlanoContas.ChequeTrocado);
        }

        /// <summary>
        /// Retorna contas de estorno de cheque
        /// </summary>
        /// <returns></returns>
        public static string ContasEstornoCheque()
        {
            return GetPlanoConta(PlanoContas.EstornoCheque) + "," + GetPlanoConta(PlanoContas.EstornoEntradaCheque) + "," +
                GetPlanoConta(PlanoContas.EstornoRecPrazoCheque) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevCheque) + "," +
                GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoCheque);
        }

        /// <summary>
        /// Retorna todas as contas de cheque
        /// </summary>
        /// <returns></returns>
        public static string ContasTodosTiposCheque()
        {
            return ContasRecebimentoCheque() + "," + ContasEstornoCheque();
        }

        #endregion

        #region Construcard

        /// <summary>
        /// Retorna contas de recebimento de construcard
        /// </summary>
        /// <returns></returns>
        public static string ContasRecebimentoConstrucard()
        {
            return GetPlanoConta(PlanoContas.EntradaConstrucard) + "," + GetPlanoConta(PlanoContas.RecPrazoConstrucard) + "," +
                GetPlanoConta(PlanoContas.VistaConstrucard) + "," + GetPlanoConta(PlanoContas.RecChequeDevConstrucard) + "," +
                GetPlanoConta(PlanoContas.DevolucaoPagtoConstrucard) + "," + GetPlanoConta(PlanoContas.PrazoConstrucard);
        }

        /// <summary>
        /// Retorna contas de estorno de construcard
        /// </summary>
        /// <returns></returns>
        public static string ContasEstornoConstrucard()
        {
            return GetPlanoConta(PlanoContas.EstornoConstrucard) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoConstrucard) + "," +
                GetPlanoConta(PlanoContas.EstornoEntradaConstrucard) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevConstrucard) + "," +
                GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoConstrucard);
        }

        /// <summary>
        /// Retorna todas as contas de construcard
        /// </summary>
        /// <returns></returns>
        public static string ContasTodosTiposConstrucard()
        {
            return ContasRecebimentoConstrucard() + "," + ContasEstornoConstrucard();
        }

        #endregion

        #region Depósito

        /// <summary>
        /// Retorna contas de recebimento de depósito
        /// </summary>
        /// <returns></returns>
        public static string ContasRecebimentoDeposito()
        {
            return GetPlanoConta(PlanoContas.EntradaDeposito) + "," + GetPlanoConta(PlanoContas.PrazoDeposito) + "," +
                GetPlanoConta(PlanoContas.RecPrazoDeposito) + "," + GetPlanoConta(PlanoContas.VistaDeposito) + "," +
                GetPlanoConta(PlanoContas.PrazoDeposito) + "," + GetPlanoConta(PlanoContas.DevolucaoPagtoDeposito) + "," +
                GetPlanoConta(PlanoContas.RecChequeDevDeposito);
        }

        /// <summary>
        /// Retorna contas de estorno de depósito
        /// </summary>
        /// <returns></returns>
        public static string ContasEstornoDeposito()
        {
            return GetPlanoConta(PlanoContas.EstornoDeposito) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoDeposito) + "," +
                GetPlanoConta(PlanoContas.EstornoEntradaDeposito) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevDeposito) + "," +
                GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoDeposito);
        }

        /// <summary>
        /// Retorna todas as contas de depósito
        /// </summary>
        /// <returns></returns>
        public static string ContasTodosTiposDeposito()
        {
            return ContasRecebimentoDeposito() + "," + ContasEstornoDeposito() + "," + GetPlanoConta(PlanoContas.DevolucaoPagtoDeposito);
        }

        #endregion

        #region Dinheiro

        /// <summary>
        /// Retorna contas de recebimento de dinheiro
        /// </summary>
        /// <returns></returns>
        public static string ContasRecebimentoDinheiro()
        {
            return GetPlanoConta(PlanoContas.EntradaDinheiro) + "," + GetPlanoConta(PlanoContas.RecPrazoDinheiro) + "," +
                GetPlanoConta(PlanoContas.VistaDinheiro) + "," + GetPlanoConta(PlanoContas.RecChequeDevDinheiro) + "," +
                GetPlanoConta(PlanoContas.DevolucaoPagtoDinheiro);
        }

        /// <summary>
        /// Retorna contas de estorno de dinheiro
        /// </summary>
        /// <returns></returns>
        public static string ContasEstornoDinheiro()
        {
            return GetPlanoConta(PlanoContas.EstornoDinheiro) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoDinheiro) + "," +
                GetPlanoConta(PlanoContas.EstornoEntradaDinheiro) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevDinheiro) + "," +
                GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoDinheiro);
        }

        /// <summary>
        /// Retorna todas as contas de dinheiro
        /// </summary>
        /// <returns></returns>
        public static string ContasTodosTiposDinheiro()
        {
            return ContasRecebimentoDinheiro() + "," + ContasEstornoDinheiro();
        }

        #endregion

        #region Permuta

        /// <summary>
        /// Retorna contas de recebimento de permuta
        /// </summary>
        /// <returns></returns>
        public static string ContasRecebimentoPermuta()
        {
            return GetPlanoConta(PlanoContas.EntradaPermuta) + "," + GetPlanoConta(PlanoContas.PrazoPermuta) + "," +
                GetPlanoConta(PlanoContas.RecPrazoPermuta) + "," + GetPlanoConta(PlanoContas.VistaPermuta) + "," +
                GetPlanoConta(PlanoContas.PrazoPermuta) + "," + GetPlanoConta(PlanoContas.DevolucaoPagtoPermuta) + "," +
                GetPlanoConta(PlanoContas.RecChequeDevPermuta);
        }

        /// <summary>
        /// Retorna contas de estorno de permuta
        /// </summary>
        /// <returns></returns>
        public static string ContasEstornoPermuta()
        {
            return GetPlanoConta(PlanoContas.EstornoPermuta) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoPermuta) + "," +
                GetPlanoConta(PlanoContas.EstornoEntradaPermuta) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevPermuta) + "," +
                GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoPermuta);
        }

        public static string ContasTodosTiposPermuta()
        {
            return ContasRecebimentoPermuta() + "," + ContasEstornoPermuta();
        }

        #endregion

        #region Crédito

        public static string ContasRecebimentoCredito()
        {
            return GetPlanoConta(PlanoContas.EntradaCredito) + "," + GetPlanoConta(PlanoContas.RecPrazoCredito) + "," +
                GetPlanoConta(PlanoContas.VistaCredito) + "," + GetPlanoConta(PlanoContas.RecChequeDevCredito) + "," +
                GetPlanoConta(PlanoContas.DevolucaoPagtoCredito);
        }

        public static string ContasEstornoCredito()
        {
            return GetPlanoConta(PlanoContas.EstornoCredito) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoCredito) + "," +
                GetPlanoConta(PlanoContas.EstornoEntradaCredito) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevCredito) + "," +
                GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoCredito);
        }

        public static string ContasTodosTiposCredito()
        {
            return ContasRecebimentoCredito() + "," + ContasEstornoCredito();
        }

        #endregion

        public static string ContasTodosTiposPrazo()
        {
            return GetPlanoConta(PlanoContas.Prazo) + "";
        }

        /// <summary>
        /// Retorna os planos de conta para uma forma de pagamento.
        /// </summary>
        /// <param name="formaPagto"></param>
        /// <returns></returns>
        public static string ContasTodasPorTipo(Glass.Data.Model.Pagto.FormaPagto formaPagto)
        {
            switch (formaPagto)
            {
                case Pagto.FormaPagto.Boleto: return ContasTodosTiposBoleto();
                case Pagto.FormaPagto.Cartao: return ContasTodosTiposCartao();
                case Pagto.FormaPagto.ChequeTerceiro: return ContasTodosTiposCheque();
                case Pagto.FormaPagto.ChequeProprio: return ContasTodosTiposCheque();
                case Pagto.FormaPagto.Construcard: return ContasTodosTiposConstrucard();
                case Pagto.FormaPagto.Deposito: return ContasTodosTiposDeposito();
                case Pagto.FormaPagto.Dinheiro: return ContasTodosTiposDinheiro();
                case Pagto.FormaPagto.Permuta: return ContasTodosTiposPermuta();
                case Pagto.FormaPagto.Prazo: return ContasTodosTiposPrazo();
                case Pagto.FormaPagto.Credito: return ContasTodosTiposCredito();
                default: return "";
            }
        }

        public static List<uint> ListContasTipo(Glass.Data.Model.Pagto.FormaPagto formaPagto)
        {
            List<uint> retorno = new List<uint>();
            foreach (string s in ContasTodasPorTipo(formaPagto).Split(','))
            {
                uint add;
                if (uint.TryParse(s, out add))
                    retorno.Add(add);
            }

            return retorno;
        }

        #endregion

        #region Planos de contas de estorno

        /// <summary>
        /// Retorna todos os possíveis estornos à vista separados por vírgula
        /// </summary>
        /// <returns></returns>
        public static string ListaEstornosAVista()
        {
            return GetPlanoConta(PlanoContas.EstornoCartao) + "," + GetPlanoConta(PlanoContas.EstornoCheque) + "," +
                GetPlanoConta(PlanoContas.EstornoConstrucard) + "," + GetPlanoConta(PlanoContas.EstornoDeposito) + "," +
                GetPlanoConta(PlanoContas.EstornoDinheiro) + "," + GetPlanoConta(PlanoContas.EstornoPermuta) + "," +
                GetPlanoConta(PlanoContas.EstornoCredito) +

                /* Chamado 45147. */
                (!string.IsNullOrEmpty(ContasTipoEstornoCartao()) ? "," + ContasTipoEstornoCartao() : string.Empty);
        }

        /// <summary>
        /// Retorna todos os possíveis estornos à prazo separados por vírgula
        /// </summary>
        /// <returns></returns>
        public static string ListaEstornosAPrazo()
        {
            return GetPlanoConta(PlanoContas.EstornoRecPrazoCartao) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoCheque) + "," +
                GetPlanoConta(PlanoContas.EstornoRecPrazoConstrucard) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoDeposito) + "," +
                GetPlanoConta(PlanoContas.EstornoRecPrazoDinheiro) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoPermuta) + "," +
                GetPlanoConta(PlanoContas.EstornoRecPrazoCredito) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoBoleto) + "," +
                GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoBancoBrasil) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoLumen) + "," +
                GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoOutros) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoSantander) +

                /* Chamado 45147. */
                (!string.IsNullOrEmpty(string.Join(",", ContasCartoes.Select(f => f.IdContaEstornoRecPrazo))) ? "," + string.Join(",", ContasCartoes.Select(f => f.IdContaEstornoRecPrazo)) : string.Empty);
        }

        /// <summary>
        /// Retorna todos os possíveis estornos de sinal separados por vírgula
        /// </summary>
        /// <returns></returns>
        public static string ListaEstornosSinalPedido()
        {
            return GetPlanoConta(PlanoContas.EstornoEntradaCartao) + "," + GetPlanoConta(PlanoContas.EstornoEntradaCheque) + "," +
                GetPlanoConta(PlanoContas.EstornoEntradaConstrucard) + "," + GetPlanoConta(PlanoContas.EstornoEntradaDeposito) + "," +
                GetPlanoConta(PlanoContas.EstornoEntradaDinheiro) + "," + GetPlanoConta(PlanoContas.EstornoEntradaPermuta) + "," +
                GetPlanoConta(PlanoContas.EstornoEntradaCredito) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoBoleto) + "," +
                GetPlanoConta(PlanoContas.EstornoEntradaBoletoBancoBrasil) + "," + GetPlanoConta(PlanoContas.EstornoEntradaBoletoLumen) + "," +
                GetPlanoConta(PlanoContas.EstornoEntradaBoletoOutros) + "," + GetPlanoConta(PlanoContas.EstornoEntradaBoletoSantander) +

                /* Chamado 45147. */
                (!string.IsNullOrEmpty(ContasTipoEstornoEntradaCartao()) ? "," + ContasTipoEstornoEntradaCartao() : string.Empty);
        }

        /// <summary>
        /// Retorna todos os possíveis estornos de sinal separados por vírgula
        /// </summary>
        /// <returns></returns>
        public static string ListaEstornosChequeDev()
        {
            return GetPlanoConta(PlanoContas.EstornoChequeDevBoleto) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevBoletoBancoBrasil) + "," +
                GetPlanoConta(PlanoContas.EstornoChequeDevBoletoLumen) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevBoletoOutros) + "," +
                GetPlanoConta(PlanoContas.EstornoChequeDevBoletoSantander) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevCartao) + "," +
                GetPlanoConta(PlanoContas.EstornoChequeDevCheque) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevConstrucard) + "," +
                GetPlanoConta(PlanoContas.EstornoChequeDevCredito) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevDeposito) + "," +
                GetPlanoConta(PlanoContas.EstornoChequeDevDinheiro) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevPermuta) + "," +
                GetPlanoConta(PlanoContas.EstornoChequeDevPermuta) +

                /* Chamado 45147. */
                (!string.IsNullOrEmpty(ContasTipoEstornoChequeDevCartao()) ? "," + ContasTipoEstornoChequeDevCartao() : string.Empty);
        }

        /// <summary>
        /// Retorna o idConta do estorno referente ao idConta a vista passado
        /// </summary>
        /// <returns></returns>
        public static uint EstornoAVista(uint idConta)
        {
            if (idConta == GetPlanoConta(PlanoContas.VistaCartao))
                return GetPlanoConta(PlanoContas.EstornoCartao);
            else if (idConta == GetPlanoConta(PlanoContas.VistaCheque))
                return GetPlanoConta(PlanoContas.EstornoCheque);
            else if (idConta == GetPlanoConta(PlanoContas.VistaConstrucard))
                return GetPlanoConta(PlanoContas.EstornoConstrucard);
            else if (idConta == GetPlanoConta(PlanoContas.VistaDeposito))
                return GetPlanoConta(PlanoContas.EstornoDeposito);
            else if (idConta == GetPlanoConta(PlanoContas.VistaDinheiro))
                return GetPlanoConta(PlanoContas.EstornoDinheiro);
            else if (idConta == GetPlanoConta(PlanoContas.VistaPermuta))
                return GetPlanoConta(PlanoContas.EstornoPermuta);
            else if (idConta == GetPlanoConta(PlanoContas.VistaCredito))
                return GetPlanoConta(PlanoContas.EstornoCredito);
            else if (ContasCartoes.Any(f => f.PossuiPlanoConta(idConta)))
                return (uint)ContasCartoes.Where(f => f.PossuiPlanoConta(idConta)).FirstOrDefault().ObterContaEstorno(idConta);
            else if (idConta == GetPlanoConta(PlanoContas.CreditoEntradaGerado))
                return GetPlanoConta(PlanoContas.EstornoCreditoEntradaGerado);
            else if (idConta == GetPlanoConta(PlanoContas.CreditoVendaGerado))
                return GetPlanoConta(PlanoContas.EstornoCreditoVendaGerado);
            else if (idConta == GetPlanoConta(PlanoContas.CreditoObraGerado))
                return GetPlanoConta(PlanoContas.CreditoObraEstorno);
            else if (idConta == GetPlanoConta(PlanoContas.RecObraCredito))
                return GetPlanoConta(PlanoContas.EstornoRecObraCredito);
            else
                throw new Exception("Plano de conta de estorno não existente.");
        }

        /// <summary>
        /// Retorna o idConta do estorno referente ao idConta a prazo passado
        /// </summary>
        /// <returns></returns>
        public static uint EstornoAPrazo(uint idConta)
        {
            if (idConta == GetPlanoConta(PlanoContas.RecPrazoCartao))
                return GetPlanoConta(PlanoContas.EstornoRecPrazoCartao);
            else if (idConta == GetPlanoConta(PlanoContas.RecPrazoBoleto))
                return GetPlanoConta(PlanoContas.EstornoRecPrazoBoleto);
            else if (idConta == GetPlanoConta(PlanoContas.RecPrazoBoletoBancoBrasil))
                return GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoBancoBrasil);
            else if (idConta == GetPlanoConta(PlanoContas.RecPrazoBoletoLumen))
                return GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoLumen);
            else if (idConta == GetPlanoConta(PlanoContas.RecPrazoBoletoOutros))
                return GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoOutros);
            else if (idConta == GetPlanoConta(PlanoContas.RecPrazoBoletoSantander))
                return GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoSantander);
            else if (idConta == GetPlanoConta(PlanoContas.RecPrazoCheque))
                return GetPlanoConta(PlanoContas.EstornoRecPrazoCheque);
            else if (idConta == GetPlanoConta(PlanoContas.RecPrazoConstrucard))
                return GetPlanoConta(PlanoContas.EstornoRecPrazoConstrucard);
            else if (idConta == GetPlanoConta(PlanoContas.RecPrazoDeposito))
                return GetPlanoConta(PlanoContas.EstornoRecPrazoDeposito);
            else if (idConta == GetPlanoConta(PlanoContas.RecPrazoDinheiro))
                return GetPlanoConta(PlanoContas.EstornoRecPrazoDinheiro);
            else if (idConta == GetPlanoConta(PlanoContas.RecPrazoPermuta))
                return GetPlanoConta(PlanoContas.EstornoRecPrazoPermuta);
            else if (idConta == GetPlanoConta(PlanoContas.RecPrazoCredito))
                return GetPlanoConta(PlanoContas.EstornoRecPrazoCredito);
            else if (ContasCartoes.Any(f => f.PossuiPlanoConta(idConta)))
                return (uint)ContasCartoes.Where(f => f.PossuiPlanoConta(idConta)).FirstOrDefault().ObterContaEstorno(idConta);
            else if (idConta == FinanceiroConfig.PlanoContaJurosCartao)
                return FinanceiroConfig.PlanoContaEstornoJurosCartao;
            else if (idConta == GetPlanoConta(PlanoContas.CreditoRecPrazoGerado) ||
                idConta == GetPlanoConta(PlanoContas.CreditoVendaGerado))
                return GetPlanoConta(PlanoContas.EstornoCreditoRecPrazoGerado);
            else if (idConta == FinanceiroConfig.PlanoContaJurosReceb)
            {
                var idContaEstorno = FinanceiroConfig.PlanoContaEstornoJurosReceb;

                if (idContaEstorno == 0)
                    throw new Exception("Plano de conta de estorno de juros de recebimento não configurado.");

                return idContaEstorno;
            }
            else if (idConta == FinanceiroConfig.PlanoContaMultaReceb)
            {
                var idContaEstorno = FinanceiroConfig.PlanoContaEstornoMultaReceb;

                if (idContaEstorno == 0)
                    throw new Exception("Plano de conta de estorno de multa de recebimento não configurado.");

                return idContaEstorno;
            }
            else
                throw new Exception("Plano de conta de estorno não existente.");
        }

        /// <summary>
        /// Retorna o idConta do estorno referente ao idConta de sinal de pedido passado
        /// </summary>
        /// <returns></returns>
        public static uint EstornoSinalPedido(uint idConta)
        {
            if (idConta == GetPlanoConta(PlanoContas.EntradaCartao))
                return GetPlanoConta(PlanoContas.EstornoEntradaCartao);
            else if (idConta == GetPlanoConta(PlanoContas.EntradaCheque))
                return GetPlanoConta(PlanoContas.EstornoEntradaCheque);
            else if (idConta == GetPlanoConta(PlanoContas.EntradaBoleto))
                return GetPlanoConta(PlanoContas.EstornoEntradaBoleto);
            else if (idConta == GetPlanoConta(PlanoContas.EntradaConstrucard))
                return GetPlanoConta(PlanoContas.EstornoEntradaConstrucard);
            else if (idConta == GetPlanoConta(PlanoContas.EntradaDeposito))
                return GetPlanoConta(PlanoContas.EstornoEntradaDeposito);
            else if (idConta == GetPlanoConta(PlanoContas.EntradaDinheiro))
                return GetPlanoConta(PlanoContas.EstornoEntradaDinheiro);
            else if (idConta == GetPlanoConta(PlanoContas.EntradaPermuta))
                return GetPlanoConta(PlanoContas.EstornoEntradaPermuta);
            else if (idConta == GetPlanoConta(PlanoContas.EntradaCredito))
                return GetPlanoConta(PlanoContas.EstornoEntradaCredito);
            else if (ContasCartoes.Any(f => f.PossuiPlanoConta(idConta)))
                return (uint)ContasCartoes.Where(f => f.PossuiPlanoConta(idConta)).FirstOrDefault().ObterContaEstorno(idConta);
            else if (idConta == GetPlanoConta(PlanoContas.CreditoEntradaGerado) ||
                idConta == GetPlanoConta(PlanoContas.CreditoVendaGerado))
                return GetPlanoConta(PlanoContas.EstornoCreditoEntradaGerado);
            else
                throw new Exception("Plano de conta de estorno não existente.");
        }

        #endregion

        #region Planos utilizados para fechamento de cx geral e diario

        /// <summary>
        /// Retorna string com idsConta separados por "," de acordo com a forma de pagamento passada 
        /// (Utilizado para fechamento de cx geral e diário)
        /// </summary>
        /// <param name="formaPagto"></param>
        /// <returns></returns>
        public static string GetLstEntradaByFormaPagto(Pagto.FormaPagto formaPagto, uint tipoCartaoBoleto, bool cxDiario)
        {
            List<uint> lst = new List<uint>();

            switch (formaPagto)
            {
                case Glass.Data.Model.Pagto.FormaPagto.Dinheiro:
                    lst = new List<uint>() {
                    GetPlanoConta(PlanoContas.EntradaDinheiro), GetPlanoConta(PlanoContas.VistaDinheiro),
                    GetPlanoConta(PlanoContas.RecPrazoDinheiro), GetPlanoConta(PlanoContas.RecChequeDevDinheiro),
                    GetPlanoConta(PlanoContas.SaldoRemanescente), GetPlanoConta(PlanoContas.TransfContaBancariaParaCxGeralDinheiro),
                    GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoDinheiro) };
                    if (cxDiario) lst.Add(GetPlanoConta(PlanoContas.TransfCaixaGeralParaDiario));
                    else lst.Add(GetPlanoConta(PlanoContas.TransfDeCxDiarioDinheiro)); break;
                case Glass.Data.Model.Pagto.FormaPagto.ChequeProprio:
                case Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro:

                    lst = new List<uint>()
                    {
                        GetPlanoConta(PlanoContas.EntradaCheque), GetPlanoConta(PlanoContas.VistaCheque),
                        GetPlanoConta(PlanoContas.PrazoCheque), GetPlanoConta(PlanoContas.RecPrazoCheque),
                        GetPlanoConta(PlanoContas.RecChequeDevCheque), GetPlanoConta(PlanoContas.ChequeDevolvido),
                        GetPlanoConta(PlanoContas.EstornoDepositoCheque)
                    };

                    if (cxDiario)
                        lst.Add(GetPlanoConta(PlanoContas.TransfDeCaixaGeralCheques));
                    else
                        lst.Add(GetPlanoConta(PlanoContas.TransfDeCxDiarioCheque));

                    break;
                case Glass.Data.Model.Pagto.FormaPagto.Cartao:
                    {
                        if (ContasCartoes.Any(f => f.IdTipoCartao == tipoCartaoBoleto))
                            lst = ContasCartoes.Where(f => f.IdTipoCartao == tipoCartaoBoleto).FirstOrDefault().IdsContasRecebimento;
                        else
                        {
                            lst.AddRange(new List<uint>() { GetPlanoConta(PlanoContas.EntradaCartao), GetPlanoConta(PlanoContas.PrazoCartao),
                                GetPlanoConta(PlanoContas.RecPrazoCartao), GetPlanoConta(PlanoContas.VistaCartao),
                                GetPlanoConta(PlanoContas.RecChequeDevCartao), GetPlanoConta(PlanoContas.TransfDeCxDiarioCartao),
                                GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoCartao) });

                            foreach (var c in ContasCartoes)
                                lst.AddRange(c.IdsContasRecebimento);
                        }

                        break;
                    }
                case Glass.Data.Model.Pagto.FormaPagto.Boleto:
                    switch (tipoCartaoBoleto)
                    {
                        case (int)Utils.TipoBoleto.BancoBrasil:
                            lst = GetLstEntradaBoletoBancoBrasil(); break;
                        case (int)Utils.TipoBoleto.Lumen:
                            lst = GetLstEntradaBoletoLumen(); break;
                        case (int)Utils.TipoBoleto.Outros:
                            lst = GetLstEntradaBoletoOutros(); break;
                        case (int)Utils.TipoBoleto.Santander:
                            lst = GetLstEntradaBoletoSantander(); break;
                        default:
                            lst.AddRange(GetLstEntradaBoleto());
                            lst.AddRange(GetLstEntradaBoletoBancoBrasil());
                            lst.AddRange(GetLstEntradaBoletoLumen());
                            lst.AddRange(GetLstEntradaBoletoOutros());
                            lst.AddRange(GetLstEntradaBoletoSantander());
                            break;
                    }
                    break;


                case Glass.Data.Model.Pagto.FormaPagto.Construcard:
                    lst = new List<uint>() { GetPlanoConta(PlanoContas.EntradaConstrucard), GetPlanoConta(PlanoContas.RecPrazoConstrucard),
                        GetPlanoConta(PlanoContas.VistaConstrucard), GetPlanoConta(PlanoContas.RecChequeDevConstrucard),
                        GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoConstrucard) }; break;
                case Glass.Data.Model.Pagto.FormaPagto.Deposito:
                    lst = new List<uint>() { GetPlanoConta(PlanoContas.EntradaDeposito), GetPlanoConta(PlanoContas.PrazoDeposito),
                        GetPlanoConta(PlanoContas.RecPrazoDeposito), GetPlanoConta(PlanoContas.VistaDeposito),
                        GetPlanoConta(PlanoContas.RecChequeDevDeposito), GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoDeposito) }; break;
                case Glass.Data.Model.Pagto.FormaPagto.Permuta:
                    lst = new List<uint>() { GetPlanoConta(PlanoContas.EntradaPermuta), GetPlanoConta(PlanoContas.PrazoPermuta),
                        GetPlanoConta(PlanoContas.RecPrazoPermuta), GetPlanoConta(PlanoContas.VistaPermuta),
                        GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoPermuta) }; break;
                case Glass.Data.Model.Pagto.FormaPagto.Credito:
                    lst = new List<uint>() { GetPlanoConta(PlanoContas.EntradaCredito),
                        GetPlanoConta(PlanoContas.VistaCredito), GetPlanoConta(PlanoContas.RecPrazoCredito),
                        GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoCredito) }; break;
                default:
                    lst = new List<uint>(); break;
            }

            string idsConta = String.Empty;

            foreach (uint id in lst)
                idsConta += id + ",";

            return idsConta.TrimEnd(',');
        }

        /// <summary>
        /// Retorna listagem de idsConta com saída da forma de pagamento passada
        /// </summary>
        /// <param name="formaPagto"></param>
        /// <param name="tipoCartao"></param>
        /// <param name="cxDiario"></param>
        /// <param name="tipoSaida">0-Busca todas movimentações de saída, 1-Incluir transf. Cx. Diário para Cx. Geral, 2-Apenas Estornos</param>
        /// <returns></returns>
        public static string GetLstSaidaByFormaPagto(Glass.Data.Model.Pagto.FormaPagto formaPagto, uint tipoCartao, int tipoSaida)
        {
            switch (formaPagto)
            {
                case Glass.Data.Model.Pagto.FormaPagto.Dinheiro:
                    return GetPlanoConta(PlanoContas.EstornoDinheiro) + "," +
                        GetPlanoConta(PlanoContas.EstornoRecPrazoDinheiro) + "," +
                        GetPlanoConta(PlanoContas.EstornoEntradaDinheiro) + "," +
                        GetPlanoConta(PlanoContas.EstornoChequeDevDinheiro) + "," +
                        GetPlanoConta(PlanoContas.DevolucaoPagtoDinheiro) + "," +
                        GetPlanoConta(PlanoContas.RecChequeDevDinheiro) +
                        (tipoSaida != 2 ? "," + GetPlanoConta(PlanoContas.PagtoDinheiro) + "," +
                            GetPlanoConta(PlanoContas.TransfCxGeralParaContaBancariaDinheiro) : "") +
                        (tipoSaida == 1 ? "," + GetPlanoConta(PlanoContas.TransfParaCxGeralDinheiro) : "");
                case Glass.Data.Model.Pagto.FormaPagto.ChequeProprio:
                case Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro:
                    return GetPlanoConta(PlanoContas.EstornoCheque) + "," +
                        GetPlanoConta(PlanoContas.EstornoRecPrazoCheque) + "," +
                        GetPlanoConta(PlanoContas.EstornoEntradaCheque) + "," +
                        GetPlanoConta(PlanoContas.EstornoChequeDevCheque) +
                        (tipoSaida != 2 ? "," + GetPlanoConta(PlanoContas.TransfBancoCheques) + "," +
                            GetPlanoConta(PlanoContas.PagtoChequeTerceiros) + "," +
                            GetPlanoConta(PlanoContas.ChequeQuitado) + "," +
                            GetPlanoConta(PlanoContas.ChequeTrocado) + "," +
                            GetPlanoConta(PlanoContas.DevolucaoPagtoCheque) : "") +
                        (tipoSaida == 1 ? "," + GetPlanoConta(PlanoContas.TransfParaCxGeralCheque) : "");
                case Glass.Data.Model.Pagto.FormaPagto.Cartao:
                    {
                        var aux = ContasCartoes.Where(f => f.IdTipoCartao == tipoCartao).FirstOrDefault();

                        if(aux != null)
                        {
                            return aux.IdContaEstorno + "," +
                                aux.IdContaEstornoRecPrazo + "," +
                                aux.IdContaEstornoEntrada + "," +
                                aux.IdContaEstornoChequeDev + 
                                (tipoSaida != 2 ? "," + aux.IdContaDevolucaoPagto : "");
                        }
                        else
                        {
                            var lst = new List<uint>();

                            lst.AddRange(ContasCartoes.Select(f => (uint)f.IdContaEstorno));
                            lst.AddRange(ContasCartoes.Select(f => (uint)f.IdContaEstornoRecPrazo));
                            lst.AddRange(ContasCartoes.Select(f => (uint)f.IdContaEstornoEntrada));
                            lst.AddRange(ContasCartoes.Select(f => (uint)f.IdContaEstornoChequeDev));

                            if (tipoSaida != 2)
                            {
                                lst.AddRange(ContasCartoes.Select(f => (uint)f.IdContaDevolucaoPagto));
                                lst.Add(GetPlanoConta(PlanoContas.EstornoCartao));
                                lst.Add(GetPlanoConta(PlanoContas.DevolucaoPagtoCartao));
                            }

                            return string.Join(",", lst);
                        }
                    }
                case Glass.Data.Model.Pagto.FormaPagto.Boleto:
                    return GetPlanoConta(PlanoContas.EstornoBoleto) + "," + GetPlanoConta(PlanoContas.EstornoEntradaBoleto) + "," +
                        GetPlanoConta(PlanoContas.EstornoChequeDevBoleto) + "," +
                        GetPlanoConta(PlanoContas.EstornoRecPrazoBoleto) +
                        (tipoSaida != 2 ? "," + GetPlanoConta(PlanoContas.DevolucaoPagtoBoleto) + "," +
                            GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoBancoBrasil) + "," +
                            GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoLumen) + "," +
                            GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoOutros) + "," +
                            GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoSantander) : "");
                case Glass.Data.Model.Pagto.FormaPagto.Permuta:
                    return GetPlanoConta(PlanoContas.EstornoPermuta) + "," + GetPlanoConta(PlanoContas.EstornoEntradaPermuta) + "," +
                        GetPlanoConta(PlanoContas.EstornoRecPrazoPermuta) + "," +
                        GetPlanoConta(PlanoContas.EstornoChequeDevPermuta) +
                        (tipoSaida != 2 ? "," + GetPlanoConta(PlanoContas.DevolucaoPagtoPermuta) : "");
                case Glass.Data.Model.Pagto.FormaPagto.Deposito:
                    return GetPlanoConta(PlanoContas.EstornoDeposito) + "," + GetPlanoConta(PlanoContas.EstornoEntradaDeposito) + "," +
                        GetPlanoConta(PlanoContas.EstornoRecPrazoDeposito) + "," +
                        GetPlanoConta(PlanoContas.EstornoChequeDevDeposito) +
                        (tipoSaida != 2 ? "," + GetPlanoConta(PlanoContas.DevolucaoPagtoDeposito) : "");
                case Glass.Data.Model.Pagto.FormaPagto.Construcard:
                    return GetPlanoConta(PlanoContas.EstornoConstrucard) + "," + GetPlanoConta(PlanoContas.EstornoEntradaConstrucard) + "," +
                        GetPlanoConta(PlanoContas.EstornoRecPrazoConstrucard) + "," +
                        GetPlanoConta(PlanoContas.EstornoChequeDevConstrucard) +
                        (tipoSaida != 2 ? "," + GetPlanoConta(PlanoContas.DevolucaoPagtoConstrucard) : "");
                case Glass.Data.Model.Pagto.FormaPagto.Credito:
                    return GetPlanoConta(PlanoContas.EstornoCredito) + "," + GetPlanoConta(PlanoContas.EstornoEntradaCredito) + "," +
                        GetPlanoConta(PlanoContas.EstornoRecPrazoCredito) + "," +
                        GetPlanoConta(PlanoContas.EstornoChequeDevCredito) +
                        (tipoSaida != 2 ? "," + GetPlanoConta(PlanoContas.DevolucaoPagtoCredito) : "");
                default:
                    return String.Empty;
            }
        }

        public static string GetLstEstornoSaidaByFormaPagto(Glass.Data.Model.Pagto.FormaPagto formaPagto, uint tipoCartaoBoleto)
        {
            List<uint> lst = new List<uint>();

            switch (formaPagto)
            {
                case Glass.Data.Model.Pagto.FormaPagto.Dinheiro:
                    lst = new List<uint>() {
                    GetPlanoConta(PlanoContas.EstornoPagtoDinheiro), GetPlanoConta(PlanoContas.EstornoRetiradaDinheiro),
                    GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoDinheiro) };
                    break;
                case Glass.Data.Model.Pagto.FormaPagto.ChequeProprio:
                case Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro:
                    lst = new List<uint>() {
                    GetPlanoConta(PlanoContas.EstornoRetiradaCheque), GetPlanoConta(PlanoContas.EstornoPagtoChequeTerceiros),
                    GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoCheque) };
                    break;
                default:
                    lst = new List<uint>(); break;
            }

            string idsConta = String.Empty;

            foreach (uint id in lst)
                idsConta += id + ",";

            return idsConta.TrimEnd(',');
        }

        public static List<uint> GetLstEntradaBoleto()
        {
            return new List<uint>() { GetPlanoConta(PlanoContas.EntradaBoleto), GetPlanoConta(PlanoContas.PrazoBoleta),
                GetPlanoConta(PlanoContas.RecPrazoBoleto), GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoBoleto),
                GetPlanoConta(PlanoContas.RecChequeDevBoleto) };
        }

        public static List<uint> GetLstEntradaBoletoBancoBrasil()
        {
            return new List<uint>() { GetPlanoConta(PlanoContas.RecPrazoBoletoBancoBrasil), GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoBoletoBancoBrasil) };
        }

        public static List<uint> GetLstEntradaBoletoLumen()
        {
            return new List<uint>() { GetPlanoConta(PlanoContas.RecPrazoBoletoLumen), GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoBoletoLumen) };
        }

        public static List<uint> GetLstEntradaBoletoOutros()
        {
            return new List<uint>() { GetPlanoConta(PlanoContas.RecPrazoBoletoOutros), GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoBoletoOutros) };
        }

        public static List<uint> GetLstEntradaBoletoSantander()
        {
            return new List<uint>() { GetPlanoConta(PlanoContas.RecPrazoBoletoSantander), GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoBoletoSantander) };
        }

        #endregion

        #region Planos de conta de crédito

        /// <summary>
        /// Retorna a lista de planos de conta usados para crédito.
        /// </summary>
        /// <param name="tipo">0-Todos, 1-Estorno, 2-Gerado, 3-Utilizado, 4-Estorno Crédito Recebido, 5-Estorno Crédito Gerado, 6-Todos Exceto Crédito Fornecedor</param>
        /// <returns></returns>
        public static List<uint> GetLstCredito(int tipo)
        {
            List<uint> retorno = new List<uint>();

            if (tipo == 0 || tipo == 6 || tipo == 1)
            {
                retorno.AddRange(new uint[] { GetPlanoConta(PlanoContas.EstornoCredito), GetPlanoConta(PlanoContas.EstornoRecPrazoCredito),
                    GetPlanoConta(PlanoContas.EstornoEntradaCredito), GetPlanoConta(PlanoContas.EstornoCreditoCompraGerado),
                    GetPlanoConta(PlanoContas.EstornoCreditoVendaGerado), GetPlanoConta(PlanoContas.EstornoCreditoEntradaGerado),
                    GetPlanoConta(PlanoContas.EstornoCreditoRecPrazoGerado), GetPlanoConta(PlanoContas.CreditoObraEstorno),
                    GetPlanoConta(PlanoContas.EstornoChequeDevCredito), GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoCredito),
                    GetPlanoConta(PlanoContas.CreditoAntencipFornecEstorno), GetPlanoConta(PlanoContas.EstornoRecObraCredito) });
            }

            if (tipo == 0 || tipo == 6 || tipo == 2)
            {
                retorno.AddRange(new uint[] { GetPlanoConta(PlanoContas.CreditoCompraGerado), GetPlanoConta(PlanoContas.CreditoVendaGerado),
                    GetPlanoConta(PlanoContas.CreditoRecPrazoGerado), GetPlanoConta(PlanoContas.CreditoEntradaGerado),
                    GetPlanoConta(PlanoContas.CreditoObraGerado) });
            }

            if (tipo == 0 || tipo == 6 || tipo == 3)
            {
                retorno.AddRange(new uint[] { GetPlanoConta(PlanoContas.EntradaCredito), GetPlanoConta(PlanoContas.VistaCredito),
                    GetPlanoConta(PlanoContas.PagtoCreditoFornecedor), GetPlanoConta(PlanoContas.RecPrazoCredito),
                    GetPlanoConta(PlanoContas.RecChequeDevCredito), GetPlanoConta(PlanoContas.DevolucaoPagtoCredito),
                    GetPlanoConta(PlanoContas.RecObraCredito)});
            }

            if (tipo == 4)
            {
                retorno.AddRange(new uint[] { GetPlanoConta(PlanoContas.EstornoCredito), GetPlanoConta(PlanoContas.EstornoRecPrazoCredito),
                    GetPlanoConta(PlanoContas.EstornoEntradaCredito), GetPlanoConta(PlanoContas.EstornoChequeDevCredito),
                    GetPlanoConta(PlanoContas.EstornoPagtoCreditoFornecedor)
                });
            }

            if (tipo == 5)
            {
                retorno.AddRange(new uint[] { GetPlanoConta(PlanoContas.EstornoCreditoCompraGerado),
                    GetPlanoConta(PlanoContas.EstornoCreditoVendaGerado), GetPlanoConta(PlanoContas.EstornoCreditoEntradaGerado),
                    GetPlanoConta(PlanoContas.EstornoCreditoRecPrazoGerado), GetPlanoConta(PlanoContas.CreditoObraEstorno),
                    GetPlanoConta(PlanoContas.CreditoAntencipFornecEstorno) });
            }

            #region Crédito Fornecedor

            if (tipo == 0 || tipo == 1)
            {
                retorno.AddRange(new uint[] { GetPlanoConta(PlanoContas.EstornoPagtoCreditoFornecedor),
                    GetPlanoConta(PlanoContas.EstornoCreditoFornecDinheiro), GetPlanoConta(PlanoContas.EstornoCreditoFornecChequeTerceiros),
                    GetPlanoConta(PlanoContas.EstornoCreditoFornecTransfBancaria), GetPlanoConta(PlanoContas.EstornoCreditoFornecBoleto),
                    GetPlanoConta(PlanoContas.EstornoCreditoFornecPermuta), GetPlanoConta(PlanoContas.EstornoCreditoFornecChequeProprio) });
            }

            if (tipo == 0 || tipo == 2)
            {
                retorno.AddRange(new uint[] { GetPlanoConta(PlanoContas.CreditoFornecDinheiro),
                    GetPlanoConta(PlanoContas.CreditoFornecChequeTerceiros), GetPlanoConta(PlanoContas.CreditoFornecTransfBanco),
                    GetPlanoConta(PlanoContas.CreditoFornecBoleto), GetPlanoConta(PlanoContas.CreditoFornecPermuta),
                    GetPlanoConta(PlanoContas.CreditoFornecChequeProprio), GetPlanoConta(PlanoContas.CreditoAntecipFornecGerado) });
            }

            if (tipo == 5)
            {
                retorno.AddRange(new uint[] {
                    GetPlanoConta(PlanoContas.EstornoCreditoFornecDinheiro), GetPlanoConta(PlanoContas.EstornoCreditoFornecChequeTerceiros),
                    GetPlanoConta(PlanoContas.EstornoCreditoFornecTransfBancaria), GetPlanoConta(PlanoContas.EstornoCreditoFornecBoleto),
                    GetPlanoConta(PlanoContas.EstornoCreditoFornecPermuta), GetPlanoConta(PlanoContas.EstornoCreditoFornecChequeProprio) });
            }

            #endregion

            return retorno;
        }

        /// <summary>
        /// Retorna a lista de planos de conta usados para crédito separados por vírgula.
        /// </summary>
        /// <param name="tipo">0-Todos, 1-Estorno, 2-Gerado, 3-Utilizado, 4-Estorno Crédito Recebido, 5-Estorno Crédito Gerado, 6-Todos Exceto Crédito Fornecedor</param>
        /// <returns></returns>
        public static string ContasCredito(int tipo)
        {
            return String.Join(",", Array.ConvertAll<uint, string>(GetLstCredito(tipo).ToArray(), x => x.ToString()));
        }

        #endregion

        #region Planos de conta para resumo diário

        public static string ResumoDiarioContasCheque()
        {
            var entradaCheque = GetPlanoConta(PlanoContas.EntradaCheque);
            var vistaCheque = GetPlanoConta(PlanoContas.VistaCheque);
            var estornoCheque = GetPlanoConta(PlanoContas.EstornoCheque);
            var estornoRecPrazoCheque = GetPlanoConta(PlanoContas.EstornoRecPrazoCheque);
            var estornoEntradaCheque = GetPlanoConta(PlanoContas.EstornoEntradaCheque);
            var prazoCheque = GetPlanoConta(PlanoContas.PrazoCheque);
            var recPrazoCheque = GetPlanoConta(PlanoContas.RecPrazoCheque);
            var recChequeDevCheque = GetPlanoConta(PlanoContas.RecChequeDevCheque);
            var estornoChequeDevCheque = GetPlanoConta(PlanoContas.EstornoChequeDevCheque);

            var retorno = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}",
                entradaCheque > 0 ? entradaCheque + "," : string.Empty,
                vistaCheque > 0 ? vistaCheque + "," : string.Empty,
                estornoCheque > 0 ? estornoCheque + "," : string.Empty,
                estornoRecPrazoCheque > 0 ? estornoRecPrazoCheque + "," : string.Empty,
                estornoEntradaCheque > 0 ? estornoEntradaCheque + "," : string.Empty,
                prazoCheque > 0 ? prazoCheque + "," : string.Empty,
                recPrazoCheque > 0 ? recPrazoCheque + "," : string.Empty,
                recChequeDevCheque > 0 ? recChequeDevCheque + "," : string.Empty,
                estornoChequeDevCheque > 0 ? estornoChequeDevCheque.ToString() : "0");

            return retorno;
        }

        public static string ResumoDiarioContasDinheiro()
        {
            var entradaDinheiro = GetPlanoConta(PlanoContas.EntradaDinheiro);
            var estornoDinheiro = GetPlanoConta(PlanoContas.EstornoDinheiro);
            var estornoRecPrazoDinheiro = GetPlanoConta(PlanoContas.EstornoRecPrazoDinheiro);
            var estornoEntradaDinheiro = GetPlanoConta(PlanoContas.EstornoEntradaDinheiro);
            var recPrazoDinheiro = GetPlanoConta(PlanoContas.RecPrazoDinheiro);
            var vistaDinheiro = GetPlanoConta(PlanoContas.VistaDinheiro);
            var recChequeDevDinheiro = GetPlanoConta(PlanoContas.RecChequeDevDinheiro);
            var estornoChequeDevDinheiro = GetPlanoConta(PlanoContas.EstornoChequeDevDinheiro);

            var retorno = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}",
                entradaDinheiro > 0 ? entradaDinheiro + "," : string.Empty,
                estornoDinheiro > 0 ? estornoDinheiro + "," : string.Empty,
                estornoRecPrazoDinheiro > 0 ? estornoRecPrazoDinheiro + "," : string.Empty,
                estornoEntradaDinheiro > 0 ? estornoEntradaDinheiro + "," : string.Empty,
                recPrazoDinheiro > 0 ? recPrazoDinheiro + "," : string.Empty,
                vistaDinheiro > 0 ? vistaDinheiro + "," : string.Empty,
                recChequeDevDinheiro > 0 ? recChequeDevDinheiro + "," : string.Empty,
                estornoChequeDevDinheiro > 0 ? estornoChequeDevDinheiro.ToString() : "0");

            return retorno;
        }

        public static string ResumoDiarioContasCartao(bool isDebito)
        {
            if (isDebito)
            {
                var recChequeDevCartao = GetPlanoConta(PlanoContas.RecChequeDevCartao);
                var estornoChequeDevCartao = GetPlanoConta(PlanoContas.EstornoChequeDevCartao);
                var debito = ContasTipoCartao(TipoCartaoEnum.Debito);

                var retorno = string.Format("{0}{1}{2}",
                    recChequeDevCartao > 0 ? recChequeDevCartao + "," : string.Empty,
                    estornoChequeDevCartao > 0 ? estornoChequeDevCartao + "," : string.Empty,
                    !string.IsNullOrEmpty(debito) ? debito : "0");

                return retorno;
            }
            else
            {
                var entradaCartao = GetPlanoConta(PlanoContas.EntradaCartao);
                var estornoCartao = GetPlanoConta(PlanoContas.EstornoCartao);
                var prazoCartao = GetPlanoConta(PlanoContas.PrazoCartao);
                var recPrazoCartao = GetPlanoConta(PlanoContas.RecPrazoCartao);
                var estornoRecPrazoCartao = GetPlanoConta(PlanoContas.EstornoRecPrazoCartao);
                var estornoEntradaCartao = GetPlanoConta(PlanoContas.EstornoEntradaCartao);
                var vistaCartao = GetPlanoConta(PlanoContas.VistaCartao);
                var credito = ContasTipoCartao(TipoCartaoEnum.Credito);

                var retorno = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}",
                    entradaCartao > 0 ? entradaCartao + "," : string.Empty,
                    estornoCartao > 0 ? estornoCartao + "," : string.Empty,
                    prazoCartao > 0 ? prazoCartao + "," : string.Empty,
                    recPrazoCartao > 0 ? recPrazoCartao + "," : string.Empty,
                    estornoRecPrazoCartao > 0 ? estornoRecPrazoCartao + "," : string.Empty,
                    estornoEntradaCartao > 0 ? estornoEntradaCartao + "," : string.Empty,
                    vistaCartao > 0 ? vistaCartao + "," : string.Empty,
                    !string.IsNullOrEmpty(credito) ? credito : "0");

                return retorno;
            }
        }

        public static string ResumoDiarioContasConstrucard()
        {
            var devolucaoPagtoConstrucard = GetPlanoConta(PlanoContas.DevolucaoPagtoConstrucard);
            var entradaConstrucard = GetPlanoConta(PlanoContas.EntradaConstrucard);
            var estornoChequeDevConstrucard = GetPlanoConta(PlanoContas.EstornoChequeDevConstrucard);
            var estornoConstrucard = GetPlanoConta(PlanoContas.EstornoConstrucard);
            var estornoDevolucaoPagtoConstrucard = GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoConstrucard);
            var estornoEntradaConstrucard = GetPlanoConta(PlanoContas.EstornoEntradaConstrucard);
            var estornoJurosVendaConstrucard = GetPlanoConta(PlanoContas.EstornoJurosVendaConstrucard);
            var estornoRecPrazoConstrucard = GetPlanoConta(PlanoContas.EstornoRecPrazoConstrucard);
            var funcConstrucard = GetPlanoConta(PlanoContas.FuncConstrucard);
            var jurosVendaConstrucard = GetPlanoConta(PlanoContas.JurosVendaConstrucard);
            var prazoConstrucard = GetPlanoConta(PlanoContas.PrazoConstrucard);
            var recChequeDevConstrucard = GetPlanoConta(PlanoContas.RecChequeDevConstrucard);
            var recPrazoConstrucard = GetPlanoConta(PlanoContas.RecPrazoConstrucard);
            var transfDeCxDiarioConstrucard = GetPlanoConta(PlanoContas.TransfDeCxDiarioConstrucard);
            var vistaConstrucard = GetPlanoConta(PlanoContas.VistaConstrucard);

            var retorno = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}",
                devolucaoPagtoConstrucard > 0 ? devolucaoPagtoConstrucard + "," : string.Empty,
                entradaConstrucard > 0 ? entradaConstrucard + "," : string.Empty,
                estornoChequeDevConstrucard > 0 ? estornoChequeDevConstrucard + "," : string.Empty,
                estornoConstrucard > 0 ? estornoConstrucard + "," : string.Empty,
                estornoDevolucaoPagtoConstrucard > 0 ? estornoDevolucaoPagtoConstrucard + "," : string.Empty,
                estornoEntradaConstrucard > 0 ? estornoEntradaConstrucard + "," : string.Empty,
                estornoJurosVendaConstrucard > 0 ? estornoJurosVendaConstrucard + "," : string.Empty,
                estornoRecPrazoConstrucard > 0 ? estornoRecPrazoConstrucard + "," : string.Empty,
                funcConstrucard > 0 ? funcConstrucard + "," : string.Empty,
                jurosVendaConstrucard > 0 ? jurosVendaConstrucard + "," : string.Empty,
                prazoConstrucard > 0 ? prazoConstrucard + "," : string.Empty,
                recChequeDevConstrucard > 0 ? recChequeDevConstrucard + "," : string.Empty,
                recPrazoConstrucard > 0 ? recPrazoConstrucard + "," : string.Empty,
                transfDeCxDiarioConstrucard > 0 ? transfDeCxDiarioConstrucard + "," : string.Empty,
                vistaConstrucard > 0 ? vistaConstrucard.ToString() : "0");

            return retorno;
        }

        public static string ResumoDiarioContasCredito()
        {
            var entradaCredito = GetPlanoConta(PlanoContas.EntradaCredito);
            var estornoCredito = GetPlanoConta(PlanoContas.EstornoCredito);
            var estornoRecPrazoCredito = GetPlanoConta(PlanoContas.EstornoRecPrazoCredito);
            var estornoEntradaCredito = GetPlanoConta(PlanoContas.EstornoEntradaCredito);
            var eecPrazoCredito = GetPlanoConta(PlanoContas.RecPrazoCredito);
            var vistaCredito = GetPlanoConta(PlanoContas.VistaCredito);
            var recChequeDevCredito = GetPlanoConta(PlanoContas.RecChequeDevCredito);
            var estornoChequeDevCredito = GetPlanoConta(PlanoContas.EstornoChequeDevCredito);

            var retorno = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}",
                entradaCredito > 0 ? entradaCredito + "," : string.Empty,
                estornoCredito > 0 ? estornoCredito + "," : string.Empty,
                estornoRecPrazoCredito > 0 ? estornoRecPrazoCredito + "," : string.Empty,
                estornoEntradaCredito > 0 ? estornoEntradaCredito + "," : string.Empty,
                eecPrazoCredito > 0 ? eecPrazoCredito + "," : string.Empty,
                vistaCredito > 0 ? vistaCredito + "," : string.Empty,
                recChequeDevCredito > 0 ? recChequeDevCredito + "," : string.Empty,
                estornoChequeDevCredito > 0 ? estornoChequeDevCredito.ToString() : "0");

            return retorno;
        }

        public static string ResumoDiarioContasCreditoGerado()
        {
            var creditoVendaGerado = GetPlanoConta(PlanoContas.CreditoVendaGerado);
            var creditoRecPrazoGerado = GetPlanoConta(PlanoContas.CreditoRecPrazoGerado);
            var creditoEntradaGerado = GetPlanoConta(PlanoContas.CreditoEntradaGerado);
            var creditoObraGerado = GetPlanoConta(PlanoContas.CreditoObraGerado);

            var retorno = string.Format("{0}{1}{2}{3}",
                creditoVendaGerado > 0 ? creditoVendaGerado + "," : string.Empty,
                creditoRecPrazoGerado > 0 ? creditoRecPrazoGerado + "," : string.Empty,
                creditoEntradaGerado > 0 ? creditoEntradaGerado + "," : string.Empty,
                creditoObraGerado > 0 ? creditoObraGerado.ToString() : "0");

            return retorno;
        }

        public static string ResumoDiarioContasTroca()
        {
            var entradaPermuta = GetPlanoConta(PlanoContas.EntradaPermuta);
            var estornoPermuta = GetPlanoConta(PlanoContas.EstornoPermuta);
            var estornoRecPrazoPermuta = GetPlanoConta(PlanoContas.EstornoRecPrazoPermuta);
            var estornoEntradaPermuta = GetPlanoConta(PlanoContas.EstornoEntradaPermuta);
            var recPrazoPermuta = GetPlanoConta(PlanoContas.RecPrazoPermuta);
            var vistaPermuta = GetPlanoConta(PlanoContas.VistaPermuta);
            var estornoChequeDevPermuta = GetPlanoConta(PlanoContas.EstornoChequeDevPermuta);

            var retorno = string.Format("{0}{1}{2}{3}{4}{5}{6}",
                entradaPermuta > 0 ? entradaPermuta + "," : string.Empty,
                estornoPermuta > 0 ? estornoPermuta + "," : string.Empty,
                estornoRecPrazoPermuta > 0 ? estornoRecPrazoPermuta + "," : string.Empty,
                estornoEntradaPermuta > 0 ? estornoEntradaPermuta + "," : string.Empty,
                recPrazoPermuta > 0 ? recPrazoPermuta + "," : string.Empty,
                vistaPermuta > 0 ? vistaPermuta + "," : string.Empty,
                estornoChequeDevPermuta > 0 ? estornoChequeDevPermuta.ToString() : "0");

            return retorno;
        }

        public static string ResumoDiarioContasPagtoChequeDevolvido()
        {
            var recChequeDevCartao = GetPlanoConta(PlanoContas.RecChequeDevCartao);
            var recChequeDevCheque = GetPlanoConta(PlanoContas.RecChequeDevCheque);
            var recChequeDevConstrucard = GetPlanoConta(PlanoContas.RecChequeDevConstrucard);
            var recChequeDevCredito = GetPlanoConta(PlanoContas.RecChequeDevCredito);
            var recChequeDevDeposito = GetPlanoConta(PlanoContas.RecChequeDevDeposito);
            var recChequeDevDinheiro = GetPlanoConta(PlanoContas.RecChequeDevDinheiro);
            var contasTipoChequeDevCartao = ContasTipoChequeDevCartao();
            var estornoChequeDevDinheiro = GetPlanoConta(PlanoContas.EstornoChequeDevDinheiro);
            var estornoChequeDevCheque = GetPlanoConta(PlanoContas.EstornoChequeDevCheque);
            var estornoChequeDevCartao = GetPlanoConta(PlanoContas.EstornoChequeDevCartao);
            var estornoChequeDevConstrucard = GetPlanoConta(PlanoContas.EstornoChequeDevConstrucard);
            var estornoChequeDevCredito = GetPlanoConta(PlanoContas.EstornoChequeDevCredito);
            var estornoChequeDevDeposito = GetPlanoConta(PlanoContas.EstornoChequeDevDeposito);
            var estornoChequeDevPermuta = GetPlanoConta(PlanoContas.EstornoChequeDevPermuta);
            var estornoChequeDevBoleto = GetPlanoConta(PlanoContas.EstornoChequeDevBoleto);
            var estornoChequeDevBoletoBancoBrasil = GetPlanoConta(PlanoContas.EstornoChequeDevBoletoBancoBrasil);
            var estornoChequeDevBoletoLumen = GetPlanoConta(PlanoContas.EstornoChequeDevBoletoLumen);
            var estornoChequeDevBoletoOutros = GetPlanoConta(PlanoContas.EstornoChequeDevBoletoOutros);
            var estornoChequeDevBoletoSantander = GetPlanoConta(PlanoContas.EstornoChequeDevBoletoSantander);

            var retorno = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}",
                recChequeDevCartao > 0 ? recChequeDevCartao + "," : string.Empty,
                recChequeDevCheque > 0 ? recChequeDevCheque + "," : string.Empty,
                recChequeDevConstrucard > 0 ? recChequeDevConstrucard + "," : string.Empty,
                recChequeDevCredito > 0 ? recChequeDevCredito + "," : string.Empty,
                recChequeDevDeposito > 0 ? recChequeDevDeposito + "," : string.Empty,
                recChequeDevDinheiro > 0 ? recChequeDevDinheiro + "," : string.Empty,
                !string.IsNullOrEmpty(contasTipoChequeDevCartao) ? contasTipoChequeDevCartao + "," : string.Empty,
                estornoChequeDevDinheiro > 0 ? estornoChequeDevDinheiro + "," : string.Empty,
                estornoChequeDevCheque > 0 ? estornoChequeDevCheque + "," : string.Empty,
                estornoChequeDevCartao > 0 ? estornoChequeDevCartao + "," : string.Empty,
                estornoChequeDevConstrucard > 0 ? estornoChequeDevConstrucard + "," : string.Empty,
                estornoChequeDevCredito > 0 ? estornoChequeDevCredito + "," : string.Empty,
                estornoChequeDevDeposito > 0 ? estornoChequeDevDeposito + "," : string.Empty,
                estornoChequeDevPermuta > 0 ? estornoChequeDevPermuta + "," : string.Empty,
                estornoChequeDevBoleto > 0 ? estornoChequeDevBoleto + "," : string.Empty,
                estornoChequeDevBoletoBancoBrasil > 0 ? estornoChequeDevBoletoBancoBrasil + "," : string.Empty,
                estornoChequeDevBoletoLumen > 0 ? estornoChequeDevBoletoLumen + "," : string.Empty,
                estornoChequeDevBoletoOutros > 0 ? estornoChequeDevBoletoOutros + "," : string.Empty,
                estornoChequeDevBoletoSantander > 0 ? estornoChequeDevBoletoSantander.ToString() : "0");

            return retorno;
        }

        public static string ResumoDiarioContasPagtoChequeDevolvidoDinheiro()
        {
            var recChequeDevDinheiro = GetPlanoConta(PlanoContas.RecChequeDevDinheiro);
            var estornoChequeDevDinheiro = GetPlanoConta(PlanoContas.EstornoChequeDevDinheiro);

            var retorno = string.Format("{0}{1}",
                recChequeDevDinheiro > 0 ? recChequeDevDinheiro + "," : string.Empty,
                estornoChequeDevDinheiro > 0 ? estornoChequeDevDinheiro.ToString() : "0");

            return retorno;
        }

        public static string ResumoDiarioContasPagtoChequeDevolvidoCheque()
        {
            var recChequeDevCheque = GetPlanoConta(PlanoContas.RecChequeDevCheque);
            var estornoChequeDevCheque = GetPlanoConta(PlanoContas.EstornoChequeDevCheque);

            var retorno = string.Format("{0}{1}",
                recChequeDevCheque > 0 ? recChequeDevCheque + "," : string.Empty,
                estornoChequeDevCheque > 0 ? estornoChequeDevCheque.ToString() : "0");

            return retorno;
        }

        public static string ResumoDiarioContasPagtoChequeDevolvidoOutros()
        {
            var recChequeDevCredito = FinanceiroConfig.RelatorioResumoDiario.PagamentoChequeDevolvidoOutrosConsiderarCredito ?
                GetPlanoConta(PlanoContas.RecChequeDevCredito) : 0;
            var estornoChequeDevCredito = FinanceiroConfig.RelatorioResumoDiario.PagamentoChequeDevolvidoOutrosConsiderarCredito ?
                GetPlanoConta(PlanoContas.EstornoChequeDevCredito) : 0;
            var recChequeDevCartao = GetPlanoConta(PlanoContas.RecChequeDevCartao);
            var recChequeDevConstrucard = GetPlanoConta(PlanoContas.RecChequeDevConstrucard);
            var recChequeDevDeposito = GetPlanoConta(PlanoContas.RecChequeDevDeposito);
            var estornoChequeDevCartao = GetPlanoConta(PlanoContas.EstornoChequeDevCartao);
            var estornoChequeDevConstrucard = GetPlanoConta(PlanoContas.EstornoChequeDevConstrucard);
            var estornoChequeDevDeposito = GetPlanoConta(PlanoContas.EstornoChequeDevDeposito);
            var estornoChequeDevPermuta = GetPlanoConta(PlanoContas.EstornoChequeDevPermuta);
            var estornoChequeDevBoleto = GetPlanoConta(PlanoContas.EstornoChequeDevBoleto);
            var estornoChequeDevBoletoBancoBrasil = GetPlanoConta(PlanoContas.EstornoChequeDevBoletoBancoBrasil);
            var estornoChequeDevBoletoLumen = GetPlanoConta(PlanoContas.EstornoChequeDevBoletoLumen);
            var estornoChequeDevBoletoOutros = GetPlanoConta(PlanoContas.EstornoChequeDevBoletoOutros);
            var estornoChequeDevBoletoSantander = GetPlanoConta(PlanoContas.EstornoChequeDevBoletoSantander);
            var contasChequeDev = ContasChequeDev();

            var retorno = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}",
                recChequeDevCredito > 0 ? recChequeDevCredito + "," : string.Empty,
                estornoChequeDevCredito > 0 ? estornoChequeDevCredito + "," : string.Empty,
                recChequeDevCartao > 0 ? recChequeDevCartao + "," : string.Empty,
                recChequeDevConstrucard > 0 ? recChequeDevConstrucard + "," : string.Empty,
                recChequeDevDeposito > 0 ? recChequeDevDeposito + "," : string.Empty,
                estornoChequeDevCartao > 0 ? estornoChequeDevCartao + "," : string.Empty,
                estornoChequeDevConstrucard > 0 ? estornoChequeDevConstrucard + "," : string.Empty,
                estornoChequeDevDeposito > 0 ? estornoChequeDevDeposito + "," : string.Empty,
                estornoChequeDevPermuta > 0 ? estornoChequeDevPermuta + "," : string.Empty,
                estornoChequeDevBoleto > 0 ? estornoChequeDevBoleto + "," : string.Empty,
                estornoChequeDevBoletoBancoBrasil > 0 ? estornoChequeDevBoletoBancoBrasil + "," : string.Empty,
                estornoChequeDevBoletoLumen > 0 ? estornoChequeDevBoletoLumen + "," : string.Empty,
                estornoChequeDevBoletoOutros > 0 ? estornoChequeDevBoletoOutros + "," : string.Empty,
                estornoChequeDevBoletoSantander > 0 ? estornoChequeDevBoletoSantander + "," : string.Empty,
                !string.IsNullOrEmpty(contasChequeDev) ? contasChequeDev : "0");

            return retorno;
        }

        public static string ResumoDiarioContasBoleto()
        {
            var entradaBoleto = GetPlanoConta(PlanoContas.EntradaBoleto);
            var estornoBoleto = GetPlanoConta(PlanoContas.EstornoBoleto);
            var estornoBoletoBancoBrasil = GetPlanoConta(PlanoContas.EstornoBoletoBancoBrasil);
            var estornoBoletoLumen = GetPlanoConta(PlanoContas.EstornoBoletoLumen);
            var estornoBoletoOutros = GetPlanoConta(PlanoContas.EstornoBoletoOutros);
            var estornoBoletoSantander = GetPlanoConta(PlanoContas.EstornoBoletoSantander);
            var estornoRecPrazoBoleto = GetPlanoConta(PlanoContas.EstornoRecPrazoBoleto);
            var estornoRecPrazoBoletoBancoBrasil = GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoBancoBrasil);
            var estornoRecPrazoBoletoLumen = GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoLumen);
            var estornoRecPrazoBoletoOutros = GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoOutros);
            var estornoRecPrazoBoletoSantander = GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoSantander);
            var estornoEntradaBoleto = GetPlanoConta(PlanoContas.EstornoEntradaBoleto);
            var estornoEntradaBoletoBancoBrasil = GetPlanoConta(PlanoContas.EstornoEntradaBoletoBancoBrasil);
            var estornoEntradaBoletoLumen = GetPlanoConta(PlanoContas.EstornoEntradaBoletoLumen);
            var estornoEntradaBoletoOutros = GetPlanoConta(PlanoContas.EstornoEntradaBoletoOutros);
            var estornoEntradaBoletoSantander = GetPlanoConta(PlanoContas.EstornoEntradaBoletoSantander);
            var prazoBoleta = GetPlanoConta(PlanoContas.PrazoBoleta);
            var recPrazoBoleto = GetPlanoConta(PlanoContas.RecPrazoBoleto);
            var recPrazoBoletoBancoBrasil = GetPlanoConta(PlanoContas.RecPrazoBoletoBancoBrasil);
            var recPrazoBoletoLumen = GetPlanoConta(PlanoContas.RecPrazoBoletoLumen);
            var recPrazoBoletoOutros = GetPlanoConta(PlanoContas.RecPrazoBoletoOutros);
            var recPrazoBoletoSantander = GetPlanoConta(PlanoContas.RecPrazoBoletoSantander);
            var estornoChequeDevBoleto = GetPlanoConta(PlanoContas.EstornoChequeDevBoleto);
            var estornoChequeDevBoletoBancoBrasil = GetPlanoConta(PlanoContas.EstornoChequeDevBoletoBancoBrasil);
            var estornoChequeDevBoletoOutros = GetPlanoConta(PlanoContas.EstornoChequeDevBoletoOutros);
            var estornoChequeDevBoletoLumen = GetPlanoConta(PlanoContas.EstornoChequeDevBoletoLumen);
            var estornoChequeDevBoletoSantander = GetPlanoConta(PlanoContas.EstornoChequeDevBoletoSantander);
            
            var retorno = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}{23}{24}{25}{26}",
                entradaBoleto > 0 ? entradaBoleto + "," : string.Empty,
                estornoBoleto > 0 ? estornoBoleto + "," : string.Empty,
                estornoBoletoBancoBrasil > 0 ? estornoBoletoBancoBrasil + "," : string.Empty,
                estornoBoletoLumen > 0 ? estornoBoletoLumen + "," : string.Empty,
                estornoBoletoOutros > 0 ? estornoBoletoOutros + "," : string.Empty,
                estornoBoletoSantander > 0 ? estornoBoletoSantander + "," : string.Empty,
                estornoRecPrazoBoleto > 0 ? estornoRecPrazoBoleto + "," : string.Empty,
                estornoRecPrazoBoletoBancoBrasil > 0 ? estornoRecPrazoBoletoBancoBrasil + "," : string.Empty,
                estornoRecPrazoBoletoLumen > 0 ? estornoRecPrazoBoletoLumen + "," : string.Empty,
                estornoRecPrazoBoletoOutros > 0 ? estornoRecPrazoBoletoOutros + "," : string.Empty,
                estornoRecPrazoBoletoSantander > 0 ? estornoRecPrazoBoletoSantander + "," : string.Empty,
                estornoEntradaBoleto > 0 ? estornoEntradaBoleto + "," : string.Empty,
                estornoEntradaBoletoBancoBrasil > 0 ? estornoEntradaBoletoBancoBrasil + "," : string.Empty,
                estornoEntradaBoletoLumen > 0 ? estornoEntradaBoletoLumen + "," : string.Empty,
                estornoEntradaBoletoOutros > 0 ? estornoEntradaBoletoOutros + "," : string.Empty,
                estornoEntradaBoletoSantander > 0 ? estornoEntradaBoletoSantander + "," : string.Empty,
                prazoBoleta > 0 ? prazoBoleta + "," : string.Empty,
                recPrazoBoleto > 0 ? recPrazoBoleto + "," : string.Empty,
                recPrazoBoletoBancoBrasil > 0 ? recPrazoBoletoBancoBrasil + "," : string.Empty,
                recPrazoBoletoLumen > 0 ? recPrazoBoletoLumen + "," : string.Empty,
                recPrazoBoletoOutros > 0 ? recPrazoBoletoOutros + "," : string.Empty,
                recPrazoBoletoSantander > 0 ? recPrazoBoletoSantander + "," : string.Empty,
                estornoChequeDevBoleto > 0 ? estornoChequeDevBoleto + "," : string.Empty,
                estornoChequeDevBoletoBancoBrasil > 0 ? estornoChequeDevBoletoBancoBrasil + "," : string.Empty,
                estornoChequeDevBoletoOutros > 0 ? estornoChequeDevBoletoOutros + "," : string.Empty,
                estornoChequeDevBoletoLumen > 0 ? estornoChequeDevBoletoLumen + "," : string.Empty,
                estornoChequeDevBoletoSantander > 0 ? estornoChequeDevBoletoSantander.ToString() : "0");

            return retorno;
        }

        public static string ResumoDiarioContasDeposito()
        {
            var entradaDeposito = GetPlanoConta(PlanoContas.EntradaDeposito);
            var estornoDeposito = GetPlanoConta(PlanoContas.EstornoDeposito);
            var estornoRecPrazoDeposito = GetPlanoConta(PlanoContas.EstornoRecPrazoDeposito);
            var prazoDeposito = GetPlanoConta(PlanoContas.PrazoDeposito);
            var recChequeDevDeposito = GetPlanoConta(PlanoContas.RecChequeDevDeposito);
            var recPrazoDeposito = GetPlanoConta(PlanoContas.RecPrazoDeposito);
            var vistaDeposito = GetPlanoConta(PlanoContas.VistaDeposito);
            var estornoEntradaDeposito = GetPlanoConta(PlanoContas.EstornoEntradaDeposito);
            var estornoChequeDevDeposito = GetPlanoConta(PlanoContas.EstornoChequeDevDeposito);

            var retorno = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}",
                entradaDeposito > 0 ? entradaDeposito + "," : string.Empty,
                estornoDeposito > 0 ? estornoDeposito + "," : string.Empty,
                estornoRecPrazoDeposito > 0 ? estornoRecPrazoDeposito + "," : string.Empty,
                prazoDeposito > 0 ? prazoDeposito + "," : string.Empty,
                recChequeDevDeposito > 0 ? recChequeDevDeposito + "," : string.Empty,
                recPrazoDeposito > 0 ? recPrazoDeposito + "," : string.Empty,
                vistaDeposito > 0 ? vistaDeposito + "," : string.Empty,
                estornoEntradaDeposito > 0 ? estornoEntradaDeposito + "," : string.Empty,
                estornoChequeDevDeposito > 0 ? estornoChequeDevDeposito.ToString() : "0");

            return retorno;
        }

        #endregion

        #region Planos de conta que não devem ser considerados no caixa geral

        /// <summary>
        /// Planos de conta que não devem ser considerados no caixa geral para montagem do DRE
        /// </summary>
        /// <returns></returns>
        public static string PlanosContaDesconsiderarCxGeral
        {
            get
            {
                string idsConta =
                    GetPlanoConta(PlanoContas.PagtoChequeProprio) + "," +

                    // Boleto
                    GetPlanoConta(PlanoContas.RecPrazoBoleto) + "," + GetPlanoConta(PlanoContas.RecPrazoBoletoBancoBrasil) + "," +
                    GetPlanoConta(PlanoContas.RecPrazoBoletoLumen) + "," + GetPlanoConta(PlanoContas.RecPrazoBoletoOutros) + "," +
                    GetPlanoConta(PlanoContas.RecPrazoBoletoSantander) + "," + GetPlanoConta(PlanoContas.EntradaBoleto) + "," +
                    GetPlanoConta(PlanoContas.EstornoBoleto) + "," + GetPlanoConta(PlanoContas.EstornoBoletoBancoBrasil) + "," +
                    GetPlanoConta(PlanoContas.EstornoBoletoLumen) + "," + GetPlanoConta(PlanoContas.EstornoBoletoOutros) + "," +
                    GetPlanoConta(PlanoContas.EstornoBoletoSantander) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoBoleto) + "," +
                    GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoBancoBrasil) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoLumen) + "," +
                    GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoOutros) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoBoletoSantander) + "," +
                    GetPlanoConta(PlanoContas.EstornoEntradaBoleto) + "," + GetPlanoConta(PlanoContas.EstornoEntradaBoletoBancoBrasil) + "," +
                    GetPlanoConta(PlanoContas.EstornoEntradaBoletoLumen) + "," + GetPlanoConta(PlanoContas.EstornoEntradaBoletoOutros) + "," +
                    GetPlanoConta(PlanoContas.EstornoEntradaBoletoSantander) + "," + GetPlanoConta(PlanoContas.PrazoBoleta) + "," +
                    GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoBoleto) + "," + GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoBoletoOutros) + "," +
                    GetPlanoConta(PlanoContas.DevolucaoPagtoBoleto) + "," + GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoOutros) + "," +
                    GetPlanoConta(PlanoContas.PagtoAntecipFornecBoleto) + "," +

                    // Deposito
                    GetPlanoConta(PlanoContas.EntradaDeposito) + "," + GetPlanoConta(PlanoContas.PrazoDeposito) + "," +
                    GetPlanoConta(PlanoContas.RecPrazoDeposito) + "," + GetPlanoConta(PlanoContas.VistaDeposito) + "," +
                    GetPlanoConta(PlanoContas.EstornoDeposito) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoDeposito) + "," +
                    GetPlanoConta(PlanoContas.EstornoEntradaDeposito) + "," + GetPlanoConta(PlanoContas.PrazoDeposito) + "," +
                    GetPlanoConta(PlanoContas.DevolucaoPagtoDeposito) + "," + GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoDeposito) + "," +
                    GetPlanoConta(PlanoContas.PagtoAntecipFornecDeposito) + "," +

                    // Cheques Devolvidos
                    GetPlanoConta(PlanoContas.EstornoChequeDevBoleto) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevBoletoBancoBrasil) + "," +
                    GetPlanoConta(PlanoContas.EstornoChequeDevBoletoLumen) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevBoletoOutros) + "," +
                    GetPlanoConta(PlanoContas.EstornoChequeDevBoletoSantander) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevDeposito) + "," +
                    GetPlanoConta(PlanoContas.RecChequeDevDeposito) + "," + GetPlanoConta(PlanoContas.RecChequeDevBoleto) + "," +
                    // O plano de contas "Cheque Devolvido" já é buscado através da movimentação bancária, portanto deve ser deconsiderado no caixa geral.
                    GetPlanoConta(PlanoContas.ChequeDevolvido) + "," +

                    //Contrucard
                    GetPlanoConta(PlanoContas.EntradaConstrucard) + "," + GetPlanoConta(PlanoContas.RecPrazoConstrucard) + "," +
                    GetPlanoConta(PlanoContas.VistaConstrucard) + "," + GetPlanoConta(PlanoContas.RecChequeDevConstrucard) + "," +
                    GetPlanoConta(PlanoContas.DevolucaoPagtoConstrucard) + "," + GetPlanoConta(PlanoContas.PrazoConstrucard) + "," +
                    GetPlanoConta(PlanoContas.EstornoConstrucard) + "," + GetPlanoConta(PlanoContas.EstornoRecPrazoConstrucard) + "," +
                    GetPlanoConta(PlanoContas.EstornoEntradaConstrucard) + "," + GetPlanoConta(PlanoContas.EstornoChequeDevConstrucard) + "," +
                    GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoConstrucard) + "," +

                    // Crédito Fornecedor
                    GetPlanoConta(PlanoContas.CreditoFornecTransfBanco) + "," + GetPlanoConta(PlanoContas.CreditoFornecBoleto) + "," +
                    GetPlanoConta(PlanoContas.EstornoCreditoFornecTransfBancaria) + "," + GetPlanoConta(PlanoContas.EstornoCreditoFornecBoleto) + "," +

                    // Cartão
                    GetPlanoConta(PlanoContas.EntradaCartao) + "," + 
                    GetPlanoConta(PlanoContas.EstornoCartao) + "," + 
                    GetPlanoConta(PlanoContas.EstornoRecPrazoCartao) + "," + 
                    GetPlanoConta(PlanoContas.EstornoEntradaCartao) + "," + 
                    GetPlanoConta(PlanoContas.DevolucaoPagtoCartao) + "," + 
                    GetPlanoConta(PlanoContas.RecPrazoCartao) + "," + 
                    GetPlanoConta(PlanoContas.VistaCartao) + "," +
                    GetPlanoConta(PlanoContas.PrazoCartao) + "," + 
                    GetPlanoConta(PlanoContas.RecChequeDevCartao) + "," +
                    GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoCartao) + "," + 
                    GetPlanoConta(PlanoContas.EstornoChequeDevCartao) +

                    /* Chamado 45147. */
                    (!string.IsNullOrEmpty(ContasTodosTiposCartao()) ? "," + ContasTodosTiposCartao() : string.Empty);

                return idsConta.Trim(',');
            }
        }

        #endregion

        #region Planos de conta para venda a funcionário

        public static uint GetPlanoContaVendaFunc(uint idFormaPagto, uint? idTipoCartao)
        {
            switch ((Pagto.FormaPagto)idFormaPagto)
            {
                case Pagto.FormaPagto.Boleto: return GetPlanoConta(PlanoContas.FuncBoleto);
                case Pagto.FormaPagto.ChequeProprio: return GetPlanoConta(PlanoContas.FuncCheque);
                case Pagto.FormaPagto.ChequeTerceiro: return GetPlanoConta(PlanoContas.FuncCheque);
                case Pagto.FormaPagto.Construcard: return GetPlanoConta(PlanoContas.FuncConstrucard);
                case Pagto.FormaPagto.Credito: return GetPlanoConta(PlanoContas.FuncCredito);
                case Pagto.FormaPagto.Deposito: return GetPlanoConta(PlanoContas.FuncDeposito);
                case Pagto.FormaPagto.Dinheiro: return GetPlanoConta(PlanoContas.FuncDinheiro);
                case Pagto.FormaPagto.Permuta: return GetPlanoConta(PlanoContas.FuncPermuta);
                case Pagto.FormaPagto.Prazo: return GetPlanoConta(PlanoContas.FuncPrazo);
                case Pagto.FormaPagto.Cartao:
                case Pagto.FormaPagto.CartaoNaoIdentificado:
                    {
                        return ContasTipoFuncCartao(idTipoCartao);
                    }
            }

            return GetPlanoConta(PlanoContas.FuncRecebimento);
        }

        #endregion

        #region Forma de pagamento do plano de conta

        /// <summary>
        /// Recupera a forma de pagamento pelo id do plano de contas.
        /// </summary>
        /// <param name="idConta"></param>
        /// <returns></returns>
        public static FormaPagto GetFormaPagtoByIdConta(uint idConta)
        {
            uint formaPagto = 0;

            try
            {
                foreach (uint fp in (uint[])Enum.GetValues(typeof(Pagto.FormaPagto)))
                {
                    formaPagto = fp;
                    List<uint> planosContas = ListContasTipo((Pagto.FormaPagto)fp);
                    if (planosContas.Contains(idConta))
                        return FormaPagtoDAO.Instance.GetElementByPrimaryKey(fp);
                }
            }
            catch
            {
                if (formaPagto == (uint)Pagto.FormaPagto.Credito)
                {
                    FormaPagto fp = new FormaPagto();
                    fp.Descricao = "Crédito";
                    return fp;
                }
            }

            return null;
        }

        /// <summary>
        /// Retorna a forma de pagamento de um plano de conta.
        /// </summary>
        /// <param name="idConta"></param>
        /// <returns></returns>
        public static string GetDescrFormaPagtoByIdConta(uint idConta)
        {
            FormaPagto fp = GetFormaPagtoByIdConta(idConta);
            return fp != null ? fp.Descricao : null;
        }

        #endregion

        #region Planos de contas de devolução de pagamentos

        /// <summary>
        /// Retorna os planos de conta para devolução de pagamento.
        /// </summary>
        /// <returns></returns>
        public static string ContasDevolucaoPagto()
        {
            return GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoBancoBrasil) + "," + GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoLumen) + "," +
                GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoOutros) + "," + GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoSantander) + "," +
                GetPlanoConta(PlanoContas.DevolucaoPagtoBoleto) + "," + GetPlanoConta(PlanoContas.DevolucaoPagtoCartao) + "," +
                GetPlanoConta(PlanoContas.DevolucaoPagtoCheque) + "," + GetPlanoConta(PlanoContas.DevolucaoPagtoConstrucard) + "," +
                GetPlanoConta(PlanoContas.DevolucaoPagtoCredito) + "," + GetPlanoConta(PlanoContas.DevolucaoPagtoDeposito) + "," +
                GetPlanoConta(PlanoContas.DevolucaoPagtoDinheiro) + "," + GetPlanoConta(PlanoContas.DevolucaoPagtoPermuta) +

                /* Chamado 45147. */
                (!string.IsNullOrEmpty(string.Join(",", ContasCartoes.Select(f => f.IdContaDevolucaoPagto))) ? "," + string.Join(",", ContasCartoes.Select(f => f.IdContaDevolucaoPagto)) : string.Empty);
        }

        /// <summary>
        /// Retorna o plano de conta usado na devolução de pagamento.
        /// </summary>
        /// <param name="idFormaPagto"></param>
        /// <param name="idTipoCartao"></param>
        /// <returns></returns>
        public static uint GetPlanoContaDevolucaoPagto(uint idFormaPagto, uint idTipoCartao, uint idTipoBoleto)
        {
            switch (idFormaPagto)
            {
                case (uint)Pagto.FormaPagto.Boleto:
                    switch (idTipoBoleto)
                    {
                        case (uint)Utils.TipoBoleto.BancoBrasil: return GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoBancoBrasil);
                        case (uint)Utils.TipoBoleto.Lumen: return GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoLumen);
                        case (uint)Utils.TipoBoleto.Outros: return GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoOutros);
                        case (uint)Utils.TipoBoleto.Santander: return GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoSantander);
                        default: return GetPlanoConta(PlanoContas.DevolucaoPagtoBoleto);
                    }
                case (uint)Pagto.FormaPagto.Cartao:
                case (uint)Pagto.FormaPagto.CartaoNaoIdentificado:
                    {
                        var tipoCartao = ContasCartoes.Where(f => f.IdTipoCartao == idTipoCartao).FirstOrDefault();

                        if (tipoCartao == null)
                            return GetPlanoConta(PlanoContas.DevolucaoPagtoCartao);
                        else
                            return (uint)tipoCartao.IdContaDevolucaoPagto;
                    }
                case (uint)Pagto.FormaPagto.ChequeProprio: return GetPlanoConta(PlanoContas.DevolucaoPagtoCheque);
                case (uint)Pagto.FormaPagto.ChequeTerceiro: return GetPlanoConta(PlanoContas.DevolucaoPagtoCheque);
                case (uint)Pagto.FormaPagto.Construcard: return GetPlanoConta(PlanoContas.DevolucaoPagtoConstrucard);
                case (uint)Pagto.FormaPagto.Credito: return GetPlanoConta(PlanoContas.DevolucaoPagtoCredito);
                case (uint)Pagto.FormaPagto.Deposito: return GetPlanoConta(PlanoContas.DevolucaoPagtoDeposito);
                case (uint)Pagto.FormaPagto.Dinheiro: return GetPlanoConta(PlanoContas.DevolucaoPagtoDinheiro);
                case (uint)Pagto.FormaPagto.Permuta: return GetPlanoConta(PlanoContas.DevolucaoPagtoPermuta);
            }

            return 0;
        }

        /// <summary>
        /// Retorna o plano de conta usado na devolução de pagamento.
        /// </summary>
        public static uint GetEstornoDevolucaoPagto(uint idConta)
        {
            if (idConta == GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoBancoBrasil))
                return GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoBoletoBancoBrasil);
            else if (idConta == GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoLumen))
                return GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoBoletoLumen);
            else if (idConta == GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoOutros))
                return GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoBoletoOutros);
            else if (idConta == GetPlanoConta(PlanoContas.DevolucaoPagtoBoletoSantander))
                return GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoBoletoSantander);
            else if (idConta == GetPlanoConta(PlanoContas.DevolucaoPagtoBoleto))
                return GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoBoleto);
            else if (ContasCartoes.Any(f => f.PossuiPlanoConta(idConta)))
                return (uint)ContasCartoes.Where(f => f.PossuiPlanoConta(idConta)).FirstOrDefault().ObterContaEstorno(idConta);
            else if (idConta == GetPlanoConta(PlanoContas.DevolucaoPagtoCartao))
                return GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoCartao);
            else if (idConta == GetPlanoConta(PlanoContas.DevolucaoPagtoCheque))
                return GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoCheque);
            else if (idConta == GetPlanoConta(PlanoContas.DevolucaoPagtoConstrucard))
                return GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoConstrucard);
            else if (idConta == GetPlanoConta(PlanoContas.DevolucaoPagtoCredito))
                return GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoCredito);
            else if (idConta == GetPlanoConta(PlanoContas.DevolucaoPagtoDeposito))
                return GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoDeposito);
            else if (idConta == GetPlanoConta(PlanoContas.DevolucaoPagtoDinheiro))
                return GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoDinheiro);
            else if (idConta == GetPlanoConta(PlanoContas.DevolucaoPagtoPermuta))
                return GetPlanoConta(PlanoContas.EstornoDevolucaoPagtoPermuta);

            return 0;
        }

        #endregion

        #region Grupos de conta internos do sistema

        /// <summary>
        /// Retorna os grupos utilizados no sistema separados por ","
        /// </summary>
        public static string GetGruposSistema
        {
            get
            {
                // Financeiras / Faturamento sobre vendas
                return "5,7,46,47,48,49,50,51";
            }
        }

        /// <summary>
        /// Retorna os grupos que não aparecem no fluxo do sistema separados por ","
        /// </summary>
        public static string GetGruposExcluirFluxoSistema
        {
            get { return "5"; }
        }

        #endregion

        #region Contas do tipo Cartão

        public static void CarregarContasCartoes()
        {
            _contasCartoes = null;
        }

        public static IList<TipoCartaoCredito> ContasCartoes
        {
            get
            {
                if (_contasCartoes == null ||
                    _contasCartoes.Any(f => f.IdContaDevolucaoPagto == 0 ||
                        f.IdContaEntrada == 0 ||
                        f.IdContaEstorno == 0 ||
                        f.IdContaEstornoChequeDev == 0 ||
                        f.IdContaEstornoDevolucaoPagto == 0 ||
                        f.IdContaEstornoEntrada == 0 ||
                        f.IdContaEstornoRecPrazo == 0 ||
                        f.IdContaFunc == 0 ||
                        f.IdContaRecChequeDev == 0 ||
                        f.IdContaRecPrazo == 0 ||
                        f.IdContaVista == 0))
                        _contasCartoes = TipoCartaoCreditoDAO.Instance.GetList();

                return _contasCartoes;
            }
        }

        public static uint? ObterTipoCartaoPorConta(uint idPlanoConta)
        {
            var tipoCartao = ContasCartoes.Where(f => f.IdContaDevolucaoPagto == idPlanoConta || f.IdContaEntrada == idPlanoConta ||
             f.IdContaEstorno == idPlanoConta || f.IdContaEstornoChequeDev == idPlanoConta || f.IdContaEstornoDevolucaoPagto == idPlanoConta ||
             f.IdContaEstornoEntrada == idPlanoConta || f.IdContaEstornoRecPrazo == idPlanoConta || f.IdContaFunc == idPlanoConta ||
             f.IdContaRecChequeDev == idPlanoConta || f.IdContaRecPrazo == idPlanoConta || f.IdContaVista == idPlanoConta)
            .FirstOrDefault();

            if (tipoCartao != null)
                return (uint)tipoCartao.IdTipoCartao;

            return null;
        }

        #endregion
    }
}