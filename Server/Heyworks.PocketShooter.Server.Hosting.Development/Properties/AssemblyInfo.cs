using System.Reflection;

// is fine until ASP.NET Core will fail to run in Photon Process, even then we may still run Meta as external process on start
[assembly:AssemblyDescription("F5 development. In process start and hosting, but with real IPC communication.")]