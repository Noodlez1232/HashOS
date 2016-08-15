namespace HashOS.HAL
{
    public static class Power
    {

        public static void Reboot()
        {
            byte good = 0x02;
            while ((good & 0x02) != 0)
                good = IOPorts.Inb(0x64);
            IOPorts.Outb(0x64, 0xFE); //Pulse reset pin
            Cosmos.Core.Global.CPU.Halt();
        }

        public static void ShutDown()
        {
            HashOS.HAL.ACPI.Shutdown();
            ACPI.Disable();
        }
    }
}
