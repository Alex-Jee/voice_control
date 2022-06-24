using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Baidu.Aip.Nlp;
using Baidu.Aip.Speech;
using Newtonsoft.Json;

namespace voice_control
{
    internal class TtsApp
    {
        // 所有操作使用同一个交互client
        internal Tts? client = null;

        // 单例模式
        private TtsApp() { }
        private static TtsApp app = null;

        public static TtsApp getApp()
        {
            if (app == null)
            {
                app = new TtsApp();
                app.client = new Tts(KEY.API_KEY, KEY.SECRET_KEY);
                app.client.Timeout = 60000;
            }
            return app;
        }
    }
}
