namespace EmisorDrones.CoreBusiness.TDAs
{
    /// <summary>
    /// Estructura base de nodo genérico para implementar memoria dinámica.
    /// Permite referencias bidireccionales (anterior y siguiente).
    /// Restricción: No se usa memoria estándar de C# para nodos - es construcción manual.
    /// </summary>
    /// <typeparam name="T">Tipo de dato que almacenará el nodo</typeparam>
    public class NodoGenerico<T>
    {
        /// <summary>
        /// Datos almacenados en el nodo
        /// </summary>
        public T Dato { get; set; }

        /// <summary>
        /// Referencia al nodo siguiente (derecha en lista)
        /// </summary>
        public NodoGenerico<T> Siguiente { get; set; }

        /// <summary>
        /// Referencia al nodo anterior (izquierda en lista)
        /// </summary>
        public NodoGenerico<T> Anterior { get; set; }

        /// <summary>
        /// Constructor que inicializa el nodo con un dato específico
        /// </summary>
        /// <param name="dato">Dato a almacenar en el nodo</param>
        public NodoGenerico(T dato)
        {
            this.Dato = dato;
            this.Siguiente = null;
            this.Anterior = null;
        }

        /// <summary>
        /// Retorna una representación en texto del nodo
        /// </summary>
        /// <returns>String con el valor del dato</returns>
        public override string ToString()
        {
            return Dato?.ToString() ?? "null";
        }
    }
}
