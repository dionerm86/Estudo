using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio da peça do plano de corte.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(PecaPlanoCorteLoader))]
    public class PecaPlanoCorte : Colosoft.Business.Entity<Data.Model.PecaPlanoCorte>, IItemPlanoCorte
    {
        #region Tipos Aninhados

        class PecaPlanoCorteLoader : Colosoft.Business.EntityLoader<PecaPlanoCorte, Data.Model.PecaPlanoCorte>
        {
            public PecaPlanoCorteLoader()
            {
                Configure()
                    .Uid(f => f.IdPecaPlanoCorte)
                    .Creator(f => new PecaPlanoCorte(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Obtém ou define o identificador da peça do plano de corte.
        /// </summary>
        public int IdPecaPlanoCorte
        {
            get { return DataModel.IdPecaPlanoCorte; }
            set
            {
                if (DataModel.IdPecaPlanoCorte != value &&
                    RaisePropertyChanging(nameof(IdPecaPlanoCorte), value))
                {
                    DataModel.IdPecaPlanoCorte = value;
                    RaisePropertyChanged(nameof(IdPecaPlanoCorte));
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
        /// Obtém ou define o identificador do produto pedido produção associado.
        /// </summary>
        public int? IdProdPedProducao
        {
            get { return DataModel.IdProdPedProducao; }
            set
            {
                if (DataModel.IdProdPedProducao != value &&
                    RaisePropertyChanging(nameof(IdProdPedProducao), value))
                {
                    DataModel.IdProdPedProducao = value;
                    RaisePropertyChanged(nameof(IdProdPedProducao));
                }
            }
        }

        /// <summary>
        /// Obtém ou define a posição geral da peça.
        /// </summary>
        public int PosicaoGeral
        {
            get { return DataModel.PosicaoGeral; }
            set
            {
                if (DataModel.PosicaoGeral != value &&
                    RaisePropertyChanging(nameof(PosicaoGeral), value))
                {
                    DataModel.PosicaoGeral = value;
                    RaisePropertyChanged(nameof(PosicaoGeral));
                }
            }
        }

        /// <summary>
        /// Obtém ou define a posição da peça.
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
        /// Obtém ou define se a peça foi rotacionada.
        /// </summary>
        public bool Rotacionada
        {
            get { return DataModel.Rotacionada; }
            set
            {
                if (DataModel.Rotacionada != value &&
                    RaisePropertyChanging(nameof(Rotacionada), value))
                {
                    DataModel.Rotacionada = value;
                    RaisePropertyChanged(nameof(Rotacionada));
                }
            }
        }

        /// <summary>
        /// Obtém ou define a forma da peça.
        /// </summary>
        public string Forma
        {
            get { return DataModel.Forma; }
            set
            {
                if (DataModel.Forma != value &&
                    RaisePropertyChanging(nameof(Forma), value))
                {
                    DataModel.Forma = value;
                    RaisePropertyChanged(nameof(Forma));
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
		/// Construtor padrão.
		/// </summary>
		public PecaPlanoCorte()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected PecaPlanoCorte(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.PecaPlanoCorte> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public PecaPlanoCorte(Data.Model.PecaPlanoCorte dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
