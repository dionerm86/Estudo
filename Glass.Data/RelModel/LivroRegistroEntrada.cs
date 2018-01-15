using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(LivroRegistroEntradaDAO))]
    public class LivroRegistroEntrada
    {
        #region Propriedades

        [PersistenceProperty("SobNumero", DirectionParameter.InputOptional)]
        public string SobNumero { get; set; }

        [PersistenceProperty("ArquivoEm", DirectionParameter.InputOptional)]
        public string ArquivoEm { get; set; }

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

        [PersistenceProperty("InscEstadual", DirectionParameter.InputOptional)]
        public string InscEstadual { get; set; }

        [PersistenceProperty("CNPJ", DirectionParameter.InputOptional)]
        public string CNPJ { get; set; }

        #endregion

        #region Propriedades de Suporte

        public List<ItemLivroRegistroEntrada> Itens { get; set; }

        public LivroRegistroEntrada()
        {
            Itens = new List<ItemLivroRegistroEntrada>();
        }

        #endregion
    }
    [PersistenceBaseDAO(typeof(ItemLivroRegistroEntradaDAO))]
    public class ItemLivroRegistroEntrada
    {
        #region Propriedades

        [PersistenceProperty("DataEntrada", DirectionParameter.InputOptional)]
        public DateTime DataEntrada { get; set; }

        [PersistenceProperty("Especie", DirectionParameter.InputOptional)]
        public string Especie { get; set; }

        [PersistenceProperty("SerieSubSerie", DirectionParameter.InputOptional)]
        public string SerieSubSerie { get; set; }

        [PersistenceProperty("NumeroNota", DirectionParameter.InputOptional)]
        public uint NumeroNota { get; set; }

        [PersistenceProperty("DataDocumento", DirectionParameter.InputOptional)]
        public DateTime DataDocumento { get; set; }

        [PersistenceProperty("CodigoEmitente", DirectionParameter.InputOptional)]
        public string CodigoEmitente { get; set; }

        [PersistenceProperty("UFOrigem", DirectionParameter.InputOptional)]
        public string UFOrigem { get; set; }

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

        /// <summary>
        /// ICMS IPI
        /// </summary>
        [PersistenceProperty("TipoImposto", DirectionParameter.InputOptional)]
        public string TipoImposto { get; set; }

        /// <summary>
        /// Código de valores fiscais
        /// 1 - Oper. com crédito do Imposto
        /// 2 - Oper. sem crédito do Imposto - Isentas ou não Tributadas
        /// 3 - Oper. sem crédito do Imposto - Outras
        /// </summary>
        [PersistenceProperty("CodTipoImposto", DirectionParameter.InputOptional)]
        public uint CodTipoImposto { get; set; }

        [PersistenceProperty("BaseCalculo", DirectionParameter.InputOptional)]
        public decimal BaseCalculo { get; set; }

        [PersistenceProperty("Aliquota", DirectionParameter.InputOptional)]
        public Single Aliquota { get; set; }

        [PersistenceProperty("ImpostoCreditado", DirectionParameter.InputOptional)]
        public decimal ImpostoCreditado { get; set; }

        [PersistenceProperty("Observacao", DirectionParameter.InputOptional)]
        public string Observacao { get; set; }

        #endregion
    }
}