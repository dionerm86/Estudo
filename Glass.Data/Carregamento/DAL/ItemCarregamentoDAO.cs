using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class ItemCarregamentoDAO : BaseCadastroDAO<ItemCarregamento, ItemCarregamentoDAO>
    {
        #region Obtem dados do item do carregamento

        private string SqlVolume(uint idCarregamento, uint idOC, uint idPedido, uint idCliente, string nomeCliente, string filtro,
            uint idCliExterno, string nomeCliExterno, uint idPedidoExterno, bool selecionar)
        {
            var nomeClienteBD = OrdemCargaConfig.ExibirRazaoSocialCliente ? 
                "Coalesce(c.nome, c.nomeFantasia)" : "Coalesce(c.nomeFantasia, c.nome)";

            string campos = @"ic.*, v.dataFechamento,
                    IF(sgp.TipoSubgrupo IN (" + (int)TipoSubgrupoProd.VidroDuplo + "," + (int)TipoSubgrupoProd.VidroLaminado + @"), 
                        (SELECT SUM(peso) FROM produtos_pedido WHERE IdProdPedParent = pp.IdProdPed) * vpp.qtde, SUM(pp.peso / pp.qtde * vpp.qtde))  as Peso,
                    c.id_cli as IdCliente, " + nomeClienteBD + @" as NomeCliente, oc.idOrdemCarga as idOc, ped.CodCliente as PedCli,
            ped.IdPedidoExterno, ped.IdClienteExterno, ped.ClienteExterno, ped.Importado as PedidoImportado";

            string sql = @"
                SELECT " + campos + @"
                FROM item_carregamento ic
                    INNER JOIN volume v ON (ic.idVolume = v.idVolume)
                    INNER JOIN volume_produtos_pedido vpp ON (v.idVolume = vpp.idVolume)
                    INNER JOIN produtos_pedido pp ON (vpp.idProdPed = pp.idProdPed)
                    INNER JOIN pedido ped ON (ic.idPedido = ped.idPedido)
                    INNER JOIN cliente c ON (ped.idCli = c.id_cli)
                    INNER JOIN produto p ON (pp.idProd = p.idProd)
                    LEFT JOIN subgrupo_prod sgp ON (p.idSubGrupoProd = sgp.idSubGrupoProd)
                    INNER JOIN (SELECT poc.idPedido, poc.idOrdemCarga 
                                FROM pedido_ordem_carga poc
                                    INNER JOIN ordem_carga oc ON (oc.idOrdemCarga = poc.idOrdemCarga)
                                WHERE oc.idCarregamento=" + idCarregamento + @") as oc ON (oc.idPedido=ped.idPedido)
                WHERE COALESCE(pp.invisivelFluxo, FALSE)=FALSE AND pp.IdProdPedParent IS NULL";


            if (idCarregamento > 0)
                sql += " AND ic.idCarregamento=" + idCarregamento;

            if (idOC > 0)
                sql += " AND oc.idOrdemCarga=" + idOC;

            if (idPedido > 0)
                sql += " AND ped.idPedido=" + idPedido;

            if (idCliente > 0)
            {
                sql += " AND c.id_cli=" + idCliente;
            }
            else if (!string.IsNullOrEmpty(nomeCliente))
            {
                var ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " AND c.id_cli IN(" + ids + ")";
            }

            if (!string.IsNullOrEmpty(filtro) && !filtro.Contains("1,2"))
            {
                var carregado = ("," + (filtro) + ",").Contains(",1,");
                sql += " AND ic.carregado=" + carregado.ToString().ToLower();
            }

            if (idCliExterno > 0)
            {
                sql += " AND ped.IdClienteExterno=" + idCliExterno;
            }
            else if (!string.IsNullOrEmpty(nomeCliExterno))
            {
                string ids = ClienteDAO.Instance.ObtemIdsClientesExternos(nomeCliExterno);

                if (!string.IsNullOrEmpty(ids))
                    sql += " AND ped.IdClienteExterno IN(" + ids + ")";
            }

            if (idPedidoExterno > 0)
                filtro += " AND ped.IdPedidoExterno = " + idPedidoExterno;

            sql += " GROUP BY ic.idVolume";

            if (selecionar)
                return sql;
            else
                return "SELECT COUNT(*) FROM (" + sql + ") as tmp";
        }

        private string SqlPeca(uint idCarregamento, uint idOC, uint idPedido, uint idCliente, string nomeCliente,
            string filtroCarregado, string numEtiqueta, int altura, int largura,
            uint idCliExterno, string nomeCliExterno, uint idPedidoExterno, bool selecionar)
        {
            var nomeClienteBD = OrdemCargaConfig.ExibirRazaoSocialCliente ?
                "Coalesce(c.nome, c.nomeFantasia)" : "Coalesce(c.nomeFantasia, c.nome)";

            string campos = selecionar ? @"ic.*, p.Descricao as DescrProduto, p.CodInterno, pp.Altura, pp.Largura, (pp.TotM / pp.qtde) as M2,
                IF(sgp.TipoSubgrupo IN (" + (int)TipoSubgrupoProd.VidroDuplo + "," + (int)TipoSubgrupoProd.VidroLaminado + @"), 
                        (SELECT SUM(peso) FROM produtos_pedido WHERE IdProdPedParent = pp.IdProdPed), pp.peso / pp.qtde)  as Peso,
                IF(ic.Carregado, {0}, null) as NumEtiqueta, {1} as PedidoEtiqueta, c.id_cli as IdCliente, " + nomeClienteBD + @" as NomeCliente,
                ppp.IdProdPed as IdProdPedEsp, oc.idOrdemCarga as idOc, ped.CodCliente as PedCli, ped.IdPedidoExterno, ped.IdClienteExterno, ped.ClienteExterno,
                ped.Importado as PedidoImportado" : "ic.IdItemCarregamento";

            string sqlItemVenda = @"
                SELECT " + string.Format(campos, "ppp.numEtiqueta", "ppp.numEtiqueta") + @"
                FROM item_carregamento ic
                    INNER JOIN produto_pedido_producao ppp ON (ic.idProdPedProducao = ppp.idProdPedProducao)
                    INNER JOIN produtos_pedido pp ON (ppp.idProdPed = pp.idProdPedEsp)
                    INNER JOIN produto p ON (pp.idProd = p.idProd)
                    LEFT JOIN subgrupo_prod sgp ON (p.idSubGrupoProd = sgp.idSubGrupoProd)
                    INNER JOIN pedido ped ON (ic.idPedido = ped.idPedido)
                    INNER JOIN cliente c ON (ped.idCli = c.id_cli)
                    INNER JOIN (SELECT poc.idPedido, poc.idOrdemCarga 
                                FROM pedido_ordem_carga poc
                                    INNER JOIN ordem_carga oc ON (oc.idOrdemCarga = poc.idOrdemCarga)
                                WHERE oc.idCarregamento=" + idCarregamento + @") as oc ON (oc.idPedido=ped.idPedido)
                WHERE COALESCE(pp.invisivelFluxo, FALSE)=FALSE 
                        AND COALESCE(sgp.produtosEstoque, FALSE)=FALSE AND pp.IdProdPedParent IS NULL {0}";

            string sqlItemRevenda = @"
                SELECT " + string.Format(campos, "COALESCE(ppp.numEtiqueta, pi.numEtiqueta)", "COALESCE(ppp.numEtiqueta, pi.numEtiqueta)") + @"
                FROM item_carregamento ic
                    INNER JOIN produtos_pedido pp ON (ic.idPedido = pp.idPedido)
                    INNER JOIN produto p ON (pp.idProd = p.idProd)
                    LEFT JOIN subgrupo_prod sgp ON (p.idSubGrupoProd = sgp.idSubGrupoProd)
                    INNER JOIN pedido ped ON (ic.idPedido = ped.idPedido)
                    INNER JOIN cliente c ON (ped.idCli = c.id_cli)
                    LEFT JOIN produto_pedido_producao ppp ON (ppp.idpedidoExpedicao IS NOT NULL
                        AND ppp.idprodpedproducao = ic.idprodpedproducao)
                    INNER JOIN (SELECT poc.idPedido, poc.idOrdemCarga 
                                FROM pedido_ordem_carga poc
                                    INNER JOIN ordem_carga oc ON (oc.idOrdemCarga = poc.idOrdemCarga)
                                WHERE oc.idCarregamento=" + idCarregamento + @") as oc ON (oc.idPedido=ped.idPedido)
                    LEFT JOIN produto_impressao pi ON (pi.IdProdImpressao = ic.IdProdImpressaoChapa)
                WHERE COALESCE(pp.invisivelFluxo, FALSE)=FALSE 
                        AND COALESCE(sgp.produtosEstoque, FALSE)=TRUE
                        AND pp.idProd = ic.idProd AND pp.IdProdPedParent IS NULL {0}";

            sqlItemRevenda += " GROUP BY ic.idItemCarregamento";

            string filtro = "";

            if (idCarregamento > 0)
                filtro += " AND ic.idCarregamento=" + idCarregamento;

            if (idOC > 0)
                filtro += " AND oc.idOrdemCarga=" + idOC;

            if (idPedido > 0)
                filtro += " AND ped.idPedido=" + idPedido;

            if (idCliente > 0)
            {
                filtro += " AND c.id_cli=" + idCliente;
            }
            else if (!string.IsNullOrEmpty(nomeCliente))
            {
                var ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                filtro += " AND c.id_cli IN(" + ids + ")";
            }

            if (!string.IsNullOrEmpty(filtroCarregado) && !filtroCarregado.Contains("1,2"))
            {
                var carregado = ("," + (filtroCarregado) + ",").Contains(",1,");
                filtro += " AND ic.carregado=" + carregado.ToString().ToLower();
            }

            if (!string.IsNullOrEmpty(numEtiqueta))
                filtro += " AND ppp.numEtiqueta = ?etq";

            if (altura > 0)
                filtro += " AND pp.Altura = " + altura;

            if (largura > 0)
                filtro += " AND pp.largura = " + largura;

            if (idCliExterno > 0)
            {
                filtro += " AND ped.IdClienteExterno=" + idCliExterno;
            }
            else if (!string.IsNullOrEmpty(nomeCliExterno))
            {
                string ids = ClienteDAO.Instance.ObtemIdsClientesExternos(nomeCliExterno);

                if (!string.IsNullOrEmpty(ids))
                    filtro += " AND ped.IdClienteExterno IN(" + ids + ")";
            }

            if (idPedidoExterno > 0)
                filtro += " AND ped.IdPedidoExterno = " + idPedidoExterno;

            sqlItemVenda = string.Format(sqlItemVenda, filtro);
            sqlItemRevenda = string.Format(sqlItemRevenda, filtro);

            var sql = sqlItemVenda + " union all " + sqlItemRevenda;

            return selecionar ? sql : "select count(*) from (" + sql + ") as tmp";
        }

        private string SqlItensPendentes(uint idCarregamento, uint idPedido, uint idCli, string nomeCli, uint idLoja,
            bool? volume, bool? itemRevenda, bool agruparProdutos, string dtSaidaIni, string dtSaidaFim, string rotas,
            bool ignorarPedidosVendaTransferencia, uint idCliExterno, string nomeCliExterno, string codRotasExternas)
        {
            var nomeClienteBD = OrdemCargaConfig.ExibirRazaoSocialCliente ?
                "Coalesce(c.nome, c.nomeFantasia)" : "Coalesce(c.nomeFantasia, c.nome)";

            string sqlvolume = @"
                SELECT ic.*, null as DescrProduto, null as CodInterno, null as Altura, null as Largura, null as M2, 
                    IF(sgp.TipoSubgrupo IN (" + (int)TipoSubgrupoProd.VidroDuplo + "," + (int)TipoSubgrupoProd.VidroLaminado + @"), 
                        (SELECT SUM(peso) FROM produtos_pedido WHERE IdProdPedParent = pp.IdProdPed) * vpp.qtde, SUM(pp.peso / pp.qtde * vpp.qtde))  as Peso,
                    c.id_cli as IdCliente, " + nomeClienteBD + @" as NomeCliente, v.dataFechamento, ped.IdClienteExterno, ped.ClienteExterno, ped.IdPedidoExterno, ped.Importado as PedidoImportado,
                    r.IdRota, r.Descricao as Rota, c.Importacao as ClienteImportacao"
                + (agruparProdutos ? @", null as Qtde, '$$$' as criterio " : "") + @", 1 as tipoItem, ped.deveTransferir
                FROM item_carregamento ic
                    INNER JOIN volume v ON (ic.idVolume = v.idVolume)
                    INNER JOIN volume_produtos_pedido vpp ON (v.idVolume = vpp.idVolume)
                    INNER JOIN produtos_pedido pp ON (vpp.idProdPed = pp.idProdPed)
                    INNER JOIN pedido ped ON (ic.idPedido = ped.idPedido)
                    INNER JOIN cliente c ON (ped.idCli = c.id_cli)
                    INNER JOIN carregamento car ON (ic.idCarregamento = car.idCarregamento)
                    LEFT JOIN rota_cliente rc ON (c.Id_Cli = rc.IdCliente)
                    LEFT JOIN rota r ON (rc.IdRota = r.IdRota)
                    INNER JOIN produto p ON (pp.idProd = p.idProd)
                    LEFT JOIN subgrupo_prod sgp ON (p.idSubGrupoProd = sgp.idSubGrupoProd)
                WHERE COALESCE(pp.invisivelFluxo, FALSE)=FALSE
                    AND pp.IdProdPedParent IS NULL {0}
                GROUP BY ic.idVolume";

            string campos = @"ic.*, p.Descricao as DescrProduto, p.CodInterno, pp.Altura, pp.Largura, (pp.TotM / pp.qtde)" + (agruparProdutos ? "* COUNT(*)" : "") + @" as M2,
                IF(sgp.TipoSubgrupo IN (" + (int)TipoSubgrupoProd.VidroDuplo + "," + (int)TipoSubgrupoProd.VidroLaminado + @"),
                    (SELECT SUM(peso) FROM produtos_pedido WHERE IdProdPedParent = pp.IdProdPed), pp.peso / pp.qtde) " + (agruparProdutos ? " * COUNT(*)" : "") + @" as Peso,
                c.id_cli as IdCliente, " + nomeClienteBD + @" as NomeCliente, null as dataFechamento, ped.IdClienteExterno, ped.ClienteExterno, ped.IdPedidoExterno, ped.Importado as PedidoImportado,
                r.IdRota, r.Descricao as Rota, c.Importacao as ClienteImportacao"
                                   + (agruparProdutos ? @", COUNT(*) as Qtde, '$$$' as criterio" : "");

            string sqlItemVenda = @"
                SELECT " + campos + @", 2 as tipoItem, ped.deveTransferir
                FROM item_carregamento ic
                    INNER JOIN produto_pedido_producao ppp ON (ic.idProdPedProducao = ppp.idProdPedProducao)
                    INNER JOIN produtos_pedido pp ON (ppp.idProdPed = pp.idProdPedEsp)
                    INNER JOIN produto p ON (pp.idProd = p.idProd)
                    LEFT JOIN subgrupo_prod sgp ON (p.idSubGrupoProd = sgp.idSubGrupoProd)
                    INNER JOIN pedido ped ON (ic.idPedido = ped.idPedido)
                    INNER JOIN cliente c ON (ped.idCli = c.id_cli)
                    INNER JOIN carregamento car ON (ic.idCarregamento = car.idCarregamento)
                    LEFT JOIN rota_cliente rc ON (c.Id_Cli = rc.IdCliente)
                    LEFT JOIN rota r ON (rc.IdRota = r.IdRota)
                WHERE COALESCE(pp.invisivelFluxo, FALSE)=FALSE 
                        AND COALESCE(sgp.produtosEstoque, FALSE)=FALSE 
                        AND pp.IdProdPedParent IS NULL {0} "
                         + (agruparProdutos ? "GROUP BY car.idCarregamento, p.idProd, pp.Altura, pp.Largura, ic.IdPedido" : "");

            string sqlItemRevenda = @"
                SELECT " + campos + @", 3 as tipoItem, ped.deveTransferir
                FROM item_carregamento ic
                    INNER JOIN produtos_pedido pp ON (ic.idPedido = pp.idPedido)
                    INNER JOIN produto p ON (pp.idProd = p.idProd)
                    LEFT JOIN subgrupo_prod sgp ON (p.idSubGrupoProd = sgp.idSubGrupoProd)
                    INNER JOIN pedido ped ON (ic.idPedido = ped.idPedido)
                    INNER JOIN cliente c ON (ped.idCli = c.id_cli)
                    INNER JOIN carregamento car ON (ic.idCarregamento = car.idCarregamento)
                    LEFT JOIN rota_cliente rc ON (c.Id_Cli = rc.IdCliente)
                    LEFT JOIN rota r ON (rc.IdRota = r.IdRota)
                WHERE COALESCE(pp.invisivelFluxo, FALSE)=FALSE 
                        AND COALESCE(sgp.produtosEstoque, FALSE)=TRUE
                        AND pp.idProd = ic.idProd 
                        AND pp.IdProdPedParent IS NULL{0}
                GROUP BY "
                         + (agruparProdutos ? "car.idCarregamento, p.idProd, pp.Altura, pp.Largura, ic.IdPedido" : "ic.idItemCarregamento");

            string sql = "";

            if (volume.HasValue && volume.Value)
                sql += sqlvolume;
            else if (itemRevenda.HasValue && itemRevenda.Value)
                sql += sqlItemRevenda;
            else if (itemRevenda.HasValue && !itemRevenda.Value)
                sql += sqlItemVenda;
            else
                sql += sqlvolume + " UNION ALL " + sqlItemVenda + " UNION ALL " + sqlItemRevenda + "";

            string filtro = " AND ic.carregado = FALSE";
            string criterio = "";

            if (idCarregamento > 0)
            {
                filtro += " AND ic.idCarregamento=" + idCarregamento;
                criterio += "Carregamento: " + idCarregamento + "     ";
            }

            if (idPedido > 0)
            {
                filtro += " and ic.idPedido=" + idPedido;
                criterio += "Pedido: " + idPedido + "     ";
            }

            if (idCli > 0)
            {
                filtro += " AND c.id_Cli=" + idCli;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCli) + "     ";
            }
            else if (!string.IsNullOrEmpty(nomeCli))
            {
                var ids = ClienteDAO.Instance.GetIds(null, nomeCli, null, 0, null, null, null, null, 0);
                filtro += " AND c.id_Cli IN(" + ids + ")";
                criterio += "Cliente: " + nomeCli + "     ";
            }

            if (idLoja > 0)
            {
                filtro += " AND car.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "     ";
            }

            bool temDataInicial = false;
            if (!string.IsNullOrEmpty(dtSaidaIni))
            {
                filtro += " AND car.dataPrevistaSaida >=?dtSaidaIni";
                criterio += "Data Prev. Saída: " + dtSaidaIni + "     ";
                temDataInicial = true;
            }

            if (!string.IsNullOrEmpty(dtSaidaFim))
            {
                filtro += " AND car.dataPrevistaSaida <=?dtSaidaFim";
                criterio += (temDataInicial ? " à " : "Data Prev. Saída: ") + dtSaidaFim + "     ";
            }

            if (!string.IsNullOrEmpty(rotas))
            {
                filtro += " AND (SELECT COUNT(*) FROM rota_cliente rc1 WHERE rc1.IdCliente=c.Id_Cli And rc1.IdRota IN (" + rotas + ")) > 0";
                criterio += "Rota: " + RotaDAO.Instance.ObtemDescrRotas(rotas) + "     ";
            }


            if (idCliExterno > 0)
            {
                filtro += " AND ped.IdClienteExterno=" + idCliExterno;
                criterio += "Cliente Externo: " + idCliExterno + "   ";
            }
            else if (!string.IsNullOrEmpty(nomeCliExterno))
            {
                string ids = ClienteDAO.Instance.ObtemIdsClientesExternos(nomeCliExterno);

                if (!string.IsNullOrEmpty(ids))
                    filtro += " AND ped.IdClienteExterno IN(" + ids + ")";

                criterio += "Cliente Externo: " + nomeCliExterno + "   ";
            }

            if (!string.IsNullOrEmpty(codRotasExternas))
            {
                var idsRotas = string.Join(",", codRotasExternas.Split(',').Select(f => "'" + f + "'").ToArray());
                filtro += " AND ped.RotaExterna IN (" + idsRotas + ")";
                criterio += "Rota Externo: " + codRotasExternas + "   ";
            }

            sql =  string.Format(sql, filtro).Replace("$$$", criterio);

            if (ignorarPedidosVendaTransferencia)
            {
                sql = @"SELECT * 
                        FROM (" + sql + @") as tmp
                            LEFT JOIN (SELECT poc.idPedido as idPedidoTmp1, oc.idCarregamento as idCarregamentoTmp1, oc.tipoOrdemCarga as tipoOrdemCargaTmp1
                                       FROM ordem_carga oc
                                            INNER JOIN pedido_ordem_carga poc ON (oc.idOrdemCarga = poc.idOrdemCarga)
                                      ) as tmp1 ON (tmp1.idPedidoTmp1 = tmp.idPedido and tmp1.idCarregamentoTmp1 = tmp.idCarregamento)

                            LEFT JOIN (SELECT v.idVolume as idVolumeTmp2, poc.idPedido as pedidoVolume, oc.idCarregamento as idCarregamentoTmp2, oc.tipoOrdemCarga as tipoOrdemCargaTmp2
                                       FROM ordem_carga oc
                                            INNER JOIN pedido_ordem_carga poc ON (oc.idOrdemCarga = poc.idOrdemCarga)
                                            INNER JOIN pedido p ON (p.idPedido = poc.idPedido)
                                            INNER JOIN volume v ON (v.idPedido = p.idPedido)
                                       ) as tmp2 ON (tmp2.pedidoVolume = tmp.idPedido AND
                                                        tmp2.idCarregamentoTmp2 = tmp.idCarregamento AND
                                                        tmp2.idVolumeTmp2 = tmp.idVolume)
                        WHERE IF(tmp.deveTransferir, COALESCE(tmp1.tipoOrdemCargaTmp1," + (int)OrdemCarga.TipoOCEnum.Transferencia +") = " + (int)OrdemCarga.TipoOCEnum.Transferencia + @"
                                AND COALESCE(tmp2.tipoOrdemCargaTmp2," + (int)OrdemCarga.TipoOCEnum.Transferencia +") = " + (int)OrdemCarga.TipoOCEnum.Transferencia + ", true)";
            }

            return sql;
        }

        #region Busca os itens do carregamento para expedição

        public IList<ItemCarregamento> GetItensCarregamentoForExpedicao(uint idCarregamento, uint idOC, uint idPedido, uint idCliente,
            string nomeCliente, string filtro, bool volume, string numEtiqueta, int altura, int largura, uint idCliExterno, string nomeCliExterno, uint idPedidoExterno,
            string sortExpression, int startRow, int pageSize)
        {
            string sql;

            if (volume && (!string.IsNullOrEmpty(numEtiqueta) || altura > 0 || largura > 0))
                return new List<ItemCarregamento>();

            if (volume)
                sql = SqlVolume(idCarregamento, idOC, idPedido, idCliente, nomeCliente, filtro, idCliExterno, nomeCliExterno, idPedidoExterno, true);
            else
            {
                sql = SqlPeca(idCarregamento, idOC, idPedido, idCliente, nomeCliente, filtro, numEtiqueta, altura, largura, idCliExterno, nomeCliExterno, idPedidoExterno, true);
                if (string.IsNullOrEmpty(sortExpression))
                    sortExpression = "M2 DESC";
            }

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, GetParams(numEtiqueta));
        }

        public int GetItensCarregamentoForExpedicaoCount(uint idCarregamento, uint idOC, uint idPedido, uint idCliente,
            string nomeCliente, string filtro, bool volume, string numEtiqueta, int altura, int largura, uint idCliExterno, string nomeCliExterno, uint idPedidoExterno)
        {
            string sql;

            if (volume && (!string.IsNullOrEmpty(numEtiqueta) || altura > 0 || largura > 0))
                return 0;

            if (volume)
                sql = SqlVolume(idCarregamento, idOC, idPedido, idCliente, nomeCliente, filtro, idCliExterno, nomeCliExterno, idPedidoExterno, false);
            else
                sql = SqlPeca(idCarregamento, idOC, idPedido, idCliente, nomeCliente, filtro, numEtiqueta, altura, largura, idCliExterno, nomeCliExterno, idPedidoExterno, false);

            return objPersistence.ExecuteSqlQueryCount(sql, GetParams(numEtiqueta));
        }

        private GDAParameter[] GetParams(string numEtiqueta)
        {
            List<GDAParameter> retorno = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(numEtiqueta))
                retorno.Add(new GDAParameter("?etq", numEtiqueta));

            return retorno.ToArray();
        }

        #endregion

        #region Busca dados do carregamento para expedição

        public List<ItemCarregamento> GetDadosCarregamentoForExpedicao(uint idCarregamento, uint idOC, uint idPedido, uint idCliente,
            string nomeCliente, bool volume, string numEtiqueta, int altura, int largura, uint idCliExterno, string nomeCliExterno, uint idPedidoExterno)
        {
            string sql;

            if (volume && (!string.IsNullOrEmpty(numEtiqueta) || altura > 0 || largura > 0))
                return new List<ItemCarregamento>();

            if (volume)
                sql = SqlVolume(idCarregamento, idOC, idPedido, idCliente, nomeCliente, null, idCliExterno, nomeCliExterno, idPedidoExterno, true);
            else
                sql = SqlPeca(idCarregamento, idOC, idPedido, idCliente, nomeCliente, null, numEtiqueta, altura, largura, idCliExterno, nomeCliExterno, idPedidoExterno, true);

            return objPersistence.LoadData(sql, GetParams(numEtiqueta));
        }

        #endregion

        /// <summary>
        /// Retorna o id do item do carregamento da etiqueta informada
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="etiqueta"></param>
        /// <returns></returns>
        private uint GetIdItemCarregamento(GDASession sessao, uint idCarregamento, string etiqueta)
        {
            string sql = @"
                SELECT idItemCarregamento
                FROM item_carregamento ic
                WHERE ic.idCarregamento=" + idCarregamento;

            if (etiqueta.ToUpper().Substring(0, 1).Equals("V"))
                sql += " AND ic.idVolume=" + etiqueta.Substring(1);
            else if (etiqueta.ToUpper().Substring(0, 1).Equals("N") || etiqueta.ToUpper().Substring(0, 1).Equals("R"))
                sql += " AND ic.IdProdImpressaoChapa = " + ProdutoImpressaoDAO.Instance.ObtemIdProdImpressaoParaCarregamento(sessao, etiqueta);
            else
                sql += " AND ic.idProdPedProducao=" + ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(sessao, etiqueta).Value;

            return ExecuteScalar<uint>(sessao, sql);
        }

        /// <summary>
        /// Retorna os ids dos itens do carregamento informado.
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <returns></returns>
        public string GetIdsItemCarregamento(GDASession sessao, uint idCarregamento)
        {
            var itens = ExecuteMultipleScalar<uint>(sessao, @"
                SELECT idItemCarregamento 
                FROM item_carregamento 
                WHERE idCarregamento=" + idCarregamento);

            return string.Join(",", itens.Select(f=>f.ToString()).ToArray());
        }

        /// <summary>
        /// Retorna o id do carregamento de um item
        /// </summary>
        /// <param name="idItemCarregamento"></param>
        /// <returns></returns>
        public uint GetIdCarregamento(GDASession sessao, uint idItemCarregamento)
        {
            return ObtemValorCampo<uint>(sessao, "idCarregamento", "idItemCarregamento=" + idItemCarregamento);
        }

        /// <summary>
        /// Recupera um item do carregamento
        /// </summary>
        /// <param name="idItemCarregamento"></param>
        /// <returns></returns>
        public ItemCarregamento GetElement(GDASession sessao, uint idItemCarregamento)
        {
            if(!Exists(sessao, idItemCarregamento))
                return null;

           return objPersistence.LoadOneData(sessao, "select * from item_carregamento where idItemCarregamento="+idItemCarregamento);
        }

        public List<ItemCarregamento> GetByIdProdPedProducao(GDASession sessao, uint idProdPedProducao)
        {
            return objPersistence.LoadData(sessao, "SELECT * FROM item_carregamento WHERE idProdPedProducao=" + idProdPedProducao).ToList();
        }

        /// <summary>
        /// Recupera os itens pendentes de carregamento
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="idPedido"></param>
        /// <param name="idCli"></param>
        /// <param name="nomeCli"></param>
        /// <param name="idLoja"></param>
        /// <param name="volume"></param>
        /// <param name="itemRevenda"></param>
        /// <param name="agruparProdutos"></param>
        /// <param name="dtSaidaIni"></param>
        /// <param name="dtSaidaFim"></param>
        /// <param name="rotas"></param>
        /// <returns></returns>
        public List<ItemCarregamento> GetItensPendentes(uint idCarregamento, uint idPedido, uint idCli, string nomeCli, uint idLoja,
            bool? volume, bool? itemRevenda, bool agruparProdutos, string dtSaidaIni, string dtSaidaFim, string rotas,
            bool ignorarPedidosVendaTransferencia, uint idCliExterno, string nomeCliExterno, string idsRotasExternas)
        {
            var sql = SqlItensPendentes(idCarregamento, idPedido, idCli, nomeCli, idLoja, volume, itemRevenda,
                agruparProdutos, dtSaidaIni, dtSaidaFim, rotas, ignorarPedidosVendaTransferencia, idCliExterno, nomeCliExterno, idsRotasExternas);

            return objPersistence.LoadData(sql, GetParamsItensPendentes(dtSaidaIni, dtSaidaFim));
        }

        /// <summary>
        /// Recupera os itens pendentes de carregamento
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="idPedido"></param>
        /// <param name="idCli"></param>
        /// <param name="nomeCli"></param>
        /// <param name="idLoja"></param>
        /// <param name="volume"></param>
        /// <param name="itemRevenda"></param>
        /// <param name="agruparProdutos"></param>
        /// <param name="dtSaidaIni"></param>
        /// <param name="dtSaidaFim"></param>
        /// <param name="rotas"></param>
        /// <returns></returns>
        public List<string> GetItensPendentesStr(uint idCarregamento, uint idPedido, uint idCli, string nomeCli, uint idLoja,
            bool? volume, bool? itemRevenda, bool agruparProdutos, string dtSaidaIni, string dtSaidaFim, string rotas,
            bool ignorarPedidosVendaTransferencia, uint idCliExterno, string nomeCliExterno, string idsRotasExternas)
        {
            var sql = SqlItensPendentes(idCarregamento, idPedido, idCli, nomeCli, idLoja, volume, itemRevenda,
                agruparProdutos, dtSaidaIni, dtSaidaFim, rotas, ignorarPedidosVendaTransferencia, idCliExterno, nomeCliExterno, idsRotasExternas);

            sql = @"
                    SELECT CONCAT(IdCarregamento, ';', IdCliente, ';', NomeCliente, ';', COALESCE(IdClienteExterno, 0), ';', COALESCE(ClienteExterno, ''), ';', SUM(Peso))
                    FROM (" + sql + @") as tmp
                    GROUP BY IdCarregamento, IdCliente, IdClienteExterno, ClienteExterno
                    ORDER BY NomeCliente";

            return ExecuteMultipleScalar<string>(sql, GetParamsItensPendentes(dtSaidaIni, dtSaidaFim));
        }

        private GDAParameter[] GetParamsItensPendentes(string dtSaidaIni, string dtSaidaFim)
        {
            var lstParams = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dtSaidaIni))
                lstParams.Add(new GDAParameter("?dtSaidaIni", DateTime.Parse(dtSaidaIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dtSaidaFim))
                lstParams.Add(new GDAParameter("?dtSaidaFim", DateTime.Parse(dtSaidaFim + " 23:59:59")));

            return lstParams.ToArray();
        }

        #endregion

        #region Verifica se o produto/volume foi carregado

        /// <summary>
        /// Verifica se um volume esta carregado
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="idVolume"></param>
        /// <returns></returns>
        public bool VolumeCarregado(GDASession sessao, uint idCarregamento, uint idVolume)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM item_carregamento
                WHERE carregado=TRUE AND idVolume=" + idVolume + " AND idCarregamento=" + idCarregamento;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se a etiqueta esta carregada.
        /// </summary>
        public bool PecaCarregada(GDASession sessao, uint idCarregamento, string codEtiqueta)
        {
            var idItemCarregamento = GetIdItemCarregamento(sessao, idCarregamento, codEtiqueta);

            var sql = string.Format("SELECT COUNT(*) FROM item_carregamento WHERE Carregado=1 AND IdFuncLeitura > 0 AND DataLeitura IS NOT NULL AND IdItemCarregamento={0}", idItemCarregamento);

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se a chapa já foi carregada.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdImpressaoChapa"></param>
        /// <returns></returns>
        public int? ChapaCarregada(GDASession sessao, uint idProdImpressaoChapa)
        {
            string sql = @"
                SELECT IdCarregamento
                FROM item_carregamento
                WHERE Carregado = 1 AND IdProdImpressaoChapa = " + idProdImpressaoChapa;

            return ExecuteScalar<int?>(sessao, sql);
        }

        #endregion

        #region Verifica o vinculo da peça ao carregamento

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se a peça é do carragamento informado
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="etiqueta"></param>
        /// <returns></returns>
        public bool PecaEstaNoCarregamento(uint idCarregamento, string etiqueta)
        {
            return PecaEstaNoCarregamento(null, idCarregamento, etiqueta);
        }

        /// <summary>
        /// Verifica se a peça é do carragamento informado
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="etiqueta"></param>
        /// <returns></returns>
        public bool PecaEstaNoCarregamento(GDASession sessao, uint idCarregamento, string etiqueta)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM item_carregamento ic
	                INNER JOIN produto_pedido_producao ppp ON (ic.idProdPedProducao = ppp.idProdPedProducao)
                WHERE ic.idCarregamento=" + idCarregamento + " AND ppp.numEtiqueta = ?codEtiqueta";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, new GDAParameter("?codEtiqueta", etiqueta)) > 0;
        }

        #endregion

        #region Verifica o vinculo do volume ao carregamento

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se o volume faz parte do carregamento informado.
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="idVolume"></param>
        /// <returns></returns>
        public bool VolumeEstaNoCarregamento(uint idCarregamento, uint idVolume)
        {
            return VolumeEstaNoCarregamento(null, idCarregamento, idVolume);
        }

        /// <summary>
        /// Verifica se o volume faz parte do carregamento informado.
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="idVolume"></param>
        /// <returns></returns>
        public bool VolumeEstaNoCarregamento(GDASession sessao, uint idCarregamento, uint idVolume)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM item_carregamento ic
                WHERE ic.idVolume =" + idVolume + " AND ic.idCarregamento=" + idCarregamento;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Verifica a quantidade a ser carregada de um produto

        /// <summary>
        /// Verifica a quantidade a ser carregada de um produto
        /// </summary>
        public int ObtemQtdeProdutoFaltaCarregar(GDASession sessao, uint idCarregamento, uint idPedido, string etiqueta, uint? idProd)
        {
            if (idProd.GetValueOrDefault(0) == 0)
                idProd = ProdutoDAO.Instance.ObtemIdProdByEtiqueta(sessao, etiqueta);

            string sql = @"
                SELECT COUNT(*)
                FROM item_carregamento ic
                where ic.idCarregamento=" + idCarregamento 
                                          + " AND ic.idProd=" + idProd 
                                          + " AND ic.idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql);
        }

        /// <summary>
        /// Verifica a quantidade a ser carregada de um produto
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCarregamento"></param>
        /// <param name="idPedido"></param>
        /// <param name="etiqueta"></param>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public int ObtemQtdeProdutoFaltaCarregar(uint idCarregamento, uint idPedido, string etiqueta, uint? idProd)
        {
            return ObtemQtdeProdutoFaltaCarregar(null, idCarregamento, idPedido, etiqueta, idProd);
        }

        #endregion

        #region Cria os itens do carregamento

        /// <summary>
        /// Cria os itens do carregamento
        /// </summary>
        public void CriaItensCarregamento(GDASession sessao, uint idCarregamento, string idsOCs, string idsPeds)
        {
            List<uint> lstIdsPedido;
            var idsPedido = String.Empty;
            var erro = String.Empty;
            var sqlVenda = String.Empty;
            var sqlVolume = String.Empty;
            var usuCad = UserInfo.GetUserInfo.CodUser;
            var dataCad = DateTime.Now;

            #region Recupera o código dos pedidos

                if (!string.IsNullOrEmpty(idsPeds))
                {
                    idsPedido = idsPeds;
                }
                else
                {
                    if (!string.IsNullOrEmpty(idsOCs))
                        lstIdsPedido = PedidoDAO.Instance.GetIdsPedidosByOCs(sessao, idsOCs);
                    else
                        lstIdsPedido = PedidoDAO.Instance.GetIdsPedidosByCarregamento(sessao, idCarregamento);

                    idsPedido = string.Join(",", lstIdsPedido.Select(i => i.ToString()).ToArray());
                }

                // Caso nenhum pedido tenha sido recuperado, lança uma exceção.
                if (String.IsNullOrEmpty(idsPedido))
                    throw new Exception("Nenhum carregamento/ordem de carga/pedido informado possui itens de carregamento a serem gerados.");

            #endregion

            #region SQL's para recuperar as peças/volumes

            // Sql que seleciona todas as peças de pedido do tipo venda que devem gerar item de carregamento.
            sqlVenda =
                "SELECT " + idCarregamento + @" AS idCarregamento, pp.idPedido, pp.idProd, ppp.idProdPedProducao, FALSE AS Carregado, FALSE AS Entregue,
                    ?dataCad as dataCad, " + usuCad + @" as usuCad
                FROM produtos_pedido pp
                    INNER JOIN produto_pedido_producao ppp ON (pp.idProdPedEsp = ppp.idProdPed)
                WHERE COALESCE(pp.invisivelFluxo, FALSE)=FALSE 
                    AND ppp.situacao = " + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @"
                    AND ppp.numEtiqueta is not null 
	                AND pp.idPedido IN (" + idsPedido + @")
                    AND (pp.IdProdPedParent IS NULL OR pp.IdProdPedParent=0)";
                
            // Sql que seleciona todos os volumes que devem gerar item de carregamento.
            sqlVolume =
                "SELECT " + idCarregamento + @" AS idCarregamento, v.idPedido, v.idVolume, FALSE AS Carregado, FALSE AS Entregue,
                    ?dataCad as dataCad, " + usuCad + @" as usuCad
                FROM volume v
                WHERE v.idPedido IN (" + idsPedido + @")
                    AND v.situacao=" + (int)Volume.SituacaoVolume.Fechado;

            #endregion

            #region Insere os itens de carregamento

            objPersistence.ExecuteCommand(sessao,
                @"INSERT INTO item_carregamento (idCarregamento, idPedido, idProd, idProdPedProducao, carregado, entregue, dataCad, usuCad) " + sqlVenda,
                new GDAParameter("?dataCad", dataCad));

            objPersistence.ExecuteCommand(sessao,
                @"INSERT INTO item_carregamento (idCarregamento, idPedido, idVolume, carregado, entregue, dataCad, usuCad) " + sqlVolume,
                new GDAParameter("?dataCad", dataCad));

            #endregion

            var itensRevenda = ProdutosPedidoDAO.Instance.GetByPedidosForExpCarregamento(sessao, idsPedido, true);
            foreach (var item in itensRevenda)
            {
                for (int i = 0; i < item.Qtde; i++)
                    Insert(sessao, new ItemCarregamento()
                    {
                        IdCarregamento = idCarregamento,
                        IdPedido = item.IdPedido,
                        IdProd = item.IdProd,
                        Carregado = false,
                        Entregue = false
                    });
            }
        }

        #endregion

        #region Atualiza itens de revenda

        /// <summary>
        /// Atualiza itens de revenda
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCarregamento"></param>
        /// <param name="idPedido"></param>
        /// <param name="etiqueta"></param>
        /// <param name="chapa"></param>
        public void AtualizaItemRevenda(GDASession sessao, uint idCarregamento, uint idPedido, string etiqueta, bool chapa)
        {
            uint idProd = 0;
            uint campoAtualizar = 0;
            string campo = chapa ? "IdProdImpressaoChapa" : "idProdPedProducao";

            if (chapa)
            {
                var tipoEtiqueta = ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(etiqueta);
                campoAtualizar = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressaoParaCarregamento(sessao, etiqueta).GetValueOrDefault(0);

                var idRetalho = tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.Retalho ?
                            Glass.Conversoes.StrParaUint(etiqueta.Substring(1, etiqueta.IndexOf('-') - 1)) : 0;

                idProd = tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal ?
                    ProdutosNfDAO.Instance.GetIdProdByEtiqueta(sessao, etiqueta) : RetalhoProducaoDAO.Instance.ObtemIdProd(sessao, idRetalho);
            }
            else
            {
                campoAtualizar = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(sessao, etiqueta).GetValueOrDefault(0);
                idProd = ProdutoDAO.Instance.ObtemIdProdByEtiqueta(sessao, etiqueta);
            }

            if (idProd == 0)
                throw new Exception("Falha ao atualizar item de revenda. Produto não encontrado.");


            if (campoAtualizar == 0)
                throw new Exception("Falha ao atualizar item de revenda. Peça da etiqueta não encontrada.");

            string sql = @"
                UPDATE item_carregamento ic
                SET ic." + campo + " = " + campoAtualizar + @"
                WHERE ic.idItemCarregamento =
                (SELECT id
                 FROM (SELECT idItemCarregamento AS id
                        FROM item_carregamento ic1
                        WHERE COALESCE(ic1." + campo + @", 0) = 0
                        AND ic1.idCarregamento = " + idCarregamento + " AND ic1.idPedido=" + idPedido + @"
                        AND ic1.idProd = " + idProd + @" LIMIT 0,1) AS tmp
                )";

            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Estorna a tualização de itens de revenda
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="idPedido"></param>
        /// <param name="etiqueta"></param>
        /// <param name="chapa"></param>
        public void EstornaAtualizaItemRevenda(uint idCarregamento, uint idPedido, string etiqueta, bool chapa)
        {
            uint idProd = 0;
            uint campoAtualizar = 0;
            string campo = chapa ? "IdProdImpressaoChapa" : "idProdPedProducao";

            if (chapa)
            {
                campoAtualizar = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressaoParaCarregamento(null, etiqueta).GetValueOrDefault(0);
                idProd = ProdutosNfDAO.Instance.GetIdProdByEtiqueta(etiqueta);
            }
            else
            {
                campoAtualizar = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(etiqueta).GetValueOrDefault(0);
                idProd = ProdutoDAO.Instance.ObtemIdProdByEtiqueta(etiqueta);
            }

            if (idProd == 0)
                throw new Exception("Falha ao estornar item de revenda. Produto não encontrado.");


            if (campoAtualizar == 0)
                throw new Exception("Falha ao estornar item de revenda. Peça da etiqueta não encontrada.");

            string sql = @"
                UPDATE item_carregamento ic
                SET ic." + campo + @" = null
               WHERE ic.idItemCarregamento =
                (SELECT id
                 FROM (SELECT idItemCarregamento AS id
                        FROM item_carregamento ic1
                        WHERE ic1.idCarregamento = " + idCarregamento + " AND ic1.idPedido=" + idPedido + @"
                        AND ic1.idProd = " + idProd + @" AND ic1." + campo + "=" + campoAtualizar + @") AS tmp
                )";

            objPersistence.ExecuteCommand(sql);
        }

        #endregion

        #region Efetua a leitura de uma peça / volume

        /// <summary>
        /// Efetua a leitura de uma peça / volume
        /// </summary>
        public void EfetuaLeitura(GDASession sessao, uint idFunc, uint idCarregamento, string etiqueta)
        {
            var idFuncLeitura = idFunc;
            var dataLeitura = DateTime.Now;

            var itemCarregamento = GetIdItemCarregamento(sessao, idCarregamento, etiqueta);

            if (itemCarregamento == 0)
                throw new Exception("Item do carregamento não encontrado.");

            string sql = @"
                UPDATE item_carregamento
                SET carregado = TRUE, idFuncLeitura=" + idFuncLeitura + @", dataLeitura=?dataLeitura
                WHERE idItemCarregamento=" + itemCarregamento;

            objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?dataLeitura", dataLeitura));
        }

        public void DeletaLeitura(uint idCarregamento, string etiqueta)
        {
            var itemCarregamento = GetIdItemCarregamento(null, idCarregamento, etiqueta);

            if (itemCarregamento == 0)
                throw new Exception("Item do carregamento não encontrado.");

            string sql = @"
                UPDATE item_carregamento
                SET carregado = FALSE, idFuncLeitura=null, dataLeitura=null
                WHERE idItemCarregamento=" + itemCarregamento;

            objPersistence.ExecuteCommand(sql);
        }

        #endregion

        #region Efetua o estorno de itens

        /// <summary>
        /// Realiza o estorno de itens do carregamento
        /// </summary>
        /// <param name="idsItens"></param>
        public void EstornaItens(GDASession sessao, string idsItens)
        {
            string sql = @"
                UPDATE item_carregamento ic
                    LEFT JOIN produto_pedido_producao ppp ON (ic.idProdPedProducao = ppp.idProdPedProducao)
                    LEFT JOIN produtos_pedido pp on (ppp.idProdPed=pp.idProdPedEsp)
                    LEFT JOIN pedido p ON (pp.idPedido=p.idPedido)
                SET ic.carregado=FALSE, ic.entregue=FALSE, ic.idFuncLeitura=null, ic.dataLeitura=null, ic.IdProdImpressaoChapa = null,
                    ic.idProdPedProducao = IF(p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @", null, ic.idProdPedProducao)
                WHERE ic.idItemCarregamento IN(" + idsItens + ") AND COALESCE(ic.carregado, FALSE) = TRUE";

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Remove todos os itens de um carregamento

        /// <summary>
        /// Remove todos os itens de um carregamento
        /// </summary>
        /// <param name="idCarregamento"></param>
        public void DeleteByCarregamento(GDASession sessao, uint idCarregamento)
        {
            string sql = @"
                DELETE FROM item_carregamento
                WHERE idCarregamento = " + idCarregamento;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Remove os itens do carregamento das ocs informadas
        /// </summary>
        /// <param name="idOC"></param>
        public void DeleteByOC(GDASession sessao, uint idOC, uint idCarregamento)
        {
            var idsPedidos = string.Join(",", PedidoDAO.Instance.GetIdsPedidosByOCs(sessao, idOC.ToString()).Select(f => f.ToString()).ToArray());

            string sql = @"
                DELETE FROM item_carregamento
                WHERE idPedido IN (" + idsPedidos + ") AND idCarregamento=" + idCarregamento;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Remove os itens do carregamento das ocs informadas
        /// </summary>
        /// <param name="idOC"></param>
        public void DeleteByOC(uint idOC, uint idCarregamento)
        {
            DeleteByOC(null, idOC, idCarregamento);
        }

        /// <summary>
        ///  Remove os itens do carregamento do pedido informado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idCarregamento"></param>
        public void DeleteByPedido(uint idPedido, uint idCarregamento)
        {
            DeleteByPedido(null, idPedido, idCarregamento);
        }

        /// <summary>
        ///  Remove os itens do carregamento do pedido informado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idCarregamento"></param>
        public void DeleteByPedido(GDASession sessao, uint idPedido, uint idCarregamento)
        {
            string sql = @"
                DELETE FROM item_carregamento
                WHERE idPedido =" + idPedido + " AND idCarregamento=" + idCarregamento;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Busca dados do item

        public uint ObtemIdItemCarregamento(GDASession sessao, uint idProdPedProducao)
        {
            return ObtemValorCampo<uint>("IdItemCarregamento", "IdProdPedProducao = " + idProdPedProducao);
        }

        #endregion
    }
}
