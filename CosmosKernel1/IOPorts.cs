namespace HashOS.HAL
{
    public class IOPorts
    {
        static Cosmos.Core.IOPort io = new Cosmos.Core.IOPort(0);
        static int PP = 0, D = 0;
        public static void Outb(ushort port, byte data)
        {
            if (io.Port != port)
                io = new Cosmos.Core.IOPort(port);
            io.Byte = data;
            PP = port;
            D = data;

        }
        public static byte Inb(ushort port)
        {
            if (io.Port != port)
                io = new Cosmos.Core.IOPort(port);
            return io.Byte;

        }
    }
}
