namespace NEWgIT.Infrastructure.Tests;

public class AnalysisRepositoryTests : IDisposable
{
    private readonly GitContext _context;
    private readonly AnalysisRepository _analysisRepository;

    private readonly Repository _gitRepository;

    private readonly string _path;

    public AnalysisRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<GitContext>();
        builder.UseSqlite(connection);
        var context = new GitContext(builder.Options);
        context.Database.EnsureCreated();
        _context = context;
        _analysisRepository = new AnalysisRepository(_context);

        _path = "./repo";

        Repository.Init(_path);
        _gitRepository = new Repository(_path).Seed();
    }

    public void Dispose()
    {
        _context.Dispose();
        Directory.Delete(_path, true);
    }


    [Fact]
    public void Creates_Analysis()
    {
        // Arrange
        var (commitDTOs, hash) = LibGitService.Instance.GetRepoCommits(_gitRepository);
        ICollection<CommitInfo> expectedCommits = _gitRepository.Commits.Select(commit => new CommitInfo(author: commit.Author.Name, date: commit.Committer.When.DateTime)).ToHashSet();

        var expectedAnalysis = new Analysis
        (
            repoIdentifier: _path,
            latestCommitHash: hash
        );

        var expectedIdentifier = _path;
        var expectedHash = hash;
        // expectedAnalysis.Commits = expectedCommits;

        // Act
        var (response, id) = _analysisRepository.Create(new AnalysisCreateDTO(
            repoIdentifier: _path,
            commits: commitDTOs,
            latestCommitHash: hash
        ));

        // Assert
        response.Should().Be(Response.Created);
        var analysis = _context.Analysis.Find(id)!;
        analysis.RepoIdentifier.Should().Be(expectedIdentifier);
        analysis.LatestCommitHash.Should().Be(expectedHash);
        analysis.Commits.Count.Should().Be(expectedCommits.Count);
    }
}
