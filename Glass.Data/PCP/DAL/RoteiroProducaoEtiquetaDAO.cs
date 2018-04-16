using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class RoteiroProducaoEtiquetaDAO : BaseDAO<RoteiroProducaoEtiqueta, RoteiroProducaoEtiquetaDAO>
    {
        //private RoteiroProducaoEtiquetaDAO() { }

        public void ApagarRoteiroEtiqueta(GDASession session, uint idProdPedProducao)
        {
            objPersistence.ExecuteCommand(session, "delete from roteiro_producao_etiqueta where idProdPedProducao=" + idProdPedProducao);
        }

        internal string Sql(uint? idProdPed, uint? idAmbientePedido, string idsProdPed, bool usarJoin)
        {
            return String.Format(@"
                select s.idSetor {5}
                from (
			        select rp.idRoteiroProducao, ep.idProcesso
			        from produtos_pedido_espelho ppe
                        left join ambiente_pedido_espelho ape on (ppe.idAmbientePedido=ape.idAmbientePedido)
                        left join pedido ped on (ppe.idPedido=ped.idPedido)

                        /* Substitui a parte removida acima */
                        inner join etiqueta_processo ep on (if(ped.tipoPedido<>{2}, ppe.idProcesso, ape.idProcesso)=ep.idProcesso)
                        inner join roteiro_producao rp on (ep.idProcesso=rp.idProcesso)

			        where {0}
                    {3}
			        /* order by rp.idGrupoProd desc, rp.idSubgrupoProd desc */
			        {4}
		        ) as rp
                    inner join roteiro_producao_setor rps on (rp.idRoteiroProducao=rps.idRoteiroProducao)
                    inner join setor s on (rps.idSetor=s.idSetor and s.tipo in ({1}))",

                (idProdPed > 0 ? "ppe.idProdPed=" + idProdPed : !String.IsNullOrEmpty(idsProdPed) ? idsProdPed : 
                    idAmbientePedido > 0 ? "ppe.idAmbientePedido=" + idAmbientePedido : "true"),
                (int)TipoSetor.PorRoteiro,
                (int)Pedido.TipoPedidoEnum.MaoDeObra,
                usarJoin ? "group by ep.idProcesso" : String.Empty,
                usarJoin ? String.Empty : "limit 1",
                usarJoin ? ", rp.idProcesso" : String.Empty);
        }

        /// <summary>
        /// Obtém os setores por peça já ordenados pelo número de sequência
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public List<Setor> ObtemSetoresPorPeca(GDASession session, uint? idProdPed, uint? idAmbientePedido)
        {
            var lstSetor = ExecuteMultipleScalar<uint>(session, Sql(idProdPed, idAmbientePedido, null, false))
                .Select(f => Utils.ObtemSetor(f))
                .Where(f => f != null)
                .OrderBy(f => f.NumeroSequencia)
                .ToList();

            return lstSetor;
        }

        public void InserirRoteiroEtiqueta(GDASession session, uint idProdPedProducao)
        {
            ApagarRoteiroEtiqueta(session, idProdPedProducao);

            uint idProdPed = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(session, idProdPedProducao);
            if (idProdPed == 0)
                return;

            var setores = ObtemSetoresPorPeca(session, idProdPed, null);
            if (setores.Count == 0)
                return;

            var ultimo = setores.
                OrderByDescending(x => x.NumeroSequencia).
                First().IdSetor;

            foreach (var s in setores)
            {
                var item = new RoteiroProducaoEtiqueta()
                {
                    IdProdPedProducao = idProdPedProducao,
                    IdSetor = (uint)s.IdSetor,
                    UltimoSetor = s.IdSetor == ultimo
                };

                RoteiroProducaoEtiquetaDAO.Instance.Insert(session, item);
            }
        }

        public IList<uint> ObtemSetoresEtiqueta(uint idProdPedProducao)
        {
            string sql = "select idSetor from roteiro_producao_etiqueta where idProdPedProducao=" + idProdPedProducao;
            return ExecuteMultipleScalar<uint>(sql);
        }

        /// <summary>
        /// Retorna o último setor do roteiro
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        public uint ObterUltimoSetor(uint idProdPedProducao)
        {
            return ExecuteScalar<uint>(string.Format("SELECT idSetor FROM roteiro_producao_etiqueta WHERE idProdPedProducao={0} AND UltimoSetor=TRUE", idProdPedProducao));
        }
    }
}
