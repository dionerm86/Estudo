using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Projeto.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(CodigoFerragemLoader))]
    public class CodigoFerragem : Colosoft.Business.Entity<Glass.Data.Model.CodigoFerragem>
    {
        #region Tipos Aninhados

        class CodigoFerragemLoader : Colosoft.Business.EntityLoader<CodigoFerragem, Glass.Data.Model.CodigoFerragem>
        {
            public CodigoFerragemLoader()
            {
                Configure()
                    .Uid(f => f.IdCodigoFerragem)
                    .Description(f => f.Codigo)
                    .FindName(f => f.Codigo)
                    .Creator(f => new CodigoFerragem(f));
            }
        }

        #endregion

        #region Propriedades

        public int IdCodigoFerragem
        {
            get { return DataModel.IdCodigoFerragem; }
            set
            {
                if (DataModel.IdCodigoFerragem != value &&
                    RaisePropertyChanging("IdCodigoFerragem", value))
                {
                    DataModel.IdCodigoFerragem = value;
                    RaisePropertyChanged("IdCodigoFerragem");
                }
            }
        }

        public int IdFerragem
        {
            get { return DataModel.IdFerragem; }
            set
            {
                if (DataModel.IdFerragem != value &&
                    RaisePropertyChanging("IdFerragem", value))
                {
                    DataModel.IdFerragem = value;
                    RaisePropertyChanged("IdFerragem");
                }
            }
        }

        public string Codigo
        {
            get { return DataModel.Codigo; }
            set
            {
                if (DataModel.Codigo != value &&
                    RaisePropertyChanging("Codigo", value))
                {
                    DataModel.Codigo = value;
                    RaisePropertyChanged("Codigo");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public CodigoFerragem()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected CodigoFerragem(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.CodigoFerragem> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public CodigoFerragem(Glass.Data.Model.CodigoFerragem dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
