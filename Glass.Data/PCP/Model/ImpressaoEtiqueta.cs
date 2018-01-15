using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ImpressaoEtiquetaDAO))]
	[PersistenceClass("impressao_etiqueta")]
	public class ImpressaoEtiqueta
    {
        #region Enumeradores

        public enum SituacaoImpressaoEtiqueta
        {
            Processando = 0,
            Ativa,
            Cancelada
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDIMPRESSAO", PersistenceParameterType.IdentityKey)]
        public uint IdImpressao { get; set; }

        [Log("Funcionário", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [Log("Loja", "NomeFantasia", typeof(LojaDAO))]
        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [Log("Data Impressão")]
        [PersistenceProperty("DATA")]
        public DateTime Data { get; set; }

        /// <summary>
        /// 0-Processando
        /// 1-Ativa
        /// 2-Cancelada
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMELOJA", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc { get; set; }

        [PersistenceProperty("TIPOIMPRESSAO", DirectionParameter.InputOptional)]
        public ProdutoImpressaoDAO.TipoEtiqueta TipoImpressao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case (int)SituacaoImpressaoEtiqueta.Processando: return "Processando";
                    case (int)SituacaoImpressaoEtiqueta.Ativa: return "Finalizada";
                    case (int)SituacaoImpressaoEtiqueta.Cancelada: return "Cancelada";
                    default: return "";
                }
            }
        }

        public string DescrTipoImpressao
        {
            get
            {
                switch (TipoImpressao)
                {
                    case ProdutoImpressaoDAO.TipoEtiqueta.Pedido: return "Pedido";
                    case ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal: return "NF-e";
                    case ProdutoImpressaoDAO.TipoEtiqueta.Retalho: return "Retalho";
                    case ProdutoImpressaoDAO.TipoEtiqueta.Box: return "Box";
                    default: return String.Empty;
                }
            }
        }

        #endregion
    }
}