using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGeotagOrganizer
{
    class Program
    {
        static async Task Main()
        {
            var files = GetFiles();
            var locations = await GetLocations();

            foreach (var location in locations)
            {
                var matchingFiles = files.Where(x => x.Coordinates != null).Where(x => x.Coordinates.GetDistanceTo(location.Coordinates) <= Settings.GroupingRadiusInKilometers).ToList();
                if (matchingFiles.Any())
                {
                    matchingFiles.ForEach(x => files.Remove(x));
                    location.Files.AddRange(matchingFiles);
                }
            }

            // Attempts to assign the remaining files to locations if the photo was taken during the stay at the location
            // This only works when locations have been visited only once during the entire trip
            foreach (var location in locations)
            {
                if (location.Files.Any())
                {
                    var startDate = location.Files.Where(x => x.DateTaken != DateTimeOffset.MaxValue).Min(x => x.DateTaken);
                    var endDate = location.Files.Where(x => x.DateTaken != DateTimeOffset.MaxValue).Max(x => x.DateTaken);

                    var matchingFiles = files.Where(x => startDate < x.DateTaken && x.DateTaken < endDate).ToList();
                    if (matchingFiles.Any())
                    {
                        matchingFiles.ForEach(x => files.Remove(x));
                        location.Files.AddRange(matchingFiles);
                    }
                }
            }

            var fileService = new FileService();
            foreach (var location in locations)
            {
                var fileIndex = 1;
                foreach (var file in location.Files.OrderBy(x => x.DateTaken))
                {
                    fileService.CopyFile(
                        file.File.FullName,
                        Path.Combine(Settings.OutputDirectory, SlugGenerator.SlugGenerator.GenerateSlug(location.Name)),
                        $"{fileIndex}.jpg");

                    fileIndex++;
                }
            }

            var remainingFileIndex = 1;
            foreach (var file in files.OrderBy(x => x.DateTaken))
            {
                fileService.CopyFile(
                    file.File.FullName,
                    Settings.UnknownDirectory,
                    $"{remainingFileIndex}.jpg");

                remainingFileIndex++;
            }
        }

        private static IList<FileWithMetadata> GetFiles()
        {
            var fileService = new FileService();
            var files = fileService.GetAllFiles(Settings.InputDirectory);
            foreach (var file in files)
            {
                if (file.DateTaken < Settings.TripBeginDate || file.DateTaken > Settings.TripEndDate)
                {
                    file.DateTaken = DateTimeOffset.MaxValue;
                }
            }
            return files;
        }

        private static async Task<List<Location>> GetLocations()
        {
            var geoCoder = new GeoCoder();
            var locations = new List<Location>();
            foreach (var locationName in Settings.Locations)
            {
                locations.Add(
                    new Location
                    {
                        Name = locationName,
                        Coordinates = await geoCoder.GetCoordinates(locationName),
                        Files = new List<FileWithMetadata>()
                    });
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            return locations;
        }
    }
}
