# OS-Decoy

OS-Decoy modifies the operating system name and version attributes for computer objects in Active Directory. By using OS-Decoy, you can make a computer appear as if it's running an outdated and unsupported operating system.

OS-Decoy is designed to complement internal honeypot machines, such as part of a [tcpTrigger](https://github.com/r-smith/tcpTrigger) deployment. Making your honepot appear to run an obsolete operating system is an effective deception strategy. Attackers doing Active Directory reconnaissance on your network will be lured to your honeypot, thinking it's an easy target.

Please note that you must run OS-Decoy as a scheduled task. This is because domain controllers refresh the operating system attributes each time a computer does a group policy refresh. Running the tool as a scheduled task ensures that the computer object is consistently modified to appear as your desired operating system.


## Usage guide

```
   ____  _____             ____                       
  / __ \/ ___/            / __ \___  _________  __  __
 / / / /\__ \   ______   / / / / _ \/ ___/ __ \/ / / /
/ /_/ /___/ /  /_____/  / /_/ /  __/ /__/ /_/ / /_/ / 
\____//____/           /_____/\___/\___/\____/\__, /  
                                             /____/

Usage:
  os-decoy.exe [options]

Standard options:
  --os-name <name>          Set operatingSystem to specified string
  --os-version <version>    Set operatingSystemVersion to specified string
  --servicepack <version>   Set operatingSystemServicePack to specified string
  --target <computer_name>  Specify the target computer object to modify;
                            if omitted, the current computer is modified
  --readonly                Display current attributes; no changes are made
  --version                 Display version information
  -?, -h, --help            Display usage information

Shortcut options:
  --2003    Set the OS name and version to replicate Server 2003 Standard
  --2008    Set the OS name and version to replicate Server 2008 Standard
  --2008r2  Set the OS name and version to replicate Server 2008 R2 Standard
  --2012    Set the OS name and version to replicate Server 2012 Standard
  --2012r2  Set the OS name and version to replicate Server 2012 R2 Standard
  --xp      Set the OS name and version to replicate Windows XP Professional
  --7       Set the OS name and version to replicate Windows 7 Professional
  --8       Set the OS name and version to replicate Windows 8 Professional
  --8.1     Set the OS name and version to replicate Windows 8.1 Professional

Examples:
  os-decoy.exe --os-name "Windows Server 2008 R2 Enterprise"
  os-decoy.exe --2012 --target "fin-sql-02"
  os-decoy.exe --2008r2
  os-decoy.exe --readonly --target "dmz-web-06"
```


## Deployment guide

Welcome to the OS-Decoy deployment guide! OS-Decoy is a tool designed to modify the operating system attributes of computer objects in Active Directory to make them appear as outdated and unsupported systems. Its intended use is to complement internal honeypot machines to lure and deceive potential attackers during network reconnaissance. This guide will walk you through the steps required to deploy OS-Decoy in your environment, as well as provide tips and considerations to get you started.

Before you start, familiarize yourself with the command line usage for OS-Decoy by running `os-decoy.exe --help`. Take the time to test and validate your desired command line options before proceeding.

Note that modifying the *operatingSystem* and related attributes is restricted to members of the **Domain Admins** and **Account Operators** domain groups, and to the local **SYSTEM** account on domain controllers. For a least-privileged approach, modifying the default permissions on the target computer object is possible. Here's an overview of the options:

- __Option A__:
	- Run `os-decoy.exe` through a scheduled task from any domain-joined computer. The scheduled task runs using an account that is a member of the **Account Operators** or **Domain Admins** group. You can target any computer object to modify.
- __Option B__:
	- Run `os-decoy.exe` through a scheduled task directly on a domain controller. The scheduled task runs using the built-in **SYSTEM** account. You can target any computer object to modify.
- __Option C__:
	- Run `os-decoy.exe` through a scheduled task directly on the target computer. The scheduled task runs using the low privilege **NETWORK SERVICE** built-in local account. The permissions on the target computer object in Active Directory are modified to give itself write access to its own *operatingSystem* and related attributes.

When choosing a deployment option, consider your specific needs and security requirements, and follow the corresponding instructions in this guide. It's worth noting that you can achieve the same desired effect using the PowerShell cmdlet `Set-ADComputer`. If you opt for PowerShell, the deployment and security considerations still apply. You can replace mentions of `os-decoy.exe` with a corresponding `Set-ADComputer` command.


### Option A: Use an administrative domain account

For this option, you'll need to provide a service account to run the scheduled task. The service account must be a member of either the **Domain Admins** or **Account Operators** group. If creating a dedicated account, it's recommended to give it a generic name and avoid indicating its purpose in the account description.

The following steps outline how to configure the scheduled task:

1. Download, then copy `os-decoy.exe` to a computer where you plan to run the task. This can be the target computer you plan to modify or any other computer on the domain.
2. Open the **Windows Task Scheduler**.
3. In the **Task Scheduler** window, click the **Create Task...** link in the **Actions** pane on the right-hand side.
4. In the **Create Task** window, enter a descriptive name in the **Name** field.
5. Under the **Security options** section, click the **Change User or Group...** button.
6. In the **Select User, Service Account, or Group** window, enter a service account to run the task, then click **OK**. The account must be a member of either the **Domain Admins** or **Account Operators** group.
7. You're returned to the **Create Task** window. Select the **Triggers** tab.
8. Click **New** to create a new trigger.
9. In the **New Trigger** window, use the following settings:
	- Begin the task: On a schedule
	- Daily
	- Recur every: 1 days
	- Repeat task every: 10 minutes for a duration of: 1 day
	- Enabled
10. Click **OK** to close the **New Trigger** window.
11. Select the **Actions** tab.
12. Click **New** to create a new action.
13. In the **New Action** window, use the following settings:
	- Action: Start a program
	- Enter the path to `os-decoy.exe` in the **Program/script** field.
	- Enter your desired OS-Decoy arguments in the **Add arguments (optional)** field.
14. Click **OK** to close the **New Action** window.
15. Click **OK** on the **Create Task** window to save and create your task.


### Option B: Run from a domain controller

Option B is similar to Option A, but instead you'll copy `os-decoy.exe` to a domain controller and run the scheduled task from the domain controller. When selecting the service account to run the task, use the local **SYSTEM** account. Just like with Option A, `os-decoy.exe` can target any computer object to modify.


### Option C: Least privilege

Option C demonstrates a least privileged approach, where the target computer is granted permissions to modify its *operatingSystem* and related attributes. However, it's important to note that this option is not recommended due to its detectability. Since object permissions are visible to any domain user, honeypot detection tools could potentially detect and identify this technique.

For this option, follow the steps outlined in Option A to create a scheduled task. However, you must create and run the task on the target computer. When selecting the service account to run the task, use the local **NETWORK SERVICE** account.

The following steps outline how to adjust the permissions on the target computer in Active Directory:

1. Open **Active Directory Users and Computers**.
2. Ensure the **Advanced Features** option is enabled by checking **View > Advanced Features**.
3. Locate the target computer object, then double-click the computer to open its **Properties** window.
4. Select the **Security** tab.
5. Click the **Advanced** button.
6. In the **Permission Entry** window, click **Select a principal**.
7. In the **Select User, Computer, Service Account, or Group** windows, enter **SELF**, then click **OK**.
8. For **Type**, choose **Allow**.
9. For **Applies to**, choose **This object only**.
10. Scroll to the bottom and click the **Clear all** button.
11. Check the follow options:
	- **Write Operating System**
	- **Write Operating System Version**
	- **Write operatingSystemServicePack**
12. Click **OK** on the **Permission Entry** window.
13. Click **OK** on the **Advanced Security Settings** window, then close the **Properties** window for the computer.

After modifying the permissions, test the scheduled task to ensure that everything is functioning as intended.
