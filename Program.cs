Synchronizer synchronizer = new Synchronizer("source", "destination", 10);
// Synchronizer synchronizer = new Synchronizer("", "", 10);

Task syncTask = synchronizer.Start();

while (synchronizer.isRunning)
{
    Menu(synchronizer);

    string option = Console.ReadLine() ?? "";
    if (option == "0") break;

    switch (option)
    {
        case "1":
            Console.Write("New source path: ");
            synchronizer.sourceFolder = Console.ReadLine() ?? "";

            break;

        case "2":
            Console.Write("New destination path: ");
            synchronizer.destinationFolder = Console.ReadLine() ?? "";
            break;

        case "3":

            Console.Write("New interval value: ");

            if (int.TryParse(Console.ReadLine() ?? "", out int value))
            {
                Console.WriteLine($"intervalo lido: {value}");
                synchronizer.Stop();
                await syncTask;
                synchronizer = new Synchronizer(synchronizer, value);
                syncTask = synchronizer.Start();
            }

            break;

        default:
            Console.WriteLine("Invalid Options.");
            continue;
    }
}

synchronizer.Stop();
await syncTask;
Console.WriteLine("Program Ending...");
void Menu(Synchronizer syncronizer)
{
    Console.WriteLine();
    Console.WriteLine($"Current Source:      {synchronizer.sourceFolder}");
    Console.WriteLine($"Current Destination: {synchronizer.destinationFolder}");
    Console.WriteLine($"Current Interval:    {synchronizer.syncInterval} s");
    Console.WriteLine();
    Console.WriteLine("1 - Update source");
    Console.WriteLine("2 - Update destination");
    Console.WriteLine("3 - Update interval");
    Console.WriteLine("0 - Exit");
    Console.Write("> ");
}

