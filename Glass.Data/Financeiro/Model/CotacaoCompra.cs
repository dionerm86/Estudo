using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CotacaoCompraDAO))]
    [PersistenceClass("cotacao_compra")]
    public class CotacaoCompra
    {
        #region Enumerações

        public enum SituacaoEnum
        {
            Aberta = 1,
            Finalizada,
            Cancelada
        }

        public enum TipoCalculoCotacao
        {
            MenorCusto = 1,
            MenorPrazo
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDCOTACAOCOMPRA", PersistenceParameterType.Key)]
        public uint IdCotacaoCompra { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBSERVACAO")]
        public string Observacao { get; set; }

        [PersistenceProperty("SITUACAO")]
        public SituacaoEnum Situacao { get; set; }

        [Log("Funcionário Cadastro", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNCCAD", DirectionParameter.OutputOnlyInsert)]
        public uint IdFuncCad { get; set; }

        [Log("Data Cadastro")]
        [PersistenceProperty("DATACAD", DirectionParameter.OutputOnlyInsert)]
        public DateTime DataCad { get; set; }

        [Log("Funcionário Finalização", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNCFIN")]
        public uint? IdFuncFin { get; set; }

        [Log("Data Finalização")]
        [PersistenceProperty("DATAFIN")]
        public DateTime? DataFin { get; set; }

        [Log("Prioridade")]
        [PersistenceProperty("PRIORIDADECALCULOFINALIZACAO")]
        public TipoCalculoCotacao PrioridadeCalculoFinalizacao { get; set; }

        #endregion
    }
}
