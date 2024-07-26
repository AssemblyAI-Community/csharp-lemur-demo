using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AssemblyAIDemo
{
    class Program
    {
        private static readonly string apiKey = "API_KEY"; // Replace with your actual API key

        static async Task Main(string[] args)
        {
            string filePath = "FILE_PATH"; // Replace with the path to your audio file
            string uploadUrl = await UploadFileAsync(apiKey, filePath);

            if (!string.IsNullOrEmpty(uploadUrl))
            {
                string transcriptId = await SubmitTranscriptionAsync(apiKey, uploadUrl);
                if (!string.IsNullOrEmpty(transcriptId))
                {
                    string transcriptText = await PollForTranscriptAsync(apiKey, transcriptId);
                    if (!string.IsNullOrEmpty(transcriptText))
                    {
                        string summary = await GenerateSummaryAsync(apiKey, transcriptId);
                        Console.WriteLine(summary);
                    }
                }
            }
        }

        public static async Task<string?> UploadFileAsync(string apiKey, string path)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(apiKey);

            using var fileContent = new ByteArrayContent(File.ReadAllBytes(path));
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync("https://api.assemblyai.com/v2/upload", fileContent);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error: {e.Message}");
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseBody);
                return json["upload_url"].ToString();
            }
            else
            {
                Console.Error.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }
        }

        public static async Task<string?> SubmitTranscriptionAsync(string apiKey, string uploadUrl)
        {
            var data = new Dictionary<string, dynamic>
            {
                { "audio_url", uploadUrl }
            };

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("authorization", apiKey);
            var content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.assemblyai.com/v2/transcript", content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = JsonConvert.DeserializeObject<dynamic>(responseContent);
                return responseJson.id.ToString();
            }
            else
            {
                Console.Error.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }
        }

        public static async Task<string> PollForTranscriptAsync(string apiKey, string transcriptId)
        {
            string pollingEndpoint = $"https://api.assemblyai.com/v2/transcript/{transcriptId}";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("authorization", apiKey);

            while (true)
            {
                var pollingResponse = await client.GetAsync(pollingEndpoint);
                var transcriptionResult = JObject.Parse(await pollingResponse.Content.ReadAsStringAsync());

                if (transcriptionResult["status"].ToString() == "completed")
                {
                    return transcriptionResult["text"].ToString();
                }
                else if (transcriptionResult["status"].ToString() == "error")
                {
                    throw new Exception($"Transcription failed: {transcriptionResult["error"]}");
                }
                else
                {
                    Thread.Sleep(3000); // Wait for 3 seconds before polling again
                }
            }
        }

        public static async Task<string?> GenerateSummaryAsync(string apiKey, string transcriptId)
        {
            string prompt = """
                            - You are an expert at writing factual, useful summaries.
                            - You focus on key details, leave out irrelevant information, and do not add in information that is not already present in the transcript.
                            - Your summaries accurately represent the information in the transcript.
                            - You are useful to the reader, are true and concise, and are written in perfect English.
                            - Use multiple parts of the transcript to form your summary.
                            - Make your summary follow the sequential order of events in the transcript.
                            - Your summaries do not describe the context of the transcript - they only summarize the events in the text.
                            - Your summaries do not describe what type of text they summarize.
                            - You do not dumb down specific language nor make big generalizations.
                            - Respond with just the summary and don't include a preamble or introduction.
                            """;

            var data = new Dictionary<string, dynamic>
            {
                { "transcript_ids", new string[] { transcriptId } },
                { "prompt", prompt },
                { "final_model", "anthropic/claude-3-5-sonnet" },
            };

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("authorization", apiKey);
            var content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.assemblyai.com/lemur/v3/generate/task", content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
                return result.response.ToString();
            }
            else
            {
                Console.Error.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }
        }
    }
}
