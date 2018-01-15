using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using GDA;
using Glass.Data.Model;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class PecasPendentesDAO : BaseDAO<PecasPendentes, PecasPendentesDAO>
    {
        //private PecasPendentesDAO() { }

        #region Busca peças pendentes para gráfico da produção

        public Dictionary<int, double> ProducaoPendenteClassificacao(string dataIni, string dataFim, int idClassificacao)
        {
            if (idClassificacao == 0)
                return new Dictionary<int, double>();

            string comand =
                @"Select Round(Sum(IF(ped.tipoPedido = 3,
                            ((((50 - IF(MOD(a.altura, 50) > 0, MOD(a.altura, 50), 50)) + a.altura) *
                            ((50 - IF(MOD(a.largura, 50) > 0, MOD(a.largura, 50), 50)) + a.largura)) / 1000000) *
                            a.qtde, ppo.TotM2Calc) / (ppo.qtde * IF(ped.tipoPedido = 3, a.qtde, 1))), 2) As TotM, 
                     Date(pe.dataFabrica) As Data
                From pedido ped
                    INNER JOIN pedido_espelho pe ON (ped.idPedido = pe.idPedido)
                    INNER JOIN produtos_pedido_espelho pp ON (ped.idPedido = pp.idPedido)
                    INNER JOIN produtos_pedido ppo ON (ppo.idProdPedEsp = pp.idProdPed)
                    INNER JOIN produto_pedido_producao ppp ON (pp.idProdPed = ppp.idProdPed)
                    LEFT JOIN ambiente_pedido_espelho a on (pp.idAmbientePedido = a.idAmbientePedido)
                    INNER JOIN (
                        SELECT IF(COUNT(*) > 0, TRUE, FALSE) AS PecaPronta, rpe.IdProdPedProducao, rpe.IdSetor
                        FROM roteiro_producao_etiqueta rpe
                        WHERE COALESCE(rpe.UltimoSetor, FALSE)
                            GROUP BY rpe.IdProdPedProducao) roteiro ON (roteiro.IdProdPedProducao=ppp.IdProdPedProducao)
                Where Date(pe.dataFabrica) >= ?dataIni And Date(pe.dataFabrica) <= ?dataFim
                    And ppp.situacaoProducao=?situacaoProd                    
                    And ppp.situacao=?situacao " +

                    /* Chamado 45622. */
                    (idClassificacao > 0 ? string.Format(" AND pp.IdProcesso IN (SELECT IdProcesso FROM roteiro_producao WHERE IdClassificacaoRoteiroProducao={0}) ", idClassificacao) : string.Empty) +

                    " Group By Date(pe.dataFabrica)";
            
            var result = objPersistence.LoadResult(comand,
                new GDAParameter("?dataIni", dataIni),
                new GDAParameter("?dataFim", dataFim),
                new GDAParameter("?situacaoProd", SituacaoProdutoProducao.Pendente),
                new GDAParameter("?situacao", ProdutoPedidoProducao.SituacaoEnum.Producao))
                .Select(f => new KeyValuePair<int, double>(f.GetDateTime("Data").Day, f["TotM"]));

            var producaoPendente = new Dictionary<int, double>();

            foreach (var i in result)
                producaoPendente.Add(i.Key, i.Value);

            return producaoPendente;
        }

        public Dictionary<int, double> ProducaoPendente(string dataIni, string dataFim)
        {
            string comand =
                @"Select Round(Sum(IF(ped.tipoPedido = 3,
                            ((((50 - IF(MOD(a.altura, 50) > 0, MOD(a.altura, 50), 50)) + a.altura) *
                            ((50 - IF(MOD(a.largura, 50) > 0, MOD(a.largura, 50), 50)) + a.largura)) / 1000000) *
                            a.qtde, ppo.TotM2Calc) / (ppo.qtde * IF(ped.tipoPedido = 3, a.qtde, 1))), 2) As TotM, 
                     Date(pe.dataFabrica) As Data
                From pedido ped
                    INNER JOIN pedido_espelho pe ON (ped.idPedido = pe.idPedido)
                    INNER JOIN produtos_pedido_espelho pp ON (ped.idPedido = pp.idPedido)
                    INNER JOIN produtos_pedido ppo ON (ppo.idProdPedEsp = pp.idProdPed)
                    INNER JOIN produto_pedido_producao ppp ON (pp.idProdPed = ppp.idProdPed)
                    LEFT JOIN ambiente_pedido_espelho a on (pp.idAmbientePedido = a.idAmbientePedido)
                Where Date(pe.dataFabrica) >= ?dataIni And Date(pe.dataFabrica) <= ?dataFim
                    And ppp.situacaoProducao=?situacaoProd
                    And ppp.situacao=?situacao
                    Group By Date(pe.dataFabrica)";

            var result = objPersistence.LoadResult(comand, 
                new GDAParameter("?dataIni", dataIni), 
                new GDAParameter("?dataFim", dataFim),
                new GDAParameter("?situacaoProd", SituacaoProdutoProducao.Pendente),
                new GDAParameter("?situacao", ProdutoPedidoProducao.SituacaoEnum.Producao))
                .Select(f => new KeyValuePair<int, double>(f.GetDateTime("Data").Day, f["TotM"]));

            var producaoPendente = new Dictionary<int, double>();
            
            foreach (var i in result)
                producaoPendente.Add(i.Key, i.Value);

            return producaoPendente;
        }

        #endregion

        #region Método Privados

        /// <summary>
        /// Recupera a quantidade de peças pendentes em seus respectivos setores.
        /// </summary>
        /// <param name="tipoPeriodo"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        private IEnumerable<PecasPendentes> ObtemPecasPendentes(string tipoPeriodo, DateTime dataIni, DateTime dataFim)
        {
            var campoData = tipoPeriodo == "PeriodoEntrega" ? "ped.dataEntrega" : "pedEsp.dataFabrica";
            var comando =
               @"SELECT ppp.IdSetor, {1} AS Data, COUNT(*) as Qtde, SUM(pp.Totm/pp.Qtde) as TotM
                FROM produto_pedido_producao ppp
                INNER JOIN produtos_pedido pp ON (ppp.idProdPed = pp.idProdPedEsp)
                INNER JOIN pedido ped ON (pp.idPedido = ped.idPedido)
                {0} WHERE {1} >= ?dataIni AND {1} <= ?dataFim AND ppp.situacao=?situacao 
                GROUP BY ppp.IdSetor, {1}";

            comando = string.Format(comando,
                tipoPeriodo == "PeriodoEntrega" ? "" :
                    " LEFT JOIN pedido_espelho pedEsp ON (ped.`IDPEDIDO` = pedEsp.`IDPEDIDO`)",
                campoData);

            using (var session = new GDASession())
            {
                // 
                var setores = objPersistence.LoadResult(session,
                    "SELECT IDSETOR, DESCRICAO, NUMSEQ FROM setor WHERE situacao=?situacao ORDER BY NUMSEQ",
                    new GDAParameter("?situacao", Glass.Situacao.Ativo))
                    .Select(f => new
                    {
                        IdSetor = (int)f["IdSetor"],
                        Descricao = f["Descricao"],
                        NumSequencia = (int)f["NumSeq"]
                    }).ToList();

                var itens = objPersistence.LoadResult(session, comando,
                    new GDAParameter("?situacao", ProdutoPedidoProducao.SituacaoEnum.Producao),
                    new GDAParameter("?dataIni", dataIni),
                    new GDAParameter("?dataFim", dataFim))
                    .Select(f => new ItemQuantidade
                    {
                        IdSetor = (uint)f["IdSetor"],
                        Data = (DateTime)f["Data"],
                        Qtd = (uint)f["Qtde"],
                        TotM = (double)f["TotM"]
                    })
                    .ToList();


                /*if (usarProximoSetor)
                    for (var i = setores.Count - 2; i >= 0; i--)
                    {
                        var setor = setores[i];

                        foreach (var j in itens.Where(f => f.IdSetor == setor.IdSetor))
                            // Altera o setor para o próximo
                            j.IdSetor = setores[i + 1].IdSetor;

                    }*/

                var totalDias = dataFim.Subtract(dataIni).TotalDays;

                foreach (var setor in setores)
                {
                    for (var j = 0; j < totalDias; j++)
                    {
                        var data = dataIni.AddDays(j).Date;

                        // Recupera as pendencias
                        var pendente = new PecasPendentes
                        {
                            Setor = setor.Descricao,
                            NumSeqSetor = setor.NumSequencia,
                            Data = data,
                            Qtde = itens.Where(f => f.IdSetor == setor.IdSetor && f.Data.Date == data)
                                        .Sum(f => f.Qtd),
                            TotM = itens.Where(f => f.IdSetor == setor.IdSetor && f.Data.Date == data)
                                        .Sum(f => f.TotM)
                        };

                        yield return pendente;
                    }
                }
            }
        }

        #endregion

        #region Métodos Publicos

        /// <summary>
        /// Recupera os registro para o report
        /// </summary>
        /// <param name="tipoPeriodo"></param>
        /// <param name="usarProximoSetor">Identifica se é para colocar as peças pendentes no próximo setor onde ela deve estar.</param>
        /// <param name="dataIni">Data de início do período.</param>
        /// <param name="dataFim">Data final do período.</param>
        /// <returns></returns>
        public IEnumerable<PecasPendentes> GetListForRpt(string tipoPeriodo, bool usarProximoSetor, DateTime dataIni, DateTime dataFim)
        {
            if (!usarProximoSetor)
            {
                foreach (var i in ObtemPecasPendentes(tipoPeriodo, dataIni, dataFim))
                    yield return i;
                
                yield break;
            }

            var campoData = tipoPeriodo == "PeriodoEntrega" ? "ped.dataEntrega" : "pedEsp.dataFabrica";
            var comando =
               @"SELECT 
                    ppp.IdProdPedProducao, ppp.IdSetor, 
                    {1} AS Data,
                    rpe.IdSetor AS IdSetorRoteiro,
                    ped.TipoEntrega,
                    ppp.SituacaoProducao
                FROM produto_pedido_producao ppp
                INNER JOIN produtos_pedido pp ON (ppp.idProdPed = pp.idProdPedEsp)
                INNER JOIN pedido ped ON (pp.idPedido = ped.idPedido)
                INNER JOIN roteiro_producao_etiqueta rpe ON (rpe.idProdPedProducao = ppp.idProdPedProducao) 
                {0} WHERE {1} >= ?dataIni AND {1} <= ?dataFim AND ppp.situacao=?situacao";

            comando = string.Format(comando,
                tipoPeriodo == "PeriodoEntrega" ? "" :
                    " LEFT JOIN pedido_espelho pedEsp ON (ped.`IDPEDIDO` = pedEsp.`IDPEDIDO`)",
                campoData);

            using (var session = new GDASession())
            {
                // Carrega os setores do sistema
                var setores = objPersistence.LoadResult(session,
                    "SELECT IDSETOR, DESCRICAO, NUMSEQ, TIPO FROM setor WHERE situacao=?situacao ORDER BY NUMSEQ",
                    new GDAParameter("?situacao", Glass.Situacao.Ativo))
                    .Select(f => new
                    {
                        IdSetor = (uint)f["IdSetor"],
                        Descricao = f["Descricao"],
                        NumSequencia = (uint)f["NumSeq"],
                        Tipo = (TipoSetor)(int)f["Tipo"]
                    }).ToList();

                // Carrega os pedidos de produção agrupados
                var pedidos = objPersistence.LoadResult(session, comando,
                    new GDAParameter("?situacao", ProdutoPedidoProducao.SituacaoEnum.Producao),
                    new GDAParameter("?dataIni", dataIni),
                    new GDAParameter("?dataFim", dataFim))
                    .Select(f => new ProdutoPedidoProducaoInfo
                    {
                        IdProdPedProducao = f["IdProdPedProducao"],
                        IdSetor = (uint)f["IdSetor"],
                        Data = ((DateTime)f["Data"]).Date,
                        IdSetorRoteiro = (uint)f["IdSetorRoteiro"],
                        TipoEntrega = f["TipoEntrega"],
                        SituacaoProducao = f["SituacaoProducao"]
                    })
                    .ToList()
                    .GroupBy(f => f.IdProdPedProducao);

                var itens = new List<ItemQuantidade>();

                foreach (var i in pedidos)
                {
                    // Recupera os setores do roteiro do pedido
                    var pedidoSetores =
                        setores.Where(f => i.Any(x => x.IdSetorRoteiro == f.IdSetor))
                        // Ordena a sequencia dos setores
                               .OrderBy(f => f.NumSequencia)
                               .ToList();

                    var pedido = i.First();
                    // Recupera atual setor do pedido
                    var atualIdSetor = pedido.IdSetor;

                    // Recuper o indice do atual setor do pedido
                    var indice = pedidoSetores.FindIndex(f => f.IdSetor == atualIdSetor);

                    if (pedido.SituacaoProducao == (int)SituacaoProdutoProducao.Pronto && 
                        Glass.Configuracoes.OrdemCargaConfig.UsarControleOrdemCarga)
                    {
                        // Localiza o indice do setor na relação de todos os setores
                        var indice2 = setores.FindIndex(f => f.IdSetor == atualIdSetor);

                        if (pedido.TipoEntrega.GetValueOrDefault() == (int)Data.Model.Pedido.TipoEntregaPedido.Balcao)
                        {
                            // Recupera o próximo setor do tipo entregue
                            var setor = setores
                                .Skip(indice2)
                                .FirstOrDefault(f => f.Tipo == TipoSetor.Entregue);

                            if (setor != null)
                                atualIdSetor = setor.IdSetor;
                        }
                        else if (pedido.TipoEntrega.GetValueOrDefault() == Helper.DataSources.Instance.GetTipoEntregaEntrega())
                        {
                            // Recupera o próximo setor do tipo Expedição carregamento
                            var setor = setores
                                .Skip(indice2)
                                .FirstOrDefault(f => f.Tipo == TipoSetor.ExpCarregamento);

                            if (setor != null)
                                atualIdSetor = setor.IdSetor;
                        }
                    }

                    // Verifica se o setor atual não foi alterado
                    if (atualIdSetor == pedido.IdSetor)
                    {
                        if (indice < 0)
                        {
                            // Recupera o setor atual
                            var setor = setores.FirstOrDefault(f => f.IdSetor == atualIdSetor);

                            // Verifica se o tipo do setor é pendente
                            if (setor != null && setor.Tipo == TipoSetor.Pendente)
                            {
                                // Recupera o primeiro setor do roteiro
                                setor = pedidoSetores.FirstOrDefault();

                                if (setor != null)
                                    atualIdSetor = setor.IdSetor;
                            }
                        }
                        // Verifica se o setor não é o último
                        else if (indice != (pedidoSetores.Count - 1))
                            // Recupera o identificador do próximo setor
                            atualIdSetor = pedidoSetores[indice + 1].IdSetor;
                    }

                    // Tenta recupera o item já catalogado
                    var item = itens.FirstOrDefault(f => f.IdSetor == atualIdSetor && f.Data == pedido.Data);

                    if (item == null)
                        // Adiciona um novo item
                        itens.Add(new ItemQuantidade
                        {
                            IdSetor = atualIdSetor,
                            Data = pedido.Data,
                            Qtd = 1
                        });
                    else
                        item.Qtd++;
                }

                var totalDias = dataFim.Subtract(dataIni).TotalDays;

                foreach(var setor in setores)
                {
                    for (var j = 0; j < totalDias; j++)
                    {
                        var data = dataIni.AddDays(j).Date;

                        // Recupera as pendencias
                        var pendente = new PecasPendentes
                        {
                            Setor = setor.Descricao,
                            NumSeqSetor = setor.NumSequencia,
                            Data = data,
                            Qtde = itens.Where(f => f.IdSetor == setor.IdSetor && f.Data.Date == data)
                                        .Sum(f => f.Qtd)
                        };

                        yield return pendente;
                    }
                }
            }
        }

        #endregion

        #region Tipos Aninhados

        /// <summary>
        /// Armazena as informações da produção do pedido do produto.
        /// </summary>
        class ProdutoPedidoProducaoInfo
        {
            /// <summary>
            /// Identificador do pedido na produção.
            /// </summary>
            public uint IdProdPedProducao;
            /// <summary>
            /// Identificador do setor.
            /// </summary>
            public uint IdSetor;
            /// <summary>
            /// Data do pedido de produção.
            /// </summary>
            public DateTime Data;
            /// <summary>
            /// Identificador do setor do roteiro.
            /// </summary>
            public uint IdSetorRoteiro;
            /// <summary>
            /// Tipo de entrega.
            /// </summary>
            public int? TipoEntrega;
            /// <summary>
            /// Situação da produção.
            /// </summary>
            public int SituacaoProducao;
        }

        /// <summary>
        /// Representa um item da peça pendente.
        /// </summary>
        class ItemQuantidade
        {
            /// <summary>
            /// Identificador do setor.
            /// </summary>
            public uint IdSetor;
            /// <summary>
            /// Data associada.
            /// </summary>
            public DateTime Data;
            /// <summary>
            /// Quantidade de peças.
            /// </summary>
            public uint Qtd;

            /// <summary>
            /// Total de M²
            /// </summary>
            public double TotM;
        }

        #endregion
    }
}
