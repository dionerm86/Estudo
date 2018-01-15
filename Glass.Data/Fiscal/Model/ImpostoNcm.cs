using System;
using System.Collections.Generic;
using System.Text;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceClass("IMPOSTO_NCM"),
    PersistenceBaseDAO(typeof(ImpostoNcmDAO))]
    public class ImpostoNcm
    {
        [PersistenceProperty("NCM", PersistenceParameterType.Key)]
        public string Ncm { get; set; }

        [PersistenceProperty("AliquotaNacional")]
        public float AliquotaNacional { get; set; }

        [PersistenceProperty("AliquotaImportacao")]
        public float AliquotaImportacao { get; set; }
    }
}
