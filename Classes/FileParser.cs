public class FileParser
{
    private string InputFile { get; set; }
    private string OutputFile { get; set; }
    private HackLanguageOptions Options { get; set; }
    
    public FileParser(string inFile, string outFile, HackLanguageOptions options) 
    {
        InputFile = inFile;
        OutputFile = outFile;
        Options = options;
    } 

    public void Parse() 
    {
        //Main logic here
        //TODO
    }
}