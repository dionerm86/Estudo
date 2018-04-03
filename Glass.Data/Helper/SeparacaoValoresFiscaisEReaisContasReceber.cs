using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Configuracoes;
using GDA;

namespace Glass.Data.Helper
{
    /// <summary>
    /// Classe com os métodos de separação de contas a receber.
    /// </summary>
    public class SeparacaoValoresFiscaisEReaisContasReceber : SeparacaoValoresFiscaisEReais
    {
        private uint[] idsLiberarPedido, idsPedido;
        
        protected override void PodeSeparar(GDASession sessao)
        {
            if (!FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
                throw new Exception("Configuração está desabilitada.");

            if (TipoDocumento != NotaFiscal.TipoDoc.Saída)
                throw new Exception("A separação de valores a receber só é feita para notas fiscais de saída.");

            PedidosNotaFiscalDAO.Instance.PodeSepararContasReceberFiscaisEReais(sessao, IdNf, out idsLiberarPedido, out idsPedido);
        }
        
        protected override bool CarregarContasReceber(GDASession sessao, ref ContasReceber[] contasReceber, ref string nomeCampo, out uint idLoja)
        {
            string idsContasReceber = null;

            #region Recupera os ids das contas geradas

            // Verifica se a NF-e foi gerada a partir de liberações
            if (idsLiberarPedido != null && idsLiberarPedido.Length > 0)
            {
                string idsString = String.Join(",", Array.ConvertAll(idsLiberarPedido, x => x.ToString()));
                idsContasReceber = ContasReceberDAO.Instance.GetValoresCampo(sessao, 
                    "select idContaR from contas_receber where idLiberarPedido in (" + idsString + ")",
                    "idContaR");

                nomeCampo = "IdLiberarPedido";
            }

            // NF-e de pedidosSS
            else if (idsPedido != null && idsPedido.Length > 0)
            {
                string idsString = String.Join(",", Array.ConvertAll(idsPedido, x => x.ToString()));
                idsContasReceber = ContasReceberDAO.Instance.GetValoresCampo(sessao, 
                    "select idContaR from contas_receber where idPedido in (" + idsString + ")",
                    "idContaR");

                nomeCampo = "IdPedido";
            }

            #endregion

            if (!String.IsNullOrEmpty(idsContasReceber))
                contasReceber = ContasReceberDAO.Instance.GetByPks(sessao, idsContasReceber).ToArray();

            // Seleciona a loja das contas a receber (se forem da mesma loja)
            // ou a loja do funcionário atual (se as contas forem de lojas diferentes)
            /* Chamado 52405. */
            idLoja = contasReceber != null && contasReceber.Length > 0 && contasReceber.Where(f => f.IdLoja > 0).Select(f => f.IdLoja).Distinct().Count() == 1 ?
                contasReceber.Where(f => f.IdLoja > 0).ToList()[0].IdLoja : UserInfo.GetUserInfo.IdLoja;

            return contasReceber != null && contasReceber.Length > 0 &&
                contasReceber.Count(x => x.Recebida) == 0;
        }
        
        protected override bool CarregarContasPagar(GDASession sessao, ref ContasPagar[] contasPagar, ref string nomeCampo, out uint idLoja)
        {
            idLoja = 0;
            return false;
        }
        
        protected override DadosPagamentoAntecipado[] ValoresPagosAntecipadamente(GDASession sessao)
        {
            string ids = "";

            // Verifica se a NF-e foi gerada a partir de liberações
            if (idsLiberarPedido.Length > 0)
            {
                string idsString = String.Join(",", Array.ConvertAll(idsLiberarPedido, x => x.ToString()));
                ids = String.Join(",", ProdutosLiberarPedidoDAO.Instance.GetByLiberacoes(sessao, idsString).Select(x => x.IdPedido.ToString()).Distinct().ToArray());
            }

            // NF-e de pedidos
            else if (idsPedido.Length > 0)
                ids = String.Join(",", Array.ConvertAll(idsPedido, x => x.ToString()));

            // Recupera os pedidos
            var pedidos = PedidoDAO.Instance.GetByString(sessao, ids);
            return pedidos.Select(x => new DadosPagamentoAntecipado()
            {
                Codigo = x.IdPedido,
                Valor = x.ValorPagamentoAntecipado + x.ValorEntrada
            }).ToArray();
        }
        
        protected override void CarregaParcelasReais(GDASession sessao, ref List<DadosParcelaReal> valores, decimal valorReal)
        {
            #region Carrega os valores para liberações

            if (idsLiberarPedido.Length > 0)
            {
                using (LiberarPedidoDAO dao = LiberarPedidoDAO.Instance)
                    foreach (uint id in idsLiberarPedido)
                    {
                        var total = dao.ObtemValorCampo<decimal>(sessao, "total", "idLiberarPedido=" + id);
                        
                        valores.Add(new DadosParcelaReal()
                        {
                            IdReferencia = id,
                            ValorVencimento = total / valorReal,
                            Reposicao = dao.ContemPedidosReposicao(sessao, id)
                        });
                    }
            }

            #endregion

            #region Carrega os valores para pedidos

            else if (idsPedido.Length > 0)
            {
                using (PedidoDAO dao = PedidoDAO.Instance)
                    foreach (uint id in idsPedido)
                    {
                        decimal total = dao.GetTotalParaLiberacao(sessao, id);

                        valores.Add(new DadosParcelaReal()
                        {
                            IdReferencia = id,
                            ValorVencimento = total / valorReal,
                            Reposicao = dao.IsPedidoReposicao(sessao, id.ToString())
                        });
                    }
            }

            #endregion
        }

        protected override void ValidarCancelamentoContasReceber(GDASession session, ParcelaNaoFiscalOriginal[] parcelasOriginais, ref List<KeyValuePair<string, uint>> nomeEValorCampo)
        {
            // Variável de retorno
            var retorno = new List<KeyValuePair<string, uint>>();

            #region Liberação de pedido

            // Busca os ids de liberação de pedido
            var ids = (from p in parcelasOriginais
                       where p.IdContaR > 0 && p.IdLiberarPedido > 0
                       select p.IdLiberarPedido.Value).Distinct();

            // Adiciona os ids à variável de retorno
            foreach (var id in ids)
                retorno.Add(new KeyValuePair<string, uint>("IdLiberarPedido", id));

            if (ids.Count() > 0)
            {
                // Valida as contas a receber de liberação
                string idsString = String.Join(",", Array.ConvertAll(ids.ToArray(), x => x.ToString()));

                if (ContasReceberDAO.Instance.ExecuteScalar<int>(session, @"select count(*) from contas_receber
                    where recebida and idLiberarPedido in (" + idsString + ")") > 0)
                {
                    throw new Exception("As liberações envolvidas na NF-e possuem uma ou mais contas recebidas. Cancele-as para prosseguir.");
                }
            }

            #endregion

            #region Pedido

            // Busca os ids de liberação de pedido
            ids = (from p in parcelasOriginais
                   where p.IdContaR > 0 && p.IdPedido > 0
                   select p.IdPedido.Value).Distinct();

            // Adiciona os ids à variável de retorno
            foreach (var id in ids)
                retorno.Add(new KeyValuePair<string, uint>("IdPedido", id));

            if (ids.Count() > 0)
            {
                // Valida as contas a receber de liberação
                string idsString = String.Join(",", Array.ConvertAll(ids.ToArray(), x => x.ToString()));

                if (ContasReceberDAO.Instance.ExecuteScalar<int>(session, @"select count(*) from contas_receber
                    where recebida and idPedido in (" + idsString + ")") > 0)
                {
                    throw new Exception("Os pedidos envolvidos na NF-e possuem uma ou mais contas recebidas. Cancele-as para prosseguir.");
                }
            }

            #endregion

            // Retorna a variável
            nomeEValorCampo = retorno;
        }
        
        protected override void ValidarCancelamentoContasPagar(GDASession session, ParcelaNaoFiscalOriginal[] parcelasOriginais, ref List<KeyValuePair<string, uint>> nomeEValorCampo)
        {
            nomeEValorCampo = null;
        }
    }
}
