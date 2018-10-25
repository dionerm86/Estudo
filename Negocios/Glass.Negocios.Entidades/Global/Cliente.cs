using System;
using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador de cliente.
    /// </summary>
    public interface IValidadorCliente
    {
        /// <summary>
        /// Valida a atualização do cliente.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaAtualizacao(Cliente cliente);

        /// <summary>
        /// Valida a existencia do cliente.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(Cliente cliente);
    }

    /// <summary>
    /// Representa a entidade de negócio do cliente.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(ClienteLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.Cliente)]
    public class Cliente : Glass.Negocios.Entidades.EntidadeBaseCadastro<Data.Model.Cliente>
    {
        #region Tipos Aninhados

        class ClienteLoader : Colosoft.Business.EntityLoader<Cliente, Data.Model.Cliente>
        {
            public ClienteLoader()
            {
                Configure()
                    .Uid(f => f.IdCli)
                    .FindName(new ClienteFindNameConverter(), f => f.Nome, f => f.NomeFantasia)
                    .Child<DescontoAcrescimoCliente, Data.Model.DescontoAcrescimoCliente>("DescontosAcrescimos",
                        f => f.DescontosAcrescimos, f => f.IdCliente)
                    .Child<Financeiro.Negocios.Entidades.ParcelasNaoUsar, Data.Model.ParcelasNaoUsar>("Parcelas", f => f.Parcelas, f => f.IdCliente)
                    .Log("Parcelas", "Parcelas não usar")
                    .Child<Financeiro.Negocios.Entidades.FormaPagtoCliente, Data.Model.FormaPagtoCliente>("FormasPagto", f => f.FormasPagto, f => f.IdCliente)
                    .Log("FormasPagto", "Formas Pagto")
                    .Reference<Comissionado, Data.Model.Comissionado>("Comissionado", f => f.Comissionado, f => f.IdComissionado)
                    .Reference<TabelaDescontoAcrescimoCliente, Data.Model.TabelaDescontoAcrescimoCliente>("TabelaDescontoAcrescimo",
                        f => f.TabelaDescontoAcrescimo, f => f.IdTabelaDesconto)
                    .Reference<Cidade, Data.Model.Cidade>("Cidade", f => f.Cidade, f => f.IdCidade)
                    .Reference<Cidade, Data.Model.Cidade>("CidadeCobranca", f => f.CidadeCobranca, f => f.IdCidadeCobranca)
                    .Reference<Cidade, Data.Model.Cidade>("CidadeEntrega", f => f.CidadeEntrega, f => f.IdCidadeEntrega)
                    .Reference<RotaCliente, Data.Model.RotaCliente>("RotaCliente", f => f.RotaCliente, () =>
                        new Colosoft.Business.EntityLoaderConditional("IdCliente=?id")
                            .Add("?id", new Colosoft.Query.ReferenceParameter("IdCli")),
                        f => f.IdRota)
                    .Reference<Fiscal.Negocios.Entidades.PlanoContaContabil, Data.Model.PlanoContaContabil>("PlanoContaContabil",
                        f => f.PlanoContaContabil, f => f.IdContaContabil)
                    .Creator(f => new Cliente(f));
            }

            class ClienteFindNameConverter : Colosoft.IFindNameConverter
            {
                /// <summary>
                /// Converte os valores para o nome do cliente.
                /// </summary>
                /// <param name="baseInfo"></param>
                /// <returns></returns>
                public string Convert(object[] baseInfo)
                {
                    var nome = (string)baseInfo[0];
                    var nomeFantasia = (string)baseInfo[1];

                    return Configuracoes.Liberacao.RelatorioLiberacaoPedido.TipoNomeExibirRelatorioPedido ==
                        Data.Helper.DataSources.TipoNomeExibirRelatorioPedido.NomeFantasia ?
                            nomeFantasia ?? nome :
                            nome ?? nomeFantasia;
                }
            }
        }

        #endregion

        #region Variáveis locais

        private Colosoft.Business.IEntityChildrenList<DescontoAcrescimoCliente> _descontosAcrescimos;
        private Colosoft.Business.IEntityChildrenList<Financeiro.Negocios.Entidades.ParcelasNaoUsar> _parcelas;
        private Colosoft.Business.IEntityChildrenList<Financeiro.Negocios.Entidades.FormaPagtoCliente> _formasPagto;

        private bool _rotaAnteriorCarregada = false;
        private Rota _rotaAnterior;
        private int? _idRota;

        private Rota _rota;

        #endregion

        #region Propriedades

        /// <summary>
        /// Código do cliente.
        /// </summary>
        public int IdCli
        {
            get { return DataModel.IdCli; }
            set
            {
                if (DataModel.IdCli != value &&
                    RaisePropertyChanging("IdCli", value))
                {
                    DataModel.IdCli = value;
                    RaisePropertyChanged("IdCli");
                }
            }
        }

        /// <summary>
        /// Código da loja do cliente.
        /// </summary>
        public int? IdLoja
        {
            get { return DataModel.IdLoja; }
            set
            {
                if (DataModel.IdLoja != value &&
                    RaisePropertyChanging("IdLoja", value))
                {
                    DataModel.IdLoja = value;
                    RaisePropertyChanged("IdLoja");
                }
            }
        }

        /// <summary>
        /// Código da cidade do cliente.
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
        /// Código do comissionado do cliente.
        /// </summary>
        public int? IdComissionado
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
        /// Código do vendedor do cliente.
        /// </summary>
        public int? IdFunc
        {
            get { return DataModel.IdFunc; }
            set
            {
                if (DataModel.IdFunc != value &&
                    RaisePropertyChanging("IdFunc", value))
                {
                    DataModel.IdFunc = value;
                    RaisePropertyChanged("IdFunc");
                }
            }
        }

        /// <summary>
        /// Código da forma de pagamento padrão do cliente.
        /// </summary>
        public int? IdFormaPagto
        {
            get { return DataModel.IdFormaPagto; }
            set
            {
                if (DataModel.IdFormaPagto != value &&
                    RaisePropertyChanging("IdFormaPagto", value))
                {
                    DataModel.IdFormaPagto = value;
                    RaisePropertyChanged("IdFormaPagto");
                }
            }
        }

        /// <summary>
        /// Código do tipo de cliente.
        /// </summary>
        public int? IdTipoCliente
        {
            get { return DataModel.IdTipoCliente; }
            set
            {
                if (DataModel.IdTipoCliente != value &&
                    RaisePropertyChanging("IdTipoCliente", value))
                {
                    DataModel.IdTipoCliente = value;
                    RaisePropertyChanged("IdTipoCliente");
                }
            }
        }

        /// <summary>
        /// Código do plano de conta contábil do cliente.
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
        /// Tipo fiscal do cliente.
        /// </summary>
        public Data.Model.TipoFiscalCliente? TipoFiscal
        {
            get { return (Data.Model.TipoFiscalCliente?)DataModel.TipoFiscal; }
            set
            {
                if (DataModel.TipoFiscal != (int?)value &&
                    RaisePropertyChanging("TipoFiscal", value))
                {
                    DataModel.TipoFiscal = (int?)value;
                    RaisePropertyChanged("TipoFiscal");
                }
            }
        }

        /// <summary>
        /// Indicador da IE do destinatário.
        /// </summary>
        public IndicadorIEDestinatario IndicadorIEDestinatario
        {
            get { return DataModel.IndicadorIEDestinatario; }
            set
            {
                if (DataModel.IndicadorIEDestinatario != value &&
                    RaisePropertyChanging("IndicadorIEDestinatario", value))
                {
                    DataModel.IndicadorIEDestinatario = value;
                    RaisePropertyChanged("IndicadorIEDestinatario");
                }
            }
        }

        /// <summary>
        /// Código da tabela de desconto/acréscimo do cliente.
        /// </summary>
        public int? IdTabelaDesconto
        {
            get { return DataModel.IdTabelaDesconto; }
            set
            {
                if (DataModel.IdTabelaDesconto != value &&
                    RaisePropertyChanging("IdTabelaDesconto", value))
                {
                    DataModel.IdTabelaDesconto = value;
                    RaisePropertyChanged("IdTabelaDesconto");
                }
            }
        }

        /// <summary>
        /// Código do transportador vinculado ao cliente.
        /// </summary>
        public int? IdTransportador
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
        /// Códido da conta bancária associada ao cliente.
        /// </summary>
        public int? IdContaBanco
        {
            get { return DataModel.IdContaBanco; }
            set
            {
                if (DataModel.IdContaBanco != value &&
                    RaisePropertyChanging("IdContaBanco", value))
                {
                    DataModel.IdContaBanco = value;
                    RaisePropertyChanged("IdContaBanco");
                }
            }
        }

        /// <summary>
        /// Tipo de pessoa do cliente.
        /// </summary>
        public Data.Model.TipoPessoa TipoPessoa
        {
            get { return (Data.Model.TipoPessoa)DataModel.TipoPessoa.FirstOrDefault(); }
            set
            {
                if ((DataModel.TipoPessoa ?? "").FirstOrDefault() != (char)value &&
                    RaisePropertyChanging("TipoPessoa", value))
                {
                    DataModel.TipoPessoa = ((char)value).ToString();
                    RaisePropertyChanged("TipoPessoa");
                }
            }
        }

        /// <summary>
        /// Nome (ou razão social) do cliente.
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
        /// Apelido (ou nome fantasia) do cliente.
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
        /// Logradouro no endereço do cliente.
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
        /// Número no endereço do cliente.
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
        /// Complemento no endereço do cliente.
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
        /// Bairro no endereço do cliente.
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
        /// CEP no endereço do cliente.
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
        /// país.
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
        /// CPF ou CNPJ do cliente.
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
        /// RG (ou inscrição estadual) do cliente.
        /// </summary>
        public string RgEscinst
        {
            get { return DataModel.RgEscinst; }
            set
            {
                if (DataModel.RgEscinst != value &&
                    RaisePropertyChanging("RgEscinst", value))
                {
                    DataModel.RgEscinst = value;
                    RaisePropertyChanged("RgEscinst");
                }
            }
        }

        /// <summary>
        /// Identificação do destinatário no caso de comprador estrangeiro.
        /// </summary>
        public string NumEstrangeiro
        {
            get { return DataModel.NumEstrangeiro; }
            set
            {
                if (DataModel.NumEstrangeiro != value &&
                    RaisePropertyChanging("NumEstrangeiro", value))
                {
                    DataModel.NumEstrangeiro = value;
                    RaisePropertyChanged("NumEstrangeiro");
                }
            }
        }

        /// <summary>
        /// Código de cadastro na SUFRAMA do cliente.
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
        /// Data de nascimento do cliente.
        /// </summary>
        public DateTime? DataNasc
        {
            get { return DataModel.DataNasc; }
            set
            {
                if (DataModel.DataNasc != value &&
                    RaisePropertyChanging("DataNasc", value))
                {
                    DataModel.DataNasc = value;
                    RaisePropertyChanged("DataNasc");
                }
            }
        }

        /// <summary>
        /// Data da última compra feita pelo cliente.
        /// </summary>
        public DateTime? DtUltCompra
        {
            get { return DataModel.DtUltCompra; }
            set
            {
                if (DataModel.DtUltCompra != value &&
                    RaisePropertyChanging("DtUltCompra", value))
                {
                    DataModel.DtUltCompra = value;
                    RaisePropertyChanged("DtUltCompra");
                }
            }
        }

        /// <summary>
        /// Total já comprado pelo cliente.
        /// </summary>
        public decimal TotalComprado
        {
            get { return DataModel.TotalComprado; }
            set
            {
                if (DataModel.TotalComprado != value &&
                    RaisePropertyChanging("TotalComprado", value))
                {
                    DataModel.TotalComprado = value;
                    RaisePropertyChanged("TotalComprado");
                }
            }
        }

        /// <summary>
        /// Telefone residencial do cliente.
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
        /// Telefone de contato do cliente.
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
        /// Telefone celular do cliente.
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
        /// Fax do cliente.
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
        /// Contato no cliente.
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
        /// Email de contato do cliente.
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
        /// Setor do contato do cliente.
        /// </summary>
        public string SetorContato
        {
            get { return DataModel.SetorContato; }
            set
            {
                if (DataModel.SetorContato != value &&
                    RaisePropertyChanging("SetorContato", value))
                {
                    DataModel.SetorContato = value;
                    RaisePropertyChanged("SetorContato");
                }
            }
        }

        /// <summary>
        /// Observações sobre o cliente.
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
        /// Histórico do cliente (feito manualmente).
        /// </summary>
        public string Historico
        {
            get { return DataModel.Historico; }
            set
            {
                if (DataModel.Historico != value &&
                    RaisePropertyChanging("Historico", value))
                {
                    DataModel.Historico = value;
                    RaisePropertyChanged("Historico");
                }
            }
        }

        /// <summary>
        /// Crédito atual do cliente.
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
        /// Limite do cliente.
        /// </summary>
        public decimal Limite
        {
            get { return DataModel.Limite; }
            set
            {
                if (DataModel.Limite != value &&
                    RaisePropertyChanging("Limite", value))
                {
                    DataModel.Limite = value;
                    RaisePropertyChanged("Limite");
                }
            }
        }

        /// <summary>
        /// Limite de cheques do cliente.
        /// </summary>
        public decimal LimiteCheques
        {
            get { return DataModel.LimiteCheques; }
            set
            {
                if (DataModel.LimiteCheques != value &&
                    RaisePropertyChanging("LimiteCheques", value))
                {
                    DataModel.LimiteCheques = value;
                    RaisePropertyChanged("LimiteCheques");
                }
            }
        }

        /// <summary>
        /// Valor inicial da média de compras mensal.
        /// </summary>
        public decimal ValorMediaIni
        {
            get { return DataModel.ValorMediaIni; }
            set
            {
                if (DataModel.ValorMediaIni != value &&
                    RaisePropertyChanging("ValorMediaIni", value))
                {
                    DataModel.ValorMediaIni = value;
                    RaisePropertyChanged("ValorMediaIni");
                }
            }
        }

        /// <summary>
        /// Valor final da média de compras mensal.
        /// </summary>
        public decimal ValorMediaFim
        {
            get { return DataModel.ValorMediaFim; }
            set
            {
                if (DataModel.ValorMediaFim != value &&
                    RaisePropertyChanging("ValorMediaFim", value))
                {
                    DataModel.ValorMediaFim = value;
                    RaisePropertyChanged("ValorMediaFim");
                }
            }
        }

        /// <summary>
        /// E-mail do cliente.
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
        /// E-mail Fiscal do cliente.
        /// </summary>
        public string EmailFiscal
        {
            get { return DataModel.EmailFiscal; }
            set
            {
                if(DataModel.EmailFiscal != value &&
                    RaisePropertyChanging("EmailFiscal", value))
                {
                    DataModel.EmailFiscal = value;
                    RaisePropertyChanged("EmailFiscal");
                }
            }
        }

        /// <summary>
        /// E-mail de Cobrança do cliente.
        /// </summary>
        public string EmailCobranca
        {
            get { return DataModel.EmailCobranca; }
            set
            {
                if (DataModel.EmailCobranca != value &&
                    RaisePropertyChanging("EmailCobranca", value))
                {
                    DataModel.EmailCobranca = value;
                    RaisePropertyChanged("EmailCobranca");
                }
            }
        }

        /// <summary>
        /// Login de acesso ao sistema B2B do cliente.
        /// </summary>
        public string Login
        {
            get { return DataModel.Login; }
            set
            {
                if (DataModel.Login != value &&
                    RaisePropertyChanging("Login", value))
                {
                    DataModel.Login = value;
                    RaisePropertyChanged("Login");
                }
            }
        }

        /// <summary>
        /// Senha de acesso ao sistema B2B do cliente.
        /// </summary>
        public string Senha
        {
            get { return DataModel.Senha; }
            set
            {
                if (DataModel.Senha != value &&
                    RaisePropertyChanging("Senha", value))
                {
                    DataModel.Senha = value;
                    RaisePropertyChanged("Senha");
                }
            }
        }

        /// <summary>
        /// Situação do cliente.
        /// </summary>
        public Data.Model.SituacaoCliente Situacao
        {
            get { return (Data.Model.SituacaoCliente)DataModel.Situacao; }
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
        /// O cliente é de revenda?.
        /// </summary>
        public bool Revenda
        {
            get { return DataModel.Revenda; }
            set
            {
                if (DataModel.Revenda != value &&
                    RaisePropertyChanging("Revenda", value))
                {
                    DataModel.Revenda = value;
                    RaisePropertyChanged("Revenda");
                }
            }
        }

        /// <summary>
        /// Nome do primeiro contato do cliente.
        /// </summary>
        public string Contato1
        {
            get { return DataModel.Contato1; }
            set
            {
                if (DataModel.Contato1 != value &&
                    RaisePropertyChanging("Contato1", value))
                {
                    DataModel.Contato1 = value;
                    RaisePropertyChanged("Contato1");
                }
            }
        }

        /// <summary>
        /// Telefone celular do primeiro contato do cliente.
        /// </summary>
        public string CelContato1
        {
            get { return DataModel.CelContato1; }
            set
            {
                if (DataModel.CelContato1 != value &&
                    RaisePropertyChanging("CelContato1", value))
                {
                    DataModel.CelContato1 = value;
                    RaisePropertyChanged("CelContato1");
                }
            }
        }

        /// <summary>
        /// Ramal do primeiro contato do cliente.
        /// </summary>
        public string RamalContato1
        {
            get { return DataModel.RamalContato1; }
            set
            {
                if (DataModel.RamalContato1 != value &&
                    RaisePropertyChanging("RamalContato1", value))
                {
                    DataModel.RamalContato1 = value;
                    RaisePropertyChanged("RamalContato1");
                }
            }
        }

        /// <summary>
        /// Email do primeiro contato do cliente.
        /// </summary>
        public string EmailContato1
        {
            get { return DataModel.EmailContato1; }
            set
            {
                if (DataModel.EmailContato1 != value &&
                    RaisePropertyChanging("EmailContato1", value))
                {
                    DataModel.EmailContato1 = value;
                    RaisePropertyChanged("EmailContato1");
                }
            }
        }

        /// <summary>
        /// Nome do segundo contato do cliente.
        /// </summary>
        public string Contato2
        {
            get { return DataModel.Contato2; }
            set
            {
                if (DataModel.Contato2 != value &&
                    RaisePropertyChanging("Contato2", value))
                {
                    DataModel.Contato2 = value;
                    RaisePropertyChanged("Contato2");
                }
            }
        }

        /// <summary>
        /// Telefone celular do segundo contato do cliente.
        /// </summary>
        public string CelContato2
        {
            get { return DataModel.CelContato2; }
            set
            {
                if (DataModel.CelContato2 != value &&
                    RaisePropertyChanging("CelContato2", value))
                {
                    DataModel.CelContato2 = value;
                    RaisePropertyChanged("CelContato2");
                }
            }
        }

        /// <summary>
        /// Ramal do segundo contato do cliente.
        /// </summary>
        public string RamalContato2
        {
            get { return DataModel.RamalContato2; }
            set
            {
                if (DataModel.RamalContato2 != value &&
                    RaisePropertyChanging("RamalContato2", value))
                {
                    DataModel.RamalContato2 = value;
                    RaisePropertyChanged("RamalContato2");
                }
            }
        }

        /// <summary>
        /// Email do segundo contato do cliente.
        /// </summary>
        public string EmailContato2
        {
            get { return DataModel.EmailContato2; }
            set
            {
                if (DataModel.EmailContato2 != value &&
                    RaisePropertyChanging("EmailContato2", value))
                {
                    DataModel.EmailContato2 = value;
                    RaisePropertyChanged("EmailContato2");
                }
            }
        }

        /// <summary>
        /// Data da última alteração no cadastro do cliente.
        /// </summary>
        public DateTime? DataAlt
        {
            get { return DataModel.DataAlt; }
            set
            {
                if (DataModel.DataAlt != value &&
                    RaisePropertyChanging("DataAlt", value))
                {
                    DataModel.DataAlt = value;
                    RaisePropertyChanged("DataAlt");
                }
            }
        }

        /// <summary>
        /// Funcionário que realizou a última alteração no cadastro do cliente.
        /// </summary>
        public int? UsuAlt
        {
            get { return DataModel.UsuAlt; }
            set
            {
                if (DataModel.UsuAlt != value &&
                    RaisePropertyChanging("UsuAlt", value))
                {
                    DataModel.UsuAlt = value;
                    RaisePropertyChanged("UsuAlt");
                }
            }
        }

        /// <summary>
        /// Parcela padrão do cliente.
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
        /// O cliente calcula ICMS ST no pedido?
        /// </summary>
        public bool CobrarIcmsSt
        {
            get { return DataModel.CobrarIcmsSt; }
            set
            {
                if (DataModel.CobrarIcmsSt != value &&
                    RaisePropertyChanging("CobrarIcmsSt", value))
                {
                    DataModel.CobrarIcmsSt = value;
                    RaisePropertyChanged("CobrarIcmsSt");
                }
            }
        }

        /// <summary>
        /// O cliente calcula IPI no pedido?
        /// </summary>
        public bool CobrarIpi
        {
            get { return DataModel.CobrarIpi; }
            set
            {
                if (DataModel.CobrarIpi != value &&
                    RaisePropertyChanging("CobrarIpi", value))
                {
                    DataModel.CobrarIpi = value;
                    RaisePropertyChanged("CobrarIpi");
                }
            }
        }

        /// <summary>
        /// Percentual de redução ao gerar NFe do cliente (produtos de venda).
        /// </summary>
        public float PercReducaoNFe
        {
            get { return DataModel.PercReducaoNFe; }
            set
            {
                if (DataModel.PercReducaoNFe != value &&
                    RaisePropertyChanging("PercReducaoNFe", value))
                {
                    DataModel.PercReducaoNFe = value;
                    RaisePropertyChanged("PercReducaoNFe");
                }
            }
        }

        /// <summary>
        /// Percentual de redução ao gerar NFe do cliente (produtos de revenda).
        /// </summary>
        public float PercReducaoNFeRevenda
        {
            get { return DataModel.PercReducaoNFeRevenda; }
            set
            {
                if (DataModel.PercReducaoNFeRevenda != value &&
                    RaisePropertyChanging("PercReducaoNFeRevenda", value))
                {
                    DataModel.PercReducaoNFeRevenda = value;
                    RaisePropertyChanged("PercReducaoNFeRevenda");
                }
            }
        }

        /// <summary>
        /// Percentual de sinal mínimo para os pedidos do cliente.
        /// </summary>
        public float? PercSinalMinimo
        {
            get { return DataModel.PercSinalMinimo; }
            set
            {
                if (DataModel.PercSinalMinimo != value &&
                    RaisePropertyChanging("PercSinalMinimo", value))
                {
                    DataModel.PercSinalMinimo = value;
                    RaisePropertyChanged("PercSinalMinimo");
                }
            }
        }

        /// <summary>
        /// Percentual de comissão do cliente (comissionado).
        /// </summary>
        public float PercentualComissao
        {
            get { return DataModel.PercentualComissao; }
            set
            {
                if (DataModel.PercentualComissao != value &&
                    RaisePropertyChanging("PercentualComissao", value))
                {
                    DataModel.PercentualComissao = value;
                    RaisePropertyChanged("PercentualComissao");
                }
            }
        }

        /// <summary>
        /// O cliente deve pagar o pedido antes de produzí-lo?
        /// </summary>
        public bool PagamentoAntesProducao
        {
            get { return DataModel.PagamentoAntesProducao; }
            set
            {
                if (DataModel.PagamentoAntesProducao != value &&
                    RaisePropertyChanging("PagamentoAntesProducao", value))
                {
                    DataModel.PagamentoAntesProducao = value;
                    RaisePropertyChanged("PagamentoAntesProducao");
                }
            }
        }

        /// <summary>
        /// O cliente ignora o bloqueio de pedido pronto (podendo liberar produtos pendentes, se necessário)?
        /// </summary>
        public bool IgnorarBloqueioPedPronto
        {
            get { return DataModel.IgnorarBloqueioPedPronto; }
            set
            {
                if (DataModel.IgnorarBloqueioPedPronto != value &&
                    RaisePropertyChanging("IgnorarBloqueioPedPronto", value))
                {
                    DataModel.IgnorarBloqueioPedPronto = value;
                    RaisePropertyChanged("IgnorarBloqueioPedPronto");
                }
            }
        }

        /// <summary>
        /// Bloquear recebimento cheque limite (verificar).
        /// </summary>
        public bool BloquearRecebChequeLimite
        {
            get { return DataModel.BloquearRecebChequeLimite; }
            set
            {
                if (DataModel.BloquearRecebChequeLimite != value &&
                    RaisePropertyChanging("BloquearRecebChequeLimite", value))
                {
                    DataModel.BloquearRecebChequeLimite = value;
                    RaisePropertyChanged("BloquearRecebChequeLimite");
                }
            }
        }

        /// <summary>
        /// Bloquear recebimento cheque próprio limite (verificar).
        /// </summary>
        public bool BloquearRecebChequeProprioLimite
        {
            get { return DataModel.BloquearRecebChequeProprioLimite; }
            set
            {
                if (DataModel.BloquearRecebChequeProprioLimite != value &&
                    RaisePropertyChanging("BloquearRecebChequeProprioLimite", value))
                {
                    DataModel.BloquearRecebChequeProprioLimite = value;
                    RaisePropertyChanged("BloquearRecebChequeProprioLimite");
                }
            }
        }

        /// <summary>
        /// Logradouro no endereço de cobrança do cliente.
        /// </summary>
        public string EnderecoCobranca
        {
            get { return DataModel.EnderecoCobranca; }
            set
            {
                if (DataModel.EnderecoCobranca != value &&
                    RaisePropertyChanging("EnderecoCobranca", value))
                {
                    DataModel.EnderecoCobranca = value;
                    RaisePropertyChanged("EnderecoCobranca");
                }
            }
        }

        /// <summary>
        /// Número no endereço de cobrança do cliente.
        /// </summary>
        public string NumeroCobranca
        {
            get { return DataModel.NumeroCobranca; }
            set
            {
                if (DataModel.NumeroCobranca != value &&
                    RaisePropertyChanging("NumeroCobranca", value))
                {
                    DataModel.NumeroCobranca = value;
                    RaisePropertyChanged("NumeroCobranca");
                }
            }
        }

        /// <summary>
        /// Complemento no endereço de cobrança do cliente.
        /// </summary>
        public string ComplCobranca
        {
            get { return DataModel.ComplCobranca; }
            set
            {
                if (DataModel.ComplCobranca != value &&
                    RaisePropertyChanging("ComplCobranca", value))
                {
                    DataModel.ComplCobranca = value;
                    RaisePropertyChanged("ComplCobranca");
                }
            }
        }

        /// <summary>
        /// Bairro no endereço de cobrança do cliente.
        /// </summary>
        public string BairroCobranca
        {
            get { return DataModel.BairroCobranca; }
            set
            {
                if (DataModel.BairroCobranca != value &&
                    RaisePropertyChanging("BairroCobranca", value))
                {
                    DataModel.BairroCobranca = value;
                    RaisePropertyChanged("BairroCobranca");
                }
            }
        }

        /// <summary>
        /// CEP no endereço de cobrança do cliente.
        /// </summary>
        public string CepCobranca
        {
            get { return DataModel.CepCobranca; }
            set
            {
                if (DataModel.CepCobranca != value &&
                    RaisePropertyChanging("CepCobranca", value))
                {
                    DataModel.CepCobranca = value;
                    RaisePropertyChanged("CepCobranca");
                }
            }
        }

        /// <summary>
        /// Código da cidade no endereço de cobrança do cliente.
        /// </summary>
        public int? IdCidadeCobranca
        {
            get { return DataModel.IdCidadeCobranca; }
            set
            {
                if (DataModel.IdCidadeCobranca != value &&
                    RaisePropertyChanging("IdCidadeCobranca", value))
                {
                    DataModel.IdCidadeCobranca = value;
                    RaisePropertyChanged("IdCidadeCobranca");
                }
            }
        }

        /// <summary>
        /// CEP no endereço de entrega do cliente.
        /// </summary>
        public string CepEntrega
        {
            get { return DataModel.CepEntrega; }
            set
            {
                if (DataModel.CepEntrega != value &&
                    RaisePropertyChanging("CepEntrega", value))
                {
                    DataModel.CepEntrega = value;
                    RaisePropertyChanged("CepEntrega");
                }
            }
        }

        /// <summary>
        /// Logradouro no endereço de entrega do cliente.
        /// </summary>
        public string EnderecoEntrega
        {
            get { return DataModel.EnderecoEntrega; }
            set
            {
                if (DataModel.EnderecoEntrega != value &&
                    RaisePropertyChanging("EnderecoEntrega", value))
                {
                    DataModel.EnderecoEntrega = value;
                    RaisePropertyChanged("EnderecoEntrega");
                }
            }
        }

        /// <summary>
        /// Número no endereço de entrega do cliente.
        /// </summary>
        public string NumeroEntrega
        {
            get { return DataModel.NumeroEntrega; }
            set
            {
                if (DataModel.NumeroEntrega != value &&
                    RaisePropertyChanging("NumeroEntrega", value))
                {
                    DataModel.NumeroEntrega = value;
                    RaisePropertyChanged("NumeroEntrega");
                }
            }
        }

        /// <summary>
        /// Complemento no endereço de entrega do cliente.
        /// </summary>
        public string ComplEntrega
        {
            get { return DataModel.ComplEntrega; }
            set
            {
                if (DataModel.ComplEntrega != value &&
                    RaisePropertyChanging("ComplEntrega", value))
                {
                    DataModel.ComplEntrega = value;
                    RaisePropertyChanged("ComplEntrega");
                }
            }
        }

        /// <summary>
        /// Bairro no endereço de entrega do cliente.
        /// </summary>
        public string BairroEntrega
        {
            get { return DataModel.BairroEntrega; }
            set
            {
                if (DataModel.BairroEntrega != value &&
                    RaisePropertyChanging("BairroEntrega", value))
                {
                    DataModel.BairroEntrega = value;
                    RaisePropertyChanged("BairroEntrega");
                }
            }
        }

        /// <summary>
        /// Código da cidade no endereço de entrega do cliente.
        /// </summary>
        public int? IdCidadeEntrega
        {
            get { return DataModel.IdCidadeEntrega; }
            set
            {
                if (DataModel.IdCidadeEntrega != value &&
                    RaisePropertyChanging("IdCidadeEntrega", value))
                {
                    DataModel.IdCidadeEntrega = value;
                    RaisePropertyChanged("IdCidadeEntrega");
                }
            }
        }

        /// <summary>
        /// O cliente não recebe email de pedido pronto?
        /// </summary>
        public bool NaoReceberEmailPedPronto
        {
            get { return DataModel.NaoReceberEmailPedPronto; }
            set
            {
                if (DataModel.NaoReceberEmailPedPronto != value &&
                    RaisePropertyChanging("NaoReceberEmailPedPronto", value))
                {
                    DataModel.NaoReceberEmailPedPronto = value;
                    RaisePropertyChanged("NaoReceberEmailPedPronto");
                }
            }
        }

        /// <summary>
        /// O cliente não recebe email de pedido PCP?
        /// </summary>
        public bool NaoReceberEmailPedPcp
        {
            get { return DataModel.NaoReceberEmailPedPcp; }
            set
            {
                if (DataModel.NaoReceberEmailPedPcp != value &&
                    RaisePropertyChanging("NaoReceberEmailPedPcp", value))
                {
                    DataModel.NaoReceberEmailPedPcp = value;
                    RaisePropertyChanged("NaoReceberEmailPedPcp");
                }
            }
        }

        /// <summary>
        /// O cliente não recebe email de nota fiscal?
        /// </summary>
        public bool NaoReceberEmailFiscal
        {
            get { return DataModel.NaoReceberEmailFiscal; }
            set
            {
                if (DataModel.NaoReceberEmailFiscal != value &&
                    RaisePropertyChanging("NaoReceberEmailFiscal", value))
                {
                    DataModel.NaoReceberEmailFiscal = value;
                    RaisePropertyChanged("NaoReceberEmailFiscal");
                }
            }
        }

        /// <summary>
        /// O cliente não recebe SMS?
        /// </summary>
        public bool NaoReceberSms
        {
            get { return DataModel.NaoReceberSms; }
            set
            {
                if (DataModel.NaoReceberSms != value &&
                    RaisePropertyChanging("NaoReceberSms", value))
                {
                    DataModel.NaoReceberSms = value;
                    RaisePropertyChanged("NaoReceberSms");
                }
            }
        }

        /// <summary>
        /// O cliente não recebe email de cobrança de contas vencidas?
        /// </summary>
        public bool NaoReceberEmailCobrancaVencida
        {
            get { return DataModel.NaoReceberEmailCobrancaVencida; }
            set
            {
                if (DataModel.NaoReceberEmailCobrancaVencida != value &&
                    RaisePropertyChanging("NaoReceberEmailCobrancaVencida", value))
                {
                    DataModel.NaoReceberEmailCobrancaVencida = value;
                    RaisePropertyChanged("NaoReceberEmailCobrancaVencida");
                }
            }
        }

        /// <summary>
        /// O cliente não recebe email de cobrança de contas a vencer?
        /// </summary>
        public bool NaoReceberEmailCobrancaVencer
        {
            get { return DataModel.NaoReceberEmailCobrancaVencer; }
            set
            {
                if (DataModel.NaoReceberEmailCobrancaVencer != value &&
                    RaisePropertyChanging("NaoReceberEmailCobrancaVencer", value))
                {
                    DataModel.NaoReceberEmailCobrancaVencer = value;
                    RaisePropertyChanged("NaoReceberEmailCobrancaVencer");
                }
            }
        }

        /// <summary>
        /// Percentual de comissão do vendedor.
        /// </summary>
        public float PercComissaoFunc
        {
            get { return DataModel.PercComissaoFunc; }
            set
            {
                if (DataModel.PercComissaoFunc != value &&
                    RaisePropertyChanging("PercComissaoFunc", value))
                {
                    DataModel.PercComissaoFunc = value;
                    RaisePropertyChanged("PercComissaoFunc");
                }
            }
        }

        /// <summary>
        /// Bloquear emissão de pedidos se houver conta vencida para o cliente?
        /// </summary>
        public bool BloquearPedidoContaVencida
        {
            get { return DataModel.BloquearPedidoContaVencida; }
            set
            {
                if (DataModel.BloquearPedidoContaVencida != value &&
                    RaisePropertyChanging("BloquearPedidoContaVencida", value))
                {
                    DataModel.BloquearPedidoContaVencida = value;
                    RaisePropertyChanged("BloquearPedidoContaVencida");
                }
            }
        }

        /// <summary>
        /// URL do sistema WebGlass do cliente.
        /// (utilizado para importação/exportação de pedidos)
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
        /// Gerar orçamento de ferragens pelo PCP?
        /// </summary>
        public bool GerarOrcamentoPcp
        {
            get { return DataModel.GerarOrcamentoPcp; }
            set
            {
                if (DataModel.GerarOrcamentoPcp != value &&
                    RaisePropertyChanging("GerarOrcamentoPcp", value))
                {
                    DataModel.GerarOrcamentoPcp = value;
                    RaisePropertyChanged("GerarOrcamentoPcp");
                }
            }
        }

        /// <summary>
        /// O cliente é produtor rural?
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
        /// Data da última consulta dos dados do cliente ao Sintegra.
        /// </summary>
        public DateTime? DtUltConSintegra
        {
            get { return DataModel.DtUltConSintegra; }
            set
            {
                if (DataModel.DtUltConSintegra != value &&
                    RaisePropertyChanging("DtUltConSintegra", value))
                {
                    DataModel.DtUltConSintegra = value;
                    RaisePropertyChanged("DtUltConSintegra");
                }
            }
        }

        /// <summary>
        /// Funcionário que realizou a última consulta dos dados do cliente ao Sintegra.
        /// </summary>
        public int? UsuUltConSintegra
        {
            get { return DataModel.UsuUltConSintegra; }
            set
            {
                if (DataModel.UsuUltConSintegra != value &&
                    RaisePropertyChanging("UsuUltConSintegra", value))
                {
                    DataModel.UsuUltConSintegra = value;
                    RaisePropertyChanged("UsuUltConSintegra");
                }
            }
        }

        /// <summary>
        /// CRT do cliente.
        /// </summary>
        public Data.Model.CrtCliente Crt
        {
            get { return (Data.Model.CrtCliente)DataModel.Crt; }
            set
            {
                if (DataModel.Crt != (int)value &&
                    RaisePropertyChanging("Crt", value))
                {
                    DataModel.Crt = (int)value;
                    RaisePropertyChanged("Crt");
                }
            }
        }

        /// <summary>
        /// Controlar estoque de vidros do cliente?
        /// (utilizado para pedidos de mão-de-obra especial)
        /// </summary>
        public bool ControlarEstoqueVidros
        {
            get { return DataModel.ControlarEstoqueVidros; }
            set
            {
                if (DataModel.ControlarEstoqueVidros != value &&
                    RaisePropertyChanging("ControlarEstoqueVidros", value))
                {
                    DataModel.ControlarEstoqueVidros = value;
                    RaisePropertyChanged("ControlarEstoqueVidros");
                }
            }
        }

        /// <summary>
        /// Observação para NFe.
        /// </summary>
        public string ObsNfe
        {
            get { return DataModel.ObsNfe; }
            set
            {
                if (DataModel.ObsNfe != value &&
                    RaisePropertyChanging("ObsNfe", value))
                {
                    DataModel.ObsNfe = value;
                    RaisePropertyChanged("ObsNfe");
                }
            }
        }

        /// <summary>
        /// Não enviar email ao cliente ao liberar pedidos?
        /// </summary>
        public bool NaoEnviarEmailLiberacao
        {
            get { return DataModel.NaoEnviarEmailLiberacao; }
            set
            {
                if (DataModel.NaoEnviarEmailLiberacao != value &&
                    RaisePropertyChanging("NaoEnviarEmailLiberacao", value))
                {
                    DataModel.NaoEnviarEmailLiberacao = value;
                    RaisePropertyChanged("NaoEnviarEmailLiberacao");
                }
            }
        }

        /// <summary>
        /// Gerar apenas OC de transferência para esse cliente?
        /// </summary>
        public bool SomenteOcTransferencia
        {
            get { return DataModel.SomenteOcTransferencia; }
            set
            {
                if (DataModel.SomenteOcTransferencia != value &&
                    RaisePropertyChanging("SomenteOcTransferencia", value))
                {
                    DataModel.SomenteOcTransferencia = value;
                    RaisePropertyChanged("SomenteOcTransferencia");
                }
            }
        }

        /// <summary>
        /// Observação ao liberar pedido.
        /// </summary>
        public string ObsLiberacao
        {
            get { return DataModel.ObsLiberacao; }
            set
            {
                if (DataModel.ObsLiberacao != value &&
                    RaisePropertyChanging("ObsLiberacao", value))
                {
                    DataModel.ObsLiberacao = value;
                    RaisePropertyChanged("ObsLiberacao");
                }
            }
        }

        /// <summary>
        /// Data de "validade" do cadastro do cliente.
        /// Inativa o cliente nessa data para nova atualização cadastral.
        /// </summary>
        public DateTime? DataLimiteCad
        {
            get { return DataModel.DataLimiteCad; }
            set
            {
                if (DataModel.DataLimiteCad != value &&
                    RaisePropertyChanging("DataLimiteCad", value))
                {
                    DataModel.DataLimiteCad = value;
                    RaisePropertyChanged("DataLimiteCad");
                }
            }
        }

        /// <summary>
        /// CNAE do cliente.
        /// </summary>
        public string Cnae
        {
            get { return DataModel.Cnae; }
            set
            {
                if (DataModel.Cnae != value &&
                    RaisePropertyChanging("Cnae", value))
                {
                    DataModel.Cnae = value;
                    RaisePropertyChanged("Cnae");
                }
            }
        }

        /// <summary>
        /// Código da rota a qual o cliente está associado.
        /// </summary>
        public int? IdRota
        {
            get
            {
                // Se o cliente for novo ou se o valor da propriedade
                // for alterado retorna a variável privada
                if (!ExistsInStorage || ChangedProperties.Contains("IdRota"))
                    return _idRota;

                else
                {
                    // Salva na variável privada a rota original do cliente
                    // (apenas a primeira vez)
                    if (_idRota == null && RotaCliente != null)
                        _idRota = RotaCliente.IdRota;


                    return _idRota;
                }
            }
            set
            {
                if (IdRota != value &&
                    RaisePropertyChanging("IdRota", value))
                {

                    if (!_rotaAnteriorCarregada)
                    {
                        _rotaAnteriorCarregada = true;
                        _rotaAnterior = Rota;
                    }

                    _idRota = value;
                    _rota = null;
                    RaisePropertyChanged("IdRota");
                }
            }
        }

        /// <summary>
        /// Indica que o cliente importa pedidos para o sistema.
        /// </summary>
        public bool Importacao
        {
            get { return DataModel.Importacao; }
            set
            {
                if (DataModel.Importacao != value &&
                    RaisePropertyChanging("Importacao", value))
                {
                    DataModel.Importacao = value;
                    RaisePropertyChanged("Importacao");
                }
            }
        }

        /// <summary>
        /// Código do atendente do cliente.
        /// </summary>
        public uint? IdFuncAtendente
        {
            get { return DataModel.IdFuncAtendente; }
            set
            {
                if (DataModel.IdFuncAtendente != value &&
                    RaisePropertyChanging("IdFuncAtendente", value))
                {
                    DataModel.IdFuncAtendente = value;
                    RaisePropertyChanged("IdFuncAtendente");
                }
            }
        }

        /// <summary>
        /// Descrição.
        /// </summary>
        public bool HabilitarEditorCad
        {
            get { return DataModel.HabilitarEditorCad; }
            set
            {
                if (DataModel.HabilitarEditorCad != value &&
                    RaisePropertyChanging("HabilitarEditorCad", value))
                {
                    DataModel.HabilitarEditorCad = value;
                    RaisePropertyChanged("HabilitarEditorCad");
                }
            }
        }

        /// <summary>
        /// Ids dos subgrupos vinculados ao cliente.
        /// </summary>
        public string IdsSubgrupoProd
        {
            get { return DataModel.IdsSubgrupoProd; }
            set
            {
                if (DataModel.IdsSubgrupoProd != value &&
                    RaisePropertyChanging("IdsSubgrupoProd", value))
                {
                    DataModel.IdsSubgrupoProd = value;
                    RaisePropertyChanged("IdsSubgrupoProd");
                }
            }
        }

        /// <summary>
        /// Ids dos subgrupos vinculados ao cliente.
        /// </summary>
        public float? DescontoEcommerce
        {
            get { return DataModel.DescontoEcommerce; }
            set
            {
                if (DataModel.DescontoEcommerce != value &&
                    RaisePropertyChanging("DescontoEcommerce", value))
                {
                    DataModel.DescontoEcommerce = value;
                    RaisePropertyChanged("DescontoEcommerce");
                }
            }
        }

        /// <summary>
        /// Define se os pedidos emitidos para esse cliente
        /// serão contabilizados no SMS de resumo diário
        /// </summary>
        public bool IgnorarNoSmsResumoDiario
        {
            get { return DataModel.IgnorarNoSmsResumoDiario; }
            set
            {
                if (DataModel.IgnorarNoSmsResumoDiario != value &&
                    RaisePropertyChanging("IgnorarNoSmsResumoDiario", value))
                {
                    DataModel.IgnorarNoSmsResumoDiario = value;
                    RaisePropertyChanged("IgnorarNoSmsResumoDiario");
                }
            }
        }

        /// <summary>
        /// Código do grupo do cliente.
        /// </summary>
        public int? IdGrupoCliente
        {
            get { return DataModel.IdGrupoCliente; }
            set
            {
                if (DataModel.IdGrupoCliente != value &&
                    RaisePropertyChanging("IdGrupoCliente", value))
                {
                    DataModel.IdGrupoCliente = value;
                    RaisePropertyChanged("IdGrupoCliente");
                }
            }
        }

        /// <summary>
        /// Obtém ou define o Percentual da bonificação do cliente.
        /// </summary>
        public decimal? PercentualBonificacao
        {
            get
            {
                return this.DataModel.PercentualBonificacao;
            }

            set
            {
                if (this.DataModel.PercentualBonificacao != value &&
                    this.RaisePropertyChanging("PercentualBonificacao", value))
                {
                    this.DataModel.PercentualBonificacao = value;
                    this.RaisePropertyChanged("PercentualBonificacao");
                }
            }
        }

        #endregion

        #region Propriedades referenciadas/filhos

        /// <summary>
        /// Descontos/acréscimos do cliente.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<DescontoAcrescimoCliente> DescontosAcrescimos
        {
            get
            {
                return IdTabelaDesconto.GetValueOrDefault() == 0 ? _descontosAcrescimos :
                    TabelaDescontoAcrescimo != null ? TabelaDescontoAcrescimo.DescontosAcrescimos :
                    null;
            }
        }

        /// <summary>
        /// Parcelas que o cliente não pode usar.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<Financeiro.Negocios.Entidades.ParcelasNaoUsar> Parcelas
        {
            get { return _parcelas; }
        }

        /// <summary>
        /// Formas de pagamento disponíveis para o cliente.
        /// </summary>
        public Colosoft.Business.IEntityChildrenList<Financeiro.Negocios.Entidades.FormaPagtoCliente> FormasPagto
        {
            get { return _formasPagto; }
        }

        /// <summary>
        /// Referência à tabela de desconto/acréscimos do cliente.
        /// </summary>
        public TabelaDescontoAcrescimoCliente TabelaDescontoAcrescimo
        {
            get { return GetReference<TabelaDescontoAcrescimoCliente>("TabelaDescontoAcrescimo", true); }
        }

        /// <summary>
        /// Referência ao comissionado do cliente.
        /// </summary>
        public Comissionado Comissionado
        {
            get { return GetReference<Comissionado>("Comissionado", true); }
        }

        /// <summary>
        /// Referência à cidade do cliente.
        /// </summary>
        public Cidade Cidade
        {
            get { return GetReference<Cidade>("Cidade", true); }
        }

        /// <summary>
        /// Referência à cidade de cobrança do cliente.
        /// </summary>
        public Cidade CidadeCobranca
        {
            get { return GetReference<Cidade>("CidadeCobranca", true); }
        }

        /// <summary>
        /// Referência à cidade de entrega do cliente.
        /// </summary>
        public Cidade CidadeEntrega
        {
            get { return GetReference<Cidade>("CidadeEntrega", true); }
        }

        /// <summary>
        /// Referência à rota do cliente.
        /// </summary>
        public RotaCliente RotaCliente
        {
            get { return GetReference<RotaCliente>("RotaCliente", true); }
        }

        /// <summary>
        /// Rota associada.
        /// </summary>
        public Rota Rota
        {
            get
            {
                if (_rota == null)
                    _rota = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<IProvedorRota>()
                        .ObtemRota(this);

                return _rota;
            }
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
        public Cliente()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Cliente(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.Cliente> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {
            _descontosAcrescimos = GetChild<DescontoAcrescimoCliente>(args.Children, "DescontosAcrescimos");
            _parcelas = GetChild<Financeiro.Negocios.Entidades.ParcelasNaoUsar>(args.Children, "Parcelas");
            _formasPagto = GetChild<Financeiro.Negocios.Entidades.FormaPagtoCliente>(args.Children, "FormasPagto");
        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Cliente(Data.Model.Cliente dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {
            _descontosAcrescimos = CreateChild<Colosoft.Business.IEntityChildrenList<DescontoAcrescimoCliente>>("DescontosAcrescimos");
            _parcelas = CreateChild<Colosoft.Business.IEntityChildrenList<Financeiro.Negocios.Entidades.ParcelasNaoUsar>>("Parcelas");
            _formasPagto = CreateChild<Colosoft.Business.IEntityChildrenList<Financeiro.Negocios.Entidades.FormaPagtoCliente>>("FormasPagto");
        }

        #endregion

        #region Métodos sobrescritos

        /// <summary>
        /// Sobrescreve o método de salvamento da entidade de cliente.
        /// </summary>
        public override Colosoft.Business.SaveResult Save(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorCliente>();

            // Executa a validação
            var resultadoValidacao = validador.ValidaAtualizacao(this);
            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.SaveResult(false, resultadoValidacao.Join(" "));

            #region Remove espaço rígido

            /* Chamado 33380.
             * Trata o caractere Espaço Rígido (0xA0 - Alt + 0 + 1 + 6 + 0 (teclado numérico)). */
            Nome = !string.IsNullOrEmpty(Nome) ? Nome.Replace(" ", "") : null;
            NomeFantasia = !string.IsNullOrEmpty(NomeFantasia) ? NomeFantasia.Replace(" ", "") : null;
            Endereco = !string.IsNullOrEmpty(Endereco) ? Endereco.Replace(" ", "") : null;
            Compl = !string.IsNullOrEmpty(Compl) ? Compl.Replace(" ", "") : null;
            Bairro = !string.IsNullOrEmpty(Bairro) ? Bairro.Replace(" ", "") : null;
            EnderecoCobranca = !string.IsNullOrEmpty(EnderecoCobranca) ? EnderecoCobranca.Replace(" ", "") : null;
            BairroCobranca = !string.IsNullOrEmpty(BairroCobranca) ? BairroCobranca.Replace(" ", "") : null;
            ComplCobranca = !string.IsNullOrEmpty(ComplCobranca) ? ComplCobranca.Replace(" ", "") : null;
            EnderecoEntrega = !string.IsNullOrEmpty(EnderecoEntrega) ? EnderecoEntrega.Replace(" ", "") : null;
            BairroEntrega = !string.IsNullOrEmpty(BairroEntrega) ? BairroEntrega.Replace(" ", "") : null;
            ComplEntrega = !string.IsNullOrEmpty(ComplEntrega) ? ComplEntrega.Replace(" ", "") : null;

            #endregion

            #region Limita o tamanho dos campos de endereço em 250

            /* Chamado 35083. */
            Endereco =
                !string.IsNullOrEmpty(Endereco) ?
                    (Endereco.Length > 250 ?
                        Endereco.Substring(0, 250) :
                        Endereco) :
                    null;
            EnderecoCobranca =
                !string.IsNullOrEmpty(EnderecoCobranca) ?
                    (EnderecoCobranca.Length > 250 ?
                        EnderecoCobranca.Substring(0, 250) :
                        EnderecoCobranca) :
                    null;
            EnderecoEntrega =
                !string.IsNullOrEmpty(EnderecoEntrega) ?
                    (EnderecoEntrega.Length > 250 ?
                        EnderecoEntrega.Substring(0, 250) :
                        EnderecoEntrega) :
                    null;

            #endregion

            if (DataLimiteCad.HasValue && DataLimiteCad.Value < DateTime.Now.Date)
                return new Colosoft.Business.SaveResult(false,
                    ("O cadastro do cliente expirou, a data limite é " + DataLimiteCad.Value.ToString("dd/MM/yyyy") +
                    ", para alterar os dados deste cliente altere a data limite do cadastro do mesmo.").GetFormatter());

            var login = Glass.Data.Helper.UserInfo.GetUserInfo;
            if (!Glass.Data.Helper.Config.PossuiPermissao(Glass.Data.Helper.Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) &&
                !Glass.Data.Helper.Config.PossuiPermissao(Glass.Data.Helper.Config.FuncaoMenuCadastro.AlterarLimiteCliente))
                IgnoreProperty("Limite");

            // Inclui as informações de alteração
            DataAlt = DateTime.Now;
            UsuAlt = (int)login.CodUser;

            // Não permite que o nome do cliente possua ' ou "
            Nome = Nome != null ? Nome.Replace("'", "").Replace("\"", "").Replace("\t", "").Replace("\n", "") : null;
            NomeFantasia = NomeFantasia != null ? NomeFantasia.Replace("'", "").Replace("\"", "").Replace("\t", "").Replace("\n", "") : null;

            // Não permite que seja inserido "INSENTO"
            if (RgEscinst != null && (RgEscinst.ToLower().Contains("insento") || RgEscinst.ToLower().Contains("insenta")))
                RgEscinst = RgEscinst.ToUpper().Replace("INSENTO", "ISENTO").Replace("INSENTA", "ISENTO");

            // Caso o percentual mínimo esteja zerado e o usuário não tenha permissão de alterá-lo, busca o que estiver configurado
            if (!ExistsInStorage && PercSinalMinimo.GetValueOrDefault(0) == 0 && Glass.Configuracoes.FinanceiroConfig.PercMinimoSinalPedidoPadrao > 0 &&
                !Glass.Data.Helper.Config.PossuiPermissao(Glass.Data.Helper.Config.FuncaoMenuCadastro.AlterarSinalMinimoCliente))
                PercSinalMinimo = Glass.Configuracoes.FinanceiroConfig.PercMinimoSinalPedidoPadrao;

            // Cópia o endereço do cliente para o endereço de entrega caso o usuário
            // não informe o endereço de entrega
            if (String.IsNullOrEmpty(EnderecoEntrega))
            {
                EnderecoEntrega = Endereco;

                if (IdCidade.HasValue)
                    IdCidadeEntrega = IdCidade.Value;

                BairroEntrega = Bairro;
                NumeroEntrega = Numero;
                ComplEntrega = Compl;
                CepEntrega = Cep;
            }

            // Variável para controle de salvamento da rota
            bool exists = ExistsInStorage;

            // Salva o cliente
            var resultado = base.Save(session);

            // Retorna em caso de erro
            if (!resultado)
                return resultado;

            // Apaga a rota se a propriedade for alterada para null
            // (apenas se o cliente já existir no banco de dados anteriormente)
            if (exists && !IdRota.HasValue && ChangedProperties.Contains("IdRota") && RotaCliente != null)
            {
                var res = RotaCliente.Delete(session);

                // Retorna em caso de erro
                if (!res)
                    return new Colosoft.Business.SaveResult(false, res.Message);
            }

            // Insere/atualiza a rota se a propriedade for alterada
            // ou se o cliente estiver sendo inserido
            if (IdRota.HasValue && (!exists || ChangedProperties.Contains("IdRota")))
            {
                // Salva a nova rota na entidade já existente
                if (RotaCliente != null && exists)
                {
                    RotaCliente.IdRota = IdRota.Value;
                    resultado = RotaCliente.Save(session);
                }

                // Insere a entidade de rota
                else
                {
                    var rota = new RotaCliente()
                    {
                        IdCliente = this.IdCli,
                        IdRota = this.IdRota.Value
                    };

                    resultado = rota.Save(session);
                }
            }

            if (ExistsInStorage && ChangedProperties.Contains("IdRota"))
            {
                var log = new Data.Model.LogAlteracao();
                log.IdLog = Colosoft.Business.EntityTypeManager.Instance.GenerateInstanceUid(typeof(Data.Model.LogAlteracao));
                log.IdRegistroAlt = IdCli;
                log.Tabela = (int)Data.Model.LogAlteracao.TabelaAlteracao.Cliente;
                log.NumEvento = Glass.Data.DAL.LogAlteracaoDAO.Instance.GetNumEvento(Data.Model.LogAlteracao.TabelaAlteracao.Cliente, IdCli);
                log.Campo = "Rota";
                log.ValorAnterior = _rotaAnteriorCarregada && _rotaAnterior != null ? string.Format("{0} : {1}", _rotaAnterior.CodInterno, _rotaAnterior.Descricao) : "";
                log.ValorAtual = Rota != null ? string.Format("{0} : {1}", Rota.CodInterno, Rota.Descricao) : "";
                log.DataAlt = DateTime.Now;
                log.IdFuncAlt = login.CodUser;

                session.Insert(log, (string[])null);
            }

            return resultado;
        }

        /// <summary>
        /// Sobrescreve o método de exclusão da entidade de cliente.
        /// </summary>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorCliente>();

            // Executa a validação
            var resultadoValidacao = validador.ValidaExistencia(this);
            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            // Apaga o registro do cliente na rota também
            if (IdRota.HasValue && RotaCliente != null)
            {
                var resultado = RotaCliente.Delete(session);

                if (!resultado)
                    return resultado;
            }

            return base.Delete(session);
        }

        #endregion
    }
}
