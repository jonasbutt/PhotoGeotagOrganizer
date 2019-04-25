using System.Collections.Generic;
using GeoCoordinatePortable;

namespace PhotoGeotagOrganizer
{
    public class Location
    {
        public string Name { get; set; }

        public GeoCoordinate Coordinates { get; set; }

        public List<FileWithMetadata> Files { get; set; }
    }
}