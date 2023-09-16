﻿namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public interface ILanguageConfiguration
{
    string Name { get; set; }
    ICompileConfiguration Compile { get; set; }
    IRunConfiguration Run { get; set; }
}