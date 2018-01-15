using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.Model;
using Glass.Data.RelModel;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class MvaProdutoUfDAO : BaseDAO<MvaProdutoUf, MvaProdutoUfDAO>
    {
        //private MvaProdutoUfDAO() { }

        #region Busca padrão

        private string Sql(int idProd, string ufOrigem, string ufDestino, bool selecionar, out string filtroAdicional)
        {
            StringBuilder sql = new StringBuilder("select ");

            sql.Append(selecionar ? "mpu.*" : "count(*)");

            sql.AppendFormat(@"
                from mva_produto_uf mpu
                where 1 {0}", FILTRO_ADICIONAL);

            StringBuilder fa = new StringBuilder();

            if (idProd > 0)
                fa.AppendFormat(" and mpu.idProd={0}", idProd);

            if (!String.IsNullOrEmpty(ufOrigem))
                fa.Append(" and mpu.ufOrigem=?ufOrigem");

            if (!String.IsNullOrEmpty(ufDestino))
                fa.Append(" and mpu.ufDestino=?ufDestino");

            filtroAdicional = fa.ToString();
            return sql.ToString();
        }

        private GDA.GDAParameter[] ObtemParametros(string ufOrigem, string ufDestino)
        {
            List<GDA.GDAParameter> lst = new List<GDA.GDAParameter>();

            if (!String.IsNullOrEmpty(ufOrigem))
                lst.Add(new GDA.GDAParameter("?ufOrigem", ufOrigem));

            if (!String.IsNullOrEmpty(ufDestino))
                lst.Add(new GDA.GDAParameter("?ufDestino", ufDestino));

            return lst.ToArray();
        }

        public IList<MvaProdutoUf> ObtemLista(uint idProd, string ufOrigem, string ufDestino, 
            string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "mpu.ufOrigem asc, mpu.ufDestino asc";

            string filtro;
            return LoadDataWithSortExpression(Sql((int)idProd, ufOrigem, ufDestino, true, out filtro), sortExpression,
                startRow, pageSize, false, filtro, ObtemParametros(ufOrigem, ufDestino));
        }

        public int ObtemNumeroRegistros(uint idProd, string ufOrigem, string ufDestino)
        {
            string filtro;
            return GetCountWithInfoPaging(Sql((int)idProd, ufOrigem, ufDestino, true, out filtro),
                false, filtro, ObtemParametros(ufOrigem, ufDestino));
        }

        #endregion

        #region Busca por produto

        /// <summary>
        /// Busca por produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public IList<MvaProdutoUf> ObtemPorProduto(int idProd)
        {
            string filtro;
            return objPersistence.LoadData(Sql(idProd, null, null, true, out filtro).Replace(FILTRO_ADICIONAL, filtro)).ToList();
        }

        /// <summary>
        /// Busca por produto e UF.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="ufOrigem"></param>
        /// <param name="ufDestino"></param>
        /// <returns></returns>
        public MvaProdutoUf ObtemPorProduto(GDASession sessao, int idProd, string ufOrigem, string ufDestino)
        {
            if (String.IsNullOrEmpty(ufOrigem))
                throw new ArgumentNullException("ufOrigem");

            if (String.IsNullOrEmpty(ufDestino))
                throw new ArgumentNullException("ufDestino");

            string filtro;
            var itens = objPersistence.LoadData(sessao, Sql(idProd, ufOrigem, ufDestino, true, 
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
        public IList<ControleMvaProdutoPorUf> ObtemParaControle(int idProd)
        {
            var itens = ObtemPorProduto(idProd);

            var agrupados = (from i in itens
                             group i by new
                             {
                                 i.MvaOriginal,
                                 i.MvaSimples
                             } into g
                             select new
                             {
                                 MvaOriginal = g.Key.MvaOriginal,
                                 MvaSimples = g.Key.MvaSimples,
                                 g.First().UfOrigem,
                                 g.First().UfDestino,
                                 Numero = g.Count()
                             }).OrderByDescending(x => x.Numero).ToList();

            var retorno = new List<MvaProdutoUf>();

            if (agrupados.Count > 0)
                retorno.Add(new MvaProdutoUf
                {
                    MvaOriginal = agrupados.Count > 0 ? agrupados[0].MvaOriginal : 0,
                    MvaSimples = agrupados.Count > 0 ? agrupados[0].MvaSimples : 0,
                    UfOrigem = null,
                    UfDestino = null
                });

            retorno.AddRange(itens.Where(f => agrupados.Count > 0 &&
                (f.MvaOriginal != agrupados[0].MvaOriginal ||
                f.MvaSimples != agrupados[0].MvaSimples)).ToList());

            return retorno.Select(x => new ControleMvaProdutoPorUf()
            {
                MvaOriginal = x.MvaOriginal,
                MvaSimples = x.MvaSimples,
                UfDestino = x.UfDestino,
                UfOrigem = x.UfOrigem
            }).ToList();
        }

        #endregion

        #region Obter MVA do produto

        internal struct DadosBuscar
        {
            public string UfOrigem, UfDestino;
            public bool Simples;
            public uint? TipoCliente;
        }

        /// <summary>
        /// Obtém os dados para buscar o MVA.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="idFornec"></param>
        /// <param name="idCliente"></param>
        /// <param name="saida"></param>
        /// <returns></returns>
        internal DadosBuscar ObterDadosParaBuscar(GDASession sessao, uint idLoja, int? idFornec, uint? idCliente, bool saida)
        {
            string ufLoja = CidadeDAO.Instance.GetNomeUf(sessao, LojaDAO.Instance.ObtemValorCampo<uint>(sessao, "idCidade", "idLoja=" + idLoja));
            //bool simplesLoja = LojaDAO.Instance.ObtemValorCampo<int>("crt", "idLoja=" + idLoja) <= 2; // 1 e 2 - simples

            if (idCliente > 0)
            {
                string ufCliente = CidadeDAO.Instance.GetNomeUf(sessao, ClienteDAO.Instance.ObtemIdCidade(sessao, idCliente.Value));
                bool simplesCliente = ClienteDAO.Instance.ObtemValorCampo<int>(sessao, "crt", "id_Cli=" + idCliente) == (int)CrtCliente.SimplesNacional;
                uint? idTipoCliente = ClienteDAO.Instance.ObtemValorCampo<uint?>(sessao, "idTipoCliente", "id_Cli=" + idCliente);

                return new DadosBuscar()
                {
                    UfOrigem = saida ? ufLoja : ufCliente,
                    UfDestino = saida ? ufCliente : ufLoja,
                    Simples = simplesCliente,
                    TipoCliente = idTipoCliente
                };
            }
            
            if (idFornec > 0)
            {
                string ufFornec = CidadeDAO.Instance.GetNomeUf(sessao, FornecedorDAO.Instance.ObtemValorCampo<uint>(sessao, "idCidade", "idFornec=" + idFornec));
                bool simplesFornec = saida ? FornecedorDAO.Instance.ObtemValorCampo<int>(sessao, "crt", "idFornec=" + idFornec) == (int)RegimeFornecedor.SimplesNacional :
                    idLoja > 0 ? LojaDAO.Instance.BuscaCrtLoja(sessao, idLoja) == (int)CrtLoja.SimplesNacional : false;

                return new DadosBuscar()
                {
                    UfOrigem = saida ? ufLoja : ufFornec,
                    UfDestino = saida ? ufFornec : ufLoja,
                    Simples = simplesFornec,
                    TipoCliente = null
                };
            }                

            // Retorna em caso de erro
            return new DadosBuscar()
            {
                UfOrigem = ufLoja,
                UfDestino = ufLoja,
                Simples = false,
                TipoCliente = null
            };
        }

        public float ObterMvaPorProduto(GDASession sessao, int idProd, uint idLoja, int? idFornec, uint? idCliente, bool saida)
        {
            var dados = ObterDadosParaBuscar(sessao, idLoja, idFornec, idCliente, saida);
            return ObterMvaPorProduto(sessao, idProd, idLoja, dados.UfOrigem, dados.UfDestino, dados.Simples, dados.TipoCliente, saida);
        }

        /// <summary>
        /// Busca o valor do MVA por produto e UF.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="ufOrigem"></param>
        /// <param name="ufDestino"></param>
        /// <param name="simples"></param>
        /// <returns></returns>
        public float ObterMvaPorProduto(GDASession sessao, int idProd, uint idLoja, string ufOrigem, string ufDestino, bool simples, uint? idTipoCliente, bool saida)
        {
            var item = ObtemPorProduto(sessao, idProd, ufOrigem, ufDestino);
            float mva = item == null ? 0 : 
                !simples ? item.MvaOriginal : item.MvaSimples;

            #region Calcula a MVA Ajustada

            // Verifica se a loja é optante pelo Simples se for nota de saída
            bool simplesLoja = !saida ? false :
                LojaDAO.Instance.ObtemValorCampo<int>(sessao, "crt", "idLoja=" + idLoja) <= 2; // 1 e 2 - simples

            // Não calcula MVA ajustada se a loja for Simples
            if (!simplesLoja && !String.Equals(ufOrigem, ufDestino, StringComparison.CurrentCultureIgnoreCase) && mva > 0)
            {
                var icms = IcmsProdutoUfDAO.Instance.ObtemPorProduto(sessao, (uint)idProd, ufOrigem, ufDestino, idTipoCliente);
                
                if (icms != null && icms.AliquotaInterestadual != icms.AliquotaIntraestadual)
                {
                    var a = 1 + (decimal)mva / 100;
                    var b = 1 - (decimal)icms.AliquotaInterestadual / 100;
                    var c = 1 - (decimal)icms.AliquotaIntraestadual / 100;

                    mva = (float)((a * b / c) - 1) * 100;
                }
            }

            #endregion

            return mva;
        }

        #endregion

        #region Salva os dados de MVA por UF do controle

        public void DeleteByProd(uint idProd)
        {
            DeleteByProd((GDASession)null, idProd);
        }

        public void DeleteByProd(GDASession session, uint idProd)
        {
            objPersistence.ExecuteCommand(session, "delete from mva_produto_uf where idProd=" + idProd);
        }

        public void SalvarDadosControle(uint idProd, IList<RelModel.ControleMvaProdutoPorUf> dadosControle)
        {
            SalvarDadosControle((GDASession)null, idProd, dadosControle);
        }

        public void SalvarDadosControle(GDASession session, uint idProd, IList<RelModel.ControleMvaProdutoPorUf> dadosControle)
        {
            // Apaga todas os dados do produto
            DeleteByProd(session, idProd);
            if (dadosControle.Count == 0)
                return;

            var listaUF = CidadeDAO.Instance.GetUf(session).Select(x => x.Key);
            var listaCombinacoes = new List<KeyValuePair<string, string>>();

            const string FORMATO_VALOR = "({0}, '{1}', '{2}', {3}, {4}), ";
            StringBuilder insert = new StringBuilder();

            // Cadastra as exceções
            foreach (var dados in dadosControle.Where(x => !String.IsNullOrEmpty(x.UfOrigem)))
            {
                insert.AppendFormat(FORMATO_VALOR,
                    idProd,
                    dados.UfOrigem,
                    dados.UfDestino,
                    dados.MvaOriginal.ToString().Replace(".", "").Replace(",", "."),
                    dados.MvaSimples.ToString().Replace(".", "").Replace(",", "."));

                listaCombinacoes.Add(new KeyValuePair<string, string>(dados.UfOrigem, dados.UfDestino));
            }

            var itemGeral = dadosControle.Where(x => String.IsNullOrEmpty(x.UfOrigem)).First();

            // Cadastra os itens gerais
            foreach (var ufOrigem in listaUF)
                foreach (var ufDestino in listaUF)
                {
                    var chave = new KeyValuePair<string, string>(ufOrigem, ufDestino);

                    if (listaCombinacoes.Count(x => x.Key == ufOrigem && x.Value == ufDestino) > 0)
                        continue;

                    insert.AppendFormat(FORMATO_VALOR,
                        idProd,
                        ufOrigem,
                        ufDestino,
                        itemGeral.MvaOriginal.ToString().Replace(".", "").Replace(",", "."),
                        itemGeral.MvaSimples.ToString().Replace(".", "").Replace(",", "."));

                    listaCombinacoes.Add(chave);
                }

            if (!String.IsNullOrEmpty(insert.ToString()))
            {
                // Executa o SQL para inserir os itens (mais rápido que executar um loop com Insert)
                objPersistence.ExecuteCommand(session, "insert into mva_produto_uf (idProd, ufOrigem, ufDestino, mvaOriginal, mvaSimples) values " +
                    insert.ToString().TrimEnd(' ', ','));
            }
        }

        #endregion
    }
}
