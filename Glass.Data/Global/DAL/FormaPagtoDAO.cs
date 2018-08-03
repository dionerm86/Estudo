using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class FormaPagtoDAO : BaseDAO<FormaPagto, FormaPagtoDAO>
    {
        //private FormaPagtoDAO() { }

        #region SQL

        private string Sql(uint idFormaPagto, uint idCliente, bool apenasUsar, bool selecionar)
        {
            string campos = selecionar ? "*" : "count(*)";
            bool usar = idCliente > 0;
            string sqlUsar = "";

            if (usar)
            {
                sqlUsar = "idFormaPagto " + (selecionar ? "" : "not ") + "in (select idFormaPagto from formapagto_cliente where idCliente=" + idCliente + ")";

                if (selecionar)
                    campos += ", (" + sqlUsar + ") as NaoUsar";
            }

            string sql = "select " + campos + " from formapagto where !apenasSistema";

            if (idFormaPagto > 0)
                sql += " and idFormaPagto=" + idFormaPagto;

            if (apenasUsar && usar)
                sql += selecionar ? " having naoUsar=false" : " and " + sqlUsar;

            if (!FinanceiroConfig.FormaPagamento.SepararTiposChequesRecebimento)
                sql += " and idFormaPagto not in (" + (int)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro + ")";

            sql += " order by descricao";
            return sql;
        }

        #endregion

        #region Formas Pagto. para recebimento de conta

        /// <summary>
        /// Busca formas de pagamento aplicáveis ao recebimento de contas
        /// </summary>
        public FormaPagto[] GetForRecebConta()
        {
            string formasPagto = 
                (uint)Pagto.FormaPagto.Boleto + "," + 
                (uint)Pagto.FormaPagto.Cartao + "," +
                (uint)Pagto.FormaPagto.ChequeProprio + "," + 
                (FinanceiroConfig.FormaPagamento.SepararTiposChequesRecebimento ? (uint)Pagto.FormaPagto.ChequeTerceiro + "," : "") + 
                (uint)Pagto.FormaPagto.Construcard + "," + (uint)Pagto.FormaPagto.Dinheiro + "," + 
                (uint)Pagto.FormaPagto.Deposito;

            if (FinanceiroConfig.FormaPagamento.PermitirFormaPagtoPermutaApenasAdministrador)
            {
                if (UserInfo.GetUserInfo.IsAdministrador)
                    formasPagto += "," + (uint) Pagto.FormaPagto.Permuta;
            }
            else
                formasPagto += "," + (uint) Pagto.FormaPagto.Permuta;

            string sql = "Select * From formapagto where !apenasSistema and IdFormaPagto In (" + formasPagto + ") Order By Descricao";

            List<FormaPagto> lst = objPersistence.LoadData(sql).ToList();
            lst.Insert(0, new FormaPagto());

            if (DepositoNaoIdentificadoDAO.Instance.PossuiNaoUtilizados())
            {
                lst.Add(new FormaPagto()
                {
                    IdFormaPagto = (uint)Pagto.FormaPagto.DepositoNaoIdentificado,
                    Descricao = GetDescricao(Pagto.FormaPagto.DepositoNaoIdentificado)
                });
            }

            lst.Add(new FormaPagto()
            {
                IdFormaPagto = (uint)Pagto.FormaPagto.CartaoNaoIdentificado,
                Descricao = GetDescricao(Pagto.FormaPagto.CartaoNaoIdentificado)
            });
            
            var retorno = lst.OrderBy(f => f.Descricao).ToArray();

            return retorno;
        }

        #endregion

        #region Formas Pagto. para geração cnab

        /// <summary>
        /// Busca formas de pagamento aplicáveis na geração do cnab
        /// </summary>
        public FormaPagto[] GetForCnab()
        {
            string formasPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Cartao + "," +
               (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio + "," + (FinanceiroConfig.FormaPagamento.SepararTiposChequesRecebimento ? (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro + "," : "") +
               (uint)Glass.Data.Model.Pagto.FormaPagto.Construcard + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro + "," +
               (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito;

            if (FinanceiroConfig.FormaPagamento.PermitirFormaPagtoPermutaApenasAdministrador)
            {
                if (UserInfo.GetUserInfo.IsAdministrador)
                    formasPagto += "," + (uint)Pagto.FormaPagto.Permuta;
            }
            else
                formasPagto += "," + (uint)Pagto.FormaPagto.Permuta;

            formasPagto += "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Prazo;

            string sql = "Select * From formapagto where !apenasSistema and IdFormaPagto In (" + formasPagto + ") Order By Descricao";

            List<FormaPagto> lst = objPersistence.LoadData(sql);
            lst.Insert(0, new FormaPagto());

            if (DepositoNaoIdentificadoDAO.Instance.PossuiNaoUtilizados())
            {
                lst.Add(new FormaPagto()
                {
                    IdFormaPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.DepositoNaoIdentificado,
                    Descricao = GetDescricao(Glass.Data.Model.Pagto.FormaPagto.DepositoNaoIdentificado)
                });
            }

            return lst.ToArray();
        }

        #endregion

        #region Formas Pagto. para consulta de contas recebidas

        /// <summary>
        /// Busca formas de pagamento aplicáveis à consulta de contas recebidas
        /// </summary>
        public FormaPagto[] GetForConsultaConta()
        {
            string formasPagto = (int)Glass.Data.Model.Pagto.FormaPagto.Boleto + "," + (int)Glass.Data.Model.Pagto.FormaPagto.Cartao + "," +
                (int)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio + "," + (int)Glass.Data.Model.Pagto.FormaPagto.Construcard + "," +
                (int)Glass.Data.Model.Pagto.FormaPagto.Dinheiro + "," + (int)Glass.Data.Model.Pagto.FormaPagto.Deposito + 
                (FinanceiroConfig.FormaPagamento.SepararTiposChequesRecebimento ? "," + (int)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro : "");

            if (FinanceiroConfig.FormaPagamento.PermitirFormaPagtoPermutaApenasAdministrador)
            {
                if (UserInfo.GetUserInfo.IsAdministrador)
                    formasPagto += "," + (uint)Pagto.FormaPagto.Permuta;
            }
            else
                formasPagto += "," + (uint)Pagto.FormaPagto.Permuta;

            string sql = "Select * From formapagto where (!apenasSistema and IdFormaPagto In (" + formasPagto + ")) OR IdFormaPagto=" + (int)Glass.Data.Model.Pagto.FormaPagto.Credito + " Order By Descricao";

            List<FormaPagto> lst = objPersistence.LoadData(sql);
            FormaPagto formaPagto = new FormaPagto();
            formaPagto.IdFormaPagto = 0;
            formaPagto.Descricao = "Todas";
            lst.Insert(0, formaPagto);

            return lst.ToArray();
        }

        #endregion

        #region Formas Pagto. para consulta de contas a receber

        /// <summary>
        /// Busca formas de pagamento aplicáveis à consulta de contas recebidas
        /// </summary>
        public FormaPagto[] GetForConsultaContasReceber()
        {
            var formasPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Cartao + "," +

                /* Chamado 15759.
                 * Caso a empresa não trabalhe com separação de tipos de cheque em recebimento então os cheques próprio e de terceiros devem
                 * aparecer no filtro para serem selecionados. */
                //(uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio + "," +
                (FinanceiroConfig.FormaPagamento.SepararTiposChequesRecebimento ?
                (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro + "," :
                (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio + ",") +

                (uint)Glass.Data.Model.Pagto.FormaPagto.Construcard + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro + "," +
                (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Prazo;

            if (FinanceiroConfig.FormaPagamento.PermitirFormaPagtoPermutaApenasAdministrador)
            {
                if (UserInfo.GetUserInfo.IsAdministrador)
                    formasPagto += "," + (uint)Pagto.FormaPagto.Permuta;
            }
            else
                formasPagto += "," + (uint)Pagto.FormaPagto.Permuta;

            /* Chamado 15759.
             * Caso a empresa não trabalhe com separação de tipos de cheque em recebimento então os cheques próprio e de terceiros devem
             * aparecer no filtro para serem selecionados. */
            //string sql = "Select *, true as apenasCheque From formapagto where !apenasSistema and IdFormaPagto In (" + formasPagto + ") Order By Descricao";
            var sql = "SELECT *, " +
                (FinanceiroConfig.FormaPagamento.SepararTiposChequesRecebimento ? "FALSE" : "TRUE") +
                " AS ApenasCheque FROM formapagto WHERE !ApenasSistema AND IdFormaPagto In (" + formasPagto + ") ORDER BY Descricao";

            var lst = objPersistence.LoadData(sql);

            return lst.ToArray();
        }

        #endregion

        #region Formas Pagto. para renegociação

        /// <summary>
        /// Busca formas de pagamento aplicáveis à renegociação de conta a receber
        /// </summary>
        public IList<FormaPagto> GetForRenegociacao()
        {
            string formasPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Cartao + "," +
                (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Construcard + "," +
                (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Prazo +
                (FinanceiroConfig.FormaPagamento.SepararTiposChequesRecebimento ? "," + (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro : "");

            string sql = "Select * From formapagto where !apenasSistema and IdFormaPagto In (" + formasPagto + ") Order By Descricao";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Formas Pagto. para quitar cheques devolvidos

        /// <summary>
        /// Busca formas de pagamento aplicáveis ao quitamento de cheques devolvidos
        /// </summary>
        public FormaPagto[] GetForQuitarChequeDev()
        {
            string formasPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Cartao + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio + "," +
                (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Construcard +
                (FinanceiroConfig.FormaPagamento.SepararTiposChequesRecebimento ? "," + (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro : "");

            if (FinanceiroConfig.FormaPagamento.PermitirFormaPagtoPermutaApenasAdministrador)
            {
                if (UserInfo.GetUserInfo.IsAdministrador)
                    formasPagto += "," + (uint)Pagto.FormaPagto.Permuta;
            }
            else
                formasPagto += "," + (uint)Pagto.FormaPagto.Permuta;

            string sql = "Select * From formapagto Where !apenasSistema and IdFormaPagto In (" + formasPagto + ") Order By Descricao";

            List<FormaPagto> lst = objPersistence.LoadData(sql);
            lst.Insert(0, new FormaPagto());

            return lst.ToArray();
        }

        #endregion

        #region Formas Pagto. para quitar cheques próprio devolvidos

        /// <summary>
        /// Busca formas de pagamento aplicáveis ao quitamento de cheques próprio devolvidos
        /// </summary>
        public FormaPagto[] ObtemParaQuitarChequeProprioDevolvido()
        {
            var idsFormaPagto =
                string.Format("{0},{1},{2},{3},{4},{5},{6}",
                    (int)Pagto.FormaPagto.Boleto, (int)Pagto.FormaPagto.Cartao, (int)Pagto.FormaPagto.ChequeProprio,
                    (int)Pagto.FormaPagto.Dinheiro, (int)Pagto.FormaPagto.Deposito, (int)Pagto.FormaPagto.Construcard,
                    (int)Pagto.FormaPagto.ChequeTerceiro);

            if (FinanceiroConfig.FormaPagamento.PermitirFormaPagtoPermutaApenasAdministrador)
            {
                if (UserInfo.GetUserInfo.IsAdministrador)
                    idsFormaPagto += string.Format(",{0}", (uint)Pagto.FormaPagto.Permuta);
            }
            else
                idsFormaPagto += string.Format(",{0}", (uint)Pagto.FormaPagto.Permuta);

            var sql =
                string.Format("SELECT * FROM formapagto WHERE !ApenasSistema AND IdFormaPagto IN ({0}) ORDER BY Descricao", idsFormaPagto);

            var formasPagto = objPersistence.LoadData(sql).ToList();
            formasPagto.Insert(0, new FormaPagto());

            return formasPagto.ToArray();
        }

        #endregion

        #region Formas Pagto. para vendas à vista com opção em branco

        /// <summary>
        /// Retorna as formas de pagto utilizadas em vendas à vista com opção em branco
        /// </summary>
        public FormaPagto[] GetAVista()
        {
            string formasPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Cartao + "," +
                (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Construcard + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro + "," +
                (FinanceiroConfig.FormaPagamento.SepararTiposChequesRecebimento ? "," + (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro : "");

            if (FinanceiroConfig.FormaPagamento.PermitirFormaPagtoPermutaApenasAdministrador)
            {
                if (UserInfo.GetUserInfo.IsAdministrador)
                    formasPagto += "," + (uint)Pagto.FormaPagto.Permuta;
            }
            else
                formasPagto += "," + (uint)Pagto.FormaPagto.Permuta;

            string sql = "Select * From formapagto where !apenasSistema and IdFormaPagto In (" + formasPagto + ") Order By Descricao";

            List<FormaPagto> lst = objPersistence.LoadData(sql);
            lst.Insert(0, new FormaPagto());

            return lst.ToArray();
        }

        #endregion

        #region Formas Pagto. para filtro de vendas à vista

        /// <summary>
        /// Retorna as formas de pagto utilizadas em vendas à vista para serem utilizadas em filtros
        /// </summary>
        public FormaPagto[] GetAVistaForFilter()
        {
            string formasPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Cartao + "," +
                (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Construcard + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro +
                (FinanceiroConfig.FormaPagamento.SepararTiposChequesRecebimento ? "," + (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro : "");

            if (FinanceiroConfig.FormaPagamento.PermitirFormaPagtoPermutaApenasAdministrador)
            {
                if (UserInfo.GetUserInfo.IsAdministrador)
                    formasPagto += "," + (uint)Pagto.FormaPagto.Permuta;
            }
            else
                formasPagto += "," + (uint)Pagto.FormaPagto.Permuta;

            string sql = "Select * From formapagto where !apenasSistema and IdFormaPagto In (" + formasPagto + ") Order By Descricao";

            List<FormaPagto> lst = objPersistence.LoadData(sql);
            FormaPagto formaPagto = new FormaPagto();
            formaPagto.IdFormaPagto = 0;
            formaPagto.Descricao = "Todas";
            lst.Insert(0, formaPagto);

            return lst.ToArray();
        }

        #endregion

        #region Formas Pagto. para pagamento de contas

        /// <summary>
        /// Método para filtro de contas
        /// </summary>
        /// <returns></returns>
        public FormaPagto[] GetForFiltroPagto()
        {
            string formasPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio + "," +
                (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro + "," +
                (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito + "," + (uint)Pagto.FormaPagto.Credito;
            if (FinanceiroConfig.FormaPagamento.PermitirFormaPagtoPermutaApenasAdministrador)
            {
                if (UserInfo.GetUserInfo.IsAdministrador)
                    formasPagto += "," + (uint)Pagto.FormaPagto.Permuta;
            }
            else
                formasPagto += "," + (uint)Pagto.FormaPagto.Permuta;
            string sql = $"Select *, true as utilizarPagamento From formapagto where (!apenasSistema OR IdFormaPagto={ (uint)Pagto.FormaPagto.Credito }) and IdFormaPagto In (" + formasPagto + ") Order By Descricao";
            List<FormaPagto> lst = objPersistence.LoadData(sql).ToList();
            lst.Insert(0, new FormaPagto());
            if (FinanceiroConfig.UsarPgtoAntecipFornec &&
                FornecedorConfig.TipoUsoAntecipacaoFornecedor == DataSources.TipoUsoAntecipacaoFornecedor.ContasPagar &&
                AntecipacaoFornecedorDAO.Instance.PossuiAntecipacoesEmAberto(0))
            {
                lst.Add(new FormaPagto()
                {
                    IdFormaPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.AntecipFornec,
                    Descricao = GetDescricao(Glass.Data.Model.Pagto.FormaPagto.AntecipFornec)
                });
            }
            return lst.ToArray();
        }

        /// <summary>
        /// Formas Pagto. para pagamento de contas.
        /// </summary>
        /// <returns></returns>
        public FormaPagto[] GetForPagto()
        {
            string formasPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio + "," +
                (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro + "," + (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro + "," +
                (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito;

            if (FinanceiroConfig.FormaPagamento.PermitirFormaPagtoPermutaApenasAdministrador)
            {
                if (UserInfo.GetUserInfo.IsAdministrador)
                    formasPagto += "," + (uint)Pagto.FormaPagto.Permuta;
            }
            else
                formasPagto += "," + (uint)Pagto.FormaPagto.Permuta;

            string sql = "Select *, true as utilizarPagamento From formapagto where !apenasSistema and IdFormaPagto In (" + formasPagto + ") Order By Descricao";

            List<FormaPagto> lst = objPersistence.LoadData(sql).ToList();
            lst.Insert(0, new FormaPagto());

            if (FinanceiroConfig.UsarPgtoAntecipFornec &&
                FornecedorConfig.TipoUsoAntecipacaoFornecedor == DataSources.TipoUsoAntecipacaoFornecedor.ContasPagar &&
                AntecipacaoFornecedorDAO.Instance.PossuiAntecipacoesEmAberto(0))
            {
                lst.Add(new FormaPagto()
                {
                    IdFormaPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.AntecipFornec,
                    Descricao = GetDescricao(Glass.Data.Model.Pagto.FormaPagto.AntecipFornec)
                });
            }

            return lst.ToArray();
        }

        #endregion

        /// <summary>
        /// Retorna a descrição de uma forma de pagamento.
        /// </summary>
        /// <param name="formaPagto"></param>
        /// <returns></returns>
        public string GetDescricao(Glass.Data.Model.Pagto.FormaPagto formaPagto)
        {
            return GetDescricao((uint)formaPagto);
        }

        /// <summary>
        /// Retorna a descrição de uma forma de pagamento.
        /// </summary>
        public string GetDescricao(uint idFormaPagto)
        {
            return GetDescricao(null, idFormaPagto);
        }

        /// <summary>
        /// Retorna a descrição de uma forma de pagamento.
        /// </summary>
        public string GetDescricao(GDASession session, uint idFormaPagto)
        {
            return ObtemValorCampo<string>(session, "descricao", "idFormaPagto=" + idFormaPagto);
        }

        public FormaPagto GetElement(uint idFormaPagto)
        {
            return objPersistence.LoadOneData(Sql(idFormaPagto, 0, false, true));
        }

        public FormaPagto GetElement(uint idFormaPagto, uint idCliente)
        {
            return objPersistence.LoadOneData(Sql(idFormaPagto, idCliente, false, true));
        }

        /// <summary>
        /// Retorna as formas de pagto que podem ser utilizadas no pedido
        /// </summary>
        public IList<FormaPagto> GetForPedido()
        {
            return GetForPedido(null, 0, 0);
        }

        /// <summary>
        /// Retorna as formas de pagto que podem ser utilizadas no pedido
        /// </summary>
        public IList<FormaPagto> GetForPedido(int idCliente)
        {
            return GetForPedido(null, idCliente, 0);
        }

        /// <summary>
        /// Retorna as formas de pagto que podem ser utilizadas no pedido
        /// </summary>
        public IList<FormaPagto> GetForPedido(GDASession session, int idCliente, int tipoVenda)
        {
            var sql = string.Format("SELECT fp.* FROM formapagto fp WHERE !fp.ApenasSistema AND fp.IdFormaPagto NOT IN ({0},{1})",
                !FinanceiroConfig.FormaPagamento.SepararTiposChequesRecebimento ? (uint)Pagto.FormaPagto.ChequeTerceiro : 0,

                /* Chamado 65135.
                 * Caso a configuração UsarControleDescontoFormaPagamentoDadosProduto esteja habilitada, o método não deve recuperar a forma de pagamento Prazo para o tipo de venda À Vista
                 * e não deve recuperar a forma de pagamento Dinheiro para o tipo de venda À Prazo.
                 * Com a configuração, citada acima, desabilitada, o método não deve retornar a forma de pagamento Dinheiro, pois a forma de pagamento não será exibida para o tipo de venda À Vista. */
                FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto ? (tipoVenda <= 1 ? (uint)Pagto.FormaPagto.Prazo : (uint)Pagto.FormaPagto.Dinheiro) :
                (uint)Pagto.FormaPagto.Dinheiro);

            if (idCliente > 0)
                sql += string.Format(" AND fp.IdFormaPagto NOT IN (SELECT fpc.IdFormaPagto FROM formapagto_cliente fpc WHERE fpc.IdCliente={0})", idCliente);

            sql += " ORDER BY fp.Descricao";

            return objPersistence.LoadData(session, sql).ToList();
        }

        /// <summary>
        /// Retorna as formas de pagto que podem ser utilizadas no pedido
        /// </summary>
        public IList<FormaPagto> GetForPedidoSel()
        {
            string sql = "Select * From formapagto where !apenasSistema and IdFormaPagto Not In (" +
                (!FinanceiroConfig.FormaPagamento.SepararTiposChequesRecebimento ? (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro : 0) + ")";
            
            sql += " Order By Descricao";

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Obtém as formas de pagamento que podem ser usadas na nota fiscal
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public IList<FormaPagto> GetForNotaFiscal(uint idNf)
        {
            var idFormaPagtoNota = NotaFiscalDAO.Instance.ObtemValorCampo<uint?>("idFormaPagto", "idNf=" + idNf);

            string sql = "Select * From formapagto where (!apenasSistema and IdFormaPagto";

            if (idNf > 0)
            {
                var idCliente = NotaFiscalDAO.Instance.ObtemIdCliente(idNf);
                if (idCliente > 0)
                    sql += " and idFormaPagto not in (select idFormaPagto from formapagto_cliente where idCliente=" + idCliente + ")";
            }

            if (idFormaPagtoNota > 0)
                sql += ") Or idFormaPagto=" + idFormaPagtoNota + " Order By Descricao";
            else
                sql += ") Order By Descricao";

            var lstFormaPagto = objPersistence.LoadData(sql).ToList();
            
            return lstFormaPagto;
        }

        /// <summary>
        /// Retorna as formas de pagto que podem ser utilizadas na compra
        /// </summary>
        public IList<FormaPagto> GetForCompra()
        {
            var sqlFormasPagto = string.Format("{0},{1},{2},{3},{4}", (uint)Pagto.FormaPagto.Boleto, (uint)Pagto.FormaPagto.ChequeProprio, (uint)Pagto.FormaPagto.Dinheiro,
                (uint)Pagto.FormaPagto.Deposito, (uint)Pagto.FormaPagto.Prazo);

            if (FinanceiroConfig.FormaPagamento.PermitirFormaPagtoPermutaApenasAdministrador)
            {
                if (UserInfo.GetUserInfo.IsAdministrador)
                    sqlFormasPagto += string.Format(",{0}", (uint)Pagto.FormaPagto.Permuta);
            }
            else
                sqlFormasPagto += string.Format(",{0}", (uint)Pagto.FormaPagto.Permuta);

            if (FinanceiroConfig.FormaPagamento.SepararTiposChequesRecebimento)
                sqlFormasPagto += string.Format(",{0}", (int)Pagto.FormaPagto.ChequeTerceiro);

            sqlFormasPagto = string.Format("SELECT *, TRUE AS UtilizarPagamento FROM formapagto WHERE !ApenasSistema AND IdFormaPagto IN ({0}) ORDER BY Descricao", sqlFormasPagto);

            var formasPagto = objPersistence.LoadData(sqlFormasPagto).ToList();

            if (FinanceiroConfig.UsarPgtoAntecipFornec && FornecedorConfig.TipoUsoAntecipacaoFornecedor == DataSources.TipoUsoAntecipacaoFornecedor.CompraOuNotaFiscal &&
                AntecipacaoFornecedorDAO.Instance.PossuiAntecipacoesEmAberto(0))
            {
                formasPagto.Add(new FormaPagto()
                {
                    IdFormaPagto = (uint)Pagto.FormaPagto.AntecipFornec,
                    Descricao = GetDescricao(Pagto.FormaPagto.AntecipFornec)
                });
            }

            return formasPagto.ToArray();
        }

        /// <summary>
        /// Retorna as formas de pagto que podem ser utilizadas no controle de parcelas.
        /// </summary>
        public IList<FormaPagto> ObterFormasPagtoParaControleParcelas()
        {
            var sql = "SELECT *, 0 AS UtilizarPagamento FROM formapagto WHERE 1";

            if (!FinanceiroConfig.FormaPagamento.SepararTiposChequesRecebimento)
                sql += string.Format(" AND !ApenasSistema AND IdFormaPagto NOT IN ({0})", (int)Pagto.FormaPagto.ChequeTerceiro);

            sql += " ORDER BY Descricao";

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Retorna as formas de pagamento que podem ser usadas por um cliente.
        /// </summary>
        public IList<FormaPagto> GetByCliente(uint idCliente)
        {
            return GetByCliente(null, idCliente);
        }

        /// <summary>
        /// Retorna as formas de pagamento que podem ser usadas por um cliente.
        /// </summary>
        public IList<FormaPagto> GetByCliente(GDASession session, uint idCliente)
        {
            return GetByCliente(session, idCliente, true);
        }

        /// <summary>
        /// Retorna as formas de pagamento que podem ser usadas por um cliente.
        /// </summary>
        public IList<FormaPagto> GetByCliente(uint idCliente, bool apenasUsar)
        {
            return GetByCliente(null, idCliente, apenasUsar);
        }

        /// <summary>
        /// Retorna as formas de pagamento que podem ser usadas por um cliente.
        /// </summary>
        public IList<FormaPagto> GetByCliente(GDASession session, uint idCliente, bool apenasUsar)
        {
            return objPersistence.LoadData(session, Sql(0, idCliente, apenasUsar, true)).ToList();
        }

        public IList<FormaPagto> GetForControle()
        {
            return objPersistence.LoadData(Sql(0, 0, false, true)).ToList();
        }
	}
}