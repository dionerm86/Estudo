using System;
using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador do fornecedor.
    /// </summary>
    public interface IValidadorFornecedor
    {
        /// <summary>
        /// Valida a existencia do fornecedor.
        /// </summary>
        /// <param name="fornecedor"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(Fornecedor fornecedor);
    }

    /// <summary>
    /// Representa a entidade de negócio do fornecedor.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(FornecedorLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.Fornecedor)]
    public class Fornecedor : Colosoft.Business.Entity<Data.Model.Fornecedor>
    {
        #region Tipos Aninhados

        class FornecedorLoader : Colosoft.Business.EntityLoader<Fornecedor, Data.Model.Fornecedor>
        {
            public FornecedorLoader()
            {
                Configure()
                    .Uid(f => f.IdFornec)
                    .FindName(new FornecedorFindNameConverter(), f => f.Nomefantasia, f => f.Razaosocial)
                    .Child<ProdutoFornecedor, Data.Model.ProdutoFornecedor>("Produtos", f => f.ProdutosFornecedor, f => f.IdFornec)
                    .Child<Financeiro.Negocios.Entidades.ParcelasNaoUsar, Data.Model.ParcelasNaoUsar>("Parcelas", f => f.Parcelas, f => f.IdFornecedor)
                    .Reference<Cidade, Data.Model.Cidade>("Cidade", f => f.Cidade, f => f.IdCidade)
                    .Reference<Fiscal.Negocios.Entidades.PlanoContaContabil, Data.Model.PlanoContaContabil>("PlanoContaContabil",
                        f => f.PlanoContaContabil, f => f.IdContaContabil)
                    .Creator(f => new Fornecedor(f));
            }

            /// <summary>
            /// Conversor do nome do fornecedor.
            /// </summary>
            class FornecedorFindNameConverter : Colosoft.IFindNameConverter
            {
                /// <summary>
                /// Converte os valores para o nome do fornecedor.
                /// </summary>
                /// <param name="baseInfo"></param>
                /// <returns></returns>
                public string Convert(object[] baseInfo)
                {
                    var nomeFantasia = (string)baseInfo[0];
                    var razaoSocial = (string)baseInfo[1];

                    return 
                        !string.IsNullOrEmpty(nomeFantasia) ? nomeFantasia :
                        !string.IsNullOrEmpty(razaoSocial) ? razaoSocial : "";
                }
            }
        }

        #endregion

        #region Variáveis Locais

        private Colosoft.Business.IEntityChildrenList<ProdutoFornecedor> _produtosFornecedor;
        private Colosoft.Business.IEntityChildrenList<Financeiro.Negocios.Entidades.ParcelasNaoUsar> _parcelas;

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do fornecedor.
        /// </summary>
        public int IdFornec
        {
            get { return DataModel.IdFornec; }
            set
            {
                if (DataModel.IdFornec != value &&
                    RaisePropertyChanging("IdFornec", value))
                {
                    DataModel.IdFornec = value;
                    RaisePropertyChanged("IdFornec");
                }
            }
        }

        /// <summary>
        /// Identificador da cidade.
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
        /// Identificador do plano de contas.
        /// </summary>
        public int? IdConta
        {
            get { return DataModel.IdConta; }
            set
            {
                if (DataModel.IdConta != value &&
                    RaisePropertyChanging("IdConta", value))
                {
                    DataModel.IdConta = value;
                    RaisePropertyChanged("IdConta");
                }
            }
        }

        /// <summary>
        /// Código do plano de conta contábil do fornecedor.
        /// </summary>
        public int? IdContaContabil
        {
            get { return DataModel.IdContaContabil; }
            set
            {
                if (DataModel.IdContaContabil != value &&
                    RaisePropertyChanging("IdContaContabil", value))
                {
                    DataModel.IdContaContabil = value;
                    RaisePropertyChanged("IdContaContabil");
                }
            }
        }

        /// <summary>
        /// Identificador do país..
        /// </summary>
        public int IdPais
        {
            get { return DataModel.IdPais; }
            set
            {
                if (DataModel.IdPais != value &&
                    RaisePropertyChanging("IdPais", value))
                {
                    DataModel.IdPais = value;
                    RaisePropertyChanged("IdPais");
                }
            }
        }

        /// <summary>
        /// Tipo de pessoa.
        /// </summary>
        public string TipoPessoa
        {
            get { return DataModel.TipoPessoa; }
            set
            {
                if (DataModel.TipoPessoa != value &&
                    RaisePropertyChanging("TipoPessoa", value))
                {
                    DataModel.TipoPessoa = value;
                    RaisePropertyChanged("TipoPessoa");
                }
            }
        }

        /// <summary>
        /// Nome fantasia..
        /// </summary>
        public string Nomefantasia
        {
            get { return DataModel.Nomefantasia; }
            set
            {
                if (DataModel.Nomefantasia != value &&
                    RaisePropertyChanging("Nomefantasia", value))
                {
                    // Não permite que o nome do fornecedor possua ' ou "
                    if (!string.IsNullOrEmpty(value))
                        value = value.Replace("'", "").Replace("\"", "");

                    DataModel.Nomefantasia = value;
                    RaisePropertyChanged("Nomefantasia", "Nome");
                }
            }
        }

        /// <summary>
        /// Razão Social.
        /// </summary>
        public string Razaosocial
        {
            get { return DataModel.Razaosocial; }
            set
            {
                if (DataModel.Razaosocial != value &&
                    RaisePropertyChanging("Razaosocial", value))
                {
                    // Não permite que o nome do fornecedor possua ' ou "
                    if (!string.IsNullOrEmpty(value))
                        value = value.Replace("'", "").Replace("\"", "");

                    DataModel.Razaosocial = value;
                    RaisePropertyChanged("Razaosocial", "Nome");
                }
            }
        }

        /// <summary>
        /// Nome do fornecedor.
        /// </summary>
        public string Nome
        {
            get
            {
                return !string.IsNullOrEmpty(Nomefantasia) ? Nomefantasia :
                       !string.IsNullOrEmpty(Razaosocial) ? Razaosocial : "";
            }
        }

        /// <summary>
        /// CPF/CNPJ.
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
        /// Situação do fornecedor.
        /// </summary>
        public Data.Model.SituacaoFornecedor Situacao
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
        /// RG/Inscrição Estadual.
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
        /// Identifica se o fornecedor é um produtor rural.
        /// </summary>
        public bool ProdutorRural
        {
            get { return DataModel.ProdutorRural; }
            set
            {
                if (DataModel.ProdutorRural != value &&
                    RaisePropertyChanging("ProdutorRural", value))
                {
                    DataModel.ProdutorRural = value;
                    RaisePropertyChanged("ProdutorRural");
                }
            }
        }

        /// <summary>
        /// Suframa.
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

        /// <summary>
        /// CRT.
        /// </summary>
        public Data.Model.RegimeFornecedor Crt
        {
            get { return DataModel.Crt; }
            set
            {
                if (DataModel.Crt != value &&
                    RaisePropertyChanging("Crt", value))
                {
                    DataModel.Crt = value;
                    RaisePropertyChanged("Crt");
                }
            }
        }

        /// <summary>
        /// Endereço.
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
        /// Número.
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
        /// Complemento do endereço.
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
        /// Bairro.
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
        /// CEP.
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
        /// Email.
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
        /// Telefone de contato.
        /// </summary>
        public string Telcont
        {
            get { return DataModel.Telcont; }
            set
            {
                if (DataModel.Telcont != value &&
                    RaisePropertyChanging("Telcont", value))
                {
                    DataModel.Telcont = value;
                    RaisePropertyChanged("Telcont");
                }
            }
        }

        /// <summary>
        /// Fax.
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
        /// Data da última compra.
        /// </summary>
        public DateTime? Dtultcompra
        {
            get { return DataModel.Dtultcompra; }
            set
            {
                if (DataModel.Dtultcompra != value &&
                    RaisePropertyChanging("Dtultcompra", value))
                {
                    DataModel.Dtultcompra = value;
                    RaisePropertyChanged("Dtultcompra");
                }
            }
        }

        /// <summary>
        /// Data da vigência da tabela de preço.
        /// </summary>
        public DateTime? DataVigenciaPrecos
        {
            get { return DataModel.DataVigenciaPrecos; }
            set
            {
                if (DataModel.DataVigenciaPrecos != value &&
                    RaisePropertyChanging("DataVigenciaPrecos", value))
                {
                    DataModel.DataVigenciaPrecos = value;
                    RaisePropertyChanged("DataVigenciaPrecos");
                }
            }
        }

        /// <summary>
        /// Crédito.
        /// </summary>
        public decimal Credito
        {
            get { return DataModel.Credito; }
            set
            {
                if (DataModel.Credito != value &&
                    RaisePropertyChanging("Credito", value))
                {
                    DataModel.Credito = value;
                    RaisePropertyChanged("Credito");
                }
            }
        }

        /// <summary>
        /// Nome do vendedor.
        /// </summary>
        public string Vendedor
        {
            get { return DataModel.Vendedor; }
            set
            {
                if (DataModel.Vendedor != value &&
                    RaisePropertyChanging("Vendedor", value))
                {
                    DataModel.Vendedor = value;
                    RaisePropertyChanged("Vendedor");
                }
            }
        }

        /// <summary>
        /// Número do celular do vendedor.
        /// </summary>
        public string Telcelvend
        {
            get { return DataModel.Telcelvend; }
            set
            {
                if (DataModel.Telcelvend != value &&
                    RaisePropertyChanging("Telcelvend", value))
                {
                    DataModel.Telcelvend = value;
                    RaisePropertyChanged("Telcelvend");
                }
            }
        }

        /// <summary>
        /// Observação.
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
        /// Tipo de pagamento.
        /// </summary>
        public int? TipoPagto
        {
            get { return DataModel.TipoPagto; }
            set
            {
                if (DataModel.TipoPagto != value &&
                    RaisePropertyChanging("TipoPagto", value))
                {
                    DataModel.TipoPagto = value;
                    RaisePropertyChanged("TipoPagto");
                }
            }
        }

        /// <summary>
        /// Endereço do web service do cliente.
        /// </summary>
        public string UrlSistema
        {
            get { return DataModel.UrlSistema; }
            set
            {
                if (DataModel.UrlSistema != value &&
                    RaisePropertyChanging("UrlSistema", value))
                {
                    DataModel.UrlSistema = value;
                    RaisePropertyChanged("UrlSistema");
                }
            }
        }

        /// <summary>
        /// Contato.
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
        /// Email de Contato.
        /// </summary>
        public string EmailContato
        {
            get { return DataModel.EmailContato; }
            set
            {
                if (DataModel.EmailContato != value &&
                    RaisePropertyChanging("EmailContato", value))
                {
                    DataModel.EmailContato = value;
                    RaisePropertyChanged("EmailContato");
                }
            }
        }

        /// <summary>
        /// Tel. de Contato.
        /// </summary>
        public string TelContato
        {
            get { return DataModel.TelContato; }
            set
            {
                if (DataModel.TelContato != value &&
                    RaisePropertyChanging("TelContato", value))
                {
                    DataModel.TelContato = value;
                    RaisePropertyChanged("TelContato");
                }
            }
        }

        /// <summary>
        /// Documento estrangeiro
        /// </summary>
        public string PassaporteDoc
        {
            get { return DataModel.PassaporteDoc; }
            set
            {
                if (DataModel.PassaporteDoc != value &&
                    RaisePropertyChanging("PassaporteDoc", value))
                {
                    DataModel.PassaporteDoc = value;
                    RaisePropertyChanged("PassaporteDoc");
                }
            }
        }

        #endregion

        #region Propriedades referenciadas/filhos

        /// <summary>
        /// Relação dos produtos do fornecedor.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<ProdutoFornecedor> ProdutosFornecedor
        {
            get { return _produtosFornecedor; }
        }

        /// <summary>
        /// Parcelas não utilizadas pelo fornecedor.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<Financeiro.Negocios.Entidades.ParcelasNaoUsar> Parcelas
        {
            get { return _parcelas; }
        }

        /// <summary>
        /// Cidade do fornecedor.
        /// </summary>
        public Cidade Cidade
        {
            get { return GetReference<Cidade>("Cidade", true); }
        }

        /// <summary>
        /// Referência ao plano de conta contábil.
        /// </summary>
        public Fiscal.Negocios.Entidades.PlanoContaContabil PlanoContaContabil
        {
            get { return GetReference<Fiscal.Negocios.Entidades.PlanoContaContabil>("PlanoContaContabil", true); }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Fornecedor()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Fornecedor(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.Fornecedor> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _produtosFornecedor = GetChild<ProdutoFornecedor>(args.Children, "Produtos");
            _parcelas = GetChild<Financeiro.Negocios.Entidades.ParcelasNaoUsar>(args.Children, "Parcelas");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Fornecedor(Data.Model.Fornecedor dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _produtosFornecedor = CreateChild<Colosoft.Business.IEntityChildrenList<ProdutoFornecedor>>("Produtos");
            _parcelas = CreateChild<Colosoft.Business.IEntityChildrenList<Financeiro.Negocios.Entidades.ParcelasNaoUsar>>("Parcelas");
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Método acionado para apgar 
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorFornecedor>();

            var resultadoValidacao = validador.ValidaExistencia(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
