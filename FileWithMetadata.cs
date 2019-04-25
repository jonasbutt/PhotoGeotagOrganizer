using System;
using System.IO;
using GeoCoordinatePortable;

namespace PhotoGeotagOrganizer
{
    public class FileWithMetadata
    {
        public FileInfo File { get; set; }

        public DateTimeOffset DateTaken { get; set; }

        public GeoCoordinate Coordinates { get; set; }
    }
}