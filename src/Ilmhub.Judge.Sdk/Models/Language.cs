using System.ComponentModel;
using Ilmhub.Judge.Sdk.Abstractions;

namespace Ilmhub.Judge.Sdk.Models;

public class Language : ILanguage
{
    public Language(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; }

    public string Name { get; }
}