namespace Glass.Data.Model
{
    public interface IDadosChapaVidro
    {
        bool ProdutoPossuiChapaVidro();
        int AlturaMinimaChapaVidro();
        int AlturaChapaVidro();
        int LarguraMinimaChapaVidro();
        int LarguraChapaVidro();
        float PercentualAcrescimoM2ChapaVidro(float m2);
    }
}
