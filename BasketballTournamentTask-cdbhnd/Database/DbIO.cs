using BasketballTournamentTask_cdbhnd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BasketballTournamentTask_cdbhnd.Database
{
    internal class DbIO
    {

        public static void SerializeToJsonFile<TKey, TValue>(string filePath, Dictionary<TKey, TValue> objects) where TKey : notnull
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new JsonDateOnlyConverter());

            string jsonObj = JsonSerializer.Serialize(objects, options);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(jsonObj);
            }
        }
        
        public static Dictionary<TKey, TValue>? DeserializeFromJsonFile<TKey, TValue>(string filePath) where TKey : notnull
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new JsonDateOnlyConverter());

            Dictionary<TKey, TValue>? objects = new Dictionary<TKey, TValue>();

            string jsonString = "";

            try
            {
                jsonString = File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem reading files.");
                Console.WriteLine(e.Message);
            }

            try
            {
                objects = JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(jsonString, options);
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem parsing json.");
                Console.WriteLine(e.Message);
            }

            return objects;
        }
        
    }
}
