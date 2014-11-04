using System.Console;

namespace RoslynDemos.Demo2
{
    class Program
    {
        /// <summary>
        /// Sample program demoing Exception filtering
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var retryAttempts = 3;

            while (retryAttempts-- > 0)
            {
                try
                {
                    SomeOperationOnExternalSystem();
                }
                catch (ExternalSystemException esx)
                {
                    if (esx.ErrorCode != ErrorCode.DeadLock)
                    {
                        throw;
                    }
                }
            }

            WriteLine("Done. Press enter to exit");
            ReadLine();
        }

        /// <summary>
        /// A dummy operation on a simulated external system
        /// </summary>
        /// <exception cref="ExternalSystemException">Thrown for demo purposes</exception>
        private static void SomeOperationOnExternalSystem()
        {
            failCounter++;

            if (failCounter == 1)
            {
                throw new ExternalSystemException { ErrorCode = ErrorCode.DeadLock };
            }
            else
            {
                throw new ExternalSystemException { ErrorCode = ErrorCode.InvalidFoo };
            }
        }

        private static int failCounter = 0;
    }
}
