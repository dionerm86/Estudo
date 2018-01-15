using System;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class EntregaCte
    {
        private Glass.Data.Model.Cte.EntregaCte _entregaCte;

        #region construtores

        public EntregaCte(Glass.Data.Model.Cte.EntregaCte entregaCte)
        {
            _entregaCte = entregaCte ?? new Glass.Data.Model.Cte.EntregaCte();
        }

        #endregion

        #region Propriedades

        public uint IdCte 
        {
            get { return _entregaCte.IdCte; }
            set { _entregaCte.IdCte = value; }
        }

        public int TipoPeriodoData 
        {
            get { return _entregaCte.TipoPeriodoData; }
            set { _entregaCte.TipoPeriodoData = value; }
        }

        public int TipoPeriodoHora 
        {
            get { return _entregaCte.TipoPeriodoHora; }
            set { _entregaCte.TipoPeriodoHora = value; }
        }

        public DateTime DataHoraProg 
        {
            get { return _entregaCte.DataHoraProg; }
            set { _entregaCte.DataHoraProg = value; }
        }

        public DateTime DataHoraIni 
        {
            get { return _entregaCte.DataHoraIni; }
            set { _entregaCte.DataHoraIni = value; }
        }

        public DateTime DataHoraFim 
        {
            get { return _entregaCte.DataHoraFim; }
            set { _entregaCte.DataHoraFim = value; }
        }

        public string DescricaoTipoPeriodoData
        {
            get
            {
                switch (TipoPeriodoData)
                {
                    case 0: return "Sem data definida";
                    case 1: return "Na data";
                    case 2: return "Até a data";
                    case 3: return "A partir da data";
                    default: return "No período";
                }
            }
        }

        public string DescricaoTipoPeriodoHora
        {
            get
            {
                switch (TipoPeriodoHora)
                {
                    case 0: return "Sem hora definida";
                    case 1: return "No horário";
                    case 2: return "Até o horário";
                    case 3: return "A partir do horário";
                    default: return "No intervalo de tempo";
                }
            }
        }

        #endregion
    }
}
