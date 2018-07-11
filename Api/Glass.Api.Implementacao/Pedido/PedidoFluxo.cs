using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Pedido
{
    public class PedidoFluxo : Glass.Api.Pedido.IPedidoFluxo
    {
        /// <summary>
        /// Gera o pedido baseado no identificador do projeto.
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <returns></returns>
        public int GerarPedido(int idProjeto)
        {
            Exception erro;
            var idPedido = Glass.Data.DAL.ProjetoDAO.Instance.GerarPedidoParceiro((uint)idProjeto, true, out erro);

            // Gerar pedido sem item.
            if (erro != null && erro.Message == "Não existem cálculos neste orçamento. Inclua pelo menos um para gerar um pedido.")
            {
               idPedido = GerarPedidoSemItem(idProjeto);
            }
            else if(erro != null)
                throw erro;

            return (int)idPedido;
        }

        /// <summary>
        /// Gera pedido sem item.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private uint GerarPedidoSemItem(int idProjeto)
        {
            var proj = Glass.Data.DAL.ProjetoDAO.Instance.GetElementByPrimaryKey(idProjeto);
            var idPedido = Glass.Data.DAL.ProjetoDAO.Instance.GerarPedido((uint)idProjeto, true, proj.TipoEntrega, true);

            // Altera os dados do pedido
            DateTime dataEntrega, dataFastDelivery;
            Glass.Data.DAL.PedidoDAO.Instance.GetDataEntregaMinima(null, Glass.Data.Helper.UserInfo.GetUserInfo.IdCliente.Value, idPedido, out dataEntrega, out dataFastDelivery);

            uint? idFuncCli = Glass.Data.DAL.ClienteDAO.Instance.ObtemIdFunc(null, Glass.Data.Helper.UserInfo.GetUserInfo.IdCliente.Value);

            var ped = Glass.Data.DAL.PedidoDAO.Instance.GetElementByPrimaryKey(idPedido);
            Glass.Data.DAL.PedidoDAO.Instance.GeraParcelaParceiro(ref ped);

            ped.GeradoParceiro = true;
            ped.DataEntrega = dataEntrega;
            ped.IdFunc = idFuncCli > 0 ? idFuncCli.Value : 0;

            ped.IdLoja = (uint)(idFuncCli > 0 ? Glass.Data.DAL.FuncionarioDAO.Instance.ObtemIdLoja(null, idFuncCli.Value) :
                Glass.Data.DAL.LojaDAO.Instance.GetCount() == 1 ? (uint)Glass.Data.DAL.LojaDAO.Instance.GetAll()[0].IdLoja : 0);

            Glass.Data.DAL.PedidoDAO.Instance.Update(ped);

            return idPedido;
        }

        /// <summary>
        /// Recuperar os pedidos do cliente.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="codCliente"></param>
        /// <param name="dtIni"></param>
        /// <param name="dtFim"></param>
        /// <param name="apenasAbertos"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<Glass.Api.Pedido.IPedidoDescritor> ObterPedidos(int idPedido, string codCliente, DateTime? dtIni, DateTime? dtFim, bool apenasAbertos,
            string sortExpression, int startRow, int pageSize)
        {
            return Glass.Data.DAL.PedidoDAO.Instance.GetListAcessoExterno((uint)idPedido, codCliente, dtIni, dtFim, apenasAbertos, sortExpression, startRow, pageSize)
                .Select(f => new Glass.Api.Implementacao.Pedido.PedidoDescritor(f)).ToList<Glass.Api.Pedido.IPedidoDescritor>();
        }

        /// <summary>
        /// Salva a foto do pedido.
        /// </summary>
        /// <param name="fotoPedido"></param>
        public Glass.Api.Pedido.IFotoPedidoDescritor SalvarFotoPedido(Glass.Api.Pedido.IFotoPedido fotoPedido)
        {
            // Cadastra a foto
            var foto = Glass.Data.Model.IFoto.Nova(Data.Model.IFoto.TipoFoto.Pedido);

            foto.Path = ServiceLocator.Current.GetInstance<IConfiguracao>().CaminhoFotoPedido;
            foto.IdParent = (uint)fotoPedido.IdPedido;
            foto.Descricao = fotoPedido.Descricao;
            foto.Extensao = string.Format(".{0}", fotoPedido.Extensao);

            // Insere entrada de foto e recupera o identificador.
            foto.IdFoto = foto.Insert();

            if (!System.IO.Directory.Exists(foto.Path))
                System.IO.Directory.CreateDirectory(foto.Path);

            var salva = ManipulacaoImagem.SalvarImagem(foto.FilePath, fotoPedido.Imagem);
            if (!salva)
                return null;

            return new Glass.Api.Implementacao.Pedido.FotoPedidoDescritor(foto);
        }

        /// <summary>
        /// Recupera as fotos do pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public IList<Glass.Api.Pedido.IFotoPedidoDescritor> ObterFotosPedido(int idPedido)
        {
            var fotos = Glass.Data.Model.IFoto.GetByParent((uint)idPedido, Data.Model.IFoto.TipoFoto.Pedido);

            return fotos.Select(f => new FotoPedidoDescritor(f)).ToList<Glass.Api.Pedido.IFotoPedidoDescritor>();
        }

        /// <summary>
        /// Apaga a foto do pedido.
        /// </summary>
        /// <param name="idFoto"></param>
        public void ApagarFotoPedido(int idFoto)
        {
            Glass.Data.DAL.FotosPedidoDAO.Instance.DeleteByPrimaryKey(idFoto);
        }

        /// <summary>
        /// Atualiza a descricao da foto pedido.
        /// </summary>
        /// <param name="idFoto"></param>
        /// <param name="descricao"></param>
        public void AtualizarFotoPedidoDescricao(int idFoto, string descricao)
        {
            var foto = Glass.Data.DAL.FotosPedidoDAO.Instance.GetElementByPrimaryKey(idFoto);
            if (foto != null && foto.Descricao != descricao)
            {
                foto.Descricao = descricao;
                Glass.Data.DAL.FotosPedidoDAO.Instance.Update(foto);
            }
        }
    }
}
