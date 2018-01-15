using System;
using Glass.Data.DAL;

namespace Glass.Data.Helper
{
    class CalculoIcmsStGeral : ICalculoIcmsSt
    {
        private int _idLoja;
        private int? _idCliente, _idFornec;
        private string _suframaCliente;
        private string _codIbgeCidadeCliente;
        private bool _debitarIcmsDoIcmsSt;

        public CalculoIcmsStGeral(GDA.GDASession sessao, int idLoja, int? idCliente, int? idFornec)
        {
            _idLoja = idLoja;
            _idCliente = idCliente;
            _idFornec = idFornec;
            _suframaCliente = _idCliente.HasValue && _idCliente.Value > 0 ? ClienteDAO.Instance.ObtemValorCampo<string>(sessao, "suframa", "id_Cli=" + idCliente) : null;
            _codIbgeCidadeCliente = _idCliente.HasValue && _idCliente.Value > 0 ? CidadeDAO.Instance.ObtemCodIbgeCompleto(sessao, ClienteDAO.Instance.ObtemIdCidade(sessao, (uint)_idCliente.Value)) : null;
            _debitarIcmsDoIcmsSt = idCliente > 0 ? Configuracoes.FiscalConfig.NotaFiscalConfig.DebitarIcmsDoIcmsStSeCliente : Configuracoes.FiscalConfig.NotaFiscalConfig.DebitarIcmsDoIcmsSt;
        }

        public float ObtemAliquotaInternaIcmsSt(Model.IProdutoIcmsSt produto, bool saida)
        {
            return ObtemAliquotaInternaIcmsSt(produto, false, saida);
        }

        public float ObtemAliquotaInternaIcmsSt(Model.IProdutoIcmsSt produto, bool incluirIpiNoCalculo, bool saida)
        {
            var tipoCalculo = !incluirIpiNoCalculo ?
                Configuracoes.FiscalConfig.NotaFiscalConfig.CalculoAliquotaIcmsSt :
                NFeUtils.ConfigNFe.TipoCalculoIcmsSt.ComIpiNoCalculo;

            var dados = MvaProdutoUfDAO.Instance.ObterDadosParaBuscar(null, (uint)_idLoja, _idFornec, (uint?)_idCliente, saida);
            var dadosIcms = IcmsProdutoUfDAO.Instance.ObtemPorProduto(null, (uint)produto.IdProd, dados.UfOrigem, dados.UfDestino, dados.TipoCliente);

            if (dadosIcms == null)
                return 0;

            decimal aliqICMS = (decimal)dadosIcms.AliquotaInterestadual;
            decimal aliqICMSST = (decimal)produto.AliquotaIcmsSt;
            decimal aliqIPI = (decimal)produto.AliquotaIpi;

            if (aliqICMSST == 0)
                aliqICMSST = (decimal)dadosIcms.AliquotaIntraestadual;

            // SE a origem e destino forem iguais, iguala a alíquota interestadual com a intraestadual, para que o cálculo abaixo 
            // "(aliqICMSST - aliqICMS)" fique correto (zerado)
            if (!String.IsNullOrEmpty(dados.UfOrigem) && dados.UfOrigem == dados.UfDestino)
                aliqICMS = aliqICMSST;

            decimal mva = produto.MvaProdutoNf > 0 ?
                (decimal)produto.MvaProdutoNf :
                Math.Round((decimal)MvaProdutoUfDAO.Instance.ObterMvaPorProduto(null, produto.IdProd, (uint)_idLoja, dados.UfOrigem, dados.UfDestino, 
                    dados.Simples, dados.TipoCliente, saida), 2);

            switch (tipoCalculo)
            {
                case NFeUtils.ConfigNFe.TipoCalculoIcmsSt.SemIpi:
                    return (float)((mva * aliqICMSST) / 100 + (aliqICMSST - aliqICMS));

                case NFeUtils.ConfigNFe.TipoCalculoIcmsSt.ComIpiNoCalculo:
                    return (float)((((aliqIPI * (1 + (mva / 100))) + mva) * aliqICMSST) / 100 + (aliqICMSST - aliqICMS));

                case NFeUtils.ConfigNFe.TipoCalculoIcmsSt.ComIpiEmbutidoNoPreco:
                    return (float)((((((aliqIPI * (1 + (mva / 100))) + mva) * aliqICMSST) / 100) + (aliqICMSST - aliqICMS)) / (1 + (aliqIPI / 100)));

                default:
                    throw new NotImplementedException("Tipo de cálculo " + tipoCalculo);
            }
        }

