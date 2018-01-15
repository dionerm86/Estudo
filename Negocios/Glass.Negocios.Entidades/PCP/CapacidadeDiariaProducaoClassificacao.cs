using System;

namespace Glass.PCP.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(CapacidadeDiariaProducaoClassificacaoLoader))]
    public class CapacidadeDiariaProducaoClassificacao : Colosoft.Business.Entity<Data.Model.CapacidadeProducaoDiariaClassificacao>
    {
        #region Tipos Aninhados

        class CapacidadeDiariaProducaoClassificacaoLoader : Colosoft.Business.EntityLoader<CapacidadeDiariaProducaoClassificacao, Data.Model.CapacidadeProducaoDiariaClassificacao>
        {
            public CapacidadeDiariaProducaoClassificacaoLoader()
            {
                Configure()
                    .Keys(f => f.Data, f => f.IdClassificacaoRoteiroProducao)
                    .Creator(f => new CapacidadeDiariaProducaoClassificacao(f));
            }
        }

        #endregion

        #region Contrutores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public CapacidadeDiariaProducaoClassificacao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected CapacidadeDiariaProducaoClassificacao(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.CapacidadeProducaoDiariaClassificacao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public CapacidadeDiariaProducaoClassificacao(Data.Model.CapacidadeProducaoDiariaClassificacao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Data.
        /// </summary>
        public DateTime Data
        {
            get { return DataModel.Data; }
            set
            {
                if (DataModel.Data != value &&
                    RaisePropertyChanging("Data", value))
                {
                    DataModel.Data = value;
                    RaisePropertyChanged("Data");
                }
            }
        }

        /// <summary>
        /// Identificador da Classificação.
        /// </summary>
        public int IdClassificacaoRoteiroProducao
        {
            get { return DataModel.IdClassificacaoRoteiroProducao; }
            set
            {
                if (DataModel.IdClassificacaoRoteiroProducao != value &&
                    RaisePropertyChanging("IdClassificacaoRoteiroProducao", value))
                {
                    DataModel.IdClassificacaoRoteiroProducao = value;
                    RaisePropertyChanged("IdClassificacaoRoteiroProducao");
                }
            }
        }

        /// <summary>
        /// Capacidade.
        /// </summary>
        public int Capacidade
        {
            get { return DataModel.Capacidade; }
            set
            {
                if (DataModel.Capacidade != value &&
                    RaisePropertyChanging("Capacidade", value))
                {
                    DataModel.Capacidade = value;
                    RaisePropertyChanged("Capacidade");
                }
            }
        }

        #endregion
    }
}
