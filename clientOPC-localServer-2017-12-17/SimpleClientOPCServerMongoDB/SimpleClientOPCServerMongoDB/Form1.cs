using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Net;
using System.IO;

using OPC;
using OPCDA;
using OPCDA.NET;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;

namespace SimpleClientOPCServerMongoDB
{
    public partial class FormConnection : Form
    {
        #region Members
        private String serverName;
        private String clientMongoURL;
        private String clientDataBaseString;
        
        private OpcServer ServersOPC;
        BrowseTree ItemTree;
        int timesReconnected;

        String pathStatusLog;
        String pathRequestsLog;
        String pathRequestsLogCSV;
            List<String> ListRequestsLogCsv;

        //TRATAMENTO DAS REQUESTS
        RequestQueue Requests;//Fila de requests
        SyncIOGroup SioGrp;
        private Thread thrOPCAccess;//Thread para responder as requests
        private bool OPCThreadRunning;
        private int sleepTimeOPCThr;
        int readRequestsN;
        int writeRequestsN;
        //talvez colocar variável para armazenar o tamanho da fila
        
        //POLING E RESPOSTAS AO DB
        RequestQueue Responses;//Fila de respostas
            //Thread para executar o polling no DB(pedido para colocar a request na fila 'Requests')
            private Thread thrMongoPolling;
            private bool mongoPollingRunning;// Ira dizer ao laço while para manter o polling no DB para buscar requests
            private int sleepTimePollingThr;
            //int requestCounter;

            //Thread para enviar respostas ao DB(retira da fila Responses )
            private Thread thrMongoResponse;
            private bool mongoRefreshRunning;// Ira dizer ao laço while para manter atualizar o DB(respostas)
            private int sleepTimeResponseThr;
            //int responsedRequests;

        //CRIAÇÃO PERIÓDICA DAS REQUESTS
        private List<String> ListServerDevicesID;
        private Thread thrPeriodicRead;//leitura periodica
        private bool periodicReadThreadRunning;
        private int sleepTimePeriodicThr;
        //public OpcServer periodicReadOpcServer;
        //talvez colocar variável para armazenar quantas amostras enviadas(n de dispositivos tmb) tamanho da fila
        
        //---------------Delegates----------
        private delegate void statusLogHelper(String str);//para atualizar logs
        private delegate void refreshNrequestsHelper(bool addRead, bool addWrite);//para incrementar qnt de req
        private delegate void refreshQueueLengthHelper(int queueLength);
        private delegate void buttonDelegateHelper(object sender, EventArgs e);
        private delegate void voidDelegateHelper();
        private delegate List<String> refreshListDelegateHelper();

        #endregion

        #region Contructor
        public FormConnection()
        {
            InitializeComponent();
            //inicializar todos os membros
            
            //----------------inicializar todos os membros------------

            //Tempos de sleep das threads
            sleepTimeOPCThr = 10; sleepTimeResponseThr = 10; sleepTimePollingThr = 500; sleepTimePeriodicThr = Convert.ToInt32(numericUpDownSampleT.Value);
            timesReconnected = 0;
            //nome do server
            serverName = "Advosol.DA3CBCS.1";//pode come'car como default, mas come'car string vazia
            
            //serverName = "Smar.hseoleserver.0";//nome do server
            clientMongoURL = "mongodb://andremonografia:Andreloboelt06@ds121622.mlab.com:21622/andre-monografia-db";
            clientDataBaseString = "andre-monografia-db";


            //clientMongoURL = "mongodb://127.0.0.1:3001/";
            //clientDataBaseString = "meteor";
            
            
            
            readRequestsN = 0; writeRequestsN = 0;
            this.createLogFiles();
            /*serverName = _serverName;
            clientMongoURL = _clientMongoURL;
            clientDataBaseString = _clientDataBaseString;
            this.createConn();
            */
            //Para o caso do FormMain criar a conexao passando o nome do servidor, chmar método que inicializa
            //TODOS os membros da classe(feito no método createConn)
            //objetos passados ao contrutor: serverName, clientMongoURL, clientDataBaseString

                       

        }
        #endregion

        //Tratamento dos eventos dos controles
        #region eventHandles
        
        //browse servers button
        private void btnBrowseServers_Click(object sender, EventArgs e)
        {
            cbBrowseServers.Items.Clear();
            appendToLog(DateTime.Now.ToString() + ": Browsing servers...\n");
            this.Update();
            OpcServerBrowser SrvList = new OpcServerBrowser();
            string[] ServersNames = null;
            SrvList.GetServerList(true, true, out ServersNames);
            if (ServersNames != null)
            {
                cbBrowseServers.Items.AddRange(ServersNames);
                cbBrowseServers.SelectedIndex = 0;
                btnCreateConn.Enabled = true;
            }
            else
            {
                btnCreateConn.Enabled = false;
            }
            appendToLog(DateTime.Now.ToString() + ": COMPLETED\n");
            this.Update();
        }

        //Create conn
        private void btnCreateConn_Click(object sender, EventArgs e)
        {
            //if (cbBrowseServers.SelectedItem != null)
            //{
            if (cbBrowseServers.Text != null)
            {
                //String serverN = cbBrowseServers.SelectedItem.ToString();//mudar depois
                String serverN = cbBrowseServers.Text;//
                
                serverName = serverN;//definindo o nome do server
                cbBrowseServers.Enabled = false;
                btnBrowseServers.Enabled = false;
                this.createConn();
            }
            else 
            {
                MessageBox.Show("Select a server!!");
                cbBrowseServers.Focus();
            }
            //appendToLog("string FROM main");
            //listBoxStaatusLog.Items.Add("create conn");
        }


        //desconectar servidor e fechar
        private void btnDisconnectServer_Click(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new buttonDelegateHelper(btnDisconnectServer_Click),new object[]{sender,e});
                return;
            }
            appendToLog(DateTime.Now.ToString() + ": Disconnecting from server...");
            this.Close();//Pedir para fechar o Form lança evento onde é chamado o metodo para desconectar o server corretamente
        }

        //Form sendo finalizado
        void Form1_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                //Encerrar todas as threads
                //Isso está somente parando, os objetos thread sao reonctruidos e portanto a memoria nao é liberada(acho)
                this.disconnectServer();
                MessageBox.Show("Server " + serverName + "Disconnected");
                
                // Prompt user to save his data
                appendToLog(DateTime.Now.ToString() + ": Application closed by the user");
                appendToLog(DateTime.Now.ToString() + ": Saving data...");
                
                exportStatusLog();
                MessageBox.Show("StatusLog saved in: " + pathStatusLog);
                
                exportRequestsLog();
                MessageBox.Show("RequestsLog saved as .txt in: " + pathRequestsLog);

                exportRequestsLogCsv();
                MessageBox.Show("RequestsLog saved as .csv in: " + pathRequestsLogCSV);
            }
            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                // Autosave and clear up ressources
                //-------------Save Here----------

                //Encerrar todas as threads
                //Isso está somente parando, os objetos thread sao reonctruidos e portanto a memoria nao é liberada(acho)
                this.disconnectServer();
                MessageBox.Show("Server " + serverName + "Disconnected");

                //Encerrar todas as threads
                appendToLog(DateTime.Now.ToString() + ": Application closed by OS");
                appendToLog(DateTime.Now.ToString() + ": Saving data...");

                exportStatusLog();
                MessageBox.Show("StatusLog saved in: " + pathStatusLog);

                exportRequestsLog();
                MessageBox.Show("RequestsLog saved as .txt in: " + pathRequestsLog);

                exportRequestsLogCsv();
                MessageBox.Show("RequestsLog saved as .csv in: " + pathRequestsLogCSV);
            }
        }

        private void enableControlerOnConnection()
        {
            if(InvokeRequired)
            {
                BeginInvoke(new voidDelegateHelper(enableControlerOnConnection));
                return;
            }
            btnAddToMongo.Enabled = true;
            btnRemoveFromMongo.Enabled = true;
            btnSetAvailableD.Enabled = true;
            btnEditDevices.Enabled = true;

            btnStartThreadOPC.Enabled = true;
            btnStartThreadPeriodic.Enabled = true;
            btnStartThreadPoliing.Enabled = true;
            btnStartThreadResponses.Enabled = true;
            //button1.Enabled = true;
            //button2.Enabled = true;

            btnCreateConn.Enabled = false;
            
        }

        #region ThreadButtons
        //Start buttons
        private void btnStartThreadResponses_Click(object sender, EventArgs e)
        {
            //colocar mudanças nos controles
            btnStartThreadResponses.Enabled = false;
            btnStopThreadResponses.Enabled = true;

            mongoRefreshRunning = true;
            thrMongoResponse.Start();
            appendToLog(DateTime.Now.ToString() + ": Started thread: " + thrMongoResponse.Name);
        }

        private void btnStartThreadOPC_Click(object sender, EventArgs e)
        {
            //colocar mudanças nos controles
            btnStartThreadOPC.Enabled = false;
            btnStopThreadOPC.Enabled = true;

            OPCThreadRunning = true;
            thrOPCAccess.Start();
            appendToLog(DateTime.Now.ToString() + ": Started thread: " + thrOPCAccess.Name);
        }

        private void btnStartThreadPeriodic_Click(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                //MessageBox.Show("InvokeRequired");
                BeginInvoke(new buttonDelegateHelper(btnStartThreadPeriodic_Click), new object[] { sender, e });
                return;
            }
            //MessageBox.Show("NOt required");
            //colocar mudanças nos controles
            btnStartThreadPeriodic.Enabled = false;
            btnStopThreadPeriodic.Enabled = true;
            //sleepTimePeriodicThr = Convert.ToInt32(tbSampleT.Text);//atualizar valor do tempo de amostragem
            sleepTimePeriodicThr = Convert.ToInt32(numericUpDownSampleT.Value);//atualizar valor do tempo de amostragem
            numericUpDownSampleT.Enabled = false;
            //parte do device manager
            btnAddToMongo.Enabled = false;
            btnRemoveFromMongo.Enabled = false;
            btnSetAvailableD.Enabled = false;
            btnEditDevices.Enabled = false;

            periodicReadThreadRunning = true;
            thrPeriodicRead.Start();
            appendToLog(DateTime.Now.ToString() + ": Started thread: " + thrPeriodicRead.Name);
            
        }
        
        private void btnStartThreadPoliing_Click(object sender, EventArgs e)
        {
            btnStartThreadPoliing.Enabled = false;
            btnStopThreadPoliing.Enabled = true;

            mongoPollingRunning = true;
            thrMongoPolling.Start();
            appendToLog(DateTime.Now.ToString() + ": Started thread: " + thrMongoPolling.Name);
        }

        //Stop buttons
        private void btnStopThreadResponses_Click(object sender, EventArgs e)
        {
            //colocar mudanças nos controles
            btnStartThreadResponses.Enabled = true;
            btnStopThreadResponses.Enabled = false;

            this.StopRefreshMongoDB();
            appendToLog(DateTime.Now.ToString() + ": Stopped thread: " + thrMongoResponse.Name);
        }

        private void btnStopThreadOPC_Click(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                //MessageBox.Show("InvokeRequired");
                BeginInvoke(new buttonDelegateHelper(btnStopThreadOPC_Click), new object[] { sender, e });
                return;
            }
            btnStartThreadOPC.Enabled = true;
            btnStopThreadOPC.Enabled = false;

            this.StopOpcAccessThread();
            appendToLog(DateTime.Now.ToString() + ": Stopped thread: " + thrOPCAccess.Name);
        }

        private void btnStopThreadPeriodic_Click(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                //MessageBox.Show("InvokeRequired");
                BeginInvoke(new buttonDelegateHelper(btnStopThreadPeriodic_Click), new object[] { sender, e });
                return;
            }
            btnStartThreadPeriodic.Enabled = true;
            btnStopThreadPeriodic.Enabled = false;
            numericUpDownSampleT.Enabled = true;//numeric up down do tmepo de amostragem
            //parte do device manager
            btnAddToMongo.Enabled = true;
            btnRemoveFromMongo.Enabled = true;
            btnSetAvailableD.Enabled = true;
            btnEditDevices.Enabled = true;

            this.StopPeriodicReadAccessThread();
            appendToLog(DateTime.Now.ToString() + ": Stopped thread: " + thrPeriodicRead.Name);
        }

        private void btnStopThreadPoliing_Click(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                //MessageBox.Show("InvokeRequired");
                BeginInvoke(new buttonDelegateHelper(btnStopThreadPoliing_Click), new object[] { sender, e });
                return;
            }
            btnStartThreadPoliing.Enabled = true;
            btnStopThreadPoliing.Enabled = false;

            this.StopPollingMongoDB();
            appendToLog(DateTime.Now.ToString() + ": Stopped thread: " + thrMongoPolling.Name);
        }
