using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.RelDAL;
using HumanAPIClient.Service;
using HumanAPIClient.Model;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.Helper
{
    public static class SMS
    {
        /// <summary>
        /// Envia SMS de pedido pronto
        /// </summary>
        public static void EnviaSMSPedidoPronto(uint idPedido)
        {
            EnviaSMSPedidoPronto(null, idPedido);
        }

        /// <summary>
        /// Envia SMS de pedido pronto
        /// </summary>
        public static void EnviaSMSPedidoPronto(GDASession session, uint idPedido)
        {
            bool exibirM2 = PedidoConfig.EnviarTotM2SMSPedidoPronto;

            uint idCli = PedidoDAO.Instance.ObtemIdCliente(session, idPedido);

            // Verifica se o cliente não recebe SMS
            if (ClienteDAO.Instance.NaoRecebeSMS(session, idCli))
                return;

            // Se o sms já tiver sido enviado, ignora.
            if (FilaSmsDAO.Instance.MensagemJaEnviada(session, "nosso numero " + idPedido))
                return;

            // Se for pedido importado de outro sistema, envia o SMS para o cliente original, ou seja, o cliente da empresa que exportou 
            // o pedido para esta empresa, caso contrário, envia para o cliente deste pedido mesmo.
            var celCliente = !PedidoDAO.Instance.IsPedidoImportado(session, idPedido) ? ClienteDAO.Instance.ObtemCelEnvioSMS(session, idCli) : PedidoDAO.Instance.ObtemCelCliExterno(session, idPedido);

            if (String.IsNullOrEmpty(celCliente))
                throw new Exception("O número do celular do cliente não foi informado.");

            while (celCliente.Contains(" "))
                celCliente = celCliente.Replace(" ", "");

            celCliente = celCliente.Replace("(", "").Replace(")", "").Replace("-", "");

            // Deve considerar que em São Paulo são 9 dígitos fora o DDD
            if (celCliente.Length < 10 || celCliente.Length > 11)
                throw new Exception("O número do celular do cliente informado é inválido.");

            uint idLoja = PedidoDAO.Instance.ObtemIdLoja(session, idPedido);
            string codCliente = PedidoDAO.Instance.ObtemValorCampo<string>(session, "codCliente", "idPedido=" + idPedido);
            decimal totalPedido = PedidoEspelhoDAO.Instance.ObtemValorCampo<decimal>(session, "total", "idPedido=" + idPedido);
            float totM = PedidoEspelhoDAO.Instance.ObtemTotM2(session, idPedido);
            float peso = PedidoEspelhoDAO.Instance.ObtemPeso(session, idPedido);
            string nomeLoja = BibliotecaTexto.GetTwoFirstNames(LojaDAO.Instance.GetNome(session, idLoja));

            // Se for pedido importado, exibe o nome do cliente (loja filial) ao enviar o SMS
            if (OrdemCargaConfig.ControlarPedidosImportados && PedidoDAO.Instance.IsPedidoImportado(session, idPedido))
            {
                nomeLoja = BibliotecaTexto.GetTwoFirstNames(ClienteDAO.Instance.GetNome(session, idCli));
                totalPedido = PedidoDAO.Instance.ObtemValorCampo<decimal>(session, "TotalPedidoExterno", "idPedido=" + idPedido);
            }
            
            var mensagem = TextoSMSPedidoPronto((int)idPedido, codCliente, ref nomeLoja, totalPedido, exibirM2, (decimal)totM, peso);
            
            EnviaSMSAsync(session, idPedido.ToString() + idCli, nomeLoja, celCliente, mensagem, false);
        }

        public static void EnviaSMSAdministradores()
        {
            if (!FilaSmsDAO.Instance.PodeEnviarSmsAdmin())
                return;

            var func = FuncionarioDAO.Instance.GetAdministradores(false);
            if (func.Count == 0)
                return;

            var lstParam = new List<GDA.GDAParameter>
            {
                new GDA.GDAParameter("?dataIni", Geral.DataInicioEnvioSMSEmailAdministradores),
                new GDA.GDAParameter("?dataFim", Geral.DataFimEnvioSMSEmailAdministradores)
            };

            // Busca os dados para a mensagem, sem considerar pedidos de produção
            string sql =
                string.Format("select sum({0}) from pedido where situacao<>{1} and dataCad>=?dataIni and dataCad<=?dataFim and tipoPedido<>{2} {3}",
                    "{0}", (int)Pedido.SituacaoPedido.Cancelado, (int)Pedido.TipoPedidoEnum.Producao, "{1}");

            // Se houver alteração neste sql, altera também no envio do email para os administradores
            decimal totalPedidos = PedidoDAO.Instance.ExecuteScalar<decimal>(
                string.Format(sql, "total",
                    EmailConfig.ConsiderarReposicaoGarantiaTotalPedidosEmitidos ?
                        string.Empty :
                        string.Format("AND TipoVenda NOT IN ({0},{1})", (int)Pedido.TipoVendaPedido.Garantia, (int)Pedido.TipoVendaPedido.Reposição)),
                    lstParam.ToArray());

            double totMPedidos = PedidoDAO.Instance.ExecuteScalar<double>(string.Format(sql, "totM", ""), lstParam.ToArray());
            
            var idsSetorPronto = SetorDAO.Instance.GetValoresCampo("Select idSetor From setor Where tipo=" + (int)TipoSetor.Pronto, "idSetor");

            // Cálculo de peças prontas baseadas em roteiro
            var totMPronto = ProducaoDAO.Instance.ExecuteScalar<double>(@"
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
                totMPronto += ProducaoDAO.Instance.ExecuteScalar<double>(string.Format(@"
                    SELECT SUM(ppe.totM/ppe.qtde) 
                    FROM produtos_pedido_espelho ppe
                    INNER JOIN (
	                    SELECT ppp.idprodped
	                    FROM leitura_producao lp
		                    INNER JOIN produto_pedido_producao ppp ON (ppp.idProdPedProducao=lp.idProdPedProducao)
	                    WHERE DATE(lp.dataLeitura)>=?dataIni AND DATE(lp.dataLeitura)<=?dataFim
		                    AND lp.IdSetor In ({0})
                    ) AS tbl ON (ppe.idProdPed=tbl.IdProdPed)", idsSetorPronto), lstParam.ToArray());

            decimal totalLiberados = LiberarPedidoDAO.Instance.ExecuteScalar<decimal>("select sum(total) from liberarpedido where situacao=" +
                (int)LiberarPedido.SituacaoLiberarPedido.Liberado + " and dataLiberacao>=?dataIni and dataLiberacao<=?dataFim", lstParam.ToArray());

            // Verifica se será enviado SMS hoje
            // Só envia se houver algum dado para enviar
            if (totalPedidos == 0 && totMPedidos == 0 && totMPronto == 0 && totalLiberados == 0)
            {
                FilaSmsDAO.Instance.MarcaNaoEnviar();
                Erro msg = new Erro
                {
                    IdFuncErro = UserInfo.GetUserInfo.CodUser,
                    DataErro = DateTime.Now,
                    UrlErro = "MarcarNaoEnviarSMSAdmin"
                };
                ErroDAO.Instance.Insert(msg);
                return;
            }

            string mensagem = String.Format("Resumo diario WebGlass{0}. Ped. emitidos: " + totalPedidos.ToString("C") +
                " (" + totMPedidos.ToString("0.##") + "m2). m2 pronto: " + totMPronto.ToString("0.##") +
                "m2. Faturado: " + totalLiberados.ToString("C"), Configuracoes.Geral.TextoAdicionalSMS);

            foreach (Funcionario f in func)
            {
                // Se não tiver celular cadastrado para este administrador, apenas não envia SMS
                if (String.IsNullOrEmpty(f.TelCel))
                    continue;

                while (f.TelCel.Contains(" "))
                    f.TelCel = f.TelCel.Replace(" ", "");

                f.TelCel = f.TelCel.Replace("(", "").Replace(")", "").Replace("-", "");

                var codSMS = DateTime.Now.DayOfYear.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + f.IdFunc;

                SMS.EnviaSMSAsync(codSMS, "WebGlass", f.TelCel, mensagem, true);
            }
        }

        public static void EnviaSMSAsync(string codMensagem, string remetente, string destinatario, string mensagem, bool smsAdmin)
        {
            EnviaSMSAsync(null, codMensagem, remetente, destinatario, mensagem, smsAdmin);
        }

        public static void EnviaSMSAsync(GDASession session, string codMensagem, string remetente, string destinatario, string mensagem, bool smsAdmin)
        {
            if (mensagem.Length > 150)
                mensagem = mensagem.Substring(0, 149);

            FilaSms sms = new FilaSms
            {
                CodMensagem = codMensagem,
                NomeLoja = remetente,
                CelCliente = destinatario,
                Mensagem = mensagem,
                DataCad = DateTime.Now,
                SmsAdmin = smsAdmin
            };

            FilaSmsDAO.Instance.Insert(session, sms);
        }

        public static string EnviarSMSCliente(string remetente, string destinatarios, string mensagem)
        {
            try
            {
                string celCliente;

                if (destinatarios.Contains("-1"))
                {
                    var lstClientes = ClienteDAO.Instance.GetAll();

                    foreach (Cliente objCliente in lstClientes)
                    {
                        if (!String.IsNullOrEmpty(objCliente.TelCel))
                        {
                            celCliente = objCliente.TelCel;

                            while (celCliente.Contains(" "))
                                celCliente = celCliente.Replace(" ", "");

                            celCliente = celCliente.Replace("(", "").Replace(")", "").Replace("-", "");
                            
                            var id = DateTime.Now.ToString("ddMMyyhhmmss" + objCliente.IdCli);

                            EnviaSMSAsync(id, remetente, celCliente, mensagem, false);
                        }
                    }
                }
                else
                {
                    string[] lstDest = destinatarios.Split(',');

                    foreach (string idCliente in lstDest)
                    {
                        if (!String.IsNullOrEmpty(idCliente))
                        {
                            celCliente = ClienteDAO.Instance.ObtemCelEnvioSMS(idCliente.StrParaUint());

                            if (!String.IsNullOrEmpty(celCliente))
                            {
                                while (celCliente.Contains(" "))
                                    celCliente = celCliente.Replace(" ", "");

                                celCliente = celCliente.Replace("(", "").Replace(")", "").Replace("-", "");

                                var id = DateTime.Now.ToString("ddMMyyhhmmss" + idCliente);

                                EnviaSMSAsync(id, remetente, celCliente, mensagem, false);
                            }
                        }
                    }
                }

                return "ok\tMensagem enviada.";
            }
            catch (Exception e)
            {
                return "Erro\t" + e.Message;
            }
        }

        internal static SMSSend.responseSendMessage EnviaSMSOld(string codMensagem, string remetente, string destinatario, string mensagem)
        {
            SMSManager.WSManager wsManager = new Glass.Data.SMSManager.WSManager();
            SMSManager.wsToken wsToken = wsManager.getAuthentication(System.Configuration.ConfigurationManager.AppSettings["sistema"].ToLower(), "webglass20");

            SMSSend.WSSend wsSend = new Glass.Data.SMSSend.WSSend();
            return wsSend.sendMessage(wsToken.token, remetente, "55" + destinatario, codMensagem, null, 0, 23, null, mensagem);
        }

        public static string EnviaSMS(string codMensagem, string remetente, string destinatario, string mensagem)
        {
            var conta = System.Configuration.ConfigurationManager.AppSettings["ContaSMS"];
            var senha = System.Configuration.ConfigurationManager.AppSettings["SenhaSMS"];

            var sms = new SimpleSending(conta, senha);

            SimpleMessage message = new SimpleMessage
            {
                From = remetente,
                To = string.Format("55{0}", destinatario),
                Message = mensagem,
                Id = codMensagem,
                Callback = HumanAPIClient.Enum.CallbackTypeEnum.FULL
            };

            var retorno = String.Join("\n", sms.send(message));

            return retorno;
        }

        public static void EnviaSmsAdministradorPrecoProdutoAlterado(Produto prodOld, Produto prodNew)
        {
            try
            {
                var idAdminEnvio = EmailConfig.AdministradorEnviarEmailSmsMensagemPrecoProdutoAlterado;
                if (idAdminEnvio == null)
                    return;

                string telCel = FuncionarioDAO.Instance.ObtemTelCel((uint)idAdminEnvio);
                if (string.IsNullOrEmpty(telCel))
                    return;

                while (telCel.Contains(" "))
                    telCel = telCel.Replace(" ", "");

                telCel = telCel.Replace("(", "").Replace(")", "").Replace("-", "");

                if (prodOld.Custofabbase == prodNew.Custofabbase && prodOld.CustoCompra == prodNew.CustoCompra && prodOld.ValorAtacado == prodNew.ValorAtacado &&
                    prodOld.ValorBalcao == prodNew.ValorBalcao && prodOld.ValorObra == prodNew.ValorObra)
                    return;

                var mensagem = "O produto " + prodOld.Descricao + " teve seu preço alterado:" + Environment.NewLine +
                    (prodOld.Custofabbase == prodNew.Custofabbase ? "" : "Custo Forn. Antigo: " + prodOld.Custofabbase.ToString("c") + " Novo: " + prodNew.Custofabbase.ToString("c") + Environment.NewLine) +
                    (prodOld.CustoCompra == prodNew.CustoCompra ? "" : "Custo Imp. Antigo: " + prodOld.CustoCompra.ToString("c") + " Novo: " + prodNew.CustoCompra.ToString("c") + Environment.NewLine) +
                    (prodOld.ValorAtacado == prodNew.ValorAtacado ? "" : "Atacado Antigo: " + prodOld.ValorAtacado.ToString("c") + " Novo: " + prodNew.ValorAtacado.ToString("c") + Environment.NewLine) +
                    (prodOld.ValorBalcao == prodNew.ValorBalcao ? "" : " Balcão Antigo: " + prodOld.ValorBalcao.ToString("c") + " Novo: " + prodNew.ValorBalcao.ToString("c") + Environment.NewLine) +
                    (prodOld.ValorObra == prodNew.ValorObra ? "" : "Obra Antigo: " + prodOld.ValorObra.ToString("c") + " Novo: " + prodNew.ValorObra.ToString("c"));

                var codSMS = DateTime.Now.DayOfYear.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + idAdminEnvio;

                /* Chamado 65394. */
                //verifica a quantidade de caracteres da mensagem pois o limite de SMS é de 150.
                if (mensagem.Length > 150)
                    mensagem = mensagem.Substring(0, 149);

                SMS.EnviaSMSAsync(codSMS, "WebGlass", telCel, mensagem, false);
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("SMS administrador preço produto alterado", ex);
            }
        }

        public static void EnviaSmsAdministradorPrecoListProdutoAlterado(IList<Produto> produtosOld, IList<Produto> produtosNew)
        {
            for (int i = 0; i < produtosOld.Count; i++)
            {
                if (produtosOld[i].IdProd == produtosNew[i].IdProd)
                    EnviaSmsAdministradorPrecoProdutoAlterado(produtosOld[i], produtosNew[i]);
            }
        }

        /// <summary>
        /// Texto que será inserido no e-mail de pedido pronto.
        /// </summary>
        public static string TextoSMSPedidoPronto(int idPedido, string codCliente, ref string nomeLoja, decimal totalPedido,
            bool exibirM2, decimal totalM2, float peso)
        {
            var mensagem = string.Empty;

            if (nomeLoja.Length > 12)
                nomeLoja = nomeLoja.Substring(0, 12);

            mensagem = string.Format(Geral.TextoSMSPedidoPronto, codCliente, idPedido, totalPedido.ToString("C"), (exibirM2 ? string.Format(", {0}m2", totalM2) : string.Empty), peso);
            
            if (mensagem.Length > 137)
                mensagem = mensagem.Substring(0, 137);

            return mensagem;
        }
    }
}