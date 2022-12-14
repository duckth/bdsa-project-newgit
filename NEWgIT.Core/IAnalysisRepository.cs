namespace NEWgIT.Core;

public interface IAnalysisRepository
{
    (Response Response, int AnalysisId) Create(AnalysisCreateDTO analysis);
    IReadOnlyCollection<AnalysisDTO> Read();
    AnalysisDTO Find(int ID);

    AnalysisDTO FindByIdentifier(string repoIdentifier);

    Response Update(AnalysisUpdateDTO analysis);

    Response Delete(AnalysisDeleteDTO analysis);
}
