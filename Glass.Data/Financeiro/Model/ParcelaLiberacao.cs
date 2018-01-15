using System;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ParcelaLiberacaoDAO))]
    public class ParcelaLiberacao
    {
        #region Propriedades

        public decimal ValorParcela { get; set; }

        public DateTime DataParcela { get; set; }

        #endregion
    }
}