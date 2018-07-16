using System;
using Glass.Data.DAL;
using System.Globalization;

namespace Glass.Data.Helper
{
    class CalculoIcmsStMT : ICalculoIcmsSt
    {
        private int _idCliente;
        private string _cnae;
        private float _margemLucro, _percCargaTributaria;
        
        public CalculoIcmsStMT(GDA.GDASession sessao, int idCliente, string cnae, float? percentualCargaTributaria)
        {
            _idCliente = idCliente;
            _cnae = cnae;
            _margemLucro = MargemLucroCnaeMTDAO.Instance.ObtemDadosMargemLucro(sessao, _cnae);
            /* Chamado 44504. */
            _percCargaTributaria = percentualCargaTributaria > 0 ? percentualCargaTributaria.Value : CargaTributariaMediaMTDAO.Instance.ObtemDadosCargaTributaria(sessao, _cnae);
        }

        public NFeUtils.ConfigNFe.TipoCalculoIcmsSt ObterCalculoAliquotaIcmsSt()
        {
            return Configuracoes.FiscalConfig.NotaFiscalConfig.CalculoAliquotaIcmsSt;
        }

        public float ObtemAliquotaInternaIcmsSt(Model.IProdutoIcmsSt produto, bool saida)
        {
            // Esse método ainda não é alterado com base nos tipos de cálculo de ICMS ST
            // conforme definido na tela de configurações
            var valor = ObtemValorIcmsSt(produto, saida);            
            return produto.Total > 0 ? (float)((valor / produto.Total) * 100) : 0;
        }

        public decimal ObtemBaseCalculoIcmsSt(Model.IProdutoIcmsSt produto, bool saida)
        {
            if (produto.AliquotaIcms == 0)
            {
                return 0;
            }

            var valorIcmsSt = ObtemValorIcmsSt(produto, saida);

            return (produto.ValorIcms + valorIcmsSt) / (decimal)(produto.AliquotaIcms / 100);
        }

        public float ObtemAliquotaIcmsSt(Model.IProdutoIcmsSt produto, bool saida)
        {
            if (produto.AliquotaIcms == 0)
            {
                return 0;
            }

            var valorIcmsSt = ObtemValorIcmsSt(produto, saida);
            var baseCalculo = ObtemBaseCalculoIcmsSt(produto, saida);

            return (float)(valorIcmsSt / baseCalculo) * 100;
        }

        public decimal ObtemValorIcmsSt(Model.IProdutoIcmsSt produto, bool saida)
        {
            if (produto.AliquotaIcms == 0)
            {
                return 0;
            }

            var baseCalculo = produto.Total + produto.ValorIpi + produto.ValorFrete + produto.ValorSeguro + produto.ValorOutrasDespesas - produto.ValorDesconto;
            var valorAgregado = baseCalculo * (decimal)(_margemLucro / 100);

            return valorAgregado * (decimal)(_percCargaTributaria / 100);
        }

        public string ObtemSqlAliquotaInternaIcmsSt(GDA.GDASession sessao, string idProd, string campoTotal, string campoValorDesconto, string campoAliquotaIcmsSt, string campoFastDelivery)
        {
            var valorIcmsSt = ObtemSqlValorIcmsSt(campoTotal, campoValorDesconto, campoAliquotaIcmsSt, campoFastDelivery);

            return $"({ valorIcmsSt } / { campoTotal }) * 100";
        }

        public string ObtemSqlValorIcmsSt(string campoTotal, string campoValorDesconto, string campoAliquotaIcmsSt, string campoFastDelivery)
        {
            campoValorDesconto = !string.IsNullOrWhiteSpace(campoValorDesconto) ? campoValorDesconto : "0";
            campoFastDelivery = !string.IsNullOrWhiteSpace(campoFastDelivery) ? campoFastDelivery : "1";

            var valorAgregado = $"COALESCE(({ campoTotal } - { campoValorDesconto }) * { campoFastDelivery }, 0) * COALESCE({ _margemLucro.ToString(CultureInfo.InvariantCulture) } / 100, 0)";

            return $"(SELECT { valorAgregado } * COALESCE({ _percCargaTributaria.ToString(CultureInfo.InvariantCulture) } / 100, 0))";
        }
    }
}