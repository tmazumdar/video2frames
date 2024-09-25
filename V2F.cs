using System.Diagnostics;

namespace video2frames;

public class V2F
{
    private string _videoFilePath;
    private string _outputPath;
    private string _command;
    private int _fps;

    public V2F(string videoFilePath, string outputPath, int fps = 0, string outputFormat = "image_%04d.jpg")
    {
        _videoFilePath = videoFilePath;
        _outputPath = string.Join('/', [outputPath, outputFormat]);
        _fps = fps;
        _command = GetCommandString();
    }

    private string GetFrameRate()
    {
        string ffprobeCommand = $"-v error -select_streams v:0 -show_entries stream=r_frame_rate -of default=noprint_wrappers=1:nokey=1 \"{_videoFilePath}\"";

        using Process process = new();
        process.StartInfo.FileName = "ffprobe";
        process.StartInfo.Arguments = ffprobeCommand;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return output.Trim(); // Return the frame rate string
    }

    private int GetFramesPerSecond()
    {
        string frameRate = GetFrameRate();
        string[] fSplit = frameRate.Split('/');
        _fps = int.Parse(fSplit[0]) / int.Parse(fSplit[1]);
        return _fps;
    }

    private string GetCommandString()
    {
        var fps = _fps != 0 ? _fps : GetFramesPerSecond();
        _command = $"-i \"{_videoFilePath}\" -vf \"fps={fps}\" \"{_outputPath}\"";
        return _command;
    }

    public string GetVideoFPS()
    {
        return _fps.ToString();
    }

    public bool ConvertToImages()
    {
        ProcessStartInfo processStartInfo = new()
        {
            FileName = "ffmpeg",
            Arguments = _command,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = new()
        { StartInfo = processStartInfo })
        {
            process.Start();

            // Read the output (optional)
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            // Display output (if needed)
            if (!string.IsNullOrEmpty(output))
            {
                Console.WriteLine("Output: " + output);
            }
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("Error: " + error);
                return false;
            }
        }

        Console.WriteLine("Video conversion to images completed.");
        return true;
    }
}
