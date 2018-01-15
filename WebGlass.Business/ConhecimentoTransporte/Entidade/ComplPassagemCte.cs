using System;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class ComplPassagemCte
    {
        private Glass.Data.Model.Cte.ComplPassagemCte _complPassagemCte;

        #region construtores

        public ComplPassagemCte(Glass.Data.Model.Cte.ComplPassagemCte complPassagemCte)
        {
            _complPassagemCte = complPassagemCte ?? new Glass.Data.Model.Cte.ComplPassagemCte();
        }

        #endregion

        #region Propriedades

        public uint IdCte 
        {
            get { return _complPassagemCte.IdCte; }
            set { _complPassagemCte.IdCte = value; }
        }
       
        public int NumSeqPassagem 
        {
            get { return _complPassagemCte.NumSeqPassagem; }
            set { _complPassagemCte.NumSeqPassagem = value; }
        }

        public string SiglaPassagem 
        {
            get { return _complPassagemCte.SiglaPassagem; }
            set { _complPassagemCte.SiglaPassagem = value; }
        }

        #endregion
    }
}
