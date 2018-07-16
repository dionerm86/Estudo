using System;
using System.Threading;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.RelDAL;
using System.Web;
using Glass.Configuracoes;

namespace Glass.Data.Helper
{
    public sealed class Threads : Glass.Pool.Singleton<Threads>
    {
        private Threads() { }

        private const int Segundo = 1000;
        private const int Minuto = Segundo * 60;
        private const int Hora = Minuto * 60;
        private const int Dia = Hora * 24;

        #region Campos Privados

        private HttpContext _context;
        private Thread _emailSms;
        private Thread _bloquearCliente;
        private Thread _emailCobranca;
        private Thread _usoLimiteCliente;
        private Thread _limparLogErro = null;
        private Thread _coletaMensagem = null;

        #endregion

        #region Classe de Suporte

        private class ThreadHandler
        {
            public delegate void ThreadMethod();

            public readonly ThreadMethod Method;
            public readonly int SleepMilliseconds;
            public readonly bool ExitOnException;

            public ThreadHandler(ThreadMethod method, int sleepMilliseconds)
                : this(method, sleepMilliseconds, false)
            {
            }

            public ThreadHandler(ThreadMethod method, int sleepMilliseconds, bool exitOnException)
            {
                Method = method;
                SleepMilliseconds = sleepMilliseconds;
                ExitOnException = exitOnException;
            }
        }

        #endregion

        /// <summary>
        /// Inicia as threads de controle do sistema, se necessário.
        /// </summary>
        public void IniciarThreads(HttpContext context)
        {
            if (UserInfo.GetUserInfo == null || context == null || System.Configuration.ConfigurationManager.AppSettings["RodarThreads"] == "false")
                return;

            _context = context;
            bool isLocal = Utils.IsLocalUrl(_context);

            // Fila de e-mail e SMS
            // Deve entrar sempre, pois no caso da vidrocel, apesar de não enviar email quando o pedido estiver pronto,
            // estiver confirmado ou enviar sms, ela ainda envia email com o xml da nota
            if (!isLocal && (_emailSms == null || !_emailSms.IsAlive))
            {
                ThreadHandler handler = new ThreadHandler(ProcessaFilaEmailSms, Segundo * 30);
                _emailSms = new Thread(ThreadBase);
                _emailSms.Start(handler);
            }

            // Bloqueio de clientes
            if (!isLocal && (_bloquearCliente == null || !_bloquearCliente.IsAlive) &&
                (FinanceiroConfig.PeriodoInativarClienteUltimaCompra > 0 || PedidoConfig.NumeroDiasPedidoProntoAtrasado > 0))
            {
                ThreadHandler handler = new ThreadHandler(BloquearCliente, Hora * 4);
                _bloquearCliente = new Thread(ThreadBase);
                _bloquearCliente.Start(handler);
            }

            if ((_emailCobranca == null || !_emailCobranca.IsAlive) &&
                FinanceiroConfig.UsarControleCobrancaEmail)
            {
                ThreadHandler handler = new ThreadHandler(EmailCobranca, Hora * 8);
                _emailCobranca = new Thread(ThreadBase);
                _emailCobranca.Start(handler);
            }

            if (_usoLimiteCliente == null || !_usoLimiteCliente.IsAlive)
            {
                var handler = new ThreadHandler(AtualizarUsoLimiteCliente, Minuto * 20);
                _usoLimiteCliente = new Thread(ThreadBase);
                _usoLimiteCliente.Start(handler);
            }

            if (_limparLogErro == null || !_limparLogErro.IsAlive)
            {
                var handler = new ThreadHandler(LimpaLogErro, Dia);
                _limparLogErro = new Thread(ThreadBase);
                _limparLogErro.Start(handler);
            }

            if (!isLocal && (_coletaMensagem == null || !_coletaMensagem.IsAlive))
            {
                var handler = new ThreadHandler(ColetaMensagemErro, Minuto * 3);
                _coletaMensagem = new Thread(ThreadBase);
                _coletaMensagem.Start(handler);
            }
        }

