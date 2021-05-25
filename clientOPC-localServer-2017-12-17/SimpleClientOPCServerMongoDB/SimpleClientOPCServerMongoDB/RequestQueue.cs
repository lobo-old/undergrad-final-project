using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
using System.Net;

using OPC;
using OPCDA;
using OPCDA.NET;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;

namespace SimpleClientOPCServerMongoDB
{
    public class RequestQueue
    {
        private Queue reqQueue;
        private Mutex mtx;

        public RequestQueue()
        {
            reqQueue = new Queue();
            mtx = new Mutex();
        }

        public int Count()
        {
            return reqQueue.Count;//precisa de mutex aqui? 
        }

        public void Add(OpcRequest req)
        {
            mtx.WaitOne();
            reqQueue.Enqueue(req);
            mtx.ReleaseMutex();
        }

        public OpcRequest Remove()
        {
            mtx.WaitOne();
            OpcRequest req = (OpcRequest)reqQueue.Dequeue();
            mtx.ReleaseMutex();
            return req;
        }
    }
}
