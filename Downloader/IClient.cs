namespace Downloader
{
    public interface IClient
    {
        bool DownloadFile(RemoteFile file);
    }
}
