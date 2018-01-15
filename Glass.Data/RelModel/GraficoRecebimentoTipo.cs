using GDA;

namespace Glass.Data.RelModel
{
    public class GraficoRecebimentoTipo
    {
        #region Propriedades

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

        #endregion
    }
}