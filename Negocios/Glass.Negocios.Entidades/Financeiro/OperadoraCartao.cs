using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador de Operadora.
    /// </summary>
    public interface IValidadorOperadoraCartao
    {
        /// <summary>
        /// Valida a alteração da Operadora
        /// </summary>
        /// <param name="operadoraCartao"></param>
        /// <returns></returns>
        Colosoft.Business.OperationResult ValidarPodeAlterar(OperadoraCartao operadoraCartao);

        /// <summary>
        /// Valida a inserção da operadora
        /// </summary>
        /// <param name="operadoraCartao"></param>
        /// <returns></returns>
        Colosoft.Business.OperationResult ValidarPodeInserir(OperadoraCartao operadoraCartao);
    }

    [Colosoft.Business.EntityLoader(typeof(OperadoraCartaoLoader))]
    //[Glass.Negocios.ControleAlteracao(Data.Model.TabelaAlteracao.OperadoraCartao)]
    public class OperadoraCartao : Colosoft.Business.Entity<Glass.Data.Model.OperadoraCartao>
    {
        #region Tipos Aninhados

        class OperadoraCartaoLoader : Colosoft.Business.EntityLoader<OperadoraCartao, Glass.Data.Model.OperadoraCartao>
        {
            public OperadoraCartaoLoader()
            {
                Configure()
                    .Uid(f => (int)f.IdOperadoraCartao)
                    .Description(f => f.Descricao)
                    .Creator(f => new OperadoraCartao(f));
            }
        }

        #endregion

        #region Propriedades

        public uint IdOperadoraCartao
        {
            get { return DataModel.IdOperadoraCartao; }
            set
            {
                if (DataModel.IdOperadoraCartao != value &&
                    RaisePropertyChanging("IdOperadoraCartao", value))
                {
                    DataModel.IdOperadoraCartao = value;
                    RaisePropertyChanged("IdOperadoraCartao");
                }
            }
        }

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

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public OperadoraCartao()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected OperadoraCartao(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.OperadoraCartao> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public OperadoraCartao(Glass.Data.Model.OperadoraCartao dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
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
                .Current.GetInstance<IValidadorOperadoraCartao>();

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
                .Current.GetInstance<IValidadorOperadoraCartao>().ValidarPodeAlterar(this);
            if (!retornoValidacao)
                return new Colosoft.Business.DeleteResult(false, retornoValidacao.Message);

            return base.Delete(session);
        }

        #endregion
    }
}
