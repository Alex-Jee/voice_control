using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace voice_control
{
    public delegate void OnSpeechResultDelegate(string text);
    public delegate void OnSpeechStateDelegate(string state);
    public interface ISpeechRecorder
    {
        void SetFileName(string fileName);
        void StartRec();
        void StopRec();
    }

    internal class SpeechRecord : ISpeechRecorder
    {
        public WaveIn waveSource = null;
        public WaveFileWriter waveFile = null;
        private string fileName = System.Environment.CurrentDirectory + "\\test.wav";
        private long lastNoiseTimestamp = 0;
        private long maxRecordSeconds = 10;
        private long startRecordTimestamp = 0;
        private bool runningFlag = false;
        OnSpeechResultDelegate result_callback = null;
        OnSpeechStateDelegate state_callback = null;

        private long GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        public void SetResultCallbackDelegate(OnSpeechResultDelegate callback) {
            result_callback = callback;
        }

        public void SetStateCallbackDelegate(OnSpeechStateDelegate callback)
        {
            state_callback = callback;
        }

        private void UpdateState(string text) {
            if (state_callback != null) {
                state_callback(text);
            }
        }

        /// <summary>
        /// 第二步：开始录音
        /// </summary>
        public void StartRec()
        {
            try
            {
                if (runningFlag)
                    return;
                runningFlag = true;

                waveSource = new WaveIn();//保证电脑有麦克接入否则报错。
                waveSource.WaveFormat = new WaveFormat(16000, 16, 1); // 16KHz，16bit,单声道的录音格式

                waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
                waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);

                waveFile = new WaveFileWriter(fileName, waveSource.WaveFormat);

                lastNoiseTimestamp = GetTimeStamp();
                startRecordTimestamp = GetTimeStamp();

                waveSource.StartRecording();
            }
            catch (Exception e)
            {
                UpdateState("开启设备失败，请检查本地麦克风设备！");
                //throw new Exception(e.Message);
            }

        }

        /// <summary>
        /// 第三步：停止录音
        /// </summary>
        public void StopRec()
        {
            if (!runningFlag)
                return;

            runningFlag = false;

            waveSource.StopRecording();

            // Close Wave(Not needed under synchronous situation)
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }

            if (waveFile != null)
            {
                waveFile.Dispose();
                waveFile = null;
            }

            Thread vThread = new Thread(this.IdentifyThread);
            vThread.Start();
        }

        /// <summary>
        /// 第一步：设置录音结束后保存的文件路径
        /// </summary>
        /// <param name="fileName">保存wav文件的路径名</param>
        public void SetFileName(string fileName)
        {
            this.fileName = fileName;
        }

        private void IdentifyThread() {
            UpdateState("识别中...");

            Thread.Sleep(100);
            string result_text = AsrApp.GetTextFromFile(fileName);
            if (result_text == "")
                result_text = "未识别到语音";

            if(result_callback != null)
                result_callback(result_text);

            UpdateState("识别完成！");
        }

        /// <summary>
        /// 开始录音回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveFile != null)
            {
                waveFile.Write(e.Buffer, 0, e.BytesRecorded);
                waveFile.Flush();

                int max = 0;
                ulong sum = 0;
                int validNum = 0;
                for (int i = 0; i < e.BytesRecorded; i++) {
                    if (e.Buffer[i] == 255)
                        continue;
                    if(max < e.Buffer[i])
                        max = e.Buffer[i];
                    sum += e.Buffer[i];
                    if (e.Buffer[i] > 130)
                        validNum++;

                }

                UpdateState("正在讲话...");

                // 最长时长到了，开始识别
                if (GetTimeStamp() - startRecordTimestamp > maxRecordSeconds) {
                    StopRec();
                }
                ulong average = sum / (ulong)e.BytesRecorded;
                //System.Diagnostics.Debug.WriteLine("max:" + max.ToString() + ",average:" + average.ToString()+ ",validNum:"+ validNum.ToString() + ",total:"+ e.BytesRecorded.ToString());
                if (average < 100) {
                    //静音超过2秒，开始识别
                    if (GetTimeStamp() - lastNoiseTimestamp > 2)
                    {
                        StopRec();
                    }
                }
                else {
                    lastNoiseTimestamp = GetTimeStamp();
                }
            }
        }

        /// <summary>
        /// 录音结束回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }

            if (waveFile != null)
            {
                waveFile.Dispose();
                waveFile = null;
            }
        }

    }
}
