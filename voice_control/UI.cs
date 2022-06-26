namespace voice_control
{
    public partial class UI : Form
    {
        SpeechRecord speech_record = new SpeechRecord();

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
            UpdateUIForThread(2,text);
        }

        private void UpdateUIForThread(int source, string text)
        {
            this.Invoke(new Action(() =>
            {
                switch (source) {
                    case 1:
                        labelState.Text = text;
                        break;
                    case 2:
                        textBox2.Text = text;
                        buttonSpeechMic.Enabled = true;
                        buttonIdentifyRightnow.Enabled = false;
                        depItems=NlpApp.TextParser(text);
                        //TODO:��������ָ��
                        //.....

                        break;
                    default:
                        break;
                }
            }));
        }
    }
}