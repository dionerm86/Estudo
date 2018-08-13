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
            var filtro = "1=1";
            var campos = "s.IdSetor";
            var agrupamento = string.Empty;
            var limite = string.Empty;

            if (idProdPed > 0)
            {
                filtro = $"ppe.IdProdPed={ idProdPed }";
            }
            else if (!string.IsNullOrEmpty(idsProdPed))
            {
                filtro = idsProdPed;
            }
            else if (idAmbientePedido > 0)
            {
                filtro = $"ppe.IdAmbientePedido={ idAmbientePedido }";
            }

            if (usarJoin)
            {
                campos += ", rp.IdProcesso";
                agrupamento = "group by ep.idProcesso";
            }
            else
            {
                limite = "LIMIT 1";
            }

            return $@"SELECT { campos }
                FROM (
			        SELECT rp.IdRoteiroProducao, ep.IdProcesso
			        FROM produtos_pedido_espelho ppe
                        LEFT JOIN ambiente_pedido_espelho ape ON (ppe.IdAmbientePedido=ape.IdAmbientePedido)
                        LEFT JOIN pedido ped ON (ppe.IdPedido=ped.IdPedido)
                        INNER JOIN etiqueta_processo ep ON (if(ped.TipoPedido<>{ (int)Pedido.TipoPedidoEnum.MaoDeObra }, ppe.IdProcesso, ape.IdProcesso)=ep.IdProcesso)
                        INNER JOIN roteiro_producao rp ON (ep.IdProcesso=rp.IdProcesso)
			        WHERE { filtro }
                    { agrupamento }
			        { limite }
		        ) AS rp
                    INNER JOIN roteiro_producao_setor rps ON (rp.IdRoteiroProducao=rps.IdRoteiroProducao)
                    INNER JOIN setor s ON (rps.IdSetor=s.IdSetor AND s.Tipo IN ({ (int)TipoSetor.PorRoteiro }))";
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
