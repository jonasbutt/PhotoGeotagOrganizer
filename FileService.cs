using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExifLib;
using GeoCoordinatePortable;

namespace PhotoGeotagOrganizer
{
    public class FileService
    {
        public void CopyFile(string sourceFilePath, string destinationDirectoryPath, string destinationFilename = default)
        {
            if (!Directory.Exists(destinationDirectoryPath))
            {
                Directory.CreateDirectory(destinationDirectoryPath);
            }

            if (destinationFilename == default)
            {
                destinationFilename = Path.GetFileName(sourceFilePath);
            }

            var destinationFilePath = Path.Combine(destinationDirectoryPath, destinationFilename);
            File.Copy(sourceFilePath, destinationFilePath);
        }

        public IList<FileWithMetadata> GetAllFiles(string directoryPath)
        {
            return Directory.EnumerateFiles(directoryPath).Select(GetFile).ToList();
        }

        private static FileWithMetadata GetFile(string filePath)
        {
            var file = new FileWithMetadata
            {
                File = new FileInfo(filePath),
                DateTaken = DateTimeOffset.MaxValue
            };

            try
            {
                
                using (var exifReader = new ExifReader(filePath))
                {
                    if (exifReader.GetTagValue<DateTime>(ExifTags.DateTimeOriginal, out var dateTaken))
                    {
                        file.DateTaken = dateTaken;
                    }
                }
            }
            catch (ExifLibException)
            {
            }

            try
            {
                using (var exifReader = new ExifReader(filePath))
                {
                    exifReader.GetTagValue<double[]>(ExifTags.GPSLatitude, out var latitude);
                    exifReader.GetTagValue<double[]>(ExifTags.GPSLongitude, out var longitude);

                    if (latitude != null && longitude != null)
                    {
                        exifReader.GetTagValue<string>(ExifTags.GPSLatitudeRef, out var latitudeRef);
                        exifReader.GetTagValue<string>(ExifTags.GPSLongitudeRef, out var longitudeRef);

                        file.Coordinates = new GeoCoordinate(ToDoubleCoordinate(latitude, latitudeRef), ToDoubleCoordinate(longitude, longitudeRef));
                    }
                }
            }
            catch (ExifLibException)
            {
            }

            return file;
        }

        private static double ToDoubleCoordinate(IReadOnlyList<double> coordinates, string gpsRef)
        {
            var doubleCoordinate = coordinates[0] + coordinates[1] / 60f + coordinates[2] / 3600f;
            return gpsRef == "S" || gpsRef == "W" ? doubleCoordinate * -1 : doubleCoordinate;
        }
    }
}