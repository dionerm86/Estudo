// <copyright file="MonitorNotaFiscal.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using Glass.Integracao.Historico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Glass.Integracao.Khan
{
    /// <summary>
    /// Representa o monitor das notas fiscais.
    /// </summary>
    internal class MonitorNotaFiscal : MonitorEventos
    {
        private const int TipoDocumentoInscricaoEstadual = 3;
        private const int TipoDocumentoNenhum = 0;

        private const int PessoaJuridica = 1;
        private const int PessoaFisica = 2;

        private const string NotaFiscalEmitida = "4";

        private readonly Colosoft.Logging.ILogger logger;
        private readonly ConfiguracaoKhan configuracao;
        private readonly Historico.IProvedorHistorico provedorHistorico;
        private readonly string pedidoServiceUid = Guid.NewGuid().ToString();
        private readonly string consultasServciceUid = Guid.NewGuid().ToString();
        private readonly System.Text.RegularExpressions.Regex dddRegex = new System.Text.RegularExpressions.Regex("\\((?<ddd>([0-9][0-9]))\\)");
        private readonly List<Item> itensIntegrando = new List<Item>();

        private DateTime ultimaConsultaItensNaoIntegrados = DateTime.Now.AddDays(-1);

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="MonitorNotaFiscal"/>.
        /// </summary>
        /// <param name="domainEvents">Eventos de domínio.</param>
        /// <param name="logger">Logger para registrar as informações.</param>
        /// <param name="configuracao">Configuração.</param>
        /// <param name="provedorHistorico">Provedor dos históricos.</param>
        public MonitorNotaFiscal(
            Colosoft.Domain.IDomainEvents domainEvents,
            Colosoft.Logging.ILogger logger,
            ConfiguracaoKhan configuracao,
            Historico.IProvedorHistorico provedorHistorico)
            : base(domainEvents)
        {
            this.logger = logger;
            this.configuracao = configuracao;
            this.provedorHistorico = provedorHistorico;
            this.AdicionarToken<Data.Domain.NotaFiscalGerada>(
                domainEvents.GetEvent<Data.Domain.NotaFiscalGerada>().Subscribe(this.NotaFiscalGerada));

            Colosoft.Net.ServiceClientsManager.Current.Register(this.pedidoServiceUid, this.CriarPedidoClient);
            Colosoft.Net.ServiceClientsManager.Current.Register(this.consultasServciceUid, this.CriarConsultasClient);
        }

        private KhanPedidoServiceReference.PedidoServiceClient PedidoClient =>
           Colosoft.Net.ServiceClientsManager.Current.Get<KhanPedidoServiceReference.PedidoServiceClient>(this.pedidoServiceUid);

        private KhanConsultasServiceReference.ConsultasServiceClient ConsultasClient =>
            Colosoft.Net.ServiceClientsManager.Current.Get<KhanConsultasServiceReference.ConsultasServiceClient>(this.consultasServciceUid);

        private static void PreencherEnderecos(KhanPedidoServiceReference.Pedido pedido, Data.Model.Cliente cliente)
        {
            pedido.endent = cliente.EnderecoEntrega;
            pedido.baient = cliente.BairroEntrega;
            pedido.cident = cliente.CidadeEntrega;
            pedido.estent = cliente.UfEntrega;
            pedido.endent_num = cliente.NumeroEntrega;
            pedido.endent_complemento = cliente.ComplEntrega;
            pedido.cepent = cliente.CepEntrega;

            if (!string.IsNullOrEmpty(cliente.EnderecoCobranca))
            {
                pedido.endfat = cliente.EnderecoCobranca;
                pedido.baifat = cliente.BairroCobranca;
                pedido.cidfat = cliente.CidadeCobranca;
                pedido.estfat = cliente.UfCobranca;
                pedido.endfat_num = cliente.NumeroCobranca;
                pedido.endfat_complemento = cliente.ComplCobranca;
                pedido.cepfat = cliente.CepCobranca;
            }
            else
            {
                pedido.endfat = cliente.Endereco;
                pedido.baifat = cliente.Bairro;
                pedido.cidfat = cliente.Cidade;
                pedido.estfat = cliente.Uf;
                pedido.endfat_num = cliente.Numero;
                pedido.endfat_complemento = cliente.Compl;
                pedido.cepfat = cliente.Cep;
            }
        }

        private static string ObterTipoPagamentoPedido(int formaPagto)
        {
            switch (formaPagto)
            {
                case 1:
                    return "0"; // à vista

                default:
                    return "10"; // faturado
            }
        }

        private KhanPedidoServiceReference.ItemPedido ConverterItemPedido(
            Data.Model.ProdutosNf produtoNf,
            Data.Model.Produto produto,
            ref int sequencia)
        {
            return new KhanPedidoServiceReference.ItemPedido
            {
                numped_int = (int)produtoNf.IdNf,
                seqped_int = 0,
                codempresa = this.configuracao.Empresa,
                seq = sequencia++,
                codpro = produto.CodInterno,
                codbar = string.Empty,
                qtdpro = produtoNf.Qtde,
                prunit = (float)produtoNf.ValorUnitario,
                vfrete = (float)produtoNf.ValorFrete,
                vdesc = (float)produtoNf.ValorDesconto,
                lote = string.Empty,
                PPIS = produtoNf.AliqPis,
                PCOFINS = produtoNf.AliqCofins,
                PIPI = produtoNf.AliqIpi,
                PICMS = produtoNf.AliqIcms,
                PICMSSUB = produtoNf.AliqIcmsSt,
                SITTRIB = "000",
                SITTRIB_IPI = string.Empty,
                SITTRIB_COFINS = string.Empty,
                SITTRIB_PIS = string.Empty,
            };
        }

        private KhanPedidoServiceReference.ParcelaPedido ConverterParcelaPedido(Data.Model.ParcelaNf parcelaNf, ref int sequencia)
        {
            return new KhanPedidoServiceReference.ParcelaPedido
            {
                numped_int = (int)parcelaNf.IdNf,
                seqped_int = 0,
                seq = sequencia++,
                datvenc = parcelaNf.Data?.ToString("yyyyMMdd"),
                codempresa = this.configuracao.Empresa,
                valor = (float)parcelaNf.Valor,
                TipoPagamento = "10",
            };
        }

        private string ObterDDD(string fone)
        {
            if (fone != null)
            {
                var match = this.dddRegex.Match(fone);

                if (match.Success)
                {
                    return match.Groups["ddd"]?.Value;
                }
            }

            return null;
        }

        private string ObterTelefoneSemDDD(string fone)
        {
            if (fone != null)
            {
                fone = this.dddRegex.Replace(fone, string.Empty);
            }

            return fone?.Trim();
        }

        private KhanPedidoServiceReference.Pedido Converter(GDA.GDASession sessao, Data.Model.NotaFiscal notaFiscal)
        {
            var cliente = Data.DAL.ClienteDAO.Instance.GetElement(sessao, notaFiscal.IdCliente.GetValueOrDefault());
            var produtosNf = Data.DAL.ProdutosNfDAO.Instance.GetByNf(sessao, notaFiscal.IdNf).ToList();
            var produtos = Data.DAL.ProdutoDAO.Instance.ObterProdutos(sessao, produtosNf.Select(f => f.IdProd).Distinct()).ToList();
            var parcelas = Data.DAL.ParcelaNfDAO.Instance.GetByNf(sessao, notaFiscal.IdNf);

            var pedido = new KhanPedidoServiceReference.Pedido
            {
                Token = this.configuracao.Token,
                codempresa = this.configuracao.Empresa,
                numped_int = (int)notaFiscal.IdNf,
                seqped_int = 0,
                datped = notaFiscal.DataCad.ToString("yyyyMMdd"),
                numpedc = string.Empty,
                nomcad = cliente.Nome,
                tipos = "Cliente,",
                tipodoc1 = cliente.TipoPessoa == "J" ? PessoaJuridica : PessoaFisica,
                numdoc1 = cliente.CpfCnpj,
                tipodoc2 = cliente.TipoPessoa == "J" ? TipoDocumentoInscricaoEstadual : TipoDocumentoNenhum,
                numdoc2 = cliente.TipoPessoa == "J" ? cliente.RgEscinst : null,
                tippag = ObterTipoPagamentoPedido(notaFiscal.FormaPagto),
                numpar = parcelas.Count,
                statusw = NotaFiscalEmitida,
                numped = 0,
                seqped = 0,
                email = cliente.EmailFiscal,
                foneddd = this.ObterDDD(cliente.TelCont),
                fonenum = this.ObterTelefoneSemDDD(cliente.TelCont),
                codven = "0",
                autoriz_comercial = string.Empty,
                contrato = string.Empty,
                numnf = notaFiscal.NumeroNFe.ToString(),
                serie = notaFiscal.Serie,
                xml = string.Empty,
                numnf_origem = string.Empty,
                numitens = produtosNf.Count,
                tipes = notaFiscal.TipoDocumento == 2 ? "S" : "E",
                tipoEnvio = string.Empty,
                codTipoEnvio = string.Empty,
                nf_chave = notaFiscal.ChaveAcesso,
                nf_contingencia = string.Empty,
                nf_data_contingencia = string.Empty,
                nf_justif_contingencia = string.Empty,
                nf_numprotocolo = notaFiscal.NumProtocolo ?? string.Empty,
                nf_operador_caixa = string.Empty,
                cp_terminal = string.Empty,
                nf_numprotocolo_Can = notaFiscal.NumProtocoloCanc ?? string.Empty,
                nf_dat_cancelamento = string.Empty,
            };

            PreencherEnderecos(pedido, cliente);

            var sequenciaItem = 1;
            pedido.Itens = produtosNf
                .Select(produtoNf => this.ConverterItemPedido(produtoNf, produtos.FirstOrDefault(produto => produto.IdProd == produtoNf.IdProd), ref sequenciaItem))
                .ToList();

            var sequenciaParcela = 1;
            pedido.Parcelas = parcelas
                .Select(parcelaNf => this.ConverterParcelaPedido(parcelaNf, ref sequenciaParcela))
                .ToList();

            return pedido;
        }

        private System.ServiceModel.ICommunicationObject CriarPedidoClient()
        {
            var serviceAddress = Colosoft.Net.ServicesConfiguration.Current[IntegradorKhan.NomePedidosService];
            var client = new KhanPedidoServiceReference.PedidoServiceClient(serviceAddress.GetBinding(), serviceAddress.GetEndpointAddress());
            client.Endpoint.EndpointBehaviors.Add(new Seguranca.KhanEndpointBehavior(this.configuracao));
            return client;
        }

        private System.ServiceModel.ICommunicationObject CriarConsultasClient()
        {
            var serviceAddress = Colosoft.Net.ServicesConfiguration.Current[IntegradorKhan.NomeConsultasService];
            var client = new KhanConsultasServiceReference.ConsultasServiceClient(serviceAddress.GetBinding(), serviceAddress.GetEndpointAddress());
            client.Endpoint.EndpointBehaviors.Add(new Seguranca.KhanEndpointBehavior(this.configuracao));
            return client;
        }

        private void NotaFiscalGerada(Data.Domain.NotaFiscalEventoArgs e)
        {
            var task = Task.Run(async () => await this.SalvarNotaFiscal(e.Sessao, e.NotaFiscal));

            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Any())
                {
                    throw ex.InnerExceptions.FirstOrDefault();
                }

                throw;
            }
        }

        private async Task SalvarNotaFiscal(GDA.GDASession sessao, Data.Model.NotaFiscal notaFiscal)
        {
            var pedido = this.Converter(sessao, notaFiscal);

            this.logger.Info($"Salvando nota fiscal (idNf: {notaFiscal.IdNf})...".GetFormatter());

            try
            {
                await this.PedidoClient.SalvarPedidoAsync(pedido);
            }
            catch (Exception ex)
            {
                var mensagem = $"Não foi possível salvar os dados da nota fiscal (IdNf: {notaFiscal.IdNf}) na Khan";
                this.provedorHistorico.RegistrarFalha(HistoricoKhan.NotasFiscais, notaFiscal, mensagem, ex);
                throw;
            }

            var item = this.provedorHistorico.NotificarIntegrando(HistoricoKhan.NotasFiscais, notaFiscal);

            lock (this.itensIntegrando)
            {
                this.itensIntegrando.Add(item);
            }
        }

        private Task SincronizarNotaFiscal(int idNf)
        {
            using (var sessao = new GDA.GDASession())
            {
                var notaFiscal = Data.DAL.NotaFiscalDAO.Instance.GetElement(sessao, (uint)idNf);

                if (notaFiscal == null)
                {
                    throw new IntegracaoException($"Não foi encontrada a nota fiscal com o identificador {idNf}.");
                }

                return this.SalvarNotaFiscal(sessao, notaFiscal);
            }
        }

        private void CarregarItensNaoIntegrados()
        {
            if (this.itensIntegrando.Count == 0 &&
                (DateTime.Now - this.ultimaConsultaItensNaoIntegrados).TotalSeconds > 30)
            {
                try
                {
                    var itens = this.provedorHistorico.ObterItensNaoIntegrados(HistoricoKhan.NotasFiscais);
                    this.ultimaConsultaItensNaoIntegrados = DateTime.Now;

                    lock (this.itensIntegrando)
                    {
                        foreach (var item in itens)
                        {
                            var idNf1 = (int)item.Identificadores.ElementAt(0);

                            if (!this.itensIntegrando.Any(f => (int)f.Identificadores.ElementAt(0) == idNf1))
                            {
                                this.itensIntegrando.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.logger.Error("Falha ao carrega as notas fiscais que ainda não foram integradas...".GetFormatter(), ex);
                }
            }
        }

        /// <summary>
        /// Configura as operações no gerenciador informado.
        /// </summary>
        /// <param name="gerenciadorOperacoes">Gerenciador onde as operações serão configuradas.</param>
        internal void ConfigurarOperacoes(GerenciadorOperacaoIntegracao gerenciadorOperacoes)
        {
            gerenciadorOperacoes.Adicionar(
                new OperacaoIntegracao(
                    "SincronizarNotaFiscal",
                    "Sincroniza os dados da nota fiscal com o ERP da KHAN",
                    new ParametroOperacaoIntegracao(
                        "idNf",
                        typeof(int),
                        "Identificador da nota fiscal",
                        0)),
                new Func<int, Task>(this.SincronizarNotaFiscal));
        }

        /// <summary>
        /// Sincroniza a situação das notas fiscais que estão integrando.
        /// </summary>
        public void SincronizarNotasFiscaisIntegrando()
        {
            this.CarregarItensNaoIntegrados();

            List<string> idsNf = null;

            lock (this.itensIntegrando)
            {
                idsNf = this.itensIntegrando.Select(f => f.Identificadores.First().ToString()).ToList();
            }

            if (idsNf.Any())
            {
                IEnumerable<KhanConsultasServiceReference.PedidoStatus> pedidosStatus = null;

                try
                {
                    pedidosStatus = this.ConsultasClient.ConsultarPedidosStatus(idsNf);
                }
                catch (Exception ex)
                {
                    this.logger.Error("Falha ao consultar a situação dos pedidos.".GetFormatter(), ex);
                    return;
                }

                using (var idsNfEnumerator = idsNf.GetEnumerator())
                using (var pedidosStatusEnumerator = pedidosStatus.GetEnumerator())
                {
                    while (idsNfEnumerator.MoveNext() && pedidosStatusEnumerator.MoveNext())
                    {
                        var idNf = int.Parse(idsNfEnumerator.Current);
                        var pedidoStatus = pedidosStatusEnumerator.Current;

                        if (pedidoStatus.status_integracao.StartsWith("Integrado", StringComparison.InvariantCultureIgnoreCase))
                        {
                            this.provedorHistorico.NotificarIntegrado(HistoricoKhan.NotasFiscais, new object[] { idNf });
                        }
                        else if (!StringComparer.InvariantCultureIgnoreCase.Equals(pedidoStatus.status_integracao, "Pendente Integração"))
                        {
                            this.provedorHistorico.RegistrarFalha(HistoricoKhan.NotasFiscais, new object[] { idNf }, pedidoStatus.status_integracao, null);
                            this.logger.Error($"Não foi possível integrar a nota fiscal (idNf={idNf}). {pedidoStatus.status_integracao}".GetFormatter());
                        }
                    }
                }

                lock (this.itensIntegrando)
                {
                    this.itensIntegrando.Clear();
                }
            }
        }

        /// <summary>
        /// Libera a instância.
        /// </summary>
        /// <param name="disposing">Identifica se a instância está sendo liberada.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Colosoft.Net.ServiceClientsManager.Current.Remove(this.pedidoServiceUid);
        }
    }
}
