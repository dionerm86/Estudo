using System;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class LacreCteRod
    {
        private Glass.Data.Model.Cte.LacreCteRod _lacreCteRod;

        #region construtores

        public LacreCteRod(Glass.Data.Model.Cte.LacreCteRod lacreCteRod)
        {
            _lacreCteRod = lacreCteRod ?? new Glass.Data.Model.Cte.LacreCteRod();
        }

        #endregion

        #region Propriedades

        public uint IdCte 
        {
            get { return _lacreCteRod.IdCte; }
            set { _lacreCteRod.IdCte = value; }
        }

        public string NumeroLacre 
        {
            get { return _lacreCteRod.NumeroLacre; }
            set { _lacreCteRod.NumeroLacre = value; }
        }

        #endregion
    }
}
