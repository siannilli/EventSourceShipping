using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDomainObjects.ValueObjects;

namespace SharedShippingDomainsObjects.ValueObjects
{
    public class GeographicPoint: ValueObject<GeographicPoint>
    {
        public decimal Latitude { get; private set; }
        public decimal Longitude { get; private set; }

        public GeographicPoint(decimal lon, decimal lat)
        {
            this.Latitude = lat;
            this.Longitude = lon;
        }

        public static bool ParseToDecimal(string lonText, string latText, out decimal lon, out decimal lat)
        {
            throw new NotImplementedException();
        }

        public static bool ParseToText(decimal lon, decimal lat, ref string lonText, ref string latText)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"Lat: {this.Latitude:D5} Lon: {this.Longitude:D5}";
        }

    }
}
