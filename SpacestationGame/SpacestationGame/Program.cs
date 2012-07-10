using System;

namespace SpacestationGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (SSGame game = new SSGame())
            {
                game.Run();
            }
        }
    }
#endif
}

