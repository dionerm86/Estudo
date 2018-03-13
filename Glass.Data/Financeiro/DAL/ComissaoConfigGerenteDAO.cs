using GDA;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public sealed class ComissaoConfigGerenteDAO : BaseDAO<Model.ComissaoConfigGerente, ComissaoConfigGerenteDAO>
    {
        /// <summary>
        /// Recupera a configuração de comissão do funcionário da loja passada para o menu de configurações
        /// </summary>
        public IList<ComissaoConfigGerente> GetForConfig(uint idLoja, uint idFunc)
        {
            var retorno = GetByIdFuncIdLoja(idLoja, idFunc);

            if (retorno != null && retorno.Count > 0)
                return retorno;

            return new List<ComissaoConfigGerente>() {
                new ComissaoConfigGerente() {
                    IdLoja = idLoja,
                    IdFuncionario = idFunc
                }};
        }

        /// <summary>
        /// Recupera a configuração de comissão do funcionário da loja passada 
        /// </summary>
        public IList<ComissaoConfigGerente> GetByIdFuncIdLoja(uint idLoja, uint idFunc)
        {
            return GetByIdFuncIdLoja(null, idLoja, idFunc);
        }

        /// <summary>
        /// Recupera a configuração de comissão do funcionário da loja passada 
        /// </summary>
        public IList<ComissaoConfigGerente> GetByIdFuncIdLoja(GDASession session, uint idLoja, uint idFunc)
        {
            var sql = @"SELECT * FROM comissao_config_gerente WHERE 1";

            if (idLoja > 0)
                sql += " AND IdLoja = " + idLoja;

            if (idFunc > 0)
                sql += " AND IdFuncionario = " + idFunc;

            return objPersistence.LoadData(session, sql).ToList();
        }

        /// <summary>
        /// Recupera o somatório da comissão a ser paga pelos pedidos passados
        /// </summary>
        public decimal GetValorComissaoPedidos(Pedido[] pedidos, uint idFunc, string dataIni, string dataFim)
        {            
            decimal retorno = 0;

            foreach (var ped in pedidos)
            {
                var comissaoConfigGerente = GetByIdFuncIdLoja(ped.IdLoja, idFunc);
                if (comissaoConfigGerente == null)
                    continue;

                foreach (var comissao in comissaoConfigGerente)
                {
                    ped.ValorComissaoGerentePagar = GetComissaoGerenteValor(comissao, (uint)ped.TipoPedido, ped.ValorBaseCalcComissao);
                    retorno += ped.ValorComissaoGerentePagar;
                }
                ped.ComissaoFuncionario = Pedido.TipoComissao.Gerente;
                
            }

            return decimal.Round(retorno, 2);
        }

        /// <summary>
        /// recupera o valor da comissao a ser cobrado para o gerente do pedido passado
        /// </summary>
        public decimal GetComissaoGerenteValor(ComissaoConfigGerente comissaoConfigGerente, uint tipoPedido, decimal valor)
        {           
            switch (tipoPedido)
            {
                case (int)Pedido.TipoPedidoEnum.Venda:
                    return valor * (comissaoConfigGerente.PercentualVenda / 100);
                  
                case (int)Pedido.TipoPedidoEnum.Revenda:
                    return valor * (comissaoConfigGerente.PercentualRevenda / 100);

                case (int)Pedido.TipoPedidoEnum.MaoDeObra:
                    return valor * (comissaoConfigGerente.PercentualMaoDeObra / 100);

                case (int)Pedido.TipoPedidoEnum.MaoDeObraEspecial:
                    return valor * (comissaoConfigGerente.PercentualMaoDeObraEspecial / 100);

                default:
                    return 0;
            }           
        }

    }
}
