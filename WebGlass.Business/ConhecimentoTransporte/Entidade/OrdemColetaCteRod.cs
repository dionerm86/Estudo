using System;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class OrdemColetaCteRod
    {
        private Glass.Data.Model.Cte.OrdemColetaCteRod _ordemColetaCteRod;

        #region construtores

        public OrdemColetaCteRod(Glass.Data.Model.Cte.OrdemColetaCteRod ordemColetaCteRod)
        {
            _ordemColetaCteRod = ordemColetaCteRod ?? new Glass.Data.Model.Cte.OrdemColetaCteRod();
        }

        #endregion

        #region Propriedades

        public uint IdCte 
        {
            get { return _ordemColetaCteRod.IdCte; }
            set { _ordemColetaCteRod.IdCte = value; }
        }

        public uint IdTransportador 
        {
            get { return _ordemColetaCteRod.IdTransportador; }
            set { _ordemColetaCteRod.IdTransportador = value; }
        }

        public int Numero 
        {
            get { return _ordemColetaCteRod.Numero; }
            set { _ordemColetaCteRod.Numero = value; }
        }

        public string Serie 
        {
            get { return _ordemColetaCteRod.Serie; }
            set { _ordemColetaCteRod.Serie = value; }
        }

        public DateTime ?DataEmissao 
        {
            get { return _ordemColetaCteRod.DataEmissao; }
            set { _ordemColetaCteRod.DataEmissao = value; }
        }

        public Glass.Data.Model.Transportador ObjTransportador { get; set; }

        #endregion
    }
}
