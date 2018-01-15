namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarLogCte : BaseFluxo<CadastrarLogCte>
    {
        private CadastrarLogCte() { }

        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="idCte"></param>
        /// <param name="evento"></param>
        /// <param name="codigo"></param>
        /// <param name="descricao"></param>
        /// <returns></returns>
        public uint NewLog(uint idCte, string evento, int codigo, string descricao)
        {
            using (Glass.Data.DAL.CTe.LogCteDAO dao = Glass.Data.DAL.CTe.LogCteDAO.Instance)
            {
                return dao.NewLog(idCte, evento, codigo, descricao);
            }
        }
    }
}
