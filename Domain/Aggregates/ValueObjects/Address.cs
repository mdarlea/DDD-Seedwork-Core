
namespace Swaksoft.Domain.Seedwork.Aggregates.ValueObjects
{
    public class Address : ValueObject<Address>
    {
        #region equality

        public override bool Equals(Address other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && string.Equals(StreetAddress, other.StreetAddress) && string.Equals(City, other.City) && Equals(State, other.State) && string.Equals(Zip, other.Zip);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (StreetAddress != null ? StreetAddress.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (City != null ? City.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (State != null ? State.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Zip != null ? Zip.GetHashCode() : 0);
                return hashCode;
            }
        }

		#endregion equality

		public Address(string streetAddress, string city, string state, string zip) {
			StreetAddress = streetAddress;
			City = city;
			State = state;
			Zip = zip;
		}

		public Address() { } //required for EF

		public string StreetAddress { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Zip { get; private set; }
    }
}
