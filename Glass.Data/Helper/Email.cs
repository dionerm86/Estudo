using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.IO;
using System.Web;
using Glass.Data.RelDAL;
using System.Linq;
using Glass.Configuracoes;
using GDA;

namespace Glass.Data.Helper
{
    public static class Email
    {
        #region Enumeradores

        public enum EmailEnvio
        {
            Fiscal,
            Comercial,
            Contato
        }

        #endregion

        /// <summary>
        /// Envia email para os clientes quando carregamento for finalizado
        /// </summary>
        public static void EnviaEmailCarregamentoFinalizado(GDASession sessao, uint idCarregamento)
        {
            var ocs = OrdemCargaDAO.Instance.GetOCsForCarregamento(sessao, idCarregamento);
            var idLoja = (UserInfo.GetUserInfo?.IdLoja).GetValueOrDefault();

            if (idLoja == 0)
            {
                throw new Exception("Não foi possível recuperar a loja do pedido ao salvar o e-mail a ser enviado.");
            }

            foreach (var idCli in ocs.Select(f => f.IdCliente).Distinct())
            {
                var email = ClienteDAO.Instance.GetEmail(sessao, idCli);

                if (string.IsNullOrEmpty(email))
                    continue;

                var ocsCli = ocs.Where(f => f.IdCliente == idCli).ToList();

                var pedidos = new List<Pedido>();

                foreach (var oc in ocsCli)
                    pedidos.AddRange(oc.Pedidos);

                pedidos = pedidos.Distinct().ToList();

                var pecas = pedidos.Sum(f => f.QtdePecasVidro);
                var volumes = pedidos.Sum(f => f.QtdeVolume);
                var totalM2 = Math.Round(pedidos.Sum(f => f.TotMOC), 2);
                var valorTotal = pedidos.Sum(f => f.Total);

                var peds = string.Join(", ", pedidos.OrderBy(f => f.IdPedido).Select(f => f.IdPedido.ToString() + (!string.IsNullOrEmpty(f.CodCliente) ? " - " + f.CodCliente : "")).ToArray());

                string texto = "Prezado(a) cliente,\n\nInformamos que seu(s) pedido(s) " + peds + " totalizando ";

                if (pecas > 0)
                    texto += pecas + " peças, " + totalM2 + " m², ";

                if (volumes > 0)
                    texto += volumes + " volumes, ";

                texto = texto.Trim().Trim(',');

                texto += " com um total de " + valorTotal.ToString("C") + " teve o seu processo de entrega iniciado.\n\nObrigado.\n" +
                    "SETOR EXPEDIÇÃO\n" + LojaDAO.Instance.GetElement(sessao, UserInfo.GetUserInfo.IdLoja).RazaoSocial;

                var anexos = new List<AnexoEmail>();

                foreach (var oc in ocsCli)
                    anexos.Add(new AnexoEmail("IdOC=" + oc.IdOrdemCarga, "OC_" + oc.IdOrdemCarga + ".pdf"));

                EnviaEmailAsync(sessao, idLoja, email, "Pedido em processo de entrega", texto, EmailEnvio.Comercial, false, anexos.ToArray());
            }
        }

        /// <summary>
        /// Envia e-mail de pedido pronto.
        /// </summary>
        public static void EnviaEmailPedidoPronto(GDASession session, uint idPedido)
        {
            try
            {
                if (idPedido == 0)
                    return;

                uint idCli = PedidoDAO.Instance.ObtemIdCliente(session, idPedido);

                if (idCli == 0)
                    return;

                // Verifica se o cliente não recebe e-mail
                if (ClienteDAO.Instance.NaoReceberEmailPedPronto(session, idCli))
                    return;

                string email = ClienteDAO.Instance.GetEmail(session, idCli);
                if (String.IsNullOrEmpty(email))
                    return;

                // Não envia email de pedidos prontos há mais de 2 dias
                DateTime? dataPronto = PedidoDAO.Instance.ObtemValorCampo<DateTime?>(session, "dataPronto", "idPedido=" + idPedido);
                if (dataPronto != null && dataPronto < DateTime.Now.AddDays(-2))
                    return;

                // Verifica se esse email já foi enviado
                if (FilaEmailDAO.Instance.EmailEnviado(session, "Pedido pronto", "(nosso número " + idPedido + ")"))
                    return;

                string codCliente = PedidoDAO.Instance.ObtemValorCampo<string>(session, "codCliente", "idPedido=" + idPedido);
                uint idLoja = PedidoDAO.Instance.ObtemIdLoja(session, idPedido);

                if (idLoja == 0)
                {
                    throw new Exception("Não foi possível recuperar a loja do pedido ao salvar o e-mail a ser enviado.");
                }

                var nomeLoja = LojaDAO.Instance.GetNome(session, idLoja);
                int tipoEntrega = PedidoDAO.Instance.ObtemTipoEntrega(session, idPedido);

                var texto = TextoEmailPedidoPronto((int)idPedido, codCliente, nomeLoja, (Pedido.TipoEntregaPedido)tipoEntrega);

                List<AnexoEmail> anexos = new List<AnexoEmail>();
                if (EmailConfig.EnviarPedidoAnexoEmail == DataSources.TipoEnvioAnexoPedidoEmail.Ambos ||
                    EmailConfig.EnviarPedidoAnexoEmail == DataSources.TipoEnvioAnexoPedidoEmail.Pronto)
                {
                    anexos.Add(PedidoEspelhoDAO.Instance.ExisteEspelho(session, idPedido)
                        ? new AnexoEmail("~/Relatorios/RelPedido.aspx?tipo=2&semThread=true&idPedido=" + idPedido,
                            "Pedido " + idPedido + ".pdf")
                        : new AnexoEmail("~/Relatorios/RelPedido.aspx?tipo=0&semThread=true&idPedido=" + idPedido,
                            "Pedido " + idPedido + ".pdf"));
                }

                EnviaEmailAsync(session, idLoja, email, "Pedido pronto", texto, EmailEnvio.Comercial, false, anexos.ToArray());
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("E-mail pedido Pronto", ex);
            }
        }

