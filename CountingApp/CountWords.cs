using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CountingApp
{
    public static class CountWords
    {
        [FunctionName("CountWords")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Input input = JsonConvert.DeserializeObject<Input>(requestBody);

            log.LogInformation(JsonConvert.SerializeObject(input));

            foreach (var value in input.values){

                
                var sentence = value.data.mergedText;

                log.LogInformation("sentence" + sentence);

                // convert to lowercase
                string text = sentence.ToLowerInvariant();
                List<string> words = text.Split(' ').ToList();

                //remove any non alphabet characters
                var onlyAlphabetRegEx = new Regex(@"^[A-z]+$");
                words = words.Where(f => onlyAlphabetRegEx.IsMatch(f)).ToList();

                // Array of stop words to be ignored
                string[] stopwords = { "", "i", "me", "my", "myself", "we", "our", "ours", "ourselves", "you", 
                    "youre", "youve", "youll", "youd", "your", "yours", "yourself", 
                    "yourselves", "he", "him", "his", "himself", "she", "shes", "her", 
                    "hers", "herself", "it", "its", "itself", "they", "them", "thats",
                    "their", "theirs", "themselves", "what", "which", "who", "whom", 
                    "this", "that", "thatll", "these", "those", "am", "is", "are", "was",
                    "were", "be", "been", "being", "have", "has", "had", "having", "do", 
                    "does", "did", "doing", "a", "an", "the", "and", "but", "if", "or", 
                    "because", "as", "until", "while", "of", "at", "by", "for", "with", 
                    "about", "against", "between", "into", "through", "during", "before", 
                    "after", "above", "below", "to", "from", "up", "down", "in", "out", 
                    "on", "off", "over", "under", "again", "further", "then", "once", "here", 
                    "there", "when", "where", "why", "how", "all", "any", "both", "each", 
                    "few", "more", "most", "other", "some", "such", "no", "nor", "not", 
                    "only", "own", "same", "so", "than", "too", "very", "s", "t", "can", 
                    "will", "just", "don", "dont", "should", "shouldve", "now", "d", "ll",
                    "m", "o", "re", "ve", "y", "ain", "aren", "arent", "couldn", "couldnt", 
                    "didn", "didnt", "doesn", "doesnt", "hadn", "hadnt", "hasn", "hasnt", 
                    "havent", "isn", "isnt", "ma", "mightn", "mightnt", "mustn", 
                    "mustnt", "needn", "neednt", "shan", "shant", "shall", "shouldn", "shouldnt", "wasn", 
                    "wasnt", "weren", "werent", "won", "wont", "wouldn", "wouldnt"}; 
                
                 words = words.Where(x => !stopwords.Contains(x)).ToList();

                // Find distict keywords by key and count, and then order by count.
                var keywords = words.GroupBy(x => x).OrderByDescending(x => x.Count());
                var klist = keywords.ToList();
                var numofWords = 10;
                if(klist.Count<10)
                    numofWords=klist.Count;


                // Return the first 10 words
                List<string> resList = new List<string>();
                for (int i = 0; i < numofWords; i++)
                {
                    resList.Add(klist[i].Key);
                }

                value.data.words = new List<string>();
                value.data.words = resList;

            }

            log.LogInformation("OUTPUT:");
            log.LogInformation(JsonConvert.SerializeObject(input));

            // return the results object
            return new OkObjectResult(input);
        }
    }


}
