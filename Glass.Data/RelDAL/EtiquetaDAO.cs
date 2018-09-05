using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using Glass.Data.Helper;
using Glass.Data.Model;
using GDA;
using System.Globalization;
using Glass.Configuracoes;
using System.Linq;
using Colosoft;

namespace Glass.Data.RelDAL
{
    public sealed class EtiquetaDAO : BaseDAO<Etiqueta, EtiquetaDAO>
    {
        //private EtiquetaDAO() { }

        #region Retorna o número da etiqueta

        /// <summary>
        /// Retorna o número da etiqueta.
        /// </summary>
        public string GetNumEtiqueta(uint idPedidoNf, int pos, int posInicial, int qtde, ProdutoImpressaoDAO.TipoEtiqueta tipoEtiqueta)
        {
            ProdutoImpressao pi = new ProdutoImpressao()
            {
                PosicaoProd = pos,
                ItemEtiqueta = posInicial,
                QtdeProd = qtde
            };

            switch (tipoEtiqueta)
            {
                case ProdutoImpressaoDAO.TipoEtiqueta.Pedido:
                    pi.IdPedido = idPedidoNf;
                    break;

                case ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal:
                    pi.IdNf = idPedidoNf;
                    break;
            }

            return ProdutoImpressaoDAO.Instance.GetNumeroEtiqueta(pi);
        }

        #endregion

        #region Impressão de etiqueta de pedido

