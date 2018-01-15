using System;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(FluxoCaixaSinteticoDAO))]
    public class FluxoCaixaSintetico
    {
        #region Propriedades

        private DateTime _data;

        public DateTime Data
        {
            get { return _data.Date; }
            set { _data = value; }
        }

        public decimal? ValorEntrada { get; set; }

        public decimal? ValorSaida { get; set; }

        public bool PrevCustoFixo { get; set; }

        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        public decimal SaldoGeral { get; set; }

        public bool IsTotal { get; set; }

        public string Descricao { get; set; }

        public uint NumSeqMov { get; set; }

        #endregion
    }
}