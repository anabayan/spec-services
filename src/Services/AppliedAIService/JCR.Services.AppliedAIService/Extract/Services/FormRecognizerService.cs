using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace JCR.Services.AppliedAIService.Extract.Services;

public class FormRecognizerService
{
    private ILogger<FormRecognizerService> _logger;

    public FormRecognizerService(ILogger<FormRecognizerService> logger)
    {
        _logger = logger;
    }

    public async Task<ObservationModel> ExtractObservationInfo(string fileName)
    {
        var credential = new AzureKeyCredential("3c8e94f852404bf196bc5a0c0a7cdf08");
        var endpoint = "https://centralus.api.cognitive.microsoft.com/";
        var modelId = "FormModel_v35";

        var documentUrl = $"https://stobservationforms.blob.core.windows.net/uploads/{fileName}";

        var fileUri = new Uri(documentUrl);
        var client = new DocumentAnalysisClient(new Uri(endpoint), credential);
        var operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, modelId, fileUri);
        var result = operation.Value;

        var observationModel = new ObservationModel();

        foreach (var document in result.Documents)
        {
            Console.WriteLine($"Document of type: {document.DocumentType}");

            foreach (KeyValuePair<string, DocumentField> fieldKvp in document.Fields)
            {
                var fieldName = fieldKvp.Key;
                var field = fieldKvp.Value;

                var propertyInfo = typeof(ObservationModel).GetProperty(fieldName);
                if (propertyInfo?.CanWrite == true)
                    propertyInfo.SetValue(observationModel, field.Content);
            }
        }

        return observationModel;
    }
}

public class ObservationModel
{
    public string SurveyTeam { get; set; }
    public string StaffInterviewed { get; set; }
    public string ObservationDate { get; set; }
    public string DepartmentLevel1 { get; set; }
    public string DepartmentLevel2 { get; set; }
    public string DepartmentLevel3 { get; set; }
    public string TotalCompletedObservations { get; set; }
    public string ObservationNote { get; set; }
    public string ObservationTitle { get; set; }
    public string Location { get; set; }
    public string EquipmentObserved { get; set; }
    public string MedicalStaff { get; set; }
}
