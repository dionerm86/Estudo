using GDA;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model
{
    [Serializable]
    [PersistenceBaseDAO(typeof(CidadeCargaMDFeDAO))]
    [PersistenceClass("cidade_carga_mdfe")]
    public class CidadeCargaMDFe
    {
        #region Propriedades

        [PersistenceProperty("IDMANIFESTOELETRONICO")]
        [PersistenceForeignKey(typeof(ManifestoEletronico), "IdManifestoEletronico")]
        public int IdManifestoEletronico { get; set; }

        [PersistenceProperty("IDCIDADE")]
        [PersistenceForeignKey(typeof(Cidade), "IdCidade")]
        public int IdCidade { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMECIDADE", DirectionParameter.InputOptional)]
        public string NomeCidade { get; set; }

        #endregion
    }
}
