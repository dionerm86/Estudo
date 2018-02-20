using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Glass.Data.Helper;
using System.Web;
using GDA;
using Glass.Configuracoes;
using Glass.Data.Model;
using Glass.Estoque.Negocios.Entidades;
using Glass;

namespace WebGlass.Business.OrdemCarga.Fluxo
{
    public class CarregamentoFluxo : BaseFluxo<CarregamentoFluxo>
    {
        private CarregamentoFluxo() { }

        #region Recuperação de Itens

        /// <summary>
        /// Recupera os ids das OCs para gerar o carregamento.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="nomeCliente"></param>
        /// <param name="idLoja"></param>
        /// <param name="idRota"></param>
        /// <param name="idsOCs"></param>
        /// <returns></returns>
        public string GetIdsOCsParaCarregar(uint idCliente, string nomeCliente, string idLoja, string idRota, string idsOCs)
        {
            try
            {
                return OrdemCargaDAO.Instance.GetIdsOCsParaCarregar(idCliente, nomeCliente, idLoja, idRota, idsOCs);
            }
            catch
            {
                return idsOCs;
            }
        }

        /// <summary>
        /// Recupera os ids das OCs para gerar carregamento
        /// </summary>
        /// <param name="idRota"></param>
        /// <param name="idLoja"></param>
        /// <param name="dtEntPedidoIni"></param>
        /// <param name="dtEntPedidoFin"></param>
        /// <returns></returns>
        public string GetIdsOCsForGerarCarregamento(string idsRotas, string idLoja, string dtEntPedidoIni, string dtEntPedidoFin)
        {
            uint loja = Glass.Conversoes.StrParaUint(idLoja);

            return OrdemCargaDAO.Instance.GetIdsOCsForGerarCarregamento(idsRotas, loja, dtEntPedidoIni, dtEntPedidoFin);
        }

        /// <summary>
        /// Recupera uma lista de ocs para o carregamento
        /// </summary>
        /// <param name="idsOCs"></param>
        /// <returns></returns>
        public Entidade.OrdemCarga[] GetOCsForCarregamento(string idsOCs)
        {
            var ocs = OrdemCargaDAO.Instance.GetOCsForCarregamento(idsOCs);

            if (ocs == null || ocs.Count() == 0)
                return null;

            List<Entidade.OrdemCarga> lstOCs = new List<Entidade.OrdemCarga>();

            foreach (var idCliente in ocs.Select(o => o.IdCliente).Distinct())
            {
                lstOCs.Add(new WebGlass.Business.OrdemCarga.Entidade
                    .OrdemCarga(new List<Glass.Data.Model.OrdemCarga>(ocs.Where(o => o.IdCliente == idCliente))));
            }

            return lstOCs.ToArray();
        }

        /// <summary>
        /// Busca os itens para expedição do carregamento
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="idOC"></param>
        /// <param name="idPedido"></param>
        /// <param name="idCliente"></param>
        /// <param name="nomeCliente"></param>
        /// <param name="filtro"></param>
        /// <param name="volume"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<Glass.Data.Model.ItemCarregamento> GetItensCarregamentoForExpedicao(uint idCarregamento, uint idOC, uint idPedido, uint idCliente,
            string nomeCliente, string filtro, bool volume, string numEtiqueta, int altura, int largura, uint idCliExterno, string nomeCliExterno, uint idPedidoExterno,
            string sortExpression, int startRow, int pageSize)
        {
            if (idCarregamento == 0)
                return new List<Glass.Data.Model.ItemCarregamento>();

            return ItemCarregamentoDAO.Instance.GetItensCarregamentoForExpedicao(idCarregamento, idOC, idPedido, idCliente, nomeCliente,
                filtro, volume, numEtiqueta, altura, largura, idCliExterno, nomeCliExterno, idPedidoExterno, sortExpression, startRow, pageSize);
        }

        public int GetItensCarregamentoForExpedicaoCount(uint idCarregamento, uint idOC, uint idPedido, uint idCliente,
            string nomeCliente, string filtro, bool volume, string numEtiqueta, int altura, int largura, uint idCliExterno, string nomeCliExterno, uint idPedidoExterno)
        {
            if (idCarregamento == 0)
                return 0;

            return ItemCarregamentoDAO.Instance.GetItensCarregamentoForExpedicaoCount(idCarregamento, idOC, idPedido, idCliente, nomeCliente,
                filtro, volume, numEtiqueta, altura, largura, idCliExterno, nomeCliExterno, idPedidoExterno);
        }

        /// <summary>
        /// Busca as informações do carregamento (qtde peças, pendentes e etc.)
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="idOC"></param>
        /// <param name="idPedido"></param>
        /// <param name="idCliente"></param>
        /// <param name="nomeCliente"></param>
        /// <returns></returns>
        public Entidade.InfoCarregamento GetInfoCarregamentoForExpedicao(uint idCarregamento, uint idOC, uint idPedido, uint idCliente,
            string nomeCliente, string numEtiqueta, int altura, int largura, uint idCliExterno, string nomeCliExterno, uint idPedidoExterno)
        {
            if (idCarregamento == 0)
                return new Entidade.InfoCarregamento();

            var itens = ItemCarregamentoDAO.Instance.GetDadosCarregamentoForExpedicao(idCarregamento, idOC, idPedido, idCliente, nomeCliente,
                false, numEtiqueta, altura, largura, idCliExterno, nomeCliExterno, idPedidoExterno);
            var volumes = ItemCarregamentoDAO.Instance.GetDadosCarregamentoForExpedicao(idCarregamento, idOC, idPedido, idCliente, nomeCliente,
                true, numEtiqueta, altura, largura, idCliExterno, nomeCliExterno, idPedidoExterno);

            return new Entidade.InfoCarregamento(itens, volumes);
        }

        /// <summary>
        /// Recupera um carregamento
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <returns></returns>
        public Glass.Data.Model.Carregamento GetElement(uint idCarregamento)
        {
            return CarregamentoDAO.Instance.GetElement(idCarregamento);
        }

        #endregion

        #region Recupera uma listagem dos carregamentos criados

        /// <summary>
        /// Recupera uma lista de carregamentos
        /// </summary>
        public IList<Glass.Data.Model.Carregamento> GetListWithExpression(uint idCarregamento, uint idOC, uint idPedido, int idRota, uint idMotorista, string placa,
            string situacao, string dtSaidaIni, string dtSaidaFim, uint idLoja, string sortExpression, int startRow, int pageSize)
        {
            return CarregamentoDAO.Instance.GetListWithExpression(idCarregamento, idOC, idPedido, idRota, idMotorista, placa, situacao,
            dtSaidaIni, dtSaidaFim, idLoja, sortExpression, startRow, pageSize);
        }

        public int GetListWithExpressionCount(uint idCarregamento, uint idOC, uint idPedido, int idRota, uint idMotorista, string placa, string situacao,
            string dtSaidaIni, string dtSaidaFim, uint idLoja)
        {
            return CarregamentoDAO.Instance.GetListWithExpressionCount(idCarregamento, idOC, idPedido, idRota, idMotorista, placa, situacao,
                dtSaidaIni, dtSaidaFim, idLoja);
        }

        /// <summary>
        /// Recupera uma lista de carregamentos para o relatório
        /// </summary>
        public IList<Glass.Data.Model.Carregamento> GetListForRpt(uint idCarregamento, uint idOC, uint idPedido, int idRota, uint idMotorista, string placa,
            string situacao, string dtSaidaIni, string dtSaidaFim, uint idLoja)
        {
            return CarregamentoDAO.Instance.GetListForRpt(idCarregamento, idOC, idPedido, idRota, idMotorista, placa, situacao,
            dtSaidaIni, dtSaidaFim, idLoja);
        }

        #endregion

        #region Leitura do Carregamento

