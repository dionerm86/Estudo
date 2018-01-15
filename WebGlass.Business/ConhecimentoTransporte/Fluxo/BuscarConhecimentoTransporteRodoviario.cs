using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarConhecimentoTransporteRodoviario : BaseFluxo<BuscarConhecimentoTransporteRodoviario>
    {
        private BuscarConhecimentoTransporteRodoviario() { }

        /// <summary>
        /// Busca dados do modal rodoviário pelo idCte
        /// </summary>
        public Entidade.ConhecimentoTransporteRodoviario GetConhecimentoTransporteRodoviario(uint idCte)
        {
            return GetConhecimentoTransporteRodoviario(null, idCte);
        }

        /// <summary>
        /// Busca dados do modal rodoviário pelo idCte
        /// </summary>
        public Entidade.ConhecimentoTransporteRodoviario GetConhecimentoTransporteRodoviario(GDASession session, uint idCte)
        {
            using (Glass.Data.DAL.CTe.ConhecimentoTransporteRodoviarioDAO dao = Glass.Data.DAL.CTe.ConhecimentoTransporteRodoviarioDAO.Instance)
            {
                var cteRod = new Entidade.ConhecimentoTransporteRodoviario(dao.GetElement(session, idCte));
                cteRod.ObjLacreCteRod = BuscarLacreCteRod.Instance.GetLacresCteRod(session, idCte);
                cteRod.ObjMotoristaCteRod = BuscarMotoristaCteRod.Instance.GetMotoristasCteRod(session, idCte);
                cteRod.ObjOrdemColetaCteRod = BuscarOrdemColetaCteRod.Instance.GetOrdensColetaCteRod(session, idCte);
                cteRod.ObjValePedagioCteRod = BuscarValePedagioCteRod.Instance.GetValesPedagioCteRod(session, idCte);

                return cteRod;
            }
        }
    }
}
