using System;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(LucroAproximadoDAO))]
    [PersistenceClass("lucro_aproximado")]
    public class LucroAproximado
    {
        #region Propriedades

        [PersistenceProperty("GRUPO")]
        public long Grupo { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("CRITERIO")]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        private bool _isTotal = false;

        public bool IsTotal
        {
            get { return _isTotal; }
            set { _isTotal = value; }
        }

        public string DescrGrupo
        {
            get
            {
                switch (Grupo)
                {
                    case 1: return "Valor total Recebido";
                    case 2: return "Valor total Retirado";
                    default: return String.Empty;
                }
            }
        }

        #endregion
    }
}