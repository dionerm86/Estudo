using Colosoft;
using System;
using System.Linq;
using Colosoft.Business;
using Colosoft.Data;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador da loja.
    /// </summary>
    public interface IValidadorLoja
    {
        /// <summary>
        /// Valida a existencia da loja.
        /// </summary>
        /// <param name="loja"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExistencia(Loja loja);
    }

    /// <summary>
    /// Representa a entidade de negócio da loja.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(LojaLoader))]
    [Glass.Negocios.ControleAlteracao(Data.Model.LogAlteracao.TabelaAlteracao.Loja)]
    public class Loja : Glass.Negocios.Entidades.EntidadeBaseCadastro<Data.Model.Loja>
    {
        #region Tipos aninhados

        class LojaLoader : Colosoft.Business.EntityLoader<Loja, Data.Model.Loja>
        {
            public LojaLoader()
            {
                Configure()
                    .Uid(f => f.IdLoja)
                    .FindName(new LojaFindNameConverter(), f => f.RazaoSocial, f => f.NomeFantasia)
                    .Reference<Cidade, Data.Model.Cidade>("Cidade", f => f.Cidade, f => f.IdCidade)
                    .Creator(f => new Loja(f));
            }

            class LojaFindNameConverter : Colosoft.IFindNameConverter
            {
                /// <summary>
                /// Converte os valores para o nome da loja.
                /// </summary>
                /// <param name="baseInfo"></param>
                /// <returns></returns>
                public string Convert(object[] baseInfo)
                {
                    var razaoSocial = (string)baseInfo[0];
                    var nomeFantasia = (string)baseInfo[1];

                    return nomeFantasia ?? razaoSocial;
                }
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Código da loja.
        /// </summary>
        public int IdLoja
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
        /// Código da cidade da loja.
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
        /// Razão Social da loja.
        /// </summary>
        public string RazaoSocial
        {
            get { return DataModel.RazaoSocial; }
            set
            {
                if (DataModel.RazaoSocial != value &&
                    RaisePropertyChanging("RazaoSocial", value))
                {
                    DataModel.RazaoSocial = value;
                    RaisePropertyChanged("RazaoSocial");
                }
            }
        }

        /// <summary>
        /// Nome Fantasia da loja.
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
        /// Situação da loja.
        /// </summary>
        public Situacao Situacao
        {
            get { return (Situacao)DataModel.Situacao; }
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
        /// Define se a loja bloqueia itens de cor e espessura diferentes no pedido.
        /// </summary>
        public bool IgnorarBloquearItensCorEspessura    
        {
            get { return DataModel.IgnorarBloquearItensCorEspessura; }
            set
            {
                if (DataModel.IgnorarBloquearItensCorEspessura != value &&
                    RaisePropertyChanging("IgnorarBloquearItensCorEspessura", value))
                {
                    DataModel.IgnorarBloquearItensCorEspessura = value;
                    RaisePropertyChanged("IgnorarBloquearItensCorEspessura");
                }
            }
        }

        /// <summary>
        /// Define se a loja só permite liberação de pedidos prontos
        /// </summary>
        public bool IgnorarLiberarApenasProdutosProntos
        {
            get { return DataModel.IgnorarLiberarApenasProdutosProntos; }
            set
            {
                if (DataModel.IgnorarLiberarApenasProdutosProntos != value &&
                    RaisePropertyChanging("IgnorarLiberarApenasProdutosProntos", value))
                {
                    DataModel.IgnorarLiberarApenasProdutosProntos = value;
                    RaisePropertyChanged("IgnorarLiberarApenasProdutosProntos");
                }
            }
        }

        /// <summary>
        /// Tipo de loja.
        /// </summary>
        public Data.Model.TipoLoja? Tipo
        {
            get { return (Data.Model.TipoLoja?)DataModel.Tipo; }
            set
            {
                if (DataModel.Tipo != value &&
                    RaisePropertyChanging("Tipo", value))
                {
                    DataModel.Tipo = value;
                    RaisePropertyChanged("Tipo");
                }
            }
        }

        /// <summary>
        /// Logradouro no endereço da loja.
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
        /// Número no endereço da loja.
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
        /// Complemento no endereço da loja.
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
        /// Bairro no endereço da loja.
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
        /// CEP no endereço da loja.
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
        /// E-mail / endereço eletrônico da loja.
        /// </summary>
        public string Site
        {
            get { return DataModel.Site; }
            set
            {
                if (DataModel.Site != value &&
                    RaisePropertyChanging("Site", value))
                {
                    DataModel.Site = value;
                    RaisePropertyChanged("Site");
                }
            }
        }

        /// <summary>
        /// Telefone (1) da loja.
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
        /// Telefone (2) da loja.
        /// </summary>
        public string Telefone2
        {
            get { return DataModel.Telefone2; }
            set
            {
                if (DataModel.Telefone2 != value &&
                    RaisePropertyChanging("Telefone2", value))
                {
                    DataModel.Telefone2 = value;
                    RaisePropertyChanged("Telefone2");
                }
            }
        }

        /// <summary>
        /// Fax da loja.
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
        /// CNPJ da loja.
        /// </summary>
        public string Cnpj
        {
            get { return DataModel.Cnpj; }
            set
            {
                if (DataModel.Cnpj != value &&
                    RaisePropertyChanging("Cnpj", value))
                {
                    DataModel.Cnpj = value;
                    RaisePropertyChanged("Cnpj");
                }
            }
        }

        /// <summary>
        /// Inscrição Estadual da loja.
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
        /// Inscrição Municipal da loja.
        /// </summary>
        public string InscMunic
        {
            get { return DataModel.InscMunic; }
            set
            {
                if (DataModel.InscMunic != value &&
                    RaisePropertyChanging("InscMunic", value))
                {
                    DataModel.InscMunic = value;
                    RaisePropertyChanged("InscMunic");
                }
            }
        }

        /// <summary>
        /// CNAE da loja.
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
        /// Número de inscrição na SUFRAMA da loja.
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
        /// CRT da loja.
        /// </summary>
        public Data.Model.CrtLoja? Crt
        {
            get { return (Data.Model.CrtLoja?)DataModel.Crt; }
            set
            {
                if (DataModel.Crt != (int?)value &&
                    RaisePropertyChanging("Crt", value))
                {
                    DataModel.Crt = (int?)value;
                    RaisePropertyChanged("Crt");
                }
            }
        }

        /// <summary>
        /// Senha do certificado da loja.
        /// </summary>
        public string SenhaCert
        {
            get { return DataModel.SenhaCert; }
            set
            {
                if (DataModel.SenhaCert != value &&
                    RaisePropertyChanging("SenhaCert", value))
                {
                    DataModel.SenhaCert = value;
                    RaisePropertyChanged("SenhaCert");
                }
            }
        }

        /// <summary>
        /// E-mail fiscal da loja.
        /// </summary>
        public string EmailFiscal
        {
            get { return DataModel.EmailFiscal; }
            set
            {
                if (DataModel.EmailFiscal != value &&
                    RaisePropertyChanging("EmailFiscal", value))
                {
                    DataModel.EmailFiscal = value;
                    RaisePropertyChanged("EmailFiscal");
                }
            }
        }

        /// <summary>
        /// Login de acesso ao e-mail fiscal da loja.
        /// </summary>
        public string LoginEmailFiscal
        {
            get { return DataModel.LoginEmailFiscal; }
            set
            {
                if (DataModel.LoginEmailFiscal != value &&
                    RaisePropertyChanging("LoginEmailFiscal", value))
                {
                    DataModel.LoginEmailFiscal = value;
                    RaisePropertyChanged("LoginEmailFiscal");
                }
            }
        }

        /// <summary>
        /// Senha de acesso ao e-mail fiscal da loja.
        /// </summary>
        public string SenhaEmailFiscal
        {
            get { return DataModel.SenhaEmailFiscal; }
            set
            {
                if (DataModel.SenhaEmailFiscal != value &&
                    RaisePropertyChanging("SenhaEmailFiscal", value))
                {
                    DataModel.SenhaEmailFiscal = value;
                    RaisePropertyChanged("SenhaEmailFiscal");
                }
            }
        }

        /// <summary>
        /// Servidor SMTP do e-mail fiscal da loja.
        /// </summary>
        public string ServidorEmailFiscal
        {
            get { return DataModel.ServidorEmailFiscal; }
            set
            {
                if (DataModel.ServidorEmailFiscal != value &&
                    RaisePropertyChanging("ServidorEmailFiscal", value))
                {
                    DataModel.ServidorEmailFiscal = value;
                    RaisePropertyChanged("ServidorEmailFiscal");
                }
            }
        }

        /// <summary>
        /// E-mail de contato da loja.
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
        /// Login de acesso ao e-mail de contato da loja.
        /// </summary>
        public string LoginEmailContato
        {
            get { return DataModel.LoginEmailContato; }
            set
            {
                if (DataModel.LoginEmailContato != value &&
                    RaisePropertyChanging("LoginEmailContato", value))
                {
                    DataModel.LoginEmailContato = value;
                    RaisePropertyChanged("LoginEmailContato");
                }
            }
        }

        /// <summary>
        /// Senha de acesso ao e-mail de contato da loja.
        /// </summary>
        public string SenhaEmailContato
        {
            get { return DataModel.SenhaEmailContato; }
            set
            {
                if (DataModel.SenhaEmailContato != value &&
                    RaisePropertyChanging("SenhaEmailContato", value))
                {
                    DataModel.SenhaEmailContato = value;
                    RaisePropertyChanged("SenhaEmailContato");
                }
            }
        }

        /// <summary>
        /// Servidor SMTP do e-mail de contato da loja.
        /// </summary>
        public string ServidorEmailContato
        {
            get { return DataModel.ServidorEmailContato; }
            set
            {
                if (DataModel.ServidorEmailContato != value &&
                    RaisePropertyChanging("ServidorEmailContato", value))
                {
                    DataModel.ServidorEmailContato = value;
                    RaisePropertyChanged("ServidorEmailContato");
                }
            }
        }

        /// <summary>
        /// E-mail comercial da loja.
        /// </summary>
        public string EmailComercial
        {
            get { return DataModel.EmailComercial; }
            set
            {
                if (DataModel.EmailComercial != value &&
                    RaisePropertyChanging("EmailComercial", value))
                {
                    DataModel.EmailComercial = value;
                    RaisePropertyChanged("EmailComercial");
                }
            }
        }

        /// <summary>
        /// Login de acesso ao e-mail comercial da loja.
        /// </summary>
        public string LoginEmailComercial
        {
            get { return DataModel.LoginEmailComercial; }
            set
            {
                if (DataModel.LoginEmailComercial != value &&
                    RaisePropertyChanging("LoginEmailComercial", value))
                {
                    DataModel.LoginEmailComercial = value;
                    RaisePropertyChanged("LoginEmailComercial");
                }
            }
        }

        /// <summary>
        /// Senha de acesso ao e-mail comercial da loja.
        /// </summary>
        public string SenhaEmailComercial
        {
            get { return DataModel.SenhaEmailComercial; }
            set
            {
                if (DataModel.SenhaEmailComercial != value &&
                    RaisePropertyChanging("SenhaEmailComercial", value))
                {
                    DataModel.SenhaEmailComercial = value;
                    RaisePropertyChanged("SenhaEmailComercial");
                }
            }
        }

        /// <summary>
        /// Servidor SMTP do e-mail comercial da loja.
        /// </summary>
        public string ServidorEmailComercial
        {
            get { return DataModel.ServidorEmailComercial; }
            set
            {
                if (DataModel.ServidorEmailComercial != value &&
                    RaisePropertyChanging("ServidorEmailComercial", value))
                {
                    DataModel.ServidorEmailComercial = value;
                    RaisePropertyChanged("ServidorEmailComercial");
                }
            }
        }

        /// <summary>
        /// Tipo de regime de atividade da loja.
        /// </summary>
        public Sync.Fiscal.Enumeracao.Loja.RegimeLoja? RegimeLoja
        {
            get { return (Sync.Fiscal.Enumeracao.Loja.RegimeLoja?)DataModel.RegimeLoja; }
            set
            {
                if (DataModel.RegimeLoja != (int?)value &&
                    RaisePropertyChanging("RegimeLoja", value))
                {
                    DataModel.RegimeLoja = (int?)value;
                    RaisePropertyChanged("RegimeLoja");
                }
            }
        }

        /// <summary>
        /// Tipo de atividade da loja.
        /// </summary>
        public Sync.Fiscal.Enumeracao.Loja.TipoAtividadeContribuicoes? TipoAtividade
        {
            get { return (Sync.Fiscal.Enumeracao.Loja.TipoAtividadeContribuicoes?)DataModel.TipoAtividade; }
            set
            {
                if (DataModel.TipoAtividade != (int?)value &&
                    RaisePropertyChanging("TipoAtividade", value))
                {
                    DataModel.TipoAtividade = (int?)value;
                    RaisePropertyChanged("TipoAtividade");
                }
            }
        }

        /// <summary>
        /// Data de vencimento do certificado da loja.
        /// </summary>
        public DateTime? DataVencimentoCertificado
        {
            get { return DataModel.DataVencimentoCertificado; }
            set
            {
                if (DataModel.DataVencimentoCertificado != value &&
                    RaisePropertyChanging("DataVencimentoCertificado", value))
                {
                    DataModel.DataVencimentoCertificado = value;
                    RaisePropertyChanged("DataVencimentoCertificado");
                }
            }
        }

        /// <summary>
        /// Número de inscrição na Junta Comercial.
        /// </summary>
        public string NIRE
        {
            get { return DataModel.NIRE; }
            set
            {
                if (DataModel.NIRE != value &&
                    RaisePropertyChanging("NIRE", value))
                {
                    DataModel.NIRE = value;
                    RaisePropertyChanged("NIRE");
                }
            }
        }

        /// <summary>
        /// Data de inscrição na Junta Comercial.
        /// </summary>
        public DateTime? DataNIRE
        {
            get { return DataModel.DataNIRE; }
            set
            {
                if (DataModel.DataNIRE != value &&
                    RaisePropertyChanging("DataNIRE", value))
                {
                    DataModel.DataNIRE = value;
                    RaisePropertyChanged("DataNIRE");
                }
            }
        }

        /// <summary>
        /// Número no Registro Nacional de Transporte Rodoviário de Cargas.
        /// </summary>
        public string RNTRC
        {
            get { return DataModel.RNTRC; }
            set
            {
                if (DataModel.RNTRC != value &&
                    RaisePropertyChanging("RNTRC", value))
                {
                    DataModel.RNTRC = value;
                    RaisePropertyChanged("RNTRC");
                }
            }
        }

        /// <summary>
        /// Identificador do Codigo de Segurança do Contribuinte NFC-e.
        /// </summary>
        public string IdCsc
        {
            get { return DataModel.IdCsc; }
            set
            {
                if (DataModel.IdCsc != value &&
                    RaisePropertyChanging("IdCsc", value))
                {
                    DataModel.IdCsc = value;
                    RaisePropertyChanged("IdCsc");
                }
            }
        }

        /// <summary>
        /// Codigo de Segurança do Contribuinte NFC-e.
        /// </summary>
        public string Csc
        {
            get { return DataModel.Csc; }
            set
            {
                if (DataModel.Csc != value &&
                    RaisePropertyChanging("Csc", value))
                {
                    DataModel.Csc = value;
                    RaisePropertyChanged("Csc");
                }
            }
        }

        /// <summary>
        /// Indica se o icms do pedido será calculado
        /// </summary>
        public bool CalcularIcmsPedido
        {
            get { return DataModel.CalcularIcmsPedido; }
            set
            {
                if (DataModel.CalcularIcmsPedido != value &&
                    RaisePropertyChanging("CalcularIcmsPedido", value))
                {
                    DataModel.CalcularIcmsPedido = value;
                    RaisePropertyChanged("CalcularIcmsPedido");
                }
            }
        }

        /// <summary>
        /// Indica se o ipi do pedido será calculado
        /// </summary>
        public bool CalcularIpiPedido
        {
            get { return DataModel.CalcularIpiPedido; }
            set
            {
                if (DataModel.CalcularIpiPedido != value &&
                    RaisePropertyChanging("CalcularIpiPedido", value))
                {
                    DataModel.CalcularIpiPedido = value;
                    RaisePropertyChanged("CalcularIpiPedido");
                }
            }
        }

        /// <summary>
        /// Indica se o icms da liberação será calculado
        /// </summary>
        public bool CalcularIcmsLiberacao
        {
            get { return DataModel.CalcularIcmsLiberacao; }
            set
            {
                if (DataModel.CalcularIcmsLiberacao != value &&
                    RaisePropertyChanging("CalcularIcmsLiberacao", value))
                {
                    DataModel.CalcularIcmsLiberacao = value;
                    RaisePropertyChanged("CalcularIcmsLiberacao");
                }
            }
        }

        /// <summary>
        /// Indica se o ipi da liberação será calculado
        /// </summary>
        public bool CalcularIpiLiberacao
        {
            get { return DataModel.CalcularIpiLiberacao; }
            set
            {
                if (DataModel.CalcularIpiLiberacao != value &&
                    RaisePropertyChanging("CalcularIpiLiberacao", value))
                {
                    DataModel.CalcularIpiLiberacao = value;
                    RaisePropertyChanged("CalcularIpiLiberacao");
                }
            }
        }

        #endregion

        #region Propriedades referenciadas/filhos

        public Cidade Cidade
        {
            get { return GetReference<Cidade>("Cidade", true); }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Loja()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Loja(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.Loja> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Loja(Data.Model.Loja dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos Públicos

        public override SaveResult Save(IPersistenceSession session)
        {
            var retorno = base.Save(session);

            if (!retorno)
                return retorno;

            // Verifica se é um produto novo
            if (!ExistsInStorage)
            {
                var provedorProdutos = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance <Glass.Global.Negocios.Entidades.IProvedorProdutos>();

                // Cria os estoques que precisam ser inseridos
                foreach (var prod in provedorProdutos.ObtemProdutos())
                {
                    var produtoLoja = new Estoque.Negocios.Entidades.ProdutoLoja
                    {
                        IdProd = prod.Id,
                        IdLoja = IdLoja
                    };

                    ((Colosoft.Business.IConnectedEntity)produtoLoja).Connect(((Colosoft.Business.IConnectedEntity)this).SourceContext);

                    retorno = produtoLoja.Save(session);
                    if (!retorno)
                        return retorno;
                }
            }

            return new Colosoft.Business.SaveResult(true, null);
        }

        /// <summary>
        /// Método acionado para apagar loja.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override Colosoft.Business.DeleteResult Delete(Colosoft.Data.IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorLoja>();

            var resultadoValidacao = validador.ValidaExistencia(this);

            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
