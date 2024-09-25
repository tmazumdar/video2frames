// See https://aka.ms/new-console-template for more information
using video2frames;

Console.WriteLine("Video to Frames Generator");
Console.WriteLine("Current Directory: " + Directory.GetCurrentDirectory());
Console.WriteLine("Please enter a video file location:");
var videoFileLocation = Console.ReadLine();

while (!File.Exists(@videoFileLocation))
{
    Console.WriteLine("Not found: " + videoFileLocation);
    Console.WriteLine("Please enter a video file location:");
    videoFileLocation = Console.ReadLine();
}

Console.WriteLine("Please specify an output directory:");
var outputDirectory = Console.ReadLine();

while (outputDirectory != null && !Directory.Exists(outputDirectory))
{
    Console.WriteLine("Directory does not exist. Create (y/n)? " + outputDirectory);
    var key = Console.ReadKey().KeyChar;
    if (key == 'y')
    {
        DirectoryInfo di = Directory.CreateDirectory(outputDirectory);
        Console.WriteLine("\nThe directory was created successfully at {0}.", Directory.GetCreationTime(outputDirectory));
    }
    else
    {
        Console.WriteLine("Please specify an output directory:");
        outputDirectory = Console.ReadLine();
    }
}

// run
if (outputDirectory != null)
{
    V2F v2f = new(videoFileLocation, outputDirectory, 15);
    Console.WriteLine("Frames Per Second (FPS): " + v2f.GetVideoFPS());
    v2f.ConvertToImages();
}