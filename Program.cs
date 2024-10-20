using System.Net.Sockets;
using System.Net;

var ip = IPAddress.Parse("26.230.244.228");
var port = 27002;
var ep = new IPEndPoint(ip, port);
var listener = new TcpListener(ep);

listener.Start();
Console.WriteLine("Server ise basladi...");
while (true)
{
    var client = listener.AcceptTcpClient();
    _ = Task.Run(() =>
    {
        try
        {
            var networkStream = client.GetStream();
            var remoteEp = client.Client.RemoteEndPoint as IPEndPoint;
            var directoryPath = Path.Combine(Environment.CurrentDirectory, remoteEp!.Address.ToString());

            
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            
            var path = Path.Combine(directoryPath, $"{DateTime.Now:dd.MM.yyyy.HH.mm.ss}.png");

            
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                var buffer = new byte[1024];
                int len;
                while ((len = networkStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fs.Write(buffer, 0, len);
                }
            }
            Console.WriteLine("Fayl qebul olundu");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xeta: {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    });
}
