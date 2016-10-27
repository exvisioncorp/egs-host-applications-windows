namespace Egs.ZkooTutorial
{
    using System;

    interface IAudioPlayer
    {
        bool IsToShowMessageBoxOfExceptions { get; set; }
        string AudioFileFullPath { get; }
        bool IsActuallyPlaying { get; }
        bool StartAsync(string audioFilePath);
        bool Stop();
        //void TogglePause();
    }
}
