namespace Ilmhub.Judge.Api.Services;
public interface IZipFileArchive
{
    ValueTask<(bool, string)> ZipArchiveValidationAsync(Stream streamfile, CancellationToken cancellationToken = default);
}