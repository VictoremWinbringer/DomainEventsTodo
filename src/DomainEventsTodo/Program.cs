using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DomainEventsTodo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Cluster cluster = Cluster.Builder().AddContactPoint("127.0.0.1")
                .WithDefaultKeyspace("main")
                .Build();

            ISession session = cluster.ConnectAndCreateDefaultKeyspaceIfNotExists();

            session.Execute(
                "create keyspace if not exists domain_events_todo with replication ={'class':'SimpleStrategy','replication_factor':3};");

            session.Execute("use domain_events_todo");

            session.Execute(
                "create table if not exists todos(Id uuid primary key, Description text, IsComplete boolean );");

            var id = Guid.NewGuid();

            session.Execute(
                session
                .Prepare("insert into todos (id, description, iscomplete) values (?,?,?);")
                .Bind(id, "test", false));

            Row result = session.Execute(session.Prepare("select * from todos where id=?")
                .Bind(id)).First();
            Console.WriteLine("{0} {1} {2}", result["id"], result["description"], result["iscomplete"]);

            session.Execute(session
                .Prepare("update todos set iscomplete =? where id =?;")
                .Bind(true, id));

            result = session.Execute(session
                .Prepare("select * from todos where id =?;")
               .Bind(id)).First();
            Console.WriteLine("{0} {1} {2}", result["id"], result["description"], result["iscomplete"]);

            session.Execute(session
                .Prepare("delete from todos where id =?;")
                .Bind(id));

            RowSet rows = session.Execute("select * from todos;");
            foreach (Row row in rows)
                Console.WriteLine("{0} {1} {2}", row["id"], row["description"], row["iscomplete"]);

            Console.ReadLine();

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
