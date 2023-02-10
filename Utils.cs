using System.Diagnostics;
using System.Drawing;

namespace FSC
{
    public static class UtilsAndStuff
    {
        public const string Greeting =
            "This command will attempt to start Docker Desktop (if it's not running) and a specified docker container.\n" +
            "The container name has to start with an alphanumeric character.\n" +
            "Usage:\n" +
            "fsc some-container-name-here\n";

        public const string DefaultDockerDesktopExePath = "C:\\Program Files\\Docker\\Docker\\Docker Desktop.exe";

        private static string GetDockerDesktopProcessName() => DefaultDockerDesktopExePath.Split("\\").Last().Replace(".exe", "");

        public static string GetDockerCliPath()
        {
            var process = Process.Start(new ProcessStartInfo()
            {
                FileName = "where",
                Arguments = "docker.exe",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            });

            if (process == null)
            {
                throw new Exception("Process is null");
            }

            var result = process.StandardOutput.ReadToEnd();
            return !result.Contains("INFO: Could not find files for the given pattern(s).")
                ? result.Split("\n").First()
                : "";
        }

        public static bool DockerDesktopIsRunning()
        {
            var processName = GetDockerDesktopProcessName();
            var asdf = Process.GetProcessesByName(GetDockerDesktopProcessName().Replace(".exe", ""));
            Console.WriteLine();
            return asdf.Any();
        }
        
        public static void TryStartingDockerContainer(string containerName, int timeoutMilliseconds)
        {
            var watch = new Stopwatch();
            watch.Start();
            while (watch.Elapsed.TotalMilliseconds < timeoutMilliseconds)
            {
                var process = StartDockerContainerProcess(containerName);
                var output = process.StandardOutput.ReadToEnd().Trim();
                if (output.Equals(containerName))
                {
                    break;
                }
            }
        }

        private static Process? StartDockerContainerProcess(string containerName) => Process.Start(new ProcessStartInfo()
        {
            Arguments = $"start {containerName}",
            FileName = GetDockerCliPath(),
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        });

        public static Process? StartDockerDesktop()
        {
            if (!File.Exists(DefaultDockerDesktopExePath))
            {
                throw new FileNotFoundException("Docker Desktop exe path points to a nonexisting file.");
            }

            return Process.Start(new ProcessStartInfo()
            {
                FileName = DefaultDockerDesktopExePath,
                UseShellExecute = true,
            });
        }
    }

    public enum ContainerStartResult
    {
        ItsRunning = 0,
        DockerDaemonIsntRunning = 1,
        SomethingWentWrong = 2,
        DidntExitBeforeTimeout = 3
    }
}
