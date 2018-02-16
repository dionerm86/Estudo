using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class ComissaoConfigDAO : BaseDAO<Model.ComissaoConfig, ComissaoConfigDAO>
    {
        //private ComissaoConfigDAO() { }

        #region M�todos privados

        /// <summary>
        /// Retorna o n�mero da faixa em que se encaixa o valor do pedido.
        /// </summary>
        /// <param name="valor">O valor do pedido.</param>
        /// <param name="comissao">O objeto com os dados da comiss�o.</param>
        /// <returns>O n�mero da faixa.</returns>
        private string GetNumeroFaixa(decimal valor, Model.ComissaoConfig comissao)
        {
            /* Chamado 41319. */
            if (valor < comissao.FaixaUm || PedidoConfig.Comissao.PerComissaoPedido)
                return "Um";
            else if (valor < comissao.FaixaDois)
                return "Dois";
            else if (valor < comissao.FaixaTres)
                return "Tres";
            else if (valor < comissao.FaixaQuatro)
                return "Quatro";
            else
                return "Cinco";
        }

        /// <summary>
        /// Retorna o n�mero da faixa em que se encaixa o valor do pedido.
        /// </summary>
        /// <param name="valor">O valor do pedido.</param>
        /// <param name="comissao">O objeto com os dados da comiss�o.</param>
        /// <returns>O n�mero da faixa.</returns>
        private int GetNumeroFaixaInt(decimal valor, Model.ComissaoConfig comissao)
        {
            /* Chamado 41319. */
            if (valor < comissao.FaixaUm || PedidoConfig.Comissao.PerComissaoPedido)
                return 1;
            else if (valor < comissao.FaixaDois)
                return 2;
            else if (valor < comissao.FaixaTres)
                return 3;
            else if (valor < comissao.FaixaQuatro)
                return 4;
            else
                return 5;
        }

        /// <summary>
        /// Retorna o valor da comiss�o.
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="comissao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        internal decimal GetValorComissao(decimal valor, Model.ComissaoConfig comissao, uint idPedido, Pedido.TipoComissao tipoComissao)
        {
            decimal valorComissao = 0;

            // Recupera as informa��es da faixa que o valor se encontra
            string numFaixa = GetNumeroFaixa(valor, comissao);
            int numFaixaInt = GetNumeroFaixaInt(valor, comissao);

            // Recupera informa��es do pedido
            uint idCli = Configuracoes.ComissaoConfig.UsarPercComissaoCliente ? PedidoDAO.Instance.ObtemIdCliente(idPedido) : 0;
            uint idFuncPedido = Configuracoes.ComissaoConfig.UsarPercComissaoCliente ? PedidoDAO.Instance.ObtemIdFunc(idPedido) : 0;
            uint? idFuncCli = Configuracoes.ComissaoConfig.UsarPercComissaoCliente ? ClienteDAO.Instance.ObtemIdFunc(idCli) : null;
            float percComissao = Configuracoes.ComissaoConfig.UsarPercComissaoCliente ? ClienteDAO.Instance.ObtemPercComissaoFunc(idCli) : 0;
            float percComissaoPed = PedidoDAO.Instance.ObtemPercComissaoAdmin(idPedido);

            if (tipoComissao != Pedido.TipoComissao.Instalador && PedidoConfig.Comissao.PerComissaoPedido && percComissaoPed > 0)
            {
                // Usa o percentual de comiss�o do pedido
                valorComissao = valor * (decimal)(percComissaoPed / 100);

                // Se a empresa trabalha com desconto por percentual, calcula o valor da comiss�o do pedido subtraindo o percentual de desconto.
                if (!Configuracoes.ComissaoConfig.DescontarComissaoPerc)
                {
                    var descontoComissao = valorComissao * ((decimal)DescontoComissaoConfigDAO.Instance.GetDescontoComissaoPerc(comissao.IdFunc, idPedido) / 100);
                    valorComissao -= valorComissao >= descontoComissao ? descontoComissao : 0;
                }
            }
            else if (tipoComissao != Pedido.TipoComissao.Instalador && Configuracoes.ComissaoConfig.UsarPercComissaoCliente && percComissao > 0)
            {
                // Usa o percentual de comiss�o do cliente
                valorComissao = valor * (decimal)(percComissao / 100);
            }
            /* Chamado 41319. */
            //else if (comissao.PercUnico)
            else if (comissao.PercUnico || PedidoConfig.Comissao.PerComissaoPedido)
            {
                // Usa o percentual de comiss�o da faixa, considerando o percentual de desconto do pedido
                float percFaixa = (float)typeof(Model.ComissaoConfig).GetProperty("PercFaixa" + numFaixa.ToString()).GetValue(comissao, null);
                percFaixa = Configuracoes.ComissaoConfig.DescontarComissaoPerc ? percFaixa - DescontoComissaoConfigDAO.Instance.GetDescontoComissaoPerc(comissao.IdFunc, idPedido) : percFaixa;

                if (percFaixa < 0)
                    percFaixa = 0;

                valorComissao = valor * (decimal)(percFaixa / 100);
            }
            else
            {
                // Calcula o percentual de comiss�o aplicando cada valor � sua respectiva faixa de comiss�o
                decimal valorTemp = valor;
                decimal valorFaixaAnt = 0;

                for (int i = 1; i <= numFaixaInt; i++)
                {
                    string descrNumFaixa = i == 1 ? "Um" : i == 2 ? "Dois" : i == 3 ? "Tres" : i == 4 ? "Quatro" : "Cinco";
                    decimal valorFaixa = (decimal)typeof(Model.ComissaoConfig).GetProperty("Faixa" + descrNumFaixa).GetValue(comissao, null);
                    decimal valorFaixaCalc = valorFaixa - valorFaixaAnt;
                    valorFaixaAnt = valorFaixa;

                    // Usa o percentual de comiss�o da faixa, considerando o percentual de desconto do pedido
                    float percFaixa = (float)typeof(Model.ComissaoConfig).GetProperty("PercFaixa" + descrNumFaixa).GetValue(comissao, null);
                    percFaixa = Configuracoes.ComissaoConfig.DescontarComissaoPerc && idPedido > 0 ? percFaixa - DescontoComissaoConfigDAO.Instance.GetDescontoComissaoPerc(comissao.IdFunc, idPedido) : percFaixa;

                    if (percFaixa < 0)
                        percFaixa = 0;

                    if (i < numFaixaInt)
                    {
                        // Calcula o valor da comiss�o considerando o valor m�ximo da faixa, e seu percentual
                        valorComissao += valorFaixaCalc * (decimal)(percFaixa / 100);
                        valorTemp -= valorFaixaCalc;
                    }
                    else
                    {
                        // Calcula o valor da comiss�o considerando o valor restante (n�o calculado ainda)
                        valorComissao += valorTemp * (decimal)(percFaixa / 100);
                    }
                }
            }

            return valorComissao;
        }

        #endregion

        #region M�todos sobrescritos

        public override void InsertOrUpdate(Model.ComissaoConfig objUpdate)
        {
            if (objUpdate.IdFunc == 0)
                objUpdate.IdFunc = null;

            if (objUpdate.IdComissaoConfig == 0)
                Insert(objUpdate);
            else
            {
                LogAlteracaoDAO.Instance.LogComissaoConfig(objUpdate);
                Update(objUpdate);
            }
        }

        #endregion

        #region M�todos de suporte

        /// <summary>
        /// Verifica se o funcion�rio s� possui 1 faixa de comiss�o.
        /// </summary>
        /// <param name="idFunc">O identificador do funcion�rio.</param>
        /// <returns>Verdadeiro, se o funcion�rio s� possuir 1 faixa de comiss�o.</returns>
        public bool IsFaixaUnica(uint idFunc)
        {
            Model.ComissaoConfig config = GetComissaoConfig(idFunc);
            return (config.FaixaDois + config.FaixaTres + config.FaixaQuatro + config.FaixaCinco == 0) ||
                (config.PercFaixaDois + config.PercFaixaTres + config.PercFaixaQuatro + config.PercFaixaCinco == 0);
        }

        /// <summary>
        /// Retorna a configura��o de comiss�o para um funcion�rio.
        /// </summary>
        /// <param name="func">O identificador do funcion�rio. Pode ser NULL para retornar o valor padr�o.</param>
        /// <returns>Um objeto ComissaoConfig com os dados da comiss�o para um funcion�rio.</returns>
        public Model.ComissaoConfig GetComissaoConfig(uint? idFunc)
        {
            var comissaoConfig = GetComissaoConfig(null, idFunc);
            return comissaoConfig;
        }

        /// <summary>
        /// Retorna a configura��o de comiss�o para um funcion�rio.
        /// </summary>
        /// <param name="func">O identificador do funcion�rio. Pode ser NULL para retornar o valor padr�o.</param>
        /// <returns>Um objeto ComissaoConfig com os dados da comiss�o para um funcion�rio.</returns>
        public Model.ComissaoConfig GetComissaoConfig(GDASession sessao, uint? idFunc)
        {
            // Busca a configura��o para o funcion�rio (ou geral, se o par�metro for null)
            string where = idFunc > 0 ? "=" + idFunc.Value.ToString() : " is null";
            List<Model.ComissaoConfig> config = objPersistence.LoadData(sessao, "select * from comissao_config where idFunc" + where);

            if (config.Count == 0)
            {
                Model.ComissaoConfig c;
                
                // Tenta buscar a configura��o geral, se o par�metro n�o for null
                if (idFunc != null)
                {
                    c = GetComissaoConfig(sessao, null);
                    c.IdFunc = idFunc;
                }

                // Cria um novo objeto para retorno
                else
                {
                    c = new Model.ComissaoConfig();
                    c.IdFunc = idFunc;
                    c.PercUnico = true;
                }
                
                config.Add(c);
            }

            return config[0];
        }

        #endregion

        /// <summary>
        /// Retorna o valor da comiss�o.
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="tipoFunc"></param>
        /// <param name="idFunc"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public decimal GetComissaoValor(decimal valor, uint idFunc, uint idPedido, Pedido.TipoComissao tipoComissao)
        {
            Model.ComissaoConfig config = GetComissaoConfig(idFunc);
            return GetValorComissao(valor, config, idPedido, tipoComissao);
        }

        public decimal GetComissaoValor(decimal valor, uint idFunc, uint idPedido, Glass.Data.Model.ComissaoConfig config, Pedido.TipoComissao tipoComissao)
        {
            return GetValorComissao(valor, config, idPedido, tipoComissao);
        }

        /// <summary>
        /// Retorna o valor correspondente � comiss�o do funcion�rio.
        /// </summary>
        /// <param name="idFunc">O identificador do funcion�rio.</param>
        /// <param name="idsPedidos">Os IDs dos pedidos.</param>
        /// <returns>O valor da comiss�o.</returns>
        public decimal GetComissaoValor(Pedido.TipoComissao tipoFunc, uint idFunc, string dataIni, string dataFim, params uint[] idsPedidos)
        {
            if (idsPedidos.Length == 0)
                return 0;

            string idsPedidosStr = "";
            foreach (uint id in idsPedidos)
                idsPedidosStr += "," + id;

            return GetComissaoValor(tipoFunc, idFunc, dataIni, dataFim, idsPedidosStr.Substring(1));
        }

        /// <summary>
        /// Retorna o valor correspondente � comiss�o do funcion�rio.
        /// </summary>
        /// <param name="idFunc">O identificador do funcion�rio.</param>
        /// <param name="idsPedidos">Os IDs dos pedidos.</param>
        /// <returns>O valor da comiss�o.</returns>
        public decimal GetComissaoValor(Pedido.TipoComissao tipoFunc, uint idFunc, string dataIni, string dataFim, string idsPedidos)
        {
            if (string.IsNullOrEmpty(idsPedidos))
                return 0;

            decimal valorComissaoRetorno = 0;
            Model.ComissaoConfig comissao = GetComissaoConfig(idFunc);

            var pedidos = PedidoDAO.Instance.GetByString(idsPedidos, idFunc, tipoFunc, dataIni, dataFim);

            if (tipoFunc == Pedido.TipoComissao.Gerente)
            {
                valorComissaoRetorno = ComissaoConfigGerenteDAO.Instance.GetValorComissaoPedidos(pedidos, idFunc, dataIni, dataFim);
            }
            else if (PedidoConfig.Comissao.PerComissaoPedido)
            {
                // Calcula o valor da comiss�o dos pedidos, considerando o percentual de desconto de cada pedido,
                // caso a empresa trabalhe dessa forma
                foreach (Pedido p in pedidos)
                    valorComissaoRetorno += p.ValorComissaoPagar;
            }
            else
                /* Chamado 54760. */
                foreach (var pedido in pedidos)
                    valorComissaoRetorno += GetComissaoValor(pedido.ValorBaseCalcComissao, idFunc, pedido.IdPedido, Pedido.TipoComissao.Funcionario);

            return valorComissaoRetorno;
        }

        /// <summary>
        /// Retorna o percentual total da comiss�o do funcion�rio.
        /// </summary>
        /// <param name="idFunc">O identificador do funcion�rio.</param>
        /// <param name="idsPedidos">Os IDs dos pedidos.</param>
        /// <returns>A porcentagem correspondente � comiss�o.</returns>
        public decimal GetComissaoPerc(Pedido.TipoComissao tipoFunc, uint idFunc, string dataIni, string dataFim, params uint[] idsPedidos)
        {
            if (idsPedidos.Length == 0)
                return 0;

            string idsPedidosStr = "";
            foreach (uint id in idsPedidos)
                idsPedidosStr += "," + id;

            return GetComissaoPerc(tipoFunc, idFunc, dataIni, dataFim, idsPedidosStr.Substring(1));
        }

        /// <summary>
        /// Retorna o percentual total da comiss�o do funcion�rio.
        /// </summary>
        /// <param name="idFunc">O identificador do funcion�rio.</param>
        /// <param name="idsPedidos">Os IDs dos pedidos.</param>
        /// <returns>A porcentagem correspondente � comiss�o.</returns>
        public decimal GetComissaoPerc(Pedido.TipoComissao tipoFunc, uint idFunc, string dataIni, string dataFim, string idsPedidos)
        {
            // Soma o total usado como base de c�lculo de comiss�o
            decimal total = 0;
            foreach (Pedido p in PedidoDAO.Instance.GetByString(idsPedidos, idFunc, tipoFunc, dataIni, dataFim))
                total += p.ValorBaseCalcComissao;

            // Calcula o valor da comiss�o
            decimal valorComissao = GetComissaoValor(tipoFunc, idFunc, dataIni, dataFim, idsPedidos);

            // Retorna o percentual de comiss�o
            return valorComissao * 100 / total;
        }

        #region Comissao Contas Recebidas

        public decimal GetComissaoPercContasRecebidas(GDASession sessao, decimal valorBaseCalc, uint idFunc)
        {
            // Calcula o valor da comiss�o
            decimal valorComissao = GetValorComissaoContasRecebidas(sessao, valorBaseCalc, idFunc);

            // Retorna o percentual de comiss�o
            return (valorComissao * 100 / valorBaseCalc) / 100;
        }

        /// <summary>
        /// Retorna o valor da comiss�o das contas recebidas
        /// </summary>
        /// <param name="valorBaseCalc"></param>
        /// <param name="idCliente"></param>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        private decimal GetValorComissaoContasRecebidas(GDASession sessao, decimal valorBaseCalc, uint idFunc)
        {
            decimal valorComissao = 0;

            Model.ComissaoConfig comissao = GetComissaoConfig(sessao, (uint)idFunc);

            // Recupera as informa��es da faixa que o valor se encontra
            string numFaixa = GetNumeroFaixa(valorBaseCalc, comissao);
            int numFaixaInt = GetNumeroFaixaInt(valorBaseCalc, comissao);
            
            /* Chamado 41319 e 41411. */
            //if (comissao.PercUnico)
            if (comissao.PercUnico || PedidoConfig.Comissao.PerComissaoPedido)
            {
                // Usa o percentual de comiss�o da faixa, considerando o percentual de desconto do pedido
                float percFaixa = (float)typeof(Model.ComissaoConfig).GetProperty("PercFaixa" + numFaixa.ToString()).GetValue(comissao, null);

                if (percFaixa < 0)
                    percFaixa = 0;

                valorComissao = valorBaseCalc * (decimal)(percFaixa / 100);
            }
            else
            {
                // Calcula o percentual de comiss�o aplicando cada valor � sua respectiva faixa de comiss�o
                decimal valorTemp = valorBaseCalc;
                decimal valorFaixaAnt = 0;

                for (int i = 1; i <= numFaixaInt; i++)
                {
                    string descrNumFaixa = i == 1 ? "Um" : i == 2 ? "Dois" : i == 3 ? "Tres" : i == 4 ? "Quatro" : "Cinco";
                    decimal valorFaixa = (decimal)typeof(Model.ComissaoConfig).GetProperty("Faixa" + descrNumFaixa).GetValue(comissao, null);
                    decimal valorFaixaCalc = valorFaixa - valorFaixaAnt;
                    valorFaixaAnt = valorFaixa;

                    // Usa o percentual de comiss�o da faixa, considerando o percentual de desconto do pedido
                    float percFaixa = (float)typeof(Model.ComissaoConfig).GetProperty("PercFaixa" + descrNumFaixa).GetValue(comissao, null);

                    if (percFaixa < 0)
                        percFaixa = 0;

                    if (i < numFaixaInt)
                    {
                        // Calcula o valor da comiss�o considerando o valor m�ximo da faixa, e seu percentual
                        valorComissao += valorFaixaCalc * (decimal)(percFaixa / 100);
                        valorTemp -= valorFaixaCalc;
                    }
                    else
                    {
                        // Calcula o valor da comiss�o considerando o valor restante (n�o calculado ainda)
                        valorComissao += valorTemp * (decimal)(percFaixa / 100);
                    }
                }
            }

            return valorComissao;
        }

        #endregion
    }
}