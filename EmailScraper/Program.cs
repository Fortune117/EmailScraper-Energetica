﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        
        [Association(ThisKey = nameof(Sender), OtherKey=nameof(EmailScraper.Employee.Email), CanBeNull = true)]
        public Employee Employee { get; set; }
        
        [Association(ThisKey = nameof(MessageID), OtherKey=nameof(RecipientInfo.MessageID))]
        public IEnumerable<RecipientInfo> Recipients { get; set; }
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
        public int MessageID { get; set; }
        
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

            var termsRegex = Regex.Matches(searchTerm, "\".+\"|\\b\\S+\\b");

            var terms = new List<string>();
            
            for (var i = 0; i < termsRegex.Count; i++)
            {
                var term = termsRegex[i];
                terms.Add(term.Value.Trim('\"')); //trim quotes - note this will remove an intentionally placed quotes in the keywords, not ideal but something to take care of later
            }
            
            //an immediate issue I notice is that this doesn't take into account word boundaries
            //SQL doesn't support regex as far as I'm aware, and I am unsure how to account for this issue.
            //You could filter the results again after having performed the query and populated the entity relations but
            //that seems messy.
            
            var messageQuery =
                from message in db.Messages
                where terms.All( x => message.Body.Contains(x)) //this is a very clumsy solution to this particular problem
                      || terms.All( x => message.Subject.Contains(x))
                      || terms.All( x => message.Sender.Contains(x))
                select message;

            //load associated employees along with messages
            messageQuery = messageQuery.LoadWith(messages => messages.Employee);
            messageQuery = messageQuery.LoadWith(messages => messages.Recipients); //this causes a significant performance drop

            Console.WriteLine($"---------------------------------");
            
            foreach (var message in messageQuery.Take(1))
            {
                //Log some info about the employee if they're not null
                if (message.Employee != null)
                {
                    Console.WriteLine($"Employee: {message.Employee.FirstName} {message.Employee.LastName}");
                }
                
                Console.WriteLine($"Sender: {message.Sender}");
                
                Console.Write("Recipients: ");
                foreach (var recipientInfo in message.Recipients)
                {
                    Console.Write($"{recipientInfo.RecipientValue}, ");
                }
                Console.Write("\n");
                
                Console.WriteLine($"Subject: {message.Subject}");
                Console.WriteLine($"\nBody:\n{message.Body}");
                
                Console.WriteLine($"---------------------------------");
            }
            
            //This seems to have a very low memory profile - it jumped up from about 16mbs to 28mbs during a test with the profiler attached, which seems ideal.
            //This does run a fairly expensive search on the SQL server though and thus presents another avenue for optimisations and improvements.
        }
    }
}
