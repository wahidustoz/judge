using System.ComponentModel;
using Ilmhub.Judge.Abstractions.Models;

namespace Ilmhub.Judge.Models;

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