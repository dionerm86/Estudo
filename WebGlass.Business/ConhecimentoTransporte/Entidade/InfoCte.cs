using System;
using System.Collections.Generic;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class InfoCte
    {
        private Glass.Data.Model.Cte.InfoCte _infoCte;

        #region construtores

        public InfoCte()
        {

        }

        public InfoCte(Glass.Data.Model.Cte.InfoCte infoCte)
        {
            _infoCte = infoCte ?? new Glass.Data.Model.Cte.InfoCte();
        }

        #endregion

        #region Propriedades

        public uint IdCte 
        {
            get { return _infoCte.IdCte; }
            set { _infoCte.IdCte = value; }
        }

        public string ProdutoPredominante
        {
            get { return _infoCte.ProdutoPredominante; }
            set { _infoCte.ProdutoPredominante = value; }
        }

        public decimal ValorCarga 
        {
            get { return _infoCte.ValorCarga; }
            set { _infoCte.ValorCarga = value; }
        }

        public string OutrasCaract 
        {
            get { return _infoCte.OutrasCaract; }
            set { _infoCte.OutrasCaract = value; }
        }        

        public List<InfoCargaCte> ObjInfoCargaCte { get; set; }

        #endregion
    }
}
