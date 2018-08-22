using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Data.RelModel;
using Glass.Data.RelDAL;
using System.Linq;
using Glass.Configuracoes;
using System.Xml;
using System.IO;

namespace Glass.Data.DAL
{
   
    public sealed class ImpressaoEtiquetaDAO : BaseDAO<ImpressaoEtiqueta, ImpressaoEtiquetaDAO>
    {
        /// <summary>
        /// Classe auxiliar para montar a composição de peçãs de vidro duplo e laminado.
        /// </summary>
        private class Composicao
        {
            #region Contrutor Padrão

            /// <summary>
            /// 
            /// </summary>
            /// <param name="idProdPedPai">IdProdPed do pai da composição</param>
            /// <param name="numEtiquetaPai">Num. da etiqueta do pai da composição</param>
            /// <param name="filhos">Filhos da composição(IdprodPed, Qtde e QtdeGerada</param>
            public Composicao(uint idProdPedPai, string numEtiquetaPai, Dictionary<uint, KeyValuePair<int, int>> filhos)
            {
                IdProdPedPai = idProdPedPai;
                NumEtiquetaPai = numEtiquetaPai;
                Filhos = filhos;
            }

            #endregion

            #region Propiedades

            public uint IdProdPedPai { get; set; }

            public string NumEtiquetaPai { get; set; }

            /// <summary>
            /// IdProdPed do Filho, Qtde e Qtde Gerada
            /// </summary>
            public Dictionary<uint, KeyValuePair<int, int>> Filhos { get; set; }

            #endregion
        }

        //private ImpressaoEtiquetaDAO() { }

        #region Busca produtos para listagem padrão

        private string Sql(uint idPedido, uint numeroNFe, uint idImpressao, string planoCorte, string lote, string dataIni, string dataFim, string etiqueta, 
            int tipoImpressao, bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            filtroAdicional = "";

            var campos = selecionar ? "i.*, l.NomeFantasia as NomeLoja, f.Nome as NomeFunc, if((" +
                SqlTipoImpressao("i.idImpressao", "idPedido") + "), " + (int)ProdutoImpressaoDAO.TipoEtiqueta.Pedido +
                ", if((" + SqlTipoImpressao("i.idImpressao", "idNf") + "), " + (int)ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal +
                ", if((" + SqlTipoImpressao("i.idImpressao", "IdRetalhoProducao") + "), " + (int)ProdutoImpressaoDAO.TipoEtiqueta.Retalho +
                ", " + (int)ProdutoImpressaoDAO.TipoEtiqueta.Box + "))) as tipoImpressao, so.IdArquivoOtimiz AS IdArquivoOtimizacao" : "Count(*)";

            var sql = "Select " + campos + @" From impressao_etiqueta i 
                Left Join loja l On (i.idLoja=l.idLoja) 
                Left Join solucao_otimizacao so ON (i.IdSolucaoOtimizacao=so.IdSolucaoOtimizacao)
                Left Join funcionario f On (i.idFunc=f.idFunc) Where 1 " + FILTRO_ADICIONAL;

            if (idPedido > 0)
            {
                sql += " And (" + idPedido + @" in (select idPedido from produto_impressao 
                    where idImpressao=i.idImpressao) OR " + idPedido + @" IN 
                    (select pp.idPedido 
                    from produtos_pedido pp 
                        inner join produto_impressao pi ON (pi.IdProdPedBox = pp.IdProdPed) 
                    where pi.idImpressao = i.IdImpressao))";

                temFiltro = true;
            }

            if (numeroNFe > 0)
            {
                sql += " And " + numeroNFe + @" in (select numeroNFe from nota_fiscal nf
                    inner join produto_impressao pi on (nf.idNf=pi.idNf) where pi.idImpressao=i.idImpressao)";