        /// <summary>
        /// Envia e-mail de pedido finalizado no PCP.
        /// </summary>
        public static void EnviaEmailPedidoPcp(GDASession sessao, uint idPedido)
        {
            try
            {
                if (idPedido == 0)
                    return;

                uint idCli = PedidoDAO.Instance.ObtemIdCliente(sessao, idPedido);

                if (idCli == 0)
                    return;

                // Verifica se o cliente não recebe e-mail
                if (ClienteDAO.Instance.NaoReceberEmailPedPcp(sessao, idCli))
                    return;

                string email = ClienteDAO.Instance.GetEmail(sessao, idCli);
                if (String.IsNullOrEmpty(email))
                    return;

                var codCliente = PedidoDAO.Instance.ObtemValorCampo<string>(sessao, "codCliente", "idPedido=" + idPedido);
                var idLoja = PedidoDAO.Instance.ObtemIdLoja(sessao, idPedido);

                if (idLoja == 0)
                {
                    throw new Exception("Não foi possível recuperar a loja do pedido ao salvar o e-mail a ser enviado.");
                }

                var nomeLoja = LojaDAO.Instance.GetNome(sessao, idLoja);
                var dataEntrega = PedidoDAO.Instance.ObtemDataEntrega(sessao, idPedido).GetValueOrDefault();

                var texto = TextoEmailPedidoFinalizadoPcp((int)idPedido, codCliente, nomeLoja, dataEntrega);

                List<AnexoEmail> anexos = new List<AnexoEmail>();
                if (EmailConfig.EnviarPedidoAnexoEmail == DataSources.TipoEnvioAnexoPedidoEmail.Ambos ||
                    EmailConfig.EnviarPedidoAnexoEmail == DataSources.TipoEnvioAnexoPedidoEmail.PCP)
                    anexos.Add(new AnexoEmail("~/Relatorios/RelPedido.aspx?tipo=2&semThread=true&idPedido=" + idPedido, "Pedido " + idPedido + ".pdf"));

                EnviaEmailAsync(sessao, idLoja, email, "Pedido confirmado", texto, EmailEnvio.Comercial, false, anexos.ToArray());
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("E-mail pedido Confirmado", ex);
            }
        }

        /// <summary>
        /// Envia e-mail de pedido finalizado no PCP para o vendedor.
        /// </summary>
        public static void EnviaEmailPedidoPcpVendedor(GDASession sessao, uint idPedido, string emailVendedor, string nomeVendedor)
        {
            try
            {
                if (String.IsNullOrEmpty(emailVendedor))
                    throw new Exception("e-mail do vendedor vazio.");

                var idCliente = PedidoDAO.Instance.ObtemValorCampo<uint>(sessao, "idcli", "idPedido=" + idPedido);
                var nomeCliente = ClienteDAO.Instance.GetNome(sessao, idCliente);
                var idLoja = PedidoDAO.Instance.ObtemIdLoja(sessao, idPedido);

                if (idLoja == 0)
                {
                    throw new Exception("Não foi possível recuperar a loja do pedido ao salvar o e-mail a ser enviado.");
                }

                string texto = nomeVendedor + " ,\n\nSegue em anexo o pedido "+ idPedido + " do seu cliente " +
                    idCliente.ToString() +" - " +  nomeCliente;

                var anexos = new List<AnexoEmail>
                {
                    new AnexoEmail("~/Relatorios/RelPedido.aspx?tipo=2&semThread=true&idPedido=" + idPedido,
                        "Pedido " + idPedido + ".pdf")
                };

                EnviaEmailAsync(sessao, idLoja, emailVendedor, "Pedido confirmado", texto, Email.EmailEnvio.Comercial, false, anexos.ToArray());
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("E-mail pedido Confirmado Vendedor", ex);
            }
        }

