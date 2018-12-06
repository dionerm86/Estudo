using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Configuracoes;
using Glass.Global;
using System.Linq;
using Glass.Data.Helper.Calculos;
using Glass.Data.Model.Calculos;

namespace Glass.Data.DAL
{
    public sealed class ProdutoTrocadoDAO : BaseDAO<ProdutoTrocado, ProdutoTrocadoDAO>
    {
        //private ProdutoTrocadoDAO() { }

        private string Sql(uint idTrocaDevolucao, uint idPedido, int tipo, int situacao, uint idCliente, string nomeCliente, string idsFuncionario,
            string idsFuncionarioAssociadoCliente, uint idProduto, string dataIni, string dataFim, bool selecionar, bool rptPerdasExternas,
            int idOrigemTrocaDevolucao, uint idTipoPerda, int idSetor)
        {
            return Sql(null, idTrocaDevolucao, idPedido, tipo, situacao, idCliente, nomeCliente, idsFuncionario, idsFuncionarioAssociadoCliente, idProduto,
                dataIni, dataFim, selecionar, rptPerdasExternas, idOrigemTrocaDevolucao, idTipoPerda, idSetor);
        }

        private string Sql(GDASession session, uint idTrocaDevolucao, uint idPedido, int tipo, int situacao, uint idCliente, string nomeCliente, string idsFuncionario,
            string idsFuncionarioAssociadoCliente, uint idProduto, string dataIni, string dataFim, bool selecionar, bool rptPerdasExternas,
            int idOrigemTrocaDevolucao, uint idTipoPerda, int idSetor)
        {
            string criterio = "";
            string campos = selecionar ? @"pt.*, td.idCliente, (pp.qtde-coalesce((select sum(qtde) from produto_trocado pt1 inner join troca_devolucao td1 on 
                (pt1.idTrocaDevolucao=td1.idTrocaDevolucao) where pt1.idProdPed=pt.idProdPed and td1.situacao<>" +
                (int)TrocaDevolucao.SituacaoTrocaDev.Cancelada + @"),0)) as qtdeProd, p.codInterno, p.descricao as descrProduto, p.custoCompra as custoCompraProduto,
                ep.codInterno as codProcesso, um.codigo as unidade, ea.codInterno as codAplicacao, p.idGrupoProd, p.idSubgrupoProd, '$$$' as criterio" +
                (rptPerdasExternas ? @", td.idPedido, fpe.nome as nomeConferente, td.descricao as descrTrocaDev, td.idFunc as idFuncTrocaDev, 
                ftd.nome as nomeFuncTrocaDev, c.IdFunc AS IdFuncionarioAssociadoCliente, fc.Nome AS NomeFuncionarioAssociadoCliente" : "") : "count(*)";

            string sql = @"
                select " + campos + @"
                from produto_trocado pt
                    left join troca_devolucao td on (pt.idTrocaDevolucao=td.idTrocaDevolucao)
                    left join produtos_pedido pp on (pt.idProdPed=pp.idProdPed)
                    left join produto p on (pt.idProd=p.idProd)
                    Left Join unidade_medida um On (p.idUnidadeMedida=um.idUnidadeMedida) 
                    left join etiqueta_processo ep on (pt.idProcesso=ep.idProcesso)
                    left join etiqueta_aplicacao ea on (pt.idAplicacao=ea.idAplicacao)" +
                    (rptPerdasExternas ? @"
                    left join pedido_espelho pe on (pp.idPedido=pe.idPedido)
                    left join funcionario fpe on (pe.idFuncConf=fpe.idFunc)
                    left join funcionario ftd on (td.idFunc=ftd.idFunc)
                    left join cliente c on (td.idCliente=c.id_Cli)
                    left join funcionario fc on (c.IdFunc=fc.IdFunc)" : "") + @"
                where 1";

            if (idTrocaDevolucao > 0)
            {
                sql += " and pt.idTrocaDevolucao=" + idTrocaDevolucao;
                criterio += "Troca/devolução: " + idTrocaDevolucao + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " and td.dataTroca>=?dataIni";
                criterio += (!String.IsNullOrEmpty(dataFim) ? "Período: de " + dataIni : "Período: a partir de " + dataIni) + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " and td.dataTroca<=?dataFim";
                criterio += (!String.IsNullOrEmpty(dataIni) ? " até " + dataFim : "Período: até " + dataFim) + "    ";
            }

            if (idOrigemTrocaDevolucao > 0)
            {
                sql += " AND td.IdOrigemTrocaDevolucao = " + idOrigemTrocaDevolucao;
                criterio += "Origem: " + OrigemTrocaDescontoDAO.Instance.ObtemDescricao(session, (uint)idOrigemTrocaDevolucao) + " ";
            }

            if (rptPerdasExternas)
            {
                TrocaDevolucao temp = new TrocaDevolucao();

                if (idPedido > 0)
                {
                    sql += " and td.idPedido=" + idPedido;
                    criterio += "Pedido: " + idPedido + "    ";
                }

                if (tipo > 0)
                {
                    sql += " and td.tipo=" + tipo;
                    temp.Tipo = tipo;
                    criterio += "Tipo: " + temp.DescrTipo + "    ";
                }

                if (situacao > 0)
                {
                    sql += " and td.situacao=" + situacao;
                    temp.Situacao = situacao;
                    criterio += "Situação: " + temp.DescrSituacao + "    ";
                }

                if (idCliente > 0)
                {
                    sql += " and td.idCliente=" + idCliente;
                    criterio += "Cliente: " + ClienteDAO.Instance.GetNome(session, idCliente) + "    ";
                }
                else if (!String.IsNullOrEmpty(nomeCliente))
                {
                    string ids = ClienteDAO.Instance.GetIds(session, null, nomeCliente, null, 0, null, null, null, null, 0);
                    sql += " And c.id_Cli in (" + ids + ")";
                    criterio += "Cliente: " + nomeCliente + "    ";
                }

                if (!String.IsNullOrEmpty(idsFuncionario) && idsFuncionario != "0")
                {
                    sql += " and td.idFunc IN (" + idsFuncionario + ")";
                    var criterioFunc = String.Empty;

                    foreach (var id in idsFuncionario.Split(','))
                        criterioFunc += FuncionarioDAO.Instance.GetNome(session, Glass.Conversoes.StrParaUint(id)) + "    ";

                    criterio += "Funcionário: " + criterioFunc;
                }

                if (!String.IsNullOrEmpty(idsFuncionarioAssociadoCliente) && idsFuncionarioAssociadoCliente != "0")
                {
                    sql += " and c.idFunc IN (" + idsFuncionarioAssociadoCliente + ")";
                    var criterioFunc = String.Empty;

                    foreach (var id in idsFuncionarioAssociadoCliente.Split(','))
                        criterioFunc += FuncionarioDAO.Instance.GetNome(session, Glass.Conversoes.StrParaUint(id)) + "    ";

                    criterio += "Funcionário Assoc. Cli.: " + criterioFunc;
                }

                if (idProduto > 0)
                {
                    sql += " and pt.idProd=" + idProduto;
                    criterio += "Produto: " + ProdutoDAO.Instance.GetElement(session, idProduto).Descricao + "    ";
                }

                if (idTipoPerda > 0)
                {
                    sql += " AND td.idTipoPerda = " + idTipoPerda;
                    criterio += "Tipo Perda: " + TipoPerdaDAO.Instance.GetNome(idTipoPerda) + "  ";
                }

                if (idSetor > 0)
                {
                    sql += " AND td.idSetor = " + idSetor;
                    criterio += "Setor: " + SetorDAO.Instance.ObtemNomeSetor((uint)idSetor) + "  ";
                }
            }

            return sql.Replace("$$$", criterio);
        }

        private GDAParameter[] GetParams(string nomeCliente, string dataIni, string dataFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeCliente))
                lst.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            return lst.ToArray();
        }

