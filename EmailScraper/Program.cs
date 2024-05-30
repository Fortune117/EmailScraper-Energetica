using System;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;


namespace EmailScraper
{
    [Table(Name = "employeelist")]
    public class EmployeeList
    {
        [Column(Name = "eid")]
        public int EmployeeID { get; set; }
        
        [Column(Name = "firstName")]
        public string FirstName { get; set; }
        
        [Column(Name = "lastName")]
        public string LastName { get; set; }
        
        [Column(Name = "Email_id", IsPrimaryKey = true)]
        public string Email { get; set; }
        
        [Column(Name = "Email2")]
        public string Email2 { get; set; }
        
        [Column(Name = "Email3")]
        public string Email3 { get; set; }
        
        [Column(Name = "EMail4")]
        public string Email4 { get; set; }
        
        [Column(Name = "folder")]
        public string Folder { get; set; }
        
        [Column(Name = "status")]
        public string Status { get; set; }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var options = new DataOptions()
                .UseMySql(@"Server=localhost;Database=enron_dump;Uid=root;Pwd=root;");

// pass configured options to data context constructor
            var db = new DataContext(options);
            
            var employeeLists = db.GetTable<EmployeeList>();

            foreach (var employee in employeeLists)
            {
                Console.WriteLine(employee.FirstName);
            }
        }
    }
}
