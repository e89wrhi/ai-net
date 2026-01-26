using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AI.Common.Utils;

/// <summary>
/// A collection of reflection helpers.
/// It's used to discover types and assemblies at runtime—for example, to automatically register all Command Handlers 
/// or find specific events by their name without hardcoding every single one.
/// </summary>
public static class TypeProvider
{
    /// <summary>
    /// Checks if a type is a C# record.
    /// </summary>
    private static bool IsRecord(this Type objectType)
    {
        return objectType.GetMethod("<Clone>$") != null ||
               ((TypeInfo)objectType)
               .DeclaredProperties.FirstOrDefault(x => x.Name == "EqualityContract")?
               .GetMethod?.GetCustomAttribute(typeof(CompilerGeneratedAttribute)) != null;
    }

    /// <summary>
    /// Searches for a type in any assembly referenced by the entry assembly.
    /// </summary>
    public static Type? GetTypeFromAnyReferencingAssembly(string typeName)
    {
        var referencedAssemblies = Assembly.GetEntryAssembly()?
            .GetReferencedAssemblies()
            .Select(a => a.FullName);

        if (referencedAssemblies == null)
            return null;

        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => referencedAssemblies.Contains(a.FullName))
            .SelectMany(a => a.GetTypes().Where(x => x.FullName == typeName || x.Name == typeName))
            .FirstOrDefault();
    }

    /// <summary>
    /// Searches for a type in all currently loaded assemblies in the AppDomain.
    /// </summary>
    public static Type? GetFirstMatchingTypeFromCurrentDomainAssembly(string typeName)
    {
        var result = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes().Where(x => x.FullName == typeName || x.Name == typeName))
            .FirstOrDefault();

        return result;
    }

    /// <summary>
    /// Recursively finds all assemblies referenced by the root assembly.
    /// </summary>
    public static IReadOnlyList<Assembly> GetReferencedAssemblies(Assembly? rootAssembly)
    {
        var visited = new HashSet<string>();
        var queue = new Queue<Assembly?>();
        var listResult = new List<Assembly>();

        var root = rootAssembly ?? Assembly.GetEntryAssembly();
        queue.Enqueue(root);

        do
        {
            var asm = queue.Dequeue();

            if (asm == null)
                break;

            listResult.Add(asm);

            foreach (var reference in asm.GetReferencedAssemblies())
            {
                if (!visited.Contains(reference.FullName))
                {
                    // Load will add assembly into the application domain of the caller.
                    // We load them explicitly because assemblies are often loaded lazily.
                    queue.Enqueue(Assembly.Load(reference));
                    visited.Add(reference.FullName);
                }
            }
        } while (queue.Count > 0);

        return listResult.Distinct().ToList().AsReadOnly();
    }

    /// <summary>
    /// Finds all assemblies marked as Application Parts starting with the same root namespace.
    /// </summary>
    public static IReadOnlyList<Assembly> GetApplicationPartAssemblies(Assembly rootAssembly)
    {
        var rootNamespace = rootAssembly.GetName().Name!.Split('.').First();
        var list = rootAssembly!.GetCustomAttributes<ApplicationPartAttribute>()
            .Where(x => x.AssemblyName.StartsWith(rootNamespace, StringComparison.InvariantCulture))
            .Select(name => Assembly.Load(name.AssemblyName))
            .Distinct();

        return list.ToList().AsReadOnly();
    }

}