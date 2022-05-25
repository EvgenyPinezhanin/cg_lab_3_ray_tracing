using System;

namespace computer_graphics_lab_3
{
    class Program
    {
        [STAThread]
        private static void Main()
        {
            using (RayTracing.RayTracing program = new RayTracing.RayTracing(1400, 920, "RayTracing(lab_work_3)"))
            {
                program.Run();
            }
        }
    }
}
