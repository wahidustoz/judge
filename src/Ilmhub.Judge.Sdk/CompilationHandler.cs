using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Abstractions.Models;
using Ilmhub.Judge.Sdk.Exceptions;
using Microsoft.Extensions.Logging;

namespace Ilmhub.Judge.Sdk;

public class CompilationHandler : ICompilationHandler
{
    private readonly ILogger<CompilationHandler> logger;
    private readonly IEnumerable<ICompiler> compilers;

    public CompilationHandler(ILogger<CompilationHandler> logger, IEnumerable<ICompiler> compilers)
    {
        this.logger = logger;
        this.compilers = compilers;
    }
    public async ValueTask<ICompilationResult> CompileAsync(string source, int languageId, string environmentFolder = null, CancellationToken cancellationToken = default)
    {
        foreach(var compiler in compilers)
            if(compiler.CanHandle(languageId))
                return await compiler.CompileAsync(source, languageId, environmentFolder, cancellationToken);
        
        throw new CompilerNotFoundException(languageId);
    }
}
