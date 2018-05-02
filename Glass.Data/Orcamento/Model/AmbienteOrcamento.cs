﻿using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AmbienteOrcamentoDAO))]
    [PersistenceClass("ambiente_orcamento")]
    public class AmbienteOrcamento
    {
        #region Propriedaes

        [PersistenceProperty("IDAMBIENTEORCA", PersistenceParameterType.IdentityKey)]
        public uint IdAmbienteOrca { get; set; }

        [PersistenceProperty("IDORCAMENTO")]
        public uint IdOrcamento { get; set; }

        [PersistenceProperty("AMBIENTE", 50)]
        public string Ambiente { get; set; }

        [PersistenceProperty("DESCRICAO", 1000)]
        public string Descricao { get; set; }

        /// <summary>
        /// Percentual da rentabilidade.
        /// </summary>
        [PersistenceProperty("PERCENTUALRENTABILIDADE")]
        public decimal PercentualRentabilidade { get; set; }

        /// <summary>
        /// Valor da rentabilidade financeira.
        /// </summary>
        [PersistenceProperty("RENTABILIDADEFINANCEIRA")]
        public decimal RentabilidadeFinanceira { get; set; }


        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("VALORPRODUTOS", DirectionParameter.InputOptional)]
        public decimal ValorProdutos { get; set; }

        #endregion
    }
}