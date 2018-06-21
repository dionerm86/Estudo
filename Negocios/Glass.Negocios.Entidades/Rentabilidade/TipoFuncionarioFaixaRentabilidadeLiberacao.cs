using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade.Negocios.Entidades
{
    /// <summary>
    /// Representa a associada do tipo de funcionário com a faixa de rentabilidade para liberação.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(TipoFuncionarioFaixaRentabilidadeLiberacaoLoader))]
    public class TipoFuncionarioFaixaRentabilidadeLiberacao : Colosoft.Business.Entity<Data.Model.TipoFuncionarioFaixaRentabilidadeLiberacao>
    {
        #region Tipos Aninhados

        class TipoFuncionarioFaixaRentabilidadeLiberacaoLoader : Colosoft.Business.EntityLoader<TipoFuncionarioFaixaRentabilidadeLiberacao, Data.Model.TipoFuncionarioFaixaRentabilidadeLiberacao>
        {
            public TipoFuncionarioFaixaRentabilidadeLiberacaoLoader()
            {
                Configure()
                    .Keys(f => f.IdFaixaRentabilidadeLiberacao, f => f.IdTipoFuncionario)
                    .Creator(f => new TipoFuncionarioFaixaRentabilidadeLiberacao(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Obtém ou define o identificador da faixa de rentabilidade pai.
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
        /// Obtém ou define o identificador do tipo de funcionário associado.
        /// </summary>
        public int IdTipoFuncionario
        {
            get { return DataModel.IdTipoFuncionario; }
            set
            {
                if (DataModel.IdTipoFuncionario != value &&
                    RaisePropertyChanging(nameof(IdTipoFuncionario), value))
                {
                    DataModel.IdTipoFuncionario = value;
                    RaisePropertyChanged(nameof(IdTipoFuncionario));
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
		/// Construtor padrão.
		/// </summary>
		public TipoFuncionarioFaixaRentabilidadeLiberacao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected TipoFuncionarioFaixaRentabilidadeLiberacao(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.TipoFuncionarioFaixaRentabilidadeLiberacao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public TipoFuncionarioFaixaRentabilidadeLiberacao(Data.Model.TipoFuncionarioFaixaRentabilidadeLiberacao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
