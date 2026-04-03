namespace EmisorDrones.CoreBusiness.Models
{
    /// <summary>
    /// Enumeración de tipos de instrucciones que puede ejecutar un dron.
    /// Cada instrucción consume exactamente 1 segundo.
    /// </summary>
    public enum TipoInstruccion
    {
        /// <summary>
        /// Dron sube 1 metro
        /// </summary>
        Subir = 1,

        /// <summary>
        /// Dron baja 1 metro
        /// </summary>
        Bajar = 2,

        /// <summary>
        /// Dron espera sin hacer nada
        /// </summary>
        Esperar = 3,

        /// <summary>
        /// Dron emite luz (solo uno por segundo en todo el sistema)
        /// </summary>
        EmitirLuz = 4
    }

    /// <summary>
    /// Representa una instrucción ejecutable por un dron.
    /// Cada instrucción toma exactamente 1 segundo.
    /// </summary>
    public class Instruccion
    {
        /// <summary>
        /// Tipo de instrucción a ejecutar
        /// </summary>
        public TipoInstruccion Tipo { get; set; }

        /// <summary>
        /// Tiempo en que se ejecuta esta instrucción (en segundos, base 0)
        /// </summary>
        public int Tiempo { get; set; }

        /// <summary>
        /// ID del dron que ejecuta esta instrucción
        /// </summary>
        public int DronId { get; set; }

        /// <summary>
        /// altura objetivo que debe alcanzar el dron antes de emitir luz
        /// </summary>
        public int AlturaObjetivo { get; set; }

        /// <summary>
        /// Constructor con parámetros
        /// </summary>
        /// <param name="tipo">Tipo de instrucción</param>
        /// <param name="tiempo">Tiempo de ejecución en segundos</param>
        /// <param name="dronId">ID del dron ejecutor</param>
        public Instruccion(TipoInstruccion tipo, int tiempo, int dronId)
        {
            this.Tipo = tipo;
            this.Tiempo = tiempo;
            this.DronId = dronId;
            this.AlturaObjetivo = -1;
        }

        public Instruccion(TipoInstruccion tipo, int tiempo, int dronId, int alturaObjetivo)
        {
            this.Tipo = tipo;
            this.Tiempo = tiempo;
            this.DronId = dronId;
            this.AlturaObjetivo = alturaObjetivo;
        }

        /// <summary>
        /// Retorna una representación legible de la instrucción
        /// </summary>
        public override string ToString()
        {
            return $"[T{Tiempo}] Dron {DronId}: {Tipo}";
        }

        /// <summary>
        /// Obtiene una descripción detallada de la instrucción
        /// </summary>
        public string ObtenerDescripcion()
        {
            return Tipo switch
            {
                TipoInstruccion.Subir => $"Subir 1 metro",
                TipoInstruccion.Bajar => $"Bajar 1 metro",
                TipoInstruccion.Esperar => $"Esperar",
                TipoInstruccion.EmitirLuz => $"Emitir Luz",
                _ => "Desconocida"
            };
        }
    }
}
