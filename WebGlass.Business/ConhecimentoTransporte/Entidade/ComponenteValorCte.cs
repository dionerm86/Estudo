using System;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class ComponenteValorCte
    {
        private Glass.Data.Model.Cte.ComponenteValorCte _componenteValorCte;

        #region construtores

        public ComponenteValorCte(Glass.Data.Model.Cte.ComponenteValorCte componenteValorCte)
        {
            _componenteValorCte = componenteValorCte ?? new Glass.Data.Model.Cte.ComponenteValorCte();
        }

        #endregion

        #region Propriedades
        
        public uint IdCte 
        {
            get { return _componenteValorCte.IdCte; }
            set { _componenteValorCte.IdCte = value; }
        }

        public string NomeComponente 
        {
            get { return _componenteValorCte.NomeComponente; }
            set { _componenteValorCte.NomeComponente = value; }
        }

        public decimal ValorComponente 
        {
            get { return _componenteValorCte.ValorComponente; }
            set { _componenteValorCte.ValorComponente = value; }
        }

        #endregion
    }
}
