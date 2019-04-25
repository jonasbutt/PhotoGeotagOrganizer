using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using Newtonsoft.Json;

namespace PhotoGeotagOrganizer
{
    public class GeoCoder
    {
        public async Task<GeoCoordinate> GetCoordinates(string location)
        {
            var requestUrl = $"https://locationiq.org/v1/search.php?key={Settings.LocationIqApiKey}&q={location}&format=json";
            var response = await new HttpClient().GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            var json = await response.Content.ReadAsStringAsync();
            var locationIqResponses = JsonConvert.DeserializeObject<List<LocationIqResponse>>(json);
            var bestResult = locationIqResponses.FirstOrDefault();
            if (bestResult == null)
            {
                return default;
            }

            return new GeoCoordinate(bestResult.Lat, bestResult.Lon);
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class LocationIqResponse
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public double Lat { get; set; }

            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public double Lon { get; set; }
        }
    }
}