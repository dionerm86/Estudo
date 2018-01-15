using System;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ListaPassivosDAO))]
    [PersistenceClass("lista_passivos")]
    public class ListaPassivos
    {
        #region Propriedades

        [PersistenceProperty("IDCONTA")]
        public uint IdConta { get; set; }

        [PersistenceProperty("VALORPAGAR")]
        public decimal ValorPagar { get; set; }

        [PersistenceProperty("VALORPAGO")]
        public decimal ValorPago { get; set; }

        [PersistenceProperty("DATAMOV")]
        public DateTime? DataMov { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRPLANOCONTA")]
        public string DescrPlanoConta { get; set; }

        [PersistenceProperty("CRITERIO")]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string TipoMov
        {
            get 
            {
                return ValorPagar > 0 && ValorPago == 0 ? "Pagar" :
                    ValorPago > 0 && ValorPagar == 0 ? "Pago" : String.Empty;
            }
        }

        public decimal Valor
        {
            get
            {
                return ValorPagar > 0 && ValorPago == 0 ? ValorPagar :
                    ValorPago > 0 && ValorPagar == 0 ? ValorPago : 0;
            }
        }

        public decimal Total
        {
            get { return ValorPagar + ValorPago; }
        }

        #endregion
    }
}