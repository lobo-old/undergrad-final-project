using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;

namespace SimpleClientOPCServerMongoDB
{
    public enum Command //ADICIONAR NOVOS COMANDOS 
    {
        Read,
        Write,
        onlineSrvs,
        devicesAvailable,
        servMongoClientOPC,
        periodicRead,

    }

    public class OpcRequest
    {
        public Command Cmd;          // command type
        public BsonDocument requestDocument; //Bson document for requests

        public OpcRequest(Command c, BsonDocument reqDocument)//passe o documento Bson inteiro
        {
            Cmd = c;
            requestDocument = reqDocument;
        }
    }
}
