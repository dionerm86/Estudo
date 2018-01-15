using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ChartOrcaVendasDAO))]
    public class ChartOrcaVendas
    {
        #region Propriedades

        public string Periodo { get; set; }

        public string Venda { get; set; }

        public string Orcamento { get; set; }

        #endregion

        #region Propriedades de Suporte

        public decimal OrcamentoDouble
        {
            get { return !string.IsNullOrEmpty(Orcamento) ? decimal.Parse(Orcamento) : 0; }
        }

        public decimal VendaDouble
        {
            get { return !string.IsNullOrEmpty(Venda) ? decimal.Parse(Venda) : 0; }
        }

        #endregion
    }

    public class ChartOrcaVendasImagem
    {
        public byte[] Buffer { get; set; }
    }
}