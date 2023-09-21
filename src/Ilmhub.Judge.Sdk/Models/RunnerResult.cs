using System.Security.Cryptography;
using System.Text;
using Ilmhub.Judge.Sdk.Abstraction.Models;
using Ilmhub.Judge.Wrapper.Abstractions.Models;

namespace Ilmhub.Judge.Sdk;

public class RunnerResult : IRunnerResult
{
    public RunnerResult(IExecutionResult execution) => Execution = execution;
    public IExecutionResult Execution { get; set; }
    public string Output { get; set; }
    public string Log { get; set; }
    public string Error { get; set; }
    public bool IsSuccess => 
        Execution.Status is EExecutionResult.Success &&
        Execution.Error is EExecutionError.NoError;
    public string OutputMd5 
    {
        get
        {
            if(string.IsNullOrWhiteSpace(Output))
                return null;

            using MD5 hasher = MD5.Create();
            byte[] bytes = Encoding.ASCII.GetBytes(Output.Trim());
            return Convert.ToHexString(hasher.ComputeHash(bytes));    
        } 
    }
    
}
