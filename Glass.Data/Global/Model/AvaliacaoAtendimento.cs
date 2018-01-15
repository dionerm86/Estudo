using GDA;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AvaliacaoAtendimentoDAO))]
    [PersistenceClass("avaliacao_atendimento")]
    public class AvaliacaoAtendimento
    {
        #region Enumeradores

        public enum SatisfacaoEnum
        {
            MuitoBaixa = 1,
            Baixa,
            Neutra,
            Alta,
            MuitoAlta
        }

        #endregion

        [PersistenceProperty("IDAVALIACAOATENDIMENTO", PersistenceParameterType.IdentityKey)]
        public uint IdAvaliacaoAtendimento { get; set; }

        [PersistenceProperty("IDCHAMADO")]
        public uint IdChamado { get; set; }

        [PersistenceProperty("ANALISTA")]
        public string Analista { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("RESOLUCAO")]
        public string Resolucao { get; set; }
    
        [PersistenceProperty("DATAFINALIZACAO")]
        public DateTime DataFinalizacao { get; set; }

        /// <summary>
        /// 1-Aprovado
        /// 2-Desaprovado
        /// </summary>
        [PersistenceProperty("AVALIACAO")]
        public int Avaliacao { get; set; }

        [PersistenceProperty("SATISFACAO")]
        public SatisfacaoEnum Satisfacao { get; set; }

        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [PersistenceProperty("DATAAVALIACAO")]
        public DateTime? DataAvaliacao { get; set; }
    }
}
