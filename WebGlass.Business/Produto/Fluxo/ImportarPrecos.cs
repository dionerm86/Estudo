using System;
using System.Collections.Generic;
using System.IO;
using Glass.Data.DAL;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace WebGlass.Business.Produto.Fluxo
{
    public sealed class ImportarPrecos : BaseFluxo<ImportarPrecos>
    {
        private ImportarPrecos() { }

        #region Classe de suporte

        private class DadosImportados
        {
            public string Codigo, Descricao;
            public decimal? CustoForn, CustoCompra, Balcao, Obra, AtacadoRepos, Minimo, Reposicao;
        }

        #endregion

        public void Importar(byte[] arquivoExcel)
        {
            using (var ms = new MemoryStream(arquivoExcel))
            {
                var arquivo = new HSSFWorkbook(ms);

                var produtos = new List<DadosImportados>();

                #region Busca as células que contém os produtos

                var planilha = arquivo.GetSheetAt(0);
                
                ICell ci = planilha.GetRow(8).Cells[0], cf;

                if (String.IsNullOrEmpty(ci.ToString() as string))
                    throw new Exception("Não há produtos no arquivo importado, ou o arquivo é inválido.");

                IRow lastRow = planilha.GetRow(planilha.LastRowNum);

                if (lastRow == null || lastRow.Cells.Count < 9)
                    lastRow = planilha.GetRow(planilha.LastRowNum - 1);

                /* Chamado 35421. */
                try
                {
                    cf = lastRow.Cells[12];
                }
                catch
                {
                    cf = lastRow.Cells[8];
                }

                if (cf == null)
                    throw new Exception("Não foi possível determinar a última linha do arquivo que possui valor.");

                var celulasProdutos = new List<IRow>();
                for (int i = ci.RowIndex; i <= cf.RowIndex; i++)
                    celulasProdutos.Add(planilha.GetRow(i));

                #endregion

                #region Busca os dados dos produtos do arquivo

                for (int i = 0; i < celulasProdutos.Count; i++)
                {
                    var p = new DadosImportados()
                    {
                        Codigo = celulasProdutos[i].Cells[0].ToString(),
                        Descricao = celulasProdutos[i].Cells[2].ToString(),
                        CustoForn = celulasProdutos[i].Cells[6].CellType == CellType.Numeric ? (decimal?)celulasProdutos[i].Cells[6].NumericCellValue : null,
                        CustoCompra = celulasProdutos[i].Cells[7].CellType == CellType.Numeric ? (decimal?)celulasProdutos[i].Cells[7].NumericCellValue : null,
                        Balcao = celulasProdutos[i].Cells[8].CellType == CellType.Numeric ? (decimal?)celulasProdutos[i].Cells[8].NumericCellValue : null,
                        Obra = celulasProdutos[i].Cells[9].CellType == CellType.Numeric ? (decimal?)celulasProdutos[i].Cells[9].NumericCellValue : null,
                        AtacadoRepos = celulasProdutos[i].Cells[10].CellType == CellType.Numeric ? (decimal?)celulasProdutos[i].Cells[10].NumericCellValue : null,
                        Minimo = celulasProdutos[i].Cells[11].CellType == CellType.Numeric ? (decimal?)celulasProdutos[i].Cells[11].NumericCellValue : null,
                        Reposicao = celulasProdutos[i].Cells[12].CellType == CellType.Numeric ? (decimal?)celulasProdutos[i].Cells[12].NumericCellValue : null
                    };

                    if (!String.IsNullOrEmpty(p.Codigo) && (p.CustoForn != null || p.CustoCompra != null || p.Balcao != null ||
                        p.Obra != null || p.AtacadoRepos != null || p.Minimo != null || p.Reposicao != null))
                        produtos.Add(p);
                }

                #endregion

                foreach (var p in produtos)
                {
                    var produto = ProdutoDAO.Instance.GetByCodInterno(p.Codigo);
                    if (produto == null)
                        continue;

                    if (p.CustoForn.HasValue) produto.Custofabbase = p.CustoForn.Value;
                    if (p.CustoCompra.HasValue) produto.CustoCompra = p.CustoCompra.Value;
                    if (p.Balcao.HasValue) produto.ValorBalcao = p.Balcao.Value;
                    if (p.Obra.HasValue) produto.ValorObra = p.Obra.Value;
                    if (p.AtacadoRepos.HasValue) produto.ValorAtacadoRepos = p.AtacadoRepos.Value;
                    if (p.Minimo.HasValue) produto.ValorMinimo = p.Minimo.Value;
                    if (p.Reposicao.HasValue) produto.ValorReposicao = p.Reposicao.Value;

                    ProdutoDAO.Instance.UpdateBase(produto);
                }
            }
        }
    }
}