        #region Método de execução das threads

        private void ThreadBase(object handler)
        {
            ThreadHandler dadosHandler = handler as ThreadHandler;
            if (dadosHandler == null || dadosHandler.Method == null)
                return;

            while (true)
            {
                try
                {
                    while (true)
                    {
                        Thread.Sleep(dadosHandler.SleepMilliseconds);
                        dadosHandler.Method.Invoke();
                    }
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    ErroDAO.Instance.InserirFromException(dadosHandler.Method.Method.Name, ex);
                    if (dadosHandler.ExitOnException)
                        break;
                }
            }
        }

        #endregion

        #region Fila de e-mail e SMS

        private void ProcessaFilaEmailSms()
        {
            var lstDataEnvioSmsAdmin = PCPConfig.EmailSMS.HorariosEnvioEmailSmsAdmin;

            try
            {
                // Envio de e-mail para administradores
                if (PCPConfig.EmailSMS.EnviarEmailAdministrador)
                {
                    foreach (DateTime d in lstDataEnvioSmsAdmin)
                        if (DateTime.Now.Hour == d.Hour && DateTime.Now.Minute >= d.Minute)
                            Email.EnviaEmailAdministradores();
                }

                FilaEmail email = FilaEmailDAO.Instance.GetNext();
                if (email != null)
                {
                    try
                    {
                        var anexos = AnexoEmailDAO.Instance.GetByEmail(email.IdEmail);

                        Email.EnviaEmail(_context, email.IdLoja, email.EmailDestinatario, email.Assunto, email.Mensagem,
                            email.EmailEnvio, anexos != null && anexos.Length > 0 ? anexos : null);

                        FilaEmailDAO.Instance.IndicaEnvio(email.IdEmail);

                    }
                    catch (Exception ex)
                    {
                        ErroDAO.Instance.InserirFromException("ProcessaEmail (1)", ex);

                        FilaEmailDAO.Instance.SetLast(email.IdEmail);
                    }
                }
            }
            catch (Exception ex)
            {
                // Salva a exceção de envio de email aqui, pois caso ocorra algum erro ao enviar o email de administradores por exemplo,
                // não impeça de enviar o sms para os administradores.
                ErroDAO.Instance.InserirFromException(
                    string.Format("ProcessaFilaEmail - Message: {0} - InnerException: {1} - InnerExceptionMessage: {2}",
                        ex.Message, ex.InnerException != null ? ex.InnerException.ToString() : string.Empty,
                        ex.InnerException != null ? ex.InnerException.Message : string.Empty), ex);
            }

            try
            {
                // Envio de SMS para administradores
                if (PCPConfig.EmailSMS.EnviarSMSAdministrador)
                {
                    foreach (DateTime d in lstDataEnvioSmsAdmin)
                        if (DateTime.Now.Hour == d.Hour && DateTime.Now.Minute >= d.Minute)
                            SMS.EnviaSMSAdministradores();
                }

                if (PCPConfig.EmailSMS.EnviarSMSPedidoPronto || PCPConfig.EmailSMS.EnviarSMSAdministrador)
                {
                    FilaSms sms = FilaSmsDAO.Instance.GetNext(!PCPConfig.EmailSMS.EnviarSMSPedidoPronto);
                    if (sms != null)
                    {
                        try
                        {
                            // Chamado 14031
                            System.Net.ServicePointManager.Expect100Continue = false;

                            if (System.Configuration.ConfigurationManager.AppSettings["EnvioNovoSMS"] == "true")
                            {
                                var resposta = ZenviaSMS.EnviaSMS(sms.CodMensagem, sms.NomeLoja, sms.CelCliente, sms.Mensagem);
                                FilaSmsDAO.Instance.IndicaEnvio(resposta.Response.StatusCode == 0, sms.IdSms, resposta.Response.StatusCode, resposta.Response.DetailDescription);
                            }
                            else
                            {
                                SMSSend.responseSendMessage resposta = SMS.EnviaSMSOld(sms.CodMensagem, sms.NomeLoja, sms.CelCliente, sms.Mensagem);
                                FilaSmsDAO.Instance.IndicaEnvio(true, sms.IdSms, resposta.result, resposta.resultDesc);
                            }
                        }
                        catch (Exception ex)
                        {
                            FilaSmsDAO.Instance.SetLast(sms.IdSms);
                            ErroDAO.Instance.InserirFromException("EnviarSMS", ex);
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Salva a exceção de envio de SMS aqui, pois caso ocorra algum erro ao enviar o SMS de administradores por exemplo,
                // a informação não se perca
                ErroDAO.Instance.InserirFromException(
                    string.Format("ProcessaFilaSMS - Message: {0} - InnerException: {1} - InnerExceptionMessage: {2}",
                        ex.Message, ex.InnerException != null ? ex.InnerException.ToString() : string.Empty,
                        ex.InnerException != null ? ex.InnerException.Message : string.Empty), ex);
            }
        }

        #endregion

        #region Otimização do banco de dados

        private void OtimizaBancoDados()
        {
            if (DateTime.Now.Hour > 18 && !OtimizacaoBancoDAO.Instance.Otimizou(DateTime.Now))
            {
                // Indica que o banco foi otimizado
                OtimizacaoBancoDAO.Instance.OtimizaBanco();

                // Indica que o banco foi otimizado
                OtimizacaoBancoDAO.Instance.Otimizado();
            }
        }

        #endregion

        #region Bloquear clientes

        private void BloquearCliente()
        {
            try
            {
                // Inativa os clientes que não fazem compras há um tempo (definido no config)
                ClienteDAO.Instance.InativarPelaUltimaCompra();
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("InativarPelaUltimaCompra", ex);
            }

            try
            {
                //Inativa os clientes que não foram pesquisados no sintegra há um tempo (definido no config)
                ClienteDAO.Instance.InativaPelaUltConSintegra();
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("InativaPelaUltConSintegra", ex);
            }

            try
            {
                //Inativa os clientes pela data limite do cadastro
                ClienteDAO.Instance.InativaPelaDataLimiteCad();
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("InativaPelaDataLimiteCad", ex);
            }

            try
            {
                // Bloqueia os clientes que tem pedidos prontos não liberados há um tempo (definido no config)
                if (!PedidoConfig.LiberarPedido)
                    ClienteDAO.Instance.BloquearProntosNaoLiberados();
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("BloquearProntosNaoLiberados", ex);
            }
        }

        #endregion

        #region Email de cobrança

        private void EmailCobranca()
        {
            // Só envia no horário comercial
            while (DateTime.Now.Hour < 8 || DateTime.Now.Hour > 18)
                Thread.Sleep(Hora * 1);

            //Envia email para os clientes que possuem contas vencidas ou a vencer
            //de acordo com o numero de dias definidos no config
            EmailCobrancaDAO.Instance.EnviaEmailCobranca();
        }

        #endregion

        #region Atualizar UsoLimiteCliente

        private void AtualizarUsoLimiteCliente()
        {
            try
            {
                if (DateTime.Now.Hour == 0)
                    ClienteDAO.Instance.AtualizarUsoLimiteCliente();
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("AtualizarUsoLimiteCliente", ex);
            }
        }

        #endregion

        #region Limpeza log erros

        public void LimpaLogErro()
        {
            ErroDAO.Instance.LimpaLogErro();
        }

        #endregion

        #region Coleta Erros

        private void ColetaMensagemErro()
        {
            try
            {
                // Recupera as mensagens que ainda não foram enviadas
                var mensagens = ErroDAO.Instance.ObtemParaEnvio();

                // Envia as mensagens
                new Sync.Utils.StatusSistema.StatusSistema().ColetaMensagemErro(
                    System.Configuration.ConfigurationManager.AppSettings["sistema"],
                    System.Configuration.ConfigurationManager.AppSettings["SiteSuporte"],
                    mensagens);

                // Marca como lidas
                ErroDAO.Instance.MarcaEnviado(mensagens);
            }
            catch
            {

            }
        }

        #endregion
    }
}