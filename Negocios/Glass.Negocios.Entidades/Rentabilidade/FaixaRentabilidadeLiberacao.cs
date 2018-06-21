using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade.Negocios.Entidades
{
    /// <summary>
    /// Representa a faixa de rentabilidade para a liberação.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(FaixaRentabilidadeLiberacaoLoader))]
    public class FaixaRentabilidadeLiberacao : Colosoft.Business.Entity<Data.Model.FaixaRentabilidadeLiberacao>
    {
        #region Tipos Aninhados

        class FaixaRentabilidadeLiberacaoLoader : Colosoft.Business.EntityLoader<FaixaRentabilidadeLiberacao, Data.Model.FaixaRentabilidadeLiberacao>
        {
            public FaixaRentabilidadeLiberacaoLoader()
            {
                Configure()
                    .Uid(f => f.IdFaixaRentabilidadeLiberacao)
                    .Child<FuncionarioFaixaRentabilidadeLiberacao, Data.Model.FuncionarioFaixaRentabilidadeLiberacao>
                        ("Funcionarios", f => f.FuncionariosFaixa, f => f.IdFaixaRentabilidadeLiberacao)
                    .Child<TipoFuncionarioFaixaRentabilidadeLiberacao, Data.Model.TipoFuncionarioFaixaRentabilidadeLiberacao>
                        ("TiposFuncionario", f => f.TiposFuncionarioFaixa, f => f.IdFaixaRentabilidadeLiberacao)
                    .Link<Global.Negocios.Entidades.Funcionario, Data.Model.Funcionario, Data.Model.FuncionarioFaixaRentabilidadeLiberacao>
                        ("Funcionarios", "Funcionarios", f => f.Funcionarios, f => f.IdFunc, f => f.IdFunc)
                    .Link<Global.Negocios.Entidades.TipoFuncionario, Data.Model.TipoFuncionario, Data.Model.TipoFuncionarioFaixaRentabilidadeLiberacao>
                        ("TiposFuncionario", "TiposFuncionario", f => f.TiposFuncionario, f => f.IdTipoFuncionario, f => f.IdTipoFuncionario)
                    .Reference<Global.Negocios.Entidades.Loja, Data.Model.Loja>("Loja", f => f.Loja, f => f.IdLoja)
                    .Creator(f => new FaixaRentabilidadeLiberacao(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Associação dos funcionários da faixa.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<FuncionarioFaixaRentabilidadeLiberacao> FuncionariosFaixa { get; }

        /// <summary>
        /// Associação dos tipos de funcionários da faixa.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<TipoFuncionarioFaixaRentabilidadeLiberacao> TiposFuncionarioFaixa { get; }

        /// <summary>
        /// Funcionários associados.
        /// </summary>
        public Colosoft.Business.IEntityLinksList<Global.Negocios.Entidades.Funcionario> Funcionarios { get; }

        /// <summary>
        /// Tipos de funcionário associados.
        /// </summary>
        public Colosoft.Business.IEntityLinksList<Global.Negocios.Entidades.TipoFuncionario> TiposFuncionario { get; }

        /// <summary>
        /// Obtém a loja associada.
        /// </summary>
        public Global.Negocios.Entidades.Loja Loja => GetReference<Global.Negocios.Entidades.Loja>("Loja", true);

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
        /// Percentual da rentabilidade da faixa.
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
        /// Obtém ou define se é requerido liberação para a faixa de rentabilidade.
        /// </summary>
        public bool RequerLiberacao
        {
            get { return DataModel.RequerLiberacao; }
            set
            {
                if (DataModel.RequerLiberacao != value &&
                    RaisePropertyChanging(nameof(RequerLiberacao), value))
                {
                    DataModel.RequerLiberacao = value;
                    RaisePropertyChanged(nameof(RequerLiberacao));
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public FaixaRentabilidadeLiberacao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected FaixaRentabilidadeLiberacao(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.FaixaRentabilidadeLiberacao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            FuncionariosFaixa = GetChild<FuncionarioFaixaRentabilidadeLiberacao>(args.Children, "Funcionarios");
            TiposFuncionarioFaixa = GetChild<TipoFuncionarioFaixaRentabilidadeLiberacao>(args.Children, "TiposFuncionario");
            Funcionarios = GetLink<Global.Negocios.Entidades.Funcionario>(args.Links, "Funcionarios");
            TiposFuncionario = GetLink<Global.Negocios.Entidades.TipoFuncionario>(args.Links, "TiposFuncionario");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public FaixaRentabilidadeLiberacao(Data.Model.FaixaRentabilidadeLiberacao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            FuncionariosFaixa = CreateChild<Colosoft.Business.IEntityChildrenList<FuncionarioFaixaRentabilidadeLiberacao>>("Funcionarios");
            TiposFuncionarioFaixa = CreateChild<Colosoft.Business.IEntityChildrenList<TipoFuncionarioFaixaRentabilidadeLiberacao>>("TiposFuncionario");
            Funcionarios = CreateLink<Colosoft.Business.IEntityLinksList<Global.Negocios.Entidades.Funcionario>>("Funcionarios");
            TiposFuncionario = CreateLink<Colosoft.Business.IEntityLinksList<Global.Negocios.Entidades.TipoFuncionario>>("TiposFuncionario");
        }

        #endregion
    }
}