        public decimal ObtemBaseCalculoIcmsSt(Model.IProdutoIcmsSt produto, bool saida)
        {
            var aliqIcmsProd = 
                produto is Model.Produto ? 
                    ProdutoDAO.Instance.ObtemValorCampo<float>("aliqIcms", "idProd=" + produto.IdProd) :
                    produto.AliquotaIcms;

            var baseCalc = produto.Total + produto.ValorIpi + produto.ValorFrete + produto.ValorSeguro +
                produto.ValorOutrasDespesas - produto.ValorDesconto;

            // Se o cliente for do Amapá e se ele for da cidade de Amapá ou Santana, 
            // o valor do icms deve ser deduzido direto da BC do ICMS ST
            if (_idCliente > 0 && !String.IsNullOrEmpty(_suframaCliente))
            {
                if (_codIbgeCidadeCliente == "1600303" || _codIbgeCidadeCliente == "1600600")
                {
                    decimal valorIcmsADebitar = produto.ValorIcms > 0 ? 
                        produto.ValorIcms : 
                        (produto.Total - produto.ValorDesconto) * (decimal)(aliqIcmsProd / 100);

                    baseCalc -= valorIcmsADebitar;
                }
            }

            var mva = produto.MvaProdutoNf > 0 ?
                produto.MvaProdutoNf :
                Math.Round(MvaProdutoUfDAO.Instance.ObterMvaPorProduto(null, produto.IdProd, (uint)_idLoja, _idFornec,
                    (uint?)_idCliente, saida), 2);

            if (UserInfo.GetUserInfo.UfLoja == "AM")
            {
                if (produto.PercentualReducaoBaseCalculo > 0)
                    baseCalc = baseCalc - ((decimal) (produto.PercentualReducaoBaseCalculo/100)*baseCalc);

                baseCalc *= (decimal) (1 + (mva/100));
            }
            else
            {
                baseCalc *= (decimal) (1 + (mva/100));

                if (produto.PercentualReducaoBaseCalculo > 0)
                    baseCalc *= (decimal) (produto.PercentualReducaoBaseCalculo/100);
            }

            return baseCalc;
        }

        public float ObtemAliquotaIcmsSt(Model.IProdutoIcmsSt produto, bool saida)
        {
            var aliqIcmsProd = produto is Model.Produto ? produto.AliquotaIcms :
                IcmsProdutoUfDAO.Instance.ObterIcmsPorProduto(null, (uint)produto.IdProd, (uint)_idLoja, (uint?)_idFornec, (uint?)_idCliente);

            var aliqIcmsStProd = produto is Model.Produto ? produto.AliquotaIcmsSt :
                ProdutoDAO.Instance.ObtemValorCampo<float>("aliqIcmsSt", "idProd=" + produto.IdProd);

            return produto.AliquotaIcmsSt > 0 ? produto.AliquotaIcmsSt : 
                aliqIcmsStProd > 0 ? aliqIcmsStProd : 
                produto.AliquotaIcms > 0 ? produto.AliquotaIcms : 
                aliqIcmsProd;
        }

        public decimal ObtemValorIcmsSt(Model.IProdutoIcmsSt produto, bool saida)
        {
            var baseCalculo = ObtemBaseCalculoIcmsSt(produto, saida);
            var aliquota = produto.AliquotaIcmsSt;
            var aliqIcmsProd = produto is Model.Produto ? produto.AliquotaIcms :
                IcmsProdutoUfDAO.Instance.ObterIcmsPorProduto(null, (uint)produto.IdProd, (uint)_idLoja, (uint?)_idFornec, (uint?)_idCliente);

            var valorIcmsADebitar = (produto.ValorIcms > 0 ? produto.ValorIcms : ((produto.Total - produto.ValorDesconto) * (decimal)(aliqIcmsProd / 100)));

            return baseCalculo * ((decimal)aliquota / 100) - 
                (_debitarIcmsDoIcmsSt ? valorIcmsADebitar : 0);
        }

