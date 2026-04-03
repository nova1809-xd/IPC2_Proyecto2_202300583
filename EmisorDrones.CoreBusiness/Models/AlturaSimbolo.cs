namespace EmisorDrones.CoreBusiness.Models
{
    public class AlturaSimbolo
    {
        public int ValorAltura { get; set; }

        public string Simbolo { get; set; }

        public AlturaSimbolo(int valorAltura, string simbolo)
        {
            ValorAltura = valorAltura;
            Simbolo = simbolo;
        }
    }
}
