using AirtableApiClient;

namespace BlazorApp
{
    public interface IAirtableService
    {
        Task<List<string>> GetData();
    }

    public class AirtableService : IAirtableService
    {
        readonly string baseId = "appCFFNWJkTCIg8kI";
        readonly string appKey = "patJU7sCbROmDa8rH.ceddd12a85144dc29d3cf6373a2a7bf4bdd3dfa5a8c0c3c0aade1213e90b3e68";
        readonly string tableId = "tblrlkMfT33IsAUvG";
        readonly string fieldName = "Phrase";
        public async Task<List<string>> GetData()
        {
            var result = new List<string>();
            string offset = null;
            string errorMessage = null;
            var records = new List<AirtableRecord>();

            using (AirtableBase airtableBase = new AirtableBase(appKey, baseId))
            {
                do
                {
                    Task<AirtableListRecordsResponse> task = airtableBase.ListRecords(tableId);

                    AirtableListRecordsResponse response = await task;

                    if (response.Success)
                    {
                        records.AddRange(response.Records.ToList());
                        offset = response.Offset;
                    }
                    else if (response.AirtableApiError is AirtableApiException)
                    {
                        errorMessage = response.AirtableApiError.ErrorMessage;
                        if (response.AirtableApiError is AirtableInvalidRequestException)
                        {
                            errorMessage += "\nDetailed error message: ";
                            errorMessage += response.AirtableApiError.DetailedErrorMessage;
                        }
                        break;
                    }
                    else
                    {
                        errorMessage = "Unknown error";
                        break;
                    }
                } while (offset != null);
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                // Error reporting
                throw new Exception(errorMessage);
            }
            else
            {
                foreach (var r in records)
                {
                    string value = r.GetField(fieldName).ToString();
                    result.Add(value);
                }
            }
            return result;
        }
    }
}
