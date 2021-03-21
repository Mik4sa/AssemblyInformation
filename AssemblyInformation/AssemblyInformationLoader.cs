using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace AssemblyInformation
{
	internal class AssemblyInformationLoader
	{
		private readonly string mainAssemblyPath;

		public AssemblyInformationLoader(string mainAssemblyPath)
		{
			this.mainAssemblyPath = mainAssemblyPath;
		}

		public AssemblyName[] GetReferencedAssemblies(AssemblyName assemblyName)
		{
			AssemblyName[] referencedAssemblies = this.GetReferencedAssemblies(assemblyName, out WeakReference assemblyLoadContextWeakReference);

			WaitForGarbageCollection(assemblyLoadContextWeakReference);

			return referencedAssemblies;
		}

		public AssemblyInformation GetAssemblyInformation(string assemblyPath)
		{
			AssemblyInformation assemblyInformation = this.GetAssemblyInformation(assemblyPath, out WeakReference assemblyLoadContextWeakReference);

			WaitForGarbageCollection(assemblyLoadContextWeakReference);

			return assemblyInformation;
		}

		// It is important to mark this method as NoInlining, otherwise the JIT could decide
		// to inline it into the Main method. That could then prevent successful unloading
		// of the assembly because some instances may get lifetime extended beyond the point
		// when the assembly is expected to be unloaded.
		[MethodImpl(MethodImplOptions.NoInlining)]
		private AssemblyName[] GetReferencedAssemblies(AssemblyName assemblyName, out WeakReference assemblyLoadContextWeakReference)
		{
			AssemblyInformationLoadContext assemblyInformationLoadContext = this.GetAssemblyInformationLoadContext(out assemblyLoadContextWeakReference);

			Assembly assembly = assemblyInformationLoadContext.LoadFromAssemblyName(assemblyName);
			AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();

			// This initiates the unload of the AssemblyInformationLoadContext. The actual unloading doesn't happen
			// right away, GC has to kick in later to collect all the stuff.
			assemblyInformationLoadContext.Unload();

			return referencedAssemblies;
		}

		// It is important to mark this method as NoInlining, otherwise the JIT could decide
		// to inline it into the Main method. That could then prevent successful unloading
		// of the assembly because some instances may get lifetime extended beyond the point
		// when the assembly is expected to be unloaded.
		[MethodImpl(MethodImplOptions.NoInlining)]
		private AssemblyInformation GetAssemblyInformation(string assemblyPath, out WeakReference assemblyLoadContextWeakReference)
		{
			AssemblyInformationLoadContext assemblyInformationLoadContext = this.GetAssemblyInformationLoadContext(out assemblyLoadContextWeakReference);

			Assembly assembly = assemblyInformationLoadContext.LoadFromAssemblyPath(assemblyPath);

			AssemblyInformation assemblyInformation = new()
			{
				FullName = assembly.FullName,
				ReferencedAssemblies = assembly.GetReferencedAssemblies(),
			};

			object[] customAttributes = assembly.GetCustomAttributes(false);
			assemblyInformation.Debuggable = customAttributes.OfType<DebuggableAttribute>().FirstOrDefault();
			assemblyInformation.TargetFramework = customAttributes.OfType<TargetFrameworkAttribute>().FirstOrDefault();

			assembly.ManifestModule.GetPEKind(out PortableExecutableKinds portableExecutableKinds, out ImageFileMachine imageFileMachine);
			assemblyInformation.PortableExecutableKinds = portableExecutableKinds;
			assemblyInformation.ImageFileMachine = imageFileMachine;

			// This initiates the unload of the AssemblyInformationLoadContext. The actual unloading doesn't happen
			// right away, GC has to kick in later to collect all the stuff.
			assemblyInformationLoadContext.Unload();

			return assemblyInformation;
		}

		// It is important to mark this method as NoInlining, otherwise the JIT could decide
		// to inline it into the Main method. That could then prevent successful unloading
		// of the assembly because some instances may get lifetime extended beyond the point
		// when the assembly is expected to be unloaded.
		[MethodImpl(MethodImplOptions.NoInlining)]
		private AssemblyInformationLoadContext GetAssemblyInformationLoadContext(out WeakReference assemblyLoadContextWeakReference)
		{
			AssemblyInformationLoadContext assemblyInformationLoadContext = new(this.mainAssemblyPath);

			// Create a weak reference to the AssemblyLoadContext that will allow us to detect when the unload completes.
			assemblyLoadContextWeakReference = new WeakReference(assemblyInformationLoadContext);

			return assemblyInformationLoadContext;
		}

		private static void WaitForGarbageCollection(WeakReference weakReference)
		{
			// Poll and run GC until the AssemblyLoadContext is unloaded
			while (weakReference.IsAlive)
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
		}
	}
}
