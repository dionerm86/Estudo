using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ContasReceberEncontroContasDAO))]
    [PersistenceClass("contas_receber_encontro_contas")]
    public class ContasReceberEncontroContas
    {
        #region Propiedades

        [PersistenceProperty("IDCONTAR", PersistenceParameterType.Key)]
        public uint IdContaR { get; set; }

        [PersistenceProperty("IDENCONTROCONTAS", PersistenceParameterType.Key)]
        public uint IdEncontroContas { get; set; }

        #endregion

        #region Propiedades Estendidas

        #endregion

        #region Propiedades de Suporte

        public ContasReceber _contasR;

        public ContasReceber ContasR
        {
            get
            {
                if(IdContaR > 0 && (_contasR == null || _contasR.IdContaR != IdContaR))
                    _contasR = ContasReceberDAO.Instance.GetElement(IdContaR);

                return _contasR;
            }
        }

        #region Rpt

        public string Referencia { get { return ContasR.Referencia; } }

        public string Parc { get { return ContasR.NumParcString; } }

        public DateTime Venc { get { return ContasR.DataVec; } }

        public decimal ValorVenc { get { return ContasR.ValorVec; } }

        #endregion

        #endregion
    }
}
