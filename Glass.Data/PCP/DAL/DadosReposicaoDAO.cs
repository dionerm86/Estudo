using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class DadosReposicaoDAO : BaseDAO<DadosReposicao, DadosReposicaoDAO>
    {
        //private DadosReposicaoDAO() { }

        private int GetNumSeq(GDASession sessao, uint idProdPedProducao)
        {
            string sql = "select coalesce(max(numSeq),0)+1 from dados_reposicao where idProdPedProducao=" + idProdPedProducao;
            return ExecuteScalar<int>(sessao, sql);
        }

        /// <summary>
        /// Salva os dados de reposição atuais do produto_pedido_producao.
        /// </summary>
        /// <param name="ppp"></param>
        public void Empilha(GDASession sessao, uint idProdPedProducao)
        {
            if (!ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(sessao, idProdPedProducao, false))
                return;

            FilaOperacoes.DadosReposicao.AguardarVez();

            try
            {
                string where = "idProdPedProducao=" + idProdPedProducao;

                DadosReposicao novo = new DadosReposicao();
                novo.IdProdPedProducao = idProdPedProducao;
                novo.NumSeq = GetNumSeq(sessao, novo.IdProdPedProducao);
                novo.IdFuncRepos = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<uint>(sessao, "idFuncRepos", where);
                novo.IdSetorRepos = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<uint>(sessao, "idSetorRepos", where);
                novo.TipoPerdaRepos = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<int>(sessao, "tipoPerdaRepos", where);
                novo.IdSubtipoPerdaRepos = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<uint?>(sessao, "idSubtipoPerdaRepos", where);
                novo.DataRepos = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<DateTime>(sessao, "dataRepos", where);
                novo.DadosReposicaoPeca = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<string>(sessao, "dadosReposicaoPeca", where);
                novo.Obs = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<string>(sessao, "obs", where);
                novo.SituacaoProducao = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<int>(sessao, "situacaoProducao", where);

                Insert(sessao, novo);
            }
            finally
            {
                FilaOperacoes.DadosReposicao.ProximoFila();
            }
        }

        /// <summary>
        /// Recupera os últimos dados inseridos para um produto_pedido_producao.
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        public DadosReposicao Desempilha(GDASession sessao, uint idProdPedProducao)
        {
            FilaOperacoes.DadosReposicao.AguardarVez();

            try
            {
                string sql = "select * from dados_reposicao where idProdPedProducao=" + idProdPedProducao +
                    " order by numSeq desc limit 1";

                List<DadosReposicao> retorno = objPersistence.LoadData(sessao, sql);
                if (retorno.Count == 0)
                    return null;
                else
                {
                    objPersistence.ExecuteCommand(sessao, "Delete From dados_reposicao Where idDadosReposicao=" + retorno[0].IdDadosReposicao);
                    return retorno[0];
                }
            }
            finally
            {
                FilaOperacoes.DadosReposicao.ProximoFila();
            }
        }
    }
}
