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
                openFileDialog1.InitialDirectory = "c:\\";//ע������д·��ʱҪ��c:\\������c:\
                openFileDialog1.Filter = "pcm��Ƶ|*.pcm|wav��Ƶ|*.wav|amr��Ƶ|*.amr";
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
                        //TODO:��������ָ��
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

        // ���ڸ��ݽ����õ���CommandItem���ı�item_id��Ӧ�ĵ�����״̬
        private void item_Change(NlpApp.CommandItem itemCommand)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            switch (itemCommand.item_id)
            {
                case "1":
                    switch (itemCommand.action)
                    {
                        case "��" or "��" or "����" or "����":
                            Image imageTemp = Image.FromHbitmap(Resource1.light_on.GetHbitmap());
                            pictureBox1.Image = imageTemp;
                            break;
                        case "�ر�" or "��":
                            imageTemp = Image.FromHbitmap(Resource1.light_off.GetHbitmap());
                            pictureBox1.Image = imageTemp;
                            break;
                    }
                    break;
                case "2":
                    {
                        switch (itemCommand.action)
                        {
                            case "��" or "����" or "����" or "��": 
                                tv = true;
                                textBox6.Text = "�����ѿ���";
                                break;
                            case "�ر�" or "�ػ�" or "��": 
                                tv = false;
                                textBox6.Text = "�����ѹر�";
                                break;
                            case "�л���һ̨" or "�л���һƵ��":
                                if (tv){
                                    channel = (channel + 1) % 255;
                                    textBox6.Text = "�����л�����" + channel + "Ƶ��";
                                    break;//���255��Ƶ��
                                }
                                else {
                                    textBox6.Text = "�����ӻ�δ��������";
                                    break;//���255��Ƶ��
                                }
                            case "�л���һ̨" or "�л���һƵ��":
                                if (tv) {
                                    channel = (255 + channel - 1) % 255;
                                    textBox6.Text = "�����л�����" + channel + "Ƶ��";
                                    break;//���255��Ƶ��
                                }
                                else{
                                    textBox6.Text = "�����ӻ�δ��������";
                                    break;//���255��Ƶ��
                                }
                            default:
                                {
                                    if (Regex.Match(itemCommand.action, @"�л���?��?\d+") == null) break;
                                    if (!tv)
                                    {
                                        //��������δ��ʱ����תƵ��������Ч��������ʾЧ������ı�
                                        textBox6.Text = "�����ӻ�δ��������";
                                        return;
                                    }
                                    if (Regex.IsMatch(itemCommand.action, @"\d+"))
                                    {
                                        channel = int.Parse(Regex.Match(itemCommand.action, @"\d+").Value);
                                    }
                                    else {
                                        string num = Regex.Match(itemCommand.action, @"[��һ�����������߰˾�ʮ]").Value;
                                        switch (num) {
                                            case "��":
                                                channel = 0;
                                                break;
                                            case "һ":
                                                channel = 1;
                                                break;
                                            case "��":
                                                channel = 2;
                                                break;
                                            case "��":
                                                channel = 3;
                                                break;
                                            case "��":
                                                channel = 4;
                                                break;
                                            case "��":
                                                channel = 5;
                                                break;
                                            case "��":
                                                channel = 6;
                                                break;
                                            case "��":
                                                channel = 7;
                                                break;
                                            case "��":
                                                channel = 8;
                                                break;
                                            case "��":
                                                channel = 9;
                                                break;
                                            case "ʮ":
                                                channel = 10;
                                                break;
                                        }
                                    }
                                    textBox6.Text = "�����л�����" + channel + "Ƶ��";
                                }
                                break;
                        }
                    }
                    break;
                case "3":
                    {
                        string area = "����";
                        switch (itemCommand.action)
                        {
                            case "��" or "����" or "����" or "��": 
                                robot = true;
                                textBox7.Text = "ɨ�ػ��ѿ���";
                                break;
                            case "�ر�" or "�ػ�" or "��": 
                                robot = false;
                                textBox7.Text = "ɨ�ػ��ѹر�";
                                break;
                            case "ɨ��" or "��ɨ" or "��ɨȫ��" or "��ɨ����" or "��ɨ��������" or "��ɨȫ��" or "��ɨȫ������":
                                if (!robot)
                                {
                                    //��ɨ�ػ�������δ��ʱ��ɨ��ָ�����Ч��������ʾ����ı�
                                    textBox7.Text = "��ɨ�ػ���δ��������";
                                    return;
                                }
                                textBox7.Text = "ɨ�ػ���ʼ��ɨ�������ӡ�";
                                break;
                            default:
                                {
                                    if (!robot)
                                    {
                                        //��ɨ�ػ�������δ��ʱ��ɨ��ָ�����Ч��������ʾ����ı�
                                        textBox7.Text = "��ɨ�ػ���δ��������";
                                        return;
                                    }
                                    if (Regex.IsMatch(itemCommand.action, @"[��ȥ](����|����|����|�鷿)(ɨ��|��ɨ)"))
                                    {
                                        area = Regex.Match(itemCommand.action, @"(����|����|����|�鷿)").Value;
                                        textBox7.Text = "ɨ�ػ�ǰ��" + area + "ɨ��";
                                        return;
                                    }
                                    else if (Regex.IsMatch(itemCommand.action, @"��ɨ(����|����|����|�鷿)")) {
                                        area = Regex.Match(itemCommand.action, @"(����|����|����|�鷿)").Value;
                                        textBox7.Text = "ɨ�ػ�ǰ��" + area + "ɨ��";
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
