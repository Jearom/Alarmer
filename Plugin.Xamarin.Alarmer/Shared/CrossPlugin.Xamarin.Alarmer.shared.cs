using Plugin.Plugin.Xamarin.Alarmer.Shared;
using System;

namespace Plugin.Plugin.Xamarin.Alarmer
{
    /// <summary>
    /// Cross Plugin.Xamarin.Alarmer
    /// </summary>
    public static class Alarmer
    {
        static Lazy<IAlarmer> implementation = new Lazy<IAlarmer>(() => CreateAlarmer(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Gets if the plugin is supported on the current platform.
    /// </summary>
    public static bool IsSupported => implementation.Value == null ? false : true;

    /// <summary>
    /// Current plugin implementation to use
    /// </summary>
    public static IAlarmer Current
    {
        get
        {
            IAlarmer ret = implementation.Value;
            if (ret == null)
            {
                throw NotImplementedInReferenceAssembly();
            }
            return ret;
        }
    }

    static IAlarmer CreateAlarmer()
    {
#if NETSTANDARD1_0 || NETSTANDARD2_0
            return null;
#else
#pragma warning disable IDE0022 // Use expression body for methods
        return new Plugin.Xamarin.AlarmerImplementation();
#pragma warning restore IDE0022 // Use expression body for methods
#endif
    }

    internal static Exception NotImplementedInReferenceAssembly() =>
        new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");

}
}
