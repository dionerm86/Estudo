using System;
using Glass.Data.EFD;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class EfdCte
    {
        internal Glass.Data.Model.Cte.EfdCte _efdCte;
        private string _descrContaContabil;

        #region Construtores

        public EfdCte()
            : this(new Glass.Data.Model.Cte.EfdCte())
        {
        }

        internal EfdCte(Glass.Data.Model.Cte.EfdCte efdCte)
        {
            _efdCte = efdCte ?? new Glass.Data.Model.Cte.EfdCte();
        }

        #endregion

        public uint IdCte
        {
            get { return _efdCte.IdCte; }
            set { _efdCte.IdCte = value; }
        }

        public uint? IdContaContabil
        {
            get { return _efdCte.IdContaContabil; }
            set
            {
                _efdCte.IdContaContabil = value;
                _descrContaContabil = null;
            }
        }

        public string DescrContaContabil
        {
            get
            {
                if (_descrContaContabil == null && IdContaContabil > 0)
                    _descrContaContabil = Glass.Data.DAL.PlanoContaContabilDAO.Instance.ObtemDescricao(IdContaContabil.Value);

                return _descrContaContabil;
            }
        }

        public int? NaturezaBcCred
        {
            get { return _efdCte.NaturezaBcCred; }
            set { _efdCte.NaturezaBcCred = value; }
        }

        public string DescrNaturezaBcCred
        {
            get { return DataSourcesEFD.Instance.GetDescrNaturezaBcCredito(NaturezaBcCred); }
        }

        public int? IndNaturezaFrete
        {
            get { return _efdCte.IndNaturezaFrete; }
            set { _efdCte.IndNaturezaFrete = value; }
        }

        public string DescrIndNaturezaFrete
        {
            get { return DataSourcesEFD.Instance.GetDescrIndNaturezaFrete(IndNaturezaFrete); }
        }

        public int? CodCont
        {
            get { return _efdCte.CodCont; }
            set { _efdCte.CodCont = value; }
        }

        public string DescrCodCont
        {
            get { return DataSourcesEFD.Instance.GetDescrCodCont(CodCont); }
        }

        public int? CodCred
        {
            get { return _efdCte.CodCred; }
            set { _efdCte.CodCred = value; }
        }

        public string DescrCodCred
        {
            get { return DataSourcesEFD.Instance.GetDescrCodCred(CodCred); }
        }
    }
}
