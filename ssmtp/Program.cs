using Ssmtp.Smtp;

string address = "127.0.0.1";
int port = 2025;

var server = new SsmtpServer(address, port);
server.Start(CancellationToken.None);

Console.WriteLine("Hit enter to continue...");
Console.Read();