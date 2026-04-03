using EmisorDrones.CoreBusiness.TDAs;

namespace EmisorDrones.CoreBusiness.Models
{
    /// <summary>
    /// Representa un dron con capacidad de movimiento vertical y emisión de luz.
    /// Altura rango: 1 a 100 metros.
    /// </summary>
    public class Drone
    {
        /// <summary>
        /// Identificador único del dron
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Altura mínima operativa en metros (1-100)
        /// </summary>
        public int AlturaMinima { get; set; }

        /// <summary>
        /// Altura máxima operativa en metros (1-100)
        /// </summary>
        public int AlturaMaxima { get; set; }

        /// <summary>
        /// Altura actual en metros (comienza en AlturaMinima)
        /// </summary>
        public int AlturaActual { get; set; }

        /// <summary>
        /// Indica si el dron está activo o inactivo
        /// </summary>
        public bool Activo { get; set; }

        /// <summary>
        /// Historial de instrucciones ejecutadas por el dron
        /// Usa TDA ListaEnlazada en lugar de List\<T\>
        /// </summary>
        public ListaEnlazada<Instruccion> HistorialInstrucciones { get; private set; }

        /// <summary>
        /// Cantidad de veces que este dron ha emitido luz
        /// </summary>
        public int VecesEmitioLuz { get; set; }

        /// <summary>
        /// Constructor que inicializa un dron con valores específicos
        /// </summary>
        /// <param name="id">Identificador único</param>
        /// <param name="alturaMinima">Altura mínima en metros (validación 1-100)</param>
        /// <param name="alturaMaxima">Altura máxima en metros (validación 1-100)</param>
        /// <exception cref="ArgumentException">Si las alturas no están en rango válido</exception>
        public Drone(int id, int alturaMinima, int alturaMaxima)
        {
            if (alturaMinima < 1 || alturaMinima > 100)
                throw new ArgumentException("Altura mínima debe estar entre 1 y 100");

            if (alturaMaxima < 1 || alturaMaxima > 100)
                throw new ArgumentException("Altura máxima debe estar entre 1 y 100");

            if (alturaMinima > alturaMaxima)
                throw new ArgumentException("Altura mínima no puede ser mayor que altura máxima");

            this.Id = id;
            this.AlturaMinima = alturaMinima;
            this.AlturaMaxima = alturaMaxima;
            this.AlturaActual = alturaMinima;
            this.Activo = true;
            this.HistorialInstrucciones = new ListaEnlazada<Instruccion>();
            this.VecesEmitioLuz = 0;
        }

        /// <summary>
        /// Sube el dron 1 metro si es posible
        /// </summary>
        /// <returns>true si se ejecutó con éxito, false si está en altura máxima</returns>
        public bool Subir()
        {
            if (AlturaActual >= AlturaMaxima)
                return false;

            AlturaActual++;
            return true;
        }

        /// <summary>
        /// Baja el dron 1 metro si es posible
        /// </summary>
        /// <returns>true si se ejecutó con éxito, false si está en altura mínima</returns>
        public bool Bajar()
        {
            if (AlturaActual <= AlturaMinima)
                return false;

            AlturaActual--;
            return true;
        }

        /// <summary>
        /// Verifica si el dron está en la altura especificada
        /// </summary>
        /// <param name="altura">Altura a verificar</param>
        /// <returns>true si la altura actual coincide</returns>
        public bool EstaEnAltura(int altura)
        {
            return AlturaActual == altura;
        }

        /// <summary>
        /// Calcula distancia que debe recorrer para llegar a una altura
        /// </summary>
        /// <param name="alturaDestino">Altura destino</param>
        /// <returns>Cantidad de metros a subir (negativo si debe bajar)</returns>
        public int CalcularDistancia(int alturaDestino)
        {
            if (alturaDestino < AlturaMinima || alturaDestino > AlturaMaxima)
                return -1; // Altura inaccesible

            return alturaDestino - AlturaActual;
        }

        /// <summary>
        /// Verifica si el dron puede alcanzar una altura específica
        /// </summary>
        /// <param name="altura">Altura destino</param>
        /// <returns>true si la altura está en rango operativo</returns>
        public bool PuedeAlcanzarAltura(int altura)
        {
            return altura >= AlturaMinima && altura <= AlturaMaxima;
        }

        /// <summary>
        /// Registro de instrucción ejecutada por el dron
        /// </summary>
        /// <param name="instruccion">Instrucción a registrar</param>
        public void RegistrarInstruccion(Instruccion instruccion)
        {
            if (instruccion == null)
                throw new ArgumentNullException(nameof(instruccion));

            HistorialInstrucciones.AgregarAlFinal(instruccion);

            if (instruccion.Tipo == TipoInstruccion.EmitirLuz)
                VecesEmitioLuz++;
        }

        /// <summary>
        /// obtiene el historial de instrucciones como lista enlazada
        /// </summary>
        public ListaEnlazada<Instruccion> ObtenerHistorial()
        {
            return HistorialInstrucciones;
        }

        /// <summary>
        /// Limpia el historial de instrucciones
        /// </summary>
        public void LimpiarHistorial()
        {
            HistorialInstrucciones.Limpiar();
            VecesEmitioLuz = 0;
        }

        /// <summary>
        /// Retorna una representación de estado del dron
        /// </summary>
        public override string ToString()
        {
            return $"Dron {Id}: Altura={AlturaActual}m [{AlturaMinima}-{AlturaMaxima}], Activo={Activo}, Emisiones={VecesEmitioLuz}";
        }

        /// <summary>
        /// Obtiene estado detallado del dron para UI
        /// </summary>
        public string ObtenerEstadoDetallado()
        {
            return $@"
Dron ID: {Id}
Estado: {(Activo ? "Activo" : "Inactivo")}
Altura Actual: {AlturaActual} m
Rango Operativo: {AlturaMinima} - {AlturaMaxima} m
Veces Emitió Luz: {VecesEmitioLuz}
Instrucciones Ejecutadas: {HistorialInstrucciones.Cantidad}";
        }
    }
}
