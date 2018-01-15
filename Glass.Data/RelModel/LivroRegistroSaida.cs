using System;
using System.Collections.Generic;
using GDA;

namespace Glass.Data.RelModel
{
    public class LivroRegistroSaida
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

        public List<ItemLivroRegistroSaida> Itens { get; set; }

        public LivroRegistroSaida()
        {
            Itens = new List<ItemLivroRegistroSaida>();
        }

        #endregion
    }

    public class ItemLivroRegistroSaida
    {
        #region Propriedades

        [PersistenceProperty("Especie", DirectionParameter.InputOptional)]
        public string Especie { get; set; }

        [PersistenceProperty("SerieSubSerie", DirectionParameter.InputOptional)]
        public string SerieSubSerie { get; set; }

        [PersistenceProperty("NumeroNota", DirectionParameter.InputOptional)]
        public uint NumeroNota { get; set; }

        [PersistenceProperty("IdNF", DirectionParameter.InputOptional)]
        public uint IdNF { get; set; }

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
        /// Fis
        /// </summary>
        [PersistenceProperty("CodigoFiscal", DirectionParameter.InputOptional)]
        public string CodigoFiscal { get; set; }

        /// <summary>
        /// ICMS IPI
        /// </summary>
        [PersistenceProperty("TipoImposto", DirectionParameter.InputOptional)]
        public string TipoImposto { get; set; }

        [PersistenceProperty("BaseCalculo", DirectionParameter.InputOptional)]
        public double BaseCalculo { get; set; }

        [PersistenceProperty("Aliquota", DirectionParameter.InputOptional)]
        public Single Aliquota { get; set; }

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

        #endregion

        #region Propriedades de Suporte

        public bool ExibirDadosST { get; set; }

        public ItemLivroRegistroSaida()
        {
            CorLinha = "Black";
            ExibirDadosST = false;
        }

        #endregion
    }
}