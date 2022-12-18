
using System.IO;

public class FileParser
{
    private string InputFile { get; set; }
    private string OutputFile { get; set; }
    private HackLanguageOptions Options { get; set; }
    public List<string> ParsedInstructions { get; set; }
  
    public FileParser(string inFile, string outFile, HackLanguageOptions options) 
    {
        InputFile = inFile;
        OutputFile = outFile;
        Options = options;
        ParsedInstructions = new List<string>();
    } 

    public void Parse() 
    {
        int lineCounter = 1;

        try 
        {
            if(!IsValidFile(InputFile)) 
            {
                throw new Exception("Invalid Input file path");
            }

            foreach (string line in File.ReadLines(InputFile))
            {  
                //Ignore comments and blank space
                if(line.Trim().StartsWith("//") || string.IsNullOrWhiteSpace(line)) 
                {
                    Console.WriteLine($"Ignored: {line}");
                    continue;
                }

                if(IsAInstruction(line)) 
                {
                    ParsedInstructions.Add(ParseAInstruction(line, lineCounter));
                    continue;
                } 

                if(IsCInstruction(line)) 
                {
                    ParsedInstructions.Add(ParseCInstruction(line, lineCounter));
                    continue;
                }
               
                lineCounter++;  
            } 
        } catch 
        {
            throw;
        }
    }

    public void WriteOutParsedInstructions() 
    {
        if(!IsValidFile(OutputFile)) 
        {
            throw new Exception("Invalid Output file path");
        }

        File.WriteAllLines(OutputFile, ParsedInstructions);
    }

    private bool IsAInstruction(string codeLine) 
    {
        return codeLine.Trim().StartsWith('@');
    }

    private bool IsCInstruction(string codeLine) 
    {
        if(!codeLine.Contains('=')) {
            return false;
        }

        return true;
    }

    private string ParseAInstruction(string instruction, int lineNum) 
    {
        /*
            A Instructions always start with a 0, and the other 15 bits represent the address

            0AAAAAAA AAAAAAAA
        */
        Console.WriteLine($"A Instruction: {instruction}");
        string output = "0";

        //Get rid of any trailing comments
        instruction = RemoveTrailingComments(instruction);

        string value = instruction.Replace("@", "");

        int address;
        if(int.TryParse(value, out address)) 
        {
            output += Convert.ToString(address, 2).PadLeft(15, '0');           
        } else //variable or label
        {
            //TODO
        }

        Console.WriteLine($"Parsed A Instruction: {output}"); 
        return output;       
    }

    private string ParseCInstruction(string instruction, int lineNum) 
    {
        /*
            C Instructions start with a 1 and are broken down as below:

            111 - First 3 bits are always 1
            a - changes A to M in instructions if 1
            c - instruction
            d - destination
            j - jump

            1111acccc ccdddjjj
        */
        Console.WriteLine($"C Instruction: {instruction}");
        string output = "111";

        //Get rid of any trailing comments
        instruction = RemoveTrailingComments(instruction);

        string d = instruction.Split('=')[0].Trim();
        string c = "";
        string j = "";
        if(instruction.Contains(';')) //Has a jump operation
        {
            j = instruction.Split(';')[1].Trim();
            c = instruction.Split('=')[1].Trim().Split(';')[0].Trim();
        } else 
        {
            c = instruction.Split('=')[1].Trim();
        }

        Comp cmp = Options.CompMap.Where(a => a.Instruction == c).FirstOrDefault();
        Dest dst = Options.DestMap.Where(a => a.Instruction == d).FirstOrDefault();
        Jump jmp = Options.JumpMap.Where(a => a.Instruction == j).FirstOrDefault();

        if(cmp != null) 
        {
            output += cmp.Value;
        } else {
           throw new Exception($"Command '{c}' was invalid"); 
        }

        if(dst != null) 
        {
            output += dst.Value;
        } else {
           throw new Exception($"Destimation '{d}' was invalid"); 
        }

        if(jmp != null) 
        {
            output += jmp.Value;
        } else {
           output += "000"; //Do not jump
        }

        Console.WriteLine($"Parsed C Instruction: {output}");        
        return output;
    }

    private string RemoveTrailingComments(string codeLine) 
    {
        string output = codeLine;

        try 
        {
            output = codeLine.Split("//")[0].Trim();
            return output;
        } catch 
        {
            return codeLine;
        }
    }

    private bool IsValidFile(string path) 
    {
        return path.IndexOfAny(Path.GetInvalidPathChars()) == -1;
    }
}