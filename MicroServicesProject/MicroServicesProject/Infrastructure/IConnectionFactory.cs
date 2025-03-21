namespace MicroServicesProject.Infrastructure
{
    public interface IConnectionFactory
    {
       DAL GetDAL {  get; }

        static void DisplayGreet()
        {
            Console.WriteLine("Hello");
        }
    }
}