        public string ObtemSqlAliquotaInternaIcmsSt(GDA.GDASession sessao, string idProd, string campoTotal,
            string campoValorDesconto, string campoAliquotaIcmsSt)
        {
            var dados = MvaProdutoUfDAO.Instance.ObterDadosParaBuscar(sessao, (uint)_idLoja, null, (uint?)_idCliente, true);

            string campo;

            string mva = dados.Simples ? "mvaSimples" : "mvaOriginal";
            string aliqIcmsSt = "if(p.aliqIcmsSt>0, p.aliqIcmsSt, i.aliquotaIntra)";
            string icms = "aliquotaInter";
            var simplesLoja = _idLoja > 0 && LojaDAO.Instance.ObtemValorCampo<int>(sessao, "crt", "idLoja=" + _idLoja) <= 2; // 1 e 2 - simples

            if (_idCliente > 0 && CidadeDAO.Instance.GetNomeUf(sessao, LojaDAO.Instance.ObtemIdCidade((uint)_idLoja)) ==
                CidadeDAO.Instance.GetNomeUf(sessao, ClienteDAO.Instance.ObtemIdCidade(sessao, (uint)_idCliente)))
                icms = "aliquotaIntra";

            // Não calcula MVA ajustada se a loja for Simples
            if (!simplesLoja && !String.Equals(dados.UfOrigem, dados.UfDestino, StringComparison.CurrentCultureIgnoreCase))
                mva = String.Format("Round(If(i.aliquotaInter <> i.aliquotaIntra, (((1 + (m.{0} / 100)) * (1 - (i.aliquotaInter / 100)) / (1 - (i.aliquotaIntra / 100))) - 1) * 100, {0}),2)", mva);

            var configRateio = Configuracoes.FiscalConfig.NotaFiscalConfig.CalculoAliquotaIcmsSt;

            // Define se deverá embutir ou não o ipi no cálculo da st, desde que a empresa não rateie ipi ao gerar nota.
            if (_idCliente > 0 && !Configuracoes.FiscalConfig.NotaFiscalConfig.RatearIpiNfPedido)
            {
                configRateio = ClienteDAO.Instance.IsCobrarIpi(sessao, (uint)_idCliente) ?
                    NFeUtils.ConfigNFe.TipoCalculoIcmsSt.ComIpiNoCalculo :
                    NFeUtils.ConfigNFe.TipoCalculoIcmsSt.SemIpi;
            }

            switch (configRateio)
            {
                case NFeUtils.ConfigNFe.TipoCalculoIcmsSt.SemIpi:
                    campo = "(" + mva + " * " + aliqIcmsSt + ") / 100 + (" + aliqIcmsSt + " - i." + icms + ")";
                    break;

                case NFeUtils.ConfigNFe.TipoCalculoIcmsSt.ComIpiNoCalculo:
                    campo = "(((p.aliqIPI * (1 + (" + mva + " / 100))) + " + mva + ") * " + aliqIcmsSt + ") / 100 + (" + aliqIcmsSt + " - i." + icms + ")";
                    break;

                case NFeUtils.ConfigNFe.TipoCalculoIcmsSt.ComIpiEmbutidoNoPreco:
                    campo = "(((((p.aliqIPI * (1 + (" + mva + " / 100))) + " + mva + ") * " + aliqIcmsSt + ") / 100) + (" + aliqIcmsSt + " - i." + icms + ")) / (1 + (p.aliqIPI / 100))";
                    break;

                default:
                    throw new NotImplementedException("Tipo de cálculo " + Configuracoes.FiscalConfig.NotaFiscalConfig.CalculoAliquotaIcmsSt);
            }

            return String.Format(@"select {0}
                from produto p
                    inner join mva_produto_uf m on (p.idProd=m.idProd and
                        m.ufOrigem='{2}' and m.ufDestino='{3}')
                    inner join icms_produto_uf i on (p.idProd=i.idProd and
                        i.ufOrigem='{2}' and i.ufDestino='{3}' {4})
                where p.idProd={1}
                order by Coalesce(i.idTipoCliente, 0) desc limit 1",

                campo,
                idProd,
                dados.UfOrigem,
                dados.UfDestino,
                dados.TipoCliente > 0 ? String.Format("and coalesce(i.idTipoCliente, {0})={0}", dados.TipoCliente) : String.Empty);
        }

        public string ObtemSqlValorIcmsSt(string campoTotal, string campoValorDesconto, string campoAliquota)
        {
            return String.Format("coalesce({0} - {1}, 0) * coalesce(({2}) / 100, 0)",
                campoTotal, 
                !String.IsNullOrEmpty(campoValorDesconto) ? campoValorDesconto : "0",
                campoAliquota);
        }
    }
}