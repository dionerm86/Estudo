using System;
using System.Collections.Generic;
using WebGlass.Business.CotacaoCompra.Entidade;
using Glass;

namespace WebGlass.Business.CotacaoCompra.Ajax
{
    public interface ICRUD
    {
        string Habilitar(string idCotacaoCompra, string idProd, string idFornec, 
            string custo, string prazo, string idParcela, string parcelasConfiguradas, string habilitar);
    }

    internal class CRUD : ICRUD
    {
        public string Habilitar(string idCotacaoCompra, string idProd, string idFornec,
            string custo, string prazo, string idParcela, string parcelasConfiguradas, string habilitar)
        {
            try
            {
                ProdutoFornecedorCotacaoCompra pfcc = new ProdutoFornecedorCotacaoCompra()
                {
                    CodigoCotacaoCompra = Glass.Conversoes.StrParaUint(idCotacaoCompra),
                    CodigoProduto = Glass.Conversoes.StrParaUint(idProd),
                    CodigoFornecedor = Glass.Conversoes.StrParaUint(idFornec),
                    CustoUnitario = Glass.Conversoes.StrParaDecimal(custo),
                    PrazoEntregaDias = Glass.Conversoes.StrParaInt(prazo),
                    CodigoParcela = Glass.Conversoes.StrParaInt(idParcela),

                    // Garante que o código da parcela possa ser -1
                    Cadastrado = true
                };

                bool h;
                if (bool.TryParse(habilitar, out h) && h)
                {
                    if (pfcc.CodigoParcela == -1 && !String.IsNullOrEmpty(parcelasConfiguradas))
                    {
                        string[] parcelas = parcelasConfiguradas.Split(';');
                        List<DateTime> datas = new List<DateTime>();

                        Array.ForEach(parcelas, x =>
                        {
                            if (!String.IsNullOrEmpty(x))
                                datas.Add(Conversoes.ConverteDataNotNull(x));
                        });

                        pfcc.DatasParcelasConfiguradas = datas.ToArray();
                    }

                    Fluxo.CRUD.Instance.HabilitarProdutoFornecedorCotacaoCompra(pfcc);
                }
                else
                    Fluxo.CRUD.Instance.DesabilitarProdutoFornecedorCotacaoCompra(pfcc);

                var item = Fluxo.CRUD.Instance.ObtemProdutosFornecedorCotacaoCompra(pfcc.CodigoCotacaoCompra, 
                    pfcc.CodigoFornecedor, pfcc.CodigoProduto, true, null, 0, 3);

                return "Ok|" + (item.Length > 0 ? item[0].CustoTotal.ToString("C") : "R$ 0,00");
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao habilitar/desabilitar.", ex);
            }
        }
    }
}
