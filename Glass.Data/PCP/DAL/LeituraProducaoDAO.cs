using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class LeituraProducaoDAO : BaseDAO<LeituraProducao, LeituraProducaoDAO>
    {
        #region Listas e Relatórios

        public IList<LeituraProducao> GetByProdPedProducao(GDASession sessao, uint idProdPedProducao)
        {
            string sql = "select * from leitura_producao where idProdPedProducao=" + idProdPedProducao;
            return objPersistence.LoadData(sessao, sql).ToList();
        }

        public int GetCountByProdPedProducao(GDASession sessao, uint idProdPedProducao)
        {
            string sql = "select count(*) from leitura_producao where idProdPedProducao=" + idProdPedProducao;
            return objPersistence.ExecuteSqlQueryCount(sessao, sql);
        }

        #endregion
        
        #region Obtém dados

        /// <summary>
        /// Obtem os setores lidos da peça passada.
        /// </summary>
        public List<int> ObterSetoresLidos(GDASession sessao, int idProdPedProducao)
        {
            if (idProdPedProducao == 0)
            {
                return new List<int>();
            }

            return ExecuteMultipleScalar<int>(sessao, $"SELECT IdSetor FROM leitura_producao WHERE IdProdPedProducao={ idProdPedProducao }");
        }

        public string ObterSqlUltimasLeiturasSetor(List<int> idsProdPedProducao, int idSetor)
        {
            if (!(idsProdPedProducao?.Any(f => f > 0) ?? false) || idSetor == 0)
            {
                return string.Empty;
            }

            return $@"SELECT lp.IdProdPedProducao,
                    NULL AS IdsSetores,
                    lp.DataLeitura AS Setor0,
                    f.Nome AS Func0
                FROM leitura_producao lp
                    LEFT JOIN funcionario f ON (lp.IdFuncLeitura = f.IdFunc)
                WHERE lp.IdProdPedProducao IN ({ string.Join(",", idsProdPedProducao) }) AND lp.IdSetor = { idSetor }";
        }

        public string ObterSqlParaGraficoProdPerdaDiaria(GDASession session, List<int> idsSetor, DateTime dataLeituraInicial, DateTime dataLeituraFinal)
        {
            if (!(idsSetor?.Any(f => f > 0) ?? false) || dataLeituraInicial == DateTime.MinValue || dataLeituraFinal == DateTime.MinValue)
            {
                return string.Empty;
            }

            var sqlLeituraProducao = $@"SELECT IdLeituraProd FROM leitura_producao
                WHERE IdSetor IN ({ string.Join(",", idsSetor) }) AND DataLeitura BETWEEN ?dataIni AND ?dataFim";

            var idsLeituraProd = ExecuteMultipleScalar<int>(session, sqlLeituraProducao,
                new GDAParameter("?dataIni", dataLeituraInicial.ToString("yyy-MM-dd hh:mm:ss")),
                new GDAParameter("?dataFim", dataLeituraFinal.ToString("yyy-MM-dd hh:mm:ss")));

            return $@"SELECT IdProdPedProducao, IdSetor, DataLeitura FROM leitura_producao
                WHERE IdLeituraProd IN ({ (idsLeituraProd.Any(f => f > 0) ? string.Join(",", idsLeituraProd) : "0") })";
        }

        public string ObterSqlParaPerdaPorProduto(GDASession session, int idSetor, DateTime dataLeituraInicial, DateTime dataLeituraFinal)
        {
            if (idSetor == 0 || dataLeituraInicial == DateTime.MinValue || dataLeituraFinal == DateTime.MinValue)
            {
                return string.Empty;
            }

            var sqlLeituraProducao = $@"SELECT IdLeituraProd FROM leitura_producao
                WHERE DataLeitura >= ?dataIni AND DataLeitura <= ?dataFim { (idSetor > 0 ? $" AND IdSetor = { idSetor.ToString() }" : string.Empty ).ToString() }";

            var idsLeituraProd = ExecuteMultipleScalar<int>(session, sqlLeituraProducao,
                new GDAParameter("?dataIni", dataLeituraInicial.ToString("yyy-MM-dd hh:mm:ss")),
                new GDAParameter("?dataFim", dataLeituraFinal.ToString("yyy-MM-dd hh:mm:ss")));

            return $@"SELECT IdProdPedProducao, IdSetor FROM leitura_producao
                WHERE IdLeituraProd IN ({ (idsLeituraProd.Any(f => f > 0) ? string.Join(",", idsLeituraProd) : "0") })";
        }

        public uint ObtemIdLeituraProd(GDASession sessao, uint idProdPedProducao, uint idSetor)
        {
            return ObtemValorCampo<uint>(sessao, "idLeituraProd", string.Format("idSetor = {0} AND idprodpedproducao = {1}", idSetor, idProdPedProducao));
        }

        /// <summary>
        /// Obtém os IDs dos funcionários que fizeram leitura no setor informado.
        /// </summary>
        public List<int> ObterIdsFuncLeituraSetor(GDASession session, int idSetor)
        {
            if (!SetorDAO.Instance.Exists(session, idSetor))
            {
                return new List<int>();
            }

            var idsFuncLeituraSetor = ExecuteMultipleScalar<int>(session, $"SELECT DISTINCT IdFuncLeitura FROM leitura_producao WHERE IdSetor={ idSetor }");

            return idsFuncLeituraSetor?.ToList() ?? new List<int>();
        }

        /// <summary>
        /// Obtém os IDs dos funcionários que fizeram leitura no setor informado.
        /// </summary>
        public int ObterIdLeituraPeloIdProdPedProducaoIdSetor(GDASession session, int idProdPedProducao, int idSetor)
        {
            if (idProdPedProducao == 0 || idSetor == 0)
            {
                return 0;
            }

            return ExecuteScalar<int>(session, $"SELECT IdLeituraProd FROM leitura_producao WHERE IdSetor={ idSetor } AND IdProdPedProducao={ idProdPedProducao }");
        }

        /// <summary>
        /// Obtém os IDs dos funcionários que fizeram leitura no setor informado.
        /// </summary>
        public List<int> ObterIdsProdPedProducaoPeloIdSetorDataLeitura(GDASession session, int idSetor, DateTime dataLeituraInicial, DateTime dataLeituraFinal)
        {
            if (idSetor == 0 || dataLeituraInicial == DateTime.MinValue || dataLeituraFinal == DateTime.MinValue)
            {
                return new List<int>();
            }

            var idsProdPedProducao = ExecuteMultipleScalar<int>(session, $@"
                SELECT lp.IdProdPedProducao FROM leitura_producao lp
		        WHERE lp.DataLeitura >= ?dataLeituraInicial
        	        AND lp.DataLeitura <= ?dataLeituraFinal
        	        AND lp.IdSetor = { idSetor }
                GROUP BY lp.IdProdPedProducao",
                new GDAParameter("?dataLeituraInicial", dataLeituraInicial),
                new GDAParameter("?dataLeituraFinal", dataLeituraFinal));

            return idsProdPedProducao ?? new List<int>();
        }

        public List<int> ObterIdsProdPedProducaoPeloIdFunc(GDASession sessao, int idFunc)
        {
            if (idFunc == 0)
            {
                return new List<int>();
            }

            var idsProdPedProducao = ExecuteMultipleScalar<int>(sessao, $"SELECT IdProdPedProducao FROM leitura_producao WHERE IdFuncLeitura={ idFunc }");

            return idsProdPedProducao ?? new List<int>();
        }

        /// <summary>
        /// Obtem o última leitura da peça passada
        /// </summary>
        public LeituraProducao ObtemUltimaLeitura(GDASession sessao, uint idProdPedProducao)
        {
            string sql = @"
                Select * 
                From leitura_producao
                Where idProdPedProducao=" + idProdPedProducao + @"
                Order By DataLeitura desc Limit 1";

            return objPersistence.LoadOneData(sessao, sql);
        }

        /// <summary>
        /// Obtem o último setor lido da peça passada
        /// </summary>
        public uint ObtemUltimoSetorLido(GDASession sessao, uint idProdPedProducao)
        {
            string sql = @"
                Select lprod.idSetor 
                From leitura_producao lprod  
                    Inner Join setor s On (lprod.idSetor=s.idSetor)
                Where idProdPedProducao=" + idProdPedProducao + @"
                Order By s.numSeq desc Limit 1";

            return ExecuteScalar<uint>(sessao, sql);
        }

        /// <summary>
        /// Obtem o último cavalete lido da peça passada
        /// </summary>
        public int? ObtemUltimoCavaleteLido(GDASession sessao, uint idProdPedProducao)
        {
            string sql = @"
                Select lprod.IdCavalete 
                From leitura_producao lprod  
                    Inner Join setor s On (lprod.idSetor=s.idSetor)
                Where IdCavalete IS NOT NULL AND idProdPedProducao=" + idProdPedProducao + @"
                Order By s.numSeq desc Limit 1";

            return ExecuteScalar<int?>(sessao, sql);
        }

        #endregion

        #region Verifica dados

        /// <summary>
        /// Verifica se a peça já foi lida no setor informado.
        /// </summary>
        public bool VerificaLeituraPeca(GDASession sessao, uint idProdPedProducao, uint idSetor)
        {
            string sql = "Select Count(*) From leitura_producao Where IdProdPedProducao=" +
                idProdPedProducao + " And idSetor=" + idSetor;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se a peça já foi lida no setor informado
        /// </summary>
        public bool VerificaLeituraPeca(string codEtiqueta, uint idSetor)
        {
            uint? id = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(codEtiqueta);
            return id > 0 ? VerificaLeituraPeca(null, id.Value, idSetor) : false;
        }

        /// <summary>
        /// Verifica se a peça já está impressa.
        /// </summary>
        public bool VerificarPecaImpressa(GDASession sessao, int idProdPedProducao)
        {
            if (idProdPedProducao == 0)
            {
                return false;
            }

            return ExecuteScalar<bool>(sessao, $"SELECT COUNT(*) > 0 FROM leitura_producao WHERE IdProdPedProducao={ idProdPedProducao } AND IdSetor=1");
        }

        /// <summary>
        /// Verifica se o setor passado vem depois de todos os já lidos na peça passada
        /// </summary>
        public bool VerificarIdSetorUltimoSetor(GDASession sessao, int idProdPedProducao, int idSetor)
        {
            if (idProdPedProducao == 0 || idSetor == 0)
            {
                return false;
            }

            var numSeqSetor = SetorDAO.Instance.ObtemNumSeq(sessao, idSetor);

            var sql = $@"SELECT COUNT(*)=0 FROM leitura_producao lp
                    INNER JOIN setor s ON (lp.IdSetor=s.IdSetor)
                WHERE lp.IdProdPedProducao={ idProdPedProducao }
                    AND s.NumSeq > { numSeqSetor } AND (s.PermitirLeituraForaRoteiro IS NULL OR s.PermitirLeituraForaRoteiro=0)";

            return ExecuteScalar<bool>(sessao, sql);
        }

        public bool VerificarFuncionarioPossuiLeitura(GDASession session, int idFuncLeitura)
        {
            if (idFuncLeitura == 0)
            {
                return false;
            }

            return ExecuteScalar<bool>(session, $"SELECT COUNT(*)>0 FROM leitura_producao WHERE IdFuncLeitura={ idFuncLeitura }");
        }

        public bool VerificarSetorTeveLeituraPosterior(GDASession sessao, int idSetor, DateTime dataLeitura)
        {
            if (idSetor == 0 || dataLeitura == DateTime.MinValue)
            {
                return false;
            }

            var sql = $@"SELECT COUNT(*)
                FROM leitura_producao
                WHERE IdSetor = { idSetor } AND DataLeitura > ?dtIni";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, new GDAParameter("?dtIni", dataLeitura)) > 0;
        }

        public bool VerificarEtiquetaLida(GDASession session, string numEtiqueta)
        {
            // Valida a etiqueta
            ProdutoPedidoProducaoDAO.Instance.ValidaEtiquetaProducao(session, ref numEtiqueta);
            var idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducaoPorSituacao(session, numEtiqueta, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);

            if (idProdPedProducao.GetValueOrDefault() == 0)
            {
                return false;
            }

            return ExecuteScalar<bool>(session, $@"SELECT COUNT(*) > 0
                FROM leitura_producao 
                WHERE DataLeitura IS NOT NULL AND IdProdPedProducao={ idProdPedProducao }");
        }

        /// <summary>
        /// Indica se a peça já passou por um setor específico.
        /// </summary>
        public bool PassouSetor(GDASession sessao, uint idProdPedProducao, uint idSetor)
        {
            uint idUltimoSetor = ObtemUltimoSetorLido(sessao, idProdPedProducao);

            if (idUltimoSetor == 0 || idSetor == 0)
                return false;

            return SetorDAO.Instance.ObtemNumSeq(idSetor) <= SetorDAO.Instance.ObtemNumSeq(idUltimoSetor);
        }

        /// <summary>
        /// Indica se a peça já passou por um setor específico.
        /// </summary>
        public bool PassouSetor(GDASession sessao, string numEtiqueta, uint idSetor)
        {
            uint? id = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(sessao, numEtiqueta);
            return id > 0 ? PassouSetor(sessao, id.Value, idSetor) : false;
        }

        /// <summary>
        /// Indica se a peça já passou por um setor laminado.
        /// </summary>
        public bool PassouSetorLaminado(GDASession sessao, string numEtiqueta)
        {
            var setores = ObterSetoresLidos(sessao, (int)ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(sessao, numEtiqueta).GetValueOrDefault());

            return setores?.Any(f => SetorDAO.Instance.ObtemIdsSetorLaminados().Contains(f.ToString())) ?? false;
        }
        
        /// <summary>
        /// Verifica se a peça passou por algum setor de corte.
        /// </summary>
        public bool PecaFoiCortada(GDASession session, uint idProdPedProducao)
        {
            string setoresCorte = "";
            Array.ForEach(Utils.GetSetores, s => setoresCorte += s.Corte ? s.IdSetor + "," : "");

            if (setoresCorte == "")
                return false;

            string sql = "select count(*) from leitura_producao where idProdPedProducao=" + idProdPedProducao +
                " and idSetor in (" + setoresCorte.TrimEnd(',') + ")";

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region Atualiza dados

        /// <summary>
        /// Insere a leitura da peça passada no setor informado
        /// </summary>
        public LeituraProducao LeituraPeca(GDASession sessao, uint idProdPedProducao, uint idSetor, uint idFuncLeitura, DateTime? dataLeitura, bool releitura, int? idCavalete)
        {
            // Garante que a peça só seja lida se houver data de impressão
            if (!VerificarPecaImpressa(sessao, (int)idProdPedProducao) && idSetor > 1)
            {
                // Marca a peça no setor "Impr. Etiqueta" se for peça reposta e cria o registro em produto_impressao
                if (ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(sessao, idProdPedProducao, false))
                {
                    ProdutoImpressaoDAO.Instance.MarcaImpressao(sessao, ProdutoPedidoProducaoDAO.Instance.ObtemEtiqueta(sessao, idProdPedProducao), 0,
                        ProdutoImpressaoDAO.TipoEtiqueta.Pedido, true);

                    LeituraProducao leituraProd = LeituraPeca(sessao, idProdPedProducao, 1, idFuncLeitura, dataLeitura, false, null);
                    Insert(sessao, leituraProd);
                }
                else
                    throw new Exception("Esta peça não pode ser lida neste setor pois não foi impressa.");
            }

            if (!VerificarIdSetorUltimoSetor(sessao, (int)idProdPedProducao, (int)idSetor) && !releitura)
                throw new Exception("Esta peça não pode ser lida neste setor, pois já foi lida em um setor posterior.");

            LeituraProducao leitura = new LeituraProducao();
            leitura.IdProdPedProducao = idProdPedProducao;
            leitura.IdSetor = idSetor;
            leitura.IdFuncLeitura = idFuncLeitura;
            leitura.DataLeitura = dataLeitura;
            leitura.IdCavalete = idCavalete;
            leitura.ProntoRoteiro = RoteiroProducaoEtiquetaDAO.Instance.ObterUltimoSetor(idProdPedProducao) == idSetor;

            return leitura;
        }

        public void AtualizarDataLeituraCavalete(GDASession session, int idLeituraProducao, int idCavalete, DateTime dataLeitura)
        {
            if (idLeituraProducao == 0)
            {
                return;
            }

            objPersistence.ExecuteCommand(session, $"UPDATE leitura_producao SET DataLeitura=?dt, IdCavalete={ idCavalete } WHERE IdLeituraProd={ idLeituraProducao }",
                new GDAParameter("?dt", dataLeitura));
        }

        public void AtualizarDataLeituraIdsProdPedProducao(GDASession session, List<int> idsProdPedProducao, DateTime? dataLeitura)
        {
            if (!idsProdPedProducao?.Any(f => f > 0) ?? false)
            {
                return;
            }

            objPersistence.ExecuteCommand(session, $@"UPDATE leitura_producao SET DataLeitura=?dataLeitura WHERE IdProdPedProducao IN ({ string.Join(",", idsProdPedProducao) })",
                new GDAParameter("?dataLeitura", dataLeitura == null || dataLeitura == DateTime.MinValue ? "NULL" : dataLeitura.Value.ToString("yyyy-MM-dd HH:mm:ss")));
        }

        public void AtualizarDataLeituraIdFuncPeloIdProdPedProducao(GDASession session, int idProdPedProducao, DateTime dataLeitura, int idFunc)
        {
            if (idProdPedProducao == 0 || dataLeitura == DateTime.MinValue || idFunc == 0)
            {
                return;
            }

            objPersistence.ExecuteCommand(session, $@"UPDATE leitura_producao SET DataLeitura=?dataLeitura, IdFuncLeitura=?idFunc 
                WHERE DataLeitura IS NULL AND IdProdPedProducao={ idProdPedProducao }",
                new GDAParameter("?dataLeitura", dataLeitura), new GDAParameter("?idFunc", idFunc));
        }

        public void ApagarPelosIdsProdPedProducao(GDASession session, List<int> idsProdPedProducao)
        {
            if (!idsProdPedProducao?.Any(f => f > 0) ?? false)
            {
                return;
            }

            objPersistence.ExecuteCommand(session, $"DELETE FROM leitura_producao WHERE IdProdPedProducao IN ({ string.Join(",", idsProdPedProducao) })");
        }

        public void ApagarPeloIdLiberarPedido(GDASession session, int idLiberarPedido)
        {
            if (idLiberarPedido == 0)
            {
                return;
            }

            var idsSetorEntregue = SetorDAO.Instance.ObterIdsSetorTipoEntregue(session);

            if (!idsSetorEntregue?.Any(f => f > 0) ?? false)
            {
                return;
            }

            var idsPedido = ProdutosLiberarPedidoDAO.Instance.GetIdsPedidoByLiberacao(session, (uint)idLiberarPedido);

            if (!idsPedido?.Any(f => f > 0) ?? false)
            {
                return;
            }

            var idsProdPed = ProdutosPedidoEspelhoDAO.Instance.ObterIdsProdPedPelosIdsPedido(session, idsPedido.Select(f => (int)f).ToList());

            if (!idsProdPed?.Any(f => f > 0) ?? false)
            {
                return;
            }

            var idsProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObterIdsProdPedProducaoPelosIdProdPed(session, idsProdPed);

            if (!idsProdPedProducao?.Any(f => f > 0) ?? false)
            {
                return;
            }

            objPersistence.ExecuteCommand(session, $"DELETE FROM leitura_producao WHERE IdSetor IN ({ string.Join(",", idsSetorEntregue) }) AND IdProdPedProducao IN ({ string.Join(",", idsProdPedProducao) })");
        }

        #endregion
    }
}
