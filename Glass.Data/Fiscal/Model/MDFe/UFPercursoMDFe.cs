using GDA;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model
{
    [Serializable]
    [PersistenceBaseDAO(typeof(UFPercursoMDFeDAO))]
    [PersistenceClass("uf_percurso_mdfe")]
    public class UFPercursoMDFe
    {
        #region Propriedades

        [PersistenceProperty("IDUFPERCURSOMDFE", PersistenceParameterType.IdentityKey)]
        public int IdUFPercursoMDFe { get; set; }

        [PersistenceProperty("IDMANIFESTOELETRONICO")]
        [PersistenceForeignKey(typeof(ManifestoEletronico), "IdManifestoEletronico")]
        public int IdManifestoEletronico { get; set; }

        [PersistenceProperty("UFPERCURSO")]
        public string UFPercurso { get; set; }

        #endregion
    }
}
