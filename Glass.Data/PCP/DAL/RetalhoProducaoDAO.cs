using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using Glass.Configuracoes;
using Colosoft;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class RetalhoProducaoDAO : BaseDAO<RetalhoProducao, RetalhoProducaoDAO>
    {
        //private RetalhoProducaoDAO() { }

        private string Sql(uint idRetalhoProducao, string idsRetalhosProducao, uint idProd, uint idProdPedProducao, string codInterno,
            string descrProduto, string dataIni, string dataFim, string dataUsoIni, string dataUsoFim, Data.Model.SituacaoRetalhoProducao? situacao, string idsCores,
            double espessura, double alturaInicio, double alturaFim, double larguraInicio, double larguraFim, string numEtiqueta,
            string observacao, bool selecionar, string filtroAdicional)
        {
            var campos = selecionar ? @"
                r.*, p.Descricao, p.CodInterno, p.Altura, p.Largura, ur.TotM AS TotMUsando, ur.Etiquetas As EtiquetaUsando,
                uso.dataLeitura as DataUso, pnf.Lote, nf.NumeroNFe, f.Nome as NomeFunc, p.Obs," +
                (PCPConfig.ExibirTotalM2RetalhoCorEspessura ? " p.Espessura, cv.Descricao as CorVidro, " : "") + "'$$$' AS Criterio" : "r.IdRetalhoProducao";
            
            string calculoTotM = 
                string.Format(@"SUM(IF(ped.TipoPedido<>{0}, (ppe.Altura * ppe.Largura) / 1000000,
                    (ape.Altura * ape.Largura) / 1000000))", (int)Pedido.TipoPedidoEnum.MaoDeObra);

            var sql =
                string.Format(@"
                    SELECT {0}
                    FROM retalho_producao r 
                        INNER JOIN produto p ON (r.IdProd=p.IdProd)
                        LEFT JOIN produto_pedido_producao ppp on(r.IdProdPedProducaoOrig = ppp.IdProdPedProducao)
	                    LEFT JOIN produto_baixa_estoque pbe ON (p.IdProdOrig = pbe.IdProd)	                
                        LEFT JOIN produtos_nf pnf ON (pnf.IdProdNf=r.IdProdNf)
                        LEFT JOIN nota_fiscal nf ON (nf.IdNf=pnf.IdNf)
                        LEFT JOIN funcionario f ON (r.UsuCad = f.IdFunc)
                        {1}
                        {3}
                        LEFT JOIN
                            (SELECT
                                ur1.IdUsoRetalhoProducao, ur1.IdRetalhoProducao, {2} AS TotM,
						        GROUP_CONCAT(CONCAT(COALESCE(ppp1.NumEtiqueta, ppp1.NumEtiquetaCanc),IF(ur1.Cancelado,'(Cancelado)',''))) AS Etiquetas
                            FROM uso_retalho_producao ur1
                                LEFT JOIN produto_pedido_producao ppp1 ON (ur1.IdProdPedProducao = ppp1.IdProdPedProducao)
                                LEFT JOIN produtos_pedido_espelho ppe ON (ppp1.IdProdPed = ppe.IdProdPed)
                                LEFT JOIN ambiente_pedido_espelho ape ON (ppe.IdAmbientePedido = ape.IdAmbientePedido)
                                LEFT JOIN pedido ped ON (ppe.IdPedido = ped.IdPedido)
                            GROUP BY ur1.IdRetalhoProducao)
                        ur ON (r.IdRetalhoProducao = ur.IdRetalhoProducao)
                    WHERE 1 ", campos,
                        string.Format(@"{0} JOIN
                            (SELECT urp.IdRetalhoProducao, lp.dataleitura FROM retalho_producao rp
                                INNER JOIN uso_retalho_producao urp ON (rp.IdRetalhoProducao = urp.IdRetalhoProducao)
                                INNER JOIN leitura_producao lp ON (urp.IdProdPedProducao = lp.IdProdPedProducao)
                                INNER JOIN setor s ON (lp.IdSetor = s.IdSetor)
                            WHERE s.Corte IS NOT NULL AND s.Corte = 1 {1} {2})
                            AS uso ON (uso.IdRetalhoProducao = r.IdRetalhoProducao)",

                            // Se estiver filtrando pela data de uso do retalho, é necessário fazer um INNER nesta tabela temporária, para que não venham retalhos com data de uso nula
                            !string.IsNullOrEmpty(dataUsoIni) || !string.IsNullOrEmpty(dataUsoFim) ? "INNER" : "LEFT",
                            !string.IsNullOrEmpty(dataUsoIni) ? "AND lp.DataLeitura >= ?dataUsoIni" : string.Empty,
                            !string.IsNullOrEmpty(dataUsoFim) ? "AND lp.DataLeitura <= ?dataUsoFim" : string.Empty),
                    calculoTotM,
                    PCPConfig.ExibirTotalM2RetalhoCorEspessura ?
                    @"LEFT JOIN cor_vidro cv ON (p.IdCorVidro=cv.IdCorVidro)" : string.Empty);

            string criterio = "";

            if (idRetalhoProducao > 0)
            {
                sql += " And r.IdRetalhoProducao=" + idRetalhoProducao;
                criterio += "Retalho: " + NumerosEtiquetas(idRetalhoProducao.ToString()) + "    ";
            }
            else if (!String.IsNullOrEmpty(idsRetalhosProducao))
            {
                sql += " And r.idRetalhoProducao in (" + idsRetalhosProducao + ")";
                criterio += "Retalhos: " + NumerosEtiquetas(idsRetalhosProducao) + "    ";
            }

            if (idProdPedProducao > 0)
            {
                sql += " And r.IdProdPedProducaoOrig=" + idProdPedProducao;
                criterio += "Etiqueta Origem: " + ProdutoPedidoProducaoDAO.Instance.ObtemEtiqueta(idProdPedProducao) + "    ";
            }

            if (idProd > 0)
            {
                sql += " And r.IdProd=" + idProd;
                criterio += "Produto: " + ProdutoDAO.Instance.GetCodInterno((int)idProd) + "    ";
            }
            else if (!string.IsNullOrEmpty(codInterno))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(codInterno, null);
                sql += " And r.idProd in (" + ids + ")";
                criterio += "Produto: " + codInterno + "    ";
            }
            else if (!String.IsNullOrEmpty(descrProduto))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descrProduto);
                sql += " And r.idProd in (" + ids + ")";
                criterio += "Produto: " + descrProduto + "    ";
            }

            if (!string.IsNullOrEmpty(dataIni))
            {
                sql += " And r.DataCad>=?dataIni";
                criterio += "Data início: " + dataIni + "    ";
            }

            if (!string.IsNullOrEmpty(dataFim))
            {
                sql += " And r.DataCad<=?dataFim";
                criterio += "Data término: " + dataFim + "    ";
            }

            if (!string.IsNullOrEmpty(dataUsoIni))
                criterio += string.Format("Data início uso: {0}    ", DateTime.Parse(dataUsoIni).ToString("dd/MM/yyyy"));

            if (!string.IsNullOrEmpty(dataUsoFim))
                criterio += string.Format("Data término uso: {0}    ", DateTime.Parse(dataUsoFim).ToString("dd/MM/yyyy"));

            if (situacao.HasValue && situacao.Value > 0)
            {
                sql += " And r.Situacao=" + (int)situacao.Value;
                criterio += "Situação: " + situacao.Translate().Format() + "    ";
            }

            if(!string.IsNullOrEmpty(idsCores))
            {
                sql += " And p.IdCorVidro in(" + idsCores + ")";
                criterio += "Cores: " + idsCores + "    ";
            }

            if (espessura > 0)
            {
                sql += " And p.Espessura =" + espessura;
                criterio += "Espessura Inicio: " + espessura + "    ";
            }
            
            if (alturaInicio > 0)
            {
                sql += " And p.Altura >=" + alturaInicio;
                criterio += "Altura Inicio: " + alturaInicio + "    ";
            }

            if (alturaFim > 0)
            {
                sql += " And p.Altura <=" + alturaFim;
                criterio += "Altura Fim: " + alturaFim + "    ";
            }

            if (larguraInicio > 0)
            {
                sql += " And p.Largura >=" + larguraInicio;
                criterio += "Largura Inicio: " + larguraInicio + "    ";
            }

            if (larguraFim > 0)
            {
                sql += " And p.Largura <=" + larguraFim;
                criterio += "Largura Fim: " + larguraFim + "    ";
            }

            if (!string.IsNullOrEmpty(numEtiqueta))
            {
                sql += " AND r.IdRetalhoProducao IN (SELECT IdRetalhoProducao FROM produto_impressao WHERE numEtiqueta = ?numEtq)";
                criterio += "Num. Etq.: " + numEtiqueta + "   ";
            }

            if (!string.IsNullOrEmpty(observacao))
            {
                sql += " AND p.Obs LIKE ?observacao";
                criterio += string.Format("Obs: {0}    ", observacao);
            }

            if (!string.IsNullOrEmpty(filtroAdicional))
                sql += filtroAdicional;

          
            sql += " GROUP BY r.IdRetalhoProducao";

            if (!selecionar)
                sql = "SELECT COUNT(*) FROM (" + sql + ") AS tmp";

            return sql.Replace("$$$", criterio);
        }

        private string NumerosEtiquetas(string idsRetalhosProducao)
        {
            if (String.IsNullOrEmpty(idsRetalhosProducao))
                return null;

            uint[] ids = Array.ConvertAll(idsRetalhosProducao.Split(','), x => Glass.Conversoes.StrParaUint(x));

            string retorno = "";
            foreach (uint id in ids)
                retorno += ObtemNumeroEtiqueta(id) + ", ";

            return retorno.TrimEnd(',', ' ');
        }

        private GDAParameter[] GetParam(string dataIni, string dataFim, string dataUsoIni, string dataUsoFim,
            string numEtiqueta, string observacao)
        {
            var lst = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!string.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            if (!string.IsNullOrEmpty(dataUsoIni))
                lst.Add(new GDAParameter("?dataUsoIni", DateTime.Parse(dataUsoIni + " 00:00:00")));

            if (!string.IsNullOrEmpty(dataUsoFim))
                lst.Add(new GDAParameter("?dataUsoFim", DateTime.Parse(dataUsoFim + " 23:59:59")));

            if (!string.IsNullOrEmpty(numEtiqueta))
                lst.Add(new GDAParameter("?numEtq", numEtiqueta));

            if (!string.IsNullOrEmpty(observacao))
                lst.Add(new GDAParameter("?observacao", string.Format("%{0}%", observacao)));

            return lst.ToArray();
        }
        
        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        public RetalhoProducao Obter(uint idRetalhoProducao)
        {
            return Obter(null, idRetalhoProducao);
        }

        public RetalhoProducao Obter(GDASession sessao, uint idRetalhoProducao)
        {
            return objPersistence.LoadOneData(sessao, Sql(idRetalhoProducao, null, 0, 0, null, null, null, null, null, null,
                null, null, 0, 0, 0, 0, 0, null, null, true, null));
        }

        public IList<RetalhoProducao> ObterLista(string codInterno, string descrProduto, string dataIni, string dataFim,
            string dataUsoIni, string dataUsoFim, Data.Model.SituacaoRetalhoProducao? situacao, string idsCores, double espessura, double alturaInicio,
            double alturaFim, double larguraInicio, double larguraFim, string numEtiqueta, string observacao,
            string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "idRetalhoProducao desc";
            return LoadDataWithSortExpression(Sql(0, null, 0, 0, codInterno, descrProduto, dataIni, dataFim, dataUsoIni,
                dataUsoFim, situacao, idsCores, espessura, alturaInicio, alturaFim, larguraInicio, larguraFim, numEtiqueta, observacao,
                true, null), sortExpression, startRow, pageSize, GetParam(dataIni, dataFim, dataUsoIni, dataUsoFim, numEtiqueta, observacao));
        }

        public IList<RetalhoProducao> GetForRpt(string codInterno, string descrProduto, string dataIni, string dataFim,
            string dataUsoIni, string dataUsoFim, SituacaoRetalhoProducao? situacao, string idsCores, double espessura, double alturaInicio,
            double alturaFim, double larguraInicio, double larguraFim, string numEtiqueta, string observacao)
        {
            string sql = Sql(0, null, 0, 0, codInterno, descrProduto, dataIni, dataFim, dataUsoIni, dataUsoFim, situacao,
                idsCores, espessura, alturaInicio, alturaFim, larguraInicio, larguraFim, numEtiqueta, observacao, true, null) +
                " order by idRetalhoProducao desc";
            return objPersistence.LoadData(sql, GetParam(dataIni, dataFim, dataUsoIni, dataUsoFim, numEtiqueta, observacao)).ToList();
        }

        public int ObterCount(string codInterno, string descrProduto, string dataIni, string dataFim,
            string dataUsoIni, string dataUsoFim, SituacaoRetalhoProducao? situacao, string idsCores, double espessura, double alturaInicio,
            double alturaFim, double larguraInicio, double larguraFim, string numEtiqueta, string observacao)
        {
            int count = objPersistence.ExecuteSqlQueryCount(Sql(0, null, 0, 0, codInterno, descrProduto, dataIni, dataFim,
                dataUsoIni, dataUsoFim, situacao, idsCores, espessura, alturaInicio, alturaFim, larguraInicio, larguraFim,
                numEtiqueta, observacao, false, null), GetParam(dataIni, dataFim, dataUsoIni, dataUsoFim, numEtiqueta, observacao));

            return count == 0 ? 1 : count;
        }

        public List<RetalhoProducao> ObterLista(string idsRetalhosProducao)
        {
            return objPersistence.LoadData(Sql(0, idsRetalhosProducao, 0, 0, null, null, null, null, null, null, null, null, 0, 0, 0, 0, 0, null, null, true, null)).ToList();
        }

        public List<RetalhoProducao> ObterLista(GDASession sessao, uint idProdPedProducao)
        {
            return objPersistence.LoadData(sessao, Sql(0, null, 0, idProdPedProducao, null, null, null, null, null, null, null, null, 0, 0, 0, 0, 0, null, null, true, null)).ToList();
        }
        
        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdOrig"></param>
        /// <returns></returns>
        public List<RetalhoProducao> ObterListaPorIdProdOrig(uint idProdOrig)
        {
            return ObterListaPorIdProdOrig(null, idProdOrig);
        }

        public List<RetalhoProducao> ObterListaPorIdProdOrig(GDASession sessao, uint idProdOrig)
        {
            var idsProdOrig = GetValoresCampo(@"select idProdBaixa from produto_baixa_estoque
                where idProd=" + idProdOrig, "idProdBaixa") + "," + idProdOrig;

            var filtroAdicional = " And (pbe.idProdBaixa in (" + idsProdOrig.Trim(',') + @") Or p.idProdOrig in (" + idsProdOrig.Trim(',') + @")
                Or p.idProdBase in (" + idsProdOrig.Trim(',') + @"))";

            var sql = Sql(0, null, 0, 0, null, null, null, null, null, null, SituacaoRetalhoProducao.Disponivel, null, 0, 0, 0, 0, 0, null, null, true, filtroAdicional) + 
                "  HAVING COUNT(ur.IdUsoRetalhoProducao) = 0 ";

            return objPersistence.LoadData(sessao, sql);
        }

        public List<RetalhoProducao> ObterRetalhosProducao(uint idProdPed)
        {
            return ObterRetalhosProducao(idProdPed, true);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <param name="usarFolga"></param>
        /// <returns></returns>
        public List<RetalhoProducao> ObterRetalhosProducao(uint idProdPed, bool usarFolga)
        {
            return ObterRetalhosProducao(null, idProdPed, usarFolga);
        }

        /// <summary>
        /// Recupera retalhos para usar na tela de impressão de etiquetas
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public List<RetalhoProducao> ObterRetalhosTelaImpressaoEtiqueta(uint idProdPed)
        {
            if (!PCPConfig.Etiqueta.UsarControleRetalhos)
                return new List<RetalhoProducao>();

            uint idProd;
            float alturaProducao;
            int larguraProducao;

            using (var dao = ProdutosPedidoEspelhoDAO.Instance)
            {
                idProd = dao.ObtemIdProd(idProdPed);
                alturaProducao = dao.ObtemAlturaProducao(idProdPed);
                larguraProducao = dao.ObtemLarguraProducao(idProdPed);
            }
            
            string calculoTotM =
                string.Format(@"SUM(IF(ped.TipoPedido<>{0}, (ppe.Altura * ppe.Largura) / 1000000,
                    (ape.Altura * ape.Largura) / 1000000))", (int)Pedido.TipoPedidoEnum.MaoDeObra);

            var listaRetalhos = objPersistence.LoadData(
                string.Format(@"
                    SELECT r.*, p.Descricao, p.CodInterno, p.Altura, p.Largura, f.Nome as NomeFunc, p.Obs
                    FROM retalho_producao r 
                        INNER JOIN produto p ON (r.IdProd=p.IdProd)
                        LEFT JOIN produto_pedido_producao ppp on(r.IdProdPedProducaoOrig = ppp.IdProdPedProducao)
	                    LEFT JOIN produto_baixa_estoque pbe ON (p.IdProdOrig = pbe.IdProd)
                        LEFT JOIN funcionario f ON (r.UsuCad = f.IdFunc)
                        LEFT JOIN
                            (SELECT ur1.IdUsoRetalhoProducao, ur1.IdRetalhoProducao
                            FROM uso_retalho_producao ur1
                            GROUP BY ur1.IdRetalhoProducao)
                        ur ON (r.IdRetalhoProducao = ur.IdRetalhoProducao)
                    WHERE r.Situacao={0} 
                         And (pbe.idProdBaixa in ({1}) Or p.idProdOrig in ({1}) Or p.idProdBase in ({1}))
                    GROUP BY r.IdRetalhoProducao 
                    HAVING COUNT(ur.IdUsoRetalhoProducao) = 0",

                    (int)SituacaoRetalhoProducao.Disponivel,
                    (GetValoresCampo(string.Format(@"select idProdBaixa from produto_baixa_estoque where idProd={0}", idProd), "idProdBaixa") + "," + idProd).TrimStart(',')));

            float folga = 1 + (float)PCPConfig.Etiqueta.FolgaRetalho / 100;

            var retalhosDisponiveis = new List<RetalhoProducao>();
            foreach (RetalhoProducao r in listaRetalhos)
            {
                bool validacaoAlturaLargura = r.Altura >= alturaProducao && r.Largura >= larguraProducao;
                bool validacaoLarguraAltura = r.Largura >= alturaProducao && r.Altura >= larguraProducao;

                bool validacaoAlturaLarguraFolga = validacaoAlturaLargura && (r.Altura <= (alturaProducao * folga) &&
                    r.Largura >= larguraProducao && r.Largura <= (larguraProducao * folga));

                bool validacaoLarguraAlturaFolga = validacaoLarguraAltura && (r.Largura <= (alturaProducao * folga) &&
                    r.Altura <= (larguraProducao * folga));

                if (validacaoAlturaLargura || validacaoLarguraAltura)
                {
                    r.DentroFolga = folga == 1 || validacaoAlturaLarguraFolga || validacaoLarguraAlturaFolga;
                    retalhosDisponiveis.Add(r);
                }
            }

            return retalhosDisponiveis;
        }

        public List<RetalhoProducao> ObterRetalhosProducao(GDASession sessao, uint idProdPed, bool usarFolga)
        {
            if (!PCPConfig.Etiqueta.UsarControleRetalhos)
                return new List<RetalhoProducao>();

            uint idProd;
            float alturaProducao;
            int larguraProducao;

            using (var dao = ProdutosPedidoEspelhoDAO.Instance)
            {
                idProd = dao.ObtemIdProd(sessao, idProdPed);
                alturaProducao = dao.ObtemAlturaProducao(sessao, idProdPed);
                larguraProducao = dao.ObtemLarguraProducao(sessao, idProdPed);
            }

            float folga = 1 + (float)PCPConfig.Etiqueta.FolgaRetalho / 100;

            usarFolga = usarFolga && folga > 1;

            List<RetalhoProducao> output = new List<RetalhoProducao>();

            foreach (RetalhoProducao r in RetalhoProducaoDAO.Instance.ObterListaPorIdProdOrig(sessao, idProd))
            {
                bool validacaoAlturaLargura = r.Altura >= alturaProducao && r.Largura >= larguraProducao;
                bool validacaoLarguraAltura = r.Largura >= alturaProducao && r.Altura >= larguraProducao;

                bool validacaoAlturaLarguraFolga = validacaoAlturaLargura && (r.Altura <= (alturaProducao * folga) &&
                    r.Largura >= larguraProducao && r.Largura <= (larguraProducao * folga));

                bool validacaoLarguraAlturaFolga = validacaoLarguraAltura && (r.Largura <= (alturaProducao * folga) &&
                    r.Altura <= (larguraProducao * folga));

                if ((usarFolga && (validacaoAlturaLarguraFolga || validacaoLarguraAlturaFolga)) ||
                    (!usarFolga && (validacaoAlturaLargura || validacaoLarguraAltura)))
                {
                    r.DentroFolga = folga == 1 || validacaoAlturaLarguraFolga || validacaoLarguraAlturaFolga;
                    output.Add(r);
                }
            }

            return output;
        }

        public int AlteraSituacao(GDASession sessao,  uint id, SituacaoRetalhoProducao situacao)
        {
            var retalho = GetElementByPrimaryKey(sessao, id);

            var sql = "update retalho_producao set Situacao=" + (int)situacao + " where idRetalhoProducao=" + id;
            var retorno = objPersistence.ExecuteCommand(sessao, sql);

            LogAlteracaoDAO.Instance.LogRetalhoProducao(sessao, retalho, UserInfo.GetUserInfo.CodUser);
            return retorno;
        }

        public int AlteraSituacao(uint id, SituacaoRetalhoProducao situacao)
        {
            return AlteraSituacao(null, id, situacao);
        }

        public SituacaoRetalhoProducao ObtemSituacao(GDASession sessao, uint idRetalhoProducao)
        {
            return ObtemValorCampo<SituacaoRetalhoProducao>(sessao, "situacao", "idRetalhoProducao=" + idRetalhoProducao);
        }

        public SituacaoRetalhoProducao ObtemSituacao(uint idRetalhoProducao)
        {
            return ObtemSituacao(null, idRetalhoProducao);
        }

        #region Validação dos retalhos

        public bool ValidaRetalhos(List<RetalhoProducaoAuxiliar> dadosRetalho, uint idProd, uint idProdNf)
        {
            return ValidaRetalhos(null, dadosRetalho, idProd, idProdNf);
        }

        public bool ValidaRetalhos(GDASession session, List<RetalhoProducaoAuxiliar> dadosRetalho, uint idProd, uint idProdNf)
        {
            if (idProd == 0 || idProdNf == 0)
                throw new Exception("Produto inválido.");

            List<int> altura = new List<int>(), largura = new List<int>(), quantidade = new List<int>();
            foreach (RetalhoProducaoAuxiliar r in dadosRetalho)
            {
                altura.Add((int)r.Altura);
                largura.Add((int)r.Largura);
                quantidade.Add(r.Quantidade);
            }

            float alturaProd = ProdutosNfDAO.Instance.ObtemValorCampo<float>(session, "altura", "idProdNf=" + idProdNf);
            int larguraProd = ProdutosNfDAO.Instance.ObtemValorCampo<int>(session, "largura", "idProdNf=" + idProdNf);
            float qtdeProd = ProdutosNfDAO.Instance.ObtemValorCampo<float>(session, "qtde", "idProdNf=" + idProdNf);

            return ValidaRetalhos(session, altura.ToArray(), largura.ToArray(), quantidade.ToArray(),
                idProd, (int)alturaProd, larguraProd, qtdeProd, false);
        }

        public bool ValidaRetalhos(List<RetalhoProducaoAuxiliar> dadosRetalho, string numEtiqueta)
        {
            if (dadosRetalho.Count == 0)
                return true;

            string altura = "", largura = "", quantidade = "";
            foreach (RetalhoProducaoAuxiliar r in dadosRetalho)
            {
                altura += r.Altura + ";";
                largura += r.Largura + ";";
                quantidade += r.Quantidade + ";";
            }

            return ValidaRetalhos(altura.TrimEnd(';'), largura.TrimEnd(';'), quantidade.TrimEnd(';'), numEtiqueta);
        }

        public bool ValidaRetalhos(string altura, string largura, string quantidade, string numEtiqueta)
        {
            return ValidaRetalhos(null, altura, largura, quantidade, numEtiqueta);
        }

        public bool ValidaRetalhos(GDASession session, string altura, string largura, string quantidade, string numEtiqueta)
        {
            // Valida a etiqueta
            uint idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(session, numEtiqueta).GetValueOrDefault();
            if (idProdPedProducao == 0)
                throw new Exception("Etiqueta não encontrada.");

            if (PCPConfig.ImpedirCriarRetalhoPecaRepostaNaoCortada &&
                !LeituraProducaoDAO.Instance.PecaFoiCortada(session, idProdPedProducao) &&
                !ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(session, idProdPedProducao, true))
                throw new Exception("Não é possível criar o retalho, pois a peça ainda não passou pelo corte");

            // Garante que a etiqueta ainda não esteja temperada
            var idsSetor = LeituraProducaoDAO.Instance.ObterSetoresLidos(session, (int)idProdPedProducao);
            foreach (var s in idsSetor?.Where(f => f > 0))
                if (Utils.ObtemSetor((uint)s)?.Forno ?? false)
                    throw new Exception("Peça já passou pelo forno. Não é possível gerar retalhos de peças temperadas.");

            // Separa os valores dos retalhos
            int[] alturaArray = Array.ConvertAll(altura.Split(';'), x => Glass.Conversoes.StrParaInt(x));
            int[] larguraArray = Array.ConvertAll(largura.Split(';'), x => Glass.Conversoes.StrParaInt(x));
            int[] quantidadeArray = Array.ConvertAll(quantidade.Split(';'), x => Glass.Conversoes.StrParaInt(x));

            if (alturaArray.Length != larguraArray.Length || alturaArray.Length != quantidadeArray.Length)
                throw new Exception("Dados de retalhos inválidos.");

            uint idProdPed = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(session, idProdPedProducao);
            uint idProd = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd(session, idProdPed);
            float alturaEtiq = ProdutosPedidoEspelhoDAO.Instance.ObtemAlturaProducao(session, idProdPed);
            int larguraEtiq = ProdutosPedidoEspelhoDAO.Instance.ObtemLarguraProducao(session, idProdPed);
            //float qtdeEtiq = ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(idProdPed);
            bool redondoEtiq = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<bool>(session, "redondo", "idProdPed=" + idProdPed);

            // Valida o tamanho dos retalhos
            return ValidaRetalhos(session, alturaArray, larguraArray, quantidadeArray, idProd, (int)alturaEtiq, larguraEtiq, 1, redondoEtiq);
        }
        private bool ValidaRetalhos(GDASession session, int[] alturas, int[] larguras, int[] quantidades, uint idProd, int alturaPeca, int larguraPeca, float qtdePeca, bool isRedondo)
        {
            float totMTotal = 0, totMPeca = 0;

            /* Chamado 66405. */
            if (ProdutoDAO.Instance.IsProdutoLamComposicao(session, (int)idProd))
                throw new Exception("A peça é um produto composto, portanto as peças de composição já foram temperadas e, por isso, não é possível gerar o retalho.");

            var pecas = new List<int[]>();

            for (int i = 0; i < alturas.Length; i++)
            {
                pecas.Add(new[] { alturas[i], larguras[i] });
            }

            if (ValidaMedidasRetalho(pecas, alturaPeca, larguraPeca).Count() > 0)
                throw new Exception(string.Join("\n", ValidaMedidasRetalho(pecas, alturaPeca, larguraPeca).ToArray()));

            totMPeca = Global.CalculosFluxo.ArredondaM2(session, larguraPeca, alturaPeca, qtdePeca, (int)idProd, isRedondo);

            for (var i = 0; i < alturas.Length; i++)
                totMTotal += Global.CalculosFluxo.ArredondaM2(session, larguras[i], alturas[i], quantidades[i], (int)idProd, isRedondo);

            if (totMTotal > totMPeca)
                return false;

            return true;
        }

        public List<string> ValidaMedidasRetalho(List<int[]> pecas, int altura, int largura)
        {

            var validacoes = new List<string>();

            foreach (var peca in pecas)
            {
                if (peca[0] > altura && peca[1] > altura)
                    validacoes.Add($"As Medidas da peça {peca[0]} X {peca[1]} ultrapassam os limites para a altura {altura}.");
                if (peca[0] > largura && peca[1] > largura)
                    validacoes.Add($"As Medidas da peça {peca[0]} X {peca[1]} ultrapassam os limites para a largura {largura}.");
            }

            return validacoes;
        }

        #endregion

        public List<uint> CriarRetalho(string altura, string largura, string quantidade, string observacao, string numEtiqueta)
        {
            return CriarRetalho(altura, largura, quantidade, observacao, numEtiqueta, null);
        }

        private static readonly object _criarRetalhoLock = new object();

        public List<uint> CriarRetalho(string altura, string largura, string quantidade, string observacao, string numEtiqueta,
            LoginUsuario usuario)
        {
            lock(_criarRetalhoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var idsRetalho = new List<uint>();

                        // Valida os retalhos
                        if (!ValidaRetalhos(transaction, altura, largura, quantidade, numEtiqueta))
                            throw new Exception("Erro de validação de retalhos.");

                        //Separa os valores dos retalhos
                        string[] alturaArray = altura.Split(';');
                        string[] larguraArray = largura.Split(';');
                        string[] quantidadeArray = quantidade.Split(';');
                        string[] observacaoArray = observacao.Split(';');

                        //Recupera ProdutoPedidoProducao a partir da etiqueta
                        ProdutoPedidoProducao ppp = ProdutoPedidoProducaoDAO.Instance.GetByEtiqueta(transaction, numEtiqueta);

                        //Recupera ProdutosPedidoEspelho a partir de ProdutoPedidoProducao
                        ProdutosPedidoEspelho ppe = ProdutosPedidoEspelhoDAO.Instance.GetElement(transaction, ppp.IdProdPed.GetValueOrDefault(), false);

                        // Usa a matéria prima da peça para gerar o retalho
                        var prodBaixa = ProdutoBaixaEstoqueDAO.Instance.GetByProd(transaction, (uint)ppe.IdProd, false);
                        var idProdRetalho = prodBaixa != null && prodBaixa.Length > 0 ? (uint)prodBaixa[0].IdProdBaixa : ppe.IdProd;

                        // Recupera o produto do retalho
                        Produto produto = ProdutoDAO.Instance.GetElement(transaction, idProdRetalho);

                        for (int i = 0; i < alturaArray.Length; i++)
                        {
                            //Verifica se o produto existe
                            Produto novoProduto = ProdutoDAO.Instance.ObterProduto(transaction, string.Format("{0}-{1}x{2}-R", produto.CodInterno, alturaArray[i], larguraArray[i]));

                            RetalhoProducao retalho;

                            //Se não existir cria um novo
                            if (novoProduto == null)
                            {
                                novoProduto = MetodosExtensao.Clonar(produto);
                                novoProduto.Altura = Glass.Conversoes.StrParaInt(alturaArray[i]);
                                novoProduto.Largura = Glass.Conversoes.StrParaInt(larguraArray[i]);
                                novoProduto.IdGrupoProd = (int)Glass.Data.Model.NomeGrupoProd.Vidro;
                                novoProduto.IdSubgrupoProd = (int)Utils.SubgrupoProduto.RetalhosProducao;
                                novoProduto.CodInterno = produto.CodInterno + "-" + novoProduto.Altura + "x" + novoProduto.Largura + "-R";
                                novoProduto.IdProdOrig = produto.IdProd;
                                novoProduto.Situacao = Glass.Situacao.Ativo;
                                novoProduto.Obs = observacaoArray != null && observacaoArray.Length > 0 && observacaoArray.Length >= i - 1 ? observacaoArray[i] : null;
                                novoProduto.DadosBaixaEstoque.Clear();

                                /* Chamado 31821. */
                                novoProduto.Usucad = usuario != null ? usuario.CodUser : UserInfo.GetUserInfo.CodUser;

                                // Chamado 65546
                                var m2 = (novoProduto.Altura.GetValueOrDefault(0) * novoProduto.Largura.GetValueOrDefault(0)) / 1000000m;
                                novoProduto.ValorAtacado = m2 * produto.ValorAtacado;
                                novoProduto.ValorBalcao = m2 * produto.ValorBalcao;
                                novoProduto.ValorObra = m2 * produto.ValorObra;

                                ProdutoDAO.Instance.Insert(transaction, novoProduto);
                            }
                            else
                            {
                                novoProduto.Descricao = produto.Descricao;
                                novoProduto.DadosBaixaEstoque.Clear();

                                // Chamado 65546
                                var m2 = (novoProduto.Altura.GetValueOrDefault(0) * novoProduto.Largura.GetValueOrDefault(0)) / 1000000m;
                                novoProduto.ValorAtacado = m2 * produto.ValorAtacado;
                                novoProduto.ValorBalcao = m2 * produto.ValorBalcao;
                                novoProduto.ValorObra = m2 * produto.ValorObra;

                                ProdutoDAO.Instance.Update(transaction, novoProduto);
                            }

                            for (int q = 0; q < Glass.Conversoes.StrParaInt(quantidadeArray[i]); q++)
                            {
                                retalho = new RetalhoProducao();
                                retalho.DataCad = DateTime.Now;
                                retalho.IdProd = (uint)novoProduto.IdProd;
                                retalho.IdProdPedProducaoOrig = ppp.IdProdPedProducao;
                                retalho.Situacao = PCPConfig.SituacaoRetalhoAoCriar;
                                /* Chamado 31821. */
                                retalho.Usuario = usuario != null ? usuario : UserInfo.GetUserInfo;

                                // Recupera o idProdNf da chapa de vidro
                                retalho.IdProdNf = ExecuteScalar<uint>(transaction, @"
                                select pic.idProdNf from produto_impressao pic
                                    right join chapa_corte_peca ccp on (pic.idProdImpressao=ccp.idProdImpressaoChapa)
                                    right join produto_impressao pip on (ccp.idProdImpressaoPeca=pip.idProdImpressao)
                                    left join impressao_etiqueta ie on (pip.idImpressao=ie.idImpressao)
                                where pip.numEtiqueta=?etiq and !coalesce(pip.cancelado, false) and ie.situacao=" +
                                            (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa, new GDAParameter("?etiq", numEtiqueta));

                                retalho.IdProdNf = retalho.IdProdNf == 0 ? null : retalho.IdProdNf;
                                uint id = Insert(transaction, retalho);
                                idsRetalho.Add(id);
                            }
                        }

                        transaction.Commit();
                        transaction.Close();

                        return idsRetalho;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException(string.Format("Falha ao criar retalho. Etiqueta: {0}.", numEtiqueta), ex);
                        throw ex;
                    }
                }
            }
        }

        public List<uint> CriarRetalho(List<RetalhoProducaoAuxiliar> dadosRetalho, uint idProd, uint idProdNf)
        {
            return CriarRetalho(dadosRetalho, idProd, idProdNf, null);
        }

        public List<uint> CriarRetalho(List<RetalhoProducaoAuxiliar> dadosRetalho, uint idProd, uint idProdNf, LoginUsuario usuario)
        {
            lock(_criarRetalhoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var tipoSubGrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(transaction, (int)idProd);
                        // Busca o ID do produto matéria-prima, se for chapa de vidro
                        if (tipoSubGrupo == TipoSubgrupoProd.ChapasVidro || tipoSubGrupo == TipoSubgrupoProd.ChapasVidroLaminado)
                            idProd = ProdutoDAO.Instance.ObtemValorCampo<uint>(transaction, "idProdBase", "idProd=" + idProd);

                        // Valida os retalhos
                        if (!ValidaRetalhos(transaction, dadosRetalho, idProd, idProdNf))
                            throw new Exception("Erro de validação de retalhos. Verifique se a altura e largura do retalho são menores que a chapa assim como a metragem total.");
                        
                        List<uint> idsRetalho = new List<uint>();

                        //Separa os valores dos retalhos
                        string[] alturaArray = Array.ConvertAll(dadosRetalho.ToArray(), x => x.Altura.ToString());
                        string[] larguraArray = Array.ConvertAll(dadosRetalho.ToArray(), x => x.Largura.ToString());
                        string[] quantidadeArray = Array.ConvertAll(dadosRetalho.ToArray(), x => x.Quantidade.ToString());
                        string[] observacaoArray = Array.ConvertAll(dadosRetalho.ToArray(), x => x.Observacao != null ? x.Observacao.ToString() : string.Empty);

                        //Recupera Produto a partir de ProdutosPedidoEspelho
                        Produto produto = ProdutoDAO.Instance.GetElement(transaction, idProd);

                        for (int i = 0; i < alturaArray.Length; i++)
                        {
                            // Verifica se o produto existe
                            Produto novoProduto = ProdutoDAO.Instance.ObterProduto(transaction, string.Format("{0}-{1}x{2}-R", produto.CodInterno, alturaArray[i], larguraArray[i]));

                            RetalhoProducao retalho;

                            //Se não existir cria um novo
                            if (novoProduto == null)
                            {
                                novoProduto = MetodosExtensao.Clonar(produto);
                                novoProduto.Altura = Glass.Conversoes.StrParaInt(alturaArray[i]);
                                novoProduto.Largura = Glass.Conversoes.StrParaInt(larguraArray[i]);
                                novoProduto.IdGrupoProd = (int)Glass.Data.Model.NomeGrupoProd.Vidro;
                                novoProduto.IdSubgrupoProd = (int)Utils.SubgrupoProduto.RetalhosProducao;
                                novoProduto.CodInterno = produto.CodInterno + "-" + novoProduto.Altura + "x" + novoProduto.Largura + "-R";
                                novoProduto.IdProdOrig = produto.IdProd;
                                novoProduto.Situacao = Glass.Situacao.Ativo;
                                novoProduto.Obs = observacaoArray != null && observacaoArray.Length > 0 && observacaoArray.Length >= i - 1 ? observacaoArray[i] : null;

                                /* Chamado 31821. */
                                novoProduto.Usucad = usuario != null ? usuario.CodUser : UserInfo.GetUserInfo.CodUser;

                                // Chamado 65546
                                var m2 = (novoProduto.Altura.GetValueOrDefault(0) * novoProduto.Largura.GetValueOrDefault(0)) / 1000000m;
                                novoProduto.ValorAtacado = m2 * produto.ValorAtacado;
                                novoProduto.ValorBalcao = m2 * produto.ValorBalcao;
                                novoProduto.ValorObra = m2 * produto.ValorObra;

                                uint id = ProdutoDAO.Instance.Insert(transaction, novoProduto);
                            }
                            else
                            {
                                novoProduto.Descricao = produto.Descricao;

                                // Chamado 65546
                                var m2 = (novoProduto.Altura.GetValueOrDefault(0) * novoProduto.Largura.GetValueOrDefault(0)) / 1000000m;
                                novoProduto.ValorAtacado = m2 * produto.ValorAtacado;
                                novoProduto.ValorBalcao = m2 * produto.ValorBalcao;
                                novoProduto.ValorObra = m2 * produto.ValorObra;

                                ProdutoDAO.Instance.Update(transaction, novoProduto);
                            }

                            for (int q = 0; q < Glass.Conversoes.StrParaInt(quantidadeArray[i]); q++)
                            {
                                retalho = new RetalhoProducao();
                                retalho.DataCad = DateTime.Now;
                                retalho.IdProd = (uint)novoProduto.IdProd;
                                retalho.IdProdNf = idProdNf;
                                retalho.Situacao = PCPConfig.SituacaoRetalhoAoCriar;
                                /* Chamado 31821. */
                                retalho.Usuario = usuario != null ? usuario : UserInfo.GetUserInfo;

                                uint id = Insert(transaction, retalho);
                                idsRetalho.Add(id);
                            }
                        }

                        transaction.Commit();
                        transaction.Close();

                        return idsRetalho;
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }

        public string ObtemNumeroEtiqueta(uint idRetalhoProducao)
        {
            return ObtemNumeroEtiqueta(null, idRetalhoProducao);
        }

        public string ObtemNumeroEtiqueta(GDASession session, uint idRetalhoProducao)
        {
            uint idProdImpressaoRetalho = ProdutoImpressaoDAO.Instance.ObtemValorCampo<uint>(session, "idProdImpressao",
                "idRetalhoProducao=" + idRetalhoProducao);

            using (var dao = ProdutoImpressaoDAO.Instance)
                if (dao.Exists(session, idProdImpressaoRetalho))
                    return dao.ObtemNumEtiqueta(session, idProdImpressaoRetalho);

            return null;
        }

        public void AssociarProducao(GDASession session, uint idRetalhoProducao, uint idProdPedProducaoNovo, uint idFunc)
        {
            var retalho = GetElementByPrimaryKey(session, idRetalhoProducao);
            var retalhoAssociado = false;

            try
            {
                UsoRetalhoProducaoDAO.Instance.AssociarRetalho(session, idRetalhoProducao, idProdPedProducaoNovo, true);
                retalhoAssociado = true;

                var sql = "update retalho_producao set situacao=" + (int)SituacaoRetalhoProducao.EmUso + " where IdRetalhoProducao=" + idRetalhoProducao;
                objPersistence.ExecuteCommand(session, sql);
            }
            catch
            {
                if (retalhoAssociado)
                    UsoRetalhoProducaoDAO.Instance.RemoverAssociacao(session, idRetalhoProducao, idProdPedProducaoNovo);
            }

            LogAlteracaoDAO.Instance.LogRetalhoProducao(session, retalho, idFunc);
        }

        public override uint Insert(RetalhoProducao objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession session, RetalhoProducao objInsert)
        {
            uint id = base.Insert(session, objInsert);
            var idLoja = objInsert.Usuario != null ? objInsert.Usuario.IdLoja : UserInfo.GetUserInfo.IdLoja;
            MovEstoqueDAO.Instance.CreditaEstoqueRetalho(session, objInsert.IdProd, idLoja, id, 1, objInsert.Usuario);
            
            return id;
        }

        public override int Delete(RetalhoProducao objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdRetalhoProducao);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            CancelarComTransacao(UserInfo.GetUserInfo.CodUser, Key, "Exclusão", true, true, true);
            return 0;
        }

        private static readonly object _cancelarRetalhoLock = new object();

        public void CancelarComTransacao(uint idFunc, uint idRetalhoProducao, string motivo, bool cancelarProdutoImpresso, bool manual, bool validar)
        {
            lock(_cancelarRetalhoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        Cancelar(transaction, idFunc, idRetalhoProducao, motivo, true, true, true);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }

        internal bool PodeCancelar(GDASession session, uint idRetalhoProducao)
        {
            var situacao = ObtemValorCampo<SituacaoRetalhoProducao>(session, "situacao", "idRetalhoProducao=" + idRetalhoProducao);

            if (situacao == SituacaoRetalhoProducao.Perda)
                return false;

            if (situacao == SituacaoRetalhoProducao.Cancelado || situacao == SituacaoRetalhoProducao.Vendido)
                return false;
            else if (situacao != SituacaoRetalhoProducao.EmUso)
                return true;

            var idsProdPedProducao = UsoRetalhoProducaoDAO.Instance.ObtemIdsProdPedProducao(session, idRetalhoProducao);

            foreach (var id in idsProdPedProducao)
            {
                if (!ProdutoPedidoProducaoDAO.Instance.PecaEstaCancelada(session, id) || LeituraProducaoDAO.Instance.PecaFoiCortada(session, id))
                    return false;
            }

            return true;
        }

        internal void Cancelar(GDASession session, uint idFunc, uint idRetalhoProducao, string motivo, bool cancelarProdutoImpresso, bool manual, bool validar)
        {
            if (validar && !PodeCancelar(session, idRetalhoProducao))
                throw new Exception("Não é possível cancelar um retalho em uso.");

            RetalhoProducao retalho = Obter(idRetalhoProducao);

            if (cancelarProdutoImpresso && retalho.NumeroEtiqueta != null)
            {
                uint idProdImpressao = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(retalho.NumeroEtiqueta,
                    ProdutoImpressaoDAO.TipoEtiqueta.Retalho);

                ImpressaoEtiquetaDAO.Instance.CancelarImpressao(session, idFunc, 0, null, null, null, idProdImpressao,
                    "Cancelamento do retalho " + retalho.NumeroEtiqueta, false);
            }

            objPersistence.ExecuteCommand(session, "update retalho_producao set situacao=" + (int)SituacaoRetalhoProducao.Cancelado +
                " where idRetalhoProducao=" + idRetalhoProducao);

            if (retalho.Situacao == SituacaoRetalhoProducao.Disponivel)
                MovEstoqueDAO.Instance.BaixaEstoqueRetalho(session, retalho.IdProd, UserInfo.GetUserInfo.IdLoja, idRetalhoProducao, 1);

            LogCancelamentoDAO.Instance.LogRetalhoProducao(session, idFunc, retalho, "Cancelamento do retalho " +
                retalho.NumeroEtiqueta + " - " + motivo, manual);
        }

        public uint ObtemIdProd(GDASession trans, uint idRetalho)
        {
            return ObtemValorCampo<uint>(trans, "IdProd", "IdRetalhoProducao = " + idRetalho);
        }

        private static readonly object _marcarPerdaLock = new object();

        public void MarcarPerda(uint idFunc, uint idRetalhoProducao, uint idTipoPerda, uint? idSubtipoPerda, string motivo)
        {
            lock(_marcarPerdaLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        RetalhoProducao retalho = Obter(transaction, idRetalhoProducao);

                        if (retalho == null)
                            throw new Exception("O retalho não foi encontrado");

                        if (retalho.Situacao == SituacaoRetalhoProducao.Cancelado || retalho.Situacao == SituacaoRetalhoProducao.Vendido || retalho.Situacao == SituacaoRetalhoProducao.Perda)
                            throw new Exception("Apenas retalhos com as situçãoes Disponível, Em Uso ou Em Estoque podem ser marcados como perda");
                        
                        if (!string.IsNullOrEmpty(retalho.NumeroEtiqueta))
                        {
                            uint idProdImpressao = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(transaction, retalho.NumeroEtiqueta, ProdutoImpressaoDAO.TipoEtiqueta.Retalho);

                            if (ChapaCortePecaDAO.Instance.ChapaPossuiLeitura(transaction, idProdImpressao))
                                throw new Exception("O retalho possui leituras na produção, cancele antes de marcar a perda");

                            ImpressaoEtiquetaDAO.Instance.CancelarImpressao(transaction, idFunc, 0, null, null, null, idProdImpressao,
                                "Cancelamento do retalho " + retalho.NumeroEtiqueta, false);

                            PerdaChapaVidroDAO.Instance.MarcaPerdaRetalho(transaction, retalho.NumeroEtiqueta, idProdImpressao, idTipoPerda, idSubtipoPerda, motivo);
                        }

                        objPersistence.ExecuteCommand(transaction, "update retalho_producao set situacao=" + (int)SituacaoRetalhoProducao.Perda + " where idRetalhoProducao=" + idRetalhoProducao);

                        if (retalho.Situacao == SituacaoRetalhoProducao.Disponivel)
                            MovEstoqueDAO.Instance.BaixaEstoqueRetalho(transaction, retalho.IdProd, UserInfo.GetUserInfo.IdLoja, idRetalhoProducao, 1);

                        LogCancelamentoDAO.Instance.LogRetalhoProducao(transaction, idFunc, retalho, "Perda do retalho " + retalho.NumeroEtiqueta + " - " + motivo, true);
                        
                        transaction.Commit();
                        transaction.Close();
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }
    }
}
