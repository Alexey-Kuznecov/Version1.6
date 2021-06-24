// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace UnityCommander.Plugin.Loader
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
#if !NET48
    using System.Runtime.Loader;

    using UnityCommander.Plugin.Internal;
    using UnityCommander.Plugin.LibraryModel;

    /// <summary>
    /// An implementation of <see cref="AssemblyLoadContext" /> which attempts to load managed and native
    /// binaries at runtime immitating some of the behaviors of corehost.
    /// </summary>

    [DebuggerDisplay("'{Name}' ({_mainAssemblyPath})")]
    internal class ManagedLoadContext : AssemblyLoadContext
    {
        private readonly string _basePath;
        private readonly string _mainAssemblyPath;
        private readonly IReadOnlyDictionary<string, ManagedLibrary> _managedAssemblies;
        private readonly IReadOnlyDictionary<string, NativeLibrary> _nativeLibraries;
        private readonly IReadOnlyCollection<string> _privateAssemblies;
        private readonly ICollection<string> _defaultAssemblies;
        private readonly IReadOnlyCollection<string> _additionalProbingPaths;
        private readonly bool _preferDefaultLoadContext;
        private readonly string[] _resourceRoots;
        private readonly bool _loadInMemory;
        private readonly bool _lazyLoadReferences;
        private readonly AssemblyLoadContext _defaultLoadContext;
#if FEATURE_NATIVE_RESOLVER
        private readonly AssemblyDependencyResolver _dependencyResolver;
#endif
        private readonly bool _shadowCopyNativeLibraries;
        private readonly string _unmanagedDllShadowCopyDirectoryPath;

        public ManagedLoadContext(string mainAssemblyPath,
            IReadOnlyDictionary<string, ManagedLibrary> managedAssemblies,
            IReadOnlyDictionary<string, NativeLibrary> nativeLibraries,
            IReadOnlyCollection<string> privateAssemblies,
            IReadOnlyCollection<string> defaultAssemblies,
            IReadOnlyCollection<string> additionalProbingPaths,
            IReadOnlyCollection<string> resourceProbingPaths,
            AssemblyLoadContext defaultLoadContext,
            bool preferDefaultLoadContext,
            bool lazyLoadReferences,
            bool isCollectible,
            bool loadInMemory,
            bool shadowCopyNativeLibraries)
#if FEATURE_UNLOAD
            : base(Path.GetFileNameWithoutExtension(mainAssemblyPath), isCollectible)
#endif
        {
            if (resourceProbingPaths == null)
            {
                throw new ArgumentNullException(nameof(resourceProbingPaths));
            }

            this._mainAssemblyPath = mainAssemblyPath ?? throw new ArgumentNullException(nameof(mainAssemblyPath));
#if FEATURE_NATIVE_RESOLVER
            _dependencyResolver = new AssemblyDependencyResolver(mainAssemblyPath);
#endif
            this._basePath = Path.GetDirectoryName(mainAssemblyPath) ?? throw new ArgumentException(nameof(mainAssemblyPath));
            this._managedAssemblies = managedAssemblies ?? throw new ArgumentNullException(nameof(managedAssemblies));
            this._privateAssemblies = privateAssemblies ?? throw new ArgumentNullException(nameof(privateAssemblies));
            this._defaultAssemblies = defaultAssemblies != null ? defaultAssemblies.ToList() : throw new ArgumentNullException(nameof(defaultAssemblies));
            this._nativeLibraries = nativeLibraries ?? throw new ArgumentNullException(nameof(nativeLibraries));
            this._additionalProbingPaths = additionalProbingPaths ?? throw new ArgumentNullException(nameof(additionalProbingPaths));
            this._defaultLoadContext = defaultLoadContext;
            this._preferDefaultLoadContext = preferDefaultLoadContext;
            this._loadInMemory = loadInMemory;
            this._lazyLoadReferences = lazyLoadReferences;

            this._resourceRoots = new[] { this._basePath }
                .Concat(resourceProbingPaths)
                .ToArray();

            this._shadowCopyNativeLibraries = shadowCopyNativeLibraries;
            this._unmanagedDllShadowCopyDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            if (shadowCopyNativeLibraries)
            {
                this.Unloading += _ => this.OnUnloaded();
            }
        }

        /// <summary>
        /// Load an assembly.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        protected override Assembly? Load(AssemblyName assemblyName)
        {
            if (assemblyName.Name == null)
            {
                // not sure how to handle this case. It's technically possible.
                return null;
            }

            if ((this._preferDefaultLoadContext || this._defaultAssemblies.Contains(assemblyName.Name)) && !this._privateAssemblies.Contains(assemblyName.Name))
            {
                // If default context is preferred, check first for types in the default context unless the dependency has been declared as private
                try
                {
                    var defaultAssembly = this._defaultLoadContext.LoadFromAssemblyName(assemblyName);
                    if (defaultAssembly != null)
                    {
                        // Add referenced assemblies to the list of default assemblies.
                        // This is basically lazy loading
                        if (this._lazyLoadReferences)
                        {
                            foreach (var reference in defaultAssembly.GetReferencedAssemblies())
                            {
                                if (reference.Name != null && !this._defaultAssemblies.Contains(reference.Name))
                                {
                                    this._defaultAssemblies.Add(reference.Name);
                                }
                            }
                        }

                        // Older versions used to return null here such that returned assembly would be resolved from the default ALC.
                        // However, with the addition of custom default ALCs, the Default ALC may not be the user's chosen ALC when
                        // this context was built. As such, we simply return the Assembly from the user's chosen default load context.
                        return defaultAssembly;
                    }
                }
                catch
                {
                    // Swallow errors in loading from the default context
                }
            }

#if FEATURE_NATIVE_RESOLVER
            var resolvedPath = _dependencyResolver.ResolveAssemblyToPath(assemblyName);
            if (!string.IsNullOrEmpty(resolvedPath) && File.Exists(resolvedPath))
            {
                return LoadAssemblyFromFilePath(resolvedPath);
            }
#endif
            // Resource assembly binding does not use the TPA. Instead, it probes PLATFORM_RESOURCE_ROOTS (a list of folders)
            // for $folder/$culture/$assemblyName.dll
            // See https://github.com/dotnet/coreclr/blob/3fca50a36e62a7433d7601d805d38de6baee7951/src/binder/assemblybinder.cpp#L1232-L1290

            if (!string.IsNullOrEmpty(assemblyName.CultureName) && !string.Equals("neutral", assemblyName.CultureName))
            {
                foreach (var resourceRoot in this._resourceRoots)
                {
                    var resourcePath = Path.Combine(resourceRoot, assemblyName.CultureName, assemblyName.Name + ".dll");
                    if (File.Exists(resourcePath))
                    {
                        return this.LoadAssemblyFromFilePath(resourcePath);
                    }
                }

                return null;
            }

            if (this._managedAssemblies.TryGetValue(assemblyName.Name, out var library) && library != null)
            {
                if (this.SearchForLibrary(library, out var path) && path != null)
                {
                    return this.LoadAssemblyFromFilePath(path);
                }
            }
            else
            {
                // if an assembly was not listed in the list of known assemblies,
                // fallback to the load context base directory
                var dllName = assemblyName.Name + ".dll";
                foreach (var probingPath in this._additionalProbingPaths.Prepend(this._basePath))
                {
                    var localFile = Path.Combine(probingPath, dllName);
                    if (File.Exists(localFile))
                    {
                        return this.LoadAssemblyFromFilePath(localFile);
                    }
                }
            }

            return null;
        }

        public Assembly LoadAssemblyFromFilePath(string path)
        {
            
            if (!this._loadInMemory)
            {
                return this.LoadFromAssemblyPath(path);
            }

            using var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var pdbPath = Path.ChangeExtension(path, ".pdb");
            if (File.Exists(pdbPath))
            {
                using var pdbFile = File.Open(pdbPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return this.LoadFromStream(file, pdbFile);

            }

            return this.LoadFromStream(file);
        }

        /// <summary>
        /// Loads the unmanaged binary using configured list of native libraries.
        /// </summary>
        /// <param name="unmanagedDllName"></param>
        /// <returns></returns>
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
#if FEATURE_NATIVE_RESOLVER
            var resolvedPath = _dependencyResolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (!string.IsNullOrEmpty(resolvedPath) && File.Exists(resolvedPath))
            {
                return LoadUnmanagedDllFromResolvedPath(resolvedPath, normalizePath: false);
            }
#endif

            foreach (var prefix in PlatformInformation.NativeLibraryPrefixes)
            {
                if (this._nativeLibraries.TryGetValue(prefix + unmanagedDllName, out var library))
                {
                    if (this.SearchForLibrary(library, prefix, out var path) && path != null)
                    {
                        return this.LoadUnmanagedDllFromResolvedPath(path);
                    }
                }
                else
                {
                    // coreclr allows code to use [DllImport("sni")] or [DllImport("sni.dll")]
                    // This library treats the file name without the extension as the lookup name,
                    // so this loop is necessary to check if the unmanaged name matches a library
                    // when the file extension has been trimmed.
                    foreach (var suffix in PlatformInformation.NativeLibraryExtensions)
                    {
                        if (!unmanagedDllName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        // check to see if there is a library entry for the library without the file extension
                        var trimmedName = unmanagedDllName.Substring(0, unmanagedDllName.Length - suffix.Length);

                        if (this._nativeLibraries.TryGetValue(prefix + trimmedName, out library))
                        {
                            if (this.SearchForLibrary(library, prefix, out var path) && path != null)
                            {
                                return this.LoadUnmanagedDllFromResolvedPath(path);
                            }
                        }
                        else
                        {
                            // fallback to native assets which match the file name in the plugin base directory
                            var prefixSuffixDllName = prefix + unmanagedDllName + suffix;
                            var prefixDllName = prefix + unmanagedDllName;

                            foreach (var probingPath in this._additionalProbingPaths.Prepend(this._basePath))
                            {
                                var localFile = Path.Combine(probingPath, prefixSuffixDllName);
                                if (File.Exists(localFile))
                                {
                                    return this.LoadUnmanagedDllFromResolvedPath(localFile);
                                }

                                var localFileWithoutSuffix = Path.Combine(probingPath, prefixDllName);
                                if (File.Exists(localFileWithoutSuffix))
                                {
                                    return this.LoadUnmanagedDllFromResolvedPath(localFileWithoutSuffix);
                                }
                            }

                        }
                    }

                }
            }

            return base.LoadUnmanagedDll(unmanagedDllName);
        }

        private bool SearchForLibrary(ManagedLibrary library, out string? path)
        {
            // 1. Check for in _basePath + app local path
            var localFile = Path.Combine(this._basePath, library.AppLocalPath);
            if (File.Exists(localFile))
            {
                path = localFile;
                return true;
            }

            // 2. Search additional probing paths
            foreach (var searchPath in this._additionalProbingPaths)
            {
                var candidate = Path.Combine(searchPath, library.AdditionalProbingPath);
                if (File.Exists(candidate))
                {
                    path = candidate;
                    return true;
                }
            }

            // 3. Search in base path
            foreach (var ext in PlatformInformation.ManagedAssemblyExtensions)
            {
                var local = Path.Combine(this._basePath, library.Name.Name + ext);
                if (File.Exists(local))
                {
                    path = local;
                    return true;
                }
            }

            path = null;
            return false;
        }

        private bool SearchForLibrary(NativeLibrary library, string prefix, out string? path)
        {
            // 1. Search in base path
            foreach (var ext in PlatformInformation.NativeLibraryExtensions)
            {
                var candidate = Path.Combine(this._basePath, $"{prefix}{library.Name}{ext}");
                if (File.Exists(candidate))
                {
                    path = candidate;
                    return true;
                }
            }

            // 2. Search in base path + app local (for portable deployments of netcoreapp)
            var local = Path.Combine(this._basePath, library.AppLocalPath);
            if (File.Exists(local))
            {
                path = local;
                return true;
            }

            // 3. Search additional probing paths
            foreach (var searchPath in this._additionalProbingPaths)
            {
                var candidate = Path.Combine(searchPath, library.AdditionalProbingPath);
                if (File.Exists(candidate))
                {
                    path = candidate;
                    return true;
                }
            }

            path = null;
            return false;
        }

        private IntPtr LoadUnmanagedDllFromResolvedPath(string unmanagedDllPath, bool normalizePath = true)
        {
            if (normalizePath)
            {
                unmanagedDllPath = Path.GetFullPath(unmanagedDllPath);
            }

            return this._shadowCopyNativeLibraries
                ? this.LoadUnmanagedDllFromShadowCopy(unmanagedDllPath)
                : this.LoadUnmanagedDllFromPath(unmanagedDllPath);
        }

        private IntPtr LoadUnmanagedDllFromShadowCopy(string unmanagedDllPath)
        {
            var shadowCopyDllPath = this.CreateShadowCopy(unmanagedDllPath);
            return this.LoadUnmanagedDllFromPath(shadowCopyDllPath);
        }

        private string CreateShadowCopy(string dllPath)
        {
            Directory.CreateDirectory(this._unmanagedDllShadowCopyDirectoryPath);

            var dllFileName = Path.GetFileName(dllPath);
            var shadowCopyPath = Path.Combine(this._unmanagedDllShadowCopyDirectoryPath, dllFileName);

            if (!File.Exists(shadowCopyPath))
            {
                File.Copy(dllPath, shadowCopyPath);
            }

            return shadowCopyPath;
        }

        private void OnUnloaded()
        {
            if (!this._shadowCopyNativeLibraries || !Directory.Exists(this._unmanagedDllShadowCopyDirectoryPath))
            {
                return;
            }

            // Attempt to delete shadow copies
            try
            {
                Directory.Delete(this._unmanagedDllShadowCopyDirectoryPath, recursive: true);
            }
            catch (Exception)
            {
                // Files might be locked by host process. Nothing we can do about it, I guess.
            }
        }
    }
#endif
}

