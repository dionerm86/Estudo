using System;
using System.Collections.Generic;
using GDA;

namespace Glass.Data.RelModel
{
    public class LivroRegistroApuracao
    {
        #region Propriedades

        [PersistenceProperty("UltimoLancamento", DirectionParameter.InputOptional)]
        public string UltimoLancamento { get; set; }

        [PersistenceProperty("Termo", DirectionParameter.InputOptional)]
        public string Termo { get; set; }

        [PersistenceProperty("LocalData", DirectionParameter.InputOptional)]
        public string LocalData { get; set; }

        [PersistenceProperty("NumeroOrdem", DirectionParameter.InputOptional)]
        public string NumeroOrdem { get; set; }

        [PersistenceProperty("Nome", DirectionParameter.InputOptional)]
        public string Nome { get; set; }

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
        
        [PersistenceProperty("InscEstadual", DirectionParameter.InputOptional)]
        public string InscEstadual { get; set; }

        [PersistenceProperty("CNPJ", DirectionParameter.InputOptional)]
        public string CNPJ { get; set; }

        #endregion

        #region Campos referentes ao Resumo da Apuração

        public decimal TotalDebito { get; set; }

        public string OutrosDebitosDescicao { get; set; }

        public decimal OutrosDebitosValor { get; set; }

        public string EstornoCreditosDescicao { get; set; }

        public decimal EstornoCreditosValor { get; set; }

        public decimal TotalCredito { get; set; }

        public string OutrosCreditosDescicao { get; set; }

        public decimal OutrosCreditosValor { get; set; }

        public string EstornoDebitosDescicao { get; set; }

        public decimal EstornoDebitosValor { get; set; }

        public decimal TotalDebitoApuracao { get; set; }

        public decimal SubTotalCreditoApuracao { get; set; }

        public decimal TotalCreditoApuracao { get; set; }

        public string DeducaoDescricao { get; set; }

        public decimal DeducaoValor { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string NIRE { get; set; }

        public DateTime? DataNIRE { get; set; }

        public string Responsavel { get; set; }

        public string CargoResponsavel { get; set; }

        public string CPFResponsavel { get; set; }

        public string Contador { get; set; }

        public string CRCContador { get; set; }

        public string CPFContador { get; set; }

        public int TotalPaginas { get; set; }

        public List<ItemLivroRegistroApuracao> Itens { get; set; }

        public LivroRegistroApuracao()
        {
            Itens = new List<ItemLivroRegistroApuracao>();
        }

        #endregion
    }

    public class ItemLivroRegistroApuracao
    {
        #region Propriedades

        [PersistenceProperty("ValorContabil", DirectionParameter.InputOptional)]
        public decimal ValorContabil { get; set; }

        /// <summary>
        /// Contab
        /// </summary>
        [PersistenceProperty("CodigoContabil", DirectionParameter.InputOptional)]
        public uint CodigoContabil { get; set; }

        /// <summary>
        /// Fis
        /// </summary>
        [PersistenceProperty("CodigoFiscal", DirectionParameter.InputOptional)]
        public string CodigoFiscal { get; set; }

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
        public decimal NumeroPagina { get; set; }

        [PersistenceProperty("Estado", DirectionParameter.InputOptional)]
        public string Estado { get; set; }
        
        #endregion
    }
}