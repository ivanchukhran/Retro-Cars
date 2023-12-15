namespace RetroCarsWebApp.Services;

public class FileInteractor
{
    public static void CheckOrCreateFile(string path)
    {
        if (!File.Exists(path)) File.Create(path).Close();
    }

    public static Task<string> ReadAsync(string path)
    {
        CheckOrCreateFile(path);
        return Task.Run(() => File.ReadAllText(path));
    }

    public static Task WriteAsync(string path, string content)
    {
        CheckOrCreateFile(path);
        return Task.Run(() => File.WriteAllText(path, content));
    }
}