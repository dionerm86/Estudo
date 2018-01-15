using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Projeto
{
    /// <summary>
    /// Representa o modelo.
    /// </summary>
    public class Modelo : Api.Projeto.IModelo
    {
        public List<Api.Projeto.IEditableItemValued> Medidas { get; }

        public List<Api.Projeto.IEditableItemValued> Pecas { get; }

        public string ImagemUrl { get; }

        /// <summary>
        /// Construtor do modelo.
        /// </summary>
        /// <param name="medidas"></param>
        /// <param name="pecas"></param>
        /// <param name="imageUrl"></param>
        public Modelo(List<Api.Projeto.IEditableItemValued> medidas, List<Api.Projeto.IEditableItemValued> pecas, string imagemUrl)
        {
            Medidas = medidas;
            Pecas = pecas;
            ImagemUrl = imagemUrl;
        }
    }

}
