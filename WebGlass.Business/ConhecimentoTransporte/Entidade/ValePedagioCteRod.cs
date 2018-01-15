using System;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class ValePedagioCteRod
    {
        private Glass.Data.Model.Cte.ValePedagioCteRod _valePedagioCteRod;

        #region construtores

        public ValePedagioCteRod(Glass.Data.Model.Cte.ValePedagioCteRod valePedagioCteRod)
        {
            _valePedagioCteRod = valePedagioCteRod ?? new Glass.Data.Model.Cte.ValePedagioCteRod();
        }

        #endregion

        #region Propriedades
        
        public uint IdCte 
        {
            get { return _valePedagioCteRod.IdCte; }
            set { _valePedagioCteRod.IdCte = value; }
        }

        public uint IdFornec 
        {
            get { return _valePedagioCteRod.IdFornec; }
            set { _valePedagioCteRod.IdFornec = value; }
        }

        public string NumeroCompra 
        {
            get { return _valePedagioCteRod.NumeroCompra; }
            set { _valePedagioCteRod.NumeroCompra = value; }
        }

        public string CnpjComprador 
        {
            get { return _valePedagioCteRod.CnpjComprador; }
            set { _valePedagioCteRod.CnpjComprador = value; }
        }
        
        #endregion
    }
}
