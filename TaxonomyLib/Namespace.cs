using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxonomyLib
{
	public class Namespace : IEquatable<Namespace>
	{
		public string Component { get; }

		public Namespace(string component)
		{
			if(string.IsNullOrEmpty(component))
				throw new ArgumentNullException(nameof(component));
			if(component.LastIndexOfAny(new []{':', ' ', '\n', '\t'}) >= 0)
				throw new ArgumentException(nameof(component));
			Component = component;
		}

		private Namespace()
		{

		}

		public static Namespace Global { get; } = new Namespace();

		public bool Equals(Namespace other)
		{
			if(ReferenceEquals(null, other)) return false;
			if(ReferenceEquals(this, other)) return true;
			return string.Equals(Component, other.Component);
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != this.GetType()) return false;
			return Equals((Namespace) obj);
		}

		public override int GetHashCode()
		{
			return Component.GetHashCode();
		}

		public static bool operator ==(Namespace left, Namespace right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Namespace left, Namespace right)
		{
			return !Equals(left, right);
		}
	}
}
