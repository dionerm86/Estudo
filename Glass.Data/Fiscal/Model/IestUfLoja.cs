using GDA;
using Glass.Data.DAL;
using Glass.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(IestUfLojaDAO))]
    [PersistenceClass("iest_uf_loja")]
    public class IestUfLoja : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDIESTUFLOJA", PersistenceParameterType.IdentityKey)]
        public int IdIestUfLoja { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("NOMEUF")]
        public string NomeUf { get; set; }

        [Log("Inscrição Estadual Substituto Tributário")]
        [PersistenceProperty("INSCESTST")]
        public string InscEstSt { get; set; }

        #endregion
    }
}