        /// <summary>
        /// Efetua a leitura de um item do carregamento
        /// </summary>
        public void EfetuaLeitura(uint idFunc, uint idCarregamento, string etiqueta, uint? idPedidoExp, uint? idCliente, string nomeCli,
            int? idOc, int? idPedidoFiltro, decimal? altura, decimal? largura, string numEtqFiltro, uint idClienteExterno, string nomeClienteExterno, uint idPedidoExterno)
        {
            uint idVolume = 0;
            uint idProdImpressaoChapa = 0;
            uint idLoja = 0;
            uint idLojaTransferencia = 0;
            List<DetalhesBaixaEstoque> dados = null;

            #region Multiplas Leituras com "P"

            // Verifica se a etiqueta é uma etiqueta de pedido
            if (etiqueta.ToUpper().Substring(0, 1).Equals("P"))
            {
                ProdutoPedidoProducaoDAO.Instance.ValidaEtiquetaProducao(null, ref etiqueta);

                var etiquetas = ProdutoPedidoProducaoDAO.Instance.GetEtiquetasByPedido(null, etiqueta.Substring(1).StrParaUint());

                #region Salva na tabela de controle

                var idLeitura = LeituraEtiquetaPedidoPlanoCorteDAO.Instance.Insert(new LeituraEtiquetaPedidoPlanoCorte()
                {
                    NumEtiquetaLida = etiqueta
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
                        EfetuaLeitura(idFunc, idCarregamento, e, null, idCliente, nomeCli, idOc, idPedidoFiltro, altura, largura, numEtqFiltro, idPedidoExterno, nomeClienteExterno, idPedidoExterno);
                    }
                    catch
                    {
                        erroEtq.Add(e);
                    }
                }

                if (erroEtq.Count > 0)
                {
                    var erros = string.Join(",", erroEtq.ToArray());
                    ErroDAO.Instance.InserirFromException("Leitura com P", new Exception("Etiqueta: " + etiqueta + " Leituras: " + erros));
                }

                return;
            }

            #endregion
            
            using (var trans = new GDATransaction())
            {
                try
                {
                    trans.BeginTransaction();

                    //Valida os filtros
                    ValidaFiltros(trans, idCliente.GetValueOrDefault(), nomeCli, etiqueta, idPedidoExp, idOc, idPedidoFiltro, altura, largura, numEtqFiltro, idClienteExterno, nomeClienteExterno, idPedidoExterno);

                    //Faz a validação da leitura
                    ValidaLeitura(trans, idCarregamento, etiqueta, idPedidoExp);

                    #region Volume

                    //Realiza a leitura
                    //Verifica se é etiqueta de volume
                    if (etiqueta.ToUpper().Substring(0, 1).Equals("V"))
                    {
                        idVolume = Glass.Conversoes.StrParaUint(etiqueta.Substring(1));
                        var idPedido = VolumeDAO.Instance.GetIdPedido(trans, idVolume);
                        idLoja = PedidoDAO.Instance.ObtemIdLoja(trans, idPedido);
                        var produtos = VolumeProdutosPedidoDAO.Instance.GetList(trans, idVolume.ToString());
                        var transferencia = OrdemCargaDAO.Instance.TemTransferencia(trans, idCarregamento, idPedido);

                        dados = produtos.Select(prod => new DetalhesBaixaEstoque()
                        {
                            IdProdPed = (int)prod.IdProdPed,
                            Qtde = prod.Qtde,
                            DescricaoBaixa = prod.DescProd
                        }).ToList();

                        //Se for transferencia usa a loja de quem esta fazendo a leitura.
                        if (transferencia)
                            idLoja = UserInfo.GetUserInfo.IdLoja;

                        //Baixa o estoque
                        Pedido.Fluxo.AlterarEstoque.Instance.BaixarEstoque(trans, idLoja, dados, idVolume, null, false);

                        //Se for transferencia credita o estoque para a loja que for transferir
                        if (transferencia)
                        {
                            idLojaTransferencia = PedidoDAO.Instance.ObtemIdLoja(trans, idPedido);
                            Pedido.Fluxo.AlterarEstoque.Instance.CreditaEstoque(trans, idLojaTransferencia, dados);
                        }
                    }

                    #endregion

                    #region NF-e / Retalho

                    else if (etiqueta.ToUpper().Substring(0, 1).Equals("N") || etiqueta.ToUpper().Substring(0, 1).Equals("R"))
                    {
                        var tipoEtiqueta = ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(etiqueta);

                        var idRetalho = tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.Retalho ?
                            Glass.Conversoes.StrParaUint(etiqueta.Substring(1, etiqueta.IndexOf('-') - 1)) : 0;

                        idLoja = PedidoDAO.Instance.ObtemIdLoja(trans, idPedidoExp.Value);
                        var transferencia = OrdemCargaDAO.Instance.TemTransferencia(trans, idCarregamento, idPedidoExp.Value);
                        var idProd = tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal ?
                            ProdutosNfDAO.Instance.GetIdProdByEtiqueta(trans, etiqueta) : RetalhoProducaoDAO.Instance.ObtemIdProd(trans, idRetalho);

                        if (idPedidoExp.GetValueOrDefault() == 0)
                            throw new Exception("Pedido não encontrado.");

                        var prodsPed = ProdutosPedidoDAO.Instance.GetByPedidoProdutoForExpCarregamento(trans, idPedidoExp.Value, idProd, true);

                        if (prodsPed == null)
                            throw new Exception("Produto não encontrado.");

                        var prodPed = Glass.MetodosExtensao.Agrupar(prodsPed.Where(f => f.Qtde > f.QtdSaida), new string[] { "IdProd" }, new string[] { "Qtde" }).Where(f => f.IdProd == idProd).FirstOrDefault();
                        idProdImpressaoChapa = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(trans, etiqueta, tipoEtiqueta);

                        if (prodPed == null)
                            throw new Exception("Produto não encontrado.");

                        //Dados que seram feitos a baixa no estoque
                        dados = new List<DetalhesBaixaEstoque>()
                        {
                            new DetalhesBaixaEstoque()
                            {
                                IdProdPed = (int)prodPed.IdProdPed,
                                DescricaoBaixa = prodPed.DescrProduto,
                                Qtde = 1
                            }
                        };

                        //Se for transferencia usa a loja de quem esta fazendo a leitura.
                        if (transferencia)
                            idLoja = UserInfo.GetUserInfo.IdLoja;

                        //Baixa o estoque
                        Pedido.Fluxo.AlterarEstoque.Instance.BaixarEstoque(trans, idLoja, dados, null, idProdImpressaoChapa, false);

                        //Se for transferencia credita o estoque para a loja que for transferir
                        if (transferencia)
                        {
                            idLojaTransferencia = PedidoDAO.Instance.ObtemIdLoja(trans, idPedidoExp.Value);
                            Pedido.Fluxo.AlterarEstoque.Instance.CreditaEstoque(trans, idLojaTransferencia, dados);
                        }

                        //Faz o vinculo da chapa no corte para que a mesma não possa ser usada novamente
                        ChapaCortePecaDAO.Instance.Inserir(trans, etiqueta, null, false, true);

                        //Marca no produto_impressão o id do pedido de expedição
                        ProdutoImpressaoDAO.Instance.AtualizaPedidoExpedicao(trans, idPedidoExp, idProdImpressaoChapa);

                        //Faz o vinculo do produto com o item do carregamento
                        Glass.Data.DAL.ItemCarregamentoDAO.Instance.AtualizaItemRevenda(trans, idCarregamento, idPedidoExp.Value, etiqueta, true);

                        PedidoDAO.Instance.AtualizaSituacaoProducao(trans, idPedidoExp.Value, SituacaoProdutoProducao.Entregue, DateTime.Now);

                        //Marca o retalho como vendido
                        if (tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.Retalho)
                            RetalhoProducaoDAO.Instance.AlteraSituacao(trans, idRetalho, Glass.Data.Model.SituacaoRetalhoProducao.Vendido);
                    }

                    #endregion

                    #region Pedido

                    else
                    {
                        var idSetorCarregamento = SetorDAO.Instance.ObtemIdSetorExpCarregamento(trans);

                        try
                        {
                            //Atualiza a situação da peça na produção
                            ProdutoPedidoProducaoDAO.Instance.AtualizaSituacao(trans, idFunc, null, etiqueta, idSetorCarregamento, false, false, null, null, null, idPedidoExp, 0, null, idCarregamento, false, null, false, 0);

                            //Se for box faz o vinculo do pedido de expedição.
                            if (idPedidoExp.HasValue)
                                ItemCarregamentoDAO.Instance.AtualizaItemRevenda(trans, idCarregamento, idPedidoExp.Value, etiqueta, false);
                        }
                        catch (Exception ex1)
                        {
                            //Chamado 68156
                            if (ex1.Message == "Esta peça já entrou neste setor.")
                            {
                                var idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(trans, etiqueta);
                                var leitura = LeituraProducaoDAO.Instance.GetByProdPedProducao(trans, idProdPedProducao.GetValueOrDefault(0))
                                    .Where(f => f.IdSetor == idSetorCarregamento).FirstOrDefault();

                                //Faz a leitura no carregamento
                                ItemCarregamentoDAO.Instance.EfetuaLeitura(trans, leitura.IdFuncLeitura, leitura.DataLeitura.GetValueOrDefault(DateTime.Now), idCarregamento, etiqueta);

                                //Verifica se terminou de carregar
                                CarregamentoDAO.Instance.AtualizaCarregamentoCarregado(trans, idCarregamento);

                                trans.Commit();
                                trans.Close();

                                return;
                            }
                            else
                            {
                                throw ex1;
                            }
                        }
                    }

                    #endregion

                    //Faz a leitura no carregamento
                    ItemCarregamentoDAO.Instance.EfetuaLeitura(trans, idFunc, DateTime.Now, idCarregamento, etiqueta);

                    /* Chamado 35100. */
                    /* Chamado 58740. */
                    if (string.IsNullOrEmpty(etiqueta) || !ItemCarregamentoDAO.Instance.PecaCarregada(trans, idCarregamento, etiqueta))
                        throw new Exception(string.Format("Falha ao marcar peça como carregada. Etiqueta: {0}.", etiqueta));

                    //Verifica se terminou de carregar
                    CarregamentoDAO.Instance.AtualizaCarregamentoCarregado(trans, idCarregamento, etiqueta);

                    trans.Commit();
                    trans.Close();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    trans.Close();

                    ErroDAO.Instance.InserirFromException("EfetuaLeitura - Carregamento: " + idCarregamento + " Etiqueta: " + etiqueta +
                        (idPedidoExp > 0 ? " Pedido Expedição: " + idPedidoExp : ""), ex);

                    throw ex;
                }
            }
        }

