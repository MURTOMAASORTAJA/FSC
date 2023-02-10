using FSC;
using static FSC.UtilsAndStuff;

if (!args.Any() || args[0].StartsWith("-"))
{
    Console.WriteLine(Greeting);
    Environment.Exit(0);
}

if (!DockerDesktopIsRunning())
{
    Console.WriteLine("Docker Desktop isn't running.");
    var result = StartDockerDesktop();
    Console.WriteLine("Started Docker Desktop.");
}

TryStartingDockerContainer(args[0], 60000);

Console.WriteLine($"Started container.");