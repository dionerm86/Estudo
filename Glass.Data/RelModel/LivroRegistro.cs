using System;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(LivroRegistroDAO))]
    public class LivroRegistro
    {
        #region Dados da empresa

        /// <summary>
        /// Nome da empresa
        /// </summary>
        [PersistenceProperty("Nome", DirectionParameter.InputOptional)]
        public string Firma { get; set; }

        /// <summary>
        /// Insc. Estadual da empresa
        /// </summary>
        [PersistenceProperty("InscEstadual", DirectionParameter.InputOptional)]
        public string InscricaoEstadual { get; set; }

        /// <summary>
        /// CNPJ da empresa
        /// </summary>
        [PersistenceProperty("CNPJ", DirectionParameter.InputOptional)]
        public string CNPJ { get; set; }

        [PersistenceProperty("Endereco", DirectionParameter.InputOptional)]
        public string Endereco { get; set; }

        [PersistenceProperty("Bairro", DirectionParameter.InputOptional)]
        public string Bairro { get; set; }

        [PersistenceProperty("Cidade", DirectionParameter.InputOptional)]
        public string Cidade { get; set; }

        [PersistenceProperty("Estado", DirectionParameter.InputOptional)]
        public string Estado { get; set; }

        [PersistenceProperty("CEP", DirectionParameter.InputOptional)]
        public string CEP { get; set; }

        /// <summary>
        /// Número de inscrição na junta comercial
        /// </summary>
        public string NIRE { get; set; }

        /// <summary>
        /// Data do número de inscrição na junta comercial
        /// </summary>
        public DateTime? DataNIRE { get; set; }


        #endregion

        #region Dados do relatório

        /// <summary>
        /// Período em que o relatório é gerado
        /// </summary>
        public string Periodo { get; set; }

        [PersistenceProperty("Termo", DirectionParameter.InputOptional)]
        public string Termo { get; set; }

        #region Dados usados nos termos de abertura e encerramento

        [PersistenceProperty("LocalData", DirectionParameter.InputOptional)]
        public string LocalData { get; set; }

        [PersistenceProperty("NumeroOrdem", DirectionParameter.InputOptional)]
        public string NumeroOrdem { get; set; }

        [PersistenceProperty("UltimoLancamento", DirectionParameter.InputOptional)]
        public string UltimoLancamento { get; set; }

        /// <summary>
        /// Total de paginas do relatório
        /// </summary>
        public int TotalPaginas { get; set; }

        /// <summary>
        /// Responsável pela empresa
        /// </summary>
        public string Responsavel { get; set; }

        /// <summary>
        /// Cargo do reponsável pela empresa
        /// </summary>
        public string CargoResponsavel { get; set; }

        /// <summary>
        /// CPF  do reponsável pela empresa
        /// </summary>
        public string CPFResponsavel { get; set; }

        /// <summary>
        /// Contador da empresa
        /// </summary>
        public string Contador { get; set; }

        /// <summary>
        /// CRC do contador da empresa
        /// </summary>
        public string CRCContador { get; set; }

        /// <summary>
        /// CPF  do contador da empresa
        /// </summary>
        public string CPFContador { get; set; }

        #endregion

        #endregion

        #region Campos referentes ao Resumo da Apuração

        /// <summary>
        /// Campo 001
        /// </summary>
        public decimal TotalDebito { get; set; }

        /// <summary>
        /// Campo 005
        /// </summary>
        public decimal TotalCredito { get; set; }

        /// <summary>
        /// Campo 004
        /// </summary>
        public decimal TotalDebitoApuracao { get; set; }

        /// <summary>
        /// Campo 008
        /// </summary>
        public decimal SubTotalCreditoApuracao { get; set; }

        /// <summary>
        /// Campo 010
        /// </summary>
        public decimal TotalCreditoApuracao { get; set; }

        /// <summary>
        /// Campo 009
        /// </summary>
        public decimal SaldoCredorMesAnterior { get; set; }

        /// <summary>
        /// Campo 011
        /// </summary>
        public decimal SaldoDevedor { get; set; }

        /// <summary>
        /// Campo 014
        /// </summary>
        public decimal SaldoCredor { get; set; }

        /// <summary>
        /// Campo 013
        /// </summary>
        public decimal ImpostoRecolher { get; set; }


        #region Campos informados pelo usuário

        /// <summary>
        /// Campo 002
        /// </summary>
        public string OutrosDebitosDescicao { get; set; }

        /// <summary>
        /// Campo 002
        /// </summary>
        public decimal OutrosDebitosValor { get; set; }

        /// <summary>
        /// Campo 003
        /// </summary>
        public string EstornoCreditosDescicao { get; set; }

        /// <summary>
        /// Campo 003
        /// </summary>
        public decimal EstornoCreditosValor { get; set; }

        /// <summary>
        /// Campo 006
        /// </summary>
        public string OutrosCreditosDescicao { get; set; }

        /// <summary>
        /// Campo 006
        /// </summary>
        public decimal OutrosCreditosValor { get; set; }

        /// <summary>
        /// Campo 007
        /// </summary>
        public string EstornoDebitosDescicao { get; set; }

        /// <summary>
        /// Campo 007
        /// </summary>
        public decimal EstornoDebitosValor { get; set; }

        /// <summary>
        /// Campo 012
        /// </summary>
        public string DeducaoDescricao { get; set; }

        /// <summary>
        /// Campo 012
        /// </summary>
        public decimal DeducaoValor { get; set; }

        #endregion

        #endregion

        #region Campos referentes ao Resumo da Apuração por Substituição Tributária

        /// <summary>
        /// Campo 001
        /// </summary>
        public decimal TotalDebitoST { get; set; }

        /// <summary>
        /// Campo 005
        /// </summary>
        public decimal TotalCreditoST { get; set; }

        /// <summary>
        /// Campo 004
        /// </summary>
        public decimal TotalDebitoApuracaoST { get; set; }

        /// <summary>
        /// Campo 008
        /// </summary>
        public decimal SubTotalCreditoApuracaoST { get; set; }

        /// <summary>
        /// Campo 010
        /// </summary>
        public decimal TotalCreditoApuracaoST { get; set; }

        /// <summary>
        /// Campo 009
        /// </summary>
        public decimal SaldoCredorMesAnteriorST { get; set; }

        /// <summary>
        /// Campo 011
        /// </summary>
        public decimal SaldoDevedorST { get; set; }

        /// <summary>
        /// Campo 014
        /// </summary>
        public decimal SaldoCredorST { get; set; }

        /// <summary>
        /// Campo 013
        /// </summary>
        public decimal ImpostoRecolherST { get; set; }

        #endregion

    }

    #region Classe que representa Apuraçõe de ICMS e IPI

    /// <summary>
    /// Referente aos itens de apuração ICMS/IPI
    /// </summary>
    [PersistenceBaseDAO(typeof(ItemApuracaoDAO))]
    public class ItemApuracao
    {
        [PersistenceProperty("CodigoContabil", DirectionParameter.InputOptional)]
        public uint Contabil { get; set; }

        [PersistenceProperty("CodigoFiscal", DirectionParameter.InputOptional)]
        public string Fiscal { get; set; }

        [PersistenceProperty("ValorContabil", DirectionParameter.InputOptional)]
        public decimal ValorContabil { get; set; }

        [PersistenceProperty("BaseCalculo", DirectionParameter.InputOptional)]
        public decimal BaseCalculo { get; set; }

        [PersistenceProperty("Imposto", DirectionParameter.InputOptional)]
        public decimal Imposto { get; set; }

        [PersistenceProperty("IsentasNaoTributadas", DirectionParameter.InputOptional)]
        public decimal IsentasNaoTributadas { get; set; }

        [PersistenceProperty("Outras", DirectionParameter.InputOptional)]
        public decimal Outras { get; set; }

        [PersistenceProperty("Operacao", DirectionParameter.InputOptional)]
        public int Operacao { get; set; }

        [PersistenceProperty("NumeroPagina", DirectionParameter.InputOptional)]
        public decimal Folha { get; set; }

        [PersistenceProperty("Estado", DirectionParameter.InputOptional)]
        public string Estado { get; set; }

        [PersistenceProperty("AliquotaICMS", DirectionParameter.InputOptional)]
        public decimal AliquotaICMS { get; set; }

        [PersistenceProperty("IdCFOP", DirectionParameter.InputOptional)]
        public uint IdCFOP { get; set; }

        [PersistenceProperty("IdNF", DirectionParameter.InputOptional)]
        public uint IdNF { get; set; }

        [PersistenceProperty("ImpostoST", DirectionParameter.InputOptional)]
        public decimal ImpostoST { get; set; }

        public bool ExibirNoRelatorio { get; set; }
    }

    #endregion

    #region Classe que representa Registro de Entrada

    [PersistenceBaseDAO(typeof(ItemEntradaDAO))]
    public class ItemEntrada
    {
        [PersistenceProperty("DataEntrada", DirectionParameter.InputOptional)]
        public DateTime DataEntrada { get; set; }

        [PersistenceProperty("Especie", DirectionParameter.InputOptional)]
        public string Especie { get; set; }

        [PersistenceProperty("SerieSubSerie", DirectionParameter.InputOptional)]
        public string SerieSubSerie { get; set; }

        [PersistenceProperty("Numero", DirectionParameter.InputOptional)]
        public string NumeroNota { get; set; }

        [PersistenceProperty("DataDocumento", DirectionParameter.InputOptional)]
        public DateTime DataDocumento { get; set; }

        [PersistenceProperty("CodigoEmitente", DirectionParameter.InputOptional)]
        public uint CodigoEmitente { get; set; }

        [PersistenceProperty("UFOrigem", DirectionParameter.InputOptional)]
        public string UFOrigem { get; set; }

        [PersistenceProperty("ValorContabil", DirectionParameter.InputOptional)]
        public decimal ValorContabil { get; set; }

        [PersistenceProperty("CodigoContabil", DirectionParameter.InputOptional)]
        public uint CodigoContabil { get; set; }

        [PersistenceProperty("Fiscal", DirectionParameter.InputOptional)]
        public string CFOP { get; set; }

        /// <summary>
        /// ICMS IPI
        /// </summary>
        [PersistenceProperty("IcmsIpi", DirectionParameter.InputOptional)]
        public string TipoImposto { get; set; }

        /// <summary>
        /// Código de valores fiscais
        /// 1 - Oper. com crédito do Imposto
        /// 2 - Oper. sem crédito do Imposto - Isentas ou não Tributadas
        /// 3 - Oper. sem crédito do Imposto - Outras
        /// </summary>
        [PersistenceProperty("CodValorFiscal", DirectionParameter.InputOptional)]
        public UInt64 CodValorFiscal { get; set; }

        [PersistenceProperty("BaseCalculo", DirectionParameter.InputOptional)]
        public decimal BaseCalculo { get; set; }

        [PersistenceProperty("Aliquota", DirectionParameter.InputOptional)]
        public decimal Aliquota { get; set; }

        [PersistenceProperty("ImpostoCreditado", DirectionParameter.InputOptional)]
        public decimal ImpostoCreditado { get; set; }

        [PersistenceProperty("IsentasNaoTributadas", DirectionParameter.InputOptional)]
        public decimal IsentasNaoTributadas { get; set; }

        [PersistenceProperty("Observacao", DirectionParameter.InputOptional)]
        public string Observacao { get; set; }

        [PersistenceProperty("NomeEmitente", DirectionParameter.InputOptional)]
        public string NomeEmitente { get; set; }

        [PersistenceProperty("CNPJEmitente", DirectionParameter.InputOptional)]
        public string CNPJEmitente { get; set; }

        [PersistenceProperty("InscEstEmitente", DirectionParameter.InputOptional)]
        public string InscEstEmitente { get; set; }

        [PersistenceProperty("IdNF", DirectionParameter.InputOptional)]
        public uint IdNF { get; set; }

        [PersistenceProperty("CST", DirectionParameter.InputOptional)]
        public string CST { get; set; }

        [PersistenceProperty("IdProdNF", DirectionParameter.InputOptional)]
        public uint IdProdNF { get; set; }

        [PersistenceProperty("NumeroPagina", DirectionParameter.InputOptional)]
        public decimal NumeroPagina { get; set; }

        [PersistenceProperty("CorLinha", DirectionParameter.InputOptional)]
        public string CorLinha { get; set; }

        [PersistenceProperty("SubTributaria", DirectionParameter.InputOptional)]
        public decimal SubTributaria { get; set; }

        [PersistenceProperty("BaseCalculoST", DirectionParameter.InputOptional)]
        public decimal BaseCalculoST { get; set; }

        public bool ExibirDadosFornecedor { get; set; }

        public bool ExibirDadosST { get; set; }

        public ItemEntrada()
        {
            CorLinha = "Black";
            ExibirDadosFornecedor = false;
            ExibirDadosST = false;
        }
    }

    #endregion

    #region Classe que representa Registro de Saída

    [PersistenceBaseDAO(typeof(ItemSaidaDAO))]
    public class ItemSaida
    {
        [PersistenceProperty("Especie", DirectionParameter.InputOptional)]
        public string Especie { get; set; }

        [PersistenceProperty("SerieSubSerie", DirectionParameter.InputOptional)]
        public string SerieSubSerie { get; set; }

        [PersistenceProperty("NumeroNota", DirectionParameter.InputOptional)]
        public uint NumeroNota { get; set; }

        [PersistenceProperty("Dia", DirectionParameter.InputOptional)]
        public string Dia { get; set; }

        [PersistenceProperty("UFDestinatario", DirectionParameter.InputOptional)]
        public string UFDestinatario { get; set; }

        [PersistenceProperty("ValorContabil", DirectionParameter.InputOptional)]
        public decimal ValorContabil { get; set; }

        /// <summary>
        /// Contab
        /// </summary>
        [PersistenceProperty("CodigoContabil", DirectionParameter.InputOptional)]
        public uint CodigoContabil { get; set; }

        /// <summary>
        /// Fis - CFOP
        /// </summary>
        [PersistenceProperty("CodigoFiscal", DirectionParameter.InputOptional)]
        public string CodigoFiscal { get; set; }

        /// <summary>
        /// ICMS IPI
        /// </summary>
        [PersistenceProperty("IcmsIpi", DirectionParameter.InputOptional)]
        public string TipoImposto { get; set; }

        [PersistenceProperty("BaseCalculo", DirectionParameter.InputOptional)]
        public decimal BaseCalculo { get; set; }

        [PersistenceProperty("Aliquota", DirectionParameter.InputOptional)]
        public decimal Aliquota { get; set; }

        [PersistenceProperty("ImpostoDebitado", DirectionParameter.InputOptional)]
        public decimal ImpostoDebitado { get; set; }

        [PersistenceProperty("IsentasNaoTributadas", DirectionParameter.InputOptional)]
        public decimal IsentasNaoTributadas { get; set; }

        [PersistenceProperty("Outras", DirectionParameter.InputOptional)]
        public decimal Outras { get; set; }

        [PersistenceProperty("SubTributaria", DirectionParameter.InputOptional)]
        public decimal SubTributaria { get; set; }

        [PersistenceProperty("BaseCalculoST", DirectionParameter.InputOptional)]
        public decimal BaseCalculoST { get; set; }

        [PersistenceProperty("Observacao", DirectionParameter.InputOptional)]
        public string Observacao { get; set; }

        [PersistenceProperty("NumeroPagina", DirectionParameter.InputOptional)]
        public decimal NumeroPagina { get; set; }

        [PersistenceProperty("CorLinha", DirectionParameter.InputOptional)]
        public string CorLinha { get; set; }

        [PersistenceProperty("Borda", DirectionParameter.InputOptional)]
        public string Borda { get; set; }
        
        [PersistenceProperty("IdNF", DirectionParameter.InputOptional)]
        public uint IdNF { get; set; }

        [PersistenceProperty("CST", DirectionParameter.InputOptional)]
        public string CST { get; set; }

        [PersistenceProperty("IdProdNF", DirectionParameter.InputOptional)]
        public uint IdProdNF { get; set; }

        public bool ExibirDadosST { get; set; }

          public ItemSaida()
        {
            CorLinha = "Black";
            ExibirDadosST = false;
            Borda = "Solid";
        }
    }

    #endregion
}