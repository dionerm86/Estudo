using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public class DipjDAO : Glass.Data.DAL.BaseDAO<RelModel.Dipj, DipjDAO>
    {
        #region Métodos Privados

        /// <summary>
        /// Formata o resultado dos forncedores e dos clientes.
        /// </summary>
        /// <param name="consulta"></param>
        /// <param name="pageSize"></param>
        /// <param name="manipuladorAjuste">Método usado para reliza os ajuste nos registros do resultado.</param>
        /// <returns></returns>
        private static IList<RelModel.Dipj> FormaResultado
            (GDA.Sql.NativeQuery consulta, int pageSize, Func<RelModel.Dipj, RelModel.Dipj> manipuladorAjuste)
        {
            if (pageSize <= 0)
                return consulta
                    .ToCursor<RelModel.Dipj>()
                    .Select(manipuladorAjuste)
                    .ToList();
            else
            {
                var resultado = consulta.ToResultList<RelModel.Dipj>(pageSize);
                resultado.LoadResultPage += (sender, e) =>
                {
                    foreach (var i in e.Page)
                        manipuladorAjuste(i);
                };

                return resultado;
            }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera as entradas de forncedores.
        /// </summary>
        /// <param name="dataEmissaoInicial">Data inicial da emissão.</param>
        /// <param name="dataEmissaoFinal">Data final da emissão.</param>
        /// <param name="pageSize">Tamanho da página de dados.</param>
        /// <returns></returns>
        public IList<RelModel.Dipj> ObtemEntradaFornecedores(DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int pageSize)
        {
            var codInternoCfop = "'1101','1102','1111','1113','1116','1117','1118','1120','1121','1122','1124','1125','1126','1151'," +
                   "'1152','1154','1401','1403','1408','1409','1410','1411','1414','1415','1501','1503','1504','1901','1902','1903','1904'," +
                   "'1905','1906','1907','1908','1909','1910','1911','1912','1913','1914','1915','1916','1917','1918','1919','1920','1921'," +
                   "'1922','1923','1924','1925','1926','1934','1949','2101','2102','2111','2113','2116','2117','2118','2120','2121','2122'," +
                   "'2124','2125','2126','2151','2152','2154','2401','2403','2408','2409','2410','2411','2414','2415','2501','2503','2504'," +
                   "'2901','2902','2903','2904','2905','2906','2907','2908','2909','2910','2911','2912','2913','2914','2915','2916','2917'," +
                   "'2918','2919','2920','2921','2922','2923','2924','2925','2934','2949','3101','3102','3126','3127','3503','3949'";

            var comando =
                @"Select 
                    CpfCnpj AS Id,
                    Fornecedor AS Nome,
                    ValorContabil,
                    ValorIpi,
                    ValorContabilIpi
                From
                    (Select 
                        f.CpfCnpj As CpfCnpj,
                        f.RazaoSocial As Fornecedor,
                        Sum(pnf.Total) + Sum(pnf.ValorIpi) As ValorContabil,
                        Sum(pnf.ValorIpi) As ValorIpi,
                        Sum(pnf.Total) As ValorContabilIpi
                    From produtos_nf pnf
                        Left Join nota_fiscal nf ON (pnf.IdNf = nf.IdNf)
                        Left Join natureza_operacao nat ON (pnf.idnaturezaoperacao = nat.idnaturezaoperacao)
                        Left Join cfop c ON (nat.idCfop = c.idCfop)
                        Left Join produto p ON (pnf.IdProd = p.IdProd)
                        Left Join produto_baixa_estoque_fiscal pbef ON (p.idProd = pbef.idProd)
                        Left Join produto prod ON (Coalesce(pbef.idProdBaixa, p.IdProd) = prod.IdProd)
                        Left Join fornecedor f On (nf.IdFornec = f.IdFornec)
                    Where nf.TipoDocumento In (?tipoDoc1, ?tipoDoc2)
                        And nf.Situacao In (?situacao1, ?situacao2)
                        And (Coalesce(nf.dataSaidaEnt, nf.dataEmissao) >= ?dataInicial
                        And Coalesce(nf.dataSaidaEnt, nf.dataEmissao) <= ?dataFinal)
                        And c.codInterno In (" + codInternoCfop + @")
                    Group By nf.idFornec) As temp
                Order By ValorContabil Desc";
            
            var consulta = new GDA.Sql.NativeQuery(comando)
                .Add("?tipoDoc1", Model.NotaFiscal.TipoDoc.Entrada)
                .Add("?tipoDoc2", Model.NotaFiscal.TipoDoc.EntradaTerceiros)
                .Add("?situacao1", Model.NotaFiscal.SituacaoEnum.Autorizada)
                .Add("?situacao2", Model.NotaFiscal.SituacaoEnum.FinalizadaTerceiros)
                .Add("?dataInicial", dataEmissaoInicial.Date)
                .Add("?dataFinal", dataEmissaoFinal.Date.AddHours(23).AddMinutes(59).AddSeconds(59));


            return FormaResultado(consulta, pageSize, 
                f =>
                {
                    if (string.IsNullOrEmpty(f.Id))
                        // Adiciona o ID como exterior para CpfCnpj vazios.
                        f.Id = "EXTERIOR";

                    return f;
                });
        }

        /// <summary>
        /// Recupera as entradas dos produtos.
        /// </summary>
        /// <param name="dataEmissaoInicial">Data inicial da emissão.</param>
        /// <param name="dataEmissaoFinal">Data final da emissão.</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<RelModel.Dipj> ObtemEntradaProdutos(DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int pageSize)
        {
            var codInternoCfop = "'1101','1102','1111','1113','1116','1117','1118','1120','1121','1122','1124','1125','1126','1151'," +
                   "'1152','1154','1401','1403','1408','1409','1410','1411','1414','1415','1501','1503','1504','1901','1902','1903','1904'," +
                   "'1905','1906','1907','1908','1909','1910','1911','1912','1913','1914','1915','1916','1917','1918','1919','1920','1921'," +
                   "'1922','1923','1924','1925','1926','1934','1949','2101','2102','2111','2113','2116','2117','2118','2120','2121','2122'," +
                   "'2124','2125','2126','2151','2152','2154','2401','2403','2408','2409','2410','2411','2414','2415','2501','2503','2504'," +
                   "'2901','2902','2903','2904','2905','2906','2907','2908','2909','2910','2911','2912','2913','2914','2915','2916','2917'," +
                   "'2918','2919','2920','2921','2922','2923','2924','2925','2934','2949','3101','3102','3126','3127','3503','3949'";

            var comando =
                @"Select 
                    Ncm AS Id,
                    Mercadoria AS Nome,
                    ValorContabil,
                    ValorIpi,
                    ValorContabilIpi
                From
                    (Select 
                        prod.Ncm As Ncm,
                        prod.Descricao As Mercadoria,
                        Sum(pnf.Total) + Sum(pnf.ValorIpi) As ValorContabil,
                        Sum(pnf.ValorIpi) As ValorIpi,
                        Sum(pnf.Total) As ValorContabilIpi
                    From produtos_nf pnf
                        Left Join nota_fiscal nf ON (pnf.IdNf = nf.IdNf)
                        Left Join natureza_operacao nat ON (pnf.idnaturezaoperacao = nat.idnaturezaoperacao)
                        Left Join cfop c ON (nat.idCfop = c.idCfop)
                        Left Join produto p ON (pnf.IdProd = p.IdProd)
                        Left Join produto_baixa_estoque_fiscal pbef ON (p.idProd = pbef.idProd)
                        Left Join produto prod ON (Coalesce(pbef.idProdBaixa, p.IdProd) = prod.IdProd)
                    Where nf.TipoDocumento In (?tipoDoc1 , ?tipoDoc2)
                        And nf.Situacao In (?situacao1, ?situacao2)
                        And (Coalesce(nf.dataSaidaEnt, nf.dataEmissao) >= ?dataInicial
                        And Coalesce(nf.dataSaidaEnt, nf.dataEmissao) <= ?dataFinal)
                        And c.codInterno In (" + codInternoCfop + @")
                    Group By prod.IdProd) As temp
                Order By ValorContabil Desc";

            var consulta = new GDA.Sql.NativeQuery(comando)
                .Add("?tipoDoc1", Model.NotaFiscal.TipoDoc.Entrada)
                .Add("?tipoDoc2", Model.NotaFiscal.TipoDoc.EntradaTerceiros)
                .Add("?situacao1", Model.NotaFiscal.SituacaoEnum.Autorizada)
                .Add("?situacao2", Model.NotaFiscal.SituacaoEnum.FinalizadaTerceiros)
                .Add("?dataInicial", dataEmissaoInicial.Date)
                .Add("?dataFinal", dataEmissaoFinal.Date.AddHours(23).AddMinutes(59).AddSeconds(59));

            return FormaResultado(consulta, pageSize,
                f =>
                {
                    if (string.IsNullOrEmpty(f.Id))
                        f.Id = "NÃO POSSUI";

                    return f;
                });
        }

        /// <summary>
        /// Recupera as saídas dos clientes.
        /// </summary>
        /// <param name="dataEmissaoInicial">Data inicial da emissão.</param>
        /// <param name="dataEmissaoFinal">Data final da emissão.</param>
        /// <param name="pageSize">Tamanho da página de dados que será recuperadas.</param>
        /// <returns></returns>
        public IEnumerable<RelModel.Dipj> ObtemSaidaClientes
            (DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int pageSize)
        {
            var codInternoCfop = "'5101','5102','5103','5104','5105','5106','5109','5110','5111','5112','5113','5114','5115','5116','5117'," +
                "'5118','5119','5120','5122','5123','5124','5125','5151','5152','5155','5156','5401','5402','5403','5405','5408','5409'," +
                "'5414','5415','5501','5502','5503','5667','5901','5902','5903','5904','5905','5906','5907','5908','5909','5910','5911'," +
                "'5912','5913','5914','5915','5916','5917','5918','5919','5920','5921','5922','5923','5924','5925','5934','5949','6101'," +
                "'6102','6103','6104','6105','6106','6107','6108','6109','6110','6111','6112','6113','6114','6115','6116','6117','6118'," +
                "'6119','6120','6122','6123','6124','6125','6151','6152','6155','6156','6401','6402','6403','6404','6408','6409','6502'," +
                "'6503','6505','6667','6901','6902','6903','6904','6905','6906','6907','6908','6909','6910','6911','6912','6913','6914'," +
                "'6915','6916','6917','6918','6919','6920','6921','6922','6923','6924','6925','6934','6949','7101','7102','7105','7106'," +
                "'7127','7501','7667','7949'";

            var comando =
                @"Select 
                    CpfCnpj AS Id,
                    Cliente AS Nome,
                    ValorContabil,
                    ValorIpi,
                    ValorContabilIpi
                From
                    (Select 
                        c.Cpf_Cnpj As CpfCnpj,
                        c.nome As Cliente,
                        Sum(pnf.Total) + Sum(pnf.ValorIpi) + Sum(pnf.valorIcmsSt) As ValorContabil,
                        Sum(pnf.ValorIpi) As ValorIpi,
                        Sum(pnf.Total) + Sum(pnf.valorIcmsSt) As ValorContabilIpi
                    From produtos_nf pnf
                        Left Join nota_fiscal nf ON (pnf.IdNf = nf.IdNf)
                        Left Join natureza_operacao nat ON (pnf.idnaturezaoperacao = nat.idnaturezaoperacao)
                        Left Join cfop cf ON (nat.idCfop = cf.idCfop)
                        Left Join produto p ON (pnf.IdProd = p.IdProd)
                        Left Join produto_baixa_estoque_fiscal pbef ON (p.idProd = pbef.idProd)
                        Left Join produto prod ON (Coalesce(pbef.idProdBaixa, p.IdProd) = prod.IdProd)
                        Left Join cliente c On (nf.IdCliente = c.Id_Cli)
                    Where nf.TipoDocumento In (?tipoDoc)
                        And nf.Situacao In (?situacao)
                        And (nf.DataEmissao >= ?dataInicial
                        And nf.DataEmissao <= ?dataFinal)
                        And cf.codInterno In (" + codInternoCfop + @")
                    Group By nf.idCliente) As temp
                Order By ValorContabil Desc";

            var consulta = new GDA.Sql.NativeQuery(comando)
                .Add("?tipoDoc", Model.NotaFiscal.TipoDoc.Saída)
                .Add("?situacao", Model.NotaFiscal.SituacaoEnum.Autorizada)
                .Add("?dataInicial", dataEmissaoInicial.Date)
                .Add("?dataFinal", dataEmissaoFinal.Date.AddHours(23).AddMinutes(59).AddSeconds(59));

            return FormaResultado(consulta, pageSize,
                f =>
                {
                    if (string.IsNullOrEmpty(f.Id))
                        // Adiciona o ID como exterior para CpfCnpj vazios.
                        f.Id = "EXTERIOR";

                    return f;
                });
        }

        /// <summary>
        /// Recupera as saídas dos produtos.
        /// </summary>
        /// <param name="dataEmissaoInicial">Data inicial da emissão.</param>
        /// <param name="dataEmissaoFinal">Data final da emissão.</param>
        /// <param name="pageSize">Tamanho da página de dados.</param>
        /// <returns></returns>
        public IEnumerable<RelModel.Dipj> ObtemSaidaProdutos
            (DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int pageSize)
        {
            var codInternoCfop = "'5101','5102','5103','5104','5105','5106','5109','5110','5111','5112','5113','5114','5115','5116','5117'," +
                "'5118','5119','5120','5122','5123','5124','5125','5151','5152','5155','5156','5401','5402','5403','5405','5408','5409'," +
                "'5414','5415','5501','5502','5503','5667','5901','5902','5903','5904','5905','5906','5907','5908','5909','5910','5911'," +
                "'5912','5913','5914','5915','5916','5917','5918','5919','5920','5921','5922','5923','5924','5925','5934','5949','6101'," +
                "'6102','6103','6104','6105','6106','6107','6108','6109','6110','6111','6112','6113','6114','6115','6116','6117','6118'," +
                "'6119','6120','6122','6123','6124','6125','6151','6152','6155','6156','6401','6402','6403','6404','6408','6409','6502'," +
                "'6503','6505','6667','6901','6902','6903','6904','6905','6906','6907','6908','6909','6910','6911','6912','6913','6914'," +
                "'6915','6916','6917','6918','6919','6920','6921','6922','6923','6924','6925','6934','6949','7101','7102','7105','7106'," +
                "'7127','7501','7667','7949'";

            var comando =
                @"Select 
                    Ncm AS Id,
                    Mercadoria AS Nome,
                    ValorContabil,
                    ValorIpi,
                    ValorContabilIpi
                From
                    (Select 
                        prod.Ncm Ncm,
                        prod.Descricao As Mercadoria,
                        Sum(pnf.Total + pnf.ValorIpi + pnf.valorIcmsSt) As ValorContabil,
                        Sum(pnf.ValorIpi) As ValorIpi,
                        Sum(pnf.Total) + Sum(pnf.valorIcmsSt) As ValorContabilIpi
                    From produtos_nf pnf
                        Left Join nota_fiscal nf ON (pnf.IdNf = nf.IdNf)
                        Left Join natureza_operacao nat ON (pnf.idnaturezaoperacao = nat.idnaturezaoperacao)
                        Left Join cfop c ON (nat.idCfop = c.idCfop)
                        Left Join produto p ON (pnf.IdProd = p.IdProd)
                        Left Join produto_baixa_estoque_fiscal pbef ON (p.idProd = pbef.idProd)
                        Left Join produto prod ON (Coalesce(pbef.idProdBaixa, p.IdProd) = prod.IdProd)
                    Where nf.TipoDocumento In (?tipoDoc)
                        And nf.Situacao In (?situacao)
                        And (nf.DataEmissao >= ?dataInicial
                        And nf.DataEmissao <= ?dataFinal)
                        And c.codInterno In (" + codInternoCfop + @")
                    Group By prod.IdProd) As temp
                Order By ValorContabil Desc";

            var consulta = new GDA.Sql.NativeQuery(comando)
                .Add("?tipoDoc", Model.NotaFiscal.TipoDoc.Saída)
                .Add("?situacao", Model.NotaFiscal.SituacaoEnum.Autorizada)
                .Add("?dataInicial", dataEmissaoInicial.Date)
                .Add("?dataFinal", dataEmissaoFinal.Date.AddHours(23).AddMinutes(59).AddSeconds(59));

            return FormaResultado(consulta, pageSize,
               f =>
               {
                   if (string.IsNullOrEmpty(f.Id))
                       f.Id = "NÃO POSSUI";

                   return f;
               });
        }

        #endregion
    }
}