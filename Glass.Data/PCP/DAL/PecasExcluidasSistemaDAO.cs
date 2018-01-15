using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PecasExcluidasSistemaDAO : BaseCadastroDAO<PecasExcluidasSistema, PecasExcluidasSistemaDAO>
    {
        //private PecasExcluidasSistemaDAO() { }

        private string ObtemStackTrace()
        {
            // Pula 2 métodos ("InserePecas" e "ObtemStackTrace")
            var t = new System.Diagnostics.StackTrace(2, true);

            List<string> trace = new List<string>();
            bool adicionarPontos = true;

            for (int i = 0; i < t.FrameCount; i++)
            {
                var f = t.GetFrame(i);

                // Só adiciona à lista os registros que tiverem arquivos vinculados
                // (arquivos do sistema)
                if (!String.IsNullOrEmpty(f.GetFileName()))
                {
                    trace.Add(f.ToString().Replace("\n", "").Replace("\r", ""));
                    adicionarPontos = true;
                }

                // Adiciona "..." se não houver referência de arquivos, para indicar
                // que há mais itens não mapeados no trace (itens do .NET)
                else if (adicionarPontos)
                {
                    trace.Add("...");
                    adicionarPontos = false;
                }
            }

            return String.Join("\r\n", trace.ToArray());
        }

        public void InserePecas(GDASession sessao, string idsProdPedProducao)
        {
            var ids = idsProdPedProducao.Split(',').Select(x => Glass.Conversoes.StrParaUint(x));
            if (ids.Count() == 0)
                return;

            string trace = ObtemStackTrace();

            foreach (var id in ids)
            {
                var item = new PecasExcluidasSistema()
                {
                    IdProdPed = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(id),
                    IdSetor = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<uint>("idSetor", "idProdPedProducao=" + id),
                    NumEtiqueta = ProdutoPedidoProducaoDAO.Instance.ObtemEtiqueta(id),
                    Situacao = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<int>("situacao", "idProdPedProducao=" + id),
                    Trace = trace
                };

                if (ProdutosPedidoEspelhoDAO.Instance.Exists(sessao, item.IdProdPed))
                    Insert(sessao, item);
            }
        }
    }
}
