using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Glass;

namespace WebGlass.Business.OrdemCarga.Fluxo
{
    public sealed class PendenciaCarregamentoFluxo : BaseFluxo<PendenciaCarregamentoFluxo>
    {
        /// <summary>
        ///  Recupera os clientes que tem itens pendentes de carregamento
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="idCli"></param>
        /// <param name="nomeCli"></param>
        /// <param name="idLoja"></param>
        /// <param name="dtSaidaIni"></param>
        /// <param name="dtSaidaFim"></param>
        /// <param name="rotas"></param>
        /// <returns></returns>
        public List<Entidade.ListagemPendenciaCarregamento> GetListagemPendenciaCarregamento(uint idCarregamento, uint idCli,
            string nomeCli, uint idLoja, string dtSaidaIni, string dtSaidaFim, string rotas, bool ignorarPedidosVendaTransferencia,
            uint idCliExterno, string nomeCliExterno, string idsRotasExternas, 
            string sortExpression, int startRow, int pageSize)
        {
            var itens = ItemCarregamentoDAO.Instance.GetItensPendentesStr(idCarregamento, 0, idCli, nomeCli, idLoja, null, null,
                false, dtSaidaIni, dtSaidaFim, rotas, ignorarPedidosVendaTransferencia, idCliExterno, nomeCliExterno, idsRotasExternas);

            var lst = new List<Entidade.ListagemPendenciaCarregamento>();

            var novosItens = itens.Select(f => new
            {
                IdCarregamento = f.Split(';')[0].StrParaUint(),
                IdCliente = f.Split(';')[1].StrParaUint(),
                NomeCliente = f.Split(';')[2],
                IdClienteExterno = f.Split(';')[3].StrParaUint(),
                ClienteExterno = f.Split(';')[4],
                PesoTotal = Math.Round(f.Split(';')[5].StrParaDecimal(), 2),
            }).ToList();

            foreach (var item in novosItens)
                lst.Add(new Entidade.ListagemPendenciaCarregamento(item.IdCarregamento,
                    item.IdCliente, item.NomeCliente, item.PesoTotal, item.IdClienteExterno, item.ClienteExterno));

            return lst.OrderByDescending(f => f.IdCarregamento).ThenBy(f => f.NomeCliente).Skip(startRow).Take(pageSize).ToList();

        }

        public int GetListagemPendenciaCarregamentoCount(uint idCarregamento, uint idCli,
            string nomeCli, uint idLoja, string dtSaidaIni, string dtSaidaFim, string rotas, bool ignorarPedidosVendaTransferencia,
            uint idCliExterno, string nomeCliExterno, string idsRotasExternas)
        {
            return ItemCarregamentoDAO.Instance.GetItensPendentesStr(idCarregamento, 0, idCli, nomeCli, idLoja, null, null,
                false, dtSaidaIni, dtSaidaFim, rotas, ignorarPedidosVendaTransferencia, idCliExterno, nomeCliExterno, idsRotasExternas)
                .Count;
        }
    }
}
