using System.ComponentModel;
using System.Diagnostics;
using System.Management;

namespace Message.Subscript.Server
{
    public static class MyDaprExtensionpublic
    {
        public static IMvcBuilder AddMyDapr(this IMvcBuilder mvcBuilder,string appId,int appPort=5000,int daprHPort= 35000, int daprGPort= 50000)
        {
#if DEBUG
            var docker = Environment.GetEnvironmentVariable("container");
            if (!string.IsNullOrWhiteSpace(docker) && docker.Equals("docker"))
            {
                mvcBuilder.AddDapr();
                return mvcBuilder;
            }

            Console.WriteLine("非Docker环境启动Dapr服务......");
            //Dapr run -a MemberService -G 50004 -H 30004
            Process[] processes = Process.GetProcessesByName("daprd");
            if (processes.Any(x => x.GetCommandLineArgs().Contains(appId)) == false)
            {
                //string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                //string daprdPath = System.IO.Path.Combine(userFolder, ".dapr\\bin\\daprd.exe");
                //string args = " -app-id subwebapi -app-port 5004 -dapr-grpc-port 50004 -dapr-http-port 30004";// -components-path D:\\Temp\\Dapr_Demo\\.dapr\\components
                //Process.Start(daprdPath, args);
                Process.Start("dapr", $"run -a {appId} -p {appPort} -G {daprGPort} -H {daprHPort} ");
            }

            //Debug 环境，让程序连接特定的Dapr端口
            mvcBuilder.AddDapr(config =>
            {
                config.UseHttpEndpoint($"http://localhost:{daprHPort}");
                config.UseGrpcEndpoint($"http://localhost:{daprGPort}");
            });
#else
            mvcBuilder.AddDapr();
#endif
            return mvcBuilder;
        }

        /// <summary>
        /// 获取一个正在运行的进程的命令行参数。
        /// 与 <see cref="Environment.GetCommandLineArgs"/> 一样，使用此方法获取的参数是包含应用程序路径的。
        /// 关于 <see cref="Environment.GetCommandLineArgs"/> 可参见：
        /// [.NET 命令行参数包含应用程序路径吗？](https://walterlv.com/post/when-will-the-command-line-args-contain-the-executable-path.html)
        /// </summary>
        /// <param name="process">一个正在运行的进程。</param>
        /// <returns>表示应用程序运行命令行参数的字符串。</returns>
        public static string GetCommandLineArgs(this Process process)
        {
            if (process is null) throw new ArgumentNullException(nameof(process));

            try
            {
                return GetCommandLineArgsCore();
            }
            catch (Win32Exception ex) when ((uint)ex.ErrorCode == 0x80004005)
            {
                // 没有对该进程的安全访问权限。
                return string.Empty;
            }
            catch (InvalidOperationException)
            {
                // 进程已退出。
                return string.Empty;
            }

            string GetCommandLineArgsCore()
            {
                using (var searcher = new ManagementObjectSearcher(
                    "SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
                using (var objects = searcher.Get())
                {
                    var @object = objects.Cast<ManagementBaseObject>().SingleOrDefault();
                    return @object?["CommandLine"]?.ToString() ?? "";
                }
            }
        }
    }
}
