using System;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(FluxoCaixaDAO))]
    public class FluxoCaixa
    {
        #region Propriedades

        private DateTime _data;

        [PersistenceProperty("DATA")]
        public DateTime Data
        {
            get
            {
                var data = (int)_data.DayOfWeek == 0 || (int)_data.DayOfWeek == 6 ? _data.AddDays(1) : _data;
                var idTipoCartao = IdConta != null ? Helper.UtilsPlanoConta.ObterTipoCartaoPorConta((uint)IdConta) : null;

                data = idTipoCartao == null ? data.AddDays(1) : data;

                return data.Date;
            }
            set { _data = value; }
        }

        [PersistenceProperty("IDCONTA")]
        public long? IdConta { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("PARCEIRO")]
        public string Parceiro { get; set; }

        /// <summary>
        /// 1-Entrada
        /// 2-Saída
        /// </summary>
        [PersistenceProperty("TIPOMOV")]
        public long TipoMov { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("PREVCUSTOFIXO")]
        public bool PrevCustoFixo { get; set; }

        #endregion

        #region Propriedades de Suporte

        public decimal SaldoDoDia { get; set; }

        public decimal SaldoGeral { get; set; }

        public string Criterio { get; set; }

        public decimal? Credito
        {
            get { return TipoMov == 1 ? (decimal?)Valor : null; }
        }

        public decimal? Debito
        {
            get { return TipoMov == 2 ? (decimal?)Valor : null; }
        }

        private bool _isTotal = false;

        public bool IsTotal
        {
            get { return _isTotal; }
            set { _isTotal = value; }
        }

        public uint NumSeqMov { get; set; }

        internal bool NaoExibirSintetico { get; set; }

        #endregion
    }
}