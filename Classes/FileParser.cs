
using System.IO;

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
        int lineCounter = 1;

        try 
        {
            foreach (string line in File.ReadLines(InputFile))
            {  
                if(IsAInstruction(line)) 
                {
                    ParseAInstruction(line, lineCounter);
                    continue;
                } 

                if(IsCInstruction(line)) 
                {
                    ParseCInstruction(line, lineCounter);
                    continue;
                }
               
                lineCounter++;  
            }            
        } catch 
        {
            throw;
        }
    }

    private bool IsAInstruction(string codeLine) 
    {
        return codeLine.Trim().StartsWith('@');
    }

    private bool IsCInstruction(string codeLine) 
    {
        if(!codeLine.Trim().StartsWith("//")) 
        {
            return false;
        }

        if(!codeLine.Contains('=')) {
            return false;
        }

        string commentsRemoved = "";
        try 
        {
            commentsRemoved = codeLine.Split("//")[0].Trim();
        } catch //No comments
        {
            commentsRemoved = codeLine;
        }

        if(commentsRemoved.Contains(';')) 
        {
            string command = commentsRemoved.Split('=')[0].Trim();
            string destination = commentsRemoved.Split('=')[1].Trim().Split(';')[0].Trim();
            string jump = commentsRemoved.Split('=')[1].Trim().Split(';')[1].Trim();

            Comp commandMatch = Options.CompMap.FirstOrDefault(cmp => cmp.Instruction == command);       
            Dest destinationMatch = Options.DestMap.FirstOrDefault(dest => dest.Instruction == destination);
            Jump jumpMatch = Options.JumpMap.FirstOrDefault(jmp => jmp.Instruction == jump);

            if(commandMatch == null || destinationMatch == null || jumpMatch == null) 
            {
                return false;
            }
        } else 
        {
            string[] command = commentsRemoved.Split('=').Select(a => a.Trim()).ToArray(); 
            Comp commandMatch = Options.CompMap.FirstOrDefault(cmp => cmp.Instruction == command[0]);       
            Dest destinationMatch = Options.DestMap.FirstOrDefault(dest => dest.Instruction == command[1]);

            if(commandMatch == null || destinationMatch == null) 
            {
                return false;
            }
        }

        return true;
    }

    private void ParseAInstruction(string instruction, int lineNum) 
    {
        //TODO
        Console.WriteLine(instruction);
    }

    private void ParseCInstruction(string instruction, int lineNum) 
    {
        //TODO
        Console.WriteLine(instruction);
    }
}