using Colosoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador de bandeira.
    /// </summary>
    public interface IValidadorBandeiraCartao
    {
        /// <summary>
        /// Valida a alteração da bandeira
        /// </summary>
        /// <param name="bandeiraCartao"></param>
        /// <returns></returns>
        Colosoft.Business.OperationResult ValidarPodeAlterar(BandeiraCartao bandeiraCartao);

        /// <summary>
        /// Valida a inserção da bandeira
        /// </summary>
        /// <param name="bandeiraCartao"></param>
        /// <returns></returns>
        Colosoft.Business.OperationResult ValidarPodeInserir(BandeiraCartao bandeiraCartao);
    }

    [Colosoft.Business.EntityLoader(typeof(BandeiraCartaoLoader))]
    //[Glass.Negocios.ControleAlteracao(Data.Model.TabelaAlteracao.BandeiraCartao)]
    public class BandeiraCartao : Colosoft.Business.Entity<Glass.Data.Model.BandeiraCartao>
    {
        #region Tipos Aninhados

        class BandeiraCartaoLoader : Colosoft.Business.EntityLoader<BandeiraCartao, Glass.Data.Model.BandeiraCartao>
        {
            public BandeiraCartaoLoader()
            {
                Configure()
                    .Uid(f => (int)f.IdBandeiraCartao)
                    .Description(f => f.Descricao)
                    .Creator(f => new BandeiraCartao(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Código da Bandeira do Cartão
        /// </summary>
        public uint IdBandeiraCartao
        {
            get { return DataModel.IdBandeiraCartao; }
            set
            {
                if(DataModel.IdBandeiraCartao != value &&
                    RaisePropertyChanging("IdBandeiraCartao", value))
                {
                    DataModel.IdBandeiraCartao = value;
                    RaisePropertyChanged("IdBandeiraCartao");
                }
            }
        }

        /// <summary>
        /// Descrição da Bandeira do Cartão
        /// </summary>
        public string Descricao
        {
            get { return DataModel.Descricao; }
            set
            {
                if(DataModel.Descricao != value &&
                    RaisePropertyChanging("Descricao", value))
                {
                    DataModel.Descricao = value;
                    RaisePropertyChanged("Descricao");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public BandeiraCartao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected BandeiraCartao(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.BandeiraCartao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public BandeiraCartao(Glass.Data.Model.BandeiraCartao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Salva os dados da entidade.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorBandeiraCartao>();

            if (ExistsInStorage)
            {
                var retornoValidacao = validador.ValidarPodeAlterar(this);
                if (!retornoValidacao)
                    return new Colosoft.Business.SaveResult(false, retornoValidacao.Message);
            }
            else
            {
                var retornoValidacao = validador.ValidarPodeInserir(this);
                if (!retornoValidacao)
                    return new Colosoft.Business.SaveResult(false, retornoValidacao.Message);
            }

            return base.Save(session);
        }

        /// <summary>
        /// Apaga os dados da entidade.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var retornoValidacao = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorBandeiraCartao>().ValidarPodeAlterar(this);
            if (!retornoValidacao)
                return new Colosoft.Business.DeleteResult(false, retornoValidacao.Message);

            return base.Delete(session);
        }

        #endregion
    }
}
