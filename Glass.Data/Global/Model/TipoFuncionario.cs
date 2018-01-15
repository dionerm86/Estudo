using GDA;
using Glass.Data.Helper;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(TipoFuncDAO))]
	[PersistenceClass("tipo_func")]
    [Colosoft.Data.Schema.Cache]
	public class TipoFuncionario : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDTIPOFUNC", PersistenceParameterType.IdentityKey)]
        public int IdTipoFuncionario { get; set; }

        [PersistenceProperty("DESCRICAO")]
        [Colosoft.Data.Schema.CacheIndexed]
        public string Descricao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public bool DeleteVisible
        {
            get 
            {
                foreach (GenericModel g in DataSourcesEFD.Instance.GetFromEnum(typeof(Utils.TipoFuncionario), null, false))
                    if (g.Id == IdTipoFuncionario)
                        return false;

                return true;
            }
        }

        public int? IdSetorProducao { get; set; }

        public string TipoFuncComSetor
        {
            get { return IdTipoFuncionario + (IdSetorProducao> 0 ? "," + IdSetorProducao : ""); }
        }

        #endregion
	}
}