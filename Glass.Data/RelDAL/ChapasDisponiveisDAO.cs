using Glass.Data.DAL;
using Glass.Data.RelModel;
using System.Collections.Generic;
using System.Linq;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.RelDAL
{
    public sealed class ChapasDisponiveisDAO : BaseDAO<ChapasDisponiveis, ChapasDisponiveisDAO>
    {
        private string Sql(uint idFornec, string nomeFornec, string codInternoProd, string descrProd, int numeroNfe, string lote, int altura, int largura,
            string idCor, int espessura, string numEtiqueta, int idLoja, bool selecionar)
        {
            var campos = selecionar?@"
                cv.descricao as Cor, p.Espessura, concat(p.codInterno, ' - ', p.descricao) as Produto,
                CONCAT(f.IdFornec, ' - ', f.NomeFantasia) as Fornecedor, nf.Numeronfe, pnf.Lote, pi.NumEtiqueta as Etiqueta, '{0}' as Criterio" 
                : "COUNT(*)";

            var sql = string.Format(@"SELECT {0}
                FROM produto_impressao pi
                    INNER JOIN produtos_nf pnf ON (pi.IdProdNf = pnf.IdProdNf)
                    INNER JOIN produto p ON (p.IdProd = pnf.IdProd)
                    INNER JOIN 
                        (SELECT nf1.IdNf, nf1.NumeroNfe, nf1.IdLoja, nf1.IdFornec FROM nota_fiscal nf1 WHERE nf1.GerarEtiqueta IS NOT NULL AND nf1.GerarEtiqueta=1
                            AND nf1.TipoDocumento IN ({3},{4},{5}) AND nf1.Situacao IN ({6},{7})
                        ) nf ON (pnf.IdNf = nf.IdNf)
                    INNER JOIN fornecedor f ON (nf.IdFornec = f.IdFornec)
                    LEFT JOIN cor_vidro cv ON (p.IdCorVidro = cv.IdCorVidro)
                    LEFT JOIN subgrupo_prod s ON (p.IdSubgrupoProd = s.IdSubgrupoProd)
                WHERE (pi.Cancelado IS NULL OR pi.Cancelado=0)
                    AND pi.IdProdNf IS NOT NULL 
                    AND pi.IdProdImpressao NOT IN (SELECT IdProdImpressaoChapa FROM chapa_corte_peca)
                    AND pi.IdProdImpressao NOT IN (SELECT IdProdImpressao FROM perda_chapa_vidro WHERE Cancelado IS NULL OR Cancelado=0)
                    AND s.TipoSubgrupo IN ({1},{2})
                {8}
                ORDER BY cv.Descricao, p.Espessura, f.Nomefantasia, nf.NumeroNfe, pi.IdProdImpressao",
                campos, (int)TipoSubgrupoProd.ChapasVidro, (int)TipoSubgrupoProd.ChapasVidroLaminado, (int)NotaFiscal.TipoDoc.EntradaTerceiros,
                (int)NotaFiscal.TipoDoc.NotaCliente, (int)NotaFiscal.TipoDoc.Entrada, (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros, (int)NotaFiscal.SituacaoEnum.Autorizada, "{1}");

            var filtro = "";
            var criterio = "";

            if (idFornec > 0)
            {
                filtro += " AND f.IdFornec = " + idFornec;
                criterio += "Fornecedor: " + FornecedorDAO.Instance.GetNome(idFornec) + "   ";
            }
            else if (!string.IsNullOrEmpty(nomeFornec))
            {
                filtro += " AND f.IdFornec IN (" + FornecedorDAO.Instance.ObtemIds(nomeFornec) + ")";
                criterio += "Fornecedor: " + nomeFornec + "   ";
            }

            if (!string.IsNullOrEmpty(codInternoProd))
            {
                filtro += " AND p.IdProd IN (" + ProdutoDAO.Instance.ObtemIds(codInternoProd, descrProd) + ")";
                criterio += "Produto: " + ProdutoDAO.Instance.ObtemDescricao(codInternoProd) + "   ";
            }
            else if (!string.IsNullOrEmpty(descrProd))
            {
                filtro += " AND p.IdProd IN (" + ProdutoDAO.Instance.ObtemIds(codInternoProd, descrProd) + ")";
                criterio += "Produto: " + descrProd + "   ";
            }

            if (numeroNfe > 0)
            {
                filtro += " AND nf.NumeroNfe = " + numeroNfe;
                criterio += "Nota Fiscal: " + numeroNfe + "   ";
            }

            if (!string.IsNullOrEmpty(lote))
            {
                filtro += " AND pnf.lote LIKE ?lote";
                criterio += "Lote: " + lote + "   ";
            }

            if (altura > 0)
            {
                filtro += " AND p.Altura = " + altura;
                criterio += "Altura: " + altura + "   ";
            }

            if (largura > 0)
            {
                filtro += " AND p.Largura = " + largura;
                criterio += "Largura: " + largura + "   ";
            }

            if (!string.IsNullOrEmpty(idCor))
            {
                filtro += " AND p.IdCorVidro IN( " + idCor + ")";

                string desc = "";

                foreach (var id in idCor.Split(',').Select(f => f.StrParaUint()).ToList())
                    desc += CorVidroDAO.Instance.GetNome(id) + ",";

                criterio += "Cor: " + desc.Trim(',') + "   ";
            }

            if (espessura > 0)
            {
                filtro += " AND p.espessura = " + espessura;
                criterio += "Espessura: " + espessura + "   ";
            }

            if (!string.IsNullOrEmpty(numEtiqueta))
            {
                filtro += " AND pi.numEtiqueta = ?numEtiqueta";
                criterio += "Etiqueta: " + numEtiqueta + "   ";
            }

            if (idLoja > 0)
            {
                filtro += " AND nf.IdLoja = "+idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome((uint)idLoja) + "   ";
            }

            return string.Format(sql, criterio, filtro);
        }

        public IList<ChapasDisponiveis> ObtemChapasDisponiveis(uint idFornec, string nomeFornec, string codInternoProd, string descrProd, int numeroNfe, string lote, int altura, int largura,
            string idCor, int espessura, string numEtiqueta, int idLoja,
            string sortExpression, int startRow, int pageSize)
        {
            var sql = Sql(idFornec, nomeFornec, codInternoProd, descrProd, numeroNfe, lote, altura, largura, idCor, espessura, numEtiqueta, idLoja, true);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, GetParams(lote,numEtiqueta));
        }

        public int ObtemChapasDisponiveisCount(uint idFornec, string nomeFornec, string codInternoProd, string descrProd, int numeroNfe, string lote, int altura, int largura,
            string idCor, int espessura, string numEtiqueta, int idLoja)
        {
            var sql = Sql(idFornec, nomeFornec, codInternoProd, descrProd, numeroNfe, lote, altura, largura, idCor, espessura, numEtiqueta, idLoja, false);

            return objPersistence.ExecuteSqlQueryCount(sql, GetParams(lote, numEtiqueta));
        }

        public IList<ChapasDisponiveis> ObtemChapasDisponiveisRpt(uint idFornec, string nomeFornec, string codInternoProd, string descrProd, int numeroNfe, string lote, int altura, int largura,
            string idCor, int espessura, string numEtiqueta, int idLoja)
        {
            var sql = Sql(idFornec, nomeFornec, codInternoProd, descrProd, numeroNfe, lote, altura, largura, idCor, espessura, numEtiqueta, idLoja, true);

            return objPersistence.LoadData(sql, GetParams(lote, numEtiqueta)).ToList();
        }

        private GDAParameter[] GetParams(string lote, string numEtiqueta)
        {
            var p = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(lote))
                p.Add(new GDAParameter("?lote", "%" + lote + "%"));

            if (!string.IsNullOrEmpty(numEtiqueta))
                p.Add(new GDAParameter("?numEtiqueta", numEtiqueta));

            return p.ToArray();
        }
    }
}
