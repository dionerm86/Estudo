using System;
using System.Collections.Generic;
using System.Text;
using GDA;
using Glass.Data.Helper;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(RecebimentoTipoDAO))]
    [PersistenceClass("recebimento_tipo")]
    public class RecebimentoTipo
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

        public string DescricaoGrafico
        {
            get { return Formatacoes.TrataStringDocFiscal(Descricao); }
        }

        private bool _isTotal = false;

        public bool IsTotal
        {
            get { return _isTotal; }
            set { _isTotal = value; }
        }

        public string DescricaoGrafico1
        {
            get { return Descricao + ", " + String.Format("{0:C}", Valor); }
        }

        #endregion
    }
}