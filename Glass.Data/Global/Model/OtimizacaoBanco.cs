using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(OtimizacaoBancoDAO))]
    [PersistenceClass("otimizacao_banco")]
    public class OtimizacaoBanco
    {
        #region Propriedades

        [PersistenceProperty("DATA", PersistenceParameterType.Key)]
        public DateTime Data { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMETABELA", DirectionParameter.InputOptional)]
        public string NomeTabela { get; set; }

        #endregion
    }
}