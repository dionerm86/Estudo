using System;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class ComplCte
    {
        private Glass.Data.Model.Cte.ComplCte _complCte;

        #region construtores

        public ComplCte(Glass.Data.Model.Cte.ComplCte complCte)
        {
            _complCte = complCte ?? new Glass.Data.Model.Cte.ComplCte();
        }

        #endregion

        #region Propriedades
        
        public uint IdCte 
        {
            get { return _complCte.IdCte; }
            set { _complCte.IdCte = value; }
        }

        public uint IdRota 
        {
            get { return _complCte.IdRota; }
            set { _complCte.IdRota = value; }
        }

        public string CaractTransporte 
        {
            get { return _complCte.CaractTransporte; }
            set { _complCte.CaractTransporte = value; }
        }

        public string CaractServico 
        {
            get { return _complCte.CaractServico; }
            set { _complCte.CaractServico = value; }
        }

        public string SiglaOrigem 
        {
            get { return _complCte.SiglaOrigem; }
            set { _complCte.SiglaOrigem = value; }
        }

        public string SiglaDestino 
        {
            get { return _complCte.SiglaDestino; }
            set { _complCte.SiglaDestino = value; }
        }

        public ComplPassagemCte ObjComplPassagemCte { get; set; }        

        #endregion

    }
}
