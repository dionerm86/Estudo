using GDA;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MovEstoqueCentroCustoDAO))]
    [PersistenceClass("mov_estoque_centro_custo")]
    public class MovEstoqueCentroCusto
    {
        #region Enumeradores

        public enum TipoMovEnum
        {
            Entrada = 1,
            Saida
        }

        #endregion

        #region Propiedades

        [PersistenceProperty("IdMovEstoqueCentroCusto", PersistenceParameterType.IdentityKey)]
        public int IdMovEstoqueCentroCusto { get; set; }

        [PersistenceProperty("IdCompra")]
        public int? IdCompra { get; set; }

        [PersistenceProperty("IdPedidoInterno")]
        public int? IdPedidoInterno { get; set; }

        [PersistenceProperty("IdProd")]
        public int IdProd { get; set; }

        [PersistenceProperty("IdLoja")]
        public int IdLoja { get; set; }

        [PersistenceProperty("IdConta")]
        public int? IdConta { get; set; }

        [PersistenceProperty("TipoMov")]
        public TipoMovEnum TipoMov { get; set; }

        [PersistenceProperty("QtdeMov")]
        public decimal QtdeMov { get; set; }

        [PersistenceProperty("ValorMov")]
        public decimal ValorMov { get; set; }

        [PersistenceProperty("SaldoQtdeMov")]
        public decimal SaldoQtdeMov { get; set; }

        [PersistenceProperty("SaldoValorMov")]
        public decimal SaldoValorMov { get; set; }

        [PersistenceProperty("DataMov")]
        public DateTime DataMov { get; set; }

        [PersistenceProperty("IdFunc")]
        public int IdFunc { get; set; }

        #endregion

    }
}
