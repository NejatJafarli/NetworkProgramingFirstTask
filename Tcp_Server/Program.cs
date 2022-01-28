
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;

class Program
{

    static void Main(string[] args)
    {
        var ip = IPAddress.Loopback;
        var port = 1111;
        var TcpServer = new TcpListener(ip, port);

        Console.WriteLine("Lisening ", ip);

        TcpServer.Start(100);
        List<TcpClient> clients = new List<TcpClient>();
        Task.Run(() =>
        {
            while (true)
            {
                var client = TcpServer.AcceptTcpClient();
                clients.Add(client);
                Console.WriteLine($"{client.Client.RemoteEndPoint} Client Connected");
            }
        });
        while (true)
        {
            while (true)
            {
                var ServerText = Console.ReadLine();
                if (ServerText == "Send")
                    break;
            }
            using var bitmap = new Bitmap(1920, 1080);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(0, 0, 0, 0,
                    bitmap.Size, CopyPixelOperation.SourceCopy);
            }

            bitmap.Save("SSA.jpg", ImageFormat.Jpeg);

            Image img = (Image)bitmap;

            byte[] bytes = (byte[])(new ImageConverter()).ConvertTo(img, typeof(byte[]));

            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Connected)
                {
                    BinaryWriter binaryWriter = new BinaryWriter(clients[i].GetStream());
                    binaryWriter.Write(bytes.ToArray());
                }
            }
            //convert byte array to image
            //Image myimg;
            //using (var ms = new MemoryStream(bytes))
            //{
            //    myimg=Image.FromStream(ms);
            //}


            //myimg.Save("Hello.jpg", ImageFormat.Jpeg);

        }

    }
}