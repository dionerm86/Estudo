using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ConciliacaoBancariaDAO))]
    [PersistenceClass("conciliacao_bancaria")]
    public class ConciliacaoBancaria : ModelBaseCadastro
    {
        public enum SituacaoEnum
        {
            Ativa = 1,
            Cancelada
        }

        #region Propriedades

        [PersistenceProperty("IDCONCILIACAOBANCARIA", PersistenceParameterType.IdentityKey)]
        public uint IdConciliacaoBancaria { get; set; }

        [Log("Conta bancária", "Descricao", typeof(ContaBancoDAO))]
        [PersistenceProperty("IDCONTABANCO")]
        public uint IdContaBanco { get; set; }

        [Log("Data conciliada")]
        [PersistenceProperty("DATACONCILIADA")]
        public DateTime DataConciliada { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        #endregion

        #region Propriedades extendidas

        [PersistenceProperty("DESCRCONTABANCO", DirectionParameter.InputOptional)]
        public string DescrContaBanco { get; set; }

        #endregion

        #region Propriedades de suporte

        [Log("Situação")]
        public string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case 1: return "Ativa";
                    case 2: return "Cancelada";
                    default: return String.Empty;
                }
            }
        }

        #endregion
    }
}
