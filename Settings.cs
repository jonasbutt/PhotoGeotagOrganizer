using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoGeotagOrganizer
{
    public static class Settings
    {
        public static string LocationIqApiKey { get; } = ""; // TODO

        public static double GroupingRadiusInKilometers { get; } = 25000;
                
        public static string WorkingDirectory { get; } = "D:\\Desktop\\images";
                
        public static string InputFolder { get; } = "input";

        public static string InputDirectory { get; } = Path.Combine(WorkingDirectory, InputFolder);

        public static string OutputFolder { get; } = "output";
                
        public static string OutputDirectory { get; } = Path.Combine(WorkingDirectory, OutputFolder);

        public static string UnknownFolder { get; } = "unknown";

        public static string UnknownDirectory { get; } = Path.Combine(OutputDirectory, UnknownFolder);
                
        public static List<string> Locations { get; } = Resources.Locations.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                
        public static DateTimeOffset TripBeginDate { get; } = new DateTimeOffset(new DateTime(2018, 10, 7));
                
        public static DateTimeOffset TripEndDate { get; } = new DateTimeOffset(new DateTime(2019, 5, 1));
    }
}