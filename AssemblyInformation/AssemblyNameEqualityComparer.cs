using System;
using System.Collections.Generic;
using System.Reflection;

namespace AssemblyInformation
{
	internal class AssemblyNameEqualityComparer : IEqualityComparer<AssemblyName>
	{
		public bool Equals(AssemblyName x, AssemblyName y)
		{
			return x == y || x.FullName == y.FullName;
		}

		public int GetHashCode(AssemblyName obj)
		{
			return obj.FullName.GetHashCode(StringComparison.OrdinalIgnoreCase);
		}
	}
}
