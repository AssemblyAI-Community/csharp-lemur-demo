# AssemblyAI LeMUR C# Demo
C# demo of how to use AssemblyAI's LeMUR framework.

## Installing and Setting Up .NET
You'll need to set up your .NET environment to make sure you can configure and run this project. Instructions for all operating systems are maintained by Microsoft and can be found [here](https://learn.microsoft.com/en-us/dotnet/core/install/).

Once it's successfully installed, you'll now have access to the .NET Command Line Interface (CLI) which will allow you to build and run this project.

## Running the Project

With the .NET CLI, navigate to the root folder of this project. Then run `dotnet build` to verify that the build completes successfully and that you've got all necessary dependencies on your system. The third-party libraries that this project depends on are already listed in the `.csproj` file, and should be installed just by running `dotnet build`.

Then, add your AssemblyAI API key on line 8 to replace the `"API_KEY"` placeholder. Your API key can be found on your [account dashboard](https://www.assemblyai.com/app/account), and if you don't have an account yet, you can sign up [here](https://www.assemblyai.com/dashboard/signup). Please note that you'll need to have an upgraded AssemblyAI account with funds deposited in order to use LeMUR.

You'll then need to specify which file you're looking to summarize by providing a file path on line 12 to replace the `"FILE_PATH"` placeholder. Verify that the path you're providing fits the right formatting for your OS, as this can differ between Windows and MacOS.

Now you can use `dotnet run` to start the program and receive a summary of the file you provided to LeMUR. We hope this gets you started using AssemblyAI's LeMUR API in your C# projects, and please feel free to contact support@assemblyai.com if you have any questions!