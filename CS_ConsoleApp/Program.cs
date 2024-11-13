namespace CS_ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");



            SecurityOperation sc = new SecurityOperation();
            sc.ImportDataFromCsv(@"C:\Users\asparanjape\Downloads\Data for securities.xlsx - Equities.csv");
        }
    }
}
