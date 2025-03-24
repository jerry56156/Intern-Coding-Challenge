# CUAVs-Coding-Challenge

Challenge Overview:

At Canadian UAVs, we handle large amounts of geospatial data, which is the focus of this challenge. The task involves correlating data from two sensors that detect anomalies. However, the sensors are not highly accurate, resulting in false positives and variations in their location readings. Your challenge is to associate the sensor readings based on their coordinates to identify common signals that may have been detected by both sensors. This correlation increases the likelihood that the signal is a genuine detection rather than a false positive.

Input Data:

The two sensors provide different output formats: one sensor outputs data in CSV format, and the other outputs data in JSON format. Please refer to the sample data for the exact format of each sensor's output. Both sensors assign a unique ID to each reading, but note that different sensors may use the same IDs. The sensor readings include location coordinates in decimal degrees, using the WGS 84 format, representing where the anomaly was detected. The sensors have an accuracy of 100 meters, meaning that the reported location is within 100 meters of the actual anomaly location.

Output:

The output should consist of pairs of IDs, where one ID is from the first sensor, and the second ID is from the second sensor.



# My Solution
Code written in C# in Program.cs file

The input CSV and JSON files are parsed with their respective functions. Then I iterate through both in a nested loop, discarding any invalid coordinates, and calculating the Haversine distance between each pair to find the coordinates that are within 100m of each other.

To run the program: dotnet run {csvFilePath} {jsonFilePath}  