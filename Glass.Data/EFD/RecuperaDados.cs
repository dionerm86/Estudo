using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Glass.Data.DAL.CTe;
using Glass.Data.RelDAL;
using Sync.Fiscal.EFD.Entidade;

namespace Glass.Data.EFD
{
    internal sealed class RecuperaDados : Glass.Pool.Singleton<RecuperaDados>, Sync.Fiscal.EFD.Suporte.IRecuperaDados
    {
        private RecuperaDados() { }

        public IEnumerable<INFe> ObtemNotasFiscais(string codigosLojas, DateTime inicio, DateTime fim)
        {
            return NotaFiscalDAO.Instance.GetForEFD(codigosLojas, inicio, fim, false, 0, null);
        }

        public IEnumerable<INFe> ObtemNotasFiscaisEFDContribuicoes(string codigosLojas, DateTime inicio, DateTime fim)
        {
            var situacoes = string.Format("{0},{1}",
                (int)Data.Model.NotaFiscal.SituacaoEnum.Autorizada,
                (int)Data.Model.NotaFiscal.SituacaoEnum.FinalizadaTerceiros);

            return NotaFiscalDAO.Instance.GetForEFD(codigosLojas, inicio, fim, false, 0, situacoes);
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IProdutoNFe> ObtemProdutosNotasFiscais(IEnumerable<Sync.Fiscal.EFD.Entidade.INFe> notasFiscais)
        {
            return ProdutosNfDAO.Instance.GetForEFD(notasFiscais).ToArray();
        }

        public IEnumerable<IProdutoNFe> ObtemProdutosNotasFiscaisEFDContribuicoes(IEnumerable<INFe> notasFiscais)
        {
            return ProdutosNfDAO.Instance.GetForEFD(notasFiscais, true).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IParcelaNFe> ObtemParcelasNotasFiscais(int codigoNotaFiscal)
        {
            return ParcelaNfDAO.Instance.GetByNf((uint)codigoNotaFiscal).ToArray();
        }

        public IEnumerable<ICTe> ObtemConhecimentosTransporte(string codigosLojas, DateTime inicio, DateTime fim)
        {
            var situacoes = string.Format("{0},{1},{2},{3},{4}",
                (int)Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Autorizado,
                (int)Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Cancelado,
                (int)Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Denegado,
                (int)Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FinalizadoTerceiros,
                (int)Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Inutilizado);

            return ConhecimentoTransporteDAO.Instance.GetForEFD(codigosLojas, inicio, fim, situacoes);
        }

        public IEnumerable<ICTe> ObtemConhecimentosTransporteEFDContribuicoes(string codigosLojas, DateTime inicio, DateTime fim)
        {
            var situacoes = string.Format("{0},{1}",
                (int)Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Autorizado,
                (int)Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FinalizadoTerceiros);

            return ConhecimentoTransporteDAO.Instance.GetForEFD(codigosLojas, inicio, fim, situacoes);
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IParticipanteCTe> ObtemParticipantesConhecimentosTransporte(IEnumerable<Sync.Fiscal.EFD.Entidade.ICTe> conhecimentosTransporte)
        {
            return ParticipanteCteDAO.Instance.GetForEFD(conhecimentosTransporte).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IImpostoCTe> ObtemImpostosCTe(int codigoConhecimentoTransporte)
        {
            return ImpostoCteDAO.Instance.GetList((uint)codigoConhecimentoTransporte).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.ILoja> ObtemLojas(IEnumerable<Sync.Fiscal.EFD.Entidade.INFe> notasFiscais, IEnumerable<Sync.Fiscal.EFD.Entidade.IParticipanteCTe> participantesConhecimentosTransporte)
        {
            return LojaDAO.Instance.GetForEFD(notasFiscais, participantesConhecimentosTransporte).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IBemAtivoImobilizado> ObtemBensAtivoImobilizado(string codigosLojas, DateTime inicio, DateTime fim)
        {
            return BemAtivoImobilizadoDAO.Instance.GetForEFD(codigosLojas, inicio, fim).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IValorRetidoFonte> ObtemValoresRetidosFonte(string codigosLojas, DateTime inicio, DateTime fim)
        {
            return ValorRetidoFonteDAO.Instance.GetForEFD(codigosLojas, inicio, fim).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IDeducaoDiversa> ObtemDeducoesDiversas(string codigosLojas, DateTime inicio, DateTime fim)
        {
            return DeducaoDiversaDAO.Instance.GetForEFD(codigosLojas, inicio, fim).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IReceitaDiversa> ObtemReceitasDiversas(string codigosLojas, DateTime inicio, DateTime fim)
        {
            return ReceitaDiversaDAO.Instance.GetForEFD(codigosLojas, inicio, fim);
        }

        public Sync.Fiscal.EFD.Entidade.INaturezaOperacao ObtemNaturezaOperacao(int codigoNaturezaOperacao)
        {
            return NaturezaOperacaoDAO.Instance.GetElementByPrimaryKey((uint)codigoNaturezaOperacao);
        }

        public Sync.Fiscal.EFD.Entidade.ICfop ObtemCfop(int codigoCfop)
        {
            return CfopDAO.Instance.GetElementByPrimaryKey((uint)codigoCfop);
        }

        public Sync.Fiscal.EFD.Entidade.IProduto ObtemProduto(int codigoProduto)
        {
            var produto = ProdutoDAO.Instance.GetElementByPrimaryKey((uint)codigoProduto);

            /* Chamado 47167. */
            if (produto.IdCest > 0)
            {
                var codigoCest = CestDAO.Instance.ObtemValorCampo<string>("Codigo", "IdCest=" + produto.IdCest);
                produto.CodigoCest = codigoCest;
            }

            return produto;
        }

        public Sync.Fiscal.EFD.Entidade.ICidade ObtemCidade(int codigoCidade)
        {
            return CidadeDAO.Instance.GetElementByPrimaryKey((uint)codigoCidade);
        }

        public IPlanoContaContabil ObtemPlanoContaContabil(int codigoPlanoContaContabil)
        {
            return PlanoContaContabilDAO.Instance.GetElementByPrimaryKey(codigoPlanoContaContabil);
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IPlanoContaContabil> ObtemPlanosContaContabeis()
        {
            return PlanoContaContabilDAO.Instance.GetAll();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.ICentroCusto> ObtemCentrosCustos(DateTime inicio)
        {
            return CentroCustoDAO.Instance.GetForEFD(inicio);
        }

        public Sync.Fiscal.EFD.Entidade.IGeneroProduto ObtemGeneroProduto(int codigoGeneroProduto)
        {
            return GeneroProdutoDAO.Instance.GetElementByPrimaryKey((uint)codigoGeneroProduto);
        }

        public Sync.Fiscal.EFD.Entidade.IUnidadeMedida ObtemUnidadeMedida(int codigoUnidadeMedida)
        {
            return UnidadeMedidaDAO.Instance.GetElementByPrimaryKey((uint)codigoUnidadeMedida);
        }

        public Sync.Fiscal.EFD.Entidade.IEfdCTe ObtemDadosEfdCTe(int codigoConhecimentoTransporte)
        {
            return EfdCteDAO.Instance.GetElementByPrimaryKey((uint)codigoConhecimentoTransporte);
        }

        public Sync.Fiscal.EFD.Entidade.IInfoCTe ObtemInfoCTe(int codigoConhecimentoTransporte)
        {
            return InfoCteDAO.Instance.GetElementByPrimaryKey((uint)codigoConhecimentoTransporte);
        }

        public Sync.Fiscal.EFD.Entidade.ICliente ObtemCliente(int codigoCliente)
        {
            return ClienteDAO.Instance.GetElementByPrimaryKey((uint)codigoCliente);
        }

        public Sync.Fiscal.EFD.Entidade.IFornecedor ObtemFornecedor(int codigoFornecedor)
        {
            return FornecedorDAO.Instance.GetElementByPrimaryKey((uint)codigoFornecedor);
        }

        public Sync.Fiscal.EFD.Entidade.ITransportador ObtemTransportador(int codigoTransportador)
        {
            return TransportadorDAO.Instance.GetElementByPrimaryKey((uint)codigoTransportador);
        }

        public Sync.Fiscal.EFD.Entidade.IAdministradoraCartao ObtemAdministradoraCartao(int codigoAdministradoraCartao)
        {
            return AdministradoraCartaoDAO.Instance.GetElementByPrimaryKey((uint)codigoAdministradoraCartao);
        }

        public Sync.Fiscal.EFD.Entidade.IProdutoLoja ObtemProdutoLoja(int codigoProduto, int codigoLoja)
        {
            return ProdutoLojaDAO.Instance.ObterParaEfd(null, codigoLoja, codigoProduto) ?? new Model.ProdutoLoja();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IAjusteBeneficioIncentivoApuracao> ObtemAjustesBeneficiosIncentivosApuracao(Sync.Fiscal.EFD.Configuracao.TipoImpostoEnum tipoImposto, DateTime inicio, DateTime fim, string uf)
        {
            return AjusteBeneficioIncentivoApuracaoDAO.Instance.GetList((ConfigEFD.TipoImpostoEnum)(int)tipoImposto, inicio.ToString("dd/MM/yyyy"), fim.ToString("dd/MM/yyyy"), uf).ToArray();
        }

        public Sync.Fiscal.EFD.Entidade.IAjusteBeneficioIncentivoApuracao ObtemAjusteBeneficioIncentivoApuracao(int codigoAjusteBeneficioIncentivoApuracao)
        {
            return AjusteBeneficioIncentivoApuracaoDAO.Instance.GetElementByPrimaryKey((uint)codigoAjusteBeneficioIncentivoApuracao);
        }

        public Sync.Fiscal.EFD.Entidade.IAjusteBeneficioIncentivo ObtemAjusteBeneficioIncentivo(int codigoAjusteBeneficioIncentivo)
        {
            return AjusteBeneficioIncentivoDAO.Instance.GetElementByPrimaryKey((uint)codigoAjusteBeneficioIncentivo);
        }

        public Sync.Fiscal.EFD.Entidade.IInfoAdicionalNFe ObtemInformacoesAdicionaisNFe(int codigoNotaFiscal)
        {
            return InfoAdicionalNfDAO.Instance.GetByNf((uint)codigoNotaFiscal);
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IAjusteDocumentoFiscal> ObtemAjustesDocumentoFiscal(int codigoNotaFiscal, int codigoConhecimentoTransporte)
        {
            if (codigoNotaFiscal > 0)
                return AjusteDocumentoFiscalDAO.Instance.ObtemPorNf((uint)codigoNotaFiscal).ToArray();
            else
                return AjusteDocumentoFiscalDAO.Instance.ObtemPorCte((uint)codigoConhecimentoTransporte).ToArray();
        }

        public Sync.Fiscal.EFD.Entidade.IContabilista ObtemContabilista(int codigoContabilista)
        {
            return ContabilistaDAO.Instance.GetElementByPrimaryKey((uint)codigoContabilista);
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IProcessoReferenciado> ObtemProcessosReferenciados(int codigoNotaFiscal, int codigoConhecimentoTransporte)
        {
            return ProcessoReferenciadoDAO.Instance.GetForEFD((uint)codigoNotaFiscal, (uint)codigoConhecimentoTransporte).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IObservacaoLancamentoFiscal> ObtemObservacoesLancamentoFiscalNFe(int codigoNotaFiscal)
        {
            return ObsLancFiscalNfDAO.Instance.GetByNf((uint)codigoNotaFiscal).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IObservacaoLancamentoFiscal> ObtemObservacoesLancamentoFiscalCTe(int codigoConhecimentoTransporte)
        {
            return ObsLancFiscalCteDAO.Instance.GetByCte((uint)codigoConhecimentoTransporte).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IObrigacaoRecolhidoRecolher> ObtemObrigacoesRecolhidoRecolher(Sync.Fiscal.EFD.Configuracao.TipoImpostoEnum tipoImposto, DateTime inicio, DateTime fim)
        {
            return ObrigacaoRecolhidoRecolherDAO.Instance.GetList((ConfigEFD.TipoImpostoEnum)(int)tipoImposto, inicio.ToString("dd/MM/yyyy"), fim.ToString("dd/MM/yyyy")).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IAjusteApuracaoIpi> ObtemAjustesApuracaoIpi(DateTime inicio, DateTime fim)
        {
            return AjusteApuracaoIPIDAO.Instance.GetList(ConfigEFD.TipoImpostoEnum.IPI, inicio.ToString("dd/MM/yyyy"), fim.ToString("dd/MM/yyyy")).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IAjusteApuracaoInfoAdicional> ObtemAjustesApuracaoInfoAdicional(Sync.Fiscal.EFD.Configuracao.TipoImpostoEnum tipoImposto, int codigoAjusteBeneficioIncentivoApuracao)
        {
            return AjusteApuracaoInfoAdicionalDAO.Instance.GetList((ConfigEFD.TipoImpostoEnum)(int)tipoImposto, (uint)codigoAjusteBeneficioIncentivoApuracao).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IAjusteApuracaoIdentificacaoDocFiscal> ObtemAjustesApuracaoIdentificacaoDocFiscal(Sync.Fiscal.EFD.Configuracao.TipoImpostoEnum tipoImposto, int codigoAjusteBeneficioIncentivoApuracao)
        {
            return AjusteApuracaoIdentificacaoDocFiscalDAO.Instance.GetList((ConfigEFD.TipoImpostoEnum)(int)tipoImposto, (uint)codigoAjusteBeneficioIncentivoApuracao).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IDocumentoArrecadacao> ObtemDocumentosArrecadacao(int codigoNotaFiscal)
        {
            return DocumentoArrecadacaoDAO.Instance.GetForEFD((uint)codigoNotaFiscal).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IDocumentoFiscal> ObtemDocumentosFiscais(int codigoNotaFiscal)
        {
            return DocumentoFiscalDAO.Instance.GetForEFD((uint)codigoNotaFiscal).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IAjusteApuracaoValorDeclaratorio> ObtemAjustesApuracaoValoresDeclaratorios(DateTime inicio, DateTime fim)
        {
            return AjusteApuracaoValorDeclaratorioDAO.Instance.GetList(inicio.ToString("dd/MM/yyyy"), fim.ToString("dd/MM/yyyy")).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IMovimentacaoEstoque> ObtemMovimentacoesEstoque(int codigoLoja, DateTime dataInventario)
        {
            return MovimentacaoEstoqueDAO.Instance.GetForEFD((uint)codigoLoja, dataInventario, true, Configuracoes.FiscalConfig.IgnorarUsoEConsumoSPED);
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IProcessoAdministrativo> ObtemProcessosAdminstrativos(DateTime inicio, DateTime fim)
        {
            return ProcessoAdministrativoDAO.Instance.GetForEFD(inicio, fim);
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IProcessoJudicial> ObtemProcessosJudiciais(DateTime inicio, DateTime fim)
        {
            return ProcessoJudicialDAO.Instance.GetForEFD(inicio, fim);
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IPagtoAdministradoraCartao> ObtemPagamentosAdministradorasCartao(string codigosLojas, DateTime inicio, DateTime fim)
        {
            return PagtoAdministradoraCartaoDAO.Instance.GetForEFD(codigosLojas, inicio, fim).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IMovimentacaoBemAtivoImobilizado> ObtemMovimentacoesBemAtivoImobilizado(string codigosLojas, DateTime inicio, DateTime fim)
        {
            return MovimentacaoBemAtivoImobDAO.Instance.GetForEFD(codigosLojas, inicio, fim).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IAjusteContribuicao> ObtemAjustesContribuicao(Sync.Fiscal.Enumeracao.CodigoContribuicaoSocial codCont, DateTime inicio, DateTime fim, Sync.Fiscal.Enumeracao.TipoImposto tipoImposto, float aliquota)
        {
            return AjusteContribuicaoDAO.Instance.GetForEFD((int)codCont, inicio, fim, Sync.Fiscal.Enumeracao.AjusteContribuicao.FonteAjuste.Contribuicao, (DataSourcesEFD.TipoImpostoEnum)(int)tipoImposto, aliquota).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IAjusteContribuicao> ObtemAjustesContribuicao(Sync.Fiscal.Enumeracao.CodigoTipoCredito codCred, DateTime inicio, DateTime fim, Sync.Fiscal.Enumeracao.TipoImposto tipoImposto, float aliquota)
        {
            return AjusteContribuicaoDAO.Instance.GetForEFD((int)codCred, inicio, fim, Sync.Fiscal.Enumeracao.AjusteContribuicao.FonteAjuste.Credito, (DataSourcesEFD.TipoImpostoEnum)(int)tipoImposto, aliquota).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IInfoValorAgregado> ObtemInfoValoresAgregados(DateTime inicio, DateTime fim)
        {
            return InfoValorAgregadoDAO.Instance.GetForEFD(inicio, fim).ToArray();
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IInfoDiferimento> ObtemInfoDirerimentos(DateTime inicio, DateTime fim, Sync.Fiscal.Enumeracao.CodigoContribuicaoSocial codCont, float aliquota, Sync.Fiscal.Enumeracao.TipoImposto tipoImposto)
        {
            return InfoDiferimentoDAO.Instance.GetForEFD(inicio, fim, (int)codCont, aliquota, (int)tipoImposto).ToArray();
        }

        public Sync.Fiscal.EFD.Entidade.IInfoAdicionalCredito ObtemInfoAdicionalCreditos(Sync.Fiscal.Enumeracao.CodigoTipoCredito codCred, string periodoGeracao, Sync.Fiscal.Enumeracao.TipoImposto tipoImposto)
        {
            return InfoAdicCreditoDAO.Instance.Obter((int)codCred, periodoGeracao, (int)tipoImposto);
        }

        public Sync.Fiscal.EFD.Entidade.ILoja ObtemLoja(int codigoLoja)
        {
            return LojaDAO.Instance.GetElementByPrimaryKey((uint)codigoLoja);
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IProdutoArquivoFCI> ObtemProdutosFCI(int codigoArquivoFCI)
        {
            return ProdutosArquivoFCIDAO.Instance.GetByIdArquivoFci((uint)codigoArquivoFCI).ToArray();
        }

        public bool VerificaDiaUtil(DateTime data)
        {
            return FuncoesData.DiaUtil(data);
        }

        public float ObtemAliquotaIcms(int codigoProduto, string ufOrigem, string ufDestino)
        {
            return IcmsProdutoUfDAO.Instance.ObterIcmsPorProduto(null, (uint)codigoProduto, ufOrigem, ufDestino, null);
        }

        public decimal ObtemValorTotalCancelado(DateTime inicio, DateTime fim)
        {
            return NotaFiscalDAO.Instance.GetTotalCancC380(inicio, fim);
        }

        public IEnumerable<T> ObtemAlteracoesAPartirDoLog<T>(T item, DateTime inicio, DateTime fim, Sync.Fiscal.Enumeracao.AlteracaoLog.TipoCampoAlteracao tipoCampoAlteracao)
            where T : Sync.Fiscal.EFD.Entidade.IBuscarAPartirDoLog
        {
            Glass.Data.Model.LogAlteracao.TabelaAlteracao tabela = 0;
            
            if (item is Sync.Fiscal.EFD.Entidade.IProduto)
                tabela = Glass.Data.Model.LogAlteracao.TabelaAlteracao.Produto;
            
            else if (item is Sync.Fiscal.EFD.Entidade.ICentroCusto)
                tabela = Glass.Data.Model.LogAlteracao.TabelaAlteracao.CentroCusto;
             
            else if (item is Sync.Fiscal.EFD.Entidade.IPlanoContaContabil)
                tabela = Glass.Data.Model.LogAlteracao.TabelaAlteracao.PlanoContaContabil;
             
            else
                throw new ArgumentException("Não foi possível identificar o tipo de objeto.", "item");

            var alteracoes = item == null ? null :
                LogAlteracaoDAO.Instance.GetByItem(tabela, (uint)(item as Sync.Fiscal.EFD.Entidade.IBuscarAPartirDoLog).Codigo, inicio, fim, true);

            return LogAlteracaoDAO.Instance.GetItems<T>(alteracoes, LogAlteracaoDAO.TipoCampoRetorno.Anterior, true);
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IAlteracaoLog> ObtemAlteracoesLog<T>(T item, DateTime inicio, DateTime fim)
            where T : Sync.Fiscal.EFD.Entidade.IParticipanteNFe
        {
            Glass.Data.Model.LogAlteracao.TabelaAlteracao tabela = 0;
            
            if (item is Sync.Fiscal.EFD.Entidade.ILoja)
                tabela = Glass.Data.Model.LogAlteracao.TabelaAlteracao.Loja;

            else if (item is Sync.Fiscal.EFD.Entidade.IFornecedor)
                tabela = Glass.Data.Model.LogAlteracao.TabelaAlteracao.Fornecedor;

            else if (item is Sync.Fiscal.EFD.Entidade.ICliente)
                tabela = Glass.Data.Model.LogAlteracao.TabelaAlteracao.Cliente;

            else if (item is Sync.Fiscal.EFD.Entidade.ITransportador)
                tabela = Glass.Data.Model.LogAlteracao.TabelaAlteracao.Transportador;

            else if (item is Sync.Fiscal.EFD.Entidade.IAdministradoraCartao)
                tabela = Glass.Data.Model.LogAlteracao.TabelaAlteracao.AdministradoraCartao;

            else
                throw new ArgumentException("Não foi possível identificar o tipo de objeto.", "item");
            
            return item == null ? new Sync.Fiscal.EFD.Entidade.IAlteracaoLog[0] :
                LogAlteracaoDAO.Instance.GetByItem(tabela, (uint)(item as Sync.Fiscal.EFD.Entidade.IParticipanteNFe).Codigo, inicio, fim, true);
        }

        public KeyValuePair<decimal, IEnumerable<Sync.Fiscal.EFD.Entidade.IControleCreditoEFD>> ObtemCreditosEFD(uint idLoja, DateTime dataCredito, Sync.Fiscal.Enumeracao.TipoImposto tipoImposto)
        {
            return ControleCreditoEfdDAO.Instance.GetForEFD(idLoja, dataCredito, (DataSourcesEFD.TipoImpostoEnum)(int)tipoImposto, null);
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IDebitoPisCofins> ObtemDebitosPisCofins(DateTime inicio, DateTime fim, Sync.Fiscal.Enumeracao.TipoImposto tipoImposto)
        {
            return DetalhamentoDebitosPisCofinsDAO.Instance.ObtemParaEfd(inicio.ToString("dd/MM/yyyy"), fim.ToString("dd/MM/yyyy"), (DataSourcesEFD.TipoImpostoEnum)(int)tipoImposto);
        }

        public void InserirCreditoUsado(uint idLoja, DateTime dataCredito, Sync.Fiscal.Enumeracao.TipoImposto tipoImposto, Sync.Fiscal.Enumeracao.CodigoTipoCredito? codCred, decimal valor)
        {
            UsoCreditoEfdDAO.Instance.InserirCreditoUsado(idLoja, dataCredito, (DataSourcesEFD.TipoImpostoEnum)(int)tipoImposto, (DataSourcesEFD.CodCredEnum?)(int?)codCred, valor);
        }

        public void InserirCredito(uint idLoja, DateTime dataCredito, Sync.Fiscal.Enumeracao.TipoImposto tipoImposto, Sync.Fiscal.Enumeracao.CodigoTipoCredito? codCred, decimal valor)
        {
            ControleCreditoEfdDAO.Instance.InserirCredito(idLoja, dataCredito, (DataSourcesEFD.TipoImpostoEnum)(int)tipoImposto, (DataSourcesEFD.CodCredEnum?)(int?)codCred, valor);
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IMovimentacaoEstoque> ObtemMovimentacoesEstoqueFinal(int codigoLoja, DateTime inicio, DateTime fim)
        {
            return MovimentacaoEstoqueDAO.Instance.GetForEFD(codigoLoja, inicio, fim);
        }

        public IEnumerable<Sync.Fiscal.EFD.Entidade.IItemProduzido> ObtemProducao(int codLoja, DateTime inicio, DateTime fim)
        {
            return Glass.Data.RelDAL.ItemProduzidoEFDDAO.Instance.ObtemItensProduzidosParaEFD(codLoja, inicio, fim);
        }

        public IEnumerable<IMovimentacaoInternaEstoque> ObtemMovimentacoesInternas(int codLoja, DateTime inicio, DateTime fim)
        {
            return MovInternaEstoqueFiscalDAO.Instance.ObtemParaEFD(codLoja, inicio, fim);
        }
    }
}
