using System;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(EstoqueIFDDAO))]
    [PersistenceClass("estoque_ifd")]
    public class EstoqueIFD
    {
        #region Propriedades

        [PersistenceProperty("Tipo")]
        public long Tipo { get; set; }

        [PersistenceProperty("ValorMercadoria")]
        public decimal ValorMercadoria { get; set; }

        [PersistenceProperty("ValorConsumo")]
        public decimal ValorConsumo { get; set; }

        [PersistenceProperty("ValorDiversos")]
        public decimal ValorDiversos { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrTipo
        {
            get
            {
                switch (Tipo)
                {
                    case 0: return "Saldo anterior";
                    case 1: return "Entradas";
                    case 2: return "Saídas";
                    default: return String.Empty;
                }
            }
        }

        public decimal ValorTotal
        {
            get { return ValorMercadoria + ValorConsumo + ValorDiversos; }
        }

        public decimal ValorMercadoriaEntrada
        {
            get { return Tipo != 2 ? ValorMercadoria : 0; }
        }

        public decimal ValorMercadoriaSaida
        {
            get { return Tipo != 2 ? 0 : ValorMercadoria; }
        }

        public decimal ValorConsumoEntrada
        {
            get { return Tipo != 2 ? ValorConsumo : 0; }
        }

        public decimal ValorConsumoSaida
        {
            get { return Tipo != 2 ? 0 : ValorConsumo; }
        }

        public decimal ValorDiversosEntrada
        {
            get { return Tipo != 2 ? ValorDiversos : 0; }
        }

        public decimal ValorDiversosSaida
        {
            get { return Tipo != 2 ? 0 : ValorDiversos; }
        }

        public decimal ValorTotalEntrada
        {
            get { return Tipo != 2 ? ValorTotal : 0; }
        }

        public decimal ValorTotalSaida
        {
            get { return Tipo != 2 ? 0 : ValorTotal; }
        }

        #endregion
    }
}