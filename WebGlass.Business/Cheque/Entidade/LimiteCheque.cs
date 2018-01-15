using Glass;

namespace WebGlass.Business.Cheque.Entidade
{
    public class LimiteCheque
    {
        internal Glass.Data.Model.LimiteChequeCpfCnpj _limite;

        #region Construtores

        public LimiteCheque()
            : this(new Glass.Data.Model.LimiteChequeCpfCnpj())
        {
        }

        internal LimiteCheque(Glass.Data.Model.LimiteChequeCpfCnpj model)
        {
            _limite = model;
        }

        #endregion

        #region Propriedades

        public uint Codigo
        {
            get { return _limite.IdLimiteCheque; }
            set { _limite.IdLimiteCheque = value; }
        }

        public string CpfCnpj
        {
            get { return Formatacoes.FormataCpfCnpj(_limite.CpfCnpj); }
            set { _limite.CpfCnpj = value != null ? value.Replace(".", "").Replace("/", "").Replace("-", "") : null; }
        }

        public decimal Limite
        {
            get { return _limite.Limite; }
            set { _limite.Limite = value; }
        }

        private decimal? _valorUtilizado;

        public decimal ValorUtilizado
        {
            get
            {
                if (_valorUtilizado == null)
                    _valorUtilizado = Fluxo.LimiteCheque.Instance.ObtemValorUtilizado(_limite.CpfCnpj);

                return _valorUtilizado.GetValueOrDefault();
            }
        }

        public decimal ValorRestante
        {
            get { return Limite - ValorUtilizado; }
        }

        public string Observacao
        {
            get { return _limite.Observacao; }
            set { _limite.Observacao = value; }
        }

        #endregion
    }
}