        #endregion

        #region Estorno do Carregamento

        private static readonly object _estornoCarregamentoLock = new object();

        /// <summary>
        /// Efetua o estorno de itens do carregamento
        /// </summary>
        public void EstornoCarregamento(string idsItensCarregamento, uint? idCarregamentoEstornar, string motivo)
        {
            lock (_estornoCarregamentoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        if (string.IsNullOrEmpty(idsItensCarregamento) &&
                            idCarregamentoEstornar.GetValueOrDefault(0) > 0)
                            idsItensCarregamento = ItemCarregamentoDAO.Instance.GetIdsItemCarregamento(transaction, idCarregamentoEstornar.Value);

                        /* Chamado 51701.
                         * Variável criada para salvar o ID do pedido com a situação de produção dele,
                         * dessa forma, o pedido será atualizado somente uma vez, ao invés de ser atualizado após o estorno de cada peça. */
                        var idsPedidoSituacaoProducao = new Dictionary<int, SituacaoProdutoProducao>();

                        //Credita o estoque
                        foreach (var item in idsItensCarregamento.Split(','))
                        {
                            var itemCarregamento = ItemCarregamentoDAO.Instance.GetElement(transaction, Glass.Conversoes.StrParaUint(item));

                            if (!itemCarregamento.Carregado)
                                continue;

                            var idLoja = PedidoDAO.Instance.ObtemIdLoja(transaction, itemCarregamento.IdPedido);
                            var dados = new List<DetalhesBaixaEstoque>();
                            var transferencia = false;

                            if (itemCarregamento.IdVolume.GetValueOrDefault(0) > 0)
                            {
                                var produtos = VolumeProdutosPedidoDAO.Instance.GetList(transaction, itemCarregamento.IdVolume.Value.ToString());
                                transferencia = OrdemCargaDAO.Instance.TemTransferencia
                                    (transaction, itemCarregamento.IdCarregamento, itemCarregamento.IdPedido);

                                foreach (var prod in produtos)
                                    dados.Add
                                        (new DetalhesBaixaEstoque()
                                        {
                                            IdProdPed = (int)prod.IdProdPed,
                                            Qtde = prod.Qtde,
                                            DescricaoBaixa = prod.DescProd
                                        });

                                //Se for transferencia usa a loja de quem esta fazendo a leitura.
                                if (transferencia)
                                    idLoja = UserInfo.GetUserInfo.IdLoja;

                                Pedido.Fluxo.AlterarEstoque.Instance.EstornaBaixaEstoque
                                    (transaction, idLoja, dados, itemCarregamento.IdVolume.Value, null);

                                //Se for transferencia estorna o estoque para da loja da transferencia
                                if (transferencia)
                                {
                                    var idLojaTransferencia = PedidoDAO.Instance.ObtemIdLoja(transaction, itemCarregamento.IdPedido);
                                    Pedido.Fluxo.AlterarEstoque.Instance.EstornoCreditaEstoque(transaction, idLojaTransferencia, dados);
                                }
                            }
                            else if (itemCarregamento.IdProdImpressaoChapa.GetValueOrDefault(0) > 0)
                            {
                                var prodsPed = ProdutosPedidoDAO.Instance.GetByPedidoProdutoForExpCarregamento(transaction, itemCarregamento.IdPedido, itemCarregamento.IdProd.Value, true);
                                var prodPed = MetodosExtensao.Agrupar(prodsPed.Where(f => f.QtdSaida > 0), new string[] { "IdProd" }, new string[] { "Qtde" })
                                    .Where(f => f.IdProd == itemCarregamento.IdProd)
                                    .FirstOrDefault();
                                transferencia = OrdemCargaDAO.Instance.TemTransferencia(transaction, itemCarregamento.IdCarregamento, itemCarregamento.IdPedido);

                                if (prodPed == null)
                                    throw new Exception("Produto não encontrado.");

                                //Dados que seram feitos a baixa no estoque
                                dados = new List<DetalhesBaixaEstoque>()
                                {
                                    new DetalhesBaixaEstoque()
                                    {
                                        IdProdPed = (int)prodPed.IdProdPed,
                                        DescricaoBaixa = prodPed.DescrProduto,
                                        Qtde = 1
                                    }
                                };

                                //Se for transferencia usa a loja de quem esta fazendo a leitura.
                                if (transferencia)
                                    idLoja = UserInfo.GetUserInfo.IdLoja;

                                //Remove no produto_impressão o id do pedido de expedição
                                ProdutoImpressaoDAO.Instance.AtualizaPedidoExpedicao
                                    (transaction, null, itemCarregamento.IdProdImpressaoChapa.Value);

                                //Remove o vinculo no corte
                                ChapaCortePecaDAO.Instance.DeleteByIdProdImpressaoChapa(transaction, itemCarregamento.IdProdImpressaoChapa.Value);

                                //Estorna a baixa do estoque
                                Pedido.Fluxo.AlterarEstoque.Instance.EstornaBaixaEstoque
                                    (transaction, idLoja, dados, null, itemCarregamento.IdProdImpressaoChapa.Value);

                                //Se for transferencia estorna o estoque para da loja da transferencia
                                if (transferencia)
                                {
                                    var idLojaTransferencia = PedidoDAO.Instance.ObtemIdLoja(transaction, itemCarregamento.IdPedido);
                                    Pedido.Fluxo.AlterarEstoque.Instance.EstornoCreditaEstoque(transaction, idLojaTransferencia, dados);
                                }

                                //Marca o retalho como vendido
                                var idRetalho = ProdutoImpressaoDAO.Instance.ObtemIdRetalho
                                    (transaction, itemCarregamento.IdProdImpressaoChapa.Value);
                                if (idRetalho.GetValueOrDefault(0) > 0)
                                    RetalhoProducaoDAO.Instance.AlteraSituacao
                                        (transaction, idRetalho.Value, Glass.Data.Model.SituacaoRetalhoProducao.Disponivel);
                            }
                            else
                            {
                                ProdutoPedidoProducaoDAO.Instance.VoltarPeca(transaction, itemCarregamento.IdProdPedProducao.Value,
                                    itemCarregamento.IdCarregamento, true, ref idsPedidoSituacaoProducao);
                            }

                            if (itemCarregamento.IdProdPedProducao.GetValueOrDefault() > 0)
                                LogAlteracaoDAO.Instance.LogProdPedProducao(transaction,
                                    ProdutoPedidoProducaoDAO.Instance.GetElementByPrimaryKey(transaction, itemCarregamento.IdProdPedProducao.Value),
                                    LogAlteracaoDAO.SequenciaObjeto.Novo);
                        }

                        // Percorre cada pedido salvo no dicionário de ID de pedido com a situação de produção dele.
                        foreach (var idPedido in idsPedidoSituacaoProducao.Keys)
                            // Atualiza a situação de produção do pedido, com base na informação salva no dicionário.
                            PedidoDAO.Instance.AtualizaSituacaoProducao(transaction, (uint)idPedido, idsPedidoSituacaoProducao[idPedido], DateTime.Now);

                        //Estorna os itens do item_carregamento
                        ItemCarregamentoDAO.Instance.EstornaItens(transaction, idsItensCarregamento);

                        //Registra o estorno
                        foreach (var item in idsItensCarregamento.Split(','))
                        {
                            EstornoItemCarregamentoDAO.Instance.Insert
                                (transaction,
                                    new Glass.Data.Model.EstornoItemCarregamento()
                                    {
                                        IdItemCarregamento = Glass.Conversoes.StrParaUint(item),
                                        Motivo = motivo
                                    });
                        }

                        //Atualiza a situação do carregamento
                        var idCarregamento = ItemCarregamentoDAO.Instance.GetIdCarregamento(transaction, idsItensCarregamento.Split(',')[0].StrParaUint());
                        CarregamentoDAO.Instance.AtualizaCarregamentoCarregado(transaction, idCarregamento, null);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException(
                            string.Format("Estorno - Carregamento: {0} Ids Itens Carregamento: {1}",
                                idCarregamentoEstornar.GetValueOrDefault(), idsItensCarregamento), ex);

                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Buscar os estornos que uma peça teve
        /// </summary>
        /// <param name="idItemCarregamento"></param>
        /// <returns></returns>
        public List<Glass.Data.Model.EstornoItemCarregamento> GetListEstornoItem(uint idItemCarregamento)
        {
            return EstornoItemCarregamentoDAO.Instance.GetByIdItemCarregamento(idItemCarregamento);
        }

        #endregion

        #region Etiqueta de Produção

        /// <summary>
        /// Verifica se a etiqueta é de revenda para fazer o vinculo com o pedido
        /// </summary>
        /// <param name="etiqueta"></param>
        /// <returns></returns>
        public bool IsEtiquetaRevenda(string etiqueta)
        {
            uint idPedido = Glass.Conversoes.StrParaUint(etiqueta.Split('-')[0]);
            return PedidoDAO.Instance.IsProducao(idPedido) || etiqueta.ToUpper().Substring(0, 1).Equals("N") || etiqueta.ToUpper().Substring(0, 1).Equals("R");
        }

        /// <summary>
        /// Recupera o id do pedido de revenda associado
        /// </summary>
        public int ObterIdPedidoRevenda(string etiqueta)
        {
            var idPedido = Glass.Conversoes.StrParaInt(etiqueta.Split('-')[0]);
            return PedidoDAO.Instance.ObterIdPedidoRevenda(null, idPedido).GetValueOrDefault();
        }

        #endregion

        #region Finaliza Carregamento

        static volatile object _finalizarCarregamentoLock = new object();

        /// <summary>
        /// Finaliza um carregamento
        /// </summary>
        public uint FinalizaCarregamento(string veiculo, uint idMotorista, DateTime dtPrevSaida, uint idLoja, string idsOCs, bool enviarEmail)
        {
            lock (_finalizarCarregamentoLock)
            {
                uint idCarregamento = 0;

                using (var trans = new GDATransaction())
                {
                    try
                    {
                        trans.BeginTransaction();

                        //Valida a finalização do carregamento
                        ValidaFinalizaCarregamento(trans, veiculo, idMotorista, dtPrevSaida, idLoja, idsOCs);

                        idCarregamento = CarregamentoDAO.Instance.Insert(trans, new Glass.Data.Model.Carregamento()
                        {
                            IdMotorista = idMotorista,
                            Placa = veiculo,
                            DataPrevistaSaida = dtPrevSaida,
                            IdLoja = idLoja,
                            Situacao = Glass.Data.Model.Carregamento.SituacaoCarregamentoEnum.PendenteCarregamento
                        });

                        //Vincula as OC's ao carregamento
                        OrdemCargaDAO.Instance.VinculaOCsCarregamento(trans, idCarregamento, idsOCs);

                        //Cria os itens do carregamento
                        ItemCarregamentoDAO.Instance.CriaItensCarregamento(trans, idCarregamento, null, null);

                        try
                        {
                            //Envia email para os clientes
                            if (!Utils.IsLocalUrl(HttpContext.Current) && enviarEmail)
                                Email.EnviaEmailCarregamentoFinalizado(trans, idCarregamento);
                        }
                        catch (Exception ex)
                        {
                            ErroDAO.Instance.InserirFromException(string.Format("FinalizaCarregamento(Erro no envio de email) - IdsOcs: {0} | Carregamento: {1}", idsOCs, idCarregamento), ex);
                        }

                        trans.Commit();
                        trans.Close();

                        return idCarregamento;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        trans.Close();

                        ErroDAO.Instance.InserirFromException(string.Format("FinalizaCarregamento - IdsOcs: {0} | Carregamento: {1}", idsOCs, idCarregamento), ex);

                        throw new Exception("Falha ao finalizar carregamento. " + ex.Message);
                    }
                }
            }
        }

        static volatile object _adicionarOcCarregamentoLock = new object();

        /// <summary>
        /// Adiciona ocs ao carregamento já criado
        /// </summary>
        public void AdicionaOCsCarregamento(uint idCarregamento, string idsOCs)
        {
            lock (_adicionarOcCarregamentoLock)
            {
                using (var trans = new GDATransaction())
                {
                    try
                    {
                        trans.BeginTransaction();

                        //Verifica se o carregamento existe.
                        if (!CarregamentoDAO.Instance.Exists(trans, idCarregamento))
                            throw new Exception("Carregamento não encontrado.");

                        //Valida as OCs
                        if (string.IsNullOrEmpty(idsOCs))
                            throw new Exception("Nenhuma OC foi informada para o carregamento.");

                        foreach (uint idOC in idsOCs.Split(',').Select(i => Glass.Conversoes.StrParaUint(i)))
                            ValidaOCForCarregamento(trans, idOC);

                        ValidaCarregamentoAcimaCapacidadeVeiculo(trans, CarregamentoDAO.Instance.ObtemPlaca(trans, idCarregamento), idsOCs);

                        //Vincula as OC's ao carregamento
                        OrdemCargaDAO.Instance.VinculaOCsCarregamento(trans, idCarregamento, idsOCs);

                        //Cria os itens do carregamento
                        ItemCarregamentoDAO.Instance.CriaItensCarregamento(trans, idCarregamento, idsOCs, null);

                        CarregamentoDAO.Instance.AtualizaCarregamentoCarregado(trans, idCarregamento, null);

                        LogAlteracaoDAO.Instance.LogCarregamentoOC(trans, (int)idCarregamento, idsOCs);

                        trans.Commit();
                        trans.Close();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        trans.Close();

                        ErroDAO.Instance.InserirFromException(string.Format("Adiciona OCs - Id Carregamento: {0} Ids Ocs: {1}",
                            idCarregamento, idsOCs), ex);

                        throw new Exception("Falha ao adicionar ocs ao carregamento. " + ex.Message);
                    }
                }
            }
        }

        #endregion

        #region Calcula peso carregamento

        public double CalcPesoCarregamento(List<uint> idsOCs)
        {
            return CalcPesoCarregamento(string.Join(",", idsOCs.Select(oc => oc.ToString()).ToArray()));
        }

        /// <summary>
        /// Calcula o peso do carregamento das ocs informadas
        /// </summary>
        public double CalcPesoCarregamento(string idsOCs)
        {
            return CalcPesoCarregamento(null, idsOCs);
        }

        /// <summary>
        /// Calcula o peso do carregamento das ocs informadas
        /// </summary>
        public double CalcPesoCarregamento(GDASession session, string idsOCs)
        {
            var ordensCarga = OrdemCargaDAO.Instance.GetOCsForCarregamento(session, idsOCs);
            double peso = 0;

            if (ordensCarga != null && ordensCarga.Count() > 0)
                peso = ordensCarga.Sum(f => f.Peso);

            return peso;
        }

        #endregion

        #region Métodos de validação

        /// <summary>
        /// Valida uma OC para inclusão no carregamento
        /// </summary>
        public void ValidaOCForCarregamento(GDASession sessao, uint idOC)
        {
            if (idOC == 0)
                throw new Exception("Nenhuma OC foi informada.");

            if (!OrdemCargaDAO.Instance.Exists(sessao, idOC))
                throw new Exception("OC " + idOC + " não foi encontrada.");

            var oc = OrdemCargaDAO.Instance.GetElementByPrimaryKey(sessao, idOC);

            if (oc.IdCarregamento > 0)
                throw new Exception("OC " + idOC + " já esta vinculada a um carregamento.");

            if (oc.Situacao != Glass.Data.Model.OrdemCarga.SituacaoOCEnum.Finalizado)
                throw new Exception("OC " + idOC + " já esta vinculada a um carregamento.");

            var idsPedidos = PedidoDAO.Instance.GetIdsPedidosByOCs(sessao, idOC.ToString());

            var idsPedidosSemVolume = "";

            foreach (var idPedido in idsPedidos)
                if (!PedidoDAO.Instance.GerouTodosVolumes(sessao, idPedido))
                    idsPedidosSemVolume += idPedido + ",";

            if (!string.IsNullOrEmpty(idsPedidosSemVolume))
                throw new Exception("Não é possivel finalizar este carregamento, pois o(s) pedido(s) " + idsPedidosSemVolume.Trim(',') +
                    " ainda possuem volumes a serem gerados.");

            var idsPedidosComPecaCanceladaSemImpressao = "";

            foreach (var idPedido in idsPedidos)
                if (ProdutoPedidoProducaoDAO.Instance.VerificaPecaCanceladaSemNovaImpressao(sessao, idPedido))
                    idsPedidosComPecaCanceladaSemImpressao += idPedido + ",";

            if (!string.IsNullOrEmpty(idsPedidosComPecaCanceladaSemImpressao))
                throw new Exception("Não é possivel finalizar este carregamento, pois o(s) pedido(s) " + idsPedidosComPecaCanceladaSemImpressao.Trim(',') +
                    " possuem peças que foram canceladas e ainda estão sem impressão.");
        }

        /// <summary>
        /// Valida a finalização do carregamento
        /// </summary>
        public void ValidaFinalizaCarregamento(GDASession sessao, string veiculo, uint idMotorista, DateTime dtPrevSaida, uint idLoja, string idsOCs)
        {
            if (string.IsNullOrEmpty(veiculo))
                throw new Exception("Informe o veículo do carregamento.");

            if (idMotorista == 0)
                throw new Exception("Informe o motorista do carregamento.");

            if (dtPrevSaida < DateTime.Now)
                throw new Exception("A data prevista de saída não pode ser menor que a data atual.");

            if (idLoja == 0)
                throw new Exception("Informe a loja do carregamento.");

            if (string.IsNullOrEmpty(idsOCs))
                throw new Exception("Nenhuma OC foi informada para o carregamento.");

            foreach (uint idOC in idsOCs.Split(',').Select(i => Glass.Conversoes.StrParaUint(i)))
                ValidaOCForCarregamento(sessao, idOC);

            ValidaCarregamentoAcimaCapacidadeVeiculo(sessao, veiculo, idsOCs);
        }

        public void ValidaCarregamentoAcimaCapacidadeVeiculo(GDASession sessao, string veiculo, string idsOCs)
        {
            if (Glass.Configuracoes.Geral.BloquearGerarCarregamentoAcimaCapacidadeVeiculo)
            {
                var capacidadeKG = VeiculoDAO.Instance.ObtemCapacidadeKgVeiculo(sessao, veiculo);
                double pesoCarregamento = CalcPesoCarregamento(sessao, idsOCs);

                if (pesoCarregamento > capacidadeKG)
                    throw new Exception("O peso do carregamento excede a capacidade do veículo (KG).");
            }
        }

        /// <summary>
        /// Valida a leitura de um item do carregamento
        /// </summary>
        public void ValidaLeitura(GDASession sessao, uint idCarregamento, string etiqueta, uint? idPedidoExp)
        {
            //Verifica se a etiqueta foi informada
            if (string.IsNullOrEmpty(etiqueta))
                throw new Exception("Informe a etiqueta.");

            //Valida o carregamento
            ValidaLeituraCarregamento(sessao, idCarregamento);

            //Verifica se a etiqueta é de volume, chapa ou produção
            if (etiqueta.ToUpper().Substring(0, 1).Equals("V"))
                ValidaLeituraVolume(sessao, idCarregamento, etiqueta);
            else if (etiqueta.ToUpper().Substring(0, 1).Equals("N") || etiqueta.ToUpper().Substring(0, 1).Equals("R"))
                ValidaLeituraChapaRetalho(sessao, idCarregamento, etiqueta, idPedidoExp);
            else
                ValidaLeituraPeca(sessao, idCarregamento, etiqueta);
        }

        /// <summary>        
        /// Valida o carregamento para a leitura de itens
        /// </summary>
        private void ValidaLeituraCarregamento(GDASession sessao, uint idCarregamento)
        {
            //Verifica se o carregamento existe
            if (!CarregamentoDAO.Instance.Exists(sessao, idCarregamento))
                throw new Exception("O carregamento informado não foi encontrado.");
        }

        /// <summary>
        /// Valida o volume para leitura
        /// </summary>
        private void ValidaLeituraVolume(GDASession sessao, uint idCarregamento, string etiqueta)
        {
            uint idVolume = Glass.Conversoes.StrParaUint(etiqueta.Substring(1));

            //Verifica se o volume existe
            if (!VolumeDAO.Instance.Exists(sessao, idVolume))
                throw new Exception("O volume da etiqueta informada não foi encontrado.");

            //Verifica se o volume foi finalizado
            if (VolumeDAO.Instance.GetSituacao(sessao, idVolume) == Glass.Data.Model.Volume.SituacaoVolume.Aberto)
                throw new Exception("O volume informado não está finalizado.");

            //Verifica se o volume ja foi carregado
            if (ItemCarregamentoDAO.Instance.VolumeCarregado(sessao, idCarregamento, idVolume))
                throw new Exception("A leitura deste volume já foi efetuada.");

            //Verifica se o volume é do carregamento informado
            if (!ItemCarregamentoDAO.Instance.VolumeEstaNoCarregamento(sessao, idCarregamento, idVolume))
                throw new Exception("O volume informado não faz parte deste carregamento.");

            /* Chamado 37226. */
            if (ProducaoConfig.ExpedirSomentePedidosLiberadosNoCarregamento)
            {
                var idPedido = VolumeDAO.Instance.GetIdPedido(sessao, idVolume);

                if (idPedido > 0 && PedidoDAO.Instance.ObtemSituacao(sessao, idPedido) != Glass.Data.Model.Pedido.SituacaoPedido.Confirmado)
                    throw new Exception(string.Format("O pedido {0} ainda não foi liberado. Após liberá-lo tente novamente.", idPedido));
            }
        }

        /// <summary>
        /// Valida a chapa de revenda para leitura
        /// </summary>
        private void ValidaLeituraChapaRetalho(GDASession sessao, uint idCarregamento, string etiqueta, uint? idPedidoNovo)
        {
            var tipoEtiqueta = ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(etiqueta);
            var idProdImpressao = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressaoParaCarregamento(sessao, etiqueta);

            uint idRetalho = tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.Retalho ?
                Glass.Conversoes.StrParaUint(etiqueta.Substring(1, etiqueta.IndexOf('-') - 1)) : 0;

            if (tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.Retalho &&
                RetalhoProducaoDAO.Instance.ObtemSituacao(sessao, idRetalho) != Glass.Data.Model.SituacaoRetalhoProducao.Disponivel)
                throw new Exception("O retalho informado não esta disponivel para uso.");

            if (idProdImpressao.GetValueOrDefault(0) == 0)
                throw new Exception("A peça da etiqueta informada não foi encontrada ou está cancelada.");

            if (idPedidoNovo.GetValueOrDefault(0) == 0)
                throw new Exception("Indique o pedido de venda/revenda que contém esse produto.");
            else if (!PedidoDAO.Instance.IsVenda(sessao, idPedidoNovo.Value))
                throw new Exception("Apenas pedidos de venda/revenda podem ser utilizados como pedido novo.");

            var idCarr = ItemCarregamentoDAO.Instance.ChapaCarregada(sessao, idProdImpressao.Value);

            if (idCarr.GetValueOrDefault(0) > 0)
                throw new Exception("A leitura desta etiqueta já foi efetuada" + (idCarr.Value != idCarregamento ? " no carregamento: " + idCarr.Value : "") + ".");

            //Verifica se a etiqueta ja foi expedida
            if (ProdutoImpressaoDAO.Instance.EstaExpedida(sessao, idProdImpressao.Value))
                throw new Exception("Esta etiqueta ja foi expedida no sistema.");

            if (ChapaCortePecaDAO.Instance.ChapaPossuiLeitura(sessao, idProdImpressao.Value))
                throw new Exception("Esta etiqueta já foi utilizada no setor de corte.");

            // Verifica se o pedido já foi liberado
            if (ProducaoConfig.ExpedirSomentePedidosLiberadosNoCarregamento &&
                PedidoDAO.Instance.ObtemSituacao(sessao, idPedidoNovo.Value) != Glass.Data.Model.Pedido.SituacaoPedido.Confirmado)
                throw new Exception("Este pedido ainda não foi liberado.");

            var prodPed = ProdutosPedidoDAO.Instance.GetByPedido(sessao, idPedidoNovo.Value);
            prodPed = Glass.MetodosExtensao.ToArray(Glass.MetodosExtensao.Agrupar(prodPed, new string[] { "IdProd" }, new string[] { "Qtde" }));
            var idProd = tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal ? ProdutosNfDAO.Instance.GetIdProdByEtiqueta(sessao, etiqueta) :
                RetalhoProducaoDAO.Instance.ObtemIdProd(sessao, idRetalho);

            bool encontrado = false;

            foreach (var pp in prodPed)
            {
                if (pp.IdProd == idProd)
                {
                    if (ItemCarregamentoDAO.Instance.ObtemQtdeProdutoFaltaCarregar(sessao, idCarregamento, idPedidoNovo.Value, null, idProd) > 0)
                    {
                        encontrado = true;
                        break;
                    }
                }
            }

            if (!encontrado)
                throw new Exception("Produto não encontrado ou já expedido no pedido de venda " + idPedidoNovo.Value + ".");

            //Verifica se a chapa foi marcada perda.
            if (tipoEtiqueta != ProdutoImpressaoDAO.TipoEtiqueta.Retalho && PerdaChapaVidroDAO.Instance.IsPerda(sessao, etiqueta))
                throw new Exception("A etiqueta esta marcada como perdida.");
        }

        /// <summary>
        /// Valida a peça para a leitura
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="etiqueta"></param>
        private void ValidaLeituraPeca(GDASession sessao, uint idCarregamento, string etiqueta)
        {
            //Valida a etiqueta
            ProdutoPedidoProducaoDAO.Instance.ValidaEtiquetaProducao(sessao, ref etiqueta);

            /* Chamado 46554. */
            var setoresRestantes = SetorDAO.Instance.ObtemSetoresRestantes(sessao, etiqueta);

            if (setoresRestantes != null && setoresRestantes.Count > 0)
                throw new Exception(string.Format("A etiqueta {0} não está pronta. Setores restantes: {1}.", etiqueta,
                    string.Join(", ", setoresRestantes.Select(f => f.Descricao).ToList())));

            //Verifica se a peça existe
            if (ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(sessao, etiqueta).GetValueOrDefault(0) == 0)
                throw new Exception("A peça da etiqueta informada não foi encontrada.");

            //Verifica se a peça é do carregamento informado
            if (!IsEtiquetaRevenda(etiqueta) && !ItemCarregamentoDAO.Instance.PecaEstaNoCarregamento(sessao, idCarregamento, etiqueta))
                throw new Exception("A peça informada não faz parte deste carregamento.");

            //Verifica se a peça ja foi carregada
            if (ItemCarregamentoDAO.Instance.PecaCarregada(sessao, idCarregamento, etiqueta))
                throw new Exception("A leitura desta etiqueta já foi efetuada.");

            // Verifica se a peça está pronta
            if (!ProdutoPedidoProducaoDAO.Instance.PecaEstaPronta(sessao, etiqueta))
                throw new Exception("A peça informada ainda não está pronta.");
        }

        /// <summary>
        /// Valida o estorno de itens do carregamento
        /// </summary>
        /// <param name="idsItensCarregamento"></param>
        public void ValidaEstornoCarregamento(List<uint> idsItensCarregamento, string motivo)
        {
            if (idsItensCarregamento.Count == 0)
                throw new Exception("Nenhum item foi informado.");

            if (string.IsNullOrEmpty(motivo))
                throw new Exception("O motivo não foi informado.");
        }

        /// <summary>
        /// Valida os filtros
        /// </summary>
        public void ValidaFiltros(GDASession sessao, uint idCliente, string nomeCli, string etiqueta, uint? idPedidoExp,
            int? idOc, int? idPedidoFiltro, decimal? altura, decimal? largura, string numEtqFiltro, uint idClienteExterno, string nomeClienteExterno, uint idPedidoExterno)
        {
            if (idCliente == 0 && string.IsNullOrEmpty(nomeCli) && idOc.GetValueOrDefault(0) == 0 && idPedidoFiltro.GetValueOrDefault(0) == 0 &&
                altura.GetValueOrDefault(0) == 0 && largura.GetValueOrDefault(0) == 0 && string.IsNullOrEmpty(numEtqFiltro) && idClienteExterno == 0
                && string.IsNullOrEmpty(nomeClienteExterno) && idPedidoExterno == 0)
                return;

            if (string.IsNullOrEmpty(etiqueta))
                throw new Exception("Etiqueta não informada");

            var idsCliente = idCliente.ToString();

            if (idCliente == 0)
                idsCliente = ClienteDAO.Instance.GetIds(sessao, null, nomeCli, null, 0, null, null, null, null, 0);

            uint idPedido = 0;

            //Verifica se é etiqueta de volume
            if (etiqueta.ToUpper().Substring(0, 1).Equals("V"))
            {
                var idVolume = Glass.Conversoes.StrParaUint(etiqueta.Substring(1));
                idPedido = VolumeDAO.Instance.GetIdPedido(sessao, idVolume);
            }
            else
            {
                if (idPedidoExp.GetValueOrDefault(0) > 0)
                    idPedido = idPedidoExp.Value;
                else
                    idPedido = Glass.Conversoes.StrParaUint(etiqueta.Split('-')[0]);
            }

            var idCliPedido = PedidoDAO.Instance.GetIdCliente(sessao, idPedido);

            if (idCliPedido == 0)
                throw new Exception("Cliente do pedido não encontrado.");

            var produtoAlturaLargura = ProdutoDAO.Instance.ObtemAlturaLarguraByEtiqueta(sessao, etiqueta);

            var valido = true;

            //Valida o cliente
            if (!string.IsNullOrEmpty(idsCliente) && idsCliente != "0" && !("," + idsCliente + ",").Contains("," + idCliPedido.ToString() + ","))
                valido = false;

            //Valida o pedido
            if (idPedidoFiltro.GetValueOrDefault(0) > 0 && idPedido != idPedidoFiltro)
                valido = false;

            //Valida a oc
            var ocs = PedidoOrdemCargaDAO.Instance.GetIdsOCsByPedidos(sessao, idPedido.ToString());
            if (idOc.GetValueOrDefault(0) > 0 && !("," + ocs + ",").Contains("," + idOc + ","))
                valido = false;

            //Vaida etiqueta
            if (!string.IsNullOrEmpty(numEtqFiltro) && numEtqFiltro != etiqueta)
                valido = false;

            //valida altura
            if (altura.GetValueOrDefault(0) > 0 && produtoAlturaLargura != null && altura != produtoAlturaLargura.Item2)
                valido = false;

            //valida largura
            if (largura.GetValueOrDefault(0) > 0 && produtoAlturaLargura != null && largura != produtoAlturaLargura.Item3)
                valido = false;

            //Valida o cliente externo
            if (idClienteExterno > 0 && PedidoDAO.Instance.ObtemIdClienteExterno(sessao, idPedido) != idClienteExterno)
                valido = false;
            else if (!string.IsNullOrEmpty(nomeClienteExterno) && !PedidoDAO.Instance.ObtemClienteExterno(sessao, idPedido).Contains(nomeClienteExterno))
                valido = false;

            //Valida o pedido externo
            if (idPedidoExterno > 0 && PedidoDAO.Instance.ObtemIdPedidoExterno(sessao, idPedido) != idPedido)
                valido = false;

            if (!valido)
                throw new Exception("Nenhum item encontrado para os filtros informados.");
        }

        #endregion

        #region Remove um carregamento

        static volatile object _apagarCarregamentoLock = new object();

        /// <summary>
        /// Remove um carregamento
        /// </summary>
        public int Delete(Carregamento objDelete)
        {
            lock (_apagarCarregamentoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        //Se ja tiver carregado alguma peça não pode deletar
                        if (CarregamentoDAO.Instance.CarregamentoTemItemCarregado(transaction, objDelete.IdCarregamento))
                            throw new Exception("Este carregamento possui itens carregados.");

                        //Verifica se tem oc de transferencia
                        var ocs = OrdemCargaDAO.Instance.GetOCsForCarregamento(transaction, objDelete.IdCarregamento)
                            .Where(f => f.TipoOrdemCarga == Glass.Data.Model.OrdemCarga.TipoOCEnum.Transferencia).ToList();

                        var idsPedidos = new List<uint>();

                        foreach (var oc in ocs)
                            if (OrdemCargaDAO.Instance.GetIdsPedidosOC(transaction, oc.IdOrdemCarga, Glass.Data.Model.OrdemCarga.TipoOCEnum.Venda).Count > 0)
                                throw new Exception("Este carregamento possui pedidos que estão vinculados a uma OC de venda.");

                        //Remove todos os itens do carregamento
                        ItemCarregamentoDAO.Instance.DeleteByCarregamento(transaction, objDelete.IdCarregamento);

                        // Desvincula as OC's a um carregamento
                        OrdemCargaDAO.Instance.DesvinculaOCsCarregamento(transaction, objDelete.IdCarregamento);

                        var retorno = CarregamentoDAO.Instance.DeleteByPrimaryKey(transaction, objDelete.IdCarregamento);

                        transaction.Commit();
                        transaction.Close();

                        return retorno;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException(string.Format("Delete - Carregamento: {0}", objDelete.IdCarregamento), ex);

                        throw ex;
                    }
                }
            }
        }

