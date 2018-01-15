using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PerdaChapaVidroDAO))]
    [PersistenceClass("perda_chapa_vidro")]
    public class PerdaChapaVidro
    {
        #region Propiedades

        [Log("Id. Perda Chapa Vidro")]
        [PersistenceProperty("IDPERDACHAPAVIDRO", PersistenceParameterType.IdentityKey)]
        public uint IdPerdaChapaVidro { get; set; }

        [PersistenceProperty("IDPRODIMPRESSAO")]
        public uint IdProdImpressao { get; set; }

        [PersistenceProperty("IDTIPOPERDA")]
        public uint IdTipoPerda { get; set; }

        [PersistenceProperty("IDSUBTIPOPERDA")]
        public uint? IdSubTipoPerda { get; set; }

        [Log("Data Perda")]
        [PersistenceProperty("DATAPERDA")]
        public DateTime DataPerda { get; set; }

        [PersistenceProperty("IDFUNCPERDA")]
        public uint IdFuncPerda { get; set; }

        [Log("Obs.")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("CANCELADO")]
        public bool Cancelado { get; set; }
        
        #endregion

        #region Propiedades Estendidas

        [PersistenceProperty("IDNF", DirectionParameter.InputOptional)]
        public uint? idNf { get; set; }

        [PersistenceProperty("IDPRODNF", DirectionParameter.InputOptional)]
        public uint? IdProdNf { get; set; }

        [PersistenceProperty("IDIMPRESSAO", DirectionParameter.InputOptional)]
        public uint? IdImpressao { get; set; }

        [PersistenceProperty("POSICAOPROD", DirectionParameter.InputOptional)]
        public int PosicaoProd { get; set; }

        [PersistenceProperty("ITEMETIQUETA", DirectionParameter.InputOptional)]
        public int ItemEtiqueta { get; set; }

        [PersistenceProperty("QTDEPROD", DirectionParameter.InputOptional)]
        public int QtdeProd { get; set; }

        [PersistenceProperty("IDPROD", DirectionParameter.InputOptional)]
        public uint IdProd { get; set; }

        [Log("Descr. Prod")]
        [PersistenceProperty("DescrProd", DirectionParameter.InputOptional)]
        public string DescrProd { get; set; }

        [Log("Tipo Perda")]
        [PersistenceProperty("TipoPerda", DirectionParameter.InputOptional)]
        public string TipoPerda { get; set; }

        [Log("Subtipo Perda")]
        [PersistenceProperty("SubtipoPerda", DirectionParameter.InputOptional)]
        public string SubtipoPerda { get; set; }

        [Log("Etiqueta")]
        [PersistenceProperty("NumEtiqueta", DirectionParameter.InputOptional)]
        public string NumEtiqueta { get; set; }

        [Log("Func. Perda")]
        [PersistenceProperty("FuncPerda", DirectionParameter.InputOptional)]
        public string FuncPerda { get; set; }

        #endregion
    }
}