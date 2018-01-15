using System;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class LogCte
    {
        private Glass.Data.Model.Cte.LogCte _logCte;

        #region contrutores

        public LogCte()
        {

        }
        public LogCte(Glass.Data.Model.Cte.LogCte logCte)
        {
            _logCte = logCte ?? new Glass.Data.Model.Cte.LogCte();
        }

        #endregion

        #region Propriedades
        
        public uint IdLogCte
        { 
            get { return _logCte.IdLogCte; }
            set { _logCte.IdLogCte = value; }
        }

        public uint IdCte 
        {
            get { return _logCte.IdCte; }
            set { _logCte.IdCte = value; }
        }

        public string Evento 
        {
            get { return _logCte.Evento; }
            set { _logCte.Evento = value; }    
        }

        public int Codigo 
        {
            get { return _logCte.Codigo; }
            set { _logCte.Codigo = value; }
        }

        public string Descricao 
        {
            get { return _logCte.Descricao; }
            set { _logCte.Descricao = value; }
        }

        public DateTime DataHora 
        {
            get { return _logCte.DataHora; }
            set { _logCte.DataHora = value; }
        }

        #endregion

        #region Propriedades de Suporte

        public string NumRecibo { get; set; }

        public string NumProtocolo { get; set; }

        #endregion
    }
}
