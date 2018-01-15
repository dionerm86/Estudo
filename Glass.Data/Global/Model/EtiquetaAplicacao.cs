using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(EtiquetaAplicacaoDAO))]
	[PersistenceClass("etiqueta_aplicacao")]
	public class EtiquetaAplicacao : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDAPLICACAO", PersistenceParameterType.IdentityKey)]
        public int IdAplicacao { get; set; }

        [Log("CODINTERNO")]
        [PersistenceProperty("CODINTERNO")]
        public string CodInterno { get; set; }

        [Log("DESCRICAO")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [Log("DESTACARETIQUETA")]
        [PersistenceProperty("DESTACARETIQUETA")]
        public bool DestacarEtiqueta { get; set; }

        /// <summary>
        /// Define se a peça com esta aplicação irá gerar uma forma inexistente na exportação para o Optyway 
        /// para o conferente saber que precisa criar uma forma para a mesma, desde que a mesma não possua forma.
        /// </summary>
        [Log("GERARFORMAINEXISTENTE")]
        [PersistenceProperty("GERARFORMAINEXISTENTE")]
        public bool GerarFormaInexistente { get; set; }

        [Log("SITUACAO")]
        [PersistenceProperty("SITUACAO")]
        public Glass.Situacao Situacao { get; set; }

        /// <summary>
        /// Numero de dias minimos para entrega do pedido com produtos dessa aplicação
        /// </summary>
        [Log("DiasMinimos")]
        [PersistenceProperty("DiasMinimos")]
        public int DiasMinimos { get; set; }

        /// <summary>
        /// Tipos de pedidos
        /// </summary>
        [PersistenceProperty("TipoPedido")]
        public string TipoPedido { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrSituacao
        {
            get { return Colosoft.Translator.Translate(Situacao).Format(); }
        }

        [Log("TipoPedido")]
        public string DescricaoTipoPedido
        {
            get
            {
                var retorno = string.Empty;

                if (string.IsNullOrEmpty(TipoPedido))
                    return retorno;

                foreach (var tipo in TipoPedido.Split(','))
                {
                    switch (tipo)
                    {
                        case "5":
                            retorno += "Mão de obra Especial, ";
                            break;
                        case "3":
                            retorno += "Mão de obra, ";
                            break;
                        case "4":
                            retorno += "Produção, ";
                            break;
                        case "1":
                            retorno += "Venda, ";
                            break;
                    }
                }

                return retorno.TrimEnd(',', ' ');
            }
        }

        #endregion
    }
}