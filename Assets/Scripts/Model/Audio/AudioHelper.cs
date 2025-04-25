namespace MH
{
    public static class AudioHelper
    {
        public static void PlaySoundEffect(Scene root, ClipID clipID)
        {
            root.Root.GetComponent<AudioComponent>().PlaySoundEffect(clipID);
        }
        public static void PlayBackgroundMusic(Scene root, ClipID clipID)
        {
            root.Root.GetComponent<AudioComponent>().PlayBackgroundMusic(clipID);
        }
    }
}