# ProcessInvoke Framework
Easy to use classes for invoking a method in another process without sockets.
NugetPackage: [ProcessInvoke](http://nuget.org/packages/ProcessInvoke)

NugetPackage: [ProcessInvoke.CurrentUser](http://nuget.org/packages/ProcessInvoke.Core)

NugetPackage: [ProcessInvoke.CurrentUser](http://nuget.org/packages/ProcessInvoke.Security.CurrentUser)
NugetPackage: [ProcessInvoke.HighestAvailable](http://nuget.org/packages/ProcessInvoke.Security.HighestAvailable)
NugetPackage: [ProcessInvoke.Administrator](http://nuget.org/packages/ProcessInvoke.Security.Administrator)

## Example Code
In the example below, we're going to simply get the process ID of the remote process that we're running under a different account.

First we need to define our interface and our implementation:
````
	//Define and interface and your remote object:
    public interface IRemoteObject {
        int HostingProcessId();
    }

	public class RemoteObject : MarshalByRefObject, IRemoteObject {
        public int HostingProcessId() {
            return System.Diagnostics.Process.GetCurrentProcess().Id;
        }
    }
````

Then when we want to call it, we do the following:
````
	//Choose the security context you want to execute under
	var Invoker = ProcessInvoke.CurrentUserProcessInvoker.Instance;
	//var Invoker = ProcessInvoke.HighestAvailableProcessInvoker.Instance;
	//var Invoker = ProcessInvoke.AdministratorProcessInvoker.Instance;

	//Start up a new copy of our hosting process.
	using(var Host = Invoker.TryStart()){

		//Tell it what kind of object we want to use.
		var Service = Host.Register<IRemoteObject, RemoteObject>();

		//Call our method on the remote object.
		var RemoteProcessID = Service.HostingProcessId();
        Assert.AreNotEqual(MyProcessID, RemoteProcessID);

	}

````

## Implementation Details
Under the hood, a few different things are happening:
1.  There are four different assemblies that work together:
ProcessInvoke.Core is the main library that you will reference.  It is a DLL.
ProcessInvoke.Securty.* are entry points (EXEs) into ProcessInvoke.Core.  Each entry includes an application manifest that tells windows what kind of permissions you want to run under.

2.  When you call ````Invoker.TryStart()````, the specified entry point is shell executed as a new process with a series of command line arguments.  One of the command line arguments is a randomly generated string that will be used to create a named pipe.

3.  The new process reads the command line arguments and starts listening on the specified named pipe.

4.  Your process then connects to the named pipe and uses the [Zyan Framework](https://github.com/zyanfx/Zyan) to handle remoting.

## Customizing Permissions
If you need to execute with custom permissions, it is really easy to do so:
1.  Add a new EXE to your project with the manifest you want.
2.  Call the ProcessInvoke Main entry point from your custom permisions app:
````
namespace ProcessInvoke.Security.CustomPermissions {
    public static class Program {
        public static Task<int> Main(string[] args) {
            return ProcessInvoke.Program.Main(args);
        }
    }
}
````

3.  Last, to make things easy, inside your new EXE, add a shortcut class:
````
using System.Reflection;

namespace ProcessInvoke {

    public class CustomProcessInvoker : ProcessInvoker {

        public static CustomProcessInvoker Instance { get; private set; } = new CustomProcessInvoker();

    }

}
````

Now you can execute your method with your custom permissions.

## Further Customizations
You can control a lot of other options about how the hosting process is launched.  Just override the appropriate methods in your ````CustomProcessInvoker```` class.