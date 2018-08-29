using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using System.Drawing;
using Glass.Data.DAL;
using Glass.Configuracoes;
using Glass.Log;
using System.Linq;
using System.ComponentModel;

namespace Glass.Data.Model
{
    public enum ModalidadeFrete
    {
        /// <summary>
        /// Frete por conta do Remetente (CIF)
        /// </summary>
        [Description("Frete por conta do Remetente (CIF)")]
        ContaDoRemetente,

        /// <summary>
        /// Frete por conta do Destinat�rio (FOB)
        /// </summary>
        [Description("Frete por conta do Destinat�rio (FOB)")]
        ContaDoDestinatario,

        /// <summary>
        /// Frete por conta de Terceiros
        /// </summary>
        [Description("Frete por conta de Terceiros")]
        ContaDeTerceiros,

        /// <summary>
        /// Transporte Pr�prio por conta do Remetente
        /// </summary>
        [Description("Transporte Pr�prio por conta do Remetente")]
        ProprioContaDoRemetente,

        /// <summary>
        /// Transporte Pr�prio por conta do Destinat�rio
        /// </summary>
        [Description("Transporte Pr�prio por conta do Destinat�rio")]
        ProprioContaDoDestinatario,

        /// <summary>
        /// Sem Ocorr�ncia de Transporte
        /// </summary>
        [Description("Sem Ocorr�ncia de Transporte")]
        SemTransporte = 9
    }

    [PersistenceBaseDAO(typeof(NotaFiscalDAO))]
    [PersistenceClass("nota_fiscal")]
    public class NotaFiscal : ModelBaseCadastro, Sync.Fiscal.EFD.Entidade.INFe
    {
        #region Enumeradores

        public enum SituacaoEnum
        {
            [Description("Aberta")]
            Aberta = 1,

            [Description("Autorizada")]
            Autorizada,

            [Description("N�o emitida")]
            NaoEmitida,

            [Description("Cancelada")]
            Cancelada,

            [Description("Inutilizada")]
            Inutilizada,

            [Description("Denegada")]
            Denegada,

            [Description("Processo de emiss�o")]
            ProcessoEmissao,

            [Description("Processo de cancelamento")]
            ProcessoCancelamento,

            [Description("Processo de inutiliza��o")]
            ProcessoInutilizacao,

            [Description("Falha ao emitir")]
            FalhaEmitir,

            [Description("Falha ao cancelar")]
            FalhaCancelar,

            [Description("Falha ao inutilizar")]
            FalhaInutilizar,

            [Description("Finalizada")]
            FinalizadaTerceiros,

            [Description("Conting�ncia offline")]
            ContingenciaOffline
        }

        public enum TipoDoc
        {
            Entrada = 1,
            Sa�da,
            EntradaTerceiros,
            NotaCliente,
            Transporte
        }

        public enum FinalidadeEmissaoEnum
        {
            Normal = 1,
            Complementar,
            Ajuste,
            Devolucao
        }

        public enum TipoEmissao
        {
            Normal = 1,
            Contingencia,
            ContingenciaComSCAN,
            ContingenciaViaDPEC,
            ContingenciaFSDA,
            ContingenciaSVCAN,
            ContingenciaSVCRS,
            ContingenciaNFCe = 9
        }

        public enum TipoFaturaEnum
        {
            Duplicata,
            Cheque,
            Promissoria,
            Recibo,
            Outros = 99
        }

        public enum PeriodoIpiEnum
        {
            Mensal,
            Decendial
        }

        public enum FormaPagtoEnum
        {
            AVista = 1,
            APrazo = 2,
            Outros = 3,
            Antecipacao = (int)Glass.Data.Model.Pagto.FormaPagto.AntecipFornec
        }        

        /// <summary>
        /// Enumera��o que indicador de presen�a do comprador no
        ///estabelecimento comercial no momento da opera��o.
        /// </summary>
        public enum IndicadorPresencaComprador
        {
            /// <summary>
            /// N�o se aplica.
            /// </summary>        
            NaoAplica = 0,
            /// <summary>
            /// Opera��o presencial.
            /// </summary>        
            OperacaoPresencial,
            /// <summary>
            /// Opera��o n�o presencial, internet.
            /// </summary>        
            OperacaoInternet,
            /// <summary>
            /// Opera��o n�o presencial, teleatendimento.
            /// </summary>        
            OperacaoTeleatendimento,
            /// <summary>
            /// NFC-e com entrega em domic�lio.
            /// </summary>
            NFCeEntregaDomicilio,
            /// <summary>
            /// Opera��o presencial, fora do estabelecimento.
            /// </summary>
            OperacaoPresencialForaEstabelecimento,
            /// <summary>
            /// Opera��o n�o presencial - outros.
            /// </summary>
            OperacaoNaoPresencialOutros = 9
        }

        /// <summary>
        /// Forma de importa��o quanto a intermedia��o.
        /// </summary>
        public enum FormaImportacao
        {
            /// <summary>
            /// Importa��o por conta pr�pria.
            /// </summary>
            ContaPropria = 1,
            /// <summary>
            /// Importa��o por conta e ordem.
            /// </summary>
            ContaOrdem,
            /// <summary>
            /// Importa��o por encomenda.
            /// </summary>
            PorEncomenda
        }

        #endregion

        #region Propriedades

        [Log("Num. Nota Fiscal")]
        [PersistenceProperty("IDNF", PersistenceParameterType.IdentityKey)]
        public uint IdNf { get; set; }

        [Log("Numeros Notas Fiscais Referenciada.")]
        [PersistenceProperty("IDSNFREF")]
        public string IdsNfRef { get; set; }

        [Log("Cliente")]
        [PersistenceProperty("IDCLIENTE")]
        public uint? IdCliente { get; set; }

        [Log("Emitente")]
        [PersistenceProperty("IDFORNEC")]
        public uint? IdFornec { get; set; }

        [Log("Transportador", "Nome", typeof(Transportador))]
        [PersistenceProperty("IDTRANSPORTADOR")]
        public uint? IdTransportador { get; set; }

