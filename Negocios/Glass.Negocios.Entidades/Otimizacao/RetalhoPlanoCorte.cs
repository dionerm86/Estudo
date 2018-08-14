using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do retalho do plano de corte.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(RetalhoPlanoCorteLoader))]
    public class RetalhoPlanoCorte : Colosoft.Business.Entity<Data.Model.RetalhoPlanoCorte>
    {
        #region Tipos Aninhados

        class RetalhoPlanoCorteLoader : Colosoft.Business.EntityLoader<RetalhoPlanoCorte, Data.Model.RetalhoPlanoCorte>
        {
            public RetalhoPlanoCorteLoader()
            {
                Configure()
                    .Uid(f => f.IdRetalhoPlanoCorte)
                    .Creator(f => new RetalhoPlanoCorte(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Obtém ou define o identificador do retalho do plano de corte.
        /// </summary>
        public int IdRetalhoPlanoCorte
        {
            get { return DataModel.IdRetalhoPlanoCorte; }
            set
            {
                if (DataModel.IdRetalhoPlanoCorte != value &&
                    RaisePropertyChanging(nameof(IdRetalhoPlanoCorte), value))
                {
                    DataModel.IdRetalhoPlanoCorte = value;
                    RaisePropertyChanged(nameof(IdRetalhoPlanoCorte));
                }
            }
        }

        /// <summary>
        /// Obtém ou define o identificador do plano de corte.
        /// </summary>
        public int IdPlanoCorte
        {
            get { return DataModel.IdPlanoCorte; }
            set
            {
                if (DataModel.IdPlanoCorte != value &&
                    RaisePropertyChanging(nameof(IdPlanoCorte), value))
                {
                    DataModel.IdPlanoCorte = value;
                    RaisePropertyChanged(nameof(IdPlanoCorte));
                }
            }
        }

        /// <summary>
        /// Obtém ou define o identificador do retalho de produção associado.
        /// </summary>
        public int? IdRetalhoProducao
        {
            get { return DataModel.IdRetalhoProducao; }
            set
            {
                if (DataModel.IdRetalhoProducao != value &&
                    RaisePropertyChanging(nameof(IdRetalhoProducao), value))
                {
                    DataModel.IdRetalhoProducao = value;
                    RaisePropertyChanged(nameof(IdRetalhoProducao));
                }
            }
        }

        /// <summary>
        /// Obtém a posição.
        /// </summary>
        public int Posicao
        {
            get { return DataModel.Posicao; }
            set
            {
                if (DataModel.Posicao != value &&
                    RaisePropertyChanging(nameof(Posicao), value))
                {
                    DataModel.Posicao = value;
                    RaisePropertyChanged(nameof(Posicao));
                }
            }
        }

        /// <summary>
        /// Obtém ou define a largura do retalho.
        /// </summary>
        public double Largura
        {
            get { return DataModel.Largura; }
            set
            {
                if (DataModel.Largura != value &&
                    RaisePropertyChanging(nameof(Largura), value))
                {
                    DataModel.Largura = value;
                    RaisePropertyChanged(nameof(Largura));
                }
            }
        }

        /// <summary>
        /// Obtém ou define a altura do retalho.
        /// </summary>
        public double Altura
        {
            get { return DataModel.Altura; }
            set
            {
                if (DataModel.Altura != value &&
                    RaisePropertyChanging(nameof(Altura), value))
                {
                    DataModel.Altura = value;
                    RaisePropertyChanged(nameof(Altura));
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
		/// Construtor padrão.
		/// </summary>
		public RetalhoPlanoCorte()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected RetalhoPlanoCorte(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.RetalhoPlanoCorte> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public RetalhoPlanoCorte(Data.Model.RetalhoPlanoCorte dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
