using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PlanoContasDAO))]
	[PersistenceClass("plano_contas")]
	public class PlanoContas : Colosoft.Data.BaseModel
    {
        #region Enumeradores

        public enum SituacaoEnum
        {
            Ativo = 1,
            Inativo
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDCONTA", PersistenceParameterType.IdentityKey)]
        public int IdConta { get; set; }

        [PersistenceProperty("IDCONTAGRUPO")]
        public int IdContaGrupo { get; set; }

        [PersistenceProperty("IDGRUPO")]
        public int IdGrupo { get; set; }

        [PersistenceProperty("IdContaContabil")]
        public int? IdContaContabil { get; set; }

        /// <summary>
        /// 1-Ativo
        /// 2-Inativo
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public Glass.Situacao Situacao { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("EXIBIRDRE")]
        public bool ExibirDre { get; set; }

        [PersistenceProperty("Fixo")]
        public bool Fixo { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRCATEGORIA", DirectionParameter.InputOptional)]
        public string DescrCategoria { get; set; }

        [PersistenceProperty("DescrGrupo", DirectionParameter.InputOptional)]
        public string DescrGrupo { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string IdDescrGrupoConta
        {
            get { return IdGrupo + " - " + DescrGrupo; }
        }

        public string DescrPlanoGrupo
        {
            get 
            {
                return !ProjetoConfig.InverterExibicaoPlanoConta ? 
                    DescrGrupo + " - " + Descricao : Descricao + " - " + DescrGrupo;
            }
        }

        public bool DeleteVisible
        {
            get { return !(new List<string>(UtilsPlanoConta.GetGruposSistema.Split(',')).Contains(IdGrupo.ToString())); }
        }

        #endregion
	}
}