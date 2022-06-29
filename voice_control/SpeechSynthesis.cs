using Baidu.Aip;
using Baidu.Aip.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace voice_control
{
    public interface ISpeechSynthesis
    {
        void SpeechSynthesisAndPlay(string text);
    }
    internal class SpeechSynthesis : ISpeechSynthesis
    {
        [DllImport("winmm.dll", SetLastError = true)]
        //#pragma warning disable IDE1006 // 命名样式
        static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);
        //#pragma warning restore IDE1006 // 命名样式

        /// <summary>
        /// 语音合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SpeechSynthesisAndPlay(string text)
        {
            // 入口条件检查
            if (text.Trim() == string.Empty)
                return;

            // 语音合成
            TtsResponse result;
            try
            {
                result = TtsApp.getApp().client.Synthesis(text);
            }
            catch (AipException exp)
            {
                MessageBox.Show(exp.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (result.Success)
            {
                // 保存为mp3格式
                File.WriteAllBytes("temp.mp3", result.Data);

                // 播放音频文件
                mciSendString("open temp.mp3 alias temp_alias", null, 0, IntPtr.Zero);
                mciSendString("play temp_alias", null, 0, IntPtr.Zero);

                // 等待播放结束
                StringBuilder strReturn = new StringBuilder(64);
                do
                {
                    mciSendString("status temp_alias mode", strReturn, 64, IntPtr.Zero);
                } while (!strReturn.ToString().Contains("stopped"));

                // 关闭音频文件
                mciSendString("close temp_alias", null, 0, IntPtr.Zero);
            }
            else
                MessageBox.Show(result.ErrorMsg, "转换失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
