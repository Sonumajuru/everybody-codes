using System.Net;
using System.Text;
class Program
{
    static HttpListener _httpListener = new HttpListener();
    static void Main(string[] args)
    {
        Console.WriteLine("Starting server...");
        _httpListener.Prefixes.Add("http://localhost:5000/");
        _httpListener.Start(); // start server
        Console.WriteLine("Server started.");
        Thread _responseThread = new Thread(ResponseThread);
        _responseThread.Start(); // start the response thread
    }

    static void ResponseThread()
    {
        while (true)
        {
            HttpListenerContext context = _httpListener.GetContext(); // get a context with request URL in context.Request.Url
            string postData = string.Empty;

            using (StreamReader reader = new StreamReader(context.Request.InputStream)) // get response stream
            {
                postData = reader.ReadToEnd(); // Save stream as string
            }

            byte[] _responseArray = Encoding.UTF8.GetBytes(postData); // Get stream bytes

            context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length); // write bytes to the output stream
            context.Response.KeepAlive = true;
            context.Response.Close();
            Console.WriteLine("\n" + postData);
            Console.WriteLine("Respone given to a request.");
        }
    }
}
