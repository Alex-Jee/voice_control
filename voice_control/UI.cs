namespace voice_control
{
    public partial class UI : Form
    {
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}