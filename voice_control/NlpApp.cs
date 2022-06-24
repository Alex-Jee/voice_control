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
    internal class NlpApp
    {
        // 所有操作使用同一个交互client
        internal Nlp? client = null;

        // 单例模式
        private NlpApp() { }
        private static NlpApp app = null;

        public static NlpApp getApp()
        {
            if (app == null)
            {
                app = new NlpApp();
                app.client = new Nlp(KEY.API_KEY, KEY.SECRET_KEY);
                app.client.Timeout = 60000;
            }
            return app;
        }
    }
}
