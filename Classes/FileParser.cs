
using System.IO;

public class FileParser
{
    private string InputFile { get; set; }
    private string OutputFile { get; set; }
    private HackLanguageOptions Options { get; set; }
    private List<string> ParsedInstructions { get; set; }
    private Dictionary<string, int> Variables { get; set; }
    private Dictionary<string, int> Labels { get; set; }
  
    public FileParser(string inFile, string outFile, HackLanguageOptions options) 
    {
        InputFile = inFile;
        OutputFile = outFile;
        Options = options;
        ParsedInstructions = new List<string>();
        Variables = new Dictionary<string, int>();
        Labels = new Dictionary<string, int>();
    } 

    public void Parse() 
    {
        int lineCounter = 0;       

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
                    lineCounter++;  
                    continue;
                } 

                if(IsCInstruction(line)) 
                {
                    ParsedInstructions.Add(ParseCInstruction(line, lineCounter));
                    lineCounter++;  
                    continue;
                }

                if(IsLabel(line)) 
                {
                    ParseLabel(line, lineCounter);
                    continue;
                }            
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

    private bool IsLabel(string codeLine) 
    {
        return codeLine.Trim().StartsWith('(') && codeLine.Trim().EndsWith(')');
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
        } else //variable
        {
            int parsedAddress = DecodeSymbol(value);
            output += Convert.ToString(parsedAddress, 2).PadLeft(15, '0');           
        }

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
           output += "000"; //Do not store result 
        }

        if(jmp != null) 
        {
            output += jmp.Value;
        } else {
           output += "000"; //Do not jump
        }

        return output;
    }

    private void ParseLabel(string instruction, int lineNum) 
    {
        Console.WriteLine($"Label: {instruction}, Line Num: {lineNum}");

        string value = instruction.Replace("(", "").Replace(")", "");
    
        if(!Labels.ContainsKey(value)) 
        {
            Labels.Add(value, lineNum);
        } else 
        {
            if(Labels[value] == -1) 
            {
                Labels[value] = lineNum;
                Console.WriteLine($"Label {Labels[value]} was seen before definition. Now set to {lineNum}");
            }
        }
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

    private int DecodeSymbol(string input) 
    {
        Console.WriteLine($"Before Decode Symbol: {input}");

        Symbol symb = Options.SymbolMap.Where(a => a.Name == input).FirstOrDefault();

        if(symb != null) 
        {
            Console.WriteLine($"Decoded Symbol as predefined: {symb.Address}");
            return symb.Address;
        } else 
        {
            if(input.Replace("_", "").All(c => char.IsUpper(c))) //Label variable
            {
                if(Labels.ContainsKey(input)) 
                {
                    Console.WriteLine($"Decoded Symbol as defined label: {Labels[input]}");
                    return Labels[input];
                } else 
                {
                    Console.WriteLine($"Decoded Symbol as label not yet defined: {input}");
                    Labels.Add(input, -1);
                }
            } else //custom variable
            {
                if(Variables.ContainsKey(input)) 
                {
                    Console.WriteLine($"Decoded Symbol as Custom Variable: {Variables[input]}");
                    return Variables[input];
                } else 
                {
                    int nextAddress = Options.UserVariableStartingAddress + Variables.Count;
                    Variables.Add(input, nextAddress);

                    Console.WriteLine($"Decoded Symbol as Custom Variable: {nextAddress}");
                    return nextAddress;
                }
            }

            return 0;
        }
    }
}