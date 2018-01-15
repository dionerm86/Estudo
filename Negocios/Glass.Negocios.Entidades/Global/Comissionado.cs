using Colosoft;
using System.Linq;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador do comissionado.
    /// </summary>
    public interface IValidadorComissionado
    {
        /// <summary>
        /// Valida a existencia do comissionado.
        /// </summary>
        /// <param name="comissionado"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(Comissionado comissionado);
    }

    /// <summary>
    /// Representa a entidade de negócio do comissionado.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ComissionadoLoader))]
    public class Comissionado : Glass.Negocios.Entidades.EntidadeBaseCadastro<Data.Model.Comissionado>
    {
        #region Tipos aninhados

        class ComissionadoLoader : Colosoft.Business.EntityLoader<Comissionado, Data.Model.Comissionado>
        {
            public ComissionadoLoader()
            {
                Configure()
                    .Uid(f => f.IdComissionado)
                    .FindName(f => f.Nome)
                    .Creator(f => new Comissionado(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Código do comissionado.
        /// </summary>
        public int IdComissionado
        {
            get { return DataModel.IdComissionado; }
            set
            {
                if (DataModel.IdComissionado != value &&
                    RaisePropertyChanging("IdComissionado", value))
                {
                    DataModel.IdComissionado = value;
                    RaisePropertyChanged("IdComissionado");
                }
            }
        }

        /// <summary>
        /// Código da cidade do comissionado.
        /// </summary>
        public int IdCidade
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
        /// Situação do comissionado.
        /// </summary>
        public Situacao Situacao
        {
            get { return (Situacao)DataModel.Situacao; }
            set
            {
                if (DataModel.Situacao != (int)value &&
                    RaisePropertyChanging("Situacao", value))
                {
                    DataModel.Situacao = (int)value;
                    RaisePropertyChanged("Situacao");
                }
            }
        }

        /// <summary>
        /// Tipo de pessoa do comissionado.
        /// </summary>
        public Data.Model.TipoPessoa TipoPessoa
        {
            get { return (Data.Model.TipoPessoa)DataModel.TipoPessoa.FirstOrDefault(); }
            set
            {
                if ((DataModel.TipoPessoa == null || DataModel.TipoPessoa.FirstOrDefault() != (char)value) &&
                    RaisePropertyChanging("TipoPessoa", value))
                {
                    DataModel.TipoPessoa = ((char)value).ToString();
                    RaisePropertyChanged("TipoPessoa");
                }
            }
        }

        /// <summary>
        /// Nome do comissionado.
        /// </summary>
        public string Nome
        {
            get { return DataModel.Nome; }
            set
            {
                if (DataModel.Nome != value &&
                    RaisePropertyChanging("Nome", value))
                {
                    if (!string.IsNullOrEmpty(value))
                        value = value.Replace("'", "").Replace("\"", "");

                    DataModel.Nome = value;
                    RaisePropertyChanged("Nome");
                }
            }
        }

        /// <summary>
        /// Logradouro no endereço do comissionado.
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
        /// Número no endereço do comissionado.
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
        /// Complemento no endereço do comissionado.
        /// </summary>
        public string Compl
        {
            get { return DataModel.Compl; }
            set
            {
                if (DataModel.Compl != value &&
                    RaisePropertyChanging("Compl", value))
                {
                    DataModel.Compl = value;
                    RaisePropertyChanged("Compl");
                }
            }
        }

        /// <summary>
        /// Bairro no endereço do comissionado.
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
        /// Cidade no endereço do comissionado.
        /// </summary>
        public string Cidade
        {
            get { return DataModel.Cidade; }
            set
            {
                if (DataModel.Cidade != value &&
                    RaisePropertyChanging("Cidade", value))
                {
                    DataModel.Cidade = value;
                    RaisePropertyChanged("Cidade");
                }
            }
        }

        /// <summary>
        /// CEP no endereço do comissionado.
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
        /// CPF/CNPJ do comissionado.
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
        /// RG/Inscrição Estadual do comissionado.
        /// </summary>
        public string RgInscEst
        {
            get { return DataModel.RgInscEst; }
            set
            {
                if (DataModel.RgInscEst != value &&
                    RaisePropertyChanging("RgInscEst", value))
                {
                    DataModel.RgInscEst = value;
                    RaisePropertyChanged("RgInscEst");
                }
            }
        }

        /// <summary>
        /// Telefone residencial do comissionado.
        /// </summary>
        public string TelRes
        {
            get { return DataModel.TelRes; }
            set
            {
                if (DataModel.TelRes != value &&
                    RaisePropertyChanging("TelRes", value))
                {
                    DataModel.TelRes = value;
                    RaisePropertyChanged("TelRes");
                }
            }
        }

        /// <summary>
        /// Telefone de contato do comissionado.
        /// </summary>
        public string TelCont
        {
            get { return DataModel.TelCont; }
            set
            {
                if (DataModel.TelCont != value &&
                    RaisePropertyChanging("TelCont", value))
                {
                    DataModel.TelCont = value;
                    RaisePropertyChanged("TelCont");
                }
            }
        }

        /// <summary>
        /// Telefone celular do comissionado.
        /// </summary>
        public string TelCel
        {
            get { return DataModel.TelCel; }
            set
            {
                if (DataModel.TelCel != value &&
                    RaisePropertyChanging("TelCel", value))
                {
                    DataModel.TelCel = value;
                    RaisePropertyChanged("TelCel");
                }
            }
        }

        /// <summary>
        /// Fax do comissionado.
        /// </summary>
        public string Fax
        {
            get { return DataModel.Fax; }
            set
            {
                if (DataModel.Fax != value &&
                    RaisePropertyChanging("Fax", value))
                {
                    DataModel.Fax = value;
                    RaisePropertyChanged("Fax");
                }
            }
        }

        /// <summary>
        /// E-mail do comissionado.
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
        /// Observações sobre o comissionado.
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
        /// Percentual de comissão do comissionado.
        /// </summary>
        public float Percentual
        {
            get { return DataModel.Percentual; }
            set
            {
                if (DataModel.Percentual != value &&
                    RaisePropertyChanging("Percentual", value))
                {
                    DataModel.Percentual = value;
                    RaisePropertyChanged("Percentual");
                }
            }
        }

        /// <summary>
        /// Código/nome do banco do comissionado.
        /// </summary>
        public string Banco
        {
            get { return DataModel.Banco; }
            set
            {
                if (DataModel.Banco != value &&
                    RaisePropertyChanging("Banco", value))
                {
                    DataModel.Banco = value;
                    RaisePropertyChanged("Banco");
                }
            }
        }

        /// <summary>
        /// Número da agência bancária do comissionado.
        /// </summary>
        public string Agencia
        {
            get { return DataModel.Agencia; }
            set
            {
                if (DataModel.Agencia != value &&
                    RaisePropertyChanging("Agencia", value))
                {
                    DataModel.Agencia = value;
                    RaisePropertyChanged("Agencia");
                }
            }
        }

        /// <summary>
        /// Número da conta bancária do comissionado.
        /// </summary>
        public string Conta
        {
            get { return DataModel.Conta; }
            set
            {
                if (DataModel.Conta != value &&
                    RaisePropertyChanging("Conta", value))
                {
                    DataModel.Conta = value;
                    RaisePropertyChanged("Conta");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Comissionado()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Comissionado(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.Comissionado> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Comissionado(Data.Model.Comissionado dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Método acionado para apagar comissionado.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorComissionado>();

            var resultadoValidacao = validador.ValidaExistencia(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
