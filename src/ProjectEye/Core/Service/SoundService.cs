using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows;
using System.Windows.Resources;

namespace ProjectEye.Core.Service
{
    public class SoundService : IService
    {
        private SoundPlayer player;
        public SoundService()
        {
            player = new SoundPlayer();


        }
        public void Play()
        {
            if (player.IsLoadCompleted)
            {
                player.PlaySync();
            }
        }


        public void Init()
        {
            Uri soundUri = new Uri("/ProjectEye;component/Resources/relentless.wav", UriKind.RelativeOrAbsolute);
            StreamResourceInfo info = Application.GetResourceStream(soundUri);
            player.Stream = info.Stream;

            player.LoadAsync();

        }
    }
}
