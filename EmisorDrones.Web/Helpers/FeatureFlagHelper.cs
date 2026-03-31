using EmisorDrones.CoreBusiness.Configuration;

namespace EmisorDrones.Web.Helpers
{
    /// <summary>
    /// ayudante para trabajar con feature flags en las vistas
    /// </summary>
    public static class FeatureFlagHelper
    {
        /// <summary>
        /// verifica si una funcionalidad está disponible
        /// </summary>
        public static bool IsAvailable(FeatureFlags.Feature feature)
        {
            return FeatureFlags.IsEnabled(feature);
        }

        /// <summary>
        /// obtiene el nombre actual del release
        /// </summary>
        public static string GetCurrentReleaseLabel()
        {
            return $"Release {(int)FeatureFlags.CurrentRelease}";
        }

        /// <summary>
        /// obtiene descripción del release actual
        /// </summary>
        public static string GetCurrentReleaseDescription()
        {
            return FeatureFlags.CurrentRelease switch
            {
                FeatureFlags.Release.Release1 => "Fundación - TDAs e Infraestructura",
                FeatureFlags.Release.Release2 => "Sistemas de Mapeo",
                FeatureFlags.Release.Release3 => "Procesamiento de Mensajes",
                FeatureFlags.Release.Release4 => "Visualización Avanzada",
                _ => "Desconocido"
            };
        }

        /// <summary>
        /// obtiene lista de funcionalidades disponibles
        /// </summary>
        public static List<(FeatureFlags.Feature feature, string description, bool available)> GetAvailableFeatures()
        {
            var features = new List<(FeatureFlags.Feature, string, bool)>();

            foreach (FeatureFlags.Feature feature in Enum.GetValues(typeof(FeatureFlags.Feature)))
            {
                bool available = FeatureFlags.IsEnabled(feature);
                string description = FeatureFlags.GetDescription(feature);
                features.Add((feature, description, available));
            }

            return features.OrderBy(f => (int)f.Item1).ToList();
        }
    }
}
