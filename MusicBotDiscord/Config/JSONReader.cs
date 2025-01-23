using Newtonsoft.Json;

namespace MusicBotDiscord.Config
{
    internal class JSONReader
    {
        public string token { get; set; }
        public string prefix { get; set; }
        public string hostName { get; set; }
        public int port { get; set; }
        public string password { get; set; }
        public bool secured { get; set; }

        public async Task ReadJSON()
        {
            using (StreamReader sr = new StreamReader("config.json"))
            {
                string json = await sr.ReadToEndAsync();
                JSONStructure data = JsonConvert.DeserializeObject<JSONStructure>(json);

                this.token = data.token;
                this.prefix = data.prefix;
                this.hostName = data.hostName;
                this.port = data.port;
                this.password = data.password;
                this.secured = data.secured;
            }
        }

        internal sealed class JSONStructure
        {
            public string token { get; set; }
            public string prefix { get; set; }
            public string hostName { get; set; }
            public int port { get; set; }
            public string password { get; set; }
            public bool secured { get; set; }
        }
    }
}
