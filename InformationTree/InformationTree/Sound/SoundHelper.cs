using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;

namespace FormsGame.Sound
{
    public static class SoundHelper
    {
        #region PlaySound

        public static void PlaySound(string file)
        {
            new SoundPlayer(file)?.Play();
        }

        #endregion PlaySound

        #region PlaySystemSound

        public static void PlaySystemSound(string soundName)
        {
            switch((soundName ?? "").ToLowerInvariant())
            {
                case "1":
                case "asterisk":
                    SystemSounds.Asterisk.Play();
                    break;
                case "2":
                case "beep":
                    SystemSounds.Beep.Play();
                    break;
                case "3":
                case "exclamation":
                    SystemSounds.Exclamation.Play();
                    break;
                case "4":
                case "hand":
                    SystemSounds.Hand.Play();
                    break;
                case "5":
                case "question":
                    SystemSounds.Question.Play();
                    break;
            }
        }

        public static void PlaySystemSound(int soundNumber)
        {
            if (soundNumber < 1 || soundNumber > 5)
                return;

            PlaySystemSound(soundNumber.ToString());
        }

        #endregion PlaySystemSound
    }
}
