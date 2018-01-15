using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceClass("IMPOSTO_NCM_UF"),
    PersistenceBaseDAO(typeof(ImpostoNcmUFDAO))]
    public class ImpostoNcmUF
    {
        [PersistenceProperty("IDIMPOSTONCMUF", PersistenceParameterType.Key)]
        public int IdImpostoNCMUf { get; set; }

        [PersistenceProperty("NCM")]
        public string Ncm { get; set; }

        [PersistenceProperty("AliquotaNacional")]
        public float AliquotaNacional { get; set; }

        [PersistenceProperty("AliquotaImportacao")]
        public float AliquotaImportacao { get; set; }

        [PersistenceProperty("AliquotaEstadual")]
        public float AliquotaEstadual { get; set; }

        [PersistenceProperty("AliquotaMunicipal")]
        public float AliquotaMunicipal { get; set; }

        [PersistenceProperty("VigenciaInicio")]
        public DateTime? VigenciaInicio { get; set; }

        [PersistenceProperty("VigenciaFim")]
        public DateTime? VigenciaFim { get; set; }

        [PersistenceProperty("UF")]
        public string UF { get; set; }

        [PersistenceProperty("Fonte")]
        public string Fonte { get; set; }

        [PersistenceProperty("Versao")]
        public string Versao { get; set; }
    }
}
