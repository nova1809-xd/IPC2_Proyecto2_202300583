# 📝 GUÍA: Cómo Actualizar Comentarios al Estilo Natural

## Principios

Los comentarios deben:
1. ✅ **Empezar con minúscula** (a menos que sea nombre propio)
2. ✅ **Ser conversacionales** - como si los escribieras tú en una conversación
3. ✅ **Ser claros pero informales** - no demasiado técnicos
4. ✅ **Evitar "El/La/Los/Las"** al inicio - se ve poco natural

---

## ANTES vs DESPUÉS

### Antes (Formal)
```csharp
/// <summary>
/// Estructura base de nodo genérico para implementar memoria dinámica.
/// Permite referencias bidireccionales (anterior y siguiente).
/// </summary>
```

### Después (Natural)
```csharp
/// <summary>
/// nodo genérico para hacer memoria dinámica.
/// tiene referencias bidireccionales así puedo encadenar nodos hacia adelante y atrás.
/// </summary>
```

---

## PATRONES DE CAMBIO

### Para Clases
```
ANTES:  /// Estructura que representa un dron
DESPUÉS: /// un dron que puede subir, bajar y emitir luz

ANTES:  /// Implementación de lista dinámica
DESPUÉS: /// lista encadenada que hago yo mismo sin usar List<T>
```

### Para Propiedades
```
ANTES:  /// Referencia al primer nodo de la lista
DESPUÉS: /// apunta al primer nodo de la cadena

ANTES:  /// Cantidad de elementos en la lista
DESPUÉS: /// cuántos elementos hay en la lista

ANTES:  /// Verifica si la lista está vacía
DESPUÉS: /// retorna true si no hay nada en la lista
```

### Para Métodos
```
ANTES:  /// Agrega un elemento al final de la lista
DESPUÉS: /// agrega un elemento al final

ANTES:  /// Constructor que inicializa el nodo con un dato específico  
DESPUÉS: /// crea un nodo con un dato inicial

ANTES:  /// Obtiene el elemento en una posición específica
DESPUÉS: /// retorna el elemento en la posición que indiques

ANTES:  /// Elimina y retorna el primer elemento
DESPUÉS: /// saca y retorna el primer elemento
```

### Para Parámetros
```
ANTES:  /// <param name="dato">Dato a agregar</param>
DESPUÉS: /// <param name="dato">el dato a agregar</param>

ANTES:  /// <param name="posicion">Posición donde insertar (0 = inicio)</param>
DESPUÉS: /// <param name="posicion">dónde insertar (0 es el inicio)</param>
```

### Para Return
```
ANTES:  /// <returns>Elemento en la posición o -1 si no existe</returns>
DESPUÉS: /// <returns>el índice del elemento o -1 si no lo encuentra</returns>

ANTES:  /// <returns>String con el valor del dato</returns>
DESPUÉS: /// <returns>el contenido del nodo como texto</returns>
```

---

## CAMBIOS CLAVE POR SECCIÓN

### Archivo: NodoGenerico.cs  
✅ Cambio hecho como ejemplo - ya está con comentarios naturales

### Archivos PENDIENTES:
- [ ] ListaEnlazada.cs
- [ ] ColaDinamica.cs
- [ ] PilaDinamica.cs
- [ ] Drone.cs
- [ ] Instruccion.cs
- [ ] RegistroDrone.cs
- [ ] ServicioDrones.cs
- [ ] HomeController.cs
- [ ] AdminController.cs

---

## EJEMPLOS ESPECÍFICOS POR ARCHIVO

### ListaEnlazada.cs
```
ANTES: "Referencia al primer nodo de la lista"
DESPUÉS: "apunta al primer nodo de la cadena"

ANTES: "Obtiene la cantidad de elementos en la lista"
DESPUÉS: "retorna cuántos elementos hay"

ANTES: "Agrega un elemento al final de la lista"
DESPUÉS: "agrega un elemento al final"

ANTES: "Agrega un elemento al inicio de la lista"
DESPUÉS: "agrega un elemento al inicio"

ANTES: "Obtiene el elemento en una posición específica (0-indexado)"
DESPUÉS: "retorna el elemento en una posición (0 empieza en 0)"

ANTES: "Elimina y retorna el primer elemento de la lista"
DESPUÉS: "elimina y retorna el primer elemento"

ANTES: "Elimina y retorna el último elemento de la lista"
DESPUÉS: "elimina y retorna el último elemento"

ANTES: "Busca el índice de un elemento en la lista"
DESPUÉS: "busca en qué posición está un elemento"

ANTES: "Verifica si un elemento existe en la lista"
DESPUÉS: "retorna true si el elemento está en la lista"

ANTES: "Vacía completamente la lista"
DESPUÉS: "vacía la lista, borra todo"

ANTES: "Obtiene todos los elementos como array"
DESPUÉS: "retorna un array con todos los elementos"

ANTES: "Retorna una representación en texto de la lista"
DESPUÉS: "convierte la lista a un string bonito"
```

### Drone.cs
```
ANTES: "Identificador único del dron"
DESPUÉS: "número único que identifica este dron"

ANTES: "Altura mínima operativa en metros (1-100)"
DESPUÉS: "la altura más baja a la que puede ir"

ANTES: "Altura máxima operativa en metros (1-100)"
DESPUÉS: "la altura más alta a la que puede llegar"

ANTES: "Altura actual en metros (comienza en AlturaMinima)"
DESPUÉS: "en dónde está ahora el dron (empieza en altura mínima)"

ANTES: "Indica si el dron está activo o inactivo"
DESPUÉS: "si el dron está listo para trabajar"

ANTES: "Historial de instrucciones ejecutadas por el dron"
DESPUÉS: "guarda todo lo que el dron ha hecho"

ANTES: "Sube el dron 1 metro si es posible"
DESPUÉS: "sube el dron 1 metro (si puede)"

ANTES: "Baja el dron 1 metro si es posible"
DESPUÉS: "baja el dron 1 metro (si puede)"

ANTES: "Verifica si el dron está en la altura especificada"
DESPUÉS: "retorna true si el dron está en esa altura exacta"

ANTES: "Calcula distancia que debe recorrer para llegar a una altura"
DESPUÉS: "calcula cuántos metros debe subir o bajar para llegar a una altura"

ANTES: "Verifica si el dron puede alcanzar una altura específica"
DESPUÉS: "retorna true si esa altura está dentro de sus límites"
```

---

## CÓMO IDENTIFICAR SI NECESITA CAMBIO

Pregúntate:
- ❓ ¿Empeza con mayúscula? → Cambiar a minúscula (excepto nombres propios)
- ❓ ¿Suena muy formal/corporativo? → Hazlo más casual
- ❓ ¿Empieza con "Referencia al/la/los/las"? → Cambiar a "apunta al", "guarda el", etc.
- ❓ ¿Empieza con "Obtiene/Retorna/Verifica/Agrega"? → Puede ser más directo/casual

---

## ESTADO ACTUAL

✅ **Cambios Hechos:**
- NodoGenerico.cs - ✅ COMPLETADO

⏳ **Por Hacer:**
Todos los demás archivos (pendientes de reescritura selectiva)

---

**Nota:** No es necesario reescribir TODOS los archivos de una sola vez.  
Se puede hacer gradualmente, archivo por archivo, cuando tengas tiempo.  
Lo importante es mantener la consistencia al escribir nuevos comentarios.
