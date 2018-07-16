using GDA;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Estoque.Negocios.Entidades;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.PCP.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negocios da expedicao
    /// </summary>
    public class ExpedicaoFluxo : IExpedicaoFluxo, Entidades.IProvedorExpBalcao
    {
        #region Exp. Balcão

        /// <summary>
        /// Classe responsavel por armazenar os dados das peças de revenda ja expedidas (chapas e box)
        /// </summary>
        private class ItemRevendaExp
        {
            public int IdPedido { get; set; }

            public int IdProd { get; set; }

            public int IdFuncLeitura { get; set; }

            public DateTime DataLeitura { get; set; }

            public string NumEtiqueta { get; set; }

            public string NomeFuncLeitura { get; set; }

            public int? IdProdPedProducao { get; set; }

            public int? IdProdImpressaoChapa { get; set; }

            public bool TrocadoDevolvido { get; set; }
        }

        /// <summary>
        /// Recupera os dados para a expedição balcão
        /// </summary>
        public Entidades.ExpBalcao BuscaParaExpBalcao(int idLiberarPedido, int idPedido, string visualizar)
        {
            if (idLiberarPedido == 0)
                return new Entidades.ExpBalcao();

            #region Variaveis Locais

            var retorno = new Entidades.ExpBalcao(idLiberarPedido);
            var itensRevenda = new List<ProdutosPedido>();
            var itensRevendaExp = new List<ItemRevendaExp>();
            var etqsUtilizadas = new List<string>();

            #endregion

            #region Consultas

            var consultaLiberacao = SourceContext.Instance.CreateQuery()
                .From<LiberarPedido>("lp")
                    .InnerJoin<Cliente>("lp.IdCliente = c.IdCli", "c")
                .Where(@"lp.IdLiberarPedido = ?id").Add("?id", idLiberarPedido)
                .Select("lp.IdLiberarPedido as IdLiberacao, c.IdCli as IdCliente, IsNull(c.NomeFantasia, c.Nome) as NomeCliente");

            var consultaVenda = SourceContext.Instance.CreateQuery()
                .From<ProdutoPedidoProducao>("ppp")
                    .InnerJoin<ProdutosPedido>("ppp.IdProdPed = pp.IdProdPedEsp", "pp")
                    .InnerJoin<Glass.Data.Model.Pedido>("p.IdPedido = pp.IdPedido", "p")
                    .InnerJoin<ProdutosLiberarPedido>("p.IdPedido = plp.IdPedido", "plp")
                    .InnerJoin<Produto>("pp.IdProd = prod.IdProd", "prod")
                .Where(@"plp.IdLiberarPedido = ?id 
                        AND p.TipoEntrega = ?tipoEntrega
                        AND (pp.InvisivelFluxo IS NULL OR pp.InvisivelFluxo=0)
                        AND ppp.Situacao = ?situacaoPPP
                        AND ppp.IdProdPedProducaoParent IS NULL")
                    .Add("?situacaoPPP", ProdutoPedidoProducao.SituacaoEnum.Producao)
                    .Add("?id", idLiberarPedido)
                    .Add("?tipoEntrega", Glass.Data.Model.Pedido.TipoEntregaPedido.Balcao)
                .Select(@"p.IdPedido, ppp.IdProdPedProducao, ppp.NumEtiqueta, p.CodCliente as PedCli,
                            (pp.Peso / pp.Qtde) as Peso, prod.CodInterno as CodProduto, prod.Descricao as DescProduto, pp.Altura, pp.Largura,
                            (pp.TotM / pp.Qtde) as M2")
                .GroupBy("ppp.IdProdPedProducao");

            var consultaRevenda = SourceContext.Instance.CreateQuery()
                .From<ProdutosPedido>("pp")
                    .InnerJoin<Glass.Data.Model.Pedido>("p.IdPedido = pp.IdPedido", "p")
                    .InnerJoin<ProdutosLiberarPedido>("p.IdPedido = plp.IdPedido", "plp")
                    .InnerJoin<Produto>("pp.IdProd = prod.IdProd", "prod")
                    .LeftJoin<SubgrupoProd>("sgp.IdSubgrupoProd = prod.IdSubgrupoProd", "sgp")
                .Where(@"plp.IdLiberarPedido = ?id
                        AND p.TipoEntrega = ?tipoEntrega
                        AND (pp.InvisivelFluxo IS NULL OR pp.InvisivelFluxo=0)
                        AND (sgp.ProdutosEstoque IS NOT NULL AND sgp.ProdutosEstoque=1)
                        AND (sgp.GeraVolume IS NULL OR sgp.GeraVolume=0) 
                        AND (p.GerarPedidoProducaoCorte IS NULL OR p.GerarPedidoProducaoCorte=0)")
                    .Add("?id", idLiberarPedido)
                    .Add("?tipoEntrega", Glass.Data.Model.Pedido.TipoEntregaPedido.Balcao)
                .Select(@"p.IdPedido, pp.IdProd, p.CodCliente as PedCli, pp.Peso, pp.Qtde, pp.TotM, prod.CodInterno,
                            prod.Descricao as DescrProduto, pp.Altura, pp.Largura, (sgp.TipoSubgrupo = ?tipoSubGrupo) as ChapaVidro")
                    .Add("?tipoSubGrupo", TipoSubgrupoProd.ChapasVidro)
                .GroupBy("pp.IdProdPed")
                .UnionAll(
                    SourceContext.Instance.CreateQuery()
                    .From<ProdutosPedido>("pp")
                    .InnerJoin<Glass.Data.Model.Pedido>("p.IdPedido=pp.IdPedido", "p")
                    .InnerJoin<ProdutosLiberarPedido>("p.IdPedidoRevenda = plp.IdPedido", "plp")
                    .InnerJoin<Produto>("pp.IdProd=prod.IdProd", "prod")
                    .LeftJoin<SubgrupoProd>("sgp.IdSubgrupoProd=prod.IdSubgrupoProd", "sgp")
                    .Select(@"p.IdPedido, pp.IdProd, p.CodCliente as PedCli, pp.Peso, pp.Qtde, pp.TotM, prod.CodInterno,
                            prod.Descricao as DescrProduto, pp.Altura, pp.Largura, (sgp.TipoSubgrupo = ?tipoSubGrupo) as ChapaVidro")
                    .Where(string.Format(@"plp.IdLiberarPedido = ?id
                            AND p.TipoEntrega=?tipoEntrega 
                            AND (pp.InvisivelFluxo IS NULL OR pp.InvisivelFluxo=0)
                            AND (sgp.GeraVolume IS NULL OR sgp.GeraVolume=0)
                            AND p.Situacao<>?situacao{0}", idPedido > 0 ? " AND p.IdPedidoRevenda=?idPedidoRevenda" : string.Empty))
                    .Add("?id", idLiberarPedido)
                    .Add("?idPedidoRevenda", idPedido)
                    .Add("?tipoSubGrupo", TipoSubgrupoProd.ChapasVidro)
                    .Add("?tipoEntrega", Glass.Data.Model.Pedido.TipoEntregaPedido.Balcao)
                    .Add("?situacao", Glass.Data.Model.Pedido.SituacaoPedido.Cancelado)
                    .GroupBy("pp.IdProdPed")
                );

            consultaRevenda.Execute();

            var consultaRevendaExp = SourceContext.Instance.CreateQuery()
                .From<ProdutoPedidoProducao>("ppp")
                    .InnerJoin<ProdutosPedido>("ppp.IdProdPed = pp.IdProdPedEsp", "pp")
                    .InnerJoin<Produto>("pp.IdProd = prod.IdProd", "prod")
                    .InnerJoin<ProdutosLiberarPedido>("ppp.IdPedidoExpedicao = plp.IdPedido", "plp")
                 .Where(@"plp.IdLiberarPedido = ?id
                        AND (pp.InvisivelFluxo IS NULL OR pp.InvisivelFluxo=0)
                        AND ppp.Situacao = ?situacaoPPP")
                     .Add("?id", idLiberarPedido)
                     .Add("?situacaoPPP", ProdutoPedidoProducao.SituacaoEnum.Producao)
                .Select(@"ppp.IdPedidoExpedicao as IdPedido, pp.IdProd, ppp.NumEtiqueta, ppp.IdProdPedProducao")
                .GroupBy("ppp.IdProdPedProducao");

            var consultaChapaExp = SourceContext.Instance.CreateQuery()
                .From<ExpedicaoChapa>("ec")
                    .InnerJoin<ProdutoImpressao>("ec.IdProdImpressaoChapa = pi.IdProdImpressao", "pi")
                    .InnerJoin<ProdutosNf>("pi.IdProdNf = pnf.IdProdNf", "pnf")
                    .InnerJoin<ProdutosLiberarPedido>("ec.IdPedido = plp.IdPedido", "plp")
                    .LeftJoin<Funcionario>("ec.IdFuncLeitura = func.IdFunc", "func")
                .Where(@"plp.IdLiberarPedido = ?id AND (pi.Cancelado IS NULL OR pi.Cancelado=0)")
                    .Add("?id", idLiberarPedido)
                .Select(@"ec.IdPedido, pnf.IdProd, ec.IdFuncLeitura, ec.DataLeitura, pi.NumEtiqueta,
                    func.Nome as NomeFuncLeitura, pi.IdProdImpressao as IdProdImpressaoChapa")
                .GroupBy("ec.IdExpChapa");

            var consultaRetalhoExp = SourceContext.Instance.CreateQuery()
               .From<ExpedicaoChapa>("ec")
                   .InnerJoin<ProdutoImpressao>("ec.IdProdImpressaoChapa = pi.IdProdImpressao", "pi")
                   .InnerJoin<RetalhoProducao>("pi.IdRetalhoProducao = r.IdRetalhoProducao", "r")
                   .InnerJoin<ProdutosLiberarPedido>("ec.IdPedido = plp.IdPedido", "plp")
                   .LeftJoin<Funcionario>("ec.IdFuncLeitura = func.IdFunc", "func")
               .Where(@"plp.IdLiberarPedido = ?id AND (pi.Cancelado IS NULL OR pi.Cancelado=0)")
                   .Add("?id", idLiberarPedido)
               .Select(@"ec.IdPedido, r.IdProd, ec.IdFuncLeitura, ec.DataLeitura, pi.NumEtiqueta,
                    func.Nome as NomeFuncLeitura, pi.IdProdImpressao as IdProdImpressaoChapa")
               .GroupBy("ec.IdExpChapa");

            var consultaVolume = SourceContext.Instance.CreateQuery()
                .From<Volume>("v")
                    .InnerJoin<Glass.Data.Model.Pedido>("p.IdPedido = v.IdPedido", "p")
                    .InnerJoin<ProdutosLiberarPedido>("p.IdPedido = plp.IdPedido", "plp")
                    .LeftJoin<Funcionario>("v.UsuSaidaExpedicao = func.IdFunc", "func")
                .Where(@"plp.IdLiberarPedido = ?id AND p.TipoEntrega = ?tipoEntrega")
                    .Add("?situacaoPPP", ProdutoPedidoProducao.SituacaoEnum.Producao)
                    .Add("?id", idLiberarPedido)
                    .Add("?tipoEntrega", Glass.Data.Model.Pedido.TipoEntregaPedido.Balcao)
                .Select(@"p.IdPedido, v.IdVolume, p.CodCliente as PedCli, v.UsuSaidaExpedicao as IdFuncLeitura,
                    v.DataSaidaExpedicao as DataLeitura, func.Nome as NomeFuncLeitura, v.DataFechamento")
                .GroupBy("v.IdVolume");

            // Cria consulta para recuperar os dados da leitura no setor entregue dos produtos de produção.
            var consultaLeituraProducaoVendaRevendaExp =
                SourceContext.Instance.CreateQuery()
                    .Select("lp.IdProdPedProducao, lp.IdFuncLeitura, lp.DataLeitura, func.Nome, ppp.TrocadoDevolvido")
                    .From<LeituraProducao>("lp")
                        .LeftJoin<Funcionario>("lp.IdFuncLeitura = func.IdFunc", "func")
                        .InnerJoin<Setor>("lp.IdSetor = s.IdSetor", "s")
                        .LeftJoin<ProdutoPedidoProducao>("lp.IdProdPedProducao=ppp.IdProdPedProducao", "ppp")
                        .Where("lp.IdProdPedProducao=?idProdPedProducao AND s.Tipo=?tipoSetor")
                            .Add("?tipoSetor", TipoSetor.Entregue);

            #endregion

            #region Filtro

            if (idPedido > 0)
            {
                consultaVenda.WhereClause.And("p.IdPedido = ?idPedido").Add("?idPedido", idPedido);
                consultaRevenda.WhereClause.And("p.IdPedido = ?idPedido").Add("?idPedido", idPedido);
                consultaRevendaExp.WhereClause.And("plp.IdPedido = ?idPedido").Add("?idPedido", idPedido);
                consultaVolume.WhereClause.And("p.IdPedido = ?idPedido").Add("?idPedido", idPedido);
            }

            #endregion

            #region Execução

            SourceContext.Instance.CreateMultiQuery()
                .Add<Entidades.ExpBalcao>(consultaLiberacao, (sender, query, result) =>
                {
                    retorno = result.FirstOrDefault();
                })
                .Add<Entidades.ItemExpBalcao>(consultaVenda, (sender, query, result) =>
                {
                    #region Recupera os dados de leitura do setor Entregue

                    var resultadoAtualizado = new List<Entidades.ItemExpBalcao>();

                    // Percorre cada resultado da consulta de venda e recupera os dados da leitura do setor Entregue.
                    foreach (var resultado in result)
                    {
                        var dadosLeitura =
                            consultaLeituraProducaoVendaRevendaExp
                                .Add("?idProdPedProducao", resultado.IdProdPedProducao)
                            .Execute()
                            .Select(f => new
                            {
                                IdProdPedProducao = f.GetInt32(0),
                                IdFuncLeitura = f.GetInt32(1),
                                DataLeitura = f.GetDateTime(2),
                                NomeFuncLeitura = f.GetString(3),
                                TrocadoDevolvido = f.GetBoolean(4)
                            }).ToList().FirstOrDefault();

                        if (dadosLeitura != null && dadosLeitura.IdProdPedProducao > 0)
                        {
                            resultado.IdFuncLeitura = dadosLeitura.IdFuncLeitura;
                            resultado.DataLeitura = dadosLeitura.DataLeitura;
                            resultado.NomeFuncLeitura = dadosLeitura.NomeFuncLeitura;
                            resultado.TrocadoDevolvido = dadosLeitura.TrocadoDevolvido;
                        }

                        // É necessário salvar os dados atualizados em uma lista separada, pois,
                        // ao salvá-los na variável result, eles não são atualizados.
                        resultadoAtualizado.Add(resultado);
                    }

                    #endregion

                    // Adiciona os resultados atualizados, com os dados de leitura, na variável ItensExp.
                    retorno.ItensExp.AddRange(resultadoAtualizado);
                })
                .Add<Entidades.ItemExpBalcao>(consultaVolume, (sender, query, result) =>
                {
                    retorno.ItensExp.AddRange(result);
                })
                .Add<ProdutosPedido>(consultaRevenda, (sender, query, result) =>
                {
                    itensRevenda.AddRange(result);
                })
                .Add<ItemRevendaExp>(consultaRevendaExp, (sender, query, result) =>
                {
                    #region Recupera os dados de leitura do setor Entregue

                    var resultadoAtualizado = new List<ItemRevendaExp>();

                    // Percorre cada resultado da consulta de revenda exp. e recupera os dados da leitura do setor Entregue.
                    foreach (var resultado in result)
                    {
                        var dadosLeitura =
                            consultaLeituraProducaoVendaRevendaExp
                                .Add("?idProdPedProducao", resultado.IdProdPedProducao)
                            .Execute()
                            .Select(f => new
                            {
                                IdProdPedProducao = f.GetInt32(0),
                                IdFuncLeitura = f.GetInt32(1),
                                DataLeitura = f.GetDateTime(2),
                                NomeFuncLeitura = f.GetString(3),
                                TrocadoDevolvido = f.GetBoolean(4)
                            }).ToList().FirstOrDefault();

                        if (dadosLeitura != null && dadosLeitura.IdProdPedProducao > 0)
                        {
                            resultado.IdFuncLeitura = dadosLeitura.IdFuncLeitura;
                            resultado.DataLeitura = dadosLeitura.DataLeitura;
                            resultado.NomeFuncLeitura = dadosLeitura.NomeFuncLeitura;
                            resultado.TrocadoDevolvido = dadosLeitura.TrocadoDevolvido;
                        }

                        // É necessário salvar os dados atualizados em uma lista separada, pois,
                        // ao salvá-los na variável result, eles não são atualizados.
                        resultadoAtualizado.Add(resultado);
                    }

                    #endregion

                    // Adiciona os resultados atualizados, com os dados de leitura, na variável itensRevendaExp.
                    itensRevendaExp.AddRange(resultadoAtualizado);
                })
                .Add<ItemRevendaExp>(consultaChapaExp, (sender, query, result) =>
                {
                    itensRevendaExp.AddRange(result);
                })
                .Add<ItemRevendaExp>(consultaRetalhoExp, (sender, query, result) =>
                {
                    itensRevendaExp.AddRange(result);
                })
                .Execute();

            foreach (var ir in itensRevenda)
            {
                for (int i = 0; i < ir.Qtde; i++)
                {
                    var novoItem = new Entidades.ItemExpBalcao()
                    {
                        IdPedido = (int)ir.IdPedido,
                        IdPedidoRevenda = ir.IdPedidoRevenda,
                        PedCli = ir.PedCli,
                        Peso = ir.Peso / ir.Qtde,
                        CodProduto = ir.CodInterno,
                        DescProduto = ir.DescrProduto,
                        Altura = ir.Altura,
                        Largura = ir.Largura,
                        M2 = ir.TotM / ir.Qtde
                    };

                    var itemExp = itensRevendaExp
                        .Where(f => (f.IdPedido == ir.IdPedido || f.IdPedido == ir.IdPedidoRevenda) && f.IdProd == ir.IdProd && !etqsUtilizadas.Contains(f.NumEtiqueta))
                        .FirstOrDefault();

                    if (itemExp != null)
                    {
                        etqsUtilizadas.Add(itemExp.NumEtiqueta);

                        novoItem.NumEtiqueta = itemExp.NumEtiqueta;
                        novoItem.IdFuncLeitura = itemExp.IdFuncLeitura;
                        novoItem.DataLeitura = itemExp.DataLeitura;
                        novoItem.NomeFuncLeitura = itemExp.NomeFuncLeitura;
                        novoItem.IdProdPedProducao = itemExp.IdProdPedProducao;
                        novoItem.IdProdImpressaoChapa = itemExp.IdProdImpressaoChapa;
                    }

                    retorno.ItensExp.Add(novoItem);
                }
            }

            #endregion

            if (!string.IsNullOrEmpty(visualizar) && !visualizar.Contains("1,2"))
            {
                var expedido = ("," + (visualizar) + ",").Contains(",1,");
                retorno.ItensExp = retorno.ItensExp.Where(f => f.Expedido == expedido).ToList();
            }

            return retorno;
        }

        /// <summary>
        /// Realiza a leitura da expedição
        /// </summary>
        public void EfetuaLeitura(int idFunc, int idLiberarPedido, string numEtiqueta, int? idPedidoExp)
        {
            //Verifica se a etiqueta foi informada
            if (string.IsNullOrEmpty(numEtiqueta))
                throw new Exception("Informe a etiqueta.");

            //Verifica se a liberação pode ser expedida.
            Glass.Data.DAL.LiberarPedidoDAO.Instance.ValidaLiberacaoParaExpedicaoBalcao((uint)idLiberarPedido);

            #region Multiplas Leituras com "P"

            if (numEtiqueta.ToUpper().Substring(0, 1).Equals("P"))
            {
                ProdutoPedidoProducaoDAO.Instance.ValidaEtiquetaProducao(null, ref numEtiqueta);

                var etiquetas = ProdutoPedidoProducaoDAO.Instance.GetEtiquetasByPedido(null, numEtiqueta.Substring(1).StrParaUint());

                #region Salva na tabela de controle

                var idLeitura = LeituraEtiquetaPedidoPlanoCorteDAO.Instance.Insert(new LeituraEtiquetaPedidoPlanoCorte()
                {
                    NumEtiquetaLida = numEtiqueta
                });

                foreach (var e in etiquetas)
                {
                    EtiquetaLidaPedidoPlanoCorteDAO.Instance.Insert(new EtiquetaLidaPedidoPlanoCorte()
                    {
                        IdLeituraEtiquetaPedPlanoCorte = idLeitura,
                        NumEtiquetaReal = e
                    });
                }

                #endregion

                var erroEtq = new List<string>();

                foreach (string e in etiquetas)
                {
                    try
                    {
                        EfetuaLeitura(idFunc, idLiberarPedido, e, idPedidoExp);
                    }
                    catch 
                    {
                        erroEtq.Add(e);
                    }
                }

                if (erroEtq.Count > 0)
                {
                    var erros = string.Join(",", erroEtq.ToArray());

                    ErroDAO.Instance.InserirFromException("Leitura com P", new Exception("Etiqueta: " + numEtiqueta + " Leituras: " + erros));
                    throw new Exception("Algumas leituras não foram efetuadas. Etiquetas: " + erros);
                }

                return;
            }

            #endregion
            
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    //Verifica se a etiqueta é de chapa ou produção
                    if (numEtiqueta.ToUpper().Substring(0, 1).Equals("N") || numEtiqueta.ToUpper().Substring(0, 1).Equals("R"))
                        ValidaLeituraChapaRetalho(transaction, idLiberarPedido, numEtiqueta, idPedidoExp);
                    else if (!numEtiqueta.ToUpper().Substring(0, 1).Equals("V"))
                        ValidaLeituraPeca(transaction, idLiberarPedido, numEtiqueta, idPedidoExp);

                    //Realiza a leitura
                    #region Volume

                    //Verifica se é etiqueta de volume
                    if (numEtiqueta.ToUpper().Substring(0, 1).Equals("V"))
                    {
                        ServiceLocator.Current.GetInstance<Glass.PCP.Negocios.IVolumeFluxo>()
                            .MarcaExpedicaoVolume(transaction, numEtiqueta, idLiberarPedido, true);
                    }

                    #endregion

                    #region NF-e / Retalho

                    else if (numEtiqueta.ToUpper().Substring(0, 1).Equals("N") || numEtiqueta.ToUpper().Substring(0, 1).Equals("R"))
                    {
                        var tipoEtiqueta = ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(numEtiqueta);

                        var idRetalho = tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.Retalho ?
                            Glass.Conversoes.StrParaUint(numEtiqueta.Substring(1, numEtiqueta.IndexOf('-') - 1)) : 0;

                        var idLoja = Glass.Data.DAL.PedidoDAO.Instance.ObtemIdLoja(transaction, (uint)idPedidoExp.Value);
                        var idProd = tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal ?
                            ProdutosNfDAO.Instance.GetIdProdByEtiqueta(transaction, numEtiqueta) : RetalhoProducaoDAO.Instance.ObtemIdProd(transaction, idRetalho);

                        if (idPedidoExp.GetValueOrDefault() == 0)
                            throw new Exception("Pedido não encontrado.");

                        var prodsPed = Glass.Data.DAL.ProdutosPedidoDAO.Instance.GetByPedidoProdutoForExpCarregamento(transaction, (uint)idPedidoExp.Value, idProd, true);

                        // Agrupa os produtos do pedido pelo produto, buscando apenas aqueles que faltam dar saída
                        var prodPed = MetodosExtensao.Agrupar(prodsPed.Where(f => f.Qtde > f.QtdSaida), new string[] {"IdProd"}, new string[] {"Qtde"}).FirstOrDefault(f => f.IdProd == idProd);
                        var idProdImpressaoChapa = Glass.Data.DAL.ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(transaction, numEtiqueta, tipoEtiqueta);

                        if (prodPed == null)
                        {
                            if (prodsPed.Count() == 0)
                                throw new Exception("Todas as peças deste pedido já foram expedidas.");
                            else
                                throw new Exception("Produto não encontrado no pedido ou já expedido.");
                        }

                        //Dados que serão feitos a baixa no estoque
                        var dados = new List<DetalhesBaixaEstoque>()
                        {
                            new DetalhesBaixaEstoque()
                            {
                                IdProdPed = (int)prodPed.IdProdPed,
                                DescricaoBaixa = prodPed.DescrProduto,
                                Qtde = 1
                            }
                        };

                        //Faz o vinculo da chapa no corte para que a mesma não possa ser usada novamente
                        Glass.Data.DAL.ChapaCortePecaDAO.Instance.Inserir(transaction, numEtiqueta, null, false, true);

                        //Realiza a exp da chapa
                        Glass.Data.DAL.ExpedicaoChapaDAO.Instance.Insert(transaction, new Glass.Data.Model.ExpedicaoChapa()
                            {
                                IdProdImpressaoChapa = (int)idProdImpressaoChapa,
                                IdPedido = idPedidoExp.Value,
                                SaidaBalcao = true,
                                DataLeitura = DateTime.Now,
                                IdFuncLeitura = idFunc
                            });

                        //Marca no produto_impressão o id do pedido de expedição
                        Glass.Data.DAL.ProdutoImpressaoDAO.Instance.AtualizaPedidoExpedicao(transaction, (uint)idPedidoExp, idProdImpressaoChapa);

                        //Atualiza a chapa_trocada_devolvida marcando a mesma como utilizada
                        ChapaTrocadaDevolvidaDAO.Instance.MarcarChapaComoUtilizada(transaction, numEtiqueta);

                        //Baixa o estoque
                        ServiceLocator.Current.GetInstance<Glass.Estoque.Negocios.IProvedorBaixaEstoque>()
                            .BaixarEstoque(transaction, idLoja, dados, null, idProdImpressaoChapa, false);

                        //Marca o retalho como vendido
                        if (tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.Retalho)
                            RetalhoProducaoDAO.Instance.AlteraSituacao(transaction, idRetalho, Glass.Data.Model.SituacaoRetalhoProducao.Vendido);

                        //Atualiza a situação do pedido
                        PedidoDAO.Instance.AtualizaSituacaoProducao(transaction, (uint)idPedidoExp, null, DateTime.Now);
                    }

                    #endregion

                    #region Pedido

                    else
                    {
                        var idSetorCarregamento = SetorDAO.Instance.ObtemIdSetorExpCarregamento(transaction);
                        var idsSetorEntregue = SetorDAO.Instance.ObterIdsSetorTipoEntregue(transaction);
                        var idSetorLeitura = idSetorCarregamento > 0 ? (int)idSetorCarregamento : idsSetorEntregue?.Any(f => f > 0) ?? false ? idsSetorEntregue.FirstOrDefault(f => f > 0) : 0;

                        if (idSetorLeitura == 0)
                        {
                            throw new Exception("Não foi possível recuperar o setor de expedição de carregamento.");
                        }

                        ProdutoPedidoProducaoDAO.Instance.AtualizaSituacao(transaction, (uint)idFunc, null, numEtiqueta, (uint)idSetorLeitura, false, false, null, null, null, (uint?)idPedidoExp, 0,
                            null, null, false, null, false, 0);
                    }

                    #endregion
                    
                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException("Expedição Balcão - Lib: " + idLiberarPedido + " - Etq:" + numEtiqueta, ex);
                    throw ex;
                }
            }
        }

        private static readonly object _estornarLiberacaoLock = new object();

        /// <summary>
        /// Estorna itens de uma expedição de liberação
        /// </summary>
        public Colosoft.Business.SaveResult EstornaLiberacao(Dictionary<int, int> itens)
        {
            lock(_estornarLiberacaoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        foreach (var item in itens)
                        {
                            #region Volume

                            if (item.Value == 3)
                            {
                                ServiceLocator.Current.GetInstance<Glass.PCP.Negocios.IVolumeFluxo>().EstornaExpedicaoVolume(transaction, (uint)item.Key);
                            }

                            #endregion

                            #region NF-e / Retalho

                            else if (item.Value == 2)
                            {
                                var prodImpressao = ProdutoImpressaoDAO.Instance.GetElementByPrimaryKey(transaction, item.Key);
                                var idPedidoExp = Glass.Data.DAL.ExpedicaoChapaDAO.Instance.ObtemPedidoExpedicao(transaction, (uint)item.Key);
                                var idLoja = Glass.Data.DAL.PedidoDAO.Instance.ObtemIdLoja(transaction, (uint)idPedidoExp);
                                var tipoEtiqueta = ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(prodImpressao.NumEtiqueta);
                                var idProd = tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal ?
                                    ProdutosNfDAO.Instance.GetIdProdByEtiqueta(transaction, prodImpressao.NumEtiqueta) : RetalhoProducaoDAO.Instance.ObtemIdProd(transaction, (uint)prodImpressao.IdRetalhoProducao);

                                if (idPedidoExp == 0)
                                    throw new Exception("Pedido não encontrado.");

                                var prodsPed = Glass.Data.DAL.ProdutosPedidoDAO.Instance.GetByPedidoProdutoForExpCarregamento(transaction, (uint)idPedidoExp, idProd, true);
                                var prodPed = Glass.MetodosExtensao.Agrupar(prodsPed, new string[] { "IdProd" }, new string[] { "Qtde" }).Where(f => f.IdProd == idProd).FirstOrDefault();
                                var idRetalho = tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.Retalho ?
                                    Glass.Conversoes.StrParaUint(prodImpressao.NumEtiqueta.Substring(1, prodImpressao.NumEtiqueta.IndexOf('-') - 1)) : 0;

                                if (prodPed == null)
                                    return new Colosoft.Business.SaveResult(false, "Produto não encontrado.".GetFormatter());

                                //Dados que serão feitos a baixa no estoque
                                var dados = new List<DetalhesBaixaEstoque>()
                                {
                                    new DetalhesBaixaEstoque()
                                    {
                                        IdProdPed = (int)prodPed.IdProdPed,
                                        DescricaoBaixa = prodPed.DescrProduto,
                                        Qtde = 1
                                    }
                                };

                                //Remove o vinculo da chapa no corte para que a mesma possa ser usada novamente
                                Glass.Data.DAL.ChapaCortePecaDAO.Instance.DeleteByIdProdImpressaoChapa(transaction, (uint)item.Key);

                                //Marca a chapa novamente como disponivel
                                ChapaTrocadaDevolvidaDAO.Instance.MarcarChapaComoDisponivel(transaction, ProdutoImpressaoDAO.Instance.ObtemNumEtiqueta((uint)item.Key));

                                //Remove a exp da chapa
                                Glass.Data.DAL.ExpedicaoChapaDAO.Instance.DeleteByIdProdImpressaoChapa(transaction, (uint)item.Key);

                                //Remove o pedido de expedicao
                                Glass.Data.DAL.ProdutoImpressaoDAO.Instance.AtualizaPedidoExpedicao(transaction, null, (uint)item.Key);

                                //Credita o estoque
                                ServiceLocator.Current.GetInstance<Glass.Estoque.Negocios.IProvedorBaixaEstoque>()
                                    .EstornaBaixaEstoque(transaction, idLoja, dados, null, (uint)item.Key);

                                //Marca o retalho como disponivel
                                if (tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.Retalho)
                                    RetalhoProducaoDAO.Instance.AlteraSituacao(transaction, idRetalho, Glass.Data.Model.SituacaoRetalhoProducao.Disponivel);

                                //Atualiza a situação do pedido
                                PedidoDAO.Instance.AtualizaSituacaoProducao(transaction, (uint)idPedidoExp, null, DateTime.Now);
                            }

                            #endregion

                            #region Pedido

                            else if (item.Value == 1)
                            {
                                Glass.Data.DAL.ProdutoPedidoProducaoDAO.Instance.VoltarPeca(transaction, (uint)item.Key, null, true);
                            }

                            #endregion
                        }

                        transaction.Commit();
                        transaction.Close();

                        return new Colosoft.Business.SaveResult(true, null);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        return new Colosoft.Business.SaveResult(false, ex.GetFormatter());
                    }
                }
            }
        }

        #region Validações

        private void ValidaLeituraChapaRetalho(GDASession session, int idLiberarPedido, string numEtiqueta, int? idPedidoExp)
        {
            var prodImpressao = Glass.Data.DAL.ProdutoImpressaoDAO.Instance.ObtemProdImpressaoParaExpedicao(session, numEtiqueta);
            var tipoEtiqueta = ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(numEtiqueta);
            var chapaTrocadaDisponivel = ChapaTrocadaDevolvidaDAO.Instance.VerificarChapaDisponivel(session, numEtiqueta);

            uint idRetalho = tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.Retalho ?
               Glass.Conversoes.StrParaUint(numEtiqueta.Substring(1, numEtiqueta.IndexOf('-') - 1)) : 0;

            if (tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.Retalho &&
                RetalhoProducaoDAO.Instance.ObtemSituacao(session, idRetalho) != Glass.Data.Model.SituacaoRetalhoProducao.Disponivel)
                throw new Exception("O retalho informado não esta disponivel para uso.");

            if (prodImpressao == null)
                throw new Exception("A peça da etiqueta informada não foi encontrada ou está cancelada.");

            if (idPedidoExp.GetValueOrDefault(0) == 0)
                throw new Exception("Indique o pedido de venda/revenda que contém esse produto.");
            else if (!Glass.Data.DAL.PedidoDAO.Instance.IsVenda(session, (uint)idPedidoExp.Value))
                throw new Exception("Apenas pedidos de venda/revenda podem ser utilizados como pedido novo.");

            if (!Glass.Data.DAL.LiberarPedidoDAO.Instance.VerificaPedidoLiberacao(session, idLiberarPedido, idPedidoExp.Value))
                throw new Exception("O pedido informado não faz parte desta liberação.");

            if(Glass.Data.DAL.ExpedicaoChapaDAO.Instance.VerificaLeitura(session, numEtiqueta) && !chapaTrocadaDisponivel)
                throw new Exception("A etiqueta informada já deu saída na expedição.");

            //Verifica se a etiqueta ja foi expedida
            if (ProdutoImpressaoDAO.Instance.EstaExpedida(session, prodImpressao.IdProdImpressao) && !chapaTrocadaDisponivel)
                throw new Exception("Esta etiqueta ja foi expedida no sistema.");

            if (Glass.Data.DAL.ChapaCortePecaDAO.Instance.ChapaPossuiLeitura(session, prodImpressao.IdProdImpressao))
                throw new Exception("Esta etiqueta já foi utilizada no setor de corte.");

            bool encontrado = false;

            if (idPedidoExp.GetValueOrDefault() == 0)
                throw new Exception("Pedido não encontrado.");

            var prodPed = Glass.Data.DAL.ProdutosPedidoDAO.Instance.GetByPedido(session, (uint)idPedidoExp.Value);
            prodPed = Glass.MetodosExtensao.ToArray(Glass.MetodosExtensao.Agrupar(prodPed, new string[] { "IdProd" }, new string[] { "Qtde" }));
            var mensagemRetorno = new List<string>();
            
            // Percorre os produtos do pedido de expedição que possuem o mesmo ID do produto a ser expedido.
            foreach (var p in prodPed.Where(f => f.IdProd == prodImpressao.IdProd).ToList())
            {
                if (p.Altura == prodImpressao.Altura && p.Largura == prodImpressao.Largura)
                {
                    if ((p.Qtde - Glass.Data.DAL.ExpedicaoChapaDAO.Instance.ObtemQuantidadeExpedida(session, idPedidoExp.Value, (int)prodImpressao.IdProd)) > 0)
                    {
                        encontrado = true;
                        break;
                    }
                    else
                        mensagemRetorno.Add("Possivelmente, este produto já foi expedido.");
                }
                else
                    mensagemRetorno.Add("Possivelmente, as medidas do produto no pedido estão diferentes das medidas do produto na nota.");
            }

            if (!encontrado)
                throw new Exception(string.Format("Produto não encontrado. {0}", string.Join(" ", mensagemRetorno)));

            //Verifica se a chapa foi marcada perda.
            if (tipoEtiqueta != ProdutoImpressaoDAO.TipoEtiqueta.Retalho && Glass.Data.DAL.PerdaChapaVidroDAO.Instance.IsPerda(session, numEtiqueta))
                throw new Exception("A etiqueta esta marcada como perdida.");
        }

        private void ValidaLeituraPeca(GDASession session, int idLiberarPedido, string numEtiqueta, int? idPedidoExp)
        {
            //Valida a etiqueta
            ProdutoPedidoProducaoDAO.Instance.ValidaEtiquetaProducao(session, ref numEtiqueta);

            var idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(session, numEtiqueta).GetValueOrDefault();

            //Verifica se a peça existe
            if (idProdPedProducao == 0)
                throw new Exception("A peça da etiqueta informada não foi encontrada.");

            //Verifica se a peça é da liberação informada
            var idPedido = Glass.Conversoes.StrParaInt(numEtiqueta.Split('-')[0]);
            if (!LiberarPedidoDAO.Instance.VerificaPedidoLiberacao(session, idLiberarPedido, idPedidoExp ?? idPedido))
                throw new Exception("A peça ou pedido informado não faz parte desta liberação.");

            //Verifica se a peça está pronta
            if (PCPConfig.BloquearExpedicaoApenasPecasProntas &&
                !ProdutoPedidoProducaoDAO.Instance.PecaEstaPronta(session, numEtiqueta))
                throw new Exception("A peça informada ainda não está pronta.");

            var pecaComposicao = ProdutoPedidoProducaoDAO.Instance.VerificarProdutoPedidoProducaoPossuiPai(session, (int)idProdPedProducao);

            /* Chamado 49082. */
            if (pecaComposicao)
                throw new Exception("Não é possível ler peças de composição no setor Entregue.");
        }

        #endregion

        #region Membros de IProvedorExpBalcao

        /// <summary>
        /// Armazena os dados para a expedição balcão
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        string Entidades.IProvedorExpBalcao.ObtemDescricaoSetoresRestantes(int idProdPedProducao)
        {
            var setores = Glass.Data.DAL.SetorDAO.Instance.ObtemSetoresRestantes((uint)idProdPedProducao, 0);
            return String.Join(", ", setores.OrderBy(x => x.NumeroSequencia).Select(x => x.Descricao).ToArray());
        }

        #endregion

        #endregion

        #region Exp. Carregamento

        #endregion
    }
}
