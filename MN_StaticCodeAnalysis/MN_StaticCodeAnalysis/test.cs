namespace MNStaticCodeAnalysis
{
    using System.Globalization;

    public class Test
    {

        private const string xxxxxx = "sd";
        public Test()
        {
            Xd();
        }

        //async void do wykrywania
        private static async void Xd()
        {
            if (true)
            {
                Xd2();
            }

            int i = 1;
            Console.WriteLine(i);
            Console.WriteLine(xxxxxx);
            try
            {
                await Task.Delay(0);
            }
            catch (Exception exz)
            {
                Console.WriteLine(exz.ToString());
            }

            using HttpClient x = new();
        }

        private static async void Xd2()
        {
            await Task.Delay(0);

            string uuu = "";
            for (int i = 0; i < 10; i++)
            {
                uuu += "essa";
            }
        }
    }
}