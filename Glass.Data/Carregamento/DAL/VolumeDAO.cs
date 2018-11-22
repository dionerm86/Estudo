using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class VolumeDAO : BaseDAO<Volume, VolumeDAO>
    {
        //private VolumeDAO() { }

        #region Métodos de retorno de itens

        string Sql(uint idVolume, string etiquetaVolume, uint idPedido, string idsPedidos, uint idCliente, string nomeCliente, string situacao,
            string dataEntregaIni, string dataEntregaFim, uint idLoja, string codRota, bool selecionar)
        {
            string campos = selecionar ? @"v.*, p.dataEntrega as DataEntregaPedido, p.CodCliente, c.id_cli as IdCliente, c.nomeFantasia as NomeFantasia, c.Nome as NomeCliente,
                sum(vpp.qtde) as QtdeItens, SUM(pp.peso / if(pp.qtde <> 0, pp.qtde, 1) * vpp.qtde) as PesoTotal, SUM(pp.TotM / if(pp.qtde <> 0, pp.qtde, 1) * vpp.qtde) as TotM,
                (Select CONCAT(r.codInterno,' - ',r.descricao) From rota r Where r.idRota In (Select rc.idRota From rota_cliente rc Where rc.idCliente=c.id_Cli)) As codRota,
                l.NomeFantasia as Loja, l.site as SiteLoja, l.telefone as TelLoja, f.nome as NomeFuncFinalizacao, '$$$' as Criterio,
                p.RotaExterna, p.IdClienteExterno, p.ClienteExterno, p.IdPedidoExterno, p.Importado as PedidoImportado, sgp.descricao AS NomeSubGrupoProd" : "COUNT(DISTINCT v.idVolume)";

            string sql = @"
                SELECT " + campos + @"
                FROM volume v
                    LEFT JOIN pedido p ON (v.idPedido = p.idPedido)
                    LEFT JOIN loja l ON (p.idLoja = l.idLoja)
                    LEFT JOIN cliente c ON (p.idCli = c.id_cli)
                    LEFT JOIN volume_produtos_pedido vpp ON (v.idVolume = vpp.idVolume)
                    LEFT JOIN produtos_pedido pp ON (vpp.idProdPed = pp.idProdPed)
                    LEFT JOIN funcionario f ON (v.IdFuncFechamento = f.IdFunc)
                    LEFT JOIN produto prod ON (pp.IdProd = prod.IdProd)
                    LEFT JOIN subgrupo_prod sgp ON (prod.IdSubGrupoProd = sgp.IdSubGrupoProd)
                WHERE 1";

            string criterio = "";

            sql += " AND p.tipoPedido<>" + (int)Pedido.TipoPedidoEnum.Producao;

            if (idVolume > 0)
            {
                sql += " AND v.idVolume=" + idVolume;
                criterio += "Num. Volume: " + idVolume + "    ";
            }

            if (!string.IsNullOrEmpty(etiquetaVolume))
            {
                uint id = Glass.Conversoes.StrParaUint(etiquetaVolume.Substring(1));
                if (id > 0)
                {
                    sql += " AND v.idVolume=" + id;
                    criterio += "Num. Etiqueta: " + etiquetaVolume + "    ";
                }
            }

            if (idPedido > 0)
            {
                sql += " AND v.IdPedido=" + idPedido;
                criterio += "Num. Pedido: " + idPedido + "    ";
            }

            if (!string.IsNullOrEmpty(idsPedidos))
            {
                sql += " AND v.IdPedido IN(" + idsPedidos + ")";
                criterio += "Num. Pedido: " + idsPedidos + "    ";
            }

            if (idCliente > 0)
            {
                sql += " AND p.idCli=" + idCliente;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCliente) + "    ";
            }
            else if (!string.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);

                sql += " AND p.idCli IN(" + ids + ")";
                criterio += "Cliente: " + nomeCliente + "    ";
            }

            if (!string.IsNullOrEmpty(situacao))
            {
                sql += " AND v.Situacao IN(" + situacao + ")";
            }

            if (!string.IsNullOrEmpty(dataEntregaIni))
            {
                sql += " AND p.DataEntrega>=?dtEntIni";
                criterio += "Data Entrega Inicial: " + dataEntregaIni + "    ";
            }

            if (!string.IsNullOrEmpty(dataEntregaFim))
            {
                sql += " AND p.DataEntrega<=?dtEntFim";
                criterio += "Data Entrega Final: " + dataEntregaFim + "    ";
            }

            if (idLoja > 0)
            {
                sql += " AND p.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (!String.IsNullOrEmpty(codRota))
            {
                sql += " And c.id_Cli In (Select idCliente From rota_cliente Where idRota In " +
                    "(Select idRota From rota where codInterno like ?codRota))";
                criterio += "Rota: " + codRota + "    ";
            }

            if (selecionar)
                sql += " GROUP BY v.idVolume";

            return sql.Replace("$$$", criterio);
        }

        /// <summary>
        /// Recupera uma lista de volume para a grid
        /// </summary>
        /// <param name="idVolume"></param>
        /// <param name="idPedido"></param>
        /// <param name="situacao"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<Volume> GetList(uint idVolume, uint idPedido, string situacao, string sortExpression, int startRow, int pageSize)
        {
            string sql = Sql(idVolume, null, idPedido, null, 0, null, situacao, null, null, 0, null, true);

            if (string.IsNullOrEmpty(sortExpression))
                sql += " ORDER BY v.idVolume DESC";

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize);
        }

        public int GetListCount(uint idVolume, uint idPedido, string situacao)
        {
            string sql = Sql(idVolume, null, idPedido, null, 0, null, situacao, null, null, 0, null, false);
            return objPersistence.ExecuteSqlQueryCount(sql);
        }

        /// <summary>
        /// Recupera uma lista de volumes para o relatorio
        /// </summary>
        /// <param name="idVolume"></param>
        /// <param name="etiquetaVolume"></param>
        /// <param name="idPedido"></param>
        /// <param name="idCliente"></param>
        /// <param name="nomeCliente"></param>
        /// <param name="situacao"></param>
        /// <param name="dataEntregaIni"></param>
        /// <param name="dataEntregaFim"></param>
        /// <param name="idLoja"></param>
        /// <param name="codRota"></param>
        /// <returns></returns>
        public IList<Volume> GetListForRpt(uint idVolume, string etiquetaVolume, uint idPedido, uint idCliente, string nomeCliente, string situacao,
            string dataEntregaIni, string dataEntregaFim, uint idLoja, string codRota)
        {
            string sql = Sql(idVolume, etiquetaVolume, idPedido, null, idCliente, nomeCliente, situacao,
                dataEntregaIni, dataEntregaFim, idLoja, codRota, true);

            sql += " ORDER BY v.idVolume DESC";

            return objPersistence.LoadData(sql, GetParameters(dataEntregaIni, dataEntregaFim, codRota)).ToList();
        }

        /// <summary>
        /// Busca um volume
        /// </summary>
        /// <param name="idVolume"></param>
        /// <returns></returns>
        public Volume GetElement(uint idVolume)
        {
            return objPersistence.LoadOneData(Sql(idVolume, null, 0, null, 0, null, null, null, null, 0, null, true));
        }

        public GDAParameter[] GetParameters(string dtEntIni, string dtEntFim, string codRota)
        {
            List<GDAParameter> parameters = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(dtEntIni))
                parameters.Add(new GDAParameter("?dtEntIni", DateTime.Parse(dtEntIni + " 00:00:00")));

            if (!string.IsNullOrEmpty(dtEntFim))
                parameters.Add(new GDAParameter("?dtEntFim", DateTime.Parse(dtEntFim + " 23:59:59")));

            if (!string.IsNullOrEmpty(codRota))
                parameters.Add(new GDAParameter("?codRota", codRota));

            return parameters.ToArray();
        }

        /// <summary>
        /// Recupera os volumes de uma lista de pedidos para a exp de carregamento
        /// </summary>
        /// <param name="idsPedidos"></param>
        /// <returns></returns>
        public Volume[] GetForExpCarregamento(string idsPedidos)
        {
            return objPersistence.LoadData(Sql(0, null, 0, idsPedidos, 0, null, null, null, null, 0, null, true)).ToArray();
        }
 
        /// <summary>
        /// Recupera os volumes de um pedido.
        /// </summary>
        public Volume[] ObterPeloPedido(GDASession session, int idPedido)
        {
            return objPersistence.LoadData(session, string.Format("SELECT * FROM volume WHERE IdPedido={0}", idPedido)).ToArray();
        }

        /// <summary>
        /// Busca volumes para gerar ordem de carga
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public Volume[] ObterVolumesParaGerarOrdemCarga(uint idPedido)
        {
            var campos = @"v.IdVolume, SUM(pp.peso / if(pp.qtde <> 0, pp.qtde, 1) * vpp.qtde) as PesoTotal";

            string sql = @"
                SELECT " + campos + @"
                FROM volume v
                    LEFT JOIN volume_produtos_pedido vpp ON (v.idVolume = vpp.idVolume)
                    LEFT JOIN produtos_pedido pp ON (vpp.idProdPed = pp.idProdPed)
                    LEFT JOIN item_carregamento ic ON (v.idVolume = ic.IdVolume)
                WHERE ic.IdVolume is null AND v.IdPedido = " + idPedido + @"
                GROUP BY v.IdVolume";

            return objPersistence.LoadData(sql).ToArray();
        }

        /// <summary>
        /// Obtém a quantidade de volumes que a ordem de carga informada possui.
        /// </summary>
        public int ObterQuantidadeVolumesPeloIdOrdemCarga(GDASession session, int idOrdemCarga)
        {
            return objPersistence.ExecuteSqlQueryCount(session, string.Format("SELECT COUNT(*) FROM volume WHERE IdOrdemCarga={0}", idOrdemCarga));
        }

        #endregion

        #region Obtem dados do volume

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Busca a situação de um volume
        /// </summary>
        /// <param name="idVolume"></param>
        /// <returns></returns>
        public Volume.SituacaoVolume GetSituacao(uint idVolume)
        {
            return GetSituacao(null, idVolume);
        }

        /// <summary>
        /// Busca a situação de um volume
        /// </summary>
        /// <param name="idVolume"></param>
        /// <returns></returns>
        public Volume.SituacaoVolume GetSituacao(GDASession sessao, uint idVolume)
        {
            return (Volume.SituacaoVolume)ObtemValorCampo<int>(sessao, "situacao", "idVolume=" + idVolume);
        }
        
        /// <summary>
        /// Busca o pedido do volume
        /// </summary>
        /// <param name="idVolume"></param>
        /// <returns></returns>
        public uint GetIdPedido(GDASession sessao, uint idVolume)
        {
            return ObtemValorCampo<uint>(sessao, "idPedido", "idVolume=" + idVolume);
        }

        /// <summary>
        /// Busca a loja do volume
        /// </summary>
        /// <param name="idVolume"></param>
        /// <returns></returns>
        public uint GetIdLoja(uint idVolume)
        {
            var idPedido = GetIdPedido(null, idVolume);

            return PedidoDAO.Instance.ObtemIdLoja(null, idPedido);
        }

        #endregion

        #region Fechar volume

        /// <summary>
        /// Fecha um volume
        /// </summary>
        /// <param name="idVolume"></param>
        public void FecharVolume(uint idVolume)
        {
            string sql = "UPDATE volume SET situacao=" + (int)Volume.SituacaoVolume.Fechado
                + ", dataFechamento=?dtFechamento, IdFuncFechamento = " + UserInfo.GetUserInfo.CodUser + " WHERE idVolume=" + idVolume;
            objPersistence.ExecuteCommand(sql, new GDAParameter("?dtFechamento", DateTime.Now));
        }

        #endregion

        #region Expedição

        /// <summary>
        /// Verifica se o volume informado pode ser expedido.
        /// </summary>
        /// <param name="idVolume"></param>
        /// <returns></returns>
        public bool VerificaSePodeExpedir(GDASession sessao, uint idVolume)
        {
            uint idPedido = GetIdPedido(sessao, idVolume);

            //So pode expedir se o pedido do volume for entrega balcão,
            //caso contrario deve ser expedido no carregamento.
            if (PedidoDAO.Instance.ObtemTipoEntrega(idPedido) != (int)Pedido.TipoEntregaPedido.Balcao)
                return false;

            //Verifica se o volume já teve a saída efetuada
            if (ObtemValorCampo<bool>(sessao, "SaidaExpedicao", "idVolume=" + idVolume))
                return false;

            return true;
        }

        /// <summary>
        /// Marca que o volume foi expedido
        /// </summary>
        /// <param name="idVolume"></param>
        public void MarcaExpedicaoVolume(GDASession sessao, uint idVolume)
        {
            objPersistence.ExecuteCommand(sessao, @"
                UPDATE volume
                SET saidaExpedicao=true,
                    UsuSaidaExpedicao=?idFunc,
                    DataSaidaExpedicao=?data
                WHERE idVolume=" + idVolume, new GDAParameter("?idFunc", UserInfo.GetUserInfo.CodUser),
                                 new GDAParameter("?data", DateTime.Now));
        }

        /// <summary>
        /// Estorna a expedição de um volume
        /// </summary>
        /// <param name="idVolume"></param>
        public void EstornaExpedicaoVolume(uint idVolume)
        {
            EstornaExpedicaoVolume(null, idVolume);
        }

        /// <summary>
        /// Estorna a expedição de um volume
        /// </summary>
        /// <param name="idVolume"></param>
        public void EstornaExpedicaoVolume(GDASession sessao, uint idVolume)
        {
            objPersistence.ExecuteCommand(@"
                UPDATE volume
                SET saidaExpedicao=false,
                    UsuSaidaExpedicao=null,
                    DataSaidaExpedicao=null
                WHERE idVolume=" + idVolume);
        }

        /// <summary>
        /// Verifica se um volume foi expedido
        /// </summary>
        public bool TemExpedicao(GDASession session, uint idVolume)
        {
            if (idVolume == 0)
                return false;

            return objPersistence.ExecuteSqlQueryCount(session, string.Format(@"SELECT COUNT(*) FROM volume v
                    LEFT JOIN saida_estoque se ON (v.IdVolume=se.IdVolume)
                WHERE v.IdVolume={0} AND ((se.IdSaidaEstoque>0 AND (se.Estornado IS NULL OR se.Estornado=0)) OR (v.SaidaExpedicao=1 AND v.DataSaidaExpedicao IS NOT NULL));", idVolume)) > 0;
        }

        /// <summary>
        /// Recupera a data da primeira expedição de volume do pedido.
        /// </summary>
        public DateTime? ObterDataPrimeiraExpedicaoVolumePedido(GDASession session, uint idPedido)
        {
            if (idPedido == 0)
                return null;

            return ExecuteScalar<DateTime?>(session, string.Format(@"SELECT COALESCE(v.DataSaidaExpedicao, se.DataCad) AS DataExpedicao FROM volume v
	                LEFT JOIN saida_estoque se ON (v.IdVolume=se.IdVolume)
                WHERE v.IdPedido={0} AND ((se.IdSaidaEstoque>0 AND (se.Estornado IS NULL OR se.Estornado=0)) OR (v.SaidaExpedicao=1 AND v.DataSaidaExpedicao IS NOT NULL))
                ORDER BY DataExpedicao LIMIT 1", idPedido));
        }

        /// <summary>
        /// Verifica se o volume informado faz parte da liberação informada
        /// </summary>
        /// <param name="idVolume"></param>
        /// <returns></returns>
        public bool FazParteLiberacao(GDASession sessao, uint idVolume, int idLiberacao)
        {
            var sql = @"
                        SELECT COUNT(*) 
                        FROM volume v
                        	INNER JOIN produtos_liberar_pedido plp ON (v.IdPedido = plp.IdPedido)
                        WHERE v.IdVolume = " + idVolume + " AND plp.IdLiberarPedido = " + idLiberacao;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Métodos sobrescritos

        /// <summary>
        /// Insere um volume
        /// </summary>
        public override uint Insert(Volume objInsert)
        {
            return Insert(null, objInsert);
        }

        /// <summary>
        /// Insere um volume
        /// </summary>
        public override uint Insert(GDASession session, Volume objInsert)
        {
            objInsert.DataCad = DateTime.Now;
            objInsert.UsuCad = UserInfo.GetUserInfo.CodUser;
            objInsert.Situacao = Volume.SituacaoVolume.Aberto;

            return base.Insert(session, objInsert);
        }

        #endregion

        #region Atualizações

        public void AtualizaIdOrdemCarga (GDASession sessao, uint idOrdemCarga, uint idPedido)
        {
            objPersistence.ExecuteCommand(sessao, "UPDATE volume SET IdOrdemCarga = ?idOrdemCarga WHERE COALESCE(IdOrdemCarga, 0) = 0 AND IdPedido = ?idPedido",
                new GDAParameter("?idOrdemCarga", idOrdemCarga), new GDAParameter("?idPedido", idPedido));
        }

        public void DesvincularOrdemCarga(GDASession sessao, uint[] idsItemCarregamento)
        {
            var sql = @"
                UPDATE volume v
                    INNER JOIN item_carregamento ic ON (ic.IdVolume = v.IdVolume)
                SET v.IdOrdemCarga = null
                WHERE ic.IdItemCarregamento IN ({0})";

            objPersistence.ExecuteCommand(sessao, string.Format(sql, string.Join(",", idsItemCarregamento)));
        }

        public void DesvincularOrdemCarga(GDASession sessao, uint idOrdemCarga)
        {
            var sql = @"
                UPDATE volume v
                SET v.IdOrdemCarga = null
                WHERE v.IdOrdemCarga = " + idOrdemCarga;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion
    }
}
