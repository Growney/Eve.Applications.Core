using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.ESIClassMaker.Schema
{
    public class ESISchema
    {
        private const string c_apiurl = "https://esi.evetech.net";

        public List<string> consumes { get; set; }
        public Dictionary<string,definition> definitions { get; set; }
        
        public string host { get; set; }
        public info info { get; set; }
        public Dictionary<string,parameter> parameters { get; set; }
        public Dictionary<string,securityDefinition> securityDefinitions { get; set; }
        public List<string> produces { get; set; }
        public List<string> schemes { get; set; }
        public Dictionary<string, Dictionary<string, path>> paths { get; set; }
        public string swagger { get; set; }

        public static async Task<ESISchema> GetSchemaAsync(string version)
        {
            using (Gware.Common.Client.Web.WebAPIClient client = new Gware.Common.Client.Web.WebAPIClient(c_apiurl))
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var getTask = client.GetAsync($"{c_apiurl}/{version}/swagger.json");

                await getTask;

                if (getTask.IsCompleted)
                {
                    System.Net.Http.HttpResponseMessage message = getTask.Result;

                    if (getTask.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var parseTask = Gware.Common.Client.Web.WebAPIClient.ParseResponseAsync<ESISchema>(message);

                        await parseTask;

                        if (parseTask.IsCompleted)
                        {
                            return parseTask.Result;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        public static async Task<List<string>> GetSchemaVersions()
        {
            using (Gware.Common.Client.Web.WebAPIClient client = new Gware.Common.Client.Web.WebAPIClient(c_apiurl))
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                string url = $"{c_apiurl}/versions/";
                var getTask = client.GetAsync(url);

                await getTask;

                if (getTask.IsCompleted)
                {
                    System.Net.Http.HttpResponseMessage message = getTask.Result;

                    if (getTask.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var parseTask = Gware.Common.Client.Web.WebAPIClient.ParseResponseAsync<List<string>>(message);

                        await parseTask;

                        if (parseTask.IsCompleted)
                        {
                            return parseTask.Result;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
