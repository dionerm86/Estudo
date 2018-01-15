using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Pedido
{
    /// <summary>
    /// Representa o descritor da foto do pedido.
    /// </summary>
    public class FotoPedidoDescritor : Glass.Api.Pedido.IFotoPedidoDescritor
    {
        /// <summary>
        /// Identificador da foto.
        /// </summary>
        public int IdFoto { get; set; }

        /// <summary>
        /// Descricão da imagem.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Endereço da imagem.
        /// </summary>
        public string ImageUrl { get; set; }


        /// <summary>
        /// Construtor do descritor da foto.
        /// </summary>
        /// <param name="foto"></param>
        public FotoPedidoDescritor(Data.Model.IFoto foto)
        {
            IdFoto = (int)foto.IdFoto;
            Descricao = foto.Descricao;
            ImageUrl = string.Format("{0}Handlers/LoadImage.ashx?path={1}\\{2}", ServiceLocator.Current.GetInstance<Glass.Api.IConfiguracao>().EnderecoServicoImagem,
                ServiceLocator.Current.GetInstance<Glass.Api.IConfiguracao>().CaminhoFotoPedido, foto.FileName);
        }
    }
}
