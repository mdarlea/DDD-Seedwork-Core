
namespace Swaksoft.Domain.Seedwork.Aggregates.ValueObjects
{
    public class LongAddress : Address
    {
        #region equality

        protected bool Equals(LongAddress other)
        {
            return base.Equals(other) && string.Equals(StreetAddress2, other.StreetAddress2) && string.Equals(County, other.County) && string.Equals(Country, other.Country);
        }

        public override bool Equals(Address obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((LongAddress) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (StreetAddress2 != null ? StreetAddress2.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (County != null ? County.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Country != null ? Country.GetHashCode() : 0);
                return hashCode;
            }
        }

		#endregion equality

		public LongAddress(string streetAddress, string streetAddress2, string city, string state, string zip, string county, string country)
				: base(streetAddress, city, state, zip)
		{
			StreetAddress2 = streetAddress2;
			County = county;
			Country = country;
		}

		public LongAddress() //required for EF
			: base(null, null, null, null)
		{
		}

        public string StreetAddress2 { get; private set; }
        public string County { get; private set; }
        public string Country { get; private set; }
    }
}
