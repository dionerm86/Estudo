using System;
using Glass.Data.Model;
using GDA;
using Glass.Data.RelDAL;
using Glass.Configuracoes;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(PedidoRptDAL))]
    public class PedidoRpt
    {
        #region Construtores

        public PedidoRpt()
        {

        }

        public PedidoRpt(Pedido ped, TipoConstrutor tipo, bool mostrarDescontoTotal)
        {
            switch (tipo)
            {
                case TipoConstrutor.ListaPedidos:
                    ListaPedidos(ped, mostrarDescontoTotal);
                    break;
                case TipoConstrutor.RelatorioPedido:
                    RelatorioPedido(ped);
                    break;
                case TipoConstrutor.RelatorioLiberacao:
                    RelatorioLiberacao(ped);
                    break;
                case TipoConstrutor.TermoAceitacao:
                    TermoAceitacao(ped);
                    break;
            }
        }

        private void ListaPedidos(Pedido ped, bool mostrarDescontoTotal)
        {
            IdPedido = ped.IdPedido;
            IdCli = ped.IdCli;
            CodCliente = ped.CodCliente;
            NomeInicialCli = ped.NomeInicialCli;
            NomeCli = ped.NomeCli;
            NomeCliente = ped.NomeCliente;
            CodRota = ped.CodRota;
            NomeFuncCliente = ped.NomeFuncCliente;
            NomeFunc = ped.NomeFunc;
            IdFunc = ped.IdFunc;
            IdFuncCliente = ped.IdFuncCliente;
            DataEntregaExibicao = ped.DataEntregaExibicao;
            DataCad = ped.DataCad;
            IntSituacao = ped.IntSituacao;
            DescricaoTipoPedido = ped.DescricaoTipoPedido;
            DescrSituacaoPedido = ped.DescrSituacaoPedido;
            DescrSituacaoProducao = ped.DescrSituacaoProducao;
            QtdePecas = ped.QtdePecas;
            QtdePecasPai = ped.QtdePecasPai;
            TotM = ped.TotM;
            Peso = ped.Peso;
            DadosVidrosVendidos = ped.DadosVidrosVendidos;
            RptCidade = ped.RptCidade;
            RptBairro = ped.RptBairro;
            RptNomeLoja = ped.RptNomeLoja;
            RptTotal = ped.RptTotal;
            Criterio = ped.Criterio;
            _total = ped.Total;
            DataPronto = ped.DataPronto;
            DataConf = ped.DataConf;
            DataConfLib = ped.DataConfLib;
            FastDeliveryString = ped.FastDeliveryString;
            Lucro = ped.Lucro;
            NumDias = ped.NumDias;
            RptEmail = ped.RptEmail;
            DataLiberacao = ped.DataLiberacao;
            TotalSemIcms = ped.TotalSemIcms;
            DataEntregaExibicao = ped.DataEntregaExibicao;
            RptPagto = ped.RptPagto;
            ValorEntrada = ped.ValorEntrada;
            ValorPagamentoAntecipado = ped.ValorPagamentoAntecipado;
            PagamentoAntecipado = ped.PagamentoAntecipado;
            DescrSitProdRpt = ped.DescrSitProdRpt;
            TotalComDescontoConcatenado = ped.TotalComDescontoConcatenado;
            TotalRecebSinalPagtoAntecip = ped.TotalRecebSinalPagtoAntecip;
            DeveTransferir = ped.DeveTransferir;
            ObsLiberacao = ped.ObsLiberacao;
            ValorIpi = ped.ValorIpi;
            ValorEntrega = ped.ValorEntrega;
            DescrTipoEntrega = ped.DescrTipoEntrega;
            IdOrcamento = ped.IdOrcamento;
            RptCompl = ped.RptCompl;

            if (mostrarDescontoTotal)
            {
                TextoDescontoTotalPerc = ped.TextoPercDescontoTotal;
                DescontoTotal = ped.Desconto;
            }
        }

        private void RelatorioPedido(Pedido ped)
        {
            IdPedido = ped.IdPedido;
            NomeCliente = ped.NomeCliente;
            NomeCli = ped.NomeCli;
            NomeFuncCliente = ped.NomeFuncCliente;
            DescrTipoEntrega = ped.DescrTipoEntrega;
            RptCpfCnpj = ped.RptCpfCnpj;
            RptRgEscinst = ped.RptRgEscinst;
            ContatoCliente = ped.ContatoCliente;
            RptEndereco = ped.RptEndereco;
            RptNumero = ped.RptNumero;
            RptCompl = ped.RptCompl;
            RptBairro = ped.RptBairro;
            RptCidade = ped.RptCidade;
            RptUf = ped.RptUf;
            RptTelContCli = ped.RptTelContCli;
            RptCep = ped.RptCep;
            NomeMedidor = ped.NomeMedidor;
            LocalizacaoObra = ped.LocalizacaoObra;
            NomeFunc = ped.NomeFunc;
            RptPagto = ped.RptPagto;
            RptSinal = ped.RptSinal;
            RptDataEntrega = ped.RptDataEntrega;
            ConfirmouRecebeuSinal = ped.ConfirmouRecebeuSinal;
            CodCliente = ped.CodCliente;
            FastDelivery = ped.FastDelivery;
            TemperaFora = ped.TemperaFora;
            TextoDescontoTotal = ped.TextoDescontoTotal;
            ValorComissao = ped.ValorComissao;
            _total = ped.Total;
            _totalEspelho = ped.TotalEspelho;
            MaoDeObra = ped.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObra;
            AcrescimoTotal = ped.AcrescimoTotal;
            DescontoTotal = ped.DescontoTotal;
            Obs = ped.Obs;
            ObsLiberacao = ped.ObsLiberacao;
            IdMedidor = ped.IdMedidor;
            EmailLoja = ped.EmailLoja;
            RptEmail = ped.RptEmail;
            CnpjLoja = ped.CnpjLoja;
            RptDataPedido = ped.RptDataPedido;
            RptTelefoneLoja = ped.RptTelefoneLoja;
            ClientePagaAntecipado = ped.ClientePagaAntecipado;
            ValorIcms = ped.ValorIcms;
            ValorIpi = ped.ValorIpi;
            ValorIcmsEspelho = ped.ValorIcmsEspelho;
            ValorIpiEspelho = ped.ValorIpiEspelho;
            DescricaoTipoPedido = ped.DescricaoTipoPedido;
            ExibirItensProdutosPedido = PedidoConfig.RelatorioPedido.ExibirItensProdutosPedido;
            FormaPagto = ped.FormaPagto;
            NomeLoja = ped.NomeLoja;
            FoneFaxLoja = ped.FoneFaxLoja;
            EnderecoLoja = ped.RptEnderecoLoja;
            ComplLoja = ped.RptComplLoja;
            BairroLoja = ped.RptBairroLoja;
            CidadeLoja = ped.RptCidadeLoja;
            UfLoja = ped.RptUfLoja;
            CepLoja = ped.RptCepLoja;
            BarCode = ped.BarCode;
            DescrSituacaoPedido = ped.DescrSituacaoPedido;
            DeveTransferir = ped.DeveTransferir;
            RptEnderecoCobranca = ped.RptEnderecoCobranca;
            RptBairroCobranca = ped.RptBairroCobranca;
            RptCidadeCobranca = ped.RptCidadeCobranca;
            RptCepCobranca = ped.RptCepCobranca;
            TemProdutoLamComposicao = ped.TemProdutoLamComposicao;
            DescricaoCompletaParcela = ped.DescricaoCompletaParcela;
            DescricaoSimplificadaParcela = ped.DescricaoSimplificadaParcela;
            BarCodeImage = ped.BarCodeImage;
            DescricaoParcelas = ped.DescricaoParcelas;
            TextoDescontoTotalPerc = ped.TextoDescontoTotalPerc;
            ValorEntrega = ped.ValorEntrega;
            TelVendedor = ped.TelVendedor;
            NomeTransportador = ped.NomeTransportador;
        }

        private void RelatorioLiberacao(Pedido ped)
        {
            IsPedidoGarantia = ped.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia;
            IsPedidoReposicao = ped.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição &&
                !Liberacao.TelaLiberacao.CobrarPedidoReposicao;

            IdPedido = ped.IdPedido;
            CodCliente = ped.CodCliente;
            DescricaoTipoPedido = ped.DescricaoTipoPedido;
            NomeFunc = ped.NomeFunc;
            DataEntregaExibicao = ped.DataEntregaString;
            Peso = ped.Peso;
            TotM = ped.TotM;
            ValorEntrada = ped.IdSinal > 0 ? ped.ValorEntrada : 0;
            ValorPagamentoAntecipado = ped.ValorPagamentoAntecipado;
            _total = ped.TotalPedidoFluxo;
            DescontoTotal = ped.DescontoTotal;
            DescontoExibirLib = ped.DescontoExibirLib;
            TextoDescontoTotal = ped.TextoDescontoTotal;
            TaxaFastDelivery = ped.TaxaFastDelivery;
            Obs = ped.Obs;
            ObsLiberacao = ped.ObsLiberacao;
            DescrTipoVenda = ped.DescrTipoVenda;
            DeveTransferir = ped.DeveTransferir;
            BarCodeImage = ped.BarCodeImage;
            ValorEntrega = ped.ValorEntrega;
            CodRota = ped.CodRota;
            PedCliExterno = ped.PedCliExterno;

            //Se for pedido de garantia e estiver marcado para nao mostrar valores.
            if (Liberacao.RelatorioLiberacaoPedido.NaoMostrarValorPedidoGarantia &&
                ped.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia)
            {
                _total = 0;
            }
        }

        private void TermoAceitacao(Pedido ped)
        {
            IdPedido = ped.IdPedido;
            NomeFunc = ped.NomeFunc;
            IdOrcamento = ped.IdOrcamento;
            InfoAdicional = ped.InfoAdicional;
            CidadeData = ped.CidadeData;
            NomeCli = ped.NomeCli;
            ValorEntrega = ped.ValorEntrega;
        }

        #endregion

        #region Enumeradores

        public enum TipoConstrutor
        {
            ListaPedidos,
            RelatorioPedido,
            RelatorioLiberacao,
            TermoAceitacao
        }

        public bool TemProdutoLamComposicao { get; set; }

        #endregion

        #region Propriedades

        public bool IsPedidoReposicao { get; set; }

        public bool IsPedidoGarantia { get; set; }

        public bool ExibirItensProdutosPedido { get; set; }

        public uint IdPedido { get; set; }

        public uint IdCli { get; set; }

        public string CodCliente { get; set; }

        public string NomeCli { get; set; }

        public string NomeInicialCli { get; set; }

        public string NomeCliente { get; set; }

        public string CodRota { get; set; }

        public string NomeFuncCliente { get; set; }

        public string NomeFunc { get; set; }

        public uint IdFunc { get; set; }

        public uint? IdFuncCliente { get; set; }

        public string DataEntregaExibicao { get; set; }

        public DateTime DataCad { get; set; }

        public int IntSituacao { get; set; }

        public string DescrSituacaoPedido { get; set; }

        public string DescrSituacaoProducao { get; set; }

        private decimal _total;

        public decimal Total
        {
            get { return !IsPedidoReposicao ? _total : 0; }
            set { _total = value; }
        }

        public string TotalComDescontoConcatenado { get; set; }

        public string TextoDescontoTotalPerc { get; set; }

        public decimal TotalRecebSinalPagtoAntecip { get; set; }

        public long QtdePecas { get; set; }

        public long QtdePecasPai { get; set; }

        public float TotM { get; set; }

        public float Peso { get; set; }

        public string DadosVidrosVendidos { get; set; }

        public string RptCidade { get; set; }

        public string RptNomeLoja { get; set; }

        public string TelVendedor { get; set; }

        public string RptTotal { get; set; }

        public string Criterio { get; set; }

        public string DescrTipoEntrega { get; set; }

        public string RptCpfCnpj { get; set; }

        public string RptRgEscinst { get; set; }

        public string ContatoCliente { get; set; }

        public string RptEndereco { get; set; }

        public string RptNumero { get; set; }

        public string RptCompl { get; set; }

        public string RptBairro { get; set; }

        public string RptUf { get; set; }

        public string RptTelContCli { get; set; }

        public string RptCep { get; set; }

        public string NomeMedidor { get; set; }

        public string LocalizacaoObra { get; set; }

        public string RptPagto { get; set; }

        public string RptSinal { get; set; }

        public string RptDataEntrega { get; set; }

        public string ConfirmouRecebeuSinal { get; set; }

        public bool FastDelivery { get; set; }

        public string FastDeliveryString { get; set; }

        public bool TemperaFora { get; set; }

        public string TextoDescontoTotal { get; set; }

        public decimal ValorComissao { get; set; }

        private decimal _totalEspelho;

        public decimal TotalEspelho
        {
            get { return !IsPedidoReposicao ? _totalEspelho : 0; }
            set { _totalEspelho = value; }
        }

        public bool MaoDeObra { get; set; }

        public decimal AcrescimoTotal { get; set; }

        public decimal DescontoTotal { get; set; }

        public decimal DescontoExibirLib { get; set; }

        public string Obs { get; set; }

        public string ObsLiberacao { get; set; }

        public uint? IdMedidor { get; set; }

        public string EmailLoja { get; set; }

        public string RptDataPedido { get; set; }

        public string RptTelefoneLoja { get; set; }

        public string CnpjLoja { get; set; }

        public bool ClientePagaAntecipado { get; set; }

        public string FormaPagto { get; set; }

        public string DescrTipoVenda { get; set; }

        public string NomeLoja { get; set; }

        public string ComplLoja { get; set; }

        public string EnderecoLoja { get; set; }

        public string BairroLoja { get; set; }

        public string CidadeLoja { get; set; }

        public string UfLoja { get; set; }

        public string CepLoja { get; set; }

        public string FoneFaxLoja { get; set; }

        public string DescricaoTipoPedido { get; set; }

        public decimal ValorEntrada { get; set; }

        public decimal ValorEntrega { get; set; }

        public decimal ValorIcms { get; set; }

        public decimal ValorIpi { get; set; }

        public decimal ValorIcmsEspelho { get; set; }

        public decimal ValorIpiEspelho { get; set; }

        public decimal ValorPagamentoAntecipado { get; set; }

        public float TaxaFastDelivery { get; set; }

        public DateTime? DataPronto { get; set; }

        public DateTime? DataConf { get; set; }

        public DateTime? DataConfLib { get; set; }

        public uint? IdOrcamento { get; set; }

        public string InfoAdicional { get; set; }

        public string CidadeData { get; set; }

        public decimal Lucro { get; set; }

        public int NumDias { get; set; }

        public string RptEmail { get; set; }

        public DateTime? DataLiberacao { get; set; }

        public decimal TotalSemIcms { get; set; }

        public bool PagamentoAntecipado { get; set; }

        public string DescrSitProdRpt { get; set; }

        public byte[] BarCode { get; set; }

        public bool DeveTransferir { get; set; }

        public string RptEnderecoCobranca { get; set; }

        public string RptBairroCobranca { get; set; }

        public string RptCidadeCobranca { get; set; }

        public string RptCepCobranca { get; set; }

        public string DescricaoCompletaParcela { get; set; }

        public string DescricaoSimplificadaParcela { get; set; }

        public string DescricaoParcelas { get; set; }

        public byte[] BarCodeImage { get; set; }

        public string PedCliExterno { get; set; }

        #region Campos usados no cálculo de totais do pedido na liberação

        public decimal DescontoRateadoLib { get; set; }

        public decimal EntradaRateadaLib { get; set; }

        public decimal PagtoAntecipRateadoLib { get; set; }

        public decimal TotalPedidoLib { get; set; }

        public string NomeTransportador { get; set; }

        #endregion

        #endregion
    }
}
