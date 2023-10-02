
using System.IO.Compression;

namespace Ilmhub.Judge.Api.Services;
public class ZipFileArchive : IZipFileArchive
{
    public async ValueTask<(bool, string)> ZipArchiveValidationAsync(Stream streamfile, CancellationToken cancellationToken = default)
    {
        bool returnStatus = false;
        string errorMessage = "";
        using (var zip = new ZipArchive(streamfile, ZipArchiveMode.Read))
        {
            if (zip.Entries.ToList().Where(x => x.Name.EndsWith(".in") || x.Name.EndsWith(".out")).Count() < 200)
            {
                if (zip.Entries.ToList().Where(x => x.Name.EndsWith(".in")).Count() == zip.Entries.ToList().Where(x => x.Name.EndsWith(".out")).Count())
                {
                    var testcasesOutput = zip.Entries.ToList().Where(x => x.Name.EndsWith(".out"));
                    foreach (var entry in zip.Entries.ToList().Where(x => x.Name.EndsWith(".in")))
                    {
                        if (testcasesOutput.Any(x => x.Name.EndsWith(".out").Equals(entry.Name.EndsWith(".in"))))
                        {
                            returnStatus = true;
                        }
                        else
                        {
                            returnStatus = false;
                            errorMessage = "Zip file is not valid.";
                            break;
                        }
                    }
                }
                else
                {
                    returnStatus = false;
                    errorMessage = "Zip file is not valid.";
                }
            }
            else
            {
                returnStatus = false;
                errorMessage = "Zip file is not valid.";
            }
        }
        return (returnStatus, errorMessage);
    }
}