using System;
using System.Linq;
using LinqToDB;
using LinqToDB.Mapping;


namespace EmailScraper
{
    [Table(Name = "employeelist")]
    public class Employee
    {
        [Column(Name = "eid", IsPrimaryKey = true)]
        public int EmployeeID { get; set; }
        
        [Column(Name = "firstName")]
        public string FirstName { get; set; }
        
        [Column(Name = "lastName")]
        public string LastName { get; set; }
        
        [Column(Name = "Email_id")]
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
    
    [Table(Name = "message")]
    public class Messages
    {
        [Column(Name = "mid", IsPrimaryKey = true)]
        public int MessageID { get; set; }
        
        [Column(Name = "sender")]
        public string Sender { get; set; }
        
        [Column(Name = "date")]
        public DateTime Date { get; set; }
        
        [Column(Name = "message_id")]
        public string InternalMessageID { get; set; }
        
        [Column(Name = "subject")]
        public string Subject { get; set; }
        
        [Column(Name = "body")]
        public string Body { get; set; }
        
        [Column(Name = "folder")]
        public string Folder { get; set; }
    }
    
    [Table(Name = "referenceinfo")]
    public class ReferenceInfo
    {
        [Column(Name = "rfid", IsPrimaryKey = true)]
        public int ReferenceID { get; set; }
        
        [Column(Name = "mid")]
        public string MessageID { get; set; }
        
        [Column(Name = "reference")]
        public string RecipientType { get; set; }
    }
    
    [Table(Name = "recipientinfo")]
    public class RecipientInfo
    {
        [Column(Name = "rid", IsPrimaryKey = true)]
        public int RecipientID { get; set; }
        
        [Column(Name = "mid")]
        public string MessageID { get; set; }
        
        [Column(Name = "rtype")]
        public string RecipientType { get; set; }
        
        [Column(Name = "rvalue")]
        public string RecipientValue { get; set; }
        
        [Column(Name = "dater")]
        public string Dater { get; set; }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var options = new DataOptions()
                .UseMySql(@"Server=localhost;Database=enron_dump;Uid=root;Pwd=root;");

// pass configured options to data context constructor
            var db = new DataContext(options);

            Console.WriteLine("Enter a keyword to search for:");
            var searchTerm = Console.ReadLine();
            
            var employeeLists = db.GetTable<Employee>();

            IQueryable<Employee> employeeQuery =
                from employee in employeeLists
                where employee.FirstName.Contains(searchTerm)
                select employee;

            foreach (var employee in employeeQuery)
            {
                Console.WriteLine(employee.FirstName);
            }
        }
    }
}
