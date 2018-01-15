using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CupomFiscalDAO))]
	[PersistenceClass("cupom_fiscal_cfe")]
	public class CupomFiscal : ModelBaseCadastro
    {
        #region Propriedades

        [PersistenceProperty("IDCUPOMFISCAL", PersistenceParameterType.IdentityKey)]
        public uint IdCupomFiscal { get; set; }

        /// <summary>
        /// Número de sessão (aleatório) gerado pela aplicação durante comunicação com o SAT
        /// </summary>
        [PersistenceProperty("NUMEROSESSAO")]
        public uint NumeroSessao { get; set; }

        [PersistenceProperty("CHAVECUPOMSAT")]
        public string ChaveCupomSat { get; set; }

        [PersistenceProperty("DATAEMISSAO")]
        public DateTime DataEmissao { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("CANCELADO")]
        public string Cancelado { get; set; }

        [PersistenceProperty("TOTALCUPOM")]
        public double TotalCupom { get; set; }

        [PersistenceProperty("VALORPAGO")]
        public double ValorPago { get; set; }

        [PersistenceProperty("IDCLIENTE")]
        public uint IdCliente { get; set; }

        #endregion
    }
}