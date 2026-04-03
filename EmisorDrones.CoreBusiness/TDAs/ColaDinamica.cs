namespace EmisorDrones.CoreBusiness.TDAs
{
    /// <summary>
    /// Cola de prioridad FIFO (First In First Out) implementada con lista enlazada.
    /// Restricción: No usa Queue\<T\> de C#.
    /// </summary>
    /// <typeparam name="T">Tipo de dato almacenado en la cola</typeparam>
    public class ColaDinamica<T>
    {
        /// <summary>
        /// Lista interna que almacena los elementos
        /// </summary>
        private ListaEnlazada<T> elementos;

        /// <summary>
        /// Constructor que inicializa cola vacía
        /// </summary>
        public ColaDinamica()
        {
            elementos = new ListaEnlazada<T>();
        }

        /// <summary>
        /// Obtiene la cantidad de elementos en la cola
        /// </summary>
        public int Cantidad => elementos.Cantidad;

        /// <summary>
        /// Verifica si la cola está vacía
        /// </summary>
        public bool EstaVacia => elementos.EstaVacia;

        /// <summary>
        /// Encola un elemento al final de la cola (rear)
        /// </summary>
        /// <param name="dato">Elemento a encolar</param>
        public void Encolar(T dato)
        {
            elementos.AgregarAlFinal(dato);
        }

        /// <summary>
        /// Desencola y retorna el primer elemento (front) de la cola
        /// </summary>
        /// <returns>Primer elemento de la cola</returns>
        public T Desencolar()
        {
            if (EstaVacia)
                throw new InvalidOperationException("No se puede desencolar de una cola vacía");

            return elementos.EliminarDelInicio();
        }

        /// <summary>
        /// Retorna el primer elemento sin eliminarlo
        /// </summary>
        /// <returns>Primer elemento de la cola</returns>
        public T Frente()
        {
            if (EstaVacia)
                throw new InvalidOperationException("La cola está vacía");

            return elementos.ObtenerEnPosicion(0);
        }

        /// <summary>
        /// Retorna el último elemento sin eliminarlo
        /// </summary>
        /// <returns>Último elemento de la cola</returns>
        public T Fondo()
        {
            if (EstaVacia)
                throw new InvalidOperationException("La cola está vacía");

            return elementos.ObtenerEnPosicion(elementos.Cantidad - 1);
        }

        /// <summary>
        /// Vacía completamente la cola
        /// </summary>
        public void Limpiar()
        {
            elementos.Limpiar();
        }

        /// <summary>
        /// Retorna una representación en texto de la cola
        /// </summary>
        public override string ToString()
        {
            return $"Cola[{elementos}]";
        }
    }
}