        public IList<ProdutoTrocado> GetList(uint idTrocaDevolucao, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idTrocaDevolucao) == 0 && !TrocaDevolucaoDAO.Instance.TemPedido(idTrocaDevolucao))
                return new ProdutoTrocado[] { new ProdutoTrocado() };

            return LoadDataWithSortExpression(Sql(idTrocaDevolucao, 0, 0, 0, 0, null, null, null, 0, null, null, true, false, 0, 0, 0),
                sortExpression, startRow, pageSize);
        }

        public int GetCount(uint idTrocaDevolucao)
        {
            int retorno = GetCountReal(idTrocaDevolucao);
            return retorno > 0 || TrocaDevolucaoDAO.Instance.TemPedido(idTrocaDevolucao) ? retorno : 1;
        }

        public int GetCountReal(uint idTrocaDevolucao)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idTrocaDevolucao, 0, 0, 0, 0, null, null, null, 0, null, null, false, false, 0, 0, 0));
        }

        public IList<ProdutoTrocado> GetByTrocaDevolucao(uint idTrocaDevolucao)
        {
            return GetByTrocaDevolucao(null, idTrocaDevolucao);
        }

        public IList<ProdutoTrocado> GetByTrocaDevolucao(GDASession session, uint idTrocaDevolucao)
        {
            return objPersistence.LoadData(session, Sql(session, idTrocaDevolucao, 0, 0, 0, 0, null, null, null, 0, null, null, true, false, 0, 0, 0)).ToList();
        }

        public IList<ProdutoTrocado> GetForRptPerdasExternas(uint idTrocaDevolucao, uint idPedido, int tipo, int situacao, uint idCliente,
            string nomeCliente, string idsFuncionario, string idsFuncionarioAssociadoCliente, string dataIni, string dataFim, uint produto,
            int idOrigemTrocaDevolucao, uint idTipoPerda, int idSetor)
        {
            var retorno = objPersistence.LoadData(Sql(idTrocaDevolucao, idPedido, tipo, situacao, idCliente, nomeCliente,
                idsFuncionario, idsFuncionarioAssociadoCliente, produto, dataIni, dataFim, true, true, idOrigemTrocaDevolucao, idTipoPerda, idSetor),
                GetParams(nomeCliente, dataIni, dataFim)).ToList();

         return retorno;
        }

        #region Obtem dados do produto trocado

        /// <summary>
        /// Obtem a quantidade disponivel para troca
        /// </summary>
        public decimal ObtemQuantidadeDisponivelParaTroca(GDASession session, uint idProdPed)
        {
            var sql = @"
                SELECT CAST(pp.qtde - COALESCE((SELECT SUM(qtde)
                    FROM produto_trocado pt
                        INNER JOIN troca_devolucao td ON (pt.idTrocaDevolucao = td.idTrocaDevolucao)
                    WHERE pt.idProdPed = pp.idProdPed AND td.situacao <> " + (int)TrocaDevolucao.SituacaoTrocaDev.Cancelada + @"
                    ), 0) AS DECIMAL(12, 2))
                FROM produtos_pedido pp
                WHERE pp.idProdPed = " + idProdPed;

            return ExecuteScalar<decimal>(session, sql);
        }

        public decimal ObterCustoProd(GDASession session, int idProdTrocado)
        {
            return ObtemValorCampo<decimal>(session, "CustoProd", $"IdProdTrocado={ idProdTrocado }");
        }

        #endregion

        #region Verifica se o produto foi trocado ou devolvido

        /// <summary>
        /// Retorna a quantidade do produto trocado ou devolvido
        /// </summary>
        public int ObtemQtdTrocadoDevolvidoFinalizado(GDASession session, uint idProdPed)
        {
            string sql = string.Format(@"SELECT COUNT(*) FROM produto_trocado pt INNER JOIN troca_devolucao td ON (pt.Idtrocadevolucao = td.Idtrocadevolucao)
                                         WHERE pt.IdProdPed= {0} AND td.situacao= {1}", idProdPed, (int)TrocaDevolucao.SituacaoTrocaDev.Finalizada);

            return objPersistence.ExecuteSqlQueryCount(session, sql);
        }

        public float ObtemQtdTrocadoDevolvido(GDASession session, uint idProdPed)
        {
            string sql = "Select sum(qtde) From produto_trocado Where idProdPed=" + idProdPed;

            return Glass.Conversoes.StrParaFloat(objPersistence.ExecuteScalar(session, sql).ToString());
        }

        #endregion

        #region Atualiza o valor do beneficiamento do produto

        /// <summary>
        /// Atualiza o valor do beneficiamento do produto.
        /// </summary>
        public void UpdateValorBenef(uint idProdTrocado)
        {
            UpdateValorBenef(null, idProdTrocado);
        }

        /// <summary>
        /// Atualiza o valor do beneficiamento do produto.
        /// </summary>
        public void UpdateValorBenef(GDASession session, uint idProdTrocado)
        {
            var idProd = ObtemValorCampo<int>(session, "idProd", "idProdTrocado=" + idProdTrocado);

            if (!Glass.Configuracoes.Geral.NaoVendeVidro() && !ProdutoDAO.Instance.CalculaBeneficiamento(session, idProd))
                objPersistence.ExecuteCommand(session, "update produto_trocado pt set valorBenef=(select sum(coalesce(valor,0)) from " +
                    "produto_trocado_benef where idProdTrocado=pt.idProdTrocado) where idProdTrocado=" + idProdTrocado);

            // Recalcula o total bruto/valor unitário bruto
            ProdutoTrocado pt = GetElementByPrimaryKey(session, idProdTrocado);
            UpdateBase(session, pt);
        }

        #endregion

        #region Métodos sobrescritos

        public uint InsertFromPedidoComTransacao(int idTrocaDev, int idProdPed, decimal qtde, string etiquetas)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = InsertFromPedido(transaction, idTrocaDev, idProdPed, qtde, etiquetas);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public uint InsertFromPedido(GDASession session, int idTrocaDev, int idProdPed, decimal qtde, string etiquetas,
            bool? alterarEstoque = null, bool comDefeito = false)
        {
            ProdutosPedido prodPed = ProdutosPedidoDAO.Instance.GetElementByPrimaryKey(session, idProdPed);

            Pedido ped = PedidoDAO.Instance.GetElementByPrimaryKey(session, prodPed.IdPedido);

            var qtdeOriginal = prodPed.Qtde;
            List<ProdutoTrocadoBenef> lstProdTrocBenef = new List<ProdutoTrocadoBenef>();

            var qtdProdutoTrocado = ObtemQtdTrocadoDevolvidoFinalizado(session, prodPed.IdProdPed);

            if (prodPed.Qtde != prodPed.QtdSaida + qtdProdutoTrocado)
                throw new Exception(@"Não é possível inserir os produtos selecionados na troca\Devolução, pois os mesmos não foram entregues totalmente.");

            //Verifica se tem quantidade suficiente para troca
            var qtdeDisponivelTroca = ObtemQuantidadeDisponivelParaTroca(session, (uint)idProdPed);
            if (qtde > qtdeDisponivelTroca)
                throw new Exception("Não a quantidade disponível para troca");

            //Valida as etiquetas informadas
            if (!string.IsNullOrWhiteSpace(etiquetas))
            {
                var lstEtq = etiquetas.Trim().Split(',').Select(f => f.Trim());

                if (lstEtq.Count() > qtde)
                    throw new Exception("A quantidade de etiquetas informadas é maior que a quantidade selecionada. Insira o produto novamente.");

                foreach (var etq in lstEtq)
                {
                    var contadorEtiquetasChapa = 0;
                    var contadorEtiquetasBox = 0;

                    if (etq.ToUpper().Substring(0, 1).Equals("N"))
                    {
                        if (contadorEtiquetasBox > 0)
                            throw new Exception("Não é possível inserir etiqueta de box junto com etiquetas de chapas");

                        var idProd = ProdutosNfDAO.Instance.GetIdProdByEtiqueta(session, etq);
                        var e = etq.Split(';')[0];
                        var idPedExp = ProdutoImpressaoDAO.Instance.ObterIdPedidoExpedicaoPelaEtiqueta(session, etq);
                        var chapaDeuSaida = ChapaCortePecaDAO.Instance.ChapaDeuSaidaEmPedidoRevenda(etq);

                        if (!chapaDeuSaida)
                            throw new Exception("A etiqueta " + e + " ainda não foi entregue. Insira o produto novamente");

                        if (idPedExp.GetValueOrDefault(0) != prodPed.IdPedido)
                            throw new Exception("A etiqueta " + e + " não esta vinculada ao pedido da troca/devolução. Insira o produto novamente");

                        if (idProd != prodPed.IdProd)
                            throw new Exception("O produto da etiqueta " + e + " é diferente do produto selecionado. Insira o produto novamente");

                        contadorEtiquetasChapa++;
                    }
                    else
                    {
                        if (contadorEtiquetasChapa > 0)
                            throw new Exception("Não é possível inserir etiqueta de box junto com etiquetas de chapas");

                        var idProdPedEtq = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(session, etq);
                        var idProdEtq = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd(session, idProdPedEtq);
                        var idPedExp = ProdutoPedidoProducaoDAO.Instance.ObtemIdPedidoExpedicao(etq);
                        var situacaoEtq = ProdutoPedidoProducaoDAO.Instance.ObtemSituacaoProducao(session, etq);

                        if (situacaoEtq != SituacaoProdutoProducao.Entregue)
                            throw new Exception("A etiqueta " + etq + " ainda não foi entregue. Insira o produto novamente");

                        if (idPedExp.GetValueOrDefault(0) != prodPed.IdPedido)
                            throw new Exception("A etiqueta " + etq + " não esta vinculada ao pedido da troca/devolução. Insira o produto novamente");

                        if (idProdEtq != prodPed.IdProd)
                            throw new Exception("O produto da etiqueta " + etq + " é diferente do produto selecionado. Insira o produto novamente");

                        contadorEtiquetasBox++;
                    }
                }
            }

            // Se a quantidade disponível para ser trocada for diferente da qtd do produto recalcula o total e os beneficiamentos
            if (prodPed.Qtde != (float)qtde)
            {
                // Recalcula o metro quadrado
                prodPed.TotM = (prodPed.TotM / prodPed.Qtde) * (float)qtde;
                prodPed.TotM2Calc = prodPed.TotM2Calc > 0 ? (prodPed.TotM2Calc / prodPed.Qtde) * (float)qtde : prodPed.TotM;

                prodPed.Qtde = (float)qtde;
                int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session, (int)prodPed.IdProd, false);

                if (tipoCalc == (uint)TipoCalculoGrupoProd.Qtd || tipoCalc == (uint)TipoCalculoGrupoProd.QtdM2 || tipoCalc == (uint)TipoCalculoGrupoProd.QtdDecimal)
                    prodPed.Total = (decimal)prodPed.Qtde * prodPed.ValorVendido;
                /* Chamado 62311. */
                else if (prodPed.TotM2Calc > 0)
                    prodPed.Total = (decimal)prodPed.TotM2Calc * prodPed.ValorVendido;
                else if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                    prodPed.Total = prodPed.ValorVendido * (decimal)(prodPed.Altura * prodPed.Qtde);
                else if (prodPed.Altura > 0)
                    prodPed.Total = (prodPed.ValorVendido * (decimal)(prodPed.Altura * prodPed.Qtde)) / 6;
                else
                    prodPed.Total = prodPed.ValorVendido * (decimal)prodPed.Qtde;

                // Recalcula o valor dos beneficiamentos para considerar apenas a quantidade a ser trocada escolhido
                foreach (ProdutoTrocadoBenef ptb in prodPed.Beneficiamentos.ToProdutosTrocado())
                {
                    ptb.Valor = (ptb.Valor / (decimal)qtdeOriginal) * (decimal)qtde;
                    lstProdTrocBenef.Add(ptb);
                }
            }
            else
                // Se a quantidade a ser trocada for a mesma quantidade do produto original, insere todos os beneficiamentos sem recalcular
                lstProdTrocBenef = prodPed.Beneficiamentos;

            ProdutoTrocado novo = new ProdutoTrocado();
            novo.IdTrocaDevolucao = (uint)idTrocaDev;
            novo.IdProdPed = (uint)idProdPed;
            novo.IdProd = prodPed.IdProd;
            novo.Qtde = (float)qtde;
            novo.Altura = prodPed.Altura;
            novo.AlturaReal = prodPed.AlturaReal;
            novo.Beneficiamentos = lstProdTrocBenef;
            novo.CustoProd = prodPed.CustoProd;
            novo.Espessura = prodPed.Espessura;
            novo.IdAplicacao = prodPed.IdAplicacao;
            novo.IdProcesso = prodPed.IdProcesso;
            novo.Largura = prodPed.Largura;
            novo.PedCli = prodPed.PedCli;
            novo.PercDescontoQtde = prodPed.PercDescontoQtde;
            novo.Redondo = prodPed.Redondo;
            novo.Total = prodPed.Total;
            novo.TotM = prodPed.TotM;
            novo.TotM2Calc = prodPed.TotM2Calc;
            novo.ValorAcrescimo = prodPed.ValorAcrescimo;
            novo.ValorAcrescimoProd = prodPed.ValorAcrescimoProd;
            novo.ValorDesconto = prodPed.ValorDesconto;
            novo.ValorDescontoProd = prodPed.ValorDescontoProd;
            novo.ValorVendido = prodPed.ValorVendido;
            novo.AlterarEstoque = alterarEstoque.HasValue ? alterarEstoque.Value : true;
            novo.ComDefeito = comDefeito;
            novo.ValorDescontoCliente = prodPed.ValorDescontoCliente;
            novo.ValorAcrescimoCliente = prodPed.ValorAcrescimoCliente;
            novo.ValorUnitarioBruto = prodPed.ValorUnitarioBruto;
            novo.ValorTabelaPedido = prodPed.ValorTabelaPedido;
            novo.TotalBruto = prodPed.TotalBruto;
            novo.ValorBenef = Math.Round((prodPed.ValorBenef / (decimal)qtdeOriginal) * qtde, 2);
            novo.ValorDescontoQtde = prodPed.ValorDescontoQtde;

            DescontoAcrescimo.Instance.RemoverDescontoQtde(session, ped, novo);
            
            if (!string.IsNullOrEmpty(etiquetas))
            {
                var lstEtq = etiquetas.Trim().Split(',').Select(f => f.Trim());

                var etqs = new List<string>();

                foreach (var e in lstEtq)
                {
                    if (e.ToUpper().Substring(0, 1).Equals("N"))
                    {
                        etqs.Add(e);
                    }
                    else
                    {
                        var idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(session, e);
                        var leitura = LeituraProducaoDAO.Instance.ObtemUltimaLeitura(session, idProdPedProducao.GetValueOrDefault(0));
                        var idItemCarregamento = ItemCarregamentoDAO.Instance.ObtemIdItemCarregamento(session, idProdPedProducao.GetValueOrDefault(0));

                        if (leitura != null)
                        {
                            etqs.Add(e + ";" + leitura.IdProdPedProducao + ";" + leitura.IdSetor +
                                ";" + leitura.IdFuncLeitura + ";" + leitura.DataLeitura + ";" + leitura.IdCavalete + ";" + idItemCarregamento);
                        }
                    }
                }

                novo.Etiquetas = string.Join("|", etqs);
            }

            novo.IdPedido = TrocaDevolucaoDAO.Instance.ObtemValorCampo<uint?>(session, "idPedido",
                "idTrocaDevolucao=" + novo.IdTrocaDevolucao);

            if (!PedidoConfig.RatearDescontoProdutos && novo.IdPedido > 0)
            {
                float percDesc = PedidoDAO.Instance.GetPercDesc(session, novo.IdPedido.Value);

                if (percDesc > 0)
                {
                    int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session, (int)novo.IdProd, false);

                    novo.Total -= (novo.Total + novo.ValorBenef) * (decimal)percDesc;

                    if (tipoCalc == (uint)TipoCalculoGrupoProd.Qtd || tipoCalc == (uint)TipoCalculoGrupoProd.QtdM2 || tipoCalc == (uint)TipoCalculoGrupoProd.QtdDecimal)
                        novo.ValorVendido = novo.Total / (decimal)novo.Qtde;
                    else if (novo.TotM2Calc > 0)
                        novo.ValorVendido = novo.Total / (decimal)novo.TotM2Calc;
                    else if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                        novo.ValorVendido = novo.Total / (decimal)(novo.Altura * novo.Qtde);
                    else if (novo.Altura > 0)
                        novo.ValorVendido = (novo.Total * 6) / (decimal)(novo.Altura * novo.Qtde);
                    else
                        novo.ValorVendido = novo.Total / (decimal)novo.Qtde;
                }
            }

            DescontoAcrescimo.Instance.AplicarDescontoQtde(session, ped, novo);

            // Soma o ICMS e IPI ao produto da troca, caso o pedido tenha cobrado
            if (ped.ValorIcms > 0 || ped.ValorIpi > 0)
            {
                if (ped.ValorIcms > 0)
                    novo.Total += prodPed.ValorIcms / (decimal)prodPed.Qtde * (decimal)novo.Qtde;

                if (ped.ValorIpi > 0)
                    novo.Total += prodPed.ValorIpi / (decimal)prodPed.Qtde * (decimal)novo.Qtde;

                decimal? valorUnitario = ValorUnitario.Instance.CalcularValor(session, ped, novo, novo.Total);
                if (valorUnitario.HasValue)
                {
                    novo.ValorVendido = valorUnitario.Value;
                }
            }

            uint retorno = base.Insert(session, novo);

            ProdutoTrocadoBenefDAO.Instance.DeleteByProdutoTrocado(session, retorno);
            foreach (ProdutoTrocadoBenef p in novo.Beneficiamentos.ToProdutosTrocado(retorno))
            {
                ProdutoTrocadoBenefDAO.Instance.Insert(session, p);
            }

            UpdateValorBenef(session, retorno);

            TrocaDevolucaoDAO.Instance.UpdateTotaisTrocaDev(session, novo.IdTrocaDevolucao);

            decimal valor = ObtemValorCampo<decimal>(session, "total+coalesce(valorBenef,0)",
                "idProdTrocado=" + retorno);

            objPersistence.ExecuteCommand(session,
                "update troca_devolucao set creditoGerado=creditoGerado+?valor where creditoGeradoMax>0 and idTrocaDevolucao=" +
                novo.IdTrocaDevolucao, new GDAParameter("?valor", valor));

            TrocaDevolucaoDAO.Instance.UpdateTotaisTrocaDev(session, novo.IdTrocaDevolucao);

            return retorno;
        }

        public uint InsertComTransacao(ProdutoTrocado objInsert)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Insert(transaction, objInsert);
                    
                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public override uint Insert(ProdutoTrocado objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession session, ProdutoTrocado objInsert)
        {
            if (objInsert.IdProd > 0)
            {
                uint idCliente = TrocaDevolucaoDAO.Instance.ObtemIdCliente(session, objInsert.IdTrocaDevolucao);
                decimal custo = objInsert.CustoProd, total = objInsert.Total;
                float altura = objInsert.Altura, totM2 = objInsert.TotM, totM2Calc = objInsert.TotM2Calc;
                int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session, (int)objInsert.IdProd, false);

                objInsert.IdPedido = TrocaDevolucaoDAO.Instance.ObtemValorCampo<uint?>(session, "idPedido",
                    "idTrocaDevolucao=" + objInsert.IdTrocaDevolucao);

                if (objInsert.IdPedido > 0 && !PedidoConfig.RatearDescontoProdutos)
                {
                    float percDesc = PedidoDAO.Instance.GetPercDesc(session, objInsert.IdPedido.Value);

                    if (percDesc > 0)
                    {
                        objInsert.Total -= (objInsert.Total + objInsert.ValorBenef) * (decimal)percDesc;

                        if (tipoCalc == (uint)TipoCalculoGrupoProd.Qtd || tipoCalc == (uint)TipoCalculoGrupoProd.QtdM2 || tipoCalc == (uint)TipoCalculoGrupoProd.QtdDecimal)
                            objInsert.ValorVendido = objInsert.Total / (decimal)objInsert.Qtde;
                        else if (objInsert.TotM > 0)
                            objInsert.ValorVendido = objInsert.Total / (decimal)objInsert.TotM;
                        else if (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                            objInsert.ValorVendido = objInsert.Total / (decimal)(objInsert.Altura * objInsert.Qtde);
                        else if (objInsert.Altura > 0)
                            objInsert.ValorVendido = (objInsert.Total * 6) / (decimal)(objInsert.Altura * objInsert.Qtde);
                        else
                            objInsert.ValorVendido = objInsert.Total / (decimal)objInsert.Qtde;
                    }
                }

                var isPedidoProducaoCorte = PedidoDAO.Instance.IsPedidoProducaoCorte(session, (uint)objInsert.IdPedido);

                Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(session, idCliente, (int)objInsert.IdProd, objInsert.Largura, objInsert.Qtde, 1, 
                    objInsert.ValorVendido, objInsert.Espessura, objInsert.Redondo, 2,
                    false, tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 && !isPedidoProducaoCorte, 
                    ref custo, ref altura, ref totM2, ref totM2Calc, ref total, false, objInsert.Beneficiamentos.CountAreaMinimaSession(session));

                objInsert.CustoProd = custo;
                objInsert.Altura = altura;
                objInsert.TotM = totM2;
                objInsert.TotM2Calc = totM2Calc;
                objInsert.Total = total;
            }

            CalculaDescontoEValorBrutoProduto(session, objInsert);

            uint retorno = base.Insert(session, objInsert);

            ProdutoTrocadoBenefDAO.Instance.DeleteByProdutoTrocado(session, retorno);
            foreach (ProdutoTrocadoBenef p in objInsert.Beneficiamentos.ToProdutosTrocado(retorno))
                ProdutoTrocadoBenefDAO.Instance.Insert(session, p);

            UpdateValorBenef(session, retorno);

            TrocaDevolucaoDAO.Instance.UpdateTotaisTrocaDev(session, objInsert.IdTrocaDevolucao);

            decimal valor = ObtemValorCampo<decimal>(session, "total+coalesce(valorBenef,0)", "idProdTrocado=" + retorno);

            objPersistence.ExecuteCommand(session, "update troca_devolucao set creditoGerado=creditoGerado+?valor where creditoGeradoMax>0 and idTrocaDevolucao=" +
                objInsert.IdTrocaDevolucao, new GDAParameter("?valor", valor));

            TrocaDevolucaoDAO.Instance.UpdateTotaisTrocaDev(session, objInsert.IdTrocaDevolucao);

            return retorno;
        }

        internal int UpdateBase(GDASession session, ProdutoTrocado objUpdate)
        {
            if (objUpdate.ValorTabelaPedido == 0)
                objUpdate.ValorTabelaPedido = ObtemValorCampo<decimal>("ValorTabelaPedido", "IdProdTrocado=" + objUpdate.IdProdTrocado);

            CalculaDescontoEValorBrutoProduto(session, objUpdate);

            return base.Update(session, objUpdate);
        }
        
        /// <summary>
        /// Este método é utilizado no lugar do Update na tela de troca/devolução para evitar que
        /// ao atualizar o produto trocado os valores fiquem incorretos.
        /// </summary>
        public int ExcluirInserir(ProdutoTrocado objUpdate)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    DeleteByPrimaryKey(transaction, objUpdate.IdProdTrocado);
                    var retorno = (int)InsertFromPedido(transaction, (int)objUpdate.IdTrocaDevolucao, (int)objUpdate.IdProdPed.GetValueOrDefault(), (decimal)objUpdate.Qtde,
                       !string.IsNullOrWhiteSpace(objUpdate.Etiquetas) ? string.Join(",", objUpdate.Etiquetas.Split('|').Select(f => f.Split(';')[0]).ToList()) : "", objUpdate.AlterarEstoque, objUpdate.ComDefeito);
                    
                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public override int Delete(ProdutoTrocado objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdProdTrocado);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return DeleteByPrimaryKey(null, Key);
        }

        public override int DeleteByPrimaryKey(GDASession session, uint Key)
        {
            uint idTrocaDevolucao = ObtemValorCampo<uint>(session, "idTrocaDevolucao", "idProdTrocado=" + Key);
            base.DeleteByPrimaryKey(session, Key);
            TrocaDevolucaoDAO.Instance.UpdateTotaisTrocaDev(session, idTrocaDevolucao);
            return 1;
        }

        private void CalculaDescontoEValorBrutoProduto(GDASession sessao, ProdutoTrocado produto)
        {
            var pedido = produto.IdPedido > 0
                ? PedidoDAO.Instance.GetElementByPrimaryKey(sessao, produto.IdPedido.Value)
                : null;

            DescontoAcrescimo.Instance.RemoverDescontoQtde(sessao, pedido, produto);
            DescontoAcrescimo.Instance.AplicarDescontoQtde(sessao, pedido, produto);
            DiferencaCliente.Instance.Calcular(sessao, pedido, produto);
            ValorBruto.Instance.Calcular(sessao, pedido, produto);
        }

        #endregion
    }
}
