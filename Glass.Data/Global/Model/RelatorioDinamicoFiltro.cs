using GDA;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Tipos de filtro possíveis
    /// </summary>
    public enum TipoControle
    {
        /// <summary>
        /// TextBox (apenas números)
        /// </summary>
        [Description("Campo texto (apenas números)")]
        Numero = 1,

        /// <summary>
        /// Texto (TextBox)
        /// </summary>
        [Description("Campo texto")]
        Texto,

        /// <summary>
        /// Lista de seleção (dropdownlist)
        /// </summary>
        [Description("Lista de seleção")]
        ListaDeSelecao,

        /// <summary>
        /// Período de data (ctrlData)
        /// </summary>
        [Description("Período de data")]
        PeriodoDeData,

        /// <summary>
        /// Caixa de seleção (checkbox)
        /// </summary>
        [Description("Caixa de seleção")]
        CaixaDeSelecao,

        /// <summary>
        /// Múltipla seleção (CheckBoxListDropDown)
        /// </summary>
        [Description("Múltipla seleção")]
        MultiplaSelecao,

        /// <summary>
        /// Agrupamento (dropdownlist)
        /// </summary>
        [Description("Agrupamento")]
        Agrupamento,

        /// <summary>
        /// Ordenação (dropdownlist)
        /// </summary>
        [Description("Ordenação")]
        Ordenacaoo,

        /// <summary>
        /// Campo de data (ctrlData)
        /// </summary>
        [Description("Campo de data")]
        Data,

        /// <summary>
        /// Campo de data e hora (ctrlData)
        /// </summary>
        [Description("Campo de data e hora")]
        DataHora,

        /// <summary>
        /// Período de data e hora (ctrlData)
        /// </summary>
        [Description("Período de data e hora")]
        PeriodoDeDataHora,

        /// <summary>
        /// Campo de pesquisa de cliente
        /// </summary>
        [Description("Campo Cliente")]
        Cliente,
    }

    [PersistenceClass("relatorio_dinamico_filtro")]
    public class RelatorioDinamicoFiltro : Colosoft.Data.BaseModel
    {
        [PersistenceProperty("IDRELATORIODINAMICOFILTRO", PersistenceParameterType.IdentityKey)]
        public int IdRelatorioDinamicoFiltro { get; set; }

        [PersistenceForeignKey(typeof(RelatorioDinamico), "IdRelatorioDinamico")]
        [PersistenceProperty("IDRELATORIODINAMICO")]
        public int IdRelatorioDinamico { get; set; }

        [PersistenceProperty("NOMEFILTRO")]
        public string NomeFiltro { get; set; }

        [PersistenceProperty("NOMECOLUNASQL")]
        public string NomeColunaSql { get; set; }

        [PersistenceProperty("TIPOCONTROLE")]
        public TipoControle TipoControle { get; set; }

        [PersistenceProperty("OPCOES")]
        public string Opcoes { get; set; }

        [PersistenceProperty("VALORPADRAO")]
        public string ValorPadrao { get; set; }

        [PersistenceProperty("NumSeq")]
        public int NumSeq { get; set; }
    }
}
