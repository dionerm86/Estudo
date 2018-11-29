using System;
using System.Web;
using System.Web.Services;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.UI.Web
{
    [WebService(Namespace = "http://webglass.org/", Name="SyncService", Description="Serviço web para a comunicação entre cliente e fornecedor.")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class wsExportacaoPedido : System.Web.Services.WebService
    {
        #region Segurança

        private enum TipoErroAutenticacao
        {
            NaoEncontrado = 0,
            ExportacaoDesativada = -1,
            ImportacaoDesativada = -2,
            ClienteInativo = -3,
            FornecedorInativo = -4
        }

        /// <summary>
        /// Autentica o usuário
        /// </summary>
        /// <param name="cnpj">CNPJ do usuário</param>
        /// <param name="tipoUsuario">1 cliente, 2 fornecedor</param>
        /// <returns>bool</returns>
        [WebMethod(Description = "Autentica o usuário no serviço.")]
        public int Autenticar(string cnpj, int tipoUsuario)
        {
            int ret = 0;

            switch (tipoUsuario)
            {
                case 1 :
                    Cliente cliente = ClienteDAO.Instance.GetByCpfCnpj(cnpj);
                    if (cliente != null)
                    {
                        if (cliente.Situacao != (int)SituacaoCliente.Ativo)
                            ret = (int)TipoErroAutenticacao.ClienteInativo;
                        else
                            ret = (int)cliente.IdCli;
                    }
                    else
                        ret = (int)TipoErroAutenticacao.NaoEncontrado;

                    break;

                case 2:
                    string fornec = FornecedorDAO.Instance.GetFornecedorByCPFCNPJ(cnpj);
                    ret = !String.IsNullOrEmpty(fornec) ? Glass.Conversoes.StrParaInt(fornec) : (int)TipoErroAutenticacao.NaoEncontrado;

                    break;
            }

            return ret;
        }

        private string GetDescrErroAutenticacao(int codErro)
        {
            string erro = "Acesso Negado para uso do serviço de exportação.\n";
            switch ((TipoErroAutenticacao)codErro)
            {
                case TipoErroAutenticacao.NaoEncontrado:
                    erro += "Cliente não cadastrado no fornecedor.";
                    break;

                case TipoErroAutenticacao.ClienteInativo:
                    erro += "Cliente Inativo no fornecedor";
                    break;

                case TipoErroAutenticacao.FornecedorInativo:
                    erro += "Fornecedor Inativo";
                    break;

                case TipoErroAutenticacao.ExportacaoDesativada:
                    erro += "Exportação desativada no sistema";
                    break;

                case TipoErroAutenticacao.ImportacaoDesativada:
                    erro += "Importação desativada no Fornecedor";
                    break;

                default:
                    erro += "Erro não especificado. Cód.: " + codErro;
                    break;
            }

            return erro;
        }

        private string GetLoginStatus()
        {
            if(HttpContext.Current.User.Identity.IsAuthenticated)
                return "logado";
            else
                return "não logado";
        }

        #endregion

        public wsExportacaoPedido()
        {
            //Uncomment the following line if using designed components
            //InitializeComponent();
        }

        /// <summary>
        /// Confirma se a conexão foi efetuada com sucesso.
        /// </summary>
        /// <returns>string</returns>
        [WebMethod(Description="Confirma se a conexão foi efetuada com sucesso.")]
        public string Conectar()
        {
            try
            {
                return "Conectado!";
            }
            catch (Exception ex)
            {
                return "Erro: " + ex.Message;
            }
        }

        [WebMethod(Description = "Autentica o usuário.")]
        public bool Login(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                System.Web.Security.FormsAuthentication.SetAuthCookie(userName, true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Exporta os pedidos enviados pelo cliente para a base de dados do fornecedor.
        /// </summary>
        /// <param name="cpfCnpj">Autenticação do cliente no serviço</param>
        /// <param name="pedido">Pedidos serializados</param>
        /// <returns>string[] com duas posições. A 1ª indica o código de retorno(0 = sucesso; 1 = erro) e a 2ª indica a mensagem de retorno</returns>
        [WebMethod(Description = "Exporta os pedidos enviados pelo cliente para a base de dados do fornecedor.")]
        public string[] EnviarPedidosFornecedor(string cpfCnpj, int tipoUsuario, byte[] pedido)
        {
            int aut;
            if ((aut = Autenticar(cpfCnpj, tipoUsuario)) > 0)
            {
                //Importa
                string[] resultado = UtilsExportacaoPedido.Importar(pedido);

                return resultado;
            }
            else
            {
                return new string[] { "1", GetDescrErroAutenticacao(aut) };
            }
        }

        [WebMethod(Description = "Atualiza a Situação do pedido exportado no cliente.")]
        public string[] CancelarPedido(string cpfCnpj, int tipoUsuario, uint idPedidoCliente)
        {
            int aut;
            if ((aut = Autenticar(cpfCnpj, tipoUsuario)) > 0)
            {
                try
                {
                    PedidoExportacaoDAO.Instance.AtualizarSituacao(null, idPedidoCliente, (int)PedidoExportacao.SituacaoExportacaoEnum.Cancelado);

                    return new string[] { "0", "Pedido " + idPedidoCliente + " cancelado com sucesso." };
                }
                catch (Exception ex)
                {
                    return new string[] { "1", "Ocorreu um erro: " + ex.Message + "." };
                }
            }
            else
            {
                return new string[] { "1", GetDescrErroAutenticacao(aut) };
            }
        }

        [WebMethod(Description = "Atualiza a Situação do pedido exportado no cliente.")]
        public string[] MarcarPedidoPronto(string cpfCnpj, int tipoUsuario, uint idPedidoCliente)
        {
            int aut;
            if ((aut = Autenticar(cpfCnpj, tipoUsuario)) > 0)
            {
                try
                {
                    PedidoExportacaoDAO.Instance.AtualizarSituacao(null, idPedidoCliente, (int)PedidoExportacao.SituacaoExportacaoEnum.Pronto);

                    return new string[] { "0", "Pedido " + idPedidoCliente + " foi marcado como pronto com sucesso." };
                }
                catch (Exception ex)
                {
                    return new string[] { "1", "Ocorreu um erro: " + ex.Message + "." };
                }
            }
            else
            {
                return new string[] { "1", GetDescrErroAutenticacao(aut) };
            }
        }

        /// <summary>
        /// Exporta os pedidos enviados pelo cliente para a base de dados do fornecedor.
        /// </summary>
        /// <param name="cpfCnpj">Autenticação do cliente no serviço</param>
        /// <param name="pedido">Pedidos serializados</param>
        /// <returns>string[] com duas posições. A 1ª indica o código de retorno(0 = sucesso; 1 = erro) e a 2ª indica a mensagem de retorno</returns>
        [WebMethod(Description = "Verfica a situação dos pedidos passados")]
        public string[] VerificarExportacaoPedidos(string cpfCnpj, int tipoUsuario, byte[] pedido)
        {
            int aut;
            if ((aut = Autenticar(cpfCnpj, tipoUsuario)) > 0)
            {
                string[] resultado = UtilsExportacaoPedido.VerificarExportacaoPedidos(pedido);

                return resultado;
            }
            else
            {
                return new string[] { "1", GetDescrErroAutenticacao(aut) };
            }
        }
    }
}

