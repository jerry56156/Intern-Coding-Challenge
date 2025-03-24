using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

// class for sensor reading (id, latitude, longitude)
public class SensorReading
{
    public int Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class Program
{
    static void Main(string[] args)
    {
        // error Checking input format
        if (args.Length != 2)
        {
            Console.WriteLine("Wrong usage\nUsage: dotnet run <csvFilePath> <jsonFilePath>");
            return;
        }

        string csvFilePath = args[0];
        string jsonFilePath = args[1];

        // read input files
        var sensor1Readings = ReadCsvFile(csvFilePath);
        var sensor2Readings = ReadJsonFile(jsonFilePath);


        // output dictionary
        Dictionary<int, int> matches = new Dictionary<int, int>();

        // iterate through data from both files to find matching coordinates
        foreach (var s1 in sensor1Readings)
        {
          // skip invalid coordinates
          if (s1.Latitude < -90 || s1.Latitude > 90 || s1.Longitude < -180 || s1.Longitude > 180)
          {
            continue;
          }

            double minDistance = double.MaxValue;
            int bestMatchId = -1;

            foreach (var s2 in sensor2Readings)
            {
              // skip invalid coordinates
                if (s2.Latitude < -90 || s2.Latitude > 90 || s2.Longitude < -180 || s2.Longitude > 180)
                {
                  continue;
                }
            
                double distance = HaversineDistance(s1.Latitude, s1.Longitude, s2.Latitude, s2.Longitude);
                if (distance <= 100.00 && distance < minDistance)
                {
                    minDistance = distance;
                    bestMatchId = s2.Id;
                }
            }

            // if mathing coordinates were found add to output
            if (bestMatchId != -1)
            {
                matches[s1.Id] = bestMatchId;
            }
        }

        // serialize the output dictionary to JSON
        string outputJson = JsonSerializer.Serialize(matches, new JsonSerializerOptions { WriteIndented = true });
        
        // Create/Overwrite output.json
        File.WriteAllText("Output.json", outputJson);
    }


    // read sensor data from CSV
    static List<SensorReading> ReadCsvFile(string filePath)
    {
        List<SensorReading> readings = new List<SensorReading>();

        using (var reader = new StreamReader(filePath))
        {
            // skip header
            string headerLine = reader.ReadLine();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(',');
                if (parts.Length < 3)
                    continue;

                // parse
                if (int.TryParse(parts[0], out int id) &&
                    double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double lat) &&
                    double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double lon))
                {
                    readings.Add(new SensorReading { Id = id, Latitude = lat, Longitude = lon });
                }
            }
        }
        return readings;
    }


    // read sensor data from JSON 
    static List<SensorReading> ReadJsonFile(string filePath)
    {
        string jsonContent = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        };
        return JsonSerializer.Deserialize<List<SensorReading>>(jsonContent, options);
    }


    // calculate Haversine distance between 2 pts
    static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6378137; // radius of the earth (metres)
        double radLat1 = DegreesToRadians(lat1);
        double radLat2 = DegreesToRadians(lat2);
        double deltaLat = DegreesToRadians(lat2 - lat1);
        double deltaLon = DegreesToRadians(lon2 - lon1);

        double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                   Math.Cos(radLat1) * Math.Cos(radLat2) *
                   Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    // degrees to radians conversion
    static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }
}