        #endregion

        #region Recupera os ids dos pedidos de um carregamento

        /// <summary>
        /// Recupera os ids dos pedidos de um carregamento
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <returns></returns>
        public string GetIdsPedidosByCarregamento(uint idCarregamento)
        {
            if (idCarregamento == 0)
                throw new Exception("Nenhum carregamento foi informado");

            var idsPedidos = PedidoDAO.Instance.GetIdsPedidosByCarregamentoParaNfTransferencia(idCarregamento);

            if (idsPedidos == null || idsPedidos.Count == 0)
                throw new Exception("Nenhum pedido foi encontrado");

            return string.Join(",", idsPedidos.Select(p => p.ToString()).ToArray());
        }

        #endregion

        #region Ajax

        private static Ajax.ICarregamento _ajax = null;

        public static Ajax.ICarregamento Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.Carregamento();

                return _ajax;
            }
        }

        #endregion

        #region Atualiza dados do carregamento

        public void Update(Carregamento carregamento)
        {
            if(Glass.Configuracoes.Geral.BloquearGerarCarregamentoAcimaCapacidadeVeiculo)
            {
                var capacidadeKG = VeiculoDAO.Instance.ObtemCapacidadeKgVeiculo(carregamento.Placa);
                if (carregamento.Peso > capacidadeKG)
                    throw new Exception("O veiculo do carregamento não pode ser alterado, pois sua capacidade (KG) é menor que o peso do carregamento.");
            }

            CarregamentoDAO.Instance.Update(carregamento);
        }

        #endregion

        #region Faturamento

        /// <summary>
        /// Busca os pedidos pendentes para finalização do faturamento
        /// </summary>
        public string BuscarPendenciasFaturamento(uint idCarregamento)
        {
            return CarregamentoDAO.Instance.BuscarPendenciasFaturamento(idCarregamento);
        }

        /// <summary>
        /// Fatura o carregamento (Libera, gera nota e boletos dos pedidos do idCarregamento)
        /// </summary>
        public string EfetuarFaturamento(int idCarregamento)
        {
            var mensagemErro = string.Empty;
            var idsClientesInvalidos = new List<uint>();
            var gerouFaturamento = false;

            var dadosFaturamento = Glass.Data.RelDAL.FaturamentoCarregamentoaDAO.Instance.ObterDadosFaturamento(idCarregamento);

            //Valida os cliente que podem realizar o faturamento
            foreach (var d in dadosFaturamento)
            {
                if (idsClientesInvalidos.Contains(d.IdCliente))
                    continue;

                var msgErroCliente = d.ValidaCliente();

                if (!string.IsNullOrEmpty(msgErroCliente))
                {
                    idsClientesInvalidos.Add(d.IdCliente);
                    mensagemErro += msgErroCliente;
                }
            }

            //Remove o faturamento dos clientes que não podem faturar
            dadosFaturamento = dadosFaturamento.Where(f => !idsClientesInvalidos.Contains(f.IdCliente)).ToList();

            foreach (var d in dadosFaturamento)
            {
                //Valida os pedidos que podem ser faturados
                mensagemErro += d.ValidaPedidos();

                if (d.LstIdsPedidos.Count == 0)
                    continue;

                var idsProdPed = ProdutosPedidoDAO.Instance.ObtemIdsProdPedByPedidos(null, d.IdsPedidos);

                var qtdesProdPed = new List<float>();
                var prodPedsProducao = new List<uint?>();

                foreach (var idProdPed in idsProdPed)
                {
                    qtdesProdPed.Add(ProdutosPedidoDAO.Instance.ObtemQtde(null, idProdPed));
                    prodPedsProducao.Add(0);
                }

                var numParcelas = (d.Parcelas.NumParcelas == 0 ? 1 : d.Parcelas.NumParcelas);
                var valoresParcelas = new List<decimal>();
                var valorParcela = d.ValorTotalPedidos / numParcelas;

                for (int i = 0; i < numParcelas; i++)
                    valoresParcelas.Add(decimal.Round(valorParcela, 2));

                if (valoresParcelas.Sum(f => f) > d.ValorTotalPedidos)
                {
                    var diferenca = valoresParcelas.Sum(f => f) - d.ValorTotalPedidos;
                    valoresParcelas[valoresParcelas.Count() - 1] = (valoresParcelas.LastOrDefault() - diferenca);
                }

                #region Liberação

                uint idLiberarPedido = 0;

                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        idLiberarPedido = LiberarPedidoDAO.Instance.CriarLiberacaoAPrazo(transaction, d.IdCliente, d.IdsPedidos, idsProdPed.ToArray(), prodPedsProducao.ToArray(),
                            qtdesProdPed.ToArray(), d.ValorTotalPedidos, numParcelas, d.Parcelas.NumeroDias, valoresParcelas.ToArray(), (uint?)d.Parcelas.IdParcela,
                            false, new uint[] { d.IdFormaPagto }, new uint[] { }, new decimal[] { }, new uint[] { }, new uint[] { }, new uint[] { }, false, 0, string.Empty, false, false,
                            new uint[] { 1, 1, 1, 1, 1 }, 2, 0, 2, 0, d.IdFormaPagto, 0, string.Empty, new string[] { });

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        mensagemErro += "Erro ao faturar os pedidos " + string.Join(", ", d.LstIdsPedidos) + "\r\n";
                        mensagemErro += MensagemAlerta.FormatErrorMsg("", ex) + "\r\n---------------\r\n";
                        continue;
                    }
                }

                #endregion

                #region NF-e

                if (idLiberarPedido > 0)
                {
                    try
                    {
                        var idsPedidoLiberados = string.Join(",", ProdutosLiberarPedidoDAO.Instance.GetIdsPedidoByLiberacao(idLiberarPedido));
                        var idClienteLiberar = LiberarPedidoDAO.Instance.GetIdCliente(idLiberarPedido);
                        var percReducao = ClienteDAO.Instance.GetPercReducaoNFe(idClienteLiberar).ToString();
                        var percReducaoRevenda = ClienteDAO.Instance.GetPercReducaoNFeRevenda(idClienteLiberar).ToString();
                        var idLoja = LiberarPedidoDAO.Instance.ObtemIdLoja(idLiberarPedido).ToString();

                        var retGerarNf = NotaFiscal.Fluxo.Gerar.Ajax.GerarNf(idsPedidoLiberados, idLiberarPedido.ToString(),
                            "", idLoja, percReducao, percReducaoRevenda, "", "", "false", "", "false", "false", "true").Split(';');

                        if (retGerarNf[0] == "Erro")
                            throw new Exception(retGerarNf[1]);

                        var idNf = retGerarNf[1].StrParaUint();

                        var retEmissaoNf = NotaFiscalDAO.Instance.EmitirNf(idNf, false, false);

                        if (retEmissaoNf != "Autorizado o uso da NF-e")
                            throw new Exception(retEmissaoNf);

                        gerouFaturamento = true;

                    }
                    catch (Exception ex)
                    {
                        mensagemErro += "Erro ao faturar a liberação " + idLiberarPedido + "\r\n";
                        mensagemErro += MensagemAlerta.FormatErrorMsg("", ex) + "\r\n---------------\r\n";
                        continue;
                    }
                }

                #endregion
            }

            if (!string.IsNullOrEmpty(mensagemErro))
                return "erro||" + mensagemErro;


            return "ok||" + (gerouFaturamento ? " ||" : "");
        }

        /// <summary>
        /// Busca as liberações e notas referentes ao faturamento do carregamento passado
        /// </summary>
        public string BuscarFaturamento(uint idCarregamento)
        {
            var carregamento = CarregamentoDAO.Instance.GetElementByPrimaryKey(idCarregamento);
            var idsPedido = PedidoDAO.Instance.GetIdsPedidosByCarregamento(null, idCarregamento);
            var clientesPedidosFormaPagto = new Dictionary<Tuple<uint, uint>, List<uint>>();
            var retorno = new List<string>();

            foreach (var idPedido in idsPedido)
            {
                var idCliente = PedidoDAO.Instance.GetIdCliente(null, idPedido);
                var idFormaPagto = PedidoDAO.Instance.GetFormaPagto(null, idPedido);

                var idCliFP = new Tuple<uint, uint>(idCliente, idFormaPagto.GetValueOrDefault());

                if (!clientesPedidosFormaPagto.ContainsKey(idCliFP))
                    clientesPedidosFormaPagto.Add(idCliFP, new List<uint>());

                clientesPedidosFormaPagto[idCliFP].Add(idPedido);
            }

            foreach (var item in clientesPedidosFormaPagto)
            {
                var idLiberacao = LiberarPedidoDAO.Instance.GetIdsLiberacaoAtivaByPedido(item.Value.FirstOrDefault()).FirstOrDefault();
                var numeroNf = Glass.Conversoes.StrParaUint(NotaFiscalDAO.Instance.ObtemNumNfePedidoLiberacao(null, item.Value.FirstOrDefault(), true));

                foreach (var idPedido in item.Value.ToList())
                {
                    var idLiberacaoVerificar = LiberarPedidoDAO.Instance.GetIdsLiberacaoAtivaByPedido(idPedido).FirstOrDefault();
                    var numeroNfVerificar = Glass.Conversoes.StrParaUint(NotaFiscalDAO.Instance.ObtemNumNfePedidoLiberacao(idLiberacaoVerificar, idPedido, true));

                    if (idLiberacaoVerificar != idLiberacao || numeroNfVerificar != numeroNf)
                        throw new Exception("Não é possível buscar esse faturamento pois os pedidos que o compõe estão sendo processados separadamente.");
                }

                var idNf = NotaFiscalDAO.Instance.GetByNumeroNFe(numeroNf, 2).FirstOrDefault().IdNf;
                retorno.Add(string.Format("{0},{1}", idLiberacao, idNf));
            }

            return string.Join(";", retorno);
        }

        #endregion
    }
}

