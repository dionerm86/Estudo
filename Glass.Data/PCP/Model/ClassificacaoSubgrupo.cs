using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceClass("classificacao_subgrupo")]
    [PersistenceBaseDAO(typeof(ClassificacaoSubgrupoDAO))]
    public class ClassificacaoSubgrupo : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDCLASSIFICACAOROTEIROPRODUCAO", PersistenceParameterType.Key)]
        public int IdClassificacaoRoteiroProducao { get; set; }

        [PersistenceProperty("IDSUBGRUPOPROD", PersistenceParameterType.Key)]
        public int IdSubgrupoProd { get; set; }

        #endregion

        #region Propriedades Estendidas

        /// <summary>
        /// Descrição do subgrupo
        /// </summary>
        [PersistenceProperty("DESCRICAO", DirectionParameter.InputOptional)]
        public string Descricao { get; set; }

        #endregion

        /// <summary>
        /// chave composta concatenada
        /// </summary>
        public int ChaveComposta
        {
            get { return (IdClassificacaoRoteiroProducao + IdSubgrupoProd.ToString().PadLeft(3, '0')).StrParaInt(); }
        }
    }
}
