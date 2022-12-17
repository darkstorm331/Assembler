public class FileParser
{
    private string InputFile { get; set; }
    private string OutputFile { get; set; }

    public FileParser(string inFile, string outFile) 
    {
        InputFile = inFile;
        OutputFile = outFile;
    } 

    public void Parse() 
    {
        Console.WriteLine($"Input File: {InputFile}, Output File: {OutputFile}");
        
        //Main logic here
    }
}