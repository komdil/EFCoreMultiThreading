using System;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreMultiThreading
{
    class Program
    {
        static void Main(string[] args)
        {
            MainContext mainContext = new MainContext();
            CreateData(mainContext);
            var task1 = Task.Run(new Action(() => GetStudents(mainContext)));
            var task2 = Task.Run(new Action(() => GetBackpacks(mainContext)));
            Task.WaitAll(task1, task2);
        }

        static void GetStudents(MainContext mainContext)
        {
            Console.WriteLine("Loading Students started");
            var students = mainContext.GetEntities<Student>().Where(s => s.FirstName == "FirstName1").ToList();
            Console.WriteLine("Students loaded");
        }

        static void GetBackpacks(MainContext mainContext)
        {
            Console.WriteLine("Loading Backpacks started");
            var backpacks = mainContext.GetEntities<Backpack>().Where(s => s.Name == "Name1").ToList();
            Console.WriteLine("Backpacks loaded");
        }

        static void CreateData(MainContext mainContext)
        {
            if (!mainContext.Set<School>().Any())
            {
                for (int i = 0; i < 10; i++)
                {
                    var school = new School() { Guid = Guid.NewGuid(), Address = $"Scho olAddress{i}", Name = $"SchoolName{i}" };
                    mainContext.Add(school);
                    for (int j = 0; j < 1000; j++)
                    {
                        var student = new Student() { Guid = Guid.NewGuid(), LastName = $"LastName{j}", FirstName = $"FirstName{j}", DOB = DateTime.Today, School = school };
                        mainContext.Add(student);
                        for (int l = 0; l < 100; l++)
                        {
                            var backpack = new Backpack() { Guid = Guid.NewGuid(), Name = $"Name{i}", Student = student, Cost = l };
                            mainContext.Add(backpack);
                        }
                    }
                }
                mainContext.SaveChanges();
            }
        }
    }
}
