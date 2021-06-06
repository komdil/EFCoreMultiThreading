using System;
using System.Collections.Generic;
using System.Text;

namespace EFCoreMultiThreading
{
    public class School
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }

    public class Student
    {
        public Guid Guid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public Guid SchoolGuid { get; set; }
        public virtual School School { get; set; }
        public virtual ICollection<Backpack> Backpacks { get; set; }
    }
    public class Backpack
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public Guid StudentGuid { get; set; }
        public virtual Student Student { get; set; }
    }
}
