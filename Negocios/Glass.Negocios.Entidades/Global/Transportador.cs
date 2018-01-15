using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Entidades
{
    public interface IValidadorTransportador
    {
        /// <summary>
        /// Valida a existencia do transportador.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(Entidades.Transportador transportador);
    }

    /// <summary>
    /// Representa a entidade de negócio do transportador.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(TransportadorLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.Transportador)]
    public class Transportador : Colosoft.Business.Entity<Data.Model.Transportador>
    {
        #region Tipos Aninhados

        class TransportadorLoader : Colosoft.Business.EntityLoader<Transportador, Data.Model.Transportador>
        {
            public TransportadorLoader()
            {
                Configure()
                    .Uid(f => f.IdTransportador)
                    .FindName(f => f.Nome)
                    .Reference<Cidade, Data.Model.Cidade>("Cidade", f => f.Cidade, f => f.IdCidade)
                    .Creator(f => new Transportador(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Cidade associada.
        /// </summary>
        public Entidades.Cidade Cidade
        {
            get { return GetReference<Entidades.Cidade>("Cidade", true); }
        }

        /// <summary>
        /// Código do transportador.
        /// </summary>
        public int IdTransportador
        {
            get { return DataModel.IdTransportador; }
            set
            {
                if (DataModel.IdTransportador != value &&
                    RaisePropertyChanging("IdTransportador", value))
                {
                    DataModel.IdTransportador = value;
                    RaisePropertyChanged("IdTransportador");
                }
            }
        }

        /// <summary>
        /// Código da cidade do transportador.
        /// </summary>
        public int? IdCidade
        {
            get { return DataModel.IdCidade; }
            set
            {
                if (DataModel.IdCidade != value &&
                    RaisePropertyChanging("IdCidade", value))
                {
                    DataModel.IdCidade = value;
                    RaisePropertyChanged("IdCidade");
                }
            }
        }

        /// <summary>
        /// Nome do transportador.
        /// </summary>
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

        /// <summary>
        /// Nome fantasia do transportador.
        /// </summary>
        public string NomeFantasia
        {
            get { return DataModel.NomeFantasia; }
            set
            {
                if (DataModel.NomeFantasia != value &&
                    RaisePropertyChanging("NomeFantasia", value))
                {
                    DataModel.NomeFantasia = value;
                    RaisePropertyChanged("NomeFantasia");
                }
            }
        }

        /// <summary>
        /// Tipo de pessoa do transportador.
        /// </summary>
        public Data.Model.TipoPessoa? TipoPessoa
        {
            get
            { 
                return DataModel.TipoPessoa == 1 ? Data.Model.TipoPessoa.Fisica :
                    DataModel.TipoPessoa == 2 ? Data.Model.TipoPessoa.Juridica : 
                    (Data.Model.TipoPessoa?)null; 
            }
            set
            {
                var conv = value == Data.Model.TipoPessoa.Fisica ? 1 :
                    value == Data.Model.TipoPessoa.Juridica ? 2 : 0;

                if (DataModel.TipoPessoa != conv &&
                    RaisePropertyChanging("TipoPessoa", value))
                {
                    DataModel.TipoPessoa = conv;
                    RaisePropertyChanged("TipoPessoa");
                }
            }
        }

        /// <summary>
        /// CPF/CNPJ do transportador.
        /// </summary>
        public string CpfCnpj
        {
            get { return DataModel.CpfCnpj; }
            set
            {
                if (DataModel.CpfCnpj != value &&
                    RaisePropertyChanging("CpfCnpj", value))
                {
                    DataModel.CpfCnpj = value;
                    RaisePropertyChanged("CpfCnpj");
                }
            }
        }

        /// <summary>
        /// Inscrição Estadual do transportador.
        /// </summary>
        public string InscEst
        {
            get { return DataModel.InscEst; }
            set
            {
                if (DataModel.InscEst != value &&
                    RaisePropertyChanging("InscEst", value))
                {
                    DataModel.InscEst = value;
                    RaisePropertyChanged("InscEst");
                }
            }
        }

        /// <summary>
        /// Placa do veículo do transportador.
        /// </summary>
        public string Placa
        {
            get { return DataModel.Placa; }
            set
            {
                if (DataModel.Placa != value &&
                    RaisePropertyChanging("Placa", value))
                {
                    DataModel.Placa = value;
                    RaisePropertyChanged("Placa");
                }
            }
        }

        /// <summary>
        /// Logradouro no endereço do transportador.
        /// </summary>
        public string Endereco
        {
            get { return DataModel.Endereco; }
            set
            {
                if (DataModel.Endereco != value &&
                    RaisePropertyChanging("Endereco", value))
                {
                    DataModel.Endereco = value;
                    RaisePropertyChanged("Endereco");
                }
            }
        }

        /// <summary>
        /// Número no endereço do transportador.
        /// </summary>
        public string Numero
        {
            get { return DataModel.Numero; }
            set
            {
                if (DataModel.Numero != value &&
                    RaisePropertyChanging("Numero", value))
                {
                    DataModel.Numero = value;
                    RaisePropertyChanged("Numero");
                }
            }
        }

        /// <summary>
        /// Complemento do endereço do transportador.
        /// </summary>
        public string Complemento
        {
            get { return DataModel.Complemento; }
            set
            {
                if (DataModel.Complemento != value &&
                    RaisePropertyChanging("Complemento", value))
                {
                    DataModel.Complemento = value;
                    RaisePropertyChanged("Complemento");
                }
            }
        }

        /// <summary>
        /// Bairro no endereço do transportador.
        /// </summary>
        public string Bairro
        {
            get { return DataModel.Bairro; }
            set
            {
                if (DataModel.Bairro != value &&
                    RaisePropertyChanging("Bairro", value))
                {
                    DataModel.Bairro = value;
                    RaisePropertyChanged("Bairro");
                }
            }
        }

        /// <summary>
        /// CEP no endereço do transportador.
        /// </summary>
        public string Cep
        {
            get { return DataModel.Cep; }
            set
            {
                if (DataModel.Cep != value &&
                    RaisePropertyChanging("Cep", value))
                {
                    DataModel.Cep = value;
                    RaisePropertyChanged("Cep");
                }
            }
        }

        /// <summary>
        /// Telefone do transportador.
        /// </summary>
        public string Telefone
        {
            get { return DataModel.Telefone; }
            set
            {
                if (DataModel.Telefone != value &&
                    RaisePropertyChanging("Telefone", value))
                {
                    DataModel.Telefone = value;
                    RaisePropertyChanged("Telefone");
                }
            }
        }

        /// <summary>
        /// E-mail do transportador.
        /// </summary>
        public string Email
        {
            get { return DataModel.Email; }
            set
            {
                if (DataModel.Email != value &&
                    RaisePropertyChanging("Email", value))
                {
                    DataModel.Email = value;
                    RaisePropertyChanged("Email");
                }
            }
        }

        /// <summary>
        /// Nome do contato no transportador.
        /// </summary>
        public string Contato
        {
            get { return DataModel.Contato; }
            set
            {
                if (DataModel.Contato != value &&
                    RaisePropertyChanging("Contato", value))
                {
                    DataModel.Contato = value;
                    RaisePropertyChanged("Contato");
                }
            }
        }

        /// <summary>
        /// Telefone do contato no transportador.
        /// </summary>
        public string TelefoneContato
        {
            get { return DataModel.TelefoneContato; }
            set
            {
                if (DataModel.TelefoneContato != value &&
                    RaisePropertyChanging("TelefoneContato", value))
                {
                    DataModel.TelefoneContato = value;
                    RaisePropertyChanged("TelefoneContato");
                }
            }
        }

        /// <summary>
        /// Observações sobre o transportador.
        /// </summary>
        public string Obs
        {
            get { return DataModel.Obs; }
            set
            {
                if (DataModel.Obs != value &&
                    RaisePropertyChanging("Obs", value))
                {
                    DataModel.Obs = value;
                    RaisePropertyChanged("Obs");
                }
            }
        }

        /// <summary>
        /// Número de registro na SUFRAMA do transportador.
        /// </summary>
        public string Suframa
        {
            get { return DataModel.Suframa; }
            set
            {
                if (DataModel.Suframa != value &&
                    RaisePropertyChanging("Suframa", value))
                {
                    DataModel.Suframa = value;
                    RaisePropertyChanged("Suframa");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Transportador()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Transportador(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.Transportador> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Transportador(Data.Model.Transportador dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Sobrescritos

        /// <summary>
        /// Apaga o transportador
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorTransportador>();

            // Executa a validação
            var resultadoValidacao = validador.ValidaExistencia(this);
            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));
          
            return base.Delete(session);
        }

        #endregion
    }
}
