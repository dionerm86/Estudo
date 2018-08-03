using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio de uma solução de otimização.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(SolucaoOtimizacaoLoader))]
    public class SolucaoOtimizacao : Colosoft.Business.Entity<Data.Model.SolucaoOtimizacao>, ISolucaoOtimizacao
    {
        #region Tipos Aninhados

        class SolucaoOtimizacaoLoader : Colosoft.Business.EntityLoader<SolucaoOtimizacao, Data.Model.SolucaoOtimizacao>
        {
            public SolucaoOtimizacaoLoader()
            {
                Configure()
                    .Uid(f => f.IdSolucaoOtimizacao)
                    .Creator(f => new SolucaoOtimizacao(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da solução.
        /// </summary>
        public int IdSolucaoOtimizacao
        {
            get { return DataModel.IdSolucaoOtimizacao; }
            set
            {
                if (DataModel.IdSolucaoOtimizacao != value &&
                    RaisePropertyChanging(nameof(IdSolucaoOtimizacao), value))
                {
                    DataModel.IdSolucaoOtimizacao = value;
                    RaisePropertyChanged(nameof(IdSolucaoOtimizacao));
                }
            }
        }

        /// <summary>
        /// Identificador do arquivo de otimização.
        /// </summary>
        public int IdArquivoOtimizacao
        {
            get { return DataModel.IdArquivoOtimizacao; }
            set
            {
                if (DataModel.IdArquivoOtimizacao != value &&
                    RaisePropertyChanging(nameof(IdArquivoOtimizacao), value))
                {
                    DataModel.IdArquivoOtimizacao = value;
                    RaisePropertyChanged(nameof(IdArquivoOtimizacao));
                }
            }
        }

        /// <summary>
        /// Identificador único..
        /// </summary>
        new public Guid Uid
        {
            get { return DataModel.Uid; }
            set
            {
                if (DataModel.Uid != value &&
                    RaisePropertyChanging(nameof(Uid), value))
                {
                    DataModel.Uid = value;
                    RaisePropertyChanged(nameof(Uid));
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
		/// Construtor padrão.
		/// </summary>
		public SolucaoOtimizacao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected SolucaoOtimizacao(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.SolucaoOtimizacao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public SolucaoOtimizacao(Data.Model.SolucaoOtimizacao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