        [Log("Natureza da Opera��o", "CodCompleto", typeof(NaturezaOperacaoDAO), "IdNaturezaOperacao", "ObtemElemento", true)]
        [PersistenceProperty("IDNATUREZAOPERACAO")]
        public uint? IdNaturezaOperacao { get; set; }

        /// <summary>
        /// Emitente
        /// </summary>
        [Log("Destinat�rio", "NomeFantasia", typeof(Loja))]
        [PersistenceProperty("IDLOJA")]
        public uint? IdLoja { get; set; }

        /// <summary>
        /// Munic�pio de Ocorr�ncia
        /// </summary>
        [Log("Munic�pio de ocorr�ncia", "NomeCidade", typeof(Cidade))]
        [PersistenceProperty("IDCIDADE")]
        public uint IdCidade { get; set; }

        [Log("Plano de conta", "Descricao", typeof(PlanoContas))]
        [PersistenceProperty("IDCONTA")]
        public uint? IdConta { get; set; }

        [Log("Antecipa��o de Pagamento de Fornecedor")]
        [PersistenceProperty("IDANTECIPFORNEC")]
        public uint? IdAntecipFornec { get; set; }

        [Log("N�mero NFe")]
        [PersistenceProperty("NUMERONFE")]
        public uint NumeroNFe { get; set; }

        [Log("Chave Acesso")]
        [PersistenceProperty("CHAVEACESSO")]
        public string ChaveAcesso { get; set; }

