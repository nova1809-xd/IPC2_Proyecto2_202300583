namespace EmisorDrones.CoreBusiness.TDAs
{
    /// <summary>
    /// pila que funciona con última-entra-primera-sale, implementada manualmente sin usar Stack de C#
    /// </summary>
    /// <typeparam name="T">tipo de dato que guarda la pila</typeparam>
    public class PilaDinamica<T>
    {
        /// <summary>
        /// lista interna que almacena los elementos
        /// </summary>
        private ListaEnlazada<T> elementos;

        /// <summary>
        /// constructor que inicializa pila vacía
        /// </summary>
        public PilaDinamica()
        {
            elementos = new ListaEnlazada<T>();
        }

        /// <summary>
        /// obtiene la cantidad de elementos en la pila
        /// </summary>
        public int Cantidad => elementos.Cantidad;

        /// <summary>
        /// verifica si la pila está vacía
        /// </summary>
        public bool EstaVacia => elementos.EstaVacia;

        /// <summary>
        /// introduce un elemento en la cima de la pila
        /// </summary>
        /// <param name="dato">elemento a empujar</param>
        public void Empujar(T dato)
        {
            elementos.AgregarAlInicio(dato);
        }

        /// <summary>
        /// extrae y retorna el elemento en la cima de la pila
        /// </summary>
        /// <returns>elemento en la cima</returns>
        public T Extraer()
        {
            if (EstaVacia)
                throw new InvalidOperationException("No se puede extraer de una pila vacía");

            return elementos.EliminarDelInicio();
        }

        /// <summary>
        /// retorna el elemento en la cima sin eliminarlo
        /// </summary>
        /// <returns>elemento en la cima</returns>
        public T Cima()
        {
            if (EstaVacia)
                throw new InvalidOperationException("La pila está vacía");

            return elementos.ObtenerEnPosicion(0);
        }

        /// <summary>
        /// vacía completamente la pila
        /// </summary>
        public void Limpiar()
        {
            elementos.Limpiar();
        }

        /// <summary>
        /// retorna una representación en texto de la pila
        /// </summary>
        public override string ToString()
        {
            return $"Pila[{elementos}]";
        }
    }
}
