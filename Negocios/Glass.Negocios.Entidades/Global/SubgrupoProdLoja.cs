using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Global.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(SubgrupoProdLojaLoader))]
    public class SubgrupoProdLoja : Colosoft.Business.Entity<Data.Model.SubgrupoProdLoja>
    {
        #region Tipos Aninhados

        class SubgrupoProdLojaLoader : Colosoft.Business.EntityLoader<SubgrupoProdLoja, Data.Model.SubgrupoProdLoja>
        {
            public SubgrupoProdLojaLoader()
            {
                Configure()
                    .Keys(f => f.IdSubgrupoProd, f => f.IdLoja)
                    .Reference<Loja, Data.Model.Loja>("Loja", f => f.Loja, f => f.IdLoja, true, Colosoft.Business.LoadOptions.Lazy)
                    .Creator(f => new SubgrupoProdLoja(f));
            }
        }

        #endregion

        #region Construtores

        /// <summary>
		/// Construtor padrão.
		/// </summary>
		public SubgrupoProdLoja()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected SubgrupoProdLoja(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.SubgrupoProdLoja> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public SubgrupoProdLoja(Data.Model.SubgrupoProdLoja dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Instancia do fornecedor associado.
        /// </summary>
        [System.Diagnostics.DebuggerHidden]
        public Loja Loja
        {
            get { return GetReference<Loja>("Loja", true); }
        }

        /// <summary>
        /// Id do Subgrupo
        /// </summary>
        public int IdSubgrupoProd
        {
            get { return DataModel.IdSubgrupoProd; }
            set
            {
                if (DataModel.IdSubgrupoProd != value &&
                    RaisePropertyChanging("IdSubgrupoProd", value))
                {
                    DataModel.IdSubgrupoProd = value;
                    RaisePropertyChanged("IdSubgrupoProd");
                }
            }
        }

        /// <summary>
        /// Id da loja.
        /// </summary>
        public int IdLoja
        {
            get { return DataModel.IdLoja; }
            set
            {
                if (DataModel.IdLoja != value &&
                    RaisePropertyChanging("IdLoja", value))
                {
                    DataModel.IdLoja = value;
                    RaisePropertyChanged("IdLoja");
                }
            }
        }

        #endregion
    }
}
