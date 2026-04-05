namespace EmisorDrones.CoreBusiness.Models
{
    /// <summary>
    /// enumeración de tipos de instrucciones que puede ejecutar un dron
    /// cada instrucción consume exactamente 1 segundo
    /// </summary>
    public enum TipoInstruccion
    {
        /// <summary>
        /// dron sube 1 metro
        /// </summary>
        Subir = 1,

        /// <summary>
        /// dron baja 1 metro
        /// </summary>
        Bajar = 2,

        /// <summary>
        /// dron espera sin hacer nada
        /// </summary>
        Esperar = 3,

        /// <summary>
        /// dron emite luz (solo uno por segundo en todo el sistema)
        /// </summary>
        EmitirLuz = 4
    }

    /// <summary>
    /// representa una instrucción ejecutable por un dron
    /// cada instrucción toma exactamente 1 segundo
    /// </summary>
    public class Instruccion
    {
        /// <summary>
        /// tipo de instrucción a ejecutar
        /// </summary>
        public TipoInstruccion Tipo { get; set; }

        /// <summary>
        /// tiempo en que se ejecuta esta instrucción (en segundos, empezando en 0)
        /// </summary>
        public int Tiempo { get; set; }

        /// <summary>
        /// id del dron que ejecuta esta instrucción
        /// </summary>
        public int DronId { get; set; }

        /// <summary>
        /// altura objetivo que debe alcanzar el dron antes de emitir luz
        /// </summary>
        public int AlturaObjetivo { get; set; }

        /// <summary>
        /// constructor con parámetros
        /// </summary>
        /// <param name="tipo">tipo de instrucción</param>
        /// <param name="tiempo">tiempo de ejecución en segundos</param>
        /// <param name="dronId">id del dron ejecutor</param>
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
        /// retorna una representación legible de la instrucción
        /// </summary>
        public override string ToString()
        {
            return $"[T{Tiempo}] Dron {DronId}: {Tipo}";
        }

        /// <summary>
        /// obtiene una descripción detallada de la instrucción
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
