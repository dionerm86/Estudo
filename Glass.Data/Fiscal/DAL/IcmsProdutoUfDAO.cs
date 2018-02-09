using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.Model;
using Glass.Data.RelModel;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class IcmsProdutoUfDAO : BaseDAO<IcmsProdutoUf, IcmsProdutoUfDAO>
    {
        //private IcmsProdutoUfDAO() { }

        #region Busca padrão

        private string Sql(uint idProd, string ufOrigem, string ufDestino, uint? idTipoCliente, bool selecionar, out string filtroAdicional)
        {
            StringBuilder sql = new StringBuilder("select ");

            sql.Append(selecionar ? "ipu.*" : "count(*)");

            sql.AppendFormat(@"
                from icms_produto_uf ipu
                where 1 {0}", FILTRO_ADICIONAL);

            StringBuilder fa = new StringBuilder();

            if (idProd > 0)
                fa.AppendFormat(" and ipu.idProd={0}", idProd);

            if (!String.IsNullOrEmpty(ufOrigem))
                fa.Append(" and ipu.ufOrigem=?ufOrigem");

            if (!String.IsNullOrEmpty(ufDestino))
                fa.Append(" and ipu.ufDestino=?ufDestino");

            if (idTipoCliente > 0)
            {
                fa.AppendFormat(" and coalesce(ipu.idTipoCliente, {0})={0}", idTipoCliente.Value);
                sql = new StringBuilder("select * from (" + sql.ToString() + @" order by ipu.idTipoCliente desc
                    /* Chamado 39660.
                     * Esse limit faz com que seja buscada a exceção que contém IdTipoCliente preenchido. */ LIMIT 1) as temp
                    group by idProd, ufOrigem, ufDestino");
            }

            filtroAdicional = fa.ToString();
            return sql.ToString();
        }

        private GDAParameter[] ObtemParametros(string ufOrigem, string ufDestino)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(ufOrigem))
                lst.Add(new GDAParameter("?ufOrigem", ufOrigem));

            if (!string.IsNullOrEmpty(ufDestino))
                lst.Add(new GDAParameter("?ufDestino", ufDestino));

            return lst.ToArray();
        }

        public IList<IcmsProdutoUf> ObtemLista(uint idProd, string ufOrigem, string ufDestino,
            string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "ipu.ufOrigem asc, ipu.ufDestino asc";

            string filtro;
            return LoadDataWithSortExpression(Sql(idProd, ufOrigem, ufDestino, null, true, out filtro), sortExpression,
                startRow, pageSize, false, filtro, ObtemParametros(ufOrigem, ufDestino));
        }

        public int ObtemNumeroRegistros(uint idProd, string ufOrigem, string ufDestino)
        {
            string filtro;
            return GetCountWithInfoPaging(Sql(idProd, ufOrigem, ufDestino, null, true, out filtro),
                false, filtro, ObtemParametros(ufOrigem, ufDestino));
        }

        #endregion

        #region Busca por produto

        /// <summary>
        /// Busca por produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public IList<IcmsProdutoUf> ObtemPorProduto(uint idProd)
        {
            string filtro;
            return objPersistence.LoadData(Sql(idProd, null, null, null, true, out filtro).Replace(FILTRO_ADICIONAL, filtro)).ToList();
        }

        /// <summary>
        /// Busca por produto e UF.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="ufOrigem"></param>
        /// <param name="ufDestino"></param>
        /// <returns></returns>
        public IcmsProdutoUf ObtemPorProduto(GDASession sessao, uint idProd, string ufOrigem, string ufDestino, uint? idTipoCliente)
        {
            if (String.IsNullOrEmpty(ufOrigem))
                throw new ArgumentNullException("ufOrigem");

            if (String.IsNullOrEmpty(ufDestino))
                throw new ArgumentNullException("ufDestino");

            string filtro;
            var itens = objPersistence.LoadData(sessao, Sql(idProd, ufOrigem, ufDestino, idTipoCliente, true,
                out filtro).Replace(FILTRO_ADICIONAL, filtro), ObtemParametros(ufOrigem, ufDestino)).ToList();

            return itens.Count > 0 ? itens[0] : null;
        }

        #endregion

        #region Busca para controle

        /// <summary>
        /// Busca para controle.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public IList<ControleIcmsProdutoPorUf> ObtemParaControle(uint idProd)
        {
            var itens = ObtemPorProduto(idProd);

            // Agrupa os itens para não buscar valores repetidos.
            var agrupados = (from i in itens
                             group i by new
                             {
                                 i.AliquotaIntraestadual,
                                 i.AliquotaInterestadual,
                                 i.AliquotaInternaDestinatario,
                                 i.IdTipoCliente,
                                 i.AliquotaFCPIntraestadual,
                                 i.AliquotaFCPInterestadual,
                             } into g
                             select new
                             {
                                 g.Key.AliquotaIntraestadual,
                                 g.Key.AliquotaInterestadual,
                                 AliquotaInternaDestinatario = g.Key.AliquotaInternaDestinatario,
                                 g.Key.AliquotaFCPIntraestadual,
                                 g.Key.AliquotaFCPInterestadual,
                                 TipoCliente = g.Key.IdTipoCliente,
                                 g.First().UfDestino,
                                 g.First().UfOrigem,
                                 Numero = g.Count()
                             }).OrderByDescending(x => x.Numero).ToList();

            var retorno = new List<IcmsProdutoUf>();

            // Adiciona a regra sem UFOrigem e UFDestino, que será utilizada quando não houver exceção.
            if (agrupados.Count > 0)
                retorno.Add(new IcmsProdutoUf
                {
                    AliquotaInterestadual = agrupados.Count > 0 ? agrupados[0].AliquotaInterestadual : 0,
                    AliquotaIntraestadual = agrupados.Count > 0 ? agrupados[0].AliquotaIntraestadual : 0,
                    AliquotaInternaDestinatario = agrupados.Count > 0 ? agrupados[0].AliquotaInternaDestinatario : 0,
                    AliquotaFCPInterestadual = agrupados.Count > 0 ? agrupados[0].AliquotaFCPInterestadual : 0,
                    AliquotaFCPIntraestadual = agrupados.Count > 0 ? agrupados[0].AliquotaFCPIntraestadual : 0,
                    IdTipoCliente = agrupados.Count > 0 ? agrupados[0].TipoCliente : 0,
                    UfDestino = null,
                    UfOrigem = null
                });

            // Adiciona as Exceções por UFOrigem e UFDestino
            retorno.AddRange(itens.Where(f => agrupados.Count > 0 && (
                f.AliquotaInterestadual != agrupados[0].AliquotaInterestadual ||
                f.AliquotaIntraestadual != agrupados[0].AliquotaIntraestadual ||
                f.AliquotaInternaDestinatario != agrupados[0].AliquotaInternaDestinatario ||
                f.AliquotaFCPInterestadual != agrupados[0].AliquotaFCPInterestadual ||
                f.AliquotaFCPIntraestadual != agrupados[0].AliquotaFCPIntraestadual)).ToList());

            // Monta o resultado para controle
            return retorno.Select(x => new ControleIcmsProdutoPorUf()
            {
                AliquotaInterestadual = x.AliquotaInterestadual,
                AliquotaIntraestadual = x.AliquotaIntraestadual,
                AliquotaInternaDestinatario = x.AliquotaInternaDestinatario,
                AliquotaFCPInterestadual = x.AliquotaFCPInterestadual,
                AliquotaFCPIntraestadual = x.AliquotaFCPIntraestadual,
                TipoCliente = x.IdTipoCliente,
                UfDestino = x.UfDestino,
                UfOrigem = x.UfOrigem
            }).ToList();
        }

        #endregion

        #region Obter ICMS do produto

        /// <summary>
        /// Obtém os dados para buscar o ICMS.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="idFornec"></param>
        /// <param name="idCliente"></param>
        /// <param name="saida"></param>
        /// <returns></returns>
        internal MvaProdutoUfDAO.DadosBuscar ObterDadosParaBuscar(GDASession sessao, uint idLoja, int? idFornec, uint? idCliente)
        {
            return MvaProdutoUfDAO.Instance.ObterDadosParaBuscar(sessao, idLoja, idFornec, idCliente, idCliente > 0);
        }

        public float ObterIcmsPorProduto(GDASession sessao, uint idProd, uint idLoja, uint? idFornec, uint? idCliente)
        {
            var dados = ObterDadosParaBuscar(sessao, idLoja, (int?)idFornec, idCliente);
            return ObterIcmsPorProduto(sessao, idProd, dados.UfOrigem, dados.UfDestino, dados.TipoCliente);
        }

        /// <summary>
        /// Busca a alíquota ICMS por produto e UF.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="ufOrigem"></param>
        /// <param name="ufDestino"></param>
        /// <param name="idTipoCliente"></param>
        /// <returns></returns>
        public float ObterIcmsPorProduto(GDASession sessao, uint idProd, string ufOrigem, string ufDestino, uint? idTipoCliente)
        {
            var item = ObtemPorProduto(sessao, idProd, ufOrigem, ufDestino, idTipoCliente);
            return item == null ? 0 :
                String.Equals(ufOrigem, ufDestino, StringComparison.CurrentCultureIgnoreCase) ? item.AliquotaIntraestadual : item.AliquotaInterestadual;
        }

        #endregion

        #region Obter FCP do produto

        /// <summary>
        /// Busca a alíquota FCP por produto e UF.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <param name="idFornec"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public float ObterFCPPorProduto(GDASession sessao, uint idProd, uint idLoja, uint? idFornec, uint? idCliente)
        {
            var dados = ObterDadosParaBuscar(sessao, idLoja, (int?)idFornec, idCliente);
            return ObterFCPPorProduto(sessao, idProd, dados.UfOrigem, dados.UfDestino, dados.TipoCliente);
        }

        /// <summary>
        /// Busca a alíquota FCP por produto e UF.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProd"></param>
        /// <param name="ufOrigem"></param>
        /// <param name="ufDestino"></param>
        /// <param name="idTipoCliente"></param>
        /// <returns></returns>
        public float ObterFCPPorProduto(GDASession sessao, uint idProd, string ufOrigem, string ufDestino, uint? idTipoCliente)
        {
            var item = ObtemPorProduto(sessao, idProd, ufOrigem, ufDestino, idTipoCliente);
            return item == null ? 0 :
                string.Equals(ufOrigem, ufDestino, StringComparison.CurrentCultureIgnoreCase) ? item.AliquotaFCPIntraestadual : item.AliquotaFCPInterestadual;
        }

        /// <summary>
        /// Obtém a alíquota de FCP ST que será utilizada
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <param name="idFornec"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public float ObterAliquotaFCPSTPorProduto(GDASession sessao, uint idProd, uint idLoja, uint? idFornec, uint? idCliente)
        {
            //if (produto.AliqFCPST > 0)
            //    return produto.AliqFCPST;

            var dados = ObterDadosParaBuscar(sessao, idLoja, (int?)idFornec, idCliente);

            var item = ObtemPorProduto(sessao, idProd, dados.UfOrigem, dados.UfDestino, dados.TipoCliente);
            return item != null ? item.AliquotaFCPIntraestadual : 0;
        }

        #endregion

        #region Salva os dados de ICMS por UF do controle

        public void DeleteByProd(uint idProd)
        {
            DeleteByProd((GDASession)null, idProd);
        }

        public void DeleteByProd(GDASession session, uint idProd)
        {
            objPersistence.ExecuteCommand(session, "delete from icms_produto_uf where idProd=" + idProd);
        }

        private class ChaveCombinacao
        {
            public string UfOrigem, UfDestino;
            public int? IdTipoCliente;
        }

        public void SalvarDadosControle(uint idProd, IList<RelModel.ControleIcmsProdutoPorUf> dadosControle)
        {
            SalvarDadosControle((GDASession)null, idProd, dadosControle);
        }

        public void SalvarDadosControle(GDASession session, uint idProd, IList<RelModel.ControleIcmsProdutoPorUf> dadosControle)
        {
            // Apaga todas os dados do produto
            DeleteByProd(session, idProd);
            if (dadosControle.Count == 0)
                return;

            var listaUF = CidadeDAO.Instance.GetUf(session).Select(x => x.Key);
            var listaCombinacoes = new List<ChaveCombinacao>();

            const string FORMATO_VALOR = "({0}, '{1}', '{2}', {3}, {4}, {5}, {6}, {7}, {8}), ";
            StringBuilder insert = new StringBuilder();

            // Cadastra as exceções
            foreach (var dados in dadosControle.Where(x => !String.IsNullOrEmpty(x.UfOrigem)))
            {
                insert.AppendFormat(FORMATO_VALOR,
                    idProd,
                    dados.UfOrigem,
                    dados.UfDestino,
                    dados.AliquotaIntraestadual.ToString().Replace(".", "").Replace(",", "."),
                    dados.AliquotaInterestadual.ToString().Replace(".", "").Replace(",", "."),
                    dados.AliquotaInternaDestinatario.ToString().Replace(".", "").Replace(",", "."),
                    dados.AliquotaFCPIntraestadual.ToString().Replace(".", "").Replace(",", "."),
                    dados.AliquotaFCPInterestadual.ToString().Replace(".", "").Replace(",", "."),
                    dados.TipoCliente != null ? dados.TipoCliente.Value.ToString() : "null");

                listaCombinacoes.Add(new ChaveCombinacao()
                {
                    UfOrigem = dados.UfOrigem,
                    UfDestino = dados.UfDestino,
                    IdTipoCliente = dados.TipoCliente
                });
            }

            var itemGeral = dadosControle.Where(x => String.IsNullOrEmpty(x.UfOrigem)).First();

            // Cadastra os itens gerais
            foreach (var ufOrigem in listaUF)
                foreach (var ufDestino in listaUF)
                {
                    var chave = new ChaveCombinacao() 
                    { 
                        UfOrigem = ufOrigem,
                        UfDestino = ufDestino
                    };

                    if (listaCombinacoes.Count(x => x.UfDestino == ufDestino && x.UfOrigem == ufOrigem && x.IdTipoCliente.GetValueOrDefault() == 0) > 0)
                        continue;

                    insert.AppendFormat(FORMATO_VALOR,
                        idProd,
                        ufOrigem,
                        ufDestino,
                        itemGeral.AliquotaIntraestadual.ToString().Replace(".", "").Replace(",", "."),
                        itemGeral.AliquotaInterestadual.ToString().Replace(".", "").Replace(",", "."),
                        itemGeral.AliquotaInternaDestinatario.ToString().Replace(".", "").Replace(",", "."),
                        itemGeral.AliquotaFCPIntraestadual.ToString().Replace(".", "").Replace(",", "."),
                        itemGeral.AliquotaFCPInterestadual.ToString().Replace(".", "").Replace(",", "."),
                        "null");

                    listaCombinacoes.Add(chave);
                }

            if (!String.IsNullOrEmpty(insert.ToString()))
            {
                // Executa o SQL para inserir os itens (mais rápido que executar um loop com Insert)
                objPersistence.ExecuteCommand(session, @"insert into icms_produto_uf (idProd, ufOrigem, ufDestino, aliquotaIntra, aliquotaInter, aliquotaInternaDestinatario,
                    AliquotaFCPIntraestadual, AliquotaFCPInterestadual, idTipoCliente) values " + insert.ToString().TrimEnd(' ', ','));
            }
        }

        #endregion
    }
}
