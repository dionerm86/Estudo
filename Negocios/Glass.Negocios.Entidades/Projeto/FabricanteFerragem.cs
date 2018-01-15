using Colosoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Projeto.Negocios.Entidades
{
    public interface IValidadorFabricanteFerragem
    {
        /// <summary>
        /// Valida a atualização do Fabricante de Ferragem.
        /// </summary>
        Colosoft.Business.OperationResult ValidarAtualizacao(FabricanteFerragem fabricanteFerragem);

        /// <summary>
        /// Valida de o fabricante pode ser excluido, se não está sendo utilizado.
        /// </summary>
        Colosoft.Business.OperationResult ValidarExclusao(FabricanteFerragem fabricanteFerragem);
    }

    [Colosoft.Business.EntityLoader(typeof(FabricanteFerragemLoader))]
    public class FabricanteFerragem : Colosoft.Business.Entity<Glass.Data.Model.FabricanteFerragem>
    {
        #region Tipos Aninhados

        class FabricanteFerragemLoader : Colosoft.Business.EntityLoader<FabricanteFerragem, Glass.Data.Model.FabricanteFerragem>
        {
            public FabricanteFerragemLoader()
            {
                Configure()
                    .Uid(f => f.IdFabricanteFerragem)
                    .Description(f => f.Nome)
                    .FindName(f => f.Nome)
                    .Creator(f => new FabricanteFerragem(f));
            }
        }

        #endregion

        #region Propriedades

        public int IdFabricanteFerragem
        {
            get { return DataModel.IdFabricanteFerragem; }
            set
            {
                if (DataModel.IdFabricanteFerragem != value &&
                    RaisePropertyChanging("IdFabricanteFerragem", value))
                {
                    DataModel.IdFabricanteFerragem = value;
                    RaisePropertyChanged("IdFabricanteFerragem");
                }
            }
        }

        public string Nome
        {
            get { return DataModel.Nome; }
            set
            {
                if (DataModel.Nome != value &&
                    RaisePropertyChanging("Nome", value))
                {
                    DataModel.Nome = value;
                    RaisePropertyChanged("Nome");
                }
            }
        }

        public string Sitio
        {
            get { return DataModel.Sitio; }
            set
            {
                if (DataModel.Sitio != value &&
                    RaisePropertyChanging("Sitio", value))
                {
                    DataModel.Sitio = value;
                    RaisePropertyChanged("Sitio");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public FabricanteFerragem()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected FabricanteFerragem(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.FabricanteFerragem> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public FabricanteFerragem(Glass.Data.Model.FabricanteFerragem dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Salva os dados da instancia.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorFabricanteFerragem>();

            var resultadoValidacao = validador.ValidarAtualizacao(this);

            if (!resultadoValidacao)
                return new Colosoft.Business.SaveResult(false, resultadoValidacao.Message);

            return base.Save(session);
        }

        /// <summary>
        /// Apaga os dados da instancia.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorFabricanteFerragem>();

            var resultadoValidacao = validador.ValidarExclusao(this);

            if (!resultadoValidacao)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Message);


            return base.Delete(session);
        }

        #endregion
    }
}
