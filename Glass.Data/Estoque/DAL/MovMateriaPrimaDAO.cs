using GDA;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.DAL
{
    public sealed class MovMateriaPrimaDAO : BaseDAO<MovMateriaPrima, MovMateriaPrimaDAO>
    {
        #region Métodos Privados

        private int? ObtemUltimoIdMovMateriaPrima(GDASession sessao, int idCorVidro, decimal espessura)
        {
            var sql = @"
                SELECT idMovMateriaPrima 
                FROM mov_materia_prima
                WHERE idCorVidro=?idCorVidro AND espessura=?espessura
                ORDER BY dataMov desc, idMovMateriaPrima desc limit 1";

            return ExecuteScalar<int?>(sessao, sql,
                new GDAParameter("?idCorVidro", idCorVidro), new GDAParameter("?espessura", espessura));
        }

        private void MovimentaMateriaPrima(GDASession sessao, int idCorVidro, decimal espessura, MovEstoque.TipoMovEnum tipoMov, decimal qntd,
            int? idProdNf, int? idPerdaChapaVidro, int? idProdImpressao, int? idProdPed, int? idProdPedEsp)
        {
            var saldoAtual = ObterSaldo(sessao, idCorVidro, espessura);

            MovMateriaPrima mov = new MovMateriaPrima();

            mov.IdCorVidro = idCorVidro;
            mov.Espessura = espessura;
            mov.IdFunc = Helper.UserInfo.GetUserInfo.CodUser;
            mov.DataMov = DateTime.Now;
            mov.TipoMov = tipoMov;
            mov.Qtde = qntd;
            mov.Saldo = tipoMov == MovEstoque.TipoMovEnum.Entrada ? saldoAtual + qntd : saldoAtual - qntd;

            mov.IdProdNf = idProdNf;
            mov.IdPerdaChapaVidro = idPerdaChapaVidro;
            mov.IdProdImpressao = idProdImpressao;
            mov.IdProdPed = idProdPed;
            mov.IdProdPedEsp = idProdPedEsp;

            Insert(sessao, mov);
        }

        /// <summary>
        /// Recupera a cor e espessura do produto, caso não possuam valor, lança uma exceção ao usuário.
        /// </summary>
        private void RecuperaCorEspessuraProduto(GDASession session, int idProd, out int idCorVidro, out decimal espessura)
        {
            if (idProd == 0)
                throw new Exception("Não foi possível recuperar o produto para movimentar a matéria-prima.");

            var codInterno = ProdutoDAO.Instance.GetCodInterno(session, idProd);

            idCorVidro = ProdutoDAO.Instance.ObtemIdCorVidro(session, idProd).GetValueOrDefault();

            if (idCorVidro == 0)
                throw new Exception(string.Format("Informe a cor do vidro {0}, em seu cadastro.", codInterno));

            espessura = (decimal)ProdutoDAO.Instance.ObtemEspessura(session, idProd);

            if (espessura == 0)
                throw new Exception(string.Format("Informe a espessura do vidro {0}, em seu cadastro.", codInterno));
        }

        #endregion

        #region Métodos Publicos

        public decimal ObterSaldo(GDASession sessao, int idCorVidro, decimal espessura)
        {
            var id = ObtemUltimoIdMovMateriaPrima(sessao, idCorVidro, espessura);

            if (id.GetValueOrDefault() == 0)
                return 0;

            return ObtemValorCampo<decimal>(sessao, "Saldo", "IdMovMateriaPrima=" + id);
        }

        public void MovimentaMateriaPrimaPerdaChapaVidro(GDASession sessao, int idProd, int idProdNf, int idPerdaChapaVidro)
        {
            //Se for materia-prima (Chapas) faz a movimentação
            var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, idProd);
            if (tipoSubgrupo == TipoSubgrupoProd.ChapasVidro || tipoSubgrupo == TipoSubgrupoProd.ChapasVidroLaminado)
            {
                int idCorVidro;
                decimal espessura;

                RecuperaCorEspessuraProduto(sessao, idProd, out idCorVidro, out espessura);

                var m2 = ProdutoDAO.Instance.ObtemM2Chapa(sessao, idProd);

                MovimentaMateriaPrima(null, idCorVidro, espessura, MovEstoque.TipoMovEnum.Saida, m2, idProdNf, idPerdaChapaVidro, null, null, null);
            }
        }

        public void MovimentaMateriaPrimaChapaCortePeca(GDASession sessao, int idProdImpressaoChapa, MovEstoque.TipoMovEnum tipoMov)
        {
            if (idProdImpressaoChapa > 0 && !ChapaCortePecaDAO.Instance.ChapaPossuiLeitura(sessao, (uint)idProdImpressaoChapa))
            {
                uint? idProd = ProdutoImpressaoDAO.Instance.GetIdProd(sessao, (uint)idProdImpressaoChapa);

                //Se for materia-prima (Chapas) faz a movimentação
                var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, (int)idProd);
                if (tipoSubgrupo == TipoSubgrupoProd.ChapasVidro || tipoSubgrupo == TipoSubgrupoProd.ChapasVidroLaminado)
                {
                    int idCorVidro;
                    decimal espessura;

                    RecuperaCorEspessuraProduto(sessao, (int)idProd.GetValueOrDefault(), out idCorVidro, out espessura);

                    var m2 = ProdutoDAO.Instance.ObtemM2Chapa(sessao, (int)idProd);
                    
                    MovimentaMateriaPrima(sessao, idCorVidro, espessura, tipoMov, m2, null, null, idProdImpressaoChapa, null, null);
                }
            }
        }

        public void MovimentaMateriaPrimaCorte(GDASession sessao, int idProdImpressaoPeca, MovEstoque.TipoMovEnum tipoMov)
        {
            var idProdPed = ProdutoImpressaoDAO.Instance.ObtemIdProdPed(sessao, idProdImpressaoPeca);
            var idProd = ProdutoImpressaoDAO.Instance.GetIdProd(sessao, (uint)idProdImpressaoPeca);

            int idCorVidro;
            decimal espessura;

            RecuperaCorEspessuraProduto(sessao, (int)idProd.GetValueOrDefault(), out idCorVidro, out espessura);

            var totM = ProdutosPedidoEspelhoDAO.Instance.ObtemTotM(sessao, (uint)idProdPed);
            var qtde = ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(sessao, (uint)idProdPed);

            if (qtde == 0 || totM == 0)
                return;

            var m2 = (decimal)Math.Round(totM / qtde, 2);

            MovimentaMateriaPrima(sessao, idCorVidro, espessura, tipoMov, m2, null, null, idProdImpressaoPeca, null, null);
        }

        public void MovimentaMateriaPrimaNotaFiscal(GDASession sessao, int idProd, int idNf, int idProdNf, decimal qtde, MovEstoque.TipoMovEnum tipoMov)
        {
            //Se for materia-prima (Chapas) faz a movimentação
            var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, idProd);
            if (NotaFiscalDAO.Instance.ObtemValorCampo<bool>("gerarEtiqueta", "idNf=" + idNf) && (tipoSubgrupo == TipoSubgrupoProd.ChapasVidro || tipoSubgrupo == TipoSubgrupoProd.ChapasVidroLaminado))
            {
                int idCorVidro;
                decimal espessura;

                RecuperaCorEspessuraProduto(sessao, idProd, out idCorVidro, out espessura);

                MovimentaMateriaPrima(null, idCorVidro, espessura, tipoMov, qtde, idProdNf, null, null, null, null);
            }
        }

        public void MovimentaMateriaPrimaPedido(GDASession sessao, int idProdPed, decimal qntd, MovEstoque.TipoMovEnum tipoMov)
        {
            var idProd = ProdutosPedidoDAO.Instance.ObtemIdProd(sessao, (uint)idProdPed);

            int idCorVidro;
            decimal espessura;

            RecuperaCorEspessuraProduto(sessao, (int)idProd, out idCorVidro, out espessura);
            
            MovimentaMateriaPrima(sessao, idCorVidro, espessura, tipoMov, qntd, null, null, null, idProdPed, null);
        }

        public void MovimentaMateriaPrimaPedidoEspelho(GDASession sessao, int idProdPedEsp, decimal qntd, MovEstoque.TipoMovEnum tipoMov)
        {
            var idProd = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd(sessao, (uint)idProdPedEsp);

            int idCorVidro;
            decimal espessura;

            RecuperaCorEspessuraProduto(sessao, (int)idProd, out idCorVidro, out espessura);
            
            MovimentaMateriaPrima(sessao, idCorVidro, espessura, tipoMov, qntd, null, null, null, null, idProdPedEsp);
        }

        public bool VeiricaPedido(int idPedido, out string msg)
        {
            var sql = @"
                SELECT CONCAT(COALESCE(p.IdCorVidro, 0), ';', COALESCE(p.Espessura, 0), ';', COALESCE(SUM(pp.TotM), 0))
                FROM produtos_pedido pp
	                LEFT JOIN produto p ON (pp.IdProd = p.IdProd)
                WHERE COALESCE(pp.InvisivelFluxo, 0) = 0 
	                AND p.IdGrupoProd = " + (int)NomeGrupoProd.Vidro + @"
	                AND pp.IdPedido = " + idPedido + @"
                GROUP BY p.IdCorVidro, p.Espessura";

            var prods = ExecuteMultipleScalar<string>(sql);

            foreach (var p in prods)
            {
                var dados = p.Split(';');
                var idCorVidro = dados[0].StrParaInt();
                var espessura = dados[1].StrParaDecimal();
                var totM = dados[2].StrParaDecimal();

                var saldo = ObterSaldo(null, idCorVidro, espessura);

                if(saldo < totM)
                {
                    var cor = CorVidroDAO.Instance.GetNome((uint)idCorVidro);
                    msg = "Não à matéria-prima suficiente para cor " + cor + " e espessura " + espessura + "mm. Disponível: " + Math.Round(saldo, 2) + " necessário: " + Math.Round(totM, 2);
                    return false;
                }
            }

            msg = "";
            return true;
        }

        #endregion
    }
}
