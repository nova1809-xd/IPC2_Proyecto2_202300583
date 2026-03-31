namespace EmisorDrones.CoreBusiness.Models
{
    /// <summary>
    /// Registro segundo a segundo del estado de un dron durante ejecución
    /// </summary>
    public class RegistroDrone
    {
        /// <summary>
        /// ID del dron
        /// </summary>
        public int DronId { get; set; }

        /// <summary>
        /// Segundo en el que ocurre el registro
        /// </summary>
        public int Segundo { get; set; }

        /// <summary>
        /// Instrucción ejecutada en este segundo
        /// </summary>
        public Instruccion Instruccion { get; set; }

        /// <summary>
        /// Altura antes de ejecutar la instrucción
        /// </summary>
        public int AlturaAntes { get; set; }

        /// <summary>
        /// Altura después de ejecutar la instrucción
        /// </summary>
        public int AlturaDespues { get; set; }

        /// <summary>
        /// Constructor con parámetros
        /// </summary>
        public RegistroDrone(int dronId, int segundo, Instruccion instruccion, int alturaAntes, int alturaDespues)
        {
            this.DronId = dronId;
            this.Segundo = segundo;
            this.Instruccion = instruccion;
            this.AlturaAntes = alturaAntes;
            this.AlturaDespues = alturaDespues;
        }

        /// <summary>
        /// Retorna representación en texto del registro
        /// </summary>
        public override string ToString()
        {
            return $"[S{Segundo}] Dron{DronId}: {Instruccion.ObtenerDescripcion()} ({AlturaAntes}m → {AlturaDespues}m)";
        }
    }
}
