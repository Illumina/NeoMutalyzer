namespace GenBankDownloader
{
    public interface IClient
    {
        bool DownloadFile(RemoteFile file);
    }
}
