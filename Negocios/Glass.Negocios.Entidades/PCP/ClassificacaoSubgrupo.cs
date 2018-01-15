using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.PCP.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio da associação de subgrupo à classificação de roteiro
    /// </summary>
    public class ClassificacaoSubgrupo :  Colosoft.Business.Entity<Glass.Data.Model.ClassificacaoSubgrupo>
    {
        #region Tipos Aninhados

        class ClassificacaoSubgrupoLoader : Colosoft.Business.EntityLoader<ClassificacaoSubgrupo, Glass.Data.Model.ClassificacaoSubgrupo>
        {
            public ClassificacaoSubgrupoLoader()
            {
                Configure()
                    .Keys(f => f.IdClassificacaoRoteiroProducao, f => f.IdSubgrupoProd)
                    .Creator(f => new ClassificacaoSubgrupo(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Id da classificação de roteiro.
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
        /// Id do subgrupo associado.
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

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ClassificacaoSubgrupo()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ClassificacaoSubgrupo(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.ClassificacaoSubgrupo> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ClassificacaoSubgrupo(Glass.Data.Model.ClassificacaoSubgrupo dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion  
    }
}
