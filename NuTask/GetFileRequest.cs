using System;
using Avalonia.Media.Imaging;

namespace NuTask;

public static class GetFileRequest
{
    public static Bitmap GetImageFilesBitMap(string imageName)
    {
        string path;
        try
        {
            path = $"Images//{imageName}.png";
            return new Bitmap(path);
        }
        catch(Exception exception)
        {
            throw;
        }
    }
}