        /// <summary>
        /// Envia e-mail para administrador se houver desconto maior que o configurado.
        /// </summary>
        public static void EnviaEmailDescontoMaior(GDASession sessao, uint idPedido, uint idOrcamento, uint idFuncDesconto, float percentualDesconto,
            float percentualConfigurado)
        {
            uint? idAdminEmail = EmailConfig.AdministradorEnviarEmailDescontoMaior;
            if (!EmailConfig.EnviarEmailAdministradorDescontoMaior || idAdminEmail == null || idFuncDesconto == idAdminEmail)
                return;

            string email = FuncionarioDAO.Instance.ObtemValorCampo<string>(sessao, "email", "idFunc=" + idAdminEmail) ?? String.Empty;
            if (String.IsNullOrEmpty(email.Trim()))
                return;

            try
            {
                var idLoja = (UserInfo.GetUserInfo?.IdLoja).GetValueOrDefault();

                if (idLoja == 0)
                {
                    throw new Exception("Não foi possível recuperar a loja do pedido ao salvar o e-mail a ser enviado.");
                }

                string nomeAdmin = FuncionarioDAO.Instance.GetNome(sessao, idAdminEmail.Value);
                string nomeFuncDesconto = FuncionarioDAO.Instance.GetNome(sessao, idFuncDesconto);

                string tipo = idPedido > 0 ? "pedido" :
                    idOrcamento > 0 ? "orçamento" :
                    "(tipo desconhecido)";

                uint numero = idPedido > 0 ? idPedido :
                    idOrcamento > 0 ? idOrcamento :
                    0;

                string texto = "Prezado(a) " + nomeAdmin + ",\n\nInformamos que o " + tipo + " de número " + numero +
                    " possui um percentual de desconto " + percentualDesconto.ToString("0.###") + "%, que é maior que o percentual configurado (" +
                    percentualConfigurado.ToString("0.###") + "%).\nEsse desconto foi concedido pelo funcionário " + nomeFuncDesconto +
                    " no dia " + DateTime.Now.ToString("dd/MM/yyyy") + " às " + DateTime.Now.ToString("HH:mm:ss") + ".";

                EnviaEmailAsync(sessao, idLoja, email, "Desconto maior que o configurado", texto, Email.EmailEnvio.Comercial, false);
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("E-mail administrador desconto maior", ex);
            }
        }

