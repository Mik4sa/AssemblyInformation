using System.Reflection;
using System.Runtime.Loader;

namespace AssemblyInformation
{
	// https://docs.microsoft.com/en-us/dotnet/standard/assembly/unloadability#use-collectible-assemblyloadcontext
	// https://github.com/dotnet/samples/blob/a9940046f3f766e611e246db057f1da45dede9d2/core/tutorials/Unloading/Host/Program.cs
	internal class AssemblyInformationLoadContext : AssemblyLoadContext
	{
		private readonly AssemblyDependencyResolver assemblyDependencyResolver;

		public AssemblyInformationLoadContext(string mainAssemblyPath) : base(true)
		{
			this.assemblyDependencyResolver = new(mainAssemblyPath);
		}

		protected override Assembly Load(AssemblyName assemblyName)
		{
			Assembly assembly = null;

			string assemblyPath = this.assemblyDependencyResolver.ResolveAssemblyToPath(assemblyName);

			if (assemblyPath != null)
			{
				assembly = this.LoadFromAssemblyPath(assemblyPath);
			}

			return assembly;
		}
	}
}
