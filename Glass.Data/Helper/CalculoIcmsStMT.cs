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
            _percCargaTributaria = percentualCargaTributaria.HasValue ? percentualCargaTributaria.Value :
                CargaTributariaMediaMTDAO.Instance.ObtemDadosCargaTributaria(sessao, _cnae);
        }

        public float ObtemAliquotaInternaIcmsSt(Glass.Data.Model.IProdutoIcmsSt produto, bool saida)
        {
            return ObtemAliquotaInternaIcmsSt(produto, false, saida);
        }

        public float ObtemAliquotaInternaIcmsSt(Glass.Data.Model.IProdutoIcmsSt produto, bool incluirIpiNoCalculo, bool saida)
        {
            // Esse método ainda não é alterado com base nos tipos de cálculo de ICMS ST
            // conforme definido na tela de configurações

            var valor = ObtemValorIcmsSt(produto, saida);
            return (float)((valor / produto.Total) * 100);
        }

        public decimal ObtemBaseCalculoIcmsSt(Glass.Data.Model.IProdutoIcmsSt produto, bool saida)
        {
            if (produto.AliquotaIcms == 0)
                return 0;

            var valorIcmsSt = ObtemValorIcmsSt(produto, saida);
            return (produto.ValorIcms + valorIcmsSt) / (decimal)(produto.AliquotaIcms / 100);
        }

        public float ObtemAliquotaIcmsSt(Model.IProdutoIcmsSt produto, bool saida)
        {
            if (produto.AliquotaIcms == 0)
                return 0;

            var valorIcmsSt = ObtemValorIcmsSt(produto, saida);
            var baseCalculo = ObtemBaseCalculoIcmsSt(produto, saida);

            return (float)(valorIcmsSt / baseCalculo) * 100;
        }

        public decimal ObtemValorIcmsSt(Glass.Data.Model.IProdutoIcmsSt produto, bool saida)
        {
            if (produto.AliquotaIcms == 0)
                return 0;

            var baseCalculo = produto.Total + produto.ValorIpi + produto.ValorFrete + produto.ValorSeguro +
                produto.ValorOutrasDespesas - produto.ValorDesconto;

            decimal valorAgregado = baseCalculo * (decimal)(_margemLucro / 100);
            return valorAgregado * (decimal)(_percCargaTributaria / 100);
        }

        public string ObtemSqlAliquotaInternaIcmsSt(GDA.GDASession sessao, string idProd, string campoTotal, 
            string campoValorDesconto, string campoAliquotaIcmsSt)
        {
            string valorIcmsSt = ObtemSqlValorIcmsSt(campoTotal, campoValorDesconto, campoAliquotaIcmsSt);

            return String.Format("({0} / {1}) * 100",
                valorIcmsSt,
                campoTotal);
        }

        public string ObtemSqlValorIcmsSt(string campoTotal, string campoValorDesconto, string campoAliquotaIcmsSt)
        {
            string valorAgregado = String.Format("coalesce({0} - {1}, 0) * coalesce({2} / 100, 0)",
                campoTotal,
                !String.IsNullOrEmpty(campoValorDesconto) ? campoValorDesconto : "0",
                _margemLucro.ToString(CultureInfo.InvariantCulture));

            return String.Format("(select {0} * coalesce({1} / 100, 0))",
                valorAgregado,
                _percCargaTributaria.ToString(CultureInfo.InvariantCulture));
        }
    }
}