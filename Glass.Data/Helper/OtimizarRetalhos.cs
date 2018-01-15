using System.Collections.Generic;
using Glass.Data.DAL;
using System.Linq;

namespace Glass.Data.Helper
{
    public static class OtimizarRetalhos
    {
        public class DadosRetalho
        {
            public string IdProdPed { get; set; }
            public int NumeroPeca { get; set; }
            public uint? IdRetalhoProducao { get; set; }
            public int NumeroLinha { get; set; }
            public string IdsRetalhosPossiveis { get; set; }
        }

        ///// <summary>
        ///// Otimiza os retalhos de produção.
        ///// </summary>
        ///// <param name="dadosRetalho"></param>
        //public static void Otimizar(ref List<DadosRetalho> dadosRetalho)
        //{
        //    // Limpa as referências dos retalhos atuais
        //    foreach (DadosRetalho d in dadosRetalho)
        //        d.IdRetalhoProducao = null;

        //    // Cria a variável que contém os dados do grafo
        //    DadosGrafo<ProdutoPedidoEspelho, Grafo.RetalhoProducao> dados = new DadosGrafo<ProdutoPedidoEspelho, Grafo.RetalhoProducao>();

        //    // Adiciona os vértices e arestas aos dados
        //    foreach (DadosRetalho r in dadosRetalho)
        //    {
        //        // Cria o vértice de origem
        //        ProdutoPedidoEspelho o = new ProdutoPedidoEspelho(ProdutosPedidoEspelhoDAO.Instance.
        //            GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(r.IdProdPed.TrimStart('R'))), r.NumeroPeca);

        //        dados.Origens.Add(o);

        //        // Busca os retalhos possíveis para o produto pedido espelho
        //        foreach (Model.RetalhoProducao rp in RetalhoProducaoDAO.Instance.ObterLista(r.IdsRetalhosPossiveis))
        //        {
        //            // Cria o vértice de destino
        //            Grafo.RetalhoProducao d;

        //            int index = dados.Destinos.FindIndex(x => x.IdRetalhoProducao == rp.IdRetalhoProducao);
        //            if (index == -1)
        //            {
        //                d = new Grafo.RetalhoProducao(rp);
        //                dados.Destinos.Add(d);
        //            }
        //            else
        //                d = dados.Destinos[index];

        //            // Adiciona uma aresta entre origem e destino
        //            dados.Arestas.Add(new Aresta<ProdutoPedidoEspelho, Grafo.RetalhoProducao>
        //            {
        //                VerticeOrigem = o,
        //                VerticeDestino = d
        //            });
        //        }
        //    }

        //    // Cria o grafo e vincula aos dados
        //    Grafo<ProdutoPedidoEspelho, Grafo.RetalhoProducao> grafo = new Grafo<ProdutoPedidoEspelho, Grafo.RetalhoProducao>(true);
        //    grafo.DadosGrafo = dados;

        //    // Executa o algoritmo de otimização e preenche a variável de retorno
        //    foreach (KeyValuePair<ProdutoPedidoEspelho, Grafo.RetalhoProducao> fluxo in grafo.FluxoMaximo(true))
        //    {
        //        int index = dadosRetalho.FindIndex(x => Glass.Conversoes.StrParaUint(x.IdProdPed.TrimStart('R')) == fluxo.Key.IdProdPed && x.NumeroPeca == fluxo.Key.NumeroPeca);
        //        dadosRetalho[index].IdRetalhoProducao = fluxo.Value != null ? (uint?)fluxo.Value.IdRetalhoProducao : null;
        //    }
        //}

        /// <summary>
        /// Otimiza os retalhos de produção.
        /// </summary>
        /// <param name="dadosRetalho"></param>
        public static void Otimizar(ref List<DadosRetalho> dadosRetalho)
        {
            var idsRetalhosDisp = new List<uint>();

            // Limpa as referências dos retalhos atuais
            foreach (DadosRetalho d in dadosRetalho)
            {
                d.IdRetalhoProducao = null;
                idsRetalhosDisp.AddRange(d.IdsRetalhosPossiveis.Split(',').Select(f => f.StrParaUint()));
            }

            idsRetalhosDisp = idsRetalhosDisp.Distinct().ToList();

            var retalhos = RetalhoProducaoDAO.Instance.ObterLista(string.Join(",", idsRetalhosDisp.Select(f => f.ToString()).ToArray()));

            foreach (DadosRetalho d in dadosRetalho)
            {
                var retDisp = retalhos.Where(f => d.IdsRetalhosPossiveis.Split(',').Select(x => x.StrParaUint()).Contains(f.IdRetalhoProducao)).ToList();

                var ppe = ProdutosPedidoEspelhoDAO.Instance.
                    GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(d.IdProdPed.TrimStart('R')));

                retDisp = retDisp.Where(f => f.TotM >= (ppe.TotM / ppe.Qtde) && ((f.Largura >= ppe.Largura || f.Largura >= ppe.Altura) && (f.Altura >= ppe.Largura || f.Altura >= ppe.Altura))).ToList();

                if (retDisp.Count == 0)
                    continue;

                float dif = retDisp[0].TotM - (ppe.TotM / ppe.Qtde);
                d.IdRetalhoProducao = retDisp[0].IdRetalhoProducao;

                for (int i = 1; i < retDisp.Count; i++)
                {
                    if (retDisp[i].TotM - (ppe.TotM / ppe.Qtde) < dif)
                    {
                        dif = retDisp[i].TotM - (ppe.TotM / ppe.Qtde);
                        d.IdRetalhoProducao = retDisp[i].IdRetalhoProducao;
                    }
                }

                idsRetalhosDisp.Remove(d.IdRetalhoProducao.Value);
                retalhos = retalhos.Where(f => f.IdRetalhoProducao != d.IdRetalhoProducao.Value).ToList();
            }
        }
    }
}
