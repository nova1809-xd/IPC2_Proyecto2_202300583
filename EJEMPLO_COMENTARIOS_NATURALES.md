namespace EmisorDrones.CoreBusiness.TDAs
{
    /// <summary>
    /// nodo genérico para hacer memoria dinámica.
    /// tiene referencias bidireccionales (anterior y siguiente) así puedo encadenar nodos en cualquier dirección.
    /// </summary>
    /// <typeparam name="T">el tipo de dato que va a guardar el nodo</typeparam>
    public class NodoGenerico<T>
    {
        /// <summary>
        /// el dato que guarda un nodo
        /// </summary>
        public T Dato { get; set; }

        /// <summary>
        /// apunta al siguiente nodo en la cadena (hacia la derecha)
        /// </summary>
        public NodoGenerico<T> Siguiente { get; set; }

        /// <summary>
        /// apunta al nodo anterior en la cadena (hacia la izquierda)
        /// </summary>
        public NodoGenerico<T> Anterior { get; set; }

        /// <summary>
        /// crea un nodo con un dato inicial
        /// </summary>
        /// <param name="dato">el dato a almacenar</param>
        public NodoGenerico(T dato)
        {
            this.Dato = dato;
            this.Siguiente = null;
            this.Anterior = null;
        }

        /// <summary>
        /// convierte el nodo a string (usa el dato si existe, sino retorna "null")
        /// </summary>
        /// <returns>representación en texto del dato</returns>
        public override string ToString()
        {
            return Dato?.ToString() ?? "null";
        }
    }
}
