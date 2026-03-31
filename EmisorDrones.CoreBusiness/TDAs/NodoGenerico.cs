namespace EmisorDrones.CoreBusiness.TDAs
{
    /// <summary>
    /// nodo genérico base para hacer memoria dinámica.
    /// tiene referencias bidireccionales así puedo encadenar nodos hacia adelante y atrás.
    /// </summary>
    /// <typeparam name="T">el tipo de dato que va a guardar el nodo</typeparam>
    public class NodoGenerico<T>
    {
        /// <summary>
        /// datos que contiene este nodo
        /// </summary>
        public T Dato { get; set; }

        /// <summary>
        /// referencia al próximo nodo a la derecha
        /// </summary>
        public NodoGenerico<T> Siguiente { get; set; }

        /// <summary>
        /// referencia al nodo anterior a la izquierda
        /// </summary>
        public NodoGenerico<T> Anterior { get; set; }

        /// <summary>
        /// inicializa un nodo con el dato que le pasas
        /// </summary>
        /// <param name="dato">dato a guardar en el nodo</param>
        public NodoGenerico(T dato)
        {
            this.Dato = dato;
            this.Siguiente = null;
            this.Anterior = null;
        }

        /// <summary>
        /// retorna el contenido del nodo como texto
        /// </summary>
        /// <returns>valor del dato como string</returns>
        public override string ToString()
        {
            return Dato?.ToString() ?? "null";
        }
    }
}
