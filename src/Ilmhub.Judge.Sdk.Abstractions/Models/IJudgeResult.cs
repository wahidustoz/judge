﻿using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Abstractions;

public interface IJudgeResult
{
    bool IsSuccess { get; }
    ICompilationResult Compilation { get; }
    IEnumerable<ITestCaseResult> TestCases { get; }
}