#endregion

        #region treeViewAndCboxListDevices

        private void treeViewDevices_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //cbSendMongo.SelectedText = "";
            //cbSendMongo.Text = "aaaa";
            if (treeViewDevices.SelectedNode != null)
            {
                //Console.WriteLine(treeViewDevices.SelectedNode.Tag.ToString());
                cbSendMongo.Text = treeViewDevices.SelectedNode.Tag.ToString();
            }
        }

        private void btnAddToMongo_Click(object sender, EventArgs e)
        {
            //if (treeViewDevices.SelectedNode.Tag != null)
            if (cbSendMongo.Text != null)
            {
                String str = cbSendMongo.Text;
                //String str = treeViewDevices.SelectedNode.Tag.ToString();
                if (!cbSendMongo.Items.Contains(str) && !ListServerDevicesID.Contains(str))
                {
                    cbSendMongo.Items.Add(str);
                    ListServerDevicesID.Add(str);
                }
                else
                {
                    MessageBox.Show("Device ID exists!!");
                }
            }
            else
            {
                MessageBox.Show("Select a device ID!!");
            }
        }

        private void btnRemoveFromMongo_Click(object sender, EventArgs e)
        {
            if (cbSendMongo.SelectedItem != null)
            {
                String str = cbSendMongo.SelectedItem.ToString();
                if (cbSendMongo.Items.Contains(str) && ListServerDevicesID.Contains(str))
                {
                    cbSendMongo.Items.Remove(str);
                    ListServerDevicesID.Remove(str);
                    cbSendMongo.SelectedText = "";
                }
                else
                {
                    MessageBox.Show("Device ID already removed!!");
                }
            }
            else
            {
                MessageBox.Show("Select a device ID!!");
            }
        }

        private void btnSetAvailableD_Click(object sender, EventArgs e)//Principal função é atualizar no DB os devices(chama método)
        {
            if (ListServerDevicesID.Count > 0)
            {
                //Faz conexao com DB só para atualizar a lista de dispositivos
                MongoClient client = new MongoClient(clientMongoURL);
                IMongoDatabase database = client.GetDatabase(clientDataBaseString); //USAR FUTURAMENTE clientDataBaseString;

                var collectionDevicesAvailable = database.GetCollection<BsonDocument>("devicesAvailable");

                var filterServerName = Builders<BsonDocument>.Filter.Eq("serverName", serverName);
                //deletando todos atuais
                collectionDevicesAvailable.DeleteMany(filterServerName);
                appendToLog(DateTime.Now.ToString() + ": devicesID deleted from server");
                appendToLog(DateTime.Now.ToString() + ": adding in DB the defined devicesID in list");
                foreach (String strID in ListServerDevicesID)
                {
                    //para cada deviceID, atualizar/criar um documento na collection devicesAvailable
                    //Console.WriteLine(strID);
                    Dictionary<String, Object> newDicDevice = new Dictionary<String, Object>();

                    newDicDevice.Add("serverName", serverName);
                    newDicDevice.Add("deviceID", strID);

                    BsonDocument newDocDevice = new BsonDocument(newDicDevice);
                    collectionDevicesAvailable.InsertOne(newDocDevice);
                    appendToLog("      " + strID + " added");

                }
                cbSendMongo.Enabled = false;
                btnRemoveFromMongo.Enabled = false;
                btnSetAvailableD.Enabled = false;
                btnAddToMongo.Enabled = false;
                //btnEditDevices.Enabled = true;
            }
            else
            {
                MessageBox.Show("The list is empty!!");
            }
        }

        private void btnEditDevices_Click_1(object sender, EventArgs e)
        {
            cbSendMongo.Enabled = true;
            btnRemoveFromMongo.Enabled = true;
            btnSetAvailableD.Enabled = true;
            btnAddToMongo.Enabled = true;
            //btnEditDevices.Enabled = false;
        }
        #endregion

        #endregion
        
        #region EventsTest
        //test
        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<String, Object> newDict = new Dictionary<String, object>();
            newDict.Add("requestT", "SimulatedData.Step");//adicionando o ID do dispositivo
            newDict.Add("serverName", serverName);
            newDict.Add("createdAt", DateTime.Now);//Verificar se tem fuso
            newDict.Add("responseTime", 0);
            newDict.Add("checked", false);
            BsonDocument newDoc = new BsonDocument(newDict);
            Responses.Add(new OpcRequest(Command.Read, newDoc));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dictionary<String, Object> newDict = new Dictionary<String, object>();
            newDict.Add("requestT", "SimulatedData.Step");//adicionando o ID do dispositivo
            newDict.Add("serverName", serverName);
            newDict.Add("createdAt", DateTime.Now);//Verificar se tem fuso
            newDict.Add("responseTime", 0);
            newDict.Add("checked", false);
            BsonDocument newDoc = new BsonDocument(newDict);
            Requests.Add(new OpcRequest(Command.Read, newDoc));
        }        

        #endregion

        #region Connection
        // Create Conn
        public void createConn() //colocar como argumento a string do Server ?????????
        {
            //appendToLog("CONNECTION");
            //----------------------- VERIFICAR RACE CONDITION ------------------
            String serverN = serverName; //Em tese, as threads ainda não foram lancadas então na há problema
            //----------------------- VERIFICAR RACE CONDITION ------------------
            try
            {
                appendToLog(DateTime.Now.ToString() + ": Connecting to " + serverN);
                ServersOPC = new OpcServer();
                int rtc = ServersOPC.Connect(serverN);//conectando
                if (HRESULTS.Failed(rtc))//conferindo
                {
                    appendToLog(DateTime.Now.ToString() + ": Error in connection with " + serverN + ", Error n: " + rtc);
                    this.serverIsOffline();
                    throw new OPCException(rtc, "Connecttion failed with error {hr}");
                }
                else
                {
                    this.serverIsOnline();
                    appendToLog(DateTime.Now.ToString() + ": Connected to " + serverN);
                    this.createTree();//criando arvore de dispositivos
                    //subst FormConn
                        //subst OpcThread
                        SioGrp = ServersOPC.AddSyncIOGroup();
                        Requests = new RequestQueue();//fila para chegadas
                        thrOPCAccess = new Thread(new ThreadStart(OpcAccessThread));//thread que trata as requests
                        thrOPCAccess.Name = "OpcAccessThread";
                        OPCThreadRunning = false;
                        //contadores de requests
                        readRequestsN = 0; writeRequestsN = 0;
                        //subst ServerMongoDB
                        //métodos online e offline, refreshDeviceList
                        Responses = new RequestQueue();//fila para as saídas
                        //thread polling
                        thrMongoPolling = new Thread(new ThreadStart(PollingMongoDB_start));
                        thrMongoPolling.Name = "PollingThread";
                        mongoPollingRunning = false;
                        //thread resposta
                        thrMongoResponse = new Thread(new ThreadStart(RefreshMongoDB_start));
                        thrMongoResponse.Name = "ResponseThread";
                        mongoRefreshRunning = false;
                        //thread de criacao periodica
                        //deviceID = "SimulatedData.Step";
                        ListServerDevicesID = new List<String>();
                        appendToLog(DateTime.Now.ToString() + ": refreshing devicesID from server " + serverN);
                        ListServerDevicesID = refreshDevicesList();
                        appendToLog(DateTime.Now.ToString() + ": DevicesID in server " + serverN + ":");
                        foreach (String str in ListServerDevicesID) { appendToLog("      " + str); }
                        

                        //usar o refresh deviceID aqui
                        thrPeriodicRead = new Thread(new ThreadStart(PeriodicReadAccessThread));
                        thrPeriodicRead.Name = "PeriodicReadThread";
                        periodicReadThreadRunning = false;

                        //Ativar controles na conexao
                        this.enableControlerOnConnection();

                        //se for comecar as threads na conexao
                        //this.startAllThreads();//lembrar de retirar do meteodo de restart para nao comecar duas vezes
                        
                }


            }
            catch (Exception ex)
            {
                appendToLog(DateTime.Now.ToString() + ": " + ex.Message);
                ServersOPC = null;
                //apontar todas as referências de todos objetos para null
                Console.WriteLine(ex.StackTrace);
                //Exportando log
                appendToLog(DateTime.Now.ToString() + ": " + ex.Message);
                exportStatusLog();
                
            }
        
        }

        //metodo para reiniciar a conexao
        private void restartConn() 
        {
            if (InvokeRequired) 
            {
                BeginInvoke(new voidDelegateHelper(restartConn));
                return;
            }
            appendToLog(DateTime.Now.ToString() + ": Saving data...");
            exportStatusLog();//NAO ESQUECER DA EXCEPTION
            exportRequestsLog();
            disconnectServer();
            createLogFiles();
            createConn();
            startAllThreads();
            
        
        }

        //me'todo para começar as threads
        private void startAllThreads() 
        {
            btnStartThreadResponses_Click(null, null);//o sender e o args não sao usados, feito assim para nao repetir codigo
            btnStartThreadPoliing_Click(null, null);
            btnStartThreadOPC_Click(null,null);
            //btnStopThreadPeriodic_Click(null, null);
        }

        //desconecta do servidor e fecha o form
        private void disconnectServer()
        {
            //parar threads e fechar form
            this.StopPollingMongoDB();
            this.StopPeriodicReadAccessThread();
            this.StopRefreshMongoDB();
            this.StopOpcAccessThread();//ordem?????
            //notificar que server está off
            this.serverIsOffline();
        }
