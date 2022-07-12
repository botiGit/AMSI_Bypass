using System;
using System.Diagnostics;

namespace sample
{
	class mysample
	{
		public static void Main()
		{
			string args = "systeminfo > C:\\Windows\\Tasks\\file.txt";
			string program = "/c" + " " + args;
			using (Process proc = new Process())
			{
				proc.StartInfo.FileName = "cmd.exe";
				proc.StartInfo.Arguments = program;
				proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				proc.StartInfo.CreateNoWindow = true;
				proc.StartInfo.Verb = "runas"; // run as admin
				proc.Start();
				proc.WaitForExit();
			}
		}
	}
}

