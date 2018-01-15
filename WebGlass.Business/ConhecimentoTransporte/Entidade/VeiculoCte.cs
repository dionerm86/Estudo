using System;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class VeiculoCte
    {
        private Glass.Data.Model.Cte.VeiculoCte _veiculoCte;

        #region contrutores

        public VeiculoCte(Glass.Data.Model.Cte.VeiculoCte veiculoCte)
        {
            _veiculoCte = veiculoCte ?? new Glass.Data.Model.Cte.VeiculoCte();
        }

        #endregion

        #region Propriedades
        
        public uint IdCte 
        {
            get { return _veiculoCte.IdCte; }
            set { _veiculoCte.IdCte = value; }
        }

        public string Placa
        {
            get { return _veiculoCte.Placa; }
            set { _veiculoCte.Placa = value; }
        }   
        
        public decimal ValorFrete 
        {
            get { return _veiculoCte.ValorFrete; }
            set { _veiculoCte.ValorFrete = value; }
        }        

        #endregion
    }
}
