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
                app = new AsrApp();
                app.client = new Asr(KEY.APP_ID, KEY.API_KEY, KEY.SECRET_KEY);
                app.client.Timeout = 60000;
            }
            return app;
        }
    }
}
