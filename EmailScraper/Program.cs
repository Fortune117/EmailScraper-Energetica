using System;
using System.Linq;
using LinqToDB;
using LinqToDB.Mapping;


namespace EmailScraper
{
    #region Containers
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
        
        [Association(ThisKey = nameof(Sender), OtherKey=nameof(EmailScraper.Employee.Email))]
        public Employee Employee { get; set; }
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
        
        [Association(ThisKey = nameof(MessageID), OtherKey=nameof(Messages.MessageID))]
        public Messages Message { get; set; }
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
        
        [Association(ThisKey = nameof(MessageID), OtherKey=nameof(Messages.MessageID))]
        public Messages Message { get; set; }
    }
    #endregion

    public class EnronData : LinqToDB.Data.DataConnection
    {
        public ITable<Employee> EmployeeList => this.GetTable<Employee>();
        public ITable<Messages> Messages => this.GetTable<Messages>();
        public ITable<ReferenceInfo> ReferenceInfo => this.GetTable<ReferenceInfo>();
        public ITable<RecipientInfo> RecipientInfo => this.GetTable<RecipientInfo>();
        
        public EnronData(DataOptions options) : base(options) { }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            //  We establish a connection to our MySQL database which houses the imported MySQL dump.
            //  in my use case, this is hosted locally on my machine.
            //
            //  In a real world use case, this login/connection info would not be stored in plaintext like it is here,
            //  and would represent a security risk if done so.
            
            var options = new DataOptions()
                .UseMySql(@"Server=localhost;Database=enron_dump;Uid=root;Pwd=root;");
            var db = new EnronData(options);

            Console.WriteLine("Enter keywords to search for:");
            var searchTerm = Console.ReadLine();

            if (searchTerm is null)
                return;

            var terms = searchTerm.Split(' ');
            
            //var employeeLists = db.GetTable<Employee>();

            var messageQuery =
                from message in db.Messages
                where terms.All( x => message.Body.Contains(x))
                select message;

            foreach (var message in messageQuery)
            {
                Console.Write(message.Body);
            }
        }
    }
}
