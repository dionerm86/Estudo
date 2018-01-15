using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class FluxoCaixaSinteticoDAO : BaseDAO<FluxoCaixaSintetico, FluxoCaixaSinteticoDAO>
    {
        //private FluxoCaixaSinteticoDAO() { }

        private FluxoCaixaSintetico GetItem(DateTime data, params FluxoCaixa[] itens)
        {
            if (itens.Length == 0)
                return null;

            var movEntrada = itens.Where(x => x.TipoMov == 1 && x.Valor > 0);
            var movSaida = itens.Where(x => x.TipoMov != 1 && x.Valor > 0);

            return new FluxoCaixaSintetico()
            {
                Criterio = itens[0].Criterio,
                Data = data,
                Descricao = itens[0].IsTotal ? itens[0].Descricao : null,
                IsTotal = itens[0].IsTotal,
                NumSeqMov = itens[0].NumSeqMov,
                PrevCustoFixo = itens[0].PrevCustoFixo,
                SaldoGeral = itens.OrderByDescending(x => x.NumSeqMov).FirstOrDefault().SaldoGeral,
                ValorEntrada = movEntrada.Count() > 0 ? (decimal?)movEntrada.Sum(x => x.Valor) : null,
                ValorSaida = movSaida.Count() > 0 ? (decimal?)movSaida.Sum(x => x.Valor) : null
            };
        }

        private FluxoCaixaSintetico[] GetFromFluxoCaixa(IEnumerable<FluxoCaixa> fluxoCaixa)
        {
            fluxoCaixa = fluxoCaixa.Where(x => !x.NaoExibirSintetico);

            List<FluxoCaixaSintetico> retorno = new List<FluxoCaixaSintetico>();

            foreach (var f in fluxoCaixa.Where(x => x.IsTotal))
                retorno.Add(GetItem(f.Data.Date, f));

            var datas = fluxoCaixa.Where(x => !x.IsTotal).Select(x => x.Data.Date).Distinct();

            foreach (DateTime data in datas)
            {
                var itens = (from f in fluxoCaixa
                             where f.Data.Date == data && !f.IsTotal
                             select f).ToArray();

                if (itens.Length > 0)
                    retorno.Add(GetItem(data, itens));
            }

            return retorno.ToArray();
        }

        public FluxoCaixaSintetico[] GetForRpt(string dataIni, string dataFim, bool prevCustoFixo, string tipoConta)
        {
            var fluxoCaixa = FluxoCaixaDAO.Instance.GetForRpt(dataIni, dataFim, prevCustoFixo, tipoConta);
            return GetFromFluxoCaixa(fluxoCaixa);
        }

        public FluxoCaixaSintetico[] GetList(string dataIni, string dataFim, bool prevCustoFixo, string tipoConta)
        {
            var fluxoCaixa = FluxoCaixaDAO.Instance.GetList(dataIni, dataFim, prevCustoFixo, tipoConta);
            return GetFromFluxoCaixa(fluxoCaixa);
        }
    }
}
