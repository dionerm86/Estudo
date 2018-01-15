using System;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ProducaoFornoResumoDAO))]
    [PersistenceClass("producao_forno_resumo")]
    public class ProducaoFornoResumo
    {
        #region Propriedades

        [PersistenceProperty("Data")]
        public DateTime? DataHora { get; set; }

        [PersistenceProperty("IdCorVidro")]
        public uint IdCorVidro { get; set; }

        [PersistenceProperty("DescrCorVidro")]
        public string DescrCorVidro { get; set; }

        [PersistenceProperty("Espessura")]
        public float Espessura { get; set; }

        [PersistenceProperty("Producao")]
        public bool Producao { get; set; }

        [PersistenceProperty("TotM2Turno1", DirectionParameter.InputOptional)]
        public double TotM2Turno1 { get; set; }

        [PersistenceProperty("TotM2Turno2", DirectionParameter.InputOptional)]
        public double TotM2Turno2 { get; set; }

        [PersistenceProperty("TotM2Turno3", DirectionParameter.InputOptional)]
        public double TotM2Turno3 { get; set; }

        [PersistenceProperty("TotM2Turno4", DirectionParameter.InputOptional)]
        public double TotM2Turno4 { get; set; }

        [PersistenceProperty("TotM2Turno5", DirectionParameter.InputOptional)]
        public double TotM2Turno5 { get; set; }

        [PersistenceProperty("NumSeqTurno")]
        public uint NumSeqTurno { get; set; }

        [PersistenceProperty("Turno")]
        public string Turno { get; set; }

        [PersistenceProperty("Criterio")]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string Dia
        {
            get { return DataHora != null ? DataHora.Value.Day.ToString() : "Total"; }
        }

        public string Tipo
        {
            get { return Producao ? Configuracoes.ProducaoConfig.DescrUsarTipoProducao : "Eng."; }
        }

        public double TotM2
        {
            get { return Math.Round(TotM2Turno1 + TotM2Turno2 + TotM2Turno3 + TotM2Turno4 + TotM2Turno5, 2); }
        }

        #endregion

        #region Propriedades para o relatório

        [PersistenceProperty("TituloGrupo", DirectionParameter.InputOptional)]
        public string TituloGrupo { get; set; }

        [PersistenceProperty("Titulo", DirectionParameter.InputOptional)]
        public string Titulo { get; set; }

        #endregion
    }
}