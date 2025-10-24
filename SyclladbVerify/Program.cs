using Cassandra;
using System.Diagnostics;
using System.Threading.Channels;

var stopwatch = System.Diagnostics.Stopwatch.StartNew();
var cluster = Cluster.Builder()
           .AddContactPoints("node-0.gce-asia-south-1.a2229da0539390ea0805.clusters.scylla.cloud", "node-1.gce-asia-south-1.a2229da0539390ea0805.clusters.scylla.cloud", "node-2.gce-asia-south-1.a2229da0539390ea0805.clusters.scylla.cloud")

           .WithAuthProvider(new PlainTextAuthProvider("scylla", "Password"))
           .Build();

var session = cluster.Connect();

Console.WriteLine("✅ Connected to Scylla Cloud");

// Create keyspace and table
await session.ExecuteAsync(new SimpleStatement(@"
            CREATE KEYSPACE IF NOT EXISTS demo 
            WITH replication = {'class': 'NetworkTopologyStrategy', 'replication_factor': 3};
        "));

await session.ExecuteAsync(new SimpleStatement("USE demo"));

await session.ExecuteAsync(new SimpleStatement(@"
            CREATE TABLE IF NOT EXISTS users (
                user_id uuid PRIMARY KEY,
                name text,
                email text
            );
        "));

Console.WriteLine("✅ Table created");
var insertStmt = await session.PrepareAsync(
    "INSERT INTO users (user_id, name, email) VALUES (?, ?, ?)"
);

// warm-up
await session.ExecuteAsync(insertStmt.Bind(Guid.NewGuid(), "Warmup", "warmup@example.com"));

var sw = Stopwatch.StartNew();
Parallel.For(0, 100, async i =>
{
    await session.ExecuteAsync(insertStmt.Bind(Guid.NewGuid(), $"User{i}", $"user{i}@example.com"));
});
//for (int i = 0; i < 1000; i++)
//{
//    await session.ExecuteAsync(insertStmt.Bind(Guid.NewGuid(), $"User{i}", $"user{i}@example.com"));
//}
sw.Stop();
Console.WriteLine($"Inserted 1000 rows in {sw.Elapsed.TotalMilliseconds} ms");
Console.WriteLine($"Avg per write: {sw.Elapsed.TotalMilliseconds / 1000:F3} ms");
Console.WriteLine("✅ Inserted user");

// Query data
stopwatch.Restart();
var rs = await session.ExecuteAsync(new SimpleStatement("SELECT * FROM users"));
foreach (var row in rs)
{
    Console.WriteLine($"User: {row.GetValue<string>("name")} - {row.GetValue<string>("email")} -{row.GetValue<Guid>("user_id")}");
}
stopwatch.Stop();
Console.WriteLine($"Query took {stopwatch.ElapsedMilliseconds} ms");

var rowSet = session.Execute("SELECT * FROM mykeyspace.monkey_species");
Console.WriteLine(string.Join(Environment.NewLine, rowSet.Select(row => row.GetValue<string>("species"))));
await session.ShutdownAsync();
cluster.Shutdown();

Console.ReadLine();