using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class ResumoCorteDAO : Glass.Pool.PoolableObject<ResumoCorteDAO>
    {
        private ResumoCorteDAO() { }

        private class Chave
        {
            public uint IdPedido;
            public uint IdProd;
            public bool IsVidro;
            public uint Id;

            public Chave(uint id, uint idPedido, uint idProd, bool isVidro)
            {
                Id = id;
                IdPedido = idPedido;
                IdProd = idProd;
                IsVidro = isVidro;
            }

            public override bool Equals(object obj)
            {
                Chave comp = obj as Chave;

                bool verificar = comp.IdPedido == IdPedido && comp.IdProd == IdProd && comp.IsVidro == IsVidro;

                if (verificar && Liberacao.DadosLiberacao.UsarRelatorioLiberacao4Vias && IsVidro)
                    return comp.Id == Id;

                return verificar;
            }

            public override int GetHashCode()
            {
                string hashCode = (IdPedido.ToString().Length > 3 ? IdPedido.ToString().Remove(0, IdPedido.ToString().Length - 3) : IdPedido.ToString()) + 
                    IdProd.ToString() + (IsVidro ? 1 : 0);

                return Glass.Conversoes.StrParaInt(hashCode);
            }
        }

        private ResumoCorte[] GetProdutosCorte<T>(IEnumerable<T> produtos, int numeroViaLiberacao) where T : IResumoCorte
        {
            var dicResumoCorte = new Dictionary<Chave, List<IResumoCorte>>();

            // Cria um dicionário com a chave de agrupamento e a lista de produtos
            foreach (IResumoCorte r in produtos)
            {
                // Chave usada para agrupar os produtos
                Chave chave = new Chave(r.Id, r.IdPedido, r.IdProd, GrupoProdDAO.Instance.IsVidro((int)r.IdGrupoProd));

                if (numeroViaLiberacao > 0 && !Liberacao.DadosLiberacao.AgruparResumoLiberacaoProduto && chave.IsVidro && !Liberacao.RelatorioLiberacaoPedido.ConsiderarVidroQualquerProdutoDoGrupoVidro)
                    chave.IsVidro = !SubgrupoProdDAO.Instance.IsSubgrupoProducao((int)r.IdProd);
                // A condição "|| Liberacao.DadosLiberacao.AgruparResumoLiberacaoProduto" abaixo foi comentada para que na contemper
                // a via de almoxarife aparecesse mesmo se a opção Liberacao.DadosLiberacao.AgruparResumoLiberacaoProduto estiver marcada
                else if ((numeroViaLiberacao == 0/* || Liberacao.DadosLiberacao.AgruparResumoLiberacaoProduto*/) && !chave.IsVidro)
                    continue;

                if (!dicResumoCorte.ContainsKey(chave))
                    dicResumoCorte.Add(chave, new List<IResumoCorte>());

                dicResumoCorte[chave].Add(r);
            }

            // Agrupa os produtos somando seus totais
            var retorno =  
                dicResumoCorte.Select(f => new ResumoCorte()
                {
                    // Recupera dados comuns aos produtos
                    IdPedido = f.Key.IdPedido,
                    IdProd = f.Key.IdProd,
                    IsVidro = f.Key.IsVidro,
                    IdGrupoProd = f.Value[0].IdGrupoProd,
                    Espessura = f.Value[0].Espessura,
                    CodAplicacao = f.Value[0].CodAplicacao,
                    CodProcesso = f.Value[0].CodProcesso,
                    CodInterno = f.Value[0].CodInterno,
                    DescrProd = f.Value[0].DescrProduto,
                    DescrGrupoProd = GrupoProdDAO.Instance.GetDescricao((int)f.Value[0].IdGrupoProd),

                    // Soma os totais dos produtos agrupados
                    Qtde = f.Value.Sum(x => x.Qtde),
                    Total = f.Value.Sum(x => x.Total),
                    Altura = f.Value.Sum(x => x.Altura),
                    Largura = f.Value.Sum(x => x.Largura),
                    TotM2 = Math.Round(f.Value.Sum(x => x.TotM), Geral.NumeroCasasDecimaisTotM),
                    TotM2Calc = Math.Round(f.Value.Sum(x => x.TotM2Calc), Geral.NumeroCasasDecimaisTotM)

                }).ToArray();

            return retorno;
        }

        /// <summary>
        /// Retorna os produtos do resumo de corte por pedido.
        /// </summary>
        /// <param name="idPedido">O id do pedido.</param>
        /// <returns></returns>
        public ResumoCorte[] GetProdutosByPedido(uint idPedido)
        {
            return GetProdutosCorte<ProdutosPedido>(ProdutosPedidoDAO.Instance.GetByPedido(idPedido), 0);
        }

        /// <summary>
        /// Retorna os produtos do resumo de corte por pedido espelho.
        /// </summary>
        /// <param name="idPedidoEspelho"></param>
        /// <returns></returns>
        public ResumoCorte[] GetProdutosByPedidoEspelho(string idsPedidos, string grupos, string produtos)
        {
            return GetProdutosCorte<ProdutosPedidoEspelho>(ProdutosPedidoEspelhoDAO.Instance.GetForResumoCorte(idsPedidos, grupos, produtos), 0);
        }

        /// <summary>
        /// Retorna os produtos do resumo de corte por pedido espelho.
        /// </summary>
        /// <param name="lstProdutosPedidoEspelho"></param>
        /// <returns></returns>
        public ResumoCorte[] GetProdutosByPedidoEspelho(ProdutosPedidoEspelho[] lstProdutosPedidoEspelho)
        {
            return GetProdutosCorte<ProdutosPedidoEspelho>(lstProdutosPedidoEspelho, 0);
        }

        /// <summary>
        /// Retorna os produtos do resumo de corte por liberação de pedido.
        /// </summary>
        /// <param name="idLiberacaoPedido"></param>
        /// <returns></returns>
        public ResumoCorte[] GetProdutosByLiberacaoPedido(uint idLiberacaoPedido)
        {
            return GetProdutosByLiberacaoPedido(ProdutosLiberarPedidoDAO.Instance.GetByLiberarPedido(idLiberacaoPedido));
        }

        /// <summary>
        /// Retorna os produtos do resumo de corte por liberação de pedido.
        /// </summary>
        /// <param name="idLiberacaoPedido"></param>
        /// <returns></returns>
        public ResumoCorte[] GetProdutosByLiberacaoPedido(IEnumerable<ProdutosLiberarPedido> produtosLiberacao)
        {
            List<ResumoCorte> retorno = new List<ResumoCorte>();
            retorno.AddRange(GetProdutosCorte<ProdutosLiberarPedido>(produtosLiberacao, 1));

            return retorno.ToArray();
        }

        /// <summary>
        /// Retorna os produtos do resumo de corte por liberação de pedido.
        /// </summary>
        /// <param name="idLiberacaoPedido"></param>
        /// <returns></returns>
        public ResumoCorte[] GetProdutosByLiberacaoPedido(IEnumerable<ProdutosLiberarPedidoRpt> produtosLiberacao, int numeroVias)
        {
            List<ResumoCorte> retorno = new List<ResumoCorte>();
            for (int i = 0; i < numeroVias; i++)
                retorno.AddRange(GetProdutosCorte<ProdutosLiberarPedidoRpt>(produtosLiberacao, i + 1));

            return retorno.ToArray();
        }

        /// <summary>
        /// Retorna os produtos do resumo de corte para impressão da liberação de pedido otimizada
        /// </summary>
        public List<ResumoCorte> ObterResumoCorte(IEnumerable<ProdutosLiberarPedidoRpt> produtosLiberacao)
        {
            // Configuração para buscar qualquer produto do grupo vidro
            var buscarTodosGrupoVidro = Liberacao.RelatorioLiberacaoPedido.ConsiderarVidroQualquerProdutoDoGrupoVidro;

            // Dicionário contendo quais grupos/subgrupos são ou não são de revenda
            var dicSubgrupoRevenda = new Dictionary<Tuple<uint, uint?>, bool>();
            foreach (var tupla in produtosLiberacao.Select(f => new Tuple<uint, uint?>(f.IdGrupoProd, f.IdSubgrupoProd)).Distinct())
                dicSubgrupoRevenda.Add(tupla, SubgrupoProdDAO.Instance.IsSubgrupoProducao((int)tupla.Item1, (int?)tupla.Item2));

            var pecasResumoCorte = produtosLiberacao
                .Where(f =>

                    // Define se irá buscar qualquer produto do grupo vidro ou apenas produtos de produção
                    buscarTodosGrupoVidro ?
                        f.IdGrupoProd == (int)NomeGrupoProd.Vidro :
                        !dicSubgrupoRevenda[new Tuple<uint, uint?>(f.IdGrupoProd, f.IdSubgrupoProd)]
                )
                .GroupBy(f => f.IdProd)
                .Select(f => new ResumoCorte()
                    {
                        // Recupera dados comuns aos produtos
                        IdPedido = f.First().IdPedido,
                        IdProd = f.First().IdProd,
                        IsVidro = f.First().IsVidro,
                        IdGrupoProd = f.First().IdGrupoProd,
                        Espessura = f.First().Espessura,
                        CodAplicacao = f.First().CodAplicacao,
                        CodProcesso = f.First().CodProcesso,
                        CodInterno = f.First().CodInterno,
                        DescrProd = f.First().DescrProduto,
                        DescrGrupoProd = GrupoProdDAO.Instance.GetDescricao((int)f.First().IdGrupoProd),

                        // Soma os totais dos produtos agrupados
                        Qtde = f.Sum(x => x.Qtde),
                        Total = f.Sum(x => x.Total),
                        Altura = f.Sum(x => x.Altura),
                        Largura = f.Sum(x => x.Largura),
                        TotM2 = Math.Round(f.Sum(x => x.TotM), Geral.NumeroCasasDecimaisTotM),
                        TotM2Calc = Math.Round(f.Sum(x => x.TotM2Calc), Geral.NumeroCasasDecimaisTotM)
                    }
                ).ToList();

            return pecasResumoCorte;
        }
    }
}
