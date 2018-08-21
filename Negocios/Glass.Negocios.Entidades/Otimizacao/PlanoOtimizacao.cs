using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do plano de otimização.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(PlanoOtimizacaoLoader))]
    public class PlanoOtimizacao : Colosoft.Business.Entity<Data.Model.PlanoOtimizacao>
    {
        #region Tipos Aninhados

        class PlanoOtimizacaoLoader : Colosoft.Business.EntityLoader<PlanoOtimizacao, Data.Model.PlanoOtimizacao>
        {
            public PlanoOtimizacaoLoader()
            {
                Configure()
                    .Uid(f => f.IdPlanoOtimizacao)
                    .Child<PlanoCorte, Data.Model.PlanoCorte>("PlanosCorte", f => f.PlanosCorte, f => f.IdPlanoOtimizacao)
                    .Reference<Global.Negocios.Entidades.Produto, Data.Model.Produto>("Produto", f => f.Produto, f => f.IdProduto)
                    .Creator(f => new PlanoOtimizacao(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Obtém os planos de corte.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<PlanoCorte> PlanosCorte { get; }

        /// <summary>
        /// Obtém o produto associado com o plano.
        /// </summary>
        public Global.Negocios.Entidades.Produto Produto
        {
            get
            {
                return GetReference<Global.Negocios.Entidades.Produto>("Produto", true);
            }
        }

        /// <summary>
        /// Obtém ou define o identificador do plano de otimização.
        /// </summary>
        public int IdPlanoOtimizacao
        {
            get { return DataModel.IdPlanoOtimizacao; }
            set
            {
                if (DataModel.IdPlanoOtimizacao != value &&
                    RaisePropertyChanging(nameof(IdPlanoOtimizacao), value))
                {
                    DataModel.IdPlanoOtimizacao = value;
                    RaisePropertyChanged(nameof(IdPlanoOtimizacao));
                }
            }
        }

        /// <summary>
        /// Obtém ou define o identificador da solução de otimização pai.
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
        /// Obtém ou define o nome.
        /// </summary>
        public string Nome
        {
            get { return DataModel.Nome; }
            set
            {
                if (DataModel.Nome != value &&
                    RaisePropertyChanging(nameof(Nome), value))
                {
                    DataModel.Nome = value;
                    RaisePropertyChanged(nameof(Nome));
                }
            }
        }

        /// <summary>
        /// Obtém ou define o identificador do produto associado.
        /// </summary>
        public int IdProduto
        {
            get { return DataModel.IdProduto; }
            set
            {
                if (DataModel.IdProduto != value &&
                    RaisePropertyChanging(nameof(IdProduto), value))
                {
                    DataModel.IdProduto = value;
                    RaisePropertyChanged(nameof(IdProduto));
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
		/// Construtor padrão.
		/// </summary>
		public PlanoOtimizacao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected PlanoOtimizacao(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.PlanoOtimizacao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            PlanosCorte = GetChild<PlanoCorte>(args.Children, "PlanosCorte");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public PlanoOtimizacao(Data.Model.PlanoOtimizacao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            PlanosCorte = CreateChild<Colosoft.Business.IEntityChildrenList<PlanoCorte>>("PlanosCorte");
        }

        #endregion
    }
}
