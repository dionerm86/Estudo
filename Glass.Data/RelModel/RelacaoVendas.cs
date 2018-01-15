using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(RelacaoVendasDAO))]
    [PersistenceClass("relacao_vendas")]
    public class RelacaoVendas
    {
        #region Propriedades

        [PersistenceProperty("Agrupar")]
        public long Agrupar { get; set; }

        [PersistenceProperty("IdFunc")]
        public uint IdFunc { get; set; }

        [PersistenceProperty("NomeFunc")]
        public string NomeFunc { get; set; }

        [PersistenceProperty("NumPedidos")]
        public long NumPedidos { get; set; }

        [PersistenceProperty("NumDias")]
        public long NumDias { get; set; }

        [PersistenceProperty("ValorTotal")]
        public decimal ValorTotal { get; set; }

        [PersistenceProperty("TotM2")]
        public double TotM2 { get; set; }

        [PersistenceProperty("Criterio")]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        public float MediaQtdDia
        {
            get { return (float)NumPedidos / (float)(NumDias > 0 ? NumDias : 1); }
        }

        public decimal MediaValorDia
        {
            get { return ValorTotal / (decimal)(NumDias > 0 ? NumDias : 1); }
        }

        public string Vendedor
        {
            get { return NomeFunc; }
        }

        #endregion
    }
}