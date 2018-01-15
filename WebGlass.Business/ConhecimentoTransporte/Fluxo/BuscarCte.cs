using GDA;
using System.Collections.Generic;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarCte : BaseFluxo<BuscarCte>
    {
        private BuscarCte() { }

        /// <summary>
        /// Busca cte pelo id
        /// </summary>
        public Entidade.Cte GetCte(uint idCte)
        {
            return GetCte(null, idCte);
        }

        /// <summary>
        /// Busca cte pelo id
        /// </summary>
        public Entidade.Cte GetCte(GDASession session, uint idCte)
        {
            using (Glass.Data.DAL.CTe.ConhecimentoTransporteDAO dao = Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance)
            {
                var cte = idCte > 0 ? new Entidade.Cte(dao.GetElement(session, idCte)) : new Entidade.Cte();

                cte.Cfop = Glass.Data.DAL.CfopDAO.Instance.GetCfop(session, cte.IdCfop);
                cte.ObjCobrancaCte = BuscarCobrancaCte.Instance.GetCobrancaCte(session, idCte);
                cte.ObjVeiculoCte = BuscarVeiculoCte.Instance.GetVeiculosCte(session, idCte);
                cte.ObjSeguroCte = BuscarSeguroCte.Instance.GetSeguroCte(session, idCte);
                cte.ObjEntregaCte = BuscarEntregaCte.Instance.GetEntregaCte(session, idCte);
                cte.ObjComponenteValorCte = BuscarComponenteValorCte.Instance.GetComponentesCte(session, idCte);
                cte.ObjInfoCte = BuscarInfoCte.Instance.GetInfoCte(session, idCte);
                cte.ObjImpostoCte = BuscarImpostoCte.Instance.GetImpostosCte(session, idCte);
                cte.ObjConhecimentoTransporteRodoviario = BuscarConhecimentoTransporteRodoviario.Instance.GetConhecimentoTransporteRodoviario(session, idCte);
                cte.ObjParticipanteCte = BuscarParticipanteCte.Instance.GetParticipantesCte(session, idCte);
                cte.ObjComplCte = BuscarComplCte.Instance.GetComplCte(session, idCte);
                cte.ObjEfdCte = BuscarEfdCte.Instance.GetEfdCte(session, idCte);

                return cte;
            }
        }

        /// <summary>
        /// Busca lista de cte pelos parâmetros
        /// </summary>
        /// <param name="numeroCte"></param>
        /// <param name="situacao"></param>
        /// <param name="idCfop"></param>
        /// <param name="formaPagto"></param>
        /// <param name="tipoEmissao"></param>
        /// <param name="tipoCte"></param>
        /// <param name="tipoServico"></param>
        /// <param name="dataEmiIni"></param>
        /// <param name="dataEmiFim"></param>
        /// <param name="ordenar"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Entidade.Cte[] GetList(int numeroCte, int idLoja, string situacao, uint idCfop,
            int formaPagto, int tipoEmissao, int tipoCte, int tipoServico, string dataEmiIni, string dataEmiFim, 
            uint idTransportador, int ordenar, uint tipoDestinatario, uint idDestinatario, uint tipoRecebedor, uint idRecebedor,
            string sortExpression, int startRow, int pageSize)
        {
            var retorno = new List<Entidade.Cte>();
            using (Glass.Data.DAL.CTe.ConhecimentoTransporteDAO dao = Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance)
            {
                var listaCte = dao.GetList(numeroCte, idLoja, situacao, idCfop, formaPagto, tipoEmissao, tipoCte, tipoServico,
                    dataEmiIni, dataEmiFim, idTransportador, ordenar, tipoDestinatario, idDestinatario, tipoRecebedor, idRecebedor,
                    sortExpression, startRow, pageSize);

                foreach (var item in listaCte)
                    retorno.Add(new WebGlass.Business.ConhecimentoTransporte.Entidade.Cte(item));
            }
            return retorno.ToArray();
        }

        public int GetUltimoNumeroCte(uint idLoja, int serie, int tipoEmissao)
        {
            return GetUltimoNumeroCte(null, idLoja, serie, tipoEmissao);
        }

        /// <summary>
        /// Pega o ultimo número de cte pelo id da loja, série e tipo de emissão do cte
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="serie"></param>
        /// <param name="tipoEmissao"></param>
        /// <returns></returns>
        public int GetUltimoNumeroCte(GDASession sessao, uint idLoja, int serie, int tipoEmissao)
        {
            using (Glass.Data.DAL.CTe.ConhecimentoTransporteDAO dao = Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance)
            {
                return dao.GetUltimoNumeroCte(sessao, idLoja, serie, tipoEmissao);
            }
        }

        /// <summary>
        /// Retorna quantidade de cte no banco de acordo com parametros
        /// </summary>
        /// <param name="numeroCte"></param>
        /// <param name="situacao"></param>
        /// <param name="idCfop"></param>
        /// <param name="formaPagto"></param>
        /// <param name="tipoEmissao"></param>
        /// <param name="tipoCte"></param>
        /// <param name="tipoServico"></param>
        /// <param name="dataEmiIni"></param>
        /// <param name="dataEmiFim"></param>
        /// <param name="ordenar"></param>
        /// <returns></returns>
        public int GetCount(int numeroCte, int idLoja, string situacao, uint idCfop, int formaPagto, int tipoEmissao,
            int tipoCte, int tipoServico, string dataEmiIni, string dataEmiFim, uint idTransportador, int ordenar,
            uint tipoDestinatario, uint idDestinatario, uint tipoRecebedor, uint idRecebedor)
        {
            using (Glass.Data.DAL.CTe.ConhecimentoTransporteDAO dao = Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance)
            {
                return dao.GetCount(numeroCte, idLoja, situacao, idCfop, formaPagto, tipoEmissao, 
                    tipoCte, tipoServico, dataEmiIni, dataEmiFim, idTransportador, ordenar,
                    tipoDestinatario, idDestinatario, tipoRecebedor, idRecebedor);
            }
        }

        /// <summary>
        /// Retorna quantidade de cte cadastrado
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            using (Glass.Data.DAL.CTe.ConhecimentoTransporteDAO dao = Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance)
            {
                return dao.GetCount();
            }
        }

        /// <summary>
        /// Obtém o motivo de cancelamento do cte
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public string ObtemMotivoCanc(uint idCte)
        {
            using (Glass.Data.DAL.CTe.ConhecimentoTransporteDAO dao = Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance)
            {
                return dao.ObtemMotivoCanc(idCte);
            }
        }

        /// <summary>
        /// Obtém o motivo de inutilização do cte
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public string ObtemMotivoInut(uint idCte)
        {
            using (Glass.Data.DAL.CTe.ConhecimentoTransporteDAO dao = Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance)
            {
                return dao.ObtemMotivoInut(idCte);
            }
        }

        public void Delete(Entidade.Cte cte)
        {
            ExcluirCte.Instance.Excluir(cte);
        }
    }
}
