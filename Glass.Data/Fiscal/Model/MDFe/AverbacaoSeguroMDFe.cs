using GDA;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model
{
    [Serializable]
    [PersistenceBaseDAO(typeof(AverbacaoSeguroMDFeDAO))]
    [PersistenceClass("averbacao_seguro_mdfe")]
    public class AverbacaoSeguroMDFe
    {
        [PersistenceProperty("IDMANIFESTOELETRONICO")]
        [PersistenceForeignKey(typeof(ManifestoEletronico), "IdManifestoEletronico")]
        public int IdManifestoEletronico { get; set; }

        [PersistenceProperty("NUMEROAVERBACAO")]
        public string NumeroAverbacao { get; set; }
    }
}
