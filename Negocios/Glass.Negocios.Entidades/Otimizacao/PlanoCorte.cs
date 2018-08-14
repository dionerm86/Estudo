using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do plano de corte.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(PlanoCorteLoader))]
    public class PlanoCorte :  Colosoft.Business.Entity<Data.Model.PlanoCorte>
    {
        #region Tipos Aninhados

        class PlanoCorteLoader : Colosoft.Business.EntityLoader<PlanoCorte, Data.Model.PlanoCorte>
        {
            public PlanoCorteLoader()
            {
                Configure()
                    .Uid(f => f.IdPlanoCorte)
                    .Child<PecaPlanoCorte, Data.Model.PecaPlanoCorte>("Pecas", f => f.Pecas, f => f.IdPlanoCorte)
                    .Child<RetalhoPlanoCorte, Data.Model.RetalhoPlanoCorte>("Retalhos", f => f.Retalhos, f => f.IdPlanoCorte)
                    .Creator(f => new PlanoCorte(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Obtém as peças.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<PecaPlanoCorte> Pecas { get; }

        /// <summary>
        /// Obtém os retalhoes do plano de corte.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<RetalhoPlanoCorte> Retalhos { get; }

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
        /// Obtém ou define a posição do plano de corte.
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
        /// Obtém ou define o identificador da chapa alocada.
        /// </summary>
        public string IdChapa
        {
            get { return DataModel.IdChapa; }
            set
            {
                if (DataModel.IdChapa != value &&
                    RaisePropertyChanging(nameof(IdChapa), value))
                {
                    DataModel.IdChapa = value;
                    RaisePropertyChanged(nameof(IdChapa));
                }
            }
        }

        /// <summary>
        /// Obtém ou define a largura.
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
        /// Obtém ou define a altura.
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
		public PlanoCorte()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected PlanoCorte(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.PlanoCorte> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            Pecas = GetChild<PecaPlanoCorte>(args.Children, "Pecas");
            Retalhos = GetChild<RetalhoPlanoCorte>(args.Children, "Retalhos");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public PlanoCorte(Data.Model.PlanoCorte dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            Pecas = CreateChild<Colosoft.Business.IEntityChildrenList<PecaPlanoCorte>>("Pecas");
            Retalhos = CreateChild<Colosoft.Business.IEntityChildrenList<RetalhoPlanoCorte>>("Retalhos");
        }

        #endregion
    }
}
