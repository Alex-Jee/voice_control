using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Baidu.Aip.Nlp;
using Baidu.Aip.Speech;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace voice_control
{
    internal class AsrApp
    {
        // 所有操作使用同一个交互client
        internal Asr? client = null;

        // 单例模式
        private AsrApp() { }
        private static AsrApp app = null;

        public static AsrApp getApp()
        {
            if (app == null)
            {
                string appId = "26546830";
                string apiKey = "5aagdNRNZyz8L4cx394yVKLN";
                string apiSecretKey = "fkvm2MrSS12q20Q7Vqc3Gi1gWk0rmA3j";

                app = new AsrApp();
                app.client = new Asr(appId, apiKey, apiSecretKey);
                //app.client = new Asr(KEY.APP_ID, KEY.API_KEY, KEY.SECRET_KEY);
                app.client.Timeout = 60000;
            }
            return app;
        }

        public static string GetTextFromFile(string full_path)
        {
            if (!File.Exists(full_path))
                return "";

            var data = File.ReadAllBytes(full_path);
            //var AppID = "112";
            var language = 1537;    //普通话模型
            var options = new Dictionary<string, object>
            {
                {"dev_pid", language}
            };
            //Baidu.Aip.Speech.Asr asr = new Baidu.Aip.Speech.Asr(AppID, apiKey, apiSecretKey) { Timeout = 120000 };
            JObject jobject = getApp().client.Recognize(data, "pcm", 16000, options);
            string ss = jobject.ToString();

            if (jobject["result"] != null)
            {
                var jarr = jobject["result"].ToArray();
                string text = "";
                for (int i = 0; i < jarr.Length; i++)
                {
                    var j = jarr[i];
                    text += j.ToString();
                }
                return text;
            }
            return "";
        }
    }
}
