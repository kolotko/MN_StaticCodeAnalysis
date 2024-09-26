namespace MNStaticCodeAnalysis
{
    public class Test
    {
        private const string c_xxxx = "sd";

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
            var i = 1;
            Console.WriteLine(i);
            Console.WriteLine(c_xxxx);
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

            var uuu = "";
            for (var i = 0; i < 10; i++)
            {
                uuu += "essa";
            }
        }
    }
}