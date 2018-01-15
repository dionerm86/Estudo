using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Projeto
{
    /// <summary>
    /// Representa o resumo do projeto.
    /// </summary>
    public class ProjetoResumo : Glass.Api.Projeto.IProjetoResumo
    {
        public int IdProjeto { get; }

        public string Valor { get; }

        public string DataEntrega { get; }

        public int TipoEntrega { get; }

        public string CodProj { get; }

        public int Situacao { get; }
        
        public string DescrSituacao { get; }

        public int IdPedido { get; }

        public IList<Glass.Api.Projeto.IITemProjetoResumo> Items { get; }

        public IList<Glass.Api.Pedido.IFotoPedidoDescritor> Fotos { get; }

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projeto"></param>
        /// <param name="itemProjeto"></param>
        public ProjetoResumo(Glass.Data.Model.Projeto projeto, IList<Glass.Data.Model.ItemProjeto> items)
        {
            IdProjeto = (int)projeto.IdProjeto;
            Valor = projeto.Total.ToString("c", new CultureInfo("pt-BR"));
            TipoEntrega = projeto.TipoEntrega;
            Situacao = projeto.Situacao;
            DescrSituacao = projeto.DescrSituacao;
            DataEntrega = projeto.DataFin == null ? "Sem Data" : projeto.DataFin.Value.ToString("dd/MM/yyyy");
            CodProj = projeto.PedCli;
            IdPedido = (int)Glass.Data.DAL.ProjetoDAO.Instance.GetIdPedidoByProjeto(projeto.IdProjeto);
            Items = items.Select(f => new ItemProjetoResumo(f)).ToList<Glass.Api.Projeto.IITemProjetoResumo>();

            if(IdPedido > 0)
            {
                var fotos = Glass.Data.Model.IFoto.GetByParent((uint)IdPedido, Data.Model.IFoto.TipoFoto.Pedido);

                Fotos = fotos.Select(f => new Glass.Api.Implementacao.Pedido.FotoPedidoDescritor(f)).ToList<Glass.Api.Pedido.IFotoPedidoDescritor>();
            }
        }

        #endregion
    }
}