        /// <summary>
        /// Envia e-mail de liberação de pedido.
        /// </summary>
        public static void EnviaEmailLiberacao(GDASession sessao, uint idLiberarPedido)
        {
            try
            {
                if (!PedidoConfig.LiberarPedido || !Liberacao.EnviarEmailAoLiberarPedido)
                    return;

                uint idCli = LiberarPedidoDAO.Instance.GetIdCliente(sessao, idLiberarPedido);

                // Verifica se o cliente não recebe e-mail
                if (ClienteDAO.Instance.NaoReceberEmailLiberacao(sessao, idCli))
                    return;

                bool pedidoEntrega = LiberarPedidoDAO.Instance.ExecuteScalar<int>(sessao, @"select count(*) from pedido
                    where idPedido in (select idPedido from produtos_liberar_pedido where idLiberarPedido=" + idLiberarPedido +
                    ") and tipoEntrega=" + DataSources.Instance.GetTipoEntregaEntrega().GetValueOrDefault()) > 0;

                if (!pedidoEntrega)
                    return;

                string email = ClienteDAO.Instance.GetEmail(sessao, idCli);
                if (String.IsNullOrEmpty(email))
                    return;

                uint idFunc = LiberarPedidoDAO.Instance.ObtemValorCampo<uint>(sessao, "idFunc", "idLiberarPedido=" + idLiberarPedido);
                uint idLoja = FuncionarioDAO.Instance.ObtemIdLoja(sessao, idFunc);

                if (idLoja == 0)
                {
                    throw new Exception("Não foi possível recuperar a loja do funcionário ao salvar o e-mail a ser enviado.");
                }

                string texto = "Prezado(a) cliente,\n\nInformamos que sua liberação de pedido " +
                    idLiberarPedido + " foi realizada.\nSegue anexo para conferência.";

                List<AnexoEmail> anexos = new List<AnexoEmail>
                {
                    new AnexoEmail(
                        "../Relatorios/RelLiberacao.aspx?semThread=true&EnvioEmail=true&idLiberarPedido=" +
                        idLiberarPedido, "Liberação de Pedido " + idLiberarPedido + ".pdf")
                };

                Email.EnviaEmailAsync(sessao, idLoja, email, "Liberação de pedido", texto, Email.EmailEnvio.Comercial, false, anexos.ToArray());
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("E-mail liberação pedido", ex);
            }
        }

        /// <summary>
        /// Envia e-mail do orçamento.
        /// </summary>
        public static void EnviaEmailOrcamento(GDASession sessao, uint idOrcamento)
        {
            try
            {
                var idCli = OrcamentoDAO.Instance.ObtemIdCliente(sessao, idOrcamento);

                var email = ClienteDAO.Instance.GetEmail(sessao, idCli.GetValueOrDefault(0));
                if (string.IsNullOrEmpty(email))
                    return;

                uint idFunc = OrcamentoDAO.Instance.ObtemValorCampo<uint>(sessao, "idFunc", "idOrcamento=" + idOrcamento);
                uint idLoja = FuncionarioDAO.Instance.ObtemIdLoja(sessao, idFunc);

                if (idLoja == 0)
                {
                    throw new Exception("Não foi possível recuperar a loja do funcionário ao salvar o e-mail a ser enviado.");
                }

                string texto = "Prezado(a) cliente, segue em anexo o seu orçamento para conferência.";

                List<AnexoEmail> anexos = new List<AnexoEmail>
                {
                    new AnexoEmail("../Relatorios/RelOrcamento.aspx?semThread=true&idOrca=" + idOrcamento, "Orçamento " + idOrcamento + ".pdf")
                };

                EnviaEmailAsync(sessao, idLoja, email, "Orçamento", texto, EmailEnvio.Comercial, false, anexos.ToArray());
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("E-mail orçamento", ex);
            }
        }

        /// <summary>
        /// /// Método utilizado para enviar email utilizando configurações de cada loja,
        /// colocando o e-mail na fila de envio.
        /// </summary>
        public static uint EnviaEmailAsync(uint idLoja, string emailDestinatario, string assunto, string mensagem, EmailEnvio emailEnvio, params AnexoEmail[] anexos)
        {
            return EnviaEmailAsyncComTransacao(idLoja, emailDestinatario, assunto, mensagem, emailEnvio, false, anexos);
        }

        /// <summary>
        /// Método utilizado para enviar email utilizando configurações de cada loja,
        /// colocando o e-mail na fila de envio.
        /// </summary>
        public static uint EnviaEmailAsyncComTransacao(uint idLoja, string emailDestinatario, string assunto, string mensagem, EmailEnvio emailEnvio, bool emailAdmin,
            params AnexoEmail[] anexos)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    uint idEmail = EnviaEmailAsync(transaction, idLoja, emailDestinatario, assunto, mensagem, emailEnvio, emailAdmin, anexos);

                    transaction.Commit();
                    transaction.Close();

                    return idEmail;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException("EnviaEmailAsync", ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Método utilizado para enviar email utilizando configurações de cada loja,
        /// colocando o e-mail na fila de envio.
        /// </summary>
        public static uint EnviaEmailAsync(GDASession session, uint idLoja, string emailDestinatario, string assunto, string mensagem, EmailEnvio emailEnvio, bool emailAdmin,
            params AnexoEmail[] anexos)
        {
            if (idLoja == 0)
                throw new Exception("Loja não informada.");

            if (string.IsNullOrEmpty(emailDestinatario))
                throw new Exception("Email do destinatário inválido.");

            var emailsDestinatario = emailDestinatario.Trim().Split(';').Where(f => !string.IsNullOrEmpty(f)).Select(f => f.Trim());

            uint idEmail = 0;
            foreach (var e in emailsDestinatario)
            {
                var email = new FilaEmail();
                email.IdLoja = idLoja;
                email.EmailDestinatario = e;
                email.Assunto = assunto;
                email.Mensagem = mensagem;
                email.EmailEnvio = emailEnvio;
                email.DataCad = DateTime.Now;
                email.EmailAdmin = emailAdmin;

                var contador = 0;

                /* Chamado 57290. */
                while (contador < 2) { try { idEmail = FilaEmailDAO.Instance.Insert(session, email); break; } catch { contador++; } };

                if (idEmail == 0)
                    idEmail = FilaEmailDAO.Instance.Insert(session, email);

                if (anexos != null && anexos.Length > 0)
                    foreach (AnexoEmail anexo in anexos)
                    {
                        anexo.IdEmail = idEmail;
                        AnexoEmailDAO.Instance.Insert(session, anexo);
                    }
            }

            return idEmail;
        }

        private static readonly object SyncRoot = new object();

        /// <summary>
        /// Método utilizado para enviar email utilizando configurações de cada loja,
        /// enviando o e-mail imediatamente.
        /// </summary>
        /// <returns>true - Email enviado, false - Falha no envio do email</returns>
        internal static void EnviaEmail(HttpContext context, uint idLoja, string emailDestinatario, string assunto, string mensagem,
            EmailEnvio emailEnvio, params AnexoEmail[] anexos)
        {
            if (idLoja == 0)
                throw new Exception("Loja não informada.");

            if (String.IsNullOrEmpty(emailDestinatario))
                throw new Exception("Email do destinatário inválido.");

            Loja loja = LojaDAO.Instance.GetElement(idLoja);

            string emailRemet = String.Empty;
            string servidorEmailRemet = String.Empty;
            string loginEmailRemet = String.Empty;
            string senhaEmailRemet = String.Empty;

            if (emailEnvio == EmailEnvio.Fiscal)
            {
                emailRemet = loja.EmailFiscal;
                servidorEmailRemet = loja.ServidorEmailFiscal;
                loginEmailRemet = loja.LoginEmailFiscal;
                senhaEmailRemet = loja.SenhaEmailFiscal;
            }
            else if (emailEnvio == EmailEnvio.Comercial)
            {
                emailRemet = loja.EmailComercial;
                servidorEmailRemet = loja.ServidorEmailComercial;
                loginEmailRemet = loja.LoginEmailComercial;
                senhaEmailRemet = loja.SenhaEmailComercial;
            }

            if (String.IsNullOrEmpty(emailRemet))
                throw new Exception("Email da loja inválido.");

            if (String.IsNullOrEmpty(loginEmailRemet))
                throw new Exception("O usuário do email da loja não foi informado.");

            if (String.IsNullOrEmpty(senhaEmailRemet))
                throw new Exception("A senha do email da loja não foi informado.");

            if (String.IsNullOrEmpty(servidorEmailRemet))
                throw new Exception("O servidor de email da loja não foi informado.");

            // Não envia o e-mail se não for possível identificar o contexto HTTP
            if (context == null)
                throw new Exception("HttpContext não identificado.");

            // Cria novo objeto MailMessage
            MailMessage mailMessage = new MailMessage {From = new MailAddress(emailRemet)};

            // Define o remetente

            // Define o endereço de resposta
            if (!String.IsNullOrEmpty(loja.EmailContato))
                mailMessage.ReplyToList.Add(new MailAddress(loja.EmailContato));

            // Define primeiro destinatário
            mailMessage.To.Add(emailDestinatario);

            // Define assunto do e-mail
            mailMessage.Subject = assunto;

            bool temPdf = false;

            //48546 - Devido a correção do chamado 13132
            //Ao fazer a remoção do item com a lista ainda em loop a proxima interação vai dar exceção pois o index é maior que a coleção
            var anexosRemover = new List<int>();

            // Anexa os arquivos
            if (anexos != null && anexos.Length > 0)
                for (int i = 0; i < anexos.Length; i++)
                {
                    anexos[i].GetDados = true;
                    anexos[i].Context = context;

                    mailMessage.Attachments.Add(new Attachment(new MemoryStream(anexos[i].Dados), anexos[i].NomeArquivo));

                    // Chamado 13132. Ocorreu um problema ao enviar o e-mail da liberação para o cliente que fez com que
                    // o anexo fosse enviado com o tamanho zerado. Por isso, incluímos a verificação abaixo que certifica
                    // que o anexo foi recuperado, e caso não seja será incluído um registro na tabela de erro.
                    if (mailMessage.Attachments[i] != null && mailMessage.Attachments[i].ContentStream != null && mailMessage.Attachments[i].ContentStream.Length == 0)
                    {
                        anexosRemover.Add(i);
                        ErroDAO.Instance.InserirFromException("Enviar e-mail.", new Exception("Falha ao adicionar o anexo " + anexos[i].NomeArquivo  + " ao e-mail."));
                    }

                    if (anexos[i] != null && anexos[i].NomeArquivo != null)
                        temPdf = temPdf || anexos[i].NomeArquivo.ToLower().Contains(".pdf");
                }

            //48546
            foreach (var index in anexosRemover)
                mailMessage.Attachments.RemoveAt(index);

            // Seta propriedade para enviar email em html como true(verdadeiro)
            // Apenas se houver PDF anexado
            mailMessage.IsBodyHtml = temPdf;

            // Seta o corpo do e-mail com a estrutura HTML gravada na stringbuilder sbBody
            mailMessage.Body = mensagem;
            if (temPdf)
            {
                mailMessage.Body += "\n\nEsse e-mail contém um arquivo PDF anexado. Para visualizar o arquivo obtenha o Adobe Reader através do endereço " +
                    "<a href=\"http://get.adobe.com/br/reader/\">http://get.adobe.com/br/reader/</a>.";
            }

            mailMessage.Body += "\n\n\nAtenção: Email gerado automaticamente via software WebGlass v" + Geral.ObtemVersao() + ".";

            if (String.IsNullOrEmpty(loja.EmailContato))
                mailMessage.Body += "\nFavor não responder esse e-mail.";

            if (mailMessage.IsBodyHtml)
                mailMessage.Body = mailMessage.Body.Replace("\n", "<br />");

            // Cria novo SmtpCliente e seta o endereço
            SmtpClient smtpClient = new SmtpClient(servidorEmailRemet)
            {
                Credentials = new NetworkCredential(loginEmailRemet, senhaEmailRemet)
            };

            // Credencial para envio por SMTP Seguro (APENAS QUANDO O SERVIDOR EXIGE AUTENTICAÇÃO)

            if (ControleSistema.PortaEnvioEmail > 0)
                smtpClient.Port = ControleSistema.PortaEnvioEmail.Value;

            if (ControleSistema.AtivarSslEnvio)
                smtpClient.EnableSsl = true;
            else
                smtpClient.EnableSsl = false;

            try
            {
                // Envia a mensagem
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("ProcessaEmail (0)", ex);
                throw ex;
            }
        }

        /// <summary>
        /// Método utilizado para enviar email para os administradores,
        /// informando o valor total de pedidos diário e cumulativo do mês, valor total de metro quadrado diário e cumulativo do mês,
        /// total de m2 pronto diário e cumulativo do mês, total liberado diário e cumulativo do mês.
        /// </summary>
        public static void EnviaEmailAdministradores()
        {
            using (var trans = new GDATransaction())
            {

                try
                {
                    FilaOperacoes.EnviarEmailAdministradores.AguardarVez();

                    trans.BeginTransaction();

                    lock (SyncRoot)
                    {
                        if (!FilaEmailDAO.Instance.PodeEnviarEmailAdmin())
                        {
                            trans.Rollback();
                            trans.Close();
                            return;
                        }

                        var func = FuncionarioDAO.Instance.GetAdministradores(false);
                        if (func.Count == 0)
                        {
                            trans.Rollback();
                            trans.Close();
                            return;
                        }

                        DateTime dataIni = Geral.DataInicioEnvioSMSEmailAdministradores;
                        DateTime dataFim = Geral.DataFimEnvioSMSEmailAdministradores;

                        var lstParam = new List<GDA.GDAParameter>
                        {
                            new GDA.GDAParameter("?dataIni", dataIni),
                            new GDA.GDAParameter("?dataFim", dataFim)
                        };

                        var textoDiaConsiderado = dataFim.Day < DateTime.Now.Day ? "ontem" : "hoje";

                        // Busca os dados para a mensagem, sem considerar pedidos de produção
                        string sqlMes = string.Format("Select Sum({0}) From pedido Where situacao<>{1} And Date_Format(dataCad, '%m, %Y')=Date_Format(now(), '%m, %Y') And tipoPedido<>{2} {3}",
                             "{0}", (int)Pedido.SituacaoPedido.Cancelado, (int)Pedido.TipoPedidoEnum.Producao, "{1}");

                        decimal totalPedidosMes = PedidoDAO.Instance.ExecuteScalar<decimal>(trans,
                            string.Format(sqlMes, "total",
                                EmailConfig.ConsiderarReposicaoGarantiaTotalPedidosEmitidos ?
                                    string.Empty :
                                    string.Format("AND TipoVenda NOT IN ({0},{1})", (int)Pedido.TipoVendaPedido.Garantia, (int)Pedido.TipoVendaPedido.Reposição)));

                        double totMPedidosMes = PedidoDAO.Instance.ExecuteScalar<double>(trans, string.Format(sqlMes, "totM", ""));

                        // Cálculo de peças prontas baseadas em roteiro
                        var totMProntoMes = ProducaoDAO.Instance.ExecuteScalar<double>(trans, @"
                            SELECT SUM(ppe.totM/ppe.qtde)
                            FROM produtos_pedido_espelho ppe
                            INNER JOIN (
	                            SELECT ppp.idprodped
	                            FROM roteiro_producao_etiqueta rpe
		                            INNER JOIN setor s ON (rpe.idSetor=s.idSetor and ultimosetor=1)
		                            INNER JOIN leitura_producao lp ON (rpe.idProdPedProducao=lp.idProdPedProducao and rpe.idSetor=lp.idSetor)
		                            INNER JOIN produto_pedido_producao ppp ON (ppp.idProdPedProducao=lp.idProdPedProducao)
	                            WHERE DATE_FORMAT(lp.dataLeitura, '%m, %Y')=DATE_FORMAT(NOW(), '%m, %Y')
                            ) AS tbl ON (ppe.idProdPed=tbl.IdProdPed)");

                        // Cálculo de peças prontas baseada em setor pronto (se a empresa tiver)
                        var idsSetorPronto = SetorDAO.Instance.GetValoresCampo(trans, "Select idSetor From setor Where tipo=" + (int)TipoSetor.Pronto, "idSetor");
                        if (!string.IsNullOrEmpty(idsSetorPronto))
                            totMProntoMes += ProducaoDAO.Instance.ExecuteScalar<double>(trans, string.Format(@"
                                SELECT SUM(ppe.totM/ppe.qtde)
                                FROM produtos_pedido_espelho ppe
                                INNER JOIN (
	                                SELECT ppp.idprodped
	                                FROM leitura_producao lp
		                                INNER JOIN produto_pedido_producao ppp ON (ppp.idProdPedProducao=lp.idProdPedProducao)
	                                WHERE DATE_FORMAT(lp.dataLeitura, '%m, %Y')=DATE_FORMAT(NOW(), '%m, %Y')
		                                AND lp.IdSetor In ({0})
                                ) AS tbl ON (ppe.idProdPed=tbl.IdProdPed)", idsSetorPronto));

                        decimal totalLiberadosMes = LiberarPedidoDAO.Instance.ExecuteScalar<decimal>(trans, "Select Sum(total) From liberarpedido Where situacao=" +
                            (int)LiberarPedido.SituacaoLiberarPedido.Liberado + " And Date_Format(dataLiberacao, '%m, %Y')=Date_Format(now(), '%m, %Y')");

                        // Busca os dados para a mensagem, sem considerar pedidos de produção
                        string sqlDia = string.Format("Select Sum({0}) From pedido Where situacao<>{1} and dataCad>=?dataIni and dataCad<=?dataFim And tipoPedido<>{2} {3}",
                            "{0}", (int)Pedido.SituacaoPedido.Cancelado, (int)Pedido.TipoPedidoEnum.Producao, "{1}");

                        // Se houver alteração neste sql, altera também no envio do sms para os administradores
                        decimal totalPedidosDia = PedidoDAO.Instance.ExecuteScalar<decimal>(trans,
                            string.Format(sqlDia, "total",
                                EmailConfig.ConsiderarReposicaoGarantiaTotalPedidosEmitidos ?
                                    string.Empty :
                                    string.Format("AND TipoVenda NOT IN ({0},{1})", (int)Pedido.TipoVendaPedido.Garantia, (int)Pedido.TipoVendaPedido.Reposição)), lstParam.ToArray());

                        double totMPedidosDia = PedidoDAO.Instance.ExecuteScalar<double>(trans, String.Format(sqlDia, "totM", ""), lstParam.ToArray());

                        // Cálculo de peças prontas baseadas em roteiro
                        var totMProntoDia = ProducaoDAO.Instance.ExecuteScalar<double>(trans, @"
                            SELECT SUM(ppe.totM/ppe.qtde)
                            FROM produtos_pedido_espelho ppe
                            INNER JOIN (
	                            SELECT ppp.idprodped
	                            FROM roteiro_producao_etiqueta rpe
		                            INNER JOIN setor s ON (rpe.idSetor=s.idSetor and ultimosetor=1)
		                            INNER JOIN leitura_producao lp ON (rpe.idProdPedProducao=lp.idProdPedProducao and rpe.idSetor=lp.idSetor)
		                            INNER JOIN produto_pedido_producao ppp ON (ppp.idProdPedProducao=lp.idProdPedProducao)
	                            WHERE DATE(lp.dataLeitura)>=?dataIni AND DATE(lp.dataLeitura)<=?dataFim
                            ) AS tbl ON (ppe.idProdPed=tbl.IdProdPed)", lstParam.ToArray());

                        // Cálculo de peças prontas baseada em setor pronto (se a empresa tiver)
                        if (!string.IsNullOrEmpty(idsSetorPronto))
                            totMProntoDia += ProducaoDAO.Instance.ExecuteScalar<double>(trans, string.Format(@"
                                SELECT SUM(ppe.totM/ppe.qtde)
                                FROM produtos_pedido_espelho ppe
                                INNER JOIN (
	                                SELECT ppp.idprodped
	                                FROM leitura_producao lp
		                                INNER JOIN produto_pedido_producao ppp ON (ppp.idProdPedProducao=lp.idProdPedProducao)
	                                WHERE DATE(lp.dataLeitura)>=?dataIni AND DATE(lp.dataLeitura)<=?dataFim
		                                AND lp.IdSetor In ({0})
                                ) AS tbl ON (ppe.idProdPed=tbl.IdProdPed)", idsSetorPronto), lstParam.ToArray());

                        decimal totalLiberadosDia = LiberarPedidoDAO.Instance.ExecuteScalar<decimal>(trans, "Select Sum(total) From liberarpedido Where situacao=" +
                            (int)LiberarPedido.SituacaoLiberarPedido.Liberado + " and dataLiberacao>=?dataIni and dataLiberacao<=?dataFim", lstParam.ToArray());

                        // Verifica se será enviado e-mail hoje
                        // Só envia se houver algum dado para enviar
                        if (totalPedidosDia == 0 && totMPedidosDia == 0 && totMProntoDia == 0 && totalLiberadosDia == 0 &&
                            totalPedidosMes == 0 && totMPedidosMes == 0 && totMProntoMes == 0 && totalLiberadosMes == 0)
                        {
                            FilaEmailDAO.Instance.MarcaNaoEnviar();
                            {
                                trans.Rollback();
                                trans.Close();
                                return;
                            }
                        }

                        decimal totalRecebidoDia = 0;
                        decimal totalRecebidoMes = 0;

                        var textoPadrao = Geral.TextoEmailAdministradores;

                        if (textoPadrao.Contains("{9}"))
                        {
                            totalRecebidoDia = RecebimentoDAO.Instance.GetRecebimentosTipo(DateTime.Now.ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"), 0, 0).Where(f => f.Descricao == "TOTAL").First().Valor;
                        }

                        if (textoPadrao.Contains("{10}"))
                        {
                            totalRecebidoMes = RecebimentoDAO.Instance.GetRecebimentosTipo(DateTime.Now.ObtemPrimeiroDiaMesAtual().ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"), 0, 0).Where(f => f.Descricao == "TOTAL").First().Valor;
                        }

                        /*{0}: Texto do dia considerado (HOJE, AMANHÃ).
                          {1}: Total dos pedidos do dia.
                          {2}: Total da metragem dos pedidos do dia.
                          {3}: Total dos pedidos do mês.
                          {4}: Total de metragem dos pedidos do mês.
                          {5}: Total de metragem pronta no dia.
                          {6}: Total de metragem pronta no mês.
                          {7}: Total liberado no dia.
                          {8}: Total liberado no mês.
                          {9}: Total recebido no dia (Baseado no relatório de recebimentos por tipo).
                          {10}: Total Recebido no mês (Baseado no relatório de recebimentos por tipo).*/
                        var parametros = new string[]
                        {
                            textoDiaConsiderado,
                            totalPedidosDia.ToString("C"),
                            totMPedidosDia.ToString("0.##"),
                            totalPedidosMes.ToString("C"),
                            totMPedidosMes.ToString("0.##"),
                            totMProntoDia.ToString("0.##"),
                            totMProntoMes.ToString("0.##"),
                            totalLiberadosDia.ToString("C"),
                            totalLiberadosMes.ToString("C"),
                            totalRecebidoDia.ToString("C"),
                            totalRecebidoMes.ToString("C"),
                        };

                        string mensagem = string.Format("\n" + Geral.TextoEmailAdministradores, parametros);

                        // Verifica mais uma vez se pode enviar email
                        if (!FilaEmailDAO.Instance.PodeEnviarEmailAdmin())
                        {
                            trans.Rollback();
                            trans.Close();
                            return;
                        }

                        foreach (Funcionario f in func)
                        {
                            // Se não tiver email cadastrado para este administrador, apenas não envia
                            if (string.IsNullOrEmpty(f.Email))
                                continue;

                            if (f.IdLoja == 0)
                            {
                                throw new Exception("Não foi possível recuperar a loja do funcionário ao salvar o e-mail a ser enviado.");
                            }

                            EnviaEmailAsync(trans, (uint)f.IdLoja, f.Email, "Resumo diário WebGlass", mensagem, EmailEnvio.Comercial, true, null);
                        }
                    }

                    trans.Commit();
                    trans.Close();
                }
                catch (Exception ex)
                {
                    try
                    {
                        trans.Rollback();
                        trans.Close();
                    }
                    catch { }

                    ErroDAO.Instance.InserirFromException("EnvioEmailAdministradores", ex);
                }
                finally
                {
                    FilaOperacoes.EnviarEmailAdministradores.ProximoFila();
                }
            }
        }

        public static void EnviaEmailAdministradorPrecoProdutoAlterado(GDASession sessao, Produto prodOld, Produto prodNew)
        {
            try
            {
                var idAdminEnvio = EmailConfig.AdministradorEnviarEmailSmsMensagemPrecoProdutoAlterado;
                if (idAdminEnvio == null)
                    return;

                string email = FuncionarioDAO.Instance.ObtemValorCampo<string>(sessao, "email", "idFunc=" + idAdminEnvio) ?? String.Empty;
                if (String.IsNullOrEmpty(email.Trim()))
                    return;

                var idLoja = FuncionarioDAO.Instance.ObtemIdLoja(sessao, (uint)idAdminEnvio);

                if (idLoja == 0)
                {
                    throw new Exception("Não foi possível recuperar a loja do funcionário ao salvar o e-mail a ser enviado.");
                }

                if (prodOld.Custofabbase == prodNew.Custofabbase && prodOld.CustoCompra == prodNew.CustoCompra && prodOld.ValorAtacado == prodNew.ValorAtacado &&
                    prodOld.ValorBalcao == prodNew.ValorBalcao && prodOld.ValorObra == prodNew.ValorObra)
                    return;

                var assunto = "Alteração no preço de venda dos produtos.";
                var mensagem = "O produto " + prodOld.Descricao + " teve seu preço alterado conforme abaixo:" + Environment.NewLine +
                    (prodOld.Custofabbase == prodNew.Custofabbase ? "" : "Custo Forn. Antigo: " + prodOld.Custofabbase.ToString("c") + " Custo Forn. Novo: " + prodNew.Custofabbase.ToString("c") + Environment.NewLine) +
                    (prodOld.CustoCompra == prodNew.CustoCompra ? "" : "Custo Imp. Antigo: " + prodOld.CustoCompra.ToString("c") + " Custo Imp. Novo: " + prodNew.CustoCompra.ToString("c") + Environment.NewLine) +
                    (prodOld.ValorAtacado == prodNew.ValorAtacado ? "" : "Valor Atacado Antigo: " + prodOld.ValorAtacado.ToString("c") + " Valor Atacado Novo: " + prodNew.ValorAtacado.ToString("c") + Environment.NewLine) +
                    (prodOld.ValorBalcao == prodNew.ValorBalcao ? "" : "Valor Balcão Antigo: " + prodOld.ValorBalcao.ToString("c") + " Valor Balcão Novo: " + prodNew.ValorBalcao.ToString("c") + Environment.NewLine) +
                    (prodOld.ValorObra == prodNew.ValorObra ? "" : "Valor Obra Antigo: " + prodOld.ValorObra.ToString("c") + " Valor Obra Novo: " + prodNew.ValorObra.ToString("c"));

                Email.EnviaEmailAsync(sessao, idLoja, email, assunto, mensagem, EmailEnvio.Comercial, true, null);
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("E-mail administrador preço produto alterado", ex);
            }
        }


        /// <summary>
        /// Texto que será inserido no e-mail de pedido pronto.
        /// </summary>
        public static string TextoEmailPedidoPronto(int idPedido, string codCliente, string nomeLoja, Pedido.TipoEntregaPedido tipoEntrega)
        {
            if (tipoEntrega == Pedido.TipoEntregaPedido.Balcao)
                return string.Format(EmailConfig.TextoEmailPedidoProntoBalcao, codCliente, idPedido, nomeLoja);
            else
                return string.Format(EmailConfig.TextoEmailPedidoProntoEntrega, codCliente, idPedido);
        }

        /// <summary>
        /// Texto que será inserido no e-mail de pedido pronto.
        /// </summary>
        public static string TextoEmailPedidoFinalizadoPcp(int idPedido, string codCliente, string nomeLoja, DateTime dataEntrega)
        {
            return string.Format(EmailConfig.TextoEmailPedidoFinalizadoPcp, codCliente, idPedido, dataEntrega.ToString("dd/MM/yyyy"), nomeLoja);
        }
    }
}
