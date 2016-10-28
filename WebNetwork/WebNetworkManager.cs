using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Assets.Plugins.SapphireGem.WebServer
{
    public class WebNetworkManager : MonoBehaviour
    {
        HttpListener myHttpListener = new HttpListener();
        bool receivedData = false;
        static string value;

        Dictionary<String, Action<HttpListenerRequest, HttpListenerResponse>> requestToListener = new Dictionary<String, Action<HttpListenerRequest, HttpListenerResponse>>();

        string prefix = "http://localhost:8080/";


        void Start()
        {
            SetupServer();

            AddHandler("session", HandleSession);
            AddHandler("status", HandleStatus);
        }

        public void AddHandler(String urlAddress, Action<HttpListenerRequest, HttpListenerResponse> action)
        {
            urlAddress = urlAddress.ToLower();
            requestToListener.Add(prefix + urlAddress, action);
        }

        private void HandleSession(HttpListenerRequest request, HttpListenerResponse response)
        {
            // Construct a response.
            string responseString = "<HTML><BODY> Hello session</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close(); 
        }

        private void HandleStatus(HttpListenerRequest request, HttpListenerResponse response)
        {
            // Construct a response.
            string responseString = "<HTML><BODY> Hello status!</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close(); 
        }

        // Create a server and listen on a port
        public void SetupServer()
        {            
            myHttpListener.Prefixes.Add( prefix );

            myHttpListener.Start();
            Thread webThread = new Thread(StartServer);
            webThread.Start();            
        }

        private void StartServer()
        {
            while (myHttpListener.IsListening)
            {
                ProcessRequest();
            }
        }

        private void ProcessRequest()
        {
            IAsyncResult result = myHttpListener.BeginGetContext(new AsyncCallback(ListenerCallback), myHttpListener);
            result.AsyncWaitHandle.WaitOne();
        }

        private void ListenerCallback(IAsyncResult result)
        {
            value = result.ToString();
            receivedData = true;

            HttpListener listener = (HttpListener)result.AsyncState;
            // Call EndGetContext to complete the asynchronous operation.
            HttpListenerContext context = listener.EndGetContext(result);

            requestToListener[context.Request.Url.ToString()](context.Request, context.Response);
        }

        void Update()
        {
            if (receivedData)
            {
                receivedData = false;
                Debug.LogError(value);
            }
        }

    }
}
