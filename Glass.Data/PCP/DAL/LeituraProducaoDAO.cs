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
        //private LeituraProducaoDAO() { }

        public IList<LeituraProducao> GetByProdPedProducao(GDASession sessao, uint idProdPedProducao)
        {
            string sql = "select * from leitura_producao where idProdPedProducao=" + idProdPedProducao;
            return objPersistence.LoadData(sessao, sql).ToList();
        }

        public IList<LeituraProducao> GetByProdPedProducao(uint idProdPedProducao)
        {
            return GetByProdPedProducao(null, idProdPedProducao);
        }

        public int GetCountByProdPedProducao(GDASession sessao, uint idProdPedProducao)
        {
            string sql = "select count(*) from leitura_producao where idProdPedProducao=" + idProdPedProducao;
            return objPersistence.ExecuteSqlQueryCount(sessao, sql);
        }

        /// <summary>
        /// Verifica se a peça passou por algum setor de corte.
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Insere a leitura da peça passada no setor informado
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <param name="idSetor"></param>
        /// <param name="idFuncLeitura"></param>
        /// <param name="dataLeitura"></param>
        /// <param name="releitura"></param>
        /// <returns></returns>
        public LeituraProducao LeituraPeca(GDASession sessao, uint idProdPedProducao, uint idSetor, uint idFuncLeitura, DateTime? dataLeitura, bool releitura, int? idCavalete)
        {
            if (idFuncLeitura == 0 && UserInfo.GetUserInfo != null && UserInfo.GetUserInfo.CodUser > 0)
                idFuncLeitura = UserInfo.GetUserInfo.CodUser;

            // Garante que a peça só seja lida se houver data de impressão
            if (!SetorDAO.Instance.IsPecaImpressa(sessao, idProdPedProducao) && idSetor > 1)
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

            if (!SetorDAO.Instance.IsUltimoSetor(sessao, idSetor, idProdPedProducao) && !releitura)
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

        /// <summary>
        /// Obtem o última leitura da peça passada
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <returns>IdSetor</returns>
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
        /// <param name="idProdPedProducao"></param>
        /// <returns>IdSetor</returns>
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
        /// <param name="sessao"></param>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Indica se a peça já passou por um setor específico.
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public bool PassouSetor(uint idProdPedProducao, uint idSetor)
        {
            return PassouSetor(null, idProdPedProducao, idSetor);
        }

        /// <summary>
        /// Indica se a peça já passou por um setor específico.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdPedProducao"></param>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public bool PassouSetor(GDASession sessao, uint idProdPedProducao, uint idSetor)
        {
            uint idUltimoSetor = ObtemUltimoSetorLido(sessao, idProdPedProducao);

            if (idUltimoSetor == 0)
                return false;

            return Utils.ObtemSetor(idSetor).NumeroSequencia <= Utils.ObtemSetor(idUltimoSetor).NumeroSequencia;
        }

        /// <summary>
        /// Indica se a peça já passou por um setor específico.
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public bool PassouSetor(string numEtiqueta, uint idSetor)
        {
            return PassouSetor(null, numEtiqueta, idSetor);
        }

        /// <summary>
        /// Indica se a peça já passou por um setor específico.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="numEtiqueta"></param>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public bool PassouSetor(GDASession sessao, string numEtiqueta, uint idSetor)
        {
            uint? id = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(sessao, numEtiqueta);
            return id > 0 ? PassouSetor(sessao, id.Value, idSetor) : false;
        }

        /// <summary>
        /// Indica se a peça já passou por um setor laminado.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public bool PassouSetorLaminado(GDASession sessao, string numEtiqueta)
        {
            var setores = ObtemSetoresLidos(ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(sessao, numEtiqueta).GetValueOrDefault());

            return !string.IsNullOrEmpty(setores) && setores.Split(',').Any(setorLido => SetorDAO.Instance.ObtemIdsSetorLaminados().Contains(setorLido));
        }

        /// <summary>
        /// Obtem os setores lidos da peça passada
        /// </summary>
        public string ObtemSetoresLidos(uint idProdPedProducao)
        {
            return ObtemSetoresLidos(null, idProdPedProducao);
        }

        /// <summary>
        /// Obtem os setores lidos da peça passada
        /// </summary>
        public string ObtemSetoresLidos(GDASession session, uint idProdPedProducao)
        {
            var sql = @"
                Select lprod.idSetor
                From leitura_producao lprod  
                    Inner Join setor s On (lprod.idSetor=s.idSetor)
                Where idProdPedProducao=" + idProdPedProducao;

            return GetValoresCampo(session, sql, "idSetor");
        }

        /// (APAGAR: quando alterar para utilizar transação)
        /// <summary>
        /// Verifica se a peça já foi lida no setor informado
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public bool VerificaLeituraPeca(uint idProdPedProducao, uint idSetor)
        {
            return VerificaLeituraPeca(null, idProdPedProducao, idSetor);
        }

        /// <summary>
        /// Verifica se a peça já foi lida no setor informado
        /// </summary>
        /// <param name="codEtiqueta"></param>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public bool VerificaLeituraPeca(GDASession sessao, uint idProdPedProducao, uint idSetor)
        {
            string sql = "Select Count(*) From leitura_producao Where IdProdPedProducao=" + 
                idProdPedProducao + " And idSetor=" + idSetor;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se a peça já foi lida no setor informado
        /// </summary>
        /// <param name="codEtiqueta"></param>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public bool VerificaLeituraPeca(string codEtiqueta, uint idSetor)
        {
            uint? id = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(codEtiqueta);
            return id > 0 ? VerificaLeituraPeca(id.Value, idSetor) : false;
        }

        /// <summary>
        /// Recupera uma lista de objetos com base no idProdPed
        /// </summary>
        /// <param name="idProdPed">Identificador</param>
        /// <returns>Array LeituraProducao</returns>
        public IList<LeituraProducao> ObterLeituraProducao(uint idProdPed)
        {
            string sql = @"select l.* from produto_pedido_producao pr
                            inner join leitura_producao l on(pr.IDPRODPEDPRODUCAO=l.IDPRODPEDPRODUCAO)
                            where pr.IDPRODPED=?id";

            return objPersistence.LoadData(sql ,new GDAParameter("?id", idProdPed)).ToList();
        }

        public uint ObtemIdLeituraProd(GDASession sessao, uint idProdPedProducao, uint idSetor)
        {
            return ObtemValorCampo<uint>(sessao, "idLeituraProd", string.Format("idSetor = {0} AND idprodpedproducao = {1}", idSetor, idProdPedProducao));
        }

        public uint ObtemIdLeituraProd(uint idProdPedProducao, uint idSetor)
        {
            return ObtemIdLeituraProd(null, idProdPedProducao, idSetor);
        }

        public override int Delete(LeituraProducao objDelete)
        {
            return Delete(null, objDelete);
        }

        public override int Delete(GDASession sessao, LeituraProducao objDelete)
        {
            return base.Delete(sessao, objDelete);
        }

        public bool TeveLeituraPosterior(GDASession sessao, int idSetor, DateTime dataInicial)
        {
            var sql = @"
                SELECT COUNT(*)
                FROM leitura_producao
                WHERE IdSetor = " + idSetor + " AND DataLeitura > ?dtIni";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, new GDAParameter("?dtIni", dataInicial)) > 0;
        }
    }
}
