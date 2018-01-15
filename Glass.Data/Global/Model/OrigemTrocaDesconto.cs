using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(OrigemTrocaDescontoDAO))]
    [PersistenceClass("origem_troca_desconto")]
    public class OrigemTrocaDesconto : ModelBaseCadastro
    {
        #region Propiedades

        [Log("Identificador da Origem Troca/Desconto")]
        [PersistenceProperty("IDORIGEMTROCADESCONTO", PersistenceParameterType.IdentityKey)]
        public int IdOrigemTrocaDesconto { get; set; }

        [Log("Descrição")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [Log("Situação")]
        [PersistenceProperty("SITUACAO")]
        public Glass.Situacao Situacao { get; set; }

        #endregion

        #region Propiedades de Suporte

        public bool EditarRemoverVisivel
        {
            get
            {
                return ContasReceberDAO.Instance.ObtemValorCampo<int>("COUNT(*)", "IdOrigemDescontoAcrescimo=" + IdOrigemTrocaDesconto) == 0;
            }
        }

        #endregion

    }
}
