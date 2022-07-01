using System.Text.RegularExpressions;

namespace voice_control
{
    public partial class UI : Form
    {
        SpeechRecord speech_record = new SpeechRecord();
        SpeechSynthesis speech_synthesis = new SpeechSynthesis();
        bool tv = false;
        int channel = 1;
        bool robot = false;
        

        public UI()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
                openFileDialog1.Filter = "pcm音频|*.pcm|wav音频|*.wav|amr音频|*.amr";
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.FilterIndex = 1;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = openFileDialog1.FileName;
                    string result_text = AsrApp.GetTextFromFile(textBox1.Text);
                    UpdateUIForThread(2, result_text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonSpeechMic_Click(object sender, EventArgs e)
        {
            buttonSpeechMic.Enabled = false;
            speech_record.SetResultCallbackDelegate(this.OnSpeechResult);
            speech_record.SetStateCallbackDelegate(this.OnSpeechState);
            speech_record.StartRec();
            buttonIdentifyRightnow.Enabled = true;
        }

        private void buttonIdentifyRightnow_Click(object sender, EventArgs e)
        {
            speech_record.StopRec();
            buttonSpeechMic.Enabled = true;
            buttonIdentifyRightnow.Enabled = false;
        }

        public void OnSpeechState(string text)
        {
            UpdateUIForThread(1, text);
        }

        public void OnSpeechResult(string text)
        {
            UpdateUIForThread(2, text);
        }

        private void UpdateUIForThread(int source, string text)
        {
            this.Invoke(new Action(() =>
            {
                switch (source)
                {
                    case 1:
                        labelState.Text = text;
                        break;
                    case 2:
                        textBox2.Text = text;
                        buttonSpeechMic.Enabled = true;
                        buttonIdentifyRightnow.Enabled = false;
                        NlpApp.CommandItem itemCommand = NlpApp.getCommand(text);
                        textBox3.Text = itemCommand.item_result;
                        //TODO:继续处理指令
                        //.....
                        item_Change(itemCommand);
                        break;
                    default:
                        break;
                }
            }));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            speech_synthesis.SpeechSynthesisAndPlay(textBox3.Text);
        }

        // 用于根据解析得到的CommandItem，改变item_id对应的电器的状态
        private void item_Change(NlpApp.CommandItem itemCommand)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            switch (itemCommand.item_id)
            {
                case "1":
                    switch (itemCommand.action)
                    {
                        case "打开" or "开" or "开启" or "开机":
                            Image imageTemp = Image.FromHbitmap(Resource1.light_on.GetHbitmap());
                            pictureBox1.Image = imageTemp;
                            break;
                        case "关闭" or "关":
                            imageTemp = Image.FromHbitmap(Resource1.light_off.GetHbitmap());
                            pictureBox1.Image = imageTemp;
                            break;
                    }
                    break;
                case "2":
                    {
                        switch (itemCommand.action)
                        {
                            case "打开" or "开启" or "开机" or "开": 
                                tv = true;
                                textBox6.Text = "电视已开启";
                                break;
                            case "关闭" or "关机" or "关": 
                                tv = false;
                                textBox6.Text = "电视已关闭";
                                break;
                            case "切换下一台" or "切换下一频道":
                                if (tv){
                                    channel = (channel + 1) % 255;
                                    textBox6.Text = "电视切换到第" + channel + "频道";
                                    break;//最多255个频道
                                }
                                else {
                                    textBox6.Text = "（电视还未开启！）";
                                    break;//最多255个频道
                                }
                            case "切换上一台" or "切换上一频道":
                                if (tv) {
                                    channel = (255 + channel - 1) % 255;
                                    textBox6.Text = "电视切换到第" + channel + "频道";
                                    break;//最多255个频道
                                }
                                else{
                                    textBox6.Text = "（电视还未开启！）";
                                    break;//最多255个频道
                                }
                            default:
                                {
                                    if (Regex.Match(itemCommand.action, @"切换到?第?\d+") == null) break;
                                    if (!tv)
                                    {
                                        //当电视尚未打开时，跳转频道不会生效，所以显示效果不会改变
                                        textBox6.Text = "（电视还未开启！）";
                                        return;
                                    }
                                    if (Regex.IsMatch(itemCommand.action, @"\d+"))
                                    {
                                        channel = int.Parse(Regex.Match(itemCommand.action, @"\d+").Value);
                                    }
                                    else {
                                        string num = Regex.Match(itemCommand.action, @"[零一二三四五六七八九十]").Value;
                                        switch (num) {
                                            case "零":
                                                channel = 0;
                                                break;
                                            case "一":
                                                channel = 1;
                                                break;
                                            case "二":
                                                channel = 2;
                                                break;
                                            case "三":
                                                channel = 3;
                                                break;
                                            case "四":
                                                channel = 4;
                                                break;
                                            case "五":
                                                channel = 5;
                                                break;
                                            case "六":
                                                channel = 6;
                                                break;
                                            case "七":
                                                channel = 7;
                                                break;
                                            case "八":
                                                channel = 8;
                                                break;
                                            case "九":
                                                channel = 9;
                                                break;
                                            case "十":
                                                channel = 10;
                                                break;
                                        }
                                    }
                                    textBox6.Text = "电视切换到第" + channel + "频道";
                                }
                                break;
                        }
                    }
                    break;
                case "3":
                    {
                        string area = "客厅";
                        switch (itemCommand.action)
                        {
                            case "打开" or "开启" or "开机" or "开": 
                                robot = true;
                                textBox7.Text = "扫地机已开启";
                                break;
                            case "关闭" or "关机" or "关": 
                                robot = false;
                                textBox7.Text = "扫地机已关闭";
                                break;
                            case "扫地" or "打扫" or "打扫全屋" or "打扫屋子" or "打扫整个屋子" or "打扫全部" or "打扫全部房间":
                                if (!robot)
                                {
                                    //当扫地机器人尚未打开时，扫地指令不会生效，所以显示不会改变
                                    textBox7.Text = "（扫地机还未开启！）";
                                    return;
                                }
                                textBox7.Text = "扫地机开始打扫整个屋子。";
                                break;
                            default:
                                {
                                    if (!robot)
                                    {
                                        //当扫地机器人尚未打开时，扫地指令不会生效，所以显示不会改变
                                        textBox7.Text = "（扫地机还未开启！）";
                                        return;
                                    }
                                    if (Regex.IsMatch(itemCommand.action, @"[到去](客厅|卧室|厨房|书房)(扫地|打扫)"))
                                    {
                                        area = Regex.Match(itemCommand.action, @"(客厅|卧室|厨房|书房)").Value;
                                        textBox7.Text = "扫地机前往" + area + "扫地";
                                        return;
                                    }
                                    else if (Regex.IsMatch(itemCommand.action, @"打扫(客厅|卧室|厨房|书房)")) {
                                        area = Regex.Match(itemCommand.action, @"(客厅|卧室|厨房|书房)").Value;
                                        textBox7.Text = "扫地机前往" + area + "扫地";
                                        return;
                                    }
                                }
                                break;
                        }

                    }
                    break;
            }
        }
    }
}
