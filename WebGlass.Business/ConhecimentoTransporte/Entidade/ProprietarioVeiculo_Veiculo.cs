using System;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class ProprietarioVeiculo_Veiculo
    {
        private Glass.Data.Model.Cte.ProprietarioVeiculo_Veiculo _propVeic_Veic;

        #region construtores

        public ProprietarioVeiculo_Veiculo(Glass.Data.Model.Cte.ProprietarioVeiculo_Veiculo propVeic_Veic)
        {
            _propVeic_Veic = propVeic_Veic ?? new Glass.Data.Model.Cte.ProprietarioVeiculo_Veiculo();
        }

        #endregion

        public uint IdPropVeic { get; set; }
        
        public string Placa { get; set; }
    }
}
