using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;

namespace AssemblyInformation
{
	internal class AssemblyInformation
	{
		public string FullName { get; set; }

		public AssemblyName[] ReferencedAssemblies { get; set; }

		public DebuggableAttribute Debuggable { get; set; }

		public TargetFrameworkAttribute TargetFramework { get; set; }

		public PortableExecutableKinds PortableExecutableKinds { get; set; }

		public ImageFileMachine ImageFileMachine { get; set; }
	}
}
