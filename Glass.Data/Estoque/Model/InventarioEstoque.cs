using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceClass("inventario_estoque")]
    [PersistenceBaseDAO(typeof(InventarioEstoqueDAO))]
    public class InventarioEstoque : ModelBaseCadastro
    {
        #region Enumerações

        public enum SituacaoEnum
        {
            Aberto = 1,
            EmContagem,
            Finalizado,
            Cancelado,
            Confirmado
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDINVENTARIOESTOQUE", PersistenceParameterType.IdentityKey)]
        public uint IdInventarioEstoque { get; set; }

        [Log("Loja", "NomeFantasia", typeof(LojaDAO))]
        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [Log("Grupo", "Descricao", typeof(GrupoProdDAO))]
        [PersistenceProperty("IDGRUPOPROD")]
        public uint IdGrupoProd { get; set; }

        [Log("Subgrupo", "Descricao", typeof(SubgrupoProdDAO))]
        [PersistenceProperty("IDSUBGRUPOPROD")]
        public uint? IdSubgrupoProd { get; set; }

        [PersistenceProperty("IDFUNCFIN")]
        public uint? IdFuncFin { get; set; }

        [PersistenceProperty("DATAFIN")]
        public DateTime? DataFin { get; set; }

        [PersistenceProperty("IDFUNCCONF")]
        public uint? IdFuncConf { get; set; }

        [PersistenceProperty("DATACONF")]
        public DateTime? DataConf { get; set; }

        [PersistenceProperty("SITUACAO")]
        public SituacaoEnum Situacao { get; set; }

        #endregion
    }
}
