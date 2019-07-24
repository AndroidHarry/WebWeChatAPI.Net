1. add nlog
	Install-Package NLog -Version 4.5.1
	
	<!--TRACE,DEBUG,INFO,WARN,ERROR,FATAL-->
	FtpTrace.WriteLine("end test, " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
	上面这句可以被 NLog 记录 是因为程序启动时把 NLog 加入到了 System.Diagnostics.TraceSource, 
	所以在 FtpTrace.EmitEvent() 时 traceSource.Listeners 里面有一个 NLog 的 listener 。
	具体的配置参见 app.config 的 <system.diagnostics> 段 。