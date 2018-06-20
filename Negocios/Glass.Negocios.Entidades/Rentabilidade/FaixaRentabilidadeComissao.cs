using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade.Negocios.Entidades
{
    /// <summary>
    /// Representa a relação da faixa do percentual da rentabilidade 
    /// com o percentual de redução de comissão aplicado.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(FaixaRentabilidadeComissaoLoader))]
    public class FaixaRentabilidadeComissao : Colosoft.Business.Entity<Data.Model.FaixaRentabilidadeComissao>
    {
        #region Tipos Aninhados

        class FaixaRentabilidadeComissaoLoader : Colosoft.Business.EntityLoader<FaixaRentabilidadeComissao, Data.Model.FaixaRentabilidadeComissao>
        {
            public FaixaRentabilidadeComissaoLoader()
            {
                Configure()
                    .Uid(f => f.IdFaixaRentabilidadeComissao)
                    .Creator(f => new FaixaRentabilidadeComissao(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da faixa.
        /// </summary>
        public int IdFaixaRentabilidadeComissao
        {
            get { return DataModel.IdFaixaRentabilidadeComissao; }
            set
            {
                if (DataModel.IdFaixaRentabilidadeComissao != value &&
                    RaisePropertyChanging(nameof(IdFaixaRentabilidadeComissao), value))
                {
                    DataModel.IdFaixaRentabilidadeComissao = value;
                    RaisePropertyChanged(nameof(IdFaixaRentabilidadeComissao));
                }
            }
        }


        /// <summary>
        /// Identificador da loja associada.
        /// </summary>
        public int IdLoja
        {
            get { return DataModel.IdLoja; }
            set
            {
                if (DataModel.IdLoja != value &&
                    RaisePropertyChanging(nameof(IdLoja), value))
                {
                    DataModel.IdLoja = value;
                    RaisePropertyChanged(nameof(IdLoja));
                }
            }
        }

        /// <summary>
        /// Identificador do funcionário associado.
        /// </summary>
        public int? IdFunc
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

        /// <summary>
        /// Faixa do percentual da rentabilidade.
        /// </summary>
        public decimal PercentualRentabilidade
        {
            get { return DataModel.PercentualRentabilidade; }
            set
            {
                if (DataModel.PercentualRentabilidade != value &&
                    RaisePropertyChanging(nameof(PercentualRentabilidade), value))
                {
                    DataModel.PercentualRentabilidade = value;
                    RaisePropertyChanged(nameof(PercentualRentabilidade));
                }
            }
        }

        /// <summary>
        /// Percentual da redução que será aplicada na comissão.
        /// </summary>
        public decimal PercentualComissao
        {
            get { return DataModel.PercentualComissao; }
            set
            {
                if (DataModel.PercentualComissao != value &&
                    RaisePropertyChanging(nameof(PercentualComissao), value))
                {
                    DataModel.PercentualComissao = value;
                    RaisePropertyChanged(nameof(PercentualComissao));
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
		/// Construtor padrão.
		/// </summary>
		public FaixaRentabilidadeComissao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected FaixaRentabilidadeComissao(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.FaixaRentabilidadeComissao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public FaixaRentabilidadeComissao(Data.Model.FaixaRentabilidadeComissao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
