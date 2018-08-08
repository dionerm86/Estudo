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
        private bool _debitarIcmsDoIcmsSt, _calcularIpi;
        private GDA.GDASession _sessao;

        public CalculoIcmsStGeral(GDA.GDASession sessao, int idLoja, int? idCliente, int? idFornec, bool calcularIpi)
        {
            _idLoja = idLoja;
            _idCliente = idCliente;
            _idFornec = idFornec;
            _calcularIpi = calcularIpi;
            _suframaCliente = _idCliente > 0 ? ClienteDAO.Instance.ObtemValorCampo<string>(sessao, "Suframa", $"Id_Cli={ idCliente }") : null;
            _codIbgeCidadeCliente = _idCliente > 0 ? CidadeDAO.Instance.ObtemCodIbgeCompleto(sessao, ClienteDAO.Instance.ObtemIdCidade(sessao, (uint)_idCliente.Value)) : null;
            _debitarIcmsDoIcmsSt = true;
            _sessao = sessao;
        }

        public NFeUtils.ConfigNFe.TipoCalculoIcmsSt ObterCalculoAliquotaIcmsSt()
        {
            return _calcularIpi ? Configuracoes.FiscalConfig.NotaFiscalConfig.CalculoAliquotaIcmsSt : NFeUtils.ConfigNFe.TipoCalculoIcmsSt.SemIpi;
        }

        public float ObtemAliquotaInternaIcmsSt(Model.IProdutoIcmsSt produto, bool saida)
        {
            var dados = MvaProdutoUfDAO.Instance.ObterDadosParaBuscar(_sessao, (uint)_idLoja, _idFornec, (uint?)_idCliente, saida);
            var dadosIcms = IcmsProdutoUfDAO.Instance.ObtemPorProduto(_sessao, (uint)produto.IdProd, dados.UfOrigem, dados.UfDestino, dados.TipoCliente);

            if (dadosIcms == null)
            {
                return 0;
            }

            var tipoCalculoIcmsSt = ObterCalculoAliquotaIcmsSt();
            var aliqICMS = (decimal)dadosIcms.AliquotaInterestadual;
            var aliqICMSST = (decimal)produto.AliquotaIcmsSt;
            var aliqIPI = (decimal)produto.AliquotaIpi;

            if (aliqICMSST == 0)
            {
                aliqICMSST = (decimal)dadosIcms.AliquotaIntraestadual;
            }

            // SE a origem e destino forem iguais, iguala a alíquota interestadual com a intraestadual, para que o cálculo abaixo 
            // "(aliqICMSST - aliqICMS)" fique correto (zerado)
            if (!string.IsNullOrEmpty(dados.UfOrigem) && dados.UfOrigem == dados.UfDestino)
            {
                aliqICMS = aliqICMSST;
            }

            var mva = produto.MvaProdutoNf > 0 ? (decimal)produto.MvaProdutoNf :
                Math.Round((decimal)MvaProdutoUfDAO.Instance.ObterMvaPorProduto(_sessao, produto.IdProd, (uint)_idLoja, dados.UfOrigem, dados.UfDestino, dados.Simples, dados.TipoCliente, saida), 2);

            switch (tipoCalculoIcmsSt)
            {
                case NFeUtils.ConfigNFe.TipoCalculoIcmsSt.SemIpi:
                    {
                        return (float)((mva * aliqICMSST) / 100 + (aliqICMSST - aliqICMS));
                    }
                case NFeUtils.ConfigNFe.TipoCalculoIcmsSt.ComIpiNoCalculo:
                    {
                        return (float)((((aliqIPI * (1 + (mva / 100))) + mva) * aliqICMSST) / 100 + (aliqICMSST - aliqICMS));
                    }
                case NFeUtils.ConfigNFe.TipoCalculoIcmsSt.ComIpiEmbutidoNoPreco:
                    {
                        return (float)((((((aliqIPI * (1 + (mva / 100))) + mva) * aliqICMSST) / 100) + (aliqICMSST - aliqICMS)) / (1 + (aliqIPI / 100)));
                    }
                default:
                    {
                        throw new NotImplementedException($"Tipo de cálculo { tipoCalculoIcmsSt }");
                    }
            }
        }

        public decimal ObtemBaseCalculoIcmsSt(Model.IProdutoIcmsSt produto, bool saida)
        {
            var aliqIcmsProd = produto is Model.Produto ? ProdutoDAO.Instance.ObtemValorCampo<float>("AliqIcms", $"IdProd={ produto.IdProd }") : produto.AliquotaIcms;
            var baseCalc = produto.Total + produto.ValorIpi + produto.ValorFrete + produto.ValorSeguro + produto.ValorOutrasDespesas - produto.ValorDesconto;
            var tipoCalculoIcmsSt = ObterCalculoAliquotaIcmsSt();
            
            //Chamado: 54090
            if (tipoCalculoIcmsSt == NFeUtils.ConfigNFe.TipoCalculoIcmsSt.SemIpi)
            {
                baseCalc -= produto.ValorIpi;
            }

            // Se o cliente for do Amapá e se ele for da cidade de Amapá ou Santana, 
            // o valor do icms deve ser deduzido direto da BC do ICMS ST
            if (_idCliente > 0 && !string.IsNullOrEmpty(_suframaCliente))
            {
                if (_codIbgeCidadeCliente == "1600303" || _codIbgeCidadeCliente == "1600600")
                {
                    var valorIcmsADebitar = produto.ValorIcms > 0 ? produto.ValorIcms : (produto.Total - produto.ValorDesconto) * (decimal)(aliqIcmsProd / 100);
                    baseCalc -= valorIcmsADebitar;
                }
            }

            var mva = produto.MvaProdutoNf > 0 ? produto.MvaProdutoNf : Math.Round(MvaProdutoUfDAO.Instance.ObterMvaPorProduto(_sessao, produto.IdProd, (uint)_idLoja, _idFornec, (uint?)_idCliente, saida), 2);

            if (UserInfo.GetUserInfo.UfLoja == "AM")
            {
                if (produto.PercentualReducaoBaseCalculo > 0)
                {
                    baseCalc = baseCalc - ((decimal)(produto.PercentualReducaoBaseCalculo / 100) * baseCalc);
                }

                baseCalc *= (decimal)(1 + (mva / 100));
            }
            else
            {
                baseCalc *= (decimal)(1 + (mva / 100));

                if (produto.PercentualReducaoBaseCalculo > 0)
                {
                    baseCalc *= (decimal)(produto.PercentualReducaoBaseCalculo / 100);
                }
            }

            //Chamado: 54090
            if (tipoCalculoIcmsSt == NFeUtils.ConfigNFe.TipoCalculoIcmsSt.ComIpiEmbutidoNoPreco)
            {
                baseCalc -= produto.ValorIpi;
            }

            return baseCalc;
        }

        public float ObtemAliquotaIcmsSt(Model.IProdutoIcmsSt produto, bool saida)
        {
            var aliqIcmsProd = produto is Model.Produto ? produto.AliquotaIcms : IcmsProdutoUfDAO.Instance.ObterIcmsPorProduto(null, (uint)produto.IdProd, (uint)_idLoja, (uint?)_idFornec, (uint?)_idCliente);
            var aliqIcmsStProd = produto is Model.Produto ? produto.AliquotaIcmsSt : IcmsProdutoUfDAO.Instance.ObterAliquotaIcmsSt(_sessao, (uint)produto.IdProd, (uint)_idLoja, (uint?)_idFornec, (uint?)_idCliente);

            return produto.AliquotaIcmsSt > 0 ? produto.AliquotaIcmsSt : aliqIcmsStProd > 0 ? aliqIcmsStProd : produto.AliquotaIcms > 0 ? produto.AliquotaIcms : aliqIcmsProd;
        }

        public decimal ObtemValorIcmsSt(Model.IProdutoIcmsSt produto, bool saida)
        {
            var baseCalculo = ObtemBaseCalculoIcmsSt(produto, saida);
            var aliqIcmsProd = produto is Model.Produto ? produto.AliquotaIcms : IcmsProdutoUfDAO.Instance.ObterIcmsPorProduto(null, (uint)produto.IdProd, (uint)_idLoja, (uint?)_idFornec, (uint?)_idCliente);
            var aliqIcmsStProd = produto.AliquotaIcmsSt;
            var valorIcmsADebitar = (produto.ValorIcms > 0 ? produto.ValorIcms : ((produto.Total - produto.ValorDesconto) * (decimal)(aliqIcmsProd / 100)));

            return baseCalculo * ((decimal)aliqIcmsStProd / 100) - (_debitarIcmsDoIcmsSt ? valorIcmsADebitar : 0);
        }

        /// <summary>
        /// Recupera a alíquota do ICMSST interna,
        /// A alíquota interna é a alíquota do ICMSST ajustada desconsiderando o ICMS, considerando o IPI no cálculo ou embutido no preço,
        /// somada a alíquota do FCP e ajustada pelo MVA do produto.
        /// </summary>
        public string ObtemSqlAliquotaInternaIcmsSt(GDA.GDASession sessao, string idProd, string campoTotal, string campoValorDesconto, string campoAliquotaIcmsSt, string campoFastDelivery)
        {
            var dados = MvaProdutoUfDAO.Instance.ObterDadosParaBuscar(sessao, (uint)_idLoja, null, (uint?)_idCliente, true);
            var campo = string.Empty;
            var mva = dados.Simples ? "MvaSimples" : "MvaOriginal";
            var aliqIcmsSt = "(i.AliquotaIntra + COALESCE(i.AliquotaFCPIntraestadual, 0))";
            var icms = "(i.AliquotaInter + COALESCE(i.AliquotaFCPInterestadual, 0))";
            var simplesLoja = _idLoja > 0 && LojaDAO.Instance.ObtemValorCampo<int>(sessao, "Crt", $"IdLoja={ _idLoja }") <= 2; // 1 e 2 - simples
            var idCidadeLoja = LojaDAO.Instance.ObtemIdCidade(sessao, (uint)_idLoja);
            var ufLoja = CidadeDAO.Instance.GetNomeUf(sessao, idCidadeLoja);
            var idCidadeCliente = ClienteDAO.Instance.ObtemIdCidade(sessao, (uint)_idCliente);
            var ufCliente = CidadeDAO.Instance.GetNomeUf(sessao, idCidadeCliente);

            if (_idCliente > 0 && ufLoja == ufCliente)
            {
                icms = "(i.AliquotaIntra + COALESCE(i.AliquotaFCPIntraestadual, 0))";
            }

            // Não calcula MVA ajustada se a loja for Simples
            if (!simplesLoja && !string.Equals(dados.UfOrigem, dados.UfDestino, StringComparison.CurrentCultureIgnoreCase))
            {
                mva = $@"ROUND(IF(i.AliquotaInter <> i.AliquotaIntra, (((1 + (m.{ mva } / 100)) * (1 - (i.AliquotaInter / 100)) / (1 - (i.AliquotaIntra / 100))) - 1) * 100, { mva }),2)";
            }

            var tipoCalculoIcmsSt = ObterCalculoAliquotaIcmsSt();

            switch (tipoCalculoIcmsSt)
            {
                case NFeUtils.ConfigNFe.TipoCalculoIcmsSt.SemIpi:
                    {
                        campo = $"({ mva } * { aliqIcmsSt }) / 100 + ({ aliqIcmsSt } - { icms })";
                        break;
                    }
                case NFeUtils.ConfigNFe.TipoCalculoIcmsSt.ComIpiNoCalculo:
                    {
                        campo = $"(((p.AliqIPI * (1 + ({ mva } / 100))) + { mva }) * { aliqIcmsSt }) / 100 + ({ aliqIcmsSt } - { icms })";
                        break;
                    }
                case NFeUtils.ConfigNFe.TipoCalculoIcmsSt.ComIpiEmbutidoNoPreco:
                    {
                        campo = $"(((((p.AliqIPI * (1 + ({ mva } / 100))) + { mva }) * { aliqIcmsSt }) / 100) + ({ aliqIcmsSt } - { icms })) / (1 + (p.AliqIPI / 100))";
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException($"Tipo de cálculo { tipoCalculoIcmsSt }");
                    }
            }

            return $@"SELECT { campo }
                FROM produto p
                    INNER JOIN mva_produto_uf m ON (p.IdProd=m.IdProd AND m.UfOrigem='{ dados.UfOrigem }' AND m.UfDestino='{ dados.UfDestino }')
                    INNER JOIN icms_produto_uf i ON (p.IdProd=i.IdProd AND i.UfOrigem='{ dados.UfOrigem }' AND i.UfDestino='{ dados.UfDestino }'
                        { (dados.TipoCliente > 0 ? $"AND COALESCE(i.IdTipoCliente, { dados.TipoCliente })={ dados.TipoCliente }" : string.Empty) })
                WHERE p.IdProd={ idProd }
                ORDER BY COALESCE(i.IdTipoCliente, 0) DESC LIMIT 1";
        }

        public string ObtemSqlValorIcmsSt(string campoTotal, string campoValorDesconto, string campoAliquota, string campoFastDelivery)
        {
            campoValorDesconto = !string.IsNullOrWhiteSpace(campoValorDesconto) ? campoValorDesconto : "0";
            campoFastDelivery = !string.IsNullOrWhiteSpace(campoFastDelivery) ? campoFastDelivery : "1";

            return string.Format("COALESCE((({0}) - {1}) * {2}, 0) * COALESCE(({3}) / 100, 0)", campoTotal, campoValorDesconto, campoFastDelivery, campoAliquota);
        }
    }
}