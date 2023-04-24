using System;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Text;

namespace OS_Decoy
{
    class Program
    {
        private const string OsNameArgument = "os-name";
        private const string OsVersionArgument = "os-version";
        private const string ServicePackArgument = "servicepack";
        private const string TargetArgument = "target";
        private const string ReadOnlyArgument = "readonly";
        private const string Os2003Argument = "2003";
        private const string Os2008Argument = "2008";
        private const string Os2008r2Argument = "2008r2";
        private const string Os2012Argument = "2012";
        private const string Os2012r2Argument = "2012r2";
        private const string OsXpArgument = "xp";
        private const string Os7Argument = "7";
        private const string Os8Argument = "8";
        private const string Os81Argument = "8.1";
        private const string OsNameAttribute = "operatingSystem";
        private const string OsVersionAttribute = "operatingSystemVersion";
        private const string ServicePackAttribute = "operatingSystemServicePack";

        static void PrintUsage()
        {
            Console.WriteLine("   ____  _____             ____                       ");
            Console.WriteLine("  / __ \\/ ___/            / __ \\___  _________  __  __");
            Console.WriteLine(" / / / /\\__ \\   ______   / / / / _ \\/ ___/ __ \\/ / / /");
            Console.WriteLine("/ /_/ /___/ /  /_____/  / /_/ /  __/ /__/ /_/ / /_/ / ");
            Console.WriteLine("\\____//____/           /_____/\\___/\\___/\\____/\\__, /  ");
            Console.WriteLine("                                             /____/   ");
            Console.WriteLine();
            Console.WriteLine("OS-Decoy modifies the operating system name and version attributes for");
            Console.WriteLine("computer objects in Active Directory. When no target is specified, the");
            Console.WriteLine("changes are applied to the current computer. To ensure that your changes");
            Console.WriteLine("persist, you'll need to run this tool as a scheduled task.");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine($"  {AppDomain.CurrentDomain.FriendlyName} [options]");
            Console.WriteLine();
            Console.WriteLine("Standard options:");
            Console.WriteLine($"  --{OsNameArgument} <name>          Set {OsNameAttribute} to specified string");
            Console.WriteLine($"  --{OsVersionArgument} <version>    Set {OsVersionAttribute} to specified string");
            Console.WriteLine($"  --{ServicePackArgument} <version>   Set {ServicePackAttribute} to specified string");
            Console.WriteLine($"  --{TargetArgument} <computer_name>  Specify the target computer object to modify;");
            Console.WriteLine("                            if omitted, the current computer is modified");
            Console.WriteLine($"  --{ReadOnlyArgument}                Display current attributes; no changes are made");
            Console.WriteLine("  --version                 Display version information");
            Console.WriteLine("  -?, -h, --help            Display usage information");
            Console.WriteLine();
            Console.WriteLine("Shortcut options:");
            Console.WriteLine($"  --{Os2003Argument}    Set the OS name and version to replicate Server 2003 Standard");
            Console.WriteLine($"  --{Os2008Argument}    Set the OS name and version to replicate Server 2008 Standard");
            Console.WriteLine($"  --{Os2008r2Argument}  Set the OS name and version to replicate Server 2008 R2 Standard");
            Console.WriteLine($"  --{Os2012Argument}    Set the OS name and version to replicate Server 2012 Standard");
            Console.WriteLine($"  --{Os2012r2Argument}  Set the OS name and version to replicate Server 2012 R2 Standard");
            Console.WriteLine($"  --{OsXpArgument}      Set the OS name and version to replicate Windows XP Professional");
            Console.WriteLine($"  --{Os7Argument}       Set the OS name and version to replicate Windows 7 Professional");
            Console.WriteLine($"  --{Os8Argument}       Set the OS name and version to replicate Windows 8 Professional");
            Console.WriteLine($"  --{Os81Argument}     Set the OS name and version to replicate Windows 8.1 Professional");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine($"  {AppDomain.CurrentDomain.FriendlyName} --{OsNameArgument} \"Windows Server 2008 R2 Enterprise\" ");
            Console.WriteLine($"  {AppDomain.CurrentDomain.FriendlyName} --{Os2012Argument} --{TargetArgument} \"fin-sql-02\"");
            Console.WriteLine($"  {AppDomain.CurrentDomain.FriendlyName} --{Os2008r2Argument}");
            Console.WriteLine($"  {AppDomain.CurrentDomain.FriendlyName} --{ReadOnlyArgument} --{TargetArgument} \"dmz-web-06\"");
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            string targetComputer = Environment.MachineName;
            string newName = string.Empty;
            string newVersion = string.Empty;
            string newServicePack = string.Empty;
            bool isNameSpecified = false;
            bool isVersionSpecified = false;
            bool isServicePackSpecified = false;
            bool isReadOnly = false;

            // Process command line arguments.
            for (var index = 0; index < args.Length; ++index)
            {
                switch (args[index].ToLower())
                {
                    case "--" + OsNameArgument:
                        if (index + 1 < args.Length)
                        {
                            newName = args[index + 1];
                            isNameSpecified = true;
                        }
                        break;
                    case "--" + OsVersionArgument:
                        if (index + 1 < args.Length)
                        {
                            newVersion = args[index + 1];
                            isVersionSpecified = true;
                        }
                        break;
                    case "--" + ServicePackArgument:
                        if (index + 1 < args.Length)
                        {
                            newServicePack = args[index + 1];
                            isServicePackSpecified = true;
                        }
                        break;
                    case "--" + TargetArgument:
                        if (index + 1 < args.Length)
                        {
                            targetComputer = args[index + 1];
                        }
                        break;
                    case "--" + ReadOnlyArgument:
                        isReadOnly = true;
                        break;
                    case "--" + Os2003Argument:
                        isNameSpecified = true;
                        isVersionSpecified = true;
                        isServicePackSpecified = true;
                        newName = OperatingSystem.Server2003Name;
                        newVersion = OperatingSystem.Server2003Version;
                        newServicePack = OperatingSystem.Server2003ServicePack;
                        break;
                    case "--" + Os2008Argument:
                        isNameSpecified = true;
                        isVersionSpecified = true;
                        isServicePackSpecified = true;
                        newName = OperatingSystem.Server2008Name;
                        newVersion = OperatingSystem.Server2008Version;
                        newServicePack = OperatingSystem.Server2008ServicePack;
                        break;
                    case "--" + Os2008r2Argument:
                        isNameSpecified = true;
                        isVersionSpecified = true;
                        isServicePackSpecified = true;
                        newName = OperatingSystem.Server2008r2Name;
                        newVersion = OperatingSystem.Server2008r2Version;
                        newServicePack = OperatingSystem.Server2008r2ServicePack;
                        break;
                    case "--" + Os2012Argument:
                        isNameSpecified = true;
                        isVersionSpecified = true;
                        isServicePackSpecified = true;
                        newName = OperatingSystem.Server2012Name;
                        newVersion = OperatingSystem.Server2012Version;
                        newServicePack = OperatingSystem.Server2012ServicePack;
                        break;
                    case "--" + Os2012r2Argument:
                        isNameSpecified = true;
                        isVersionSpecified = true;
                        isServicePackSpecified = true;
                        newName = OperatingSystem.Server2012r2Name;
                        newVersion = OperatingSystem.Server2012r2Version;
                        newServicePack = OperatingSystem.Server2012r2ServicePack;
                        break;
                    case "--" + OsXpArgument:
                        isNameSpecified = true;
                        isVersionSpecified = true;
                        isServicePackSpecified = true;
                        newName = OperatingSystem.WindowsXpName;
                        newVersion = OperatingSystem.WindowsXpVersion;
                        newServicePack = OperatingSystem.WindowsXpServicePack;
                        break;
                    case "--" + Os7Argument:
                        isNameSpecified = true;
                        isVersionSpecified = true;
                        isServicePackSpecified = true;
                        newName = OperatingSystem.Windows7Name;
                        newVersion = OperatingSystem.Windows7Version;
                        newServicePack = OperatingSystem.Windows7ServicePack;
                        break;
                    case "--" + Os8Argument:
                        isNameSpecified = true;
                        isVersionSpecified = true;
                        isServicePackSpecified = true;
                        newName = OperatingSystem.Windows8Name;
                        newVersion = OperatingSystem.Windows8Version;
                        newServicePack = OperatingSystem.Windows8ServicePack;
                        break;
                    case "--" + Os81Argument:
                        isNameSpecified = true;
                        isVersionSpecified = true;
                        isServicePackSpecified = true;
                        newName = OperatingSystem.Windows81Name;
                        newVersion = OperatingSystem.Windows81Version;
                        newServicePack = OperatingSystem.Windows81ServicePack;
                        break;
                    case "--version":
                        Console.WriteLine(typeof(Program).Assembly.GetName().Version);
                        return;
                    case "/?":
                    case "-?":
                    case "-h":
                    case "--help":
                        {
                            PrintUsage();
                            return;
                        }
                }
            }

            // Exit the application if no OS name arguments were specified.
            if (!isReadOnly && !isNameSpecified && !isVersionSpecified && !isServicePackSpecified)
            {
                PrintUsage();
                Environment.Exit(-1);
            }

            // Get the current computer's Active Directory domain.
            Domain computerDomain = null;
            try
            {
                Console.WriteLine($"Retrieving Active Directory domain...");
                computerDomain = Domain.GetComputerDomain();
            }
            // Exit the application if the current computer isn't joined to Active Directory.
            catch (ActiveDirectoryObjectNotFoundException)
            {
                Console.WriteLine("Error: You must run this tool from a domain-joined computer.");
                Console.WriteLine();
                Environment.Exit(-1);
            }

            // Display domain information.
            Console.WriteLine($"  Domain: '{computerDomain}'");
            Console.WriteLine();

            // Find the targeted computer object in Active Directory and update the operating system attributes.
            try
            {
                using (DirectoryEntry parentEntry = new DirectoryEntry($"LDAP://{computerDomain}", null, null, AuthenticationTypes.Secure & AuthenticationTypes.Sealing))
                using (DirectorySearcher directorySearch = new DirectorySearcher(parentEntry))
                {
                    directorySearch.Filter = $"(&(objectClass=computer)(cn={targetComputer}))";
                    directorySearch.PropertiesToLoad.Add(OsNameAttribute);
                    directorySearch.PropertiesToLoad.Add(OsVersionAttribute);
                    directorySearch.PropertiesToLoad.Add(ServicePackAttribute);
                    SearchResult searchResult = directorySearch.FindOne();
                    // Exit the application if the computer object wasn't found.
                    if (searchResult == null)
                    {
                        Console.WriteLine($"Error: Computer '{targetComputer}' was not found.");
                        Console.WriteLine();
                        Environment.Exit(-1);
                    }

                    using (DirectoryEntry entry = new DirectoryEntry(searchResult.GetDirectoryEntry().Path))
                    {
                        // Read the current operating system attributes from AD.
                        string currentName = (string)entry.Properties[OsNameAttribute].Value ?? string.Empty;
                        string currentVersion = (string)entry.Properties[OsVersionAttribute].Value ?? string.Empty;
                        string currentServicePack = (string)entry.Properties[ServicePackAttribute].Value ?? string.Empty;

                        // Display the current operating system attributes.
                        Console.OutputEncoding = Encoding.Default;
                        Console.WriteLine($"Current attributes for computer object '{targetComputer.ToUpper()}':");
                        Console.WriteLine($"  {OsNameAttribute}: '{currentName}'");
                        Console.WriteLine($"  {OsVersionAttribute}: '{currentVersion}'");
                        Console.WriteLine($"  {ServicePackAttribute}: '{currentServicePack}'");
                        Console.WriteLine();

                        // Exit the application if the read-only argument was specified.
                        if (isReadOnly)
                        {
                            return;
                        }

                        // Determine if any attributes need to be updated.
                        bool isNameChanged = isNameSpecified && !(newName == currentName);
                        bool isVersionChanged = isVersionSpecified && !(newVersion == currentVersion);
                        bool isServicePackChanged = isServicePackSpecified && !(newServicePack == currentServicePack);

                        // Exit the application if no changes need to be made.
                        if (!isNameChanged && !isVersionChanged && !isServicePackChanged)
                        {
                            Console.WriteLine("There are no changes to make. Exiting.");
                            Console.WriteLine();
                            return;
                        }

                        // Write the new operating system attributes to Active Directory.
                        Console.WriteLine("Writing changes to Active Directory...");
                        if (isNameChanged)
                        {
                            if (string.IsNullOrEmpty(newName))
                                entry.Properties["operatingSystem"].Clear();
                            else
                                entry.Properties["operatingSystem"].Value = newName;
                        }
                        if (isVersionChanged)
                        {
                            if (string.IsNullOrEmpty(newVersion))
                                entry.Properties["operatingSystemVersion"].Clear();
                            else
                                entry.Properties["operatingSystemVersion"].Value = newVersion;
                        }
                        if (isServicePackChanged)
                        {
                            if (string.IsNullOrEmpty(newServicePack))
                                entry.Properties["operatingSystemServicePack"].Clear();
                            else
                                entry.Properties["operatingSystemServicePack"].Value = newServicePack;
                        }
                        entry.CommitChanges();

                        // Display the new operating system attributes.
                        Console.WriteLine();
                        Console.WriteLine($"New attributes for computer object '{targetComputer.ToUpper()}':");
                        Console.WriteLine($"  {OsNameAttribute}: " + (isNameChanged ? $"'{newName}'" : $"'{currentName}' [unchanged]"));
                        Console.WriteLine($"  {OsVersionAttribute}: " + (isVersionChanged ? $"'{newVersion}'" : $"'{currentVersion}' [unchanged]"));
                        Console.WriteLine($"  {ServicePackAttribute}: " + (isServicePackChanged ? $"'{newServicePack}'" : $"'{currentServicePack}' [unchanged]"));
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                // If something went wrong, display the reason and exit.
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine();
                Environment.Exit(-1);
            }
        }
    }
}
