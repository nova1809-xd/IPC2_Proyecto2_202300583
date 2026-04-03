namespace EmisorDrones.CoreBusiness.TDAs
{
    /// <summary>
    /// Pila LIFO (Last In First Out) implementada con lista enlazada.
    /// Restricción: No usa Stack\<T\> de C#.
    /// </summary>
    /// <typeparam name="T">Tipo de dato almacenado en la pila</typeparam>
    public class PilaDinamica<T>
    {
        /// <summary>
        /// Lista interna que almacena los elementos
        /// </summary>
        private ListaEnlazada<T> elementos;

        /// <summary>
        /// Constructor que inicializa pila vacía
        /// </summary>
        public PilaDinamica()
        {
            elementos = new ListaEnlazada<T>();
        }

        /// <summary>
        /// Obtiene la cantidad de elementos en la pila
        /// </summary>
        public int Cantidad => elementos.Cantidad;

        /// <summary>
        /// Verifica si la pila está vacía
        /// </summary>
        public bool EstaVacia => elementos.EstaVacia;

        /// <summary>
        /// Introduce un elemento en la cima de la pila
        /// </summary>
        /// <param name="dato">Elemento a empujar</param>
        public void Empujar(T dato)
        {
            elementos.AgregarAlInicio(dato);
        }

        /// <summary>
        /// Extrae y retorna el elemento en la cima de la pila
        /// </summary>
        /// <returns>Elemento en la cima</returns>
        public T Extraer()
        {
            if (EstaVacia)
                throw new InvalidOperationException("No se puede extraer de una pila vacía");

            return elementos.EliminarDelInicio();
        }

        /// <summary>
        /// Retorna el elemento en la cima sin eliminarlo
        /// </summary>
        /// <returns>Elemento en la cima</returns>
        public T Cima()
        {
            if (EstaVacia)
                throw new InvalidOperationException("La pila está vacía");

            return elementos.ObtenerEnPosicion(0);
        }

        /// <summary>
        /// Vacía completamente la pila
        /// </summary>
        public void Limpiar()
        {
            elementos.Limpiar();
        }

        /// <summary>
        /// Retorna una representación en texto de la pila
        /// </summary>
        public override string ToString()
        {
            return $"Pila[{elementos}]";
        }
    }
}
