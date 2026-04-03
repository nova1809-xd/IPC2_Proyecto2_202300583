namespace EmisorDrones.CoreBusiness.TDAs
{
    /// <summary>
    /// Lista enlazada bidireccional implementada con memoria dinámica.
    /// Restricción: No usa List\<T\> de C#, solo nodos manuales.
    /// </summary>
    /// <typeparam name="T">Tipo de dato almacenado en la lista</typeparam>
    public class ListaEnlazada<T>
    {
        /// <summary>
        /// Referencia al primer nodo de la lista
        /// </summary>
        private NodoGenerico<T> cabeza;

        /// <summary>
        /// Referencia al último nodo de la lista
        /// </summary>
        private NodoGenerico<T> cola;

        /// <summary>
        /// Cantidad de elementos en la lista
        /// </summary>
        private int cantidad;

        /// <summary>
        /// Constructor que inicializa lista vacía
        /// </summary>
        public ListaEnlazada()
        {
            cabeza = null;
            cola = null;
            cantidad = 0;
        }

        /// <summary>
        /// Obtiene la cantidad de elementos en la lista
        /// </summary>
        public int Cantidad => cantidad;

        /// <summary>
        /// permite recorrer la lista desde el primer nodo
        /// </summary>
        public NodoGenerico<T> Cabeza => cabeza;

        /// <summary>
        /// Verifica si la lista está vacía
        /// </summary>
        public bool EstaVacia => cantidad == 0;

        /// <summary>
        /// Agrega un elemento al final de la lista
        /// </summary>
        /// <param name="dato">Elemento a agregar</param>
        public void AgregarAlFinal(T dato)
        {
            NodoGenerico<T> nuevoNodo = new NodoGenerico<T>(dato);

            if (EstaVacia)
            {
                cabeza = nuevoNodo;
                cola = nuevoNodo;
            }
            else
            {
                cola.Siguiente = nuevoNodo;
                nuevoNodo.Anterior = cola;
                cola = nuevoNodo;
            }

            cantidad++;
        }

        /// <summary>
        /// Agrega un elemento al inicio de la lista
        /// </summary>
        /// <param name="dato">Elemento a agregar</param>
        public void AgregarAlInicio(T dato)
        {
            NodoGenerico<T> nuevoNodo = new NodoGenerico<T>(dato);

            if (EstaVacia)
            {
                cabeza = nuevoNodo;
                cola = nuevoNodo;
            }
            else
            {
                cabeza.Anterior = nuevoNodo;
                nuevoNodo.Siguiente = cabeza;
                cabeza = nuevoNodo;
            }

            cantidad++;
        }

        /// <summary>
        /// Agrega un elemento en una posición específica (0-indexado)
        /// </summary>
        /// <param name="dato">Elemento a agregar</param>
        /// <param name="posicion">Posición donde insertar (0 = inicio)</param>
        public void AgregarEnPosicion(T dato, int posicion)
        {
            if (posicion < 0 || posicion > cantidad)
                throw new ArgumentOutOfRangeException(nameof(posicion), "Posición fuera de rango");

            if (posicion == 0)
                AgregarAlInicio(dato);
            else if (posicion == cantidad)
                AgregarAlFinal(dato);
            else
            {
                NodoGenerico<T> actual = ObtenerNodo(posicion);
                NodoGenerico<T> nuevoNodo = new NodoGenerico<T>(dato);

                nuevoNodo.Siguiente = actual;
                nuevoNodo.Anterior = actual.Anterior;
                actual.Anterior.Siguiente = nuevoNodo;
                actual.Anterior = nuevoNodo;

                cantidad++;
            }
        }

        /// <summary>
        /// Obtiene el elemento en una posición específica
        /// </summary>
        /// <param name="posicion">Posición del elemento (0-indexado)</param>
        /// <returns>Elemento en la posición</returns>
        public T ObtenerEnPosicion(int posicion)
        {
            if (posicion < 0 || posicion >= cantidad)
                throw new ArgumentOutOfRangeException(nameof(posicion), "Posición fuera de rango");

            return ObtenerNodo(posicion).Dato;
        }

        /// <summary>
        /// Obtiene el nodo en una posición específica (interno)
        /// </summary>
        private NodoGenerico<T> ObtenerNodo(int posicion)
        {
            NodoGenerico<T> actual;

            if (posicion < cantidad / 2)
            {
                actual = cabeza;
                for (int i = 0; i < posicion; i++)
                    actual = actual.Siguiente;
            }
            else
            {
                actual = cola;
                for (int i = cantidad - 1; i > posicion; i--)
                    actual = actual.Anterior;
            }

            return actual;
        }

        /// <summary>
        /// Elimina y retorna el primer elemento de la lista
        /// </summary>
        public T EliminarDelInicio()
        {
            if (EstaVacia)
                throw new InvalidOperationException("No se puede eliminar de una lista vacía");

            T dato = cabeza.Dato;

            if (cantidad == 1)
            {
                cabeza = null;
                cola = null;
            }
            else
            {
                cabeza = cabeza.Siguiente;
                cabeza.Anterior = null;
            }

            cantidad--;
            return dato;
        }

        /// <summary>
        /// Elimina y retorna el último elemento de la lista
        /// </summary>
        public T EliminarDelFinal()
        {
            if (EstaVacia)
                throw new InvalidOperationException("No se puede eliminar de una lista vacía");

            T dato = cola.Dato;

            if (cantidad == 1)
            {
                cabeza = null;
                cola = null;
            }
            else
            {
                cola = cola.Anterior;
                cola.Siguiente = null;
            }

            cantidad--;
            return dato;
        }

        /// <summary>
        /// Elimina el elemento en una posición específica
        /// </summary>
        /// <param name="posicion">Posición a eliminar (0-indexado)</param>
        public T EliminarEnPosicion(int posicion)
        {
            if (posicion < 0 || posicion >= cantidad)
                throw new ArgumentOutOfRangeException(nameof(posicion), "Posición fuera de rango");

            if (posicion == 0)
                return EliminarDelInicio();
            else if (posicion == cantidad - 1)
                return EliminarDelFinal();
            else
            {
                NodoGenerico<T> actual = ObtenerNodo(posicion);
                T dato = actual.Dato;

                actual.Anterior.Siguiente = actual.Siguiente;
                actual.Siguiente.Anterior = actual.Anterior;

                cantidad--;
                return dato;
            }
        }

        /// <summary>
        /// Busca el índice de un elemento en la lista
        /// </summary>
        /// <param name="dato">Elemento a buscar</param>
        /// <returns>Índice del elemento o -1 si no existe</returns>
        public int BuscarIndice(T dato)
        {
            NodoGenerico<T> actual = cabeza;
            int indice = 0;

            while (actual != null)
            {
                if (actual.Dato.Equals(dato))
                    return indice;

                actual = actual.Siguiente;
                indice++;
            }

            return -1;
        }

        /// <summary>
        /// Verifica si un elemento existe en la lista
        /// </summary>
        /// <param name="dato">Elemento a verificar</param>
        /// <returns>true si existe, false en caso contrario</returns>
        public bool Contiene(T dato)
        {
            return BuscarIndice(dato) != -1;
        }

        /// <summary>
        /// Vacía completamente la lista
        /// </summary>
        public void Limpiar()
        {
            cabeza = null;
            cola = null;
            cantidad = 0;
        }

        /// <summary>
        /// Retorna una representación en texto de la lista
        /// </summary>
        public override string ToString()
        {
            if (EstaVacia)
                return "vacia";

            string resultado = "[";
            NodoGenerico<T> actual = cabeza;

            while (actual != null)
            {
                resultado += actual.Dato?.ToString() ?? "null";
                if (actual.Siguiente != null)
                    resultado += ", ";
                actual = actual.Siguiente;
            }

            resultado += "]";
            return resultado;
        }
    }
}
