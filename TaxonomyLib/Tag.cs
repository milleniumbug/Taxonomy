using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxonomyLib
{
	public class Tag : IEquatable<Tag>
	{
		public bool Equals(Tag other)
		{
			if(ReferenceEquals(null, other)) return false;
			if(ReferenceEquals(this, other)) return true;
			return Id == other.Id;
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != this.GetType()) return false;
			return Equals((Tag) obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public static bool operator ==(Tag left, Tag right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Tag left, Tag right)
		{
			return !Equals(left, right);
		}

		internal long Id { get; set; }
		public Namespace Namespace { get; }
		public TagName Name { get; }

		internal Tag(long id, Namespace ns, TagName name)
		{
			Id = id;
			Namespace = ns;
			Name = name;
		}

		public override string ToString()
		{
			return $"{Namespace.Component}:{Name.Name}";
		}
	}
}
