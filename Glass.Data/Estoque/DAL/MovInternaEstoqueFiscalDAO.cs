using GDA;
using Glass.Configuracoes;
using Glass.Data.Helper;
using Glass.Data.Model;
using System;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public sealed class MovInternaEstoqueFiscalDAO : BaseDAO<MovInternaEstoqueFiscal, MovInternaEstoqueFiscalDAO>
    {
        public void SalvarMovimentacao(string codProdOrigem, string codProdDestino, decimal qtdeOrigem, decimal qtdeDestino, int idLoja)
        {
            using (var session = new GDA.GDATransaction())
            {
                try
                {
                    session.BeginTransaction();

                    if (qtdeOrigem <= 0)
                        throw new Exception("A qtde. de origem não pode ser menor ou igual a 0");

                    if (qtdeDestino <= 0)
                        throw new Exception("A qtde. de destino não pode ser menor ou igual a 0");

                    if (!LojaDAO.Instance.Exists(session, idLoja))
                        throw new Exception("A loja informada não existe.");

                    var idProdOrigem = ProdutoDAO.Instance.ObtemIdProd(session, codProdOrigem);
                    var idProdDestino = ProdutoDAO.Instance.ObtemIdProd(session, codProdDestino);

                    if (idProdOrigem == 0)
                        throw new Exception("Informe o produto de origem");

                    if (idProdDestino == 0)
                        throw new Exception("Informe o produto de destino");


                    var mov = new MovInternaEstoqueFiscal()
                    {
                        IdProdOrigem = idProdOrigem,
                        IdProdDestino = idProdDestino,
                        QtdeOrigem = qtdeOrigem,
                        QtdeDestino = qtdeDestino,
                        IdLoja = idLoja,
                        Usucad = UserInfo.GetUserInfo.CodUser,
                        DataCad = DateTime.Now
                    };

                    var id = Insert(session, mov);

                    MovEstoqueFiscalDAO.Instance.BaixaEstoqueManual(session, (uint)mov.IdProdOrigem, (uint)mov.IdLoja, mov.QtdeOrigem, null, DateTime.Now, "Movimentação Interna de Estoque Nº" + id);
                    MovEstoqueFiscalDAO.Instance.CreditaEstoqueManual(session, (uint)mov.IdProdDestino, (uint)mov.IdLoja, mov.QtdeDestino, null, DateTime.Now, "Movimentação Interna de Estoque Nº" + id);

                    session.Commit();
                    session.Close();
                }
                catch (Exception ex)
                {
                    session.Rollback();
                    session.Close();

                    ErroDAO.Instance.InserirFromException("Movimentação Interna de Estoque Fiscal", ex);
                    throw ex;
                }
            }
        }

        public List<MovInternaEstoqueFiscal> ObtemParaEFD(int idLoja, DateTime inicio, DateTime fim)
        {
            var sql = @"
                SELECT IdProdOrigem, IdProdDestino, DataCad, SUM(QtdeOrigem) as QtdeOrigem, SUM(QtdeDestino) as QtdeDestino
                FROM mov_interna_estoque_fiscal
                WHERE dataCad >= ?dtIni AND dataCad <= ?dtFim
                    AND idLoja = " + idLoja+@"
                GROUP BY IdProdOrigem, IdProdDestino, DataCad";

            return objPersistence.LoadData(sql, GetParams(inicio, fim));
        }

        public GDAParameter[] GetParams(DateTime? dtIni, DateTime? dtFim)
        {
            var parameters = new List<GDAParameter>();

            if (dtIni.HasValue)
                parameters.Add(new GDAParameter("?dtIni", DateTime.Parse(dtIni.Value.ToShortDateString() + " 00:00:00")));

            if (dtFim.HasValue)
                parameters.Add(new GDAParameter("?dtFim", DateTime.Parse(dtFim.Value.ToShortDateString() + " 23:59:59")));

            return parameters.ToArray();
        }
    }
}
