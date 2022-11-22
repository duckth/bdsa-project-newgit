
namespace NEWgIT.Infrastructure;
public class AnalysisRepository : IAnalysisRepository
{

    private readonly GitContext _context;

    public AnalysisRepository(GitContext context)
    {
        _context = context;
    }

    public void Create(AnalysisDTO analysisDTO)
    {
        var conflicts = _context.Analysis.Where(dbAnalysis => dbAnalysis.RepoIdentifier == analysis.repoIdentifier);

        if (conflicts.Any()) return (Response.Conflict, conflicts.First().Id);

        // map each dto to commitinfo object, as this is our model
        ICollection<CommitInfo> commits = analysis.commits.Select(dto => new CommitInfo(author: dto.author, date: dto.date, hash: dto.hash)).ToHashSet();

        var analysisObject = new Analysis(
            repoIdentifier: analysis.repoIdentifier,
            latestCommitHash: analysis.latestCommitHash
        );
        analysisObject.Commits = commits;

        _context.Analysis.Add(analysisObject);
        _context.SaveChanges();

        var response = Response.Created;
        // return TypedResult.

        return (response, analysisObject.Id);
    }

    public IReadOnlyCollection<AnalysisDTO> Read() =>
        _context.Analysis.Select(analysis => new AnalysisDTO(analysis.Id, analysis.RepoIdentifier, GetCommitDTOs(analysis), analysis.LatestCommitHash))
                          .ToArray();




    public AnalysisDTO Find(int ID)
    {
        var analysis = _context.Analysis.Find(ID);
        if (analysis == null) return null!;
        _context.Commits.Where(commit => commit.Analysis.Id == ID).Load();
        return new AnalysisDTO(analysis.Id, analysis.RepoIdentifier, GetCommitDTOs(analysis), analysis.LatestCommitHash);
    }

    public AnalysisDTO FindByIdentifier(string repoIdentifier)
    {
        var analysis = _context.Analysis.Where(analysis => analysis.RepoIdentifier == repoIdentifier).FirstOrDefault();
        if (analysis == null) return null!;
        return Find(analysis.Id);
    }

    public Response Update(AnalysisUpdateDTO analysis)
    {
        var analysisObject = _context.Analysis.FindByIdentifier(analysis.repoIdentifier);

        if (analysisObject == null) return Response.NotFound;
        if (analysisObject.LatestCommitHash == analysis.latestCommitHash) return Response.Ok;

        analysisObject.Commits.Clear();
        analysisObject.Commits = analysis.commits.Select(dto => new CommitInfo(author: dto.author, date: dto.date, hash: dto.hash)).ToHashSet();
        analysisObject.LatestCommitHash = analysis.latestCommitHash;
        _context.SaveChanges();

        return Response.Updated;
    }

    public Response Delete(AnalysisDeleteDTO analysis)
    {
        var analysisObject = _context.Analysis.FindByIdentifier(analysis.repoIdentifier);

        if (analysisObject is null) return Response.NotFound;

        _context.Analysis.Remove(analysisObject);
        _context.SaveChanges();
        return Response.Deleted;
    }

    private static ICollection<CommitDTO> GetCommitDTOs(Analysis analysis) => analysis
        .Commits.Select(commit => new CommitDTO(
            commit.Author, commit.Date, commit.Hash)
        ).ToHashSet();

    public async Task<Results<Created<AnalysisDTO>, Conflict<AnalysisDTO>>> CreateAsync(AnalysisDTO analysisDTO)
    {
        var conflict = await _context.Analysis.FirstOrDefaultAsync(dbAnalysis => dbAnalysis.RepoIdentifier == analysisDTO.RepoIdentifier());

        if (conflict is not null) return TypedResults.Conflict(new AnalysisDTO(conflict.Id, conflict.RepoIdentifier, GetCommitDTOs(conflict), conflict.LatestCommitHash));

        // map each dto to commitinfo object, as this is our model
        ICollection<CommitInfo> commits = analysis.commits.Select(dto => new CommitInfo(author: dto.author, date: dto.date, hash: dto.hash)).ToHashSet();

        var analysisObject = new Analysis(
            repoIdentifier: analysis.repoIdentifier,
            latestCommitHash: analysis.latestCommitHash
        );
        analysisObject.Commits = commits;

        _context.Analysis.Add(analysisObject);
        _context.SaveChanges();

        var response = Response.Created;
        // return TypedResult.

        return (response, analysisObject.Id);
    }

    public Task<IReadOnlyCollection<AnalysisDTO>> ReadAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Results<Ok<AnalysisDTO>, NotFound<int>>> FindAsync(int ID)
    {
        throw new NotImplementedException();
    }

    public Task<Results<Ok<AnalysisDTO>, NotFound<string>>> FindByIdentifierAsync(string repoIdentifier)
    {
        throw new NotImplementedException();
    }

    public Task<Results<NoContent, NotFound<int>>> UpdateAsync(AnalysisDTO analysis)
    {
        throw new NotImplementedException();
    }

    public Task<Results<NoContent, NotFound<int>>> DeleteAsync(AnalysisDTO analysis)
    {
        throw new NotImplementedException();
    }
}

