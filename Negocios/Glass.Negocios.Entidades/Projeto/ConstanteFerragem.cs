using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Projeto.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(ConstanteFerragemLoader))]
    public class ConstanteFerragem : Colosoft.Business.Entity<Glass.Data.Model.ConstanteFerragem>
    {
        #region Tipos Aninhados

        class ConstanteFerragemLoader : Colosoft.Business.EntityLoader<ConstanteFerragem, Glass.Data.Model.ConstanteFerragem>
        {
            public ConstanteFerragemLoader()
            {
                Configure()
                    .Uid(f => f.IdConstanteFerragem)
                    .Description(f => f.Nome)
                    .FindName(f => f.Nome)
                    .Creator(f => new ConstanteFerragem(f));
            }
        }

        #endregion

        #region Propriedades

        public int IdConstanteFerragem
        {
            get { return DataModel.IdConstanteFerragem; }
            set
            {
                if (DataModel.IdConstanteFerragem != value &&
                    RaisePropertyChanging("IdConstanteFerragem", value))
                {
                    DataModel.IdConstanteFerragem = value;
                    RaisePropertyChanged("IdConstanteFerragem");
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

        public string Nome
        {
            get { return DataModel.Nome; }
            set
            {
                if (DataModel.Nome != value &&
                    RaisePropertyChanging("Nome", value))
                {
                    DataModel.Nome = value;
                    RaisePropertyChanged("Nome");
                }
            }
        }

        public double Valor
        {
            get { return DataModel.Valor; }
            set
            {
                if (DataModel.Valor != value &&
                    RaisePropertyChanging("Valor", value))
                {
                    DataModel.Valor = value;
                    RaisePropertyChanged("Valor");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ConstanteFerragem()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ConstanteFerragem(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.ConstanteFerragem> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ConstanteFerragem(Glass.Data.Model.ConstanteFerragem dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
