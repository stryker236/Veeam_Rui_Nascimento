public class Synchronizer
{
    private string _sourceFolder = "";
    private string _destinationFolder = "";
    private int _syncInterval = 2;
    private PeriodicTimer time;
    public bool isRunning = true;

    public string sourceFolder
    {
        get => _sourceFolder;
        set
        {
            if (!Directory.Exists(value)) Console.WriteLine("Folder doesnt exist");
            else if (Path.GetFullPath(value) == this.destinationFolder) Console.WriteLine("Cant be the same folder as destination");
            else _sourceFolder = Path.GetFullPath(value);
        }
    }
    public string destinationFolder
    {
        get => _destinationFolder;
        set
        {
            if (!Directory.Exists(value)) Console.WriteLine("Folder doesnt exist");
            else if (Path.GetFullPath(value) == this.sourceFolder) Console.WriteLine("Cant be the same folder as ");
            else _destinationFolder = Path.GetFullPath(value);
        }
    }
    public int syncInterval
    {
        get => _syncInterval;
        set
        {
            if (value <= 0) Console.WriteLine($"Invalid interval number. Must be greater that 0");
            else _syncInterval = value;
        }
    }

    public Synchronizer(string sourceFolder, string destinationFolder, int syncInterval)
    {
        this.sourceFolder = sourceFolder;
        this.destinationFolder = destinationFolder;
        this.syncInterval = syncInterval;

        this.time = new PeriodicTimer(TimeSpan.FromSeconds(this.syncInterval));
    }

    public Synchronizer(Synchronizer syncronizer, int syncInterval)
    {
        this.sourceFolder = syncronizer.sourceFolder;
        this.destinationFolder = syncronizer.destinationFolder;
        this.syncInterval = syncInterval;

        this.time = new PeriodicTimer(TimeSpan.FromSeconds(this.syncInterval));
    }

    public async Task Start()
    {
        // prints whats missing
        if (!IsMissingOrInvaluidValues())
        {
            Console.WriteLine("Starting synchronization...");
            Console.WriteLine($"Source Folder: {this.sourceFolder}");
            Console.WriteLine($"Destination Folder: {this.destinationFolder}");
            Console.WriteLine($"Sync Interval: {this.syncInterval} seconds");
            Console.WriteLine("\n");

            Sync();

            while (await time.WaitForNextTickAsync())
            {
                Sync();
            }
        }
        this.isRunning = false;
    }


    public void Stop()
    {
        Console.WriteLine("Stopping synchronization...");
        this.time.Dispose();
    }

    public void Sync()
    {
        Console.WriteLine("Synchronizing files...");

        List<string> sourceFiles = Directory.EnumerateFiles(this.sourceFolder, "*", SearchOption.AllDirectories).ToList();
        List<string> sourceDirectories = Directory.EnumerateDirectories(this.sourceFolder, "*", SearchOption.AllDirectories).ToList();

        List<string> destFiles = Directory.EnumerateFiles(this.destinationFolder, "*", SearchOption.AllDirectories).ToList();
        List<string> destDirectories = Directory.EnumerateDirectories(this.destinationFolder, "*", SearchOption.AllDirectories).ToList();

        CreateDestDirectories(sourceDirectories);
        CreateDestFiles(sourceFiles);
        RemoveSourceFiles(destFiles);
        RemoveDestDirectories(destDirectories);
    }

    void CreateDestDirectories(List<string> sourceDirectories)
    {
        foreach (string sourceDirectory in sourceDirectories)
        {
            string sourceDirectoryRelative = Path.GetRelativePath(this.sourceFolder, sourceDirectory);
            string destDirectoryToCheck = Path.Combine(this.destinationFolder, sourceDirectoryRelative);

            if (!Directory.Exists(destDirectoryToCheck))
            {
                Directory.CreateDirectory(destDirectoryToCheck);
                Console.WriteLine($"Created directory: {destDirectoryToCheck}");
            }
        }
    }

    void CreateDestFiles(List<string> sourceFiles)
    {
        foreach (string sourceFile in sourceFiles)
        {
            string sourcePathRelative = Path.GetRelativePath(this.sourceFolder, sourceFile);
            string destFileToCheck = Path.Combine(this.destinationFolder, sourcePathRelative);

            if (!File.Exists(destFileToCheck) || IsDifferent(sourceFile, destFileToCheck))
            {
                File.Copy(sourceFile, destFileToCheck, true);
                Console.WriteLine($"Copied File: {sourceFile} to {destFileToCheck}");
            }
        }
    }

    void RemoveSourceFiles(List<string> destFiles)
    {
        foreach (string destFile in destFiles)
        {
            string destPathRelative = Path.GetRelativePath(this.destinationFolder, destFile);
            string sourceAbsPathToCheck = Path.Combine(this.sourceFolder, destPathRelative);

            if (!File.Exists(sourceAbsPathToCheck))
            {
                File.Delete(destFile);
                Console.WriteLine($"Deleted file: {destFile}");
            }
        }
    }

    void RemoveDestDirectories(List<string> destDirectories)
    {
        foreach (string destDirectory in destDirectories)
        {
            string destPathRelative = Path.GetRelativePath(this.destinationFolder, destDirectory);
            string sourceAbsPathToCheck = Path.Combine(this.sourceFolder, destPathRelative);

            if (!Directory.Exists(sourceAbsPathToCheck))
            {
                Directory.Delete(destDirectory);
                Console.WriteLine($"Deleted directory: {destDirectory}");
            }
        }
    }

    bool IsDifferent(string sourceFile, string destFile)
    {
        FileInfo sourceFileInfo = new FileInfo(sourceFile);
        FileInfo destFileInfo = new FileInfo(destFile);
        return sourceFileInfo.Length != destFileInfo.Length || sourceFileInfo.LastWriteTime != destFileInfo.LastWriteTime;
    }

    bool IsMissingOrInvaluidValues()
    {
        List<string> missing = new List<string>();

        if (string.IsNullOrEmpty(sourceFolder)) missing.Add("sourceFolder");
        if (string.IsNullOrEmpty(destinationFolder)) missing.Add("destinationFolder");

        if (missing.Count == 0)
        {
            return false;
        }
        Console.WriteLine($"Values missing or invalid: {string.Join(", ", missing)}");
        Console.WriteLine("Go to line 1 in Program.cs and set them properly");
        return true;
    }

}