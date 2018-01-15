using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class DescontoComissaoConfigDAO : BaseDAO<DescontoComissaoConfig, DescontoComissaoConfigDAO>
    {
        //private DescontoComissaoConfigDAO() { }

        #region Métodos privados

        /// <summary>
        /// Retorna o número da faixa em que se encaixa o valor do pedido.
        /// </summary>
        /// <param name="percDescontoPedido">O percentual de desconto do pedido.</param>
        /// <param name="descontoComissao">O objeto com os dados do desconto da comissão.</param>
        /// <returns>O número da faixa.</returns>
        private string GetNumeroFaixa(decimal percDescontoPedido, DescontoComissaoConfig descontoComissao)
        {
            if (percDescontoPedido < descontoComissao.FaixaUm)
                return "Um";
            else if (percDescontoPedido < descontoComissao.FaixaDois)
                return "Dois";
            else if (percDescontoPedido < descontoComissao.FaixaTres)
                return "Tres";
            else if (percDescontoPedido < descontoComissao.FaixaQuatro)
                return "Quatro";
            else
                return "Cinco";
        }

        /// <summary>
        /// Retorna o número da faixa em que se encaixa o valor do pedido.
        /// </summary>
        /// <param name="percDescontoPedido">O percentual de desconto do pedido.</param>
        /// <param name="descontoComissao">O objeto com os dados do desconto da comissão.</param>
        /// <returns>O número da faixa.</returns>
        private int GetNumeroFaixaInt(decimal percDescontoPedido, DescontoComissaoConfig descontoComissao)
        {
            if (percDescontoPedido < descontoComissao.FaixaUm)
                return 1;
            else if (percDescontoPedido < descontoComissao.FaixaDois)
                return 2;
            else if (percDescontoPedido < descontoComissao.FaixaTres)
                return 3;
            else if (percDescontoPedido < descontoComissao.FaixaQuatro)
                return 4;
            else
                return 5;
        }

        #endregion

        #region Métodos sobrescritos

        public override void InsertOrUpdate(DescontoComissaoConfig objUpdate)
        {
            if (objUpdate.IdFunc == 0)
                objUpdate.IdFunc = null;

            if (objUpdate.IdDescontoComissaoConfig == 0)
               GDAOperations.Insert(objUpdate);
            else
                GDAOperations.Update(objUpdate);
        }

        #endregion

        /// <summary>
        /// Retorna a configuração de desconto da comissão para um funcionário.
        /// </summary>
        /// <param name="func">O identificador do funcionário. Pode ser NULL para retornar o valor padrão.</param>
        /// <returns>Um objeto DescontoComissaoConfig com os dados do desconto da comissão para um funcionário.</returns>
        public DescontoComissaoConfig GetDescontoComissaoConfig(uint? idFunc)
        {
            // Busca a configuração de desconto do funcionário (ou padrão, se o parâmetro for null)
            string where = idFunc > 0 ? "=" + idFunc.Value.ToString() : " is null";
            List<DescontoComissaoConfig> config = objPersistence.LoadData("select * from desconto_comissao_config where idFunc" + where);

            if (config.Count == 0)
            {
                DescontoComissaoConfig d;

                // Tenta buscar a configuração padrão
                if (idFunc != null)
                {
                    d = GetDescontoComissaoConfig(null);
                    d.IdFunc = idFunc;
                }

                // Cria um objeto para retorno
                else
                {
                    d = new DescontoComissaoConfig();
                    d.IdFunc = idFunc;
                }

                config.Add(d);
            }

            return config[0];
        }

        /// <summary>
        /// Retorna a porcentagem correspondente ao desconto da comissão do funcionário.
        /// </summary>
        /// <param name="idFunc">O identificador do funcionário. Pode ser NULL para retornar o valor padrão.</param>
        /// <param name="idPedido">O ID do pedido.</param>
        /// <returns>O valor do desconto da comissão.</returns>
        public float GetDescontoComissaoPerc(uint? idFunc, uint idPedido)
        {
            if (idPedido == 0)
                return 0;

            DescontoComissaoConfig comissao = GetDescontoComissaoConfig(idFunc);

            // Recupera o valor descontado no pedido
            Pedido p = PedidoDAO.Instance.GetForTotalBruto(idPedido);
            decimal valorDescontoPedido = p.DescontoTotal;

            if (p.TotalSemDesconto == 0)
                return 0;

            // Calcula o percentual de desconto no pedido
            decimal percDescontoPedido = (valorDescontoPedido / p.TotalSemDesconto) * 100;

            // Se o percentual de desconto for=0 ou se o desconto do pedido for menor que 1 real.
            if (percDescontoPedido == 0 || valorDescontoPedido <= 1)
                return 0;

            // Recupera o percentual de desconto na comissão
            string numFaixa = GetNumeroFaixa(percDescontoPedido, comissao);
            return (float)typeof(DescontoComissaoConfig).GetProperty("PercFaixa" + numFaixa.ToString()).GetValue(comissao, null);
        }
    }
}
