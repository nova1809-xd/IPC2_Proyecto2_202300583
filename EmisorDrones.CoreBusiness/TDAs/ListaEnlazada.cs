namespace EmisorDrones.CoreBusiness.TDAs
{
    /// <summary>
    /// lista enlazada que puedes recorrer hacia adelante y atrás, hecha totalmente a mano sin usar List de C#
    /// </summary>
    /// <typeparam name="T">tipo de dato almacenado en la lista</typeparam>
    public class ListaEnlazada<T>
    {
        /// <summary>
        /// referencia al primer nodo de la lista
        /// </summary>
        private NodoGenerico<T> cabeza;

        /// <summary>
        /// referencia al último nodo de la lista
        /// </summary>
        private NodoGenerico<T> cola;

        /// <summary>
        /// cantidad de elementos en la lista
        /// </summary>
        private int cantidad;

        /// <summary>
        /// constructor que inicializa lista vacía
        /// </summary>
        public ListaEnlazada()
        {
            cabeza = null;
            cola = null;
            cantidad = 0;
        }

        /// <summary>
        /// obtiene la cantidad de elementos en la lista
        /// </summary>
        public int Cantidad => cantidad;

        /// <summary>
        /// permite empezar a recorrer la lista desde el primer nodo
        /// </summary>
        public NodoGenerico<T> Cabeza => cabeza;

        /// <summary>
        /// verifica si la lista está vacía
        /// </summary>
        public bool EstaVacia => cantidad == 0;

        /// <summary>
        /// agrega un elemento al final de la lista
        /// </summary>
        /// <param name="dato">elemento a agregar</param>
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
        /// agrega un elemento al inicio de la lista
        /// </summary>
        /// <param name="dato">elemento a agregar</param>
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
        /// agrega un elemento en una posición específica (0-indexado)
        /// </summary>
        /// <param name="dato">elemento a agregar</param>
        /// <param name="posicion">posición donde insertar (0 = inicio)</param>
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
        /// obtiene el elemento en una posición específica
        /// </summary>
        /// <param name="posicion">posición del elemento (0-indexado)</param>
        /// <returns>elemento en la posición</returns>
        public T ObtenerEnPosicion(int posicion)
        {
            if (posicion < 0 || posicion >= cantidad)
                throw new ArgumentOutOfRangeException(nameof(posicion), "Posición fuera de rango");

            return ObtenerNodo(posicion).Dato;
        }

        /// <summary>
        /// obtiene el nodo en una posición específica (interno)
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
        /// elimina y retorna el primer elemento de la lista
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
        /// elimina y retorna el último elemento de la lista
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
        /// elimina el elemento en una posición específica
        /// </summary>
        /// <param name="posicion">posición a eliminar (0-indexado)</param>
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
        /// busca el índice de un elemento en la lista
        /// </summary>
        /// <param name="dato">elemento a buscar</param>
        /// <returns>índice del elemento o -1 si no existe</returns>
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
        /// verifica si un elemento existe en la lista
        /// </summary>
        /// <param name="dato">elemento a verificar</param>
        /// <returns>true si existe, false en caso contrario</returns>
        public bool Contiene(T dato)
        {
            return BuscarIndice(dato) != -1;
        }

        /// <summary>
        /// ordena la lista alfabéticamente usando ordenamiento de burbuja hecho a mano
        /// no usa estructuras nativas ni LINQ
        /// </summary>
        /// <param name="selectorNombre">función para obtener el texto a comparar</param>
        public void OrdenarAlfabeticamente(Func<T, string> selectorNombre)
        {
            if (selectorNombre == null)
                throw new ArgumentNullException(nameof(selectorNombre));

            if (cantidad <= 1)
                return;

            bool huboIntercambio = true;

            while (huboIntercambio)
            {
                huboIntercambio = false;
                NodoGenerico<T> actual = cabeza;

                while (actual != null && actual.Siguiente != null)
                {
                    string nombreActual = selectorNombre(actual.Dato) ?? string.Empty;
                    string nombreSiguiente = selectorNombre(actual.Siguiente.Dato) ?? string.Empty;

                    if (string.Compare(nombreActual, nombreSiguiente, StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        T temporal = actual.Dato;
                        actual.Dato = actual.Siguiente.Dato;
                        actual.Siguiente.Dato = temporal;
                        huboIntercambio = true;
                    }

                    actual = actual.Siguiente;
                }
            }
        }

        /// <summary>
        /// vacía completamente la lista
        /// </summary>
        public void Limpiar()
        {
            cabeza = null;
            cola = null;
            cantidad = 0;
        }

        /// <summary>
        /// retorna una representación en texto de la lista
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
