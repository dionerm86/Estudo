using System;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class CobrancaDuplCte
    {
        private Glass.Data.Model.Cte.CobrancaDuplCte _cobrancaDuplCte;

        #region contrutores

        public CobrancaDuplCte(Glass.Data.Model.Cte.CobrancaDuplCte cobrancaDuplCte)
        {
            _cobrancaDuplCte = cobrancaDuplCte ?? new Glass.Data.Model.Cte.CobrancaDuplCte();
        }

        #endregion

        #region Propriedades

        public uint IdCte
        {
            get { return _cobrancaDuplCte.IdCte; }
            set { _cobrancaDuplCte.IdCte = value; }
        }

        public string NumeroDupl 
        {
            get { return _cobrancaDuplCte.NumeroDupl; }
            set { _cobrancaDuplCte.NumeroDupl = value; }
        }

        public DateTime? DataVenc 
        {
            get { return _cobrancaDuplCte.DataVenc; }
            set { _cobrancaDuplCte.DataVenc = value; }
        }

        public decimal ValorDupl 
        {
            get { return _cobrancaDuplCte.ValorDupl; }
            set { _cobrancaDuplCte.ValorDupl = value; }
        }

        #endregion
    }
}
