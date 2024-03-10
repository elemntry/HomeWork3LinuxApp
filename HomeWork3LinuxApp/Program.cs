using System.Diagnostics;


string passwdFilePath = "/etc/passwd";

string[] passwdLines = File.ReadAllLines(passwdFilePath);

Console.WriteLine("##################################");
Console.WriteLine("########Users information##########");
Console.WriteLine("##################################");
foreach (string line in passwdLines)
{
    string[] fields = line.Split(':');
    string username = fields[0];
    string uid = fields[2];
    string gid = fields[3];
    string homeDirectory = fields[5];
    string shell = fields[6];

    Console.WriteLine($"Username: {username}");
    Console.WriteLine($"UID: {uid}");
    Console.WriteLine($"GID: {gid}");
    Console.WriteLine($"Home Directory: {homeDirectory}");
    Console.WriteLine($"Shell: {shell}");
    Console.WriteLine();
}

string versionFilePath = "/proc/version";

string versionInfo = File.ReadAllText(versionFilePath);


string hostname = GetCommandOutput("hostname").Trim();

string cpuInfo = GetCommandOutput("cat /proc/cpuinfo | grep 'model name' | uniq").Trim();

string totalMemory = GetCommandOutput("cat /proc/meminfo | grep 'MemTotal'").Trim();

Console.WriteLine("##################################");
Console.WriteLine("######## ABOUT HOST ##############");
Console.WriteLine("##################################");
Console.WriteLine($"Kernel Version Information:");
Console.WriteLine(versionInfo);
Console.WriteLine($"Hostname: {hostname}");
Console.WriteLine($"CPU Info: {cpuInfo}");
Console.WriteLine($"Total Memory: {totalMemory}");


Console.WriteLine("##################################");
Console.WriteLine("######## LAST LOGIN ##############");
Console.WriteLine("##################################");
ProcessStartInfo processInfo = new ProcessStartInfo
{
    FileName = "last",
    Arguments = "-F",
    RedirectStandardOutput = true,
    UseShellExecute = false,
    CreateNoWindow = true
};

Process process = new Process
{
    StartInfo = processInfo
};

process.Start();

string output = process.StandardOutput.ReadToEnd();

process.WaitForExit();

Console.WriteLine("Last login times for all users:");
Console.WriteLine(output);


Console.WriteLine("##################################");
Console.WriteLine("####### ALL CURRENT SESSIONS #####");
Console.WriteLine("##################################");

ProcessStartInfo processInfoSession = new ProcessStartInfo
{
    FileName = "who",
    RedirectStandardOutput = true,
    UseShellExecute = false,
    CreateNoWindow = true
};

Process processSession = new Process
{
    StartInfo = processInfoSession
};

processSession.Start();

string outputSession = processSession.StandardOutput.ReadToEnd();

processSession.WaitForExit();

Console.WriteLine("Current sessions:");
Console.WriteLine(outputSession);


Console.WriteLine("##################################");
Console.WriteLine("# INFO ABOUT USERS and ROOT DIRs #");
Console.WriteLine("##################################");
string passwdFilePathDirs = "/etc/passwd";

string[] passwdLinesDirs = File.ReadAllLines(passwdFilePathDirs);

Console.WriteLine("User Information:");
Console.WriteLine("--------------------------------------");

foreach (string line in passwdLinesDirs)
{
    string[] fields = line.Split(':');

    string username = fields[0];
    string uid = fields[2];
    string homeDirectory = fields[5];

    Console.WriteLine($"Username: {username}");
    Console.WriteLine($"UID: {uid}");
    Console.WriteLine($"Home Directory: {homeDirectory}");
    Console.WriteLine("--------------------------------------");
}


Console.WriteLine("##################################");
Console.WriteLine("# ABOUT USEFUL DIRS ##############");
Console.WriteLine("##################################");

string[] programDirectories = {
    "/bin", "/sbin", "/usr/bin", "/usr/sbin", "/usr/local/bin", 
    "/usr/local/sbin", "/opt", "/etc", "/var/log", "/var/lib"
};

Console.WriteLine("Directories with useful information about programs:");
Console.WriteLine("--------------------------------------");

foreach (string directory in programDirectories)
{
    if (Directory.Exists(directory))
    {
        Console.WriteLine(directory);
        foreach (var s in Directory.GetDirectories(directory))
        {
            Console.WriteLine(s);
        }

        
    }
}


Console.WriteLine("##################################");
Console.WriteLine("######## ABOUT WINE ##############");
Console.WriteLine("##################################");

string winePrefix = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".wine");
string applicationsDirectory = Path.Combine(winePrefix, "drive_c", "Program Files");

if (Directory.Exists(applicationsDirectory))
{
    Console.WriteLine("Installed Wine Applications:");

    foreach (string appDirectory in Directory.GetDirectories(applicationsDirectory))
    {
        string appName = Path.GetFileName(appDirectory);
        Console.WriteLine($"Application: {appName}");

    }
}
else
{
    Console.WriteLine("Wine applications directory not found.");
}

Console.WriteLine("##################################");
Console.WriteLine("######## ABOUT HISTORY ###########");
Console.WriteLine("##################################");

string historyFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bash_history");

if (File.Exists(historyFilePath))
{
    Console.WriteLine("Console History:");

    string[] historyLines = File.ReadAllLines(historyFilePath);

    foreach (string line in historyLines)
    {
        Console.WriteLine(line);
    }
}
else
{
    Console.WriteLine("Bash history file not found.");
}

Console.WriteLine("##################################");
Console.WriteLine("######## ABOUT PROCESSES #########");
Console.WriteLine("##################################");

Process[] processes = Process.GetProcesses();

Console.WriteLine("Processes List:");
Console.WriteLine("----------------");

foreach (Process proc in processes)
{
    Console.WriteLine($"PID: {proc.Id}, Name: {proc.ProcessName}");

    if (IsDaemon(proc))
    {
        Console.WriteLine("   (Daemon)");
    }
}


Console.WriteLine("##################################");
Console.WriteLine("##### HIDDEN USERS FILES #########");
Console.WriteLine("##################################");
string usersDirectory = "/home";

Console.WriteLine("Hidden Files in Users' Directories:");
Console.WriteLine("-----------------------------------");

string[] userDirectories = Directory.GetDirectories(usersDirectory);

foreach (string userDirectory in userDirectories)
{
    string[] hiddenFiles = Directory.GetFiles(userDirectory, ".*", SearchOption.AllDirectories);

    foreach (string hiddenFile in hiddenFiles)
    {
        Console.WriteLine(hiddenFile);
    }
}



string GetCommandOutput(string command)
{
    ProcessStartInfo processInfo = new ProcessStartInfo()
    {
        FileName = "/bin/bash",
        Arguments = $"-c \"{command}\"",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    Process process = new Process()
    {
        StartInfo = processInfo
    };

    process.Start();
    string output = process.StandardOutput.ReadToEnd();
    process.WaitForExit();

    return output;
}

static bool IsDaemon(Process process)
{
    return string.IsNullOrEmpty(process.MainWindowTitle);
}