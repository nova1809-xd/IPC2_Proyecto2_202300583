namespace EmisorDrones.CoreBusiness.Configuration
{
    /// <summary>
    /// configuración de flags para habilitar/deshabilitar funcionalidades por release
    /// </summary>
    public static class FeatureFlags
    {
        /// <summary>
        /// identificador del release actual
        /// </summary>
        public enum Release
        {
            /// <summary>release 1: infraestructura base y gestión de drones</summary>
            Release1 = 1,

            /// <summary>release 2: sistemas de mapeo y configuración</summary>
            Release2 = 2,

            /// <summary>release 3: procesamiento de mensajes y algoritmo</summary>
            Release3 = 3,

            /// <summary>release 4: visualización y exportación</summary>
            Release4 = 4
        }

        /// <summary>
        /// release actual activo
        /// </summary>
        public static Release CurrentRelease { get; set; } = Release.Release1;

        /// <summary>
        /// verifica si una funcionalidad está disponible en el release actual
        /// </summary>
        /// <param name="feature">funcionalidad a verificar</param>
        /// <returns>true si está disponible en el release actual o anterior</returns>
        public static bool IsEnabled(Feature feature)
        {
            var featureReleases = new Dictionary<Feature, Release>
            {
                // Release 1
                { Feature.GestionarDrones, Release.Release1 },
                { Feature.VerDrones, Release.Release1 },
                { Feature.AgregarDron, Release.Release1 },
                { Feature.DetallesDron, Release.Release1 },
                { Feature.EliminarDron, Release.Release1 },
                { Feature.Dashboard, Release.Release1 },

                // Release 2
                { Feature.GestionarSistemas, Release.Release2 },
                { Feature.ConfigurarMapeoLetras, Release.Release2 },
                { Feature.VisualizarGrafo, Release.Release2 },
                { Feature.CargarXml, Release.Release2 },

                // Release 3
                { Feature.ProcesarMensajes, Release.Release3 },
                { Feature.VerMensajes, Release.Release3 },
                { Feature.AlgoritmoOptimizacion, Release.Release3 },
                { Feature.SimuladorPrevio, Release.Release3 },

                // Release 4
                { Feature.SimulacionPasoAPaso, Release.Release4 },
                { Feature.GraficoTimeline, Release.Release4 },
                { Feature.GraficoOcupacion, Release.Release4 },
                { Feature.ExportarXml, Release.Release4 },
                { Feature.Estadisticas, Release.Release4 }
            };

            return featureReleases.TryGetValue(feature, out var requiredRelease) &&
                   (int)requiredRelease <= (int)CurrentRelease;
        }

        /// <summary>
        /// lista de funcionalidades con sus releases de introducción
        /// </summary>
        public enum Feature
        {
            // ============ RELEASE 1 ============
            /// <summary>gestión básica de drones (ABM)</summary>
            GestionarDrones = 1,

            /// <summary>ver listado de drones registrados</summary>
            VerDrones = 2,

            /// <summary>agregar nuevo dron al sistema</summary>
            AgregarDron = 3,

            /// <summary>ver detalles y historial de un dron</summary>
            DetallesDron = 4,

            /// <summary>eliminar dron del sistema</summary>
            EliminarDron = 5,

            /// <summary>dashboard principal con estado del sistema</summary>
            Dashboard = 6,

            // ============ RELEASE 2 ============
            /// <summary>crear y gestionar sistemas de drones (mapeo letra-altura)</summary>
            GestionarSistemas = 7,

            /// <summary>configurar mapeo de letras a alturas</summary>
            ConfigurarMapeoLetras = 8,

            /// <summary>visualizar gráfico graphviz del sistema</summary>
            VisualizarGrafo = 9,

            /// <summary>cargar archivo XML de configuración</summary>
            CargarXml = 10,

            // ============ RELEASE 3 ============
            /// <summary>procesar mensajes y calcular instrucciones</summary>
            ProcesarMensajes = 11,

            /// <summary>ver lista de mensajes a procesar</summary>
            VerMensajes = 12,

            /// <summary>algoritmo de optimización para tiempo mínimo</summary>
            AlgoritmoOptimizacion = 13,

            /// <summary>simulador de pre-visualización</summary>
            SimuladorPrevio = 14,

            // ============ RELEASE 4 ============
            /// <summary>simulación paso a paso visual</summary>
            SimulacionPasoAPaso = 15,

            /// <summary>gráfico de timeline en graphviz</summary>
            GraficoTimeline = 16,

            /// <summary>gráfico de ocupación de alturas</summary>
            GraficoOcupacion = 17,

            /// <summary>exportar resultados a XML</summary>
            ExportarXml = 18,

            /// <summary>estadísticas finales de ejecución</summary>
            Estadisticas = 19
        }

        /// <summary>
        /// obtiene descripción legible de una funcionalidad
        /// </summary>
        public static string GetDescription(Feature feature)
        {
            return feature switch
            {
                Feature.GestionarDrones => "Gestión de Drones",
                Feature.VerDrones => "Ver Drones",
                Feature.AgregarDron => "Agregar Dron",
                Feature.DetallesDron => "Detalles de Dron",
                Feature.EliminarDron => "Eliminar Dron",
                Feature.Dashboard => "Dashboard",
                Feature.GestionarSistemas => "Gestionar Sistemas",
                Feature.ConfigurarMapeoLetras => "Configurar Mapeo",
                Feature.VisualizarGrafo => "Visualizar Grafo",
                Feature.CargarXml => "Cargar XML",
                Feature.ProcesarMensajes => "Procesar Mensajes",
                Feature.VerMensajes => "Ver Mensajes",
                Feature.AlgoritmoOptimizacion => "Algoritmo Optimización",
                Feature.SimuladorPrevio => "Simulador Previo",
                Feature.SimulacionPasoAPaso => "Simulación",
                Feature.GraficoTimeline => "Gráfico Timeline",
                Feature.GraficoOcupacion => "Gráfico Ocupación",
                Feature.ExportarXml => "Exportar XML",
                Feature.Estadisticas => "Estadísticas",
                _ => "Desconocida"
            };
        }
    }
}
