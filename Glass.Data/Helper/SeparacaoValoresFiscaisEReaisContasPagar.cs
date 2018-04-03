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
    /// Classe com os métodos de separação de contas a pagar.
    /// </summary>
    public class SeparacaoValoresFiscaisEReaisContasPagar : SeparacaoValoresFiscaisEReais
    {
        private uint[] idsCompra;
        
        protected override void PodeSeparar(GDASession sessao)
        {
            if (!FinanceiroConfig.SepararValoresFiscaisEReaisContasPagar)
                throw new Exception("Configuração está desabilitada.");

           if (TipoDocumento == NotaFiscal.TipoDoc.Saída)
               throw new Exception("A separação de valores a pagar só é feita para notas fiscais de entrada.");

            CompraNotaFiscalDAO.Instance.PodeSepararContasPagarFiscaisEReais(sessao, IdNf, out idsCompra);
        }
        
        protected override bool CarregarContasReceber(GDASession sessao, ref ContasReceber[] contasReceber, ref string nomeCampo, out uint idLoja)
        {
            idLoja = 0;
            return false;
        }
        
        protected override bool CarregarContasPagar(GDASession sessao, ref ContasPagar[] contasPagar, ref string nomeCampo, out uint idLoja)
        {
            string idsContasPagar = null;

            #region Recupera os ids das contas geradas

            // Verifica se a NF-e foi gerada a partir de liberações
            if (idsCompra.Length > 0)
            {
                string idsString = String.Join(",", Array.ConvertAll(idsCompra, x => x.ToString()));
                idsContasPagar = ContasReceberDAO.Instance.GetValoresCampo(sessao, 
                    "select idContaPg from contas_pagar where idCompra in (" + idsString + ")",
                    "idContaPg");

                nomeCampo = "IdCompra";
            }

            #endregion

            if (!String.IsNullOrEmpty(idsContasPagar))
                contasPagar = ContasPagarDAO.Instance.GetByString(sessao, idsContasPagar);

            // Seleciona a loja das contas a pagar (se forem da mesma loja)
            // ou a loja do funcionário atual (se as contas forem de lojas diferentes)
            idLoja = contasPagar != null && contasPagar.Length > 0 && contasPagar.Select(x => x.IdLoja).Distinct().Count() == 1 ?
                contasPagar[0].IdLoja.GetValueOrDefault() : UserInfo.GetUserInfo.IdLoja;

            return contasPagar != null && contasPagar.Length > 0 &&
                contasPagar.Count(x => x.Paga) == 0;
        }
        
        protected override DadosPagamentoAntecipado[] ValoresPagosAntecipadamente(GDASession sessao)
        {
            return null;
        }
        
        protected override void CarregaParcelasReais(GDASession sessao, ref List<DadosParcelaReal> valores, decimal valorReal)
        {
            #region Carrega os valores para compra

            if (idsCompra.Length > 0)
            {
                using (CompraDAO dao = CompraDAO.Instance)
                    foreach (uint id in idsCompra)
                    {
                        decimal total = dao.ObtemValorCampo<decimal>(sessao, "total", "idCompra=" + id);

                        valores.Add(new DadosParcelaReal()
                        {
                            IdReferencia = id,
                            ValorVencimento = total / valorReal,
                            Reposicao = false
                        });
                    }
            }

            #endregion
        }

        protected override void ValidarCancelamentoContasReceber(GDASession session, ParcelaNaoFiscalOriginal[] parcelasOriginais, ref List<KeyValuePair<string, uint>> nomeEValorCampo)
        {
            nomeEValorCampo = null;
        }
        
        protected override void ValidarCancelamentoContasPagar(GDASession session, ParcelaNaoFiscalOriginal[] parcelasOriginais, ref List<KeyValuePair<string, uint>> nomeEValorCampo)
        {
            // Variável de retorno
            List<KeyValuePair<string, uint>> retorno = new List<KeyValuePair<string, uint>>();

            #region Compra

            // Busca os ids de liberação de pedido
            var ids = (from p in parcelasOriginais
                       where p.IdContaPg > 0 && p.IdCompra > 0
                       select p.IdCompra.Value).Distinct();

            // Adiciona os ids à variável de retorno
            foreach (var id in ids)
                retorno.Add(new KeyValuePair<string, uint>("IdCompra", id));

            // Valida as contas a pagar de compra
            if (ids.Count() > 0)
            {
                string idsString = String.Join(",", Array.ConvertAll(ids.ToArray(), x => x.ToString()));

                if (ContasPagarDAO.Instance.ExecuteScalar<int>(session, @"select count(*) from contas_pagar
                    where paga and idCompra in (" + idsString + ")") > 0)
                {
                    throw new Exception("A compra envolvida na NF-e possui uma ou mais contas recebidas. Cancele-as para prosseguir.");
                }
            }

            #endregion

            // Retorna a variável
            nomeEValorCampo = retorno;
        }
    }
}
