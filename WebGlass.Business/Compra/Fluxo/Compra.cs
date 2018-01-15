using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;

namespace WebGlass.Business.Compra.Fluxo
{
    public sealed class Compra : BaseFluxo<Compra>
    {
        private Compra() { }

        public void CompraPcpInserted(uint idCompra, string[] dadosProdutos)
        {
            ProdutosPedidoEspelho prodPed;
            ProdutosCompra prodCompra;

            uint idFornec = CompraDAO.Instance.ObtemIdFornec(idCompra);

            foreach (string produtos in dadosProdutos)
            {
                string[] dados = produtos.Split(';');
                prodPed = ProdutosPedidoEspelhoDAO.Instance.GetElementForCompraPcp(Glass.Conversoes.StrParaUint(dados[0]));

                var prod = ProdutoDAO.Instance.GetByCodInterno(dados[1]);

                if (prod == null)
                    throw new System.Exception(string.Format("O produto de código {0} está inativo, portanto, ative-o para gerar uma compra de mercadoria para ele.", dados[1]));

                bool apenasBeneficiamentos = bool.Parse(dados[4]);

                prodCompra = new ProdutosCompra();
                prodCompra.IdCompra = idCompra;
                prodCompra.IdProdPed = prodPed.IdProdPed;

                decimal precoForn = ProdutoFornecedorDAO.Instance.GetCustoCompra((int)idFornec, prod.IdProd);
                decimal custoCompra = precoForn > 0 ? precoForn : prod.Custofabbase > 0 ? prod.Custofabbase : prod.CustoCompra;
                prodCompra.Valor = custoCompra;

                prodCompra.IdProd = (uint)prod.IdProd;
                prodCompra.Qtde = float.Parse(dados[2]);
                prodCompra.Largura = prodPed.LarguraProducao;
                prodCompra.Altura = prodPed.AlturaProducao;
                prodCompra.TotM = Glass.Global.CalculosFluxo.ArredondaM2Compra((int)prodPed.LarguraProducao, (int)prodPed.AlturaProducao, (int)prodCompra.Qtde);
                prodCompra.Redondo = prodPed.Redondo;
                prodCompra.Espessura = prodPed.Espessura > 0 ? prodPed.Espessura : prod.Espessura;
                prodCompra.NaoCobrarVidro = apenasBeneficiamentos;
                prodCompra.Obs = prodPed.ObsGrid;

                if (dados[3].Length > 0)
                {
                    List<uint> ids = new List<uint>();
                    foreach (string s in dados[3].Split(','))
                        ids.Add(Glass.Conversoes.StrParaUint(s));

                    GenericBenefCollection beneficiamentos = new GenericBenefCollection();
                    foreach (GenericBenef b in prodPed.Beneficiamentos)
                    {
                        if (ids.Contains(b.IdBenefConfig))
                        {
                            if (Glass.Configuracoes.CompraConfig.TelaCadastroPcp.UsarCustoBeneficiamento)
                            {
                                b.Valor = b.Custo;
                                b.ValorUnit = BenefConfigPrecoDAO.Instance.ObtemCustoBenef(null, b.IdBenefConfig, prodCompra.Espessura);
                            }

                            beneficiamentos.Add(b);
                        }
                    }

                    prodCompra.Beneficiamentos = beneficiamentos;
                }

                ProdutosCompraDAO.Instance.Insert(prodCompra);
            }
        }

        public int QuantidadeBenefCompraPcp(uint? idBenef, uint? idProdPed, int qtde)
        {
            int qtdeJaFeita = idBenef != null ? (
                idProdPed != null ? ProdutosCompraBenefDAO.Instance.GetCountByProdPedBenef(idProdPed.Value, idBenef.Value) : 0
            ) : 0;

            return qtde - qtdeJaFeita;
        }
    }
}
