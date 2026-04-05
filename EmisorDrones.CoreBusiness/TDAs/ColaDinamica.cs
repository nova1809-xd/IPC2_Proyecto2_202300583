namespace EmisorDrones.CoreBusiness.TDAs
{
    /// <summary>
    /// cola que funciona primero-entra-primero-sale, implementada manualmente sin usar Queue de C#
    /// </summary>
    /// <typeparam name="T">tipo de dato almacenado en la cola</typeparam>
    public class ColaDinamica<T>
    {
        /// <summary>
        /// lista interna que almacena los elementos
        /// </summary>
        private ListaEnlazada<T> elementos;

        /// <summary>
        /// constructor que inicializa cola vacía
        /// </summary>
        public ColaDinamica()
        {
            elementos = new ListaEnlazada<T>();
        }

        /// <summary>
        /// obtiene la cantidad de elementos en la cola
        /// </summary>
        public int Cantidad => elementos.Cantidad;

        /// <summary>
        /// verifica si la cola está vacía
        /// </summary>
        public bool EstaVacia => elementos.EstaVacia;

        /// <summary>
        /// agrega un elemento al final de la cola
        /// </summary>
        /// <param name="dato">elemento a encolar</param>
        public void Encolar(T dato)
        {
            elementos.AgregarAlFinal(dato);
        }

        /// <summary>
        /// saca y retorna el primer elemento de la cola
        /// </summary>
        /// <returns>primer elemento de la cola</returns>
        public T Desencolar()
        {
            if (EstaVacia)
                throw new InvalidOperationException("No se puede desencolar de una cola vacía");

            return elementos.EliminarDelInicio();
        }

        /// <summary>
        /// retorna el primer elemento sin eliminarlo
        /// </summary>
        /// <returns>primer elemento de la cola</returns>
        public T Frente()
        {
            if (EstaVacia)
                throw new InvalidOperationException("La cola está vacía");

            return elementos.ObtenerEnPosicion(0);
        }

        /// <summary>
        /// retorna el último elemento sin eliminarlo
        /// </summary>
        /// <returns>último elemento de la cola</returns>
        public T Fondo()
        {
            if (EstaVacia)
                throw new InvalidOperationException("La cola está vacía");

            return elementos.ObtenerEnPosicion(elementos.Cantidad - 1);
        }

        /// <summary>
        /// vacía completamente la cola
        /// </summary>
        public void Limpiar()
        {
            elementos.Limpiar();
        }

        /// <summary>
        /// retorna una representación en texto de la cola
        /// </summary>
        public override string ToString()
        {
            return $"Cola[{elementos}]";
        }
    }
}
