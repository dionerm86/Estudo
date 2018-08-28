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
                var data = new DateTime();

                var idTipoCartao = this.IdConta != null ? Helper.UtilsPlanoConta.ObterTipoCartaoPorConta((uint)this.IdConta) : null;
                data = idTipoCartao == null ? this._data.AddDays(1) : this._data;

                while (!Glass.FuncoesData.DiaUtil(data))
                {
                    data = data.AddDays(1);
                }

                return data.Date;
            }
            set { this._data = value; }
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