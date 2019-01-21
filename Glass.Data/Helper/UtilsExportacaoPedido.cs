using GDA;
using Glass.Data.DAL;
using Glass.Data.Exceptions;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Glass.Data.Helper
{
    /// <summary>
    /// Criado para satisfazer a serialização do objeto Item de ExportarPedido
    /// </summary>
    [Serializable]
    public struct KeyValuePairSerializable<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }
    }

    #region Classe de Suporte

    [Serializable]
    public class ExportarPedido
    {
        [Serializable]
        public class FiguraProdutoPedido
        {
            #region Construtores

            public FiguraProdutoPedido()
            {
            }

            public FiguraProdutoPedido(uint idProdPed, uint? idMaterItemProj, int? item, byte[] figura)
            {
                IdProdPed = idProdPed;
                IdMaterItemProj = idMaterItemProj;
                Item = item;
                Figura = figura;
            }

            #endregion

            public uint IdProdPed;
            public uint? IdMaterItemProj;
            public int? Item;
            public byte[] Figura;
        }

        #region Item

        [Serializable]
        public class DadosAmbientePedido
        {
            public AmbientePedido AmbientePedido;
            public ProdutosPedido[] ProdutosPedido;
        }

        [Serializable]
        public class DadosItemProjeto
        {
            public ItemProjeto ItemProjeto;
            public MaterialItemProjeto[] MateriaisItemProjeto;
            public PecaItemProjeto[] PecasItemProjeto;
            public MedidaItemProjeto[] MedidasItemProjeto;
            public FiguraPecaItemProjeto[] FigurasItemProjeto;
        }

        [Serializable]
        public class ArquivoMesaCorte
        {
            public int IdProdPed;
            public TipoArquivoMesaCorte TipoArquivo;
            public bool paraSGlass;
            public byte[] Arquivo;
        }

        [Serializable]
        public class Item
        {
            #region Construtores

            public Item()
            {
            }

            public Item(uint idPedido, uint[] idsProdutosPedido, bool benef, bool somenteVidros)
            {
                ArquivoMesaCorte = new List<ArquivoMesaCorte>();
                Pedido = PedidoDAO.Instance.GetElementByPrimaryKey(idPedido);
                Loja = LojaDAO.Instance.GetElementByPrimaryKey(Pedido.IdLoja);
                UsarEspelho = PedidoEspelhoDAO.Instance.ExisteEspelho(idPedido);
                SistemaLite = Configuracoes.Geral.SistemaLite;
                EtiquetasExportacao = new List<Helper.KeyValuePairSerializable<int, List<string>>>();

                Pedido.IdPedidoExterno = Pedido.IdPedido;
                Pedido.IdClienteExterno = Pedido.IdCli;
                Pedido.RotaExterna = RotaDAO.Instance.ObtemCodRota(Pedido.IdCli);
                Pedido.ClienteExterno = ClienteDAO.Instance.GetNome(Pedido.IdCli);
                Pedido.PedCliExterno = Pedido.CodCliente;
                Pedido.CelCliExterno = ClienteDAO.Instance.ObtemCelEnvioSMS(Pedido.IdCli);
                Pedido.TotalPedidoExterno = Pedido.TemEspelho ? PedidoEspelhoDAO.Instance.ObtemValorCampo<decimal>("total", "idPedido=" + idPedido) : Pedido.Total;
                Pedido.EnderecoClienteExterno = ClienteDAO.Instance.ObtemEnderecoCompleto(Pedido.IdCli);
                var cidadeUf = ClienteDAO.Instance.ObtemCidadeUf(Pedido.IdCli);
                Pedido.CidadeClienteExterno = cidadeUf.Split('/')[0];
                Pedido.UfClienteExterno = cidadeUf.Split('/')[1];
                Pedido.IdTransportador = null;

                // Zera a entrada e o pagamento antecipado para não der erro de validação de valor ao exportar o pedido.
                Pedido.ValorEntrada = 0;
                Pedido.ValorPagamentoAntecipado = 0;

                List<FiguraProdutoPedido> fig = new List<FiguraProdutoPedido>();

                if (Glass.Configuracoes.PedidoConfig.DadosPedido.AmbientePedido)
                {
                    ProdutosPedido = null;
                    AmbientePedido[] amb = AmbientePedidoDAO.Instance.GetForExportacao(idPedido, idsProdutosPedido, UsarEspelho).ToArray();
                    AmbientesPedido = new DadosAmbientePedido[amb.Length];

                    for (int i = 0; i < amb.Length; i++)
                    {
                        AmbientesPedido[i] = new DadosAmbientePedido();
                        AmbientesPedido[i].AmbientePedido = amb[i];
                        AmbientesPedido[i].ProdutosPedido = ProdutosPedidoDAO.Instance.GetForExportacao(idPedido, amb[i].IdAmbientePedido, idsProdutosPedido, UsarEspelho, somenteVidros);

                        foreach (ProdutosPedido pp in AmbientesPedido[i].ProdutosPedido)
                        {
                            pp.UsarBenefPcp = UsarEspelho;
                            pp.ObsProjeto = amb[i].IdItemProjeto > 0 ? ItemProjetoDAO.Instance.ObtemObs(amb[i].IdItemProjeto.Value) : null;

                            if (!benef)
                            {
                                pp.BuscarBenefImportacao = false;
                                pp.Beneficiamentos = new GenericBenefCollection();
                            }

                            if (UsarEspelho)
                            {
                                GetImagemProdPed(pp.IdProdPed, pp.IdMaterItemProj, ref fig);

                                if (pp.IdProdPedEsp > 0)
                                {
                                    var ppe = ProdutosPedidoEspelhoDAO.Instance.GetElementByPrimaryKey(pp.IdProdPedEsp.Value);
                                    var etiquetasExportacao = RecuperarEtiquetasProdutoPedido(pp, ppe);

                                    // Caso as etiquetas tenham sido recuperadas, salva na propriedade de exportação o ID do produto de pedido com suas respectivas etiquetas.
                                    if (etiquetasExportacao.Key > 0 && etiquetasExportacao.Value.Count > 0 && !EtiquetasExportacao.Contains(etiquetasExportacao))
                                        EtiquetasExportacao.Add(etiquetasExportacao);

                                    #region Arquivo de mesa

                                    if (ppe.IdMaterItemProj.GetValueOrDefault() > 0)
                                    {
                                        var pecaItemProjeto =
                                            PecaItemProjetoDAO.Instance.GetByMaterial(ppe.IdMaterItemProj.Value);

                                        // Se o material não estiver associado à uma peça, não há associação com o arquivo de mesa de corte vindo do projeto
                                        if (pecaItemProjeto != null && pecaItemProjeto.Item != null && pecaItemProjeto.Tipo == 1 /** Instalação **/)
                                        {
                                            GerarArquivoMesaCorte(pp, pecaItemProjeto);
                                        }
                                    }

                                    #endregion
                                }
                            }
                        }
                    }
                }
                else
                {
                    AmbientesPedido = null;
                    ProdutosPedido = ProdutosPedidoDAO.Instance.GetForExportacao(idPedido, 0, idsProdutosPedido, UsarEspelho, somenteVidros);

                    foreach (ProdutosPedido pp in ProdutosPedido)
                    {
                        uint idItemProjeto = pp.IdAmbientePedido > 0 ? AmbientePedidoDAO.Instance.ObtemItemProjeto(pp.IdAmbientePedido.Value) : pp.IdItemProjeto.GetValueOrDefault(0);

                        pp.UsarBenefPcp = UsarEspelho;
                        pp.ObsProjeto = idItemProjeto > 0 ? ItemProjetoDAO.Instance.ObtemObs(idItemProjeto) : null;

                        if (!benef)
                        {
                            pp.BuscarBenefImportacao = false;
                            pp.Beneficiamentos = new GenericBenefCollection();
                        }

                        // Salva o ambiente do projeto no pedCli do produto, desta forma, esta informação irá aparecer no campo "Ambiente"
                        // ao imprimir a etiqueta
                        string obsAmbProj = String.Empty;

                        if (pp.IdAmbientePedido > 0)
                            obsAmbProj = " " + AmbientePedidoDAO.Instance.ObtemValorCampo<string>("Ambiente", "idAmbientePedido=" + pp.IdAmbientePedido);
                        else if (idItemProjeto > 0)
                            obsAmbProj = " " + ItemProjetoDAO.Instance.ObtemAmbiente(idItemProjeto);

                        if (!String.IsNullOrEmpty(obsAmbProj))
                        {
                            pp.PedCli += obsAmbProj;

                            if (pp.PedCli != null && pp.PedCli.Length > 50)
                                pp.PedCli = pp.PedCli.Substring(0, 50);
                        }

                        if (UsarEspelho)
                        {
                            GetImagemProdPed(pp.IdProdPed, pp.IdMaterItemProj, ref fig);

                            if (pp.IdProdPedEsp > 0)
                            {
                                var ppe = ProdutosPedidoEspelhoDAO.Instance.GetElementByPrimaryKey(pp.IdProdPedEsp.Value);
                                var etiquetasExportacao = RecuperarEtiquetasProdutoPedido(pp, ppe);

                                // Caso as etiquetas tenham sido recuperadas, salva na propriedade de exportação o ID do produto de pedido com suas respectivas etiquetas.
                                if (etiquetasExportacao.Key > 0 && etiquetasExportacao.Value.Count > 0 && !EtiquetasExportacao.Contains(etiquetasExportacao))
                                    EtiquetasExportacao.Add(etiquetasExportacao);

                                #region Arquivo de mesa

                                if (ppe.IdMaterItemProj.GetValueOrDefault() > 0)
                                {
                                    var pecaItemProjeto = PecaItemProjetoDAO.Instance.GetByMaterial(ppe.IdMaterItemProj.Value);

                                    // Se o material não estiver associado à uma peça, não há associação com o arquivo de mesa de corte vindo do projeto
                                    if (pecaItemProjeto != null &&
                                        pecaItemProjeto.Item != null &&
                                        pecaItemProjeto.Tipo == 1 /** Instalação **/)
                                    {
                                        this.GerarArquivoMesaCorte(pp, pecaItemProjeto);
                                    }
                                }

                                #endregion
                            }
                        }
                    }
                }

                FigurasProdutosPedido = fig.ToArray();
            }

            private void GerarArquivoMesaCorte(ProdutosPedido pp, PecaItemProjeto pecaItemProjeto)
            {
                using (var sessao = new GDASession())
                {
                    if (PecaItemProjetoDAO.Instance.DeveGerarArquivoSag(sessao, pecaItemProjeto))
                    {
                        uint? idArquivoMesaCorte = null;
                        int tipoArquivo = (int)TipoArquivoMesaCorte.SAG;

                        using (var outputStream = new MemoryStream())
                        {
                            ArquivoMesaCorteDAO.Instance.GetArquivoMesaCorte(
                                sessao,
                                pp.IdPedido,
                                pp.IdProdPedEsp.Value,
                                null,
                                ref idArquivoMesaCorte,
                                true,
                                outputStream,
                                ref tipoArquivo,
                                false,
                                false,
                                false);

                            if (outputStream.Length > 0)
                            {
                                var dadosArquivoMesaCorte = new ArquivoMesaCorte();
                                dadosArquivoMesaCorte.IdProdPed = (int)pp.IdProdPed;
                                dadosArquivoMesaCorte.TipoArquivo = TipoArquivoMesaCorte.SAG;
                                dadosArquivoMesaCorte.Arquivo = outputStream.ToArray();
                                this.ArquivoMesaCorte.Add(dadosArquivoMesaCorte);
                            }
                        }
                    }

                    using (var outputStream = new MemoryStream())
                    {
                        UtilsProjeto.GerarArquivoCadProject(pecaItemProjeto, true, outputStream);

                        if (outputStream.Length > 0)
                        {
                            var dadosArquivoMesaCorte = new ArquivoMesaCorte();
                            dadosArquivoMesaCorte.IdProdPed = (int)pp.IdProdPed;
                            dadosArquivoMesaCorte.TipoArquivo = TipoArquivoMesaCorte.Todos;
                            dadosArquivoMesaCorte.Arquivo = outputStream.ToArray();
                            this.ArquivoMesaCorte.Add(dadosArquivoMesaCorte);
                        }
                    }
                }
            }

            #endregion

            #region Métodos de suporte

            #region Etiqueta exportação

            private static KeyValuePairSerializable<int, List<string>> RecuperarEtiquetasProdutoPedido(ProdutosPedido produtoPedido, ProdutosPedidoEspelho produtoPedidoEspelho)
            {
                var etiquetasExportacao = new KeyValuePairSerializable<int, List<string>>();

                if (produtoPedidoEspelho != null)
                {
                    // Busca as etiquetas associadas ao produto de pedido espelho.
                    var etiquetasProdutoPedidoEspelho = ProdutoPedidoProducaoDAO.Instance.GetEtiquetasByIdProdPed(produtoPedidoEspelho.IdProdPed);
                    etiquetasExportacao.Value = new List<string>();

                    // Verifica se o produto possui etiquetas.
                    if (!string.IsNullOrWhiteSpace(etiquetasProdutoPedidoEspelho))
                    {
                        // Variável criada para salvar o ID do produto de pedido e as etiquetas dele.
                        etiquetasExportacao.Key = (int)produtoPedido.IdProdPed;
                        // Salva as etiquetas do produto de pedido na variável criada acima.
                        etiquetasExportacao.Value.AddRange(etiquetasProdutoPedidoEspelho.Split(','));
                    }
                }

                return etiquetasExportacao;
            }

            #endregion

            private static void GetImagemProdPed(uint idProdPed, uint? idMaterItemProj, ref List<FiguraProdutoPedido> imagens)
            {
                var idProdPedEsp = ProdutosPedidoDAO.Instance.ObterIdProdPedEsp(idProdPed);
                if (idProdPedEsp.GetValueOrDefault() == 0)
                    return;

                var ppe = ProdutosPedidoEspelhoDAO.Instance.GetForImagemPeca(idProdPedEsp.Value);
                idMaterItemProj = idMaterItemProj > 0 ? idMaterItemProj : ppe.IdMaterItemProj;
                var itensPecasItemProjeto = PecaItemProjetoDAO.Instance.GetItensByProdPedEsp(idProdPedEsp.Value);

                if (itensPecasItemProjeto != null && itensPecasItemProjeto.Count() > 0)
                {
                    foreach (var s in itensPecasItemProjeto)
                    {
                        if (string.IsNullOrEmpty(s))
                            continue;

                        ppe.Item = s.StrParaInt();

                        if (Utils.ArquivoExiste(ppe.ImagemUrl))
                        {
                            byte[] imagem = Utils.GetImageFromRequest(ppe.ImagemUrl);
                            imagens.Add(new FiguraProdutoPedido(idProdPed, idMaterItemProj, ppe.Item, imagem));
                        }
                        else if (PecaProjetoModeloDAO.Instance.ObtemValorCampo<int>("Tipo", string.Format("IdProjetoModelo={0} AND Item={1}",
                            ItemProjetoDAO.Instance.GetIdProjetoModelo(ppe.IdItemProjeto.Value), ppe.Item)) == 1)
                            throw new Exception(string.Format("Todos os itens do tipo Instalação devem ter imagem associada. Projeto: {0}. Url: {1}",
                                ProjetoModeloDAO.Instance.ObtemCodigo(ItemProjetoDAO.Instance.GetIdProjetoModelo(ppe.IdItemProjeto.Value)), ppe.ImagemUrl));
                    }
                }
                /* Chamado 63032. */
                else if (Utils.ArquivoExiste(ppe.ImagemUrl))
                {
                    byte[] imagem = Utils.GetImageFromRequest(ppe.ImagemUrl);
                    imagens.Add(new FiguraProdutoPedido(idProdPed, idMaterItemProj, ppe.Item, imagem));
                }
            }

            #endregion

            public bool UsarEspelho;
            public bool SistemaLite = false;
            public Pedido Pedido;
            public Loja Loja;
            public DadosAmbientePedido[] AmbientesPedido;
            public ProdutosPedido[] ProdutosPedido;
            public List<KeyValuePairSerializable<int, List<string>>> EtiquetasExportacao;
            public FiguraProdutoPedido[] FigurasProdutosPedido;
            public List<ArquivoMesaCorte> ArquivoMesaCorte;
        }

        #endregion

        private List<Item> _itens = new List<Item>();

        public List<Item> Itens
        {
            get { return _itens; }
        }
    }

    #endregion

    public static class UtilsExportacaoPedido
    {
        #region Métodos de serialização

        private static ExportarPedido Deserializar(byte[] arquivo)
        {
            ExportarPedido retorno;
            XmlSerializer s = new XmlSerializer(typeof(ExportarPedido));

            using (MemoryStream m = new MemoryStream(arquivo))
                retorno = s.Deserialize(m) as ExportarPedido;

            return retorno;
        }

        private static byte[] Serializar(ExportarPedido pedido)
        {
            byte[] retorno;
            XmlSerializer s = new XmlSerializer(typeof(ExportarPedido));

            using (MemoryStream m = new MemoryStream())
            {
                s.Serialize(m, pedido);
                m.Position = 0;
                using (BinaryReader r = new BinaryReader(m))
                    retorno = r.ReadBytes((int)m.Length);
            }

            return retorno;
        }

        #endregion

        #region Exportar

        /// <summary>
        /// Exporta os pedidos selecionados para um arquivo, retornando os bytes desse arquivo.
        /// </summary>
        /// <param name="idsPedidos"></param>
        //public static byte[] CriarExportacao(Dictionary<uint, bool> pedidos, string idsProdutos)
        //{
        //    if (pedidos.Count == 0)
        //        throw new Exception("Selecione pelo menos 1 pedido para exportar.");

        //    List<uint> id = new List<uint>();
        //    foreach (string s in idsPedidos.TrimEnd(',').Split(','))
        //        id.Add(Utils.StrToUint(s));

        //    List<uint> idProd = new List<uint>();
        //    foreach (string s in idsProdutos.TrimEnd(',').Split(','))
        //        idProd.Add(Utils.StrToUint(s));

        //    return CriarExportacao(id.ToArray(), idProd.ToArray());
        //}

        /// <summary>
        /// Exporta os pedidos selecionados para um arquivo, retornando os bytes desse arquivo.
        /// </summary>
        /// <param name="idPedido"></param>
        public static byte[] ConfigurarExportacao(Dictionary<uint, bool> pedidos, uint[] idProduto)
        {
            if (pedidos.Count == 0)
                throw new Exception("Selecione pelo menos 1 pedido para exportar.");

            ExportarPedido pedido = new ExportarPedido();
            foreach (KeyValuePair<uint, bool> item in pedidos)
                if (PedidoDAO.Instance.PodeExportar(item.Key))
                    pedido.Itens.Add(new ExportarPedido.Item(item.Key, idProduto, item.Value, false));

            if (pedido.Itens.Count == 0)
                throw new Exception("Selecione apenas pedidos confirmados e finalizados no PCP.");

            return Glass.Arquivos.Compactar(Serializar(pedido));
        }

        #endregion

        #region Importar

        public static string[] Importar(byte[] buffer)
        {
            ExportarPedido pedido = new ExportarPedido();
            ResultadoSalvar retorno = new ResultadoSalvar();

            string[] resultado = new string[3];

            try
            {
                pedido = Deserializar(Glass.Arquivos.Descompactar(buffer));
                retorno = SalvarItens(pedido.Itens);
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException && ex.InnerException != null)
                    ex = ex.InnerException;

                resultado[0] = "1";
                resultado[1] = "Erro na importação do pedido no fornecedor: " + ex.Message;

                return resultado;
            }

            if (!String.IsNullOrEmpty(retorno.NaoInseridos))
            {
                if (!String.IsNullOrEmpty(retorno.Inseridos))
                {
                    resultado[0] = "0";
                    resultado[1] = "Alguns pedidos não foram importados.\n" +
                        "Pedidos importados: " + retorno.Inseridos + "\n" +
                        "Pedidos não importados: " + retorno.NaoInseridos + "\n\n" +
                        "Verifique esse(s) erro(s) com seu fornecedor.";
                    resultado[2] = retorno.Inseridos;
                }
                else
                {
                    resultado[0] = "1";
                    resultado[1] = "Não foi possível importar nenhum pedido.\n" +
                        "Pedidos não importados: " + retorno.NaoInseridos + "\n\n" +
                        "Verifique esse(s) erro(s) com seu fornecedor.";
                }
            }
            else
            {
                resultado[0] = "0";
                resultado[1] = "Importação realizada com sucesso!\n" +
                    "Pedidos: " + retorno.Inseridos;
                resultado[2] = retorno.Inseridos;
            }

            return resultado;
        }

        #endregion

        #region Valida a importação

        private static void ValidarProdAplProc(string codProd, string descrProd, string ncm, string codAplicacao, string codProcesso)
        {
            ValidarProdAplProc(null, codProd, descrProd, ncm, codAplicacao, codProcesso);
        }

        private static void ValidarProdAplProc(GDASession session, string codProd, string descrProd, string ncm, string codAplicacao, string codProcesso)
        {
            var idProd = ProdutoDAO.Instance.ObtemIdProd(session, codProd);
            if (idProd == 0 || ProdutoDAO.Instance.ObtemNcm(session, idProd, 0) != ncm)
                throw new Exception("Produto '" + descrProd + "' (cód. " + codProd +
                    ", NCM " + ncm + ") não encontrado. Verifique se o item está cadastrado corretamente.");

            if (ProdutoDAO.Instance.ObtemValorCampo<int>(session, "situacao", "idProd=" + idProd) == (int)Glass.Situacao.Inativo)
                throw new Exception("Produto '" + descrProd + "' está inativo no fornecedor.");

            if (!string.IsNullOrEmpty(codAplicacao) && EtiquetaAplicacaoDAO.Instance.ObtemIdAplicacaoAtivo(session, codAplicacao).GetValueOrDefault() == 0)
                throw new Exception("Aplicação '" + codAplicacao + "' não encontrada. " +
                    "Verifique se o item está cadastrado corretamente.");

            if (!string.IsNullOrEmpty(codProcesso) && EtiquetaProcessoDAO.Instance.ObtemIdProcessoAtivo(session, codProcesso).GetValueOrDefault() == 0)
                throw new Exception("Processo '" + codProcesso + "' não encontrado. " +
                    "Verifique se o item está cadastrado corretamente.");
        }

        private static void ValidaImportacao(GDASession session, ExportarPedido.Item item)
        {
            if (LojaDAO.Instance.GetLojaByCNPJIE(session, item.Loja.Cnpj, null, true) != null)
                throw new Exception("Não é possível importar um arquivo gerado pela própria empresa.");

            var cli = ClienteDAO.Instance.GetByCpfCnpj(session, item.Loja.Cnpj);
            if (cli == null)
                throw new Exception("Cliente '" + item.Loja.NomeFantasia + "' (CNPJ " + item.Loja.Cnpj +
                    ") não encontrado. Verifique se o CNPJ do cliente está correto.");

            if (cli.IdFormaPagto.GetValueOrDefault(0) == 0)
                throw new Exception("Cliente '" + cli.Nome + "' não tem uma forma de pagamento padrão definida.");

            if (cli.TipoPagto.GetValueOrDefault(0) == 0)
                throw new Exception("Cliente '" + cli.Nome + "' não tem uma parcela padrão definida.");

            if (cli.IdFunc.GetValueOrDefault(0) == 0)
                throw new Exception("Cliente '" + cli.Nome + "' não tem um vendedor definido.");

            if (PedidoDAO.Instance.CodigoClienteUsado(session, 0, (uint)cli.IdCli, item.Pedido.IdPedido.ToString(), true))
                throw new Exception("Já existe um pedido para o cliente '" + cli.Nome + "' com o Cód. Pedido Cli. '" + item.Pedido.IdPedido +
                    "'. Verifique se esse pedido já foi importado ou se o cód. pedido cli. do pedido está correto.");

            if (item.ProdutosPedido == null && item.AmbientesPedido == null)
                throw new Exception("Inclua pelo menos um produto no pedido para exportá-lo.");

            //if (item.Pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.Revenda &&
            //    Glass.Configuracoes.PedidoConfig.DadosPedido.BloquearItensTipoPedido)
            //    throw new Exception("Produtos de Revenda não podem ser inseridos em pedidos de Venda.");

            if (item.ProdutosPedido != null)
                foreach (ProdutosPedido pp in item.ProdutosPedido)
                    ValidarProdAplProc(session, pp.CodInterno, pp.DescrProduto, pp.Ncm, pp.CodAplicacao, pp.CodProcesso);

            else if (item.AmbientesPedido != null)
                for (int i = 0; i < item.AmbientesPedido.Length; i++)
                {
                    if (item.AmbientesPedido[i].AmbientePedido != null && item.Pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObra)
                        ValidarProdAplProc(session, item.AmbientesPedido[i].AmbientePedido.CodInterno, item.AmbientesPedido[i].AmbientePedido.PecaVidro,
                            item.AmbientesPedido[i].AmbientePedido.Ncm, item.AmbientesPedido[i].AmbientePedido.CodAplicacao,
                            item.AmbientesPedido[i].AmbientePedido.CodProcesso);

                    foreach (ProdutosPedido pp in item.AmbientesPedido[i].ProdutosPedido)
                        ValidarProdAplProc(session, pp.CodInterno, pp.DescrProduto, pp.Ncm, pp.CodAplicacao, pp.CodProcesso);
                }
        }

        #endregion

        #region Salva os itens de exportação de pedido

        #region Classes de suporte

        private class DadosPedido
        {
            public uint idPedido, idPedidoOriginal;
            public Dictionary<uint, uint> ambientesPedido = new Dictionary<uint, uint>();
            public Dictionary<uint, List<uint>> produtosPedido = new Dictionary<uint, List<uint>>();
            public Dictionary<uint, uint> itensProjeto = new Dictionary<uint, uint>();
            public Dictionary<uint, KeyValuePair<string, string>> prodPedObs = new Dictionary<uint, KeyValuePair<string, string>>();
        }

        private class ResultadoSalvar
        {
            public string Inseridos, NaoInseridos;
        }

        #endregion

        private static ResultadoSalvar SalvarItens(List<ExportarPedido.Item> itens)
        {
            var dados = new List<DadosPedido>();
            var naoCadastrados = new Dictionary<int, string>();
            var espelhosGerados = new List<int>();

            for (var i = 0; i < itens.Count; i++)
            {
                var valido = false;

                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        dados.Add(new DadosPedido());
                        dados[i].idPedidoOriginal = itens[i].Pedido.IdPedido;

                        ValidaImportacao(transaction, itens[i]);
                        valido = true;

                        #region Busca o cliente

                        var cliente = ClienteDAO.Instance.GetByCpfCnpj(transaction, itens[i].Loja.Cnpj);
                        if (cliente == null)
                            throw new Exception("Cliente não encontrado. CNPJ: " + itens[i].Loja.Cnpj);

                        var parc = cliente.TipoPagto > 0 ? ParcelasDAO.Instance.GetElementByPrimaryKey(transaction, (uint)cliente.TipoPagto.Value) : null;

                        #endregion

                        #region Salva o pedido

                        var rota = RotaDAO.Instance.GetByCliente(transaction, (uint)cliente.IdCli);

                        itens[i].Pedido.IdPedido = 0;
                        itens[i].Pedido.IdCli = (uint)cliente.IdCli;
                        itens[i].Pedido.CodCliente = dados[i].idPedidoOriginal.ToString();
                        itens[i].Pedido.IdFormaPagto = (uint?)cliente.IdFormaPagto;
                        itens[i].Pedido.TipoVenda = parc != null && parc.NumParcelas > 0 ? (int)Pedido.TipoVendaPedido.APrazo : (int)Pedido.TipoVendaPedido.AVista;
                        itens[i].Pedido.IdParcela = (uint?)cliente.TipoPagto;
                        itens[i].Pedido.Situacao = Pedido.SituacaoPedido.Ativo;
                        itens[i].Pedido.IdFunc = (uint)cliente.IdFunc.Value;
                        itens[i].Pedido.Usucad = (uint)cliente.IdFunc.Value;
                        itens[i].Pedido.DataEntrega = DateTime.Now.AddDays(1);
                        itens[i].Pedido.SituacaoProducao = (int)Pedido.SituacaoProducaoEnum.NaoEntregue;

                        if (!Glass.Configuracoes.PedidoConfig.ExportacaoPedido.ManterTipoEntregaPedido)
                        {
                            if (rota != null && rota.EntregaBalcao)
                                itens[i].Pedido.TipoEntrega = (int)Pedido.TipoEntregaPedido.Balcao;
                            else
                                itens[i].Pedido.TipoEntrega = (int?)Configuracoes.PedidoConfig.TipoEntregaPadraoPedido;
                        }

                        // Limpa campos do pedido
                        itens[i].Pedido.IdSinal = null;
                        itens[i].Pedido.IdLiberarPedido = null;
                        itens[i].Pedido.IdComissionado = (uint?)cliente.IdComissionado;
                        itens[i].Pedido.PercComissao = cliente.IdComissionado > 0 ? ComissionadoDAO.Instance.ObtemValorCampo<float>(transaction, "percentual", "idComissionado=" + cliente.IdComissionado) : 0;
                        itens[i].Pedido.IdFormaPagto2 = null;
                        itens[i].Pedido.IdLoja = FuncionarioDAO.Instance.ObtemIdLoja(transaction, (uint)cliente.IdFunc.Value);
                        itens[i].Pedido.IdMedidor = null;
                        itens[i].Pedido.IdObra = null;
                        itens[i].Pedido.IdOrcamento = null;
                        itens[i].Pedido.IdPedidoAnterior = null;
                        itens[i].Pedido.IdProjeto = null;
                        itens[i].Pedido.IdTipoCartao = null;
                        itens[i].Pedido.IdTipoCartao2 = null;
                        itens[i].Pedido.DataCanc = null;
                        itens[i].Pedido.DataConf = null;
                        itens[i].Pedido.DataEntregaOriginal = null;
                        itens[i].Pedido.DataPedido = DateTime.Now;
                        itens[i].Pedido.DataPronto = null;
                        itens[i].Pedido.UsuCanc = null;
                        itens[i].Pedido.UsuConf = null;
                        itens[i].Pedido.Importado = true;
                        itens[i].Pedido.ValorEntrada = 0;
                        // Chamado 48312 - Quando o IdFuncVenda da loja que exportou está inativo na loja que importou, não é possível realizar alteração no pedido.
                        itens[i].Pedido.IdFuncVenda = null;

                        // Remove desconto e acréscimo
                        itens[i].Pedido.TipoDesconto = 1;
                        itens[i].Pedido.Desconto = 0;
                        itens[i].Pedido.TipoAcrescimo = 1;
                        itens[i].Pedido.Acrescimo = 0;

                        dados[i].idPedido = PedidoDAO.Instance.Insert(transaction, itens[i].Pedido);

                        #endregion

                        #region Salva os ambientes

                        if (Glass.Configuracoes.PedidoConfig.DadosPedido.AmbientePedido || itens[i].Pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObra)
                        {
                            if (itens[i].AmbientesPedido != null)
                                for (var j = 0; j < itens[i].AmbientesPedido.Length; j++)
                                {
                                    var idAmbiente = itens[i].AmbientesPedido[j].AmbientePedido.IdAmbientePedido;

                                    var amb = itens[i].AmbientesPedido[j].AmbientePedido;
                                    amb.IdAmbientePedido = 0;
                                    amb.IdPedido = dados[i].idPedido;
                                    amb.IdItemProjeto = null;

                                    if (amb.IdProd > 0)
                                    {
                                        amb.IdProd = (uint)ProdutoDAO.Instance.ObtemIdProd(transaction, amb.CodInterno);

                                        /* Chamado 56335. */
                                        if (itens[0].SistemaLite)
                                        {
                                            amb.IdAplicacao = Configuracoes.InstalacaoConfig.AplicacaoInstalacao ?? Configuracoes.ProjetoConfig.Caixilho.AplicacaoCaixilho;
                                            amb.IdProcesso = Configuracoes.InstalacaoConfig.ProcessoInstalacao ?? Configuracoes.ProjetoConfig.Caixilho.ProcessoCaixilho;
                                        }
                                        else
                                        {
                                            amb.IdAplicacao = EtiquetaAplicacaoDAO.Instance.ObtemIdAplicacao(transaction, amb.CodAplicacao);
                                            amb.IdProcesso = EtiquetaProcessoDAO.Instance.ObtemIdProcesso(transaction, amb.CodProcesso);
                                        }
                                    }
                                    else
                                    {
                                        amb.IdProd = null;
                                        amb.IdAplicacao = null;
                                        amb.IdProcesso = null;
                                    }

                                    // Remove desconto e acréscimo
                                    amb.TipoDesconto = 1;
                                    amb.Desconto = 0;
                                    amb.TipoAcrescimo = 1;
                                    amb.Acrescimo = 0;

                                    dados[i].ambientesPedido.Add(idAmbiente, AmbientePedidoDAO.Instance.Insert(transaction, amb));
                                }
                            else if (itens[i].ProdutosPedido != null && itens[i].ProdutosPedido.Length > 0)
                            {
                                var amb = new AmbientePedido();
                                amb.IdPedido = dados[i].idPedido;
                                amb.Ambiente = "Importação";

                                dados[i].ambientesPedido.Add(0, AmbientePedidoDAO.Instance.Insert(transaction, amb));
                            }
                        }

                        #endregion

                        #region Salva os produtos

                        if (itens[i].AmbientesPedido != null)
                        {
                            // Variável criada para salvar a quantidade de produtos importados.
                            var qtdeTotalProdPedImportado = 0F;
                            // Variável criada para salvar a quantidade total dos produtos do pedido, caso seja diferente da quantidade
                            // importada, então é lançada uma mensagem de erro.
                            var qtdeTotalProdPed = 0F;

                            for (var j = 0; j < itens[i].AmbientesPedido.Length; j++)
                                for (var k = 0; k < itens[i].AmbientesPedido[j].ProdutosPedido.Length; k++)
                                {
                                    var idProdPed = itens[i].AmbientesPedido[j].ProdutosPedido[k].IdProdPed;
                                    var qtdeDividir = Math.Max(itens[i].FigurasProdutosPedido.Count(x => x.IdProdPed == idProdPed), 1);

                                    dados[i].produtosPedido.Add(idProdPed, new List<uint>());

                                    var qtdeOriginal = itens[i].AmbientesPedido[j].ProdutosPedido[k].Qtde;
                                    qtdeTotalProdPed += qtdeOriginal;
                                    var qtdeCalc = qtdeOriginal / qtdeDividir;

                                    for (var l = 0; l < qtdeDividir; l++)
                                    {
                                        var pp = itens[i].AmbientesPedido[j].ProdutosPedido[k];
                                        pp.Qtde = qtdeCalc;
                                        pp.BuscarBenefImportacao = false;
                                        pp.IdPedido = dados[i].idPedido;
                                        pp.IdAmbientePedido = (!Glass.Configuracoes.PedidoConfig.DadosPedido.AmbientePedido && itens[i].Pedido.TipoPedido != (int)Pedido.TipoPedidoEnum.MaoDeObra) ||
                                            // Chamado 45113.
                                            // Quando o ambiente é inserido o sistema atualiza automaticamente esse ID.
                                            pp.IdAmbientePedido.GetValueOrDefault(0) == 0 ? (uint?)null : itens[i].AmbientesPedido[j].AmbientePedido.IdAmbientePedido;
                                        pp.IdProd = (uint)ProdutoDAO.Instance.ObtemIdProd(transaction, pp.CodInterno);

                                        /* Chamado 56335. */
                                        if (itens[0].SistemaLite)
                                        {
                                            pp.IdAplicacao = Configuracoes.InstalacaoConfig.AplicacaoInstalacao ?? Configuracoes.ProjetoConfig.Caixilho.AplicacaoCaixilho;
                                            pp.IdProcesso = Configuracoes.InstalacaoConfig.ProcessoInstalacao ?? Configuracoes.ProjetoConfig.Caixilho.ProcessoCaixilho;
                                        }
                                        else
                                        {
                                            pp.IdAplicacao = EtiquetaAplicacaoDAO.Instance.ObtemIdAplicacao(transaction, pp.CodAplicacao);
                                            pp.IdProcesso = EtiquetaProcessoDAO.Instance.ObtemIdProcesso(transaction, pp.CodProcesso);
                                        }

                                        pp.ValorVendido = ProdutoDAO.Instance.GetValorTabela(transaction, (int)pp.IdProd, itens[i].Pedido.TipoEntrega,
                                            (uint)cliente.IdCli, cliente.Revenda, false, pp.PercDescontoQtde, (int?)pp.IdPedido, null, null, pp.Altura);
                                        pp.QtdSaida = 0;
                                        /* Chamado 24452. */
                                        pp.ObsProjetoExterno = pp.ObsProjeto;

                                        // Atualiza campos de controle do produto pedido
                                        pp.InvisivelAdmin = false;
                                        pp.InvisivelFluxo = false;
                                        pp.InvisivelPedido = false;
                                        pp.QtdeInvisivel = 0;
                                        pp.IdItemProjeto = null;
                                        pp.IdMaterItemProj = null;

                                        // Zera desconto/acréscimo porque o valor vendido será atualizado
                                        // Valores serão recalculados ao finalizar o cadatro do pedido
                                        pp.ValorAcrescimo = 0;
                                        pp.ValorAcrescimoCliente = 0;
                                        pp.ValorAcrescimoProd = 0;
                                        pp.ValorDesconto = 0;
                                        pp.ValorDescontoCliente = 0;
                                        pp.ValorDescontoProd = 0;
                                        pp.ValorDescontoQtde = 0;
                                        pp.ValorTabelaOrcamento = 0;
                                        pp.ValorTabelaPedido = 0;

                                        if (pp.IdAmbientePedido == null)
                                        {
                                            string ambiente = itens[i].AmbientesPedido[j].AmbientePedido.Ambiente;
                                            if (!string.IsNullOrEmpty(pp.PedCli) && !dados[i].prodPedObs.ContainsKey(idProdPed))
                                                dados[i].prodPedObs.Add(idProdPed, new KeyValuePair<string, string>(pp.PedCli, ambiente));

                                            pp.PedCli = ambiente;
                                        }

                                        dados[i].produtosPedido[idProdPed].Add(ProdutosPedidoDAO.Instance.Insert(transaction, pp));
                                    }

                                    itens[i].AmbientesPedido[j].ProdutosPedido[k].Qtde = qtdeOriginal;
                                }

                            // Recupera a quantidade de produtos importada.
                            foreach (var prod in ProdutosPedidoDAO.Instance.GetByPedido(transaction, dados[i].idPedido))
                                qtdeTotalProdPedImportado += prod.Qtde;

                            // Verifica se a quantidade de proputos importada é diferente da quantidade de produtos exportada.
                            // Caso seja diferente, lança uma exceção e informa que o pedido deve ser exportado novamente.
                            if (qtdeTotalProdPed != qtdeTotalProdPedImportado)
                                throw new Exception("A quantidade importada é diferente da quantidade exportada. Exporte o pedido novamente.");
                        }
                        else if (itens[i].ProdutosPedido != null)
                        {
                            // Variável criada para salvar a quantidade de produtos importados.
                            var qtdeTotalProdPedImportado = 0F;
                            // Variável criada para salvar a quantidade total dos produtos do pedido, caso seja diferente da quantidade
                            // importada, então é lançada uma mensagem de erro.
                            var qtdeTotalProdPed = 0F;

                            for (var j = 0; j < itens[i].ProdutosPedido.Length; j++)
                            {
                                var idProdPed = itens[i].ProdutosPedido[j].IdProdPed;
                                var qtdeDividir = Math.Max(itens[i].FigurasProdutosPedido.Count(x => x.IdProdPed == idProdPed), 1);

                                dados[i].produtosPedido.Add(idProdPed, new List<uint>());

                                var qtdeOriginal = itens[i].ProdutosPedido[j].Qtde;
                                qtdeTotalProdPed += qtdeOriginal;
                                var qtdeCalc = qtdeOriginal / qtdeDividir;

                                for (var l = 0; l < qtdeDividir; l++)
                                {
                                    var pp = itens[i].ProdutosPedido[j];
                                    pp.Qtde = qtdeCalc;
                                    pp.BuscarBenefImportacao = false;
                                    pp.IdPedido = dados[i].idPedido;
                                    pp.IdAmbientePedido = (!Glass.Configuracoes.PedidoConfig.DadosPedido.AmbientePedido && itens[i].Pedido.TipoPedido != (int)Pedido.TipoPedidoEnum.MaoDeObra) ||
                                        pp.IdAmbientePedido.GetValueOrDefault(0) == 0 ? (uint?)null : (uint?)dados[i].ambientesPedido[0];
                                    pp.IdProd = (uint)ProdutoDAO.Instance.ObtemIdProd(transaction, pp.CodInterno);
                                    pp.IdAplicacao = EtiquetaAplicacaoDAO.Instance.ObtemIdAplicacao(transaction, pp.CodAplicacao);
                                    pp.IdProcesso = EtiquetaProcessoDAO.Instance.ObtemIdProcesso(transaction, pp.CodProcesso);
                                    pp.ValorVendido = ProdutoDAO.Instance.GetValorTabela(transaction, (int)pp.IdProd, itens[i].Pedido.TipoEntrega, (uint)cliente.IdCli, cliente.Revenda, false, pp.PercDescontoQtde, (int?)pp.IdPedido, null, null, pp.Altura);
                                    pp.QtdSaida = 0;
                                    /* Chamado 24452. */
                                    pp.ObsProjetoExterno = pp.ObsProjeto;

                                    // Atualiza campos de controle do produto pedido
                                    pp.InvisivelAdmin = false;
                                    pp.InvisivelFluxo = false;
                                    pp.InvisivelPedido = false;
                                    pp.QtdeInvisivel = 0;
                                    pp.IdItemProjeto = null;
                                    pp.IdMaterItemProj = null;

                                    // Zera desconto/acréscimo porque o valor vendido foi atualizado
                                    // Valores serão recalculados ao finalizar o cadatro do pedido
                                    pp.ValorAcrescimo = 0;
                                    pp.ValorAcrescimoCliente = 0;
                                    pp.ValorAcrescimoProd = 0;
                                    pp.ValorDesconto = 0;
                                    pp.ValorDescontoCliente = 0;
                                    pp.ValorDescontoProd = 0;
                                    pp.ValorDescontoQtde = 0;
                                    pp.ValorTabelaOrcamento = 0;
                                    pp.ValorTabelaPedido = 0;

                                    dados[i].produtosPedido[idProdPed].Add(ProdutosPedidoDAO.Instance.Insert(transaction, pp));
                                }

                                itens[i].ProdutosPedido[j].Qtde = qtdeOriginal;
                            }

                            // Recupera a quantidade de produtos importada.
                            foreach (var prod in ProdutosPedidoDAO.Instance.GetByPedido(transaction, dados[i].idPedido))
                                qtdeTotalProdPedImportado += prod.Qtde;

                            // Verifica se a quantidade de proputos importada é diferente da quantidade de produtos exportada.
                            // Caso seja diferente, lança uma exceção e informa que o pedido deve ser exportado novamente.
                            if (qtdeTotalProdPed != qtdeTotalProdPedImportado)
                                throw new Exception("A quantidade importada é diferente da quantidade exportada. Exporte o pedido novamente.");
                        }

                        #endregion

                        // Atualiza a data de entrega do pedido
                        PedidoDAO.Instance.SetDataEntregaMinima(transaction, dados[i].idPedido);

                        var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(transaction, dados[i].idPedido);
                        PedidoDAO.Instance.RecalculaParcelas(transaction, ref pedido, PedidoDAO.TipoCalculoParcelas.Ambas);

                        // Atualiza dados do pedido (desconto, acréscimo, comissão, custo, total, parcelas...)
                        PedidoDAO.Instance.Update(transaction, pedido);

                        if (Glass.Configuracoes.PedidoConfig.LiberarPedido)
                        {
                            if (itens[i].UsarEspelho)
                            {
                                #region Gera o espelho, se necessário

                                PedidoDAO.Instance.FinalizarPedido(transaction, dados[i].idPedido, false);

                                var idsPedidoOk = new List<int>();
                                var idsPedidoErro = new List<int>();

                                PedidoDAO.Instance.ConfirmarLiberacaoPedido(transaction, new List<int> { dados[i].idPedido.ToString().StrParaInt() }, out idsPedidoOk, out idsPedidoErro, false);

                                PedidoEspelhoDAO.Instance.GeraEspelho(transaction, dados[i].idPedido);
                                espelhosGerados.Add(i);

                                if (itens[i].AmbientesPedido != null)
                                {
                                    foreach (var a in itens[i].AmbientesPedido)
                                        foreach (var pp in a.ProdutosPedido)
                                            if (!string.IsNullOrEmpty(pp.ObsProjeto))
                                            {
                                                foreach (var id in dados[i].produtosPedido.First(f => f.Value.Any(x => x == pp.IdProdPed)).Value)
                                                {
                                                    uint? idProdPedEsp = ProdutosPedidoDAO.Instance.ObterIdProdPedEsp(transaction, id);
                                                    if (idProdPedEsp > 0)
                                                        ProdutosPedidoEspelhoDAO.Instance.ExecuteScalar<int>(transaction, @"update produtos_pedido_espelho set
                                                            obs=Concat(Coalesce(obs, ''), ' ', ?obs) where idProdPed=" + idProdPedEsp, new GDAParameter("?obs", pp.ObsProjeto));
                                                }
                                            }
                                }
                                else if (itens[i].ProdutosPedido != null)
                                {
                                    foreach (var pp in itens[i].ProdutosPedido)
                                        if (!string.IsNullOrEmpty(pp.ObsProjeto))
                                        {
                                            foreach (var id in dados[i].produtosPedido.First(f => f.Value.Any(x => x == pp.IdProdPed)).Value)
                                            {
                                                var idProdPedEsp = ProdutosPedidoDAO.Instance.ObterIdProdPedEsp(transaction, id);
                                                if (idProdPedEsp > 0)
                                                    ProdutosPedidoEspelhoDAO.Instance.ExecuteScalar<int>(transaction, @"update produtos_pedido_espelho set
                                                        obs=Concat(Coalesce(obs, ''), ' ', ?obs) where idProdPed=" + idProdPedEsp, new GDAParameter("?obs", pp.ObsProjeto));
                                            }
                                        }
                                }

                                foreach (uint idProdPed in dados[i].prodPedObs.Keys)
                                {
                                    foreach (var id in dados[i].produtosPedido[idProdPed])
                                    {
                                        var idProdPedEsp = ProdutosPedidoDAO.Instance.ObterIdProdPedEsp(transaction, id);
                                        if (idProdPedEsp > 0)
                                            ProdutosPedidoEspelhoDAO.Instance.ExecuteScalar<int>(transaction, "update produtos_pedido_espelho set obs=?pedCli where idProdPed=" + idProdPedEsp,
                                                new GDAParameter("?pedCli", dados[i].prodPedObs[idProdPed].Key));
                                    }
                                }

                                PedidoEspelhoDAO.Instance.FinalizarPedido(transaction, dados[i].idPedido);

                                #region Etiquetas exportadas

                                // Salva as etiquetas dos produtos do pedido exportado com a referência dos produtos do pedido importado
                                if (itens[i].EtiquetasExportacao != null)
                                {
                                    foreach (var etiquetaExportacao in itens[i].EtiquetasExportacao)
                                    {
                                        // Caso o produto tenha sido dividido (ocorre somente caso o pedido exportado tenha projeto e a peça do projeto possua quantidade maior que 2,
                                        // definida no cadastro do projeto) salva as mesmas etiquetas para os produtos divididos.
                                        if (dados[i].produtosPedido.ContainsKey((uint)etiquetaExportacao.Key) && dados[i].produtosPedido[(uint)etiquetaExportacao.Key] != null &&
                                            dados[i].produtosPedido[(uint)etiquetaExportacao.Key].Count > 0)
                                        {
                                            for (var j = 0; j < dados[i].produtosPedido[(uint)etiquetaExportacao.Key].Count; j++)
                                            {
                                                foreach (var id in dados[i].produtosPedido[(uint)etiquetaExportacao.Key])
                                                {
                                                    // Recupera o ID do produto do pedido espelho associado.
                                                    var idProdPedEsp = ProdutosPedidoDAO.Instance.ObterIdProdPedEsp(transaction, id);

                                                    if (idProdPedEsp > 0 && etiquetaExportacao.Value != null && etiquetaExportacao.Value.Count > 0)
                                                    {
                                                        // Recupera os produtos de produção dos produtos de pedido espelho criados ao finalizar o pedido espelho.
                                                        var idsProdutoPedidoProducao = ProdutoPedidoProducaoDAO.Instance.ObterIdsProdutoPedidoProducaoPeloIdProdPedEsp(transaction, (int)idProdPedEsp);

                                                        // Percorre cada ID de produto de produção e associa uma etiqueta importada à ele.
                                                        for (var p = 0; p < idsProdutoPedidoProducao.Count; p++)
                                                            if (etiquetaExportacao.Value.Count > p)
                                                                ProdutoPedidoProducaoDAO.Instance.AssociarProdutoPedidoProducaoEtiquetaImportada(transaction, idsProdutoPedidoProducao[p],
                                                                    etiquetaExportacao.Value[p]);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                #endregion

                                foreach (var a in itens[i].ArquivoMesaCorte)
                                {
                                    foreach (var id in dados[i].produtosPedido[(uint)a.IdProdPed])
                                    {
                                        var idProdPedEsp = ProdutosPedidoDAO.Instance.ObterIdProdPedEsp(transaction, id);
                                        if (idProdPedEsp > 0)
                                            ArquivoMesaCorteDAO.Instance.SalvarArquivoMesaCorte(transaction, idProdPedEsp.Value, a);
                                    }
                                }

                                #endregion

                                #region Salva as figuras anexas de produtos/materiais

                                for (int j = 0; j < itens[i].FigurasProdutosPedido.Length; j++)
                                {
                                    var lstPos = new Dictionary<int, int>();
                                    var contadorItem = 0;

                                    foreach (var figuraProdutosPedido in itens[i].FigurasProdutosPedido.
                                        Where(x => x.IdProdPed == itens[i].FigurasProdutosPedido[j].IdProdPed))
                                        lstPos.Add(figuraProdutosPedido.Item.Value, contadorItem++);

                                    var idProdPed = dados[i].produtosPedido[itens[i].FigurasProdutosPedido[j].IdProdPed]
                                        [lstPos[itens[i].FigurasProdutosPedido[j].Item.Value]];
                                    idProdPed = ProdutosPedidoDAO.Instance.GetIdProdPedEspByProdPed(transaction, idProdPed);

                                    if (idProdPed == 0)
                                        continue;

                                    var ppe = ProdutosPedidoEspelhoDAO.Instance.GetForImagemPeca(transaction, idProdPed);

                                    ManipulacaoImagem.SalvarImagem(HttpContext.Current.Server.MapPath(ppe.ImagemUrlSalvar), itens[i].FigurasProdutosPedido[j].Figura);

                                    if (!System.IO.File.Exists(HttpContext.Current.Server.MapPath(ppe.ImagemUrlSalvar)) || itens[i].FigurasProdutosPedido[j].Figura == null)
                                        throw new Exception("Não foi possível salvar uma das imagens do pedido. Exporte-o novamente, caso o problema persista entre em contato com o suporte WebGlass.");
                                }

                                #endregion
                            }
                            else
                            {
                                PedidoDAO.Instance.FinalizarPedido(transaction, dados[i].idPedido, false);
                            }
                        }

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        if (valido)
                        {
                            // Salva o erro apenas se o pedido passar pela validação
                            var url = HttpContext.Current != null ? HttpContext.Current.Request.Url.ToString() : null;
                            ErroDAO.Instance.InserirFromException(url, new ImportarPedidoException(dados[i].idPedidoOriginal > 0 ? (uint?)dados[i].idPedido : null, ex));
                        }

                        naoCadastrados.Add(i, ex.Message);
                    }
                }
            }

            ResultadoSalvar retorno = new ResultadoSalvar();
            retorno.Inseridos = string.Empty;
            retorno.NaoInseridos = string.Empty;

            for (var i = 0; i < dados.Count; i++)
            {
                if (naoCadastrados.ContainsKey(i))
                    retorno.NaoInseridos += dados[i].idPedidoOriginal + " (motivo: " + naoCadastrados[i] + "), ";
                else
                    retorno.Inseridos += dados[i].idPedidoOriginal + (espelhosGerados.Contains(i) ? " (PCP)" : "") + ", ";
            }

            retorno.Inseridos = retorno.Inseridos.TrimEnd(' ', ',');
            retorno.NaoInseridos = retorno.NaoInseridos.TrimEnd(' ', ',');

            return retorno;
        }

        #endregion

        #region Validar Pedidos

        /// <summary>
        /// Verifica se os pedidos passados estão exportados corretamente
        /// </summary>
        public static string[] VerificarExportacaoPedidos(byte[] buffer)
        {
            var retorno = new List<string>();
            var pedido = Deserializar(Arquivos.Descompactar(buffer));

            var cli = ClienteDAO.Instance.GetByCpfCnpj(null, pedido.Itens[0].Loja.Cnpj);

            foreach (var item in pedido.Itens)
            {
                var exportado = PedidoDAO.Instance.CodigoClienteUsado(null, 0, (uint)cli.IdCli, item.Pedido.IdPedido.ToString(), true);
                retorno.Add(string.Format("{0}|{1}", item.Pedido.IdPedido, exportado));
            }

            return retorno.ToArray();
        }

        #endregion

        #region Atualizar Exportação

        /// <summary>
        /// Cria uma nova Exportação
        /// </summary>
        public static void CriarExportacao(uint idFornec, uint[] pedidos, Dictionary<uint, List<uint>> idsProdutosPedido)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    Exportacao nova = new Exportacao();
                    nova.IdFornec = idFornec;
                    nova.IdFunc = Helper.UserInfo.GetUserInfo.CodUser;
                    nova.DataExportacao = DateTime.Now;
                    uint idExportacao = ExportacaoDAO.Instance.Insert(nova);

                    foreach (uint item in pedidos)
                        PedidoExportacaoDAO.Instance.InserirSituacaoExportado(transaction, idExportacao, item,
                            (int)PedidoExportacao.SituacaoExportacaoEnum.Exportando);

                    foreach (KeyValuePair<uint, List<uint>> i in idsProdutosPedido)
                        foreach (uint id in i.Value)
                            ProdutoPedidoExportacaoDAO.Instance.InserirExportado(transaction, idExportacao, i.Key, id);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// Processa o retorno da exportação
        /// </summary>
        public static void ProcessarDadosExportacao(string[] dados, Dictionary<uint, bool> listaPedidos)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    uint[] idsPedidosExportados;

                    if (dados[0] == "0")
                    {
                        if (dados.Length > 2 && dados[2] != null)
                        {
                            idsPedidosExportados = Array.ConvertAll<string, uint>(dados[2].Split(','),
                                delegate (string x) { return Glass.Conversoes.StrParaUint(x.Replace(" (PCP)", "")); });
                        }
                        else
                        {
                            idsPedidosExportados = new uint[listaPedidos.Count];
                            listaPedidos.Keys.CopyTo(idsPedidosExportados, 0);
                        }

                        foreach (uint item in idsPedidosExportados)
                        {
                            PedidoExportacaoDAO.Instance.AtualizarSituacao(transaction, item,
                                (int)PedidoExportacao.SituacaoExportacaoEnum.Exportado);

                            listaPedidos.Remove(item);
                        }

                        foreach (var item in listaPedidos)
                        {
                            PedidoExportacaoDAO.Instance.AtualizarSituacao(transaction, item.Key,
                                (int)PedidoExportacao.SituacaoExportacaoEnum.Cancelado);
                        }
                    }
                    else
                    {
                        foreach (var item in listaPedidos)
                            PedidoExportacaoDAO.Instance.AtualizarSituacao(transaction, item.Key,
                                (int)PedidoExportacao.SituacaoExportacaoEnum.Cancelado);
                    }

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// Atualiza a situação dos pedidos no cliente.
        /// </summary>
        /// <param name="dados">Dados para exportação.</param>
        public static void AtualizarPedidosExportacao(string[] dados)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    AtualizarPedidosExportacao(transaction, dados);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// Atualiza a situação dos pedidos no cliente.
        /// </summary>
        /// <param name="session">A sessão atual para transação.</param>
        /// <param name="dados">Dados para exportação.</param>
        public static void AtualizarPedidosExportacao(GDASession session, string[] dados)
        {
            try
            {
                foreach (var item in dados)
                {
                    var pedidoExportado = new Tuple<uint, bool>(item.Split('|')[0].StrParaUint(), bool.Parse(item.Split('|')[1]));

                    var situacao = PedidoExportacaoDAO.Instance.GetSituacaoExportacao(pedidoExportado.Item1);

                    if (situacao == 0)
                    {
                        throw new Exception($"Não há registro de exportação para o pedido {pedidoExportado.Item1} no sistema, crie uma exportação para ele para que a mesma possa ser atualizada. ");
                    }

                    if (pedidoExportado.Item2 && (situacao == PedidoExportacao.SituacaoExportacaoEnum.Exportando || situacao == PedidoExportacao.SituacaoExportacaoEnum.Cancelado))
                    {
                        PedidoExportacaoDAO.Instance.AtualizarSituacao(session, pedidoExportado.Item1, (int)PedidoExportacao.SituacaoExportacaoEnum.Exportado);
                    }
                    else if (!pedidoExportado.Item2 && (situacao != PedidoExportacao.SituacaoExportacaoEnum.Cancelado))
                    {
                        PedidoExportacaoDAO.Instance.AtualizarSituacao(session, pedidoExportado.Item1, (int)PedidoExportacao.SituacaoExportacaoEnum.Cancelado);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion
    }
}
