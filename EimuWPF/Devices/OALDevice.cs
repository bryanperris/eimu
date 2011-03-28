//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Eimu.Core;
//using Eimu.Core.Systems.Chip8X
//using OpenTK;
//using OpenTK.Audio.OpenAL;
//using OpenTK.Audio;
//using System.Runtime.InteropServices;
//using System.Security;

//namespace Eimu.Devices
//{
//    public sealed class OALDevice : AudioDevice
//    {
//        private AudioContext m_Context;
//        private uint m_Buffer;
//        private byte[] m_Wave;
//        private uint m_Source;

//        public override void Beep()
//        {
//            AL.SourcePlay(m_Source);
//        }

//        protected unsafe override void OnInit()
//        {
//            m_Wave = new byte[100];

//            for (int i = 0; i < m_Wave.Length; i+=2)
//            {
//                m_Wave[i+1] = 0xB6;
//                m_Wave[i] = 0x0D; //DB6
//            }

//            m_Context = new AudioContext();
//            AL.GenBuffer(out m_Buffer);

//            IntPtr wavePtr;

//            fixed (byte* ptr = m_Wave)
//            {
//                wavePtr = new IntPtr((void*)ptr);
//            }

//            //AL.BufferData(m_Buffer, ALFormat.Stereo16, wavePtr, m_Wave.Length, 44100);

//            AL.GenSource(out m_Source);

//            AL.SourceQueueBuffer((int)m_Source, (int)m_Buffer);

            
//        }

//        protected override void OnShutdown()
//        {
//            AL.DeleteBuffer(ref m_Buffer);
//            m_Context.Dispose();
//        }

//        protected override void OnPauseStateChange(bool paused)
//        {
            
//        }
//    }
//}
