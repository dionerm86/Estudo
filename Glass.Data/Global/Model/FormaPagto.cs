using GDA;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FormaPagtoDAO))]
	[PersistenceClass("formapagto")]
	public class FormaPagto
    {
        #region Propriedades

        [PersistenceProperty("IDFORMAPAGTO", PersistenceParameterType.IdentityKey)]
        public uint? IdFormaPagto { get; set; }

		private string _descricao;

        [PersistenceProperty("DESCRICAO")]
        public string Descricao
        {
            get
            {
                if ((IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio || IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro) &&
                    (ApenasCheque || (!FinanceiroConfig.FormaPagamento.SepararTiposChequesRecebimento && !UtilizarPagamento)))
                    return "Cheque";
                else if (IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito && UtilizarPagamento)
                    return "Pagto. Bancário";
                else
                    return _descricao;
            }
            set { _descricao = value; }
        }
        
        [PersistenceProperty("APENASSISTEMA")]
        public bool ApenasSistema { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NAOUSAR", DirectionParameter.InputOptional)]
        public bool NaoUsar { get; set; }

        [PersistenceProperty("APENASCHEQUE", DirectionParameter.InputOptional)]
        public bool ApenasCheque { get; set; }

        [PersistenceProperty("UTILIZARPAGAMENTO", DirectionParameter.InputOptional)]
        public bool UtilizarPagamento { get; set; }

        #endregion

        #region Propriedades de Suporte

        public bool DeleteVisible
        {
            get { return IdFormaPagto > 8; }
        }

        #endregion
	}
}