using Glass.Data.RelModel;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.Data.RelDAL
{
    public sealed class ProducaoContagemDAO : BaseDAO<ProducaoContagem, ProducaoContagemDAO>
    {
        //private ProducaoContagemDAO() { }

        private string Sql(uint idProdPedProducao, int idCarregamento, string idPedido, uint idProdPed, string codEtiqueta, string codRota,
            uint idImpressao, string codPedCli, uint idCliente, string nomeCliente, string dataIni, string dataFim, string dataIniEnt,
            string dataFimEnt, string dataIniFabr, string dataFimFabr, string dataIniConfPed, string dataFimConfPed, int idSetor,
            string situacao, int situacaoPedido, string tipoPedido, bool setoresAnteriores, bool setoresPosteriores, string idsSubgrupos,
            uint tipoEntrega, string pecasProdCanc, uint idFunc, uint idCorVidro, int altura, int largura, float espessura, string idsProc,
            string idsApl, uint fastDelivery, ProdutoPedidoProducaoDAO.TipoRetorno tipoRetorno, bool selecionar)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = ProdutoPedidoProducaoDAO.Instance.Sql(idProdPedProducao, null, idCarregamento, null, idPedido, null, idProdPed,
                codEtiqueta, codRota, idImpressao, codPedCli, idCliente, nomeCliente, dataIni, dataFim, dataIniEnt, dataFimEnt,
                dataIniFabr, dataFimFabr, dataIniConfPed, dataFimConfPed, idSetor, situacao, situacaoPedido, tipoPedido, setoresAnteriores,
                setoresPosteriores, false, idsSubgrupos, tipoEntrega, pecasProdCanc, idFunc, idCorVidro, altura, largura, espessura,
                idsProc, idsApl, null, tipoRetorno, 0, null, null, fastDelivery, false, selecionar, false, false, 0,
                ProdutoPedidoProducaoDAO.ProdutoComposicao.ProdutoSemIdProdPedParent, 0, 0, null, selecionar, out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?", filtroAdicional);

            if (selecionar)
                sql = @"select idPedido, idPedidoExibir, idSetor, descrSetor as nomeSetor, count(*) as numeroPecas, sum(totM2) as totM2, criterio
                    from (" + sql + @") as producao_contagem
                    group by idPedidoExibir, idSetor";

            return sql;
        }

        public ProducaoContagem[] GetForRpt(int idCarregamento, uint idPedido, uint idImpressao, string codPedCli, string codRota, uint idCliente,
            string nomeCliente, string numEtiqueta, string dataIni, string dataFim, string dataIniEnt, string dataFimEnt, string dataIniFabr, 
            string dataFimFabr, string dataIniConfPed, string dataFimConfPed, int idSetor, string situacao, int situacaoPedido, int tipoSituacoes, string idsSubgrupos, uint tipoEntrega,
            string pecasProdCanc, uint idFunc, string tipoPedido, uint idCorVidro, int altura, int largura, float espessura, string idsProc,
            string idsApl, bool aguardExpedicao, bool aguardEntrEstoque, uint fastDelivery)
        {
            bool situacoesAnteriores = tipoSituacoes == 1;
            bool situacoesPosteriores = tipoSituacoes == 2;

                var retorno = objPersistence.LoadData(Sql(0, idCarregamento, idPedido.ToString(), 0, numEtiqueta, codRota, idImpressao, 
                codPedCli, idCliente, nomeCliente, dataIni, dataFim, dataIniEnt, dataFimEnt, dataIniFabr, dataFimFabr, dataIniConfPed, dataFimConfPed, idSetor, situacao, 
                situacaoPedido, tipoPedido, situacoesAnteriores, situacoesPosteriores, idsSubgrupos, tipoEntrega, pecasProdCanc, idFunc, idCorVidro, 
                altura, largura, espessura, idsProc, idsApl, fastDelivery, aguardExpedicao ? ProdutoPedidoProducaoDAO.TipoRetorno.AguardandoExpedicao : 
                aguardEntrEstoque ? ProdutoPedidoProducaoDAO.TipoRetorno.EntradaEstoque : ProdutoPedidoProducaoDAO.TipoRetorno.Normal, true), 
                ProdutoPedidoProducaoDAO.Instance.GetParam(null, numEtiqueta, codRota, dataIni, dataFim, dataIniEnt, dataFimEnt, dataIniFabr,
                dataFimFabr, nomeCliente, codPedCli, null, null, espessura)).ToList();

            if (retorno.Count > 0)
                foreach (Setor s in Utils.GetSetores)
                {
                    ProducaoContagem item = new ProducaoContagem();
                    item.IdPedido = retorno[0].IdPedido;
                    item.IdPedidoExibir = retorno[0].IdPedidoExibir;
                    item.IdSetor = (uint)s.IdSetor;
                    item.NomeSetor = s.Descricao;
                    item.NumeroPecas = 0;
                    item.TotM2 = 0;
                    retorno.Add(item);
                }

            return retorno.ToArray();
        }
    }
}
