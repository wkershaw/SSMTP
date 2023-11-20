using System.Net.Mail;

var smtpClient = new SmtpClient("127.0.0.1")
{
    Port = 2025,
    EnableSsl = false
};
    
Console.WriteLine("Sending...");

smtpClient.Send("from@test.com", "recipient@test.com", "subject", "body");

Console.WriteLine("Sent");