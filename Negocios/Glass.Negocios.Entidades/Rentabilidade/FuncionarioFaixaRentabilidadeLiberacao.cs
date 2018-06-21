using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft;

namespace Glass.Rentabilidade.Negocios.Entidades
{
    /// <summary>
    /// Representa a associação do funcionário com a faixa de rentabilidade.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(FuncionarioFaixaRentabilidadeLiberacaoLoader))]
    public class FuncionarioFaixaRentabilidadeLiberacao : Colosoft.Business.Entity<Data.Model.FuncionarioFaixaRentabilidadeLiberacao>
    {
        #region Tipos Aninhados

        class FuncionarioFaixaRentabilidadeLiberacaoLoader : Colosoft.Business.EntityLoader<FuncionarioFaixaRentabilidadeLiberacao, Data.Model.FuncionarioFaixaRentabilidadeLiberacao>
        {
            public FuncionarioFaixaRentabilidadeLiberacaoLoader()
            {
                Configure()
                    .Keys(f => f.IdFaixaRentabilidadeLiberacao, f => f.IdFunc)
                    .Creator(f => new FuncionarioFaixaRentabilidadeLiberacao(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da faixa de rentabilidade.
        /// </summary>
        public int IdFaixaRentabilidadeLiberacao
        {
            get { return DataModel.IdFaixaRentabilidadeLiberacao; }
            set
            {
                if (DataModel.IdFaixaRentabilidadeLiberacao != value &&
                    RaisePropertyChanging(nameof(IdFaixaRentabilidadeLiberacao), value))
                {
                    DataModel.IdFaixaRentabilidadeLiberacao = value;
                    RaisePropertyChanged(nameof(IdFaixaRentabilidadeLiberacao));
                }
            }
        }

        /// <summary>
        /// Identificador do funcionário associado.
        /// </summary>
        public int IdFunc
        {
            get { return DataModel.IdFunc; }
            set
            {
                if (DataModel.IdFunc != value &&
                    RaisePropertyChanging(nameof(IdFunc), value))
                {
                    DataModel.IdFunc = value;
                    RaisePropertyChanged(nameof(IdFunc));
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
		/// Construtor padrão.
		/// </summary>
		public FuncionarioFaixaRentabilidadeLiberacao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected FuncionarioFaixaRentabilidadeLiberacao(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.FuncionarioFaixaRentabilidadeLiberacao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public FuncionarioFaixaRentabilidadeLiberacao(Data.Model.FuncionarioFaixaRentabilidadeLiberacao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