        [PersistenceProperty("CODALEATORIO")]
        public string CodAleatorio { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [Log("Modelo")]
        [PersistenceProperty("MODELO")]
        public string Modelo { get; set; }

        [Log("Serie")]
        [PersistenceProperty("SERIE")]
        public string Serie { get; set; }

        [Log("Subs�rie")]
        [PersistenceProperty("SUBSERIE")]
        public string Subserie { get; set; }

        /// <summary>
        /// 1-Entrada
        /// 2-Sa�da
        /// 3-Entrada (terceiros)
        /// 4-Nota Fiscal de cliente
        /// </summary>
        [PersistenceProperty("TIPODOCUMENTO")]
        public int TipoDocumento { get; set; }

        /// <summary>
        /// Identifica se a nota � de conhecimento de transporte
        /// </summary>
        [PersistenceProperty("TRANSPORTE", DirectionParameter.Input)]
        public bool Transporte { get; set; }

        private bool _complementar;

        /// <summary>
        /// Campo utilizado apenas em nota de entrada de terceiros, para identificar se � nota complementar e no SPED, para verificar se � nota complementar
        /// </summary>
        [PersistenceProperty("COMPLEMENTAR")]
        public bool Complementar 
        { 
            get
            {
                return _complementar || FinalidadeEmissao == (int)FinalidadeEmissaoEnum.Complementar;
            }
            set { _complementar = value; }
        }

        /// <summary>
        /// 1-Retrato
        /// 2-Paisagem
        /// 3-Simplificado
        /// 4-NFCe
        /// 5-NFCeMensagemEletronica
        /// </summary>
        [PersistenceProperty("TIPOIMPRESSAO")]
        public int TipoImpressao { get; set; }

        [Log("Data de emiss�o")]
        [PersistenceProperty("DATAEMISSAO")]
        public DateTime DataEmissao { get; set; }

        [Log("Data de sa�da/entrada")]
        [PersistenceProperty("DATASAIDAENT")]
        public DateTime? DataSaidaEnt { get; set; }

        /// <summary>
        /// 1-� Vista
        /// 2-� Prazo
        /// 3-Outros
        /// 12-Antecipa��o
        /// </summary>
        [Log("Forma de pagamento")]
        [PersistenceProperty("FORMAPAGTO")]
        public int FormaPagto { get; set; }

        /// <summary>
        /// 1-Normal
        /// 2-Conting�ncia FS
        /// 3-Conting�ncia com SCAN
        /// 4-Conting�ncia via DPEC
        /// 5-Conting�ncia FS-DA
        /// 6-Conting�ncia SVC-AN
        /// 7-Conting�ncia SVC-RS
        /// 9-Conting�ncia off-line da NFC-e
        /// </summary>
        [PersistenceProperty("FORMAEMISSAO")]
        public int FormaEmissao { get; set; }

        /// <summary>
        /// 1-NF-e normal
        /// 2-NF-e complementar
        /// 3-NF-e de ajuste
        /// 4-Devolu��o/Retorno
        /// </summary>
        [PersistenceProperty("FINALIDADEEMISSAO")]
        public int FinalidadeEmissao { get; set; }

        [Log("Base c�lc. ICMS")]
        [PersistenceProperty("BCICMS")]
        public decimal BcIcms { get; set; }

        [Log("Valor ICMS")]
        [PersistenceProperty("VALORICMS")]
        public decimal Valoricms { get; set; }

        [Log("Base c�lc. FCP")]
        [PersistenceProperty("BCFCP")]
        public decimal BcFcp { get; set; }

        [Log("Valor FCP")]
        [PersistenceProperty("VALORFCP")]
        public decimal ValorFcp { get; set; }

        [Log("Base c�lc. ICMS ST")]
        [PersistenceProperty("BCICMSST")]
        public decimal BcIcmsSt { get; set; }

        [Log("Valor ICMS ST")]
        [PersistenceProperty("VALORICMSST")]
        public decimal ValorIcmsSt { get; set; }

        [Log("Base c�lc. FCP ST")]
        [PersistenceProperty("BCFCPST")]
        public decimal BcFcpSt { get; set; }

        [Log("Valor FCP ST")]
        [PersistenceProperty("VALORFCPST")]
        public decimal ValorFcpSt { get; set; }

        [Log("Valor IPI")]
        [PersistenceProperty("VALORIPI")]
        public decimal ValorIpi { get; set; }

        [Log("Valor do IPI Devolvido")]
        [PersistenceProperty("VALORIPIDEVOLVIDO")]
        public decimal ValorIpiDevolvido { get; set; }

        [Log("Total dos produtos")]
        [PersistenceProperty("TOTALPROD")]
        public decimal TotalProd { get; set; }

        [PersistenceProperty("MODALIDADEFRETE")]
        public ModalidadeFrete ModalidadeFrete { get; set; }

        [Log("Valor frete")]
        [PersistenceProperty("VALORFRETE")]
        public decimal ValorFrete { get; set; }

        [Log("Placa do ve�culo")]
        [PersistenceProperty("VEICPLACA")]
        public string VeicPlaca { get; set; }

        [Log("RNTC do ve�culo")]
        [PersistenceProperty("VEICRNTC")]
        public string VeicRntc { get; set; }

        [Log("UF do ve�culo")]
        [PersistenceProperty("VEICUF")]
        public string VeicUf { get; set; }

        [Log("Qtd. volumes transp.")]
        [PersistenceProperty("QTDVOL")]
        public int QtdVol { get; set; }

        [Log("Marca volumes transp.")]
        [PersistenceProperty("MARCAVOL")]
        public string MarcaVol { get; set; }

        [Log("Numera��o volumes transp.")]
        [PersistenceProperty("NUMERACAOVOL")]
        public string NumeracaoVol { get; set; }

        [Log("Esp�cie dos volumes transp.")]
        [PersistenceProperty("ESPECIE")]
        public string Especie { get; set; }

        [Log("Valor do seguro")]
        [PersistenceProperty("VALORSEGURO")]
        public decimal ValorSeguro { get; set; }

        [Log("Outras despesas")]
        [PersistenceProperty("OUTRASDESPESAS")]
        public decimal OutrasDespesas { get; set; }

        [Log("Peso cont�iner")]
        [PersistenceProperty("PESOCONTEINER")]
        public decimal PesoConteiner { get; set; }

        [Log("Peso bruto transp.")]
        [PersistenceProperty("PESOBRUTO")]
        public decimal PesoBruto { get; set; }

        [Log("Peso l�quido transp.")]
        [PersistenceProperty("PESOLIQ")]
        public decimal PesoLiq { get; set; }

        [Log("Desconto")]
        [PersistenceProperty("DESCONTO")]
        public decimal Desconto { get; set; }

        [Log("N�mero de parcelas")]
        [PersistenceProperty("NUMPARC")]
        public int? NumParc { get; set; }

        [Log("Valor base das parcelas")]
        [PersistenceProperty("VALORPARC")]
        public decimal ValorParc { get; set; }

        [Log("Data base de venc. das parcelas")]
        [PersistenceProperty("DATABASEVENC")]
        public DateTime? DataBaseVenc { get; set; }

        [Log("Total manual da nota")]
        [PersistenceProperty("TOTALMANUAL")]
        public decimal TotalManual { get; set; }

        [Log("Total da nota")]
        [PersistenceProperty("TOTALNOTA")]
        public decimal TotalNota { get; set; }

        [PersistenceProperty("NUMLOTE")]
        public int? NumLote { get; set; }

        [PersistenceProperty("NUMRECIBO")]
        public string NumRecibo { get; set; }

        [PersistenceProperty("NUMPROTOCOLO")]
        public string NumProtocolo { get; set; }

        [PersistenceProperty("NUMPROTOCOLOCANC")]
        public string NumProtocoloCanc { get; set; }

        [PersistenceProperty("MOTIVOCANC")]
        public string MotivoCanc { get; set; }

        [PersistenceProperty("MOTIVOINUT")]
        public string MotivoInut { get; set; }

        [Log("Informa��es complementares")]
        [PersistenceProperty("INFCOMPL")]
        public string InfCompl { get; set; }

        [Log("Obs. SEFAZ")]
        [PersistenceProperty("OBSSEFAZ")]
        public string ObsSefaz { get; set; }

        /// <summary>
        /// 1-Produ��o
        /// 2-Homologa��o
        /// </summary>
        [PersistenceProperty("TIPOAMBIENTE")]
        public int TipoAmbiente { get; set; }

        [Log("Gerar cotas a pagar")]
        [PersistenceProperty("GERARCONTASPAGAR")]
        public bool GerarContasPagar { get; set; }

        [Log("Gerar estoque real")]
        [PersistenceProperty("GERARESTOQUEREAL")]
        public bool GerarEstoqueReal { get; set; }

        [Log("Gerar etiqueta de nota fiscal?")]
        [PersistenceProperty("GerarEtiqueta")]
        public bool GerarEtiqueta { get; set; }

        [Log("Aliq. PIS")]
        [PersistenceProperty("ALIQPIS")]
        public float AliqPis { get; set; }

        [Log("Valor PIS")]
        [PersistenceProperty("VALORPIS")]
        public decimal ValorPis { get; set; }

        [Log("Aliq. Cofins")]
        [PersistenceProperty("ALIQCOFINS")]
        public float AliqCofins { get; set; }

        [Log("Valor Cofins")]
        [PersistenceProperty("VALORCOFINS")]
        public decimal ValorCofins { get; set; }

        [PersistenceProperty("TIPOFATURA")]
        public int? TipoFatura { get; set; }

        [Log("Descri��o Fatura")]
        [PersistenceProperty("DESCRFATURA")]
        public string DescrFatura { get; set; }

        [Log("N�mero Fatura")]
        [PersistenceProperty("NUMFATURA")]
        public string NumFatura { get; set; }

        [Log("Per�odo Apura��o IPI")]
        [PersistenceProperty("PERIODOAPURACAOIPI")]
        public int PeriodoApuracaoIpi { get; set; }

        [PersistenceProperty("ENTROUESTOQUE")]
        public bool EntrouEstoque { get; set; }

        [PersistenceProperty("SAIUESTOQUE")]
        public bool SaiuEstoque { get; set; }

        [Log("N�mero Documento FS-DA")]
        [PersistenceProperty("NUMDOCFSDA")]
        public long? NumeroDocumentoFsda { get; set; }

        [PersistenceProperty("VALORISS")]
        public decimal ValorISS { get; set; }

        [PersistenceProperty("SERVICO")]
        public bool Servico { get; set; }

        [Log("Valor total tributado")]
        [PersistenceProperty("ValorTotalTrib")]
        public decimal ValorTotalTrib { get; set; }

        /// <summary>
        /// Indica que � uma NFC-e
        /// </summary>
        [Log("Valor total tributado")]
        [PersistenceProperty("Consumidor")]
        public bool Consumidor { get; set; }

        /// <summary>
        /// CPF identificado na NFC-e
        /// </summary>
        [Log("CPF(NFC-e)")]
        [PersistenceProperty("Cpf")]
        public string Cpf { get; set; }

        /// <summary>
        /// UF de embarque
        /// </summary>
        [Log("UF de embarque")]
        [PersistenceProperty("UfEmbarque")]
        public string UfEmbarque { get; set; }

        /// <summary>
        /// Local de embarque
        /// </summary>
        [Log("Local de embarque")]
        [PersistenceProperty("LocalEmbarque")]
        public string LocalEmbarque { get; set; }

        /// <summary>
        /// Local de despacho
        /// </summary>
        [Log("Local de despacho")]
        [PersistenceProperty("LocalDespacho")]
        public string LocalDespacho { get; set; }

        /// <summary>
        /// Percentual da rentabilidade.
        /// </summary>
        [PersistenceProperty("PERCENTUALRENTABILIDADE", Direction = DirectionParameter.Input)]
        public decimal PercentualRentabilidade { get; set; }

        /// <summary>
        /// Valor da rentabilidade financeira.
        /// </summary>
        [PersistenceProperty("RENTABILIDADEFINANCEIRA", Direction = DirectionParameter.Input)]
        public decimal RentabilidadeFinanceira { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CODNATUREZAOPERACAO", DirectionParameter.InputOptional)]
        public string CodNaturezaOperacao { get; set; }

        [Log("CFOP")]
        [PersistenceProperty("CODCFOP", DirectionParameter.InputOptional)]
        public string CodCfop { get; set; }

        [PersistenceProperty("DESCRCFOP", DirectionParameter.InputOptional)]
        public string DescrCfop { get; set; }

        [Log("Munic�pio de ocorr�ncia")]
        [PersistenceProperty("MUNICOCOR", DirectionParameter.InputOptional)]
        public string MunicOcor { get; set; }

        [Log("Emitente")]
        [PersistenceProperty("NOMEEMITENTE", DirectionParameter.InputOptional)]
        public string NomeEmitente { get; set; }

        [Log("Emitente CNPJ")]
        [PersistenceProperty("CNPJEMITENTE", DirectionParameter.InputOptional)]
        public string CnpjEmitente { get; set; }

        [PersistenceProperty("SUFRAMACLIENTE", DirectionParameter.InputOptional)]
        public string SuframaCliente { get; set; }

        [Log("Destinat�rio/Remetente")]
        [PersistenceProperty("NOMEDESTREM", DirectionParameter.InputOptional)]
        public string NomeDestRem { get; set; }

        [Log("Destinat�rio/Remetente CPF/CNPJ")]
        [PersistenceProperty("CPFCNPJDESTREM", DirectionParameter.InputOptional)]
        public string CpfCnpjDestRem { get; set; }

        [Log("Transportador")]
        [PersistenceProperty("NOMETRANSPORTADOR", DirectionParameter.InputOptional)]
        public string NomeTransportador { get; set; }

        [PersistenceProperty("TIPOENTREGA", DirectionParameter.InputOptional)]
        public long TipoEntrega { get; set; }

        [PersistenceProperty("IDSPEDIDO", DirectionParameter.InputOptional)]
        public string IdsPedido { get; set; }

        [PersistenceProperty("DESCRFORMAPAGTO", DirectionParameter.InputOptional)]
        public string DescrFormaPagto { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("DESCRPLANOCONTAS", DirectionParameter.InputOptional)]
        public string DescrPlanoContas { get; set; }

        [PersistenceProperty("TEMMOVIMENTACAOBEMATIVO", DirectionParameter.InputOptional)]
        public bool TemMovimentacaoBemAtivo { get; set; }

        [PersistenceProperty("ISNFIMPORTACAO", DirectionParameter.InputOptional)]
        public bool IsNfImportacao { get; set; }

        [PersistenceProperty("CODMUNICIPIO", DirectionParameter.InputOptional)]
        public string CodMunicipio { get; set; }

        #endregion

        #region Propriedades de Suporte

        private uint? _idCfop;

        public uint? IdCfop
        {
            get
            {
                if (_idCfop == null && IdNaturezaOperacao > 0)
                    _idCfop = NaturezaOperacaoDAO.Instance.ObtemIdCfop(IdNaturezaOperacao.Value);

                return _idCfop;
            }
        }

        #region M�todos internos est�ticos

        internal static string GetTipoDocumento(int tipo)
        {
            switch (tipo)
            {
                case 1: return "Entrada";
                case 2: return "Sa�da";
                case 3: return "Entrada (terceiros)";
                case 4: return "Nota Fiscal de Cliente";
                default: return "";
            }
        }

        #endregion

        public decimal TotalCST60 { get; set; }

        public DateTime[] DatasParcelas { get; set; }

        public decimal[] ValoresParcelas { get; set; }

        public string[] BoletosParcelas { get; set; }

        public string DescrParcelas { get; set; }

        public string DadosFatura { get; set; }

        /// <summary>
        /// Campo usado para gerar mais de um registro 50 se for o caso
        /// </summary>
        public float? AliqIcms { get; set; }

        /// <summary>
        /// Controla visibilidade da opcao editar na grid
        /// </summary>
        public bool EditVisible
        {
            get
            {
                return Glass.Data.DAL.NotaFiscalDAO.Instance.PodeEditar(IdNf);
            }
        }

        /// <summary>
        /// Controla visibilidade da opcao cancelar na grid
        /// </summary>
        public bool CancelVisible
        {
            get
            {
                LoginUsuario login = UserInfo.GetUserInfo;
                bool flagSituacao = Situacao == (int)SituacaoEnum.Aberta || Situacao == (int)SituacaoEnum.FalhaEmitir ||
                    Situacao == (int)SituacaoEnum.Autorizada || Situacao == (int)SituacaoEnum.FalhaCancelar;

                return flagSituacao;
            }
        }

        public bool ExibirPedidosVisible
        {
            get { return TipoDocumento == (int)TipoDoc.Sa�da; }
        }

        private bool? _exibirComprasVisible;

        public bool ExibirComprasVisible
        {
            get
            {
                if (_exibirComprasVisible == null)
                    _exibirComprasVisible = CompraNotaFiscalDAO.Instance.PossuiCompra(IdNf);

                return _exibirComprasVisible.GetValueOrDefault();
            }
        }

        private string _idCompras;

        public string IdCompras
        {
            get
            {
                if (_idCompras == null)
                    _idCompras = CompraNotaFiscalDAO.Instance.ObtemIdCompras(IdNf);

                return _idCompras;
            }
        }

        public bool EmitirNfFsVisible
        {
            get
            {
                return Situacao == (int)SituacaoEnum.NaoEmitida && FormaEmissao == (int)TipoEmissao.ContingenciaFSDA &&
                    FiscalConfig.NotaFiscalConfig.ContingenciaNFe == DataSources.TipoContingenciaNFe.NaoUtilizar;
            }
        }

        public bool SepararValoresVisible
        {
            get
            {
                return
                    Config.PossuiPermissao(Config.FuncaoMenuFiscal.SepararValoresFiscaisReais) &&
                    Situacao == (int)SituacaoEnum.Autorizada &&
                    FormaPagto != (int)NotaFiscal.FormaPagtoEnum.AVista &&
                    ((FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber && TipoDocumento == (int)TipoDoc.Sa�da) ||
                    (FinanceiroConfig.SepararValoresFiscaisEReaisContasPagar && TipoDocumento == (int)TipoDoc.EntradaTerceiros));
            }
        }

        public bool CancelarSeparacaoValoresVisible
        {
            get
            {
                return
                    UserInfo.GetUserInfo.IsAdministrador &&
                    (Situacao == (int)SituacaoEnum.Autorizada || Situacao == (int)SituacaoEnum.Cancelada) &&
                    FormaPagto != (int)NotaFiscal.FormaPagtoEnum.AVista &&
                    ((FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber && TipoDocumento == (int)TipoDoc.Sa�da) ||
                    (FinanceiroConfig.SepararValoresFiscaisEReaisContasPagar && TipoDocumento == (int)TipoDoc.EntradaTerceiros));
            }
        }

        public bool BaixarXmlVisible
        {
            get
            {
                return PrintDanfeVisible;
            }
        }

        public bool PrintDanfeVisible
        {
            get
            {
                return Situacao == (int)SituacaoEnum.Autorizada || ((Situacao == (int)SituacaoEnum.Cancelada ||
                    Situacao == (int)SituacaoEnum.FalhaCancelar) && !String.IsNullOrEmpty(NumProtocolo)) ||
                    EmitirNfFsVisible || Situacao == (int)SituacaoEnum.ContingenciaOffline;
            }
        }

        public bool AnexarXMLTercVisible
        {
            get { return TipoDocumento == (int)TipoDoc.EntradaTerceiros && Situacao == (int)SituacaoEnum.Aberta; }
        }

        public bool BaixarXMLTercVisible
        {
            get { return TipoDocumento == (int)TipoDoc.EntradaTerceiros && ExisteArquivoXmlNotaFiscalEntradaTerceiro; }
        }

        public bool ExisteArquivoXmlNotaFiscalEntradaTerceiro
        {
            get
            {
                var nome = Glass.Data.Helper.Utils.GetNfeXmlPath + ChaveAcesso + "-ter.xml";
                return System.IO.File.Exists(nome);
            }
        }

        public bool GerarNFComplVisible
        {
            get
            {
                return Situacao == (int)SituacaoEnum.Autorizada && (FinalidadeEmissao == (int)FinalidadeEmissaoEnum.Normal || FinalidadeEmissao == (int)FinalidadeEmissaoEnum.Devolucao) &&
                    !String.IsNullOrEmpty(NumProtocolo) && !Consumidor;
            }
        }

        public bool CartaCorrecaoVisible
        {
            get
            {
                return Situacao == (int)SituacaoEnum.Autorizada && !Consumidor;
            }
        }

        public bool ConsSitVisible
        {
            get
            {
                return (Situacao == (int)SituacaoEnum.ProcessoEmissao || Situacao == (int)SituacaoEnum.ProcessoInutilizacao || 
                    Situacao == (int)SituacaoEnum.ProcessoCancelamento || Situacao == (int)SituacaoEnum.FalhaEmitir ||
                    Situacao == (int)SituacaoEnum.FalhaCancelar || Situacao == (int)SituacaoEnum.FalhaInutilizar);
            }
        }

        public bool ConsSitLoteVisible
        {
            get
            {
                return Situacao == (int)SituacaoEnum.ProcessoEmissao || Situacao == (int)SituacaoEnum.ProcessoInutilizacao ||
                    Situacao == (int)SituacaoEnum.ProcessoCancelamento || Situacao == (int)SituacaoEnum.FalhaEmitir ||
                    Situacao == (int)SituacaoEnum.FalhaCancelar || Situacao == (int)SituacaoEnum.FalhaInutilizar;
            }
        }

        public bool TerceirosEmAbertoVisible
        {
            get
            {
                return Situacao == (int)SituacaoEnum.Aberta &&
                    (TipoDocumento == (int)TipoDoc.EntradaTerceiros || TipoDocumento == (int)TipoDoc.NotaCliente);
            }
        }

        public bool ReenviarEmailXmlVisible
        {
            get
            {
                return Situacao == (int)SituacaoEnum.Autorizada && TipoDocumento == (int)TipoDoc.Sa�da;
            }
        }

        public bool ReenviarEmailXmlCancelamentoVisible
        {
            get
            {
                return Situacao == (int)SituacaoEnum.Cancelada && TipoDocumento == (int)TipoDoc.Sa�da;
            }
        }

        public string SituacaoString
        {
            get
            {
                return Situacao == (int)SituacaoEnum.Aberta ? "Aberta" :
                    Situacao == (int)SituacaoEnum.Autorizada ? "Autorizada" :
                    Situacao == (int)SituacaoEnum.NaoEmitida ? "N�o emitida" :
                    Situacao == (int)SituacaoEnum.Cancelada ? "Cancelada" :
                    Situacao == (int)SituacaoEnum.Inutilizada ? "Inutilizada" :
                    Situacao == (int)SituacaoEnum.Denegada ? "Denegada" :
                    Situacao == (int)SituacaoEnum.ProcessoEmissao ? "Processo de emiss�o" :
                    Situacao == (int)SituacaoEnum.ProcessoCancelamento ? "Processo de cancelamento" :
                    Situacao == (int)SituacaoEnum.ProcessoInutilizacao ? "Processo de inutiliza��o" :
                    Situacao == (int)SituacaoEnum.FalhaEmitir ? "Falha ao emitir" :
                    Situacao == (int)SituacaoEnum.FalhaCancelar ? "Falha ao cancelar" :
                    Situacao == (int)SituacaoEnum.FalhaInutilizar ? "Falha ao inutilizar" :
                    Situacao == (int)SituacaoEnum.FinalizadaTerceiros ? "Finalizada" :
                    Situacao == (int)SituacaoEnum.ContingenciaOffline ? "Conting�ncia Offline" :
                    String.Empty;
            }
        }

        [Log("Forma de pagamento")]
        public string FormaPagtoString
        {
            get
            {
                if (Consumidor)
                    return PagamentoNfceStr;

                return FormaPagto == 1 ? "� Vista" :
                    FormaPagto == 2 ? "� Prazo" :
                    FormaPagto == 3 ? "Outros" :
                    FormaPagto == 12 ? "Antecipa��o" :
                    "N�o informado";
            }
        }

        [Log("Tipo Documento")]
        public string TipoDocumentoString
        {
            get { return GetTipoDocumento(TipoDocumento); }
        }

        [Log("Modalidade do frete")]
        public string ModalidadeFreteString
        {
            get
            {
                return ModalidadeFrete == ModalidadeFrete.ContaDoRemetente ? "Frete por conta do Remetente (CIF)" :
                    ModalidadeFrete == ModalidadeFrete.ContaDoDestinatario ? "Frete por conta do Destinat�rio (FOB)" :
                    ModalidadeFrete == ModalidadeFrete.ContaDeTerceiros ? "Frete por conta de Terceiros" :
                    ModalidadeFrete == ModalidadeFrete.ProprioContaDoRemetente ? "Transporte Pr�prio por conta do Remetente" :
                    ModalidadeFrete == ModalidadeFrete.ProprioContaDoDestinatario ? "Transporte Pr�prio por conta do Destinat�rio" :
                    ModalidadeFrete == ModalidadeFrete.SemTransporte ? "Sem Ocorr�ncia de Transporte" :
                    string.Empty;
            }
        }

        public System.Drawing.Color CorSituacao
        {
            get
            {
                return Situacao == (int)SituacaoEnum.FalhaCancelar || Situacao == (int)SituacaoEnum.FalhaEmitir ||
                    Situacao == (int)SituacaoEnum.FalhaInutilizar || Situacao == (int)SituacaoEnum.ContingenciaOffline ? System.Drawing.Color.Red :
                    Situacao == (int)SituacaoEnum.Autorizada || Situacao == (int)SituacaoEnum.FinalizadaTerceiros ? System.Drawing.Color.Blue :
                    EmitirNfFsVisible ? System.Drawing.Color.Green : System.Drawing.Color.Black;
            }
        }

        public string DescrGerarContasPagar
        {
            get { return GerarContasPagar ? "Gerar contas a pagar" : ""; }
        }

        public bool EditEmitente
        {
            get { return TipoDocumento == (int)TipoDoc.EntradaTerceiros || TipoDocumento == (int)TipoDoc.NotaCliente; }
        }

        public bool ExibirReabrir
        {
            get
            {
                LoginUsuario login = UserInfo.GetUserInfo;
                bool flagSituacao = Situacao == (int)SituacaoEnum.FinalizadaTerceiros;

                return flagSituacao && !TemMovimentacaoBemAtivo;
            }
        }

        public bool ExibirGerarEstoque
        {
            get { return EstoqueConfig.EntradaEstoqueManual; }
        }

        public string CorLinhaGrid
        {
            get
            {
                Color retorno =
                    FormaEmissao == (int)TipoEmissao.ContingenciaComSCAN ||
                    FormaEmissao == (int)TipoEmissao.ContingenciaSVCAN ||
                    FormaEmissao == (int)TipoEmissao.ContingenciaSVCRS ? Color.Blue : Color.Black;

                return retorno.Name;
            }
        }

        [Log("Tipo Fatura")]
        public string DescrTipoFatura
        {
            get { return DataSources.Instance.GetDescrTipoFaturaNF(TipoFatura); }
        }

        [Log("Per�odo Apura��o IPI")]
        public string DescrPeriodoApuracaoIpi
        {
            get { return ((PeriodoIpiEnum)PeriodoApuracaoIpi).ToString(); }
        }

        public bool IsNfEnergiaEletrica
        {
            get { return Modelo != null && Modelo.TrimStart('0') == "6"; }
        }

        public bool IsNfGas
        {
            get { return Modelo != null && Modelo.TrimStart('0') == "28"; }
        }

        public bool IsNfAgua
        {
            get { return Modelo != null && Modelo.TrimStart('0') == "29"; }
        }

        public bool IsNfComunicacao
        {
            get { return Modelo != null && Modelo.TrimStart('0') == "21"; }
        }

        public bool IsNfTelecomunicacao
        {
            get { return Modelo != null && Modelo.TrimStart('0') == "22"; }
        }

        public bool ExibirLinkInfoAdic
        {
            get
            {
                return Transporte || IsNfAgua || IsNfEnergiaEletrica || IsNfGas ||
                    IsNfComunicacao || IsNfTelecomunicacao;
            }
        }

        public bool ExibirDocRef
        {
            get { return TipoDocumento == (int)TipoDoc.EntradaTerceiros /*&& !String.IsNullOrEmpty(_infCompl) */; }
        }

        public Sync.Fiscal.Enumeracao.NFe.NotaCreditoDebito NotaCreditoDebito
        {
            get { return TipoDocumento == (int)TipoDoc.Sa�da ? Sync.Fiscal.Enumeracao.NFe.NotaCreditoDebito.Debito : Sync.Fiscal.Enumeracao.NFe.NotaCreditoDebito.Credito; }
        }

        private int _contadorNotasEFD = 1;

        internal int ContadorNotasEFD
        {
            get { return _contadorNotasEFD; }
            set { _contadorNotasEFD = value; }
        }

        public bool IsNfExtemporanea
        {
            get
            {
                DateTime saidaEnt = DataSaidaEnt.GetValueOrDefault(DataEmissao);
                return DataEmissao.Year != saidaEnt.Year || DataEmissao.Month != saidaEnt.Month;
            }
        }

        public string MensagemNaturezasOperacao
        {
            get { return NotaFiscalDAO.Instance.ObtemMensagemNaturezasOperacao(IdNf); }
        }

        public bool ExibirBoleto
        {
            get
            {
                if (!Glass.Configuracoes.FinanceiroConfig.FinanceiroRec.ExibirCnab)
                    return false;

                List<int> pagto = new List<int>() {
                    (int)NotaFiscal.FormaPagtoEnum.APrazo,
                    (int)NotaFiscal.FormaPagtoEnum.Outros
                };

                if (Situacao != (int)NotaFiscal.SituacaoEnum.Autorizada || !pagto.Contains(FormaPagto) ||
                    (FinanceiroConfig.EmitirBoletoApenasContaTipoPagtoBoleto &&
                    !PagtoNotaFiscalDAO.Instance.ObtemPagamentos(null, (int)IdNf).Any(p => p.FormaPagto == (int)FormaPagtoNotaFiscalEnum.BoletoBancario)))
                    return false;

                return ContasReceberDAO.Instance.NfeTemContasReceber(IdNf);
            }
        }

        /// <summary>
        /// Propriedade usada para o SPED.
        /// </summary>
        internal int? CstIpi { get; set; }

        /// <summary>
        /// Verifica se o valor do centro de custo foi totalmente informado.
        /// </summary>
        public bool CentroCustoCompleto
        {
            get
            {
                return TotalNota == CentroCustoAssociadoDAO.Instance.ObtemTotalPorNotaFiscal((int)IdNf);
            }
        }

        /// <summary>
        /// Verifica se deve exibir o icone do centro de custo
        /// </summary>
        public bool ExibirCentroCusto
        {
            get
            {
                return FiscalConfig.UsarControleCentroCusto && CentroCustoDAO.Instance.GetCountReal() > 0 && TipoDocumento == (int)TipoDoc.EntradaTerceiros;
            }
        }

        private List<PagtoNotaFiscal> _pagamentoNfce;

        /// <summary>
        /// Formas de Pagamento da NF
        /// </summary>
        public List<PagtoNotaFiscal> PagamentoNfce
        {
            get
            {
                if (_pagamentoNfce == null)
                    _pagamentoNfce = IdNf > 0 ? PagtoNotaFiscalDAO.Instance.ObtemPagamentos(null, (int)IdNf) 
                        : new List<PagtoNotaFiscal>();

                return _pagamentoNfce;
            }

            set
            {
                _pagamentoNfce = value;
            }
        }

        [Log("Pagamento Nota Fiscal")]
        public string PagamentoNfceStr
        {
            get
            {
                return string.Join(", ", PagamentoNfce.Select(f => Colosoft.Translator.Translate((FormaPagtoEnum)f.FormaPagto).Format()));
            }
        }

        public bool EmitirNFCeVisible
        {
            get
            {
                return Situacao == (int)SituacaoEnum.ContingenciaOffline;
            }
        }

        public bool ExibirSalvarInutilizacao
        {
            get { return Situacao == (int)SituacaoEnum.Inutilizada || 
                         Situacao == (int)SituacaoEnum.ProcessoInutilizacao || 
                         Situacao == (int)SituacaoEnum.FalhaInutilizar; }
        }

        /// <summary>
        /// Valor do sinal dos pedidos da nota fiscal
        /// </summary>
        public decimal ValoresPagosAntecipadamente
        {
            get {return NotaFiscalDAO.Instance.ObterValoresPagosAntecipadamente((int) IdNf); }
        }

        /// <summary>
        /// Descri��o das parcelas
        /// </summary>
        public string DescrFaturas
        {
            get
            {
                var retorno = string.Empty;

                if (DatasParcelas.Any() && ValoresParcelas.Any())
                {
                    for (int i = 0; i < NumParc; i++)
                        retorno += string.Format("Num.: {0} Venc.: {1} Valor.: {2} / ", i + 1, DatasParcelas[i], ValoresParcelas[i]);
                }

                return retorno;
            }
        }

        public string IdNomeDestRem
        {
            get { return (IdCliente > 0 ? IdCliente : IdFornec) + " - " + NomeDestRem; }
        }

        #endregion

        #region INFe Members

        int Sync.Fiscal.EFD.Entidade.INFe.Codigo
        {
            get { return (int)IdNf; }
        }

        int? Sync.Fiscal.EFD.Entidade.INFe.CodigoNaturezaOperacao
        {
            get { return (int?)IdNaturezaOperacao; }
        }

        int Sync.Fiscal.EFD.Entidade.INFe.CodigoCidade
        {
            get { return (int)IdCidade; }
        }

        int? Sync.Fiscal.EFD.Entidade.INFe.CodigoCfop
        {
            get { return null; }
        }

        string Sync.Fiscal.EFD.Entidade.INFe.SubSerie
        {
            get { return Subserie; }
        }

        Sync.Fiscal.Enumeracao.NFe.Situacao Sync.Fiscal.EFD.Entidade.INFe.Situacao
        {
            get { return (Sync.Fiscal.Enumeracao.NFe.Situacao)Situacao; }
        }

        int Sync.Fiscal.EFD.Entidade.INFe.FormaPagamento
        {
            get { return FormaPagto; }
        }

        string Sync.Fiscal.EFD.Entidade.INFe.InformacoesComplementares
        {
            get { return InfCompl; }
        }

        DateTime Sync.Fiscal.EFD.Entidade.INFe.DataCadastro
        {
            get { return DataCad; }
        }

        string Sync.Fiscal.EFD.Entidade.INFe.CpfCnpjEmitente
        {
            get { return CnpjEmitente; }
            set { CnpjEmitente = value; }
        }

        string Sync.Fiscal.EFD.Entidade.INFe.CpfCnpjDestinatario
        {
            get { return CpfCnpjDestRem; }
            set { CpfCnpjDestRem = value; }
        }

        bool Sync.Fiscal.EFD.Entidade.INFe.Entrada
        {
            get { return TipoDocumento == (int)TipoDoc.Entrada; }
        }

        bool Sync.Fiscal.EFD.Entidade.INFe.Saida
        {
            get { return TipoDocumento == (int)TipoDoc.Sa�da; }
        }

        bool Sync.Fiscal.EFD.Entidade.INFe.EntradaTerceiros
        {
            get { return TipoDocumento == (int)TipoDoc.EntradaTerceiros; }
        }

        bool Sync.Fiscal.EFD.Entidade.INFe.Comunicacao
        {
            get { return IsNfComunicacao; }
        }

        bool Sync.Fiscal.EFD.Entidade.INFe.Telecomunicacao
        {
            get { return IsNfTelecomunicacao; }
        }

        bool Sync.Fiscal.EFD.Entidade.INFe.Agua
        {
            get { return IsNfAgua; }
        }

        bool Sync.Fiscal.EFD.Entidade.INFe.EnergiaEletrica
        {
            get { return IsNfEnergiaEletrica; }
        }

        bool Sync.Fiscal.EFD.Entidade.INFe.Gas
        {
            get { return IsNfGas; }
        }

        bool Sync.Fiscal.EFD.Entidade.INFe.Importacao
        {
            get { return IsNfImportacao; }
        }

        bool Sync.Fiscal.EFD.Entidade.INFe.MovimentarItens
        {
            get { return true; }
        }

        DateTime? Sync.Fiscal.EFD.Entidade.INFe.DataEntradaSaida
        {
            get { return DataSaidaEnt; }
            set { DataSaidaEnt = value; }
        }

        DateTime Sync.Fiscal.EFD.Entidade.INFe.DataEmissao
        {
            get { return DataEmissao; }
            set { DataEmissao = value; }
        }

        Sync.Fiscal.Enumeracao.NFe.TipoFatura? Sync.Fiscal.EFD.Entidade.INFe.TipoFatura
        {
            get { return (Sync.Fiscal.Enumeracao.NFe.TipoFatura?)TipoFatura; }
        }

        string Sync.Fiscal.EFD.Entidade.INFe.DescricaoFatura
        {
            get { return DescrFatura; }
        }

        string Sync.Fiscal.EFD.Entidade.INFe.NumeroFatura
        {
            get { return NumFatura; }
        }

        decimal Sync.Fiscal.EFD.Entidade.INFe.ValorDesconto
        {
            get { return Desconto; }
        }

        decimal Sync.Fiscal.EFD.Entidade.INFe.TotalProdutos
        {
            get { return TotalProd; }
        }

        int? Sync.Fiscal.EFD.Entidade.INFe.ModalidadeFrete
        {
            get { return (int?)ModalidadeFrete; }
        }

        bool Sync.Fiscal.EFD.Entidade.INFe.ContingenciaFsda
        {
            get { return FormaEmissao == (int)NotaFiscal.TipoEmissao.ContingenciaFSDA; }
        }

        Sync.Fiscal.Enumeracao.NFe.PeriodoApuracaoIpi Sync.Fiscal.EFD.Entidade.INFe.PeriodoApuracaoIpi
        {
            get { return (Sync.Fiscal.Enumeracao.NFe.PeriodoApuracaoIpi)PeriodoApuracaoIpi; }
        }

        decimal Sync.Fiscal.EFD.Entidade.INFe.ValorIcms
        {
            get { return Valoricms; }
            set { Valoricms = value; }
        }

        float Sync.Fiscal.EFD.Entidade.INFe.AliqIcms
        {
            get { return AliqIcms ?? 0; }
            set { AliqIcms = value; }
        }

        int Sync.Fiscal.EFD.Entidade.INFe.NumeroNFe
        {
            get { return (int)NumeroNFe; }
        }

        #endregion

        #region IParticipante Members

        int? Sync.Fiscal.EFD.Entidade.IParticipante.CodigoLoja
        {
            get { return (int?)IdLoja; }
            set { IdLoja = (uint?)value; }
        }

        int? Sync.Fiscal.EFD.Entidade.IParticipante.CodigoCliente
        {
            get { return (int?)IdCliente; }
            set { IdCliente = (uint?)value; }
        }

        int? Sync.Fiscal.EFD.Entidade.IParticipante.CodigoFornecedor
        {
            get { return (int?)IdFornec; }
            set { IdFornec = (uint?)value; }
        }

        int? Sync.Fiscal.EFD.Entidade.IParticipante.CodigoTransportador
        {
            get { return (int?)IdTransportador; }
            set { IdTransportador = (uint?)value; }
        }

        int? Sync.Fiscal.EFD.Entidade.IParticipante.CodigoAdministradoraCartao
        {
            get { return null; }
            set { }
        }

        #endregion
    }
}