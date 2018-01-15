using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(GrupoContaDAO))]
	[PersistenceClass("grupo_conta")]
	public class GrupoConta : Colosoft.Data.BaseModel
    {
        #region Enumeradores

        public enum SituacaoEnum
        {
            Ativo = 1,
            Inativo
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDGRUPO", PersistenceParameterType.Key)]
        public int IdGrupo { get; set; }

        [PersistenceProperty("IDCATEGORIACONTA")]
        [PersistenceForeignKey(typeof(CategoriaConta), "IdCategoriaConta")]
        public int? IdCategoriaConta { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        /// <summary>
        /// 1 - Ativo
        /// 2 - Inativo
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public Glass.Situacao Situacao { get; set; }

        [PersistenceProperty("PONTOEQUILIBRIO")]
        public bool PontoEquilibrio { get; set; }

        [PersistenceProperty("NUMSEQ")]
        public int NumeroSequencia { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRCATEGORIA", DirectionParameter.InputOptional)]
        public string DescrCategoria { get; set; }

        #endregion

        #region Propriedades de Suporte

        public bool DeleteVisible
        {
            get
            {
                return !(new List<string>(UtilsPlanoConta.GetGruposSistema.Split(',')).Contains(IdGrupo.ToString()));
            }
        }

        #endregion
    }
}