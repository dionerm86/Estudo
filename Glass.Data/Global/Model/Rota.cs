using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.DAL;
using System.ComponentModel;
using Glass.Log;

namespace Glass.Data.Model
{
    /// <summary>
    /// Possíveis dias da semana para a rota.
    /// </summary>
    [Flags]
    public enum DiasSemana
    {
        /// <summary>
        /// Nenhum.
        /// </summary>
        [Description("Nenhum")]
        Nenhum = 0,

        /// <summary>
        /// Domingo.
        /// </summary>
        [Description("Domingo")]
        Domingo = 1,

        /// <summary>
        /// Segunda-Feira.
        /// </summary>
        [Description("Segunda-Feira")]
        Segunda = 2,

        /// <summary>
        /// Terça-Feira.
        /// </summary>
        [Description("Terça-Feira")]
        Terca = 4,

        /// <summary>
        /// Quarta-Feira.
        /// </summary>
        [Description("Quarta-Feira")]
        Quarta = 8,

        /// <summary>
        /// Quinta-Feira.
        /// </summary>
        [Description("Quinta-Feira")]
        Quinta = 16,

        /// <summary>
        /// Sexta-Feira.
        /// </summary>
        [Description("Sexta-Feira")]
        Sexta = 32,

        /// <summary>
        /// Sábado.
        /// </summary>
        [Description("Sábado")]
        Sabado = 64,
    }

    [PersistenceBaseDAO(typeof(RotaDAO))]
    [PersistenceClass("rota")]
    public class Rota : ModelBaseCadastro
    {
        #region Propriedades

        [PersistenceProperty("IDROTA", PersistenceParameterType.IdentityKey)]
        public int IdRota { get; set; }

        [Log("Código")]
        [PersistenceProperty("CODINTERNO")]
        public string CodInterno { get; set; }

        [Log("Descrição")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [PersistenceProperty("DISTANCIA")]
        public int Distancia { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("DIASSEMANA")]
        public DiasSemana DiasSemana { get; set; }

        [Log("Número Mínimo Dias Entrega")]
        [PersistenceProperty("NUMEROMINIMODIASENTREGA")]
        public int NumeroMinimoDiasEntrega { get; set; }

        [PersistenceProperty("EntregaBalcao")]
        public bool EntregaBalcao { get; set; }

        #endregion

        #region Propriedades de Suporte

        [Log("Distância")]
        public string DistanciaLabel
        {
            get { return Distancia + "km"; }
        }

        [Log("Situação")]
        public string SituacaoLabel
        {
            get { return Situacao == 1 ? "Ativa" : Situacao == 2 ? "Inativa" : "N/D"; }
        }

        [Log("Dias da Semana")]
        public string DescrDiaSemana
        {
            get
            {
                if (DiasSemana == DiasSemana.Nenhum)
                    return "";

                List<string> dias = new List<string>();

                if ((DiasSemana & DiasSemana.Domingo) == DiasSemana.Domingo)
                    dias.Add("Domingo");

                if ((DiasSemana & DiasSemana.Segunda) == DiasSemana.Segunda)
                    dias.Add("Segunda-feira");

                if ((DiasSemana & DiasSemana.Terca) == DiasSemana.Terca)
                    dias.Add("Terça-feira");

                if ((DiasSemana & DiasSemana.Quarta) == DiasSemana.Quarta)
                    dias.Add("Quarta-feira");

                if ((DiasSemana & DiasSemana.Quinta) == DiasSemana.Quinta)
                    dias.Add("Quinta-feira");

                if ((DiasSemana & DiasSemana.Sexta) == DiasSemana.Sexta)
                    dias.Add("Sexta-feira");

                if ((DiasSemana & DiasSemana.Sabado) == DiasSemana.Sabado)
                    dias.Add("Sábado");


                return string.Join(", ", dias.ToArray());
            }
        }

        #endregion
    }
}