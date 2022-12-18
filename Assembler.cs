using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

class Assembler
{
    static void Main(string[] args)
    {
        try 
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)                
                .Build();            

            HackLanguageOptions options = new();
            config.GetSection(nameof(HackLanguageOptions))
                .Bind(options);

            if(!ArgsValid(args)) 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Arguments were either not provided, or were not valid");
                return;
            }

            Console.WriteLine($"{args[0]} will now be read"); 
            FileParser fp = new FileParser(args[0], args[1], options);
            fp.Parse();
            Console.WriteLine($"{args[0]} was successfully read. Will now be written to {args[1]}"); 
            fp.WriteOutParsedInstructions();
            Console.WriteLine($"{args[1]} was successfully written");
        }
        catch (Exception err)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(err.GetBaseException().Message);
        } finally 
        {
            Console.WriteLine("Terminating...");
        }
    }

    static bool ArgsValid(string[] args) {
        int argLength = args.Length;

        //Check that there are 2 arguments (Input file and Output file)
        if(argLength != 2) 
        {
            return false;
        }

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