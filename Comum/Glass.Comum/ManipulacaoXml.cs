using System.Xml;

namespace Glass
{
    public static class ManipulacaoXml
    {
        #region Tratamento de XML

        /// <summary>
        /// Criar um novo nodo e adiciona no "repos" com os valores passados
        /// </summary>
        /// <param name="doc">XmlDocument</param>
        /// <param name="repos">XmlElement Que receberá um novo nodo</param>
        /// <param name="name">Nome do nodo</param>
        /// <param name="value">Valor do nodo</param>
        public static void SetNode(XmlDocument doc, XmlElement repos, string name, string value)
        {
            XmlElement xElem = doc.CreateElement(name);
            xElem.InnerText = value;

            repos.AppendChild(xElem);
        }

        #endregion
    }
}
