namespace WebGlass.Business.ConhecimentoTransporte
{
    /// <summary>
    /// Classe de métodos utilizados no data source
    /// </summary>
    public class LogCteOds : BaseFluxo<LogCteOds>
    {
        private LogCteOds() { }

        public uint NewLog(uint idCte, string evento, int codigo, string descricao)
        {
            return WebGlass.Business.ConhecimentoTransporte.Fluxo.CadastrarLogCte.Instance.NewLog(idCte, evento, codigo, descricao);
        }

        public Entidade.LogCte[] GetList(uint idCte, string sortExpression, int startRow, int pageSize)
        {
            return Fluxo.BuscarLogCte.Instance.GetList(idCte, sortExpression, startRow, pageSize);
        }

        public int GetCount(uint idCte)
        {
            return Fluxo.BuscarLogCte.Instance.GetCount(idCte);
        }

        public int ObtemUltimoCodigo(uint idCte)
        {
            return Fluxo.BuscarLogCte.Instance.ObtemUltimoCodigo(idCte);
        }
    }    
}
