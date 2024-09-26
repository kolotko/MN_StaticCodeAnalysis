namespace MNStaticCodeAnalysis
{
    public class Test
    {
        private const string c_xxxx = "sd";

        public Test()
        {
        }

        //async void do wykrywania
        private static async Task Xd(CancellationToken ct)
        {
            var tes = "xxx";
            Console.WriteLine(tes);
            if (true)
            {
                await Xd2();
            }

            var i = 1;
            Console.WriteLine(i);
            Console.WriteLine(c_xxxx);
            try
            {
                await Task.Delay(0, ct);
            }
            catch (Exception exz)
            {
                Console.WriteLine(exz.ToString());
            }

            using HttpClient x = new();
            var uuu = await x.GetAsync(new Uri(c_xxxx), cancellationToken: ct);
            Xd3();
        }

        private static async Task Xd2()
        {
            await Task.Delay(0);

            var uuu = "";
            for (var i = 0; i < 10; i++)
            {
                uuu += "essa";
            }
        }

        private static void Xd3()
        {
            var words = new[] { "foo", "ddd", "baz" };
            var actions = new List<Action>();

            foreach(var word in words)
            {
                actions.Add(() => Console.WriteLine(word));
            }
        }
    }
}