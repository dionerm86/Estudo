using System;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(FluxoCaixaIFDDAO))]
    [PersistenceClass("fluxo_caixa_ifd")]
    public class FluxoCaixaIFD
    {
        #region Propriedades

        [PersistenceProperty("DescrGrupoConta")]
        public string DescrGrupoConta { get; set; }

        [PersistenceProperty("DescrPlanoConta")]
        public string DescrPlanoConta { get; set; }

        [PersistenceProperty("Tipo")]
        public long Tipo { get; set; }

        [PersistenceProperty("ValorAtual")]
        public decimal ValorAtual { get; set; }

        [PersistenceProperty("ValorSemana")]
        public decimal ValorSemana { get; set; }

        [PersistenceProperty("ValorProxSemana")]
        public decimal ValorProxSemana { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrTipo
        {
            get
            {
                switch (Tipo)
                {
                    case 1: return "Entradas";
                    case 2: return "Saídas";
                    default: return String.Empty;
                }
            }
        }

        public decimal ValorAtualEntrada
        {
            get { return Tipo == 1 ? ValorAtual : 0; }
        }

        public decimal ValorAtualSaida
        {
            get { return Tipo == 1 ? 0 : ValorAtual; }
        }

        public decimal ValorSemanaEntrada
        {
            get { return Tipo == 1 ? ValorSemana : 0; }
        }

        public decimal ValorSemanaSaida
        {
            get { return Tipo == 1 ? 0 : ValorSemana; }
        }

        public decimal ValorProxSemanaEntrada
        {
            get { return Tipo == 1 ? ValorProxSemana : 0; }
        }

        public decimal ValorProxSemanaSaida
        {
            get { return Tipo == 1 ? 0 : ValorProxSemana; }
        }

        #endregion
    }
}