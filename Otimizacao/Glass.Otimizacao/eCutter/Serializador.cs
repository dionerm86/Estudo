using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.eCutter
{
    /// <summary>
    /// Representa o serializador dos dados para a comunicação com o ecutter.
    /// </summary>
    public static class Serializador
    {
        #region Métodos Privados

        /// <summary>
        /// Recupera o tipo de material.
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        private static string ObterTipoMaterial(TipoMaterial tipo)
        {
            switch(tipo)
            {
                case TipoMaterial.Laminado:
                    return "Laminated";
                case TipoMaterial.Monolitico:
                    return "Monolithic";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Serializa os dados da peça padrão.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="pecaPadrao"></param>
        private static void Serializar(System.Xml.XmlWriter writer, IPecaPadrao pecaPadrao)
        {
            writer.WriteStartAttribute("code");
            writer.WriteValue(pecaPadrao.Codigo);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("quantity");
            writer.WriteValue(pecaPadrao.Qtde);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("xDimension");
            writer.WriteValue(pecaPadrao.Largura);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("yDimension");
            writer.WriteValue(pecaPadrao.Altura);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("grinding");
            writer.WriteValue(pecaPadrao.Aresta);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("canRotate");
            writer.WriteValue(pecaPadrao.PodeRotacionar);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("x1Grinding");
            writer.WriteValue(pecaPadrao.ArestaX1);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("y1Grinding");
            writer.WriteValue(pecaPadrao.ArestaY1);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("x2Grinding");
            writer.WriteValue(pecaPadrao.ArestaX2);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("y1Grinding");
            writer.WriteValue(pecaPadrao.ArestaY2);
            writer.WriteEndAttribute();
        }

        /// <summary>
        /// Serializa os dados da entrada do estoque.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="entrada"></param>
        private static void Serializar(System.Xml.XmlWriter writer, IEntradaEstoqueChapa entrada)
        {
            writer.WriteStartAttribute("materialCode");
            writer.WriteValue(entrada.CodigoMaterial);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("position");
            writer.WriteValue(entrada.Posicao);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("quantity");
            writer.WriteValue(entrada.Quantidade);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("xDimension");
            writer.WriteValue(entrada.Largura);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("yDimension");
            writer.WriteValue(entrada.Altura);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("rack");
            writer.WriteValue(entrada.Cavalete);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("priority");
            writer.WriteValue(entrada.Prioridade);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("crosscutType");
            writer.WriteValue(entrada.TipoCorte.ToString());
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("isWaste");
            writer.WriteValue(entrada.Retalho);
            writer.WriteEndAttribute();
        }

        /// <summary>
        /// Serializa os dados do material.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="material"></param>
        private static void Serializar(System.Xml.XmlWriter writer, IMaterial material)
        {
            writer.WriteAttributeString("code", material.Codigo);
            writer.WriteAttributeString("type", ObterTipoMaterial(material.Tipo));
            writer.WriteStartAttribute("thickness1");
            writer.WriteValue(material.Espessura1);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("thickness2");
            writer.WriteValue(material.Espessura2);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("thickness3");
            writer.WriteValue(material.Espessura3);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("thickness4");
            writer.WriteValue(material.Espessura4);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("minDistance");
            writer.WriteValue(material.DistanciaMinima);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("trimX1");
            writer.WriteValue(material.RecorteX1);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("trimY1");
            writer.WriteValue(material.RecorteY1);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("trimX2");
            writer.WriteValue(material.RecorteX2);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("trimY2");
            writer.WriteValue(material.RecorteY2);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("xCrosscut");
            writer.WriteValue(material.CorteTransversalX);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("yCrosscut");
            writer.WriteValue(material.CorteTransversalY);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("minXOffcut");
            writer.WriteValue(material.RetalhoMinX);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("minYOffcut");
            writer.WriteValue(material.RetalhoMinY);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("autoShapeTrim");
            writer.WriteValue(material.RecorteAutomaticoForma);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("autoShapeTrimAngle");
            writer.WriteValue(material.AnguloRecorteAutomaticoForma);
            writer.WriteEndAttribute();
            writer.WriteStartElement("Description");
            writer.WriteValue(material.Descricao);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Recupera o tipo de mensagem.
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        private static string ObterTipoMensagem(TipoMensagemTransacao tipo)
        {
            switch(tipo)
            {
                case TipoMensagemTransacao.Informacao:
                    return "Information";
                case TipoMensagemTransacao.Alerta:
                    return "Warning";
                case TipoMensagemTransacao.Erro:
                    return "Error";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Serializa os dados da mensgem.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="mensagem"></param>
        private static void Serializar(System.Xml.XmlWriter writer, MensagemTransacao mensagem)
        {
            if (mensagem == null)
                throw new ArgumentNullException(nameof(mensagem));

            writer.WriteStartAttribute("title");
            writer.WriteValue(mensagem.Titulo);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("type");
            writer.WriteValue(ObterTipoMensagem(mensagem.Tipo));
            writer.WriteEndAttribute();

            writer.WriteStartElement("Message");
            writer.WriteValue(mensagem.Mensagem);
            writer.WriteEndElement();
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Serializa o estoque de chapas.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="estoque"></param>
        public static void Serializar(System.Xml.XmlWriter writer, IEstoqueChapa estoque)
        {
            writer.WriteStartElement("Materials");
            foreach(var material in estoque.Materiais)
            {
                writer.WriteStartElement("Material");
                Serializar(writer, material);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Entries");
            foreach(var entrada in estoque.Entradas)
            {
                writer.WriteStartElement("SheetStockEntry");
                Serializar(writer, entrada);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Serializa as peças padrão.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="pecasPadrao"></param>
        public static void Serializar(System.Xml.XmlWriter writer, IEnumerable<IPecaPadrao> pecasPadrao)
        {
            writer.WriteStartElement("StandardPieceRepository");
            foreach(var pecaPadrao in pecasPadrao)
            {
                writer.WriteStartElement("StandardPiece");
                Serializar(writer, pecaPadrao);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Serializa o resultado da operação de salvar a transação.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="resultado"></param>
        public static void Serializar(System.Xml.XmlWriter writer, ResultadoSalvarTransacao resultado)
        {
            writer.WriteStartElement("Success");
            writer.WriteValue(resultado.Sucesso);
            writer.WriteEndElement();

            writer.WriteStartElement("RedirectUri");
            writer.WriteValue(resultado.UriRedirecionar?.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Messages");
            
            foreach(var mensagem in resultado.Mensagens)
            {
                writer.WriteStartElement("ProtocolTransactionMessage");
                Serializar(writer, mensagem);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        #endregion
    }
}
