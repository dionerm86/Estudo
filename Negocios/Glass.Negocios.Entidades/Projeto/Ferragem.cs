using Colosoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Projeto.Negocios.Entidades
{
    public interface IValidadorFerragem
    {
        IMessageFormattable[] ValidarAtualizacao(Ferragem ferragem);
    }

    /// <summary>
    /// Representa uma ferragem.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(FerragemLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.Ferragem)]
    public class Ferragem : Colosoft.Business.Entity<Glass.Data.Model.Ferragem>
    {
        #region Tipos Aninhados

        class FerragemLoader : Colosoft.Business.EntityLoader<Ferragem, Glass.Data.Model.Ferragem>
        {
            public FerragemLoader()
            {
                Configure()
                    .Uid(f => f.IdFerragem)
                    .Description(f => f.Nome)
                    .FindName(f => f.Nome)
                    .Child<CodigoFerragem, Data.Model.CodigoFerragem>("Codigos", f => f.Codigos, f => f.IdFerragem)
                    .Child<ConstanteFerragem, Data.Model.ConstanteFerragem>("Constantes", f => f.Constantes, f => f.IdFerragem)
                    .Reference<FabricanteFerragem, Data.Model.FabricanteFerragem>("Fabricante", f => f.Fabricante, f => f.IdFabricanteFerragem)
                    .Creator(f => new Ferragem(f));
            }
        }

        #endregion

        #region Variáveis Locais

        private Colosoft.Business.IEntityChildrenList<CodigoFerragem> _codigos;
        private Colosoft.Business.IEntityChildrenList<ConstanteFerragem> _constantes;

        #endregion

        #region Propriedades

        /// <summary>
        /// Código de Ferragens associadas.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<CodigoFerragem> Codigos
        {
            get { return _codigos; }
        }

        /// <summary>
        /// Constante de Ferragens associadas.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<ConstanteFerragem> Constantes
        {
            get { return _constantes; }
        }

        /// <summary>
        /// Fabricante associado.
        /// </summary>
        public FabricanteFerragem Fabricante
        {
            get { return GetReference<FabricanteFerragem>("Fabricante", false); }
        }

        /// <summary>
        /// Identificador da ferragem
        /// </summary>
        public int IdFerragem
        {
            get { return DataModel.IdFerragem; }
            set
            {
                if (DataModel.IdFerragem != value &&
                    RaisePropertyChanging("IdFerragem", value))
                {
                    DataModel.IdFerragem = value;
                    RaisePropertyChanged("IdFerragem");
                }
            }
        }

        /// <summary>
        /// Identificador do fabricante da ferragem
        /// </summary>
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

        /// <summary>
        /// Nome da Ferragem
        /// </summary>
        public string Nome
        {
            get { return DataModel.Nome; }
            set
            {
                if(DataModel.Nome != value &&
                    RaisePropertyChanging("Nome", value))
                {
                    DataModel.Nome = value;
                    RaisePropertyChanged("Nome");
                }
            }
        }

        /// <summary>
        /// Situacao da Ferragem
        /// </summary>
        public Situacao Situacao
        {
            get { return DataModel.Situacao; }
            set
            {
                if (DataModel.Situacao != value &&
                    RaisePropertyChanging("Situacao", value))
                {
                    DataModel.Situacao = value;
                    RaisePropertyChanged("Situacao");
                }
            }
        }

        /// <summary>
        /// Define se pode ser rotacionada
        /// </summary>
        public bool PodeRotacionar
        {
            get { return DataModel.PodeRotacionar; }
            set
            {
                if(DataModel.PodeRotacionar != value &&
                    RaisePropertyChanging("PodeRotacionar", value))
                {
                    DataModel.PodeRotacionar = value;
                    RaisePropertyChanged("PodeRotacionar");
                }
            }
        }

        /// <summary>
        /// Define se pode espelhar
        /// </summary>
        public bool PodeEspelhar
        {
            get { return DataModel.PodeEspelhar; }
            set
            {
                if (DataModel.PodeEspelhar != value &&
                    RaisePropertyChanging("PodeEspelhar", value))
                {
                    DataModel.PodeEspelhar = value;
                    RaisePropertyChanged("PodeEspelhar");
                }
            }
        }

        /// <summary>
        /// Define se utiliza medidas estaticas
        /// </summary>
        public bool MedidasEstaticas
        {
            get { return DataModel.MedidasEstaticas; }
            set
            {
                if (DataModel.MedidasEstaticas != value &&
                    RaisePropertyChanging("MedidasEstaticas", value))
                {
                    DataModel.MedidasEstaticas = value;
                    RaisePropertyChanged("MedidasEstaticas");
                }
            }
        }

        /// <summary>
        /// Define o estilo de ancoragem da ferragem
        /// </summary>
        public Data.Model.EstiloAncoragem EstiloAncoragem
        {
            get { return DataModel.EstiloAncoragem; }
            set
            {
                if (DataModel.EstiloAncoragem != value &&
                    RaisePropertyChanging("EstiloAncoragem", value))
                {
                    DataModel.EstiloAncoragem = value;
                    RaisePropertyChanged("EstiloAncoragem");
                }
            }
        }

        /// <summary>
        /// Altura da Ferragem
        /// </summary>
        public double Altura
        {
            get { return DataModel.Altura; }
            set
            {
                if (DataModel.Altura != value &&
                    RaisePropertyChanging("Altura", value))
                {
                    DataModel.Altura = value;
                    RaisePropertyChanged("Altura");
                }
            }
        }

        /// <summary>
        /// Largura da Ferragem
        /// </summary>
        public double Largura
        {
            get { return DataModel.Largura; }
            set
            {
                if (DataModel.Largura != value &&
                    RaisePropertyChanging("Largura", value))
                {
                    DataModel.Largura = value;
                    RaisePropertyChanged("Largura");
                }
            }
        }

        /// <summary>
        /// Data da Alteração
        /// </summary>
        public DateTime DataAlteracao
        {
            get { return DataModel.DataAlteracao; }
            set
            {
                if (DataModel.DataAlteracao != value &&
                    RaisePropertyChanging("DataAlteracao", value))
                {
                    DataModel.DataAlteracao = value;
                    RaisePropertyChanged("DataAlteracao");
                }
            }
        }

        /// <summary>
        /// Identificador único.
        /// </summary>
        public Guid UUID
        {
            get { return DataModel.UUID; }
            set
            {
                if (DataModel.UUID != value &&
                    RaisePropertyChanging("UUID", value))
                {
                    DataModel.UUID = value;
                    RaisePropertyChanged("UUID");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Ferragem()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Ferragem(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.Ferragem> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _codigos = GetChild<Projeto.Negocios.Entidades.CodigoFerragem>(args.Children, "Codigos");
            _constantes = GetChild<Projeto.Negocios.Entidades.ConstanteFerragem>(args.Children, "Constantes");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Ferragem(Glass.Data.Model.Ferragem dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _codigos = CreateChild<Colosoft.Business.IEntityChildrenList<Projeto.Negocios.Entidades.CodigoFerragem>>("Codigos");
            _constantes = CreateChild<Colosoft.Business.IEntityChildrenList<Projeto.Negocios.Entidades.ConstanteFerragem>>("Constantes");
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
                .Current.GetInstance<IValidadorFerragem>();

            var resultadoValidacao = validador.ValidarAtualizacao(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.SaveResult(false, resultadoValidacao.Join(" "));

            // Define a última data de alteração
            DataAlteracao = DateTime.Now;

            return base.Save(session);
        }

        #endregion
    }
}
