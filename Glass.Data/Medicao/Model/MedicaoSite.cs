using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MedicaoSiteDAO))]
    [PersistenceClass("medicao_site")]
    public class MedicaoSite
    {
        #region Propriedades

        [PersistenceProperty("CodMedicao", PersistenceParameterType.IdentityKey)]
        public uint CodMedicao { get; set; }

        [PersistenceProperty("Nome")]
        public string Nome { get; set; }

        [PersistenceProperty("Endereco")]
        public string Endereco { get; set; }

        [PersistenceProperty("Telefone")]
        public string Telefone { get; set; }

        [PersistenceProperty("Email")]
        public string Email { get; set; }

        [PersistenceProperty("DiaHoraDisp")]
        public string DiaHoraDisp { get; set; }

        [PersistenceProperty("TpObra")]
        public string TpObra { get; set; }

        [PersistenceProperty("Responsavel")]
        public string Responsavel { get; set; }

        [PersistenceProperty("DataPedido")]
        public DateTime DataPedido { get; set; }

        [PersistenceProperty("Emitido")]
        public bool Emitido { get; set; }

        [PersistenceProperty("Observacoes", DirectionParameter.InputOptional)]
        public string Observacoes { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string _FlagEmitido
        {
            get
            {
                if (Emitido)
                    return "Sim";
                else
                    return "Não";
            }
                
        }

        #endregion
    }
}