#endregion
        //--------------------THREADS---------------------------
        //Thread que trata as requests
        #region ThrRequests
        private void OpcAccessThread() 
        {
           //try
            //{
                while (OPCThreadRunning)
                {
                    //Começar somente com a de leitura
                    if (Requests.Count() > 0)
                    {
                        //COLOCAR NO LOG ????????????????????
                        appendToLog(DateTime.Now.ToString() + ": Number of Requests in queue: " + Requests.Count());
                        //atualizar tamanho da fila de requests
                        OpcRequest req = Requests.Remove();
                        queueReqLengthRefresh(Requests.Count());

                        OpcRequest respReq;
                        // request do tipo leitura
                        if (req.Cmd == Command.Read)
                        {
                            //FUNCAO DE PREPARO DA RESPOSTA EN BsonDocument
                            //respReq = ReadResponseDocumentPreparation(req);//voltar depois

                            //---------------------TESTE------------------------------//---------------------TESTE------------------------------
                            //OpcRequest resp = respDoc;
                            OpcRequest resp = req;
                            String reqText = resp.requestDocument.GetValue("requestT").ToString();
                            OPCItemState val = new OPCItemState();
                            string s = "";
                            if (SioGrp == null)
                            {
                                String status = "Server NULL Reference, SioGrp NULL Reference";
                                SERVERSTATUS stat;
                                if (ServersOPC != null)
                                {
                                    ServersOPC.GetStatus(out stat);
                                    status = "Server status: " + stat.eServerState.ToString() + ", SioGrp NULL Reference";
                                }   //Atualizando arquivo
                                //TEM QUE TERMINAR
                                resp.requestDocument.Set("checked", true);
                                resp.requestDocument.Set("responseValue", status);
                                appendToLog(DateTime.Now.ToString() + ": referencia SioGrp ou ServerOPC: " + status);
                                return ;
                            }

                            int rtc = 0;
                            try
                            {
                                ItemDef item = SioGrp.Item(reqText);
                                
                                if (item == null)
                                    SioGrp.Add(reqText);
                                item = SioGrp.Item(reqText);
                                //MessageBox.Show(item.OpcIDef.RequestedDataType.ToString());
                                //MessageBox.Show(item.OpcIDef.RequestedDa);
                                rtc = SioGrp.Read(OPCDATASOURCE.OPC_DS_CACHE, item, out val);
                                    //rtc = SioGrp.Read(OPCDATASOURCE.OPC_DS_CACHE, reqText, out val);
                                //throw new System.NullReferenceException();//??????
                            }
                            catch (Exception ex)
                            {
                                s = "Read failed: " + ex.Message;
                                appendToLog(DateTime.Now.ToString() + ": Error Message: " + ex.Message);
                                appendToLog(DateTime.Now.ToString() + ": Error StackTrace: " + ex.StackTrace + "\n");
                                //Atualizando arquivo
                                //TEM QUE TERMINAR
                                resp.requestDocument.Set("checked", true);
                                resp.requestDocument.Set("responseValue", s);
                                //Tentar reiniciar a conexao
                                appendToLog(DateTime.Now.ToString() + ": Restarting connection...\n");
                                Responses.Add(resp); //MANDAR DOC com falha
                                //chamar método
                                if (timesReconnected < 3)
                                {
                                    timesReconnected++;
                                    restartConn();
                                }
                                else 
                                {
                                    appendToLog(DateTime.Now.ToString() + ": times Reconnected: " + timesReconnected);
                                    btnDisconnectServer_Click(null, null);//fechar todo o programa
                                    //para thread polling e opc, deixar o programa terminar de enviar as respostas ao DB
                                    //btnStopThreadPeriodic_Click(null,null);
                                    //btnStopThreadPoliing_Click(null,null);
                                    //btnStopThreadOPC_Click(null, null);
                                    
                                    return;
                                }
                                return;
                            }
                            //try
                            //{
                                if (HRESULTS.Failed(rtc))
                                {
                                    s = "Read failed with error 0x" + rtc.ToString("X");
                                    //Atualizando arquivo
                                    //TEM QUE TERMINAR
                                    resp.requestDocument.Set("checked", true);
                                    resp.requestDocument.Set("responseValue", s);
                                    Responses.Add(resp);
                                    //return resp;
                                }
                                else if (HRESULTS.Failed(val.Error))//ESTE TRATAMENTO NÃO ESTA FUNCIONANDO, VERIFICAR TABELAS DE ERROS
                                {
                                    s = "Read failed with error 0x" + val.Error.ToString("X");
                                    //Atualizando arquivo
                                    resp.requestDocument.Set("checked", true);
                                    resp.requestDocument.Set("responseValue", s);
                                    //TEM QUE TERMINAR
                                    Responses.Add(resp); 
                                }
                                else
                                {
                                    s = String.Empty;
                                    if (val.DataValue != null)
                                    {
                                        s = val.DataValue.ToString();
                                       
                                        Console.WriteLine("Data value: " + s);
                                    }
                                    else
                                    {
                                        s = "NULL";
                                    }
                                    String elementID;
                                    BsonValue Bvalue;
                                    if (resp.requestDocument.TryGetValue("_id", out Bvalue))
                                    {
                                        //Console.WriteLine("TEM ID");
                                        elementID = Bvalue.ToString();
                                    }
                                    else
                                    {
                                        //Console.WriteLine(" NAO TEM ID");
                                        elementID = "NULL";
                                    }
                                    DateTime datenowA = DateTime.Now.ToUniversalTime();
                                    //DateTime datenowA = DateTime.Now.ToUniversalTime();//testar
                                    
                                    DateTime createdAt = resp.requestDocument.GetValue("createdAt").ToUniversalTime();
                                    //createdAt = createdAt.AddHours(-3);//para o caso em que
                                    //createdAt = createdAt.AddHours(-5);//para o caso em que
                                    //datenowA = val.TimeStampNet;
                                    //DateTime createdAt = resp.requestDocument.GetValue("createdAt");
                                    //appendToLog(datenowA.ToString() + ": _id: " + elementID + ", CreatedAt: " + resp.requestDocument.GetValue("createdAt").ToString() + ", Value: " + s);
                                    appendToLog(DateTime.Now.ToString() + ": Read Request, DeviceID: " + reqText + ", Value: " + s);//CORRETO
                                    resp.requestDocument.Add("checkedAt", datenowA);
                                    resp.requestDocument.Set("responseTime", (datenowA - createdAt).TotalSeconds);
                                    resp.requestDocument.Set("checked", true);
                                    resp.requestDocument.Set("responseValue", s);//CORRETO

                                    //Salvando Request no listbox(separação de dados por espaço)
                                    appendToRequestsLog(reqText + " " + s + " " + datenowA);
                                    
                                    //Salvando Request na lista que sera exportada para arquivos csv(separaçao por vírgulas)
                                    //adicionar a lista(possui demarcadores de tipos float com ponto(.))
                                    ListRequestsLogCsv.Add(reqText + "," + s.Replace(",", ".") + "," + datenowA + "\r\n"); 
                                    
                                    //pedir para o form incrementar Read request counter
                                    requestsRefresh(true, false);
                                    Responses.Add(resp); 
                                }
                            /*}
                            catch (Exception ex)
                            {
                                s = "Read failed: " + ex.Message;
                                appendToLog(DateTime.Now.ToString() + ": Error Message: " + ex.Message);
                                appendToLog(DateTime.Now.ToString() + ": Error StackTrace: " + ex.StackTrace);
                                //Atualizando arquivo
                                //TEM QUE TERMINAR
                                resp.requestDocument.Set("checked", true);
                                resp.requestDocument.Set("responseValue", s);
                                throw ex;
                                //return resp;
                            }*/

                            //---------------------TESTE------------------------------


                            //inserir na fila de resposta
                            //Responses.Add(respReq); //VOLTAR DEPOIS
                        }
                        // A value is written to the OPC server item
                        else if (req.Cmd == Command.Write)
                        {
                            //FUNCAO DE PREPARO DA RESPOSTA EN BsonDocument
                            respReq = WriteResponseDocumentPreparation(req);
                            //inserir na fila de resposta
                            Responses.Add(respReq);
                        }

                        Thread.Sleep(sleepTimeOPCThr);//retirar depois
                    }
                }//while(1)
            /*}
            catch (Exception ex)
            {
                //Chmar método de parada desta thread
                StopOpcAccessThread();//verificar se não é uma operacao invalida
                appendToLog(DateTime.Now.ToString() + ": Error: " + ex.Message);
                //Log
                appendToLog(DateTime.Now.ToString() + ": Message: " + ex.Message);
                appendToLog(DateTime.Now.ToString() + ": Data: " + ex.Data);
                appendToLog(DateTime.Now.ToString() + ": StackTrace: " + ex.StackTrace);
                //exportStatusLog();
                
                throw ex;
            }*/
        }
        //Criar método de parada segura da thread
        public void StopOpcAccessThread()
        {
            // terminate the server access thread
            OPCThreadRunning = false;
            if(thrOPCAccess != null){
                while(thrOPCAccess.IsAlive)
                {
                    appendToLog(DateTime.Now.ToString() + ": Thread " + thrOPCAccess.Name + " still alive");
                }
            }
            thrOPCAccess = null;
            thrOPCAccess = new Thread(new ThreadStart(OpcAccessThread));
            thrOPCAccess.Name = "OpcAccessThread";
            appendToLog(DateTime.Now.ToString() + ": thread " + thrOPCAccess.Name + " stopped");
        }
        #endregion
        //Thread de resposta ao DB
        #region ThrResponse
        private void RefreshMongoDB_start()
        {
            try
            {
                
                //----------------------- VERIFICAR RACE CONDITION ------------------
                String newclientMongoURL = clientMongoURL;
                String newclientDataBaseString = clientDataBaseString;
                //----------------------- VERIFICAR RACE CONDITION ------------------
                MongoClient clientRefresh = new MongoClient(newclientMongoURL);
                IMongoDatabase databaseRefresh = clientRefresh.GetDatabase(newclientDataBaseString);

                //Collections FALTA PERIODICA ????????????
                var collectionRead = databaseRefresh.GetCollection<BsonDocument>("readRequests");
                var collectionWrite = databaseRefresh.GetCollection<BsonDocument>("writeRequests");
                //var collectionDevices = databaseRefresh.GetCollection<BsonDocument>("devicesAvailable");

                while (mongoRefreshRunning) 
                {
                    if (Responses.Count() > 0)
                    {
                        appendToLog(DateTime.Now.ToString() + ": Number of Responses in queue: " + Responses.Count());
                        
                        //Retirar da fila Responses e escrever no DB
                        OpcRequest docToRefresh = Responses.Remove();
                        queueRespoLengthRefresh(Responses.Count());//atualizar tamanho da fila de respostas

                        //escrever na collection read
                        if (docToRefresh.Cmd == Command.Read)
                        {
                            //nao tem id, vem das leituras periodicas
                            if (!docToRefresh.requestDocument.Contains("_id"))
                            {
                                //Console.WriteLine(DateTime.Now.ToString() + ": doc sem _id, vem de leitura periodica");
                                collectionRead.InsertOne(docToRefresh.requestDocument);
                            }
                            else //request pedida manualmente pelo usuario
                            {
                                var docId = docToRefresh.requestDocument.GetValue("_id");
                                var filterId = Builders<BsonDocument>.Filter.Eq("_id", docId);
                                //substituicao do documento de mesmo iD
                                collectionRead.ReplaceOne(filterId, docToRefresh.requestDocument);
                            }
                        }
                        //escrever na collection write
                        else if (docToRefresh.Cmd == Command.Write)
                        {
                            //nao tem id, vem das leituras periodicas
                            if (!docToRefresh.requestDocument.Contains("_id"))
                            {
                                //Console.WriteLine(DateTime.Now.ToString() + ": doc sem _id, vem de leitura periodica");
                                collectionWrite.InsertOne(docToRefresh.requestDocument);
                            }
                            else //request pedida manualmente pelo usuario
                            {
                                var docId = docToRefresh.requestDocument.GetValue("_id");
                                var filterId = Builders<BsonDocument>.Filter.Eq("_id", docId);
                                //substituicao do documento de mesmo iD
                                collectionWrite.ReplaceOne(filterId, docToRefresh.requestDocument);
                            }
                        }
                        Thread.Sleep(sleepTimeResponseThr);//retirar depois
                    }
                }//while(1)
            }
            catch (Exception ex)
            {
                //Chmar método de parada desta thread
                StopRefreshMongoDB();//verificar se não é uma operacao invalida
                appendToLog(DateTime.Now.ToString() + ": Error: " + ex.Message);
                //log
                appendToLog(DateTime.Now.ToString() + ": " + ex.Message + " " + ex.Data);
                //exportStatusLog();
                throw ex;
            }
        }
        //Criar método de parada segura da thread
        public void StopRefreshMongoDB()
        {
            // terminate the server response thread
            mongoRefreshRunning = false;
            if (thrMongoResponse != null)
            {
                while (thrMongoResponse.IsAlive)
                {
                    appendToLog(DateTime.Now.ToString() + ": Thread " + thrMongoResponse.Name + " still alive");
                }
            }
            thrMongoResponse = null;
            thrMongoResponse = new Thread(new ThreadStart(RefreshMongoDB_start));
            thrMongoResponse.Name = "ResponseThread";
            appendToLog(DateTime.Now.ToString() + ": thread " + thrMongoResponse.Name + " stopped");

        }
        #endregion
        //Thread para pedido de leitura periodica
        #region ThrPeriodic
        //Thread de criação de requests periodicas
        private void PeriodicReadAccessThread()
        {
            try{
                //neste momento é feita uma copia da lista de devicesID deste server, por enquanto somente um deviceID
                //----------------------- VERIFICAR RACE CONDITION ------------------
                List<String> newListdevicesID = ListServerDevicesID;//ATENTAR PARA RACECONDITION
                String serverN = serverName;
                //----------------------- VERIFICAR RACE CONDITION ------------------
                String newclientMongoURL = clientMongoURL;
                String newclientDataBaseString = clientDataBaseString;
                //----------------------- VERIFICAR RACE CONDITION ------------------

                MongoClient clientPolling = new MongoClient(newclientMongoURL);
                IMongoDatabase databasePolling = clientPolling.GetDatabase(newclientDataBaseString);
                
                //collections envolvidas na leitura periodica
                //var collectionperiodicRead = databasePolling.GetCollection<BsonDocument>("periodicReadRequests");
                var collectionserversOPC = databasePolling.GetCollection<BsonDocument>("serversOPC");
                //listas para as requests
                //List<BsonDocument> periodicReadDocuments = new List<BsonDocument>();
                List<BsonDocument> serversOPCDocuments = new List<BsonDocument>();

                //filtros simples
                var filterChecked = Builders<BsonDocument>.Filter.Eq("checked", false);//Procurando por requests com tag falsa
                var filterServerName = Builders<BsonDocument>.Filter.Eq("serverName", serverN);
                //var filterStart = Builders<BsonDocument>.Filter.Eq("command", "start");//filtro para comando START
                //var filterStop = Builders<BsonDocument>.Filter.Eq("command", "stop");//filtro para comando STOP

                var filterStarted = Builders<BsonDocument>.Filter.Eq("command", "started");//filtro para comando START
                var filterStopped = Builders<BsonDocument>.Filter.Eq("command", "stopped");//filtro para comando STOP
                //filtros combinados
                //filtro checked com serverName
                var filterCombCheckedServerName = filterChecked & filterServerName;
                //filtro para comando START combinado com checked e serverName
                //var filterCombStart = filterStart & filterServerName & filterChecked;
                //var filterCombStop = filterStop & filterServerName & filterChecked;

                var filterCombStarted = filterStarted & filterServerName & filterChecked;
                var filterCombStopped = filterStopped & filterServerName & filterChecked;

                //notificar no documento do server atual que a leitura está acontecendo
                serversOPCDocuments = collectionserversOPC.FindSync(filterServerName).ToList<BsonDocument>();
                if (serversOPCDocuments.Count == 1) //documento é único
                {
                    //como o if garante que 'e somente um elemento, posso fazer o foreach abaixo
                    BsonDocument startDoc = new BsonDocument();
                    foreach (BsonDocument doc in serversOPCDocuments) { startDoc = doc; }
                    var docId = startDoc.GetValue("_id");
                    var filterId = Builders<BsonDocument>.Filter.Eq("_id", docId);
                    startDoc.Set("checked", true);
                    startDoc.Set("command", "started");
                    collectionserversOPC.ReplaceOne(filterId, startDoc);
                    
                }
                else
                {
                    appendToLog(DateTime.Now.ToString() + ": " + serverN + " doc not found");
                }
                //Criar uma request para cada dispositivo e colocar na fila
                while (periodicReadThreadRunning)//a flag é usada para o usuário controlar. Depois fazer um AND com uma variável do tempo de leitura prévio e/ou tempo máx
                {
                    foreach (String devID in newListdevicesID)
                    {
                        Dictionary<String, Object> newDict = new Dictionary<String, object>();
                        newDict.Add("requestT", devID);//adicionando o ID do dispositivo
                        newDict.Add("serverName", serverN);
                        newDict.Add("createdAt", DateTime.Now);//Verificar se tem fuso
                        newDict.Add("responseTime", 0);
                        newDict.Add("checked", false);
                        BsonDocument newDoc = new BsonDocument(newDict);
                        Requests.Add(new OpcRequest(Command.Read, newDoc));
                        queueReqLengthRefresh(Requests.Count());
                    }
                    Thread.Sleep(sleepTimePeriodicThr);//Intervalo de 500ms
                }
            }
            catch (Exception ex)
            {
                //Chmar método de parada desta thread
                StopPeriodicReadAccessThread();//verificar se não é uma operacao invalida
                appendToLog(DateTime.Now.ToString() + ": Error: " + ex.Message);
                //log
                appendToLog(DateTime.Now.ToString() + ": " + ex.Message + " " + ex.Data);
                //exportStatusLog();
                throw ex;
            }
        }
        //Criar método de parada segura da thread
        public void StopPeriodicReadAccessThread()
        {
            // terminar thread de leitura periodica
            periodicReadThreadRunning = false;
            if(thrPeriodicRead != null){
                while (thrPeriodicRead.IsAlive)
                {
                    //appendToLog(DateTime.Now.ToString() + ": Thread " + thrPeriodicRead.Name + " still alive");
                }
            }
            thrPeriodicRead = null;
            thrPeriodicRead = new Thread(new ThreadStart(PeriodicReadAccessThread));
            thrPeriodicRead.Name = "PeriodicReadThread";
            appendToLog(DateTime.Now.ToString() + ": thread " + thrPeriodicRead.Name + " stopped");
            
            //notificar no documento do server atual que a leitura foi parada
            String serverN = serverName;
            String newclientMongoURL = clientMongoURL;
            String newclientDataBaseString = clientDataBaseString;
            //----------------------- VERIFICAR RACE CONDITION ------------------

            MongoClient clientPolling = new MongoClient(newclientMongoURL);
            IMongoDatabase databasePolling = clientPolling.GetDatabase(newclientDataBaseString);

            //collections envolvidas na leitura periodica
            //var collectionperiodicRead = databasePolling.GetCollection<BsonDocument>("periodicReadRequests");
            var collectionserversOPC = databasePolling.GetCollection<BsonDocument>("serversOPC");
            //listas para as requests
            //List<BsonDocument> periodicReadDocuments = new List<BsonDocument>();
            List<BsonDocument> serversOPCDocuments = new List<BsonDocument>();

            //filtros simples
            var filterChecked = Builders<BsonDocument>.Filter.Eq("checked", false);//Procurando por requests com tag falsa
            var filterServerName = Builders<BsonDocument>.Filter.Eq("serverName", serverN);
            //var filterStart = Builders<BsonDocument>.Filter.Eq("command", "start");//filtro para comando START
            //var filterStop = Builders<BsonDocument>.Filter.Eq("command", "stop");//filtro para comando STOP

            var filterStarted = Builders<BsonDocument>.Filter.Eq("command", "started");//filtro para comando START
            var filterStopped = Builders<BsonDocument>.Filter.Eq("command", "stopped");//filtro para comando STOP
            //filtros combinados
            //filtro checked com serverName
            var filterCombCheckedServerName = filterChecked & filterServerName;
            //filtro para comando START combinado com checked e serverName
            //var filterCombStart = filterStart & filterServerName & filterChecked;
            //var filterCombStop = filterStop & filterServerName & filterChecked;

            var filterCombStarted = filterStarted & filterServerName & filterChecked;
            var filterCombStopped = filterStopped & filterServerName & filterChecked;
            serversOPCDocuments = collectionserversOPC.FindSync(filterServerName).ToList<BsonDocument>();
            if (serversOPCDocuments.Count == 1) //documento é único
            {
                //como o if garante que 'e somente um elemento, posso fazer o foreach abaixo
                BsonDocument startDoc = new BsonDocument();
                foreach (BsonDocument doc in serversOPCDocuments) { startDoc = doc; }
                var docId = startDoc.GetValue("_id");
                var filterId = Builders<BsonDocument>.Filter.Eq("_id", docId);
                startDoc.Set("checked", true);
                startDoc.Set("command", "stopped");
                collectionserversOPC.ReplaceOne(filterId, startDoc);

            }
            else
            {
                appendToLog(DateTime.Now.ToString() + ": " + serverN + " doc not found");
            }
        }
        #endregion
        //Thread polling no DB
        #region ThrPolling
        private void PollingMongoDB_start()
        {
            try{
                //----------------------- VERIFICAR RACE CONDITION ------------------
                String newclientMongoURL = clientMongoURL;
                String newclientDataBaseString = clientDataBaseString;
                String serverN = serverName;
                //----------------------- VERIFICAR RACE CONDITION ------------------

                MongoClient clientPolling = new MongoClient(newclientMongoURL);
                IMongoDatabase databasePolling = clientPolling.GetDatabase(newclientDataBaseString);
            
                //Dando polling em todas as collections do DB
                var collectionRead = databasePolling.GetCollection<BsonDocument>("readRequests");
                var collectionWrite = databasePolling.GetCollection<BsonDocument>("writeRequests");
                //var collectionperiodicRead = databasePolling.GetCollection<BsonDocument>("periodicReadRequests");
                var collectionserversOPC = databasePolling.GetCollection<BsonDocument>("serversOPC");
            
                //listas para as requests
                List<BsonDocument> readDocuments = new List<BsonDocument>();//readRequests
                List<BsonDocument> writeDocuments = new List<BsonDocument>();//writeRequests
                //List<BsonDocument> periodicReadDocuments = new List<BsonDocument>();
                List<BsonDocument> serversOPCDocuments = new List<BsonDocument>();

                //filtros simples
                var filterChecked = Builders<BsonDocument>.Filter.Eq("checked", false);//Procurando por requests com tag falsa
                var filterServerName = Builders<BsonDocument>.Filter.Eq("serverName",serverN);
                //var filterStart = Builders<BsonDocument>.Filter.Eq("command", "start");//filtro para comando START
                //var filterStop = Builders<BsonDocument>.Filter.Eq("command", "stop");//filtro para comando STOP

                var filterStarted = Builders<BsonDocument>.Filter.Eq("command", "started");//filtro para comando START
                var filterStopped = Builders<BsonDocument>.Filter.Eq("command", "stopped");//filtro para comando STOP
                //filtros combinados
                    //filtro checked com serverName
                    var filterCombCheckedServerName = filterChecked & filterServerName;
                    //filtro para comando START combinado com checked e serverName
                    //var filterCombStart = filterStart & filterServerName & filterChecked;
                    //var filterCombStop = filterStop & filterServerName & filterChecked;

                    var filterCombStarted = filterStarted & filterServerName & filterChecked;
                    var filterCombStopped = filterStopped & filterServerName & filterChecked;

                while (mongoPollingRunning)
                {
                    //busca no DB para pedidos de leitura
                    readDocuments = collectionRead.FindSync(filterCombCheckedServerName).ToList<BsonDocument>();
                    foreach (BsonDocument readDoc in readDocuments)
                    {
                        //Console.WriteLine(readDoc.GetValue("_id").ToString());
                        //Adicionar na fila de requests a tratar
                        Requests.Add(new OpcRequest(Command.Read, readDoc));
                        queueReqLengthRefresh(Requests.Count());
                    }

                    //busca no DB para pedidos de escrita
                    writeDocuments = collectionWrite.FindSync(filterCombCheckedServerName).ToList<BsonDocument>();
                    foreach (BsonDocument writeDoc in writeDocuments)
                    {
                        //Console.WriteLine(writeDoc.GetValue("_id").ToString());
                        //Adicionar na fila de requests a tratar
                        Requests.Add(new OpcRequest(Command.Write, writeDoc));
                        queueReqLengthRefresh(Requests.Count());
                    }
                    
                        //n~ao conflitar a possibilidade de iniciar a thread periodica com o botao e pela request
                        //a mesma flag est'a sendo usada
                    //busca no DB para pedidos de leitura periódica para o servidor atual
                    serversOPCDocuments = collectionserversOPC.FindSync(filterCombStarted).ToList<BsonDocument>();
                    if (serversOPCDocuments.Count == 1) //o pedido de inicio foi feito(depois sera analisado o de parada)
                    {
                        //como o if garante que 'e somente um elemento, posso fazer o foreach abaixo
                        BsonDocument startDoc = new BsonDocument();
                        foreach (BsonDocument doc in serversOPCDocuments) { startDoc = doc; }
                        var docId = startDoc.GetValue("_id");
                        var filterId = Builders<BsonDocument>.Filter.Eq("_id", docId);

                        appendToLog(DateTime.Now.ToString() + ": START periodic read demanded");
                        //come'car a thread se ela estiver parada, se n~ao so dar checked=true
                        if (!periodicReadThreadRunning)
                        {
                            //------CRIAR DELEGATE PARA PEDIR PARA A MAIN THREAD LANCAR A THREAD--------
                            /*periodicReadThreadRunning = true;//flag do la'co da thread
                            thrPeriodicRead.Start();*/
                            btnStartThreadPeriodic_Click(null, null);
                            //Console.WriteLine(DateTime.Now.ToString() + ": periodic read STARTED, thread name: " + thrPeriodicRead.Name);//cuidado com acesso do objeto fora da thread
                            //----------------- DELEGATE -----------------------

                            appendToLog(DateTime.Now.ToString() + ": periodic read STARTED");
                            //checked=true no document
                            startDoc.Set("checked", true);
                            startDoc.Set("command","started");
                            collectionserversOPC.ReplaceOne(filterId, startDoc);
                        }
                        else
                        {
                            //thread j'a est'a rodando, dar checked=true
                            startDoc.Set("checked", true);
                            startDoc.Set("command", "started");
                            collectionserversOPC.ReplaceOne(filterId, startDoc);
                            appendToLog(DateTime.Now.ToString() + ": periodic thread alredy STARTED");
                        }
                    }
                    else
                    {
                        serversOPCDocuments = collectionserversOPC.FindSync(filterCombStopped).ToList<BsonDocument>();
                        if (serversOPCDocuments.Count == 1)//o pedido de parada foi feito, prioridade sobre o de inicio
                        {
                            //como o if garante que 'e somente um elemento, posso fazer o foreach abaixo
                            BsonDocument stopDoc = new BsonDocument(); ;
                            foreach (BsonDocument doc in serversOPCDocuments) { stopDoc = doc; }
                            var docId = stopDoc.GetValue("_id");
                            var filterId = Builders<BsonDocument>.Filter.Eq("_id", docId);

                            appendToLog(DateTime.Now.ToString() + ": STOP periodic read demanded");
                            //Parar thread se ela estiver rodando, se n~ao so dar checked=true
                            if (periodicReadThreadRunning)
                            {
                                //--CRIAR DELEGATE PARA PEDIR PARA A MAIN THREAD PARAR A THREAD(invocar metodo StopThread)
                                /*periodicReadThreadRunning = false;//flag do la'co da thread
                                thrPeriodicRead = null;
                                thrPeriodicRead = new Thread(new ThreadStart(PeriodicReadAccessThread));
                                thrPeriodicRead.Name = "PeriodicReadThread";*///cuidado ao acessar objeto
                                btnStopThreadPeriodic_Click(null,null);                                           
                                //Pensar em maneira de verificar se a thread terminou, pode ser a flag
                                //Console.WriteLine(DateTime.Now.ToString() + ": periodic read STOPPED, thread name: " + thrPeriodicRead.Name);
                                ////--------DELEGATE----------

                                appendToLog(DateTime.Now.ToString() + ": periodic read STOPPED");
                                //checked=true
                                stopDoc.Set("checked", true);
                                stopDoc.Set("command", "stopped");
                                collectionserversOPC.ReplaceOne(filterId, stopDoc);//substituicao do documento de mesmo iD
                                
                            }
                            else
                            {
                                //o pedido de parada n~ao tem efeito, dar checked=true no doc
                                stopDoc.Set("checked", true);
                                stopDoc.Set("command", "stopped");
                                collectionserversOPC.ReplaceOne(filterId, stopDoc);
                                appendToLog(DateTime.Now.ToString() + ": periodic read already STOPPED");
                            }
                        }
                    }

                    //busca no DB para pedidos de leitura periódica para o servidor atual
                    /*periodicReadDocuments = collectionperiodicRead.FindSync(filterCombStart).ToList<BsonDocument>();
                    if (periodicReadDocuments.Count == 1) //o pedido de inicio foi feito(depois sera analisado o de parada)
                    {
                        //como o if garante que 'e somente um elemento, posso fazer o foreach abaixo
                        BsonDocument periodicDoc = new BsonDocument();
                        foreach (BsonDocument doc in periodicReadDocuments) { periodicDoc = doc; }
                        var docId = periodicDoc.GetValue("_id");
                        var filterId = Builders<BsonDocument>.Filter.Eq("_id", docId);

                        appendToLog(DateTime.Now.ToString() + ": START periodic read demanded");
                        //come'car a thread se ela estiver parada, se n~ao so dar checked=true
                        if (!periodicReadThreadRunning)
                        {
                            //------CRIAR DELEGATE PARA PEDIR PARA A MAIN THREAD LANCAR A THREAD--------
                            //periodicReadThreadRunning = true;//flag do la'co da thread
                            //thrPeriodicRead.Start();
                            btnStartThreadPeriodic_Click(null, null);
                            //Console.WriteLine(DateTime.Now.ToString() + ": periodic read STARTED, thread name: " + thrPeriodicRead.Name);//cuidado com acesso do objeto fora da thread
                            //----------------- DELEGATE -----------------------

                            appendToLog(DateTime.Now.ToString() + ": periodic read STARTED");
                            //checked=true no document
                            periodicDoc.Set("checked", true);
                            collectionperiodicRead.ReplaceOne(filterId, periodicDoc);
                        }
                        else
                        {
                            //thread j'a est'a rodando, dar checked=true
                            periodicDoc.Set("checked", true);
                            collectionperiodicRead.ReplaceOne(filterId, periodicDoc);
                            appendToLog(DateTime.Now.ToString() + ": periodic thread alredy STARTED");
                        }
                    }
                    else
                    {
                        periodicReadDocuments = collectionperiodicRead.FindSync(filterCombStop).ToList<BsonDocument>();
                        if (periodicReadDocuments.Count == 1)//o pedido de parada foi feito, prioridade sobre o de inicio
                        {
                            //como o if garante que 'e somente um elemento, posso fazer o foreach abaixo
                            BsonDocument periodicDoc = new BsonDocument(); ;
                            foreach (BsonDocument doc in periodicReadDocuments) { periodicDoc = doc; }
                            var docId = periodicDoc.GetValue("_id");
                            var filterId = Builders<BsonDocument>.Filter.Eq("_id", docId);

                            appendToLog(DateTime.Now.ToString() + ": STOP periodic read demanded");
                            //Parar thread se ela estiver rodando, se n~ao so dar checked=true
                            if (periodicReadThreadRunning)
                            {
                                //--CRIAR DELEGATE PARA PEDIR PARA A MAIN THREAD PARAR A THREAD(invocar metodo StopThread)
                                //periodicReadThreadRunning = false;//flag do la'co da thread
                                //thrPeriodicRead = null;
                                //thrPeriodicRead = new Thread(new ThreadStart(PeriodicReadAccessThread));
                                //thrPeriodicRead.Name = "PeriodicReadThread";
                                //cuidado ao acessar objeto
                                btnStopThreadPeriodic_Click(null, null);
                                //Pensar em maneira de verificar se a thread terminou, pode ser a flag
                                //Console.WriteLine(DateTime.Now.ToString() + ": periodic read STOPPED, thread name: " + thrPeriodicRead.Name);
                                ////--------DELEGATE----------

                                appendToLog(DateTime.Now.ToString() + ": periodic read STOPPED");
                                //checked=true
                                periodicDoc.Set("checked", true);
                                collectionperiodicRead.ReplaceOne(filterId, periodicDoc);//substituicao do documento de mesmo iD

                            }
                            else
                            {
                                //o pedido de parada n~ao tem efeito, dar checked=true no doc
                                periodicDoc.Set("checked", true);
                                collectionperiodicRead.ReplaceOne(filterId, periodicDoc);
                                appendToLog(DateTime.Now.ToString() + ": periodic read already STOPPED");
                            }
                        }
                    }*/

                    Thread.Sleep(sleepTimePollingThr);//retirar depois ou aumentar o tempo
                }//while(1)
                
            }
            catch (Exception ex)
            {
                appendToLog(DateTime.Now.ToString() + ": Error: " + ex.ToString());//OCORRENDO ERRO DE PERDA DE REFERENCIA
                //log
                appendToLog(DateTime.Now.ToString() + ": " + ex.Message + " " + ex.Data);
                //exportStatusLog();
            }

        }
        //Criar método de parada segura da thread
        public void StopPollingMongoDB()
        {
            // terminar thread do polling
            mongoPollingRunning = false;
            if(thrMongoPolling != null)
            {
                while (thrMongoPolling.IsAlive)
                {
                    appendToLog(DateTime.Now.ToString() + ": Thread " + thrMongoPolling.Name + " still alive");
                }
            }
            thrMongoPolling = null;
            thrMongoPolling = new Thread(new ThreadStart(PollingMongoDB_start));
            thrMongoPolling.Name = "PollingThread";
            appendToLog(DateTime.Now.ToString() + ": thread " + thrMongoPolling.Name + " stopped");
        }
        #endregion
        //--------------------THREADS---------------------------
        //métodos auxiliares
        #region methodesAux
        
        #region requestsResponsePreparation
        //Resposta das requests de leitura
        //LEMBRAR QUE ESTE MÉTODO TEM QUE SER TODO REFEITO
        private OpcRequest ReadResponseDocumentPreparation(OpcRequest respDoc)//MÉTODO USADO PELA Thread OPC
        {
            return respDoc;
            /*
            OpcRequest resp = respDoc;
            String reqText = resp.requestDocument.GetValue("requestT").ToString();
            OPCItemState val = new OPCItemState();
            string s = "";
            if (SioGrp == null) 
            {
                String status = "Server NULL Reference, SioGrp NULL Reference";
                SERVERSTATUS stat;
                if (ServersOPC != null)
                {
                    ServersOPC.GetStatus(out stat);
                    status = "Server status: " + stat.eServerState.ToString() +", SioGrp NULL Reference";
                }   //Atualizando arquivo
                    //TEM QUE TERMINAR
                resp.requestDocument.Set("checked", true);
                resp.requestDocument.Set("responseValue", status);
                return resp;
            }

            int rtc = 0;
            try
            {
                rtc = SioGrp.Read(OPCDATASOURCE.OPC_DS_CACHE, reqText, out val);
            }catch(Exception ex)
            {
                s = "Read failed: " + ex.Message;
                appendToLog("Problema na fun;cao de leitura");
                appendToLog(DateTime.Now.ToString() + ": Error Message: " + ex.Message);
                appendToLog(DateTime.Now.ToString() + ": Error StackTrace: " + ex.StackTrace);
                //Atualizando arquivo
                //TEM QUE TERMINAR
                resp.requestDocument.Set("checked", true);
                resp.requestDocument.Set("responseValue", s);
                return resp;
            }
                try
                {
                if (HRESULTS.Failed(rtc))
                {
                    s = "Read failed with error 0x" + rtc.ToString("X");
                    //Atualizando arquivo
                    //TEM QUE TERMINAR
                    resp.requestDocument.Set("checked", true);
                    resp.requestDocument.Set("responseValue", s);
                    return resp;
                }
                else if (HRESULTS.Failed(val.Error))//ESTE TRATAMENTO NÃO ESTA FUNCIONANDO, VERIFICAR TABELAS DE ERROS
                {
                    s = "Read failed with error 0x" + val.Error.ToString("X");
                    //Atualizando arquivo
                    resp.requestDocument.Set("checked", true);
                    resp.requestDocument.Set("responseValue", s);
                    //TEM QUE TERMINAR
                    return resp;
                }
                else
                {
                    s = String.Empty;
                    if (val.DataValue != null)
                    {
                        s = val.DataValue.ToString();
                        Console.WriteLine("Data value: " + s);
                    }
                    else
                    {
                        s = "NULL";
                    }
                    String elementID;
                    BsonValue Bvalue;
                    if (resp.requestDocument.TryGetValue("_id", out Bvalue))
                    {
                        //Console.WriteLine("TEM ID");
                        elementID = Bvalue.ToString();
                    }
                    else
                    {
                        //Console.WriteLine(" NAO TEM ID");
                        elementID = "NULL";
                    }
                    DateTime datenowA = DateTime.Now;
                    DateTime createdAt = resp.requestDocument.GetValue("createdAt").ToUniversalTime();
                    createdAt = createdAt.AddHours(-3);
                    //DateTime createdAt = resp.requestDocument.GetValue("createdAt");
                    //appendToLog(datenowA.ToString() + ": _id: " + elementID + ", CreatedAt: " + resp.requestDocument.GetValue("createdAt").ToString() + ", Value: " + s);
                    appendToLog(datenowA.ToString() + ": Read Request, DeviceID: " + reqText + ", Value: " + s);//CORRETO
                    resp.requestDocument.Add("checkedAt", datenowA);
                    resp.requestDocument.Set("responseTime", (datenowA - createdAt).TotalSeconds);
                    resp.requestDocument.Set("checked", true);
                    resp.requestDocument.Set("responseValue", s);//CORRETO

                    //Salvando Request no listbox
                    appendToRequestsLog(reqText + " " + s + " " + datenowA);
                    //pedir para o form incrementar Read request counter
                    requestsRefresh(true, false);
                    return resp;
                }
                }catch(Exception ex)
                {
                    s = "Read failed: " + ex.Message;
                    appendToLog(DateTime.Now.ToString() + ": Error Message: " + ex.Message);
                    appendToLog(DateTime.Now.ToString() + ": Error StackTrace: " + ex.StackTrace);
                    //Atualizando arquivo
                    //TEM QUE TERMINAR
                    resp.requestDocument.Set("checked", true);
                    resp.requestDocument.Set("responseValue", s);
                    return resp;
                }*/
            
        }

        //Resposta das requests de escrita
        private OpcRequest WriteResponseDocumentPreparation(OpcRequest respDoc)//MÉTODO USADO PELA Thread OPC
        {
            OpcRequest resp = respDoc;
            String reqText = resp.requestDocument.GetValue("requestT").ToString();
            //String reqValueToWrite = respDoc.requestDocument.GetValue("requestValue").ToString();
            object reqValueToWrite = respDoc.requestDocument.GetValue("requestValue").ToString();
            int rtc = SioGrp.Write(reqText, reqValueToWrite);
            if (HRESULTS.Failed(rtc))
            {
                string s = "Write failed with error 0x" + rtc.ToString("X");

                //Atualizando arquivo
                //TEM QUE TERMINAR
                resp.requestDocument.Set("checked", true);
                resp.requestDocument.Set("responseT", s);
                return resp;
            }
            else
            {
                //Atualizando arquivo
                //TEM QUE TERMINAR
                //Colocar:
                /*
                var updateResponseTime = new BsonDocument("$set", new BsonDocument("responseTime", (datenowA - createdAt).TotalSeconds.ToString()));
                var result3 = collectionRead.UpdateOne(filterId, updateResponseTime);
                */
                DateTime datenowA = DateTime.Now.ToUniversalTime();
                DateTime createdAt = resp.requestDocument.GetValue("createdAt").ToUniversalTime();
                //createdAt = createdAt.AddHours(-3);

                String elementID;
                BsonValue Bvalue;
                if (resp.requestDocument.TryGetValue("_id", out Bvalue))
                {
                    //Console.WriteLine("TEM ID");
                    elementID = Bvalue.ToString();
                }
                else
                {
                    //Console.WriteLine(" NAO TEM ID");
                    elementID = "NULL";
                }

                //DateTime createdAt = resp.requestDocument.GetValue("createdAt");
                //appendToLog(datenowA.ToString() + ": _id: " + resp.requestDocument.GetValue("_id").ToString() + ", CreatedAt: " + resp.requestDocument.GetValue("createdAt").ToString());
                appendToLog(DateTime.Now.ToString() + ": Write Request, DeviceID: " + reqText + ", Value: " + reqValueToWrite);
                resp.requestDocument.Add("checkedAt", datenowA);
                resp.requestDocument.Set("responseTime", (datenowA - createdAt).TotalSeconds);
                resp.requestDocument.Set("checked", true);
                resp.requestDocument.Set("responseT", "Write succeed");//enviar mais informacoes da leitura
                //somando mais uma write req
                requestsRefresh(false, true);
                return resp;

            }
        }
        #endregion

        //metodos para notificar DB se o server esta on ou off
        #region notifyDBServerOnOff
        //Server is online: atualizar documento serversOPC para online
        private void serverIsOnline()
        {
            //----------------------- VERIFICAR RACE CONDITION ------------------
            String serverN = serverName;
            String newclientMongoURL = clientMongoURL;
            String newclientDataBaseString = clientDataBaseString;
            //----------------------- VERIFICAR RACE CONDITION ------------------
           
            MongoClient client = new MongoClient(newclientMongoURL);
            IMongoDatabase database = client.GetDatabase(newclientDataBaseString);
            var collectionServersOPC = database.GetCollection<BsonDocument>("serversOPC");
            var filterServerN = Builders<BsonDocument>.Filter.Eq("serverName", serverN);
            //criando document
            Dictionary<String, Object> newDicServer = new Dictionary<String, Object>();
            newDicServer.Add("serverName", serverN);
            newDicServer.Add("connected", true);//online
            newDicServer.Add("checked", true);//periodic flag
            newDicServer.Add("command", "stopped");//periodic command
            DateTime dateOnline = DateTime.Now;
            newDicServer.Add("lastTimeOn", dateOnline);//tmepo de conexão enviado ao servidor
            newDicServer.Add("inMachineNamed", OPC.Common.ComApi.GetComputerName());
            BsonDocument newDocServer = new BsonDocument(newDicServer);
            List<BsonDocument> list = new List<BsonDocument>();
            list = collectionServersOPC.FindSync(filterServerN).ToList<BsonDocument>();
            if (list != null && list.Count == 1)//Doc existe e é único
            {
                foreach (BsonDocument doc in list)
                {
                    var docId = doc.GetValue("_id");
                    //adicionar mesmo id do doc já existente
                    newDocServer.Add("_id", docId);
                    //filtro com mesmo id e substituir
                    var filterId = Builders<BsonDocument>.Filter.Eq("_id", docId);
                    collectionServersOPC.ReplaceOne(filterId, newDocServer);
                }

            }
            else//doc Server não existe, criar novo documento     LEMBRAR QUE PODE TER MAIS DE UM, esse é um problema
            {
                collectionServersOPC.InsertOne(newDocServer);
            }
            appendToLog(dateOnline + ": Server " + serverN + " is online");
        }
        
        //Server is offline: atualizar documento serversOPC para offline
        private void serverIsOffline()
        {
            //----------------------- VERIFICAR RACE CONDITION ------------------
            String serverN = serverName;
            String newclientMongoURL = clientMongoURL;
            String newclientDataBaseString = clientDataBaseString;
            //----------------------- VERIFICAR RACE CONDITION ------------------

            MongoClient client = new MongoClient(newclientMongoURL);
            IMongoDatabase database = client.GetDatabase(newclientDataBaseString);
            var collectionServersOPC = database.GetCollection<BsonDocument>("serversOPC");
            var filterServerN = Builders<BsonDocument>.Filter.Eq("serverName", serverN);
            //criando document
            Dictionary<String, Object> newDicServer = new Dictionary<String, Object>();
            newDicServer.Add("serverName", serverN);
            newDicServer.Add("connected", false);//offline
            newDicServer.Add("checked", true);//periodic flag
            newDicServer.Add("command", "stopped");//periodic command
            DateTime dateOffline = DateTime.Now;
            newDicServer.Add("lastTimeOn", dateOffline);
            newDicServer.Add("inMachineNamed", "offline");
            BsonDocument newDocServer = new BsonDocument(newDicServer);
            List<BsonDocument> list = new List<BsonDocument>();
            list = collectionServersOPC.FindSync(filterServerN).ToList<BsonDocument>();
            if (list != null && list.Count == 1)//Doc existe e é único
            {
                foreach (BsonDocument doc in list)
                {
                    var docId = doc.GetValue("_id");
                    //adicionar mesmo id do doc já existente
                    newDocServer.Add("_id", docId);
                    //filtro com mesmo id e substituir
                    var filterId = Builders<BsonDocument>.Filter.Eq("_id", docId);
                    collectionServersOPC.ReplaceOne(filterId, newDocServer);
                }

            }
            else//doc Server não existe, criar novo documento     LEMBRAR QUE PODE TER MAIS DE UM, esse é um problema
            {
                collectionServersOPC.InsertOne(newDocServer);
            }
            appendToLog(dateOffline + ": Server " + serverName + " is offline");
        }
        #endregion

        //metodos(com auxilio de delegates) para atualizar os labels(tam das filas e N req)
        #region RefreshLabels
        //Metodo para atualizar numero de requests e tamanhos atuais das filas
        private void requestsRefresh(bool addReadReq, bool addWriteReq) //thread OPC
        {
            if(InvokeRequired)
            {
                BeginInvoke(new refreshNrequestsHelper(requestsRefresh),new object[] {addReadReq,addWriteReq});
                return;
            }
            if (addReadReq) //mais uma read requests tratada
            { 
                readRequestsN++;
                NreadReq.Text = readRequestsN.ToString();
            }
            if (addWriteReq) //mais uma write request tratada
            { 
                writeRequestsN++;
                NwriteReq.Text = writeRequestsN.ToString();
            }
        }

        //ATUALIZAR TAMANHO DAS FILAS: Response
        private void queueRespoLengthRefresh(int respoLength) 
        {
            if(InvokeRequired)
            {
                BeginInvoke(new refreshQueueLengthHelper(queueRespoLengthRefresh),new Object[]{respoLength});
                return;
            }
            NqueueRespoLength.Text = respoLength.ToString();
        }

        //ATUALIZAR TAMANHO DAS FILAS: Requests
        private void queueReqLengthRefresh(int reqLength)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new refreshQueueLengthHelper(queueReqLengthRefresh), new Object[] { reqLength });
                return;
            }
            NqueueReqLength.Text = reqLength.ToString();
        }
        #endregion

        //metodos para escrever nos listBoxes(logs) e exportar para os respectivos .txt
        #region WriteExportLogs
        private void createLogFiles()
        {
            //Criação dos arquivos de statusLog e requestsLog
            String dateID = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "--StartedAt-" + DateTime.Now.Hour.ToString() + "h-" + DateTime.Now.Minute.ToString() + "m-" + DateTime.Now.Second.ToString() + "s";
            //Caminho para status log
            pathStatusLog = Directory.GetCurrentDirectory() + "/statusLog-" + dateID + ".txt";
            //Caminho para request log
            pathRequestsLog = Directory.GetCurrentDirectory() + "/requestsLog-" + dateID + ".txt";
            //Caminho para request log csv
            pathRequestsLogCSV = Directory.GetCurrentDirectory() + "/requestsLogCSV-" + dateID + ".csv";
            //starting files
            File.AppendAllText(pathStatusLog, "Status Log, Date: " + DateTime.Now.ToString() + "\n\n\n");//log txt
            File.AppendAllText(pathRequestsLog, DateTime.Now.ToString() + "\n");// requests txt
            File.AppendAllText(pathRequestsLogCSV, "Device ID,Value,Time Stamp\r\n");//requests csv - cabeçalho
            ListRequestsLogCsv = new List<String>();//inicializando a lista que conterá as requests para serem salvas no arquivo csv
        }

        //método para adicionar string no list box de status
        private void appendToLog(String str) //inserir string que será passada
        {
            if (InvokeRequired)//metodo foi chamado por outra thread -- chamar novamente o método usando o delegate
            {
                //Console.WriteLine("Invoke Required");
                BeginInvoke(new statusLogHelper(appendToLog), new object[] { str });
                return;
            }
            //metodo foi chamado pela main thread --- Executar aqui
            //Console.WriteLine("Invoke NOT required");
            Console.WriteLine(str);
            listBoxStaatusLog.Items.Add(str);
            if(checkBoxStatusLog.Checked)
            {
                listBoxStaatusLog.SelectedIndex = listBoxStaatusLog.Items.Count - 1;//Opcao do usuario parar o autoscroll
            }
            //Pode ser que está atrasando muito
            //File.AppendAllText(pathStatusLog, str + "\n");//CUIDADO COM COMPETICAO PELO pathStatusLog
        }
        //método para adicionar string no list box de requests
        private void appendToRequestsLog(String str) //inserir string que será passada
        {
            if (InvokeRequired)//metodo foi chamado por outra thread -- chamar novamente o método usando o delegate
            {
                //Console.WriteLine("Invoke Required");
                BeginInvoke(new statusLogHelper(appendToRequestsLog), new object[] { str });
                return;
            }
            //metodo foi chamado pela main thread --- Executar aqui
            //Console.WriteLine("Invoke NOT required");
            Console.WriteLine(str);
            listBoxRequestsLog.Items.Add(str);//adicionar a lista de requests que é vista(possui demarcadores de tipos float com vírgula(,))
            
            if(checkBoxRequestsLog.Checked)
            {
                listBoxRequestsLog.SelectedIndex = listBoxRequestsLog.Items.Count - 1;//Opcao do usuario parar o autoscroll
            }
            //Pode ser que está atrasando muito
            //File.AppendAllText(pathRequestsLog, str + "\n");//CUIDADO COM COMPETICAO PELO pathRequestsLog
        
        }
        //Exportar log dos eventos para um txt
        private void exportStatusLog() 
        {
            if (InvokeRequired)//metodo foi chamado por outra thread -- chamar novamente o método usando o delegate
            {
                //MessageBox.Show("Invoke exportStatusLog");
                BeginInvoke(new voidDelegateHelper(exportRequestsLog));
                return;
            }
            foreach(String str in listBoxStaatusLog.Items)
            {
                //Console.WriteLine(str);
                File.AppendAllText(pathStatusLog, str + "\n");
            }
            File.AppendAllText(pathStatusLog,"Total of Read requests: " + readRequestsN + "\n");
            File.AppendAllText(pathStatusLog, "Total of Write requests: " + writeRequestsN + "\n");
            appendToLog("StatusLog saved in " + pathStatusLog);
        }
        //Exportar log das requests para um txt(CRIAR METODO)
        private void exportRequestsLog() 
        {
            if (InvokeRequired)//metodo foi chamado por outra thread -- chamar novamente o método usando o delegate
            {
                //MessageBox.Show("Invoke exportRequestsLog");
                BeginInvoke(new voidDelegateHelper(exportRequestsLog));
                return;
            }
            File.AppendAllText(pathRequestsLog, readRequestsN.ToString() + "\n");//total de readRequests
            //Numero de dispositivos lidos
            foreach (String str in listBoxRequestsLog.Items)
            {
                //Console.WriteLine(str);
                File.AppendAllText(pathRequestsLog, str + "\n");
            }
            appendToLog("RequestsLog saved as .txt in " + pathRequestsLog);
        }

        private void exportRequestsLogCsv()
        {
            /*StringBuilder csvContent = new StringBuilder();
            
            // Adding Header Or Column in the First Row of CSV
            csvContent.AppendLine("First Name,Last Name");
            csvContent.AppendLine("Lajapathy,Arun");

            // Save or upload CSV format File (.csv)
            File.AppendAllText(pathRequestsLogCSV, csvContent.ToString());*/

            
            //File.AppendAllText(pathRequestsLogCSV, "SimulatedData.Step,85,13/09/2017 00:49:43\r\n");//TESTE
            //File.AppendAllText(pathRequestsLogCSV, "SimulatedData.Sine,1.22460635382238E-16,13/09/2017 00:49:43\r\n");//TESTE
            
            //ListRequestsLogCsv.Add("SimulatedData.Step,85,13/09/2017 00:49:43\r\n");
            //ListRequestsLogCsv.Add("SimulatedData.Sine,1.22460635382238E-16,13/09/2017 00:49:43\r\n");
            //Numero de dispositivos lidos
            if (InvokeRequired)//metodo foi chamado por outra thread -- chamar novamente o método usando o delegate
            {
                //MessageBox.Show("Invoke exportRequestsLogCSV");
                BeginInvoke(new voidDelegateHelper(exportRequestsLogCsv));
                return;
            }
            foreach (String str in ListRequestsLogCsv)
            {
                //Console.WriteLine(str);
                File.AppendAllText(pathRequestsLogCSV, str);
               
            }
            appendToLog("RequestsLog saved as .csv in " + pathRequestsLogCSV);
        }
        #endregion

        //Criar arvore para mostrar dispositivos
        private void createTree()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new voidDelegateHelper(createTree));
                return;
            }
            treeViewDevices.Nodes.Clear();
            this.Update();
            SERVERSTATUS stat;
            int rtc = ServersOPC.GetStatus(out stat);
            if (HRESULTS.Succeeded(rtc))
                appendToLog(DateTime.Now.ToString() + ": Server Status: " + stat.eServerState.ToString());
            else
            {
                appendToLog(DateTime.Now.ToString() + ": Error " + rtc.ToString() + " at GetStatus.");
                return;
            }

            treeViewDevices.Nodes.Clear();
            ItemTree = new BrowseTree(ServersOPC, treeViewDevices);
            ItemTree.BrowseModeOneLevel = true;
            rtc = ItemTree.CreateTree();		// Browse server from root
            if (HRESULTS.Succeeded(rtc))
            {
                treeViewDevices.ImageList = ItemTree.ImageList;
                treeViewDevices.Nodes.AddRange(ItemTree.Root());
            }
            else
            {
                treeViewDevices.Text = ServersOPC.GetErrorString(rtc, 0);
                return;
            }
            
        }

        private List<String> refreshDevicesList()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new refreshListDelegateHelper(refreshDevicesList));
                List<String> listEmpty = new List<String>();
                return listEmpty;
            }
            //----------------------- VERIFICAR RACE CONDITION ------------------
            String serverN = serverName;
            String newclientMongoURL = clientMongoURL;
            String newclientDataBaseString = clientDataBaseString;
            //----------------------- VERIFICAR RACE CONDITION ------------------

            List<String> newListDevicesID = new List<String>();//lista para retornar os valores
            //Faz conexao com DB só para atualizar a lista de dispositivos
            MongoClient client = new MongoClient(newclientMongoURL);
            IMongoDatabase database = client.GetDatabase(newclientDataBaseString);

            var collectionDevicesAvailable = database.GetCollection<BsonDocument>("devicesAvailable");

            var filterServerName = Builders<BsonDocument>.Filter.Eq("serverName", serverN);
            List<BsonDocument> listDevices = new List<BsonDocument>();
            listDevices = collectionDevicesAvailable.FindSync(filterServerName).ToList<BsonDocument>();

            cbSendMongo.Items.Clear(); // SE FOR COLOCAR NO COMBO BOX
            foreach (BsonDocument doc in listDevices)
            {
                String str = doc.GetValue("deviceID").ToString();
                cbSendMongo.Items.Add(str); // SE FOR COLOCAR NO COMBO BOX
                newListDevicesID.Add(str);
            }
            return newListDevicesID;

        }

        

        #endregion

        

        
    }
}
