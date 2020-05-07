using System;

namespace CoreConsoleApp
{
    class Person
    {
        public string Name { get; set; }
        public virtual void Display() => Console.WriteLine($"Person {Name}");
    }
    class Client : Person
    {
        public override void Display() => Console.WriteLine($"Client {Name}");
    }

    delegate void GetInfo<in T>(T item);

    class Program
    {
        static void Main(string[] args)
        {
            
                GetInfo<Person> personInfo = PersonInfo;
                GetInfo<Client> clientInfo = personInfo;
                // контравариантность

                Client client = new Client { Name = "Tom" };
                clientInfo(client); // Client: Tom

                Console.Read();
        }

        private static void PersonInfo(Person p) => p.Display();
        private static void ClientInfo(Client cl) { }
    }
}
