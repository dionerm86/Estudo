using System.Linq;
using Colosoft;

namespace Glass.Fiscal.Negocios.Entidades.Cte
{
    /// <summary>
    /// Assinatura do validador de chave de acesso
    /// </summary>
    public interface IValidadorChaveAcesso
    {
        /// <summary>
        /// Valida a atualizacao dos dados da chave de acesso
        /// </summary>
        /// <param name="chaveAcessoCte"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaAtualizacao(Entidades.Cte.ChaveAcessoCte chaveAcessoCte);
    }

    /// <summary>
    /// Representa a entidade de negocio da chave de acesso do CT-e
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ChaveAcessoLoader))]
    public class ChaveAcessoCte : Colosoft.Business.Entity<Data.Model.Cte.ChaveAcessoCte>
    {
        #region Tipos Aninhados

        class ChaveAcessoLoader : Colosoft.Business.EntityLoader<ChaveAcessoCte, Data.Model.Cte.ChaveAcessoCte>
        {
            public ChaveAcessoLoader()
            {
                Configure()
                    .Uid(f => f.IdChaveAcessoCte)
                    .FindName(f => f.ChaveAcesso)
                    .Creator(f => new ChaveAcessoCte(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da chave de acesso.
        /// </summary>
        public int IdChaveAcessoCte
        {
            get { return DataModel.IdChaveAcessoCte; }
            set
            {
                if (DataModel.IdChaveAcessoCte != value &&
                    RaisePropertyChanging("IdChaveAcessoCte", value))
                {
                    DataModel.IdChaveAcessoCte = value;
                    RaisePropertyChanged("IdChaveAcessoCte");
                }
            }
        }

        /// <summary>
        /// Identificador do CT-e.
        /// </summary>
        public int IdCte
        {
            get { return DataModel.IdCte; }
            set
            {
                if (DataModel.IdCte != value &&
                    RaisePropertyChanging("IdCte", value))
                {
                    DataModel.IdCte = value;
                    RaisePropertyChanged("IdCte");
                }
            }
        }

        /// <summary>
        /// Chave de Acesso.
        /// </summary>
        public string ChaveAcesso
        {
            get { return DataModel.ChaveAcesso; }
            set
            {
                if (DataModel.ChaveAcesso != value &&
                    RaisePropertyChanging("ChaveAcesso", value))
                {
                    DataModel.ChaveAcesso = value;
                    RaisePropertyChanged("ChaveAcesso");
                }
            }
        }

        /// <summary>
        /// PIN.
        /// </summary>
        public string PIN
        {
            get { return DataModel.PIN; }
            set
            {
                if (DataModel.PIN != value &&
                    RaisePropertyChanging("PIN", value))
                {
                    DataModel.PIN = value;
                    RaisePropertyChanged("PIN");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ChaveAcessoCte()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ChaveAcessoCte(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.Cte.ChaveAcessoCte> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ChaveAcessoCte(Glass.Data.Model.Cte.ChaveAcessoCte dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Publicos 

        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            if (string.IsNullOrEmpty(ChaveAcesso))
                return new Colosoft.Business.SaveResult(false, "A chave de acesso não foi informada".GetFormatter());

            ChaveAcesso = Glass.Formatacoes.RetiraCaracteresEspeciais(ChaveAcesso).Replace(" ", "");

            if (ChaveAcesso.Length != 44)
                return new Colosoft.Business.SaveResult(false, "Chave de acesso inválida.".GetFormatter());

            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorChaveAcesso>();

            var resultadoValidacao = validador.ValidaAtualizacao(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.SaveResult(false, resultadoValidacao.Join(" "));

            return base.Save(session);
        }

        #endregion
    }
}
