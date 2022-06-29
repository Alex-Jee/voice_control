using Baidu.Aip;
using Baidu.Aip.Nlp;
using Newtonsoft.Json.Linq;

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
            public string item_result;
        }
        private static List<StructDepItem> TextParser(string text)
        {
            // 调用百度API
            JObject result;
            try
            {
                app = getApp();
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                result = app.client.DepParser(text.Trim());
                System.Console.WriteLine(result);
            }
            catch (AipException exp)
            {
                MessageBox.Show(exp.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
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
            Dictionary<string, string> ItemIdTable =
            new Dictionary<string, string>() {{"电灯", "1"},
                                               {"灯", "1"},
                                               {"电视", "2"},
                                               {"扫地机", "3"},
                                               };
            List<StructDepItem> depItems = TextParser(text);
            // TODO: 获取指令返回ItemCommand
            CommandItem cmdItem = new CommandItem { };
            string applianceName = "";
            foreach (var depItem in depItems)
            {

                //获取执行指令的主体
                if (depItem.postag == "n" || depItem.postag == "nz")
                {
                    applianceName = depItem.word;
                    if (applianceName == "电灯" || applianceName == "电视" || applianceName == "扫地机" || applianceName == "灯")
                    {
                        cmdItem.item_id = ItemIdTable[applianceName];
                    }
                    else
                    {
                        cmdItem.item_id = "0";
                    }
                    break;
                }
            }
            int hed_id = 0;
            //获取执行指令主体成功后
            if (cmdItem.item_id != "0")
            {
                foreach (var depItem in depItems)
                {
                    //获取执行指令的核心关系以及动作
                    if (depItem.deprel == "HED")
                    {
                        cmdItem.action = depItem.word;
                        hed_id = depItem.id;
                    }
                    if (depItem.id > hed_id && depItem.word != applianceName && depItem.postag != "w")
                    {
                        cmdItem.action = cmdItem.action + depItem.word;
                    }
                }
                cmdItem.item_result = "执行指令" + applianceName + cmdItem.action + "完成。";
            }   
            return cmdItem; 
        }
    }
    
}
