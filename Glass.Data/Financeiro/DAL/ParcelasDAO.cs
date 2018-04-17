using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class ParcelasDAO : BaseDAO<Parcelas, ParcelasDAO>
    {
        //private ParcelasDAO() { }

        public enum TipoConsulta
        {
            Todos,
            Vista,
            Prazo
        }

        private string Sql(uint idParcela, uint idCliente, uint idFornecedor, bool apenasUsar, TipoConsulta tipo, 
            int numeroParcelas, bool selecionar)
        {
            string campos = selecionar ? "*" : "count(*)";
            bool usar = idCliente > 0 || idFornecedor > 0;
            string sqlUsar = "";

            if (usar)
            {
                if (idCliente > 0)
                    sqlUsar = "idParcela " + (selecionar ? "" : "not ") + "in (select idParcela from parcelas_nao_usar where idCliente=" + idCliente + ")";
                else if (idFornecedor > 0)
                    sqlUsar = "idParcela " + (selecionar ? "" : "not ") + "in (select idParcela from parcelas_nao_usar where idFornec=" + idFornecedor + ")";

                if (selecionar)
                    campos += ", (" + sqlUsar + ") as NaoUsar";
            }

            string sql = "select " + campos + " from parcelas where 1";

            if (idParcela > 0)
                sql += " and idParcela=" + idParcela;

            if (tipo == TipoConsulta.Vista)
                sql += " and numParcelas=0";
            else if (tipo == TipoConsulta.Prazo)
                sql += " and numParcelas>0";

            if (numeroParcelas > 0)
                sql += " and numParcelas=" + numeroParcelas;

            if (apenasUsar && usar)
                sql += selecionar ? " having naoUsar=false" : " and " + sqlUsar;

            return sql;
        }

        public Parcelas GetElement(uint idParcela)
        {
            return GetElement(null, idParcela);
        }

        public Parcelas GetElement(GDASession session, uint idParcela)
        {
            return objPersistence.LoadOneData(session, Sql(idParcela, 0, 0, false, TipoConsulta.Todos, 0, true));
        }

        public Parcelas GetElement(uint idParcela, uint idCliente, uint idFornecedor)
        {
            return objPersistence.LoadOneData(Sql(idParcela, idCliente, idFornecedor, false, TipoConsulta.Todos, 0, true));
        }

        public IList<Parcelas> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, 0, 0, false, TipoConsulta.Todos, 0, true), sortExpression, startRow, pageSize);
        }

        public int GetListCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, 0, 0, false, TipoConsulta.Todos, 0, false));
        }

        public Parcelas[] GetByClienteFornecedor(uint idCliente, uint idFornecedor, bool apenasUsar, TipoConsulta tipo)
        {
            return GetByClienteFornecedor(idCliente, idFornecedor, apenasUsar, tipo, 0);
        }

        public Parcelas[] GetByClienteFornecedor(uint idCliente, uint idFornecedor, bool apenasUsar, TipoConsulta tipo, int numeroParcelas)
        {
            return objPersistence.LoadData(Sql(0, idCliente, idFornecedor, apenasUsar, tipo, numeroParcelas, true)).ToList().ToArray();
        }

        public IList<Parcelas> GetByCliente(uint idCliente, TipoConsulta tipo)
        {
            return GetByCliente(null, idCliente, tipo);
        }

        public IList<Parcelas> GetByCliente(GDASession session, uint idCliente, TipoConsulta tipo)
        {
            return objPersistence.LoadData(session, Sql(0, idCliente, 0, true, tipo, 0, true)).ToList();
        }

        public int GetCountByCliente(uint idCliente, TipoConsulta tipo)
        {
            return GetCountByCliente(null, idCliente, tipo);
        }

        public int GetCountByCliente(GDASession session, uint idCliente, TipoConsulta tipo)
        {
            return objPersistence.ExecuteSqlQueryCount(session, Sql(0, idCliente, 0, true, tipo, 0, false));
        }

        public Parcelas GetPadraoCliente(uint idCliente)
        {
            return GetPadraoCliente(null, idCliente);
        }

        public Parcelas GetPadraoCliente(GDASession sessao, uint idCliente)
        {
            string sql = Sql(0, 0, 0, false, TipoConsulta.Todos, 0, true);
            sql += " and idParcela=(select tipoPagto from cliente where id_Cli=" + idCliente + ")";

            List<Parcelas> itens = objPersistence.LoadData(sessao, sql);
            return itens.Count > 0 ? itens[0] : null;
        }

        public IList<Parcelas> GetByFornecedor(uint idFornecedor, TipoConsulta tipo)
        {
            return objPersistence.LoadData(Sql(0, 0, idFornecedor, true, tipo, 0, true)).ToList();
        }

        public int GetCountByFornecedor(uint idFornecedor, TipoConsulta tipo)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, 0, idFornecedor, true, tipo, 0, false));
        }

        public string ObtemDescricao(uint idParcela)
        {
            return ObtemDescricao(null, idParcela);
        }

        public string ObtemDescricao(GDASession session, uint idParcela)
        {
            return ObtemValorCampo<string>(session, "descricao", "idParcela=" + idParcela);
        }

        public decimal ObtemDesconto(GDASession sessao, uint idParcela)
        {
            return ObtemValorCampo<decimal>(sessao, "desconto", "idParcela=" + idParcela);
        }

        public string GetTextoParcela(uint idCliente, uint idFornecedor)
        {
            try
            {
                uint? idParcela = null;

                if (idCliente > 0)
                    idParcela = ClienteDAO.Instance.ObtemValorCampo<uint?>("tipoPagto", "id_Cli=" + idCliente);

                else if (idFornecedor > 0)
                    idParcela = FornecedorDAO.Instance.ObtemValorCampo<uint?>("tipoPagto", "idFornec=" + idFornecedor);

                if (idParcela != null)
                    return ObtemDescricao(idParcela.Value);
            }
            catch { }

            return "";
        }

        /// <summary>
        /// Retorna o valor da propriedade ParcelaAVista, da parcela informada por parâmetro.
        /// </summary>
        public bool ObterParcelaAVista(GDASession sessao, int idParcela)
        {
            return ObtemValorCampo<bool>(sessao, "ParcelaAVista", string.Format("IdParcela={0}", idParcela));
        }

        public GenericModel[] GetNumeroParcelas()
        {
            string sql = "select concat('0,', cast(group_concat(distinct numParcelas) as char)) from parcelas where numParcelas>0 order by numParcelas";
            string parcelas = objPersistence.ExecuteScalar(sql).ToString();

            List<GenericModel> retorno = new List<GenericModel>();

            foreach (string s in parcelas.Split(','))
            {
                uint numParcela = Glass.Conversoes.StrParaUint(s);
                string descricao = numParcela == 0 ? "À vista" : numParcela + " parcela" + (numParcela > 1 ? "s" : "");
                retorno.Add(new GenericModel(numParcela, descricao));
            }

            return retorno.ToArray();
        }

        /// <summary>
        /// Retorna o prazo máximo (em dias) que o cliente/fornecedor possui.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idFornecedor"></param>
        /// <returns></returns>
        public int GetPrazoMaximoDias(uint idCliente, uint idFornecedor)
        {
            int numeroDias = 0;
            foreach (Parcelas p in GetByClienteFornecedor(idCliente, idFornecedor, true, TipoConsulta.Prazo))
            {
                int diasParcela = p.NumeroDias[p.NumeroDias.Length - 1];
                if (numeroDias < diasParcela)
                    numeroDias = diasParcela;
            }

            return numeroDias;
        }

        /// <summary>
        /// Retorna a primeira parcela encontrada com o número de parcelas indicado
        /// </summary>
        /// <param name="numParc"></param>
        /// <returns></returns>
        public Parcelas GetByNumeroParcelas(int numParc)
        {
            List<Parcelas> parc = objPersistence.LoadData(Sql(0, 0, 0, false, TipoConsulta.Prazo, numParc, true));
            return parc.Count > 0 ? parc[0] : null;
        }

        #region Controle de seleção de parcelas

        private uint? GetMenorPrazo(string idsPedidos)
        {
            string sql = "select * from parcelas where numParcelas>0 and idParcela in (select distinct idParcela " +
                "from pedido where idPedido in (" + idsPedidos + ")) order by numParcelas";

            int numeroParcelas = int.MaxValue;
            int numeroDias = int.MaxValue;
            
            uint? retorno = null;
            foreach (Parcelas p in objPersistence.LoadData(sql))
            {
                if (p.NumParcelas > numeroParcelas)
                    break;

                numeroParcelas = p.NumParcelas;
                if (p.NumeroDias[0] < numeroDias)
                {
                    numeroDias = p.NumeroDias[0];
                    retorno = (uint)p.IdParcela;
                }
            }

            return retorno;
        }

        public IList<Parcelas> GetForControleSelecionar(TipoConsulta tipo)
        {
            var erro = "";
            return GetForControleSelecionar(0, 0, "", false, tipo, out erro);
        }

        public IList<Parcelas> GetForControleSelecionar(uint idCliente, uint idFornecedor, string idsPedidos,
            bool exibirDatasParcelas, TipoConsulta tipo, out string msgErro)
        {
            Parcelas[] retorno;
            msgErro = String.Empty;

            if (!exibirDatasParcelas && !PedidoConfig.FormaPagamento.ExibirDatasParcelasPedido)
            {
                retorno = new Parcelas[PedidoConfig.FormaPagamento.NumParcelasPedido];
                for (int i = 0; i < PedidoConfig.FormaPagamento.NumParcelasPedido; i++)
                {
                    retorno[i] = new Parcelas();
                    retorno[i].IdParcela = i + 1;
                    retorno[i].NumParcelas = i + 1;
                    retorno[i].Descricao = retorno[i].NumParcelas.ToString();
                    retorno[i].Situacao = Situacao.Ativo;
                }
            }
            else if (idCliente == 0 && idFornecedor == 0)
            {
                retorno = GetAll();
                /* for (int i = 0; i < retorno.Length; i++)
                    retorno[i].Descricao = ""; */
            }
            else
            {
                if (String.IsNullOrEmpty(idsPedidos) && !PedidoConfig.DadosPedido.BloquearDadosClientePedido)
                    idCliente = 0;

                retorno = GetByClienteFornecedor(idCliente, idFornecedor, true, tipo);
            }

            if (!String.IsNullOrEmpty(idsPedidos) && Liberacao.DadosLiberacao.UsarMenorPrazoLiberarPedido)
            {
                uint? idMenorPrazo = GetMenorPrazo(idsPedidos);
                if (idMenorPrazo > 0)
                {
                    var menorParcela = Array.FindAll<Parcelas>(retorno, new Predicate<Parcelas>(
                        delegate(Parcelas p)
                        {
                            return p.IdParcela == idMenorPrazo;
                        }
                    ));

                    if (menorParcela.Length == 0)
                    {
                        retorno = GetByClienteFornecedor(idCliente, idFornecedor, true, tipo);

                        var parcelaMenorPrazo = ParcelasDAO.Instance.ObtemValorCampo<string>("descricao", "idParcela=" + idMenorPrazo);
                        msgErro = "Este cliente não possui mais associação com a parcela " + parcelaMenorPrazo + " que foi inserida neste pedido.";

                        return retorno;
                    }
                    
                    retorno = Array.FindAll<Parcelas>(retorno, new Predicate<Parcelas>(
                        delegate(Parcelas p)
                        {
                            return p.IdParcela == idMenorPrazo || p.NumParcelas <= menorParcela[0].NumParcelas;
                        }
                    ));
                }
            }

            return retorno.Where(f=>f.Situacao == Situacao.Ativo).ToList();
        }

        public List<Parcelas>ObterParcelasAtivas()
        {
            var sql = @"SELECT * FROM PARCELAS
                        WHERE SITUACAO=" + (int)Situacao.Ativo;

            return objPersistence.LoadData(sql);
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(Parcelas objInsert)
        {
            /*uint idParcela = base.Insert(objInsert);

            // Marca que todos os clientes não poderão usar esta parcela cadastrada
            objPersistence.ExecuteCommand("Insert Into parcelas_nao_usar (IdParcela, IdCliente) (Select " + idParcela + 
                ", id_Cli From cliente)");

            // Marca que todos os fornecedores não poderão usar esta parcela cadastrada
            objPersistence.ExecuteCommand("Insert Into parcelas_nao_usar (IdParcela, IdFornec) (Select " + idParcela +
                ", idFornec From fornecedor)");

            return idParcela;*/

            throw new NotSupportedException();
        }

        public override int Delete(Parcelas objDelete)
        {
            /*// Verifica se algum cliente possui associação com a parcela sendo excluída
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From cliente Where tipoPagto=" + objDelete.IdParcela) > 0)
                throw new Exception("Esta parcela não pode ser excluída pois existem clientes associados à mesma.");

            // Exclui as associações desta parcela com as parcelas que os clientes/fornecedores não podem usar.
            objPersistence.ExecuteCommand("Delete from parcelas_nao_usar Where idParcela=" + objDelete.IdParcela);

            return base.Delete(objDelete);*/
            throw new NotSupportedException();
        }

        #endregion
    }
}