                temFiltro = true;
            }

            if (idImpressao > 0)
                filtroAdicional += " And i.idImpressao=" + idImpressao;

            if (!string.IsNullOrEmpty(planoCorte))
                filtroAdicional += " And i.idImpressao In (Select idImpressao From produto_impressao Where planoCorte LIKE ?planoCorte)";

            if (!string.IsNullOrEmpty(lote))
                filtroAdicional += " AND i.IdImpressao IN (SELECT IdImpressao FROM produto_impressao WHERE Lote=?lote)";

            if (!string.IsNullOrEmpty(dataIni))
                filtroAdicional += " And i.Data>=?dataIni";

            if (!string.IsNullOrEmpty(dataFim))
                filtroAdicional += " And i.Data<=?dataFim";

            if (!string.IsNullOrEmpty(etiqueta))
            {
                var dadosEtiqueta = etiqueta.Split('-', '.', '/');

                if (dadosEtiqueta.Length != 4)
                    filtroAdicional += " and false";
                else
                {
                    var campoBusca = dadosEtiqueta[0][0] == 'N' ? "idNf" : 
                        dadosEtiqueta[0][0] == 'R' ? "idRetalhoProducao" : "idPedido";

                    dadosEtiqueta[0] = campoBusca + "=" + dadosEtiqueta[0].TrimStart('N', 'R');

                    filtroAdicional += string.Format(@" and i.idImpressao in (select idImpressao from produto_impressao
                        where {0} and posicaoProd={1} and itemEtiqueta={2} and qtdeProd={3})", dadosEtiqueta);
                }
            }

            if (tipoImpressao > 0)
            {
                sql += " having tipoImpressao=" + tipoImpressao;
                temFiltro = true;
            }

            return sql;
        }

        public IList<ImpressaoEtiqueta> GetList(uint idPedido, uint numeroNFe, uint idImpressao, string planoCorte, string lote, string dataIni, 
            string dataFim, string etiqueta, int tipoImpressao, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = String.IsNullOrEmpty(sortExpression) ? "i.idImpressao desc " : sortExpression;

            bool temFiltro;
            string filtroAdicional;

            return LoadDataWithSortExpression(Sql(idPedido, numeroNFe, idImpressao, planoCorte, lote, dataIni, dataFim, etiqueta, tipoImpressao, true, out temFiltro, 
                out filtroAdicional), sortExpression, startRow, pageSize, temFiltro, filtroAdicional, GetParam(planoCorte, lote, dataIni, dataFim, etiqueta));
        }

        public int GetCount(uint idPedido, uint numeroNFe, uint idImpressao, string planoCorte, string lote, string dataIni, string dataFim, string etiqueta, int tipoImpressao)
        {
            bool temFiltro;
            string filtroAdicional;

            return GetCountWithInfoPaging(Sql(idPedido, numeroNFe, idImpressao, planoCorte, lote, dataIni, dataFim, etiqueta, tipoImpressao, true, out temFiltro,
                out filtroAdicional), temFiltro, filtroAdicional, GetParam(planoCorte, lote, dataIni, dataFim, etiqueta));
        }

        private GDAParameter[] GetParam(string planoCorte, string lote, string dataIni, string dataFim, string etiqueta)
        {
            var lstParam = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(planoCorte))
                lstParam.Add(new GDAParameter("?planoCorte", $"%{planoCorte}%"));

            if (!string.IsNullOrEmpty(lote))
                lstParam.Add(new GDAParameter("?lote", lote));

            if (!string.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!string.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            if (!string.IsNullOrEmpty(etiqueta))
                lstParam.Add(new GDAParameter("?etiqueta", etiqueta));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Exporta arquivo de otimização para o corte certo

        /// <summary>
        /// Exporta arquivo txt com dados para inserção no Corte Certo
        /// </summary>
        public string ArquivoOtimizacaoCorteCerto(uint idFunc, uint idImpressao, string etiquetas, bool ignorarExportadas,
            ref List<Etiqueta> lstEtiqueta)
        {
            string arqOtimiz = "";

            // Salva uma lista com os pedidos usados para montar a etiqueta
            var dicPedidos = new Dictionary<uint, Pedido>();

            // Salva uma lista com os produtos usados para montar a etiqueta
            var dicProd = new Dictionary<uint, Produto>();

            // Se o idImpressão tiver sido passado, apenas recupera lista de produtosImpresao
            if (idImpressao > 0)
                lstEtiqueta = new List<Glass.Data.RelModel.Etiqueta>(Glass.Data.RelDAL.EtiquetaDAO.Instance.GetListPedidoComTransacao(
                    idFunc, idImpressao.ToString(), 0, 0, null, true, true, null, false, null, null));
            else
            {
                string[] lista = etiquetas.TrimEnd('|').Split('|');
                /* Chamado 45983. */
                var idLojaPedidoAux = 0;

                List<uint> idProdutoPedidoApagar = new List<uint>();

                bool sql2Executado = false;

                // Para cada produto a ser impresso, pega os valores para serem inseridos no BD
                foreach (string item in lista)
                {
                    string[] campos = item.Split(';');

                    // Busca produtoPedidoEspelho
                    campos[0] = campos[0].Replace("R", "").Replace("A", "");
                    var prodPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetForArquivoOtimizacao(campos[0].StrParaUint());

                    #region Verifica se todos pedidos são da mesma loja

                    if (EtiquetaConfig.RelatorioEtiqueta.ModeloEtiquetaPorLoja)
                    {
                        var idLojaPedido = (int)PedidoDAO.Instance.ObtemIdLoja(null, prodPedEsp.IdPedido);

                        if (idLojaPedidoAux == 0)
                            idLojaPedidoAux = idLojaPedido;
                        else if (idLojaPedidoAux != idLojaPedido)
                            throw new Exception("Não é possível otimizar etiquetas de pedidos associados a lojas diferentes, pois, há um modelo de etiqueta por loja.");
                    }

                    #endregion

                    bool isPecaReposta = prodPedEsp.PecaReposta;

                    // Busca a descrição do beneficiamento
                    string descrBenef = ProdutoPedidoEspelhoBenefDAO.Instance.GetDescrBenef(prodPedEsp.IdProdPed);

                    // Apaga as etiquetas desse pedido
                    if (!idProdutoPedidoApagar.Contains(prodPedEsp.IdProdPed))
                    {
                        idProdutoPedidoApagar.Add(prodPedEsp.IdProdPed);
                        ApagarEtiquetasOtimizacao(prodPedEsp.IdProdPed, !sql2Executado, true);
                        sql2Executado = true;
                    }

                    if (campos.Length >= 3 && !String.IsNullOrEmpty(campos[3]))
                        isPecaReposta = ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(campos[3].Split('_')[0], false);

                    // Pega a quantidade a ser impresso deste item
                    int qtdAImprimir = campos[1].StrParaInt();
                    int qtdGerada = 0;
                    int posItem = 0; // Utilizado para verificar se o item da peça já foi impresso
                    int prodPedEspQtde = (int)prodPedEsp.Qtde;

                    // Pega a posição deste item no pedido
                    int pos = ProdutosPedidoEspelhoDAO.Instance.GetProdPosition(null, prodPedEsp.IdPedido, prodPedEsp.IdProdPed);

                    //Se for produto de composição ajusta as quantidades
                    if (prodPedEsp.IdProdPedParent.GetValueOrDefault(0) > 0)
                    {
                        prodPedEspQtde *= (int)ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(null, prodPedEsp.IdProdPedParent.Value);

                        var idProdPedParentPai = ProdutosPedidoEspelhoDAO.Instance.ObterIdProdPedParent(null, prodPedEsp.IdProdPedParent.Value);
                        if (idProdPedParentPai.GetValueOrDefault(0) > 0)
                        {
                            prodPedEspQtde *= (int)ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(null, idProdPedParentPai.Value); ;
                        }
                    }

                    // Verifica se todas as etiquetas deste produto já foram impressas
                    if (prodPedEsp.QtdImpresso >= prodPedEspQtde && !isPecaReposta)
                        continue;

                    List<KeyValuePair<string, string>> temp = new List<KeyValuePair<string, string>>();

                    // Gera a qtd de etiquetas requisitadas
                    // Se não for peça reposta ou se a qtd a imprimir for maior que 0 (necessário para os casos em que uma das etiquetas foi reposta),
                    // imprime as peças não repostas
                    if (!isPecaReposta || qtdAImprimir > 0)
                    {
                        while (qtdGerada < qtdAImprimir)
                        {
                            if (ProdutosPedidoEspelhoDAO.Instance.PossuiFilhosComposicao(null, prodPedEsp.IdProdPed))
                                throw new Exception("Produtos principais da composição não podem ser exportados.");

                            if (ignorarExportadas && posItem >= prodPedEspQtde)
                                break;

                            string numEtiqueta = Glass.Data.RelDAL.EtiquetaDAO.Instance
                                .GetNumEtiqueta(prodPedEsp.IdPedido, pos, posItem + 1, prodPedEspQtde, ProdutoImpressaoDAO.TipoEtiqueta.Pedido);

                            // Valida o número da etiqueta
                            ProdutoPedidoProducaoDAO.Instance.ValidaEtiquetaProducao(null, ref numEtiqueta);

                            posItem++;

                            // Verifica se esta etiqueta já foi impressa
                            if (ProdutoImpressaoDAO.Instance.EstaImpressa(numEtiqueta, ProdutoImpressaoDAO.TipoEtiqueta.Pedido))
                                continue;

                            //Verifica se deve ignorar etiquetas ja exportas e se a mesma realmente ja foi exportada.
                            if (ignorarExportadas && ArquivoOtimizacaoDAO.Instance.EtiquetaExportada(numEtiqueta))
                                continue;

                            int adicAlt = 0;
                            int adicLarg = 0;

                            Glass.Data.RelModel.Etiqueta etiqueta = new Glass.Data.RelModel.Etiqueta
                            {
                                BarCodeData = numEtiqueta,
                                NumEtiqueta = numEtiqueta,
                                CodCliente =
                                    PedidoDAO.Instance.ObtemValorCampo<string>("codCliente",
                                        "idPedido=" + prodPedEsp.IdPedido),
                                IdCliente = PedidoDAO.Instance.ObtemIdCliente(null, prodPedEsp.IdPedido)
                            };
                            etiqueta.NomeCliente = ClienteDAO.Instance.GetNome(etiqueta.IdCliente);
                            etiqueta.IdPedido = prodPedEsp.IdPedido.ToString();
                            etiqueta.NumItem = pos + " . " + posItem + "/" + prodPedEspQtde;
                            etiqueta.DataImpressao = DateTime.Now;
                            etiqueta.DescrProd = prodPedEsp.DescrProduto;
                            etiqueta.CodApl = prodPedEsp.CodAplicacao;
                            etiqueta.CodProc = prodPedEsp.CodProcesso;
                            etiqueta.CodOtimizacao = prodPedEsp.CodOtimizacao;
                            etiqueta.Altura = (prodPedEsp.AlturaProducao + adicAlt).ToString(CultureInfo.InvariantCulture);
                            etiqueta.Largura = (prodPedEsp.LarguraProducao + adicLarg).ToString();
                            etiqueta.AlturaLargura = etiqueta.Altura + " X " + etiqueta.Largura;
                            etiqueta.Obs = !PCPConfig.Etiqueta.NaoExibirObsPecaAoImprimirEtiqueta ? prodPedEsp.Obs + " " + descrBenef : descrBenef;
                            etiqueta.DataPedido = PedidoDAO.Instance.ObtemValorCampo<DateTime>("dataPedido", "idPedido=" + prodPedEsp.IdPedido);

                            if (PCPConfig.ExibirLarguraAlturaCorteCerto)
                                etiqueta.AlturaLargura = etiqueta.Largura + " X " + etiqueta.Altura;

                            etiqueta.DataEntrega = PedidoDAO.Instance.ObtemDataEntrega(null, prodPedEsp.IdPedido).GetValueOrDefault();

                            lstEtiqueta.Add(etiqueta);

                            qtdGerada++;
                        }
                    }
                    else
                    {
                        ProdutoImpressao prodImp;

                        if (campos.Length >= 3 && !String.IsNullOrEmpty(campos[3]))
                        {
                            foreach (string etiq in campos[3].Split('_'))
                            {
                                prodImp = ProdutoImpressaoDAO.Instance.GetElementByEtiqueta(etiq,
                                    ProdutoImpressaoDAO.TipoEtiqueta.Pedido);

                                lstEtiqueta.Add(Glass.Data.RelDAL.EtiquetaDAO.Instance.MontaEtiqueta(null, idFunc, prodImp,
                                    prodPedEsp.DescrBeneficiamentos, null, ref temp, ref dicPedidos, ref dicProd, true));
                            }
                        }
                        else
                        {
                            prodImp = ProdutoImpressaoDAO.Instance.GetElementByEtiqueta(prodPedEsp.NumEtiqueta,
                                ProdutoImpressaoDAO.TipoEtiqueta.Pedido);

                            lstEtiqueta.Add(Glass.Data.RelDAL.EtiquetaDAO.Instance.MontaEtiqueta(null, idFunc, prodImp, prodPedEsp.DescrBeneficiamentos,
                                null, ref temp, ref dicPedidos, ref dicProd, true));
                        }
                    }
                }
            }

            int cont = 1;

            foreach (Glass.Data.RelModel.Etiqueta etiqueta in lstEtiqueta)
            {
                arqOtimiz +=
                    cont.ToString().PadLeft(7, '0') + " " + // Posição da peça no arquivo
                    "0000001 " + // Quantidade dessa peça
                    etiqueta.Largura.PadLeft(7, '0') + " " + // Largura da peça
                    etiqueta.Altura.PadLeft(7, '0') + " " + // Altura da peça
                    etiqueta.CodOtimizacao + " " + // Código da cor utilizado no Corte Certo
                    (!String.IsNullOrEmpty(etiqueta.BarCodeData) ? etiqueta.BarCodeData : etiqueta.IdPedido) + "\r\n"; // Label que irá aparecer na etiqueta

                cont++;
            }

            return arqOtimiz;
        }

        #endregion

        #region Exporta arquivo de otimização para Opty Way

        /// <summary>
        /// Retorna o desconto da lapidação aplicável para cada empresa
        /// </summary>
        public float GetAresta(GDASession session, int idProd, uint? idArquivoMesaCorte, List<int> idsBenefConfig, string descrBenef, int idProcesso)
        {
            if (PCPConfig.UsarNovoControleAresta && !string.IsNullOrEmpty(PCPConfig.ObtemArestaConfig))
                return GetArestaNovo(session, idProd, idProcesso, idArquivoMesaCorte, idsBenefConfig);

            var aresta = 0F;
            var espessura = ProdutoDAO.Instance.ObtemEspessura(session, idProd);
            var codInternoProd = ProdutoDAO.Instance.GetCodInterno(session, idProd);
            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(session, idProd);
            var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(session, idProd);

            // Não gera aresta se for .fml básico.
            if (idArquivoMesaCorte != null && (idArquivoMesaCorte.GetValueOrDefault() > 0 &&
                (TipoArquivoMesaCorte)ArquivoMesaCorteDAO.Instance.ObtemTipoArquivo(session, idArquivoMesaCorte.Value) == TipoArquivoMesaCorte.FMLBasico))
            {
                return 0;
            }

            switch (System.Configuration.ConfigurationManager.AppSettings["sistema"].ToLower())
            {                    
                case "vidrorapido":
                    {
                        bool lapidado = descrBenef.ToLower().Contains("lap");
                        bool bisote = descrBenef.ToLower().Contains("bis") || descrBenef.ToLower().Contains("biz");
                        bool temperado = Glass.Data.DAL.GrupoProdDAO.Instance.IsVidroTemperado(session, idGrupoProd, idSubgrupoProd);
                        bool sier = idSubgrupoProd > 0 ? SubgrupoProdDAO.Instance.GetDescricao(session, idSubgrupoProd.Value).ToLower().Contains("sier") : false;

                        bool lapidacaoReta = descrBenef.ToLower().Contains("lapidação reta");
                        bool lapidacaoPainel = descrBenef.ToLower().Contains("lapidação painel");

                        if (sier)
                        {
                            aresta = espessura < 5 ? 4 : 5;
                            break;
                        }

                        aresta =
                            espessura == 4 && temperado ? 3 :
                            (idSubgrupoProd > 0 ? SubgrupoProdDAO.Instance.GetDescricao(session, idSubgrupoProd.Value).ToLower() :
                                "").Contains("espelho") && espessura <= 5 ? 3 :
                            espessura <= 8 && lapidado ? 3 :
                            lapidado && espessura <= 10 ? 3 :
                            lapidado && espessura > 10 ? 5 :
                            espessura <= 5 ? 3 :
                            espessura <= 8 && temperado ? 3 :
                            espessura == 10 && temperado ? 4 :
                            5;

                        if (bisote || lapidacaoReta || lapidacaoPainel)
                            aresta = 5;

                        break;
                    }

                case "pontovidraceiro":
                    aresta = 2;
                    break;

                case "contemper":
                    aresta = idSubgrupoProd == (uint)Utils.SubgrupoProduto.BoxPadrao ? 2 :
                        espessura < 8 ? 2 :
                        espessura == 8 ? 3 : 4;
                    break;
                    
                case "tempermax":
                    aresta = 4;
                    break;

                case "noroestevidros":
                    {
                        var lapidacao = descrBenef.ToLower().Contains("lap");

                        aresta = idSubgrupoProd == 2 || idSubgrupoProd == 5 || idSubgrupoProd == 9 || idSubgrupoProd == 10 || lapidacao ? 2 : 0;
                        break;
                    }
                    
                case "amazonrecife":
                    aresta = 3;
                    break;

                case "divine":
                    aresta = 2;
                    break;

                case "invitra":
                    aresta = espessura == 6 || espessura == 8 ? 2 :
                        espessura == 10 ? 3 : 0;
                    break;

                case "msvidros":
                    aresta = 
                        idSubgrupoProd == 3 || idSubgrupoProd == 4 || idSubgrupoProd == 7 || idSubgrupoProd == 9 ||
                        idSubgrupoProd == 216 || idSubgrupoProd == 217 || idSubgrupoProd == 218 || idSubgrupoProd == 229 ? 0 : 
                        
                        (idSubgrupoProd == 10 || idSubgrupoProd == 233) && ProdutoDAO.Instance.ObtemValorCampo<int>(session, "Altura", "idProd=" + idProd) == 1900 ? 0 : 4;
                    break;

                case "emvidros":
                    {
                        var lapidacao = descrBenef.ToLower().Contains("lap");
                        var bisote = descrBenef.ToLower().Contains("bis") || descrBenef.ToLower().Contains("biz");

                        aresta = (idSubgrupoProd == 1 || idSubgrupoProd == 7 || idSubgrupoProd == 65) && !lapidacao && !bisote ? 0 : 3;
                        break;
                    }

                case "estruturalvidros":
                    {
                        var bisote = descrBenef.ToLower().Contains("bis") || descrBenef.ToLower().Contains("biz");
                        aresta = idSubgrupoProd == 6 && espessura <= 6 && bisote ? 2 : 4;
                        break;
                    }
            }

            return aresta;
        }

        /// <summary>
        /// Obtem a aresta
        /// </summary>
        public float GetArestaNovo(GDASession session, int idProd, int idProcesso, uint? idArquivoMesaCorte, List<int> idsBenefConfig)
        {
            var aresta = 0F;
            var espessura = ProdutoDAO.Instance.ObtemEspessura(session, idProd);
            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(session, idProd);
            var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(session, idProd);

            // Não gera aresta se for .fml básico.
            if (idArquivoMesaCorte != null && (idArquivoMesaCorte.GetValueOrDefault() > 0 &&
                (TipoArquivoMesaCorte)ArquivoMesaCorteDAO.Instance.ObtemTipoArquivo(session, idArquivoMesaCorte.Value) == TipoArquivoMesaCorte.FMLBasico))
            {
                return 0;
            }

            var configAresta = PCPConfig.ObtemArestaConfig;

            int index = 0;
            var dados = configAresta.Split('|')[0].Split(';').Select(f => new
            {
                IdSubgrupoProd = configAresta.Split('|')[0].Split(';')[index].StrParaInt(),
                IdBenefConfig = configAresta.Split('|')[1].Split(';')[index].StrParaInt(),
                CondEspessura = configAresta.Split('|')[2].Split(';')[index].StrParaInt(),
                Espessura = configAresta.Split('|')[3].Split(';')[index].StrParaFloat(),
                Aresta = configAresta.Split('|')[4].Split(';')[index].StrParaFloat(),
                IdProcesso = configAresta.Split('|')[5].Split(';')[index++].StrParaInt(),
            }).ToList();

            foreach (var d in dados)
            {
                var validacaoSubgrupo = false;
                var validacaoBenefConfig = false;
                var validacaoEspessura = false;
                var validacaoProcesso= false;

                if (d.IdSubgrupoProd > 0)
                    validacaoSubgrupo = d.IdSubgrupoProd == idSubgrupoProd;
                else
                    validacaoSubgrupo = true;

                if (d.IdBenefConfig > 0)
                    validacaoBenefConfig = idsBenefConfig.Contains(d.IdBenefConfig);
                else
                    validacaoBenefConfig = true;

                if (d.CondEspessura > 0)
                {
                    switch (d.CondEspessura)
                    {
                        case 1:
                            validacaoEspessura = espessura == d.Espessura;
                            break;
                        case 2:
                            validacaoEspessura = espessura != d.Espessura;
                            break;
                        case 3:
                            validacaoEspessura = espessura > d.Espessura;
                            break;
                        case 4:
                            validacaoEspessura = espessura < d.Espessura;
                            break;
                        case 5:
                            validacaoEspessura = espessura >= d.Espessura;
                            break;
                        case 6:
                            validacaoEspessura = espessura <= d.Espessura;
                            break;

                        default:
                            validacaoEspessura = false;
                            break;
                    }
                }
                else
                    validacaoEspessura = true;

                if (d.IdProcesso > 0)
                    validacaoProcesso = d.IdProcesso == idProcesso;
                else
                    validacaoProcesso = true;

                if (validacaoSubgrupo && validacaoBenefConfig && validacaoEspessura && validacaoProcesso &&
                    ((d.IdSubgrupoProd > 0 || d.IdBenefConfig > 0 || d.CondEspessura > 0 || d.IdProcesso > 0) ||
                    (d.IdSubgrupoProd == 0 && d.IdBenefConfig == 0 && d.CondEspessura == 0 && d.IdProcesso == 0 && d.Aresta > 0)))
                {
                    aresta = d.Aresta;
                    break;
                }
            }

            return aresta;
        }

        /// <summary>
        /// Exporta arquivo txt com dados para inserção no Opty Way
        /// </summary>
        public string ArquivoOtimizacaoOptyWay(uint idFunc, uint idImpressao, string etiquetas, ref List<Etiqueta> lstEtiqueta,
            ref List<byte[]> lstArqMesa, ref List<string> lstCodArq, ref List<KeyValuePair<string, Exception>> lstErrosArq, bool ignorarExportadas, bool ignorarSag)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    #region Busca as etiquetas

                    // Salva uma lista com os pedidos usados para montar a etiqueta
                    var dicPedidos = new Dictionary<uint, Pedido>();

                    // Salva uma lista com os produtos usados para montar a etiqueta
                    var dicProd = new Dictionary<uint, Produto>();

                    // Se o idImpressão tiver sido passado, apenas recupera lista de produtosImpresao
                    if (idImpressao > 0)
                        lstEtiqueta = new List<Etiqueta>(EtiquetaDAO.Instance.GetListPedido(transaction,
                            idFunc, idImpressao.ToString(), 0, 0, null, true, true, null, false, null, null));
                    else
                    {
                        List<uint> idProdutoPedidoApagar = new List<uint>();
                        /* Chamado 45983. */
                        var idLojaPedidoAux = 0;

                        string[] lista = etiquetas.TrimEnd('|').Split('|');

                        bool sql2Executado = false;

                        // Para cada produto a ser impresso, pega os valores para serem inseridos no BD
                        foreach (string item in lista)
                        {
                            string[] campos = item.Split(';');

                            // Não permite otimizar pedido mão de obra
                            if (campos[0].Contains("A"))
                                throw new Exception("Não é possível otimizar pedidos de mão de obra.");

                            // Busca produtoPedidoEspelho
                            campos[0] = campos[0].Replace("R", "").Replace("A", "");
                            var prodPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetForArquivoOtimizacao(transaction, Glass.Conversoes.StrParaUint(campos[0]));

                            #region Verifica se todos pedidos são da mesma loja

                            if (EtiquetaConfig.RelatorioEtiqueta.ModeloEtiquetaPorLoja)
                            {
                                var idLojaPedido = (int)PedidoDAO.Instance.ObtemIdLoja(transaction, prodPedEsp.IdPedido);

                                if (idLojaPedidoAux == 0)
                                    idLojaPedidoAux = idLojaPedido;
                                else if (idLojaPedidoAux != idLojaPedido)
                                    throw new Exception("Não é possível otimizar etiquetas de pedidos associados a lojas diferentes, pois, há um modelo de etiqueta por loja.");
                            }

                            #endregion

                            bool isPecaReposta = prodPedEsp.PecaReposta;
                                                        
                            // Apaga as etiquetas desse pedido
                            if (!idProdutoPedidoApagar.Contains(prodPedEsp.IdProdPed))
                            {
                                idProdutoPedidoApagar.Add(prodPedEsp.IdProdPed);
                                ApagarEtiquetasOtimizacao(transaction, prodPedEsp.IdProdPed, !sql2Executado, true);
                                sql2Executado = true;
                            }

                            if (campos.Length >= 3 && !String.IsNullOrEmpty(campos[3]))
                                isPecaReposta = ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(transaction, campos[3].Split('_')[0], false);

                            // Busca a descrição do beneficiamento
                            string descrBenef = ProdutoPedidoEspelhoBenefDAO.Instance.GetDescrBenef(transaction, prodPedEsp.IdPedido, prodPedEsp.IdProdPed, false);

                            // Pega a quantidade a ser impresso deste item
                            int qtdAImprimir = campos[1].StrParaInt();
                            int qtdGerada = 0;
                            int posItem = 0; // Utilizado para verificar se o item da peça já foi impresso
                            int prodPedEspQtde = (int)prodPedEsp.Qtde;

                            // Pega a posição deste item no pedido
                            int pos = ProdutosPedidoEspelhoDAO.Instance.GetProdPosition(transaction, prodPedEsp.IdPedido, prodPedEsp.IdProdPed);

                            //Se for produto de composição ajusta as quantidades
                            if (prodPedEsp.IdProdPedParent.GetValueOrDefault(0) > 0)
                            {
                                prodPedEspQtde *= (int)ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(transaction, prodPedEsp.IdProdPedParent.Value);

                                var idProdPedParentPai = ProdutosPedidoEspelhoDAO.Instance.ObterIdProdPedParent(transaction, prodPedEsp.IdProdPedParent.Value);
                                if (idProdPedParentPai.GetValueOrDefault(0) > 0)
                                {
                                    prodPedEspQtde *= (int)ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(transaction, idProdPedParentPai.Value); ;
                                }
                            }

                            // Verifica se todas as etiquetas deste produto já foram impressas
                            if (prodPedEsp.QtdImpresso >= prodPedEspQtde && !isPecaReposta)
                                continue;

                            List<KeyValuePair<string, string>> temp = new List<KeyValuePair<string, string>>();

                            // Gera a qtd de etiquetas requisitadas
                            // Se não for peça reposta ou se a qtd a imprimir for maior que 0 (necessário para os casos em que uma das etiquetas foi reposta),
                            // imprime as peças não repostas
                            if (!isPecaReposta || qtdAImprimir > 0)
                            {
                                while (qtdGerada < qtdAImprimir)
                                {
                                    if (ProdutosPedidoEspelhoDAO.Instance.PossuiFilhosComposicao(transaction, prodPedEsp.IdProdPed))
                                        throw new Exception("Produtos principais da composição não podem ser exportados.");

                                    if (ignorarExportadas && posItem >= prodPedEspQtde)
                                        break;

                                    string numEtiqueta = Glass.Data.RelDAL.EtiquetaDAO.Instance
                                        .GetNumEtiqueta(prodPedEsp.IdPedido, pos, posItem + 1, prodPedEspQtde, ProdutoImpressaoDAO.TipoEtiqueta.Pedido);

                                    // Valida o número da etiqueta
                                    ProdutoPedidoProducaoDAO.Instance.ValidaEtiquetaProducao(transaction, ref numEtiqueta);

                                    posItem++;

                                    // Verifica se esta etiqueta já foi impressa
                                    if (ProdutoImpressaoDAO.Instance.EstaImpressa(transaction, numEtiqueta, ProdutoImpressaoDAO.TipoEtiqueta.Pedido) ||
                                        ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(transaction, numEtiqueta, false))
                                        continue;

                                    //Verifica se deve ignorar etiquetas ja exportas e se a mesma realmente ja foi exportada.
                                    if (ignorarExportadas && ArquivoOtimizacaoDAO.Instance.EtiquetaExportada(transaction, numEtiqueta))
                                        continue;

                                    if (PedidoDAO.Instance.ObtemSituacao(transaction, prodPedEsp.IdPedido) == Pedido.SituacaoPedido.Cancelado)
                                        throw new Exception("O pedido " + prodPedEsp.IdPedido + " está cancelado, portanto não pode ser exportado para o optyway.");

                                    Glass.Data.RelModel.Etiqueta etiqueta = new Glass.Data.RelModel.Etiqueta
                                    {
                                        CodCliente =
                                            PedidoDAO.Instance.ObtemValorCampo<string>(transaction, "codCliente",
                                                "idPedido=" + prodPedEsp.IdPedido),
                                        IdCliente = PedidoDAO.Instance.ObtemIdCliente(transaction, prodPedEsp.IdPedido)
                                    };
                                    etiqueta.NomeCliente = ClienteDAO.Instance.GetNome(transaction, etiqueta.IdCliente);
                                    etiqueta.IdPedido = prodPedEsp.IdPedido.ToString();
                                    etiqueta.IdProdPedEsp = prodPedEsp.IdProdPed;
                                    etiqueta.CodInterno = prodPedEsp.CodInterno;
                                    etiqueta.NumItem = pos + " . " + posItem + "/" + prodPedEspQtde;
                                    //etiqueta.BarCodeData = EtiquetaDAO.Instance.GetNumEtiqueta(prodPedEsp.IdPedido, pos, posItem, (int)prodPedEsp.Qtde, ProdutoImpressaoDAO.TipoEtiqueta.Pedido);
                                    etiqueta.BarCodeData = numEtiqueta;
                                    etiqueta.DataImpressao = DateTime.Now;
                                    etiqueta.DescrProd = prodPedEsp.DescrProduto;
                                    etiqueta.CodApl = prodPedEsp.CodAplicacao;
                                    etiqueta.CodProc = prodPedEsp.CodProcesso;
                                    etiqueta.CodOtimizacao = prodPedEsp.CodOtimizacao != null ? prodPedEsp.CodOtimizacao.ToUpper() : String.Empty;
                                    etiqueta.Altura = prodPedEsp.AlturaProducao.ToString(CultureInfo.InvariantCulture);
                                    etiqueta.Largura = prodPedEsp.LarguraProducao.ToString();
                                    etiqueta.Obs = !PCPConfig.Etiqueta.NaoExibirObsPecaAoImprimirEtiqueta ? prodPedEsp.Obs + " " + descrBenef + " " + prodPedEsp.ObsGrid : descrBenef + " " + prodPedEsp.ObsGrid;
                                    etiqueta.DataPedido = PedidoDAO.Instance.ObtemValorCampo<DateTime>(transaction, "dataPedido", "idPedido=" + prodPedEsp.IdPedido);
                                    etiqueta.Espessura = prodPedEsp.Espessura;
                                    etiqueta.NumEtiqueta = numEtiqueta;
                                    etiqueta.IdCorVidro = ProdutoDAO.Instance.ObtemValorCampo<uint?>(transaction, "idCorVidro", "idProd=" + prodPedEsp.IdProd);
                                    etiqueta.PedCliExterno = prodPedEsp.CodCliente;
                                    etiqueta.FastDelivery = PedidoDAO.Instance.IsFastDelivery(transaction, prodPedEsp.IdPedido);
                                    etiqueta.ClienteExterno = PedidoDAO.Instance.ObtemValorCampo<string>(transaction, "ClienteExterno", "idPedido=" + prodPedEsp.IdPedido);

                                    if (Glass.Configuracoes.PCPConfig.ExportarInfoEtiquetaOptyWay)
                                    {
                                        etiqueta.DataProducao = prodPedEsp.DataFabrica;
                                        etiqueta.RotaExterna = prodPedEsp.RotaCliente;
                                        etiqueta.NomeCidade = prodPedEsp.NomeCidade;
                                    }

                                    if (prodPedEsp.IdPedido > 0)
                                    {
                                        // Carrega a rota do cliente
                                        Rota rota = RotaDAO.Instance.GetByCliente(transaction, etiqueta.IdCliente);
                                        etiqueta.CodRota = rota != null ? rota.CodInterno : string.Empty;
                                        etiqueta.DescrRota = rota != null ? rota.Descricao : string.Empty;
                                    }

                                    etiqueta.DataEntrega = PedidoDAO.Instance.ObtemDataEntrega(transaction, prodPedEsp.IdPedido).GetValueOrDefault();

                                    lstEtiqueta.Add(etiqueta);

                                    qtdGerada++;

                                    // Põe peça em produção
                                    if (!ProdutoPedidoProducaoDAO.Instance.EstaEmProducao(transaction, numEtiqueta))
                                        ProdutoPedidoProducaoDAO.Instance.InserePeca(transaction, idImpressao, numEtiqueta, string.Empty, idFunc, true);
                                }
                            }
                            else
                            {
                                ProdutoImpressao prodImp;

                                if (campos.Length >= 3 && !String.IsNullOrEmpty(campos[3]))
                                {
                                    foreach (string etiq in campos[3].Split('_'))
                                    {
                                        prodImp = ProdutoImpressaoDAO.Instance.GetElementByEtiqueta(transaction, etiq,
                                            ProdutoImpressaoDAO.TipoEtiqueta.Pedido);

                                        // Chamado 11444. Corrigimos o problema ao imprimir etiqueta reposta e cancelada, porém,
                                        // não executamos sql para corrigir os problemas que ocorreram, por isso, caso ocorra
                                        // com alguma outra etiqueta, então inserimos um registro na tabela de erro para facilitar a resolução. 
                                        if (prodImp == null)
                                        {
                                            var msgErro = "Erro ao imprimir a etiqueta " + etiq + ", não foi possível recuperar o produto de impressão";
                                            ErroDAO.Instance.InserirFromException("ImprimirEtiquetas", new Exception(msgErro));

                                            throw new Exception(msgErro);
                                        }

                                        var etiquetaRep = Glass.Data.RelDAL.EtiquetaDAO.Instance.MontaEtiqueta(transaction, idFunc, prodImp,
                                            prodPedEsp.DescrBeneficiamentos, null, ref temp, ref dicPedidos, ref dicProd, true);

                                        // Chamado 47948 - Necessário pois no MontaEtiqueta essas informações são utilizadas de forma diferente.
                                        // As etiquetas de peças repostas estavam saindo sem Rota e Data no arquivo de otimização.
                                        if (PCPConfig.ExportarInfoEtiquetaOptyWay)
                                        {
                                            etiquetaRep.DataProducao = prodPedEsp.DataFabrica;
                                            etiquetaRep.RotaExterna = prodPedEsp.RotaCliente;
                                        }

                                        lstEtiqueta.Add(etiquetaRep);
                                    }
                                }
                                else
                                {
                                    prodImp = ProdutoImpressaoDAO.Instance.GetElementByEtiqueta(transaction, prodPedEsp.NumEtiqueta,
                                        ProdutoImpressaoDAO.TipoEtiqueta.Pedido);

                                    // Chamado 11444. Corrigimos o problema ao imprimir etiqueta reposta e cancelada, porém,
                                    // não executamos sql para corrigir os problemas que ocorreram, por isso, caso ocorra
                                    // com alguma outra etiqueta, então inserimos um registro na tabela de erro para facilitar a resolução.
                                    if (prodImp == null)
                                    {
                                        ErroDAO.Instance.InserirFromException("ImprimirEtiquetas",
                                            new Exception("Erro ao imprimir a etiqueta " + prodPedEsp.NumEtiqueta +
                                                ", não foi possível recuperar o produto de impressão"));
                                        continue;
                                    }

                                    var etiquetaRep = Glass.Data.RelDAL.EtiquetaDAO.Instance.MontaEtiqueta(transaction, idFunc, prodImp,
                                        prodPedEsp.DescrBeneficiamentos, null, ref temp, ref dicPedidos, ref dicProd, true);

                                    // Chamado 47948 - Necessário pois no MontaEtiqueta essas informações são utilizadas de forma diferente.
                                    // As etiquetas de peças repostas estavam saindo sem Rota e Data no arquivo de otimização.
                                    if (PCPConfig.ExportarInfoEtiquetaOptyWay)
                                    {
                                        etiquetaRep.DataProducao = prodPedEsp.DataFabrica;
                                        etiquetaRep.RotaExterna = prodPedEsp.RotaCliente;
                                    }

                                    lstEtiqueta.Add(etiquetaRep);
                                }
                            }
                        }
                    }

                    #endregion

                    if (!ignorarSag)
                        MontaArquivoMesaOptyway(transaction, lstEtiqueta, lstArqMesa, lstCodArq, lstErrosArq, 0, true, false);

                    var versao = PCPConfig.VersaoArquivoOptyway;

                    if (versao != 4 && versao != 7)
                        throw new Exception(string.Format("Versão configurada para o arquivo inexistente ({0}), a versão deve ser 4 ou 7.", versao));

                    // Começa a montagem do arquivo de otimização
                    // 1ª linha (58 chars)
                    string arqOtimiz =
                        "Arquivo gerado via WebGlass".PadRight(32) + // Descrição do serviço
                        DateTime.Now.ToString("ddMMyyyy") + // Data do serviço
                        "".PadLeft(8) + // Flags utilizados pelo opty way (ficar em branco)
                        lstEtiqueta.Count.ToString().PadLeft(8) + // Qtde de peças
                        "V" + versao + "\r\n"; // Identificador da versão do Opty Way

                    // 2ª linha (8 chars)
                    arqOtimiz += lstEtiqueta.Count.ToString().PadLeft(8) + "\r\n"; // Qtd de linhas que serão inseridas no arquivo

                    int cont = 1;

                    bool gerarArqMesa = PCPConfig.Etiqueta.GerarArquivoMesaCorte;

                    // Próximas linhas, peças que serão otimizadas
                    foreach (Glass.Data.RelModel.Etiqueta etiqueta in lstEtiqueta)
                    {
                        if (etiqueta.CodOtimizacao == null)
                            throw new Exception("Informe o código de otimização da peça no cadastro de produtos.");

                        var idProd = etiqueta.IdProdPedEsp > 0 ? (int)ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd(transaction, etiqueta.IdProdPedEsp) : 0;
                        var produtoProducao = ProdutoDAO.Instance.IsProdutoProducao(transaction, (int)idProd);
                        var idsBenef = ProdutoPedidoEspelhoBenefDAO.Instance.GetByProdutoPedido(transaction, etiqueta.IdProdPedEsp).Select(f => (int)f.IdBenefConfig).ToList();
                        var descricaoBeneficiamento = ProdutoPedidoEspelhoBenefDAO.Instance.GetDescrBenef(transaction, etiqueta.IdProdPedEsp);
                        var idProcesso = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProcesso(transaction, etiqueta.IdProdPedEsp);
                        var aresta = GetAresta(transaction, idProd, etiqueta.IdArquivoMesaCorte, idsBenef, descricaoBeneficiamento, (int)idProcesso);
                        float arestaLadoaLado = 0;

                        //Recupera o produto pedido
                        int? idProdBaixa = 0;
                        var produtoPedido = ProdutosPedidoEspelhoDAO.Instance.GetElementByPrimaryKey(transaction, etiqueta.IdProdPedEsp);

                        //Caso o produto seja filho recupera o id do pai para buscar a forma
                        if (produtoPedido.IdProdPedParent > 0)
                            idProdBaixa = produtoPedido.IdProdBaixaEst;

                        var isPecaReposta = new Lazy<bool>(() => ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(transaction, etiqueta.IdPedido + "-" + etiqueta.NumItem.Replace(" ", ""), false));

                        string nomeCliente = etiqueta.NomeCliente.Length > 12 ? etiqueta.NomeCliente.Substring(0, 12).ToUpper() : etiqueta.NomeCliente.ToUpper();
                        var formaProduto = gerarArqMesa && isPecaReposta.Value && !produtoProducao ? String.Empty : ProdutoDAO.Instance.ObtemForma(transaction, idProd, idProdBaixa);
                        string shapeId = !String.IsNullOrEmpty(etiqueta.Forma) ? etiqueta.Forma : formaProduto;

                        if (shapeId == "XXXXXXXX" && !String.IsNullOrEmpty(formaProduto))
                            shapeId = formaProduto;

                        shapeId = shapeId != null && shapeId.Length > 8 ? shapeId.Substring(0, 8) : String.IsNullOrEmpty(shapeId) ? String.Empty : shapeId;

                        // Chamado 13815
                        if (!gerarArqMesa)
                        {
                            etiqueta.Forma = String.Empty;
                            shapeId = String.Empty;
                        }

                        string prioridade = "0";

                        // Se for pedido de fast delivery ou se for peça de reposição, altera a preferência para 999 no optyway
                        if (PCPConfig.PrioridadeMaximaArquivoOptywaySeFastDeliveryOuPecaReposta &&
                            (PedidoDAO.Instance.IsFastDelivery(transaction, etiqueta.IdPedido.StrParaUint()) || isPecaReposta.Value))
                            prioridade = "999";

                        if (PCPConfig.GerarShapeIdVazioSePecaRepostaEEtiquetaSemForma && etiqueta.Forma == String.Empty && isPecaReposta.Value)
                            shapeId = String.Empty;

                        // Chamado 15432: Se a peça tiver XXXXXX ou 999999, não deve zerar o shapeId, a menos que a empresa gere SAG de peça reposta
                        if (isPecaReposta.Value && shapeId != null && !shapeId.Contains("XXXXXX") && !shapeId.Contains("999999") && !PCPConfig.GerarMarcacaoPecaReposta && !produtoProducao)
                            shapeId = string.Empty;

                        // Chamado 17954: Se tiver que gerar marcação de peça reposta
                        if (PCPConfig.GerarMarcacaoPecaReposta && String.IsNullOrEmpty(shapeId))
                            shapeId = ProdutoImpressaoDAO.Instance.ObtemForma(transaction, etiqueta.NumEtiqueta, ProdutoImpressaoDAO.TipoEtiqueta.Pedido);

                        if (String.IsNullOrEmpty(etiqueta.CodOtimizacao))
                            throw new Exception("O produto " + etiqueta.DescrProd + " não possui código de otimização, informe-o no cadastro de produto.");

                        /* Chamado 18056. */
                        shapeId = String.IsNullOrEmpty(shapeId) ? String.Empty : shapeId;

                        // Alteração realizada para Divine
                        string rotacao = etiqueta.CodOtimizacao != "ANT08" ? "Y" : "N";

                        if (PCPConfig.EnviarOptywayRotacaoNSeCanelado &&
                            (etiqueta.CodInterno == "CANELFUME" || etiqueta.CodInterno == "CANELADO"))
                            rotacao = "N";

                        string notasAdicionais1 = String.Empty;
                        string notasAdicionais2 = String.Empty;
                        string notasAdicionais3 = String.Empty;
                        string notasAdicionais4 = String.Empty;

                        // Salva o processo e a observação da peça nos campos NotaAdicional1 e NotaAdicional1 do arquivo de exportação
                        if (PCPConfig.ExportarProcessoObsRotaOptyway)
                        {
                            notasAdicionais1 = etiqueta.CodProc ?? String.Empty;

                            if (etiqueta.IdProdPedEsp > 0)
                            {
                                notasAdicionais2 = ProdutosPedidoEspelhoDAO.Instance.ObtemObs(transaction, etiqueta.IdProdPedEsp);
                                if (notasAdicionais2 != null && notasAdicionais2.Length > 32)
                                    notasAdicionais2 = notasAdicionais2.Substring(0, 32);

                                if (notasAdicionais2 == null)
                                    notasAdicionais2 = String.Empty;

                                notasAdicionais2.Trim();
                            }

                            // Informa a rota
                            notasAdicionais3 = etiqueta.CodRota ?? String.Empty;
                        }
                        else if (PCPConfig.ExportarSerigrafiaPinturaOptyway)
                        {
                            notasAdicionais1 = etiqueta.CodProc ?? String.Empty;
                            notasAdicionais2 = etiqueta.CodApl ?? String.Empty;

                            if (ProdutoPedidoEspelhoBenefDAO.Instance.PossuiSerigrafia(transaction, etiqueta.IdProdPedEsp))
                                notasAdicionais4 += "Serigrafia";

                            if (ProdutoPedidoEspelhoBenefDAO.Instance.PossuiPintura(transaction, etiqueta.IdProdPedEsp))
                                notasAdicionais4 += !string.IsNullOrEmpty(notasAdicionais4) ? ", Pintura" : "Pintura";
                        }

                        //transaction, etiqueta.IdProdPedEsp;

                        #region Tratamento tipo subgrupo Modulado

                        var numberOfCuttingPieces = 1;

                        // Recupera a quantidade da matéria prima do produto caso o subgrupo associado seja do tipo Modulado.
                        // Salva a quantidade na variável numberOfCuttingPieces que será utilizada no arquivo de otimização.
                        if (idProd > 0)
                        {
                            var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(transaction, idProd);

                            if (idSubgrupoProd > 0)
                            {
                                var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(transaction, idProd);

                                if (tipoSubgrupo == TipoSubgrupoProd.Modulado)
                                {
                                    if (ProdutoBaixaEstoqueDAO.Instance.TemProdutoBaixa(transaction, (uint)idProd))
                                        numberOfCuttingPieces = (int)ProdutoBaixaEstoqueDAO.Instance.GetByProd(transaction, (uint)idProd)[0].Qtde;
                                }
                            }
                        }

                        #endregion

                        if (versao == 4)
                        {
                            var exportarInfoEtiquetaOptyWay = Glass.Configuracoes.PCPConfig.ExportarInfoEtiquetaOptyWay;

                            arqOtimiz +=
                                // Campos obrigatórios
                                etiqueta.CodOtimizacao.PadRight(16) + // Identificação da peça
                                cont.ToString().PadLeft(5) + // Posição na peça 
                                Formatacoes.RetiraCaracteresEspeciais(nomeCliente.Replace("&", "")).PadRight(12) + // Identifição do cliente
                                etiqueta.IdPedido.PadRight(12) + // IdPedido
                                shapeId.PadRight(8) + // Shape Id, forma cadastrada no Opty Way
                                aresta.ToString("N2").Replace(",", ".").PadRight(8) + // Tamanho da lapidação
                                prioridade.PadLeft(3) + // Prioridade da peça
                                rotacao + // Permitir que a peça seja rotacionada (Y/N)
                                numberOfCuttingPieces.ToString().PadLeft(8) + // Number of Cutting Pieces
                                "(null)".PadRight(8) + // Tool Thickness (Não usa)
                                (etiqueta.Largura + ".0").PadLeft(8) + // Largura da peça
                                (etiqueta.Altura + ".0").PadLeft(8) + // Altura da peça
                                "".PadRight(8) + // Spacer (Não usa)
                                etiqueta.BarCodeData.PadRight(32) + // Etiqueta
                                "0".PadLeft(5) + // Number of labels
                                "0".PadLeft(5) + // Exceded pieces
                                "0".PadLeft(5) + // Preference value

                                // Campos opcionais
                                "".PadRight(6) + // Rack (Não usa)
                                (etiqueta.DataEntrega != null ? etiqueta.DataEntrega.Value.ToString("ddMMyyyy") : "00000000") + // Data de entrega
                                "000" + // Fixed Text Code (for bending machine)
                                "0.0".PadLeft(8); // Spacer Distance From Glass Edge

                            if (exportarInfoEtiquetaOptyWay)
                            {
                                etiqueta.BarCodeData = EtiquetaDAO.Instance.ConcatenaEspAltLargNumEtiqueta(etiqueta);

                                //1 - IdPedido (178 .. 209)
                                arqOtimiz += (etiqueta.IdPedido ?? "").PadLeft(32);

                                //2 - Nome do Cliente (210 .. 241)
                                var nomeCli = !string.IsNullOrEmpty(etiqueta.NomeCliente) ? Glass.Formatacoes.RetiraCaracteresEspeciais(etiqueta.NomeCliente) : "";
                                arqOtimiz += (nomeCli.Length > 32 ? nomeCli.Substring(0, 32) : nomeCli).PadLeft(32);

                                //3 - Rota (242 .. 273)
                                var rota = !string.IsNullOrEmpty(etiqueta.RotaExterna) ? Glass.Formatacoes.RetiraCaracteresEspeciais(etiqueta.RotaExterna) : "";
                                arqOtimiz += rota.PadLeft(32);

                                //4 - Num. Etq. (274 .. 305)
                                arqOtimiz += (etiqueta.NumEtiqueta ?? "").PadLeft(32);

                                //5 - Total da pecas no pedido (306 .. 337)
                                arqOtimiz += ProdutosPedidoEspelhoDAO.Instance.ObtemQtdPecasVidroPedido(transaction, etiqueta.IdPedido.StrParaUint()).ToString().PadLeft(32);

                                //6 - Numero da etiquta do produto importado (338 .. 369)
                                arqOtimiz += (etiqueta.NumEtiquetaCliente ?? string.Empty).PadLeft(32);

                                //7 - Produto (370 .. 401)
                                var prod = !string.IsNullOrEmpty(etiqueta.DescrProd) ? Glass.Formatacoes.RetiraCaracteresEspeciais(etiqueta.DescrProd) : "";
                                arqOtimiz += (prod.Length > 32 ? prod.Substring(0, 32) : prod).PadLeft(32);

                                //8 - Numero referente a marcacao feita no Optyway ou Webglass (402 .. 433)
                                arqOtimiz += "".PadLeft(32);

                                //9 - Data da Fabrica (434 .. 465)
                                arqOtimiz += (etiqueta.DataProducao != null ? etiqueta.DataProducao.Value.ToString("dd/MM/yy") : "").PadLeft(32);

                                //10 - Cidade (466 .. 497)
                                var cidade = !string.IsNullOrEmpty(etiqueta.NomeCidade) ? Glass.Formatacoes.RetiraCaracteresEspeciais(etiqueta.NomeCidade) : "";
                                arqOtimiz += (cidade.Length > 32 ? cidade.Substring(0, 32) : cidade).PadLeft(32);
                            }
                            else
                            {
                                arqOtimiz +=
                                    "".PadRight(32) + // Campo opcional (Número da etiqueta)
                                    "".PadLeft(288); // 2-10 Campos adicionais, 32 chars cada
                            }

                            arqOtimiz +=
                                "".PadLeft(8) + // Second Spacer
                                (arestaLadoaLado > 0 ? arestaLadoaLado.ToString("N1") : "0.0").PadLeft(8) + // Bottom/X1 Grinding Value
                                (arestaLadoaLado > 0 ? arestaLadoaLado.ToString("N1") : "0.0").PadLeft(8) + // Left/Y1 Grinding Value
                                (arestaLadoaLado > 0 ? arestaLadoaLado.ToString("N1") : "0.0").PadLeft(8) + // Top/X2 Grinding Value
                                (arestaLadoaLado > 0 ? arestaLadoaLado.ToString("N1") : "0.0").PadLeft(8); // Right/Y2 Grinding Value

                            if (exportarInfoEtiquetaOptyWay)
                            {
                                etiqueta.PossuiDxf = ProdutosPedidoEspelhoDAO.Instance.PossuiDxf(transaction, etiqueta.IdProdPedEsp, etiqueta.NumEtiqueta);
                                etiqueta.PossuiSGlass = ProdutosPedidoEspelhoDAO.Instance.PossuiSGlass(transaction, etiqueta.IdProdPedEsp, etiqueta.NumEtiqueta);

                                //11 - Obs (538 .. 569)
                                var obs = (etiqueta.DescrBenef != null ? etiqueta.DescrBenef.Trim() : "") + (!string.IsNullOrEmpty(etiqueta.DescrBenef) ? " " : "") +
                                    etiqueta.Obs != null ? etiqueta.Obs.Trim() : "";
                                obs = Glass.Formatacoes.RetiraCaracteresEspeciais(obs);
                                arqOtimiz += (obs.Length > 32 ? obs.Substring(0, 32) : obs).PadLeft(32);

                                //12 - Larg x Alt (570 .. 601)
                                if (!PedidoConfig.EmpresaTrabalhaAlturaLargura)
                                    arqOtimiz += (etiqueta.Largura + " X " + etiqueta.Altura).PadLeft(32);
                                else
                                    arqOtimiz += (etiqueta.Altura + " X " + etiqueta.Largura).PadLeft(32);

                                //13 - Ped. Cli (602 .. 633)
                                var pedCliExterno = etiqueta.PedCliExterno != null && etiqueta.PedCliExterno.Length > 32 ? etiqueta.PedCliExterno.Substring(0, 32) : etiqueta.PedCliExterno;
                                pedCliExterno = Glass.Formatacoes.RetiraCaracteresEspeciais(pedCliExterno);
                                arqOtimiz += (pedCliExterno ?? String.Empty).PadLeft(32);

                                //14 - Cód de Barras (634 .. 665)
                                arqOtimiz += etiqueta.BarCode.PadLeft(32);

                                //15 - Apl. Proc. (666 .. 697)
                                arqOtimiz += (etiqueta.CodApl + " " + etiqueta.CodProc).PadLeft(32);

                                //16 - Fast Delivery (698 .. 729)
                                arqOtimiz += (etiqueta.FastDelivery ? "Fast-Delivery" : "").PadLeft(32);

                                //17 -- (730 .. 761)
                                arqOtimiz += ((etiqueta.PossuiDxf ? "WA" : "") + (etiqueta.PossuiDxf && etiqueta.PossuiSGlass ? "/" : "") + (etiqueta.PossuiSGlass ? "SGLASS" : "")).PadLeft(32);

                                //18 -- (762 .. 793)
                                var nomeCliExterno = etiqueta.ClienteExterno != null && etiqueta.ClienteExterno.Length > 32 ? etiqueta.ClienteExterno.Substring(0, 32) : etiqueta.ClienteExterno;
                                nomeCliExterno = Formatacoes.RetiraCaracteresEspeciais(nomeCliExterno);
                                arqOtimiz += (nomeCliExterno ?? string.Empty).PadLeft(32);

                                //19 -- (794 .. 825)
                                arqOtimiz += "".PadLeft(32);

                                //20 -- (826 .. 857)
                                arqOtimiz += "".PadLeft(32);
                            }
                            else
                            {
                                arqOtimiz +=
                                notasAdicionais1.PadLeft(32) + // Campo adicional 1
                                notasAdicionais2.PadLeft(32) + // Campo adicional 2
                                notasAdicionais3.PadLeft(32) + // Campo adicional 3
                                notasAdicionais4.PadLeft(32) + // Campo adicional 4
                                "".PadLeft(192); // 12-20 Campos adicionais, 32 chars cada
                            }

                            arqOtimiz +=
                               "".PadLeft(48) + // Descrição do material
                               (etiqueta.DataPedido != null ? etiqueta.DataPedido.Value.ToString("ddMMyyyy") : "00000000") + // Data do pedido
                               cont.ToString().PadLeft(8) + "(null)";/* + // Order line number
                            /*"".PadLeft(40) + // Customer trade name
                            "".PadLeft(40) + // Customer address (street)
                            "".PadLeft(30) + // Customer address (city)
                            "".PadLeft(6) + // Customer address (province)
                            "".PadLeft(4) + // Customer address (state/nation)
                            "".PadLeft(8); // Customer address (zip/post code)*/
                        }
                        else if (versao == 7)
                        {
                            arqOtimiz +=
                                // Campos obrigatórios
                                etiqueta.CodOtimizacao.PadRight(64) + // Identificação da peça
                                cont.ToString().PadLeft(5) + // Posição na peça 
                                Formatacoes.RetiraCaracteresEspeciais(nomeCliente.Replace("&", "")).PadRight(12) + // Identifição do cliente
                                etiqueta.IdPedido.PadRight(12) + // IdPedido
                                shapeId.PadRight(8) + // Shape Id, forma cadastrada no Opty Way
                                "SAG".PadRight(4) + // Extensão do arquivo shape
                                aresta.ToString("N2").Replace(",", ".").PadRight(8) + // Tamanho da lapidação
                                prioridade.PadLeft(3) + // Prioridade da peça
                                rotacao + // Permitir que a peça seja rotacionada (Y/N)
                                numberOfCuttingPieces.ToString().PadLeft(8) + // Number of Cutting Pieces
                                "(null)".PadRight(8) + // Tool Thickness (Não usa)
                                (etiqueta.Largura + ".0").PadLeft(8) + // Largura da peça
                                (etiqueta.Altura + ".0").PadLeft(8) + // Altura da peça
                                "".PadRight(8) + // Spacer (Não usa)
                                (etiqueta.IdPedido + "-" + etiqueta.NumItem.Replace(" ", "")).PadRight(32) + // Etiqueta
                                "0".PadLeft(5) + // Number of labels
                                "0".PadLeft(5) + // Exceded pieces
                                "0".PadLeft(5) + // Preference value

                                // Campos opcionais
                                "".PadRight(6) + // Rack (Não usa)
                                (etiqueta.DataEntrega != null ? etiqueta.DataEntrega.Value.ToString("ddMMyyyy") : "00000000") + // Data de entrega
                                "000" + // Fixed Text Code (for bending machine)
                                "0.0".PadLeft(8) + // Spacer Distance From Glass Edge
                                "".PadRight(32) + // Campo opcional (Número da etiqueta)
                                "".PadLeft(288) + // 2-10 Campos adicionais, 32 chars cada
                                "".PadLeft(8) + // Second Spacer
                                "0.0".PadLeft(8) + // Bottom/X1 Grinding Value
                                "0.0".PadLeft(8) + // Left/Y1 Grinding Value
                                "0.0".PadLeft(8) + // Top/X2 Grinding Value
                                "0.0".PadLeft(8) + // Right/Y2 Grinding Value
                                notasAdicionais1.PadLeft(32) + // Campo adicional 1
                                notasAdicionais2.PadLeft(32) + // Campo adicional 2
                                "".PadLeft(256) + // 12-20 Campos adicionais, 32 chars cada
                                "".PadLeft(48) + // Descrição do material
                                (etiqueta.DataPedido != null ? etiqueta.DataPedido.Value.ToString("ddMMyyyy") : "00000000") + // Data do pedido
                                cont.ToString().PadLeft(8) + "(null)";/* + // Order line number
                            /*"".PadLeft(40) + // Customer trade name
                            "".PadLeft(40) + // Customer address (street)
                            "".PadLeft(30) + // Customer address (city)
                            "".PadLeft(6) + // Customer address (province)
                            "".PadLeft(4) + // Customer address (state/nation)
                            "".PadLeft(8); // Customer address (zip/post code)*/
                        }

                        // Quebra linha para ir para o próximo produto
                        arqOtimiz += "\r\n";

                        if (!String.IsNullOrEmpty(shapeId))
                            arqOtimiz += shapeId.PadRight(8) + " -1 -1 -1 -1\r\n";

                        cont++;
                    }

                    transaction.Commit();
                    transaction.Close();

                    return arqOtimiz;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException("ArquivoOtimizacaoOptyWay", ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Monta os arquivos de mesa com base nas etiquetas passadas
        /// </summary>
        public void MontaArquivoMesaOptyway(List<Etiqueta> lstEtiqueta, List<byte[]> lstArqMesa, List<string> lstCodArq,
            List<KeyValuePair<string, Exception>> lstErrosArq, uint idSetor, bool arquivoOtimizacao, bool converterCaractereEspecial)
        {
            MontaArquivoMesaOptyway(null, lstEtiqueta, lstArqMesa, lstCodArq, lstErrosArq, idSetor, arquivoOtimizacao, converterCaractereEspecial);
        }

        /// <summary>
        /// Monta os arquivos de mesa com base nas etiquetas passadas
        /// </summary>
        public void MontaArquivoMesaOptyway(GDASession session, List<Etiqueta> lstEtiqueta, List<byte[]> lstArqMesa, List<string> lstCodArq,
             List<KeyValuePair<string, Exception>> lstErrosArq, uint idSetor, bool arquivoOtimizacao, bool converterCaractereEspecial)
        {
            MontaArquivoMesaOptyway(session, lstEtiqueta, lstArqMesa, lstCodArq, lstErrosArq, idSetor, arquivoOtimizacao, 0, false, false, converterCaractereEspecial);
        }

        /// <summary>
        /// Monta os arquivos de mesa com base nas etiquetas passadas
        /// </summary>
        public void MontaArquivoMesaOptyway(GDASession session, List<Etiqueta> lstEtiqueta, List<byte[]> lstArqMesa, List<string> lstCodArq,
             List<KeyValuePair<string, Exception>> lstErrosArq, uint idSetor, bool arquivoOtimizacao, int tipoArquivo, bool forSGlass, bool forIntermac, bool converterCaractereEspecial)
        {
            // Quantidade máxima de dígitos que os arquivos de mesa podem ter
            int qtdDigitosArqMesa = PCPConfig.QtdDigitosNomeArquivoMesa;

            bool criarArquivoSAGComNumDecimal = PCPConfig.CriarArquivoSAGComNumDecimal;

            if (lstEtiqueta.Count > 0 && PCPConfig.Etiqueta.GerarArquivoMesaCorte)
            {
                List<Etiqueta> lstEtiqMesaCorte = new List<Etiqueta>();
                lstEtiqMesaCorte.AddRange(lstEtiqueta);

                // Ordena a lista de etiquetas pelo pedido, para contar as peças de cada pedido
                lstEtiqMesaCorte = lstEtiqMesaCorte.OrderBy(f => f.IdPedido).ToList();

                // Monta o arquivo da mesa de corte e o nome do arquivo físico dos mesmos
                var tipoArquivoOriginal = tipoArquivo;

                foreach (Etiqueta etiq in lstEtiqMesaCorte)
                {
                    uint? idArquivoMesaCorte = null;
                    byte[] arqMesa = null;

                    tipoArquivo = tipoArquivoOriginal;

                    var pecaEstaReposta = ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(session, etiq.NumEtiqueta, false);

                    // Gera os arquivos de Mesa
                    using (var ms = new System.IO.MemoryStream())
                    {
                        try
                        {
                            ArquivoMesaCorteDAO.Instance.GetArquivoMesaCorte(session, etiq.IdPedido.StrParaUint(), etiq.IdProdPedEsp,
                                idSetor, ref idArquivoMesaCorte, arquivoOtimizacao, ms, ref tipoArquivo, forSGlass, forIntermac);
                        }
                        catch (Exception ex)
                        {
                            ErroDAO.Instance.InserirFromException($"MontaArquivoMesaOptway. Etiqueta {etiq.NumEtiqueta}", ex);
                            lstErrosArq.Add(new KeyValuePair<string, Exception>(etiq.NumEtiqueta, ex));
                        }
                        arqMesa = ms.ToArray();
                    }

                    // Preenche o idArquivoMesaCorte aqui para que ao chamar o método de buscar a aresta, passe o idArquivoMesaCorte correto
                    etiq.IdArquivoMesaCorte = idArquivoMesaCorte;

                    var idProd = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd(session, etiq.IdProdPedEsp);
                    var produtoProducao = ProdutoDAO.Instance.IsProdutoProducao(session, (int)idProd);

                    /* Chamado 16479.
                     * Peças repostas não podem gerar arquivo de marcação, devem gerar o arquivo .ASC com forma inexistente. */
                    if (arqMesa != null && arqMesa.Length > 0 && idArquivoMesaCorte != null && (!pecaEstaReposta || PCPConfig.GerarMarcacaoPecaReposta || produtoProducao))
                    {
                        var forma = string.Empty;
                        var nomeArquivo = ObterNomeArquivo(session, etiq, (TipoArquivoMesaCorte)tipoArquivo, null, null, forIntermac, out forma, converterCaractereEspecial);

                        // Verifica se é um arquivo .zip
                        if (Utils.VerificarArquivoZip(arqMesa) && !StringComparer.InvariantCultureIgnoreCase.Equals(System.IO.Path.GetExtension(nomeArquivo), ".zip"))
                            nomeArquivo = System.IO.Path.GetFileNameWithoutExtension(nomeArquivo) + ".zip";

                        lstArqMesa.Add(arqMesa);
                        lstCodArq.Add(nomeArquivo);

                        etiq.Forma = forma;
                    }
                    // Se a peça não possuir forma, não for produto de estoque e o processo da mesma estiver marcado 
                    // para gerar uma forma inexistente, gera a mesma
                    else
                    {
                        var idPedido = ProdutosPedidoEspelhoDAO.Instance.ObtemIdPedido(session, etiq.IdProdPedEsp);
                        var pedidoImportado = PedidoDAO.Instance.IsPedidoImportado(session, idPedido);

                        if (!pecaEstaReposta || PCPConfig.GerarMarcacaoPecaReposta)
                        {
                            /* Chamado 57976. */
                            if (pedidoImportado)
                            {
                                var caminhoSalvarFMLPedidoImportado = ArquivoMesaCorteDAO.Instance.CaminhoSalvarArquivoPedidoImportado(session, etiq.NumEtiqueta, (int)etiq.IdProdPedEsp,
                                    TipoArquivoMesaCorte.FML);

                                var caminhoSalvarDXFPedidoImportado = ArquivoMesaCorteDAO.Instance.CaminhoSalvarArquivoPedidoImportado(session, etiq.NumEtiqueta, (int)etiq.IdProdPedEsp,
                                    TipoArquivoMesaCorte.DXF);

                                if (System.IO.File.Exists(caminhoSalvarFMLPedidoImportado) || System.IO.File.Exists(caminhoSalvarDXFPedidoImportado))
                                    continue;
                            }

                            var idMaterItemProj = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<uint?>(session, "IdMaterItemProj", string.Format("IdProdPed={0}", etiq.IdProdPedEsp));

                            //Verifica se tem arquivo dxf salvo editado anteriormente.
                            if (idMaterItemProj > 0)
                            {
                                var caminhoDxf = string.Format("{0}{1}.dxf", PCPConfig.CaminhoSalvarCadProject(true), etiq.IdProdPedEsp);

                                if (System.IO.File.Exists(caminhoDxf))
                                    continue;

                                var pecaItemProjeto = PecaItemProjetoDAO.Instance.GetByMaterial(session, idMaterItemProj.Value);
                                var caminhoDxfProjeto = pecaItemProjeto != null && pecaItemProjeto.IdPecaItemProj > 0 ?
                                    string.Format("{0}{1}.dxf", PCPConfig.CaminhoSalvarCadProjectProjeto(), pecaItemProjeto.IdPecaItemProj) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(caminhoDxfProjeto) && System.IO.File.Exists(caminhoDxfProjeto))
                                    continue;
                            }
                        }                           

                        var produtoPossuiImagemAssociada = ProdutosPedidoEspelhoDAO.Instance.PossuiImagemAssociada(session, etiq.IdProdPedEsp);
                        var idAplicacao = ProdutosPedidoEspelhoDAO.Instance.ObtemIdAplicacao(session, etiq.IdProdPedEsp);
                        var aplicacaoGeraFormaInexistente = EtiquetaAplicacaoDAO.Instance.ObtemGerarFormaInexistente(session, idAplicacao);
                        var idProcesso = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProcesso(session, etiq.IdProdPedEsp);
                        var processoGeraFormaInexistente = EtiquetaProcessoDAO.Instance.ObtemGerarFormaInexistente(session, idProcesso);                        

                        if ((tipoArquivo != (int)TipoArquivoMesaCorte.DXF || produtoPossuiImagemAssociada) && ((aplicacaoGeraFormaInexistente || processoGeraFormaInexistente) && !produtoProducao))
                        {
                            etiq.Forma = PCPConfig.PreenchimentoFormaNaoExistente;

                            // Verifica se o produto possui forma cadastrada e a utiliza ao invés de colocar "XXXXXXXX"
                            if (Glass.Configuracoes.PCPConfig.UsarFormaProdutoSeForFormaInexistente)
                            {
                                int? idProdBaixa = 0;
                                var produtoPedido = ProdutosPedidoEspelhoDAO.Instance.GetElementByPrimaryKey(session, etiq.IdProdPedEsp);

                                if (produtoPedido.IdProdPedParent > 0)
                                    idProdBaixa = produtoPedido.IdProdBaixaEst;

                                string forma = ProdutoDAO.Instance.ObtemForma(session, (int)ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd(session, etiq.IdProdPedEsp), idProdBaixa);
                                if (!String.IsNullOrEmpty(forma))
                                    etiq.Forma = forma;
                            }

                            /* Chamado 33177. */
                            if (Configuracoes.PCPConfig.PreencherReposicaoGarantiaCampoForma && etiq.IdProdPedEsp > 0)
                            {
                                var tipoVenda = PedidoDAO.Instance.ObtemTipoVenda(session, idPedido);

                                if (tipoVenda == (int)Pedido.TipoVendaPedido.Reposição)
                                    etiq.Forma = "REPOSICAO";
                                else if (tipoVenda == (int)Pedido.TipoVendaPedido.Garantia)
                                    etiq.Forma = "GARANTIA";
                            }

                            if (PCPConfig.PreencherRedondoCampoForma &&
                                ProdutosPedidoEspelhoDAO.Instance.IsRedondo(session, etiq.IdProdPedEsp))
                                etiq.Forma = "REDONDO";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Recupera o nome do arquivo de marcação, com base no tipo de arquivo de marcação.
        /// </summary>
        public string ObterNomeArquivo(GDASession session, Etiqueta etiqueta, TipoArquivoMesaCorte tipoArquivo, int? idProdPed, string numeroEtiqueta, bool arquivoIntermac, out string forma,
            bool converterCaractereEspecial)
        {
            var retorno = string.Empty;
            // Quantidade máximama de dígitos que os arquivos de mesa podem ter
            var qtdDigitosArqMesa = PCPConfig.QtdDigitosNomeArquivoMesa;
            var criarArquivoSAGComNumDecimal = PCPConfig.CriarArquivoSAGComNumDecimal;
            // Cria um campo forma com o numero do pedido + código hexa
            var contador = ContadorArquivoSagDAO.Instance.GetNext();

            retorno = forma = (!criarArquivoSAGComNumDecimal ? string.Format("{0:X2}", contador) : contador.ToString()).PadLeft(qtdDigitosArqMesa, '0');
            numeroEtiqueta = numeroEtiqueta != null ? numeroEtiqueta.Replace("  ", "").Replace(" ", "") : etiqueta.NumEtiqueta.Replace("  ", "").Replace(" ", "");

            if (tipoArquivo != TipoArquivoMesaCorte.SAG && tipoArquivo > 0)
            {
                if (PCPConfig.NomeArquivoMesaComHifenEOCraseado && arquivoIntermac)
                    retorno = numeroEtiqueta.Replace("-", "'").Replace('/', converterCaractereEspecial ? Convert.ToChar(149) : 'ò');
                else if (PCPConfig.NomeArquivoMesaBarraPorPontoVirgula)
                    retorno = numeroEtiqueta.Replace("/", ";");
                else if (PCPConfig.NomeArquivoMesaBarraPorCeCedilha)
                    retorno = numeroEtiqueta.Replace("/", "ç");
                else if (PCPConfig.NomeArquivoMesaRecriado)
                {
                    if (etiqueta == null || etiqueta.Altura == null || etiqueta.Largura == null || etiqueta.Espessura == 0)
                    {
                        idProdPed = idProdPed.GetValueOrDefault() == 0 && etiqueta != null && etiqueta.IdProdPedEsp > 0 ? (int)etiqueta.IdProdPedEsp : idProdPed.GetValueOrDefault();

                        var idProd = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd(session, (uint)idProdPed);
                        var espessura = ProdutosPedidoEspelhoDAO.Instance.ObterEspessura(session, (uint)idProdPed);
                        var altura = ProdutosPedidoEspelhoDAO.Instance.ObtemAlturaProducao(session, (uint)idProdPed);
                        var largura = ProdutosPedidoEspelhoDAO.Instance.ObtemLarguraProducao(session, (uint)idProdPed);

                        retorno = string.Format("{0}{1}_{2}", espessura.ToString().PadLeft(2, '0'),
                            altura > largura ?
                                string.Format("{0}{1}", altura.ToString().PadLeft(4, '0'), largura.ToString().PadLeft(4, '0')) :
                                string.Format("{0}{1}", largura.ToString().PadLeft(4, '0'), altura.ToString().PadLeft(4, '0')), numeroEtiqueta);
                    }
                    else
                        retorno = EtiquetaDAO.Instance.ConcatenaEspAltLargNumEtiqueta(etiqueta);

                    retorno = retorno.Replace(".", "a").Replace("/", "b").Replace("_", "c").Replace("-", "d");
                }
                else if (tipoArquivo == TipoArquivoMesaCorte.DXF)
                {
                    //Chamado 66266
                    if (PCPConfig.NomeArquivoDxfComAspasECedilha)
                        retorno = numeroEtiqueta.Replace("-", "'").Replace('/', Convert.ToChar(135));
                    else
                        retorno = numeroEtiqueta.Replace(".", "").Replace("/", "");
                }
            }

            retorno +=
                arquivoIntermac ? ".CNI" :
                tipoArquivo == TipoArquivoMesaCorte.SAG ? ".SAG" :
                tipoArquivo == TipoArquivoMesaCorte.FORTxt ? ".FOR" :
                tipoArquivo == TipoArquivoMesaCorte.ISO ? ".ISO" :
                tipoArquivo == TipoArquivoMesaCorte.DXF ? ".DXF" :
                tipoArquivo == TipoArquivoMesaCorte.FMLBasico || tipoArquivo == TipoArquivoMesaCorte.FML ? ".FML" :
                ".SAG";

            return retorno.Replace("  ", "").Replace(" ", "");
        }

        /// <summary>
        /// Põe peças do pedido passado na produção, este método deve ser chamado somente após finalizar a conferência do pedido,
        /// devido às validações removidas.
        /// </summary>
        public void GerarEtiquetasProducao(GDASession sessao, uint idPedido, bool exportandoEtiqueta)
        {
            //// Não gera etiquetas de pedido mão de obra
            //if (PedidoDAO.Instance.IsMaoDeObra(idPedido))
            //    return;

            bool sql2Executado = false;
            List<string> lstEtiquetasInseridas = new List<string>();

            var composicoes = new List<Composicao>();

            //Sequencial de quantidade de peças que uma composição contem
            var dicTotalPecasPai = new Dictionary<string, int>();

            if (!PedidoDAO.Instance.IsMaoDeObra(sessao, idPedido))
            {
                // Percorre todas as peças do pedido para gerar uma etiqueta de cada uma e inserir na produção
                foreach (uint id in ProdutosPedidoEspelhoDAO.Instance.ObterIdsParaImpressaoFinalizacaoPCP(sessao, (int)idPedido))
                {
                    // Busca produtoPedidoEspelho
                    ProdutosPedidoEspelho prodPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetForArquivoOtimizacao(sessao, id);

                    if (!ProdutoDAO.Instance.IsVidro(sessao, (int)prodPedEsp.IdProd))
                        continue;

                    // Apaga as etiquetas desse pedido
                    ApagarEtiquetasOtimizacao(sessao, prodPedEsp.IdProdPed, !sql2Executado, exportandoEtiqueta);
                    sql2Executado = true;

                    // Pega a quantidade a ser impresso deste item
                    int qtdAImprimir = (int)prodPedEsp.Qtde * prodPedEsp.QtdeAmbiente;
                    int qtdGerada = 0;
                    int posItem = 0; // Utilizado para verificar se o item da peça já foi impresso

                    // Pega a posição deste item no pedido
                    int pos = ProdutosPedidoEspelhoDAO.Instance.GetProdPosition(sessao, prodPedEsp.IdPedido, prodPedEsp.IdProdPed);

                    //Se for produto de composição ajusta as quantidades
                    if (prodPedEsp.IdProdPedParent.GetValueOrDefault(0) > 0)
                    {
                        qtdAImprimir *= (int)ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(sessao, prodPedEsp.IdProdPedParent.Value);

                        var idProdPedParentPai = ProdutosPedidoEspelhoDAO.Instance.ObterIdProdPedParent(sessao, prodPedEsp.IdProdPedParent.Value);
                        if (idProdPedParentPai.GetValueOrDefault(0) > 0)
                            qtdAImprimir *= (int)ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(sessao, idProdPedParentPai.Value);
                    }

                    // Cria o código da etiqueta e insere a peça na produção
                    while (qtdGerada < qtdAImprimir)
                    {
                        string numEtiqueta = Glass.Data.RelDAL.EtiquetaDAO.Instance.GetNumEtiqueta(idPedido, pos, posItem + 1, qtdAImprimir, ProdutoImpressaoDAO.TipoEtiqueta.Pedido);

                        // Valida o número da etiqueta
                        ProdutoPedidoProducaoDAO.Instance.ValidaEtiquetaProducao(sessao, ref numEtiqueta);

                        posItem++;
                        qtdGerada++;

                        /* Chamado 16017.
                         * Caso a etiqueta tenha sido impressa passa para a próxima etiqueta. */
                        if (lstEtiquetasInseridas.Contains(numEtiqueta))
                            continue;

                        var numEtqVinculada = "";
                        var posEtiquetaParent = "";

                        if (ProdutosPedidoDAO.Instance.TemProdutoLamComposicao(idPedido))
                        {
                            //Busca possiveis filhos da peça que esta sendo gerada
                            var filhos = ProdutosPedidoEspelhoDAO.Instance.ObterFilhosComposicao(sessao, prodPedEsp.IdProdPed);

                            if (filhos != null && filhos.Count > 0)
                            {
                                var dicFilhos = filhos.ToDictionary(f => f.IdProdPed, f => new KeyValuePair<int, int>((int)f.Qtde, 0));
                                composicoes.Add(new Composicao(prodPedEsp.IdProdPed, numEtiqueta, dicFilhos));
                            }

                            if (prodPedEsp.IdProdPedParent.GetValueOrDefault(0) > 0)
                            {
                                //Busca a peça pai que ainda não foi utilizada.
                                var comp = composicoes.Where(f => f.IdProdPedPai == prodPedEsp.IdProdPedParent.Value && f.Filhos[prodPedEsp.IdProdPed].Key > f.Filhos[prodPedEsp.IdProdPed].Value).FirstOrDefault();

                                //Informa que a peça pai foi utilizada.
                                comp.Filhos[prodPedEsp.IdProdPed] = new KeyValuePair<int, int>(comp.Filhos[prodPedEsp.IdProdPed].Key, comp.Filhos[prodPedEsp.IdProdPed].Value + 1);

                                numEtqVinculada = comp.NumEtiquetaPai;
                                posEtiquetaParent = string.Format("{0} - {1}/{2}", comp.NumEtiquetaPai, comp.Filhos.Sum(f => f.Value.Value), comp.Filhos.Sum(f => f.Value.Key));
                            }
                        }

                        // Põe peça em produção
                        ProdutoPedidoProducaoDAO.Instance.InserePeca(sessao, null, numEtiqueta, string.Empty, UserInfo.GetUserInfo.CodUser, true, null, prodPedEsp.IdProdPed, numEtqVinculada, posEtiquetaParent);

                        /* Chamado 16017.
                         * Salva o número da etiqueta impressa para evitar que sejam inseridos produtos de produção duplicados. */
                        lstEtiquetasInseridas.Add(numEtiqueta);
                    }
                }
            }
            else
            {
                // Insere as peças na produção de acordo com cada ambiente do pedido espelho.
                foreach (var ambiente in AmbientePedidoEspelhoDAO.Instance.GetByPedido(sessao, idPedido))
                {
                    var qtdAImprimir = ambiente.Qtde.GetValueOrDefault(); // Pega a quantidade a ser impresso deste item.
                    var qtdGerada = 0;
                    var posItem = 0; // Utilizada para verificar se o item da peça já foi impresso.
                    var pos = 0; // Utilizada para a posição do item no pedido.
                    var temp = new List<KeyValuePair<string, string>>();

                    // Na produção, caso o ambiente possua mais de um produto, o id de um dos produtos de pedido é salvo, por isso,
                    // é necessário que todos os produtos sejam percorridos até encontrar qual deles tem associação com o produto de produção.
                    foreach (var prodPedEsp in ProdutosPedidoEspelhoDAO.Instance.GetByAmbienteFast(sessao, idPedido, ambiente.IdAmbientePedido))
                    {
                        pos = ProdutosPedidoEspelhoDAO.Instance.GetProdPosition(sessao, ambiente.IdPedido, prodPedEsp.IdProdPed);

                        // Garante que o produto recuperado, para o ambiente, é o produto correto.
                        if (pos == 0)
                            continue;

                        break;
                    }

                    // Cria o código da etiqueta e insere a peça na produção.
                    while (qtdGerada < qtdAImprimir)
                    {
                        // Recupera o número da etiqueta.
                        var numEtiqueta = Glass.Data.RelDAL.EtiquetaDAO.Instance.GetNumEtiqueta(idPedido, pos, posItem + 1, qtdAImprimir,
                            Glass.Data.DAL.ProdutoImpressaoDAO.TipoEtiqueta.Pedido);

                        // Valida o número da etiqueta.
                        ProdutoPedidoProducaoDAO.Instance.ValidaEtiquetaProducao(sessao, ref numEtiqueta);

                        posItem++;
                        qtdGerada++;

                        /* Chamado 16017.
                         * Caso a etiqueta tenha sido impressa passa para a próxima etiqueta. */
                        if (lstEtiquetasInseridas.Contains(numEtiqueta))
                            continue;

                        // Põe peça em produção.
                        ProdutoPedidoProducaoDAO.Instance.InserePeca(sessao, null, numEtiqueta, String.Empty, UserInfo.GetUserInfo.CodUser, true, null, 0, null, null);
                        /* Chamado 16017.
                         * Salva o número da etiqueta impressa para evitar que sejam inseridos produtos de produção duplicados. */
                        lstEtiquetasInseridas.Add(numEtiqueta);
                    }
                }
            }
        }

        #endregion

        #region Importa o arquivo de otimizado pelo Opty Way

        /// <summary>
        /// Importa as peças otimizadas pelo Opty Way.
        /// </summary>
        public void ImportarArquivoOtimizacaoOptyWay(XmlDocument arquivoOtimizado, ref List<string> etiquetas, ref string etiquetasJaImpressas, ref List<int> pedidosAlteradosAposExportacao,
            ref List<ProdutosPedidoEspelho> produtosPedidoEspelho, ref int qtdPecasImpressas)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    #region Retorno Opty Way

                    var planoCorte = string.Empty;

                    foreach (XmlNode node in arquivoOtimizado["DATAPACKET"]["ROWDATA"])
                    {
                        #region Recupera o plano de corte

                        // A tag que possui o atributo "ELABORATO", contém o plano de corte, as tags subsequentes à ela contém as peças
                        if (node.Attributes["ELABORATO"] != null)
                        {
                            if (node.Attributes["CODCOM"] == null)
                                throw new Exception("Tag ELABORATO sem informação do plano de corte (CODCOM). Provavelmente foi inserida uma peça após a otimização das etiquetas, fazendo com que esta ficasse sem plano de corte.");

                            planoCorte =
                                string.Format("{0}Z{1}-{2}{3}",
                                    node.Attributes["CODCOM"].Value,
                                    DateTime.Now.ToString("ddMMyy"),
                                    node.Attributes["ELABORATO"].Value.Substring(0, node.Attributes["ELABORATO"].Value.IndexOf('/')).PadLeft(2, '0'),
                                    node.Attributes["ELABORATO"].Value.Substring(
                                    node.Attributes["ELABORATO"].Value.IndexOf('/'),
                                    node.Attributes["ELABORATO"].Value.IndexOf(' ') - node.Attributes["ELABORATO"].Value.IndexOf('/')));

                            continue;
                        }

                        #endregion

                        if (node.Attributes["NOTES"] == null)
                            throw new Exception("Uma ou mais peças não possuem o código da etiqueta exportado pelo WebGlass (Campo NOTES).");

                        #region Recupera a altura e largura da etiqueta

                        var largura = 0;

                        if (node.Attributes["DIMXPZR"] != null)
                            largura = node.Attributes["DIMXPZR"].Value.Split('.')[0].StrParaInt();
                        else if (node.Attributes["DIMXPZ"] != null)
                            largura = node.Attributes["DIMXPZ"].Value.Split('.')[0].StrParaInt();

                        var altura = 0;

                        if (node.Attributes["DIMYPZR"] != null)
                            altura = node.Attributes["DIMYPZR"].Value.Split('.')[0].StrParaInt();
                        else if (node.Attributes["DIMYPZ"] != null)
                            altura = node.Attributes["DIMYPZ"].Value.Split('.')[0].StrParaInt();

                        #endregion

                        // Pega a etiqueta da peça
                        var etiqueta = node.Attributes["NOTES"].Value;

                        // Verifica se a etiqueta é aceita no webglass
                        if (!etiqueta.Contains(".") || !etiqueta.Contains("/") || !etiqueta.Contains("-"))
                            throw new Exception("Uma das etiquetas do arquivo é inválida. Etiqueta: Notes=" + etiqueta);

                        // Busca o produtoPedidoEspelho pela etiqueta
                        var isPecaReposta = ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(transaction, etiqueta, true);

                        // Verifica se a etiqueta já foi impressa
                        if (!isPecaReposta && ProdutoImpressaoDAO.Instance.EstaImpressa(transaction, etiqueta, ProdutoImpressaoDAO.TipoEtiqueta.Pedido))
                        {
                            etiquetasJaImpressas += etiqueta + ", ";
                            continue;
                        }

                        if (etiquetas.Contains(etiqueta.Trim()))
                        {
                            // Caso a empresa tenha colocado arquivos de tábua ou algo do tipo no arquivo de retorno, deve ser sugerido à eles que 
                            // preencham o campo NOTES com "ignorar", para não dar erro no arquivo e permitir importar as outras peças
                            if (etiqueta.Trim().ToLower() != "ignorar")
                                throw new Exception("Este arquivo possui etiquetas duplicadas. Etiqueta: " + etiqueta);

                            continue;
                        }

                        etiquetas.Add(etiqueta.Trim());

                        // Pega a posição da peça no arquivo de otimização
                        var posicaoArqOtimiz = node.Attributes["POSPZ"].Value.Split('/')[0].StrParaInt();

                        // Pega a posição de ordenação da peça no arquivo de otimização
                        var numSeq = node.Attributes["POSTOT"] != null ? node.Attributes["POSTOT"].Value.StrParaInt() : 0;

                        // Pega o campo forma, se houver
                        var forma = node.Attributes["SAGOMA"] != null ? node.Attributes["SAGOMA"].Value : string.Empty;

                        ProdutosPedidoEspelho prodPed;
                        var msgErro = string.Format("A etiqueta '{0}' não possui uma peça associada. O pedido pode ter sido alterado após a geração do arquivo para o Opty Way ou a impressão pode ter sido cancelada. {1}",
                            etiqueta, "Refaça a otimização das etiquetas com um novo arquivo de otimização gerado pelo sistema.");

                        var idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(transaction, etiqueta);
                        uint? idProdPed = null;

                        if (idProdPedProducao == null)
                        {
                            // Se já houver uma etiqueta cancelada com este código, apenas re-insere a mesma ao invés de dar mensagem de erro
                            if (ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducaoCanc(transaction, etiqueta) > 0)
                            {
                                ProdutoPedidoProducaoDAO.Instance.InserePeca(transaction, null, etiqueta, String.Empty, UserInfo.GetUserInfo.CodUser, true);
                                idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(transaction, etiqueta);
                            }
                            else
                                throw new Exception(msgErro);
                        }

                        idProdPed = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(transaction, idProdPedProducao.Value);

                        prodPed = ProdutosPedidoEspelhoDAO.Instance.GetProdPedByEtiqueta(transaction, etiqueta, idProdPed, true);

                        if (prodPed == null)
                            throw new Exception(msgErro);

                        var qtde = prodPed.Qtde;

                        /* Chamado 44607. */
                        if (prodPed.IsProdutoLaminadoComposicao)
                            qtde = (int)prodPed.QtdeImpressaoProdLamComposicao;
                        else if (prodPed.IsProdFilhoLamComposicao)
                        {
                            qtde = ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(transaction, prodPed.IdProdPedParent.Value) * prodPed.Qtde;

                            var idProdPedParentPai = ProdutosPedidoEspelhoDAO.Instance.ObterIdProdPedParent(transaction, prodPed.IdProdPedParent.Value);

                            if (idProdPedParentPai.GetValueOrDefault(0) > 0)
                                qtde *= ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(transaction, idProdPedParentPai.Value);
                        }

                        if (etiqueta.Split('/')[1].StrParaInt() > qtde)
                            throw new Exception(string.Format("Etiqueta {0} é inválida. O produto referido tem quantidade {1} e a etiqueta indica {2}.", etiqueta, prodPed.Qtde, etiqueta.Split('/')[1]));

                        if (prodPed.IdPedido > 0)
                        {
                            var dataFinalizacaoPCP = PedidoEspelhoDAO.Instance.ObtemDataConf(transaction, prodPed.IdPedido).GetValueOrDefault();
                            var dataUltimaExportacaoEtiqueta = ArquivoOtimizacaoDAO.Instance.ObtemDataUltimaExportacaoEtiqueta(transaction, etiqueta);

                            if (dataUltimaExportacaoEtiqueta != DateTime.MinValue && dataFinalizacaoPCP > dataUltimaExportacaoEtiqueta)
                            {
                                pedidosAlteradosAposExportacao.Add((int)prodPed.IdPedido);
                                continue;
                            }
                        }

                        prodPed.PlanoCorte = planoCorte;
                        prodPed.Etiquetas += etiqueta + "_";
                        prodPed.PecaReposta = isPecaReposta;

                        try
                        {
                            // Insere/Atualiza etiqueta na tabela de produto_impressao
                            ProdutoImpressaoDAO.Instance.InsertOrUpdatePeca(transaction, etiqueta, planoCorte, posicaoArqOtimiz, numSeq, ProdutoImpressaoDAO.TipoEtiqueta.Pedido, forma);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Falha ao importar etiquetas.", ex);
                        }

                        // Marca a qtd como 1, pois cada produto no arquivo é qtd 1, caso o produto se repita, soma a qtd
                        prodPed.QtdAImprimir = !isPecaReposta ? 1 : 0;

                        var jaInserido = false;

                        for (var i = 0; i < produtosPedidoEspelho.Count; i++)
                            if (prodPed.IdProdPed == produtosPedidoEspelho[i].IdProdPed && prodPed.PlanoCorte == produtosPedidoEspelho[i].PlanoCorte && prodPed.PecaReposta == produtosPedidoEspelho[i].PecaReposta)
                            {
                                if (!isPecaReposta)
                                    produtosPedidoEspelho[i].QtdAImprimir += 1;

                                produtosPedidoEspelho[i].Etiquetas += etiqueta + "_";
                                jaInserido = true;
                                break;
                            }

                        if (!jaInserido)
                            produtosPedidoEspelho.Add(prodPed);

                        qtdPecasImpressas++;
                    }

                    #endregion

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

        #endregion

        #region Cria uma nova impressão

        #region Classe de suporte

        private static class DadosEtiqueta
        {
            public class Dados
            {
                public uint? IdProdPed, IdAmbientePedido, IdProdNf;
                public uint IdPedido, IdNf;
                public int Pos;
                public float Qtde;
                public string Lote;
                public List<int> ItensImpressos;
            }

            private static readonly List<Dados> _itens = new List<Dados>();

            public static void Limpar()
            {
                _itens.Clear();
            }

            public static void SetItemImpresso(Dados dados, int itemImpresso)
            {
                int index = _itens.IndexOf(dados);
                if (index > -1 && index < _itens.Count)
                    _itens[index].ItensImpressos.Add(itemImpresso);

                _itens[index].ItensImpressos.Sort();
            }

            public static Dados GetItem(uint? idProdPed, uint? idAmbientePedido, uint? idProdNf)
            {
                return GetItem(null, idProdPed, idAmbientePedido, idProdNf);
            }

            public static Dados GetItem(GDASession session, uint? idProdPed, uint? idAmbientePedido, uint? idProdNf)
            {
                Dados ret;

                if ((ret = (idProdPed > 0 ? _itens.Find(x => x.IdProdPed == idProdPed) :
                    idAmbientePedido > 0 ? _itens.Find(x => x.IdAmbientePedido == idAmbientePedido) :
                    _itens.Find(x => x.IdProdNf == idProdNf))) == null)
                {
                    ret = new Dados();

                    if (idProdPed > 0)
                    {
                        ret.IdProdPed = idProdPed;
                        ret.IdPedido = ProdutosPedidoEspelhoDAO.Instance.ObtemIdPedido(session, idProdPed.Value);
                        ret.Pos = ProdutosPedidoEspelhoDAO.Instance.GetProdPosition(session, ret.IdPedido, idProdPed.Value);
                        ret.Qtde = ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(session, idProdPed.Value);

                        var idProdPedParent = ProdutosPedidoEspelhoDAO.Instance.ObterIdProdPedParent(session, idProdPed.Value);
                        if (idProdPedParent.GetValueOrDefault(0) > 0)
                        {
                            ret.Qtde *= ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(session, idProdPedParent.Value);

                            var idProdPedParentPai = ProdutosPedidoEspelhoDAO.Instance.ObterIdProdPedParent(session, idProdPedParent.Value);

                            if (idProdPedParentPai.GetValueOrDefault(0) > 0)
                                ret.Qtde *= ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(session, idProdPedParentPai.Value);
                        }

                        string itensImpressos = ImpressaoEtiquetaDAO.Instance.GetValoresCampo(session, @"
                            select itemEtiqueta from produto_impressao where idProdPed=?pp and !coalesce(cancelado,false) and 
                            idImpressao in (select * from (select idImpressao from impressao_etiqueta where situacao<>" +
                            (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Cancelada + ") as temp)", "itemEtiqueta", ",",
                            new GDAParameter("?pp", idProdPed));

                        ret.ItensImpressos = new List<int>(String.IsNullOrEmpty(itensImpressos) ? new int[0] :
                            Array.ConvertAll(itensImpressos.Split(','), x => x.StrParaInt()));

                        ret.ItensImpressos.Sort();

                        _itens.Add(ret);
                    }
                    else if (idAmbientePedido > 0)
                    {
                        ret.IdAmbientePedido = idAmbientePedido;
                        ret.IdPedido = AmbientePedidoEspelhoDAO.Instance.ObtemIdPedido(session, idAmbientePedido.Value);
                        ret.Pos = AmbientePedidoEspelhoDAO.Instance.GetAmbientePosition(session, ret.IdPedido, idAmbientePedido.Value);
                        ret.Qtde = AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<int>(session, "qtde", "idAmbientePedido=" + idAmbientePedido.Value);

                        string itensImpressos = ImpressaoEtiquetaDAO.Instance.GetValoresCampo(session, @"
                            select itemEtiqueta from produto_impressao where idAmbientePedido=?ap and !coalesce(cancelado,false) and
                            idImpressao in (select * from (select idImpressao from impressao_etiqueta where situacao<>" +
                            (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Cancelada + ") as temp)", "itemEtiqueta", ",",
                            new GDAParameter("?ap", idAmbientePedido));

                        ret.ItensImpressos = new List<int>(String.IsNullOrEmpty(itensImpressos) ? new int[0] :
                            Array.ConvertAll(itensImpressos.Split(','), x => x.StrParaInt()));

                        ret.ItensImpressos.Sort();

                        _itens.Add(ret);
                    }
                    else if (idProdNf > 0)
                    {
                        ret.IdProdNf = idProdNf;
                        ret.IdNf = ProdutosNfDAO.Instance.ObtemIdNf(session, idProdNf.Value);
                        ret.Pos = ProdutosNfDAO.Instance.GetProdPosition(session, ret.IdNf, idProdNf.Value);
                        ret.Lote = ProdutosNfDAO.Instance.ObtemValorCampo<string>(session, "lote", "idProdNf=" + idProdNf.Value);
                        ret.Qtde = ProdutosNfDAO.Instance.ObtemValorCampo<int>(session, "qtde", "idProdNf=" + idProdNf.Value);

                        string itensImpressos = ImpressaoEtiquetaDAO.Instance.GetValoresCampo(session, @"
                            select itemEtiqueta from produto_impressao where idProdNf=?pnf and !coalesce(cancelado,false) and
                            idImpressao in (select * from (select idImpressao from impressao_etiqueta where situacao<>" +
                            (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Cancelada + ") as temp)", "itemEtiqueta", ",",
                            new GDAParameter("?pnf", idProdNf));

                        ret.ItensImpressos = new List<int>(String.IsNullOrEmpty(itensImpressos) ? new int[0] :
                            Array.ConvertAll(itensImpressos.Split(','), x => x.StrParaInt()));

                        ret.ItensImpressos.Sort();

                        _itens.Add(ret);
                    }
                }

                return ret;
            }
        }

        #endregion

        #region Impressão de pedido

        /// <summary>
        /// Cria uma nova impressão, retornando o ID Gerado
        /// </summary>
        public uint NovaImpressaoPedido(GDASession session, uint idFunc, uint[] vetIdProdPed, uint[] vetIdAmbPed, int[] vetQtdJaImpressa, int[] vetQtdImpressao, int[] vetQtdImpressaoAmb,
            string[] vetEtiqueta, string idImpressaoParam, List<uint> lstPedImpresso, List<uint> lstIdProdPed, List<int> lstQtdImpresso,
            ref List<uint> idsProdPedAlterados, List<string> lstObs, List<uint> lstIdAmbPed, List<int> lstQtdImpressoAmb, List<string> lstObsAmb,
            ref List<uint> idsAmbPedAlterados, int? idSolucaoOtimizacao)
        {
            try
            {
                string etiquetasImpressas = "";

                #region Verifica se há alguma impressão com as mesmas etiquetas

                const int numMaxEtiquetas = 50;
                int numEtiquetas = 0;

                DadosEtiqueta.Limpar();

                for (int i = 0; i < vetIdProdPed.Length; i++)
                {
                    if (numEtiquetas >= numMaxEtiquetas)
                        break;

                    DadosEtiqueta.Dados dados = DadosEtiqueta.GetItem(session, vetIdProdPed[i], null, null);
                    if (dados.ItensImpressos.Count + vetQtdImpressao[i] > dados.Qtde)
                        for (int j = 0; j < dados.ItensImpressos.Count; j++)
                        {
                            // Mesmo que o sistema acuse que determinada peça já foi impressa, pode ser que não tenha sido,
                            // um exemplo ocorreu quando a etiqueta x-y.2/2 estava impressa mas a x-y.1/2 ainda não estava, ao tentar imprimir a segunda
                            // entrava nesta condição dizendo que a etiqueta x-y.2/2 já havia sido impressa, apesar de estar tentando imprimir a x-y.1/2,
                            // a condição abaixo foi feita para resolver esta questão.
                            //if (PCPConfig.ControlarProducao && !String.IsNullOrEmpty(vetEtiqueta[i]) &&
                            //    (!ProdutoPedidoProducaoDAO.Instance.EstaEmProducao(vetEtiqueta[i]) || 
                            //    !ProdutoPedidoProducaoDAO.Instance.LeuProducao(vetEtiqueta[i]) || 
                            //    ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(vetEtiqueta[i], true)))
                            //    continue;

                            if (numEtiquetas++ < numMaxEtiquetas)
                                etiquetasImpressas += dados.IdPedido + "-" + dados.Pos + "." + dados.ItensImpressos[j] + "/" + (int)dados.Qtde + ", ";
                            else
                            {
                                etiquetasImpressas += "...";
                                break;
                            }
                        }
                }

                for (int i = 0; i < vetIdAmbPed.Length; i++)
                {
                    if (numEtiquetas >= numMaxEtiquetas)
                        break;

                    DadosEtiqueta.Dados dados = DadosEtiqueta.GetItem(session, null, vetIdAmbPed[i], null);
                    if (dados.ItensImpressos.Count + vetQtdImpressaoAmb[i] > dados.Qtde)
                        for (int j = 0; j < dados.ItensImpressos.Count; j++)
                        {
                            // Mesmo que o sistema acuse que determinada peça já foi impressa, pode ser que não tenha sido,
                            // um exemplo ocorreu quando a etiqueta x-y.2/2 estava impressa mas a x-y.1/2 ainda não estava, ao tentar imprimir a segunda
                            // entrava nesta condição dizendo que a etiqueta x-y.2/2 já havia sido impressa, apesar de estar tentando imprimir a x-y.1/2,
                            // a condição abaixo foi feita para resolver esta questão.
                            //if (PCPConfig.ControlarProducao && !String.IsNullOrEmpty(vetEtiqueta[i]) &&
                            //    (!ProdutoPedidoProducaoDAO.Instance.EstaEmProducao(vetEtiqueta[i]) || 
                            //    !ProdutoPedidoProducaoDAO.Instance.LeuProducao(vetEtiqueta[i]) || 
                            //    ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(vetEtiqueta[i], true)))
                            //    continue;

                            if (numEtiquetas++ < numMaxEtiquetas)
                                etiquetasImpressas += dados.IdPedido + "-" + dados.Pos + "." + dados.ItensImpressos[j] + "/" + (int)dados.Qtde + ", ";
                            else
                            {
                                etiquetasImpressas += "...";
                                break;
                            }
                        }
                }

                if (!String.IsNullOrEmpty(etiquetasImpressas))
                    etiquetasImpressas = etiquetasImpressas.TrimEnd(',', ' ');

                if (idImpressaoParam == "0" && !String.IsNullOrEmpty(etiquetasImpressas))
                    throw new Exception("Pelo menos uma dessas etiquetas já foi impressa.\nEtiquetas impressas: " + etiquetasImpressas);

                #endregion

                // Verifica se algum pedido está cancelado
                string idsPedido = String.Join(",", Array.ConvertAll(lstPedImpresso.ToArray(), x => x.ToString()));

                // Verifica se há algum pedido cancelado de todos os informados
                string idsPedidoCancelados = PedidoDAO.Instance.ObtemCancelados(session, idsPedido);
                if (!String.IsNullOrEmpty(idsPedidoCancelados))
                    throw new Exception("Os pedidos " + idsPedidoCancelados + " não podem ser impressos porque foram cancelados.");

                // Verifica se algum pedido foi reaberto
                if (PedidoEspelhoDAO.Instance.ExecuteScalar<bool>(session, @"
                    Select Count(*)>0 From pedido 
                    Where idPedido in (?idsPedidos) And situacao In (" + (int)PedidoEspelho.SituacaoPedido.Aberto + "," +
                    (int)PedidoEspelho.SituacaoPedido.Processando + ")", new GDAParameter("?idsPedidos", idsPedido)))
                    throw new Exception("Alguns pedidos foram reabertos após a inserção das peças na tela.");

                // Verifica se algum pedido espelho foi reaberto.
                if (PedidoEspelhoDAO.Instance.ExecuteScalar<bool>(session,
                    string.Format(@"
                        SELECT COUNT(*)>0 FROM pedido_espelho
                        WHERE IdPedido IN (?idsPedidos) AND Situacao IN ({0},{1})",
                        (int)PedidoEspelho.SituacaoPedido.Aberto, (int)PedidoEspelho.SituacaoPedido.Processando),
                    new GDAParameter("?idsPedidos", idsPedido)))
                    throw new Exception("Alguns pedidos foram reabertos após a inserção das peças na tela.");

                /* Chamado 54086. */
                // Verifica se algum pedido está pronto ou entregue.
                if (PedidoEspelhoDAO.Instance.ExecuteScalar<bool>(session, string.Format("SELECT COUNT(*)>0 FROM pedido WHERE IdPedido IN (?idsPedidos) AND SituacaoProducao IN ({0},{1})",
                    (int)Pedido.SituacaoProducaoEnum.Pronto, (int)Pedido.SituacaoProducaoEnum.Entregue), new GDAParameter("?idsPedidos", idsPedido)))
                    throw new Exception("Alguns pedidos estão prontos ou entregues. Portanto, não possuem etiquetas disponíveis para impressão.");

                ImpressaoEtiqueta impressao = new ImpressaoEtiqueta();
                impressao.IdLoja = UserInfo.GetByIdFunc(idFunc).IdLoja;
                impressao.IdFunc = UserInfo.GetByIdFunc(idFunc).CodUser;
                impressao.Data = DateTime.Now;
                impressao.Situacao = (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Processando;
                impressao.IdSolucaoOtimizacao = idSolucaoOtimizacao;
                
                uint idImpressao = 0;

                // Gera uma nova impressão
                idImpressao = Insert(session, impressao);

                ProdutoImpressao prodImp;

                // Salva quais produtos e com qual qtde foram impressos
                for (int i = 0; i < vetIdProdPed.Length; i++)
                {
                    if (vetQtdImpressao[i] == 0)
                    {
                        // Atualiza a observação se for peça de reposição
                        ProdutosPedidoEspelhoDAO.Instance.AtualizaObs(session, lstIdProdPed[i], lstObs[i]);

                        continue;
                    }

                    DadosEtiqueta.Dados dados = DadosEtiqueta.GetItem(session, vetIdProdPed[i], null, null);

                    List<string> etiquetas = new List<string>(!String.IsNullOrEmpty(vetEtiqueta[i]) ?
                        vetEtiqueta[i].Split('_') : new string[0]);

                    // Valida as etiquetas
                    for (int l = 0; l < etiquetas.Count; l++)
                    {
                        // Retira os espaços em branco no início e no final da etiqueta
                        string etiqueta = etiquetas[l];

                        ProdutoPedidoProducaoDAO.Instance.ValidaEtiquetaProducao(session, ref etiqueta);

                        if (etiqueta.Split('/')[1].StrParaInt() != (int)dados.Qtde)
                            throw new Exception("Etiqueta importada é inválida para o produto relacionado. Etiqueta: " + etiqueta);
                    }

                    int offset = 0;
                    for (int j = 1; j <= vetQtdImpressao[i] + offset; j++)
                    {
                        string etiqueta = dados.IdPedido + "-" + dados.Pos + "." + j + "/" + (int)dados.Qtde;
                        ProdutoPedidoProducaoDAO.Instance.ValidaEtiquetaProducao(session, ref etiqueta);

                        // Utiliza etiquetas otimizadas (se houver)
                        if (etiquetas.Count > 0)
                        {
                            if (!etiquetas.Contains(etiqueta))
                            {
                                offset++;
                                continue;
                            }
                        }

                        // Imprime apenas as etiquetas não-impressas
                        if (dados.ItensImpressos.Contains(j))
                        {
                            offset++;
                            continue;
                        }

                        prodImp = ProdutoImpressaoDAO.Instance.GetElementByEtiqueta(session, etiqueta, ProdutoImpressaoDAO.TipoEtiqueta.Pedido);

                        if (prodImp == null)
                        {
                            prodImp = new ProdutoImpressao
                            {
                                IdPedido = dados.IdPedido,
                                PosicaoProd = dados.Pos,
                                ItemEtiqueta = j,
                                QtdeProd = (int) dados.Qtde
                            };
                        }

                        prodImp.IdImpressao = idImpressao;
                        prodImp.IdProdPed = vetIdProdPed[i];
                        ProdutoImpressaoDAO.Instance.InsertOrUpdate(session, prodImp);

                        DadosEtiqueta.SetItemImpresso(dados, j);
                    }
                }

                for (int i = 0; i < vetIdAmbPed.Length; i++)
                {
                    if (ProdutosPedidoEspelhoDAO.Instance.GetCountByAmbiente(session, vetIdAmbPed[i]) == 0)
                        throw new Exception("Cadastre alguma mão de obra no vidro '" + AmbientePedidoEspelhoDAO.Instance.ObtemPecaVidro(session, vetIdAmbPed[i]) + "' antes de continuar.");

                    DadosEtiqueta.Dados dados = DadosEtiqueta.GetItem(session, null, vetIdAmbPed[i], null);

                    int offset = 0;
                    for (int j = 1; j <= vetQtdImpressaoAmb[i] + offset; j++)
                    {
                        // Imprime apenas as etiquetas não-impressas
                        if (dados.ItensImpressos.Contains(j))
                        {
                            offset++;
                            continue;
                        }

                        string etiqueta = dados.IdPedido + "-" + dados.Pos + "." + j + "/" + (int)dados.Qtde;
                        ProdutoPedidoProducaoDAO.Instance.ValidaEtiquetaProducao(session, ref etiqueta);

                        prodImp = ProdutoImpressaoDAO.Instance.GetElementByEtiqueta(session, etiqueta, ProdutoImpressaoDAO.TipoEtiqueta.Pedido);

                        if (prodImp == null)
                        {
                            prodImp = new ProdutoImpressao
                            {
                                IdPedido = dados.IdPedido,
                                PosicaoProd = dados.Pos,
                                ItemEtiqueta = j,
                                QtdeProd = (int) dados.Qtde
                            };
                        }

                        prodImp.IdImpressao = idImpressao;
                        prodImp.IdAmbientePedido = vetIdAmbPed[i];
                        ProdutoImpressaoDAO.Instance.InsertOrUpdate(session, prodImp);

                        DadosEtiqueta.SetItemImpresso(dados, j);
                    }
                }

                // Altera situação dos pedidos para "impresso" se usuário não for Aux. Adm. etiqueta, 
                // caso contrário, altera situação para "impresso comum"
                if (UserInfo.GetByIdFunc(idFunc).TipoUsuario != (uint)Utils.TipoFuncionario.AuxEtiqueta || Glass.Configuracoes.PedidoConfig.LiberarPedido)
                    PedidoEspelhoDAO.Instance.AlteraSituacao(session, lstPedImpresso, Glass.Data.Model.PedidoEspelho.SituacaoPedido.Impresso);
                else
                    PedidoEspelhoDAO.Instance.AlteraSituacao(session, lstPedImpresso, Glass.Data.Model.PedidoEspelho.SituacaoPedido.ImpressoComum);

                // Põe pedidos impressos em produção
                if (!PedidoConfig.LiberarPedido)
                    foreach (uint idPed in lstPedImpresso)
                        PedidoCorteDAO.Instance.AlteraSituacao(session, idFunc, idPed, 2);

                // Marca a quantidade dos itens que serão impressos
                for (int i = 0; i < lstIdProdPed.Count; i++)
                {
                    if (lstQtdImpresso[i] == 0)
                        continue;

                    ProdutosPedidoEspelhoDAO.Instance.MarcarImpressao(session, lstIdProdPed[i], lstQtdImpresso[i], lstObs[i]);

                    if (!idsProdPedAlterados.Contains(lstIdProdPed[i]))
                        idsProdPedAlterados.Add(lstIdProdPed[i]);
                }

                for (int i = 0; i < lstIdAmbPed.Count; i++)
                {
                    AmbientePedidoEspelhoDAO.Instance.MarcarImpressao(session, lstIdAmbPed[i], lstQtdImpressoAmb[i], lstObsAmb[i]);

                    if (!idsAmbPedAlterados.Contains(lstIdAmbPed[i]))
                        idsAmbPedAlterados.Add(lstIdAmbPed[i]);
                }

                if (idSolucaoOtimizacao.HasValue)
                    ProdutoImpressaoDAO.Instance.AtualizarImpressaoRetalhosSolucaoOtimizacao(session, idSolucaoOtimizacao.Value, (int)idImpressao);

                return idImpressao;
            }
            catch (Exception ex)
            {
                // Chamado 13478.
                // Caso ocorra algum erro ao imprimir etiquetas nós saberemos se o erro ocorreu neste método ou não.
                ErroDAO.Instance.InserirFromException("NovaImpressaoPedido", ex);
                    
                throw;
            }
        }

        #endregion

        #region Impressão de nota fiscal

        /// <summary>
        /// Cria uma nova impressão, retornando o ID Gerado
        /// </summary>
        public uint NovaImpressaoNFe(GDASession session, uint idFunc, uint[] vetIdProdNf, int[] vetQtdJaImpressa, int[] vetQtdImpressao, string idImpressaoParam,
            List<uint> lstNfImpresso, List<uint> lstIdProdNf, List<int> lstQtdImpresso, List<string> lstObs, ref List<uint> idsProdNfAlterados)
        {
            string etiquetasImpressas = "";

            #region Verifica se há alguma impressão com as mesmas etiquetas

            const int numMaxEtiquetas = 50;
            int numEtiquetas = 0;

            DadosEtiqueta.Limpar();

            for (int i = 0; i < vetIdProdNf.Length; i++)
            {
                if (numEtiquetas >= numMaxEtiquetas)
                    break;

                DadosEtiqueta.Dados dados = DadosEtiqueta.GetItem(session, null, null, vetIdProdNf[i]);
                if (dados.ItensImpressos.Count + vetQtdImpressao[i] > dados.Qtde)
                    for (int j = 0; j < dados.ItensImpressos.Count; j++)
                    {
                        // Mesmo que o sistema acuse que determinada peça já foi impressa, pode ser que não tenha sido,
                        // um exemplo ocorreu quando a etiqueta x-y.2/2 estava impressa mas a x-y.1/2 ainda não estava, ao tentar imprimir a segunda
                        // entrava nesta condição dizendo que a etiqueta x-y.2/2 já havia sido impressa, apesar de estar tentando imprimir a x-y.1/2,
                        // a condição abaixo foi feita para resolver esta questão.
                        //if (PCPConfig.ControlarProducao && !String.IsNullOrEmpty(vetEtiqueta[i]) &&
                        //    (!ProdutoPedidoProducaoDAO.Instance.EstaEmProducao(vetEtiqueta[i]) || 
                        //    !ProdutoPedidoProducaoDAO.Instance.LeuProducao(vetEtiqueta[i]) || 
                        //    ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(vetEtiqueta[i], true)))
                        //    continue;

                        if (numEtiquetas++ < numMaxEtiquetas)
                            etiquetasImpressas += "N" + dados.IdNf + "-" + dados.Pos + "." + dados.ItensImpressos[j] + "/" + (int)dados.Qtde + ", ";
                        else
                        {
                            etiquetasImpressas += "...";
                            break;
                        }
                    }
            }

            if (!String.IsNullOrEmpty(etiquetasImpressas))
                etiquetasImpressas = etiquetasImpressas.TrimEnd(',', ' ');

            if (idImpressaoParam == "0" && !String.IsNullOrEmpty(etiquetasImpressas))
                throw new Exception("Pelo menos uma dessas etiquetas já foi impressa.\nEtiquetas impressas: " + etiquetasImpressas);

            #endregion

            // Verifica se alguma nota fiscal está aberta
            string idsNf = String.Join(",", Array.ConvertAll(lstNfImpresso.ToArray(), x => x.ToString()));

            // Verifica se há algum pedido cancelado de todos os informados
            string idsNfCancelados = NotaFiscalDAO.Instance.ObtemAbertas(session, idsNf);
            if (!String.IsNullOrEmpty(idsNfCancelados))
                throw new Exception("As NF-e " + idsNfCancelados + " não podem ser impressos porque estão abertas.");

            ImpressaoEtiqueta impressao = new ImpressaoEtiqueta
            {
                IdLoja = UserInfo.GetByIdFunc(idFunc).IdLoja,
                IdFunc = UserInfo.GetByIdFunc(idFunc).CodUser,
                Data = DateTime.Now,
                Situacao = (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Processando
            };

            uint idImpressao = 0;

            // Gera uma nova impressão
            idImpressao = Insert(session, impressao);

            // Salva quais produtos e com qual qtde foram impressos
            for (int i = 0; i < vetIdProdNf.Length; i++)
            {
                DadosEtiqueta.Dados dados = DadosEtiqueta.GetItem(session, null, null, vetIdProdNf[i]);

                int offset = 0;
                for (int j = 1; j <= vetQtdImpressao[i] + offset; j++)
                {
                    // Imprime apenas as etiquetas não-impressas
                    if (dados.ItensImpressos.Contains(j))
                    {
                        offset++;
                        continue;
                    }

                    string etiqueta = "N" + dados.IdNf + "-" + dados.Pos + "." + j + "/" + (int)dados.Qtde;
                    var prodImp = ProdutoImpressaoDAO.Instance.GetElementByEtiqueta(session, etiqueta, ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal);

                    if (prodImp == null)
                    {
                        prodImp = new ProdutoImpressao
                        {
                            IdNf = dados.IdNf,
                            PosicaoProd = dados.Pos,
                            ItemEtiqueta = j,
                            Lote = dados.Lote,
                            QtdeProd = (int)dados.Qtde
                        };
                    }

                    prodImp.IdImpressao = idImpressao;
                    prodImp.IdProdNf = vetIdProdNf[i];
                    ProdutoImpressaoDAO.Instance.InsertOrUpdate(session, prodImp);

                    DadosEtiqueta.SetItemImpresso(dados, j);
                }
            }

            // Marca a quantidade dos itens que serão impressos
            for (int i = 0; i < lstIdProdNf.Count; i++)
            {
                if (lstQtdImpresso[i] > 0)
                    ProdutosNfDAO.Instance.MarcarImpressao(session, lstIdProdNf[i], lstQtdImpresso[i], lstObs[i]);

                if (!idsProdNfAlterados.Contains(lstIdProdNf[i]))
                    idsProdNfAlterados.Add(lstIdProdNf[i]);
            }

            return idImpressao;
        }

        #endregion

        #region Impressão de retalhos

        public uint NovaImpressaoRetalho(GDASession sessao, uint idFunc, List<RetalhoProducao> listaRetalhoProducao)
        {
            ImpressaoEtiqueta impressao = new ImpressaoEtiqueta
            {
                IdLoja = UserInfo.GetByIdFunc(idFunc).IdLoja,
                IdFunc = UserInfo.GetByIdFunc(idFunc).CodUser,
                Data = DateTime.Now,
                Situacao = (int) ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Processando
            };

            uint idImpressao;

            // Gera uma nova impressão
            idImpressao = Insert(sessao, impressao);

            foreach (RetalhoProducao item in listaRetalhoProducao)
            {
                string etiqueta = "R" + item.IdRetalhoProducao + "-1/1";
                var prodImp = ProdutoImpressaoDAO.Instance.GetElementByEtiqueta(sessao, etiqueta, ProdutoImpressaoDAO.TipoEtiqueta.Retalho);

                if (prodImp == null)
                {
                    prodImp = new ProdutoImpressao
                    {
                        IdRetalhoProducao = item.IdRetalhoProducao,
                        PosicaoProd = 1,
                        ItemEtiqueta = 1,
                        QtdeProd = 1,
                        Lote = item.Lote
                    };
                }

                prodImp.IdImpressao = idImpressao;

                ProdutoImpressaoDAO.Instance.InsertOrUpdate(sessao, prodImp);
            }

            return idImpressao;
        }

        #endregion

        #region Impressao de Box

        /// <summary>
        /// Cria uma nova impressão de etiqueta de box
        /// </summary>
        public uint NovaImpressaoBox(uint idFunc, List<Etiqueta> lstPecasBox)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var impressao = new ImpressaoEtiqueta
                    {
                        IdLoja = UserInfo.GetByIdFunc(idFunc).IdLoja,
                        IdFunc = UserInfo.GetByIdFunc(idFunc).CodUser,
                        Data = DateTime.Now,
                        Situacao = (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Processando
                    };

                    uint idImpressao = 0;

                    // Gera uma nova impressão
                    idImpressao = Insert(transaction, impressao);

                    // Salva quais produtos foram impressos
                    for (int i = 0; i < lstPecasBox.Count; i++)
                    {
                        ProdutoImpressaoDAO.Instance.InsertOrUpdate(transaction, new ProdutoImpressao()
                        {
                            IdImpressao = idImpressao,
                            IdProdPedBox = lstPecasBox[i].IdProdPedBox
                        });
                    }

                    //Marca a quantidade de box que foi impressa
                    foreach (var i in lstPecasBox.GroupBy(f => f.IdProdPedBox).Select(f => new { IdProdPedBox = f.Key, Qtde = f.Count() }))
                        ProdutosPedidoDAO.Instance.MarcarBoxImpressao(transaction, i.IdProdPedBox, i.Qtde);

                    transaction.Commit();
                    transaction.Close();

                    return idImpressao;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        #endregion

        #region Retorna os números das etiquetas não impressas

        public IList<string> ObtemEtiquetasNaoImpressas(uint idProdPed, int qtdeImprimir)
        {
            DadosEtiqueta.Limpar();

            var retorno = new List<string>();
            var dados = DadosEtiqueta.GetItem(idProdPed, null, null);

            int qtdeSomar = 0;
            for (int i = 0; i < (int)dados.Qtde && qtdeSomar < qtdeImprimir; i++)
            {
                if (dados.ItensImpressos.Contains(i))
                    continue;

                retorno.Add(EtiquetaDAO.Instance.GetNumEtiqueta(dados.IdPedido, dados.Pos, i + 1, (int)dados.Qtde, ProdutoImpressaoDAO.TipoEtiqueta.Pedido));
                qtdeSomar++;
            }

            return retorno;
        }

        #endregion

        #endregion

        #region Cancela uma impressão

        public void CancelarImpressaoComTransacao(uint idFunc, uint idImpressao, uint? idPedido, uint? numeroNFe, string planoCorte, uint idProdImpressao,
            string motivo, bool cancelarRetalhos)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    #region Recupera as peças impressas do pedido antes do cancelamento

                    var pedidosDescricaoPecasImpressasAntigas = new Dictionary<int, string>();

                    if (PedidoConfig.SalvarLogPecasImpressasNoPedido)
                    {
                        if (idPedido > 0)
                            pedidosDescricaoPecasImpressasAntigas.Add((int)idPedido.Value,
                                ProdutoImpressaoDAO.Instance.ObterEtiquetasImpressasPeloPedido(transaction, (int)idPedido.Value));
                        else if (idProdImpressao > 0)
                        {
                            var idsPedido =
                                ProdutoImpressaoDAO.Instance.ExecuteMultipleScalar<int>(transaction,
                                    string.Format("SELECT DISTINCT IdPedido FROM produto_impressao WHERE IdProdImpressao={0}",
                                        idProdImpressao));

                            if (idsPedido != null && idsPedido.Count > 0)
                                foreach (var id in idsPedido)
                                    pedidosDescricaoPecasImpressasAntigas.Add(id,
                                        ProdutoImpressaoDAO.Instance.ObterEtiquetasImpressasPeloPedido(transaction, id));
                        }
                        else if (!string.IsNullOrEmpty(planoCorte))
                        {
                            var idsPedido =
                                ProdutoImpressaoDAO.Instance.ExecuteMultipleScalar<int>(transaction,
                                    string.Format("SELECT DISTINCT IdPedido FROM produto_impressao WHERE IdImpressao={0} AND PlanoCorte=?planoCorte",
                                        idImpressao), new GDAParameter("?planoCorte", planoCorte));

                            if (idsPedido != null && idsPedido.Count > 0)
                                foreach (var id in idsPedido)
                                    pedidosDescricaoPecasImpressasAntigas.Add(id,
                                        ProdutoImpressaoDAO.Instance.ObterEtiquetasImpressasPeloPedido(transaction, id));
                        }
                        else if (idImpressao > 0)
                        {
                            var idsPedido =
                                ProdutoImpressaoDAO.Instance.ExecuteMultipleScalar<int>(transaction,
                                    string.Format("SELECT DISTINCT IdPedido FROM produto_impressao WHERE IdImpressao={0}", idImpressao));

                            if (idsPedido != null && idsPedido.Count > 0)
                                foreach (var id in idsPedido)
                                    pedidosDescricaoPecasImpressasAntigas.Add(id,
                                        ProdutoImpressaoDAO.Instance.ObterEtiquetasImpressasPeloPedido(transaction, id));
                        }
                    }

                    #endregion

                    CancelarImpressao(transaction, idFunc, idImpressao, idPedido, numeroNFe, planoCorte, idProdImpressao, motivo, cancelarRetalhos);

                    #region Recupera as peças impressas do pedido após o cancelamento e insere o log

                    if (PedidoConfig.SalvarLogPecasImpressasNoPedido)
                        foreach (var pedidoDescricaoPecasImpresasAntigas in pedidosDescricaoPecasImpressasAntigas)
                        {
                            var idPedidoLog = pedidoDescricaoPecasImpresasAntigas.Key;
                            var descricaoAntiga = pedidoDescricaoPecasImpresasAntigas.Value;
                            var descricaoNova =
                                ProdutoImpressaoDAO.Instance.ObterEtiquetasImpressasPeloPedido(transaction, idPedidoLog);

                            LogAlteracaoDAO.Instance.LogPedidoPecaImpressa(transaction, idFunc, idPedidoLog, descricaoAntiga, descricaoNova);
                        }

                    #endregion

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
        /// Cancela uma impressão.
        /// </summary>
        internal void CancelarImpressao(GDASession sessao, uint idFunc, uint idImpressao, uint? idPedido, uint? numeroNFe, string planoCorte, uint idProdImpressao,
            string motivo, bool cancelarRetalhos)
        {
            try
            {
                string idsProdImpressao;

                // Define se será necessário efetuar cancelamento peça a peça da impressão (ocorre quando não se deseja cancelar toda a impressão,
                // somente determinado pedido ou plano de corte ou peça individual)
                bool cancelarPecaAPeca = idPedido > 0 || numeroNFe > 0 || !String.IsNullOrEmpty(planoCorte) || idProdImpressao > 0;

                // Recupera todos os produto_impressao que serão cancelados
                if (idProdImpressao > 0)
                {
                    var numEtiqueta = ProdutoImpressaoDAO.Instance.ObtemNumEtiqueta(sessao, idProdImpressao);

                    if (ProdutoImpressaoDAO.Instance.ObtemValorCampo<bool>(sessao, "cancelado", "idProdImpressao=" + idProdImpressao))
                        throw new Exception("Etiqueta já está cancelada.");
                    
                    if (ChapaCortePecaDAO.Instance.ChapaPossuiLeitura(sessao, idProdImpressao))
                        throw new Exception("Matéria-prima associada a peça já foi utilizada.");

                    idsProdImpressao = idProdImpressao.ToString();
                    idImpressao = ProdutoImpressaoDAO.Instance.ObtemValorCampo<uint>(sessao, "idImpressao", "idProdImpressao=" + idProdImpressao);
                }
                else
                    idsProdImpressao = ProdutoImpressaoDAO.Instance.GetByIdPedidoPlanoCorte(sessao, idImpressao, idPedido, numeroNFe, planoCorte);

                var situacaoImpressao = ObtemValorCampo<ImpressaoEtiqueta.SituacaoImpressaoEtiqueta>(sessao, "situacao",
                    "idImpressao=" + idImpressao);

                // Se a impressão esta na situação processando ela deve ser cancelada totalmente.
                if (situacaoImpressao == ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Processando && cancelarPecaAPeca)
                    throw new Exception("Impressões na situação Processando devem ser canceladas toda a impressão.");

                if (idProdImpressao == 0 && (!Exists(sessao, idImpressao) || situacaoImpressao == ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Cancelada))
                    return;

                #region Valida cancelamento por pedido/plano de corte

                /* Chamado 16544.
                 * Caso o usuário tente cancelar as etiquetas, já canceladas, de um pedido, lança uma exceção. */
                if (String.IsNullOrEmpty(idsProdImpressao) && idPedido > 0)
                    throw new Exception("As etiquetas deste pedido já foram canceladas nesta impressão.");

                /* Chamado 16544.
                 * Caso o usuário tente cancelar as etiquetas, já canceladas, de um plano de corte, lança uma exceção. */
                if (String.IsNullOrEmpty(idsProdImpressao) && !String.IsNullOrEmpty(planoCorte))
                    throw new Exception("As etiquetas deste plano de corte já foram canceladas nesta impressão.");

                #endregion

                // Verifica se há alguma peça para cancelar.
                // 04/12/2014. Ocorreu na Modelo e na MS Vidros casos em que a impressão não tinha produto, ao tentar cancelar a impressão
                // o sistema lançava uma exceção, mas o correto é marcar a impressão como cancelada. Alteramos o sistema para que, neste caso,
                // a impressão seja cancelada e o log de cancelamento seja incluído no banco de dados.
                if (string.IsNullOrEmpty(idsProdImpressao))
                {
                    // Altera a situação da impressão de etiqueta.
                    objPersistence.ExecuteCommand(sessao, "UPDATE impressao_etiqueta ie SET ie.situacao=" +
                        (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Cancelada + " WHERE ie.idImpressao=" + idImpressao);

                    // Insere o log de cancelamento.
                    LogCancelamentoDAO.Instance.LogImpressaoEtiquetas(sessao, idFunc, GetElementByPrimaryKey(sessao, idImpressao), motivo, true);

                    // Força a saída do método.
                    return;
                }

                /* Chamado 63971. */
                if (ChapaCortePecaDAO.Instance.ChapasPossuemLeitura(sessao, idsProdImpressao.Split(',').Select(f => f.ToString().StrParaInt()).ToList()))
                    throw new Exception("Uma ou mais matérias-primas a serem canceladas já foram utilizadas.");

                // Verifica se esta impressão possui algum pedido já liberado e bloqueia
                if (PedidoConfig.LiberarPedido && ExecuteScalar<bool>(sessao, @"
                    Select Count(*)>0 
                    From produto_impressao pi
                        Inner Join pedido p On (pi.idPedido=p.idPedido)
                    Where idProdImpressao In (" + idsProdImpressao + @") 
                        And p.situacao=" + (int)Pedido.SituacaoPedido.Confirmado + @"
                        And p.tipoPedido<>" + (int)Pedido.TipoPedidoEnum.Producao)
                    )
                    throw new Exception("A impressão não pode ser cancelada. Alguns pedidos desta impressão já foram liberados, cancele a liberação antes de cancelar esta impressão.");

                ProdutoImpressaoDAO.TipoEtiqueta tipoImpressao = GetTipoImpressao(sessao, idImpressao);

                bool temCarregamento = false;

                List<uint> idsProdPedProducao = new List<uint>();

                //Verifica se esta impressão possui algum pedido que esta em um carregamento e bloqueia
                if (tipoImpressao == ProdutoImpressaoDAO.TipoEtiqueta.Pedido)
                {
                    var sqlCarregamento = @"
                        SELECT DISTINCT(ic.IdPedido)
                        FROM item_carregamento ic
                            INNER JOIN produto_pedido_producao ppp ON (ic.idProdPedProducao = ppp.idProdPedProducao)
                            INNER JOIN produto_impressao pi ON (ppp.idImpressao = pi.idImpressao AND pi.numEtiqueta = ppp.numEtiqueta)
                        WHERE pi.idProdImpressao IN (" + idsProdImpressao + ")";

                    // Verifica se tem pedidos vinculados a Carregamento, e quais são eles.
                    var pedidosVinculadosCarregamento = ExecuteMultipleScalar<uint>(sessao, sqlCarregamento);
                    if (pedidosVinculadosCarregamento != null && pedidosVinculadosCarregamento.Count > 0)
                        temCarregamento = true;

                    if (temCarregamento && situacaoImpressao != ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Processando)
                        throw new Exception(string.Format(@"A impressão não pode ser cancelada. O(s) pedido(s) {0} desta impressão está(ão) vinculado(s) a um carregamento, cancele o carregamento antes de cancelar esta impressão.",
                            string.Join(",", pedidosVinculadosCarregamento)));

                    idsProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdsProdPedProducaoByIdProdImpressao(sessao, idsProdImpressao);

                    /* Chamado 55897. */
                    if (idsProdPedProducao != null && idsProdPedProducao.Count > 0)
                    {
                        foreach (var idProdPedProducao in idsProdPedProducao)
                        {
                            var idProdPedProducaoParent = ProdutoPedidoProducaoDAO.Instance.ObterIdProdPedProducaoParent(sessao, idProdPedProducao);

                            if (idProdPedProducaoParent > 0)
                            {
                                var numEtiquetaPai = ProdutoPedidoProducaoDAO.Instance.ObterNumEtiqueta(sessao, idProdPedProducaoParent.Value);

                                if (!string.IsNullOrWhiteSpace(numEtiquetaPai) && ProdutoImpressaoDAO.Instance.EstaImpressa(sessao, numEtiquetaPai, ProdutoImpressaoDAO.TipoEtiqueta.Pedido))
                                    throw new Exception("Não é possível cancelar peças de composição que estejam associadas à peças compostas impressas.");
                            }
                        }
                    }
                }
                else if (tipoImpressao == ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal)
                {
                    var sqlPerdas = @"
                        SELECT count(*) > 0
                        FROM perda_chapa_vidro
                         WHERE IdProdImpressao IN (" + idsProdImpressao + ") and coalesce(cancelado,0) = 0";

                    if (ExecuteScalar<bool>(sessao, sqlPerdas))
                        throw new Exception("A impressão não pode ser cancelada, pois possui peças que foram marcadas como perda. Cancele as perdas antes de cancelar esta impressão");
                }

                // Marca a impressão como processando, para que caso o processo não seja concluído, não faça nada com esta impressão
                if ((idPedido ?? 0) == 0 && (numeroNFe ?? 0) == 0 && String.IsNullOrEmpty(planoCorte) && idProdImpressao == 0 && idImpressao > 0)
                    objPersistence.ExecuteCommand(sessao, "Update impressao_etiqueta Set situacao=" +
                        (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Processando + " Where idImpressao=" + idImpressao);

                List<uint> idsProdImpressaoRetalho = new List<uint>();

                if (tipoImpressao == ProdutoImpressaoDAO.TipoEtiqueta.Retalho)
                {
                    // Recupera quais produtos impressos são referentes a retalhos
                    idsProdImpressaoRetalho = ExecuteMultipleScalar<uint>(sessao, @"select idRetalhoProducao from produto_impressao 
                        where idRetalhoProducao is not null and idProdImpressao in (" + idsProdImpressao + ")").ToList().FindAll(x => x > 0);

                    foreach (uint idRetalhoProducao in idsProdImpressaoRetalho)
                        if (!RetalhoProducaoDAO.Instance.PodeCancelar(sessao, idRetalhoProducao))
                        {
                            RetalhoProducao r = RetalhoProducaoDAO.Instance.Obter(sessao, idRetalhoProducao);
                            throw new Exception("Não é possível cancelar o retalho " + r.NumeroEtiqueta + " porque ele está em uso.");
                        }
                }

                // Chamado 11444. Marca os registros produtoPedidoProducao como não repostos para evitar o erro ao imprimir peças
                // repostas de impressões canceladas, que ocorria porque ao recuperar o produto de impressão nada era recuperado,
                // pois, o produto de impressão estava reposto e cancelado, e ele é recuperado somente se estiver reposto e ativo.
                // A peça deve ser marcada como não reposta neste momento para que o campo "QtdImpresso" seja alterado corretamente.
                if (cancelarPecaAPeca)
                    objPersistence.ExecuteCommand(sessao, @"
                        UPDATE produto_pedido_producao ppp
                            INNER JOIN produto_impressao pi ON (ppp.idImpressao=pi.idImpressao AND pi.idProdPed=ppp.idProdPed AND 
                                ppp.numEtiqueta=pi.numEtiqueta)
                        SET ppp.pecaReposta=FALSE
                        WHERE pi.idProdImpressao IN (" + idsProdImpressao + ")");
                else if (idImpressao > 0)
                    objPersistence.ExecuteCommand(sessao, @"
                        UPDATE produto_pedido_producao ppp
                            INNER JOIN produto_impressao pi ON (ppp.idImpressao=pi.idImpressao AND pi.idProdPed=ppp.idProdPed AND 
                                ppp.numEtiqueta=pi.numEtiqueta)
                        SET ppp.pecaReposta=FALSE
                        Where ppp.idImpressao=" + idImpressao);

                // Subtrai a qtd impressa de cada item desta impressão no produtoPedidoEspelho que os mesmos estão relacionados.
                objPersistence.ExecuteCommand(sessao, @"Update produtos_pedido_espelho ppe 
                    Inner Join (
                        Select idProdPed, Count(*) As qtdeImpresso
                        From (
                            Select pi.idProdPed
                            From produto_impressao pi
                                /* Johan - deve ser Left Join para que o cancelamento seja feito se houver erro na impressão */
                                Left Join produto_pedido_producao ppp On (pi.idProdPed=ppp.idProdPed and 
                                    pi.numEtiqueta=ppp.numEtiqueta)
                            Where pi.idProdImpressao In (" + idsProdImpressao + @") And Coalesce(ppp.pecaReposta, False)=False
                                And !Coalesce(pi.cancelado, False) And pi.idImpressao Is Not Null
                        ) As temp
                        Group By idProdPed
                    ) pi On (ppe.idProdPed=pi.idProdPed) 
                    Set ppe.qtdImpresso=IF(ppe.qtdImpresso=0, 0, ppe.qtdImpresso-Coalesce(pi.qtdeImpresso,0))");

                // Subtrai a qtd impressa de cada item desta impressão nos ambientesPedidoEspelho.
                objPersistence.ExecuteCommand(sessao, @"Update ambiente_pedido_espelho ape
                    Inner Join (
                        Select idAmbientePedido, Count(*) As qtdeImpresso
                        From (
                            Select pi.idAmbientePedido
                            From produto_impressao pi
                                /* Johan - deve ser Left Join para que o cancelamento seja feito se houver erro na impressão */
                                Left Join produto_pedido_producao ppp On (pi.idProdPed=ppp.idProdPed and 
                                    pi.numEtiqueta=ppp.numEtiqueta)
                            Where pi.idProdImpressao In (" + idsProdImpressao + @") And Coalesce(ppp.pecaReposta, False)=False
                                And !Coalesce(pi.cancelado, False) And pi.idImpressao Is Not Null
                        ) As temp
                        Group By idAmbientePedido
                    ) pi On (ape.idAmbientePedido=pi.idAmbientePedido)
                    Set ape.qtdeImpresso=ape.qtdeImpresso-Coalesce(pi.qtdeImpresso, 0)");

                // Subtrai a qtd impressa de cada item desta impressão nos produtos da nota fiscal
                objPersistence.ExecuteCommand(sessao, @"update produtos_nf pnf
                    Inner Join (
                        Select pi.idProdNf, Count(*) as qtdeImpresso
                        From produto_impressao pi
                        Where pi.idProdImpressao In (" + idsProdImpressao + @") And !Coalesce(pi.cancelado, False)
                            And pi.idImpressao Is Not Null
                        Group By pi.idProdNf
                    ) pi On (pnf.idProdNf=pi.idProdNf)
                    Set pnf.qtdImpresso=pnf.qtdImpresso-Coalesce(pi.qtdeImpresso,0)");

                //Subtrai a qtde impressa de cada item desta impressão dos box impressos
                objPersistence.ExecuteCommand(sessao, @"update produtos_pedido pp
                    Inner Join (
                        Select pi.IdProdPedBox, Count(*) as qtdeImpresso
                        From produto_impressao pi
                        Where pi.idProdImpressao In (" + idsProdImpressao + @") And !Coalesce(pi.cancelado, False)
                            And pi.idImpressao Is Not Null
                        Group By pi.IdProdPedBox
                    ) pi On (pp.IdProdPed = pi.IdProdPedBox)
                    Set pp.qtdeBoxImpresso = pp.qtdeBoxImpresso - Coalesce(pi.qtdeImpresso, 0)");

                /* Chamado 32174. */
                if (!string.IsNullOrEmpty(idsProdImpressao) &&
                    tipoImpressao == ProdutoImpressaoDAO.TipoEtiqueta.Pedido)
                    ChapaCortePecaDAO.Instance.DeleteByIdsProdImpressaoPeca(sessao,
                        idsProdImpressao.Split(',').Select(f => f.StrParaInt()).ToList());

                string ids;

                // Apaga as datas de leituras das peças dessa impressão, considerando peça por peça da impressão
                if (cancelarPecaAPeca)
                    ids = string.Join(",", ExecuteMultipleScalar<uint>(sessao, @"
                        Select Distinct ppp.idProdPedProducao 
                        From produto_impressao pi
                            Inner Join produto_pedido_producao ppp On (pi.idImpressao=ppp.idImpressao And pi.numEtiqueta=ppp.numEtiqueta)
                        Where pi.idProdImpressao In (" + idsProdImpressao + @") 
                            And !Coalesce(pi.cancelado,false)"));
                // Apaga as datas de leituras das peças dessa impressão, considerando todas as peças da impressão
                else
                    ids = string.Join(",", ExecuteMultipleScalar<uint>(sessao, @"
                        Select Distinct ppp.idProdPedProducao From produto_pedido_producao ppp Where ppp.idImpressao=" + idImpressao));

                if (!string.IsNullOrEmpty(ids))
                {
                    if (temCarregamento && situacaoImpressao == ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Processando)
                    {
                        LeituraProducaoDAO.Instance.AtualizarDataLeituraIdsProdPedProducao(sessao, ids?.Split(',')?.Select(f => f.StrParaInt())?.ToList() ?? new List<int>(), null);
                    }
                    else
                    {
                        LeituraProducaoDAO.Instance.ApagarPelosIdsProdPedProducao(sessao, ids?.Split(',')?.Select(f => f.StrParaInt())?.ToList() ?? new List<int>());
                    }

                    /* Chamado 45146. */
                    foreach (var id in ids.Split(','))
                    {
                        var idProdPedProducao = id.StrParaInt();
                        
                        if (idProdPedProducao > 0 && PedidoDAO.Instance.IsProducao(sessao, ProdutoPedidoProducaoDAO.Instance.ObtemIdPedido(sessao, (uint)idProdPedProducao)))
                        {
                            var codEtiqueta = ProdutoPedidoProducaoDAO.Instance.ObtemEtiqueta(sessao, (uint)idProdPedProducao);

                            if (ProdutoPedidoProducaoDAO.Instance.EntrouEmEstoque(sessao, codEtiqueta))
                            {
                                LoginUsuario login = UserInfo.GetUserInfo;
                                ProdutosPedidoEspelho prodPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetProdPedByEtiqueta(sessao, null,
                                    ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(sessao, (uint)idProdPedProducao), true);

                                float m2Calc = Global.CalculosFluxo.ArredondaM2(sessao, prodPedEsp.Largura, (int)prodPedEsp.Altura, 1, 0, prodPedEsp.Redondo);
                                
                                MovEstoqueDAO.Instance.BaixaEstoqueProducao(sessao, prodPedEsp.IdProd, login.IdLoja, (uint)idProdPedProducao, 1, 0, false, false, true);

                                // Só baixa apenas se a peça possuir produto para baixa associado
                                MovEstoqueDAO.Instance.CreditaEstoqueProducao(sessao, prodPedEsp.IdProd, login.IdLoja, (uint)idProdPedProducao, (decimal)(m2Calc > 0 &&
                                    !ProdutoPedidoProducaoDAO.Instance.PecaPassouSetorLaminado(sessao, codEtiqueta) ? m2Calc : 1), true, true);

                                // Marca que este produto entrou em estoque
                                objPersistence.ExecuteCommand(sessao, "UPDATE produto_pedido_producao SET EntrouEstoque=0 WHERE IdProdPedProducao=" + idProdPedProducao);
                            }
                        }
                    }
                }

                // Atualiza as peças repostas na tabela produto_impressao, voltando-as para a impressão anterior.
                string sqlIdImpressao = ProdutoPedidoProducaoDAO.Instance.SqlIdImpressao("ppp.dadosReposicaoPeca");
                foreach (ProdutoImpressao pi in ProdutoImpressaoDAO.Instance.GetListByIds(sessao, idsProdImpressao, true))
                    if (ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(sessao, pi.NumEtiqueta, false))
                    {
                        // Foi necessário recuperar o idImpressao antes de atualizá-lo, 
                        // pois em alguns casos estava ocorreno erro de "truncated INTEGER", pois recuperava um valor vazio
                        var idImpressaoRep = ExecuteScalar<uint>(sessao, @"
                            Select Cast((" + sqlIdImpressao + @") As Signed) 
                            From produto_impressao pi
                                Inner Join produto_pedido_producao ppp On (pi.idImpressao=ppp.idImpressao And pi.idProdPed=ppp.idProdPed and pi.numEtiqueta=ppp.numEtiqueta)
                            Where pi.idProdImpressao=" + pi.IdProdImpressao);

                        objPersistence.ExecuteCommand(sessao, @"
                            Update produto_impressao pi
                            Set pi.idImpressao=" + idImpressaoRep + @"
                            Where pi.idProdImpressao=" + pi.IdProdImpressao);
                    }

                if (temCarregamento && situacaoImpressao == ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Processando)
                {
                    // Se tiver carregamento e a situacao da impressão for processando, não marca os registros produtoPedidoProducao como cancelados
                    // apenas remove o vinculo da impressão.
                    if (cancelarPecaAPeca)
                        objPersistence.ExecuteCommand(sessao, @"Update produto_pedido_producao ppp
                            Inner Join produto_impressao pi On (ppp.idImpressao=pi.idImpressao And pi.idProdPed=ppp.idProdPed and 
                                ppp.numEtiqueta=pi.numEtiqueta)
                            Inner Join produtos_pedido_espelho ppe On (ppp.idProdPed=ppe.idProdPed)
                            Inner Join pedido ped On (ppe.idPedido=ped.idPedido)
                        Set ppp.canceladoAdmin=False, ppp.planoCorte=Null, 
                            ppp.idImpressao = null, ppp.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @"
                        Where pi.idProdImpressao In (" + idsProdImpressao + ") And !Coalesce(ppp.pecaReposta, False)");
                    else
                        objPersistence.ExecuteCommand(sessao, @"Update produto_pedido_producao ppp
                            Inner Join produtos_pedido_espelho ppe On (ppp.idProdPed=ppe.idProdPed)
                            Inner Join pedido ped On (ppe.idPedido=ped.idPedido)
                        Set ppp.canceladoAdmin=False, ppp.planoCorte=Null, 
                            ppp.idImpressao = null, ppp.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @"
                        Where ppp.idImpressao=" + idImpressao + " And !Coalesce(ppp.pecaReposta, False)");
                }
                else
                {
                    // Marca os registros produtoPedidoProducao como cancelados (Exceto as peças repostas)
                    if (cancelarPecaAPeca)
                        objPersistence.ExecuteCommand(sessao, @"Update produto_pedido_producao ppp
                            Inner Join produto_impressao pi On (ppp.idImpressao=pi.idImpressao And 
                                ppp.numEtiqueta=pi.numEtiqueta)
                            Inner Join produtos_pedido_espelho ppe On (ppp.idProdPed=ppe.idProdPed)
                            Inner Join pedido ped On (ppe.idPedido=ped.idPedido)
                        Set ppp.canceladoAdmin=False, ppp.planoCorte=Null, 
                            ppp.numEtiquetaCanc=ppp.numEtiqueta, ppp.numEtiqueta=Null,
                            ppp.situacao=If(ped.tipoPedido<>" + (int)Pedido.TipoPedidoEnum.MaoDeObra + ", " +
                                    (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaVenda + ", " +
                                    (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaMaoObra + @")
                        Where pi.idProdImpressao In (" + idsProdImpressao + ") And !Coalesce(ppp.pecaReposta, False)");
                    else
                        objPersistence.ExecuteCommand(sessao, @"Update produto_pedido_producao ppp
                            Inner Join produtos_pedido_espelho ppe On (ppp.idProdPed=ppe.idProdPed)
                            Inner Join pedido ped On (ppe.idPedido=ped.idPedido)
                        Set ppp.canceladoAdmin=False, ppp.planoCorte=Null, 
                            ppp.numEtiquetaCanc=ppp.numEtiqueta, ppp.numEtiqueta=Null,
                            ppp.situacao=If(ped.tipoPedido<>" + (int)Pedido.TipoPedidoEnum.MaoDeObra + ", " +
                                    (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaVenda + ", " +
                                    (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaMaoObra + @")
                        Where ppp.idImpressao=" + idImpressao + " And !Coalesce(ppp.pecaReposta, False)");
                }

                // Cancela os retalhos associados
                if (cancelarRetalhos && tipoImpressao == ProdutoImpressaoDAO.TipoEtiqueta.Retalho)
                {
                    foreach (uint idRetalhoProducao in idsProdImpressaoRetalho)
                        RetalhoProducaoDAO.Instance.Cancelar(sessao, idFunc, idRetalhoProducao, "Cancelamento da impressão " + idImpressao,
                            false, false, false);
                }

                // Cancela os retalhos em uso pelas etiquetas de pedido
                if (tipoImpressao == ProdutoImpressaoDAO.TipoEtiqueta.Pedido)
                {
                    foreach (var id in idsProdPedProducao)
                    {
                        var idRetalho = UsoRetalhoProducaoDAO.Instance.ObtemIdRetalhoProducao(sessao, id);
                        if (idRetalho.GetValueOrDefault(0) > 0)
                            UsoRetalhoProducaoDAO.Instance.RemoverAssociacao(sessao, idRetalho.Value, id);
                    }
                }

                // Cancela as etiquetas repostas que foram impressas nesta impressão e que não possuem outra impressão, o motivo deste comando
                // é cancelar as etiquetas repostas que possuem apenas um impressão para que seja impressa desde o início do processo
                // porque caso elas ficassem como repostas não teriam referência de uma impressão de etiquetas anterior não-cancelada
                // para que pudessem ser reimpressas.
                if (idImpressao > 0)
                {
                    foreach (string etiqueta in ProdutoImpressaoDAO.Instance.GetEtiquetasRepostasNaImpressao(sessao, idImpressao, idsProdImpressao,
                        ProdutoImpressaoDAO.TipoEtiqueta.Pedido))
                    {
                        string[] vetEtiqueta = etiqueta.Split('-', '.', '/');
                        if (objPersistence.ExecuteSqlQueryCount(sessao, "Select Count(*) From produto_impressao Where idPedido=" + vetEtiqueta[0] +
                            " And posicaoProd=" + vetEtiqueta[1] + " And itemEtiqueta=" + vetEtiqueta[2] + " And qtdeProd=" + vetEtiqueta[3] +
                            " And !Coalesce(cancelado, False) Group By idImpressao") == 1)
                        {
                            objPersistence.ExecuteCommand(sessao, @"Update produto_pedido_producao ppp
                                    Inner Join produtos_pedido_espelho ppe on (ppp.idProdPed=ppe.idProdPed)
                                    Inner Join pedido ped on (ppe.idPedido=ped.idPedido)
                                Set ppp.canceladoAdmin=False, ppp.planoCorte=Null, ppp.pecaReposta=Null, ppp.tipoPerdaRepos=Null,
                                    ppp.idFuncRepos=Null, ppp.dataRepos=Null, ppp.idSetorRepos=Null,
                                    ppp.numEtiquetaCanc=ppp.numEtiqueta, ppp.numEtiqueta=Null,
                                    ppp.situacao=If(ped.tipoPedido<>" + (int)Pedido.TipoPedidoEnum.MaoDeObra + ", " +
                                        (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaVenda + ", " +
                                        (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaMaoObra + @")
                                Where ppp.numEtiqueta='" + etiqueta + "'");

                            // Diminui a quantidade impressao desta peça na tabela produtos_pedido_espelho ou ambiente_pedido_espelho.
                            uint idProdPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetIdProdPedByEtiqueta(sessao, etiqueta, false);
                            if (!PedidoDAO.Instance.IsMaoDeObra(sessao, vetEtiqueta[0].StrParaUint()))
                                objPersistence.ExecuteCommand(sessao, "Update produtos_pedido_espelho Set qtdImpresso=IF(QtdImpresso=0, 0, -1) Where idProdPed=" +
                                    idProdPedEsp);
                            else
                                objPersistence.ExecuteCommand(sessao, "Update ambiente_pedido_espelho Set qtdeImpresso=IF(QtdeImpresso=0, 0, -1) Where idAmbientePedido=" +
                                    ProdutosPedidoEspelhoDAO.Instance.ObtemIdAmbientePedido(sessao, idProdPedEsp));
                        }
                    }
                }

                if ((idPedido ?? 0) == 0 && (numeroNFe ?? 0) == 0 && String.IsNullOrEmpty(planoCorte) && idProdImpressao == 0 && idImpressao > 0)
                {
                    // Marca os produtos da impressão como cancelados.
                    objPersistence.ExecuteCommand(sessao, "Update produto_impressao Set cancelado=True Where idImpressao=" + idImpressao);

                    LogCancelamentoDAO.Instance.LogImpressaoEtiquetas(sessao, idFunc, GetElementByPrimaryKey(sessao, idImpressao), motivo, true);

                    // Atualiza a situação dos pedidos.
                    AtualizaSituacaoPedidos(sessao, idImpressao, idPedido, planoCorte);

                    objPersistence.ExecuteCommand(sessao, "Update impressao_etiqueta Set situacao=" +
                        (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Cancelada + " Where idImpressao=" + idImpressao);
                }
                else
                {
                    // Marca os produtos da impressão como cancelados.
                    objPersistence.ExecuteCommand(sessao, "Update produto_impressao Set cancelado=True Where idProdImpressao In (" + idsProdImpressao + ")");

                    // Atualiza a situação dos pedidos.
                    if (idImpressao > 0 && (numeroNFe ?? 0) == 0 && idProdImpressao == 0)
                        AtualizaSituacaoPedidos(sessao, idImpressao, idPedido, planoCorte);

                    if (idProdImpressao > 0)
                    {
                        var idPedidoProdImpressao = ProdutoImpressaoDAO.Instance.ObtemValorCampo<int?>(sessao, "IdPedido", "IdProdImpressao=" + idProdImpressao);

                        if ((numeroNFe ?? 0) == 0 && idPedidoProdImpressao.GetValueOrDefault() > 0)
                            AtualizaSituacaoPedidos(sessao, 0, (uint)idPedidoProdImpressao.Value, planoCorte);

                        LogCancelamentoDAO.Instance.LogProdutoImpressao(sessao, ProdutoImpressaoDAO.Instance.GetElementByPrimaryKey(sessao, idProdImpressao),
                            motivo, true);
                    }
                    else if (idImpressao > 0)
                    {
                        string descr = (idPedido > 0 ? " das etiquetas do pedido " + idPedido : "") +
                            (numeroNFe > 0 ? " das etiquetas da NFe " + numeroNFe : "") +
                            (!String.IsNullOrEmpty(planoCorte) ? " das etiquetas com plano de corte '" + planoCorte + "'" : "");

                        LogCancelamentoDAO.Instance.LogImpressaoEtiquetas(sessao, idFunc, GetElementByPrimaryKey(sessao, idImpressao),
                            "Cancelamento" + descr + " - " + motivo, true);
                    }
                }

                if (idImpressao > 0)
                {
                    bool pecasCanceladas = ExecuteScalar<bool>(sessao, "Select IF(count(*) = Sum(cancelado),TRUE,FALSE) from produto_impressao where idImpressao = " + idImpressao);
                    if (pecasCanceladas)
                    {
                        objPersistence.ExecuteCommand(sessao, "Update impressao_etiqueta Set situacao=" +
                        (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Cancelada + " Where idImpressao=" + idImpressao);
                    }
                }
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("CancelarImpressao", ex);
                throw ex;
            }
        }

        /// <summary>
        /// Atualiza a situação dos pedidos cancelados.
        /// </summary>
        private void AtualizaSituacaoPedidos(uint idImpressao, uint? idPedido, string planoCorte)
        {
            AtualizaSituacaoPedidos(null, idImpressao, idPedido, planoCorte);
        }

        /// <summary>
        /// Atualiza a situação dos pedidos cancelados.
        /// </summary>
        private void AtualizaSituacaoPedidos(GDASession session, uint idImpressao, uint? idPedido, string planoCorte)
        {
            if (idPedido > 0)
            {
                PedidoEspelhoDAO.Instance.AtualizaSituacaoImpressao(session, idPedido.Value);

                if (!ProdutoImpressaoDAO.Instance.PedidoPossuiPecaImpressa(session, idPedido.Value))
                {
                    PedidoDAO.Instance.AlteraSituacaoProducao(session, idPedido.Value, Pedido.SituacaoProducaoEnum.NaoEntregue);

                    /* Chamado 34685.
                     * Caso todas peças do pedido de produção tenham sido canceladas a situação deve ser alterada para Confirmado PCP. */
                    if (PedidoDAO.Instance.GetTipoPedido(session, idPedido.Value) == Pedido.TipoPedidoEnum.Producao)
                        PedidoDAO.Instance.AlteraSituacao(session, idPedido.Value, Pedido.SituacaoPedido.ConfirmadoLiberacao);
                }
                else
                    PedidoDAO.Instance.AtualizaSituacaoProducao(session, idPedido.Value, null, DateTime.Now);
            }
            else
            {
                var idsPedidos = String.Join(",", ExecuteMultipleScalar<string>(session,
                    @"SELECT DISTINCT IdPedido FROM produto_impressao WHERE IdImpressao=" + idImpressao +
                    (!String.IsNullOrEmpty(planoCorte) ? " AND PlanoCorte=?planoCorte" : ""), new GDAParameter("?planoCorte", planoCorte)));

                foreach (var s in idsPedidos.Split(','))
                    if (!String.IsNullOrEmpty(s))
                    {
                        var idPedidoCanc = s.StrParaUint();

                        PedidoEspelhoDAO.Instance.AtualizaSituacaoImpressao(session, idPedidoCanc);

                        if (!ProdutoImpressaoDAO.Instance.PedidoPossuiPecaImpressa(session, idPedidoCanc))
                        {
                            PedidoDAO.Instance.AlteraSituacaoProducao(session, idPedidoCanc, Pedido.SituacaoProducaoEnum.NaoEntregue);

                            /* Chamado 34685.
                             * Caso todas peças do pedido de produção tenham sido canceladas a situação deve ser alterada para Confirmado PCP. */
                            if (PedidoDAO.Instance.GetTipoPedido(session, idPedidoCanc) == Pedido.TipoPedidoEnum.Producao)
                                PedidoDAO.Instance.AlteraSituacao(session, idPedidoCanc, Pedido.SituacaoPedido.ConfirmadoLiberacao);
                        }
                        else
                            PedidoDAO.Instance.AtualizaSituacaoProducao(session, idPedidoCanc, null, DateTime.Now);
                    }
            }
        }

        #endregion

        #region Apaga as etiquetas não usadas no arquivo de otimização

        private string BuscaRegistros(GDASession sessao, uint idProdPed, string campo, string resultadoFiltroVazio, bool executarSql2)
        {
            StringBuilder temp = new StringBuilder();

            // Apaga os registros da tabela produto_pedido_producao que não tem registro na tabela
            // leitura_producao (registros gerados por exportação de arquivo de otimização)
            // que tem o mesmo idProdPed do argumento
            string sql1 = @"
                select distinct " + campo + @"
                from produto_pedido_producao ppp
                    inner join produtos_pedido_espelho ppe on (ppp.idProdPed=ppe.idProdPed)
                    left join leitura_producao lp on (ppp.idProdPedProducao=lp.idProdPedProducao)
                where ppe.idProdPed=" + idProdPed + @"
                group by ppp.idProdPedProducao
                having count(lp.idLeituraProd)<=1 and group_concat(lp.dataLeitura) is null";

            // Apaga os registros da tabela produto_pedido_producao cujo idProdPed não existe mais na
            // tabela produtos_pedido_espelho (em caso de erro ao apagar um registro na tabela ou
            // registros gerados por arquivo de exportação)
            string sql2 = @"
                select distinct " + campo + @"
                from produto_pedido_producao ppp
                    left join produtos_pedido_espelho ppe on (ppp.idProdPed=ppe.idProdPed)
                where ppe.idProdPed is null";

            // Retira o alias do nome do campo            
            string nomeCampo = campo.Contains(".") ? campo.Split('.')[1] : campo;

            if (nomeCampo.IndexOf(",", StringComparison.Ordinal) > 0)
                nomeCampo = nomeCampo.Substring(0, nomeCampo.IndexOf(",", StringComparison.Ordinal));

            // Recupera os registros
            temp.Append(GetValoresCampo(sessao, sql1, nomeCampo) + ",");

            if (executarSql2)
                temp.Append(GetValoresCampo(sessao, sql2, nomeCampo));

            string retorno = temp.ToString().Trim(',').Replace(",,", ",");
            return retorno.Length > 0 ? retorno : resultadoFiltroVazio;
        }

        /// <summary>
        /// Apaga as etiquetas não usadas no arquivo de otimização.
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <param name="executarSql2"></param>
        /// <param name="exportandoEtiquetas"></param>
        public void ApagarEtiquetasOtimizacao(uint idProdPed, bool executarSql2, bool exportandoEtiquetas)
        {
            ApagarEtiquetasOtimizacao(null, idProdPed, executarSql2, exportandoEtiquetas);
        }

        /// <summary>
        /// Apaga as etiquetas não usadas no arquivo de otimização.
        /// </summary>
        public void ApagarEtiquetasOtimizacao(GDASession sessao, uint idProdPed, bool executarSql2, bool exportandoEtiquetas)
        {
            //Condição foi criada para não apagar as peças que foram colocadas em produção quando o pedido foi finalizado
            //O motivo é que quando ia exportar para o optway as peças era apagadas e recriadas somente as que foram exportadas.
            if (exportandoEtiquetas)
                return;

            var ids = BuscaRegistros(sessao, idProdPed, "ppp.idProdPedProducao", "0", executarSql2);
            if (ids == "0")
                return;

            PecasExcluidasSistemaDAO.Instance.InserePecas(sessao, ids);

            var sql = @"
                delete from produto_impressao
                where coalesce(cancelado, false) and numEtiqueta in (select * from (
                    select coalesce(numEtiqueta, numEtiquetaCanc) from produto_pedido_producao
                    where idProdPedProducao in (" + ids + @")
                ) as temp);

                delete from roteiro_producao_etiqueta
                where idProdPedProducao in (" + ids + @");
                
                delete from produto_pedido_producao
                where idProdPedProducao in (" + ids + @")";

            LeituraProducaoDAO.Instance.ApagarPelosIdsProdPedProducao(sessao, ids?.Split(',')?.Select(f => f.StrParaInt())?.ToList() ?? new List<int>());

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Recupera dados da impressão

        /// <summary>
        /// Recupera a situação da impressão.
        /// </summary>
        public int ObtemSituacao(uint idImpressao)
        {
            return ObtemValorCampo<int>("situacao", "idImpressao=" + idImpressao);
        }

        /// <summary>
        /// Busca impressões que estejam na situação "Processando"
        /// </summary>
        public string ObtemImpressoesProcessando()
        {
            return ObtemValorCampo<string>("cast(group_concat(idImpressao) as char)", "situacao=" + (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Processando);
        }

        /// <summary>
        /// Obtem a lista de ids dos setores lidos de uma impressão
        /// </summary>
        public string ObtemSetoresProd(uint idImpressao, string idsPedidos)
        {
            string sql;

            // Faz o join com as etiquetas somente se algum pedido tiver sido informado
            if (!string.IsNullOrEmpty(idsPedidos))
            {
                sql = @"
                    SELECT DISTINCT ppp.`IDSETOR`
                    FROM produto_impressao pi
                        INNER JOIN produto_pedido_producao ppp ON (pi.idImpressao=ppp.idImpressao and 
                            pi.numEtiqueta=ppp.numEtiqueta)
                    WHERE pi.idImpressao=" + idImpressao + @" 
                        AND !coalesce(pi.cancelado,FALSE)
                        AND pi.idPedido in(" + idsPedidos + ")";
            }
            else
            {
                sql = @"
                    SELECT DISTINCT ppp.`IDSETOR`
                    FROM produto_pedido_producao ppp 
                    WHERE ppp.idImpressao=" + idImpressao + @" 
                        AND ppp.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;
            }

            var ids = objPersistence.LoadResult(sql, null).Select(f => f.GetUInt32(0))
                       .ToList();

            return string.Join(",", Array.ConvertAll(ids.ToArray(), x => x.ToString()));
        }

        /// <summary>
        /// Recupera o funcionário que gerou a impressão.
        /// </summary>
        public int ObterIdFunc(GDASession session, int idImpressao)
        {
            return ObtemValorCampo<int>(session, "IdFunc", string.Format("IdImpressao={0}", idImpressao));
        }

        #endregion

        #region Atualiza situação da impressão

        /// <summary>
        /// Atualiza situação da impressão
        /// </summary>
        public void AtualizaSituacao(GDASession session, uint idImpressao, ImpressaoEtiqueta.SituacaoImpressaoEtiqueta situacao)
        {
            objPersistence.ExecuteCommand(session, "Update impressao_etiqueta Set situacao=" + (int)situacao + " Where idImpressao=" + idImpressao);
        }

        #endregion

        #region Verifica se existem produtos de impressão sem impressão associada

        /// <summary>
        /// Verifica se existem produtos de impressão sem impressão associada (ocorre quando dá erro na impressão)
        /// </summary>
        /// <returns></returns>
        public bool ExistemProdutosImpressaoSemImpressao()
        {
            return objPersistence.ExecuteSqlQueryCount(
                @"Select Count(*) From produto_impressao Where idImpressao <> 0 and 
                idImpressao not in (Select idImpressao From impressao_etiqueta)") > 0;
        }

        #endregion

        #region Busca os ids das impressões de um determinado pedido/NFe

        /// <summary>
        /// Busca os ids das impressões de um determinado pedido.
        /// </summary>
        public string GetIdsByPedido(GDASession session, uint idPedido)
        {
            var sql = @"select distinct pi.idImpressao
                from produto_impressao pi
                    left join impressao_etiqueta ie on (pi.idImpressao=ie.idImpressao)
                where ie.situacao=" + (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa + @"
                    and !coalesce(pi.cancelado,false) and pi.idPedido=" + idPedido + " order by pi.idImpressao";

            return GetValoresCampo(session, sql, "idImpressao");
        }
        
        /// <summary>
        /// Busca os ids das impressões de uma determinada nota fiscal.
        /// </summary>
        public string GetIdsByNFe(uint idNf)
        {
            var sql = @"select distinct pi.idImpressao
                from produto_impressao pi
                    left join impressao_etiqueta ie on (pi.idImpressao=ie.idImpressao)
                where ie.situacao=" + (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa + @"
                    and !coalesce(pi.cancelado,false) and pi.idNf=" + idNf + " order by pi.idImpressao";

            return GetValoresCampo(sql, "idImpressao");
        }

        #endregion

        #region Recupera as impressões ativas

        /// <summary>
        /// Recupera as impressões ativas.
        /// </summary>
        public string GetAtivas(string idsImpressoes)
        {
            var sql = "select idImpressao from impressao_etiqueta where idImpressao in (" + 
                idsImpressoes + ") and situacao=" + (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa;

            return GetValoresCampo(sql, "idImpressao");
        }

        #endregion

        #region Recupera o tipo da impressão

        private string SqlTipoImpressao(string idImpressao, string campo)
        {
            var sql = @"select count(*)>0 from produto_impressao 
                where {0} is not null and idImpressao=" + idImpressao;

            return string.Format(sql, campo);
        }

        /// <summary>
        /// Recupera o tipo da impressão.
        /// </summary>
        public ProdutoImpressaoDAO.TipoEtiqueta GetTipoImpressao(uint idImpressao)
        {
            return GetTipoImpressao(null, idImpressao);
        }

        /// <summary>
        /// Recupera o tipo da impressão.
        /// </summary>
        public ProdutoImpressaoDAO.TipoEtiqueta GetTipoImpressao(GDASession session, uint idImpressao)
        {
            if (ExecuteScalar<bool>(session, SqlTipoImpressao(idImpressao.ToString(), "idPedido")))
                return ProdutoImpressaoDAO.TipoEtiqueta.Pedido;

            if (ExecuteScalar<bool>(session, SqlTipoImpressao(idImpressao.ToString(), "idNf")))
                return ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal;

            if (ExecuteScalar<bool>(session, SqlTipoImpressao(idImpressao.ToString(), "idRetalhoProducao")))
                return ProdutoImpressaoDAO.TipoEtiqueta.Retalho;

            if (ExecuteScalar<bool>(session, SqlTipoImpressao(idImpressao.ToString(), "IdProdPedBox")))
                return ProdutoImpressaoDAO.TipoEtiqueta.Box;

            throw new Exception("Tipo da impressão não definido.");
        }

        #endregion
    }
}