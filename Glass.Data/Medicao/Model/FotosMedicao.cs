using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FotosMedicaoDAO))]
    [PersistenceClass("fotos_medicao")]
    public class FotosMedicao : IFoto
    {
        #region Propriedades

        [PersistenceProperty("IDFOTO", PersistenceParameterType.IdentityKey)]
        public override uint IdFoto { get; set; }

        [PersistenceProperty("IDMEDICAO")]
        public uint IdMedicao { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public override string Descricao { get; set; }

        [PersistenceProperty("EXTENSAO")]
        public override string Extensao { get; set; }

        [PersistenceProperty("AREAQUADRADA")]
        public override Single AreaQuadrada { get; set; }

        [PersistenceProperty("METROLINEAR")]
        public override Single MetroLinear { get; set; }

        /// <summary>
        /// Código interno do produto associado 
        /// </summary>
        [PersistenceProperty("CODINTERNO")]
        public override string CodInterno { get; set; }

        [PersistenceProperty("FIXACAO")]
        public uint Fixacao { get; set; }

        [PersistenceProperty("ESCALAP1X")]
        public int EscalaP1X { get; set; }

        [PersistenceProperty("ESCALAP1Y")]
        public int EscalaP1Y { get; set; }

        [PersistenceProperty("ESCALAP2X")]
        public int EscalaP2X { get; set; }

        [PersistenceProperty("ESCALAP2Y")]
        public int EscalaP2Y { get; set; }

        [PersistenceProperty("ESCALA")]
        public int Escala { get; set; }

        #endregion

        #region IFoto Members

        public override uint IdParent
        {
            get { return IdMedicao; }
            set { IdMedicao = value; }
        }

        public override IFoto.TipoFoto Tipo
        {
            get { return TipoFoto.Medicao; }
        }

        public override bool PermiteCalcularArea
        {
            get { return true; }
        }

        public override bool ApenasImagens
        {
            get { return true; }
        }

        #endregion
    }
}