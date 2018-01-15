using System;
using GDA;
using Glass.Data.DAL.CTe;
using System.ComponentModel;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(InfoCargaCteDAO))]
    [PersistenceClass("info_carga_cte")]
    public class InfoCargaCte
    {
        #region Enumeradores

        public enum TipoUnidadeEnum
        {
            [Description("M³")]
            M3,

            [Description("KG")]
            Kg,

            [Description("TON")]
            Ton,

            [Description("UN")]
            Un,

            [Description("LT")]
            Lt,

            [Description("MMBTU")]
            Mmbtu
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDINFOCARGA", PersistenceParameterType.Key)]
        public uint IdInfoCarga { get; set; }

        [PersistenceProperty("IDCTE")]
        public uint IdCte { get; set; }

        [PersistenceProperty("TIPOUNIDADE")]
        public int TipoUnidade { get; set; }

        [PersistenceProperty("TIPOMEDIDA")]
        public string TipoMedida { get; set; }

        [PersistenceProperty("QUANTIDADE")]
        public float Quantidade { get; set; }

        #endregion
    }
}
