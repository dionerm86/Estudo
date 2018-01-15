using System;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class MotoristaCteRod
    {
        private Glass.Data.Model.Cte.MotoristaCteRod _motoristaCteRod;

        #region construtores

        public MotoristaCteRod(Glass.Data.Model.Cte.MotoristaCteRod motoristaCteRod)
        {
            _motoristaCteRod = motoristaCteRod ?? new Glass.Data.Model.Cte.MotoristaCteRod();
        }

        #endregion

        #region Propriedades

        public uint IdCte 
        {
            get { return _motoristaCteRod.IdCte; }
            set { _motoristaCteRod.IdCte = value; }
        }
        
        public uint IdFunc 
        {
            get { return _motoristaCteRod.IdFunc; }
            set { _motoristaCteRod.IdFunc = value; }
        }

        #endregion
    }
}
