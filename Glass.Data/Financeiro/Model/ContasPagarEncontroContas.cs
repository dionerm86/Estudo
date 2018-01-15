using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ContasPagarEncontroContasDAO))]
    [PersistenceClass("contas_pagar_encontro_contas")]
    public class ContasPagarEncontroContas
    {
        #region Propiedades

        [PersistenceProperty("IDCONTAPG", PersistenceParameterType.Key)]
        public uint IdContaPg { get; set; }

        [PersistenceProperty("IDENCONTROCONTAS", PersistenceParameterType.Key)]
        public uint IdEncontroContas { get; set; }

        #endregion

        #region Propiedades Estendidas

        #endregion

        #region Propiedades de Suporte

        public ContasPagar _contasPg;

        public ContasPagar ContasPg
        {
            get
            {
                if (IdContaPg > 0 && (_contasPg == null || _contasPg.IdContaPg != IdContaPg))
                    _contasPg = ContasPagarDAO.Instance.GetElementByPrimaryKey(IdContaPg);

                return _contasPg;
            }
        }

        #region Rpt

        public string Referencia { get { return ContasPg.Referencia; } }

        public string Parc { get { return ContasPg.DescrNumParc; } }

        public DateTime Venc { get { return ContasPg.DataVenc; } }

        public decimal ValorVenc { get { return ContasPg.ValorVenc; } }

        #endregion

        #endregion
    }
}
