

class Assembler
{
    static void Main(string[] args)
    {
        if(!ArgsValid(args)) 
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Arguments were either not provided, or were not valid");
            return;
        }

        FileParser fp = new FileParser(args[0], args[1]);
        fp.Parse();
    }

    static bool ArgsValid(string[] args) {
        int argLength = args.Length;

        //Check that there are 2 arguments (Input file and Output file)
        if(argLength != 2) 
        {
            return false;
        }

        //Check that each argument is a valid file path 
        //TODO

        string inputFileExtension = Path.GetExtension(args[0]);
        string outputFileExtension = Path.GetExtension(args[1]);

        //Check that the input file is a .asm file
        if(inputFileExtension != ".asm") 
        {
            return false;
        }

        //Check that the output file is a .hack file
        if(outputFileExtension != ".hack") 
        {
            return false;
        }

        return true;
    }
}