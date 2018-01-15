using System;
using System.Collections.Generic;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class ConhecimentoTransporteRodoviario
    {
        private Glass.Data.Model.Cte.ConhecimentoTransporteRodoviario _conhecimentoTransporteRod;

        #region construtores

        public ConhecimentoTransporteRodoviario(Glass.Data.Model.Cte.ConhecimentoTransporteRodoviario conhecimentoTransporteRod)
        {
            _conhecimentoTransporteRod = conhecimentoTransporteRod ?? new Glass.Data.Model.Cte.ConhecimentoTransporteRodoviario();
        }

        #endregion

        #region Propriedades

        public uint IdCte 
        {
            get { return _conhecimentoTransporteRod.IdCte; }
            set { _conhecimentoTransporteRod.IdCte = value; }
        }

        public DateTime? DataPrevistaEntrega 
        {
            get { return _conhecimentoTransporteRod.DataPrevistaEntrega; }
            set { _conhecimentoTransporteRod.DataPrevistaEntrega = value; }
        }

        public bool Lotacao 
        {
            get { return _conhecimentoTransporteRod.Lotacao; }
            set { _conhecimentoTransporteRod.Lotacao = value; }
        }

        public string CIOT 
        {
            get { return _conhecimentoTransporteRod.CIOT; }
            set { _conhecimentoTransporteRod.CIOT = value; }
        }

        public List<LacreCteRod> ObjLacreCteRod { get; set; }

        public List<MotoristaCteRod> ObjMotoristaCteRod { get; set; }

        public List<OrdemColetaCteRod> ObjOrdemColetaCteRod { get; set; }

        public List<ValePedagioCteRod> ObjValePedagioCteRod { get; set; }

        #endregion

        #region Propriedade suporte

        public string DataPrevistaEntregaString
        {
            get { return DataPrevistaEntrega != null ? DataPrevistaEntrega.Value.ToShortDateString() : String.Empty; }
        }

        #endregion
    }
}
