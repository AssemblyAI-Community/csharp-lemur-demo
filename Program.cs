using AssemblyAI;
using AssemblyAI.Lemur;
using AssemblyAI.Transcripts;

const string apiKey = "API_KEY"; // Replace with your actual API key
const string filePath = "FILE_PATH"; // Replace with the path to your audio file

var client = new AssemblyAIClient(apiKey);
await using var fileStream = File.OpenRead(filePath);
var transcript = await client.Transcripts.TranscribeAsync(fileStream);
transcript.EnsureStatusCompleted();

const string prompt = """
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

var taskResponse = await client.Lemur.TaskAsync(new LemurTaskParams
{
    TranscriptIds = [transcript.Id],
    Prompt = prompt,
    FinalModel = LemurModel.AnthropicClaude3_5_Sonnet
});

Console.WriteLine(taskResponse.Response);