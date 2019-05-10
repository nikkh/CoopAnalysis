using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CoopAnalysis
{
    class Engine
    {

        public void Run(string[] args)
        {
            // take poscode from command line


            string inputFile = args[0];
            
            try
            {
                using (StreamReader sr = new StreamReader(inputFile))
                {
                    while (!sr.EndOfStream)
                    {
                        string postcode = sr.ReadLine();
                        ProcessPostcode(postcode);
                        Console.WriteLine();
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"File {inputFile} could not be read:");
                Console.WriteLine(e.Message);
                throw;
            }
            Console.ReadLine();

        }
            private void ProcessPostcode(string postcode)
            {

            JObject locationResults = null;
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://api.postcodes.io/");
                try
                {
                    string response = httpClient.GetStringAsync($"postcodes/{postcode}").Result;
                    locationResults = JObject.Parse(response);
                    
                }
                catch (Exception ex)
                {
                    // Details in ex.Message and ex.HResult.
                    throw;
                }
            }
            // legible for debugging
            var temp = locationResults.ToString();
            string latitude = locationResults["result"]["latitude"].ToString();
            string longitude = locationResults["result"]["longitude"].ToString();
            string ward = locationResults["result"]["admin_ward"].ToString();

            Console.WriteLine($"Postcode={postcode}");
            Console.WriteLine($"Local Government Ward={ward}");
            Console.WriteLine($"Latitude={latitude}");
            Console.WriteLine($"Longitude={longitude}");
        
            JArray crimeResults = null;
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://data.police.uk/");
                try
                {
                    string response = httpClient.GetStringAsync($"api/crimes-street/all-crime?lat={latitude}&lng={longitude}").Result;
                    crimeResults = JArray.Parse(response);
                }
                catch
                {
                    // Details in ex.Message and ex.HResult.
                    throw;
                }
            }
            

            temp = crimeResults.ToString();
            Dictionary<string, int> crimes = new Dictionary<string, int>();

            foreach (var item in crimeResults)
            {
                string crimeCategory = item["category"].ToString();
                if (!crimes.ContainsKey(crimeCategory))
                {
                    crimes.Add(crimeCategory, 1);
                }
                else
                {
                    crimes[crimeCategory]++;
                }
            }

            foreach (var crime in crimes)
            {
                Console.WriteLine($"Crime: {crime.Key} = {crime.Value}");
            }

            Console.WriteLine($"Total Crimes: {crimes.Sum(c => c.Value)}");

           
        }

        
           
        }
    }

