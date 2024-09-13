using EFCoreSpecificationExample.BLL;
using EFCoreSpecificationExample.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;

namespace EFCoreSpecificationExample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var repository = GetRepository();
            var currentStateEndTime = new DateTime(9999, 12, 31, 23, 59, 59, 999);

            Console.WriteLine("Query with parameters...");
            DisplayData(repository.Search(currentOnly: true, stateCode: "DONE"));

            Console.WriteLine("Query with predicate...");
            DisplayData(repository.Search(wi => wi.StateEndTime == currentStateEndTime && wi.StateCode == "DONE"));

            Console.WriteLine("Query with specification...");
            var currentState = new WorkItemData.CurrentStateSpecification();
            var stateIsDone = new WorkItemData.StateIsSpecification("DONE");
            DisplayData(repository.Search(currentState.And(stateIsDone)));

            Console.Write("\nDone...");
            Console.ReadLine();
        }

        public static IWorkItemRepository GetRepository()
        {
            var connectionString = "Server=DESKTOP-PTKPKQ7\\SQLEXPRESS; Database=EFSpecificationExample; Integrated Security=True; Encrypt=False";
            var services = new ServiceCollection();
            services.AddDbContext<DAL.DbModel.DataContext>(options => options.UseSqlServer(connectionString));
            services.AddTransient<IWorkItemRepository, WorkItemRepository>();
            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetRequiredService<IWorkItemRepository>();
        }

        public static void DisplayData(IEnumerable<WorkItemData> data)
        {
            Console.WriteLine();
            foreach (var item in data) 
                Console.WriteLine(item);
            Console.WriteLine();
        }
    }
}
