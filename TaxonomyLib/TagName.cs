using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxonomyLib
{
	public class TagName : IEquatable<TagName>
	{
		public string Name { get; }

		public TagName(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));
			if(name.LastIndexOfAny(new[] { ':', ' ', '\n', '\t' }) >= 0)
				throw new ArgumentException(nameof(name));
			Name = name;
		}

		public bool Equals(TagName other)
		{
			if(ReferenceEquals(null, other)) return false;
			if(ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name);
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != this.GetType()) return false;
			return Equals((TagName) obj);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public static bool operator ==(TagName left, TagName right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(TagName left, TagName right)
		{
			return !Equals(left, right);
		}
	}
}
