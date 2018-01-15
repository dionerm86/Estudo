using WebGlass.Business.ConhecimentoTransporte.Fluxo;

namespace WebGlass.Business.ConhecimentoTransporte
{
    /// <summary>
    /// Classe de métodos utilizados no data source
    /// </summary>
    public class CteOds : BaseFluxo<CteOds>
    {
        private CteOds() { }

        public Entidade.Cte GetCte(uint idCte)
        {
            return BuscarCte.Instance.GetCte(idCte);
        }

        public Entidade.Cte[] GetList(int numeroCte, int idLoja, string situacao, uint idCfop, int formaPagto, int tipoEmissao,
            int tipoCte, int tipoServico, string dataEmiIni, string dataEmiFim, uint idTransportador, int ordenar, uint tipoRemetente, uint idRemetente,
            uint tipoDestinatario, uint idDestinatario, uint tipoRecebedor, uint idRecebedor, string sortExpression, int startRow, int pageSize)
        {
            return BuscarCte.Instance.GetList(numeroCte, idLoja, situacao, idCfop, formaPagto, tipoEmissao, tipoCte, tipoServico, dataEmiIni,
                dataEmiFim, idTransportador, ordenar, tipoRemetente, idRemetente, tipoDestinatario, idDestinatario, tipoRecebedor, idRecebedor, sortExpression, startRow, pageSize);
        }

        public int GetCount(int numeroCte, int idLoja, string situacao, uint idCfop, int formaPagto, int tipoEmissao, int tipoCte, 
            int tipoServico, string dataEmiIni, string dataEmiFim, uint idTransportador, int ordenar, uint tipoRemetente, uint idRemetente,
            uint tipoDestinatario, uint idDestinatario, uint tipoRecebedor, uint idRecebedor)
        {
            return BuscarCte.Instance.GetCount(numeroCte, idLoja, situacao, idCfop, formaPagto, tipoEmissao, tipoCte, tipoServico, 
                dataEmiIni, dataEmiFim, idTransportador, ordenar, tipoRemetente, idRemetente, tipoDestinatario, idDestinatario, tipoRecebedor, idRecebedor);
        }

        public int GetCount()
        {
            return BuscarCte.Instance.GetCount();
        }

        public uint Insert(Entidade.Cte cte)
        {
            return CadastrarCte.Instance.Insert(cte);
        }

        public void Update(Entidade.Cte cte)
        {
            CadastrarCte.Instance.Update(cte);
        }
    }
}
