using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CustoFixoDAO))]
	[PersistenceClass("custo_fixo")]
	public class CustoFixo : ModelBaseCadastro
	{
        #region Enumeradores
        
        public enum SituacaoEnum
        {
            Ativo = 1,
            Inativo
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDCUSTOFIXO", PersistenceParameterType.IdentityKey)]
        public uint IdCustoFixo { get; set; }

        [PersistenceProperty("IDFORNEC")]
        public uint IdFornec { get; set; }

        [PersistenceProperty("IDCONTA")]
        public uint IdConta { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        /// <summary>
        /// 1-Ativo
        /// 2-Inativo
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("DIAVENC")]
        public int DiaVenc { get; set; }

        [PersistenceProperty("VALORVENC")]
        public decimal ValorVenc { get; set; }

        [PersistenceProperty("DATAULTGERADO")]
        public DateTime? DataUltGerado { get; set; }

        [PersistenceProperty("DATAULTPAGTO")]
        public DateTime? DataUltPagto { get; set; }

        [PersistenceProperty("CONTABIL")]
        public bool Contabil { get; set; }

        [PersistenceProperty("PontoEquilibrio")]
        public bool PontoEquilibrio { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEFORNEC", DirectionParameter.InputOptional)]
        public string NomeFornec { get; set; }

        [PersistenceProperty("NOMELOJA", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        [PersistenceProperty("DESCRPLANOCONTA", DirectionParameter.InputOptional)]
        public string DescrPlanoConta { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("Obs", DirectionParameter.InputOptional)]
        public string Obs { get; set; }

        [PersistenceProperty("DataVenc", DirectionParameter.InputOptional)]
        public DateTime DataVenc { get; set; }

        [PersistenceProperty("ValorContaGerada", DirectionParameter.InputOptional)]
        public decimal ValorContaGerada { get; set; }

        [PersistenceProperty("IdContaPg", DirectionParameter.InputOptional)]
        public int IdContaPg { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrSituacao
        {
            get 
            {
                switch (Situacao)
                {
                    case 1:
                        return "Ativo";
                    case 2:
                        return "Inativo";
                    default:
                        return String.Empty;
                }
            }
        }

        public string IdNomeFornec
        {
            get { return IdFornec + " - " + NomeFornec; }
        }

        public bool EditDeleteVisible
        {
            get { return DataUltPagto == null; }
        }

        /// <summary>
        /// Verifica se o valor do centro de custo foi totalmente informado.
        /// </summary>
        public bool CentroCustoCompleto
        {
            get
            {
                return ValorContaGerada == CentroCustoAssociadoDAO.Instance.ObtemTotalPorContaPagar((int)IdContaPg);
            }
        }

        #endregion
    }
}