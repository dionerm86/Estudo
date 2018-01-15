using System;
using GDA;
using Glass.Data.RelDAL;
using Glass.Data.DAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(PrevFinanPedidosDAO))]
    [PersistenceClass("PrevFinanPedidos")]
    public class PrevFinanPedidos
    {
        #region Propiedades

        [PersistenceProperty("IdPedido")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("CodCliente")]
        public string CodCliente { get; set; }

        [PersistenceProperty("IdCliente")]
        public uint IdCliente { get; set; }

        [PersistenceProperty("Cliente")]
        public string Cliente { get; set; }

        [PersistenceProperty("Funcionario")]
        public string Funcionario { get; set; }

        [PersistenceProperty("Loja")]
        public string Loja { get; set; }

        [PersistenceProperty("Entrega")]
        public DateTime Entrega { get; set; }

        [PersistenceProperty("SituacaoProducao")]
        public int SituacaoProducao { get; set; }

        [PersistenceProperty("Valor")]
        public decimal Valor { get; set; }


        #endregion

        #region Propiedades de Suporte

        public string SituacaoProducaoString
        {
            get { return PedidoDAO.Instance.GetSituacaoProducaoPedido(SituacaoProducao); }
        }

        public string DataEntregaString
        {
            get { return Entrega.ToString("dd/MM/yyyy"); }
        }

        #endregion
    }
}