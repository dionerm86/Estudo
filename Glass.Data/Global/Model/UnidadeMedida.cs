using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(UnidadeMedidaDAO))]
    [PersistenceClass("unidade_medida")]
    public class UnidadeMedida : ModelBaseCadastro, Sync.Fiscal.EFD.Entidade.IUnidadeMedida
    {
        #region Propriedades

        [PersistenceProperty("IDUNIDADEMEDIDA", PersistenceParameterType.IdentityKey)]
        public int IdUnidadeMedida { get; set; }

        [PersistenceProperty("CODIGO")]
        public string Codigo { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        #endregion

        #region IUnidadeMedida Members

        int Sync.Fiscal.EFD.Entidade.IUnidadeMedida.Codigo
        {
            get { return IdUnidadeMedida; }
        }

        string Sync.Fiscal.EFD.Entidade.IUnidadeMedida.CodigoInterno
        {
            get { return Codigo; }
        }

        #endregion
    }
}