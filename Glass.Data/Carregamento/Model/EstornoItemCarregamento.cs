using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(EstornoItemCarregamentoDAO))]
    [PersistenceClass("estorno_item_carregamento")]
    public class EstornoItemCarregamento
    {
        #region Propiedades 

        [PersistenceProperty("IDESTORNO", PersistenceParameterType.IdentityKey)]
        public uint IdEstorno { get; set; }

        [PersistenceProperty("IDITEMCARREGAMENTO")]
        [PersistenceForeignMember(typeof(ItemCarregamento), "IdItemCarregamento")]
        public uint IdItemCarregamento { get; set; }

        [PersistenceProperty("MOTIVO")]
        public string Motivo { get; set; }

        [PersistenceProperty("USUCAD")]
        public uint UsuCad { get; set; }

        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        #endregion

        #region Propiedades de Estendidas

        [PersistenceProperty("NomeFuncionario", DirectionParameter.InputOptional)]
        public string NomeFuncionario { get; set; }

        [PersistenceProperty("CodInternoDescrPeca", DirectionParameter.InputOptional)]
        public string CodInternoDescrPeca { get; set; }

        #endregion
    }
}
