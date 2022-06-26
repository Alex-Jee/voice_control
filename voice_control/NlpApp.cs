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
        private struct StructDepItem
        {
            public int id;
            public string postag;
            public int head;
            public string word;
            public string deprel;
        }
        public struct CommandItem
        {
            public string item_id; // 主体id
            public string action;
        }
        private List<StructDepItem> TextParser(string text)
        {
            // 调用百度API
            JObject result;
            try
            {
                nlpClient = getApp();
                result = nlpClient.DepParser(text.Trim());
            }
            catch (AipException exp)
            {
                MessageBox.Show(exp.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 整理结果数据
            List<StructDepItem> depItems = new List<StructDepItem>();
            int itemIndex = 1;
            foreach (var item in result["items"])
            {
                StructDepItem depItem = new StructDepItem
                {
                    id = itemIndex++,
                    postag = item["postag"].ToString(),
                    head = item.Value<int>("head"),
                    word = item["word"].ToString(),
                    deprel = item["deprel"].ToString()
                };
                depItems.Add(depItem);
                
            }
            
            return depItems;
        }
        public static CommandItem getCommand(string text) {
            depItems = TextParser(text);
            // TODO: 获取指令返回ItemCommand
            return null; 
        }
    }
}
