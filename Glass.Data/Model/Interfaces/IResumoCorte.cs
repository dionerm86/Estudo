namespace Glass.Data.Model
{
    internal interface IResumoCorte
    {
        #region Propriedades

        uint Id { get; }
        uint IdPedido { get; }
        uint IdProd { get; }
        string CodInterno { get; }
        string DescrProduto { get; }
        float TotM { get; }
        float TotM2Calc { get; }
        decimal Total { get; }
        uint IdGrupoProd { get; }
        float Qtde { get; }
        float Espessura { get; }
        float Altura { get; }
        int Largura { get; }
        string CodAplicacao { get; }
        string CodProcesso { get; }

        #endregion
    }
}