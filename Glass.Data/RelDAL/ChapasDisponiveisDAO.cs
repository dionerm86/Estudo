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

            var sql = "SELECT " + campos + @"
                FROM produto_impressao pi
                    INNER JOIN produtos_nf pnf ON (pi.idprodnf = pnf.idprodnf)
                    INNER JOIN produto p ON (p.idprod = pnf.idprod)
                    INNER JOIN nota_fiscal nf ON (pnf.idnf = nf.idnf)
                    INNER JOIN fornecedor f ON (nf.idFornec = f.idFornec)
                    LEFT JOIN chapa_corte_peca ccp On (ccp.idProdimpressaochapa = pi.idprodimpressao)
                    LEFT JOIN cor_vidro cv ON (p.IdCorVidro = cv.IdCorVidro)
                    LEFT JOIN perda_chapa_vidro pcv ON (pi.IdProdImpressao = pcv.IdProdImpressao AND COALESCE(pcv.Cancelado, 0) = 0)
                    LEFT JOIN subgrupo_prod s ON (p.idSubgrupoProd = s.idSubgrupoProd)
                WHERE !pi.cancelado 
                    AND pi.idprodnf IS NOT NULL 
                    AND ccp.IDCHAPACORTEPECA IS NULL
                    AND (pcv.IdPerdaChapaVidro IS NULL OR pcv.Cancelado = 1)
                    AND s.tipoSubgrupo IN(" + (int)TipoSubgrupoProd.ChapasVidro + "," + (int)TipoSubgrupoProd.ChapasVidroLaminado + @")
                    AND nf.tipoDocumento in (" + (int)NotaFiscal.TipoDoc.EntradaTerceiros + "," + (int)NotaFiscal.TipoDoc.NotaCliente + "," + (int)NotaFiscal.TipoDoc.Entrada + @")
                    AND nf.situacao IN(" + (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros + "," + (int)NotaFiscal.SituacaoEnum.Autorizada + @")
                    AND COALESCE(nf.gerarEtiqueta, 1) = 1 
                {1}
                ORDER BY cv.descricao, p.espessura, f.nomefantasia, nf.numeronfe, pi.IDPRODIMPRESSAO";

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