        /// <summary>
        /// Busca lista de etiquetas a serem impressas, se não for re-impressao, salva impressão realizada, 
        /// altera situação do pedido_espelho para impresso e marca a quantidade dos itens que foram impressos
        /// </summary>
        public Etiqueta[] GetListPedidoComTransacao(uint idFunc, string idImpressao, uint idProdPed, uint idAmbientePedido, string idsProdPed, bool arqOtimizacao, bool reImpressao,
            string numEtiqueta, bool apenasPlano, string[] listaRetalhos, int? idSolucaoOtimizacao)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = GetListPedido(transaction, idFunc, idImpressao, idProdPed, idAmbientePedido, idsProdPed, arqOtimizacao, reImpressao,
                        numEtiqueta, apenasPlano, listaRetalhos, idSolucaoOtimizacao);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("Etiqueta {0}", numEtiqueta), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao gerar Etiquetas.", ex));
                }
            }
        }

        /// <summary>
        /// Busca lista de etiquetas a serem impressas, se não for re-impressao, salva impressão realizada, 
        /// altera situação do pedido_espelho para impresso e marca a quantidade dos itens que foram impressos
        /// </summary>
        public Etiqueta[] GetListPedido(GDASession session, uint idFunc, string idImpressao, uint idProdPed, uint idAmbientePedido, string idsProdPed, bool arqOtimizacao, bool reImpressao,
            string numEtiqueta, bool apenasPlano, string[] listaRetalhos, int? idSolucaoOtimizacao)
        {
            var lstEtiq = new List<Etiqueta>();
            Etiqueta etiqueta;

            // Variáveis utilizadas no caso de ocorrer algum erro na inserção da impressão no BD
            var idsProdPedAlterados = new List<uint>();
            var idsAmbPedAlterados = new List<uint>();
            var etiquetasRepostas = new List<string>();

            // Salva se determinada etiqueta é de reposição para não verificar no banco inúmeras vezes
            var dicEtiquetaReposta = new Dictionary<string, bool>();
            // Usado para salvar o log de peças impressas no pedido.
            var pedidosDescricaoPecasImpressasAntigas = new Dictionary<int, string>();

            // Controla a impressão
            var imprimir = false;

            var acao = "Início";

            #region Insere dados da impressão

            // Se estiver imprimindo pela primeira vez, salva dados da impressão
            if (!reImpressao)
            {
                #region Recupera as peças impressas do pedido antes da impressão ser inserida

                if (PedidoConfig.SalvarLogPecasImpressasNoPedido || EtiquetaConfig.RelatorioEtiqueta.ModeloEtiquetaPorLoja)
                {
                    if (!string.IsNullOrEmpty(idsProdPed))
                    {
                        var idsPedido = new List<int>();
                        var idsProdutoPedido = new List<int>();
                        var idsAmbientePedido = new List<int>();
                        var dadosProdutosPedido = idsProdPed.TrimEnd('|').Split('|');

                        foreach (var dadosProdutoPedido in dadosProdutosPedido)
                        {
                            var dados = dadosProdutoPedido.Split('\t');

                            if (dadosProdutoPedido[0] != 'A')
                                idsProdutoPedido.Add(dados[0].IndexOf('_') > 0 ? dados[0].Split('_')[0].StrParaInt() : dados[0].StrParaInt());
                            else
                                idsAmbientePedido.Add(dados[0].IndexOf('_') > 0 ? dados[0].Split('_')[0].Substring(1).StrParaInt() : dados[0].Substring(1).StrParaInt());
                        }

                        if (idsProdutoPedido.Count() > 0)
                            idsPedido.AddRange(
                                ProdutosPedidoEspelhoDAO.Instance.ExecuteMultipleScalar<int>(session,
                                    string.Format(
                                        @"SELECT DISTINCT IdPedido FROM produtos_pedido_espelho WHERE IdProdPed IN ({0})",
                                            string.Join(",", idsProdutoPedido))));

                        if (idsAmbientePedido.Count() > 0)
                            idsPedido.AddRange(
                                AmbientePedidoEspelhoDAO.Instance.ExecuteMultipleScalar<int>(session,
                                    string.Format(
                                        @"SELECT DISTINCT IdPedido FROM ambiente_pedido_espelho WHERE IdAmbientePedido IN ({0})",
                                            string.Join(",", idsAmbientePedido))));

                        #region Recupera a descrição para gerar o Log

                        if (PedidoConfig.SalvarLogPecasImpressasNoPedido)
                        {
                            if (idsPedido != null && idsPedido.Count > 0)
                                foreach (var id in idsPedido.Distinct())
                                    pedidosDescricaoPecasImpressasAntigas.Add(id,
                                        ProdutoImpressaoDAO.Instance.ObterEtiquetasImpressasPeloPedido(session, id));
                        }

                        #endregion

                        #region Verifica se todos pedidos são da mesma loja

                        if (EtiquetaConfig.RelatorioEtiqueta.ModeloEtiquetaPorLoja)
                        {
                            if (idsPedido != null && idsPedido.Count > 0)
                            {
                                var existemPedidosAssociadosALojasDiferentes =
                                    ExecuteScalar<bool>(session, string.Format("SELECT COUNT(*) > 1 FROM (SELECT Idloja FROM pedido p WHERE p.IdPedido IN ({0}) GROUP BY IdLoja) AS temp",
                                        string.Join(",", idsPedido.Distinct().ToList())));

                                /* Chamado 43775. */
                                if (existemPedidosAssociadosALojasDiferentes)
                                    throw new Exception("Não é possível imprimir etiquetas de pedidos associados a lojas diferentes, pois, há um modelo de etiqueta por loja.");
                            }
                        }

                        #endregion
                    }
                    else if (idProdPed > 0)
                    {
                        var idPedido = ProdutosPedidoDAO.Instance.ObtemIdPedido(session, idProdPed);
                        
                        #region Recupera a descrição para gerar o Log

                        if (PedidoConfig.SalvarLogPecasImpressasNoPedido)
                        {
                            pedidosDescricaoPecasImpressasAntigas.Add((int)idPedido,
                                ProdutoImpressaoDAO.Instance.ObterEtiquetasImpressasPeloPedido(session, (int)idPedido));
                        }

                        #endregion
                    }
                }

                #endregion

                // Lista com pedidos que deverão ser alterados para "Impresso"
                var lstPedImpresso = new List<uint>();

                // Lista com produtos, lista com planos de corte, lista com ambientes, lista com qtde que serão impressas e lista de obs
                var lstIdProdPed = new List<uint>();
                var lstIdAmbPed = new List<uint>();
                var lstQtdJaImpressa = new List<int>();
                var lstQtdImpresso = new List<int>();
                var lstQtdImpressoAmb = new List<int>();
                var lstObs = new List<string>();
                var lstObsAmb = new List<string>();
                var lstEtiqueta = new List<string>();

                var temProdutoImprimir = false;

                var lista = idsProdPed.TrimEnd('|').Split('|');

                // Para cada produto a ser impresso, pega os valores para serem inseridos no BD
                foreach (var item in lista)
                {
                    var campos = item.Split('\t');

                    if (item[0] != 'R' && campos[2].StrParaInt() == 0)
                        continue;

                    temProdutoImprimir = true;

                    uint idPedidoImp = 0;

                    if (item[0] != 'A')
                    {
                        var idProdPedStr = campos[0].IndexOf('_') > 0 ? campos[0].Split('_')[0] : campos[0];
                        var planoCorte = campos[0].IndexOf('_') > 0 ? campos[0].Split('_')[1] : string.Empty;

                        // Busca produtoPedidoEspelho
                        var idProdPedEsp = idProdPedStr.Replace("R", "").StrParaUint();

                        // Busca Pedido
                        idPedidoImp = ProdutosPedidoEspelhoDAO.Instance.ObtemIdPedido(session, idProdPedEsp);

                        // Caso a empresa crie o clone dos produtos então é obrigatório ter a referência do produto
                        // do pedido espelho no produto pedido.
                        if (PCPConfig.CriarClone)
                        {
                            var idProdPedRef = ProdutosPedidoDAO.Instance.ObtemValorCampo<uint?>(session, "idProdPed", "idProdPedEsp=" +
                                idProdPedEsp).GetValueOrDefault();

                            // Caso o idProdPed seja nulo a etiqueta não deve ser impressa, pois, todos os produtos de
                            // pedido espelho devem ser referenciados nos produtos de pedido. Feito para a personal, chamado 6673.
                            if (idProdPedRef == 0)
                                throw new Exception("Erro ao imprimir a(s) etiqueta(s) do pedido " + idPedidoImp +
                                    ", exclua a conferência do mesmo, gere novamente e imprima a(s) etiqueta(s).");
                        }

                        // Salva a quantidade de determinado item que já foi impresso
                        lstIdProdPed.Add(idProdPedEsp);
                        lstQtdJaImpressa.Add(campos[1].StrParaInt());
                        lstQtdImpresso.Add(campos[2].StrParaInt());

                        // Salva as etiquetas que estão repostas e sendo reimpressas
                        if (lstQtdImpresso[lstQtdImpresso.Count - 1] == 0)
                            etiquetasRepostas.AddRange(campos[4].Split('_'));

                        // Verifica se a impressão deve ser feita
                        imprimir = imprimir || lstQtdImpresso[lstQtdImpresso.Count - 1] > 0;

                        // Salva obs
                        lstObs.Add(campos[3]);

                        // Salva as etiquetas retornadas
                        lstEtiqueta.Add(campos[4]);
                    }
                    else
                    {
                        // Indica que a impressão deve ser feita
                        imprimir = true;

                        // Busca Ambiente do Pedido Espelho
                        var idAmbientePedidoEspStr = campos[0].IndexOf('_') > 0 ? campos[0].Split('_')[0] : campos[0];
                        var idAmbientePedidoEsp = idAmbientePedidoEspStr.Substring(1).StrParaUint();

                        // Busca Pedido
                        idPedidoImp = AmbientePedidoEspelhoDAO.Instance.ObtemIdPedido(session, idAmbientePedidoEsp);

                        // Salva a quantidade de determinado item que já foi impresso
                        lstIdAmbPed.Add(idAmbientePedidoEsp);
                        lstQtdJaImpressa.Add(campos[1].StrParaInt());
                        lstQtdImpressoAmb.Add(campos[2].StrParaInt());

                        // Salva obs
                        lstObsAmb.Add(campos[3]);

                        // Salva as etiquetas retornadas
                        lstEtiqueta.Add(campos[4]);
                    }

                    // Salva pedido para ser alterado para impresso
                    if (!lstPedImpresso.Contains(idPedidoImp))
                        lstPedImpresso.Add(idPedidoImp);
                }

                if (!temProdutoImprimir)
                    throw new Exception("Nenhum produto selecionado para impressão possui quantidade informada.");

                if (imprimir)
                {
                    // Salva na tabela Impressao_Etiqueta esta impressão e quais produtos e qtdes foram impressos
                    idImpressao = ImpressaoEtiquetaDAO.Instance.NovaImpressaoPedido(session, idFunc, lstIdProdPed.ToArray(), lstIdAmbPed.ToArray(),
                        lstQtdJaImpressa.ToArray(), lstQtdImpresso.ToArray(), lstQtdImpressoAmb.ToArray(), lstEtiqueta.ToArray(),
                        idImpressao, lstPedImpresso, lstIdProdPed, lstQtdImpresso, ref idsProdPedAlterados, lstObs, lstIdAmbPed,
                        lstQtdImpressoAmb, lstObsAmb, ref idsAmbPedAlterados, idSolucaoOtimizacao).ToString();
                }
                else
                {
                    // Atualiza a observação de peças repostas
                    for (var i = 0; i < lstIdProdPed.Count; i++)
                        if (lstIdProdPed[i] > 0 && !string.IsNullOrEmpty(lstObs[i]))
                            ProdutosPedidoEspelhoDAO.Instance.AtualizaObs(session, lstIdProdPed[i], lstObs[i]);
                }
            }

            #endregion

            acao = "Impressao Inserida";

            // Busca produtos que serão impressos
            var lstProd = new List<ProdutoImpressao>();

            if (reImpressao || imprimir)
                lstProd.AddRange(ProdutoImpressaoDAO.Instance.GetListImpressao(session, idImpressao));

            acao = "Buscar peças repostas";

            // Se for reimpressao e não tiver sido passado peças repostas por parâmetro, força inserção de peças repostas na impressão
            if (reImpressao && idImpressao.StrParaUint() > 0 && etiquetasRepostas.Count == 0)
            {
                foreach (var e in ProdutoImpressaoDAO.Instance.GetEtiquetasRepostas(session, idImpressao.StrParaUint(), ProdutoImpressaoDAO.TipoEtiqueta.Pedido))
                {
                    // Garante que não inclua uma etiqueta que já está na lista, duplicando-a
                    if (lstProd.Find(x => x.NumEtiqueta == e) != null)
                        continue;

                    var pi = ProdutoImpressaoDAO.Instance.GetElementByEtiqueta(session, e, ProdutoImpressaoDAO.TipoEtiqueta.Pedido);

                    /* Chamado 14627.
                        * A impressão possuía etiquetas canceladas e ao recuperar estes produtos ocorria um erro ao setar o IdImpressao
                        * do mesmo, pois, como estava cancelada o produto de impressão não era recuperado. Neste caso o produto de impressão
                        * não deve ser adicionado à lista.
                    */
                    if (pi == null)
                        continue;

                    pi.IdImpressao = idImpressao.StrParaUint();

                    lstProd.Add(pi);
                }
            }

            acao = "Buscar peças repostas 2";

            var etiqRepErro = string.Empty;

            // Insere as etiquetas repostas inseridas nesta impressão
            foreach (var etiq in etiquetasRepostas)
            {
                if (string.IsNullOrEmpty(etiq))
                    throw new Exception("Falha ao recuperar etiqueta. Erro: tentativa de recuperar impressão de etiqueta nula.");

                var prodImpr = ProdutoImpressaoDAO.Instance.GetElementByEtiqueta(session, etiq, ProdutoImpressaoDAO.TipoEtiqueta.Pedido);

                if (prodImpr == null)
                    etiqRepErro += etiq + ",";

                if (prodImpr != null)
                {
                    if (imprimir)
                    {
                        // Marca o produto com o idImpressao novo, o motivo disso é fazer com que o plano de corte seja carregado
                        // corretamente na primeira impressão
                        prodImpr.IdImpressao = idImpressao.StrParaUint();

                        int[] dadosEtiqueta = Array.ConvertAll(etiq.Split('-', '.', '/'), x => x.StrParaInt());
                        prodImpr.IdPedido = (uint)dadosEtiqueta[0];
                        prodImpr.PosicaoProd = dadosEtiqueta[1];
                        prodImpr.ItemEtiqueta = dadosEtiqueta[2];
                        prodImpr.QtdeProd = dadosEtiqueta[3];
                        prodImpr.NumEtiqueta = ProdutoImpressaoDAO.Instance.GetNumeroEtiqueta(prodImpr);
                    }

                    lstProd.Add(prodImpr);
                }
            }

            if (!string.IsNullOrEmpty(etiqRepErro))
                throw new Exception("Falha ao recuperar etiqueta, a etiqueta foi reposta e em seguida cancelada, " +
                    "será necessário desfazer a reposição para imprimí-la. Etiquetas: " + etiqRepErro.TrimEnd(','));

            // Salva uma lista com os planos de corte
            var planosCorte = new List<KeyValuePair<string, string>>();

            // Salva uma lista com os pedidos usados para montar a etiqueta
            var dicPedidos = new Dictionary<uint, Pedido>();

            // Salva uma lista com os produtos usados para montar a etiqueta
            var dicProd = new Dictionary<uint, Produto>();

            // Recupera apenas as etiquetas do produto desejado
            if (idProdPed > 0)
                lstProd = lstProd.FindAll(x => x.IdProdPed == idProdPed);

            if (idAmbientePedido > 0)
                lstProd = lstProd.FindAll(x => x.IdAmbientePedido == idAmbientePedido);

            // Seleciona apenas a etiqueta desejada
            if (!string.IsNullOrEmpty(numEtiqueta))
                lstProd = lstProd.FindAll(x => x.NumEtiqueta == numEtiqueta);

            acao = "Buscar peças a serem impressas";

            // Para cada produto a ser impresso
            foreach (ProdutoImpressao prodImp in lstProd)
            {
                var descrBenef = "";

                if (prodImp.IdPedido.HasValue)
                {
                    // Busca produtoPedidoEspelho
                    if (prodImp.IdAmbientePedido == null)
                    {
                        // Busca a descrição do beneficiamento
                        descrBenef = ProdutoPedidoEspelhoBenefDAO.Instance.GetDescrBenef(session, null, prodImp.IdProdPed.Value, true);
                    }
                    else
                    {
                        // Busca a mão de obra feita no pedido
                        var itens = ProdutosPedidoEspelhoDAO.Instance.GetByAmbienteFast(session, 0, prodImp.IdAmbientePedido.Value);

                        for (var i = 0; i < itens.Length; i++)
                        {
                            var qtde = GrupoProdDAO.Instance.TipoCalculo(session, (int)itens[i].IdProd) == (int)TipoCalculoGrupoProd.Qtd ?
                                itens[i].Qtde + " " : "";

                            itens[i].BenefEtiqueta = true;
                            descrBenef += qtde + itens[i].DescricaoProdutoComBenef + "; ";
                        }
                    }
                }

                acao = "Monta etiqueta " + prodImp.NumEtiqueta;

                // Imprime a etiqueta diretamente, uma vez que as impressões estão corretas
                // com todas as etiquetas relacionadas à impressão correta, e não há mais
                // necessidade de controle de impressão de etiquetas
                var etiq = MontaEtiqueta(session, idFunc, prodImp, descrBenef, 
                    prodImp.IdPedido.HasValue ? PedidoEspelhoDAO.Instance.ObtemDataFabrica(session, prodImp.IdPedido.Value) : null,
                    IsPecaReposta(session, prodImp.NumEtiqueta, ref dicEtiquetaReposta), ref planosCorte, ref dicPedidos, ref dicProd, false);

                if (lstEtiq.All(f => f.BarCodeData != etiq.BarCodeData))
                    lstEtiq.Add(etiq);
            }

            #region Retalhos

            //List<uint> idsRetalho = new List<uint>();

            if (!PCPConfig.Etiqueta.UsarControleRetalhos || listaRetalhos == null)
                listaRetalhos = new string[0];

            #endregion

            // A condição "&& imprimir" foi criada para que caso fosse impressa apenas uma peça de reposição não entrasse nesta condição abaixo,
            // tendo em vista que nesta situação não é criada uma nova impressão
            if (!reImpressao && !imprimir)
            {
                // Marca leitura das peças repostas
                foreach (var etiq in lstEtiq)
                    if (IsPecaReposta(session, etiq.BarCodeData, ref dicEtiquetaReposta))
                    {
                        ProdutoPedidoProducaoDAO.Instance.InserePeca(session, null, etiq.BarCodeData, etiq.PlanoCorte,
                            idFunc, false, true, etiq.IdProdPedEsp, null, null);

                        var idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducaoPorSituacao(session, etiq.BarCodeData,
                            (int)ProdutoPedidoProducao.SituacaoEnum.Producao);

                        etiq.Obs += PreencheRetalho(session, etiq, ref listaRetalhos, idProdPedProducao, idFunc);
                    }
            }
            else if (!reImpressao)
            {
                var idImpr = idImpressao.StrParaUint();
                var imprPedido = ImpressaoEtiquetaDAO.Instance.GetTipoImpressao(session, idImpr) == ProdutoImpressaoDAO.TipoEtiqueta.Pedido;

                if (lstEtiq.Count == 0 && imprPedido)
                    throw new Exception("Ocorreu uma falha ao recuperar as etiquetas a serem impressas.");

                // Põe peças em produção
                foreach (var etiq in lstEtiq)
                {
                    // Ignora retalhos
                    if (etiq.NumEtiqueta?.StartsWith("R") ?? false)
                        continue;

                    acao = "Põe etiqueta " + etiq.BarCode + " em produção";

                    ProdutoPedidoProducaoDAO.Instance.InserePeca(session, idImpr, etiq.BarCodeData, etiq.PlanoCorte,
                        idFunc, false, IsPecaReposta(session, etiq.BarCodeData, ref dicEtiquetaReposta), etiq.IdProdPedEsp, null, null);

                    var idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducaoPorSituacao(session, etiq.BarCodeData,
                        (int)ProdutoPedidoProducao.SituacaoEnum.Producao);

                    if (idImpr > 0 && imprPedido &&
                        idProdPedProducao.GetValueOrDefault() == 0)
                        throw new Exception("As etiquetas não foram geradas na produção.");

                    etiq.Obs += PreencheRetalho(session, etiq, ref listaRetalhos, idProdPedProducao, idFunc);

                    // Altera situação de produção do pedido (Se for reposição)
                    var idPedidoAnterior = PedidoDAO.Instance.ObtemValorCampo<uint>(session, "idPedidoAnterior", "idPedido=" + etiq.IdPedido.Replace("R", ""));
                    if (idPedidoAnterior > 0)
                        PedidoDAO.Instance.AtualizaSituacaoProducao(session, idPedidoAnterior, null, DateTime.Now);
                }

                acao = "Atualiza situação da impressão e associa peça reposta à impressão";

                // Altera a situação da impressão de processando para ativa
                ImpressaoEtiquetaDAO.Instance.AtualizaSituacao(session, idImpressao.StrParaUint(), ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa);

                // Mesmo se for peça de reposição, marca a mesma nesta impressão corrente, para a impressão ficar correta
                if (imprimir)
                {
                    foreach (var etiq in lstEtiq)
                        if (IsPecaReposta(session, etiq.BarCodeData, ref dicEtiquetaReposta))
                        {
                            ProdutoImpressaoDAO.Instance.MarcaImpressao(session, etiq.BarCodeData, idImpressao.StrParaUint(),
                                ProdutoImpressaoDAO.TipoEtiqueta.Pedido, true);
                        }
                }
            }

            // Cria as etiquetas com os planos de corte
            if ((!reImpressao || idProdPed == 0) && planosCorte != null)
            {
                if (apenasPlano)
                    lstEtiq.Clear();

                acao = "Monta planos de corte";

                foreach (var pc in planosCorte)
                {
                    etiqueta = new Etiqueta
                    {
                        PlanoCorte = pc.Key,
                        DescrMaterial =
                            ProdutoImpressaoDAO.Instance.GetMaterialPlanoCorte(session, pc.Key,
                                pc.Value != "0" && !string.IsNullOrEmpty(pc.Value) ? pc.Value : idImpressao),
                        QtdEtiquetaPlanoCorte =
                            ProdutoImpressaoDAO.Instance.GetQtdEtiquetaPlanoCorte(session, pc.Key,
                                pc.Value != "0" && !string.IsNullOrEmpty(pc.Value) ? pc.Value : idImpressao),
                        BarCodeData = "C" + pc.Key,
                        NumSeq =
                            ProdutoImpressaoDAO.Instance.ObtemNumSeqPlanoCorte(session,
                                idImpressao.StrParaUint(), pc.Key),
                        DataFabrica =
                            ProdutoImpressaoDAO.Instance.ObtemDataFabrica(session, pc.Key,
                                pc.Value != "0" && !string.IsNullOrEmpty(pc.Value) ? pc.Value : idImpressao)
                    };

                    if (etiqueta.QtdEtiquetaPlanoCorte > 0 && lstEtiq.All(f => f.BarCodeData != etiqueta.BarCodeData))
                        lstEtiq.Add(etiqueta);
                }
            }

            acao = "Formata código de barras da etiqueta para o CNC";

            // Se for o caso, concatena a espessura, altura e largura ao código de barras da etiqueta
            foreach (Etiqueta etiq in lstEtiq)
                etiq.BarCodeData = ConcatenaEspAltLargNumEtiqueta(etiq);

            acao = "Atualiza idProdPed na tabela de produção";

            // Em alguns casos não identificados (possivelmente por gerar registro na tabela produto_pedido_producao antes de efetivamente 
            // imprimir as etiquetas) o idProdPed da tabela produto_pedido_producao estava ficando diferente da tabela produto_impressao
            // (fato verificado fazendo join ppp.numEtiqueta com dados da tabela produto_impressao), por isso, o comando abaixo foi criado para 
            // garantir que o idprodped da tabela produto_pedido_producao fique igual ao da tabela produto_impressao
            if (!reImpressao && idImpressao.StrParaUint() > 0)
            {
                objPersistence.ExecuteCommand(session, @"
                update produto_pedido_producao ppp 
                    inner join produto_impressao pi on (ppp.idImpressao=pi.idImpressao and
                        ppp.numEtiqueta=pi.numEtiqueta and !coalesce(pi.cancelado, false))
                    left join produtos_pedido_espelho ppe On (ppp.idProdPed=ppe.idProdPed)
                    left join ambiente_pedido_espelho ape On (pi.idAmbientePedido=ape.idAmbientePedido)
                set ppp.idprodped=Coalesce(ppe.idProdPed, (Select idProdPed From produtos_pedido_espelho Where idambientepedido=ape.idAmbientePedido limit 1), ppp.idprodped)
                where pi.idImpressao=" + idImpressao + " And pi.idprodped<>ppp.idprodped And ppp.numEtiqueta is not null");
            }
            
            #region Recupera as peças impressas do pedido após o cancelamento e insere o log
 
            if (!reImpressao && PedidoConfig.SalvarLogPecasImpressasNoPedido)
                foreach (var pedidoDescricaoPecasImpresasAntigas in pedidosDescricaoPecasImpressasAntigas)
                {
                    var idPedidoLog = pedidoDescricaoPecasImpresasAntigas.Key;
                    var descricaoAntiga = pedidoDescricaoPecasImpresasAntigas.Value;
                    var descricaoNova =
                        ProdutoImpressaoDAO.Instance.ObterEtiquetasImpressasPeloPedido(session, idPedidoLog);
                    
                    LogAlteracaoDAO.Instance.LogPedidoPecaImpressa(session, idFunc, idPedidoLog, descricaoAntiga, descricaoNova);
                }

            #endregion

            return lstEtiq.ToArray();
        }
        
        /// <summary>
        /// Associa retalho à peça
        /// </summary>
        /// <returns>Observação da etiqueta</returns>
        private string PreencheRetalho(GDASession session, Etiqueta etiq, ref string[] listaRetalhos, uint? idProdPedProducao, uint idFunc)
        {
            if (listaRetalhos.Length > 0 && listaRetalhos[0] != "")
            {
                string buscar = Array.Find(listaRetalhos, x =>
                {
                    string[] dados = x.Split('|');
                    if (dados[0].Replace("R", "") != etiq.IdProdPedEsp.ToString() || dados[1].Length == 0)
                        return false;

                    foreach (string id in dados[1].Split(','))
                        //if (!String.IsNullOrEmpty(id) && !idsRetalho.Contains(Glass.Conversoes.StrParaUint(id)))
                        if (!String.IsNullOrEmpty(id))
                            return true;

                    return false;
                });

                if (buscar != null)
                {
                    string[] dadosRetalho = buscar.Split('|');

                    var validacaoRetalho = "";

                    var retalhos = dadosRetalho[1].Split(',');

                    for (int i = 0; i < retalhos.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(retalhos[i]))
                        {
                            var ret = RetalhoProducaoDAO.Instance.Obter(session, retalhos[i].StrParaUint());

                            if (!UsoRetalhoProducaoDAO.Instance.PossuiAssociacao(session, (uint)ret.IdRetalhoProducao, idProdPedProducao.GetValueOrDefault()))
                            {
                                validacaoRetalho = ValidaRetalho(session, etiq.IdProdPedEsp, ret);

                                if (string.IsNullOrEmpty(validacaoRetalho))
                                    RetalhoProducaoDAO.Instance.AssociarProducao(session, (uint)ret.IdRetalhoProducao, idProdPedProducao.GetValueOrDefault(), idFunc);
                                else if (i < retalhos.Length)
                                    continue;
                                else throw new Exception(validacaoRetalho);
                            }

                            /* Chamado 16190.
                             * Foi solicitado que a altura e largura do retalho fossem exibidas na etiqueta.
                            etiq.Obs += " Retalho: " + ret.NumeroEtiqueta;*/
                            var altura = ProdutoDAO.Instance.ObtemValorCampo<int>("Altura", "IdProd=" + ret.IdProd);
                            var largura = ProdutoDAO.Instance.ObtemValorCampo<int>("Largura", "IdProd=" + ret.IdProd);

                            listaRetalhos[i] = listaRetalhos[i].Replace(retalhos[i], "").Replace("|,", "|");

                            return " Retalho: " + ret.NumeroEtiqueta + " (" + altura + " x " + largura + ")";
                        }
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Altera o código da etiqueta, inserido espessura, largura e altura ao mesmo
        /// </summary>
        /// <param name="etiq"></param>
        internal string ConcatenaEspAltLargNumEtiqueta(Etiqueta etiq)
        {
            if (etiq.BarCodeData == null)
                etiq.BarCodeData = etiq.NumEtiqueta;

            var possuiFml = false;
            var possuiDxf = false;
            var possuiSGlass = false;
            var possuiIntermac = false;

            if (PCPConfig.EmpresaGeraArquivoFml)
                possuiFml = ProdutosPedidoEspelhoDAO.Instance.PossuiFml(null, etiq.IdProdPedEsp, etiq.NumEtiqueta, true, true);
            if (PCPConfig.EmpresaGeraArquivoDxf)
                possuiDxf = ProdutosPedidoEspelhoDAO.Instance.PossuiDxf(null, etiq.IdProdPedEsp, etiq.NumEtiqueta);
            if (PCPConfig.EmpresaGeraArquivoSGlass)
                possuiSGlass = ProdutosPedidoEspelhoDAO.Instance.PossuiSGlass(null, etiq.IdProdPedEsp, etiq.NumEtiqueta);
            if (PCPConfig.EmpresaGeraArquivoIntermac)
                possuiIntermac = ProdutosPedidoEspelhoDAO.Instance.PossuiIntermac(null, (int)etiq.IdProdPedEsp, etiq.NumEtiqueta);

            if (PCPConfig.ConcatenarEspAltLargAoNumEtiqueta &&
                //Chamado 76946 (Deve ser inserido no codigo de barras da etiqueta a altura/largura/espessura da peça apenas se possuir FML/DXF/SGLASS/Intermac)
                (possuiFml || possuiDxf || possuiSGlass || possuiIntermac) && 
                etiq.BarCodeData != null &&
                etiq.BarCodeData[0].ToString().ToUpper() != "C" && 
                etiq.BarCodeData[0].ToString().ToUpper() != "R" &&
                etiq.BarCodeData[0].ToString().ToUpper() != "N")
            {
                etiq.BarCodeData = (etiq.Espessura.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') +
                    (Glass.Conversoes.StrParaInt(etiq.Altura) > Glass.Conversoes.StrParaInt(etiq.Largura) ?
                        etiq.Altura.PadLeft(4, '0') + etiq.Largura.PadLeft(4, '0') :
                        etiq.Largura.PadLeft(4, '0') + etiq.Altura.PadLeft(4, '0')) +
                    "_" + etiq.BarCodeData.Trim()).Trim();
            }

            return etiq.BarCodeData;
        }

        #endregion

        #region Impressão de etiqueta de nota fiscal

        /// <summary>
        /// Busca lista de etiquetas a serem impressas, se não for re-impressao, salva impressão realizada, 
        /// altera situação do pedido_espelho para impresso e marca a quantidade dos itens que foram impressos
        /// </summary>
        /// <param name="idImpressao"></param>
        /// <param name="idProdNf"></param>
        /// <param name="idsProdPed">idProdPed/Qtd a Imprimir/Obs/Etiquetas</param>
        /// <param name="arqOtimizacao">Identifica se irá gerar as etiquetas para o arquivo de otimização</param>
        /// <param name="reImpressao"></param>
        /// <returns></returns>
        public Etiqueta[] GetListNFe(uint idFunc, string idImpressao, uint idProdNf, string idsProdNf, bool reImpressao,
            string numEtiqueta)
        {
            List<Etiqueta> lstEtiq = new List<Etiqueta>();
            //Etiqueta etiqueta;

            // Variáveis utilizadas no caso de ocorrer algum erro na inserção da impressão no BD
            List<uint> idsProdNfAlterados = new List<uint>();

            // Salva uma lista com os pedidos usados para montar a etiqueta
            var dicPedidos = new Dictionary<uint, Pedido>();

            // Salva uma lista com os produtos usados para montar a etiqueta
            var dicProd = new Dictionary<uint, Produto>();

            // Controla a impressão
            bool imprimir = false;
            
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    #region Insere dados da impressão

                    // Se estiver imprimindo pela primeira vez, salva dados da impressão
                    if (!reImpressao)
                    {
                        // Lista com pedidos que deverão ser alterados para "Impresso"
                        List<uint> lstNfImpressa = new List<uint>();

                        // Lista com produtos, lista com planos de corte, lista com ambientes, lista com qtde que serão impressas e lista de obs
                        List<uint> lstIdProdNf = new List<uint>();
                        List<int> lstQtdJaImpressa = new List<int>();
                        List<int> lstQtdImpresso = new List<int>();
                        List<string> lstObs = new List<string>();
                        List<string> lstEtiqueta = new List<string>();

                        string[] lista = idsProdNf.TrimEnd('|').Split('|');

                        // Para cada produto a ser impresso, pega os valores para serem inseridos no BD
                        foreach (string item in lista)
                        {
                            string[] campos = item.Split('\t');

                            string idProdNfStr = campos[0].IndexOf('_') > 0 ? campos[0].Split('_')[0] : campos[0];

                            // Busca produtoNf
                            uint idProdNfBuscar = Glass.Conversoes.StrParaUint(idProdNfStr.Substring(1));

                            // Busca NF
                            uint idNfImp = ProdutosNfDAO.Instance.ObtemIdNf(transaction, idProdNfBuscar);

                            // Salva a quantidade de determinado item que já foi impresso
                            lstIdProdNf.Add(idProdNfBuscar);
                            lstQtdJaImpressa.Add(Glass.Conversoes.StrParaInt(campos[1]));
                            lstQtdImpresso.Add(Glass.Conversoes.StrParaInt(campos[2]));

                            // Verifica se a impressão deve ser feita
                            imprimir = imprimir || lstQtdImpresso[lstQtdImpresso.Count - 1] > 0;

                            // Salva obs
                            lstObs.Add(campos[3]);

                            // Salva as etiquetas retornadas
                            lstEtiqueta.Add(campos[4]);

                            // Salva pedido para ser alterado para impresso
                            if (!lstNfImpressa.Contains(idNfImp))
                                lstNfImpressa.Add(idNfImp);
                        }

                        if (imprimir)
                        {
                            // Salva na tabela Impressao_Etiqueta esta impressão e quais produtos e qtdes foram impressos
                            idImpressao = ImpressaoEtiquetaDAO.Instance.NovaImpressaoNFe(transaction, idFunc, lstIdProdNf.ToArray(),
                                lstQtdJaImpressa.ToArray(), lstQtdImpresso.ToArray(), idImpressao, lstNfImpressa,
                                lstIdProdNf, lstQtdImpresso, lstObs, ref idsProdNfAlterados).ToString();
                        }
                        else
                        {
                            // Atualiza a observação de peças repostas
                            for (int i = 0; i < lstIdProdNf.Count; i++)
                                if (lstIdProdNf[i] > 0 && !String.IsNullOrEmpty(lstObs[i]))
                                    ProdutosNfDAO.Instance.AtualizaObs(transaction, lstIdProdNf[i], lstObs[i]);
                        }
                    }

                    #endregion

                    // Busca produtos que serão impressos
                    List<ProdutoImpressao> lstProd = new List<ProdutoImpressao>();

                    if (reImpressao || imprimir)
                        lstProd.AddRange(ProdutoImpressaoDAO.Instance.GetListImpressao(transaction, idImpressao));

                    // Salva uma lista com os planos de corte
                    List<KeyValuePair<string, string>> planosCorte = new List<KeyValuePair<string, string>>();

                    // Recupera apenas as etiquetas do produto desejado
                    if (idProdNf > 0)
                        lstProd = lstProd.FindAll(x => x.IdProdNf == idProdNf);

                    // Seleciona apenas a etiqueta desejada
                    if (!String.IsNullOrEmpty(numEtiqueta))
                        lstProd = lstProd.FindAll(x => x.NumEtiqueta == numEtiqueta);

                    // Para cada produto a ser impresso
                    foreach (Glass.Data.Model.ProdutoImpressao prodImp in lstProd)
                    {
                        // Imprime a etiqueta diretamente, uma vez que as impressões estão corretas
                        // com todas as etiquetas relacionadas à impressão correta, e não há mais
                        // necessidade de controle de impressão de etiquetas
                        lstEtiq.Add(MontaEtiqueta(transaction, idFunc, prodImp, "", null, false, ref planosCorte, ref dicPedidos, ref dicProd, false));
                    }

                    if (!reImpressao)
                    {
                        // Altera a situação da impressão de processando para ativa
                        ImpressaoEtiquetaDAO.Instance.AtualizaSituacao(transaction, Glass.Conversoes.StrParaUint(idImpressao), ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa);
                    }

                    transaction.Commit();
                    transaction.Close();

                    return lstEtiq.ToArray();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao gerar Etiquetas.", ex));
                }
            }
        }

        #endregion

        #region Impressão de etiqueta de retalho

        public Etiqueta[] GetListRetalho(uint idFunc, string idImpressao, List<RetalhoProducao> listaRetalhoProducao, uint idRetalhoProducao, bool reImpressao)
        {
            List<Etiqueta> lstEtiq = new List<Etiqueta>();

            // Variáveis utilizadas no caso de ocorrer algum erro na inserção da impressão no BD
            List<uint> idsRetalhoAlterados = new List<uint>();

            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    #region Insere dados da impressão

                    // Se estiver imprimindo pela primeira vez, salva dados da impressão
                    if (!reImpressao)
                    {
                        // Salva na tabela Impressao_Etiqueta esta impressão e quais produtos e qtdes foram impressos
                        idImpressao = ImpressaoEtiquetaDAO.Instance.NovaImpressaoRetalho(sessao, idFunc, listaRetalhoProducao).ToString();
                    }

                    #endregion

                    // Busca produtos que serão impressos
                    List<ProdutoImpressao> lstProd = new List<ProdutoImpressao>();

                    lstProd.AddRange(ProdutoImpressaoDAO.Instance.GetListImpressao(sessao, idImpressao));

                    // Salva uma lista com os planos de corte
                    List<KeyValuePair<string, string>> planosCorte = new List<KeyValuePair<string, string>>();

                    // Salva uma lista com os pedidos usados para montar a etiqueta
                    var dicPedidos = new Dictionary<uint, Pedido>();

                    // Salva uma lista com os produtos usados para montar a etiqueta
                    var dicProd = new Dictionary<uint, Produto>();

                    // Recupera apenas as etiquetas do produto desejado
                    if (idRetalhoProducao > 0)
                        lstProd = lstProd.FindAll(x => x.IdRetalhoProducao == idRetalhoProducao);

                    // Para cada produto a ser impresso
                    foreach (Glass.Data.Model.ProdutoImpressao prodImp in lstProd)
                    {
                        // Imprime a etiqueta diretamente, uma vez que as impressões estão corretas
                        // com todas as etiquetas relacionadas à impressão correta, e não há mais
                        // necessidade de controle de impressão de etiquetas
                        lstEtiq.Add(MontaEtiqueta(sessao, idFunc, prodImp, "", null, false, ref planosCorte, ref dicPedidos, ref dicProd, false));
                    }

                    if (!reImpressao)
                    {
                        // Altera a situação da impressão de processando para ativa
                        ImpressaoEtiquetaDAO.Instance.AtualizaSituacao(sessao, Glass.Conversoes.StrParaUint(idImpressao), ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa);
                    }
                    
                    sessao.Commit();
                    sessao.Close();

                    return lstEtiq.ToArray();
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    sessao.Close();

                    ErroDAO.Instance.InserirFromException("RelEtiquetas.aspx", ex);

                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao gerar Etiquetas.", ex));
                }
            }
        }

        #endregion

        #region Verifica se peça é de reposição

        /// <summary>
        /// Verifica se peça é de reposição
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <param name="dicEtiquetaReposta"></param>
        /// <returns></returns>
        internal bool IsPecaReposta(GDASession session, string numEtiqueta, ref Dictionary<string, bool> dicEtiquetaReposta)
        {
            if (String.IsNullOrEmpty(numEtiqueta))
                return false;

            if (dicEtiquetaReposta.ContainsKey(numEtiqueta))
                return dicEtiquetaReposta[numEtiqueta];
            else
            {
                bool isPecaReposta = ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(session, numEtiqueta, false);
                dicEtiquetaReposta.Add(numEtiqueta, isPecaReposta);

                return isPecaReposta;
            }
        }

        #endregion

        #region Monta Etiqueta

        internal Etiqueta MontaEtiqueta(GDASession session, uint idFunc, ProdutoImpressao prodImp, string descrBenef, DateTime? dataFabrica,
            ref List<KeyValuePair<string, string>> planosCorte, ref Dictionary<uint, Pedido> dicPedidos, ref Dictionary<uint, Produto> dicProd, bool isArquivoOtimizacao)
        {
            return MontaEtiqueta(session, idFunc, prodImp, descrBenef, dataFabrica,
                ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(prodImp.NumEtiqueta, false), ref planosCorte, ref dicPedidos, ref dicProd, isArquivoOtimizacao);
        }

        internal Etiqueta MontaEtiqueta(GDASession session, uint idFunc, ProdutoImpressao prodImp, string descrBenef, DateTime? dataFabrica,
            bool isPecaReposta, ref List<KeyValuePair<string, string>> planosCorte, ref Dictionary<uint, Pedido> dicPedidos, ref Dictionary<uint, Produto> dicProd, bool isArquivoOtimizacao)
        {
            // Salva os pedidos no dicionário, para não precisar buscar sempre os mesmos dados
            Pedido pedido = null;

            if (prodImp.IdPedido > 0)
            {
                if (!dicPedidos.ContainsKey(prodImp.IdPedido.Value))
                    dicPedidos.Add(prodImp.IdPedido.Value, PedidoDAO.Instance.GetElementByPrimaryKey(session, prodImp.IdPedido.Value));

                pedido = dicPedidos[prodImp.IdPedido.Value];
            }
            
            uint idProd = prodImp.IdProdNf > 0 ? ProdutosNfDAO.Instance.ObtemValorCampo<uint>(session, "idProd", "idProdNf=" + prodImp.IdProdNf) :
                prodImp.IdRetalhoProducao > 0 ? RetalhoProducaoDAO.Instance.ObtemValorCampo<uint>(session, "IdProd", "IdRetalhoProducao=" + prodImp.IdRetalhoProducao) :
                prodImp.IdAmbientePedido > 0 ? AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<uint>(session, "idProd", "idAmbientePedido=" + prodImp.IdAmbientePedido) :
                ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<uint>(session, "idProd", "idProdPed=" + prodImp.IdProdPed);

            // Salva os produtos no dicionário, para não precisar buscar sempre os mesmos dados
            Produto produto = null;
            if (!dicProd.ContainsKey(idProd))
                dicProd.Add(idProd, ProdutoDAO.Instance.GetElementByPrimaryKey(session, idProd));

            produto = dicProd[idProd];

            uint idClienteFornec = prodImp.IdPedido > 0 ? pedido.IdCli :
                prodImp.IdRetalhoProducao > 0 ? 0 :
                NotaFiscalDAO.Instance.ObtemIdFornec(session, prodImp.IdNf.Value).GetValueOrDefault();

            uint? idPedidoAnterior = prodImp.IdPedido > 0 ? pedido.IdPedidoAnterior : null;
            string codCliente = prodImp.IdPedido > 0 ? pedido.CodCliente : null;
            
            bool isPedidoImportado = prodImp.IdPedido > 0 ? pedido.Importado : false;

            bool redondo = prodImp.IdProdNf > 0 ? false : prodImp.IdAmbientePedido > 0 ?
                AmbientePedidoEspelhoDAO.Instance.IsRedondo(session, prodImp.IdAmbientePedido.Value) :
                prodImp.IdRetalhoProducao > 0 ? false :
                ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<bool>(session, "redondo", "idProdPed=" + prodImp.IdProdPed);

            float alturaEtiqueta = prodImp.IdProdNf > 0 ? ProdutosNfDAO.Instance.ObtemValorCampo<float>(session, "altura", "idProdNf=" + prodImp.IdProdNf) :
                prodImp.IdRetalhoProducao > 0 ? produto.Altura.GetValueOrDefault() :
                prodImp.IdAmbientePedido > 0 ? AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<float>(session, "altura", "idAmbientePedido=" + prodImp.IdAmbientePedido) :
                ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<float>(session, "if(alturaReal>0, alturaReal, altura)", "idProdPed=" + prodImp.IdProdPed);

            int larguraEtiqueta = redondo ? 0 : prodImp.IdProdNf > 0 ? ProdutosNfDAO.Instance.ObtemValorCampo<int>(session, "largura", "idProdNf=" + prodImp.IdProdNf) :
                prodImp.IdRetalhoProducao > 0 ? produto.Largura.GetValueOrDefault() :
                prodImp.IdAmbientePedido > 0 ? AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<int>(session, "largura", "idAmbientePedido=" + prodImp.IdAmbientePedido) :
                ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<int>(session, "if(larguraReal>0, larguraReal, largura)", "idProdPed=" + prodImp.IdProdPed);

            if (prodImp.IdPedido > 0 && alturaEtiqueta == 0 && larguraEtiqueta == 0 && PedidoDAO.Instance.IsMaoDeObra(null, prodImp.IdPedido.Value))
            {
                var idAmbiente = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<uint>(session, "idAmbientePedido", "idProdPed=" + prodImp.IdProdPed);
                alturaEtiqueta = AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<float>(session, "altura", "idAmbientePedido=" + idAmbiente);
                larguraEtiqueta = AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<int>(session, "largura", "idAmbientePedido=" + idAmbiente);
            }

            string ambiente = prodImp.IdProdNf > 0 ? null : prodImp.IdRetalhoProducao > 0 ? null : prodImp.IdAmbientePedido > 0 ?
                AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<string>(session, "ambiente", "idAmbientePedido=" + prodImp.IdAmbientePedido) :
                !isPedidoImportado || PedidoConfig.DadosPedido.AmbientePedido ?
                AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<string>(session, "ambiente", "idAmbientePedido=" +
                ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<uint>(session, "idAmbientePedido", "idProdPed=" + prodImp.IdProdPed)) :
                ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<string>(session, "pedCli", "idProdPed=" + prodImp.IdProdPed);

            uint? idProcesso = prodImp.IdProdNf > 0 ? null : prodImp.IdRetalhoProducao > 0 ? null : prodImp.IdAmbientePedido > 0 ?
                AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<uint?>(session, "idProcesso", "idAmbientePedido=" + prodImp.IdAmbientePedido) :
                ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<uint?>(session, "idProcesso", "idProdPed=" + prodImp.IdProdPed);

            uint? idAplicacao = prodImp.IdProdNf > 0 ? null : prodImp.IdRetalhoProducao > 0 ? null : prodImp.IdAmbientePedido > 0 ?
                AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<uint?>(session, "idAplicacao", "idAmbientePedido=" + prodImp.IdAmbientePedido) :
                ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<uint?>(session, "idAplicacao", "idProdPed=" + prodImp.IdProdPed);

            string obs = prodImp.IdProdNf > 0 ? ProdutosNfDAO.Instance.ObtemValorCampo<string>(session, "obs", "idProdNf=" + prodImp.IdProdNf) :
                prodImp.IdRetalhoProducao > 0 ? ProdutoDAO.Instance.ObtemValorCampo<string>(session, "obs", "idProd=" + prodImp.IdProd) :
                prodImp.IdAmbientePedido > 0 ? AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<string>(session, "obs", "idAmbientePedido=" + prodImp.IdAmbientePedido) :
                ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<string>(session, "obs", "idProdPed=" + prodImp.IdProdPed);

            var obsGrid = prodImp.IdProdPed > 0 ? ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<string>(session, "obsgrid", "idProdPed=" + prodImp.IdProdPed) : string.Empty;

            bool isEtiquetaNF = prodImp.IdProdNf > 0;

            var etiqueta = new Etiqueta();
            string nomeClienteFornec = "";
            string lote = "";

            var idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(session, prodImp.NumEtiqueta);
            if (idProdPedProducao.GetValueOrDefault(0) > 0)
            {
                var idRetalhoProducaoUsado = UsoRetalhoProducaoDAO.Instance.ObtemIdRetalhoProducao(session, idProdPedProducao.GetValueOrDefault(0));
                if (idRetalhoProducaoUsado.GetValueOrDefault(0) > 0)
                {
                    var rp = RetalhoProducaoDAO.Instance.GetElementByPrimaryKey(session, idRetalhoProducaoUsado.Value);
                    var altura = ProdutoDAO.Instance.ObtemValorCampo<int>("Altura", "IdProd=" + rp.IdProd);
                    var largura = ProdutoDAO.Instance.ObtemValorCampo<int>("Largura", "IdProd=" + rp.IdProd);

                    /* Chamado 16190.
                     * Foi solicitado que a altura e largura do retalho fossem exibidas na etiqueta.
                    obs = "Retalho: " + rp.NumeroEtiqueta + (!String.IsNullOrEmpty(obs) ? " - " : "") + obs;*/
                    obs = "Retalho: " + rp.NumeroEtiqueta + " (" + altura + " x " + largura + ")" + (!String.IsNullOrEmpty(obs) ? " - " : "") + obs;
                }

                //Busca etiqueta pai e posição
                etiqueta.PosEtiquetaParent = ProdutoPedidoProducaoDAO.Instance.ObterPosEtiquetaParent(session, idProdPedProducao.Value);
            }

            if (isEtiquetaNF)
            {
                lote = prodImp.Lote;
                nomeClienteFornec = FornecedorDAO.Instance.GetNome(session, idClienteFornec);
            }
            else if (prodImp.IdRetalhoProducao.GetValueOrDefault() > 0)
            {
                lote = prodImp.Lote;

                uint? idProdNf = RetalhoProducaoDAO.Instance.ObtemValorCampo<uint?>(session, "IdProdNf", "IdRetalhoProducao=" + prodImp.IdRetalhoProducao);

                if (idProdNf.GetValueOrDefault() == 0)
                    nomeClienteFornec = " ";
                else
                {
                    uint idNfe = ProdutosNfDAO.Instance.ObtemValorCampo<uint>(session, "IdNF", "IdProdNf=" + idProdNf);
                    uint idFornec = NotaFiscalDAO.Instance.ObtemIdFornec(session, idNfe).GetValueOrDefault();

                    nomeClienteFornec = FornecedorDAO.Instance.ObtemValorCampo<string>(session, "COALESCE(Razaosocial, NomeFantasia)", "IdFornec=" + idFornec);
                    etiqueta.NumeroNFe = idNfe > 0 ? NotaFiscalDAO.Instance.ObtemNumeroNf(session, idNfe).ToString() : "";

                    uint? idProdPedProducaoOrig = RetalhoProducaoDAO.Instance.ObtemValorCampo<uint?>(session, "IdProdPedProducaoOrig", "IdRetalhoProducao=" + prodImp.IdRetalhoProducao);
                    if (!idProdPedProducaoOrig.HasValue)
                        obs = "NFe: " + NotaFiscalDAO.Instance.ObtemNumeroNf(session, idNfe) + (!String.IsNullOrEmpty(obs) ? " - " : "") + obs;
                }
            }
            else
            {
                etiqueta.NomeFantasiaCliente = ClienteDAO.Instance.GetNomeFantasia(session, idClienteFornec);
                etiqueta.RazaoSocialCliente = ClienteDAO.Instance.GetNome(session, idClienteFornec);

                if (string.IsNullOrEmpty(etiqueta.NomeFantasiaCliente))
                    etiqueta.NomeFantasiaCliente = etiqueta.RazaoSocialCliente;

                nomeClienteFornec = etiqueta.RazaoSocialCliente;
            }
            
            if (prodImp.IdCliente > 0)
            {
                etiqueta.ComplementoCliente = ClienteDAO.Instance.ObterComplementoEndereco(session, (int)prodImp.IdCliente);
                etiqueta.TelefoneCliente = ClienteDAO.Instance.ObtemCelEnvioSMS(prodImp.IdCliente);
                etiqueta.Contato2 = ClienteDAO.Instance.ObtemValorCampo<string>(session, "contato2", "id_cli = " + prodImp.IdCliente);
            }

            DateTime? dataEntrFabr = 
                prodImp.IdProdNf > 0 ? NotaFiscalDAO.Instance.ObtemDataEntradaSaida(prodImp.IdNf.Value) : 
                prodImp.IdRetalhoProducao > 0 ? null : 
                EtiquetaConfig.TipoDataEtiqueta == DataSources.TipoDataEtiquetaEnum.Fábrica && dataFabrica != null && dataFabrica.Value.Ticks > 0 ? dataFabrica : 
                pedido.DataEntrega;

            var pedCli = prodImp.IdProdPed != null ? ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<string>(session, "pedCli", "idProdPed=" + prodImp.IdProdPed) : "";
            var usarPedidoRepos = PCPConfig.ControlarProducao && idPedidoAnterior != null;

            // Concatena o código do cliente com o pedCli somente se o pedido não for importado.
            if (prodImp.IdPedido > 0 && !pedido.Importado)
                etiqueta.CodCliente = (!String.IsNullOrEmpty(codCliente) ? codCliente + (!String.IsNullOrEmpty(pedCli) ? ". Prod.: " : "") : "") + pedCli;
            else
                etiqueta.CodCliente = codCliente;

            etiqueta.NomeCliente = nomeClienteFornec;

            if (prodImp.IdPedido > 0)
            {
                etiqueta.IdPedido = prodImp.IdPedido.ToString();
                etiqueta.TipoPedido = (int)PedidoDAO.Instance.GetTipoPedido(session, prodImp.IdPedido.Value);
                etiqueta.TipoVendaPedido = (int)PedidoDAO.Instance.ObtemTipoVenda(session, prodImp.IdPedido.Value);

                etiqueta.NomeFuncCadPedido = PedidoDAO.Instance.ObtemNomeFuncResp(session, Glass.Conversoes.StrParaUint(etiqueta.IdPedido));
                etiqueta.DataCadPedido = pedido.DataPedido;

                if (PCPConfig.Etiqueta.DestacarAlturaLarguraSeAresta0OuCNC)
                {
                    if (prodImp.IdProdPed > 0)
                    {
                        var idsBenef = ProdutoPedidoEspelhoBenefDAO.Instance.GetByProdutoPedido(session, prodImp.IdProdPed.Value).Select(f => (int)f.IdBenefConfig).ToList();
                        var aresta = ImpressaoEtiquetaDAO.Instance.GetAresta(null, (int)idProd, null, idsBenef, descrBenef, (int)idProcesso.GetValueOrDefault(0));
                        etiqueta.DestacarAlturaLargura = aresta == 0 || (descrBenef != null ? descrBenef.ToLower().Contains("cnc") : false);
                    }
                }

                if (prodImp.IdProdPed.GetValueOrDefault() > 0 || prodImp.IdAmbientePedido.GetValueOrDefault() > 0)
                {
                    etiqueta.SetoresRoteiro = string.Empty;

                    foreach (
                        var setor in
                            RoteiroProducaoEtiquetaDAO.Instance.ObtemSetoresPorPeca(session, prodImp.IdProdPed,
                                prodImp.IdAmbientePedido))
                        etiqueta.SetoresRoteiro += setor.Sigla + "\n";

                    etiqueta.SetoresRoteiro = etiqueta.SetoresRoteiro.TrimEnd(' ', '-');
                }
            }

            if (isEtiquetaNF)
            {
                etiqueta.IdNf = prodImp.IdNf.ToString();
                etiqueta.DataEntradaSaidaNFe = NotaFiscalDAO.Instance.ObtemDataEntradaSaida(prodImp.IdNf.Value);
                etiqueta.DataEmissaoNFe = NotaFiscalDAO.Instance.ObtemDataEmissao(prodImp.IdNf.Value);
            }

            etiqueta.CodInterno = !string.IsNullOrEmpty(produto.CodInterno) ? produto.CodInterno : string.Empty;

            /* Chamado 47689. */
            if (prodImp.IdProdPed.GetValueOrDefault() > 0)
            {
                var idProdPedParent = ProdutosPedidoEspelhoDAO.Instance.ObterIdProdPedParent(null, prodImp.IdProdPed.Value);

                if (idProdPedParent > 0)
                {
                    var idProdParent = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd(idProdPedParent.Value);
                    var descricaoProdParent = ProdutoDAO.Instance.GetDescrProduto((int)idProdParent);

                    etiqueta.CodInternoParent = ProdutoDAO.Instance.GetCodInterno((int)idProdParent);
                    etiqueta.DescrProdParent = descricaoProdParent +
                        (ProdutosPedidoEspelhoDAO.Instance.IsRedondo(idProdPedParent.Value) && !descricaoProdParent.ToLower().Contains("redondo") ? " REDONDO" : "");
                }
            }

            etiqueta.NumItem = !String.IsNullOrEmpty(prodImp.NumEtiqueta) ? prodImp.NumEtiqueta.Split('-')[1] :
                prodImp.PosicaoProd + "." + prodImp.ItemEtiqueta + "/" + prodImp.QtdeProd;
            etiqueta.ItemEtiqueta = prodImp.ItemEtiqueta;
            etiqueta.QtdeProd = prodImp.QtdeProd;

            etiqueta.IdSubgrupoProd = (uint?)produto.IdSubgrupoProd;

            var descrProd = produto.Descricao;
            etiqueta.DataImpressao = DateTime.Now;
            etiqueta.DescrProd = descrProd + (redondo && !descrProd.ToLower().Contains("redondo") ? " REDONDO" : "");
            etiqueta.CodApl = idAplicacao != null ? EtiquetaAplicacaoDAO.Instance.ObtemValorCampo<string>(session, "codInterno", "idAplicacao=" + idAplicacao) : null;
            etiqueta.CodProc = idProcesso != null ? EtiquetaProcessoDAO.Instance.ObtemValorCampo<string>("codInterno", "idProcesso=" + idProcesso) : null;

            /* Chamado 16513.
             * Foi solicitado que a tarja preta da etiqueta fosse alterada de posição de acordo com o tipo da peça, instalação ou caixilho.
             * Esta informação pode ser controlada através do tipo do processo associado à peça, dessa forma é possível exibir a tarja conforme solicitado. */
            etiqueta.TipoProcesso = idProcesso != null ? EtiquetaProcessoDAO.Instance.ObtemValorCampo<int?>("TipoProcesso", "idProcesso=" + idProcesso).GetValueOrDefault() : 0;

            etiqueta.CodOtimizacao = produto.CodOtimizacao;
            etiqueta.Altura = alturaEtiqueta.ToString();
            etiqueta.Largura = larguraEtiqueta.ToString();
            etiqueta.Ambiente = ambiente;
            etiqueta.Lote = lote;
            etiqueta.Obs = descrBenef.ToUpper() + " " + (!PCPConfig.Etiqueta.NaoExibirObsPecaAoImprimirEtiqueta ? obs  + " " + obsGrid: obsGrid);
            etiqueta.DescrBenef = descrBenef.ToUpper();
            etiqueta.IdImpressao = prodImp.IdImpressao.GetValueOrDefault();
            etiqueta.IdCliente = idClienteFornec;            
            etiqueta.PecaReposta = isPecaReposta;
            etiqueta.PedidoRepos = usarPedidoRepos ? "(" + idPedidoAnterior + "R)" : "";
            etiqueta.IdProdPedEsp = prodImp.IdProdPed > 0 ? prodImp.IdProdPed.Value :
                prodImp.IdAmbientePedido > 0 ? ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<uint>(session, "idProdPed", "idAmbientePedido=" + prodImp.IdAmbientePedido.Value) : 0;
            etiqueta.PlanoCorte = prodImp.PlanoCorte;
            etiqueta.DataProducao = dataFabrica;
            etiqueta.PosicaoArqOtimiz = prodImp.PosicaoArqOtimiz;
            etiqueta.Forma = prodImp.IdProdPed > 0 ? prodImp.Forma : String.Empty;
            etiqueta.DataEntrega = dataEntrFabr;
            etiqueta.DataFabrica = dataFabrica;
            etiqueta.NumSeq = prodImp.NumSeq;
            etiqueta.BarCodeData = prodImp.NumEtiqueta;
            etiqueta.NumEtiqueta = prodImp.NumEtiqueta;
            etiqueta.IdRetalhoProducao = (uint?)prodImp.IdRetalhoProducao;
            etiqueta.IdCorVidro = (uint?)prodImp.Cor;
            etiqueta.Espessura = (int)prodImp.Espessura;
            /* Chamado 40219. */
            etiqueta.Peso = Utils.CalcPeso(session, (int)prodImp.IdProd, prodImp.Espessura, prodImp.TotM2, (float)prodImp.Qtde, (float)prodImp.Altura, prodImp.IdNf > 0);

            if (prodImp.IdPedido > 0)
            {
                etiqueta.DataPedido = pedido.DataPedido;
                etiqueta.FastDelivery = pedido.FastDelivery;

                /* Chamado 50830. */
                if (pedido.IdLoja > 0)
                {
                    etiqueta.IdLoja = pedido.IdLoja;
                    etiqueta.TelefoneLoja = LojaDAO.Instance.ObtemTelefone(session, pedido.IdLoja);
                }

                // Carrega a rota do cliente
                Rota rota = RotaDAO.Instance.GetByCliente(session, idClienteFornec);
                etiqueta.CodRota = rota != null ? rota.CodInterno : null;
                etiqueta.DescrRota = rota != null ? rota.Descricao : null;

                // Carrega a cidade do cliente
                etiqueta.NomeCidade = CidadeDAO.Instance.GetNome(session, ClienteDAO.Instance.ObtemIdCidade(session, idClienteFornec));

                if (pedido.Importado)
                {
                    etiqueta.RotaExterna = pedido.RotaExterna;
                    etiqueta.ClienteExterno = pedido.ClienteExterno;
                    etiqueta.PedCliExterno = pedido.PedCliExterno;
                }

                etiqueta.MaoDeObra = pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObra;

                if (prodImp.IdProdPed > 0)
                {
                    var idMaterItemProj = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<uint>(session, "idMaterItemProj", "idProdPed=" + prodImp.IdProdPed);
                    var pecaProjMod = PecaItemProjetoDAO.Instance.GetByMaterial(session, idMaterItemProj);

                    if (pecaProjMod == null)
                    {
                        var idProduto = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd(session, prodImp.IdProdPed.Value);
                        etiqueta.Flags = string.Join(",", FlagArqMesaDAO.Instance.ObtemPorProduto(session, (int)idProduto, false)?.Select(f => f.Descricao));
                    }
                    else
                    {
                        etiqueta.Flags = string.Join(",", FlagArqMesaDAO.Instance.ObtemPorPecaProjMod((int)pecaProjMod.IdPecaProjMod, true)?.Select(f => f.Descricao));
                    }

                    if (PCPConfig.EmpresaGeraArquivoFml)
                    {
                        etiqueta.PossuiFml = ProdutosPedidoEspelhoDAO.Instance.PossuiFml(session, prodImp.IdProdPed.Value, etiqueta.NumEtiqueta, true);
                    }

                    if (PCPConfig.EmpresaGeraArquivoDxf)
                    {
                        etiqueta.PossuiDxf = ProdutosPedidoEspelhoDAO.Instance.PossuiDxf(session, prodImp.IdProdPed.Value, etiqueta.NumEtiqueta);
                    }

                    if (PCPConfig.EmpresaGeraArquivoSGlass)
                    {
                        etiqueta.PossuiSGlass = ProdutosPedidoEspelhoDAO.Instance.PossuiSGlass(session, prodImp.IdProdPed.Value, etiqueta.NumEtiqueta);
                    }

                    if (PCPConfig.EmpresaGeraArquivoIntermac)
                    {
                        etiqueta.PossuiIntermac = ProdutosPedidoEspelhoDAO.Instance.PossuiIntermac(session, (int)prodImp.IdProdPed.Value, etiqueta.NumEtiqueta);
                    }
                }
            }
            else if(prodImp.IdNf > 0)
            {
                // Recupera o ID e o telefone da loja da nota fiscal.
                etiqueta.IdLoja = NotaFiscalDAO.Instance.ObtemIdLoja(session, prodImp.IdNf.Value);
                var idFornecedorNf = NotaFiscalDAO.Instance.ObtemIdFornec(session, prodImp.IdNf.Value);

                if (idFornecedorNf > 0)
                    etiqueta.TelefoneLoja = FornecedorDAO.Instance.ObterTelCont(session, idFornecedorNf.Value);

                // Carrega a cidade do cliente
                etiqueta.NomeCidade = CidadeDAO.Instance.GetNome(session, FornecedorDAO.Instance.ObtemValorCampo<uint>(session, "idCidade", "idFornec=" + idClienteFornec));

                // Recupera o número da nota fiscal
                etiqueta.NumeroNFe = NotaFiscalDAO.Instance.ObtemNumerosNFePeloIdNf(prodImp.IdNf.Value.ToString());
            }

            // Carrega a quantidade de peças que existem no pedido/nota fiscal desta etiqueta
            etiqueta.QtdPecasPedido = prodImp.IdPedido > 0 ? ProdutosPedidoEspelhoDAO.Instance.ObtemQtdPecasVidroPedido(session, prodImp.IdPedido.Value) :
                prodImp.IdRetalhoProducao > 0 ? 1 :
                ProdutosNfDAO.Instance.ObtemQtdPecasNf(prodImp.IdNf.Value);

            uint? idItemProjeto = null;

            // Verifica se há destaque na etiqueta
            if (prodImp.IdProdPed > 0 || prodImp.IdAmbientePedido > 0)
            {
                if (idProcesso != null)
                    etiqueta.DestacarEtiqueta = EtiquetaProcessoDAO.Instance.ObtemValorCampo<bool>(session, "DestacarEtiqueta", string.Format("IdProcesso={0}", idProcesso));

                if (!etiqueta.DestacarEtiqueta && idAplicacao != null)
                    etiqueta.DestacarEtiqueta = EtiquetaAplicacaoDAO.Instance.ObtemValorCampo<bool>(session, "DestacarEtiqueta", string.Format("IdAplicacao={0}", idAplicacao));

                // Carrega a observação do item projeto.
                idItemProjeto = prodImp.IdProdPed > 0 ? ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<uint?>(session, "idItemProjeto", "idProdPed=" + prodImp.IdProdPed) : idItemProjeto;

                if (idItemProjeto.GetValueOrDefault() > 0)
                {
                    etiqueta.ObsItemProjeto = ItemProjetoDAO.Instance.ObtemValorCampo<string>(session, "obs", "idItemProjeto=" + idItemProjeto);

                    if (!string.IsNullOrEmpty(etiqueta.ObsItemProjeto))
                        etiqueta.ObsItemProjeto = etiqueta.ObsItemProjeto.Replace("\n", "");
                }
            }

            if (!String.IsNullOrEmpty(etiqueta.PlanoCorte))
            {
                bool encontrou = false;

                // Chamado 17526: Foi colocado o "|| isPecaReposta" para resolver este chamado
                foreach (KeyValuePair<string, string> pc in planosCorte)
                    if (pc.Key == etiqueta.PlanoCorte && (pc.Value == prodImp.IdImpressao.ToString() || isPecaReposta))
                    {
                        encontrou = true;
                        break;
                    }

                if (!encontrou)
                    planosCorte.Add(new KeyValuePair<string, string>(etiqueta.PlanoCorte, prodImp.IdImpressao.ToString()));
            }

            etiqueta.AlturaLargura = etiqueta.Altura + " X " + etiqueta.Largura;
            if (!PedidoConfig.EmpresaTrabalhaAlturaLargura)
                etiqueta.AlturaLargura = etiqueta.Largura + " X " + etiqueta.Altura;
            
            if (prodImp.IdPedido > 0)
                etiqueta.DescrTipoEntrega = Utils.GetDescrTipoEntrega(pedido.TipoEntrega);

            if (produto.IdSubgrupoProd > 0)
                etiqueta.DescricaoSubgrupo = SubgrupoProdDAO.Instance.ObtemValorCampo<string>(session, "descricao", "idSubgrupoProd=" + produto.IdSubgrupoProd);
            
            if (EtiquetaConfig.RelatorioEtiqueta.CarregarDescricaoGrupoProjeto && idItemProjeto.GetValueOrDefault() > 0)
            {
                etiqueta.ObsItemProjeto = etiqueta.ObsItemProjeto != null ? etiqueta.ObsItemProjeto.Replace("\n", "") : "";
                        
                var idProjetoModelo = ItemProjetoDAO.Instance.ObtemValorCampo<uint?>
                    (session, "idProjetoModelo", "idItemProjeto=" + idItemProjeto.Value);

                var idGrupoModelo = idProjetoModelo.GetValueOrDefault() > 0 ? ProjetoModeloDAO.Instance.ObtemValorCampo<uint?>
                    (session, "idGrupoModelo", "idProjetoModelo=" + idProjetoModelo.Value) : 0;

                etiqueta.DescrGrupoProj = idGrupoModelo.GetValueOrDefault() > 0 ?
                    GrupoModeloDAO.Instance.ObtemValorCampo<string>(session, "Descricao", "idGrupoModelo=" + idGrupoModelo.Value) : "";

                etiqueta.DescrGrupoProj = etiqueta.DescrGrupoProj != null && etiqueta.DescrGrupoProj.Contains(" ") ?
                    etiqueta.DescrGrupoProj.Split(' ')[0] : etiqueta.DescrGrupoProj;
            }

            if (prodImp.IdPedido > 0)
                etiqueta.DestacarVidroTemperadoEtiqueta = SubgrupoProdDAO.Instance.IsVidroTemperado(session, idProd);

            etiqueta.ImpressoPor = BibliotecaTexto.GetTwoFirstNames(FuncionarioDAO.Instance.GetNome(session, idFunc));

            return etiqueta;
        }

        #endregion

        #region Validação de retalhos

        public string ValidaRetalho(GDASession session, uint idProdPedEsp, RetalhoProducao ret)
        {
            var alturaPeca = (decimal)ProdutosPedidoEspelhoDAO.Instance.ObtemAlturaProducao(session, idProdPedEsp);
            var larguraPeca = (decimal)ProdutosPedidoEspelhoDAO.Instance.ObtemLarguraProducao(session, idProdPedEsp);

            if ((ret.Altura < alturaPeca || ret.Largura < larguraPeca) &&
                (ret.Altura < larguraPeca || ret.Largura < alturaPeca))
                return string.Format("O retalho {0} é menor do que a peça.", ret.NumeroEtiqueta);

            // Considera o m² peça sem o múltiplo de 5
            var totMPeca = (alturaPeca * larguraPeca) / 1000000M;

            if ((decimal)ret.TotM - (decimal)ret.TotMUsando < totMPeca)
                return string.Format("O M2 disponível do retalho {0} é menor que o M2 da peça. M2 disponível do retalho: {1}; M2 da peça: {2}.",
                    ret.NumeroEtiqueta, Math.Round(ret.TotM - ret.TotMUsando, 3), Math.Round(totMPeca, 3));

            return "";
        }

        #endregion

        #region Etiquetas DXF

        /// <summary>
        /// Busca as etiquetas que podem gerar arquivo DXF
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstProdPedEsp"></param>
        /// <returns></returns>
        public List<Etiqueta> EtiquetasGerarDxf(GDASession session, ProdutosPedidoEspelho[] lstProdPedEsp)
        {
            var lstEtiqueta = new List<Etiqueta>();

            // Percorre os produtos
            foreach (var prodPedEsp in lstProdPedEsp)
            {
                var gerar = false;

                if (prodPedEsp.IdMaterItemProj.GetValueOrDefault() == 0 ||  MaterialItemProjetoDAO.Instance.ObtemidPecaItemProj(session, prodPedEsp.IdMaterItemProj.Value) == 0)
                {
                    if (prodPedEsp.IdProcesso.GetValueOrDefault() == 0)
                        continue;

                    var tipoProcesso = EtiquetaProcessoDAO.Instance.ObtemTipoProcesso(prodPedEsp.IdProcesso.Value);

                    // Verifica se a peça avulsa é de instalação. Se não for instalação passa para a próxima.
                    if (tipoProcesso != (int)EtiquetaTipoProcesso.Caixilho && tipoProcesso != (int)EtiquetaTipoProcesso.Instalacao)
                        continue;

                    //Se o produto avulso for de instalação verifica se o mesmo tem arquivo DXF.
                    var idArquivoMesaCorte = ProdutoDAO.Instance.ObtemIdArquivoMesaCorte(session, prodPedEsp.IdProd);

                    if (idArquivoMesaCorte.GetValueOrDefault() == 0)
                        continue;

                    var tipoArquivo = ProdutoDAO.Instance.ObterTipoArquivoMesaCorte(session, (int)prodPedEsp.IdProd);

                    if (tipoArquivo == TipoArquivoMesaCorte.DXF)
                        gerar = true;
                }
                else // Se o item for de projeto
                {
                    var idPecaItemProj = MaterialItemProjetoDAO.Instance.ObtemidPecaItemProj(session, prodPedEsp.IdMaterItemProj.Value);

                    var pecaItemProjeto = PecaItemProjetoDAO.Instance.GetElement(session, idPecaItemProj);

                    // Se a peça não for instalação passa para a próxima.
                    if (pecaItemProjeto == null)
                        continue;

                    // Se a peça não tiver arquivo DXF
                    var idArquivoMesaCorte = pecaItemProjeto.IdArquivoMesaCorte.GetValueOrDefault();

                    if (idArquivoMesaCorte == 0)
                        continue;

                    var tipoArquivoMesaCorte = PecaProjetoModeloDAO.Instance.ObtemValorCampo<TipoArquivoMesaCorte?>(session, "TipoArquivo", "IdPecaProjMod=" + pecaItemProjeto.IdPecaProjMod);

                    if (tipoArquivoMesaCorte == null)
                        continue;

                    var flags = FlagArqMesaDAO.Instance.ObtemPorPecaProjMod((int)pecaItemProjeto.IdPecaProjMod, true);

                    if (tipoArquivoMesaCorte == TipoArquivoMesaCorte.DXF || (flags != null && flags.Any(f => f.Descricao.ToLower() == TipoArquivoMesaCorte.DXF.ToString().ToLower())))
                        gerar = true;
                }

                if (gerar)
                {
                    // Busca as etiquetas da peça
                    var etiquetas = PecaItemProjetoDAO.Instance.ObtemEtiquetas(session, prodPedEsp.IdPedido, prodPedEsp.IdProdPed, (int)prodPedEsp.Qtde);

                    // Percorre as etiquetas da peça
                    foreach (var etiqueta in etiquetas.Split(','))
                    {
                        lstEtiqueta.Add(new Etiqueta()
                        {
                            NumEtiqueta = etiqueta,
                            IdPedido = prodPedEsp.IdPedido.ToString(),
                            IdProdPedEsp = prodPedEsp.IdProdPed,
                            Espessura = prodPedEsp.Espessura,
                            Altura = prodPedEsp.AlturaProducao.ToString(),
                            Largura = prodPedEsp.LarguraProducao.ToString(),
                        });
                    }
                }

                gerar = false;
            }

            return lstEtiqueta;
        }

        #endregion
    }
}