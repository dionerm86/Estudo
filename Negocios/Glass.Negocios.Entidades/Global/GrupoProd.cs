using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador dos grupos de produto.
    /// </summary>
    public interface IValidadorGrupoProd
    {
        /// <summary>
        /// Valida a atualização dos dados do grupo.
        /// </summary>
        IMessageFormattable[] ValidaAtualizacao(GrupoProd grupoProd);

        /// <summary>
        /// Valida a existema o grupo de produtos.
        /// </summary>
        /// <param name="grupoProd"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(GrupoProd grupoProd);
    }

    /// <summary>
    /// Representa a entidade de negócio do grupo de produtos.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(GrupoProdLoader))]
    [Glass.Negocios.ControleAlteracao(
        Data.Model.LogAlteracao.TabelaAlteracao.GrupoProduto)]
    public class GrupoProd : Colosoft.Business.Entity<Data.Model.GrupoProd>
    {
        #region Tipos Aninhados

        class GrupoProdLoader : Colosoft.Business.EntityLoader<GrupoProd, Data.Model.GrupoProd>
        {
            public GrupoProdLoader()
            {
                Configure()
                    .Uid(f => f.IdGrupoProd)
                    .FindName(f => f.Descricao)
                    .Child<SubgrupoProd, Data.Model.SubgrupoProd>("Subgrupos", f => f.Subgrupos, f => f.IdGrupoProd)
                    .Creator(f => new GrupoProd(f));
            }
        }

        #endregion

        #region Variáveis Locais

        private Colosoft.Business.IEntityChildrenList<SubgrupoProd> _subgrupos;

        #endregion

        #region Propriedades

        /// <summary>
        /// Subgrupos associados.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<SubgrupoProd> Subgrupos
        {
            get { return _subgrupos; }
        }

        /// <summary>
        /// Identificador do grupo.
        /// </summary>
        public int IdGrupoProd
        {
            get { return DataModel.IdGrupoProd; }
            set
            {
                if (DataModel.IdGrupoProd != value &&
                    RaisePropertyChanging("IdGrupoProd", value))
                {
                    DataModel.IdGrupoProd = value;
                    RaisePropertyChanged("IdGrupoProd", "GrupoSistema");
                }
            }
        }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao
        {
            get { return DataModel.Descricao; }
            set
            {
                if (DataModel.Descricao != value &&
                    RaisePropertyChanging("Descricao", value))
                {
                    DataModel.Descricao = value;
                    RaisePropertyChanged("Descricao");
                }
            }
        }

        /// <summary>
        /// Tipo de Cálculo.
        /// </summary>
        public Data.Model.TipoCalculoGrupoProd? TipoCalculo
        {
            get { return DataModel.TipoCalculo; }
            set
            {
                if (DataModel.TipoCalculo != value &&
                    RaisePropertyChanging("TipoCalculo", value))
                {
                    DataModel.TipoCalculo = value;
                    RaisePropertyChanged("TipoCalculo");
                }
            }
        }

        /// <summary>
        /// Tipo de cálculo da nota fiscal.
        /// </summary>
        public Data.Model.TipoCalculoGrupoProd? TipoCalculoNf
        {
            get { return DataModel.TipoCalculoNf; }
            set
            {
                if (DataModel.TipoCalculoNf != value &&
                    RaisePropertyChanging("TipoCalculoNf", value))
                {
                    DataModel.TipoCalculoNf = value;
                    RaisePropertyChanged("TipoCalculoNf");
                }
            }
        }

        /// <summary>
        /// Identifica se é para bloquear o estoque.
        /// </summary>
        public bool BloquearEstoque
        {
            get { return DataModel.BloquearEstoque; }
            set
            {
                if (DataModel.BloquearEstoque != value &&
                    RaisePropertyChanging("BloquearEstoque", value))
                {
                    DataModel.BloquearEstoque = value;
                    RaisePropertyChanged("BloquearEstoque");
                }
            }
        }

        /// <summary>
        /// Identifica que não é para alterar o estoque.
        /// </summary>
        public bool NaoAlterarEstoque
        {
            get { return DataModel.NaoAlterarEstoque; }
            set
            {
                if (DataModel.NaoAlterarEstoque != value &&
                    RaisePropertyChanging("NaoAlterarEstoque", value))
                {
                    DataModel.NaoAlterarEstoque = value;
                    RaisePropertyChanged("NaoAlterarEstoque", "AlterarEstoque");
                }
            }
        }

        /// <summary>
        /// Identifica que é para alterar o estoque fiscal.
        /// </summary>
        public bool AlterarEstoque
        {
            get { return !NaoAlterarEstoque; }
            set { NaoAlterarEstoque = !value; }
        }

        /// <summary>
        /// Identifica que não para alterar o estoque fiscal.
        /// </summary>
        public bool NaoAlterarEstoqueFiscal
        {
            get { return DataModel.NaoAlterarEstoqueFiscal; }
            set
            {
                if (DataModel.NaoAlterarEstoqueFiscal != value &&
                    RaisePropertyChanging("NaoAlterarEstoqueFiscal", value))
                {
                    DataModel.NaoAlterarEstoqueFiscal = value;
                    RaisePropertyChanged("NaoAlterarEstoqueFiscal", "AlterarEstoqueFiscal");
                }
            }
        }

        /// <summary>
        /// Identifica que é para alterar o estoque fiscal.
        /// </summary>
        public bool AlterarEstoqueFiscal
        {
            get { return !NaoAlterarEstoqueFiscal; }
            set { NaoAlterarEstoqueFiscal = !value; }
        }

        /// <summary>
        /// Tipo de grupo.
        /// </summary>
        public Data.Model.TipoGrupoProd TipoGrupo
        {
            get { return DataModel.TipoGrupo; }
            set
            {
                if (DataModel.TipoGrupo != value &&
                    RaisePropertyChanging("TipoGrupo", value))
                {
                    DataModel.TipoGrupo = value;
                    RaisePropertyChanged("TipoGrupo");
                }
            }
        }

        /// <summary>
        /// Identifica se é para exibir a mensagem estoque.
        /// </summary>
        public bool ExibirMensagemEstoque
        {
            get { return DataModel.ExibirMensagemEstoque; }
            set
            {
                if (DataModel.ExibirMensagemEstoque != value &&
                    RaisePropertyChanging("ExibirMensagemEstoque", value))
                {
                    DataModel.ExibirMensagemEstoque = value;
                    RaisePropertyChanged("ExibirMensagemEstoque");
                }
            }
        }

        /// <summary>
        /// Identifica se gera volume.
        /// </summary>
        public bool GeraVolume
        {
            get { return DataModel.GeraVolume; }
            set
            {
                if (DataModel.GeraVolume != value &&
                    RaisePropertyChanging("GeraVolume", value))
                {
                    DataModel.GeraVolume = value;
                    RaisePropertyChanged("GeraVolume");
                }
            }
        }        

        /// <summary>
        /// Identifica se é um grupo do sistema.
        /// </summary>
        public bool GrupoSistema
        {
            get { return IdGrupoProd > 0 && IdGrupoProd <= 10; }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public GrupoProd()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected GrupoProd(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.GrupoProd> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _subgrupos = GetChild<SubgrupoProd>(args.Children, "Subgrupos");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public GrupoProd(Data.Model.GrupoProd dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _subgrupos = CreateChild<Colosoft.Business.IEntityChildrenList<SubgrupoProd>>("Subgrupos");
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera o tipo de calculo.
        /// </summary>
        /// <param name="tipoCalculoGrupo"></param>
        /// <param name="tipoCalculoNfGrupo"></param>
        /// <param name="tipoCalculoSubgrupo"></param>
        /// <param name="tipoCalculoNfSubgrupo"></param>
        /// <param name="notaFiscal"></param>
        /// <returns></returns>
        public static Data.Model.TipoCalculoGrupoProd ObtemTipoCalculo(
            Data.Model.TipoCalculoGrupoProd? tipoCalculoGrupo,
            Data.Model.TipoCalculoGrupoProd? tipoCalculoNfGrupo,
            Data.Model.TipoCalculoGrupoProd? tipoCalculoSubgrupo,
            Data.Model.TipoCalculoGrupoProd? tipoCalculoNfSubgrupo,
            bool notaFiscal)
        {
            Data.Model.TipoCalculoGrupoProd? tipoCalc = null;

            if ((tipoCalculoSubgrupo != null ||  tipoCalculoNfSubgrupo != null))
                tipoCalc = !notaFiscal ? tipoCalculoSubgrupo : tipoCalculoNfSubgrupo;

            if ((tipoCalc == null || tipoCalc <= 0) && 
                (tipoCalculoGrupo != null || tipoCalculoNfGrupo != null))
                tipoCalc = !notaFiscal ? tipoCalculoGrupo : tipoCalculoNfGrupo;

            if (notaFiscal && (tipoCalc == null || tipoCalc <= 0))
                tipoCalc = (tipoCalculoSubgrupo != null || tipoCalculoNfSubgrupo != null) &&
                            tipoCalculoSubgrupo.GetValueOrDefault() > 0 ? tipoCalculoSubgrupo : tipoCalculoGrupo;

            return tipoCalc > 0 ? tipoCalc.Value : Glass.Data.Model.TipoCalculoGrupoProd.Qtd;
        }

        /// <summary>
        /// Método acionado para apagar a entidade.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            if (GrupoSistema)
                return new Colosoft.Business.DeleteResult(false, "Não é possível apagar um grupo do sistema".GetFormatter());

            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorGrupoProd>();

            var resultadoValidacao = validador.ValidaExistencia(this);
            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        /// <summary>
        /// Método acionado para salvar a entidade.
        /// </summary>
        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorGrupoProd>();

            var resultadoValidacao = validador.ValidaAtualizacao(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.SaveResult(false, resultadoValidacao.Join(" "));

            return base.Save(session);
        }

        #endregion
    }
}
